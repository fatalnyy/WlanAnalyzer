using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WlanAnalyzer.Models;
using Xamarin.Forms;

namespace WlanAnalyzer.ViewModels
{
    public class StatisticalAnalyzePageViewModel : BaseViewModel
    {
        private INavigation _navigation;
        public Command LoadDataFromFileCommand { get; set; }
        public Command LoadDataFromCurrentScanListCommand { get; set; }
        public Command LoadDataFromDatabaseCommand { get; set; }
        public Command SortCollectionByLevelCommand { get; set; }
        public Command SortCollectionByFrequencyCommand { get; set; }
        public Command OpenChannelTrafficToolbarCommand { get; set; }
        private bool _isBusy;
        private string _fileNameToAnalyze;
        private int _numberOfWifiNetworksToAnalyze;
        public static ObservableCollection<WifiParameters> collectionOfWifiParametersToAnalyzeTrafficChannel;
        private static ObservableCollection<WifiParameters> _collectionOfWifiNetworksToAnalyze;
        private List<WifiParameters> _listOfWifiNetworksToAnalyze;
        StatisticalAnalyzePageViewModel StatisticalAnalyzePageViewModel1 { get; set; }
        public StatisticalAnalyzePageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            CollectionOfWifiNetworksToAnalyze = new ObservableCollection<WifiParameters>();
            collectionOfWifiParametersToAnalyzeTrafficChannel = new ObservableCollection<WifiParameters>();

            LoadDataFromFileCommand = new Command(async () => await LoadDataFromFile());
            LoadDataFromCurrentScanListCommand = new Command(async () => await LoadDataFromCurrentScanList());
            LoadDataFromDatabaseCommand = new Command(async () => await LoadDataFromDatabase());
            SortCollectionByLevelCommand = new Command(async () => await SortCollectionByLevel());
            SortCollectionByFrequencyCommand = new Command(async () => await SortCollectionByFrequency());
            OpenChannelTrafficToolbarCommand = new Command(async () => await OpenChannelTraffic());
        }
        public ObservableCollection<WifiParameters> CollectionOfWifiNetworksToAnalyze
        {
            get
            {
                return _collectionOfWifiNetworksToAnalyze;
            }
            set
            {
                _collectionOfWifiNetworksToAnalyze = value;
                RaisePropertyChanged("CollectionOfWifiNetworksToAnalyze");
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
                RaisePropertyChanged(nameof(IsBusy));
            }
        }
        public string FileNameToAnalyze
        {
            get
            {
                return _fileNameToAnalyze;
            }
            set
            {
                _fileNameToAnalyze = value;
                RaisePropertyChanged(nameof(FileNameToAnalyze));
            }
        }
        public int NumberOfWifiNetworksToAnalyze
        {
            get
            {
                return _numberOfWifiNetworksToAnalyze;
            }
            set
            {
                _numberOfWifiNetworksToAnalyze = value;
                RaisePropertyChanged(nameof(NumberOfWifiNetworksToAnalyze));
                RaisePropertyChanged(nameof(NumberOfWifiNetworksToAnalyzeText));
            }
        }
        public string NumberOfWifiNetworksToAnalyzeText
        {
            get
            {
                return ($"Number of detected access points: {NumberOfWifiNetworksToAnalyze}");
            }
        }
        public List<WifiParameters> ListOfWifiNetworksToAnalyze
        {
            get
            {
                return _listOfWifiNetworksToAnalyze;
            }
            set
            {
                _listOfWifiNetworksToAnalyze = value;
                RaisePropertyChanged(nameof(ListOfWifiNetworksToAnalyze));
            }
        }
        private async Task LoadDataFromFile()
        {
            IsBusy = true;
            CollectionOfWifiNetworksToAnalyze.Clear();
            await Task.Delay(2000);
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            //var sdCardPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(sdCardPath, FileNameToAnalyze + ".json");

            if (File.Exists(filePath))
            {
                foreach (var wifiNetwork in WifiParametersJSON.FromJson(filePath))
                {
                    CollectionOfWifiNetworksToAnalyze.Add(wifiNetwork);
                }
                collectionOfWifiParametersToAnalyzeTrafficChannel = CollectionOfWifiNetworksToAnalyze;
                NumberOfWifiNetworksToAnalyze = CollectionOfWifiNetworksToAnalyze.Count;
                Toast.MakeText(Android.App.Application.Context, "Data from file was loaded successfully.", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, "File with such name does not exist!", ToastLength.Short).Show();
                NumberOfWifiNetworksToAnalyze = 0;
            }
            IsBusy = false;
        }

