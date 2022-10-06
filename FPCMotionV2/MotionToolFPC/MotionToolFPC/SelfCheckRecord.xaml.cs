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

namespace MotionToolFPC
{
    /// <summary>
    /// Interaction logic for SelfCheckRecord.xaml
    /// </summary>
    public partial class SelfCheckRecord : Window
    {
        private System.Timers.Timer TimerUpdateUI = new System.Timers.Timer(500);
        private ScanPLC PLC = ScanPLC.GetInstance();
        private Globals globals = Globals.GetInstance();
        public bool flag1 = false;
        public bool flag2 = false;
        public bool flag3 = false;
        public bool flag4 = false;
        public bool flag5 = false;
        public bool flag6 = false;
        public int Step = 0;
        public string contentRecord { get; set; } = "RECORD SELF CHECK: \r\n";
        public string[] CheckList =
        {
            "Step 1: Checking Motion of X2 Axis",
            "Step 2: Checking Motion of X1 Axis",
            "Step 3: Checking Motion of Y Axis",
            "Step 4: Checking Motion of R Axis",
            "Step 5: Checking Cylinder 1",
            "Step 6: Checking Cylinder 2",
            "Complete Checking hardware of machine!"
        };
        public SelfCheckRecord()
        {
            InitializeComponent();
            TimerUpdateUI.Elapsed += OntimedEvent;
            TimerUpdateUI.Enabled = true;
        }

        private void OntimedEvent(object sender, ElapsedEventArgs e)
        {
            TimerUpdateUI.Enabled = false;
            this.Dispatcher.Invoke(() =>
            {
                lblRecordSelfTest.Content += CheckList[Step];
            });

            if (Step == 0)
            {
                if (!flag1)
                {
                    flag1 = true;
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 11));
                    
                }
                if (globals.OprAndLimit[14])
                {
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 11));
                }
                if (globals.OprAndLimit[6])
                {
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 11));
                    Step++;
                }

            }
            if(Step == 1)
            {
                if (!flag2)
                {
                    flag2 = true;
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 10));
                }
                if (globals.OprAndLimit[13])
                {
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 10));
                }
                if (globals.OprAndLimit[5])
                {
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 10));
                    Step++;
                }

            }

            if (Step == 2)
            {
                if (!flag3)
                {
                    flag3 = true;
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 10 }, 12));
                }
                if (globals.OprAndLimit[14])
                {
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 12));
                }
                if (globals.OprAndLimit[7])
                {
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 0 }, 12));
                    Step++;
                }

            }

            if (Step == 3)
            {
                if (!flag4)
                {
                    flag4 = true;
                    PLC.IsRead = Mode.Write;
                    PLC.dataSends.Add(new dataSend(new int[] { 360000 }, 950));
                    PLC.dataSends.Add(new dataSend(new int[] { 6 }, 5));
                }
            }
            if (Step == 4)
            {

            }


                TimerUpdateUI.Enabled = false;
        }
    }
}
