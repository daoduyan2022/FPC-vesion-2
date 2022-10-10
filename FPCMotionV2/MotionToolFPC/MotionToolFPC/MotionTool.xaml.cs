﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Protocol;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace MotionToolFPC
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MotionTool : UserControl, INotifyPropertyChanged
    {
        private System.Timers.Timer TimerUpdateUI = new System.Timers.Timer(200);
        public Globals globals { get; set; } = null;
        public ObservableCollection<Parameter> Parameters { get; set; }
        public ObservableCollection<DataPoint> DataPoints { get; set; }
        public ObservableCollection<Progress> SourceFunction { get; set; }

        public ScanPLC PLC = null;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string Name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
        public SolidColorBrush StatusConnect { get; set; } = Brushes.Red;
        public int _targetPoint { get; set; } = 1;
        public int _offsetX1 { get; set; } = 0;
        public int _offsetX2 { get; set; } = 0;
        public int _offsetY { get; set; } = 0;
        public int _offsetR { get; set; } = 0;
        public int _percentSpeedX1X2 { get; set; } = 100;
        public int _percentSpeedY { get; set; } = 100;
        public string CycleTime { get; set; }
        public int _percentSpeedR { get; set; } = 100;
        public string _sjogmode { get; set; } = "Incremental";
        public JogMode _jogmode { get; set; } = JogMode.Incremental; // JOG mode
        public bool ModeRun { get; set; } = false; // Enable operation in AutoMode
        public bool EnableModify { get; set; } = false; // Enable modify: JOG, Parameter, DataPoint in Manual mode
        public bool EnableSelfCheck { get; set; } = false; // 
        public bool Emergence { get; set; } = false; // Emergence
        public string TextLight1 { get; set; } = " Light OFF";
        public string TextLight2 { get; set; } = " Light OFF";
        public string TextLight3 { get; set; } = " Light OFF";
        public string StServo_X1 { get; set; } = "OFF";
        public string StServo_X2 { get; set; } = "OFF";
        public string StServo_Y { get; set; } = "OFF";
        public string StServo_R { get; set; } = "OFF";
        public string StCurrentStep { get; set; } = "";
        public string StRunMode { get; set; } = "";
        public string StStatusMachine { get; set; } = "";
        public int AutoTargetPoint { get; set; } // Show tartget point in UI
        public SolidColorBrush ColorLight1 { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorLight2 { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorLight3 { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorVisionReady { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorVisionBusy { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorCylinder1Up { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorCylinder1Down { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorCylinder2Up { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorCylinder2Down { get; set; } = Brushes.Gray;
        public SolidColorBrush ColorServo_X1 { get; set; } = Brushes.Red;
        public SolidColorBrush ColorServo_X2 { get; set; } = Brushes.Red;
        public SolidColorBrush ColorServo_Y { get; set; } = Brushes.Red;
        public SolidColorBrush ColorServo_R { get; set; } = Brushes.Red;
        public SolidColorBrush ColorStatus_X1 { get; set; } = Brushes.Yellow;
        public SolidColorBrush ColorStatus_X2 { get; set; } = Brushes.Yellow;
        public SolidColorBrush ColorStatus_Y { get; set; } = Brushes.Yellow;
        public SolidColorBrush ColorStatus_R { get; set; } = Brushes.Yellow;
        public SolidColorBrush ColorRunMode { get; set; } = Brushes.Yellow;
        public SolidColorBrush ColorStatusMachine { get; set; } = Brushes.Yellow;
        public SolidColorBrush ColorRecycle { get; set; } = Brushes.Yellow;
        public bool _statusLight1 { get; set; } = false;
        public bool _statusLight2 { get; set; } = false;
        public bool _statusLight3 { get; set; } = false;

        public Stopwatch stCycleTime = new Stopwatch();// Caculate cycle time of machine

        public bool StatusLight1
        {
            get { return _statusLight1; }
            set
            {
                if (globals.Y0Y17[14])
                {
                    TextLight1 = "Light ON";
                    ColorLight1 = Brushes.Green;
                }
                else
                {
                    TextLight1 = "Light OFF";
                    ColorLight1 = Brushes.Gray;
                }
                _statusLight2 = value;
                OnPropertyChanged();
            }
        }
        public bool StatusLight2
        {
            get { return _statusLight2; }
            set
            {
                if (globals.Y0Y17[13])
                {
                    TextLight2 = "Light ON";
                    ColorLight2 = Brushes.Green;
                }
                else
                {
                    TextLight2 = "Light OFF";
                    ColorLight2 = Brushes.Gray;
                }
                _statusLight2 = value;
                OnPropertyChanged();
            }
        }

        public bool StatusLight3
        {
            get { return _statusLight3; }
            set
            {
                if (globals.Y0Y17[12])
                {
                    TextLight3 = "Light ON";
                    ColorLight3 = Brushes.Green;
                }
                else
                {
                    TextLight3 = "Light OFF";
                    ColorLight3 = Brushes.Gray;
                }
                _statusLight3 = value;
                OnPropertyChanged();
            }
        }

        public int TargetPoint // Target point in verify mode
        {
            get { return _targetPoint; }
            set
            {
                if (value <= DataPoints.Count && value > 0)
                {
                    _targetPoint = value;
                }
                else
                {
                    _targetPoint = 1;
                }
                OnPropertyChanged();
            }
        }
        public int OffsetX1
        {
            get { return _offsetX1; }
            set { _offsetX1 = value; OnPropertyChanged(); }
        }
        public int OffsetX2
        {
            get { return _offsetX2; }
            set { _offsetX2 = value; OnPropertyChanged(); }
        }
        public int OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; OnPropertyChanged(); }
        }
        public int OffsetR
        {
            get { return _offsetR; }
            set { _offsetR = value; OnPropertyChanged(); }
        }
        public int PercentSpeedX1X2
        {
            get { return _percentSpeedX1X2; }
            set { _percentSpeedX1X2 = value; OnPropertyChanged(); }
        }
        public int PercentSpeedY
        {
            get { return _percentSpeedX1X2; }
            set { _percentSpeedX1X2 = value; OnPropertyChanged(); }
        }
        public int PercentSpeedR
        {
            get { return _percentSpeedR; }
            set { _percentSpeedR = value; OnPropertyChanged(); }
        }
        public JogMode ModeJog
        {
            get { return _jogmode; }
            set
            {
                _jogmode = value;
                if (_jogmode == JogMode.Incremental)
                {
                    _sjogmode = "Incremental";
                }
                else
                {
                    _sjogmode = "Continuous";
                }
                OnPropertyChanged();
            }
        }

        // Step Name
        public string[] StepName { get; set; } = new string[20000];
        public void LoadStepName()
        {
            StepName[4] = "Dwell";
            StepName[19041] = "Light Cam 3 On";
            StepName[19040] = "Light Cam 3 Off";
            StepName[19051] = "Light Cam 2 On";
            StepName[19050] = "Light Cam 2 Off";
            StepName[19061] = "Light Cam 1 On";
            StepName[19060] = "Light Cam 1 Off";
            StepName[502] = "Trigger Cam2";
            StepName[503] = "Trigger Cam3";
            StepName[501] = "Trigger Cam1";
            StepName[504] = "Trigger Cam4";
            StepName[505] = "Trigger Cam5";
            StepName[506] = "Complete";
            StepName[500] = "Check Cam Ready?";
            StepName[511] = "Wait Cam1 Capture Finish";
            StepName[512] = "Wait Cam1 Ready";
            StepName[513] = "Wait Cam2 Capture Finish";
            StepName[514] = "Wait Cam2 Ready";
            StepName[515] = "Wait Cam3 Capture Finish";
            StepName[516] = "Wait Cam3 Ready";
            StepName[517] = "Wait Cam4 Capture Finish";
            StepName[518] = "Wait Cam4 Ready";
            StepName[519] = "Wait Cam5 Capture Finish";
            StepName[520] = "Wait Cam5 Ready";
            StepName[531] = "Press Next Point !!!";
            StepName[1030] = "Finish Progress";
            StepName[901] = "Run Point";
            StepName[18827] = "Wait Button Start Signal";
            StepName[0] = "Press Start !!!";
            StepName[19010] = "Check Servo Ready R";
            StepName[19011] = "Check Servo Ready X1";
            StepName[19012] = "Check Servo Ready X2";
            StepName[19013] = "Check Servo Ready Y";
            StepName[19401] = "Run Rotate R";
            StepName[18834] = "Check SS Down Cylinder 1";
            StepName[18836] = "Check SS Down Cylinder 2";
            StepName[18835] = "Check SS Up Cylinder 1";
            StepName[18837] = "Check SS Up Cylinder 2";
            StepName[19070] = "Down Cylinder 1";
            StepName[19071] = "Up Cylinder 1";
            StepName[19200] = "Down Cylinder 2";
            StepName[19201] = "Up Cylinder 2";
            StepName[19069] = "Up Cylinder 1 and 2";
            StepName[19096] = "Down Cylinder 1 and 2";
        }
        public void ShowProgress()
        {
            if (SourceFunction != null) SourceFunction.Clear();
            SourceFunction = new ObservableCollection<Progress>();
            for (int i = 0; i < globals.Function.Length; i++)
            {
                SourceFunction.Add(new Progress(i + 1, StepName[globals.Function[i]]));
            }
        }


        public MotionTool()
        {
            WaittingWindow waittingWindow = new WaittingWindow();
            waittingWindow.ShowDialog();
            globals = Globals.GetInstance();
            PLC = ScanPLC.GetInstance();
            InitParameters();
            InitDataPoints();
            InitializeComponent();
            LoadStepName();
            ShowProgress();
            TimerUpdateUI.Elapsed += OntimedEvent;
            TimerUpdateUI.Enabled = true;
            this.DataContext = this;
            OnPropertyChanged();
        }
        bool stflag = false;
        int CountOld = 0;
        private void OntimedEvent(object sender, ElapsedEventArgs e)
        {
            TimerUpdateUI.Enabled = false;
            if (globals.D0D499[38] != CountOld)
            {
                stflag = true;
                CountOld = globals.D0D499[38];
            }
            if (globals.D0D499[490] == 1 && stflag)
            {
                stCycleTime.Restart();
            }
            if (globals.D0D499[490] == 2)
            {
                stCycleTime.Restart();
                stflag = false;
            }
            CycleTime = stCycleTime.ElapsedMilliseconds.ToString() + " ms";
            object ob = new object();
            lock (ob)
            {
                // Check connect PLC
                if (globals.D0D499[499] == 1)
                {
                    StatusConnect = Brushes.Green;
                    //globals.D0D499[499] = 0;
                }
                else
                {
                    StatusConnect = Brushes.Red;
                    var result = MessageBox.Show("Please check connection of PLC!" + "\r\n" + "Press OK to reconnect", "[Error PLC connection ]", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        WaittingConnectPLC waitConnectPLC = null;
                        this.Dispatcher.Invoke(() =>
                        {
                            waitConnectPLC = new WaittingConnectPLC();
                        });
                        try
                        {
                            PLC.DisconnectPLC();
                            this.Dispatcher.Invoke(() =>
                            {
                                waitConnectPLC.Show();
                                waitConnectPLC.Focus();
                            });


                            PLC.ConnectPLC("192.168.3.3", 9000);
                        }
                        catch
                        {
                        }
                        this.Dispatcher.Invoke(() =>
                        {
                            waitConnectPLC.Close();
                        });
                    }
                }
            }
            //
            if (globals.Y0Y17[14])
            {
                StatusLight1 = true;
            }
            else
            {
                StatusLight1 = false;
            }

            if (globals.Y0Y17[13])
            {
                StatusLight2 = true;
            }
            else
            {
                StatusLight2 = false;
            }

            if (globals.Y0Y17[12])
            {
                StatusLight3 = true;
            }
            else
            {
                StatusLight3 = false;
            }
            // Cylinder
            if (globals.Y0Y17[15])
            {
                ColorCylinder1Up = Brushes.Green;
                ColorCylinder1Down = Brushes.Gray;
            }
            else
            {
                ColorCylinder1Up = Brushes.Gray;
                ColorCylinder1Down = Brushes.Green;
            }

            if (globals.Y20Y37[0])
            {
                ColorCylinder2Up = Brushes.Green;
                ColorCylinder2Down = Brushes.Gray;
            }
            else
            {
                ColorCylinder2Up = Brushes.Gray;
                ColorCylinder2Down = Brushes.Green;
            }
            //Vision
            if (globals.D500D999[0] == 1)
            {
                ColorVisionReady = Brushes.Green;
                ColorVisionBusy = Brushes.Gray;
            }
            else
            {
                ColorVisionReady = Brushes.Gray;
                ColorVisionBusy = Brushes.Red;
            }
            //Servo On
            if (globals.Y0Y17[8])
            {
                ColorServo_R = Brushes.Green;
                StServo_R = "ON";

            }
            else
            {
                ColorServo_R = Brushes.Red;
                StServo_R = "OFF";
            }

            if (globals.Y0Y17[9])
            {
                ColorServo_X1 = Brushes.Green;
                StServo_X1 = "ON";

            }
            else
            {
                ColorServo_X1 = Brushes.Red;
                StServo_X1 = "OFF";

            }

            if (globals.Y0Y17[10])
            {
                ColorServo_X2 = Brushes.Green;
                StServo_X2 = "ON";

            }
            else
            {
                ColorServo_X2 = Brushes.Red;
                StServo_X2 = "OFF";

            }

            if (globals.Y0Y17[11])
            {
                ColorServo_Y = Brushes.Green;
                StServo_Y = "ON";
            }
            else
            {
                ColorServo_Y = Brushes.Red;
                StServo_Y = "OFF";

            }

            // Status Axis

            if (globals.AxisStatus[0])
            {
                ColorStatus_R = Brushes.Green;
            }
            else
            {
                ColorStatus_R = Brushes.Yellow;
            }

            if (globals.AxisStatus[1])
            {
                ColorStatus_X1 = Brushes.Green;
            }
            else
            {
                ColorStatus_X1 = Brushes.Yellow;
            }

            if (globals.AxisStatus[2])
            {
                ColorStatus_X2 = Brushes.Green;
            }
            else
            {
                ColorStatus_X2 = Brushes.Yellow;
            }

            if (globals.AxisStatus[3])
            {
                ColorStatus_Y = Brushes.Green;
            }
            else
            {
                ColorStatus_Y = Brushes.Yellow;
            }

            

            if (globals.D0D499[203] == 1)
            {
                StRunMode = "Auto";
                ColorRunMode = Brushes.Green;
                ModeRun = true;
                EnableModify = false;
            }
            else if (globals.D0D499[203] == 2)
            {
                StRunMode = "Manual";
                ColorRunMode = Brushes.Yellow;
                ModeRun = false;
                EnableModify = true;
            }
            else
            {
                StRunMode = "None";
                ColorRunMode = Brushes.Red;
                ModeRun = false;
                EnableModify = true;
            }
            if (globals.D0D499[35] - 1 >= 0)
            {
                StCurrentStep = "Step" + (globals.D0D499[35]).ToString() + ": " + StepName[globals.Function[globals.D0D499[35] - 1]];
            }
            else
            {
                StCurrentStep = "Idle";
            }

            if (globals.D0D499[81] == 1)
            {
                ColorRecycle = Brushes.Green;
            }
            else
            {
                ColorRecycle = Brushes.Gray;
            }

            if (globals.D0D499[205] == 0 && !globals.X20X37[0])
            {
                StStatusMachine = "Pause";
                ColorStatusMachine = Brushes.Yellow;
                EnableSelfCheck = false;
                Emergence = true;

            }
            if (globals.D0D499[205] == 1 && !globals.X20X37[0])
            {
                StStatusMachine = "Running";
                ColorStatusMachine = Brushes.Green;
                EnableSelfCheck = false;
                Emergence = true;
            }
            if (globals.X20X37[0])
            {
                StStatusMachine = "Emergence";
                ColorStatusMachine = Brushes.Red;
                EnableModify = false;
                EnableSelfCheck = false;
                Emergence = false;
            }

            AutoTargetPoint = (globals.D0D499[45] + 2) / 2 - 1;
            TimerUpdateUI.Enabled = true;

        }

        public void InitParameters()
        {
            if (Parameters != null)
            {
                Parameters.Clear();
            }
            string[] Axis = { "X1", "X2", "Y", "R" };
            Parameters = new ObservableCollection<Parameter>();
            for (int i = 0; i < 4; i++)
            {
                Parameters.Add(new Parameter(Axis[i], globals.Parameter[i], globals.Parameter[i + 4], globals.Parameter[i + 8], globals.Parameter[i + 12], globals.Parameter[i + 16], globals.Parameter[i + 20], globals.Parameter[i + 24]));
            }
        }
        public void InitDataPoints()
        {
            if (DataPoints != null)
            {
                DataPoints.Clear();
            }
            DataPoints = new ObservableCollection<DataPoint>();
            for (int i = 0; i < globals.DataPointX1.Length; i++)
            {
                DataPoints.Add(new DataPoint(i + 1, globals.DataPointX1[i], globals.DataPointX2[i], globals.DataPointY[i], globals.DataPointR[i], globals.DataSpeedX1[i], globals.DataSpeedX2[i], globals.DataSpeedY[i], globals.DataSpeedR[i]));
            }
        }

        private void dgvParameter_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                OnPropertyChanged();
                //int column = e.Column.DisplayIndex;
                int row = e.Row.GetIndex();
                globals.Parameter[row] = Parameters[row].HomeSpeed;
                globals.Parameter[row + 4] = Parameters[row].CreepSpeed;
                globals.Parameter[row + 8] = Parameters[row].JogContinuosSpeed;
                globals.Parameter[row + 12] = Parameters[row].JogIncreSpeed;
                globals.Parameter[row + 16] = Parameters[row].JogIncreSize;
                globals.Parameter[row + 20] = Parameters[row].StratPoint;
                globals.Parameter[row + 24] = Parameters[row].StartPointSpeed;
            }
            catch { }
        }

        private void PauseHome_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[48] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 48));
                PLC.IsRead = Mode.Write;
            }
            else
            {
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 48));
                PLC.IsRead = Mode.Write;
            }

        }

        private void HomeAll_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0 && globals.D0D499[200] == -1 &&
                globals.ServoReadys[1] && globals.ServoReadys[2] && globals.ServoReadys[3] && globals.ServoReadys[0] &&
                globals.Y0Y17[15] && globals.Y20Y37[0] && globals.X20X37[12] && globals.X20X37[14])
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 0));
                PLC.IsRead = Mode.Write;

            }
            else
            {
                MessageBox.Show("Step 1: Check Motion Control Flag is all off \r\nStep 2: Check servo ready signal of X1,X2,Y,R axis\r\nStep3 : Check cylinder 1 and 2 down status", "[Error Operation]");
            }

        }

        private void HomeX1X2_Click(object sender, RoutedEventArgs e)
        {
            if (!globals.AxisStatus[1] && !globals.AxisStatus[2] && globals.D0D499[200] == -1 && globals.ServoReadys[1] && globals.ServoReadys[2])
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 2));
                PLC.IsRead = Mode.Write;
            }
            else
            {
                MessageBox.Show("Step 1: Check Motion Control Flag is all off \r\nStep 2: Check servo ready signal of X1 and X2 axis", "[Error Operation]");

            }
        }

        private void HomeY_Click(object sender, RoutedEventArgs e)
        {
            if (!globals.AxisStatus[3] && globals.D0D499[200] == -1 && globals.ServoReadys[3])
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 4));
                PLC.IsRead = Mode.Write;

            }
            else
            {
                MessageBox.Show("Step 1: Check Motion Control Flag is all off \r\nStep 2: Check servo ready signal Y axis", "[Error Operation]");

            }

        }

        private void HomeR_Click(object sender, RoutedEventArgs e)
        {
            if (!globals.AxisStatus[0] && !globals.Y0Y17[15] && !globals.Y20Y37[0] && globals.X20X37[12] && globals.X20X37[14])
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 1));
                PLC.IsRead = Mode.Write;
            }
            else
            {
                MessageBox.Show("Step 1: Check Motion Control Flag is all off \r\nStep 2: Check servo ready signal Y axis\r\nStep 3: Check cylinder 1 and 2 down status", "[Error Operation]");
            }
        }

        // Offset control

        private void btnExcuteOffsetX1_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < globals.DataPointX1.Length; i++)
            {
                globals.DataPointX1[i] += OffsetX1;
            }
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcuteOffsetX2_Click(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(OffsetX2);
            for (int i = 0; i < globals.DataPointX2.Length; i++)
            {
                globals.DataPointX2[i] += OffsetX2;
            }
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcuteOffsetY_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < globals.DataPointY.Length; i++)
            {
                globals.DataPointY[i] += OffsetY;
            }
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcuteOffsetR_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < globals.DataPointR.Length; i++)
            {
                globals.DataPointY[i] += OffsetR;
            }
            InitDataPoints();
            OnPropertyChanged();
        }
        private void btnExcutePercentSpeedX1_Click(object sender, RoutedEventArgs e)
        {
            this.btnExcutePercentSpeedX2_Click(null, null);
        }
        private void btnExcutePercentSpeedX2_Click(object sender, RoutedEventArgs e)
        {
            double x = (double)PercentSpeedX1X2 / 100;
            for (int i = 0; i < globals.DataSpeedX1.Length; i++)
            {
                globals.DataSpeedX1[i] = (int)(globals.DataSpeedX1[i] * x);
                globals.DataSpeedX2[i] = (int)(globals.DataSpeedX2[i] * x);
            }
            PercentSpeedX1X2 = 100;
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcutePercentSpeedY_Click(object sender, RoutedEventArgs e)
        {
            double x = (double)PercentSpeedR / 100;
            for (int i = 0; i < globals.DataSpeedY.Length; i++)
            {
                globals.DataSpeedY[i] = (int)(globals.DataSpeedY[i] * x);
            }
            PercentSpeedY = 100;
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcutePercentSpeedR_Click(object sender, RoutedEventArgs e)
        {
            double x = (double)PercentSpeedY / 100;
            for (int i = 0; i < globals.DataSpeedR.Length; i++)
            {
                globals.DataSpeedR[i] = (int)(globals.DataSpeedR[i] * x);
            }
            PercentSpeedR = 100;
            InitDataPoints();
            OnPropertyChanged();
        }

        // Verify point
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (TargetPoint > 0)
                TargetPoint--;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (TargetPoint < DataPoints.Count)
                TargetPoint++;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            // Điều kiện chạy : máy ở chế độ manual, các trục ở trạng thái dừng, đang không thực hiện mã lệnh D200.
            if (globals.D0D499[480] == 0 && globals.D0D499[200] == -1 && ModeRun == false)
            {
                int value = TargetPoint * 2 - 2;
                PLC.dataSends.Add(new dataSend(new int[] { value }, 90));
                PLC.dataSends.Add(new dataSend(new int[] { 18008 }, 200));
                PLC.IsRead = Mode.Write;
            }
            else
            {
                MessageBox.Show("Step1: Switch Run mode to Manual Mode \r\n Step2: Check Motion Control Flag is all off \r\n Step3: Press again", "[Error Operation]");
            }
        }
        private void btnLoadRecentData_Click(object sender, RoutedEventArgs e)
        {
            globals.ReadFileModel();
            InitDataPoints();
        }
        private void btnSaveWiteDataPointToPLC_Click(object sender, RoutedEventArgs e)
        {
            globals.SaveFileModel();
            btnWriteModelToPLC_Click(null, null);
        }

        private void txtTargetPoint_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnReadParam_Click(object sender, RoutedEventArgs e)
        {
            globals.ReadFileConfig();
            InitParameters();
            OnPropertyChanged();
        }

        private void btnWriteParam_Click(object sender, RoutedEventArgs e)
        {
            globals.SaveFileConfig();
            List<int> HomeCreepJogCSpeedAndStartPoint = new List<int>
            {
                globals.Parameter[3], globals.Parameter[0], globals.Parameter[1] , globals.Parameter[2], // HomeSpeed X1-X2-Y-R <=> D1000 - D1006
                globals.Parameter[7], globals.Parameter[4], globals.Parameter[5] , globals.Parameter[6], // CreepSpeed X1-X2-Y-R <=> D1008 - D1014
                globals.Parameter[11], globals.Parameter[8], globals.Parameter[9] , globals.Parameter[10]  // JogContinuousSpeed X1-X2-Y-R <=> D1016 - D1022
            };
            List<int> StartPoint = new List<int>
            {
                globals.Parameter[23], globals.Parameter[20], globals.Parameter[21] , globals.Parameter[22],  // StartCoordinate X1-X2-Y-R <=> D1056 - D1062
                globals.Parameter[27], globals.Parameter[24], globals.Parameter[25] , globals.Parameter[26]  // StartSpeed X1-X2-Y-R <=> D1064 - D1070

            };

            PLC.dataSends.Add(new dataSend(globals.IntToRegister(HomeCreepJogCSpeedAndStartPoint), 1000));
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(StartPoint), 1056));
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[19] }), 950));// StepSzie R <=> D950
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[16] }), 960));// StepSzie X1 <=> D960
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[17] }), 970));// StepSzie X2 <=> D970
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[18] }), 980));// StepSzie Y <=> D980

            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[15] }), 958));// JogStepSpeed R <=> D958
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[12] }), 968));// JogStepSpeed X1 <=> D568
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[13] }), 978));// JogStepSpeed X2 <=> D978
            PLC.dataSends.Add(new dataSend(globals.IntToRegister(new List<int> { globals.Parameter[14] }), 988));// SteJogStepSpeedpSzie Y <=> D988
            PLC.IsRead = Mode.Write;
            while (true)
            {
                if (PLC.IsRead == Mode.Read)
                {
                    MessageBox.Show("Write Parameter Done!", "[Successfully]");
                    break;
                }
                if (PLC.IsError)
                {
                    MessageBox.Show("Write Data Fail! \r\n Please check data and connect of PLC \r\n After that write again", "[Send Data Fail]");
                    break;
                }
            }
        }


        // JOG CONTROL
        // X1 --
        private void btnSub_X1_Click(object sender, RoutedEventArgs e)
        {
            //D6
            if (ModeJog == JogMode.Incremental)
            {
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 10 }, 6));
            }
        }
        private void btnSub_X1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D10
                if (PLC.dataSends.Count == 0)
                {
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 10));
                }
            }
        }

        private void btnSub_X1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D10
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 10));
                PLC.IsRead = Mode.Write;

            }
        }


        //R --
        private void btnSub_R_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                //D5
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 10 }, 5));
            }
        }

        private void btnSub_R_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D10
                if (PLC.dataSends.Count == 0)
                {
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 9));
                    PLC.IsRead = Mode.Write;
                }
            }
        }
        private void btnSub_R_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D10
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 9));
                PLC.IsRead = Mode.Write;
            }
        }

        //X2 --

        private void btnSub_X2_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                //D7
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 10 }, 7));
            }
        }
        private void btnSub_X2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D11
                if (PLC.dataSends.Count == 0)
                {
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 11));
                    PLC.IsRead = Mode.Write;
                }
            }
        }

        private void btnSub_X2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D8
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 11));
                PLC.IsRead = Mode.Write;
            }
        }


        // Y ++
        private void btnAdd_Y_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                //D8
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 6 }, 8));
            }
        }
        private void btnAdd_Y_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                if (PLC.dataSends.Count == 0)
                {
                    //D12
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 12));
                }
            }
        }

        private void btnAdd_Y_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D12
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 12));
                PLC.IsRead = Mode.Write;
            }
        }

        // Pause JOG
        private void btnPauseJog_Click(object sender, RoutedEventArgs e)
        {
            //D5 --> D12
            PLC.IsRead = Mode.Write;
            PLC.dataSends.Add(new dataSend(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 5));
        }
        private void btnPauseJog_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void btnPauseJog_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        // Y--
        private void btnSub_Y_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                //D8
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 10 }, 8));
            }
        }
        private void btnSub_Y_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                if (PLC.dataSends.Count == 0)
                {
                    //D12
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 12));
                }
            }
        }

        private void btnSub_Y_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D12
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 12));
                PLC.IsRead = Mode.Write;
            }
        }

        // X1++

        private void btnAdd_X1_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                //D6
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 6 }, 6));
            }
        }
        private void btnAdd_X1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                if (PLC.dataSends.Count == 0)
                {
                    //D10
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 10));
                }
            }
        }

        private void btnAdd_X1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D10
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 10));
                PLC.IsRead = Mode.Write;
            }
        }


        //R ++
        private void btnAdd_R_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                //D5
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 6 }, 5));
            }
        }

        private void btnAdd_R_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                if (PLC.dataSends.Count == 0)
                {
                    //D9
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 9));
                }
            }
        }

        private void btnAdd_R_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D9
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 9));
                PLC.IsRead = Mode.Write;
            }
        }

        // X2 ++
        private void btnAdd_X2_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                //D7
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 6 }, 7));
            }

        }

        private void btnAdd_X2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                if (PLC.dataSends.Count == 0)
                {
                    //D11
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 11));
                }
            }
        }

        private void btnAdd_X2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ModeJog == JogMode.Continuous)
            {
                //D11
                PLC.dataSends.Clear();
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 11));
                PLC.IsRead = Mode.Write;
            }
        }

        //Change Jog mode
        private void btnChangeJogMode_Click(object sender, RoutedEventArgs e)
        {
            if (ModeJog == JogMode.Incremental)
            {
                ModeJog = JogMode.Continuous;
            }
            else
            {
                ModeJog = JogMode.Incremental;
            }
        }

        private void btnLighPowerCam1_Click(object sender, RoutedEventArgs e)
        {
            //Y16
            if (globals.Y0Y17[14])
            {
                // D200 = 19060 Off Lamp3
                PLC.dataSends.Add(new dataSend(new int[] { 19060 }, 200));
            }
            else
            {
                // D200 = 19061 On Lamp3
                PLC.dataSends.Add(new dataSend(new int[] { 19061 }, 200));
            }
            PLC.IsRead = Mode.Write;
        }

        private void btnFinishCaptureCam1_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 511));
            PLC.IsRead = Mode.Write;
        }

        private void btnTriggerCam1_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 501));
            PLC.IsRead = Mode.Write;
        }

        private void btnLighPowerCam2_Click(object sender, RoutedEventArgs e)
        {
            //Y15
            if (globals.Y0Y17[13])
            {
                // D200 = 19050 Off Lamp2
                PLC.dataSends.Add(new dataSend(new int[] { 19050 }, 200));
            }
            else
            {
                // D200 = 19051 On Lamp2
                PLC.dataSends.Add(new dataSend(new int[] { 19051 }, 200));
            }
            PLC.IsRead = Mode.Write;
        }

        private void btnFinishCaptureCam2_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 513));
            PLC.IsRead = Mode.Write;
        }

        private void btnTriggerCam2_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 502));
            PLC.IsRead = Mode.Write;
        }

        private void btnLighPowerCam3_Click(object sender, RoutedEventArgs e)
        {
            //Y14
            if (globals.Y0Y17[12])
            {
                // D200 = 19040 Off Lamp1
                PLC.dataSends.Add(new dataSend(new int[] { 19040 }, 200));
            }
            else
            {
                // D200 = 19041 On Lamp1
                PLC.dataSends.Add(new dataSend(new int[] { 19041 }, 200));
            }
            PLC.IsRead = Mode.Write;

        }

        private void btnFinishCaptureCam3_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 515));
            PLC.IsRead = Mode.Write;
        }

        private void btnTriggerCam3_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 503));
            PLC.IsRead = Mode.Write;
        }

        private void btnVisionPrReady_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 500));
            PLC.IsRead = Mode.Write;
        }

        private void btnVisionPrBusy_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 0 }, 500));
            PLC.IsRead = Mode.Write;
        }

        private void btnCylinder1Up_Click(object sender, RoutedEventArgs e)
        {
            if (!globals.AxisStatus[0])
            {
                PLC.dataSends.Add(new dataSend(new int[] { 19071 }, 200));
                PLC.IsRead = Mode.Write;
            }
            else
            {
                MessageBox.Show("Axis R is moving. Can not up cylinder!", "[Operation error]");
            }
        }

        private void btnCylinder1Down_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 19070 }, 200));
            PLC.IsRead = Mode.Write;
        }

        private void btnCylinder2Up_Click(object sender, RoutedEventArgs e)
        {

            if (!globals.AxisStatus[0])
            {
                PLC.dataSends.Add(new dataSend(new int[] { 19201 }, 200));
                PLC.IsRead = Mode.Write;
            }
            else
            {
                MessageBox.Show("Axis R is moving. Can not up cylinder!", "[Operation error]");
            }
        }

        private void btnCylinder2Down_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 19200 }, 200));
            PLC.IsRead = Mode.Write;
        }

        // Servo on off
        private void btnServoOn_X1_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 1201));
                PLC.IsRead = Mode.Write;
            }

        }

        private void btnServoOff_X1_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 1201));
                PLC.IsRead = Mode.Write;
            }
        }

        private void btnServoOn_X2_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 1202));
                PLC.IsRead = Mode.Write;
            }

        }

        private void btnServoOff_X2_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 1202));
                PLC.IsRead = Mode.Write;
            }
        }

        private void btnServoOn_Y_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 1203));
                PLC.IsRead = Mode.Write;
            }
        }

        private void btnServoOff_Y_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 1203));
                PLC.IsRead = Mode.Write;
            }
        }

        private void btnServoOn_R_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 1200));
                PLC.IsRead = Mode.Write;
            }
        }

        private void btnServoOff_R_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[480] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 1200));
                PLC.IsRead = Mode.Write;
            }
        }

        private void btnWriteGcodeToPLC_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(globals.funtionRunRotate, 1600));
            PLC.IsRead = Mode.Write;
            while (true)
            {
                if (PLC.IsRead == Mode.Read)
                {
                    MessageBox.Show("Write Gcode Done!", "[Successfully]");
                    break;
                }
                if (PLC.IsError)
                {
                    MessageBox.Show("Write Data Fail! \r\n Please check data and connect of PLC \r\n After that write again", "[Send Data Fail]");
                    break;
                }
            }

        }

        private void btnWriteModelToPLC_Click(object sender, RoutedEventArgs e)
        {
            globals.ReadFileModel();
            int[] CoordX1 = globals.IntToRegister(globals.DataPointX1.ToList()); // Coordinate X1 : D2200
            int[] CoordX2 = globals.IntToRegister(globals.DataPointX2.ToList());// Coordinate X2 : D2400
            int[] CoordY = globals.IntToRegister(globals.DataPointY.ToList());// Coordinate Y : D2600
            int[] CoordR = globals.IntToRegister(globals.DataPointR.ToList());// Coordinate R : D2800

            int[] SpeedX1 = globals.IntToRegister(globals.DataSpeedX1.ToList()); // Speed X1 : D3400
            int[] SpeedX2 = globals.IntToRegister(globals.DataSpeedX2.ToList());// Speed X2 : D3600
            int[] SpeedY = globals.IntToRegister(globals.DataSpeedY.ToList());// Speed Y : D3800
            int[] SpeedR = globals.IntToRegister(globals.DataSpeedR.ToList());// Speed X1 : D4000

            PLC.dataSends.Add(new dataSend(CoordX1, 2200));
            PLC.dataSends.Add(new dataSend(CoordX2, 2400));
            PLC.dataSends.Add(new dataSend(CoordY, 2600));
            PLC.dataSends.Add(new dataSend(CoordR, 2800));

            PLC.dataSends.Add(new dataSend(SpeedX1, 3400));
            PLC.dataSends.Add(new dataSend(SpeedX2, 3600));
            PLC.dataSends.Add(new dataSend(SpeedY, 3800));
            PLC.dataSends.Add(new dataSend(SpeedR, 4000));

            PLC.dataSends.Add(new dataSend(globals.TimeDelay, 4200));// Time Delay after complete Run Point: D4200
            PLC.IsRead = Mode.Write;

            while (true)
            {
                if (PLC.IsRead == Mode.Read)
                {
                    MessageBox.Show("Write Data Point And Speed Done!", "[Successfully]");
                    break;
                }
                if (PLC.IsError)
                {
                    MessageBox.Show("Write Data Fail! \r\n Please check data and connect of PLC \r\n After that write again", "[Send Data Fail]");
                    break;
                }
            }

        }

        private void btnStartMachine_Click(object sender, RoutedEventArgs e)
        {
            //D205 = 1
            PLC.IsRead = Mode.Write;
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 205));
        }

        private void btnStopMachine_Click(object sender, RoutedEventArgs e)
        {
            //D205 = 0
            PLC.IsRead = Mode.Write;
            PLC.dataSends.Add(new dataSend(new int[] { 0 }, 205));
        }

        private void btnResetMachine_Click(object sender, RoutedEventArgs e)
        {
            //D1030
            PLC.IsRead = Mode.Write;
            PLC.dataSends.Add(new dataSend(new int[] { 1030 }, 200));
        }

        private void btnManualModeNextStep_Click(object sender, RoutedEventArgs e)
        {
            //D204
            PLC.IsRead = Mode.Write;
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 204));
        }

        private void btnSwitchMode_Click(object sender, RoutedEventArgs e)
        {
            //D203
            if (globals.D0D499[203] == 2)
            {
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 203));
            }
            else
            {
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 2 }, 203));
            }

        }

        private void btnPointToPointMode_Click(object sender, RoutedEventArgs e)
        {
            //D530
            if (globals.D500D999[30] == 1)
            {
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 0, 0 }, 530));
            }
            else
            {
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 530));
            }
        }

        private void btnRecycle_Click(object sender, RoutedEventArgs e)
        {
            //D81
            if (globals.D0D499[81] == 1)
            {
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 81));
            }
            else
            {
                PLC.IsRead = Mode.Write;
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 81));
            }
        }

        private void M126_Click(object sender, RoutedEventArgs e)
        {
            //Check D49.9
            if (globals.RunInstrucTion[9])
            {
                var result = MessageBox.Show("Stop Run instruction axis X1 ?");
                if (result == MessageBoxResult.OK)
                {

                }
            }
        }

        private void btnSelfCheck_Click(object sender, RoutedEventArgs e)
        {
            if(globals.OprAndLimit[0] && globals.OprAndLimit[1] && globals.OprAndLimit[2] && globals.OprAndLimit[3])
            {
                var selfCheck = new SelfCheckRecord();
                selfCheck.ShowDialog();
            }
        }

        private void lvwProgress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void M181_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 181));
            PLC.IsRead = Mode.Write;
        }

        private void M182_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 182));
            PLC.IsRead = Mode.Write;
        }

        private void M183_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 183));
            PLC.IsRead = Mode.Write;
        }

        private void M180_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 180));
            PLC.IsRead = Mode.Write;
        }

        private void M184_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 184));
            PLC.IsRead = Mode.Write;
        }


        Point _lastMouseDown;
        TreeViewItem draggedItem, _target;

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.AllowedEffects;
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            
        }

        

        private void tvwCommand_Drop(object sender, DragEventArgs e)
        {
            
        }

        private void tvwCommand_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void tvwCommand_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(tvwCommand);
            }
        }

        private void tvwCommand_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(tvwCommand);
                draggedItem = (TreeViewItem)tvwCommand.SelectedItem;
                if (draggedItem != null)
                {
                    DragDropEffects finalDropEffect = DragDrop.DoDragDrop(tvwCommand, tvwCommand.SelectedValue,
                                DragDropEffects.Move);
                }
            }
        }
    }
}
