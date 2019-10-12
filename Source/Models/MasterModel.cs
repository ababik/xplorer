namespace Xplorer.Models
{
    public class MasterModel
    {
        public LayoutMode LayoutMode { get; }
        public NavigationModel PrimaryNavigation { get; }
        public NavigationModel SecondaryNavigation { get; }
        public ToolbarModel Toolbar { get; }

        public MasterModel(LayoutMode layoutMode, NavigationModel primaryNavigation, NavigationModel secondaryNavigation, ToolbarModel toolbar)
        {
            LayoutMode = layoutMode;
            PrimaryNavigation = primaryNavigation;
            SecondaryNavigation = secondaryNavigation;
            Toolbar = toolbar;
        }
    }
}