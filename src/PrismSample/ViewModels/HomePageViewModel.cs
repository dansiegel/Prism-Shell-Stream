using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using Prism.Navigation;

namespace PrismSample.ViewModels
{
    public class HomePageViewModel : BindableBase, IInitialize, INavigationAware
    {

        public string LabelText => "Hello from Prism";

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

        public void Initialize(INavigationParameters parameters)
        {
            Initialized = true;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}