using System;
using Xplorer.States;

namespace Xplorer.Components;

public class MasterComponent : Component<MasterState>
{
    private NavigationComponent PrimaryNavigation { get; set; }
    private NavigationComponent SecondaryNavigation { get; set; }
    private ToolbarComponent Toolbar { get; set; }

    public MasterComponent(ITheme theme) : base(theme)
    {
        Toolbar = new ToolbarComponent(Theme);
    }

    public override void Render()
    {
        if (State.PrimaryNavigation != null)
        {
            PrimaryNavigation = PrimaryNavigation ?? new NavigationComponent(Theme);
        }
        else
        {
            PrimaryNavigation = null;
        }

        if (State.SecondaryNavigation != null)
        {
            SecondaryNavigation = SecondaryNavigation ?? new NavigationComponent(Theme);
        }
        else
        {
            SecondaryNavigation = null;
        }

        var primaryNavigationWidth = Console.WindowWidth;

        if (SecondaryNavigation != null)
        {
            primaryNavigationWidth = Console.WindowWidth / 2;
        }
        
        PrimaryNavigation?.Position(0, 0, primaryNavigationWidth, Console.WindowHeight - 1);
        PrimaryNavigation?.Render(State.PrimaryNavigation);
        SecondaryNavigation?.Position(0, primaryNavigationWidth, Console.WindowWidth - primaryNavigationWidth, Console.WindowHeight - 1);
        SecondaryNavigation?.Render(State.SecondaryNavigation);
        
        Toolbar.Position(Console.WindowHeight - 1, 0, Console.WindowWidth - 1, 1);
        Toolbar.Render(State.Toolbar);
    }
}