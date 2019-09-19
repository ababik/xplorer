using System;
using System.IO;

namespace Xplorer
{
    internal class NavigationEntry
    {
        public string Name { get; }
        public string Extension { get; }
        public NavigationEntryType Type { get; }

        public NavigationEntry(string name, NavigationEntryType type)
        {
            Name = name;
            Type = type;
            Extension = Path.GetExtension(Name).TrimStart('.');
        }

        public bool IsExecutable()
        {
            return Extension == "exe";
        }

        public bool IsDocument()
        {
            return Extension == "pdf" || Extension == "docx" || Extension == "doc";
        }
    }
}