/*using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prism.Common;
using Prism.Ioc;
using Xamarin.Forms;
using System.Linq;
using Prism.Behaviors;

namespace Prism.Navigation.Shell
{
    public partial class PrismShellNavigationService : IShellNavigationService
    {
        private IContainerExtension _container { get; }
        private IPageBehaviorFactory _pageBehaviorFactory { get; }

        // Fourth
        public Task AppearingAsync(ShellLifecycleArgs args)
        {
            Console.WriteLine($"Route: {args.PathPart.ShellPart.Route}");
            return Task.CompletedTask;
        }

        // Third
        public void ApplyParameters(ShellLifecycleArgs args)
        {
            Console.WriteLine($"Route: {args.PathPart.ShellPart.Route}");
        }


        public Page Create(ShellContentCreateArgs args)
        {
            return CreatePageFromSegment(args.Content.Route);
        }

        // Second... About to Navigate... we're initiating the Navigating routine
        public async Task<ShellRouteState> NavigatingToAsync(ShellNavigationArgs args)
        {
            var pathPart = args.FutureState.CurrentRoute.GetCurrent();
            if(pathPart is PrismPathPart prismPathPart && prismPathPart.ShellPart is ShellContent shellContent)
            {
                
                var page = CreatePageFromSegment(prismPathPart.Path);
                

                shellContent.Content = page;
            }

            //args.FutureState.CurrentRoute.GetCurrent().

            // args.Shell.RouteState.
            var currentlyVisiblePage = args.Shell.RouteState.GetCurrentPage();

            return args.FutureState;
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

        private INavigationParameters _currentParameters;
        // First... validates URI
        public Task<ShellRouteState> ParseAsync(ShellUriParserArgs args)
        {
            var newState = RebuildCurrentState(args.Shell.CurrentState.Location, args.Shell.Items);
            //var currentUri = args.Shell.CurrentState.Location;
            var navigationSegments = UriParsingHelper.GetUriSegments(args.Uri);
            var newPathParts = navigationSegments.Select(x =>
            {
                var segmentName = UriParsingHelper.GetSegmentName(x);

                if (segmentName == RemovePageRelativePath && newState.Count > 0)
                {
                    // We might want to grab that last state object to do OnNavigatedFrom
                    newState.Remove(newState.Last());
                    return null;
                }

                var navigationParameters = UriParsingHelper.GetSegmentParameters(x, _currentParameters);
                var shellItem = GetShellItem(args.Shell.Items, segmentName);
                return new PrismPathPart(shellItem, segmentName, navigationParameters);
            });

            newState.AddRange(newPathParts.Where(x => x != null && x.ShellPart != null));

            var routePath = new RoutePath(newState, null);
            var routeState = new ShellRouteState(routePath);
            // Home/Demo

            // Routing.ImplicitPrefix = "IMPL_";
            //var item = args.Shell.Items.First(x => x.Route.StartsWith("IMPL_"));

            return Task.FromResult(routeState);
        }

        private static List<PathPart> RebuildCurrentState(Uri uri, IList<ShellItem> shellItems)
        {
            var pathParts = new List<PathPart>();


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

        private class PrismPathPart : PathPart
        {
            public PrismPathPart(BaseShellItem baseShellItem, string path, INavigationParameters parameters)
                : base(baseShellItem, null)
            {
                NavigationParameters = parameters;
                Path = path;
            }

            public new INavigationParameters NavigationParameters { get; }

            public new string Path { get; }
        }
    }

    internal static class ShellExtensions
    {
        public static ShellRouteState AddRange(this ShellRouteState routeState, IEnumerable<PathPart> pathParts)
        {
            foreach(var pathPart in pathParts)
            {
                if (pathPart is null || pathPart.ShellPart is null)
                    continue;

                routeState.Add(pathPart);
            }

            return routeState;
        }
    }
}
*/