using Xplorer.Models;

namespace Xplorer.Components
{
    public class PrimaryNavigationComponent : Component<PrimaryNavigationModel>
    {
        public NavigationComponent Navigation { get; }
        
        public PrimaryNavigationComponent(ITheme theme) : base(theme)
        {
            Navigation = new NavigationComponent(Theme);
        }

        public override void Render()
        {
            Navigation.Position(Top, Left, Width, Height);
            Navigation.Render(Model.Navigation);
        }
    }
}