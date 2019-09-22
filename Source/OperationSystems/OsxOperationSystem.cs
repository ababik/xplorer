namespace Xplorer.OperationSystems
{
    internal class OsxOperationSystem : IOperationSystem
    {
        private Context Context { get; }

        public OsxOperationSystem(Context context)
        {
            Context = context;
        }

        public void Reveal()
        {
            var location = Context.GetActiveLocation();
            var argument = "/";

            if (location != null)
            {
                argument = "\"" + location + "\"";

                var entry = Context.Entries[Context.ActiveIndex];
                
                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    argument = "-R \"" + location + "\"";
                }
            }

            System.Diagnostics.Process.Start("open", argument);
        }
    }
}