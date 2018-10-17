using Syncfusion.SfChart.XForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace WlanAnalyzer.Models
{
    public class SfChartExt : SfChart
    {

        public static readonly BindableProperty SourceProperty =
        BindableProperty.Create("Source", typeof(object), typeof(SfChartExt), null, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty SeriesTemplateProperty =
        BindableProperty.Create("SeriesTemplate", typeof(DataTemplate), typeof(SfChartExt), propertyChanged: OnPropertyChanged);

        //Gets or sets the ItemsSource of collection of collections.

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        //Gets or sets the template for the series to be generated.

        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }

        //private static void OnPropertyChanged(BindableObject d, EventArgs e)
        //{
        //    (d as SfChartExt).GenerateSeries();
        //}
        private static void OnPropertyChanged(BindableObject d, object oldValue, object newValue)
        {
            (d as SfChartExt).GenerateSeries();
        }
        //Generate the series per the counts in the itemssource.
        private void GenerateSeries()
        {
            if (Source == null || SeriesTemplate == null)
                return;

            var commonItemsSource = (Source as IEnumerable).GetEnumerator();

            while (commonItemsSource.MoveNext())
            {
                ChartSeries series = SeriesTemplate.CreateContent() as ChartSeries;
                series.BindingContext = commonItemsSource.Current;
                Series.Add(series);
            }
        }
    }
}
