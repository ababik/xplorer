using System;
using Xplorer.Models;

namespace Xplorer.Components
{
    public class ToolbarComponent : Component<ToolbarModel>
    {
        public ToolbarComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render()
        {
            Console.SetCursorPosition(Left, Top);
            Console.BackgroundColor = ConsoleColor.Blue;
            Write(null);
            Console.ResetColor();
        }
    }
}