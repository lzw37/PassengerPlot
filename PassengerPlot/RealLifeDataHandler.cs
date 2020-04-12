using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Windows;

namespace PassengerPlot
{
    public class RealLifeDataHandler
    {
        private Dictionary<string, Station> StationRefLinkDic = new Dictionary<string, Station>();
        private Dictionary<string, StopFacility> StopFacilityRefLinkDic = new Dictionary<string, StopFacility>();
        private Dictionary<string, List<Activity>> ActivityDic = new Dictionary<string, List<Activity>>();
        private Dictionary<string, StopFacility> StopFacilityNameDic = new Dictionary<string, StopFacility>();

        public void Run()
        {
            string directory = "../../../ReallifeData/visualization/";
            LoadNetwork(directory);
            LoadStopFacility(directory);
            LoadTimetable(directory);
            LoadPassengerEvent(directory);
        }

        private void LoadNetwork(string directory)
        {
            EntityData.StationList = new Dictionary<string, Station>();
            XDocument xmlDoc = XDocument.Load(directory + "input/abmt_pt_network.xml");
            XElement nodeSetElement = xmlDoc.Element("network").Element("nodes");
            foreach (XElement nodeElement in nodeSetElement.Elements("node"))
            {
                Station sta = new Station()
                {
                    ID = nodeElement.Attribute("id").Value,
                    Name = nodeElement.Attribute("id").Value,
                    Location = new Point(Convert.ToDouble(nodeElement.Attribute("x").Value),
                                         Convert.ToDouble(nodeElement.Attribute("y").Value))
                };
                EntityData.StationList.Add(sta.ID, sta);
                AddNodeRefLink(sta);
            }

            EntityData.SectionList = new Dictionary<string, Section>();
            XElement linkSetElement = xmlDoc.Element("network").Element("links");
            foreach (XElement linkElement in linkSetElement.Elements("link"))
            {
                Section sec = new Section()
                {
                    ID = linkElement.Attribute("id").Value,
                    Name = linkElement.Attribute("id").Value,
                    FromStation = EntityData.GetStation(linkElement.Attribute("from").Value),
                    ToStation = EntityData.GetStation(linkElement.Attribute("to").Value),
                    Mode = linkElement.Attribute("modes").Value,
                };
                EntityData.SectionList.Add(sec.ID, sec);
            }

            EntityData.CarriageList = new Dictionary<string, PassengerPlot.Carriage>();
            EntityData.PassengerDict = new Dictionary<string, Passenger>();
        }

