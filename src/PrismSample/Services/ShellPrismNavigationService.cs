using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PrismSample.Services
{
    public class ShellPrismNavigationService :
        ShellNavigationService,
        INavigationService
    {
        Task<INavigationResult> INavigationService.GoBackAsync()
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.GoBackAsync(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.NavigateAsync(Uri uri)
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.NavigateAsync(Uri uri, INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.NavigateAsync(string name)
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.NavigateAsync(string name, INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
