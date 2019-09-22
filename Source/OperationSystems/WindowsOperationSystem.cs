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
            var argument = "/select, \"" + Context.GetActiveLocation() + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}