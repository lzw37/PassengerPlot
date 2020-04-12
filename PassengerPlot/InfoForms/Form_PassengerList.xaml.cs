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
    /// Form_PassengerList.xaml 的交互逻辑
    /// </summary>
    public partial class Form_PassengerList : Window
    {
        List<VPassenger> OrgPassengerViewList;
        public Form_PassengerList(List<VPassenger> passengerViewList)
        {
            InitializeComponent();

            OrgPassengerViewList = passengerViewList;
            dg_PassengerView.ItemsSource = OrgPassengerViewList;
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (tb_ID.Text.Trim() != "")
            {
                var query = from p in OrgPassengerViewList
                            where p.Entity.ID.Contains(tb_ID.Text.Trim())
                            select p;
                dg_PassengerView.ItemsSource = query;

            }
            if (tb_Name.Text.Trim() != "")
            {
                var query = from p in OrgPassengerViewList
                            where p.Entity.Name.Contains(tb_Name.Text.Trim())
                            select p;
                dg_PassengerView.ItemsSource = query.ToList<VPassenger>();
            }
        }

        private void btn_Visiblize_Click(object sender, RoutedEventArgs e)
        {
            foreach (VPassenger p in dg_PassengerView.ItemsSource)
            {
                p.IsVisible = true;
            }
        }

        private void btn_Invisiblize_Click(object sender, RoutedEventArgs e)
        {
            foreach (VPassenger p in dg_PassengerView.ItemsSource)
            {
                p.IsVisible = false;
            }
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            dg_PassengerView.ItemsSource = OrgPassengerViewList;
        }
    }
}
