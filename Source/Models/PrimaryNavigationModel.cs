namespace Xplorer.Models
{
    public class PrimaryNavigationModel
    {
        public NavigationModel Navigation { get; }

        public PrimaryNavigationModel(NavigationModel navigation)
        {
            Navigation = navigation;
        }
    }
}