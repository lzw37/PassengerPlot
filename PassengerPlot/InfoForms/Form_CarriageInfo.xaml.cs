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
    /// Form_CarriageInfo.xaml 的交互逻辑
    /// </summary>
    public partial class Form_CarriageInfo : Window
    {
        public Form_CarriageInfo(Carriage carriage)
        {
            InitializeComponent();
            this.DataContext = carriage;
            dg_event.ItemsSource = carriage.EventList;
        }
    }
}
