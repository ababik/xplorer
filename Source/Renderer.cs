using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xplorer
{
    internal class Renderer
    {
        private Context Context { get; }
        private ITheme Theme { get; }
        private List<NavigationEntry> PrevEntries { get; set; }
        private int PrevFirstIndex { get; set; }
        private int? PrevActiveIndex { get; set; }

        public Renderer(Context context, ITheme theme)
        {
            Context = context;
            Theme = theme;
        }
        public void Render()
        {
            if (Context.Entries != PrevEntries)
            {
                PrevEntries = Context.Entries;
                PrevFirstIndex = 0;
                PrevActiveIndex = null;
            }

            var offset = 0;

            offset += RenderStatus();

            var maxItemsCount = Console.WindowHeight - offset;
            var firstIndex = PrevFirstIndex;
            var lastIndex = firstIndex + maxItemsCount - 1;

            if (lastIndex >= Context.Entries.Count)
            {
                lastIndex = Context.Entries.Count - 1;
            }

            if (Context.ActiveIndex < firstIndex)
            {
                firstIndex = Context.ActiveIndex;
                lastIndex = firstIndex + maxItemsCount - 1;

                if (lastIndex >= Context.Entries.Count)
                {
                    lastIndex = Context.Entries.Count - 1;
                }
            }

            if (Context.ActiveIndex > lastIndex)
            {
                lastIndex = Context.ActiveIndex;
                firstIndex = lastIndex - maxItemsCount + 1;

                if (firstIndex < 0)
                {
                    firstIndex = 0;
                }
            }

            if (PrevFirstIndex == firstIndex && PrevActiveIndex.HasValue)
            {
                var prevEntry = Context.Entries[PrevActiveIndex.Value];
                var prevPosition = PrevActiveIndex.Value - firstIndex + offset;
                Console.SetCursorPosition(0, prevPosition);
                RenderMarker(prevEntry);
                Write(prevEntry.Name);

                var entry = Context.Entries[Context.ActiveIndex];
                var position = Context.ActiveIndex  - firstIndex + offset;
                Console.SetCursorPosition(0, position);
                RenderMarker(entry);
                SetCursorColor();
                Write(entry.Name);
                ResetCursorColor();
            }
            else
            {
                for (var i = offset; i <= maxItemsCount; i++)
                {
                    Console.SetCursorPosition(0, i);

                    var index = firstIndex + i - offset;
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
            }

            PrevFirstIndex = firstIndex;
            PrevActiveIndex = Context.ActiveIndex;

            Console.SetCursorPosition(0, 0);
        }

        public int RenderStatus(string message = null)
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

            return 1;
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