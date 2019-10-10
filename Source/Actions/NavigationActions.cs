using System;
using System.IO;
using System.Linq;
using Xplorer.Models;
using Remutable.Extensions;

namespace Xplorer.Actions
{
    public static class NavigationActions
    {
        public static MasterModel CreateMasterModel(Context context)
        {
            context.PrimaryFileSystem.Navigate(null);

            var location1 = context.PrimaryFileSystem.Location;
            var entries1 = context.PrimaryFileSystem.Entries;
            var activeIndex1 = 0;
            var firstIndex1 = 0;
            entries1 = SliceVisibleEntries(entries1, activeIndex1, ref firstIndex1, out var activeNavigationEntry1);
            var items1 = Convert(entries1, activeNavigationEntry1);

            var entryList1 = new NavigationEntryListModel(firstIndex1, items1);
            var scrollbar1 = CalculateScrollbarModel(firstIndex1, entries1.Length, Layout.GetNavigationEntryListHeight());
            var statusbar1 = CreateStatusbarModel(null, location1, string.Empty);
            var navigation1 = new NavigationModel(null, location1, string.Empty, true, activeIndex1, activeNavigationEntry1, context.PrimaryFileSystem.Entries, statusbar1, entryList1, scrollbar1);

            context.SecondaryFileSystem.Navigate("C:\\");

            var location2 = context.SecondaryFileSystem.Location;
            var entries2 = context.SecondaryFileSystem.Entries;
            var activeIndex2 = 0;
            var firstIndex2 = 0;
            entries2 = SliceVisibleEntries(entries2, activeIndex2, ref firstIndex2, out var activeNavigationEntry2);
            var items2 = Convert(entries2, null);

            var entryList2 = new NavigationEntryListModel(firstIndex2, items2);
            var scrollbar2 = CalculateScrollbarModel(firstIndex2, entries2.Length, Layout.GetNavigationEntryListHeight());
            var statusbar2 = CreateStatusbarModel(null, location2, string.Empty);
            var navigation2 = new NavigationModel(null, location2, string.Empty, false, activeIndex2, activeNavigationEntry2, context.SecondaryFileSystem.Entries, statusbar2, entryList2, scrollbar2);

            var primaryNavigation = new PrimaryNavigationModel(navigation1);
            var secondaryNavigation = new SecondaryNavigationModel(navigation2);

            var toolbar = new ToolbarModel();
            var result = new MasterModel(primaryNavigation, secondaryNavigation, true, toolbar);

            return result;
        }

        public static MasterModel ToggleActiveNavigation(Context context, MasterModel model)
        {
            var i1 = model.PrimaryNavigation.Navigation.EntryList.Items.ToList().FindIndex(x => x.Entry == model.PrimaryNavigation.Navigation.ActiveNavigationEntry);
            var items1 = model.PrimaryNavigation.Navigation.EntryList.Items.Clone() as NavigationEntryListItemModel[];
            var item1 = items1[i1];
            item1 = item1.Remute(x => x.IsActive, !item1.IsActive);
            items1[i1] = item1;
            model = model.Remute(x => x.PrimaryNavigation.Navigation.EntryList.Items, items1);

            var i2 = model.SecondaryNavigation.Navigation.EntryList.Items.ToList().FindIndex(x => x.Entry == model.SecondaryNavigation.Navigation.ActiveNavigationEntry);
            var items2 = model.SecondaryNavigation.Navigation.EntryList.Items.Clone() as NavigationEntryListItemModel[];
            var item2 = items2[i2];
            item2 = item2.Remute(x => x.IsActive, !item2.IsActive);
            items2[i2] = item2;
            model = model.Remute(x => x.SecondaryNavigation.Navigation.EntryList.Items, items2);

            model = model.Remute(x => x.PrimaryNavigation.Navigation.IsActive, !model.PrimaryNavigation.Navigation.IsActive);
            model = model.Remute(x => x.SecondaryNavigation.Navigation.IsActive, !model.SecondaryNavigation.Navigation.IsActive);

            return model;
        }

