using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WlanAnalyzer.Models
{
    public class WifiParametersJSON
    {
        public ObservableCollection<WifiParameters> CollectionOfSavedWifiNetworks { get; set; }

    }
}
