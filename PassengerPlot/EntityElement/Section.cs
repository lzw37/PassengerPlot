using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerPlot
{
    public class Section : EntityElement
    {
        internal VLink LinkView { get; set; }

        internal VLink View
        {
            get
            {
                return LinkView;
            }

            set
            {
                LinkView = (VLink)value;
            }
        }
        public Station FromStation { get; set; }

        public Station ToStation { get; set; }

        public string Mode { get; set; }
    }
}
