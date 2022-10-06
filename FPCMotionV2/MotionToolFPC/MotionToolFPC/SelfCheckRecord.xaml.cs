using System;
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
using System.Windows.Shapes;
using System.Timers;
using System.Threading;
using System.ComponentModel;

namespace MotionToolFPC
{
    /// <summary>
    /// Interaction logic for SelfCheckRecord.xaml
    /// </summary>
    public partial class SelfCheckRecord : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string Name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
        private System.Timers.Timer TimerUpdateUI = new System.Timers.Timer(500);
        private ScanPLC PLC = ScanPLC.GetInstance();
        private Globals globals = Globals.GetInstance();
        public bool flag1_0 = false;
        public bool flag1_1 = false;
        public bool flag1_2 = false;
        public bool flag1_3 = false;

        public bool flag2_0 = false;
        public bool flag2_1 = false;
        public bool flag2_2 = false;
        public bool flag2_3 = false;

        public bool flag3_0 = false;
        public bool flag3_1 = false;
        public bool flag3_2 = false;
        public bool flag3_3 = false;

        public bool flag4_0 = false;
        public bool flag4_1 = false;
        public bool flag4_2 = false;
        public bool flag4_3 = false;

        public bool flag5_0 = false;
        public bool flag5_1 = false;

        public bool flag6_0 = false;
        public int Step = 0;
        public int OldStep = -1;
        public string contentRecord { get; set; }
        public int ValueProgress { get; set; }
        public string[] CheckList =
        {
            "Step 1: Checking Motion of X2 Axis \r\n",
            "Step 2: Checking Motion of X1 Axis \r\n",
            "Step 3: Checking Motion of Y Axis\r\n",
            "Step 4: Checking Motion of R Axis\r\n",
            "Step 5: Origin all axis \r\n",
            "Complete Checking hardware of machine!\r\n"
        };
        public SelfCheckRecord()
        {
            InitializeComponent();
            TimerUpdateUI.Elapsed += OntimedEvent;
            TimerUpdateUI.Enabled = true;
            this.DataContext = this;
        }

        private void OntimedEvent(object sender, ElapsedEventArgs e)
        {
            
            TimerUpdateUI.Enabled = false;


            if (OldStep != Step)
            {
                OldStep = Step;
                contentRecord += CheckList[Step];
                OnPropertyChanged();
            }
            // Check Axis X2
            if (Step == 0)
            {
                if (!flag1_0)
                {
                    flag1_0 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 11));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;

                }
                if (globals.OprAndLimit[14] && !flag1_1 && flag1_0)
                {
                    
                    flag1_1 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 11));
                    PLC.IsRead = Mode.Write;
                    Thread.Sleep(1000);
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 11));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;

                }
                if (globals.OprAndLimit[6] && !flag1_2 && flag1_1 && flag1_0)
                {
                    flag1_3 = true;
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 11));
                    Step++;
                    ValueProgress += 5;
                }

            }

            // Check Axis X1
            if (Step == 1)
            {
                if (!flag2_0)
                {
                    flag2_0 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 10));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;

                }
                if (globals.OprAndLimit[13] && !flag2_1 && flag2_0)
                {
                    flag2_1 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 10));
                    PLC.IsRead = Mode.Write;
                    Thread.Sleep(1000);
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 10));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;
                }
                if (globals.OprAndLimit[5] && !flag2_2 && flag2_1 && flag2_0)
                {
                    flag2_2 = false;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 10));
                    PLC.IsRead = Mode.Write;
                    Step++;
                    ValueProgress += 5;
                }

            }


            // Check Axis Y
            if (Step == 2)
            {
                if (!flag3_0)
                {
                    flag3_0 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 12));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;



                }
                if (globals.OprAndLimit[15] && !flag3_1 && flag3_0)
                {
                    flag3_1 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 12));
                    PLC.IsRead = Mode.Write;
                    Thread.Sleep(1000);
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 12));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;


                }
                if (globals.OprAndLimit[7] && !flag3_2 && flag3_1 && flag3_0)
                {
                    flag3_2 = true;
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 12));
                    Step++;
                    ValueProgress += 5;

                }

            }
            // Check Axis R
            if (Step == 3)
            {
                if (!flag4_0)
                {
                    flag4_0 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 9));
                    PLC.dataSends.Add(new dataSend(new int[] { 50 },1016));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;

                }
                if (globals.CurrentPosR > 360000 && !flag4_1 && flag4_0)
                {
                    flag4_1 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 9));
                    PLC.dataSends.Add(new dataSend(new int[] { globals.Parameter[11]}, 1016));
                    PLC.IsRead = Mode.Write;
                    Step++;
                    ValueProgress += 10;

                }
            }
            if (Step == 4 && !flag5_0)
            {
                if (!flag5_0)
                {
                    flag5_0 = true;
                    PLC.dataSends.Add(new dataSend(new int[] { 1 }, 2));
                    PLC.dataSends.Add(new dataSend(new int[] { 1 }, 4));
                    PLC.dataSends.Add(new dataSend(new int[] { 1 }, 1));
                    PLC.IsRead = Mode.Write;
                    ValueProgress += 5;
                }
                else if(globals.D0D499[70] == 1 && globals.D0D499[71] == 1 && globals.D0D499[72] == 1 && globals.D0D499[73] == 1 && flag5_0 && !flag5_1)
                {
                    flag5_1 = true;
                    ValueProgress = 100;
                    Step++;
                }

            }
            if (Step == 5 && !flag6_0)
            {

            }


            TimerUpdateUI.Enabled = true;
        }

        private void btnExitSelfcheck_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(()=>
            {
                TimerUpdateUI.Stop();
                this.Close();
            });
        }
    }
}
