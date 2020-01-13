using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Prism.Common;
using Prism.Ioc;
using Prism.Behaviors;

namespace PrismSample.Services
{
    public class ShellPrismNavigationService :
        ShellNavigationService,
        INavigationService
    {
        private readonly IContainerExtension _container;
        private IPageBehaviorFactory _pageBehaviorFactory { get; }

        public ShellPrismNavigationService(
            IContainerExtension containerExtension,
            IPageBehaviorFactory pageBehaviorFactory)
        {
            _container = containerExtension;
            _pageBehaviorFactory = pageBehaviorFactory;
        }

        public override async Task<ShellRouteState> NavigatingToAsync(ShellNavigationArgs args)
        {
            var pathPart = args.FutureState.CurrentRoute.GetCurrent();

            if (pathPart.ShellPart is ShellContent shellContent)
            {
                var page = CreatePageFromSegment(shellContent.Route);
                shellContent.Content = page;
                await PageUtilities.OnInitializedAsync(page,
                    new NavigationParameters()
                    );
            }

            return args.FutureState;
        }

        public override Task AppearingAsync(ShellLifecycleArgs args)
        {
            return base.AppearingAsync(args);
        }

        protected virtual Page CreatePage(string segmentName)
        {
            try
            {
                return _container.Resolve<object>(segmentName) as Page;
            }
            catch (Exception ex)
            {
                if (((IContainerRegistry)_container).IsRegistered<object>(segmentName))
                    throw new NavigationException(NavigationException.ErrorCreatingPage, null, ex);

                throw new NavigationException(NavigationException.NoPageIsRegistered, null, ex);
            }
        }

        protected virtual Page CreatePageFromSegment(string segment)
        {
            string segmentName = null;
            try
            {
                segmentName = UriParsingHelper.GetSegmentName(segment);
                var page = CreatePage(segmentName);
                if (page == null)
                {
                    var innerException = new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));
                    throw new NavigationException(NavigationException.NoPageIsRegistered, null, innerException);
                }

                PageUtilities.SetAutowireViewModelOnPage(page);
                _pageBehaviorFactory.ApplyPageBehaviors(page);

                // Not Relavent for Shell since we only work with Content Page and not Tabbed or Carousel Pages
                //ConfigurePages(page, segment);

                return page;
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
                System.Diagnostics.Debugger.Break();
#endif
                throw;
            }
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
