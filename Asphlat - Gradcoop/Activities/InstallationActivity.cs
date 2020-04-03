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
    [Activity(Label = "InstallationActivity")]
    public class InstallationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.installation_layout);

            TableCreator tc = new TableCreator(this);

            tc.SetInstallationTable(Resource.Id.InstallationTable);

            Button next = FindViewById<Button>(Resource.Id.InstallationNext);
            next.Click += CheckForPriceChange;
        }

        private void CheckForPriceChange(object sender, EventArgs eventArgs)
        {
            if (CheckImportedData())
            {
                StartActivity(typeof(ResultActivity));
            }
            else
                Toast.MakeText(this, "Nije dobar unos cene.\nIspravite zapis.", ToastLength.Long).Show();
        }

        private bool CheckImportedData()
        {
            TableLayout tableLayout = FindViewById<TableLayout>(Resource.Id.InstallationTable);

            TableRow tableRow = (TableRow)tableLayout.GetChildAt(1);

            EditText priceField = (EditText)tableRow.GetChildAt(1);

            double price = Constants.GetInvariantCultureDouble(priceField.Text);

            if (!double.IsNaN(price))
            {
                foreach (var offer in User.Offers)
                {
                    offer.InstallationPrice = price;
                }
            }
            else
                return false;
            return true;
        }
    }
}