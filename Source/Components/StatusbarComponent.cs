using System;
using Xplorer.Models;

namespace Xplorer.Components
{
    public class StatusbarComponent : Component<StatusbarModel>
    {
        public StatusbarComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render()
        {
            Console.SetCursorPosition(Left, Top);
            Write(Model.Status);
        }
    }
}