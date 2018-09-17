using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace WlanAnalyzer.Models
{
    public class WifiParametersJSON
    {
        public WifiParametersJSON()
        {

        }

        public static void ToJson(string filePath, ObservableCollection<WifiParameters> listOfWifiNetworks)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, listOfWifiNetworks);
                file.Dispose();
            }
            //if (NumberOfDetectedAccessPoints != 0)
            //    Toast.MakeText(Android.App.Application.Context, "List of wifi networks has been saved to file successfully.", ToastLength.Short).Show();
        }
        public static ObservableCollection<WifiParameters> FromJson(string filePath)
        {
            using (StreamReader streamReader = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                ObservableCollection<WifiParameters> DeserializedCollectionOfWifiNetworks = (ObservableCollection<WifiParameters>)serializer.Deserialize(streamReader, typeof(ObservableCollection<WifiParameters>));
                streamReader.Dispose();
                return DeserializedCollectionOfWifiNetworks;
            }
        }

    }
}
