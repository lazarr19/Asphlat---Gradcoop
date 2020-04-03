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
    public class AsphaltBase 
    {
        public class AsphaltVersion
        {
            public AsphaltVersion(double price, DateTime date)
            {
                Date = date;
                Price = price;
            }

            public DateTime Date { get; set; }
            public double Price { get; set; }
        }

        public AsphaltBase(string name, double lat, double lon)
        {
            Location.Name = name;
            Location.Latitude = lat;
            Location.Longitude = lon;
        }
        public Location Location { get; set; } = new Location();

        public Dictionary<Asphalt, AsphaltVersion> AsphaltTypes = new Dictionary<Asphalt, AsphaltVersion>();

        public void AddAsphalt(double price, DateTime date, Asphalt asphalt)
        {
            AsphaltTypes[asphalt] = new AsphaltVersion(price, date);
        }

        public bool Contains(Asphalt asphalt)
        {
            if (AsphaltTypes.ContainsKey(asphalt))
                return true;
            else
                return false;
        }

        public void Remove(Asphalt asphalt)
        {
            if (Contains(asphalt))
            {
                AsphaltTypes.Remove(asphalt);
            }
        }

        public double GetPrice(Asphalt asphalt)
        {
            if (AsphaltTypes.ContainsKey(asphalt))
                return AsphaltTypes[asphalt].Price;
            else
                return 0;
        }

        public void SetAsphaltVersion(Asphalt asphalt, double price, DateTime date)
        {
            if (AsphaltTypes.ContainsKey(asphalt)) { 
                AsphaltTypes[asphalt].Price = price;
                AsphaltTypes[asphalt].Date = date;
            }
            else
                AsphaltTypes.Add(asphalt, new AsphaltVersion(price, date));
        }

        public DateTime GetDate(Asphalt asphalt)
        {
            if (AsphaltTypes.ContainsKey(asphalt))
                return AsphaltTypes[asphalt].Date;
            else
                return default; // Watch out!
        }

        public DateTime GetLatestDate()
        {
            DateTime latest = new DateTime();
            foreach(var asphalt in Asphalt.GetListOfAsphaltTypes())
            {
                if(AsphaltTypes.ContainsKey(asphalt))
                    if (DateTime.Compare(latest, AsphaltTypes[asphalt].Date)<0)
                    {
                        latest = AsphaltTypes[asphalt].Date;
                    }
            }
            return latest;
        }

    }
}