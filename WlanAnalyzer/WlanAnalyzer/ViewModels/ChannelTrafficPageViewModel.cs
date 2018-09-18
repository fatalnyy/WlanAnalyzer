using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using WlanAnalyzer.Models;

namespace WlanAnalyzer.ViewModels
{
    public class ChannelTrafficPageViewModel :BaseViewModel
    {
        private Dictionary<int, int> _channelsDictionary;
        public List<int> Channels;
        public ChannelTrafficPageViewModel()
        {
            Channels = new List<int>();
            ChannelsDictionary = new Dictionary<int, int>();
            GetChannels();
            UpdatingChannelsDictionary();
            //ChannelTrafficCalc();
        }
        public Dictionary<int, int> ChannelsDictionary
        {
            get
            {
                return _channelsDictionary;
            }
            set
            {
                _channelsDictionary = value;
                RaisePropertyChanged(nameof(ChannelsDictionary));
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
        #region Number of access points in channels properties
        private int _numberOfWifiNetworksChannel1 = 0;
        public int NumberOfWifiNetworksChannel1
        {
            get
            {
                return _numberOfWifiNetworksChannel1;
            }
            set
            {
                _numberOfWifiNetworksChannel1 = value;
                RaisePropertyChanged(nameof(NumberOfWifiNetworksChannel1));
            }
        }
        public int NumberOfWifiNetworksChannel2 { get; set; }
        public int NumberOfWifiNetworksChannel3 { get; set; }
        public int NumberOfWifiNetworksChannel4 { get; set; }
        public int NumberOfWifiNetworksChannel5 { get; set; }
        public int NumberOfWifiNetworksChannel6 { get; set; }
        public int NumberOfWifiNetworksChannel7 { get; set; }
        public int NumberOfWifiNetworksChannel8 { get; set; }
        public int NumberOfWifiNetworksChannel9 { get; set; }
        public int NumberOfWifiNetworksChannel10 { get; set; }
        public int NumberOfWifiNetworksChannel11 { get; set; }
        public int NumberOfWifiNetworksChannel12 { get; set; }
        public int NumberOfWifiNetworksChannel13 { get; set; }
        public int NumberOfWifiNetworksChannel36 { get; set; }
        public int NumberOfWifiNetworksChannel40 { get; set; }
        public int NumberOfWifiNetworksChannel44 { get; set; }
        public int NumberOfWifiNetworksChannel48 { get; set; }
        public int NumberOfWifiNetworksChannel52 { get; set; }
        public int NumberOfWifiNetworksChannel56 { get; set; }
        public int NumberOfWifiNetworksChannel60 { get; set; }
        public int NumberOfWifiNetworksChannel64 { get; set; }
        public int NumberOfWifiNetworksChannel100 { get; set; }
        public int NumberOfWifiNetworksChannel104 { get; set; }
        public int NumberOfWifiNetworksChannel108 { get; set; }
        public int NumberOfWifiNetworksChannel112 { get; set; }
        public int NumberOfWifiNetworksChannel116 { get; set; }
        public int NumberOfWifiNetworksChannel120 { get; set; }
        public int NumberOfWifiNetworksChannel124 { get; set; }
        public int NumberOfWifiNetworksChannel128 { get; set; }
        public int NumberOfWifiNetworksChannel132 { get; set; }
        public int NumberOfWifiNetworksChannel136 { get; set; }
        public int NumberOfWifiNetworksChannel140 { get; set; }
        public int NumberOfWifiNetworksChannel144 { get; set; }
        public int NumberOfWifiNetworksChannel149 { get; set; }
        public int NumberOfWifiNetworksChannel153 { get; set; }
        public int NumberOfWifiNetworksChannel157 { get; set; }
        public int NumberOfWifiNetworksChannel161 { get; set; }
        public int NumberOfWifiNetworksChannel165 { get; set; }

        #endregion

        private void GetChannels()
        {
            //Channels = (List<int>)StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.GroupBy(x => x.Channel).Select(y => y.First()).ToList();
            Channels = StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.Select(x => x.Channel).Distinct().ToList();
        }

        private void UpdatingChannelsDictionary()
        {
            foreach (var channel in Channels)
            {
                ChannelsDictionary.Add(channel, ChannelTrafficCalc(channel));
            }
        }
        private int ChannelTrafficCalc(int channelNumber)
        {

            return StatisticalAnalyzePageViewModel.collectionOfWifiParametersToAnalyzeTrafficChannel.Count(x => x.Channel == channelNumber);

                //slownik[wifiNetwork.Channel]++;
                //switch (wifiNetwork.Channel)
                //{
                //    case 1:
                //        NumberOfWifiNetworksChannel1++;
                //        break;
                //    case 2:
                //        NumberOfWifiNetworksChannel2++;
                //        break;
                //    case 3:
                //        NumberOfWifiNetworksChannel3++;
                //        break;
                //    case 4:
                //        NumberOfWifiNetworksChannel4++;
                //        break;
                //    case 5:
                //        NumberOfWifiNetworksChannel5++;
                //        break;
                //    case 6:
                //        NumberOfWifiNetworksChannel6++;
                //        break;
                //    case 7:
                //        NumberOfWifiNetworksChannel7++;
                //        break;
                //    case 8:
                //        NumberOfWifiNetworksChannel8++;
                //        break;
                //    case 9:
                //        NumberOfWifiNetworksChannel9++;
                //        break;
                //    case 10:
                //        NumberOfWifiNetworksChannel10++;
                //        break;
                //    case 11:
                //        NumberOfWifiNetworksChannel11++;
                //        break;
                //    case 12:
                //        NumberOfWifiNetworksChannel12++;
                //        break;
                //    case 13:
                //        NumberOfWifiNetworksChannel13++;
                //        break;
                //    case 36:
                //        NumberOfWifiNetworksChannel36++;
                //        break;
                //    case 40:
                //        NumberOfWifiNetworksChannel40++;
                //        break;
                //    case 44:
                //        NumberOfWifiNetworksChannel44++;
                //        break;
                //    case 48:
                //        NumberOfWifiNetworksChannel48++;
                //        break;
                //    case 52:
                //        NumberOfWifiNetworksChannel52++;
                //        break;
                //    case 56:
                //        NumberOfWifiNetworksChannel56++;
                //        break;
                //    case 60:
                //        NumberOfWifiNetworksChannel60++;
                //        break;
                //    case 64:
                //        NumberOfWifiNetworksChannel64++;
                //        break;
                //    case 100:
                //        NumberOfWifiNetworksChannel100++;
                //        break;
                //    case 104:
                //        NumberOfWifiNetworksChannel104++;
                //        break;
                //    case 108:
                //        NumberOfWifiNetworksChannel108++;
                //        break;
                //    case 112:
                //        NumberOfWifiNetworksChannel112++;
                //        break;
                //    case 116:
                //        NumberOfWifiNetworksChannel116++;
                //        break;
                //    case 120:
                //        NumberOfWifiNetworksChannel120++;
                //        break;
                //    case 124:
                //        NumberOfWifiNetworksChannel124++;
                //        break;
                //    case 128:
                //        NumberOfWifiNetworksChannel128++;
                //        break;
                //    case 132:
                //        NumberOfWifiNetworksChannel132++;
                //        break;
                //    case 136:
                //        NumberOfWifiNetworksChannel136++;
                //        break;
                //    case 140:
                //        NumberOfWifiNetworksChannel140++;
                //        break;
                //    case 144:
                //        NumberOfWifiNetworksChannel144++;
                //        break;
                //    case 149:
                //        NumberOfWifiNetworksChannel149++;
                //        break;
                //    case 153:
                //        NumberOfWifiNetworksChannel153++;
                //        break;
                //    case 157:
                //        NumberOfWifiNetworksChannel157++;
                //        break;
                //    case 161:
                //        NumberOfWifiNetworksChannel161++;
                //        break;
                //    case 165:
                //        NumberOfWifiNetworksChannel165++;
                //        break;
                //    default:
                //        break;
                //}
                //if (wifiNetwork.Channel == 1)
                //    NumberOfWifiNetworksChannel1++;
            
        }
    }
}
