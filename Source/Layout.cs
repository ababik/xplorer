using System;

namespace Xplorer
{
    public static class Layout
    {
        public static int GetToolbarHeight()
        {
            return 1;
        }

        public static int GetStatusbarHeight()
        {
            return 1;
        }

        public static int GetNavigationEntryListHeight()
        {
            return Console.WindowHeight - GetToolbarHeight() - GetStatusbarHeight();
        }
    }
}