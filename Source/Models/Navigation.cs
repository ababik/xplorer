using System.Collections.Generic;

namespace Xplorer.Models
{
    public class Navigation
    {
        public string Location { get; set; }
        public string Message { get; set; }
        public string Filter { get; set; }
        public bool IsActive { get; set; }
        public int ActiveIndex { get; set; }
        public int FirstIndex { get; set; }
        public NavigationEntry ActiveEntry { get; set; }
        public HashSet<NavigationEntry> SelectedEntries { get; set; }
        public NavigationEntry[] ContentEntries { get; set; }
    }
}