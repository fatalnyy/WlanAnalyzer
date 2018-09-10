using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Net.Wifi;
using WlanAnalyzer.Models;

namespace WlanAnalyzer.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Context context = null;
        private static WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        //public static List<string> WifiNetworks;
        public ObservableCollection<Person> PersonList { get; set; }
        public static ObservableCollection<Wifi> WifiNetworks { get; set; }

        public MainPageViewModel()
        {
            context = Android.App.Application.Context;
            WifiNetworks = new ObservableCollection<Wifi>();
            GetWifiNetworks();
            //PersonList = new ObservableCollection<Person>();
            //for (int i = 0; i < 10; i++)
            //{
            //    PersonList.Add(new Person() { Name = "adam", Age = 12 });
            //}


            //PersonList = new ObservableCollection<Person>
            //{
            //    new Person(){Name = "Adam", Age =22},
            //    new Person(){Name = "Gjj", Age =21},
            //    new Person(){Name = "Cwel", Age =25},
            //};
        }
        //public string Name { get; set; }
        //public int Age { get; set; }
        public void GetWifiNetworks()
        {
            

            wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);

            wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            wifiManager.StartScan();
        }

        class WifiReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> scanWifiNetworks = wifiManager.ScanResults;
                foreach (ScanResult wifiNetwork in scanWifiNetworks)
                {
                    WifiNetworks.Add(new Wifi() { SSID = wifiNetwork.Ssid, BSSID = wifiNetwork.Bssid, Frequency = wifiNetwork.Frequency, Level = wifiNetwork.Level });
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
