using Xplorer.Models;

namespace Xplorer.Components
{
    public class SecondaryNavigationComponent : Component<SecondaryNavigationModel>
    {
        public NavigationComponent Navigation { get; }
        
        public SecondaryNavigationComponent(ITheme theme) : base(theme)
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