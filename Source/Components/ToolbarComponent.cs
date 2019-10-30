using System;
using Xplorer.States;

namespace Xplorer.Components
{
    public class ToolbarComponent : Component<ToolbarState>
    {
        public static int BaseHeight => 1;

        public ToolbarComponent(ITheme theme) : base(theme)
        {
        }

        public override void Render()
        {
            switch (State.Dialog)
            {
                case CopyCheckDialogState dialog: RenderCopyCheckDialog(); break;
                case CopyConfirmDialogState dialog: RenderCopyConfirmDialog(); break;
                default: RenderDefault(); break;
            }
        }

        private void RenderCopyCheckDialog()
        {
            Console.SetCursorPosition(Left, Top);
            Console.BackgroundColor = ConsoleColor.Gray;
            Write(null);

            var label = "Preparing to copy...";
            Console.SetCursorPosition(Left, Top);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(label);

            var button = new ButtonComponent(Theme);
            button.Position(Top, Left + label.Length + 1, 0, 0);
            button.Render(new ButtonState("ESC Cancel", true));
        }

        private void RenderCopyConfirmDialog()
        {
            Console.SetCursorPosition(Left, Top);
            Console.BackgroundColor = ConsoleColor.Gray;
            Write(null);

            var label = "Overwrite?";
            Console.SetCursorPosition(Left, Top);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(label);

            var padding = label.Length;

            void RenderButton(ButtonState state)
            {
                var button = new ButtonComponent(Theme);
                button.Position(Top, Left + padding, 0, 0);
                button.Render(state);
                padding += button.Width + 1;
            }

            RenderButton(new ButtonState("Enter Yes", true));
            RenderButton(new ButtonState("ESC Cancel", true));

            Console.ResetColor();
        }

        private void RenderCopyProcessDialog()
        {
            Console.SetCursorPosition(Left, Top);
            Console.BackgroundColor = ConsoleColor.Gray;
            Write(null);

            var label = "Copying...";
            Console.SetCursorPosition(Left, Top);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(label);

            var button = new ButtonComponent(Theme);
            button.Position(Top, Left + label.Length + 1, 0, 0);
            button.Render(new ButtonState("ESC Cancel", true));

            Console.ResetColor();
        }

        private void RenderDefault()
        {
            Console.SetCursorPosition(Left, Top);
            Write(null);

            var padding = 0;

            void RenderButton(ButtonState state)
            {
                var button = new ButtonComponent(Theme);
                button.Position(Top, Left + padding, 0, 0);
                button.Render(state);
                padding += button.Width + 1;
            }

            RenderButton(State.HelpButton);
            RenderButton(State.CopyButton);
            RenderButton(State.MoveButton);
            RenderButton(State.RenameButton);
            RenderButton(State.RemoveButton);
        }
    }
}