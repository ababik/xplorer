namespace Xplorer.States;

public class MasterState
{
    public NavigationState PrimaryNavigation { get; }
    public NavigationState SecondaryNavigation { get; }
    public ToolbarState Toolbar { get; }

    public MasterState(NavigationState primaryNavigation, NavigationState secondaryNavigation, ToolbarState toolbar)
    {
        PrimaryNavigation = primaryNavigation;
        SecondaryNavigation = secondaryNavigation;
        Toolbar = toolbar;
    }
}