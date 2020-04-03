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
using Newtonsoft.Json;

namespace Asphalt__Gradcoop.Activities
{
    [Activity(Label = "ChangePrices")]
    public class ChangePrices : Activity
    {
        AsphaltBase asphaltBase;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.changePrices_layout);

            TableCreator tc = new TableCreator(this);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new Json.DictionaryAsArrayResolver();
            asphaltBase = JsonConvert.DeserializeObject<AsphaltBase>(Intent.GetStringExtra("AsphaltBase"),settings);

            // In order to get real asphalt base reference
            foreach(AsphaltBase asphaltBase1 in Constants.AsphaltBases)
            {
                if (asphaltBase.Location.Name == asphaltBase1.Location.Name)
                {
                    asphaltBase = asphaltBase1;
                    break;
                }
            }

            tc.SetChangePricesTable(asphaltBase, Resource.Id.ChangeTable);

            TextView baseName = FindViewById<TextView>(Resource.Id.BaseName);

            baseName.Text = asphaltBase.Location.Name.Trim();

            Button confirm = FindViewById<Button>(Resource.Id.ConfirmButton);
            confirm.Click += UpdateAsphaltBasesUserImport;


            Button back = FindViewById<Button>(Resource.Id.ChangeBackButton);
            back.Click += GoBack;

        }

        private void GoBack(object sender, EventArgs eventArgs)
        {
            Finish();
        }

        private void UpdateAsphaltBasesUserImport(object sender, EventArgs eventArgs)
        {
            if (CheckImportedData())
            {
                Constants.UpdateAsphaltBases();
                Finish();
            }
            else
                Toast.MakeText(this, "Nije dobar unos cena i datuma.\nIspravite zapis.", ToastLength.Long).Show();
        }

        private bool CheckImportedData()
        {
            TableLayout table = FindViewById<TableLayout>(Resource.Id.ChangeTable);

            Tuple<Asphalt, double, DateTime>[] data = new Tuple<Asphalt, double, DateTime>[table.ChildCount];

            bool allOk = true;
            for (int i = 1, j = table.ChildCount; i < j; i++)
            {
                TableRow tableRow = (TableRow)table.GetChildAt(i);

                

                string asphaltName = ((TextView)tableRow.GetChildAt(0)).Text;

                Asphalt asphalt = Asphalt.Find(asphaltName);

                EditText priceEdit = (EditText)tableRow.GetChildAt(1);
                EditText dateField = (EditText)tableRow.GetChildAt(2);

                if(dateField.Text == "" && priceEdit.Text == "")
                {
                    asphaltBase.Remove(asphalt);
                }

                if (dateField.Text == "" || priceEdit.Text == "")
                    continue;

                double price = Constants.GetInvariantCultureDouble(priceEdit.Text);

                DateTime Test;
                if (DateTime.TryParseExact(dateField.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out Test) == true && !double.IsNaN(price))
                {
                    data[i] =  new Tuple<Asphalt,double,DateTime>(asphalt,price,Test);
                }
                else { 
                    allOk = false;
                    break;
                }
            }

            if (!allOk)
                return false;

            for (int i = 1, j = table.ChildCount; i < j; i++)
            {
                if (data[i] == null)
                    continue;
                asphaltBase.SetAsphaltVersion(data[i].Item1, data[i].Item2, data[i].Item3);
            }
            return true;
        }
    }
}