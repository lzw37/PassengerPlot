using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PassengerPlot
{
    public class Carriage : EntityElement
    {
        internal VCarriage CarriageView { get; set; }

        public Scenario AttachedScenario { get; set; }

        public override string ToString()
        {
            return string.Format("<LineID:{0};RouteID:{1};DepartureID:{2}>", LineID, RouteID, DepartureID);
        }

        internal VCarriage View
        {
            get
            {
                return CarriageView;
            }

            set
            {
                CarriageView = (VCarriage)value;
            }
        }

        public string LineID { get; set; }

        public string RouteID { get; set; }

        public string DepartureID { get; set; }

        public string ReferenceVehicleID { get; set; }

        public List<CarriageEvent> EventList { get; set; } = new List<CarriageEvent>();

        private Dictionary<int, Point> LocationDic { get; set; }
        
        internal void CalculateLocation(int time)
        {
            CarriageView.Location = GetLocationByTime(time);
        }

        public Point GetLocationByTime(int time)
        {
            CarriageEvent previousEvent = EventList.First();
            CarriageEvent followingEvent = EventList.Last();

            if (time < previousEvent.Time || time > followingEvent.Time)
            {
                return new Point(0, 0);
            }

            for (int i = 0; i < EventList.Count - 1; i++)
            {
                if (EventList[i].Time <= time && EventList[i + 1].Time >= previousEvent.Time)
                {
                    previousEvent = EventList[i];
                    followingEvent = EventList[i + 1];
                }
            }

            if (previousEvent == followingEvent)
            {
                //CarriageView.RotationTangent = 0;
                return previousEvent.AttachedStopFacility.LinkedStation.Location;
            }

            if (previousEvent.Type == CarriageEventTypes.Arrive && followingEvent.Type == CarriageEventTypes.Depart) // dwelling at station
            {
                //CarriageView.RotationTangent = 0;
                return previousEvent.AttachedStopFacility.LinkedStation.Location;
            }

            if (previousEvent.Type == CarriageEventTypes.Depart && followingEvent.Type == CarriageEventTypes.Arrive)// running
            {
                //Section sec = EntityData.GetSection(previousEvent.AttachedStopFacility.LinkedStation, followingEvent.AttachedStopFacility.LinkedStation);

                double percentage = Convert.ToDouble(time - previousEvent.Time) / (followingEvent.Time - previousEvent.Time);

                //CarriageView.RotationTangent = sec.LinkView.RotationTangent;

                double x = percentage * (followingEvent.AttachedStopFacility.LinkedStation.Location.X - previousEvent.AttachedStopFacility.LinkedStation.Location.X) + previousEvent.AttachedStopFacility.LinkedStation.Location.X;
                double y = percentage * (followingEvent.AttachedStopFacility.LinkedStation.Location.Y - previousEvent.AttachedStopFacility.LinkedStation.Location.Y) + previousEvent.AttachedStopFacility.LinkedStation.Location.Y;

                Station fromNode = previousEvent.AttachedStopFacility.LinkedStation;
                Station toNode = followingEvent.AttachedStopFacility.LinkedStation;
                //CarriageView.RotationTangent = (fromNode.Location.Y - toNode.Location.Y) / (fromNode.Location.X - toNode.Location.X);
                return new Point(x, y);
            }

            return new Point(0, 0);
        }
    }

    public class CarriageEvent
    {
        public Carriage AttachedCarriage { get; set; }

        public StopFacility AttachedStopFacility { get; set; }

        public int Time { get; set; }

        public CarriageEventTypes Type { get; set; }
    }

    public enum CarriageEventTypes
    {
        Arrive,

        Depart,

        Pass,
    }
}
