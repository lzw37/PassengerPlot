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
    /// Form_StopFacilityList.xaml 的交互逻辑
    /// </summary>
    public partial class Form_StopFacilityList : Window
    {
        List<VStopFacility> OrgStopFacilityViewList;

        public Form_StopFacilityList(List<VStopFacility> stopFacilityViewList)
        {
            InitializeComponent();
            OrgStopFacilityViewList = stopFacilityViewList;
            dg_StopFacilityView.ItemsSource = OrgStopFacilityViewList;
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (tb_ID.Text.Trim() != "")
            {
                var query = from sf in OrgStopFacilityViewList
                            where sf.Entity.Name.Contains(tb_ID.Text.Trim())
                            select sf;
                dg_StopFacilityView.ItemsSource = query.ToList<VStopFacility>();
            }
        }

        private void btn_Visiblize_Click(object sender, RoutedEventArgs e)
        {
            foreach (VStopFacility sf in dg_StopFacilityView.ItemsSource)
            {
                sf.Entity.IsDisplayName = true;
            }
        }

        private void btn_Invisiblize_Click(object sender, RoutedEventArgs e)
        {
            foreach (VStopFacility sf in dg_StopFacilityView.ItemsSource)
            {
                sf.Entity.IsDisplayName = false;
            }
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            dg_StopFacilityView.ItemsSource = OrgStopFacilityViewList;
        }

    }
}
