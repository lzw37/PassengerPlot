using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PassengerPlot
{
    public static partial class EntityData
    {
        // Write file
        public static void OutputDisplayData(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("PassengerDisplayData");
            xmlDoc.AppendChild(rootElement);

            XmlElement stationSetElement = xmlDoc.CreateElement("Stations");
            rootElement.AppendChild(stationSetElement);
            OutputStation(xmlDoc, stationSetElement);

            XmlElement sectionSetElement = xmlDoc.CreateElement("Sections");
            rootElement.AppendChild(sectionSetElement);
            OutputSection(xmlDoc, sectionSetElement);

            XmlElement stopFacilitySetElement = xmlDoc.CreateElement("Facilities");
            rootElement.AppendChild(stopFacilitySetElement);
            OutputStopFacility(xmlDoc, stopFacilitySetElement);

            XmlElement carriageSetElement = xmlDoc.CreateElement("Carriages");
            rootElement.AppendChild(carriageSetElement);
            OutputCarriage(xmlDoc, carriageSetElement);

            XmlElement passengerSetElement = xmlDoc.CreateElement("Passengers");
            rootElement.AppendChild(passengerSetElement);
            OutputPassenger(xmlDoc, passengerSetElement);


            xmlDoc.Save(fileName);
        }

        private static void OutputStation(XmlDocument xmlDoc, XmlElement stationSetElement)
        {
            foreach (Station sta in StationList.Values)
            {
                XmlElement staElement = xmlDoc.CreateElement("Station");
                stationSetElement.AppendChild(staElement);

                staElement.SetAttribute("ID", sta.ID);
                staElement.SetAttribute("Name", sta.Name);
                staElement.SetAttribute("Location", sta.Location.ToString());
            }
        }

        private static void OutputSection(XmlDocument xmlDoc, XmlElement sectionSetElement)
        {
            foreach (Section sec in SectionList.Values)
            {
                XmlElement secElement = xmlDoc.CreateElement("Section");
                sectionSetElement.AppendChild(secElement);

                secElement.SetAttribute("ID", sec.ID);
                secElement.SetAttribute("FromStationID", sec.FromStation.ID);
                secElement.SetAttribute("ToStationID", sec.ToStation.ID);
                secElement.SetAttribute("Name", sec.Name);
                secElement.SetAttribute("Mode", sec.Mode);
            }
        }

        private static void OutputStopFacility(XmlDocument xmlDoc, XmlElement sfSetElement)
        {
            foreach (StopFacility sf in StopFacilityList.Values)
            {
                XmlElement sfElement = xmlDoc.CreateElement("StopFacility");
                sfSetElement.AppendChild(sfElement);

                sfElement.SetAttribute("ID", sf.ID);
                sfElement.SetAttribute("LinkedStationID", sf.LinkedStation.ID);
                sfElement.SetAttribute("Name", sf.Name);
                sfElement.SetAttribute("Location", sf.Location.ToString());
                sfElement.SetAttribute("IsDisplayName", sf.IsDisplayName.ToString());
            }
        }

        private static void OutputCarriage(XmlDocument xmlDoc, XmlElement caSetElement)
        {
            foreach (Carriage ca in CarriageList.Values)
            {
                XmlElement caElement = xmlDoc.CreateElement("Carriage");
                caSetElement.AppendChild(caElement);

                caElement.SetAttribute("LineID", ca.LineID);
                caElement.SetAttribute("RouteID", ca.RouteID);
                caElement.SetAttribute("DepartureID", ca.DepartureID);
                caElement.SetAttribute("ReferenceVehicleID", ca.ReferenceVehicleID);

                XmlElement eventSetElement = xmlDoc.CreateElement("EventList");
                caElement.AppendChild(eventSetElement);

                foreach (CarriageEvent e in ca.EventList)
                {
                    XmlElement eElement = xmlDoc.CreateElement("Event");
                    eventSetElement.AppendChild(eElement);

                    eElement.SetAttribute("AttachedStopFacilityID", e.AttachedStopFacility.ID);
                    eElement.SetAttribute("Time", e.Time.ToString());
                    eElement.SetAttribute("Type", e.Type.ToString());
                }
            }
        }

        private static void OutputPassenger(XmlDocument xmlDoc, XmlElement paSetElement)
        {
            foreach (Passenger pa in PassengerDict.Values)
            {
                XmlElement paElement = xmlDoc.CreateElement("Passenger");
                paSetElement.AppendChild(paElement);

                paElement.SetAttribute("ID", pa.ID);
                paElement.SetAttribute("Name", pa.Name);

                XmlElement eventSetElement = xmlDoc.CreateElement("EventList");
                paElement.AppendChild(eventSetElement);

                foreach (PassengerEvent e in pa.EventList)
                {
                    XmlElement eElement = xmlDoc.CreateElement(e.GetType().Name);
                    eventSetElement.AppendChild(eElement);

                    eElement.SetAttribute("Time", e.Time.ToString());
                    eElement.SetAttribute("Type", e.Type.ToString());
                    eElement.SetAttribute("TripType", e.TripType.ToString());

                    if (e is PassengerPrivateTripEvent)
                    {
                        eElement.SetAttribute("Location", ((PassengerPrivateTripEvent)e).Location.ToString());
                    }
                    else if (e is PassengerActivityEvent)
                    {
                        eElement.SetAttribute("ActivityName", ((PassengerActivityEvent)e).ActivityName);
                        eElement.SetAttribute("Location", ((PassengerActivityEvent)e).Location.ToString());
                    }
                    else if (e is PassengerTripEvent)
                    {
                        eElement.SetAttribute("AttachedCarriageReferenceVehicleID", ((PassengerTripEvent)e).AttachedCarriage.ReferenceVehicleID);
                    }
                }
            }
        }

        // Read file
        public static void ReadDisplayData(string fileName)
        {
            XDocument doc = XDocument.Load(fileName);
            XElement rootElement = doc.Element("PassengerDisplayData");

            XElement stationSetElement = rootElement.Element("Stations");
            ReadStation(stationSetElement);

            XElement sectionSetElement = rootElement.Element("Sections");
            ReadSection(sectionSetElement);

            XElement stopFacilitySetElement = rootElement.Element("Facilities");
            ReadStopFacility(stopFacilitySetElement);

            XElement carriageSetElement = rootElement.Element("Carriages");
            ReadCarriage(carriageSetElement);

            XElement passengerSetElement = rootElement.Element("Passengers");
            ReadPassenger(passengerSetElement);
        }

        private static void ReadStation(XElement stationSetElement)
        {
            EntityData.StationList = new Dictionary<string, Station>();

            foreach (XElement element in stationSetElement.Elements("Station"))
            {
                Station sta = new PassengerPlot.Station()
                {
                    ID = element.Attribute("ID").Value,
                    Name = element.Attribute("Name").Value,
                    Location = System.Windows.Point.Parse(element.Attribute("Location").Value)
                };
                EntityData.StationList.Add(sta.ID, sta);
            }
        }

        private static void ReadSection(XElement sectionSetElement)
        {
            EntityData.SectionList = new Dictionary<string, Section>();

            foreach (XElement element in sectionSetElement.Elements("Section"))
            {
                Section sec = new PassengerPlot.Section()
                {
                    ID = element.Attribute("ID").Value,
                    FromStation = EntityData.StationList[element.Attribute("FromStationID").Value],
                    ToStation = EntityData.StationList[element.Attribute("ToStationID").Value],
                    Name = element.Attribute("Name").Value,
                    Mode = element.Attribute("Mode").Value
                };
                EntityData.SectionList.Add(sec.ID, sec);
            }
        }

        private static void ReadStopFacility(XElement stopFacilitySetElement)
        {
            EntityData.StopFacilityList = new Dictionary<string, PassengerPlot.StopFacility>();

            foreach (XElement element in stopFacilitySetElement.Elements("StopFacility"))
            {
                StopFacility sf = new PassengerPlot.StopFacility()
                {
                    ID = element.Attribute("ID").Value,
                    LinkedStation = EntityData.StationList[element.Attribute("LinkedStationID").Value],
                    Name = element.Attribute("Name").Value,
                    Location = System.Windows.Point.Parse(element.Attribute("Location").Value),
                    IsDisplayName = bool.Parse(element.Attribute("IsDisplayName").Value)
                };
                EntityData.StopFacilityList.Add(sf.ID, sf);
            }
        }

        private static void ReadCarriage(XElement carriageSetElement)
        {
            EntityData.CarriageList = new Dictionary<string, PassengerPlot.Carriage>();

            foreach (XElement element in carriageSetElement.Elements("Carriage"))
            {
                Carriage ca = new Carriage()
                {
                    LineID = element.Attribute("LineID").Value,
                    RouteID = element.Attribute("RouteID").Value,
                    DepartureID = element.Attribute("DepartureID").Value,
                    ReferenceVehicleID = element.Attribute("ReferenceVehicleID").Value
                };
                EntityData.CarriageList.Add(ca.ReferenceVehicleID, ca);

                foreach (XElement eventElement in element.Element("EventList").Elements("Event"))
                {
                    CarriageEvent e = new CarriageEvent()
                    {
                        AttachedCarriage = ca,
                        AttachedStopFacility = EntityData.StopFacilityList[eventElement.Attribute("AttachedStopFacilityID").Value],
                        Time = Convert.ToInt32(eventElement.Attribute("Time").Value),
                        Type = (CarriageEventTypes)Enum.Parse(typeof(CarriageEventTypes), eventElement.Attribute("Type").Value)
                    };
                    ca.EventList.Add(e);
                }
            }
        }

        private static void ReadPassenger(XElement passengerSetElement)
        {
            EntityData.PassengerDict = new Dictionary<string, PassengerPlot.Passenger>();

            foreach (XElement element in passengerSetElement.Elements("Passenger"))
            {
                Passenger pa = new PassengerPlot.Passenger()
                {
                    ID = element.Attribute("ID").Value,
                    Name = element.Attribute("Name").Value
                };
                EntityData.PassengerDict.Add(pa.ID, pa);

                foreach (XElement eventElement in element.Element("EventList").Elements())
                {
                    PassengerEvent e = null;
                    switch(eventElement.Name.LocalName)
                    {
                        case "PassengerPrivateTripEvent":
                            PassengerPrivateTripEvent ept = new PassengerPrivateTripEvent()
                            {
                                Location = System.Windows.Point.Parse(eventElement.Attribute("Location").Value)
                            };
                            pa.EventList.Add(ept);
                            e = ept;
                            break;
                        case "PassengerActivityEvent":
                            PassengerActivityEvent ea = new PassengerActivityEvent()
                            {
                                ActivityName = eventElement.Attribute("ActivityName").Value,
                                Location = System.Windows.Point.Parse(eventElement.Attribute("Location").Value)
                            };
                            pa.EventList.Add(ea);
                            e = ea;
                            break;
                        case "PassengerTripEvent":
                            PassengerTripEvent et = new PassengerTripEvent()
                            {
                                AttachedCarriage = EntityData.CarriageList[eventElement.Attribute("AttachedCarriageReferenceVehicleID").Value]
                            };
                            pa.EventList.Add(et);
                            e = et;
                            break;
                        default:
                            break;                      
                    }
                    e.Time = Convert.ToInt32(eventElement.Attribute("Time").Value);
                    e.Type = (PassengerEventTypes)Enum.Parse(typeof(PassengerEventTypes), eventElement.Attribute("Type").Value);
                    e.TripType = (TripTypes)Enum.Parse(typeof(TripTypes), eventElement.Attribute("TripType").Value);
                }
            }
        }
    }
}
