using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android;
using System.Threading.Tasks;

namespace WlanAnalyzer.Droid
{
    [Activity(Label = "WlanAnalyzer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            await TryToGetPermissions();
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
                return;
            }
        }
        int RequestLocationId = 0;

        readonly string[] PermissionsGroupLocation =
        {
                            //TODO add more permissions
                            Manifest.Permission.AccessCoarseLocation,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessWifiState,
                            Manifest.Permission.ChangeWifiState,
                            Manifest.Permission.ChangeWifiMulticastState,
        };
        async Task GetPermissionsAsync()
        {
            //const string permission = Manifest.Permission.AccessFineLocation;
            //const string permission1 = Manifest.Permission.AccessWifiState;
            foreach(string permission in PermissionsGroupLocation)
            {
                if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
                {
                    //TODO change the message to show the permissions name
                    Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                    continue;
                }

                if (ShouldShowRequestPermissionRationale(permission))
                {
                    //set alert for executing the task
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Permissions Needed");
                    alert.SetMessage("The application need special permissions to continue");
                    alert.SetPositiveButton("Request Permissions", (senderAlert, args) =>
                    {
                        RequestPermissions(PermissionsGroupLocation, RequestLocationId);
                    });

                    alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                    {
                        Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();


                    return;
                }

                RequestPermissions(PermissionsGroupLocation, RequestLocationId);
                RequestLocationId++;
            }
        }
        //public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    switch (requestCode)
        //    {
        //        case RequestLocationId:
        //            {
        //                if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
        //                {
        //                    Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();

        //                }
        //                else
        //                {
        //                    //Permission Denied :(
        //                    Toast.MakeText(this, "Special permissions denied", ToastLength.Short).Show();

        //                }
        //            }
        //            break;
        //    }
        //    //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}
    }
}