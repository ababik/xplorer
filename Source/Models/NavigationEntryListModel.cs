namespace Xplorer.Models
{
    public class NavigationEntryListModel
    {
        public int FirstIndex { get; }
        public NavigationEntryListItemModel[] Items { get; }

        public NavigationEntryListModel(int firstIndex, NavigationEntryListItemModel[] items)
        {
            FirstIndex = firstIndex;
            Items = items;
        }
    }
}