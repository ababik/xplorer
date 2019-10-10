using System;
using System.IO;
using System.Linq;
using Remutable.Extensions;
using Xplorer.Models;

namespace Xplorer.Actions
{
    public static class NavigationActions
    {
        public static MasterModel CreateModel(Context context)
        {
            static NavigationModel CreateNavigation(IFileSystem fileSystem, bool isActive)
            {
                var activeIndex = 0;
                var firstIndex = 0;
                var message = null as string;
                var filter = string.Empty;
                var visibleEntryList = CreateVisibleEntryList(fileSystem.Entries, activeIndex, firstIndex, out var activeEntry, isActive);
                var scrollbar = CreateScrollbar(visibleEntryList.FirstIndex, fileSystem.Entries.Length);
                var statusbar = CreateStatusbar(message, fileSystem.Location, filter);
                return new NavigationModel(message, fileSystem.Location, filter, isActive, activeIndex, activeEntry, fileSystem.Entries, visibleEntryList, statusbar, scrollbar);
            }
            
            context.PrimaryFileSystem.Navigate(null);
            context.SecondaryFileSystem.Navigate("C:\\");

            var isPrimaryActive = true;
            var isSocondaryVisible = true;

            var primaryNavigation = CreateNavigation(context.PrimaryFileSystem, isPrimaryActive);
            var secondaryNavigation = CreateNavigation(context.SecondaryFileSystem, !isPrimaryActive);
            var toolbar = CreateToolbar();

            return new MasterModel(primaryNavigation, secondaryNavigation, isSocondaryVisible, toolbar);
        }

