namespace Xplorer
{
    internal interface IFileSystem
    {
        bool IsExecutable { get; }
        string Location { get; }
        NavigationEntry[] Entries { get; }
        void Navigate(string path);
    }
}