using System;

namespace Xplorer.Themes
{
    internal class TerminalTheme : ITheme
    {
        public void SetCursorColor()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ReSetCursorColor()
        {
            Console.ResetColor();
        }
    }
}