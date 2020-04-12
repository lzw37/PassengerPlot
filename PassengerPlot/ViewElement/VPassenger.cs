using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PassengerPlot
{
    public class VPassenger : INotifyPropertyChanged
    {
        public VPassenger(EntityElement entity)
        {
        }

        public override string ToString()
        {
            return "Passenger ID: " + Entity.ID;
        }

        public Passenger Entity { get; set; }

        internal Point Location { get; set; }

        internal PassengerStatus Status { get; set; }

        private bool isVisible = true;

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisible"));
            }
        }

        private bool isHighlighted = false;

        public bool IsHighlighted
        {
            get
            {
                return isHighlighted;
            }
            set
            {
                isHighlighted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsHighlighted"));
            }
        }

        private bool isTrackDisplayed = false;

        public bool IsTrackDisplayed
        {
            get
            {
                return isTrackDisplayed;
            }
            set
            {
                isTrackDisplayed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsTrackDisplayed"));
            }
        }

        internal void Draw(Transformation trans, System.Drawing.Graphics g)
        {
            if (!IsVisible)
                return;

            if (Location.X == 0 && Location.Y == 0)
            {
                DrawTrack(trans, g);
                return;
            }
            System.Drawing.Point p = trans.CalculatePixel(Location);

            int size = 5;
            if(IsHighlighted)
            {
                size = StyleManager.PassengerHightlightSize;
            }
            else
            {
                size = StyleManager.PassengerSize;
            }

            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(p.X - size, p.Y - size, size * 2, size * 2);
            g.FillEllipse(new System.Drawing.SolidBrush(StyleManager.GetPassengerColor(Status, IsHighlighted)), rect);

            if(IsHighlighted)
            {
                g.DrawEllipse(new System.Drawing.Pen(new System.Drawing.SolidBrush(System.Drawing.Color.White), 2), rect);
            }

            DrawID(trans, g);
            DrawTrack(trans, g);
        }

        internal void DrawID(Transformation trans, System.Drawing.Graphics g)
        {
            System.Drawing.Point p = trans.CalculatePixel(Location);
            g.DrawString(Entity.Name, new System.Drawing.Font("Calibri", 10), new System.Drawing.SolidBrush(StyleManager.PassengerNameColor), p);
        }

        internal bool HitTest(Point p, Transformation trans)
        {
            if (!IsVisible)
                return false;

            int size = 5;
            if (IsHighlighted)
            {
                size = StyleManager.PassengerHightlightSize;
            }
            else
            {
                size = StyleManager.PassengerSize;
            }

            System.Drawing.Point pictureRectangle = trans.CalculatePixel(Location);
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pictureRectangle.X - size, pictureRectangle.Y - size, size * 2, size * 2);

            if (rect.Contains((float)p.X, (float)p.Y))
                return true;
            else
                return false;
        }

        private Dictionary<int, Point> trackPointList = null;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void GeneratePassengerTrack()
        {
            trackPointList = Entity.CalculateTrack();
        }

        internal void DrawTrack(Transformation trans, System.Drawing.Graphics g)
        {
            if (!IsTrackDisplayed)
                return;

            if (trackPointList.Count < 2)
                return;


            List<System.Drawing.Point> drawingPointList = new List<System.Drawing.Point>();
            foreach (int t in trackPointList.Keys)
            {
                if (t < trans.TrackStartTime || t > trans.TrackEndTime)
                    continue;

                System.Drawing.Point p = trans.CalculatePixel(trackPointList[t]);
                drawingPointList.Add(p);
            }
            if (drawingPointList.Count <= 0)
                return;

            System.Drawing.Point[] drawingPointArray = drawingPointList.ToArray();
            g.DrawLines(new System.Drawing.Pen(StyleManager.PassengerTrackColor, 4), drawingPointArray);
        }
    }

    public enum PassengerStatus
    {
        Activity,

        PublicTrip,

        PrivateTrip,
    }
}
