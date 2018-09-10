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
        public long TimeStamp { get; set; }
    }
}
