using Xplorer.Models;

namespace Xplorer;

public interface IFileSystem
{
    bool Executed { get; }
    string Location { get; }
    NavigationEntry[] Entries { get; }
    void Navigate(string path);
    bool CheckCopy(NavigationEntry[] entries, string destination, out long size);
    void Copy(NavigationEntry[] entries, string destination, bool overwrite);
}