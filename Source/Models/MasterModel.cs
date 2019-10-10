namespace Xplorer.Models
{
    public class MasterModel
    {
        public NavigationModel PrimaryNavigation { get; }
        public NavigationModel SecondaryNavigation { get; }
        public ToolbarModel Toolbar { get; }
        public bool SecondaryNavigationVisible { get; }

        public MasterModel(NavigationModel primaryNavigation, NavigationModel secondaryNavigation, bool secondaryNavigationVisible, ToolbarModel toolbar)
        {
            PrimaryNavigation = primaryNavigation;
            SecondaryNavigation = secondaryNavigation;
            SecondaryNavigationVisible = secondaryNavigationVisible;
            Toolbar = toolbar;
        }
    }
}