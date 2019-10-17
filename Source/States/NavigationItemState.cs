using Xplorer.Models;

namespace Xplorer.States
{
    public class NavigationItemState
    {
        public NavigationEntry Entry { get; }
        public bool IsActive { get; }
        public bool IsSelected { get; }

        public NavigationItemState(NavigationEntry entry, bool isActive, bool isSelected)
        {
            Entry = entry;
            IsActive = isActive;
            IsSelected = isSelected;
        }
    }
}