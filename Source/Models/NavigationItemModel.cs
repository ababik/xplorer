namespace Xplorer.Models
{
    public class NavigationItemModel
    {
        public NavigationEntry Entry { get; }
        public bool IsActive { get; }

        public NavigationItemModel(NavigationEntry entry, bool isActive)
        {
            Entry = entry;
            IsActive = isActive;
        }
    }
}