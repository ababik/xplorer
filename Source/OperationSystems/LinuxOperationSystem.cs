namespace Xplorer.OperationSystems
{
    internal class LinuxOperationSystem : IOperationSystem
    {
        private Context Context { get; }

        public LinuxOperationSystem(Context context)
        {
            Context = context;
        }

        public void Reveal()
        {
        }
    }
}