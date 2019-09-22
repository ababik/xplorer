using System;

namespace Xplorer.Themes
{
    internal class RevertableTheme : ITheme
    {
        private ConsoleColor MainBackgroundColor { get; }
        private ConsoleColor MainForegroundColor { get; }

        public RevertableTheme()
        {
            MainBackgroundColor = Console.BackgroundColor;
            MainForegroundColor = Console.ForegroundColor;
        }

        public ConsoleColor GetMainBackgroundColor()
        {
            return MainBackgroundColor;
        }

        public ConsoleColor GetMainForegroundColor()
        {
            return MainForegroundColor;
        }

        public ConsoleColor GetCursorBackgroundColor()
        {
            return MainForegroundColor;
        }

        public ConsoleColor GetCursorForegroundColor()
        {
            return MainBackgroundColor;
        }

        public ConsoleColor GetErrorForegroundColor()
        {
            return ConsoleColor.Red;
        }

        public ConsoleColor GetMarkerDirectoryColor()
        {
            return ConsoleColor.Yellow;
        }

        public ConsoleColor GetMarkerDocumentColor()
        {
            return ConsoleColor.Cyan;
        }

        public ConsoleColor GetMarkerExecutableColor()
        {
            return ConsoleColor.Green;
        }

        public ConsoleColor GetMarkerEmptyColor()
        {
            return ConsoleColor.Gray;
        }

        public ConsoleColor GetScrollBackgroundColor()
        {
            return ConsoleColor.Gray;
        }
        public ConsoleColor GetScrollGripColor()
        {
            return ConsoleColor.DarkGray;
        }
    }
}