using Xplorer.Actions;
using Xplorer.Models;

namespace Xplorer.OperationSystems
{
    internal class OsxOperationSystem : IOperationSystem
    {
        public void Reveal(NavigationModel navigation)
        {
            var location = NavigationActions.GetActiveLocation(navigation);
            var argument = "/";

            if (location != null)
            {
                argument = "\"" + location + "\"";

                var entry = navigation.EntryList.Items[navigation.ActiveIndex].Entry;
                
                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    argument = "-R \"" + location + "\"";
                }
            }

            System.Diagnostics.Process.Start("open", argument);
        }
    }
}