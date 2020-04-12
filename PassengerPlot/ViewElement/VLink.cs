using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PassengerPlot
{
    internal class VLink
    {
        public VLink(EntityElement entity)
        {
            Entity = (Section)entity;
        }

        internal Section Entity { get; set; }

        internal VNode FromNode { get; set; }
        internal VNode ToNode { get; set; }

        internal double RotationTangent
        {
            get
            {
                return (FromNode.Location.Y - ToNode.Location.Y) / (FromNode.Location.X - ToNode.Location.X);
            }
        }

        internal void Draw(Transformation trans, System.Drawing.Graphics g)
        {
            System.Drawing.Point p1 = trans.CalculatePixel(FromNode.Location);
            System.Drawing.Point p2 = trans.CalculatePixel(ToNode.Location);

            g.DrawLine(new System.Drawing.Pen(StyleManager.LineColor, 1), p1, p2);
        }
    }
}
