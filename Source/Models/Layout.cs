using System;

namespace Xplorer.Models;

[Flags]
public enum Layout
{
    None = 0,
    PrimaryNavigation = 1 << 0,
    SecondaryNavigation = 1 << 1
}