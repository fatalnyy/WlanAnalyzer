using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WlanAnalyzer.Models
{
    public class WifiParameters
    {
        [PrimaryKey, AutoIncrement]
        public int WifiID { get; set; }
        public string SSID { get; set; }
        public string BSSID { get; set; }
        public double Frequency { get; set; }
        public int Level { get; set; }
        public int Channel { get; set; }
        public long TimeStamp { get; set; }

        //public WifiParameters(string ssid, string bssid, int frequency, int level, int channel, long timeStamp)
        //{
        //    SSID = ssid;
        //    BSSID = bssid;
        //    Frequency = frequency;
        //    Level = level;
        //    Channel = channel;
        //    TimeStamp = timeStamp;
        //}

        public static int GetChannel(int Frequency)
        {
            if (Frequency == 2484)
                return 14;

            if (Frequency < 2484)
                return (Frequency - 2407) / 5;

            return Frequency / 5 - 1000;
        }
    }
}
