using System;
using Xplorer.Models;

namespace Xplorer.Components
{
    public class MasterComponent : Component<MasterModel>
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
            if (Model.LayoutMode.HasFlag(LayoutMode.PrimaryNavigation))
            {
                PrimaryNavigation = PrimaryNavigation ?? new NavigationComponent(Theme);
            }
            else
            {
                PrimaryNavigation = null;
            }

            if (Model.LayoutMode.HasFlag(LayoutMode.SecondaryNavigation))
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
            PrimaryNavigation?.Render(Model.PrimaryNavigation);
            SecondaryNavigation?.Position(0, primaryNavigationWidth, Console.WindowWidth - primaryNavigationWidth, Console.WindowHeight - 1);
            SecondaryNavigation?.Render(Model.SecondaryNavigation);
            
            Toolbar.Position(Console.WindowHeight - 1, 0, Console.WindowWidth - 1, 1);
            Toolbar.Render();
        }
    }
}