using System;
using System.Diagnostics;

namespace Xplorer
{
    internal class Renderer
    {
        private Context Context { get; }
        private ITheme Theme { get; }

        public Renderer(Context context, ITheme theme)
        {
            Context = context;
            Theme = theme;
        }
        public void Render()
        {
            RenderStatus();

            var startIndex = 0;
            var maxIndex = Console.WindowHeight - 1;

            if (Context.ActiveIndex > maxIndex - 1)
            {
                startIndex = Context.ActiveIndex - maxIndex + 1;
            }

            for (var i = 1; i <= maxIndex; i++)
            {
                Console.SetCursorPosition(0, i);

                var index = startIndex + i - 1;
                var text = string.Empty;
                var entry = null as NavigationEntry;

                if (index < Context.Entries.Count)
                {
                    entry = Context.Entries[index];
                    text = entry.Name;
                }

                if (entry == null)
                {
                    RenderMarker(null);
                    Write(null);
                    continue;
                }

                RenderMarker(entry);

                if (index == Context.ActiveIndex)
                {
                    SetCursorColor();
                }

                Write(text);

                if (index == Context.ActiveIndex)
                {
                    ResetCursorColor();
                }
            }

            Console.SetCursorPosition(0, 0);
        }

        public void RenderStatus(string message = null)
        {
            Console.SetCursorPosition(0, 0);

            var foregroundColor = null as ConsoleColor?;

            if (message == null)
            {
                message = Context.Location;

                if (Context.Filter != string.Empty)
                {
                    message = $"Filter: {Context.Filter} (ESC to clear)";
                }
            }
            else
            {
                foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = Theme.GetErrorForegroundColor();
            }

            Write(message ?? string.Empty);

            if (foregroundColor.HasValue)
            {
                Console.ForegroundColor = foregroundColor.Value;
            }
        }

        private void RenderMarker(NavigationEntry entry)
        {
            var backgroundColor = Console.BackgroundColor;

            if (entry == null)
            {
                Console.Write(" ");
            }
            else
            {
                if (entry.Type == NavigationEntryType.Directory || entry.Type == NavigationEntryType.Drive || entry.Type == NavigationEntryType.NavUpControl)
                {
                    Console.BackgroundColor = Theme.GetMarkerDirectoryColor();
                    Console.Write(" ");
                    Console.BackgroundColor = backgroundColor;
                }
                else
                {
                    if (entry.IsExecutable())
                    {
                        Console.BackgroundColor = Theme.GetMarkerExecutableColor();
                    }
                    else if (entry.IsDocument())
                    {
                        Console.BackgroundColor = Theme.GetMarkerDocumentColor();
                    }
                    else
                    {
                        Console.BackgroundColor = Theme.GetMarkerEmptyColor();
                    }
                    Console.Write(" ");
                    Console.BackgroundColor = backgroundColor;
                }
            }

            Console.Write(" ");
        }

        private void SetCursorColor()
        {
            Console.BackgroundColor = Theme.GetMainForegroundColor();
            Console.ForegroundColor = Theme.GetMainBackgroundColor();
        }

        private void ResetCursorColor()
        {
            Console.BackgroundColor = Theme.GetMainBackgroundColor();
            Console.ForegroundColor = Theme.GetMainForegroundColor();
        }

        private static void Write(string value)
        {
            Console.Write((value ?? string.Empty).PadRight(Console.WindowWidth - 3));
        }
    }
}