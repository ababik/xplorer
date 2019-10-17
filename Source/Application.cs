using System;
using Xplorer.Actions;
using Xplorer.Components;

namespace Xplorer
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var application = new Application();
            application.Run();
        }

        public static event Action OnResize;

        private Context Context { get; }
        private MasterComponent MasterComponent { get; }

        private Application()
        {
            Context = new Context();
            MasterComponent = new MasterComponent(Context.Theme);
        }

        public void Run()
        {
            Console.CancelKeyPress += (sender, args) =>
            {
                Console.Clear();
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, 0);
                System.Environment.Exit(0);
            };

            Console.Clear();
            Console.CursorVisible = false;

            var state = NavigationActions.Init(Context);
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            while (true)
            {
                MasterComponent.Render(state);
                Console.SetCursorPosition(0, 0);

                var input = Console.ReadKey(true);

                if (Console.WindowWidth != width || Console.WindowHeight != height)
                {
                    Console.Clear();
                    Console.CursorVisible = false;

                    width = Console.WindowWidth;
                    height = Console.WindowHeight;
                    
                    OnResize?.Invoke();
                    state = NavigationActions.Resize(Context, state);
                }

                if (input.Key == ConsoleKey.Spacebar)
                {
                    state = NavigationActions.Reveal(Context, state);
                    continue;
                }

                switch (input.Key)
                {
                    case ConsoleKey.DownArrow: 
                        state = NavigationActions.MoveDown(Context, state);
                        break;
                    case ConsoleKey.UpArrow: 
                        state = NavigationActions.MoveUp(Context, state);
                        break;
                    case ConsoleKey.LeftArrow: 
                        state = NavigationActions.MoveLeft(Context, state);
                        break;
                    case ConsoleKey.RightArrow:
                        state = NavigationActions.MoveRight(Context, state);
                        break;
                    case ConsoleKey.Enter: 
                        state = NavigationActions.Navigate(Context, state);
                        break;
                    case ConsoleKey.Escape:
                        state = NavigationActions.Escape(Context, state);
                        break;
                    case ConsoleKey.Backspace:
                        state = NavigationActions.Back(Context, state);
                        break;
                }

                switch (input.KeyChar)
                {
                    case ']': state = NavigationActions.ToggleSecondaryNavigation(Context, state); break;
                    case '[': state = NavigationActions.TogglePrimaryNavigation(Context, state); break;
                    case '/': state = NavigationActions.ToggleSelectedItem(Context, state); break;
                    case '?': state = NavigationActions.RevertSelectedItems(Context, state); break;
                }

                if (char.IsLetterOrDigit(input.KeyChar))
                {
                    state = NavigationActions.ApplyFilter(Context, state, input.KeyChar);
                }
            }
        }
    }
}