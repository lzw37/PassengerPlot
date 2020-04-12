using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerPlot
{
    public class VStopFacility
    {
        public VStopFacility(StopFacility entity)
        {
            Entity = entity;
        }

        public StopFacility Entity { get; set; }

        internal void Draw(Transformation trans, System.Drawing.Graphics g)
        {
            //System.Drawing.Point p = trans.CalculatePixel(Entity.Location);
            //System.Drawing.RectangleF rect = new System.Drawing.RectangleF(p.X - 1, p.Y - 1, 2, 2);

            //g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Black, 1), rect);
            //g.FillEllipse(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(130, System.Drawing.Color.Blue)), rect);

            DrawName(trans, g);
        }

        internal void DrawName(Transformation trans, System.Drawing.Graphics g)
        {
            if (!Entity.IsDisplayName)
                return;

            System.Drawing.Point p = trans.CalculatePixel(Entity.Location);

            g.DrawString(Entity.Name, new System.Drawing.Font("Arial", 12), new System.Drawing.SolidBrush(StyleManager.FacilityNameColor), p);
        }
    }
}
