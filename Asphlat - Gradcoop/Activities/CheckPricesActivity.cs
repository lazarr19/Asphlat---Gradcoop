using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Asphalt__Gradcoop.Activities
{
    [Activity(Label = "CheckPricesActivity")]
    public class CheckPricesActivity : Activity
    {
        private TableLayout tableLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            UserDialogs.Init(this);

            SetContentView(Resource.Layout.checkPrices_layout);

            tableLayout = FindViewById<TableLayout>(Resource.Id.CheckTable);

            TableCreator tc = new TableCreator(this);

            foreach(var offer in User.Offers)
            {
                tc.AddChangePricesTable(Resource.Id.CheckTable, offer);
            }

            Button next = FindViewById<Button>(Resource.Id.CheckButton);
            next.Click += CheckForPriceChange;
        }


        private void CheckForPriceChange(object sender, EventArgs eventArgs)
        {
            if (CheckImportedData())
            {
                Constants.UpdateAsphaltBases();
                AskForEmulsion();
            }
            else
                Toast.MakeText(this, "Nije dobar unos cena i datuma.\nIspravite zapis.", ToastLength.Long).Show();
        }

        private async void AskForEmulsion()
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = "Da li se ugradjuje emulzija?",
                OkText = "Da",
                CancelText = "Ne"
            });
            User.Emulsion = result;

            StartActivity(typeof(TransportActivity));
        }

        private bool CheckImportedData()
        {

            Tuple<AsphaltBase, Asphalt, double, DateTime>[] data = new Tuple<AsphaltBase,Asphalt, double, DateTime>[tableLayout.ChildCount];

            bool allOk = true;
            Asphalt asphalt=null;

            for (int i = 0, j = tableLayout.ChildCount; i < j; i++)
            {
                TableRow tableRow = (TableRow)tableLayout.GetChildAt(i);

                string firstBoxText = ((TextView)tableRow.GetChildAt(0)).Text;

                if (firstBoxText == "Baza")
                    continue;
                else
                {
                    if (tableRow.ChildCount == 1)
                    {
                        asphalt = Asphalt.Find(firstBoxText);
                        continue;
                    }
                }

                string baseName = ((TextView)tableRow.GetChildAt(0)).Text;

                AsphaltBase asphaltBase = Constants.FindAsphaltBase(baseName);

                EditText priceEdit = (EditText)tableRow.GetChildAt(1);
                EditText dateField = (EditText)tableRow.GetChildAt(2);

                if (dateField.Text == "" && priceEdit.Text == "")
                {
                    asphaltBase.Remove(asphalt);
                }

                if (dateField.Text == "" || priceEdit.Text == "")
                    continue;

                double price = Constants.GetInvariantCultureDouble(priceEdit.Text);

                DateTime Test;
                if (DateTime.TryParseExact(dateField.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out Test) == true && !double.IsNaN(price))
                {
                    data[i] = new Tuple<AsphaltBase,Asphalt, double, DateTime>(asphaltBase, asphalt, price, Test);
                }
                else
                {
                    allOk = false;
                    break;
                }
            }

            if (!allOk)
                return false;

            for (int i = 0, j = data.Length; i < j; i++)
            {
                if (data[i] == null)
                    continue;
                data[i].Item1.SetAsphaltVersion(data[i].Item2, data[i].Item3, data[i].Item4);
            }
            return true;
        }
    }
}