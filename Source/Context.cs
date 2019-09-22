using System.Collections.Generic;
using System.IO;

namespace Xplorer
{
    internal class Context
    {
        public string Location { get; set; }
        public List<NavigationEntry> Entries { get; set; }
        public int ActiveIndex { get; set; }
        public string Filter { get; set; }

        public string GetActiveLocation()
        {
            var result = Location;
            var entry = Entries[ActiveIndex];

            if (Location != null && entry != null)
            {
                if (entry.Type != NavigationEntryType.NavUpControl)
                {
                    result = Path.Combine(Location, entry.Name);
                }
            }

            return result;
        }
    }
}