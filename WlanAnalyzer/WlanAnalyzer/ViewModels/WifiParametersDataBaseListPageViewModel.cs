using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WlanAnalyzer.Models;
using Xamarin.Forms;

namespace WlanAnalyzer.ViewModels
{
    public class WifiParametersDataBaseListPageViewModel : BaseViewModel
    {
        private INavigation _navigation;
        private List<WifiParameters> _listOfWifiParameters;
        private ObservableCollection<WifiParameters> _collectionOfWifiParameters;
        private int _numberOfWifiNetworksDB;
        public int NumberOfWifiNetworksDB
        {
            get
            {
                return _numberOfWifiNetworksDB;
            }
            set
            {
                _numberOfWifiNetworksDB = value;
                RaisePropertyChanged(nameof(NumberOfWifiNetworksDB));
            }
        }
        public ObservableCollection<WifiParameters> CollectionOfWifiParameters
        {
            get
            {
                return _collectionOfWifiParameters;
            }
            set
            {
                _collectionOfWifiParameters = value;
                RaisePropertyChanged("CollectionOfWifiParameters");
            }
        }
        public List<WifiParameters> ListOfWifiParameters
        {
            get
            {
                return _listOfWifiParameters;
            }
            set
            {
                _listOfWifiParameters = value;
                RaisePropertyChanged("ListOfWifiParameters");
            }
        }
        public Command DeleteSelectedWifiNetworkCommand { get; set; }
        public Command ClearDatabaseCommand { get; set; }
   
        public WifiParameters SelectedWifiNetwork { get; set; }

        public WifiParametersDataBaseListPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            CollectionOfWifiParameters = new ObservableCollection<WifiParameters>();
            GetListOfWifiParameters();
            DeleteSelectedWifiNetworkCommand = new Command(async () => await DeleteSelectedWifiNetwork());
            ClearDatabaseCommand = new Command(ClearDatabase);
        }

        private async void GetListOfWifiParameters()
        {
            ListOfWifiParameters = await App.Database.GetListOfWifiParametersAsync();
            foreach (var wifiNetwork in ListOfWifiParameters)
            {
                CollectionOfWifiParameters.Add(wifiNetwork);
            }
            NumberOfWifiNetworksDB = CollectionOfWifiParameters.Count;
        }

        private async Task DeleteSelectedWifiNetwork()
        {
            if(SelectedWifiNetwork != null)
            {
                CollectionOfWifiParameters.Remove(SelectedWifiNetwork);
                ListOfWifiParameters.Remove(SelectedWifiNetwork);
                await App.Database.DeleteParticularWifiParameters(SelectedWifiNetwork);
                Toast.MakeText(Android.App.Application.Context, "Selected wifi network has been removed successfully.", ToastLength.Short).Show();
                NumberOfWifiNetworksDB--;
            }
        }

        private void ClearDatabase()
        {
            App.Database.DeleteAllObjectsFromDatabase();
            CollectionOfWifiParameters.Clear();
            Toast.MakeText(Android.App.Application.Context, "Database has been cleared successfully.", ToastLength.Short).Show();
            NumberOfWifiNetworksDB = 0;
        }
    }
}
