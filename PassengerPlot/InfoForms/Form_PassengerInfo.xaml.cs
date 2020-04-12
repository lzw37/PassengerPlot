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

namespace PassengerPlot
{
    /// <summary>
    /// Form_PassengerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class Form_PassengerInfo : Window
    {
        public Form_PassengerInfo(VPassenger vPassenger)
        {
            InitializeComponent();
            this.DataContext = vPassenger;
            dg_event.ItemsSource = vPassenger.Entity.EventList;
            dg_event.Focus();
        }
    }
}
