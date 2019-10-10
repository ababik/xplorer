using Xplorer.Models;

namespace Xplorer.Components
{
    public class NavigationEntryListComponent : Component<NavigationEntryListModel>
    {
        private NavigationEntryListItemComponent[] Components;

        public NavigationEntryListComponent(ITheme theme) : base(theme)
        {
            var maxItemsCount = Layout.GetNavigationEntryListHeight();
            Components = new NavigationEntryListItemComponent[maxItemsCount];

            for (var i = 0; i < maxItemsCount; i++)
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

                if (i < Model.Items.Length)
                {
                    component.Render(Model.Items[i]);
                }
                else
                {
                    component.Render(null);
                }
            }
        }
    }
}