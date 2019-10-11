using System;
using Xplorer.Models;

namespace Xplorer.Components
{
    public class NavigationItemComponent : Component<NavigationItemModel>
    {
        public NavigationItemComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render()
        {
            Console.SetCursorPosition(Left, Top);
            
            if (Model == null)
            {
                Write(null);
                return;
            }

            var entry = Model.Entry;

            RenderMarker(entry);

            if (Model.IsActive)
            {
                SetCursorColor();
            }

            Write(entry.Name, 2);

            if (Model.IsActive)
            {
                ResetCursorColor();
            }
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

        private void SetCursorColor()
        {
            Console.BackgroundColor = Theme.GetCursorBackgroundColor();
            Console.ForegroundColor = Theme.GetCursorForegroundColor();
        }

        private void ResetCursorColor()
        {
            Console.ResetColor();
        }
    }
}