using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Asphalt__Gradcoop
{
    [Activity(Label = "UpdatePricesActivity")]
    public class UpdatePricesActivity : Activity
    {
        private TableCreator tc;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.updatePrices_layout);

            tc = new TableCreator(this, typeof(Activities.ChangePrices));
            tc.SetBasesTable(Resource.Id.BasesTable);

            Button back = FindViewById<Button>(Resource.Id.UpdateBackButton);
            back.Click += GoBack;
        }

        protected override void OnResume()
        {
            UpdateBasesTable();
            base.OnResume();
        }

        private void UpdateBasesTable()
        {
            TableLayout table = FindViewById<TableLayout>(Resource.Id.BasesTable);

            Tuple<Asphalt, DateTime>[] data = new Tuple<Asphalt, DateTime>[table.ChildCount];

            for (int i = 2, j = table.ChildCount; i < j; i++)
            {
                TableRow tableRow = (TableRow)table.GetChildAt(i);

                string asphaltBaseName = ((TextView)tableRow.GetChildAt(0)).Text;

                AsphaltBase asphaltBase = Constants.AsphaltBases[i-2];
                DateTime date = asphaltBase.GetLatestDate();

                TextView dateText = (TextView)tableRow.GetChildAt(1);

                dateText.Text = DateTime.Compare(date, default) != 0 ? date.Date.ToShortDateString() : "";
            }
        }


        private void GoBack(object sender, EventArgs eventArgs)
        {
            Finish();
        }




    }
}