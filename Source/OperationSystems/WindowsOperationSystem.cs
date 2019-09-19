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
            var entry = Context.Entries[Context.ActiveIndex];
            var location = Context.Location?.TrimEnd('\\');
            var argument = string.Empty;

            if (location != null)
            {
                argument = "\"" + location +"\"";

                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    argument = "/select, \"" + location + "\\" + entry.Name +"\"";
                }
            }
            

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}