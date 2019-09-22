using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Xplorer.FileSystems
{
    internal class FileSystem : IFileSystem
    {
        public bool Executed { get; private set; }
        public string Location { get; private set; }
        public NavigationEntry[] Entries { get; private set; }

        public void Navigate(string location)
        {
            Executed = false;

            var isUndefined = location == null && Location == null;
            var isNavUp = location == null && Location != null;

            if (isUndefined)
            {
                location = Directory.GetCurrentDirectory();
            }

            if (location != Location)
            {
                location = isNavUp
                    ? Path.GetDirectoryName(Location)
                    : Path.Combine(Location ?? string.Empty, location);

                if (location != null)
                {
                    var attributes = System.IO.File.GetAttributes(location);

                    if (attributes.HasFlag(System.IO.FileAttributes.Directory) == false)
                    {
                        Executed = true;
                        ExecuteFile(location);
                        return;
                    }
                }
            }

            var entries = default(NavigationEntry[]);

            if (location == null)
            {
                entries = System.IO.DriveInfo
                    .GetDrives()
                    .Select(ConvertDrive).ToArray();
            }
            else
            {
                var directories = System.IO.Directory.GetDirectories(location)
                    .Select(ConvertDirectory)
                    .OrderBy(x => x.Name);
                var files = System.IO.Directory.GetFiles(location)
                    .Select(ConvertFile)
                    .OrderBy(x => x.Name);
                entries = directories
                    .Concat(files)
                    .ToArray();
            }

            Location = location;
            Entries = entries;
        }

        private static void ExecuteFile(string path)
        {
            var proess = new Process();

            proess.StartInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true
            };

            proess.Start();
        }

        private static NavigationEntry ConvertDrive(DriveInfo driveInfo)
        {
            return new NavigationEntry(driveInfo.Name.Trim(), NavigationEntryType.Drive);
        }

        private static NavigationEntry ConvertDirectory(string path)
        {
            path = path.Trim();
            return new NavigationEntry(Path.GetFileName(path), NavigationEntryType.Directory);
        }

        private static NavigationEntry ConvertFile(string path)
        {
            path = path.Trim();
            var extension = Path.GetExtension(path).TrimStart('.');
            return new NavigationEntry(Path.GetFileName(path), NavigationEntryType.File);
        }
    }
}