using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xplorer.Models;

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
                var list = new List<NavigationEntry>();
                list.Add(new NavigationEntry(NavigationEntry.NavUpControlName, NavigationEntryType.NavUpControl));
                list.AddRange(directories);
                list.AddRange(files);
                entries = list.ToArray();
            }

            Location = location;
            Entries = entries;
        }

        public bool CheckCopy(NavigationEntry[] entries, string destination, out long size)
        {
            var bytes = 0L;
            var result = false;

            void ProcessFile(FileInfo file)
            {
                bytes += file.Length;

                var destinationPath = RemapDestinationPath(file.FullName, Location, destination);

                if (result == false && File.Exists(destinationPath))
                {
                    result = true;
                }
            }

            void ProcessDirectory(DirectoryInfo directory)
            {
                var destinationPath = RemapDestinationPath(directory.FullName, Location, destination);

                if (result == false && Directory.Exists(destinationPath))
                {
                    result = true;
                }
            }

            Convert(entries, out var files, out var directories);
            Recursion(files, directories, ProcessFile, ProcessDirectory);

            size = bytes;
            return result;
        }

        public void Copy(NavigationEntry[] entries, string destination, bool overwrite)
        {
            void ProcessFile(FileInfo file)
            {
                var destinationPath = RemapDestinationPath(file.FullName, Location, destination);

                if (File.Exists(destinationPath) && overwrite == false)
                {
                    return;
                }

                var destinationDirectory = Path.GetDirectoryName(destinationPath);

                if (Directory.Exists(destinationDirectory) == false)
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                file.CopyTo(destinationPath, true);
            }

            void ProcessDirectory(DirectoryInfo directory)
            {
                var destinationPath = RemapDestinationPath(directory.FullName, Location, destination);

                Directory.CreateDirectory(destinationPath);
            }

            Convert(entries, out var files, out var directories);
            Recursion(files, directories, ProcessFile, ProcessDirectory);
        }

        private void Convert(NavigationEntry[] entries, out FileInfo[] files, out DirectoryInfo[] directories)
        {
            files = entries
                .Where(x => x.Type == NavigationEntryType.File)
                .Select(x => new FileInfo(Path.Combine(Location, x.Name)))
                .ToArray();

            directories = entries
                .Where(x => x.Type == NavigationEntryType.Directory)
                .Select(x => new DirectoryInfo(Path.Combine(Location, x.Name)))
                .ToArray();
        }

        private static void Recursion(FileInfo[] files, DirectoryInfo[] directories, Action<FileInfo> processFileCallback, Action<DirectoryInfo> processDirectoryCallback)
        {
            foreach (var file in files)
            {
                processFileCallback.Invoke(file);
            }

            foreach (var directory in directories)
            {
                processDirectoryCallback.Invoke(directory);
                Recursion(directory.GetFiles(), directory.GetDirectories(), processFileCallback, processDirectoryCallback);
            }
        }

        private static string RemapDestinationPath(string path, string source, string destination)
        {
            var relativePath = Path.GetRelativePath(source, path);
            var destinationPath = Path.Combine(destination, relativePath);
            return destinationPath;
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