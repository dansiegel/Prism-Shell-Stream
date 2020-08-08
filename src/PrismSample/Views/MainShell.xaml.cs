using Prism.Navigation;
using System.ComponentModel;

namespace PrismSample.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainShell
    {
        public MainShell(/*INavigationService navigationService*/)
        {
            //SetNavigationService(navigationService);
            try
            {
                InitializeComponent();
            }
            catch(System.Exception e)
            {

            }
        }
    }
}
