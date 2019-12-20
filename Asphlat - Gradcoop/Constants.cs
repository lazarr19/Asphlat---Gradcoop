using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Asphalt__Gradcoop {

    class Constants {

        public static List<AsphaltBase> AsphaltBases = new List<AsphaltBase>()
        {   new AsphaltBase("Beograd Put - Rakovica", 44.723162, 20.445409),
            new AsphaltBase("Sumadija Put - Bub. Pot.", 44.725777, 20.532425),
            new AsphaltBase("Gemax - Zemun", 44.872927, 20.359663),
            new AsphaltBase("Strabag - Obrenovac", 44.622132, 20.188998),
            new AsphaltBase("Vojvodina put - Pancevo", 44.829894, 20.678748),
            new AsphaltBase("Teko Mining - Vinca", 44.777293, 20.586656)
            };

        public static void SetRadnomPrices() {
            AsphaltBases[0].AsphaltPrice[Asphalt.AB11] = 10234;
            AsphaltBases[1].AsphaltPrice[Asphalt.AB11] = 7100;
            AsphaltBases[3].AsphaltPrice[Asphalt.AB11] = 9890;
            AsphaltBases[4].AsphaltPrice[Asphalt.AB11] = 12890;
            AsphaltBases[5].AsphaltPrice[Asphalt.AB11] = 6520;
        }

        public static bool CheckString(string s) {
            return s.All(c => char.IsLetterOrDigit(c) || c==' ');
        }

        public static double PriceForDistance(double distance) {
            if (distance <= 15)
                return distance * 3500;
            else
                if (distance <= 20)
                return distance * 3300;
            else
                if (distance <= 25)
                return distance * 3000;
            else
                if (distance <= 30)
                return distance * 2900;
            else
                if (distance <= 50)
                return distance * 2500;
            else
                if (distance <= 100)
                return distance * 2300;
            else
                return distance * 2000;
        }

        public static double GetInvariantCultureDouble(string str) {
            if (!double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                return double.NaN;
            return number;
        }

        public static string GetInvariantCultureString(double d) {
            return d.ToString(CultureInfo.InvariantCulture);
        }


    }
}