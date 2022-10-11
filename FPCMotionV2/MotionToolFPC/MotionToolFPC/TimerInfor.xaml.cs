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
using System.ComponentModel;

namespace MotionToolFPC
{
    /// <summary>
    /// Interaction logic for TimerInfor.xaml
    /// </summary>
    public partial class TimerInfor : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string Name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public Globals globals { get; set; } = Globals.GetInstance();
        public TimerInfor()
        {
            InitializeComponent();
            this.DataContext = this;
            this.OnPropertyChanged();
        }

        private void tbxTimeDwell_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnPropertyChanged();
        }
    }
}
