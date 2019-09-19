using System;

namespace Xplorer.Themes
{
    internal class CmdTheme : ITheme
    {
        public void SetCursorColor()
        {
            var backgroundColor = Console.BackgroundColor;
            var foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = foregroundColor;
            Console.ForegroundColor = backgroundColor;
        }

        public void ReSetCursorColor()
        {
            SetCursorColor();
        }
    }
}