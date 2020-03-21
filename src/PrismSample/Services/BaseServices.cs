using Prism.Navigation;
using Prism.Services;

namespace PrismSample.Services
{
    public sealed class BaseServices
    {
        public BaseServices(INavigationService navigationService, IPageDialogService pageDialogs)
        {
            NavigationService = navigationService;
            PageDialogs = pageDialogs;
        }

        public INavigationService NavigationService { get; }

        public IPageDialogService PageDialogs { get; }
    }
}
