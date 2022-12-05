using System;

namespace Xplorer.Themes
{
    internal class TerminalTheme : RevertableTheme
    {
        public override ConsoleColor GetCursorBackgroundColor()
        {
            return ConsoleColor.White;
        }

        public override ConsoleColor GetCursorForegroundColor()
        {
            return ConsoleColor.Black;
        }
    }
}