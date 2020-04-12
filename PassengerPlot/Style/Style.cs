using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PassengerPlot
{
    public static class StyleManager
    {
        // Dark style
        public static Color Background { get; set; } = Color.Black;

        public static Color LineColor { get; set; } = Color.FromArgb(50, Color.Green);

        public static Color CarriageDefaultColor { get; set; } = Color.FromArgb(40, System.Drawing.Color.White);

        public static Color PassengerNameColor { get; set; } = Color.FromArgb(200, 255, 255, 255);

        public static Color FacilityNameColor { get; set; } = Color.FromArgb(80, 255, 255, 255);

        public static Color PassengerTrackColor { get; set; } = Color.FromArgb(200, System.Drawing.Color.White);

        public static Color GetPassengerColor(PassengerStatus Status, bool IsHighlight)
        {
            if (IsHighlight)
            {
                if (Status == PassengerStatus.Activity)
                {
                    return Color.FromArgb(255, 6, 96, 122);
                }
                else if (Status == PassengerStatus.PrivateTrip)
                {
                    return Color.FromArgb(255, 255, 0, 78);
                }
                else
                {
                    return Color.FromArgb(255, 229, 217, 61);
                }
            }
            else
            {
                if (Status == PassengerStatus.Activity)
                {
                    return Color.FromArgb(180, 6, 96, 122);
                }
                else if (Status == PassengerStatus.PrivateTrip)
                {
                    return Color.FromArgb(180, 255, 0, 78);
                }
                else
                {
                    return Color.FromArgb(180, 229, 217, 61);
                }
            }
        }

        public static int PassengerSize { get; set; } = 5;

        public static int PassengerHightlightSize { get; set; } = 10;

        // Plain style
        //public static Color Background { get; set; } = Color.White;

        //public static Color LineColor { get; set; } = Color.FromArgb(30, Color.Red);

        //public static Color CarriageDefaultColor { get; set; } = Color.FromArgb(40, System.Drawing.Color.Blue);

        //public static Color PassengerNameColor { get; set; } = Color.Black;

        //public static Color FacilityNameColor { get; set; } = Color.Blue;

        //public static Color GetPassengerColor(PassengerStatus Status)
        //{
        //    if (Status == PassengerStatus.Activity)
        //    {
        //        return Color.FromArgb(220, Color.Yellow);
        //    }
        //    else if (Status == PassengerStatus.PrivateTrip)
        //    {
        //        return Color.FromArgb(220, Color.Blue);
        //    }
        //    else
        //    {
        //        return Color.FromArgb(220, Color.Green);
        //    }
        //}
    }
}
