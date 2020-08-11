using Prism.Navigation;
using PrismSample.Services;

namespace PrismSample.ViewModels
{
    public class TabAViewModel : ViewModelBase, 
        INavigationAware
    {
        BaseServices _baseServices;
        private INavigationService _navigationService => _baseServices.NavigationService;
        public TabAViewModel(BaseServices baseServices) : base(baseServices)
        {
            _baseServices = baseServices;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}