using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PassengerPlot
{
    public class Station : EntityElement
    {
        internal VNode NodeView { get; set; }

        internal VNode View
        {
            get
            {
                return NodeView;
            }

            set
            {
                NodeView = (VNode)value;
            }
        }

        public Point Location { get; set; }
    }
}
