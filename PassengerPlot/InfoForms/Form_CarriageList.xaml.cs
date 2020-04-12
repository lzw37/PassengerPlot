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
    /// Form_CarriageList.xaml 的交互逻辑
    /// </summary>
    public partial class Form_CarriageList : Window
    {
        List<VCarriage> OrgCarriageViewList;
        public Form_CarriageList(List<VCarriage> carriageViewList)
        {
            InitializeComponent();

            OrgCarriageViewList = carriageViewList;
            dg_CarriageView.ItemsSource = OrgCarriageViewList;
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (tb_ID.Text.Trim() != "")
            {
                var query = from p in OrgCarriageViewList
                            where p.Entity.LineID.Contains(tb_ID.Text.Trim())
                            select p;
                dg_CarriageView.ItemsSource = query.ToList<VCarriage>();

            }
        }

        private void btn_Visiblize_Click(object sender, RoutedEventArgs e)
        {
            foreach (VCarriage c in dg_CarriageView.ItemsSource)
            {
                c.IsVisible = true;
            }
        }

        private void btn_Invisiblize_Click(object sender, RoutedEventArgs e)
        {
            foreach (VCarriage c in dg_CarriageView.ItemsSource)
            {
                c.IsVisible = false;
            }
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            dg_CarriageView.ItemsSource = OrgCarriageViewList;
        }
    }
}
