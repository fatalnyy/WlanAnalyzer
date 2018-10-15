using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WlanAnalyzer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace WlanAnalyzer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChartsPage : ContentPage
	{
		public ChartsPage ()
		{
			InitializeComponent ();
            BindingContext = new ChartsPageViewModel();

            //SfChart chart = new SfChart();
            //ChartZoomPanBehavior zoomPan = new ChartZoomPanBehavior();
            //chart.ChartBehaviors.Add(zoomPan);

            //chart.PrimaryAxis = new CategoryAxis();
            //chart.SecondaryAxis = new NumericalAxis();
            //chart.PrimaryAxis.AutoScrollingDelta = 15;
            //chart.PrimaryAxis.AutoScrollingMode = ChartAutoScrollingMode.Start;

            //chart.BindingContext = new ChartsPageViewModel();
            //chart.SetBinding(SfChart.SeriesProperty, "DataCollection");

            //Content = chart;
        }
	}
}