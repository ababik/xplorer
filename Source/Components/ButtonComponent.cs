using System;
using Xplorer.States;

namespace Xplorer.Components
{
    public class ButtonComponent : Component<ButtonState>
    {
        public ButtonComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render()
        {
            var label = " " + State.Label + " ";
            Width = label.Length;

            Console.SetCursorPosition(Left, Top);
            Console.BackgroundColor = State.Enabled ? Theme.GetEnabledButtonBackgroundColor() : Theme.GetDisabledButtonBackgroundColor();
            Console.ForegroundColor = State.Enabled ? Theme.GetEnabledButtonForegroundColor() : Theme.GetDisabledButtonForegroundColor();
            Console.Write(label);
            Console.ResetColor();
        }
    }
}