using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PassengerPlot
{
    internal class VNode
    {
        public VNode(Station entity)
        {
            Entity = entity;
        }

        internal Station Entity { get; set; }

        internal Point Location { get; set; }

        internal void Draw(Transformation trans, System.Drawing.Graphics g)
        {
            System.Drawing.Point p = trans.CalculatePixel(Location);

            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(p.X - 1, p.Y - 1, 2, 2);

            //g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Black, 1), rect);
            //g.FillEllipse(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(130, System.Drawing.Color.Blue)), rect);

            //DrawName(trans, g);
        }

        //internal void DrawName(Transformation trans, System.Drawing.Graphics g)
        //{
        //    System.Drawing.Point p = trans.CalculatePixel(Location);

        //    g.DrawString(Entity.Name, new System.Drawing.Font("Arial", 10), new System.Drawing.SolidBrush(System.Drawing.Color.Black), p);
        //}
    }
}
