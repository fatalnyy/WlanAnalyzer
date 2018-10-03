using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Java.Util;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace WlanAnalyzer.ViewModels
{
    public class WifiGPSPageViewModel : BaseViewModel
    {
        private double _latitude;
        private double _longitude;

        public WifiGPSPageViewModel()
        {
            GetCurrentLocationCommand = new Command(async () => await GetCurrentLocation());
        }
        public Command GetCurrentLocationCommand { get; set; }
        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = value;
                RaisePropertyChanged(nameof(Latitude));
            }
        }
        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;
                RaisePropertyChanged(nameof(Longitude));
            }
        }
        private async Task GetCurrentLocation()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 10;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);

            Latitude = position.Latitude;
            Longitude = position.Longitude;
        }
        //public double y{ get; set; }
        //public double x{ get; set; }
       
        private void GetPointsOfIntersectionOfTwoCircles(double x1, double y1, double r1, double x2, double y2, double r2)
        {
            double x, y;
            if(y1 == y2)
            {
                x = (Math.Pow(x1, 2) - Math.Pow(x2, 2) - Math.Pow(r1, 2) + Math.Pow(r2, 2)) / (2 * (x1 - x2));
            }
            else if(x1 == x2)
            {
                y = Math.Pow(y1, 2) - Math.Pow(y2, 2) - Math.Pow(r1, 2) + Math.Pow(r2, 2) / (2 * (y1 - y2));
            }
            else
            {
                //Vector y = (-2 * x * (x1 - x2) + Math.Pow(x1, 2) - Math.Pow(x2, 2) + Math.Pow(y1, 2) - Math.Pow(y2, 2) - Math.Pow(r1, 2) + Math.Pow(r2, 2)) / (2 * (y1 - y2));
                //(-2 * x * (x1 - x2) + Math.Pow(x1, 2) - Math.Pow(x2, 2) + Math.Pow(y1, 2) - Math.Pow(y2, 2) - Math.Pow(r1, 2) + Math.Pow(r2, 2)) / (2 * (y1 - y2))
                //y + (2 * x * (x1 - x2)) = 1;
                
                //Math.Pow(x - x1, 2) + Math.Pow(((-2 * x * (x1 - x2) + Math.Pow(x1, 2) - Math.Pow(x2, 2) + Math.Pow(y1, 2) - Math.Pow(y2, 2) - Math.Pow(r1, 2) + Math.Pow(r2, 2)) / (2 * (y1 - y2))) - y1, 2) = Math.Pow(r1, 2);
                
            
            }

            //(Math.Pow((x - x1), 2) + Math.Pow((y - y1), 2)) = Math.Pow(r1, 2);
            //y
        }
    }
}
