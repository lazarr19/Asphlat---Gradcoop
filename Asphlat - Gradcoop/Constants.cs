using Android.App;
using Android.Content;
using Newtonsoft.Json;
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

        public static AsphaltBase FindAsphaltBase(string name)
        {
            foreach(var aBase in AsphaltBases){
                if (aBase.Location.Name == name)
                    return aBase;
            }
            return null;
        }

        public static void LoadAsphaltBases() { 
            var prefs = Application.Context.GetSharedPreferences("PreferencesConstants", FileCreationMode.Private);

            if (prefs.Contains("lista"))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new Json.DictionaryAsArrayResolver();
                AsphaltBases = JsonConvert.DeserializeObject<List<AsphaltBase>>(prefs.GetString("lista", null), settings);
            }
        }

        public static void UpdateAsphaltBases()
        {
            var prefs = Application.Context.GetSharedPreferences("PreferencesConstants", FileCreationMode.Private);
            var editor = prefs.Edit();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new Json.DictionaryAsArrayResolver();

            editor.PutString("lista", JsonConvert.SerializeObject(AsphaltBases, settings));
            editor.Commit();
        }


        public static void SetRadnomPrices() { 
            AsphaltBases[0].AddAsphalt(10234,new DateTime(2020,1,1),Asphalt.AB11);
            AsphaltBases[1].AddAsphalt(7100, new DateTime(2020, 1, 1), Asphalt.AB11);
            AsphaltBases[3].AddAsphalt(9890, new DateTime(2020, 1, 1), Asphalt.AB11);
            AsphaltBases[4].AddAsphalt(12890, new DateTime(2020, 1, 1), Asphalt.AB11);
            AsphaltBases[5].AddAsphalt(6520, new DateTime(2020, 1, 1), Asphalt.AB11);
        }

        public static bool CheckString(string s) {
            return s.All(c => char.IsLetterOrDigit(c) || c==' ');
        }

        public static double EmulsionPrice { get; set; } = 100;

        public static double InstallationPrice { get; set; } = 1200;

        public static double CoefficientForDistance(double distance)
        {
            if (distance <= 10)
                return 170;
            else
                if (distance <= 15)
                return 220;
            else
                if (distance <= 20)
                return 250;
            else
                if (distance <= 25)
                return 280;
            else
                if (distance <= 30)
                return 310;
            else
                if (distance <= 35)
                return 350;
            else
                if (distance <= 40)
                return 390;
            else
                if (distance <= 45)
                return 430;
            else
                if (distance <= 50)
                return 470;
            else
                if (distance <= 55)
                return 510;
            else
                if (distance <= 60)
                return 550;
            else
                if (distance <= 65)
                return 590;
            else
                if (distance <= 70)
                return 630;
            else
                if (distance <= 75)
                return 670;
            else
                if (distance <= 80)
                return 711;
            else
                if (distance <= 100)
                return 880;
            else
                if (distance <= 120)
                return 1050;
            else
                if (distance <= 150)
                return 1300;
            else
                return 1550;
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