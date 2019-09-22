using System;

namespace Xplorer.Themes
{
    internal class TerminalTheme : RevertableTheme
    {
        public override ConsoleColor GetCursorBackgroundColor()
        {
            return ConsoleColor.Black;
        }

        public override ConsoleColor GetCursorForegroundColor()
        {
            return ConsoleColor.White;
        }
    }
}