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

namespace WlanAnalyzer.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Context context = null;
        private bool _isBusy;
        private static WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        private int _numberOfDetectedAccessPoints;
        public static ObservableCollection<WifiParameters> ListOfWifiNetworks;
        //public ObservableCollection<Person> PersonList { get; set; }
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
        public MainPageViewModel()
        {
            DetectedWifiNetworks = new ObservableCollection<WifiParameters>();
            ListOfWifiNetworks = new ObservableCollection<WifiParameters>();
            context = Android.App.Application.Context;
            StartScanningCommand = new Command(GetWifiNetworks);
            ClearCommand = new Command(ClearWifiNetworksCollection);
            WriteToJSONFileCommand = new Command(Serialization);
                                   

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
                    DetectedWifiNetworks = ListOfWifiNetworks;
                    CollectionofNetworksArrived.Reset();
                }
                IsBusy = false;
                NumberOfDetectedAccessPoints = DetectedWifiNetworks.Count;
            });

        }
        private void ClearWifiNetworksCollection()
        {
            DetectedWifiNetworks.Clear();
            ListOfWifiNetworks.Clear();
            NumberOfDetectedAccessPoints = 0;
        }
        private string ToJson()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<WifiParameters>));
                serializer.WriteObject(stream, DetectedWifiNetworks);
                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        private ObservableCollection<WifiParameters> FromJson(string filePath)
        {
            Stream stream = null;
            stream = File.Open(filePath, FileMode.Open);
            //StreamWriter writer = new StreamWriter(stream);
            //writer.Write(json);
            //writer.Flush();
            //stream.Position = 0;
      
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(ObservableCollection<WifiParameters>));
                    ObservableCollection<WifiParameters> DeserializedCollectionOfWifiNetworks = (ObservableCollection<WifiParameters>)deserializer.ReadObject(stream);

            stream.Dispose();
            return DeserializedCollectionOfWifiNetworks;

        }
        private void Serialization()
        {
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            var filePath = Path.Combine(sdCardPath, "WifiParameters.json");
            

                
            //string s1 = Encoding.UTF8.GetString(stream.ToArray());

            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.Write(ToJson());
                    writer.Dispose();
                }
            }
            else
            {
                foreach (var wifiNetwork in FromJson(filePath))
                {
                    DetectedWifiNetworks.Add(wifiNetwork);
                }
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {

                    //FileStream stream1 = new FileStream(filePath, FileMode.OpenOrCreate);
                    //DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(ObservableCollection<WifiParameters>));
                    //stream1.Position = 0;
                    //ObservableCollection<WifiParameters> DeserializedCollectionOfWifiNetworks = (ObservableCollection<WifiParameters>)deserializer.ReadObject(stream1);

                    
                    //var convertedJson = new DataContractJsonSerializer(typeof(ObservableCollection<WifiParameters>));
                    //deserializer.WriteObject(stream, DeserializedCollectionOfWifiNetworks);
                    //string s2 = Encoding.UTF8.GetString(stream.ToArray());
                    writer.Write(ToJson());
                    writer.Dispose();
                }
            }
        }
        private void WriteTextFile()
        {
            


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
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
