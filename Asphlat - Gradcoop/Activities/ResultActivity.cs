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
        Button CreatePDF;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.result_layout);

            TableCreator tc = new TableCreator(this);

            foreach(var offer in User.Offers)
                tc.SetTotalTable(Resource.Id.TotalTable, offer);
            if(User.Emulsion)
                tc.AddEmulsionToTotalTable(Resource.Id.TotalTable);

            CreatePDF = FindViewById<Button>(Resource.Id.PDFButton);
            CreatePDF.Click += CreatePDFDocument;
         }

        private void CreatePDFDocument(object sender, EventArgs eventArgs)
        {
            bool allSelected = true;
            foreach(var offer in User.Offers)
            {
                if(!offer.IsSet())
                {
                    allSelected = false;
                    break;
                }
            }
            if (allSelected)
            {
                PdfDocumentCreator.CreateAsphaltOffer(User.Location.Name, User.Offers);
                Toast.MakeText(this, "Uspesno ste napravili ponudu!\nFajl se nalazi u Files\\Documents.", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Nisu odabrane ponude!", ToastLength.Long).Show();
            }

        }

    }
}