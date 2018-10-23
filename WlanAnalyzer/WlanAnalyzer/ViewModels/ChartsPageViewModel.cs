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
        #region Fields
        private ObservableCollection<WifiParameters> _collectionOfWifiNetworksChart;
        private bool _isBusy;
        #endregion

        #region Constuctors
        public ChartsPageViewModel()
        {
            AllWifiNetworksCollection = new ObservableCollection<WifiParameters>();
            SpecificWifiNetworksList = new List<WifiParameters>();
            _collectionOfWifiNetworksChart = new ObservableCollection<WifiParameters>();
            SortCollectionByLevelDescendingCommand = new Command(async () => await SortCollectionByLevelDescending());
            SortCollectionByLevelAscendingCommand = new Command(async () => await SortCollectionByLevelAscending());
            SwitchChartsCommand = new Command(async () => await SwitchCharts());
            LoadData();
        }
        #endregion

        #region Properties
        public Command SortCollectionByLevelDescendingCommand { get; set; }
        public Command SortCollectionByLevelAscendingCommand { get; set; }
        public Command SwitchChartsCommand { get; set; }
        public ObservableCollection<WifiParameters> AllWifiNetworksCollection { get; set; }
        public List<WifiParameters> SpecificWifiNetworksList { get; set; }
        public bool IsSwitched { get; set; }

        public bool IsBusy {
            get {
                return _isBusy;
            }
            set {
                _isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
            }
        }
        public ObservableCollection<WifiParameters> CollectionOfWifiNetworksChart {
            get {
                return _collectionOfWifiNetworksChart;
            }
            set {
                _collectionOfWifiNetworksChart = value;
                RaisePropertyChanged(nameof(CollectionOfWifiNetworksChart));
            }
        }
        #endregion

        #region Methods
        private void LoadData() {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(10);
       
            Task.Run(async () =>
            {
                IsBusy = true;
                await Task.Delay(10000);
                IsBusy = false;
            });
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                CollectionOfWifiNetworksChart.Clear();
                foreach (var wifiNetwork in MainPageViewModel.ListOfWifiNetworks) {
                    AllWifiNetworksCollection.Add(wifiNetwork);
                    if (IsSwitched) {
                        SpecificWifiNetworksList = AllWifiNetworksCollection.Where(x => x.SSID == wifiNetwork.SSID && x.BSSID == wifiNetwork.BSSID).ToList();
                        wifiNetwork.Level = SpecificWifiNetworksList.Select(y => y.Level).ToList().Average();
                    }
                    CollectionOfWifiNetworksChart.Insert(0, wifiNetwork);
                }
                return true;
            });
        }
        private async Task SwitchCharts() {
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
        private async Task SortCollectionByLevelDescending() {
            IsBusy = true;
            if (CollectionOfWifiNetworksChart.Count != 0)
            {
                await Task.Delay(500);
                CollectionOfWifiNetworksChart = new ObservableCollection<WifiParameters>(CollectionOfWifiNetworksChart.OrderByDescending(x => x.Level).ToList());
            }
            IsBusy = false;
        }
        private async Task SortCollectionByLevelAscending() {
            IsBusy = true;
            if (CollectionOfWifiNetworksChart.Count != 0)
            {
                await Task.Delay(500);
                CollectionOfWifiNetworksChart = new ObservableCollection<WifiParameters>(CollectionOfWifiNetworksChart.OrderBy(x => x.Level).ToList());
            }
            IsBusy = false;
        }
        #endregion
    }
}
