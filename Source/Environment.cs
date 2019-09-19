using System.Runtime.InteropServices;
using Xplorer.FileSystems;
using Xplorer.OperationSystems;
using Xplorer.Themes;

namespace Xplorer
{
    internal class Environment
    {
        private OSPlatform Platform { get; }

        public Environment()
        {
            Platform = DetectPlatform();
        }

        public IFileSystem CreateFileSystem()
        {
            return new FileSystem();
        }

        public IOperationSystem CreateOperationSystem(Context context)
        {
            if (Platform == OSPlatform.Windows)
            {
                return new WindowsOperationSystem(context);
            }

            if (Platform == OSPlatform.OSX)
            {
                return new OsxOperationSystem(context);
            }

            return new LinuxOperationSystem(context);
        }

        public ITheme CreateTheme()
        {
            if (Platform == OSPlatform.OSX)
            {
                return new TerminalTheme();
            }

            return new RevertableTheme();
        }

        private static OSPlatform DetectPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            return OSPlatform.Linux;
        }
    }
}