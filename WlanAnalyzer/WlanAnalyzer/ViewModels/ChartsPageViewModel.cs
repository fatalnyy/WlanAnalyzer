using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Widget;
using Syncfusion.SfChart.XForms;
using WlanAnalyzer.Models;
using Xamarin.Forms;

namespace WlanAnalyzer.ViewModels
{
    public class ChartsPageViewModel : BaseViewModel
    {
        private ChartSeriesCollection _seriesCollection;
        private ObservableCollection<WifiParameters> _collectionOfWifiNetworksChart;
        private ObservableCollection<WifiParameters> _collectionOfWifiNetworksChart1;
        private List<WifiParameters> _testCollection;
        private double _averageLevel;
        public ChartsPageViewModel()
        {
            TestCollection = new List<WifiParameters>();
            SeriesCollection = new ChartSeriesCollection();
            _collectionOfWifiNetworksChart = new ObservableCollection<WifiParameters>();
            _collectionOfWifiNetworksChart1 = new ObservableCollection<WifiParameters>();
            SortCollectionByLevelDescendingCommand = new Command(async () => await SortCollectionByLevelDescending());
            SortCollectionByLevelAscendingCommand = new Command(async () => await SortCollectionByLevelAscending());
            //CollectionOfWifiNetworksChart = MainPageViewModel.ListOfWifiNetworks;
            ScanCount = 0;
            LoadData();

        }
        public Command SortCollectionByLevelDescendingCommand { get; set; }
        public Command SortCollectionByLevelAscendingCommand { get; set; }
        public double AverageLevel
        {
            get
            {
                return _averageLevel;
            }
            set
            {
                _averageLevel = value;
                RaisePropertyChanged(nameof(AverageLevel));
            }
        }
        public ObservableCollection<WifiParameters> CollectionOfWifiNetworksChart
        {
            get
            {
                return _collectionOfWifiNetworksChart;
            }
            set
            {
                _collectionOfWifiNetworksChart = value;
                RaisePropertyChanged(nameof(CollectionOfWifiNetworksChart));
            }
        }
        public ObservableCollection<WifiParameters> CollectionOfWifiNetworksChart1
        {
            get
            {
                return _collectionOfWifiNetworksChart1;
            }
            set
            {
                _collectionOfWifiNetworksChart1 = value;
                RaisePropertyChanged(nameof(CollectionOfWifiNetworksChart1));
            }
        }
        public List<WifiParameters> TestCollection
        {
            get
            {
                return _testCollection;
            }
            set
            {
                _testCollection = value;
                RaisePropertyChanged(nameof(TestCollection));
            }
        }
        public ChartSeriesCollection SeriesCollection
        {
            get
            {
                return _seriesCollection;
            }
            set
            {
                _seriesCollection = value;
                RaisePropertyChanged(nameof(SeriesCollection));
            }
        }
        public int ScanCount { get; set; }

        private void LoadData()
        {
            //await Task.Delay(200);
          

            Device.StartTimer(new TimeSpan(0, 0, 0, 10), () =>
            {
                //var newWifiNetworks = MainPageViewModel.ListOfWifiNetworks.Where(x => !CollectionOfWifiNetworksChart.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID)).ToList();
                //if (newWifiNetworks != null)
                //{
                //    foreach (var wifiNetwork in newWifiNetworks)
                //    {
                //        CollectionOfWifiNetworksChart.Add(wifiNetwork);
                //        //TestCollection = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).ToList();
                //        //foreach (var wifi in TestCollection)
                //        //{
                //        //    CollectionOfWifiNetworksChart1.Add(wifi);
                //        //}
                //        //SeriesCollection.Add(new LineSeries() { ItemsSource = CollectionOfWifiNetworksChart, XBindingPath = "SSID", YBindingPath = "Level", StrokeWidth = 4, Label = wifiNetwork.SSID });
                //    }
                //}
                var newWifiNetworks = MainPageViewModel.ListOfWifiNetworks.Where(x => !CollectionOfWifiNetworksChart.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID)).ToList();
                foreach (var wifiNetwork in newWifiNetworks)
                {
                    CollectionOfWifiNetworksChart.Insert(0, wifiNetwork);
                    //AverageLevel = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).Average(y => y.Level);
                    //TestCollection = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).ToList();
                    //foreach (var wifi in TestCollection)
                    //{
                    //    CollectionOfWifiNetworksChart1.Add(wifi);
                    //}

                    //var found = CollectionOfWifiNetworksChart.FirstOrDefault(x => x.SSID == wifiNetwork.SSID || x.BSSID == wifiNetwork.BSSID);
                    //if (found != null)
                    //{
                    //    int i = CollectionOfWifiNetworksChart.IndexOf(found);
                    //    CollectionOfWifiNetworksChart[i] = wifiNetwork;
                    //    //SeriesCollection.Add(new LineSeries() { ItemsSource = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID), XBindingPath = "Channel", YBindingPath = "Level" });
                    //}
                }
                foreach (var wifiNetwork in MainPageViewModel.ListOfWifiNetworks)
                {
                    var found = CollectionOfWifiNetworksChart.FirstOrDefault(x => x.SSID == wifiNetwork.SSID || x.BSSID == wifiNetwork.BSSID);
                    if (found != null)
                    {
                        int i = CollectionOfWifiNetworksChart.IndexOf(found);
                        CollectionOfWifiNetworksChart[i] = wifiNetwork;
                    }
                }
                var wifiNetworksToDelete = CollectionOfWifiNetworksChart.Where(x => !MainPageViewModel.ListOfWifiNetworks.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID));
                if (wifiNetworksToDelete != null)
                {
                    foreach (var wifiNetwork in wifiNetworksToDelete.ToList())
                    {
                        CollectionOfWifiNetworksChart.Remove(wifiNetwork);
                    }
                }
                
                return true;
            });
        }
        private async Task SortCollectionByLevelDescending()
        {
            if (CollectionOfWifiNetworksChart.Count != 0)
            {
                await Task.Delay(500);
                CollectionOfWifiNetworksChart = new ObservableCollection<WifiParameters>(CollectionOfWifiNetworksChart.OrderByDescending(x => x.Level).ToList());
            }
        }
        private async Task SortCollectionByLevelAscending()
        {
            if (CollectionOfWifiNetworksChart.Count != 0)
            {
                await Task.Delay(500);
                CollectionOfWifiNetworksChart = new ObservableCollection<WifiParameters>(CollectionOfWifiNetworksChart.OrderBy(x => x.Level).ToList());
            }
        }
    }
}
