using System;
using Xplorer.States;

namespace Xplorer.Components;

public class NavigationComponent : Component<NavigationState>
{
    public static int ContentHeight => Console.WindowHeight - ToolbarComponent.BaseHeight - StatusbarComponent.BaseHeight; 
    public StatusbarComponent Statusbar { get; }
    public ScrollbarComponent Scrollbar { get; }
    public NavigationItemComponent[] NavigationItems { get; private set; }

    public NavigationComponent(ITheme theme) : base(theme)
    {
        Statusbar = new StatusbarComponent(Theme);
        Scrollbar = new ScrollbarComponent(Theme);
        Application.OnResize += HandleResize;
    }

    public override void Render()
    {
        Statusbar.Position(Top, Left, Width, 1);
        Statusbar.Render(State.Statusbar);

        Scrollbar.Position(Top + 1, Left + Width - 1, 1, Height - 1);
        Scrollbar.Render(State.Scrollbar);

        if (NavigationItems == null)
        {
            CreateNavigationItems();
        }

        for (var i = 0; i < State.Items.Length; i++)
        {
            var component = NavigationItems[i];
            var model = State.Items[i];
            component.Position(Top + 1 + i, Left, Width - 1, 1);
            component.Render(model);
        }
    }

    private void HandleResize()
    {
        NavigationItems = null;
    }

    private void CreateNavigationItems()
    {
        NavigationItems = new NavigationItemComponent[State.Items.Length];

        for (var i = 0; i < State.Items.Length; i++)
        {
            NavigationItems[i] = new NavigationItemComponent(Theme);
        }
    }
}