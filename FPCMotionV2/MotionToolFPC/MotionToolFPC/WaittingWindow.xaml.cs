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
using System.Threading;
using System.IO;

namespace MotionToolFPC
{
    /// <summary>
    /// Interaction logic for WaittingWindow.xaml
    /// </summary>
    public partial class WaittingWindow : Window
    {
        string[] order = new string[] { "Load File Config", "Load File Function", "Load File Model", "Connect to PLC", "Ending" };
        bool Kill = false;
        Globals globals = Globals.GetInstance();
        public WaittingWindow()
        {
            InitializeComponent();
            
        }
        private void OntimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Kill)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            }
        }
        public void Init()
        {
            for(int i = 0; i < order.Length; i++)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lbStatus.Content = order[i];
                });
                if (i == 0)
                {
                    globals.ReadFileConfig();
                }
                if (i == 1)
                {
                    globals.ReadFileModel();
                }
                if (i == 2)
                {
                    globals.ReadFileFunction();
                }
                if (i == 3)
                {
                    Thread PLCScaner = new Thread(() => {
                        ScanPLC plc = ScanPLC.GetInstance();
                        plc.ConnectPLC("192.168.3.3", 9000);
                        plc.StartScan();
                    });
                    PLCScaner.IsBackground = true;
                    PLCScaner.Start();
                }
                if (i == 4)
                {
                    Thread.Sleep(2000);
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread init = new Thread(new ThreadStart(Init));
            init.IsBackground = true;
            init.Start();
        }
        
    }
}
