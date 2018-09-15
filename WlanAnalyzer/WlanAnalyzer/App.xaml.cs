using System;
using WlanAnalyzer.DataBase;
using WlanAnalyzer.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WlanAnalyzer
{
    public partial class App : Application
    {
        static WifiParametersDataBase database;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        public static WifiParametersDataBase Database
        {
            get
            {
                if (database == null) {
                    database = new WifiParametersDataBase(DependencyService.Get<ILocalFileHelper>().GetLocalFilePath("WifiParameters.db3"));
                }
                return database;
            }
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
