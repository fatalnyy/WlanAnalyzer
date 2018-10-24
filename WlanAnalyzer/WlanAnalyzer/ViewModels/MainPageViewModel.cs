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
using WlanAnalyzer.Views;

namespace WlanAnalyzer.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Fields
        public static ObservableCollection<WifiParameters> ListOfWifiNetworks;
        public static ObservableCollection<WifiParameters> ListOfWifiNetworksFromFile;
        private static WifiManager wifiManager;
        private int _numberOfDetectedAccessPoints;
        private int _refreshTime;
        private int _currentWifiNetworkIP;
        private int _currentWifiNetworkSpeed;
        private string _currentWifiNetworkName;
        private string _currentWifiNetworkIPText;
        private string _fileName;
        private bool _isBusy;
        private Context context = null;
        private Timer timer;
        private WifiReceiver wifiReceiver;
        private List<int> _refreshTimeList;
        private ObservableCollection<WifiParameters> _detectedWifiNetworks;
        #endregion

        #region Constructors
        private INavigation _navigation;
        public MainPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            context = Android.App.Application.Context;

            CurrentWifiNetworkName = "-";
            CurrentWifiNetworkSpeed = 0;
            CurrentWifiNetworkIPText = "-";
            RefreshTime = 10;

            RefreshTimeList = new List<int>();
            DetectedWifiNetworks = new ObservableCollection<WifiParameters>();
            ListOfWifiNetworks = new ObservableCollection<WifiParameters>();
            ListOfWifiNetworksFromFile = new ObservableCollection<WifiParameters>();
            FillRefreshTimeList();

            StartScanningCommand = new Command(GetWifiNetworks);
            StopScanningCommand = new Command(StopScanning);
            ClearCommand = new Command(ClearWifiNetworksCollection);
            WriteToJSONFileCommand = new Command(SaveListToFile);
            DatabaseToolbarCommand = new Command(async () => await OpenDatabase());
            AnalyzeToolbarCommand = new Command(async () => await OpenAnalyze());
            ChartsPageToolbarCommand = new Command(async () => await OpenChartsPage());
            SaveListToDatabaseCommand = new Command(async () => await SaveListToDatabase());
            SaveListToDatabaseAutoCommand = new Command(async () => await SaveListToDatabaseAuto());
            SaveListToFileAutoCommand = new Command(async () => await SaveListToFileAuto());
            AddSelectedWifiNetworkToDataBaseCommand = new Command(async () => await AddSelectedWifiNetworkToDataBase());
            SaveFileToDatabaseCommand = new Command(async () => await SaveFileToDatabase());
        }
        #endregion

        #region Events and Interfaces
        public static AutoResetEvent CollectionofNetworksArrived = new AutoResetEvent(false);
 
        #endregion

        #region Properties
        public Command StartScanningCommand { get; set; }
        public Command StopScanningCommand { get; set; }
        public Command ClearCommand { get; set; }
        public Command WriteToJSONFileCommand { get; set; }
        public Command DatabaseToolbarCommand { get; set; }
        public Command AnalyzeToolbarCommand { get; set; }
        public Command ChartsPageToolbarCommand { get; set; }
        public Command SaveListToDatabaseCommand { get; set; }
        public Command SaveListToDatabaseAutoCommand { get; set; }
        public Command SaveListToFileAutoCommand { get; set; }
        public Command AddSelectedWifiNetworkToDataBaseCommand { get; set; }
        public Command SaveFileToDatabaseCommand { get; set; }

        public static double Latitude { get; set; }
        public static double Longitude { get; set; }
        public bool IsScanning { get; set; }
        public bool AutoSaveToDatabase { get; set; }
        public bool AutoSaveToFile { get; set; }
        public WifiParameters SelectedWifiNetwork { get; set; }

        public ObservableCollection<WifiParameters> DetectedWifiNetworks
        {
            get {
                return _detectedWifiNetworks;
            }
            set {
                _detectedWifiNetworks = value;
                RaisePropertyChanged("DetectedWifiNetworks");
            }
        }
        public List<int> RefreshTimeList
        {
            get {
                return _refreshTimeList;
            }
            set {
                _refreshTimeList = value;
                RaisePropertyChanged(nameof(RefreshTimeList));
            }
        }
        public int RefreshTime
        {
            get {
                return _refreshTime;
            }
            set {
                if (!IsScanning)
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
            get {
                return _isBusy;
            }
            set {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }
        public int NumberOfDetectedAccessPoints
        {
            get {
                return _numberOfDetectedAccessPoints;
            }
            set {
                _numberOfDetectedAccessPoints = value;
                RaisePropertyChanged("NumberOfDetectedAccessPoints");
                RaisePropertyChanged("NumberOfDetectedAccessPointsText");
            }
        }
        public string NumberOfDetectedAccessPointsText
        {
            get {
                return ($"Number of detected access points: {NumberOfDetectedAccessPoints}");
            }
        }
        public string FileName
        {
            get { return _fileName; }
            set {
                _fileName = value;
                RaisePropertyChanged(nameof(FileName));
            }
        }
        public string CurrentWifiNetworkName
        {
            get {
                return _currentWifiNetworkName;
            }
            set {
                _currentWifiNetworkName = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkName));
            }
        }
        public string CurrentWifiNetworkIPText
        {
            get {
                return _currentWifiNetworkIPText;
            }
            set {
                _currentWifiNetworkIPText = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkIPText));
            }
        }
        public int CurrentWifiNetworkIP
        {
            get {
                return _currentWifiNetworkIP;
            }
            set {
                _currentWifiNetworkIP = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkIP));
            }
        }
        public int CurrentWifiNetworkSpeed
        {
            get {
                return _currentWifiNetworkSpeed;
            }
            set {
                _currentWifiNetworkSpeed = value;
                RaisePropertyChanged(nameof(CurrentWifiNetworkSpeed));
            }
        }
        #endregion

        #region Methods
        private void GetWifiNetworks()
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
                    locator.DesiredAccuracy = 5;

                    var startTimeSpan = TimeSpan.Zero;
                    var periodTimeSpan = TimeSpan.FromSeconds(RefreshTime);

                    timer = new Timer(async (e) =>
                    {
                        ListOfWifiNetworks.Clear();

                        wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
                        await locator.StartListeningAsync(TimeSpan.FromSeconds(1), 0.01, false, null);
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
                        CollectionofNetworksArrived.WaitOne();
                        context.UnregisterReceiver(wifiReceiver);
                        if (ListOfWifiNetworks.Count > 0)
                        {
                            DetectedWifiNetworks.Clear();
                            foreach (var item in ListOfWifiNetworks)
                            {
                                DetectedWifiNetworks.Add(item);
                            }
                        }
                        CollectionofNetworksArrived.Reset();
                        IsBusy = false;
                        if (AutoSaveToDatabase) {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await SaveListToDatabase();
                            });
                        }
                        if (AutoSaveToFile) {
                            Device.BeginInvokeOnMainThread( () =>
                            {
                                SaveListToFile();
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

        private void SaveListToFile()
        {
            if (NumberOfDetectedAccessPoints == 0) {
                Toast.MakeText(Android.App.Application.Context, "There is nothing to be saved!", ToastLength.Short).Show();
                AutoSaveToFile = false;
                return;
            }

            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            string filePath = Path.Combine(sdCardPath, FileName + ".json");

            foreach (var wifiNetwork in ListOfWifiNetworks) {
                ListOfWifiNetworksFromFile.Add(wifiNetwork);
            }

            if (!File.Exists(filePath)) {
                WifiParametersJSON.ToJson(filePath, ListOfWifiNetworks);
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been saved to file successfully.", ToastLength.Short).Show();
            }
            else {
                foreach (var wifiNetwork in WifiParametersJSON.FromJson(filePath)) {
                    ListOfWifiNetworksFromFile.Add(wifiNetwork);
                }
                WifiParametersJSON.ToJson(filePath, ListOfWifiNetworksFromFile);
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been saved to file successfully.", ToastLength.Short).Show();
            }
            ListOfWifiNetworksFromFile.Clear();
        }

        private void StopScanning()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                IsScanning = false;
                AutoSaveToDatabase = false;
                AutoSaveToFile = false;
                Toast.MakeText(Android.App.Application.Context, "Scanning has been stopped!", ToastLength.Short).Show();
            }

            else
                Toast.MakeText(Android.App.Application.Context, "You have to start scanning first!", ToastLength.Short).Show();

        }

        private void FillRefreshTimeList()
        {
            RefreshTimeList.AddRange(new List<int>
            {
                10,15,20,30,60
            });
        }

        private async Task SaveFileToDatabase()
        {
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            string filePath = Path.Combine(sdCardPath, FileName + ".json");

            foreach (var wifiNetwork in ListOfWifiNetworks) {
                ListOfWifiNetworksFromFile.Add(wifiNetwork);
            }

            if (File.Exists(filePath))
            {
                foreach (var wifiNetwork in WifiParametersJSON.FromJson(filePath)) {
                    ListOfWifiNetworksFromFile.Add(wifiNetwork);
                }
                await App.Database.SaveCollectionOfWifiParameters(ListOfWifiNetworksFromFile);
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks from file has been added to database successfully.", ToastLength.Short).Show();
            }
            else
                Toast.MakeText(Android.App.Application.Context, "File with such name does not exist!", ToastLength.Short).Show();
            ListOfWifiNetworksFromFile.Clear();
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
            if(DetectedWifiNetworks.Count != 0)
                await _navigation.PushAsync(new ChartsPage());
            else
                Toast.MakeText(Android.App.Application.Context, "You have to start scanning first!", ToastLength.Short).Show();
        }

        private async Task SaveListToDatabase()
        {
            if (NumberOfDetectedAccessPoints == 0) {
                Toast.MakeText(Android.App.Application.Context, "There is nothing to be added!", ToastLength.Short).Show();
                AutoSaveToDatabase = false;
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

        private async Task SaveListToFileAuto()
        {
            if(FileName != null) {
                AutoSaveToFile = true;
                await Task.Delay(1);
                Toast.MakeText(Android.App.Application.Context, "Autosave to file is turned on!", ToastLength.Short).Show();
            }
            else {
                Toast.MakeText(Android.App.Application.Context, "Enter file name first!", ToastLength.Short).Show();
            }
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
        #endregion

        #region Classes
        class WifiReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> scanWifiNetworks = wifiManager.ScanResults;
                foreach (ScanResult wifiNetwork in scanWifiNetworks)
                {
                    if (wifiNetwork.Ssid == "")
                        wifiNetwork.Ssid = "Unknown SSID";
                    ListOfWifiNetworks.Add(new WifiParameters() { SSID = wifiNetwork.Ssid, BSSID = wifiNetwork.Bssid, Frequency = wifiNetwork.Frequency, Level = wifiNetwork.Level, Channel = WifiParameters.GetChannel(wifiNetwork.Frequency), Latitude = Latitude, Longitude = Longitude });
                }
                CollectionofNetworksArrived.Set(); 
            }
        }
        #endregion
    }
}
