using System;

namespace Xplorer
{
    internal interface ITheme
    {
        ConsoleColor GetMainBackgroundColor();
        ConsoleColor GetMainForegroundColor();
        ConsoleColor GetCursorBackgroundColor();
        ConsoleColor GetCursorForegroundColor();
        ConsoleColor GetErrorForegroundColor();
        ConsoleColor GetMarkerDirectoryColor();
        ConsoleColor GetMarkerExecutableColor();
        ConsoleColor GetMarkerDocumentColor();
        ConsoleColor GetMarkerEmptyColor();
    }
}