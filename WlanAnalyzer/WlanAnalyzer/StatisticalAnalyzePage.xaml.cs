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
	public partial class StatisticalAnalyzePage : ContentPage
	{
		public StatisticalAnalyzePage ()
		{
			InitializeComponent ();
            BindingContext = new StatisticalAnalyzePageViewModel(this.Navigation);
		}
	}
}