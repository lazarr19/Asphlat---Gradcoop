using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Asphalt__Gradcoop
{
    class TableCreator
    {

        private Activity activity;
        private Type nextType;

        public TableCreator(Activity activity)
        {
            this.activity = activity;
        }

        public TableCreator(Activity activity, Type nextType)
        {
            this.activity = activity;
            this.nextType = nextType;
        }

        public void SetInstallationTable(int resourceId)
        {
            TableLayout installationTable = activity.FindViewById<TableLayout>(resourceId);

            AddTitle(installationTable, "Cena ugradnje");

            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            AddTextViewCell(row, "Cena ugradnje po toni");
            AddEditTextCell(row, Constants.GetInvariantCultureString(Constants.InstallationPrice));

            installationTable.AddView(row);
        }

        public void AddChangePricesTable(int resourceId, Offer offer)
        {
            TableLayout asphaltTable = activity.FindViewById<TableLayout>(resourceId);

            AddTitle(asphaltTable, offer.TypeOfAsphalt.Name);
            AddHeader(asphaltTable, new string[] { "Baza", "Cena", "Vreme" });

            var list = Constants.AsphaltBases;

            foreach (var asphaltBase in list)
            {
                AddCheckRow(asphaltTable, asphaltBase, offer.TypeOfAsphalt);
            }
        }



        public void SetTransportTable(int resourceId)
        {
            TableLayout transportTable = activity.FindViewById<TableLayout>(resourceId);

            AddTitle(transportTable, "Cena transporta");
            AddHeader(transportTable, new string[] { "Baza", "Razdaljina", "Cena (t/km)"});

            var sortedList = Constants.AsphaltBases.OrderBy(o => User.Distance[o.Location]).ToList();

            foreach (var asphaltBase in sortedList)
            {
                AddRow(transportTable, asphaltBase);
            }

        }

        private void AddRow(TableLayout table, AsphaltBase asphaltBase)
        {
            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            AddTextViewCell(row, asphaltBase.Location.Name.Trim());//Trim in order to get space
            double distance;
            AddTextViewCell(row, User.Distance.TryGetValue(asphaltBase.Location, out distance) ? Constants.GetInvariantCultureString(distance) : "");
            AddEditTextCell(row, Constants.GetInvariantCultureString(Constants.CoefficientForDistance(distance)));

            table.AddView(row);
        }

        public void SetChangePricesTable(AsphaltBase asphaltBase, int resourceId)
        {
            TableLayout asphaltTable = activity.FindViewById<TableLayout>(resourceId);

            AddHeader(asphaltTable, new string[] { "Vrsta", "Cena", "Vreme azuriranja" });

            var sortedList = Asphalt.GetListOfAsphaltTypes();

            foreach (var asphalt in sortedList)
            {
                AddRow(asphaltTable, asphaltBase, asphalt);
            }
        }


        public void SetBasesTable(int resourceId)
        {
            TableLayout basesTable = activity.FindViewById<TableLayout>(resourceId);

            AddTitle(basesTable, "Asfaltne baze");
            AddHeader(basesTable, new string[] { "Asfaltna baza", "Najskorije azuriranje" });

            var list = Constants.AsphaltBases;

            foreach(var asphaltBase in list)
            {
                AddRowClick(basesTable, asphaltBase, asphaltBase.GetLatestDate());
            }

        }

        /*
        public void SetDistanceTable(int resourceId)
        {
            TableLayout distanceTable = activity.FindViewById<TableLayout>(resourceId);

            AddTitle(distanceTable, "Trosak transporta");
            AddHeader(distanceTable, new string[] { "Asfaltna baza", "Daljina", "Cena" });

            var sortedList = Constants.AsphaltBases.OrderBy(o => User.Distance[o.Location]).ToList();

            foreach (var asphaltBase in sortedList)
            {
                var distance = User.Distance[asphaltBase.Location];
                AddRow(distanceTable, asphaltBase, distance, Constants.PriceForDistance(distance));
            }
        }
        */

        public void SetAsphaltPriceTable(int resourceId, Offer offer)
        {
            TableLayout priceTable = activity.FindViewById<TableLayout>(resourceId);

            AddTitle(priceTable, "Cena ponude");
            AddHeader(priceTable, new string[] { "Asfaltna baza", "Cena/t", "Ukupno" });

            var sortedList = Constants.AsphaltBases.OrderBy(o => o.GetPrice(offer.TypeOfAsphalt)).ToList();

            foreach (var asphaltBase in sortedList)
            {
                var price = asphaltBase.GetPrice(offer.TypeOfAsphalt);
                if (price != 0)
                    AddRow(priceTable, asphaltBase, price, Math.Round(price * offer.AsphaltWeight));
            }
        }
        
        public void SetTotalTable(int resourceId, Offer offer)
        {
            TableLayout totalTable = activity.FindViewById<TableLayout>(resourceId);
            AddTitle(totalTable, "Iznos za "+offer.TypeOfAsphalt.Name);
            AddHeader(totalTable, new string[] { "Asfaltna baza", "Ukupna cena" });

            var sortedList = Constants.AsphaltBases.OrderBy(o => offer.GetPrice(o)).ToList();

            foreach (var asphlatBase in sortedList)
                if (asphlatBase.GetPrice(offer.TypeOfAsphalt) != 0)
                    AddRowClick(totalTable, asphlatBase, offer);
        }

        public void AddEmulsionToTotalTable(int resourceId)
        {
            TableLayout totalTable = activity.FindViewById<TableLayout>(resourceId);

            totalTable.AddView(new TableRow(activity));
            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);
            AddTextViewCell(row, "");

            AddTextViewCell(row, "Emulzija");
            AddTextViewCell(row, Constants.GetInvariantCultureString(User.PriceForEmulsion()));

            totalTable.AddView(row);
        }




        public void AddTitle(TableLayout table, string title)
        {
            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            TextView tv = new TextView(activity)
            {
                Text = title
            };
            tv.SetTextColor(Color.ParseColor("#ffffff00"));
            tv.TextSize = 24;
            tv.SetPadding(15, 15, 15, 15);
            row.AddView(tv);

            table.AddView(row);
        }

        public void AddHeader(TableLayout table, string[] headers)
        {

            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            foreach (var name in headers)
            {
                AddTextViewCell(row, name);
            }
            table.AddView(row);
        }

        private void AddCheckRow(TableLayout table, AsphaltBase asphaltBase, Asphalt asphalt)
        {
            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            AddTextViewCell(row, asphaltBase.Location.Name.Trim());//Trim in order to get space
            double price = asphaltBase.GetPrice(asphalt);
            AddEditTextCell(row, price == 0 ? "" : Constants.GetInvariantCultureString(price));
            DateTime date = asphaltBase.GetDate(asphalt);
            AddEditTextCell(row, DateTime.Compare(date, default) != 0 ? date.Date.ToShortDateString() : "");

            table.AddView(row);
        }

        private void AddRow(TableLayout table, AsphaltBase asphaltBase, Asphalt asphalt)
        {
            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            AddTextViewCell(row, asphalt.Name.Trim());//Trim in order to get space
            double price = asphaltBase.GetPrice(asphalt);
            AddEditTextCell(row, price == 0 ? "" : Constants.GetInvariantCultureString(price));
            DateTime date = asphaltBase.GetDate(asphalt);
            AddEditTextCell(row, DateTime.Compare(date, default) != 0 ? date.Date.ToShortDateString() : "");

            table.AddView(row);
        }

        public void AddRowClick(TableLayout table, AsphaltBase asphaltBase, DateTime date)
        {

            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            AddTextViewCell(row, asphaltBase.Location.Name.Trim());//Trim in order to get space
            AddTextViewCell(row, DateTime.Compare(date,default)!=0?date.Date.ToShortDateString():"");
            if (nextType != null)
            {
                row.Click += (sender, eventArgs) =>
                {
                    Intent intent = new Intent(activity, nextType);
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ContractResolver = new Json.DictionaryAsArrayResolver();
                    intent.PutExtra("AsphaltBase", JsonConvert.SerializeObject(asphaltBase,settings));
                    activity.StartActivity(intent);
                };
            }
            table.AddView(row);
        }

        public void AddRow(TableLayout table, AsphaltBase asphaltBase, double distance, double price)
        {

            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            AddTextViewCell(row, asphaltBase.Location.Name.Trim());//Trim in order to get space
            AddTextViewCell(row, Constants.GetInvariantCultureString(distance));
            AddTextViewCell(row, Constants.GetInvariantCultureString(price));

            table.AddView(row);
        }

        private void AddTextViewCell(TableRow row, string text)
        {
            TextView tv = new TextView(activity)
            {
                Text = text
            };
            tv.SetTextColor(Color.ParseColor("#ffffff00"));
            tv.SetPadding(15, 15, 15, 15);
            tv.SetBackgroundResource(Resource.Mipmap.border);
            row.AddView(tv);
        }

        private void AddEditTextCell(TableRow row, string text)
        {
            EditText editText = new EditText(activity)
            {
                Text = text
            };
            editText.SetTextColor(Color.ParseColor("#ffffff00"));
            editText.SetPadding(15, 15, 15, 15);
            editText.SetBackgroundResource(Resource.Mipmap.border);
            row.AddView(editText);
        }
        
        private void AddRowClick(TableLayout table, AsphaltBase asphaltBase, Offer offer)
        {
            TableRow row = new TableRow(activity);
            row.SetGravity(GravityFlags.Center);

            var totalPrice = Math.Round(offer.GetPrice(asphaltBase));

            AddTextViewCell(row, asphaltBase.Location.Name);
            AddTextViewCell(row, Constants.GetInvariantCultureString(totalPrice));

            row.Click += delegate (object sender, EventArgs e) {
                SelectRowClick(sender, e, asphaltBase, offer, table); };

            table.AddView(row);
        }

        private void SelectRowClick(Object sender, EventArgs eventArgs, AsphaltBase asphaltBase, Offer offer, TableLayout table)
        {
            offer.AsphaltBase = asphaltBase;

            TableRow tr = (TableRow)sender;

            int offerNumber = User.GetPosition(offer);

            SetTableToDefaultStyle(table, offerNumber);

            for(int i = 0, j = tr.ChildCount; i < j; i++)
            {
                TextView tv = (TextView)tr.GetChildAt(i);
                tv.SetTextColor(Color.Black);
                tv.SetBackgroundColor(Color.Yellow);
            }
        }

        private void SetTableToDefaultStyle(TableLayout table, int offerNumber)
        {
            int o = -1; // current offer
            for(int i=0,j=table.ChildCount; i < j; i++)
            {
                TableRow tr = (TableRow)table.GetChildAt(i);
                if (tr.ChildCount == 1)
                {
                    o++;
                    continue;
                }
                if (offerNumber==o)
                {
                    for (int k = 0, l = tr.ChildCount; k < l; k++)
                    {
                        TextView tv = (TextView)tr.GetChildAt(k);
                        if (tv.CurrentTextColor == Color.Black)
                        {
                            tv.SetBackgroundColor(Color.Black);
                            tv.SetTextColor(Color.Yellow);
                            tv.SetBackgroundResource(Resource.Mipmap.border);
                        }
                    }
                }
                else
                {
                    if (offerNumber < o)
                        break;
                }
            }
        }
        /*
        private async void CreatePDF(AsphaltBase asphaltBase, double totalPrice)
        {
            //Create PDF document

            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = "Da li zelite da napravire PDF ponudu sa baze " + asphaltBase.Location.Name + " sa vrednoscu ponude " + Constants.GetInvariantCultureString(totalPrice) + "?",
                OkText = "Napravi",
                CancelText = "Otkazi"
            });
            if (result)
            {
                try
                {
                    PdfDocumentCreator.CreateAsphaltOffer(User.Location.Name, asphaltBase);
                    Toast.MakeText(activity, "Uspesno ste napravili ponudu!\nFajl se nalazi u Files\\Documents.", ToastLength.Long).Show();
                }
                catch
                {
                    Toast.MakeText(activity, "Javio se problem. Probajte ponovo.", ToastLength.Long).Show();
                }
            }
        }*/

    }
}