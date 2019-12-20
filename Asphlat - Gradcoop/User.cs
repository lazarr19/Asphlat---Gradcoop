using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Asphalt__Gradcoop
{
    static class User
    {
        public static Location Location { get; set; } = new Location();

        public static double RoadLength { get; set; }
        public static double RoadWidth { get; set; }
        public static double RoadHeight { get; set; }
        public static double AsphlatVelocity { get { return RoadLength * RoadWidth * RoadHeight /1000; } } // /1000 because of Height is in mm
        public static Asphalt TypeOfAsphalt { get; set; }
        public static double AsphaltWeight { get { return AsphlatVelocity * TypeOfAsphalt.Coefficient; } }

        public static Dictionary<Location, double> Distance = new Dictionary<Location, double>();

        public static void CalculateDistance() {
            foreach (var asphaltBase in Constants.AsphaltBases) {
                double distance = LocationProxy.CalculateRouteDistance(asphaltBase.Location, Location);
                distance = Math.Round(distance, 1);
                if (Distance.ContainsKey(asphaltBase.Location))
                {
                    Distance[asphaltBase.Location] = distance;
                }
                else {
                    Distance.Add(asphaltBase.Location, distance);
                }
            }
        }
    }
}