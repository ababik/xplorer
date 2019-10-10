using System.Collections.Generic;

namespace Xplorer.Models
{
    public class NavigationModel
    {
        public string Message { get; }
        public string Location { get; }
        public string Filter { get; }
        public bool IsActive { get; }
        public int ActiveIndex { get; }
        public NavigationEntry ActiveNavigationEntry { get; }
        public NavigationEntry[] NavigationEntries { get; }
        public StatusbarModel Statusbar { get; }
        public NavigationEntryListModel EntryList { get; }
        public ScrollbarModel Scrollbar { get; }

        public NavigationModel(string message, string location, string filter, bool isActive, int activeIndex, NavigationEntry activeNavigationEntry, NavigationEntry[] navigationEntries, StatusbarModel statusbar, NavigationEntryListModel entryList, ScrollbarModel scrollbar)
        {
            Message = message;
            Location = location;
            Filter = filter;
            IsActive = isActive;
            ActiveIndex = activeIndex;
            ActiveNavigationEntry = activeNavigationEntry;
            NavigationEntries = navigationEntries;
            Statusbar = statusbar;
            EntryList = entryList;
            Scrollbar = scrollbar;
        }
    }
}