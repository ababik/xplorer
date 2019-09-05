using System;

namespace Xplorer
{
    internal class NavigationEntry
    {
        public string Name { get; }
        public NavigationEntryType Type { get; }

        public NavigationEntry(string name, NavigationEntryType type)
        {
            Name = name;
            Type = type;
        }
    }
}