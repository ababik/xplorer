namespace Xplorer.Models
{
    public class MasterModel
    {
        public PrimaryNavigationModel PrimaryNavigation { get; }
        public SecondaryNavigationModel SecondaryNavigation { get; }
        public ToolbarModel Toolbar { get; }
        public bool SecondaryNavigationVisible { get; }

        public MasterModel(PrimaryNavigationModel primaryNavigation, SecondaryNavigationModel secondaryNavigation, bool secondaryNavigationVisible, ToolbarModel toolbar)
        {
            PrimaryNavigation = primaryNavigation;
            SecondaryNavigation = secondaryNavigation;
            SecondaryNavigationVisible = secondaryNavigationVisible;
            Toolbar = toolbar;
        }
    }
}