using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ClearDatabaseCommand = new Command(async () => await ClearDatabase());
        }

        private async void GetListOfWifiParameters()
        {
            ListOfWifiParameters = await App.Database.GetListOfWifiParametersAsync();
            foreach (var wifiNetwork in ListOfWifiParameters)
            {
                CollectionOfWifiParameters.Add(wifiNetwork);
            }
        }

        private async Task DeleteSelectedWifiNetwork()
        {
            if(SelectedWifiNetwork != null)
            {
                CollectionOfWifiParameters.Remove(SelectedWifiNetwork);
                ListOfWifiParameters.Remove(SelectedWifiNetwork);
                await App.Database.DeleteParticularWifiParameters(SelectedWifiNetwork);
            }
        }

        private async Task ClearDatabase()
        {
            await App.Database.DeleteAllObjectsFromDatabase();
        }
    }
}
