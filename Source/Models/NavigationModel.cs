namespace Xplorer.Models
{
    public class NavigationModel
    {
        public string Location { get; }
        public string Message { get; }
        public string Filter { get; }
        public bool IsActive { get; }
        public int ActiveIndex { get; }
        public int FirstIndex { get; }
        public NavigationEntry ActiveEntry { get; }
        public NavigationEntry[] Entries { get; }
        public NavigationItemModel[] VisibleItems { get; }
        public StatusbarModel Statusbar { get; }
        public ScrollbarModel Scrollbar { get; }

        public NavigationModel(string location, string message, string filter, bool isActive, int activeIndex, int firstIndex, NavigationEntry activeEntry, NavigationEntry[] entries, NavigationItemModel[] visibleItems, StatusbarModel statusbar, ScrollbarModel scrollbar)
        {
            Location = location;
            Message = message;
            Filter = filter;
            IsActive = isActive;
            ActiveIndex = activeIndex;
            FirstIndex = firstIndex;
            ActiveEntry = activeEntry;
            Entries = entries;
            VisibleItems = visibleItems;
            Statusbar = statusbar;
            Scrollbar = scrollbar;
        }
    }
}