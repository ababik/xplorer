namespace Xplorer.Models
{
    public class NavigationModel
    {
        public string Message { get; }
        public string Location { get; }
        public string Filter { get; }
        public bool IsActive { get; }
        public int ActiveIndex { get; }
        public NavigationEntry ActiveEntry { get; }
        public NavigationEntry[] Entries { get; }
        public NavigationEntryListModel VisibleEntryList { get; }
        public StatusbarModel Statusbar { get; }
        public ScrollbarModel Scrollbar { get; }

        public NavigationModel(string message, string location, string filter, bool isActive, int activeIndex, NavigationEntry activeEntry, NavigationEntry[] entries, NavigationEntryListModel visibleEntryList, StatusbarModel statusbar, ScrollbarModel scrollbar)
        {
            Message = message;
            Location = location;
            Filter = filter;
            IsActive = isActive;
            ActiveIndex = activeIndex;
            ActiveEntry = activeEntry;
            Entries = entries;
            VisibleEntryList = visibleEntryList;
            Statusbar = statusbar;
            Scrollbar = scrollbar;
        }
    }
}