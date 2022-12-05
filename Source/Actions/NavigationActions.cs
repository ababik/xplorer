using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Remutable.Extensions;
using Xplorer.Models;
using Xplorer.States;

namespace Xplorer.Actions;

public static class NavigationActions
{
    public static MasterState Init(Context context)
    {
        static Navigation CreateNavigation(IFileSystem fileSystem, bool isActive)
        {
            return new Navigation()
            {
                Location = fileSystem.Location,
                Message = string.Empty,
                Filter = string.Empty,
                IsActive = isActive,
                ActiveIndex = 0,
                FirstIndex = 0,
                ActiveEntry = fileSystem.Entries[0],
                SelectedEntries = new HashSet<NavigationEntry>(),
                ContentEntries = fileSystem.Entries
            };
        }

        static NavigationState CreateNavigationState(IFileSystem fileSystem, Navigation navigation, bool isActive)
        {
            var state = CreateDefaultNavigationState(isActive);
            state = UpdateNavigation(navigation, state);
            return state;
        }

        context.PrimaryFileSystem.Navigate(null);
        context.SecondaryFileSystem.Navigate(null);

        context.Model = new Model();
        context.Model.Layout = Layout.PrimaryNavigation;
        context.Model.PrimaryNavigation = CreateNavigation(context.PrimaryFileSystem, true);
        context.Model.SecondaryNavigation = CreateNavigation(context.SecondaryFileSystem, false);

        var primaryNavigation = CreateNavigationState(context.PrimaryFileSystem, context.Model.PrimaryNavigation, true);
        var secondaryNavigation = null as NavigationState;
        var toolbar = CreateDefaultToolbarState();

        return new MasterState(primaryNavigation, secondaryNavigation, toolbar);
    }

    public static MasterState RefreshNavigations(Context context, MasterState state)
    {
        NavigationState Refresh(IFileSystem fileSystem, Navigation navigation, NavigationState navigationState)
        {
            fileSystem.Navigate(fileSystem.Location);
            var entries = fileSystem.Entries;
            if (navigation.Filter != string.Empty)
            {
                entries = entries
                    .Where(x => x.Name.Contains(navigation.Filter, StringComparison.OrdinalIgnoreCase) || x.Type == NavigationEntryType.NavUpControl)
                    .ToArray();
            }

            navigation.ContentEntries = entries;
            if (navigation.ActiveIndex >= navigation.ContentEntries.Length)
            {
                navigation.ActiveIndex = 0;
                navigation.ActiveEntry = navigation.ContentEntries[navigation.ActiveIndex];
            }
            navigationState = UpdateNavigation(navigation, navigationState);
            return navigationState;
        }

        var primaryNavigationState = Refresh(context.PrimaryFileSystem, context.Model.PrimaryNavigation, state.PrimaryNavigation);
        var secondaryNavigationState = Refresh(context.SecondaryFileSystem, context.Model.SecondaryNavigation, state.SecondaryNavigation);

        state = state
            .Remute(x => x.PrimaryNavigation, primaryNavigationState)
            .Remute(x => x.SecondaryNavigation, secondaryNavigationState);
        
        return state;
    }

