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
            var argument = "\"" + Context.Location +"\"";

            if (Context.Entries[Context.ActiveIndex].Type != NavigationEntryType.NavUpControl)
            {
                argument = "/select, \"" + Context.Location + "\\" + Context.Entries[Context.ActiveIndex].Name +"\"";
            }

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}