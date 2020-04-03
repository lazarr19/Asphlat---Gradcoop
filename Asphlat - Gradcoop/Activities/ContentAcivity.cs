using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Asphalt__Gradcoop
{
    [Activity(Label = "ContentAcivity")]
    public class ContentAcivity : Activity
    {
        TextView offerNumber;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            UserDialogs.Init(this);

            SetContentView(Resource.Layout.content_layout);


            Spinner spinner = FindViewById<Spinner>(Resource.Id.AsphaltSipnner);

            //Set list of users for tagging
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this, Resource.Mipmap.spinner_item, Asphalt.GetListOfAsphaltTypesStrings());
            dataAdapter.SetDropDownViewResource(Resource.Mipmap.spinner_item);
            spinner.Adapter = dataAdapter;

            Button nextButton = FindViewById<Button>(Resource.Id.NextButton2);
            nextButton.Click += ImportData;

            offerNumber = FindViewById<TextView>(Resource.Id.OfferNumber);
        }

        private void ImportData(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            if (CheckNetwork())
            {
                try
                {
                    double length = Constants.GetInvariantCultureDouble((FindViewById<EditText>(Resource.Id.Length)).Text);
                    double width = Constants.GetInvariantCultureDouble((FindViewById<EditText>(Resource.Id.Width)).Text);
                    double height = Constants.GetInvariantCultureDouble((FindViewById<EditText>(Resource.Id.Height)).Text);

                    if (double.IsNaN(length) || double.IsNaN(width) || double.IsNaN(height))
                    {
                        Snackbar.Make(view, "Lose unete dimenzije za asfalt.", Snackbar.LengthShort).Show();
                        return;
                    }

                    Asphalt asphalt = Asphalt.Find(FindViewById<Spinner>(Resource.Id.AsphaltSipnner).SelectedItem.ToString());
                    if (asphalt == null)
                    {
                        Snackbar.Make(view, "Asfalt nije pronadjen.", Snackbar.LengthShort).Show();
                        return;
                    }

                    AskIfSureAsync(length, width, height, asphalt);
                }
                catch {
                    Toast.MakeText(this, "Pokusajte ponovo, dogodila se greska.", ToastLength.Long).Show();
                }
            }
            else {
                Toast.MakeText(this, "Proverite Vasu internet konekciju.", ToastLength.Long).Show();
            }
        }

        private bool CheckNetwork()
        {
            var current = Connectivity.NetworkAccess;

            return current == NetworkAccess.Internet;

        }

        private async void AskIfSureAsync(double length, double width, double height, Asphalt asphalt)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = String.Format("Asfalt: {0}\nDuzina: {1}m\nSirina: {2}m\nDebljina: {3}mm\nDa li ste dobro uneli podatke?",asphalt.Name,length,width,height),
                OkText = "Da",
                CancelText = "Ne"
            });
            if (result)
            {
                User.AddOffer(length, width, height, asphalt);
                AskForAnotherOffer();
            }
        }


        private async void AskForAnotherOffer()
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = "Da li zelite jos neki asfalt na ovoj lokaciji?",
                OkText = "Da",
                CancelText = "Ne"
            });
            if (!result)
            {
                StartActivity(typeof(Activities.CheckPricesActivity));
            }
            else
            {
                offerNumber.Text = "Ponuda broj " + (User.currentOffer + 2).ToString();
            }

        }


    }
}