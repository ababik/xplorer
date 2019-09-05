using System.Collections.Generic;

namespace Xplorer
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var application = new Application();
            application.Run();
        }

        internal static string NavUpControlName { get; } = "..";

        private IFileSystem FileSystem { get; }
        private IOperationSystem OperationSystem { get; }
        private ITheme Theme { get; }
        private Renderer Renderer { get; }
        private List<NavigationEntry> Entries { get; set; }
        private int ActiveIndex { get; set; }
        private string Filter { get; set; }

        private Application()
        {
            //FileSystemProvider = new FileSystemProvider();
            //Filter = string.Empty;
        }

        private void Run()
        {

        }
    }
}