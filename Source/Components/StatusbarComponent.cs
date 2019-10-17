using System;
using Xplorer.States;

namespace Xplorer.Components
{
    public class StatusbarComponent : Component<StatusbarState>
    {
        public static int Height => 1;

        public StatusbarComponent(ITheme theme) : base(theme)
        {
            Application.OnResize += HandleResize;
        }

        private void HandleResize()
        {
            State = null;
        }

        public override void Render()
        {
            Console.SetCursorPosition(Left, Top);
            Write(State.Status);
        }
    }
}