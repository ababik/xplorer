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
                var activeEntry = fileSystem.Entries[0];
                var statusbar = new StatusbarModel(null);
                var scrollbar = new ScrollbarModel(isActive, 0, 0);
                var navigation = new NavigationModel(fileSystem.Location, string.Empty, string.Empty, isActive, 0, 0, activeEntry, fileSystem.Entries, null, statusbar, scrollbar);
                navigation = UpdateNavigation(navigation);
                return navigation;
            }
            
            context.PrimaryFileSystem.Navigate(null);
            context.SecondaryFileSystem.Navigate(null);

            var primaryNavigation = CreateNavigation(context.PrimaryFileSystem, true);
            var secondaryNavigation = CreateNavigation(context.SecondaryFileSystem, false);
            var toolbar = null as ToolbarModel;

            return new MasterModel(LayoutMode.PrimaryNavigation, primaryNavigation, secondaryNavigation, toolbar);
        }

        public static MasterModel Navigate(Context context, MasterModel model)
        {
            GetActive(context, model, out var fileSystem, out var navigation);

            var entry = navigation.ActiveEntry;
            var location = navigation.Location;
            var activeIndex = 0;

            if (entry.Type == NavigationEntryType.NavUpControl)
            {
                if (model.PrimaryNavigation.IsActive && model.LayoutMode.HasFlag(LayoutMode.SecondaryNavigation))
                {
                    var layoutMode = model.LayoutMode;
                    layoutMode = layoutMode &= ~LayoutMode.SecondaryNavigation;
                    model = model.Remute(x => x.LayoutMode, layoutMode);
                }
            }
            
            try
            {
                fileSystem.Navigate(entry.Type == NavigationEntryType.NavUpControl ? null : entry.Name);
            }
            catch (Exception ex)
            {
                navigation = navigation.Remute(x => x.Message, ex.Message);
                navigation = UpdateStatusbar(navigation);
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

            navigation = navigation
                .Remute(x => x.Location, fileSystem.Location)
                .Remute(x => x.Message, string.Empty)
                .Remute(x => x.Filter, string.Empty)
                .Remute(x => x.ActiveIndex, activeIndex)
                .Remute(x => x.FirstIndex, 0)
                .Remute(x => x.ActiveEntry, fileSystem.Entries[activeIndex])
                .Remute(x => x.Entries, fileSystem.Entries);
            
            navigation = UpdateNavigation(navigation);
            
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

            navigation = navigation
                .Remute(x => x.Message, string.Empty)
                .Remute(x => x.Filter, string.Empty)
                .Remute(x => x.ActiveIndex, 0)
                .Remute(x => x.FirstIndex, 0)
                .Remute(x => x.ActiveEntry, navigation.Entries[0])
                .Remute(x => x.Entries, fileSystem.Entries);
            
            navigation = UpdateNavigation(navigation);
            
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
            if (model.LayoutMode.HasFlag(LayoutMode.SecondaryNavigation) == false)
            {   
                var layoutMode = model.LayoutMode;
                layoutMode = layoutMode |= LayoutMode.SecondaryNavigation;
                model = model.Remute(x => x.LayoutMode, layoutMode);
            }

            var isPrimaryActive = model.PrimaryNavigation.IsActive;

            model = model
                .Remute(x => x.PrimaryNavigation.IsActive, !isPrimaryActive)
                .Remute(x => x.PrimaryNavigation.Scrollbar.Visible, !isPrimaryActive)
                .Remute(x => x.SecondaryNavigation.IsActive, isPrimaryActive)
                .Remute(x => x.SecondaryNavigation.Scrollbar.Visible, isPrimaryActive);

            var primaryNavigation = UpdateNavigation(model.PrimaryNavigation);
            var secondaryNavigation = UpdateNavigation(model.SecondaryNavigation);

            model = model
                .Remute(x => x.PrimaryNavigation, primaryNavigation)
                .Remute(x => x.SecondaryNavigation, secondaryNavigation);

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

        public static MasterModel Resize(Context context, MasterModel model)
        {
            var primaryNavigation = model.PrimaryNavigation.Remute(x => x.VisibleItems, null);
            primaryNavigation = UpdateNavigation(primaryNavigation);

            var secondaryNavigation = model.SecondaryNavigation.Remute(x => x.VisibleItems, null);
            secondaryNavigation = UpdateNavigation(secondaryNavigation);

            model = model
                .Remute(x => x.PrimaryNavigation, primaryNavigation)
                .Remute(x => x.SecondaryNavigation, secondaryNavigation);
 
            return model;
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
            navigation = navigation
                .Remute(x => x.Message, string.Empty)
                .Remute(x => x.ActiveIndex, activeIndex)
                .Remute(x => x.ActiveEntry, navigation.Entries[activeIndex]);
            navigation = UpdateNavigation(navigation);
            return navigation;
        }

        private static NavigationModel Filter(NavigationModel navigation, IFileSystem fileSystem, string filter)
        {
            var entries = fileSystem.Entries
                .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) || x.Type == NavigationEntryType.NavUpControl)
                .ToArray();
            var activeIndex = GetFilterActiveIndex(entries);
            
            navigation = navigation
                .Remute(x => x.Message, string.Empty)
                .Remute(x => x.Filter, filter)
                .Remute(x => x.ActiveIndex, activeIndex)
                .Remute(x => x.ActiveEntry, entries[activeIndex])
                .Remute(x => x.Entries, entries);

            navigation = UpdateNavigation(navigation);
            
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
        
        private static NavigationModel UpdateNavigation(NavigationModel navigation)
        {
            navigation = UpdateVisibleItems(navigation);
            navigation = UpdateScrollbar(navigation);
            navigation = UpdateStatusbar(navigation);
            return navigation;
        }

        private static NavigationModel UpdateVisibleItems(NavigationModel navigation)
        {
            var height = Layout.GetNavigationEntryListHeight();
            var activeIndex = navigation.ActiveIndex;
            var firstIndex = navigation.FirstIndex;
            var entries = navigation.Entries;

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

            var visibleEntries = entries
                .Skip(firstIndex)
                .Take(lastIndex - firstIndex + 1)
                .ToArray();
            
            var prevItems = navigation.VisibleItems ?? new NavigationItemModel[height];
            var nextItems = new NavigationItemModel[prevItems.Length];

            for (var i = 0; i < prevItems.Length; i++)
            {
                var entry = i < visibleEntries.Length ? visibleEntries[i] : null;
                var isActive = navigation.IsActive && entry == navigation.ActiveEntry;
                
                if (entry == null)
                {
                    nextItems[i] = null;
                    continue;
                }

                if (prevItems[i] == null)
                {
                    nextItems[i] = new NavigationItemModel(entry, isActive);
                    continue;
                }

                nextItems[i] = prevItems[i]
                    .Remute(x => x.Entry, entry)
                    .Remute(x => x.IsActive, isActive);
            }
            
            navigation = navigation
                .Remute(x => x.FirstIndex, firstIndex)
                .Remute(x => x.VisibleItems, nextItems);
            
            return navigation;
        }

        private static NavigationModel UpdateScrollbar(NavigationModel navigation)
        {
            var firstIndex = navigation.FirstIndex;
            var contentSize = navigation.Entries.Length;
            var windowSize = navigation.VisibleItems.Length;

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

            navigation = navigation
                .Remute(x => x.Scrollbar.GripStartPosition, start)
                .Remute(x => x.Scrollbar.GripEndPosition, end);

            return navigation;
        }

        private static NavigationModel UpdateStatusbar(NavigationModel navigation)
        {
            var message = navigation.Message;
            var location = navigation.Location;
            var filter = navigation.Filter;

            if (message == string.Empty)
            {
                message = location ?? System.Environment.UserName + "@" + System.Environment.MachineName;

                if (filter != string.Empty)
                {
                    message = $"Filter: {filter} (ESC to clear)";
                }
            }

            navigation = navigation
                .Remute(x => x.Statusbar.Status, message);

            return navigation;
        }
    }
}