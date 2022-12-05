using Xplorer.Actions;
using Xplorer.Models;

namespace Xplorer.OperationSystems;

internal class WindowsOperationSystem : IOperationSystem
{
    public void Reveal(Navigation navigation)
    {
        var location = NavigationActions.GetActiveLocation(navigation);
        var argument = string.Empty;

        if (location != null)
        {
            argument = "\"" + location + "\"";

            var entry = navigation.ActiveEntry;
            
            if (entry.Type != NavigationEntryType.NavUpControl)
            {
                argument = "/select, \"" + location + "\"";
            }
        }

        System.Diagnostics.Process.Start("explorer.exe", argument);
    }
}