    public static MasterState Navigate(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        var entry = navigation.ActiveEntry;
        var location = navigation.Location;
        var activeIndex = 0;

        try
        {
            fileSystem.Navigate(entry.Type == NavigationEntryType.NavUpControl ? null : entry.Name);
        }
        catch (Exception ex)
        {
            navigation.Message = ex.Message;
            navigationState = UpdateStatusbar(navigation, navigationState);
            return SetActiveNavigationState(context, state, navigationState);
        }

        if (fileSystem.Executed)
        {
            return state;
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

        navigation.Location = fileSystem.Location;
        navigation.Message = string.Empty;
        navigation.Filter = string.Empty;
        navigation.ActiveIndex = activeIndex;
        navigation.FirstIndex = 0;
        navigation.ActiveEntry = fileSystem.Entries[activeIndex];
        navigation.SelectedEntries.Clear();
        navigation.ContentEntries = fileSystem.Entries;

        navigationState = UpdateNavigation(navigation, navigationState);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState MoveDown(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.ActiveIndex == navigation.ContentEntries.Length - 1)
        {
            return state;
        }

        var activeIndex = navigation.ActiveIndex + 1;
        navigationState = Cursor(navigation, navigationState, activeIndex);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState MoveUp(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.ActiveIndex == 0)
        {
            return state;
        }

        var activeIndex = navigation.ActiveIndex - 1;
        navigationState = Cursor(navigation, navigationState, activeIndex);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState MoveLeft(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        var activeIndex = 0;
        navigationState = Cursor(navigation, navigationState, activeIndex);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState MoveRight(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        var activeIndex = navigation.ContentEntries.Length == 0 ? 0 : navigation.ContentEntries.Length - 1;
        navigationState = Cursor(navigation, navigationState, activeIndex);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState ApplyFilter(Context context, MasterState state, char value)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (fileSystem.Location == null)
        {
            return state;
        }

        var filter = navigation.Filter + value;
        navigationState = Filter(navigation, navigationState, fileSystem, filter);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState ReduceFilter(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.Filter == string.Empty)
        {
            return state;
        }

        var filter = navigation.Filter.Substring(0, navigation.Filter.Length - 1);
        navigationState = Filter(navigation, navigationState, fileSystem, filter);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState ClearFilter(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.Filter == string.Empty)
        {
            return state;
        }

        navigation.Message = string.Empty;
        navigation.Filter = string.Empty;
        navigation.ActiveIndex = 0;
        navigation.FirstIndex = 0;
        navigation.ActiveEntry = navigation.ContentEntries[0];
        navigation.ContentEntries = fileSystem.Entries;

        navigationState = UpdateNavigation(navigation, navigationState);

        return SetActiveNavigationState(context, state, navigationState);
    }

    public static MasterState Escape(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.Filter == string.Empty)
        {
            return NavigateUp(context, state, fileSystem, navigation);
        }
        else
        {
            return ClearFilter(context, state);
        }
    }

    public static MasterState Back(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.Filter == string.Empty)
        {
            return NavigateUp(context, state, fileSystem, navigation);
        }
        else
        {
            return ReduceFilter(context, state);
        }
    }

    public static MasterState Reveal(Context context, MasterState state)
    {
        var navigation = GetActiveNavigation(context);

        context.OperationSystem.Reveal(navigation);

        return state;
    }

    public static string GetActiveLocation(Navigation navigation)
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

    public static MasterState RevertSelectedItems(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.Location == null)
        {
            return state;
        }

        foreach (var entry in navigation.ContentEntries)
        {
            if (entry.Type == NavigationEntryType.NavUpControl)
            {
                continue;
            }

            if (navigation.SelectedEntries.Contains(entry))
            {
                navigation.SelectedEntries.Remove(entry);
            }
            else
            {
                navigation.SelectedEntries.Add(entry);
            }
        }

        navigationState = UpdateItems(navigation, navigationState);

        state = SetActiveNavigationState(context, state, navigationState);

        return state;
    }

    public static MasterState ToggleSelectedItem(Context context, MasterState state)
    {
        GetActive(context, state, out var fileSystem, out var navigation, out var navigationState);

        if (navigation.Location == null)
        {
            return state;
        }

        if (navigation.ActiveEntry.Type == NavigationEntryType.NavUpControl)
        {
            state = MoveDown(context, state);
            return state;
        }

        if (navigation.SelectedEntries.Contains(navigation.ActiveEntry))
        {
            navigation.SelectedEntries.Remove(navigation.ActiveEntry);
        }
        else
        {
            navigation.SelectedEntries.Add(navigation.ActiveEntry);
        }

        if (navigation.ActiveIndex == navigation.ContentEntries.Length - 1)
        {
            navigationState = UpdateItems(navigation, navigationState);
            state = SetActiveNavigationState(context, state, navigationState);
        }
        else
        {
            state = MoveDown(context, state);
        }

        return state;
    }

    public static MasterState ToggleSecondaryNavigation(Context context, MasterState state)
    {
        if (context.Model.Layout.HasFlag(Layout.SecondaryNavigation))
        {
            if (context.Model.SecondaryNavigation.IsActive)
            {
                context.Model.Layout = context.Model.Layout.UnsetFlag<Layout>(Layout.SecondaryNavigation);
                state = ToggleActiveNavigation(context, state, true);
            }
            else
            {
                state = ToggleActiveNavigation(context, state, false);
            }
        }
        else
        {
            context.Model.Layout = context.Model.Layout.ToggleFlag<Layout>(Layout.SecondaryNavigation);
            state = ToggleActiveNavigation(context, state, false);
        }

        return state;
    }

    public static MasterState TogglePrimaryNavigation(Context context, MasterState state)
    {
        return ToggleActiveNavigation(context, state, true);
    }