        private void LoadStopFacility(string directory)
        {
            EntityData.StopFacilityList = new Dictionary<string, PassengerPlot.StopFacility>();
            XDocument xmlDoc = XDocument.Load(directory + "input/abmt_pt_schedule.xml");
            XElement transitStopsElements = xmlDoc.Element("transitSchedule").Element("transitStops");
            foreach (XElement stopElement in transitStopsElements.Elements("stopFacility"))
            {
                StopFacility sf = new PassengerPlot.StopFacility()
                {
                    Name = stopElement.Attribute("name").Value,
                    ID = stopElement.Attribute("id").Value,
                    Location = new Point(Convert.ToDouble(stopElement.Attribute("x").Value), Convert.ToDouble(stopElement.Attribute("y").Value)),
                    LinkedStation = GetStationIDByFacilityRefID(stopElement.Attribute("id").Value)
                };
                EntityData.StopFacilityList.Add(sf.ID, sf);
                if(!StopFacilityNameDic.ContainsKey(sf.Name))
                    StopFacilityNameDic.Add(sf.Name, sf);
            }

            System.IO.FileStream fs = new System.IO.FileStream(directory + "input/NameDisplayFacilities.txt", System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs);
            string facilityID = sr.ReadLine();
            while(facilityID != null && facilityID.Trim() != "")
            {
                if (EntityData.StopFacilityList.ContainsKey(facilityID))
                {
                    StopFacility sf = EntityData.StopFacilityList[facilityID];
                    sf.IsDisplayName = true;
                }
                facilityID = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
        }

        private void LoadTimetable(string directory)
        {
            XDocument xmlDoc = XDocument.Load(directory + "output/1. output_transitSchedule.xml");
            XElement transitLinesElements = xmlDoc.Element("transitSchedule");
            foreach (XElement transitLineElement in transitLinesElements.Elements("transitLine"))
            {
                string lineID = transitLineElement.Attribute("id").Value;
                foreach (XElement routeElement in transitLineElement.Elements("transitRoute"))
                {
                    string routeID = routeElement.Attribute("id").Value;
                    XElement routeProfile = routeElement.Element("routeProfile");
                    foreach (XElement departureElement in routeElement.Element("departures").Elements("departure"))
                    {
                        string departureID = departureElement.Attribute("id").Value;
                        string departureTime = departureElement.Attribute("departureTime").Value;
                        string referenceVehicleID = departureElement.Attribute("vehicleRefId").Value;

                        Carriage ca = new PassengerPlot.Carriage()
                        {
                            LineID = lineID,
                            RouteID = routeID,
                            DepartureID = departureID,
                            ReferenceVehicleID  = referenceVehicleID,
                            ID = string.Format("{0},{1},{2}", lineID, routeID, departureID)
                        };
                        EntityData.CarriageList.Add(ca.ReferenceVehicleID, ca);

                        foreach (XElement stopElement in routeProfile.Elements("stop"))
                        {
                            CarriageEvent caEvent1 = new CarriageEvent()
                            {
                                AttachedCarriage = ca,
                                AttachedStopFacility = EntityData.GetStopFacility(stopElement.Attribute("refId").Value),
                                Type = CarriageEventTypes.Arrive,
                                Time = TimeTickConverter(stopElement.Attribute("arrivalOffset").Value) + TimeTickConverter(departureTime)

                            };
                            ca.EventList.Add(caEvent1);

                            CarriageEvent caEvent2 = new CarriageEvent()
                            {
                                AttachedCarriage = ca,
                                AttachedStopFacility = EntityData.GetStopFacility(stopElement.Attribute("refId").Value),
                                Type = CarriageEventTypes.Depart,
                                Time = TimeTickConverter(stopElement.Attribute("departureOffset").Value) + TimeTickConverter(departureTime)
                            };
                            ca.EventList.Add(caEvent2);

                        }
                    }
                }
            }
        }


        private void PresolvePassengerEventData(string directory)
        {
            System.IO.FileStream fs = new System.IO.FileStream(directory + "output/1.seperatePersons.txt", System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs);

            sr.ReadLine();
            string l = sr.ReadLine();

            PassengerEvent lastEvent = null;
            string lastStationName = "";
            int passengerSequence = 0;

            while(l!=null && l.Trim()!="")
            {
                string[] data = l.Split('\t');

                if (data[5] == "pt interaction")
                {
                    l = sr.ReadLine();
                    continue;
                }

                Passenger passenger;
                if (!EntityData.PassengerDict.ContainsKey(data[1]))
                {
                    passenger = new PassengerPlot.Passenger()
                    {
                        ID = data[1],
                        Name = (passengerSequence++).ToString()
                    };
                    EntityData.PassengerDict.Add(data[1], passenger);
                }
                else
                {
                    passenger = EntityData.PassengerDict[data[1]];
                }

                PassengerEvent currentEvent;

                if(data[4]=="actend" || data[4]=="actstart")
                {
                    currentEvent = AddActivityEvent(passenger, data[4], data[5], (int)Convert.ToDouble(data[2]));
                }
                else
                {
                    currentEvent = SearchCarriage(passenger, data[4], data[5], (int)Convert.ToDouble(data[2]));
                }


                if (lastEvent!=null &&
                    lastEvent.Time != currentEvent.Time &&
                    (lastEvent.Type == PassengerEventTypes.ActivityEnd || lastEvent.Type == PassengerEventTypes.TripEnd) &&
                    (currentEvent.Type == PassengerEventTypes.ActivityStart || currentEvent.Type == PassengerEventTypes.TripStart))
                {
                    bool isOnPublicTrip = false;
                    if(lastEvent is PassengerTripEvent && currentEvent is PassengerTripEvent)
                    {
                        PassengerTripEvent lastPtEvent = lastEvent as PassengerTripEvent;
                        PassengerTripEvent currentPtEvent = currentEvent as PassengerTripEvent;
                        if(lastPtEvent.AttachedCarriage.ID == currentPtEvent.AttachedCarriage.ID)
                            isOnPublicTrip = true;
                    }

                    if(!isOnPublicTrip)
                        AddPrivateTrip(passenger, lastEvent, lastStationName, currentEvent, data[6]);
                }

                lastStationName = data[6];
                passenger.EventList.Add(currentEvent);
                lastEvent = currentEvent;
                l = sr.ReadLine();
            }

            sr.Close();
            fs.Close();
        }

        private PassengerEvent AddActivityEvent(Passenger passenger, string eventType, string activityName, int time)
        {

            // find from "plan" file
            foreach(Activity act in ActivityDic[passenger.ID])
            {
                if(act.Name==activityName)
                {
                    PassengerEventTypes type = PassengerEventTypes.ActivityStart;

                    if (eventType == "actend")
                    {
                        type = PassengerEventTypes.ActivityEnd;
                    }

                    PassengerActivityEvent e = new PassengerPlot.PassengerActivityEvent()
                    {
                        AttachedPassenger = passenger,
                        Time = time,
                        Type = type,
                        ActivityName = activityName,
                        Location = act.Location
                    };
                    return e;
                }
            }
            throw new ApplicationException("Can't find activity!");
        }

        private void ExtractActivity(string directory)
        {
            XDocument xmlDoc = XDocument.Load(directory + "input/500.plans.xml");
            foreach (XElement person in xmlDoc.Element("population").Elements("person"))
            {
                string personID = person.Attribute("id").Value;
                ActivityDic.Add(personID, new List<Activity>());

                foreach(XElement plan in person.Elements("plan"))
                {
                    if(plan.Attribute("selected").Value=="yes")
                    {
                        foreach(XElement activity in plan.Elements("activity"))
                        {
                            string activityName = activity.Attribute("type").Value;
                            string x = activity.Attribute("x").Value;
                            string y = activity.Attribute("y").Value;

                            Activity act = new Activity()
                            {
                                personID = personID,
                                Name = activityName,
                                Location = new Point(Convert.ToDouble(x), Convert.ToDouble(y))
                            };
                            ActivityDic[personID].Add(act);
                        }
                    }
                }
            }
        }

        private PassengerEvent SearchCarriage(Passenger passenger, string eventType, string vehID, int time)
        {
            if (!EntityData.CarriageList.ContainsKey(vehID))
                throw new ApplicationException("Can't find carriage!");

            Carriage carriage = EntityData.CarriageList[vehID];
            PassengerEventTypes eType = PassengerEventTypes.TripStart;
            if(eventType=="arriveAt")
            {
                eType = PassengerEventTypes.TripEnd;
            }
            TripTypes tripType = TripTypes.Public;

            PassengerTripEvent e = new PassengerPlot.PassengerTripEvent()
            {
                AttachedPassenger = passenger,
                AttachedCarriage = carriage,
                Type = eType,
                TripType = tripType,
                Time = time,
            };
            return e;
        }

        private void AddPrivateTrip(Passenger passenger, PassengerEvent lastEvent, string lastStationName, 
            PassengerEvent currentEvent, string currentStationName)
        {
            Point lastLocation;
            if (lastStationName == "-")
            {
                lastLocation = (lastEvent as PassengerActivityEvent).Location;
            }
            else
            {
                StopFacility lastF = StopFacilityNameDic[lastStationName];
                lastLocation = lastF.LinkedStation.Location;
            }

            Point currentLocation;
            if (currentStationName == "-")
            {
                currentLocation = (currentEvent as PassengerActivityEvent).Location;
            }
            else
            {
                StopFacility currentF = StopFacilityNameDic[currentStationName];
                currentLocation = currentF.LinkedStation.Location;
            }


            PassengerPrivateTripEvent e = new PassengerPrivateTripEvent()
            {
                Type = PassengerEventTypes.TripStart,
                Time = lastEvent.Time,  
                Location = lastLocation           
            };
            passenger.EventList.Add(e);

            e = new PassengerPlot.PassengerPrivateTripEvent()
            {
                Type = PassengerEventTypes.TripEnd,
                Time = currentEvent.Time,
                Location = currentLocation
            };
            passenger.EventList.Add(e);
        }

        private void LoadPassengerEvent(string directory)
        {
            ExtractActivity(directory);
            PresolvePassengerEventData(directory);
        }

        /// <summary>
        /// Convert the datetime format to simulation clock tick 
        /// </summary>
        /// <param name="DateTime">DateTime format "HH:mm:ss"</param>
        /// <returns>Simulation clock tick</returns>
        private int TimeTickConverter(string Time)
        {
            try
            {
                string[] timeArray = Time.Split(':');
                int HH = Convert.ToInt32(timeArray[0]);
                int mm = Convert.ToInt32(timeArray[1]);
                int ss = Convert.ToInt32(timeArray[2]);

                int tick = HH * 3600 + mm * 60 + ss;
                return tick;
            }
            catch
            {
                throw new ApplicationException("Time tick converting failed! Illegal value " + Time);
            }
        }

        private void AddNodeRefLink(Station station)
        {
            string originalID = station.ID;
            string[] splitID = originalID.Split('.');

            if (splitID.Length < 2)
                return;

            string[] link = splitID[1].Split(':');
            if (link[0] != "link")
                return;

            string linkRefId = link[1];

            if (StationRefLinkDic.ContainsKey(linkRefId))
                return;

            StationRefLinkDic.Add(linkRefId, station);
        }

        private Station GetStationIDByFacilityRefID(string id)
        {
            string[] splitID = id.Split('.');

            if (splitID.Length < 2)
                return null;

            string[] link = splitID[1].Split(':');
            if (link[0] != "link")
                return null;

            if (StationRefLinkDic.ContainsKey(link[1]))
            {
                return StationRefLinkDic[link[1]];
            }
            else
            {
                throw new ApplicationException("Reference node not found!");
            }
        }

        private int TimeToSimulationClockTime(string time)
        {
            return 0;
        }
    }
}

