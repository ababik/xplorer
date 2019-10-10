using Xplorer.Models;

namespace Xplorer.Components
{
    public class NavigationEntryListComponent : Component<NavigationEntryListModel>
    {
        private NavigationEntryListItemComponent[] Components;

        public NavigationEntryListComponent(ITheme theme) : base(theme)
        {
            var height = Layout.GetNavigationEntryListHeight();
            Components = new NavigationEntryListItemComponent[height];

            for (var i = 0; i < height; i++)
            {
                Components[i] = new NavigationEntryListItemComponent(Theme);
            }
        }

        public override void Render()
        {
            for (var i = 0; i < Components.Length; i++)
            {
                var component = Components[i];
                component.Position(Top + i, Left, Width, 1);

                var value = i < Model.Items.Length ? Model.Items[i] : null;
                component.Render(value);
            }
        }
    }
}