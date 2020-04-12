using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PassengerPlot
{
    public class StopFacility : EntityElement
    {
        public Point Location { get; set; }

        public Station LinkedStation { get; set; }

        public bool IsDisplayName { get; set; } = false;
    }
}
