using System;
using Xplorer.States;

namespace Xplorer.Components
{
    public class ScrollbarComponent : Component<ScrollbarState>
    {
        public ScrollbarComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render()
        {
            if (State.Visible)
            {
                for (var i = 0; i < Height; i++)
                {
                    var index = i + Top;
                    var color = i >= State.GripStartPosition && i < State.GripEndPosition ? Theme.GetScrollGripColor() : Theme.GetScrollBackgroundColor();
                    Console.SetCursorPosition(Left, index);
                    Console.BackgroundColor = color;
                    Console.Write(" ");
                }

                Console.ResetColor();
            }
            else
            {
                for (var i = 0; i < Height; i++)
                {
                    var index = i + Top;
                    Console.SetCursorPosition(Left, index);
                    Console.Write(" ");
                }
            }
        }
    }
}