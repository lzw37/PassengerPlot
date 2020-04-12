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
    /// Form_RealifeDataHandler.xaml 的交互逻辑
    /// </summary>
    public partial class Form_RealifeDataHandler : Window
    {
        public Form_RealifeDataHandler()
        {
            InitializeComponent();
        }

        private event EventHandler<EventArgs> DataHanderlerTerminate;

        private readonly TaskScheduler _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        // background action
        private void RunDataHandler()
        {
            RealLifeDataHandler handler = new PassengerPlot.RealLifeDataHandler();
            handler.Run();

            Task.Factory.StartNew(() =>
                    {
                        DataHanderlerTerminate.Invoke(this, null);
                    },
                    new System.Threading.CancellationTokenSource().Token,
                    TaskCreationOptions.None, _taskScheduler).Wait();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataHanderlerTerminate += new EventHandler<EventArgs>((i_sender, i_e) =>
            {
                this.DialogResult = true;
            });
            Task.Factory.StartNew(RunDataHandler);
        }
    }
}
