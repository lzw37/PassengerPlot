using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerPlot
{
    public class EntityElement : System.ComponentModel.INotifyPropertyChanged
    {
        public string ID { get; set; }

        public string Name { get; set; }

        internal virtual ViewElement View { get; set; }

        public virtual event PropertyChangedEventHandler PropertyChanged;
    }
}
