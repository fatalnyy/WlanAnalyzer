using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using WlanAnalyzer.Models;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace WlanAnalyzer.ViewModels
{
    public class ChannelTrafficPageViewModel :BaseViewModel
    {
        private Dictionary<int, int> _channelsDictionary2GHz;
        private Dictionary<int, int> _channelsDictionary5GHz;
        private List<int> _freeChannels;
        public List<int> BusyChannels;
        public List<double> Frequencies;
        private bool _isBusy;
        public int[] Channels = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 36, 40, 44, 48, 52, 56, 60, 64, 100, 104, 108, 112, 116, 120, 124, 128, 132, 136, 140, 144, 149, 153, 157, 161, 165 };
        public ChannelTrafficPageViewModel()
        {
            BusyChannels = new List<int>();
            FreeChannels = new List<int>();
            Frequencies = new List<double>();
            ChannelsDictionary2GHz = new Dictionary<int, int>();
            ChannelsDictionary5GHz = new Dictionary<int, int>();
            SortNumberOfWLANsAscendingCommand = new Command(async () => await SortNumberOfWLANsAscending());
            SortNumberOfWLANsDescendingCommand = new Command(async () => await SortNumberOfWLANsDescending());
            SortByChannelsCommand = new Command(async () => await SortByChannels());
            GetChannels();
            GetFrequencies();
            UpdatingChannelsDictionary();
            AverageFrequency5GHz = Math.Round(GetAverageFrequency5GHz(), 2);
            AverageFrequency2GHz = Math.Round(GetAverageFrequency2GHz(), 2);
            //ChannelTrafficCalc();
        }
        public Command SortNumberOfWLANsAscendingCommand { get; set; }
        public Command SortNumberOfWLANsDescendingCommand { get; set; }
        public Command SortByChannelsCommand { get; set; }
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
        public double AverageFrequency5GHz { get; set; }
        public double AverageFrequency2GHz { get; set; }

        public Dictionary<int, int> ChannelsDictionary2GHz
        {
            get
            {
                return _channelsDictionary2GHz;
            }
            set
            {
                _channelsDictionary2GHz = value;
                RaisePropertyChanged(nameof(ChannelsDictionary2GHz));
            }
        }
        public Dictionary<int, int> ChannelsDictionary5GHz
        {
            get
            {
                return _channelsDictionary5GHz;
            }
            set
            {
                _channelsDictionary5GHz = value;
                RaisePropertyChanged(nameof(ChannelsDictionary5GHz));
            }
        }
        public List<int> FreeChannels
        {
            get
            {
                return _freeChannels;
            }
            set
            {
                _freeChannels = value;
                RaisePropertyChanged(nameof(FreeChannels));
                RaisePropertyChanged(nameof(FreeChannelsText));
            }
        }
        public string FreeChannelsText
        {
            get
            {
                return string.Join(", ", FreeChannels);
                //1return ($"Free Channels: {FreeChannels}");
            }
        }
        public int NumberOfWifiNetworksInChannel { get; set; }
        public int NumberOfWifiNetworks
        {
            get
            {
                return StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.Count;
            }
            set
            {
                NumberOfWifiNetworks = value;
                RaisePropertyChanged(nameof(NumberOfWifiNetworks));
            }
        }

        private void GetFrequencies()
        {
            Frequencies = StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.Select(x => x.Frequency).ToList();
        }
        private double GetAverageFrequency5GHz()
        {
            return Frequencies.Where(x => x > 2484).ToList().Average();
        }
        private double GetAverageFrequency2GHz()
        {
            return Frequencies.Where(x => x <= 2484).ToList().Average();
        }
        private void GetChannels()
        {
            //Channels = (List<int>)StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.GroupBy(x => x.Channel).Select(y => y.First()).ToList();
            BusyChannels = StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.Select(x => x.Channel).Distinct().ToList();
            //FreeChannels = Channels.Where(x => !BusyChannels.Any(x => x.));
            FreeChannels = Channels.Except(BusyChannels).ToList();
        }

        private void UpdatingChannelsDictionary()
        {
            foreach (var channel in BusyChannels)
            {
                if(channel > 13)
                    ChannelsDictionary5GHz.Add(channel, ChannelTrafficCalc(channel));
                else
                    ChannelsDictionary2GHz.Add(channel, ChannelTrafficCalc(channel));
            }
            ChannelsDictionary5GHz = ChannelsDictionary5GHz.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            ChannelsDictionary2GHz = ChannelsDictionary2GHz.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
        private int ChannelTrafficCalc(int channelNumber)
        {
            return StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.Count(x => x.Channel == channelNumber);      
        }
        private async Task SortNumberOfWLANsAscending()
        {
            IsBusy = true;
            await Task.Delay(1500);
            ChannelsDictionary5GHz = ChannelsDictionary5GHz.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            ChannelsDictionary2GHz = ChannelsDictionary2GHz.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            IsBusy = false;
        }
        private async Task SortNumberOfWLANsDescending()
        {
            IsBusy = true;
            await Task.Delay(1500);
            ChannelsDictionary5GHz = ChannelsDictionary5GHz.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            ChannelsDictionary2GHz = ChannelsDictionary2GHz.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            IsBusy = false;
        }
        private async Task SortByChannels()
        {
            IsBusy = true;
            await Task.Delay(1500);
            ChannelsDictionary5GHz = ChannelsDictionary5GHz.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            ChannelsDictionary2GHz = ChannelsDictionary2GHz.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            IsBusy = false;
        }
    }
}
