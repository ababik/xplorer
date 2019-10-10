using System;
using Xplorer.Models;

namespace Xplorer.Components
{
    public class ScrollbarComponent : Component<ScrollbarModel>
    {
        public ScrollbarComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render(ScrollbarModel model)
        {
            if (Model?.GripEndPosition != model?.GripEndPosition || Model?.GripStartPosition != model?.GripStartPosition)
            {
                Model = model;
                Render();
            }
        }
        
        public override void Render()
        {
            //System.Diagnostics.Debugger.Log(0, "Render", $"Scroll\n");

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
    }
}