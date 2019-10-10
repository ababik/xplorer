using System;
using Xplorer.Models;

namespace Xplorer.Components
{
    public class MasterComponent : Component<MasterModel>
    {
        private PrimaryNavigationComponent PrimaryNavigation { get; }
        private SecondaryNavigationComponent SecondaryNavigation { get; }
        private ToolbarComponent Toolbar { get; }

        public MasterComponent(ITheme theme) : base(theme)
        {
            PrimaryNavigation = new PrimaryNavigationComponent(Theme);
            SecondaryNavigation = new SecondaryNavigationComponent(Theme);
            Toolbar = new ToolbarComponent(Theme);
        }

        public override void Render()
        {
            var primaryNavigationWidth = Console.WindowWidth;

            if (Model.SecondaryNavigationVisible)
            {
                primaryNavigationWidth = Console.WindowWidth / 2;
            }
            
            PrimaryNavigation.Position(0, 0, primaryNavigationWidth, Console.WindowHeight - 1);
            PrimaryNavigation.Render(Model.PrimaryNavigation);

            if (Model.SecondaryNavigationVisible)
            {
                SecondaryNavigation.Position(0, primaryNavigationWidth, Console.WindowWidth - primaryNavigationWidth, Console.WindowHeight - 1);
                SecondaryNavigation.Render(Model.SecondaryNavigation);
            }
            
            Toolbar.Position(Console.WindowHeight - 1, 0, Console.WindowWidth, 1);
            Toolbar.Render(Model.Toolbar);
        }
    }
}