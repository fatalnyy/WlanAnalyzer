using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android;
using System.Threading.Tasks;
using Com.Syncfusion.Charts;

namespace WlanAnalyzer.Droid
{
    [Activity(Label = "WlanAnalyzer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            await TryToGetPermissions();


            Xamarin.Forms.Forms.ViewInitialized += Forms_ViewInitialized;


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
                            Manifest.Permission.AccessCoarseLocation,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessWifiState,
                            Manifest.Permission.ChangeWifiState,
                            Manifest.Permission.ChangeWifiMulticastState,
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage
        };
        async Task GetPermissionsAsync()
        {
            await Task.Delay(1);
            foreach(string permission in PermissionsGroupLocation)
            {
                if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
                {
                    Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                    continue;
                }

                if (ShouldShowRequestPermissionRationale(permission))
                {
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
        void Forms_ViewInitialized(object sender, Xamarin.Forms.ViewInitializedEventArgs e)
        {
            if (e.NativeView is SfChart)
            {
                SfChart chart = e.NativeView as SfChart;
                chart.Touch += chart_Touch;
            }
        }

        void chart_Touch(object sender, View.TouchEventArgs e)
        {
            (sender as SfChart).Parent.RequestDisallowInterceptTouchEvent(true);
            e.Handled = false;
        }
    }
}