    private static MasterState ToggleActiveNavigation(Context context, MasterState state, bool isPrimaryActive)
    {
        context.Model.PrimaryNavigation.IsActive = isPrimaryActive;
        context.Model.SecondaryNavigation.IsActive = !isPrimaryActive;

        var primaryNavigation = UpdateNavigation(context.Model.PrimaryNavigation, state.PrimaryNavigation);
        var secondaryNavigation = null as NavigationState;

        if (context.Model.Layout.HasFlag(Layout.SecondaryNavigation))
        {
            secondaryNavigation = CreateDefaultNavigationState(false);
            secondaryNavigation = UpdateNavigation(context.Model.SecondaryNavigation, secondaryNavigation);
        }

        state = state
            .Remute(x => x.PrimaryNavigation, primaryNavigation)
            .Remute(x => x.SecondaryNavigation, secondaryNavigation);

        state = state
            .Remute(x => x.PrimaryNavigation.Scrollbar.Visible, isPrimaryActive);

        if (state.SecondaryNavigation != null)
        {
            state = state
                .Remute(x => x.SecondaryNavigation.Scrollbar.Visible, !isPrimaryActive);
        }

        return state;
    }

    public static MasterState Resize(Context context, MasterState state)
    {
        var primaryNavigation = state.PrimaryNavigation.Remute(x => x.Items, null);
        primaryNavigation = UpdateNavigation(context.Model.PrimaryNavigation, primaryNavigation);

        var secondaryNavigation = state.SecondaryNavigation.Remute(x => x.Items, null);
        secondaryNavigation = UpdateNavigation(context.Model.SecondaryNavigation, secondaryNavigation);

        state = state
            .Remute(x => x.PrimaryNavigation, primaryNavigation)
            .Remute(x => x.SecondaryNavigation, secondaryNavigation);

        return state;
    }

    private static MasterState NavigateUp(Context context, MasterState state, IFileSystem fileSystem, Navigation navigation)
    {
        if (fileSystem.Entries.Length > 0 && fileSystem.Entries[0].Type == NavigationEntryType.NavUpControl)
        {
            navigation.ActiveEntry = fileSystem.Entries[0];
            state = Navigate(context, state);
        }

        return state;
    }

    public static void GetActive(Context context, MasterState state, out IFileSystem fileSystem, out Navigation navigation, out NavigationState navigationState)
    {
        var model = context.Model;
        fileSystem = GetActiveFileSystem(context);
        navigation = GetActiveNavigation(context);
        navigationState = GetActiveNavigationState(context, state);
    }

    public static IFileSystem GetActiveFileSystem(Context context)
    {
        return context.Model.PrimaryNavigation.IsActive
            ? context.PrimaryFileSystem
            : context.SecondaryFileSystem;
    }

    public static Navigation GetActiveNavigation(Context context)
    {
        return context.Model.PrimaryNavigation.IsActive
            ? context.Model.PrimaryNavigation
            : context.Model.SecondaryNavigation;
    }

    public static NavigationState GetActiveNavigationState(Context context, MasterState state)
    {
        return context.Model.PrimaryNavigation.IsActive
            ? state.PrimaryNavigation
            : state.SecondaryNavigation;
    }

    public static MasterState SetActiveNavigationState(Context context, MasterState masterState, NavigationState navigationState)
    {
        if (context.Model.PrimaryNavigation.IsActive)
        {
            return masterState.Remute(x => x.PrimaryNavigation, navigationState);
        }
        else
        {
            return masterState.Remute(x => x.SecondaryNavigation, navigationState);
        }
    }

    private static NavigationState Cursor(Navigation navigation, NavigationState state, int activeIndex)
    {
        navigation.Message = string.Empty;
        navigation.ActiveIndex = activeIndex;
        navigation.ActiveEntry = navigation.ContentEntries[activeIndex];

        state = UpdateNavigation(navigation, state);

        return state;
    }

