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
        }
    }
}