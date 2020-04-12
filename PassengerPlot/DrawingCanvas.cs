using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace PassengerPlot
{
    internal class DrawingCanvas : Canvas
    {
        internal List<Visual> Visuals = new List<Visual>();

        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            return Visuals[index];
        }

        internal void AddVisual(Visual visual)
        {
            Visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }
        internal void DeleteVisual(Visual visual)
        {
            Visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }
        internal void ClearVisual()
        {
            foreach (Visual visual in Visuals)
            {
                base.RemoveVisualChild(visual);
                base.RemoveLogicalChild(visual);
            }
            Visuals.Clear();
        }

        internal void Redraw(Transformation trans)
        {
            foreach(Visual v in Visuals)
            {
                if(v is ViewElement)
                {
                    ((ViewElement)v).Draw(trans);
                }
            }
        }

        private List<DrawingVisual> hits = new List<DrawingVisual>();
        internal List<DrawingVisual> GetVisuals(Point point)
        {
            hits.Clear();
            //Rect rect = new Rect(new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y + 2));
            //RectangleGeometry rectGeo = new RectangleGeometry(rect);
            //GeometryHitTestParameters parameters = new GeometryHitTestParameters(rectGeo);
            PointHitTestParameters parameters = new PointHitTestParameters(point);
            HitTestResultCallback callback = new HitTestResultCallback(this.HitTestCallback);
            VisualTreeHelper.HitTest(this, null, callback, parameters);
            try
            {
                if (hits.Count == 0)
                    return null;
                else
                    return hits;
            }
            catch
            {
                return null;
            }
        }

        internal DrawingVisual GetVisual(Point point)
        {
            List<DrawingVisual> hitVisuals = GetVisuals(point);
            if (hitVisuals != null)
                return hitVisuals[0];
            else
                return null;
        }

        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            //GeometryHitTestResult geometryResult = (GeometryHitTestResult)result;
            DrawingVisual visual = result.VisualHit as DrawingVisual;

            //if (visual != null && geometryResult.IntersectionDetail == IntersectionDetail.Intersects)
            hits.Add(visual);
            return HitTestResultBehavior.Continue;
        }
    }
}
