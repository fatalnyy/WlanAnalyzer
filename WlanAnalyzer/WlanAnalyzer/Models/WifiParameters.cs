using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WlanAnalyzer.Models
{
    public class WifiParameters
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int WifiID { get; set; }
        public string SSID { get; set; }
        public string BSSID { get; set; }
        public double Frequency { get; set; }
        public double Level { get; set; }
        public int Channel { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [JsonIgnore]
        public double AverageLevel  { get; set; }

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
