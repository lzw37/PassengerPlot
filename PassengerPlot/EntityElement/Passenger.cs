using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PassengerPlot
{
    public class Passenger : EntityElement
    {
        public override event PropertyChangedEventHandler PropertyChanged;

        public Scenario AttachedScenario { get; set; }

        internal VPassenger PassengerView { get; set; }

        internal VPassenger View
        {
            get
            {
                return PassengerView;
            }
            set
            {
                PassengerView = (VPassenger)value;
            }
        }

        public List<PassengerEvent> EventList { get; set; } = new List<PassengerEvent>();

        private PassengerEvent currentEvent = null;

        public PassengerEvent CurrentEvent
        {
            get
            {
                return currentEvent;
            }
            set
            {
                currentEvent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentEvent"));
            }
        }

        public void CalculateLocation(int time)
        {
            // search event by current time
            int previousEventIndex = -1;
            int followingEventIndex = -1;

            for (int i = 0; i < EventList.Count -1; i++)
            {
                PassengerEvent e = EventList[i];
                PassengerEvent nexte = EventList[i + 1];
                if (e.Time <= time && time <= nexte.Time)
                {
                    previousEventIndex = i;
                    followingEventIndex = i + 1;
                    break;
                }
            }

            if (previousEventIndex == -1 || followingEventIndex == -1)
            {
                PassengerView.Location = new Point(0, 0);
                return;
            }

            PassengerEvent previousEvent = EventList[previousEventIndex];
            PassengerEvent followingEvent = EventList[followingEventIndex];

            CurrentEvent = previousEvent;

            PassengerStatus status = PassengerStatus.PublicTrip;
            PassengerView.Location = CalculateLocationByEvent(previousEvent, followingEvent, time, ref status);
            PassengerView.Status = status;         
        }

        public Point CalculateLocationByEvent(PassengerEvent previousEvent, PassengerEvent followingEvent, int time, ref PassengerStatus status)
        {
            // On public trip
            if (previousEvent is PassengerTripEvent && followingEvent is PassengerTripEvent)
            {
                status = PassengerStatus.PublicTrip;
                return ((PassengerTripEvent)previousEvent).AttachedCarriage.CarriageView.Location;
            }

            // On private trip
            else if (previousEvent is PassengerPrivateTripEvent && followingEvent is PassengerPrivateTripEvent)
            {
                status = PassengerStatus.PrivateTrip;

                if (previousEvent == followingEvent)
                {
                    return ((PassengerPrivateTripEvent)previousEvent).Location;
                }

                double percentage = Convert.ToDouble(time - previousEvent.Time) / (followingEvent.Time - previousEvent.Time);
                Point previousLocation = ((PassengerPrivateTripEvent)previousEvent).Location;
                Point currentLocation = ((PassengerPrivateTripEvent)followingEvent).Location;

                double x = percentage * (currentLocation.X - previousLocation.X) + previousLocation.X;
                double y = percentage * (currentLocation.Y - previousLocation.Y) + previousLocation.Y;
              
                return new Point(x, y);
            }

            // On activity
            else if (previousEvent is PassengerActivityEvent && followingEvent is PassengerActivityEvent)
            {
                status = PassengerStatus.Activity;
                return ((PassengerActivityEvent)previousEvent).Location;
            }

            // switch
            else
            {
                return PassengerView.Location;
            }
        }

        public Dictionary<int, Point> CalculateTrack()
        {
            Dictionary<int, Point> pointList = new Dictionary<int, Point>();
            for (int i = 0; i < EventList.Count - 1; i++)
            {
                PassengerEvent e = EventList[i];
                if(!pointList.ContainsKey(e.Time))
                    pointList.Add(e.Time, CalculateEventLocation(e));
            }
            return pointList;
        }

        private Point CalculateEventLocation(PassengerEvent e)
        {
            if (e is PassengerActivityEvent)
                return ((PassengerActivityEvent)e).Location;
            else if (e is PassengerPrivateTripEvent)
                return ((PassengerPrivateTripEvent)e).Location;
            else if (e is PassengerTripEvent)
            {
                PassengerTripEvent pte = e as PassengerTripEvent;
                return pte.AttachedCarriage.GetLocationByTime(e.Time);
            }
            else
                return new Point(-1, -1);
        }
    }

    public abstract class PassengerEvent
    {
        public Passenger AttachedPassenger { get; set; }

        public int Time { get; set; }

        public PassengerEventTypes Type { get; set; }

        public TripTypes TripType { get; set; } = TripTypes.Private;

        public virtual string Abstract { get; }
    }

    public class PassengerTripEvent : PassengerEvent
    {
        public Carriage AttachedCarriage { get; set; }

        public override string Abstract
        {
            get
            {
                return "Carriage: " + AttachedCarriage.ToString();
            }
        }
    }

    public class PassengerPrivateTripEvent : PassengerEvent
    {
        public Point Location { get; set; }

        public override string Abstract
        {
            get
            {
                return "Location: " + Location.ToString();
            }
        }
    }

    public class PassengerActivityEvent : PassengerEvent
    {
        public string ActivityName { get; set; }

        public Point Location { get; set; }

        public override string Abstract
        {
            get
            {
                return "Activity: " + ActivityName + " - Location: " + Location.ToString();
            }
        }
    }

    public class Activity
    {
        public string personID { get; set; }

        public string Name { get; set; }

        public Point Location { get; set; }
    }

    public enum TripTypes
    {
        Private,
        Public
    }

    public enum PassengerEventTypes
    {
        ActivityStart,
        ActivityEnd,
        TripStart,
        TripEnd
    }
}
