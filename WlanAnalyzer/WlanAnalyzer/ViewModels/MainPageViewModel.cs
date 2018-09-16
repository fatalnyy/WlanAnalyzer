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
        public static ObservableCollection<WifiParameters> ListOfWifiNetworks;
        //public ObservableCollection<WifiParameters> ListOfWifiNetworks1 { get; set; }
        private ObservableCollection<WifiParameters> _detectedWifiNetworks;
        public static AutoResetEvent CollectionofNetworksArrived = new AutoResetEvent(false);
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
        public Command ClearCommand { get; set; }
        public Command WriteToJSONFileCommand { get; set; }
        public Command DatabaseToolbarCommand { get; set; }
        public Command SaveListToDatabaseCommand { get; set; }
        public Command AddSelectedWifiNetworkToDataBaseCommand { get; set; }
        public Command SaveFileToDatabaseCommand { get; set; }

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
            }
        }
        public WifiParameters SelectedWifiNetwork { get; set; }

        public MainPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            DetectedWifiNetworks = new ObservableCollection<WifiParameters>();
            ListOfWifiNetworks = new ObservableCollection<WifiParameters>();

            context = Android.App.Application.Context;
            StartScanningCommand = new Command(GetWifiNetworks);
            ClearCommand = new Command(ClearWifiNetworksCollection);
            WriteToJSONFileCommand = new Command(Serialization);
            DatabaseToolbarCommand = new Command(async () => await OpenDatabase());
            SaveListToDatabaseCommand = new Command(async () => await SaveListToDatabase());
            AddSelectedWifiNetworkToDataBaseCommand = new Command(async () => await AddSelectedWifiNetworkToDataBase());
            SaveFileToDatabaseCommand = new Command(async () => await SaveFileToDatabase());
            //for (int i = 0; i < 10; i++)
            //{
            //    WifiNetworks.Add(new Wifi() { SSID = "fsagsa", BSSID = "gasgdsagsa", Frequency = 231, Level = 12 });
            //}


            //GetWifiNetworks();
            //PersonList = new ObservableCollection<Person>();
            //for (int i = 0; i < 10; i++)
            //{
            //    PersonList.Add(new Person() { Name = "adam", Age = 12 });
            //}


        }
        //public string Name { get; set; }
        //public int Age { get; set; }

        public void GetWifiNetworks()
        {

            Task.Run(() =>
            {
                ClearWifiNetworksCollection();
                wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);

                wifiReceiver = new WifiReceiver();
                context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
                IsBusy = true;
                wifiManager.StartScan();
                Thread.Sleep(3000);
                CollectionofNetworksArrived.WaitOne();
                context.UnregisterReceiver(wifiReceiver);
                if (ListOfWifiNetworks.Count > 0)
                {
                    //DetectedWifiNetworks = ListOfWifiNetworks;
                    foreach(var wifiNetwork in ListOfWifiNetworks)
                    {
                        DetectedWifiNetworks.Add(wifiNetwork);
                    }
                    CollectionofNetworksArrived.Reset();
                }
                IsBusy = false;
                NumberOfDetectedAccessPoints = DetectedWifiNetworks.Count;
            });

        }
        private void ClearWifiNetworksCollection()
        {
            if(NumberOfDetectedAccessPoints != 0) {
                DetectedWifiNetworks.Clear();
                ListOfWifiNetworks.Clear();
                NumberOfDetectedAccessPoints = 0;
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been cleared successfully.", ToastLength.Short).Show();
            }
        }
        private void ToJson(string filePath)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, ListOfWifiNetworks);
                file.Dispose();
            }
            if (NumberOfDetectedAccessPoints != 0)
                Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been saved to file successfully.", ToastLength.Short).Show();
        }
        private ObservableCollection<WifiParameters> FromJson(string filePath)
        {
            using (StreamReader streamReader = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                ObservableCollection<WifiParameters> DeserializedCollectionOfWifiNetworks = (ObservableCollection<WifiParameters>)serializer.Deserialize(streamReader, typeof(ObservableCollection<WifiParameters>));
                streamReader.Dispose();
                return DeserializedCollectionOfWifiNetworks;
            }
        }
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
                ToJson(filePath);
            }
            else
            {
                foreach (var wifiNetwork in FromJson(filePath))
                {
                    ListOfWifiNetworks.Add(wifiNetwork);
                }
                ToJson(filePath);
            }
        }
        private async Task SaveFileToDatabase()
        {
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            string filePath = Path.Combine(sdCardPath, FileName + ".json");

            if (File.Exists(filePath))
            {
                foreach (var wifiNetwork in FromJson(filePath))
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
        private async Task SaveListToDatabase()
        {
            if (NumberOfDetectedAccessPoints == 0) {
                Toast.MakeText(Android.App.Application.Context, "There is nothing to be added!", ToastLength.Short).Show();
                return;
            }

            await App.Database.SaveCollectionOfWifiParameters(DetectedWifiNetworks);
            Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been added to database successfully.", ToastLength.Short).Show();
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
        class WifiReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> scanWifiNetworks = wifiManager.ScanResults;
                foreach (ScanResult wifiNetwork in scanWifiNetworks)
                {
                    ListOfWifiNetworks.Add(new WifiParameters() { SSID = wifiNetwork.Ssid, BSSID = wifiNetwork.Bssid, Frequency = wifiNetwork.Frequency, Level = wifiNetwork.Level, Channel = WifiParameters.GetChannel(wifiNetwork.Frequency), TimeStamp = wifiNetwork.Timestamp });
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
