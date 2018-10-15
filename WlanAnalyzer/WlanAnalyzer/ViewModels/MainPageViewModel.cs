using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Net.Wifi;
using WlanAnalyzer.Models;
using Xamarin.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;
using Android.Widget;
using Plugin.Geolocator;
using System.Linq;

namespace WlanAnalyzer.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private INavigation _navigation;
        private Context context = null;
        private bool _isBusy;
        private static WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        private int _numberOfDetectedAccessPoints;
        private string _fileName;
        private double _refreshTime;
        private Timer timer;
        private string _currentWifiNetworkName;
        private int _currentWifiNetworkIP;
        private string _currentWifiNetworkIPText;
        private int _currentWifiNetworkSpeed;
        public static ObservableCollection<WifiParameters> ListOfWifiNetworks;
        //public ObservableCollection<WifiParameters> ListOfWifiNetworks1 { get; set; }
        private List<double> _refreshTimeList;
        private ObservableCollection<WifiParameters> _detectedWifiNetworks;
        public static AutoResetEvent CollectionofNetworksArrived = new AutoResetEvent(false);
        public static AutoResetEvent SaveToDatabaseAuto = new AutoResetEvent(false);
       // private WifiParametersJSON _wifiParametersJSON =  new WifiParametersJSON() { CollectionOfSavedWifiNetworks = DetectedWifiNetworks };
        public ObservableCollection<WifiParameters> DetectedWifiNetworks
        {
            get
            {
                return _detectedWifiNetworks;
            }
            set
            {
                _detectedWifiNetworks = value;
               RaisePropertyChanged("DetectedWifiNetworks");
            }
        }
        public Command StartScanningCommand { get; set; }
        public Command StopScanningCommand { get; set; }
        public Command ClearCommand { get; set; }
        public Command WriteToJSONFileCommand { get; set; }
        public Command DatabaseToolbarCommand { get; set; }
        public Command AnalyzeToolbarCommand { get; set; }
        public Command ChartsPageToolbarCommand { get; set; }
        public Command SaveListToDatabaseCommand { get; set; }
        public Command SaveListToDatabaseAutoCommand { get; set; }
        public Command AddSelectedWifiNetworkToDataBaseCommand { get; set; }
        public Command SaveFileToDatabaseCommand { get; set; }

        public static double Latitude { get; set; }
        public static double Longitude { get; set; }

        public List<double> RefreshTimeList
        {
            get
            {
                return _refreshTimeList;
            }
            set
            {
                _refreshTimeList = value;
                RaisePropertyChanged(nameof(RefreshTimeList));
            }
        }
        public double RefreshTime
        {
            get
            {
                return _refreshTime;
            }
            set
            {
                if(!IsScanning)
                {
                    _refreshTime = value;
                    RaisePropertyChanged(nameof(RefreshTime));
                }
                else
                    Toast.MakeText(Android.App.Application.Context, "You have to stop scanning first!", ToastLength.Short).Show();
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }
        public int NumberOfDetectedAccessPoints
        {
            get
            {
                return _numberOfDetectedAccessPoints;
            }
            set
            {
                _numberOfDetectedAccessPoints = value;
                RaisePropertyChanged("NumberOfDetectedAccessPoints");
                RaisePropertyChanged("NumberOfDetectedAccessPointsText");
            }
        }
        public string NumberOfDetectedAccessPointsText
        {
            get
            {
                return ($"Number of detected access points: {NumberOfDetectedAccessPoints}");
            }
        }
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(nameof(FileName));
            }
        }
        public string CurrentWifiNetworkName
        {
            get
            {
                return _currentWifiNetworkName;
            }
            set
            {
                _currentWifiNetworkName = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkName));
            }
        }
        public string CurrentWifiNetworkIPText
        {
            get
            {
                return _currentWifiNetworkIPText;
            }
            set
            {
                _currentWifiNetworkIPText = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkIPText));
            }
        }
        public int CurrentWifiNetworkIP
        {
            get
            {
                return _currentWifiNetworkIP;
            }
            set
            {
                _currentWifiNetworkIP = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkIP));
            }
        }
        public int CurrentWifiNetworkSpeed
        {
            get
            {
                return _currentWifiNetworkSpeed;
            }
            set
            {
                _currentWifiNetworkSpeed = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkSpeed));
            }
        }
        public WifiParameters SelectedWifiNetwork { get; set; }
        public bool IsScanning { get; set; }
        public bool AutoSaveToDatabase { get; set; }

        public MainPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            RefreshTimeList = new List<double>();
            DetectedWifiNetworks = new ObservableCollection<WifiParameters>();
            ListOfWifiNetworks = new ObservableCollection<WifiParameters>();
            CurrentWifiNetworkName = "-";
            CurrentWifiNetworkSpeed = 0;
            CurrentWifiNetworkIPText = "-";
            //RefreshTime = 15;
            FillRefreshTimeList();
            //timer = new Timer();

            context = Android.App.Application.Context;
            StartScanningCommand = new Command(GetWifiNetworks);
            StopScanningCommand = new Command(StopScanning);
            ClearCommand = new Command(ClearWifiNetworksCollection);
            WriteToJSONFileCommand = new Command(Serialization);
            DatabaseToolbarCommand = new Command(async () => await OpenDatabase());
            AnalyzeToolbarCommand = new Command(async () => await OpenAnalyze());
            ChartsPageToolbarCommand = new Command(async () => await OpenChartsPage());
            SaveListToDatabaseCommand = new Command(async () => await SaveListToDatabase());
            SaveListToDatabaseAutoCommand = new Command(async () => await SaveListToDatabaseAuto());
            AddSelectedWifiNetworkToDataBaseCommand = new Command(async () => await AddSelectedWifiNetworkToDataBase());
            SaveFileToDatabaseCommand = new Command(async () => await SaveFileToDatabase());
        }


        public void GetWifiNetworks()
        {
            if (!IsScanning)
            {
                if(RefreshTime < 10)
                {
                    Toast.MakeText(Android.App.Application.Context, "You have to set refresh time first!", ToastLength.Short).Show();
                }
                else
                {
                    IsScanning = true;
                    Toast.MakeText(Android.App.Application.Context, "Scanning has been started!", ToastLength.Short).Show();

                    var locator = CrossGeolocator.Current;           
                    locator.DesiredAccuracy = 10;

                    var startTimeSpan = TimeSpan.Zero;
                    var periodTimeSpan = TimeSpan.FromSeconds(RefreshTime);

                    timer = new Timer(async (e) =>
                    {
                        //DetectedWifiNetworks.Clear();
                        ListOfWifiNetworks.Clear();
                        //NumberOfDetectedAccessPoints = 0;
                        wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
                        await locator.StartListeningAsync(TimeSpan.FromSeconds(1), 0.01, false, null);
                        //locator.DesiredAccuracy = 10;
                        var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);

                        if(wifiManager.ConnectionInfo.NetworkId != -1 && wifiManager.ConnectionInfo.SSID != "UNKNOWNSSID") {
                            CurrentWifiNetworkName = wifiManager.ConnectionInfo.SSID;
                            CurrentWifiNetworkIP = wifiManager.ConnectionInfo.IpAddress;
                            CurrentWifiNetworkIPText = Android.Text.Format.Formatter.FormatIpAddress(CurrentWifiNetworkIP);
                            CurrentWifiNetworkSpeed = wifiManager.ConnectionInfo.LinkSpeed;
                        }
                        else {
                            CurrentWifiNetworkName = "-";
                            CurrentWifiNetworkIPText = "-";
                            CurrentWifiNetworkSpeed = 0;
                        }

                        Latitude = position.Latitude;
                        Longitude = position.Longitude;

                        wifiReceiver = new WifiReceiver();
                        context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
                        IsBusy = true;

                        wifiManager.StartScan();
                        //Thread.Sleep(1000);
                        CollectionofNetworksArrived.WaitOne();
                        context.UnregisterReceiver(wifiReceiver);
                        if (ListOfWifiNetworks.Count > 0)
                        {
                            var newWifiNetworks = ListOfWifiNetworks.Where(x => !DetectedWifiNetworks.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID));
                            if (newWifiNetworks != null)
                            {
                                foreach (var wifiNewtork in newWifiNetworks.ToList())
                                {
                                    DetectedWifiNetworks.Add(wifiNewtork);
                                }
                            }

                            foreach (var wifiNetwork in ListOfWifiNetworks)
                            {
                                var found = DetectedWifiNetworks.FirstOrDefault(x => x.SSID == wifiNetwork.SSID || x.BSSID == wifiNetwork.BSSID);
                                if (found != null)
                                {
                                    int i = DetectedWifiNetworks.IndexOf(found);
                                    DetectedWifiNetworks[i] = wifiNetwork;
                                }
                            }

                            var wifiNetworksToDelete = DetectedWifiNetworks.Where(x => !ListOfWifiNetworks.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID));
                            if (wifiNetworksToDelete != null)
                            {
                                foreach (var wifiNetwork in wifiNetworksToDelete.ToList())
                                {
                                    DetectedWifiNetworks.Remove(wifiNetwork);
                                }
                            }
                        }
                        CollectionofNetworksArrived.Reset();
                        IsBusy = false;
                        if (AutoSaveToDatabase)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await SaveListToDatabase();
                            });
                        }
                        NumberOfDetectedAccessPoints = DetectedWifiNetworks.Count;
                        await locator.StopListeningAsync();
          
                    }, null, startTimeSpan, periodTimeSpan);
                }              
            }
            else
                Toast.MakeText(Android.App.Application.Context, "Scanning has already started!", ToastLength.Short).Show();
        }
        
        private void ClearWifiNetworksCollection()
        {
            if (NumberOfDetectedAccessPoints != 0)
            {
                DetectedWifiNetworks.Clear();
                ListOfWifiNetworks.Clear();
                NumberOfDetectedAccessPoints = 0;
                CurrentWifiNetworkName = "-";
                CurrentWifiNetworkSpeed = 0;
                CurrentWifiNetworkIP = 0;
                CurrentWifiNetworkIPText = "-";
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been cleared successfully.", ToastLength.Short).Show();
            }
        }
        //private void ToJson(string filePath)
        //{
        //    using (StreamWriter file = File.CreateText(filePath))
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        serializer.Serialize(file, ListOfWifiNetworks);
        //        file.Dispose();
        //    }
        //    if (NumberOfDetectedAccessPoints != 0)
        //        Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been saved to file successfully.", ToastLength.Short).Show();
        //}
        //private ObservableCollection<WifiParameters> FromJson(string filePath)
        //{
        //    using (StreamReader streamReader = File.OpenText(filePath))
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        ObservableCollection<WifiParameters> DeserializedCollectionOfWifiNetworks = (ObservableCollection<WifiParameters>)serializer.Deserialize(streamReader, typeof(ObservableCollection<WifiParameters>));
        //        streamReader.Dispose();
        //        return DeserializedCollectionOfWifiNetworks;
        //    }
        //}
        private void Serialization()
        {
            if (NumberOfDetectedAccessPoints == 0) {
                Toast.MakeText(Android.App.Application.Context, "There is nothing to be saved!", ToastLength.Short).Show();
                return;
            }

            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            //var sdCardPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(sdCardPath, FileName + ".json");

            if (!File.Exists(filePath))
            {
                WifiParametersJSON.ToJson(filePath, ListOfWifiNetworks);
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been saved to file successfully.", ToastLength.Short).Show();
            }
            else
            {
                foreach (var wifiNetwork in WifiParametersJSON.FromJson(filePath))
                {
                    ListOfWifiNetworks.Add(wifiNetwork);
                }
                WifiParametersJSON.ToJson(filePath, ListOfWifiNetworks);
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been saved to file successfully.", ToastLength.Short).Show();
            }
            ListOfWifiNetworks.Clear();
        }
        private async Task SaveFileToDatabase()
        {
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            string filePath = Path.Combine(sdCardPath, FileName + ".json");

            if (File.Exists(filePath))
            {
                foreach (var wifiNetwork in WifiParametersJSON.FromJson(filePath))
                {
                    ListOfWifiNetworks.Add(wifiNetwork);
                }
                await App.Database.SaveCollectionOfWifiParameters(ListOfWifiNetworks);
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks from files has been added to database successfully.", ToastLength.Short).Show();
            }
            else
                Toast.MakeText(Android.App.Application.Context, "File with such name does not exist!", ToastLength.Short).Show();
        }
        private async Task OpenDatabase()
        {
             await _navigation.PushAsync(new WifiParametersDataBaseListPage());
        }
        private async Task OpenAnalyze()
        {
             await _navigation.PushAsync(new StatisticalAnalyzePage());
        }
        private async Task OpenChartsPage()
        {
            await _navigation.PushAsync(new ChartsPage());
        }
        private async Task SaveListToDatabase()
        {
            if (NumberOfDetectedAccessPoints == 0) {
                Toast.MakeText(Android.App.Application.Context, "There is nothing to be added!", ToastLength.Short).Show();
                return;
            }

            await App.Database.SaveCollectionOfWifiParameters(DetectedWifiNetworks);
            Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been added to database successfully.", ToastLength.Short).Show();
        }
        private async Task SaveListToDatabaseAuto()
        {
            AutoSaveToDatabase = true;
            await Task.Delay(1);
            Toast.MakeText(Android.App.Application.Context, "Autosave to database is turned on!", ToastLength.Short).Show();
        }
            
        private async Task AddSelectedWifiNetworkToDataBase()
        {
            if(SelectedWifiNetwork != null) {
                if (SelectedWifiNetwork.WifiID == 0)
                {
                    await App.Database.SaveWifiParametersAsync(SelectedWifiNetwork);
                    Toast.MakeText(Android.App.Application.Context, "Selected WifiNetwork has been added to database successfully.", ToastLength.Short).Show();
                }
                else
                {
                    await App.Database.UpdateWifiParametersAsync(SelectedWifiNetwork);
                    Toast.MakeText(Android.App.Application.Context, "Selected WifiNetwork has been already added to database!", ToastLength.Short).Show();
                }
            }
        }
        private async void GetCurrentLocation()
        {
            var locator = CrossGeolocator.Current;
            await locator.StartListeningAsync(TimeSpan.FromSeconds(1), 1, false, null);
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
            //locator.DesiredAccuracy = 50;
            //var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
            //var position = await locator.GetPositionAsync(10000);

            Latitude = position.Latitude;
            Longitude = position.Longitude;
        }
        private void StopScanning()
        {
            if(timer != null) {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                IsScanning = false;
                AutoSaveToDatabase = true;
                Toast.MakeText(Android.App.Application.Context, "Scanning has been stopped!", ToastLength.Short).Show();
            }

            else
                Toast.MakeText(Android.App.Application.Context, "You have to start scanning first!", ToastLength.Short).Show();

        }
        private void FillRefreshTimeList()
        {
            RefreshTimeList.AddRange(new List<double>
            {
                10,15,20,30,60
            });
        }
        class WifiReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> scanWifiNetworks = wifiManager.ScanResults;
                foreach (ScanResult wifiNetwork in scanWifiNetworks)
                {
                    ListOfWifiNetworks.Add(new WifiParameters() { SSID = wifiNetwork.Ssid, BSSID = wifiNetwork.Bssid, Frequency = wifiNetwork.Frequency, Level = wifiNetwork.Level, Channel = WifiParameters.GetChannel(wifiNetwork.Frequency), Latitude = Latitude, Longitude = Longitude });
                   // ListOfWifiNetworks.Add(new WifiParameters(wifiNetwork.Ssid, wifiNetwork.Bssid, wifiNetwork.Frequency, wifiNetwork.Level, WifiParameters.GetChannel(wifiNetwork.Frequency), wifiNetwork.Timestamp));
                }
                CollectionofNetworksArrived.Set(); 
            }

        }
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }

            //public Person(string name, int age)
            //{
            //    Name = name;
            //    Age = age;
            //}
        }
        private string _name = "Adam";

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
                RaisePropertyChanged("DisplayMessage");
            }
        }

        public string DisplayMessage
        {
            get { return $"Your new friend is named {Name}"; }
        }
    }
}
