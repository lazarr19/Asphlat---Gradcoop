using System;
using System.Linq;
using Acr.UserDialogs;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Asphalt__Gradcoop
{
    [Activity(Label = "ResultActivity")]
    public class ResultActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.result_layout);

            SetPricesForAsphaltBases();

            User.CalculateDistance();

            SetTotalTable();

            SetDistanceTable();

            SetAsphaltPriceTable();
        }

        private void SetPricesForAsphaltBases() {
            /*
             * Code when we do want to initialize prices for asphalt bases only on app install
            var prefs = Application.Context.GetSharedPreferences("PreferencesConstants", FileCreationMode.Private);
            if (!prefs.Contains("FirstExecution"))
            {
                Constants.SetRadnomPrices();

                var editor = prefs.Edit();
                editor.PutBoolean("FirstExecution", false);
                editor.Commit();
            }
            */
            Constants.SetRadnomPrices();
        }

        private void SetDistanceTable() {
            TableLayout distanceTable = FindViewById<TableLayout>(Resource.Id.DistanceTable);

            AddTitle(distanceTable, "Trosak transporta");
            AddHeader(distanceTable, new string[] { "Asfaltna baza", "Daljina", "Cena" });

            var sortedList = Constants.AsphaltBases.OrderBy(o => User.Distance[o.Location]).ToList();

            foreach (var asphaltBase in sortedList) {
                var distance = User.Distance[asphaltBase.Location];
                AddRow(distanceTable, asphaltBase, distance , Constants.PriceForDistance(distance));
            }
        }


        private void SetAsphaltPriceTable()
        {
            TableLayout priceTable = FindViewById<TableLayout>(Resource.Id.PriceTable);

            AddTitle(priceTable, "Kupovina asfalta");
            AddHeader(priceTable, new string[] { "Asfaltna baza", "Cena/t", "Ukupno" });

            var sortedList = Constants.AsphaltBases.OrderBy(o => o.GetPrice(User.TypeOfAsphalt)).ToList();

            foreach (var asphaltBase in sortedList)
            {
                var price = asphaltBase.GetPrice(User.TypeOfAsphalt);
                if(price!=0)
                    AddRow(priceTable, asphaltBase, price, Math.Round(price * User.AsphaltWeight));
            }
        }

        private void SetTotalTable() {
            TableLayout totalTable = FindViewById<TableLayout>(Resource.Id.TotalTable);
            AddTitle(totalTable, "Kompletan iznos");
            AddHeader(totalTable, new string[] { "Asfaltna baza", "Ukupna cena" });

            var sortedList = Constants.AsphaltBases.OrderBy(o => Constants.PriceForDistance(User.Distance[o.Location]) + User.AsphaltWeight * o.GetPrice(User.TypeOfAsphalt)).ToList();

            foreach (var asphlatBase in sortedList)
                if(asphlatBase.GetPrice(User.TypeOfAsphalt)!=0)
                    AddRow(totalTable, asphlatBase);


            TextView note = new TextView(this)
            {
                Text = "(klikom na tabelu mozete generisati PDF fajl)"
            };
            note.SetTextColor(Color.ParseColor("#ffffff00"));
            note.TextSize = 10;
            note.SetPadding(0, 15, 0, 15);
            TableRow row = new TableRow(this);
            row.SetGravity(GravityFlags.Center);
            row.AddView(note);
            totalTable.AddView(row);
        }

        private void AddTitle(TableLayout table, string title) {
            TableRow row = new TableRow(this);
            row.SetGravity(GravityFlags.Center);

            TextView tv = new TextView(this)
            {
                Text = title
            };
            tv.SetTextColor(Color.ParseColor("#ffffff00"));
            tv.TextSize = 24;
            tv.SetPadding(15, 15, 15, 15);
            row.AddView(tv);

            table.AddView(row);
        }

        private void AddHeader(TableLayout table, string[] headers) {

            TableRow row = new TableRow(this);
            row.SetGravity(GravityFlags.Center);

            foreach (var name in headers) {
                AddTextViewCell(row, name);
            }
            table.AddView(row);
        }

        private void AddRow(TableLayout table, AsphaltBase asphaltBase) {
            TableRow row = new TableRow(this);
            row.SetGravity(GravityFlags.Center);

            var totalPrice = Math.Round( User.AsphaltWeight * asphaltBase.GetPrice(User.TypeOfAsphalt) + Constants.PriceForDistance(User.Distance[asphaltBase.Location]) );

            AddTextViewCell(row, asphaltBase.Location.Name);
            AddTextViewCell(row, Constants.GetInvariantCultureString(totalPrice));

            row.Click += (sender, eventArgs) => {
                CreatePDF(asphaltBase, totalPrice);
                };

            table.AddView(row);
        }

        private void AddRow(TableLayout table, AsphaltBase asphaltBase, double distance, double price) {

            TableRow row = new TableRow(this);
            row.SetGravity(GravityFlags.Center);

            AddTextViewCell(row, asphaltBase.Location.Name.Trim());//Trim in order to get space
            AddTextViewCell(row, Constants.GetInvariantCultureString(distance));
            AddTextViewCell(row, Constants.GetInvariantCultureString(price));

            table.AddView(row);
        }

        private void AddTextViewCell(TableRow row, string text) {
            TextView tv = new TextView(this)
            {
                Text = text
            };
            tv.SetTextColor(Color.ParseColor("#ffffff00"));
            tv.SetPadding(15, 15, 15, 15);
            tv.SetBackgroundResource(Resource.Mipmap.border);
            row.AddView(tv);
        }

        private async void CreatePDF(AsphaltBase asphaltBase, double totalPrice)
        {
            //Create PDF document

            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = "Da li zelite da napravire PDF ponudu sa baze " + asphaltBase.Location.Name + " sa vrednoscu ponude "+ Constants.GetInvariantCultureString(totalPrice)  + "?",
                OkText = "Napravi",
                CancelText = "Otkazi"
            });
            if (result)
            {
                try
                {
                    PDF.CreatePDF(asphaltBase);
                    Toast.MakeText(this, "Uspesno ste napravili ponudu!\nFajl se nalazi u Files\\Documents.", ToastLength.Long).Show();
                }
                catch
                {
                    Toast.MakeText(this, "Javio se problem. Probajte ponovo.", ToastLength.Long).Show();
                }
            }
        }

    }
}