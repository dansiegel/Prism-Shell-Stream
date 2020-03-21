using System;
using Prism.Ioc;
using Prism.Navigation;
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
                //var shell = Container.Resolve<MainPage>();
                //MainPage = shell;
                //var result = await NavigationService.NavigateAsync("HomePage");
                try
                {
                    var result = await UseShellNavigation<MainShell>().NavigateAsync("TabA?message=Hello%20from%App");

                    if (!result.Success)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    System.Diagnostics.Debugger.Break();
                    throw;
                }
                
            }
            catch
            {
                throw;
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<INavigationService, ShellPrismNavigationService>();
            //containerRegistry.RegisterForNavigation<NavigationPage>();
            // containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<FlyoutPageA, FlyoutPageAViewModel>();
            containerRegistry.RegisterForNavigation<FlyoutPageB, FlyoutPageBViewModel>();
            containerRegistry.RegisterForNavigation<TabA, TabAViewModel>();
            containerRegistry.RegisterForNavigation<TabB, TabBViewModel>();
            containerRegistry.RegisterForNavigation<ModalPage, ModalPageViewModel>();
            containerRegistry.RegisterForNavigation<ViewA, ViewAViewModel>();
            containerRegistry.RegisterForNavigation<ViewB, ViewBViewModel>();
            containerRegistry.RegisterForNavigation<ViewC, ViewCViewModel>();
        }
    }
}
