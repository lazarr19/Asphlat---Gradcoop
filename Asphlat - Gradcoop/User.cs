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

        public static bool Emulsion = false;

        public static Dictionary<Location, double> Distance = new Dictionary<Location, double>();

        public static Dictionary<AsphaltBase, double> TransportPrice = new Dictionary<AsphaltBase, double>();

        public static List<Offer> Offers = new List<Offer>();

        public static int currentOffer = -1;

        public static void AddOffer(double length, double width, double height, Asphalt asphalt)
        {
            Offer newOffer = new Offer(length, width, height, asphalt);
            Offers.Add(newOffer);
            currentOffer++;
        }
        
        public static Offer GetOffer(int i)
        {
            return Offers[i];
        }

        public static int GetPosition(Offer offer)
        {
            for(int i=0,j=Offers.Count;i<j;i++)
            {
                if (Offers[i].TypeOfAsphalt.Name == offer.TypeOfAsphalt.Name && Offers[i].AsphlatVelocity == offer.AsphlatVelocity)
                    return i;
            }
            return -1;
        }

        public static Offer GerCurrentOffer()
        {
            if (currentOffer == -1)
                return null;
            else
                return Offers[currentOffer];
        }

        public static double EmulsionSurface()
        {
            double max = 0;
            foreach (var offer in Offers)
            {
                double surface = offer.RoadWidth * offer.RoadLength;
                if (max < surface)
                    max = surface;
            }
            return max;
        }

        public static double PriceForEmulsion()
        {
            double max = 0;
            foreach(var offer in Offers)
            {
                double surface = offer.RoadWidth * offer.RoadLength;
                if (max < surface)
                    max = surface;
            }
            return max * Constants.EmulsionPrice;

        }

        public static double PriceForDistance(AsphaltBase asphaltBase)
        {
            return TransportPrice[asphaltBase] * Distance[asphaltBase.Location];
        }

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