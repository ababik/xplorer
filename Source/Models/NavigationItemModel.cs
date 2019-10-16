namespace Xplorer.Models
{
    public class NavigationItemModel
    {
        public NavigationEntry Entry { get; }
        public bool IsActive { get; }
        public bool IsSelected { get; }

        public NavigationItemModel(NavigationEntry entry, bool isActive, bool isSelected)
        {
            Entry = entry;
            IsActive = isActive;
            IsSelected = isSelected;
        }
    }
}