        private async Task LoadDataFromCurrentScanList()
        {

            IsBusy = true;
            CollectionOfWifiNetworksToAnalyze.Clear();
            await Task.Delay(2000);
            if (MainPageViewModel.ListOfWifiNetworks.Count != 0)
            {
                foreach (var wifiNetwork in MainPageViewModel.ListOfWifiNetworks)
                {
                    CollectionOfWifiNetworksToAnalyze.Add(wifiNetwork);
                }
                collectionOfWifiParametersToAnalyzeTrafficChannel = CollectionOfWifiNetworksToAnalyze;
                NumberOfWifiNetworksToAnalyze = CollectionOfWifiNetworksToAnalyze.Count;
                Toast.MakeText(Android.App.Application.Context, "Data from current scan list was loaded successfully.", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, "You have to scan wifi networks first!.", ToastLength.Short).Show();
                NumberOfWifiNetworksToAnalyze = 0;
            }
            IsBusy = false;
        }

        private async Task LoadDataFromDatabase()
        {

            IsBusy = true;
            CollectionOfWifiNetworksToAnalyze.Clear();
            await Task.Delay(2000);
            ListOfWifiNetworksToAnalyze = await App.Database.GetListOfWifiParametersAsync();
            if(ListOfWifiNetworksToAnalyze.Count != 0)
            {
                foreach (var wifiNetwork in ListOfWifiNetworksToAnalyze)
                {
                    CollectionOfWifiNetworksToAnalyze.Add(wifiNetwork);
                }
                collectionOfWifiParametersToAnalyzeTrafficChannel = CollectionOfWifiNetworksToAnalyze;
                NumberOfWifiNetworksToAnalyze = CollectionOfWifiNetworksToAnalyze.Count;
                Toast.MakeText(Android.App.Application.Context, "Data from database was loaded successfully.", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, "Your database is empty!", ToastLength.Short).Show();
                NumberOfWifiNetworksToAnalyze = 0;
            }
            IsBusy = false;
        }

        private async Task SortCollectionByLevel()
        {
            if(CollectionOfWifiNetworksToAnalyze.Count != 0)
            {
                IsBusy = true;
                await Task.Delay(1500);
                CollectionOfWifiNetworksToAnalyze = new ObservableCollection<WifiParameters>(CollectionOfWifiNetworksToAnalyze.OrderByDescending(x => x.Level).ToList());
                IsBusy = false;
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, "You have to load your data first!", ToastLength.Short).Show();
            }
        }
        private async Task SortCollectionByFrequency()
        {
            if(CollectionOfWifiNetworksToAnalyze.Count != 0)
            {
                IsBusy = true;
                await Task.Delay(1500);
                CollectionOfWifiNetworksToAnalyze = new ObservableCollection<WifiParameters>(CollectionOfWifiNetworksToAnalyze.OrderByDescending(x => x.Frequency).ToList());
                IsBusy = false;
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, "You have to load your data first!", ToastLength.Short).Show();
            }
        }

        private async Task OpenChannelTraffic()
        {
            if(CollectionOfWifiNetworksToAnalyze.Count != 0)
                await _navigation.PushAsync(new ChannelTrafficPage());
            else
                Toast.MakeText(Android.App.Application.Context, "You have to load your data first!", ToastLength.Short).Show();
        }
    }
}
