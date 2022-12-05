using System;
using Xplorer.Models;
using Xplorer.States;

namespace Xplorer.Components;

public class NavigationItemComponent : Component<NavigationItemState>
{
    public NavigationItemComponent(ITheme theme) : base(theme)
    {
    }

    public override void Render()
    {
        Console.SetCursorPosition(Left, Top);
        
        if (State == null)
        {
            Write(null);
            return;
        }

        var entry = State.Entry;

        RenderMarker(entry);

        if (State.IsActive)
        {
            SetCursorColor();
        }

        if (State.IsSelected)
        {
            Console.ForegroundColor = Theme.GetSelectedEntryColor();
        }

        var name = entry.Name;
        if (name.Length > Width - 2)
        {
            name = name.Remove(Width - 2 - 3) + "...";
        }

        Write(name, 2);

        if (State.IsActive || State.IsSelected)
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