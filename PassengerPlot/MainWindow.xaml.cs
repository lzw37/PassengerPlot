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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.ComponentModel;

namespace PassengerPlot
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            Binding binding = new Binding("SimulationClock") { Source = passengerPlotCanva.Transformation };
            simulationClock.SetBinding(TextBlock.TextProperty, binding);

            Binding simulationTimeBinding = new Binding("SimulationTime") { Source = passengerPlotCanva.Transformation };
            simulationTime.SetBinding(TextBlock.TextProperty, simulationTimeBinding);

            Binding mouseLocationBinding = new Binding("MouseLocation") { Source = passengerPlotCanva.Transformation };
            mouseLocation.SetBinding(TextBlock.TextProperty, mouseLocationBinding);

            Binding mouseAccompanyInfoBinding = new Binding("MouseAccompanyInfo") { Source = passengerPlotCanva.Transformation };
            mouseAccompanyInfo.SetBinding(TextBlock.TextProperty, mouseAccompanyInfoBinding);

            Binding orgXBinding = new Binding("OrgX") { Source = passengerPlotCanva.Transformation };
            tb_orgX.SetBinding(TextBox.TextProperty, orgXBinding);

            Binding orgYBinding = new Binding("OrgY") { Source = passengerPlotCanva.Transformation };
            tb_orgY.SetBinding(TextBox.TextProperty, orgYBinding);

            Binding zoomXBinding = new Binding("ZoomX") { Source = passengerPlotCanva.Transformation };
            tb_zoomX.SetBinding(TextBox.TextProperty, zoomXBinding);

            Binding zoomYBinding = new Binding("ZoomY") { Source = passengerPlotCanva.Transformation };
            tb_zoomY.SetBinding(TextBox.TextProperty, zoomYBinding);

            Binding stepsizeBinding = new Binding("TickStepSize") { Source = passengerPlotCanva.Transformation };
            tb_stepsize.SetBinding(TextBox.TextProperty, stepsizeBinding);

            passengerPlotCanva.Transformation.PropertyChanged += Transformation_PropertyChanged;
        }

        private void Transformation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HitCarriage" || e.PropertyName == "HitPassenger")
            {
                mouseAccompanyInfoBorder.Margin = new Thickness(passengerPlotCanva.Transformation.MouseLocation.X + 5, passengerPlotCanva.Transformation.MouseLocation.Y, 0, 0);
            }
        }

        public bool isBackgroundWorking = false;

        public bool IsBackgroundWorking
        {
            get
            {
                return isBackgroundWorking;
            }
            set
            {
                isBackgroundWorking = value;
                if(value == true)
                {
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsBackgroundWorking"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonsVisibility"));
            }
        }

        public Visibility ButtonsVisibility
        {
            get
            {
                if (IsBackgroundWorking)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
        }

        /// <summary>
        /// Next tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            passengerPlotCanva.Transformation.StepForward();
            passengerPlotCanva.CalculatePlot();
            passengerPlotCanva.Draw();
        }

        /// <summary>
        /// Run data handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.IsBackgroundWorking = true;
            this.tb_info.Text = "Running Data Handler...";

            Form_RealifeDataHandler dataHandler = new PassengerPlot.Form_RealifeDataHandler();
            dataHandler.ShowDialog();
            if (dataHandler.DialogResult == true)
                LoadViews();
            else
            {
                this.tb_info.Text = "No data is loaded";
                this.IsBackgroundWorking = false;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            passengerPlotCanva.Transformation.StepBackward();
            passengerPlotCanva.CalculatePlot();
            passengerPlotCanva.Draw();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                passengerPlotCanva.Transformation.SimulationClock = Convert.ToInt32(tb_time.Text);
                passengerPlotCanva.CalculatePlot();
                passengerPlotCanva.Draw();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Drawer()
        {
            passengerPlotCanva.Transformation.StepForward();
            passengerPlotCanva.CalculatePlot();
            passengerPlotCanva.Draw();
        }

        private readonly TaskScheduler _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private bool IsStopRequested = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private void Play()
        {
            while (true)
            {
                if (IsStopRequested)
                    break;

                if (passengerPlotCanva.Transformation.SimulationClock >= 86400)
                    break;

                System.Threading.Thread.Sleep(40);

                Task.Factory.StartNew(Drawer,
                    new System.Threading.CancellationTokenSource().Token,
                    TaskCreationOptions.None, _taskScheduler).Wait();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsAnimationPlaying)
                return;

            IsStopRequested = false;
            Task.Factory.StartNew(Play);
            IsAnimationPlaying = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (!IsAnimationPlaying)
                return;

            IsStopRequested = true;
            IsAnimationPlaying = false;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            this.IsBackgroundWorking = true;
            EntityData.OutputDisplayData("DisplayData.xml");
            this.IsBackgroundWorking = false;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            this.IsBackgroundWorking = true;
            this.tb_info.Text = "Loading Display Data...";
            Task.Factory.StartNew(() => LoadDisplayData("DisplayData.xml"));
        }

        private void LoadDisplayData(string filePath)
        {
            EntityData.ReadDisplayData(filePath);
            Task.Factory.StartNew(LoadViews,
                    new System.Threading.CancellationTokenSource().Token,
                    TaskCreationOptions.None, _taskScheduler).Wait();
        }

        private void LoadViews()
        {
            passengerPlotCanva.GenerateViewData();

            passengerPlotCanva.CalculatePlot();
            passengerPlotCanva.DrawNetwork();
            passengerPlotCanva.Draw();
            this.IsBackgroundWorking = false;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (controlPanel.Visibility == Visibility.Collapsed)
                controlPanel.Visibility = Visibility.Visible;
            else
                controlPanel.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            // default view
            passengerPlotCanva.SetDefaultView();
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            // custom zoom in
            passengerPlotCanva.ActivateCustomZoom();
        }

        private void btn_findPassenger_Click(object sender, RoutedEventArgs e)
        {
            if(passengerPlotCanva.HighlightPassengerView(tb_passengerName.Text) == null)
            {
                tb_passengerName.Text = "Passenger not found!";
            }
        }

        private void btn_clearPassengerHighlight_Click(object sender, RoutedEventArgs e)
        {
            passengerPlotCanva.ClearHightlight();
        }

        private void btn_PassengerList_Click(object sender, RoutedEventArgs e)
        {
            Form_PassengerList pl = new PassengerPlot.Form_PassengerList(passengerPlotCanva.passengerViewList);
            pl.ShowDialog();
            passengerPlotCanva.Draw();
        }

        private void btn_CarriageList_Click(object sender, RoutedEventArgs e)
        {
            Form_CarriageList cl = new PassengerPlot.Form_CarriageList(passengerPlotCanva.carriageViewList);
            cl.ShowDialog();
            passengerPlotCanva.Draw();
        }

        private bool isAnimationPlaying;

        public bool IsAnimationPlaying
        {
            get
            {
                return isAnimationPlaying;
            }
            set
            {
                isAnimationPlaying = value;
            }
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (IsAnimationPlaying)
                {
                    IsStopRequested = true;
                }
                else
                {
                    IsStopRequested = false;
                    Task.Factory.StartNew(Play);
                }
                IsAnimationPlaying = !IsAnimationPlaying;
            }
        }

        private void btn_MoveView_Click(object sender, RoutedEventArgs e)
        {
            passengerPlotCanva.ActivateViewMove();
        }

        private void btn_FacilityList_Click(object sender, RoutedEventArgs e)
        {
            Form_StopFacilityList sf = new PassengerPlot.Form_StopFacilityList(passengerPlotCanva.facilityViewList);
            sf.ShowDialog();
            passengerPlotCanva.DrawNetwork();
            passengerPlotCanva.Draw();
        }

        private void btn_showPassengerList_Click(object sender, RoutedEventArgs e)
        {
            VPassenger vp = passengerPlotCanva.HighlightPassengerView(tb_passengerName.Text);
            if (vp == null)
            {
                tb_passengerName.Text = "Passenger not found!";
            }
            else
            {
                Form_PassengerInfo pi = new Form_PassengerInfo(vp);
                pi.Show();
            }
        }
    }
}
