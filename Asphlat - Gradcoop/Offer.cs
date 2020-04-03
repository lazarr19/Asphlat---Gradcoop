using System;
using System.Collections.Generic;
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
    public class Offer
    {
        public Offer(double length, double width, double height, Asphalt asphalt)
        {
            RoadLength = length;
            RoadWidth = width;
            RoadHeight = height;
            TypeOfAsphalt = asphalt;
        }


        public double TotalPrice { get; set; }
        public double RoadLength { get; set; }
        public double RoadWidth { get; set; }
        public double RoadHeight { get; set; }
        public double AsphlatVelocity { get { return RoadLength * RoadWidth * RoadHeight / 1000; } } // /1000 because of Height is in mm
        public Asphalt TypeOfAsphalt { get; set; }
        public double AsphaltWeight { get { return AsphlatVelocity * TypeOfAsphalt.Coefficient; } }
        public AsphaltBase AsphaltBase { get; set; } = null;

        public double InstallationPrice { get; set; }

        public double GetPrice(AsphaltBase asphaltBase)
        {
            return User.PriceForDistance(asphaltBase) + (asphaltBase.GetPrice(TypeOfAsphalt) + InstallationPrice) * AsphaltWeight;
        }

        public double GetAsphaltPrice()
        {
            return AsphaltBase.GetPrice(TypeOfAsphalt) * AsphaltWeight;
        }

        public bool IsSet()
        {
            return AsphaltBase != null;
        }

    }
}