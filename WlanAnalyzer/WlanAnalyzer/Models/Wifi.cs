using System;
using System.Collections.Generic;
using System.Text;

namespace WlanAnalyzer.Models
{
    public class Wifi
    {
        public string SSID { get; set; } = "cjik";
        public string BSSID { get; set; }
        public int Frequency { get; set; }
        public int Level { get; set; }
        public int Freq1 { get; set; }
        public int Freq2 { get; set; }
        public int ChannelWidth { get; set; }
        public long TimeStamp { get; set; }

        public Wifi(string ssid, string bssid, int frequency, int level, int channelWidth, int freq1, int freq2, long timeStamp)
        {
            SSID = ssid;
            BSSID = bssid;
            Frequency = frequency;
            Level = level;
            ChannelWidth = channelWidth;
            Freq1 = freq1;
            Freq2 = freq2;
            TimeStamp = timeStamp;
        }
    }
}
