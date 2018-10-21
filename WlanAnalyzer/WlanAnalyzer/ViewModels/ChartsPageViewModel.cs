using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        private ObservableCollection<WifiParametersCollection> _collectionOfWifiNetworksChartAverage;
        private ObservableCollection<WifiParameters> _collectionOfWifiNetworksChartAverage1;
        private ObservableCollection<WifiParameters> _allWifiNetworksCollection;
        private List<WifiParameters> _allWifiNetworksCollection1;
        private double _averageLevel;
        private INavigation _navigation;
        public ChartsPageViewModel()
        {
            AllWifiNetworksCollection = new ObservableCollection<WifiParameters>();
            SeriesCollection = new ChartSeriesCollection();
            _collectionOfWifiNetworksChart = new ObservableCollection<WifiParameters>();
            _collectionOfWifiNetworksChartAverage = new ObservableCollection<WifiParametersCollection>();
            _collectionOfWifiNetworksChartAverage1 = new ObservableCollection<WifiParameters>();
            SortCollectionByLevelDescendingCommand = new Command(async () => await SortCollectionByLevelDescending());
            SortCollectionByLevelAscendingCommand = new Command(async () => await SortCollectionByLevelAscending());
            SwitchChartsCommand = new Command(async () => await SwitchCharts());
            //CollectionOfWifiNetworksChart = MainPageViewModel.ListOfWifiNetworks;
            ScanCount = 0;
            LoadData();

        }
        public Command SortCollectionByLevelDescendingCommand { get; set; }
        public Command SortCollectionByLevelAscendingCommand { get; set; }
        public Command SwitchChartsCommand { get; set; }

        public bool IsSwitched { get; set; }
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
            }
        }
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
        public ObservableCollection<WifiParametersCollection> CollectionOfWifiNetworksChartAverage
        {
            get
            {
                return _collectionOfWifiNetworksChartAverage;
            }
            set
            {
                _collectionOfWifiNetworksChartAverage = value;
                RaisePropertyChanged(nameof(CollectionOfWifiNetworksChartAverage));
            }
        }
        public ObservableCollection<WifiParameters> CollectionOfWifiNetworksChartAverage1
        {
            get
            {
                return _collectionOfWifiNetworksChartAverage1;
            }
            set
            {
                _collectionOfWifiNetworksChartAverage1 = value;
                RaisePropertyChanged(nameof(CollectionOfWifiNetworksChartAverage1));
            }
        }
        public ObservableCollection<WifiParameters> AllWifiNetworksCollection
        {
            get
            {
                return _allWifiNetworksCollection;
            }
            set
            {
                _allWifiNetworksCollection = value;
                RaisePropertyChanged(nameof(AllWifiNetworksCollection));
            }
        }
        public List<WifiParameters> AllWifiNetworksCollection1
        {
            get
            {
                return _allWifiNetworksCollection1;
            }
            set
            {
                _allWifiNetworksCollection1 = value;
                RaisePropertyChanged(nameof(AllWifiNetworksCollection1));
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
        private Timer _chartTimer;
        private bool _isBusy;

        private void LoadData()
        {
            //await Task.Delay(200);
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(10);


            //_chartTimer = new Timer(async (e) =>
            //{
            //    await Task.Delay(1);
            //    //var newWifiNetworks = MainPageViewModel.ListOfWifiNetworks.Where(x => !CollectionOfWifiNetworksChart.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID)).ToList();
            //    //if (newWifiNetworks != null)
            //    //{
            //    //    foreach (var wifiNetwork in newWifiNetworks)
            //    //    {
            //    //        CollectionOfWifiNetworksChart.Add(wifiNetwork);
            //    //        //TestCollection = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).ToList();
            //    //        //foreach (var wifi in TestCollection)
            //    //        //{
            //    //        //    CollectionOfWifiNetworksChart1.Add(wifi);
            //    //        //}
            //    //        //SeriesCollection.Add(new LineSeries() { ItemsSource = CollectionOfWifiNetworksChart, XBindingPath = "SSID", YBindingPath = "Level", StrokeWidth = 4, Label = wifiNetwork.SSID });
            //    //    }
            //    //}

            //    foreach (var wifiNetwork in MainPageViewModel.ListOfWifiNetworks)
            //    {
            //        AllWifiNetworksCollection.Add(wifiNetwork);

            //        var found = CollectionOfWifiNetworksChart.FirstOrDefault(x => x.SSID == wifiNetwork.SSID || x.BSSID == wifiNetwork.BSSID);
            //        if (found != null)
            //        {
            //            int i = CollectionOfWifiNetworksChart.IndexOf(found);
            //            if (IsSwitched)
            //            {
            //                var specificWifiNetworkCollection = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID).ToList();
            //                wifiNetwork.Level = specificWifiNetworkCollection.Select(y => y.Level).ToList().Average();
            //            }
            //            CollectionOfWifiNetworksChart[i] = wifiNetwork;
            //        }
            //    }
            //    var wifiNetworksToDelete = CollectionOfWifiNetworksChart.Where(x => !MainPageViewModel.ListOfWifiNetworks.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID));
            //    if (wifiNetworksToDelete != null)
            //    {
            //        foreach (var wifiNetwork in wifiNetworksToDelete.ToList())
            //        {
            //            CollectionOfWifiNetworksChart.Remove(wifiNetwork);
            //        }
            //    }


            //    //SeriesCollection.Add(new BarSeries() { ItemsSource = CollectionOfWifiNetworksChart, XBindingPath = "Channel", YBindingPath = "Level", StrokeWidth = 4, Label = "SSID"});

            //    //foreach (var wifiNetwork in AllWifiNetworksCollection)
            //    //{
            //    //    foreach (var specificWifiNetwork in AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID).ToList())
            //    //    {
            //    //        CollectionOfWifiNetworksChartAverage.Add(new WifiParametersCollection()
            //    //        {
            //    //            CollectionOfWifiParameters = new ObservableCollection<WifiParameters>()
            //    //            {
            //    //                new WifiParameters(){SSID = specificWifiNetwork.SSID, BSSID = specificWifiNetwork.BSSID, Level = specificWifiNetwork.Level}
            //    //            },
            //    //            //AverageLevel = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID).Average(y => y.Level)
            //    //            AverageLevel = 15
            //    //        });
            //    //    }
            //    //}
            //    var newWifiNetworks = MainPageViewModel.ListOfWifiNetworks.Where(x => !CollectionOfWifiNetworksChart.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID)).ToList();
            //    foreach (var wifiNetwork in newWifiNetworks)
            //    {
            //        if (IsSwitched)
            //        {
            //            var specificWifiNetworkCollection = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID).ToList();
            //            wifiNetwork.Level = specificWifiNetworkCollection.Select(y => y.Level).ToList().Average();
            //        }
            //        //wifiNetwork.AverageLevel = 15;
            //        //wifiNetwork.AverageLevel = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID).Select(y => y.Level).Average();
            //        CollectionOfWifiNetworksChart.Insert(0, wifiNetwork);
            //        //SeriesCollection.Add(new ColumnSeries() { ItemsSource = CollectionOfWifiNetworksChart, XBindingPath = "SSID", YBindingPath = "Level", StrokeWidth = 4, Label = wifiNetwork.Level.ToString() });
            //        //AverageLevel = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).Average(y => y.Level);
            //        //TestCollection = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).ToList();
            //        //foreach (var wifi in TestCollection)
            //        //{
            //        //    CollectionOfWifiNetworksChart1.Add(wifi);
            //        //}

            //        //var found = CollectionOfWifiNetworksChart.FirstOrDefault(x => x.SSID == wifiNetwork.SSID || x.BSSID == wifiNetwork.BSSID);
            //        //if (found != null)
            //        //{
            //        //    int i = CollectionOfWifiNetworksChart.IndexOf(found);
            //        //    CollectionOfWifiNetworksChart[i] = wifiNetwork;
            //        //    SeriesCollection.Add(new LineSeries() { ItemsSource = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID), XBindingPath = "Channel", YBindingPath = "Level" });
            //        //}
            //    }
            //}, null, startTimeSpan, periodTimeSpan);
            Task.Run(async () =>
            {
                IsBusy = true;
                await Task.Delay(10000);
                IsBusy = false;
            });
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
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
                CollectionOfWifiNetworksChart.Clear();
                foreach (var wifiNetwork in MainPageViewModel.ListOfWifiNetworks) {
                    AllWifiNetworksCollection.Add(wifiNetwork);
                    if (IsSwitched) {
                        AllWifiNetworksCollection1 = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID).ToList();
                        wifiNetwork.Level = AllWifiNetworksCollection1.Select(y => y.Level).ToList().Average();
                    }
                    CollectionOfWifiNetworksChart.Insert(0, wifiNetwork);
                }
                //foreach (var wifiNetwork in MainPageViewModel.ListOfWifiNetworks)
                //{
                //    AllWifiNetworksCollection.Add(wifiNetwork);

                //    var found = CollectionOfWifiNetworksChart.FirstOrDefault(x => x.SSID == wifiNetwork.SSID || x.BSSID == wifiNetwork.BSSID);
                //    if (found != null)
                //    {
                //        int i = CollectionOfWifiNetworksChart.IndexOf(found);
                //        if (IsSwitched)
                //        {
                //            AllWifiNetworksCollection1 = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID).ToList();
                //            wifiNetwork.Level = AllWifiNetworksCollection1.Select(y => y.Level).ToList().Average();
                //        }
                //        CollectionOfWifiNetworksChart[i] = wifiNetwork;
                //    }
                //}
                //var wifiNetworksToDelete = CollectionOfWifiNetworksChart.Where(x => !MainPageViewModel.ListOfWifiNetworks.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID));
                //if (wifiNetworksToDelete != null)
                //{
                //    foreach (var wifiNetwork in wifiNetworksToDelete.ToList())
                //    {
                //        CollectionOfWifiNetworksChart.Remove(wifiNetwork);
                //    }
                //}


                //SeriesCollection.Add(new BarSeries() { ItemsSource = CollectionOfWifiNetworksChart, XBindingPath = "Channel", YBindingPath = "Level", StrokeWidth = 4, Label = "SSID"});

                //foreach (var wifiNetwork in AllWifiNetworksCollection)
                //{
                //    foreach (var specificWifiNetwork in AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID).ToList())
                //    {
                //        CollectionOfWifiNetworksChartAverage.Add(new WifiParametersCollection()
                //        {
                //            CollectionOfWifiParameters = new ObservableCollection<WifiParameters>()
                //            {
                //                new WifiParameters(){SSID = specificWifiNetwork.SSID, BSSID = specificWifiNetwork.BSSID, Level = specificWifiNetwork.Level}
                //            },
                //            //AverageLevel = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID).Average(y => y.Level)
                //            AverageLevel = 15
                //        });
                //    }
                //}


                //var newWifiNetworks = MainPageViewModel.ListOfWifiNetworks.Where(x => !CollectionOfWifiNetworksChart.Any(y => x.SSID == y.SSID || x.BSSID == y.BSSID)).ToList();
                //foreach (var wifiNetwork in newWifiNetworks)
                //{
                //    if (IsSwitched)
                //    {
                //        var specificWifiNetworkCollection = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID).ToList();
                //        wifiNetwork.Level = specificWifiNetworkCollection.Select(y => y.Level).ToList().Average();
                //    }
                //    //wifiNetwork.AverageLevel = 15;
                //    //wifiNetwork.AverageLevel = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID).Select(y => y.Level).Average();
                //    CollectionOfWifiNetworksChart.Insert(0, wifiNetwork);
                //    //SeriesCollection.Add(new ColumnSeries() { ItemsSource = CollectionOfWifiNetworksChart, XBindingPath = "SSID", YBindingPath = "Level", StrokeWidth = 4, Label = wifiNetwork.Level.ToString() });
                //    //AverageLevel = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).Average(y => y.Level);
                //    //TestCollection = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID).ToList();
                //    //foreach (var wifi in TestCollection)
                //    //{
                //    //    CollectionOfWifiNetworksChart1.Add(wifi);
                //    //}

                //    //var found = CollectionOfWifiNetworksChart.FirstOrDefault(x => x.SSID == wifiNetwork.SSID || x.BSSID == wifiNetwork.BSSID);
                //    //if (found != null)
                //    //{
                //    //    int i = CollectionOfWifiNetworksChart.IndexOf(found);
                //    //    CollectionOfWifiNetworksChart[i] = wifiNetwork;
                //    //    SeriesCollection.Add(new LineSeries() { ItemsSource = CollectionOfWifiNetworksChart.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID), XBindingPath = "Channel", YBindingPath = "Level" });
                //    //}
                //}
                return true;
            });
        }
        private async Task SwitchCharts()
        {
            if(CollectionOfWifiNetworksChart.Count != 0)
            {
                IsBusy = true;
                await Task.Delay(500);
                if (!IsSwitched) {
                    IsSwitched = true;
                    Toast.MakeText(Android.App.Application.Context, "Chart was switched to average level successfully!", ToastLength.Short).Show();
                }
                else {
                    IsSwitched = false;
                    Toast.MakeText(Android.App.Application.Context, "Chart was switched to level successfully!", ToastLength.Short).Show();
                }
                IsBusy = false;
            }
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
