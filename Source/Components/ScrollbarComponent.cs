using System;
using Xplorer.Models;

namespace Xplorer.Components
{
    public class ScrollbarComponent : Component<ScrollbarModel>
    {
        public ScrollbarComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render()
        {
            if (Model.Visible)
            {
                for (var i = 0; i < Height; i++)
                {
                    var index = i + Top;
                    var color = i >= Model.GripStartPosition && i < Model.GripEndPosition ? Theme.GetScrollGripColor() : Theme.GetScrollBackgroundColor();
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