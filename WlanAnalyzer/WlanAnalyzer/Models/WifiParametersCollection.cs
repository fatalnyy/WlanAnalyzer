using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WlanAnalyzer.Models
{
    public class WifiParametersCollection
    {
        public double AverageLevel { get; set; }
        public ObservableCollection<WifiParameters> CollectionOfWifiParameters { get; set; }
    }
}
