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

namespace WlanAnalyzer.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Context context = null;
        private bool _isBusy;
        private static WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        private int _numberOfDetectedAccessPoints;
        public static ObservableCollection<Wifi> ListOfWifiNetworks;
        //public ObservableCollection<Person> PersonList { get; set; }
        private ObservableCollection<Wifi> _detectedWifiNetworks;
        public static AutoResetEvent CollectionofNetworksArrived = new AutoResetEvent(false);
        public ObservableCollection<Wifi> DetectedWifiNetworks
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
                return ($"Number of detected access points {NumberOfDetectedAccessPoints}");
            }
        }
        public MainPageViewModel()
        {
            DetectedWifiNetworks = new ObservableCollection<Wifi>();
            ListOfWifiNetworks = new ObservableCollection<Wifi>();
            context = Android.App.Application.Context;
            StartScanningCommand = new Command(GetWifiNetworks);
            ClearCommand = new Command(ClearWifiNetworksCollection);
                                                  

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
        class WifiReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> scanWifiNetworks = wifiManager.ScanResults;
                foreach (ScanResult wifiNetwork in scanWifiNetworks)
                {
                    ListOfWifiNetworks.Add(new Wifi(wifiNetwork.Ssid, wifiNetwork.Bssid, wifiNetwork.Frequency, wifiNetwork.Level, wifiNetwork.ChannelWidth, wifiNetwork.CenterFreq0, wifiNetwork.CenterFreq1, wifiNetwork.Timestamp));
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
