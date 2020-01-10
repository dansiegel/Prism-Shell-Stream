using System;
using Prism.Ioc;
using Prism.Navigation;
using PrismSample.Services;
using PrismSample.ViewModels;
using PrismSample.Views;
using Xamarin.Forms;

namespace PrismSample
{
    public partial class App
    {
        public App()
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var shell = new MainPage();
            shell.SetNavigationService(Container.Resolve<INavigationService>());
            MainPage = shell;
            var result = await NavigationService.NavigateAsync("HomePage");

            if(!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<INavigationService, ShellPrismNavigationService>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            // containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
        }
    }
}
