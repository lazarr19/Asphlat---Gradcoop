using System;
using System.Collections.Generic;
using System.Globalization;

namespace Asphalt__Gradcoop {

    public class Asphalt
    {
        public Asphalt(string name, double coef)
        {
            Name = name;
            Coefficient = coef;
        }
        public string Name { get; set; }
        public double Coefficient { get; set; }

        public static List<string> GetListOfAsphaltTypes() {
            List<string> list = new List<string>();
            foreach (var prop in typeof(Asphalt).GetProperties())
            {
                if (prop.GetGetMethod().IsStatic && prop.PropertyType == typeof(Asphalt))
                {
                    var asphalt = (Asphalt)prop.GetValue(null);
                    list.Add(asphalt.Name);
                }
            }
            return list;
        }
        public static Asphalt Find(string name) {
            foreach (var prop in typeof(Asphalt).GetProperties())
            {
                if (prop.GetGetMethod().IsStatic && prop.PropertyType == typeof(Asphalt))
                {
                    var asphalt = (Asphalt)prop.GetValue(null);
                    if(asphalt.Name == name)
                        return asphalt;
                }
            }
            return null;
        }

        public static Asphalt AB8 { get; } = new Asphalt("AB8", 2);
        public static Asphalt AB11 { get; } = new Asphalt("AB11", 1.53);
        public static Asphalt BNS22A { get; } = new Asphalt("BNS22A", 1.53);
    }


    public class AsphaltBase
    {
        public AsphaltBase(string name, double lat, double lon) {
            Location.Name = name;
            Location.Latitude = lat;
            Location.Longitude = lon;
        }
        public Location Location { get; set; } = new Location();

        public Dictionary<Asphalt, double> AsphaltPrice = new Dictionary<Asphalt, double>();

        public double GetPrice(Asphalt asphalt)
        {
            if (AsphaltPrice.ContainsKey(asphalt))
                return AsphaltPrice[asphalt];
            else
                return 0;
        }

    }
}