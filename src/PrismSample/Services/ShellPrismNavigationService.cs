using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Prism.Common;

namespace PrismSample.Services
{
    public class ShellPrismNavigationService :
        ShellNavigationService,
        INavigationService
    {
        public ShellPrismNavigationService()
        {
        }

        public override Page Create(ShellContentCreateArgs args)
        {
            return base.Create(args);
        }

        Task<INavigationResult> INavigationService.GoBackAsync()
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.GoBackAsync(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.NavigateAsync(Uri uri){
            return ((INavigationService)this).NavigateAsync(uri, null);
        }

        async Task<INavigationResult> INavigationService.NavigateAsync(Uri uri, INavigationParameters parameters)
        {
            await  Shell.Current.GoToAsync(uri);
            return new NavigationResult()
            {
                Success = true
            };
        }

        
        public override Task<ShellRouteState> ParseAsync(ShellUriParserArgs args)
        {
            return base.ParseAsync(args);
        }

        Task<INavigationResult> INavigationService.NavigateAsync(string name)
        {
            return ((INavigationService)this).NavigateAsync(name, null);
        }

        Task<INavigationResult> INavigationService.NavigateAsync(string name, INavigationParameters parameters)
        {
            return ((INavigationService)this).NavigateAsync(UriParsingHelper.Parse(name), parameters);
        }
    }
}