    private static NavigationState Filter(Navigation navigation, NavigationState state, IFileSystem fileSystem, string filter)
    {
        var entries = fileSystem.Entries
            .Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) || x.Type == NavigationEntryType.NavUpControl)
            .ToArray();

        var activeIndex = GetFilterActiveIndex(entries);

        navigation.Message = string.Empty;
        navigation.Filter = filter;
        navigation.ActiveIndex = activeIndex;
        navigation.ActiveEntry = entries[activeIndex];
        navigation.ContentEntries = entries;

        state = UpdateNavigation(navigation, state);

        return state;
    }

    public static NavigationState UpdateNavigation(Navigation navigation, NavigationState state)
    {
        state = UpdateItems(navigation, state);
        state = UpdateScrollbar(navigation, state);
        state = UpdateStatusbar(navigation, state);
        return state;
    }

    private static NavigationState UpdateItems(Navigation navigation, NavigationState state)
    {
        var height = Components.NavigationComponent.ContentHeight;
        var activeIndex = navigation.ActiveIndex;
        var firstIndex = navigation.FirstIndex;
        var entries = navigation.ContentEntries;

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

        var prevItems = state.Items ?? new NavigationItemState[height];
        var nextItems = new NavigationItemState[prevItems.Length];

        for (var i = 0; i < prevItems.Length; i++)
        {
            var entry = i < visibleEntries.Length ? visibleEntries[i] : null;
            var isActive = navigation.IsActive && entry != null && entry.Name == navigation.ActiveEntry.Name && entry.Type == navigation.ActiveEntry.Type;
            var isSelected = navigation.SelectedEntries.Contains(entry);

            if (entry == null)
            {
                nextItems[i] = null;
                continue;
            }

            if (prevItems[i] == null)
            {
                nextItems[i] = new NavigationItemState(entry, isActive, isSelected);
                continue;
            }

            nextItems[i] = prevItems[i]
                .Remute(x => x.Entry, entry)
                .Remute(x => x.IsActive, isActive)
                .Remute(x => x.IsSelected, isSelected);
        }

        navigation.FirstIndex = firstIndex;

        state = state
            .Remute(x => x.Items, nextItems);

        return state;
    }

    private static NavigationState UpdateScrollbar(Navigation navigation, NavigationState state)
    {
        var firstIndex = navigation.FirstIndex;
        var contentSize = navigation.ContentEntries.Length;
        var windowSize = state.Items.Length;

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

        state = state
            .Remute(x => x.Scrollbar.GripStartPosition, start)
            .Remute(x => x.Scrollbar.GripEndPosition, end);

        return state;
    }

    private static NavigationState UpdateStatusbar(Navigation navigation, NavigationState state)
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

        state = state
            .Remute(x => x.Statusbar.Status, message);

        return state;
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

    private static NavigationState CreateDefaultNavigationState(bool isScrollbarVisible)
    {
        var statusbar = new StatusbarState(null);
        var scrollbar = new ScrollbarState(isScrollbarVisible, 0, 0);
        var state = new NavigationState(null, statusbar, scrollbar);
        return state;
    }

    private static ToolbarState CreateDefaultToolbarState()
    {
        return new ToolbarState(
            new ButtonState("F1 Help", true),
            new ButtonState("F5 Copy", false),
            new ButtonState("F6 Move", false),
            new ButtonState("F7 Rename", false),
            new ButtonState("F8 Remove", false),
            null
        );
    }

    public static MasterState InterceptTransactionAction(Context context, MasterState state)
    {
        return state;
    }

    public static MasterState UpdateToolbarActions(Context context, MasterState state)
    {
        var isCopyEnabled = IsCopyEnabled(context);
        var isRemoveEnabled = IsRemoveEnabled(context);

        state = state
            .Remute(x => x.Toolbar.CopyButton.Enabled, isCopyEnabled)
            .Remute(x => x.Toolbar.MoveButton.Enabled, isCopyEnabled)
            .Remute(x => x.Toolbar.RemoveButton.Enabled, isRemoveEnabled);

        return state;
    }

    public static MasterState Copy(Context context, MasterState state)
    {
        if (NavigationActions.IsCopyEnabled(context) == false)
        {
            return state;
        }

        var transaction = new CopyTransaction(context);
        transaction.Start();

        return null;
    }

    public static bool IsCopyEnabled(Context context)
    {
        if (context.Model.Transaction != null)
        {
            return false;
        }

        if (context.PrimaryFileSystem.Location == context.SecondaryFileSystem.Location)
        {
            return false;
        }

        var navigation = GetActiveNavigation(context);
        var entries = GetNavigationEntriesForTransaction(navigation);

        if (entries.Any() == false)
        {
            return false;
        }

        if (context.Model.PrimaryNavigation.IsActive)
        {
            return context.Model.Layout.HasFlag(Layout.SecondaryNavigation);
        }

        if (context.Model.SecondaryNavigation.IsActive)
        {
            return true;
        }

        return false;
    }

    public static bool IsRemoveEnabled(Context context)
    {
        if (context.Model.Transaction != null)
        {
            return false;
        }

        var navigation = GetActiveNavigation(context);
        var entries = GetNavigationEntriesForTransaction(navigation);

        return entries.Any();
    }
    public static HashSet<NavigationEntry> GetNavigationEntriesForTransaction(Navigation navigation)
    {
        if (navigation.SelectedEntries.Any())
        {
            return navigation.SelectedEntries;
        }

        if (navigation.ActiveEntry != null && navigation.Location != null && navigation.ActiveEntry.Type != NavigationEntryType.NavUpControl)
        {
            return new HashSet<NavigationEntry>() { navigation.ActiveEntry };
        }

        return new HashSet<NavigationEntry>();
    }
}