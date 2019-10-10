using Xplorer.Actions;
using Xplorer.Models;

namespace Xplorer.OperationSystems
{
    internal class WindowsOperationSystem : IOperationSystem
    {
        public void Reveal(NavigationModel navigation)
        {
            var location = NavigationActions.GetActiveLocation(navigation);
            var argument = string.Empty;

            if (location != null)
            {
                argument = "\"" + location + "\"";

                var entry = navigation.EntryList.Items[navigation.ActiveIndex].Entry;
                
                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    argument = "/select, \"" + location + "\"";
                }
            }

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}