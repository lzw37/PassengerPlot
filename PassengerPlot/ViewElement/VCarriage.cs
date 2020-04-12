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
    public class VCarriage : System.ComponentModel.INotifyPropertyChanged
    {
        public VCarriage(EntityElement entity)
        {
            Entity = (Carriage)entity;
        }

        public override string ToString()
        {
            return Entity.ToString();
        }

        public Carriage Entity { get; set; }

        private bool isVisible = true;

        public event PropertyChangedEventHandler PropertyChanged;

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

        internal Point Location { get; set; }

        internal double DisplayLength { get; set; } = 0.4;

        internal double RotationTangent { get; set; }

        internal void Draw(Transformation trans, System.Drawing.Graphics g)
        {
            if (!IsVisible)
                return;
            if(Location.X ==0 && Location.Y ==0)
            {
                return;
            }

            System.Drawing.Point p = trans.CalculatePixel(Location);

            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(p.X - 5, p.Y - 5, 10, 10);
            g.FillEllipse(new System.Drawing.SolidBrush(StyleManager.CarriageDefaultColor), rect);
        }

        internal bool HitTest(Point p, Transformation trans)
        {
            if (!IsVisible)
                return false;

            System.Drawing.Point pictureRectangle = trans.CalculatePixel(Location);
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pictureRectangle.X - 5, pictureRectangle.Y - 5, 10, 10);

            if (rect.Contains((float)p.X, (float)p.Y))
                return true;
            else
                return false;
        }
    }
}
