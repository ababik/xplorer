using System;

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

                RenderMarker(entry);

                if (index == Context.ActiveIndex)
                {
                    Theme.SetCursorColor();
                }

                Write(text);

                if (index == Context.ActiveIndex)
                {
                    Theme.ReSetCursorColor();
                }
            }

            Console.SetCursorPosition(0, 0);
        }

        public void RenderStatus(string message = null)
        {
            Console.SetCursorPosition(0, 0);

            var initialForegroundColor = null as ConsoleColor?;

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
                initialForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Write(message ?? string.Empty);

            if (initialForegroundColor.HasValue)
            {
                Console.ForegroundColor = initialForegroundColor.Value;
            }
        }

        private static void RenderMarker(NavigationEntry entry)
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
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write(" ");
                    Console.BackgroundColor = backgroundColor;
                }
                else
                {
                    if (entry.IsExecutable())
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    else if (entry.IsDocument())
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                    }
                    Console.Write(" ");
                    Console.BackgroundColor = backgroundColor;
                }
            }

            Console.Write(" ");
        }

        private static void Write(string value)
        {
            Console.Write((value ?? string.Empty).PadRight(Console.WindowWidth - 3));
        }
    }
}