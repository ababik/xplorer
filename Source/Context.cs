using System.Collections.Generic;

namespace Xplorer
{
    internal class Context
    {
        public string Location { get; set; }
        public List<NavigationEntry> Entries { get; set; }
        public int ActiveIndex { get; set; }
        public string Filter { get; set; }
    }
}