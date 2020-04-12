using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerPlot
{
    public static partial class EntityData
    {
        public static Dictionary<string, Station> StationList { get; set; }

        public static Dictionary<string, Section> SectionList { get; set; }

        public static Dictionary<string, StopFacility> StopFacilityList { get; set; }

        public static Dictionary<string, Carriage> CarriageList { get; set; }

        public static Dictionary<string, Passenger> PassengerDict { get; set; }

        public static Station GetStation(string ID)
        {
            if(StationList.ContainsKey(ID))
                return StationList[ID];
            else
                throw new ApplicationException(string.Format("Station '{0}' not exist!", ID));
        }

        public static StopFacility GetStopFacilityByName(string name)
        {
            foreach(StopFacility sf in EntityData.StopFacilityList.Values)
            {
                if (sf.Name == name)
                    return sf;
            }
            return null; 
        }

        public static Section GetSection(Station fromStation, Station toStation)
        {
            List<Section> sectionList = SectionList.Values.ToList<Section>();
            Section sec = sectionList.Find(x => x.FromStation == fromStation && x.ToStation == toStation);

            if (sec != null)
            {
                return sec;
            }
            sec = sectionList.Find(x => x.FromStation == toStation && x.ToStation == fromStation);
            if (sec != null)
            {
                return sec;
            }
            else
                throw new ApplicationException(string.Format("Section '{0}-{1}' not exist!", fromStation.ID, toStation.ID));
        }

        public static StopFacility GetStopFacility(string ID)
        {
            if (StopFacilityList.ContainsKey(ID))
                return StopFacilityList[ID];
            else
                throw new ApplicationException(string.Format("Stop facility '{0}' not exist!", ID));
        }        
    }
}
