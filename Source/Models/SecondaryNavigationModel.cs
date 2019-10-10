namespace Xplorer.Models
{
    public class SecondaryNavigationModel
    {
        public NavigationModel Navigation { get; }

        public SecondaryNavigationModel(NavigationModel navigation)
        {
            Navigation = navigation;
        }
    }
}