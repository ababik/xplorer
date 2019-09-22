namespace Xplorer.OperationSystems
{
    internal class WindowsOperationSystem : IOperationSystem
    {
        private Context Context { get; }

        public WindowsOperationSystem(Context context)
        {
            Context = context;
        }

        public void Reveal()
        {
            var location = Context.GetActiveLocation();
            var argument = string.Empty;

            if (location != null)
            {
                argument = "\"" + location + "\"";

                var entry = Context.Entries[Context.ActiveIndex];
                
                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    argument = "/select, \"" + location + "\"";
                }
            }

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}