using System;

namespace Xplorer.Models
{
    [Flags]
    public enum LayoutMode
    {
        None = 0,
        PrimaryNavigation = 1 << 0,
        SecondaryNavigation = 1 << 1
    }
}