        public static MasterModel Navigate(Context context, MasterModel model)
        {
            var fileSystem = GetActiveFileSystem(context, model);
            var navigation = GetActiveNavigationModel(model);

            var entry = navigation.ActiveNavigationEntry;
            var location = navigation.Location;
            
            try
            {
                fileSystem.Navigate(entry.Type == NavigationEntryType.NavUpControl ? null : entry.Name);
            }
            catch (Exception ex)
            {
                navigation = context.Remute.With(navigation, x => x.Message, ex.Message);
                return model;
            }

            if (fileSystem.Executed)
            {
                return model;
            }

            var entries = fileSystem.Entries;

            var activeIndex = 0;

            if (entry.Type == NavigationEntryType.NavUpControl)
            {
                var name = (fileSystem.Location == null) ? location : Path.GetFileName(location);
                var item = entries.SingleOrDefault(x => x.Name == name);
                if (item != null)
                {
                    activeIndex = entries.ToList().IndexOf(item);
                }
            }

            var firstIndex = 0;
            entries = SliceVisibleEntries(entries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            
            navigation = context.Remute.With(navigation, x => x.ActiveIndex, activeIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);
            navigation = context.Remute.With(navigation, x => x.NavigationEntries, fileSystem.Entries);
            navigation = context.Remute.With(navigation, x => x.Filter, string.Empty);
            navigation = context.Remute.With(navigation, x => x.Location, fileSystem.Location);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = context.Remute.With(navigation, x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.Statusbar, CreateStatusbarModel(navigation));

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
        }

        private static NavigationEntryListItemModel[] Convert(NavigationEntry[] entries, NavigationEntry activeNavigationEntry)
        {
            var result = new NavigationEntryListItemModel[entries.Length];

            for (var i = 0; i < entries.Length; i++)
            {
                var navigationEntry = entries[i];
                result[i] = new NavigationEntryListItemModel(entries[i], navigationEntry == activeNavigationEntry);
            }

            return result;
        }

        private static StatusbarModel CreateStatusbarModel(NavigationModel navigation)
        {
            return CreateStatusbarModel(navigation.Message, navigation.Location, navigation.Filter);
        }

        private static StatusbarModel CreateStatusbarModel(string message, string location, string filter)
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

        private static NavigationEntry[] SliceVisibleEntries(NavigationEntry[] entries, int activeIndex, ref int firstIndex, out NavigationEntry activeNavigationEntry)
        {
            var maxItemsCount = Layout.GetNavigationEntryListHeight();

            var lastIndex = firstIndex + maxItemsCount - 1;

            if (lastIndex >= entries.Length)
            {
                lastIndex = entries.Length - 1;
            }

            if (activeIndex < firstIndex)
            {
                firstIndex = activeIndex;
                lastIndex = firstIndex + maxItemsCount - 1;

                if (lastIndex >= entries.Length)
                {
                    lastIndex = entries.Length - 1;
                }
            }

            if (activeIndex > lastIndex)
            {
                lastIndex = activeIndex;
                firstIndex = lastIndex - maxItemsCount + 1;

                if (firstIndex < 0)
                {
                    firstIndex = 0;
                }
            }

            var index = activeIndex - firstIndex;
            activeNavigationEntry = entries[activeIndex];

            return entries.Skip(firstIndex).Take(lastIndex - firstIndex + 1).ToArray();
        }

        public static MasterModel MoveDown(Context context, MasterModel model)
        {
            var fileSystem = GetActiveFileSystem(context, model);
            var navigation = GetActiveNavigationModel(model);

            if (navigation.ActiveIndex == navigation.NavigationEntries.Length - 1)
            {
                return model;
            }

            var activeIndex = navigation.ActiveIndex + 1;
            var firstIndex = navigation.EntryList.FirstIndex;
            var entries = SliceVisibleEntries(navigation.NavigationEntries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            navigation = navigation.Remute(x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveIndex, activeIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
        }

        public static MasterModel MoveUp(Context context, MasterModel model)
        {
            var fileSystem = GetActiveFileSystem(context, model);
            var navigation = GetActiveNavigationModel(model);

            if (navigation.ActiveIndex == 0)
            {
                return model;
            }

            var activeIndex = navigation.ActiveIndex - 1;
            var firstIndex = navigation.EntryList.FirstIndex;
            var entries = SliceVisibleEntries(navigation.NavigationEntries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            navigation = navigation.Remute(x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveIndex, activeIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
        }

        public static MasterModel MoveLeft(Context context, MasterModel model)
        {
            var fileSystem = GetActiveFileSystem(context, model);
            var navigation = GetActiveNavigationModel(model);

            var activeIndex = 0;
            var firstIndex = navigation.EntryList.FirstIndex;
            var entries = SliceVisibleEntries(navigation.NavigationEntries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            navigation = navigation.Remute(x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveIndex, activeIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
        }

        public static MasterModel MoveRight(Context context, MasterModel model)
        {
            var fileSystem = GetActiveFileSystem(context, model);
            var navigation = GetActiveNavigationModel(model);

            var activeIndex = navigation.NavigationEntries.Length == 0 ? 0 : navigation.NavigationEntries.Length - 1;
            var firstIndex = navigation.EntryList.FirstIndex;
            var entries = SliceVisibleEntries(navigation.NavigationEntries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            navigation = navigation.Remute(x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveIndex, activeIndex);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
        }

        public static MasterModel Escape(Context context, MasterModel model)
        {
            var fileSystem = GetActiveFileSystem(context, model);
            var navigation = GetActiveNavigationModel(model);

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
            var fileSystem = GetActiveFileSystem(context, model);
            var navigation = GetActiveNavigationModel(model);

            if (navigation.Filter == string.Empty)
            {
                return NavigateUp(context, model, navigation, fileSystem);
            }
            else
            {
                return ReduceFilter(context, model);
            }
        }

        private static MasterModel NavigateUp(Context context, MasterModel model, NavigationModel navigation, IFileSystem fileSystem)
        {
            if (fileSystem.Entries.Length > 0 && fileSystem.Entries[0].Type == NavigationEntryType.NavUpControl)
            {
                navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, fileSystem.Entries[0]);
                model = SetActiveNavigationModel(context, model, navigation);
                model = Navigate(context, model);
            }

            return model;
        }

        public static MasterModel Reveal(Context context, MasterModel model)
        {
            var navigation = GetActiveNavigationModel(model);
            
            context.OperationSystem.Reveal(navigation);

            return model;
        }

        public static MasterModel ApplyFilter(Context context, MasterModel model, char value)
        {
            var navigation = GetActiveNavigationModel(model);
            var fileSystem = GetActiveFileSystem(context, model);

            var filter = navigation.Filter + value;
            var entries = fileSystem.Entries
                .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) || x.Type == NavigationEntryType.NavUpControl)
                .ToArray();
            var activeIndex = GetFilterActiveIndex(entries);
            var firstIndex = 0;
            entries = SliceVisibleEntries(entries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            navigation = navigation.Remute(x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = navigation.Remute(x => x.ActiveIndex, activeIndex);
            navigation = navigation.Remute(x => x.NavigationEntries, entries);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);
            navigation = context.Remute.With(navigation, x => x.Filter, filter);          

            var statusbar = CreateStatusbarModel(navigation);
            navigation = navigation.Remute(x => x.Statusbar, statusbar);

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
        }

        private static MasterModel ReduceFilter(Context context, MasterModel model)
        {
            var navigation = GetActiveNavigationModel(model);
            var fileSystem = GetActiveFileSystem(context, model);

            if (navigation.Filter == string.Empty)
            {
                return model;
            }

            var filter = navigation.Filter.Substring(0, navigation.Filter.Length - 1);
            var entries = fileSystem.Entries
                .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) || x.Type == NavigationEntryType.NavUpControl)
                .ToArray();
            var activeIndex = GetFilterActiveIndex(entries);
            var firstIndex = 0;
            entries = SliceVisibleEntries(entries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            navigation = navigation.Remute(x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = navigation.Remute(x => x.ActiveIndex, activeIndex);
            navigation = navigation.Remute(x => x.NavigationEntries, entries);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);
            navigation = context.Remute.With(navigation, x => x.Filter, filter);

            var statusbar = CreateStatusbarModel(navigation);
            navigation = navigation.Remute(x => x.Statusbar, statusbar);

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
        }

        private static MasterModel ClearFilter(Context context, MasterModel model)
        {
            var navigation = GetActiveNavigationModel(model);
            var fileSystem = GetActiveFileSystem(context, model);

            if (navigation.Filter == string.Empty)
            {
                return model;
            }

            var entries = fileSystem.Entries.ToArray();
            var activeIndex = 0;
            var firstIndex = 0;
            entries = SliceVisibleEntries(entries, activeIndex, ref firstIndex, out var entryListActiveIndex);
            var items = Convert(entries, entryListActiveIndex);
            navigation = navigation.Remute(x => x.EntryList.Items, items);
            navigation = navigation.Remute(x => x.EntryList.FirstIndex, firstIndex);
            navigation = navigation.Remute(x => x.ActiveIndex, activeIndex);
            navigation = navigation.Remute(x => x.NavigationEntries, entries);
            navigation = context.Remute.With(navigation, x => x.ActiveNavigationEntry, entryListActiveIndex);
            navigation = context.Remute.With(navigation, x => x.Filter, string.Empty);

            var statusbar = CreateStatusbarModel(navigation);
            navigation = navigation.Remute(x => x.Statusbar, statusbar);

            var scrollbar = CalculateScrollbarModel(firstIndex, navigation.NavigationEntries.Length, Layout.GetNavigationEntryListHeight());
            navigation = navigation.Remute(x => x.Scrollbar, scrollbar);

            return SetActiveNavigationModel(context, model, navigation);
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

        private static IFileSystem GetActiveFileSystem(Context context, MasterModel model)
        {
            return model.PrimaryNavigation.Navigation.IsActive
                ? context.PrimaryFileSystem
                : context.SecondaryFileSystem;
        }

        private static NavigationModel GetActiveNavigationModel(MasterModel model)
        {
            return model.PrimaryNavigation.Navigation.IsActive
                ? model.PrimaryNavigation.Navigation
                : model.SecondaryNavigation.Navigation;
        }

        private static MasterModel SetActiveNavigationModel(Context context, MasterModel model, NavigationModel navigation)
        {
            if (model.PrimaryNavigation.Navigation.IsActive)
            {
                return context.Remute.With(model, x => x.PrimaryNavigation.Navigation, navigation);
            }
            else
            {
                return context.Remute.With(model, x => x.SecondaryNavigation.Navigation, navigation);
            }
        }

        public static string GetActiveLocation(NavigationModel model)
        {
            var result = model.Location;
            var entry = model.EntryList.Items[model.ActiveIndex].Entry;

            if (model.Location != null && entry != null)
            {
                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    result = Path.Combine(model.Location, entry.Name);
                }
            }

            return result;
        }

        private static ScrollbarModel CalculateScrollbarModel(int windowPosition, int contentSize, int windowSize)
        {
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

            var windowPositionRatio = (float)windowPosition / windowScrollAreaSize;
            var trackScrollAreaSize = trackSize - gripSize;
            var start = (int)(trackScrollAreaSize * windowPositionRatio);
            var end = start + gripSize;

            return new ScrollbarModel(start, end);
        }
    }
}