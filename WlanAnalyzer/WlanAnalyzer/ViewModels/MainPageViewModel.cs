using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public static List<Wifi> ListOfWifiNetworks;
        //public ObservableCollection<Person> PersonList { get; set; }
        private ObservableCollection<Wifi> _detectedWifiNetworks;
        public ObservableCollection<Wifi> DetectedWifiNetworks
        {
            get
            {
                return _detectedWifiNetworks;
            }
            set
            {
                _detectedWifiNetworks = value;
                OnPropertyChanged("DetectedWifiNetworks");
            }
        }     

        public Command StartScanningCommand { get; set; }
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        public MainPageViewModel()
        {
            DetectedWifiNetworks = new ObservableCollection<Wifi>();
            ListOfWifiNetworks = new List<Wifi>();
            context = Android.App.Application.Context;
            StartScanningCommand = new Command(async ()=> await GetWifiNetworks(),
                                                     () => !IsBusy);

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

        public async Task GetWifiNetworks()
        {

            wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);

            wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            wifiManager.StartScan();
            IsBusy = true;
            await Task.Delay(2500);
            IsBusy = false;
            if (ListOfWifiNetworks.Count > 0)
            {
                foreach (var wlan in ListOfWifiNetworks)
                {
                    DetectedWifiNetworks.Add(new Wifi() { SSID = wlan.SSID, BSSID = wlan.BSSID, Frequency = wlan.Frequency, Level = wlan.Level, TimeStamp = wlan.TimeStamp });
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
                    ListOfWifiNetworks.Add(new Wifi() { SSID = wifiNetwork.Ssid, BSSID = wifiNetwork.Bssid, Frequency = wifiNetwork.Frequency, Level = wifiNetwork.Level, TimeStamp = wifiNetwork.Timestamp });
                }
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
        //private string _name = "Adam";
        //public MainPageViewModel()
        //{

        //}

        //public string Name
        //{
        //    get { return _name; }
        //    set
        //    {
        //        _name = value;
        //        OnPropertyChanged("Name");
        //        OnPropertyChanged("DisplayMessage");
        //    }
        //}

        //public string DisplayMessage
        //{
        //    get { return $"Your new friend is named {Name}"; } 
        //}
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