        public static MasterModel Navigate(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            var entry = navigation.ActiveEntry;
            var location = navigation.Location;
            var activeIndex = 0;
            var firstIndex = 0;
            var message = null as string;
            var filter = string.Empty;
            
            try
            {
                fileSystem.Navigate(entry.Type == NavigationEntryType.NavUpControl ? null : entry.Name);
            }
            catch (Exception ex)
            {
                navigation = navigation.Remute(x => x.Message, ex.Message);
                return SetActiveNavigation(model, navigation);
            }

            if (fileSystem.Executed)
            {
                return model;
            }

            if (entry.Type == NavigationEntryType.NavUpControl)
            {
                var name = (fileSystem.Location == null) ? location : Path.GetFileName(location);
                var item = fileSystem.Entries.SingleOrDefault(x => x.Name == name);
                if (item != null)
                {
                    activeIndex = fileSystem.Entries.ToList().IndexOf(item);
                }
            }

            var visibleEntryList = CreateVisibleEntryList(fileSystem.Entries, activeIndex, firstIndex, out var activeEntry);
            var scrollbar = CreateScrollbar(visibleEntryList.FirstIndex, fileSystem.Entries.Length);
            var statusbar = CreateStatusbar(message, fileSystem.Location, filter);

            navigation = navigation
                .Remute(x => x.Message, message)
                .Remute(x => x.Filter, filter)
                .Remute(x => x.ActiveIndex, activeIndex)
                .Remute(x => x.ActiveEntry, activeEntry)
                .Remute(x => x.Entries, fileSystem.Entries)
                .Remute(x => x.Location, fileSystem.Location)
                .Remute(x => x.VisibleEntryList, visibleEntryList)
                .Remute(x => x.Scrollbar, scrollbar)
                .Remute(x => x.Statusbar, statusbar);
            
            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel MoveDown(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            if (navigation.ActiveIndex == navigation.Entries.Length - 1)
            {
                return model;
            }

            var activeIndex = navigation.ActiveIndex + 1;
            navigation = Cursor(navigation, activeIndex);
            
            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel MoveUp(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            if (navigation.ActiveIndex == 0)
            {
                return model;
            }

            var activeIndex = navigation.ActiveIndex - 1;
            navigation = Cursor(navigation, activeIndex);

            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel MoveLeft(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            var activeIndex = 0;
            navigation = Cursor(navigation, activeIndex);

            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel MoveRight(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            var activeIndex = navigation.Entries.Length == 0 ? 0 : navigation.Entries.Length - 1;
            navigation = Cursor(navigation, activeIndex);

            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel ApplyFilter(Context context, MasterModel model, char value)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            var filter = navigation.Filter + value;
            navigation = Filter(navigation, fileSystem, filter);

            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel ReduceFilter(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            if (navigation.Filter == string.Empty)
            {
                return model;
            }

            var filter = navigation.Filter.Substring(0, navigation.Filter.Length - 1);
            navigation = Filter(navigation, fileSystem, filter);

            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel ClearFilter(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            if (navigation.Filter == string.Empty)
            {
                return model;
            }

            var entries = fileSystem.Entries;
            var activeIndex = 0;
            var firstIndex = 0;
            var message = null as string;
            var filter = string.Empty;

            var visibleEntryList = CreateVisibleEntryList(entries, activeIndex, firstIndex, out var activeEntry);
            var scrollbar = CreateScrollbar(visibleEntryList.FirstIndex, entries.Length);
            var statusbar = CreateStatusbar(message, fileSystem.Location, filter);

            navigation = navigation
                .Remute(x => x.Message, message)
                .Remute(x => x.Filter, filter)
                .Remute(x => x.ActiveIndex, activeIndex)
                .Remute(x => x.ActiveEntry, activeEntry)
                .Remute(x => x.Entries, entries)
                .Remute(x => x.VisibleEntryList, visibleEntryList)
                .Remute(x => x.Scrollbar, scrollbar)
                .Remute(x => x.Statusbar, statusbar);
            
            return SetActiveNavigation(model, navigation);
        }

        public static MasterModel Escape(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            if (navigation.Filter == string.Empty)
            {
                return NavigateUp(context, model, navigation, fileSystem);
            }
            else
            {
                return ClearFilter(context, model);
            }
        }

        public static MasterModel Back(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            if (navigation.Filter == string.Empty)
            {
                return NavigateUp(context, model, navigation, fileSystem);
            }
            else
            {
                return ReduceFilter(context, model);
            }
        }

        public static MasterModel Reveal(Context context, MasterModel model)
        {
            var navigation = GetActiveNavigation(model);
            
            context.OperationSystem.Reveal(navigation);

            return model;
        }

        public static MasterModel ToggleActiveNavigation(Context context, MasterModel model)
        {
            return model;
        }

        public static string GetActiveLocation(NavigationModel navigation)
        {
            var entry = navigation.ActiveEntry;
            var result = navigation.Location;

            if (navigation.Location != null && entry != null)
            {
                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    result = Path.Combine(navigation.Location, entry.Name);
                }
            }

            return result;
        }

        private static MasterModel NavigateUp(Context context, MasterModel model, NavigationModel navigation, IFileSystem fileSystem)
        {
            if (fileSystem.Entries.Length > 0 && fileSystem.Entries[0].Type == NavigationEntryType.NavUpControl)
            {
                navigation = navigation.Remute(x => x.ActiveEntry, fileSystem.Entries[0]);
                model = SetActiveNavigation(model, navigation);
                model = Navigate(context, model);
            }

            return model;
        }
        
        private static NavigationModel Cursor(NavigationModel navigation, int activeIndex)
        {
            var visibleEntryList = CreateVisibleEntryList(navigation.Entries, activeIndex, navigation.VisibleEntryList.FirstIndex, out var activeEntry);
            var scrollbar = CreateScrollbar(visibleEntryList.FirstIndex, navigation.Entries.Length);

            navigation = navigation
                .Remute(x => x.Message, null)
                .Remute(x => x.ActiveIndex, activeIndex)
                .Remute(x => x.ActiveEntry, activeEntry)
                .Remute(x => x.VisibleEntryList, visibleEntryList)
                .Remute(x => x.Scrollbar, scrollbar);
            
            return navigation;
        }

        private static NavigationModel Filter(NavigationModel navigation, IFileSystem fileSystem, string filter)
        {
            var message = null as string;
            var entries = fileSystem.Entries
                .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) || x.Type == NavigationEntryType.NavUpControl)
                .ToArray();
            var activeIndex = GetFilterActiveIndex(entries);
            
            var visibleEntryList = CreateVisibleEntryList(entries, activeIndex, navigation.VisibleEntryList.FirstIndex, out var activeEntry);
            var scrollbar = CreateScrollbar(visibleEntryList.FirstIndex, entries.Length);
            var statusbar = CreateStatusbar(message, navigation.Location, filter);

            navigation = navigation
                .Remute(x => x.Message, message)
                .Remute(x => x.Filter, filter)
                .Remute(x => x.ActiveIndex, activeIndex)
                .Remute(x => x.ActiveEntry, activeEntry)
                .Remute(x => x.Entries, entries)
                .Remute(x => x.VisibleEntryList, visibleEntryList)
                .Remute(x => x.Scrollbar, scrollbar)
                .Remute(x => x.Statusbar, statusbar);
            
            return navigation;
        }
        
        private static int GetFilterActiveIndex(NavigationEntry[] entries)
        {
            var index = 0;

            if (entries.Length >= 2 && entries[0].Type == NavigationEntryType.NavUpControl)
            {
                index = 1;
            }
            
            return index;
        }

        private static void GetActive(Context context, MasterModel model, out IFileSystem fileSystem, out NavigationModel navigation)
        {
            fileSystem = GetActiveFileSystem(context, model);
            navigation = GetActiveNavigation(model);
        }

        private static IFileSystem GetActiveFileSystem(Context context, MasterModel model)
        {
            return model.PrimaryNavigation.IsActive
                ? context.PrimaryFileSystem
                : context.SecondaryFileSystem;
        }

        private static NavigationModel GetActiveNavigation(MasterModel model)
        {
            return model.PrimaryNavigation.IsActive
                ? model.PrimaryNavigation
                : model.SecondaryNavigation;
        }

        private static MasterModel SetActiveNavigation(MasterModel model, NavigationModel navigation)
        {
            if (model.PrimaryNavigation.IsActive)
            {
                return model.Remute(x => x.PrimaryNavigation, navigation);
            }
            else
            {
                return model.Remute(x => x.SecondaryNavigation, navigation);
            }
        }
        
        private static NavigationEntryListModel CreateVisibleEntryList(NavigationEntry[] entries, int activeIndex, int firstIndex, out NavigationEntry activeEntry, bool isActive = true)
        {
            var height = Layout.GetNavigationEntryListHeight();

            var lastIndex = firstIndex + height - 1;

            if (lastIndex >= entries.Length)
            {
                lastIndex = entries.Length - 1;
            }

            if (activeIndex < firstIndex)
            {
                firstIndex = activeIndex;
                lastIndex = firstIndex + height - 1;

                if (lastIndex >= entries.Length)
                {
                    lastIndex = entries.Length - 1;
                }
            }

            if (activeIndex > lastIndex)
            {
                lastIndex = activeIndex;
                firstIndex = lastIndex - height + 1;

                if (firstIndex < 0)
                {
                    firstIndex = 0;
                }
            }

            activeEntry = entries[activeIndex];

            entries = entries
                .Skip(firstIndex)
                .Take(lastIndex - firstIndex + 1)
                .ToArray();

            var items = new NavigationEntryListItemModel[entries.Length];

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                items[i] = new NavigationEntryListItemModel(entries[i], isActive && entry == activeEntry);
            }

            return new NavigationEntryListModel(firstIndex, items);
        }

        private static ScrollbarModel CreateScrollbar(int firstIndex, int contentSize)
        {
            var windowSize = Layout.GetNavigationEntryListHeight();
            var trackSize = windowSize;
            var windowContentRatio = (float)windowSize / contentSize;
            var gripSize = (int)(trackSize * windowContentRatio);

            var minimalGripSize = 1;
            var maximumGripSize = trackSize;

            if (gripSize < minimalGripSize)
            {
                gripSize = minimalGripSize;
            }

            if (gripSize > maximumGripSize)
            {
                gripSize = maximumGripSize;
            }

            var windowScrollAreaSize = contentSize - windowSize;
            if (windowScrollAreaSize <= 0)
            {
                windowScrollAreaSize = windowSize;
            }

            var windowPositionRatio = (float)firstIndex / windowScrollAreaSize;
            var trackScrollAreaSize = trackSize - gripSize;
            var start = (int)(trackScrollAreaSize * windowPositionRatio);
            var end = start + gripSize;

            return new ScrollbarModel(start, end);
        }

        private static StatusbarModel CreateStatusbar(string message, string location, string filter)
        {
            if (message == null)
            {
                message = location ?? System.Environment.UserName + "@" + System.Environment.MachineName;

                if (filter != string.Empty)
                {
                    message = $"Filter: {filter} (ESC to clear)";
                }
            }

            return new StatusbarModel(message);
        }

        private static ToolbarModel CreateToolbar()
        {
            return null;
        }
    }
}