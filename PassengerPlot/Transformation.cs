using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerPlot
{
    public class Transformation : System.ComponentModel.INotifyPropertyChanged
    {
        private double zoomX = 0.05;

        public double ZoomX
        {
            get
            {
                return zoomX;
            }
            set
            {
                zoomX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomX"));
                RequireDrawing?.Invoke(this, new PropertyChangedEventArgs("SetDefault"));
            }
        }

        private double zoomY = 0.05;

        public double ZoomY
        {
            get
            {
                return zoomY;
            }
            set
            {
                zoomY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomY"));
                RequireDrawing?.Invoke(this, new PropertyChangedEventArgs("SetDefault"));
            }
        }

        private double orgX = -2660000;

        public double OrgX
        {
            get
            {
                return orgX;
            }
            set
            {
                orgX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgX"));
                RequireDrawing?.Invoke(this, new PropertyChangedEventArgs("SetDefault"));
            }
        }

        private double orgY = -1260000;

        public double OrgY
        {
            get
            {
                return orgY;
            }
            set
            {
                orgY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgY"));
                RequireDrawing?.Invoke(this, new PropertyChangedEventArgs("SetDefault"));
            }
        }

        public int SimulationStartTime { get; set; } = 0;

        public int SimulationEndTime { get; set; } = 86400;

        private int simulationClock = 28800;

        public int SimulationClock
        {
            get
            {
                return simulationClock;
            }
            set
            {
                simulationClock = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SimulationClock"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SimulationTime"));
            }
        }

        public string SimulationTime
        {
            get
            {
                int HH = simulationClock / 3600;
                int mm = (simulationClock - HH * 3600) / 60;
                int ss = simulationClock - HH * 3600 - mm * 60;

                string str_HH = TimeStringParse(HH);
                string str_mm = TimeStringParse(mm);
                string str_ss = TimeStringParse(ss);
                return str_HH + ":" + str_mm + ":" + str_ss;
            }
        }

        private string TimeStringParse(int xx)
        {
            if (xx < 10)
                return "0" + xx.ToString();
            else
                return xx.ToString();
        }

        public int TickStepSize { get; set; } = 10;

        public void StepForward()
        {
            SimulationClock += TickStepSize;
        }
        
        public void StepBackward()
        {
            SimulationClock -= TickStepSize;
        }

        public System.Drawing.Point CalculatePixel(System.Windows.Point location)
        {
            return new System.Drawing.Point((int)(ZoomX * (location.X + OrgX)),
                (int)(-ZoomY * (location.Y + OrgY)));
        }

        public System.Windows.Point CalculateActualLocation(System.Windows.Point pixel)
        {
            return new System.Windows.Point(pixel.X / ZoomX - OrgX, pixel.Y / (-ZoomY) - OrgY);
        }

        private System.Windows.Point mouseLocation;

        public System.Windows.Point MouseLocation
        {
            get
            {
                return mouseLocation;
            }
            set
            {
                mouseLocation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MouseLocation"));
            }
        }

        private VCarriage hitCarriage;

        public VCarriage HitCarriage
        {
            get
            {
                return hitCarriage;
            }
            set
            {
                hitCarriage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HitCarriage"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MouseAccompanyInfo"));
            }
        }

        private VPassenger hitPassenger;

        public VPassenger HitPassenger
        {
            get
            {
                return hitPassenger;
            }
            set
            {
                hitPassenger = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HitPassenger"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MouseAccompanyInfo"));
            }
        }

        public string MouseAccompanyInfo
        {
            get
            {
                string info = "";
                if(HitPassenger != null)
                {
                    info += HitPassenger.ToString();
                    if (HitCarriage != null)
                        info += "\r\n";
                }
                if(HitCarriage != null)
                {
                    info += HitCarriage.ToString();
                }
                return info;
            }
        }

        public void SetViewScale(double zoomX, double zoomY, double orgX, double orgY)
        {
            this.zoomX = zoomX;
            this.zoomY = zoomY;
            this.orgX = orgX;
            this.orgY = orgY;
            RequireDrawing?.Invoke(this, new PropertyChangedEventArgs("SetDefault"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomX"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ZoomY"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgX"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgY"));
        }

        public void SetNewOrgLocation(double orgX, double orgY)
        {
            this.orgX = orgX;
            this.orgY = orgY;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgX"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrgY"));
            RequireDrawing?.Invoke(this, new PropertyChangedEventArgs("SetDefault"));
        }

        public int TrackStartTime { get; set; } = 0;

        public int TrackEndTime { get; set; } = 86400;

        public int ScreenPixel_X { get; set; } = 1920;

        public int ScreenPixel_Y { get; set; } = 1200;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler RequireDrawing;
    }
}
