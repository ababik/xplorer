using System;

namespace Xplorer
{
    public interface ITheme
    {
        ConsoleColor GetCursorBackgroundColor();
        ConsoleColor GetCursorForegroundColor();
        ConsoleColor GetErrorForegroundColor();
        ConsoleColor GetMarkerDirectoryColor();
        ConsoleColor GetMarkerExecutableColor();
        ConsoleColor GetMarkerDocumentColor();
        ConsoleColor GetMarkerEmptyColor();
        ConsoleColor GetSelectedEntryColor();
        ConsoleColor GetScrollBackgroundColor();
        ConsoleColor GetScrollGripColor();
    }
}