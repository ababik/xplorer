using Xplorer.Models;

namespace Xplorer.Components
{
    public class NavigationComponent : Component<NavigationModel>
    {
        public StatusbarComponent Statusbar { get; }
        public NavigationEntryListComponent NavigationEntryList { get; }
        public ScrollbarComponent Scrollbar { get; }

        public NavigationComponent(ITheme theme) : base(theme)
        {
            Statusbar = new StatusbarComponent(Theme);
            NavigationEntryList = new NavigationEntryListComponent(Theme);
            Scrollbar = new ScrollbarComponent(Theme);
        }

        public override void Render()
        {
            Statusbar.Position(Top, Left, Width, 1);
            Statusbar.Render(Model.Statusbar);

            Scrollbar.Position(Top + 1, Left + Width - 1, 1, Height - 1);
            Scrollbar.Render(Model.Scrollbar);

            NavigationEntryList.Position(Top + 1, Left, Width - 1, Height - 1);
            NavigationEntryList.Render(Model.EntryList);
        }
    }
}