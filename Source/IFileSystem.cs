namespace Xplorer
{
    internal interface IFileSystem
    {
        bool Executed { get; }
        string Location { get; }
        NavigationEntry[] Entries { get; }
        void Navigate(string path);
    }
}