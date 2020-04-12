using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PassengerPlot
{
    /// <summary>
    /// PassengerPlotCanva.xaml 的交互逻辑
    /// </summary>
    public partial class PassengerPlotCanva : UserControl, System.ComponentModel.INotifyPropertyChanged
    {
        public PassengerPlotCanva()
        {
            InitializeComponent();
            this.Transformation.RequireDrawing += Transformation_RequireDrawing;
        }

        /// <summary>
        /// Drawing request by Transformation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transformation_RequireDrawing(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SetDefault")
            {
                DrawNetwork();
                Draw();
            }
        }

        /// <summary>
        /// Transformation of the current canva
        /// </summary>
        public Transformation Transformation { get; set; } = new Transformation();

        private bool isDataLoaded = false;

        /// <summary>
        /// Boolean value, indicating if the display data has been loaded
        /// </summary>
        public bool IsDataLoaded
        {
            get
            {
                return isDataLoaded;
            }
            set
            {
                isDataLoaded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDataLoaded"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WelcomeFrameVisibility"));
            }
        }

        /// <summary>
        /// Visualization status of the Welcome Frame
        /// </summary>
        public Visibility WelcomeFrameVisibility
        {
            get
            {
                if (isDataLoaded)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
        }

        // Dataset of view elements
        internal List<VNode> nodeViewList = new List<VNode>();
        internal List<VLink> linkViewList = new List<VLink>();
        internal List<VStopFacility> facilityViewList = new List<VStopFacility>();
        internal List<VCarriage> carriageViewList = new List<VCarriage>();
        internal List<VPassenger> passengerViewList = new List<VPassenger>();

        /// <summary>
        /// Bitmap of the infrastructure network
        /// </summary>
        BitmapSource backgroundImage;

        /// <summary>
        /// Generate the view dataset by the entity data
        /// </summary>
        public void GenerateViewData()
        {
            if (EntityData.StationList == null || EntityData.StationList.Count == 0)
                return;
            if (EntityData.SectionList == null || EntityData.SectionList.Count == 0)
                return;

            canvas.ClearVisual();

            foreach (Station sta in EntityData.StationList.Values)
            {
                VNode n = new VNode(sta)
                {
                    Location = sta.Location
                };
                sta.View = n;
                nodeViewList.Add(n);
            }

            foreach (Section sec in EntityData.SectionList.Values)
            {
                VLink l = new VLink(sec)
                {
                    FromNode = sec.FromStation.NodeView,
                    ToNode = sec.ToStation.NodeView,
                };
                sec.View = l;
                linkViewList.Add(l);
            }

            foreach(StopFacility sf in EntityData.StopFacilityList.Values)
            {
                VStopFacility vsf = new PassengerPlot.VStopFacility(sf);
                facilityViewList.Add(vsf);
            }

            foreach (Carriage ca in EntityData.CarriageList.Values)
            {
                VCarriage c = new VCarriage(ca) { Location = new Point(10, 10) };          
                ca.View = c;
                carriageViewList.Add(c);
            }

            foreach (Passenger p in EntityData.PassengerDict.Values)
            {
                VPassenger vp = new VPassenger(p) { Location = new Point(10, 10), Entity = p };
                p.View = vp;
                passengerViewList.Add(vp);
                p.View.GeneratePassengerTrack();
            }

            IsDataLoaded = true;
        }

        /// <summary>
        /// Calculate the position of the mobile view elements by the given moment
        /// </summary>
        public void CalculatePlot()
        {
            foreach (Carriage ca in EntityData.CarriageList.Values)
            {
                ca.CalculateLocation(Transformation.SimulationClock);
            }
            foreach(Passenger p in EntityData.PassengerDict.Values)
            {
                p.CalculateLocation(Transformation.SimulationClock);
            }
        }

        /// <summary>
        /// Draw the network to bitmap object
        /// </summary>
        /// <returns></returns>
        public BitmapSource DrawNetwork()
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Transformation.ScreenPixel_X, Transformation.ScreenPixel_Y);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.FillRectangle(new System.Drawing.SolidBrush(PassengerPlot.StyleManager.Background), new System.Drawing.Rectangle(0, 0, Transformation.ScreenPixel_X, Transformation.ScreenPixel_Y));

            foreach (VNode n in nodeViewList)
            {
                n.Draw(Transformation, g);
            }

            foreach (VLink l in linkViewList)
            {
                l.Draw(Transformation, g);
            }

            foreach(VStopFacility vf in facilityViewList)
            {
                vf.Draw(Transformation, g);
            }

            //bitmap.Save("network.png", System.Drawing.Imaging.ImageFormat.Png);
            g.Dispose();

            backgroundImage = BitmapToBitmapImage(bitmap);
            return backgroundImage;
        }

        /// <summary>
        /// Draw carriage views
        /// </summary>
        /// <param name="g"></param>
        public void DrawCarriage(System.Drawing.Graphics g)
        {
            foreach (VCarriage c in carriageViewList)
            {
                c.Draw(Transformation, g);
            }
        }

        /// <summary>
        /// Draw moving view, including passenger views and carriage views
        /// </summary>
        /// <returns></returns>
        public BitmapSource DrawMovingObjects()
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Transformation.ScreenPixel_X, Transformation.ScreenPixel_Y);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawCarriage(g);
            DrawPassenger(g);

            g.Dispose();

            //bitmap.Save("moving.png", System.Drawing.Imaging.ImageFormat.Png);
            return BitmapToBitmapImage(bitmap);
        }

        /// <summary>
        /// Draw passenger views
        /// </summary>
        /// <param name="g"></param>
        public void DrawPassenger(System.Drawing.Graphics g)
        {
            foreach (VPassenger vp in passengerViewList)
            {
                vp.Draw(Transformation, g);
            }
        }

        /// <summary>
        /// Global drawing function, draw the static network and mobile view elements
        /// </summary>
        public void Draw()
        {
            Transformation.ScreenPixel_X = (int)SystemParameters.PrimaryScreenWidth;
            Transformation.ScreenPixel_Y = (int)SystemParameters.PrimaryScreenHeight;

            canvas.ClearVisual();
            DrawingVisual i = new DrawingVisual();
            canvas.AddVisual(i);

            using (DrawingContext dc = i.RenderOpen())
            {
                dc.DrawImage(backgroundImage, new Rect(0, 0, Transformation.ScreenPixel_X, Transformation.ScreenPixel_Y));
                dc.DrawImage(DrawMovingObjects(), new Rect(0, 0, Transformation.ScreenPixel_X, Transformation.ScreenPixel_Y));
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);

            // Custom zoom
            if(isCustomZoomActivated)
            {
                rectZoomOrg = e.GetPosition(this);
                isDrawingZoomRect = true;
                return;
            }

            VPassenger hitVp = HitPassenger(p);
            if (hitVp != null)
            {
                // Show passenger information list

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Form_PassengerInfo passengerInfoForm = new PassengerPlot.Form_PassengerInfo(hitVp);
                    passengerInfoForm.Show();
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    hitVp.IsTrackDisplayed = !hitVp.IsTrackDisplayed;
                    Draw();
                }
            }
            else
            {   
                // Show passenger track

                foreach (VPassenger vp in passengerViewList)
                {
                    vp.IsTrackDisplayed = false;
                }
                Draw();
            }

            // Move views
            if(isViewMovingActivated)
            {
                ViewMoveBegin(p);
            }
        }

        /// <summary>
        /// Hit carriage view by mouse
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private VCarriage HitCarriage(Point point)
        {
            foreach(VCarriage ca in carriageViewList)
            {
                if (ca.HitTest(point, Transformation))
                    return ca;
            }
            return null;
        }

        /// <summary>
        /// Hit passenger view by mouse
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private VPassenger HitPassenger(Point point)
        {
            foreach(VPassenger vp in passengerViewList)
            {
                if (vp.HitTest(point, Transformation))
                    return vp;
            }
            return null;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Custom zoom
            if(isCustomZoomActivated && isDrawingZoomRect)
            {
                DrawZoomRect(e.GetPosition(this));
                return;
            }

            // Display mouse location
            Transformation.MouseLocation = e.GetPosition(this);

            // Hit carriage view
            VCarriage ca = HitCarriage(e.GetPosition(this));
            if(ca != Transformation.HitCarriage)
                Transformation.HitCarriage = ca;

            // Hit passenger view
            VPassenger p = HitPassenger(e.GetPosition(this));
            if(p != Transformation.HitPassenger)
                Transformation.HitPassenger = p;

            // View Moving
            if(isViewMoving)
            {
                ViewMoving(e.GetPosition(this));
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Execute custom zoom
            if (isCustomZoomActivated)
            {
                DoCustomZoom();
                isCustomZoomActivated = false;
                isDrawingZoomRect = false;
                canvas.DeleteVisual(zoomRectVisual);
                Draw();
            }

            // Execute view moving
            if(isViewMoving)
            {
                DoViewMove();
            }
        }

        private BitmapSource BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {            
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png); // The transparent will be missing if the bitmap format is set to be .bmp
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.

                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        /// <summary>
        /// Set the view to the default scale 
        /// </summary>
        public void SetDefaultView()
        {
            this.Cursor = Cursors.Wait;
            Transformation.SetViewScale(0.05, 0.05, -2660000, -1260000);
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Parameters for custom zoom
        /// </summary>
        private bool isCustomZoomActivated = false;
        private bool isDrawingZoomRect = false;
        private Point rectZoomOrg;
        DrawingVisual zoomRectVisual;
        Rect zoomRect;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Activate custom zoom mode
        /// </summary>
        public void ActivateCustomZoom()
        {
            isCustomZoomActivated = true;
            zoomRectVisual = new DrawingVisual();
            canvas.AddVisual(zoomRectVisual);
        }

        /// <summary>
        /// Draw zoom reference rectangle
        /// </summary>
        /// <param name="mousePoint"></param>
        private void DrawZoomRect(Point mousePoint)
        {
            if (mousePoint.X >= rectZoomOrg.X)
            {
                zoomRect.X = rectZoomOrg.X;
                zoomRect.Width = mousePoint.X - rectZoomOrg.X;
            }
            else
            {
                zoomRect.X = mousePoint.X;
                zoomRect.Width = rectZoomOrg.X - mousePoint.X;
            }
            if (mousePoint.Y >= rectZoomOrg.Y)
            {
                zoomRect.Y = rectZoomOrg.Y;
                zoomRect.Height = mousePoint.Y - rectZoomOrg.Y;
            }
            else
            {

                zoomRect.Y = mousePoint.Y;
                zoomRect.Height = rectZoomOrg.Y - mousePoint.Y;
            }
            using (DrawingContext dc = zoomRectVisual.RenderOpen())
            {
                Pen p = new Pen(Brushes.White, 2);
                dc.DrawRectangle(null, p, zoomRect);
                dc.Close();
            }
        }


        /// <summary>
        /// Execute the custom zoom in
        /// </summary>
        private void DoCustomZoom()
        {
            this.Cursor = Cursors.Wait;

            double widthRatio = this.ActualWidth / zoomRect.Width;
            double heightRatio = this.ActualHeight / zoomRect.Height;

            double zoomX = widthRatio * Transformation.ZoomX;
            double zoomY = heightRatio * Transformation.ZoomY;

            Point actualOrgPoint = Transformation.CalculateActualLocation(zoomRect.Location);

            double orgX = -actualOrgPoint.X;
            double orgY = -actualOrgPoint.Y;

            Transformation.SetViewScale(zoomX, zoomY, orgX, orgY);

            this.Cursor = Cursors.Arrow;
        }

        // parameters for view moving
        private bool isViewMovingActivated = false;
        private bool isViewMoving = false;
        private Point previousMouseLocation;
        private double previousOrgLocationX;
        private double previousOrgLocationY;
        private double deltaX;
        private double deltaY;

        /// <summary>
        /// Activate view moving mode
        /// </summary>
        public void ActivateViewMove()
        {
            isViewMovingActivated = true;
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Mouse down - begin moving
        /// </summary>
        /// <param name="location"></param>
        private void ViewMoveBegin(Point location)
        {
            previousMouseLocation = location;
            previousOrgLocationX = Transformation.OrgX;
            previousOrgLocationY = Transformation.OrgY;
            isViewMoving = true;
        }

        /// <summary>
        /// Mouse move - moving the view
        /// </summary>
        /// <param name="location"></param>
        private void ViewMoving(Point location)
        {
            // Just move the background and hide all mobile entities.
            deltaX = location.X - previousMouseLocation.X;
            deltaY = location.Y - previousMouseLocation.Y;

            canvas.ClearVisual();
            DrawingVisual i = new DrawingVisual();
            canvas.AddVisual(i);

            using (DrawingContext dc = i.RenderOpen())
            {
                dc.DrawImage(backgroundImage, new Rect(deltaX, deltaY, Transformation.ScreenPixel_X, Transformation.ScreenPixel_Y));
            }
        }

        /// <summary>
        /// Mouse up - finish moving
        /// </summary>
        private void DoViewMove()
        {
            this.Cursor = Cursors.Wait;
            double newOrgX = previousOrgLocationX + deltaX / Transformation.ZoomX;
            double newOrgY = previousOrgLocationY - deltaY / Transformation.ZoomY;
            Transformation.SetNewOrgLocation(newOrgX, newOrgY);
            isViewMoving = false;
            isViewMovingActivated = false;
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Highlight passenger view by name
        /// </summary>
        /// <param name="PassengerName"></param>
        /// <returns></returns>
        public VPassenger HighlightPassengerView(string PassengerName)
        {
            foreach(VPassenger vp in passengerViewList)
            {
                if(PassengerName==vp.Entity.Name)
                {
                    vp.IsHighlighted = true;
                    Draw();
                    return vp;
                }
            }
            return null;
        }

        /// <summary>
        /// Clear all of the highlighted passengers
        /// </summary>
        public void ClearHightlight()
        {
            foreach (VPassenger vp in passengerViewList)
            {
                vp.IsHighlighted = false;
                vp.IsTrackDisplayed = false;
            }
            Draw();
        }
    }
}
