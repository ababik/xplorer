using System.Collections.Generic;
using System.IO;

namespace Xplorer.Models
{
    public class NavigationEntry
    {
        private static HashSet<string> Executables = new HashSet<string>()
        {
            "exe", "com", "bat"
        };

        private static HashSet<string> Documents = new HashSet<string>()
        {
            "pdf", "doc", "docx", "html", "htm", "xls", "xlsx", "ppt", "pptx",
            "txt", "log", "xml", "json"
        };

        public static string NavUpControlName { get; } = "..";

        public string Name { get; }
        public string Extension { get; }
        public bool IsExecutable { get; }
        public bool IsDocument { get; }
        public NavigationEntryType Type { get; }

        public NavigationEntry(string name, NavigationEntryType type)
        {
            Name = name;
            Type = type;
            Extension = Path.GetExtension(Name).TrimStart('.').ToLower();
            IsExecutable = Executables.Contains(Extension);
            IsDocument = Documents.Contains(Extension);
        }
    }
}