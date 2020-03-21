using System;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using PrismSample.Services;

namespace PrismSample.ViewModels
{
    public abstract class ViewModelBase : BindableBase, IAutoInitialize, IInitialize, INavigationAware, IDestructible
    {
        private BaseServices _baseServices { get; }
        private INavigationService _navigationService => _baseServices.NavigationService;
        protected IPageDialogService PageDialogs => _baseServices.PageDialogs;

        protected ViewModelBase(BaseServices baseServices)
        {
            _baseServices = baseServices;
        }

        public string LabelText => $"Hello from {GetType().Name}";

        private bool _initialized;
        public bool Initialized
        {
            get => _initialized;
            set => SetProperty(ref _initialized, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _navigatedTo;
        public bool NavigatedTo
        {
            get => _navigatedTo;
            set => SetProperty(ref _navigatedTo, value);
        }

        public void Initialize(INavigationParameters parameters)
        {
            Initialized = true;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            NavigatedTo = true;
        }

        public void Destroy()
        {
            Console.WriteLine($"{GetType().Name} is being Destroyed...");
        }
    }
}