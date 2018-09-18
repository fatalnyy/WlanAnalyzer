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
	public partial class ChannelTrafficPage : ContentPage
	{
		public ChannelTrafficPage ()
		{
            InitializeComponent();
            BindingContext = new ChannelTrafficPageViewModel();
        }
	}
}