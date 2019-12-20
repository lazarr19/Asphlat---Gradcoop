using System;
using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;


namespace Asphalt__Gradcoop
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.main_layout);

            UserDialogs.Init(this);

            RequestPermissions();

            ImageButton getLocation = FindViewById<ImageButton>(Resource.Id.LocationButton);
            getLocation.Click += SetLocation;

            Button nextButton = FindViewById<Button>(Resource.Id.NextButton);
            nextButton.Click += AcceptLocation;

        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



        private bool GPSStatusCheck()
        {
            LocationManager manager = (LocationManager)GetSystemService(LocationService);
            return manager.IsProviderEnabled(LocationManager.GpsProvider);
        }

        private void RequestPermissions()
        {
            //Permissions
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted || ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessNetworkState, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 2);
            }
        }

        
        private void SetLocation(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            TextView text = FindViewById<TextView>(Resource.Id.LocationText);

            var gpsCheck = GPSStatusCheck();

            if (gpsCheck && LocationProxy.IsLocationAvailable())
            {
                var location = LocationProxy.GetCurrentPosition().Result;
                if (location != null)
                {
                    text.Text = LocationProxy.GetLatLongString(location);
                }
                else
                {
                    text.Text = "";
                    Snackbar.Make(view, "Dogodila se greska, probajte ponovo.", Snackbar.LengthShort).Show();
                }
            }
            else
            {
                Snackbar.Make(view, "Lokacija nije dostupna! Pokusajte da je upalite.", Snackbar.LengthShort).Show();
            }

        }

        private void AcceptLocation(object sender, EventArgs eventArgs) {

            View view = (View)sender;
            TextView text = FindViewById<TextView>(Resource.Id.LocationText);

            if (User.Location.SetLatLongString(text.Text))
            {
                Continue();
            }
            else {
                Snackbar.Make(view, "Probajte ponovo. Zahtevani format za koordinate je \"Latituda,Longituda\".", Snackbar.LengthShort).Show();
            }
        }

        private async void Continue() {
            var result = await UserDialogs.Instance.PromptAsync(new PromptConfig
            {
                Title = "Lokacija",
                Message = "Molimo Vas da unesite naziv lokacije:",
                Placeholder = "Naziv lokacije",
                InputType = InputType.Name,
                OkText = "Nastavi",
                CancelText = "Otkazi"
            });

            var text = result.Text.Trim();

            if (result.Ok)
            {
                if (text == "")
                {
                    Toast.MakeText(this, "Naziv lokacije ne moze biti prazan.", ToastLength.Short).Show();
                }
                else
                {
                    if (Constants.CheckString(text))
                    {
                        User.Location.Name = text;
                        StartActivity(typeof(ContentAcivity));
                    }
                    else
                    {
                        Toast.MakeText(this, "U nazivu lokacije smeju da se nalaze samo slova, brojevi i razmak.", ToastLength.Long).Show();
                    }
                }
            }
            else {
                return;
            }
        }

    }
}

