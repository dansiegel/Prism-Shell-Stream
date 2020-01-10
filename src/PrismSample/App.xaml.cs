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
            try
            {
                InitializeComponent();

                var shell = new MainPage();
                var navigationService = Container.Resolve<INavigationService>();

                shell.SetNavigationService(navigationService);
                MainPage = shell;
                var result = await NavigationService.NavigateAsync("HomePage");

                if (!result.Success)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
            catch
            {
                throw;
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
