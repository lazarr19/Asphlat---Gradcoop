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

namespace Asphalt__Gradcoop.Activities
{
    [Activity(Label = "Transport")]
    public class TransportActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.transport_layout);

            User.CalculateDistance();

            PredictPrices();

            TableCreator tc = new TableCreator(this);

            tc.SetTransportTable(Resource.Id.TransportTable);

            Button next = FindViewById<Button>(Resource.Id.TransportNext);
            next.Click += CheckForPriceChange;
        }

        private void PredictPrices() {
            foreach (var asphaltBase in Constants.AsphaltBases)
            {
                double distance;
                if (User.Distance.TryGetValue(asphaltBase.Location, out distance))
                {
                    User.TransportPrice.Add(asphaltBase, Constants.CoefficientForDistance(distance));
                }
            }
        }

        private void CheckForPriceChange(object sender, EventArgs eventArgs)
        {
            if (CheckImportedData())
            {
                StartActivity(typeof(InstallationActivity));
            }
            else
                Toast.MakeText(this, "Nije dobar unos cena.\nIspravite zapis.", ToastLength.Long).Show();
        }

        private bool CheckImportedData()
        {
            TableLayout tableLayout = FindViewById<TableLayout>(Resource.Id.TransportTable);

            Tuple<AsphaltBase, double>[] data = new Tuple<AsphaltBase, double>[tableLayout.ChildCount];

            bool allOk = true;

            for (int i = 1, j = tableLayout.ChildCount; i < j; i++)
            {
                TableRow tableRow = (TableRow)tableLayout.GetChildAt(i);

                string firstBoxText = ((TextView)tableRow.GetChildAt(0)).Text;

                if (firstBoxText == "Baza")
                    continue;

                string baseName = ((TextView)tableRow.GetChildAt(0)).Text;

                AsphaltBase asphaltBase = Constants.FindAsphaltBase(baseName);

                EditText priceField = (EditText)tableRow.GetChildAt(2);

                if (priceField.Text == "")
                {
                    User.TransportPrice.Remove(asphaltBase);
                }

                double price = Constants.GetInvariantCultureDouble(priceField.Text);

                if (!double.IsNaN(price))
                {
                    data[i] = new Tuple<AsphaltBase, double>(asphaltBase, price);
                }
                else
                {
                    allOk = false;
                    break;
                }
            }

            if (!allOk)
                return false;

            for (int i = 1, j = data.Length; i < j; i++)
            {
                if (data[i] == null)
                    continue;
                if (User.TransportPrice.ContainsKey(data[i].Item1))
                    User.TransportPrice[data[i].Item1] = data[i].Item2;
                else
                    User.TransportPrice.Add(data[i].Item1, data[i].Item2);
            }
            return true;
        }
    }
}