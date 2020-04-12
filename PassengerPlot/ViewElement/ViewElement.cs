using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace PassengerPlot
{
    internal abstract class ViewElement : DrawingVisual
    {
        internal EntityElement Entity { get; set; }

        internal ViewElement(EntityElement entity)
        {
            Entity = entity;
        }

        internal virtual void Draw(Transformation trans)
        {
        }

        protected Point GetPlotLocation(Point location, Transformation trans)
        {
            return new Point(location.X * trans.ZoomX + trans.OrgX, location.Y * trans.ZoomY + trans.OrgY);
        }

        internal virtual string ClickInfo()
        {
            return Entity.GetType().ToString() + ":" + this.Entity.ID;
        }
    }
}
