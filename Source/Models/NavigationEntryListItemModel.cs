namespace Xplorer.Models
{
    public class NavigationEntryListItemModel
    {
        public NavigationEntry Entry { get; }
        public bool IsActive { get; }

        public NavigationEntryListItemModel(NavigationEntry entry, bool isActive)
        {
            Entry = entry;
            IsActive = isActive;
        }
    }
}