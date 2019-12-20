using System;

using Android.App;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.content_layout);


            Spinner spinner = FindViewById<Spinner>(Resource.Id.AsphaltSipnner);

            //Set list of users for tagging
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this, Resource.Mipmap.spinner_item, Asphalt.GetListOfAsphaltTypes());
            dataAdapter.SetDropDownViewResource(Resource.Mipmap.spinner_item);
            spinner.Adapter = dataAdapter;

            Button nextButton = FindViewById<Button>(Resource.Id.NextButton2);
            nextButton.Click += ImportData;
        }

        private void ImportData(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            if (CheckNetwork())
            {
                try
                {
                    User.RoadLength = Constants.GetInvariantCultureDouble((FindViewById<EditText>(Resource.Id.Length)).Text);
                    User.RoadWidth = Constants.GetInvariantCultureDouble((FindViewById<EditText>(Resource.Id.Width)).Text);
                    User.RoadHeight = Constants.GetInvariantCultureDouble((FindViewById<EditText>(Resource.Id.Height)).Text);

                    if (double.IsNaN(User.RoadLength) || double.IsNaN(User.RoadWidth) || double.IsNaN(User.RoadHeight))
                    {
                        Snackbar.Make(view, "Lose unete dimenzije za asfalt.", Snackbar.LengthShort).Show();
                        return;
                    }

                    User.TypeOfAsphalt = Asphalt.Find(FindViewById<Spinner>(Resource.Id.AsphaltSipnner).SelectedItem.ToString());
                    if (User.TypeOfAsphalt == null)
                    {
                        Snackbar.Make(view, "Asfalt nije pronadjen.", Snackbar.LengthShort).Show();
                        return;
                    }

                    StartActivity(typeof(ResultActivity));
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

    }
}