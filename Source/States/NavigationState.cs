namespace Xplorer.States
{
    public class NavigationState
    {
        public NavigationItemState[] Items { get; }
        public StatusbarState Statusbar { get; }
        public ScrollbarState Scrollbar { get; }

        public NavigationState(NavigationItemState[] items, StatusbarState statusbar, ScrollbarState scrollbar)
        {
            Items = items;
            Statusbar = statusbar;
            Scrollbar = scrollbar;
        }
    }
}