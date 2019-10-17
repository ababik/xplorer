using System;
using Xplorer.States;

namespace Xplorer.Components
{
    public class ToolbarComponent : Component<ToolbarState>
    {
        public static int Height => 1;
        
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