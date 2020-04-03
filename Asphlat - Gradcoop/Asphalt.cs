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

    public class Asphalt
    {
        public Asphalt() { }

        public Asphalt(string name, double coef)
        {
            Name = name;
            Coefficient = coef;
        }
        public string Name { get; set; }
        public double Coefficient { get; set; }


        public static List<string> GetListOfAsphaltTypesStrings()
        {
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

        public static List<Asphalt> GetListOfAsphaltTypes()
        {
            List<Asphalt> list = new List<Asphalt>();
            foreach (var prop in typeof(Asphalt).GetProperties())
            {
                if (prop.GetGetMethod().IsStatic && prop.PropertyType == typeof(Asphalt))
                {
                    var asphalt = (Asphalt)prop.GetValue(null);
                    list.Add(asphalt);
                }
            }
            return list;
        }
        public static Asphalt Find(string name)
        {
            foreach (var prop in typeof(Asphalt).GetProperties())
            {
                if (prop.GetGetMethod().IsStatic && prop.PropertyType == typeof(Asphalt))
                {
                    var asphalt = (Asphalt)prop.GetValue(null);
                    if (asphalt.Name == name)
                        return asphalt;
                }
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (this == null || obj == null)
                return false;
            Asphalt poredi = (Asphalt)obj;
            if (poredi == null)
                return false;
            if (Name == poredi.Name)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 8191;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Coefficient.GetHashCode();
            return hash;
        }

        public static Asphalt AB8 { get; } = new Asphalt("AB8", 2);
        public static Asphalt AB11 { get; } = new Asphalt("AB11", 1.53);
        public static Asphalt BNS22A { get; } = new Asphalt("BNS22A", 1.53);
    }
}