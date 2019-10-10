/*
using System;
using System.Collections.Generic;

namespace Xplorer
{
    internal class Renderer
    {
        private Context Context { get; }
        private ITheme Theme { get; }
        private int PrevWindowWidth { get; set; }
        private int PrevWindowHeight { get; set; }
        private List<NavigationEntry> PrevEntries { get; set; }
        private int PrevFirstIndex { get; set; }
        private int? PrevActiveIndex { get; set; }
        private int PrevScrollStartIndex { get; set; }
        private int PrevScrollEndIndex { get; set; }

        private int ToolbarHeight { get; set; } = 1;

        public Renderer(Context context, ITheme theme)
        {
            Context = context;
            Theme = theme;

            PrevWindowWidth = Console.WindowWidth;
            PrevWindowHeight = Console.WindowHeight;
        }

        public void Render()
        {
            if (IsResetPrevsRequired())
            {
                ResetPrevs();
            }

            RenderStatus();

            var maxItemsCount = Console.WindowHeight - ToolbarHeight;
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
                var prevPosition = PrevActiveIndex.Value - firstIndex + ToolbarHeight;
                Console.SetCursorPosition(0, prevPosition);
                RenderMarker(prevEntry);
                Write(prevEntry.Name);

                var entry = Context.Entries[Context.ActiveIndex];
                var position = Context.ActiveIndex  - firstIndex + ToolbarHeight;
                Console.SetCursorPosition(0, position);
                RenderMarker(entry);
                SetCursorColor();
                Write(entry.Name);
                ResetCursorColor();
            }
            else
            {
                for (var i = ToolbarHeight; i < Console.WindowHeight; i++)
                {
                    Console.SetCursorPosition(0, i);

                    var index = firstIndex + i - ToolbarHeight;
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

            SetPrevs(firstIndex);

            RenderScroll(firstIndex);

            Console.SetCursorPosition(0, 0);
        }

        public void RenderStatus(string message = null)
        {
            Console.SetCursorPosition(0, 0);

            if (message == null)
            {
                message = Context.Location ?? System.Environment.UserName + "@" + System.Environment.MachineName;

                if (Context.Filter != string.Empty)
                {
                    message = $"Filter: {Context.Filter} (ESC to clear)";
                }
            }
            else
            {
                Console.ForegroundColor = Theme.GetErrorForegroundColor();
            }

            Write(message, 0);

            Console.ResetColor();
        }

        private void RenderMarker(NavigationEntry entry)
        {
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
                    Console.ResetColor();
                }
                else
                {
                    if (entry.IsExecutable)
                    {
                        Console.BackgroundColor = Theme.GetMarkerExecutableColor();
                    }
                    else if (entry.IsDocument)
                    {
                        Console.BackgroundColor = Theme.GetMarkerDocumentColor();
                    }
                    else
                    {
                        Console.BackgroundColor = Theme.GetMarkerEmptyColor();
                    }
                    Console.Write(" ");
                    Console.ResetColor();
                }
            }

            Console.Write(" ");
        }

        private void RenderScroll(int windowPosition)
        {
            var contentSize = Context.Entries.Count;
            var windowSize = Console.WindowHeight - ToolbarHeight;
            var trackSize = windowSize;
            var windowContentRatio = (float)windowSize / contentSize;
            var gripSize = (int)(trackSize * windowContentRatio);

            var minimalGripSize = 1;
            var maximumGripSize = trackSize;

            if (gripSize < minimalGripSize)
            {
                gripSize = minimalGripSize;
            }

            if (gripSize > maximumGripSize)
            {
                gripSize = maximumGripSize;
            }

            var windowScrollAreaSize = contentSize - windowSize;
            if (windowScrollAreaSize < 0)
            {
                windowScrollAreaSize = windowSize;
            }

            var windowPositionRatio = (float)windowPosition / windowScrollAreaSize;
            var trackScrollAreaSize = trackSize - gripSize;
            var start = (int)(trackScrollAreaSize * windowPositionRatio);
            var end = start + gripSize;

            if (start == PrevScrollStartIndex && end == PrevScrollEndIndex)
            {
                return;
            }

            PrevScrollStartIndex = start;
            PrevScrollEndIndex = end;

            for (var i = ToolbarHeight; i < Console.WindowHeight; i++)
            {
                var index = i - ToolbarHeight;
                var color = index >= start && index < end ? Theme.GetScrollGripColor() : Theme.GetScrollBackgroundColor();
                Console.SetCursorPosition(Console.WindowWidth - 1, i);
                Console.BackgroundColor = color;
                Console.Write(" ");
            }

            Console.ResetColor();
        }

        private void SetPrevs(int firstIndex)
        {
            PrevFirstIndex = firstIndex;
            PrevActiveIndex = Context.ActiveIndex;
            PrevWindowWidth = Console.WindowWidth;
            PrevWindowHeight = Console.WindowHeight;
        }

        private void ResetPrevs()
        {
            PrevEntries = Context.Entries;
            PrevFirstIndex = 0;
            PrevActiveIndex = null;
            PrevScrollStartIndex = 0;
            PrevScrollEndIndex = 0;
        }

        private bool IsResetPrevsRequired()
        {
            return
                Context.Entries != PrevEntries ||
                Console.WindowWidth != PrevWindowWidth ||
                Console.WindowHeight != PrevWindowHeight;
        }

        private void SetCursorColor()
        {
            Console.BackgroundColor = Theme.GetCursorBackgroundColor();
            Console.ForegroundColor = Theme.GetCursorForegroundColor();
        }

        private void ResetCursorColor()
        {
            Console.ResetColor();
        }

        private static void Write(string value, int leftPadding = 3)
        {
            Console.Write((value ?? string.Empty).PadRight(Console.WindowWidth - leftPadding));
        }
    }
}
 */