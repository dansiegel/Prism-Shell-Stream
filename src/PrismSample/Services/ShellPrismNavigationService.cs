using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Prism.Common;
using Prism.Ioc;
using Prism.Behaviors;
using System.Linq;

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
                   _currentParameters
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

        public override void ApplyParameters(ShellLifecycleArgs args)
        {
            base.ApplyParameters(args);

            var element = args.Element;
            var prismParameters = _currentParameters;
            string fullSegmentPath = args.PathPart.NavigationParameters["foo"];
            var navigationParameters = UriParsingHelper.GetSegmentParameters(fullSegmentPath, _currentParameters);

            PageUtilities.OnInitializedAsync(element, navigationParameters);

            if (element is ShellContent content && content.Content != null)
                PageUtilities.OnInitializedAsync(content.Content, navigationParameters);
        }

        internal const string RemovePageRelativePath = "../";
        private INavigationParameters _currentParameters;
        private Dictionary<PathPart, INavigationParameters> _generateCurrentNavigationParameters;

        // First... validates URI
        public override Task<ShellRouteState> ParseAsync(ShellUriParserArgs args)
        {
            ShellRouteState newState = args.Shell.RouteState;

            //var newState = RebuildCurrentState(args.Shell.CurrentState.Location, args.Shell.Items);
            //var currentUri = args.Shell.CurrentState.Location;
            var navigationSegments = UriParsingHelper.GetUriSegments(args.Uri);
            navigationSegments.ToList().ForEach(x =>
            {
                var segmentName = UriParsingHelper.GetSegmentName(x);

                // ../ViewA
                if (segmentName == RemovePageRelativePath && newState.CurrentRoute.PathParts.Count >= 3)
                {
                    var currentRoutes = newState.CurrentRoute.PathParts.ToList();
                    currentRoutes.RemoveAt(newState.CurrentRoute.PathParts.Count - 1);
                    newState.CurrentRoute.PathParts = currentRoutes;
                    //return null;
                }
                else
                {
                    //ViewA?id=5&grapes=purple/ViewB?id=3
                    //ViewA?id=5&id=4&id=2&id=1&grapes=purple

                    var segmentParts = segmentName.Split('?');
                    var navigationParameters = UriParsingHelper.GetSegmentParameters(x, _currentParameters);
                    var shellItem = GetShellItem(args.Shell.Items, segmentName);
                    //return new PrismPathPart(shellItem, segmentName, navigationParameters);
                    newState.Add(new PathPart(shellItem, new Dictionary<string, string> { { "foo", x } }));
                }
            });

           // newState.Add(newPathParts.Where(x => x != null && x.ShellPart != null).ToList());

           // var routePath = new RoutePath(newState, null);
           // var routeState = new ShellRouteState(routePath);
            // Home/Demo

            // Routing.ImplicitPrefix = "IMPL_";
            //var item = args.Shell.Items.First(x => x.Route.StartsWith("IMPL_"));

            return Task.FromResult(newState);
        }

        private static List<PathPart> RebuildCurrentState(Uri uri, IList<ShellItem> shellItems)
        {
            var pathParts = new List<PathPart>();
            // TODO

            return pathParts;
        }

        private static BaseShellItem GetShellItem(IList<ShellItem> items, string name)
        {
            var shellItem = items.FirstOrDefault(x => !IsImplicitRoute(x) && UriParsingHelper.GetSegmentName(x.Route) == name);
            if (shellItem != null)
                return shellItem;

            var implicitItems = items.Where(x => IsImplicitRoute(x));
            foreach (var implicitItem in implicitItems)
            {
                var item = GetShellItem(implicitItem.Items, name);
                if (item != null)
                    return item;
            }

            return null;
        }

        private static BaseShellItem GetShellItem(IList<ShellSection> items, string name)
        {
            var shellItem = items.FirstOrDefault(x => !IsImplicitRoute(x) && UriParsingHelper.GetSegmentName(x.Route) == name);
            if (shellItem != null)
                return shellItem;

            var implicitItems = items.Where(x => IsImplicitRoute(x));
            foreach (var implicitItem in implicitItems)
            {
                var item = GetShellItem(implicitItem.Items, name);
                if (item != null)
                    return item;
            }

            return null;
        }

        private static BaseShellItem GetShellItem(IList<ShellContent> items, string name)
        {
            var shellItem = items.FirstOrDefault(x => !IsImplicitRoute(x) && UriParsingHelper.GetSegmentName(x.Route) == name);
            if (shellItem != null)
                return shellItem;

            return null;
        }

        private static bool IsImplicitRoute(BaseShellItem item) =>
            item.Route.StartsWith("IMPL_");

        //private class PrismPathPart : PathPart
        //{
        //    public PrismPathPart(BaseShellItem baseShellItem, string path, INavigationParameters parameters)
        //        : base(baseShellItem, null)
        //    {
        //        NavigationParameters = parameters;
                
        //        //"page?id=4"
        //        //Path == page
        //        Path = path;
        //    }

        //    public new INavigationParameters NavigationParameters { get; }

        //    public new string Path { get; }
        //}

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
