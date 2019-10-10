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

            var model = NavigationActions.CreateModel(Context);

            while (true)
            {
                MasterComponent.Render(model);
                Console.SetCursorPosition(0, 0);

                var input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.Spacebar)
                {
                    model = NavigationActions.Reveal(Context, model);
                    continue;
                }

                switch (input.Key)
                {
                    case ConsoleKey.DownArrow: 
                        model = NavigationActions.MoveDown(Context, model);
                        break;
                    case ConsoleKey.UpArrow: 
                        model = NavigationActions.MoveUp(Context, model);
                        break;
                    case ConsoleKey.LeftArrow: 
                        model = NavigationActions.MoveLeft(Context, model);
                        break;
                    case ConsoleKey.RightArrow:
                        model = NavigationActions.MoveRight(Context, model);
                        break;
                    case ConsoleKey.Enter: 
                        model = NavigationActions.Navigate(Context, model);
                        break;
                    case ConsoleKey.Escape:
                        model = NavigationActions.Escape(Context, model);
                        break;
                    case ConsoleKey.Backspace:
                        model = NavigationActions.Back(Context, model);
                        break;
                    case ConsoleKey.Tab:
                        model = NavigationActions.ToggleActiveNavigation(Context, model);
                        break;
                }

                if (char.IsLetterOrDigit(input.KeyChar))
                {
                    model = NavigationActions.ApplyFilter(Context, model, input.KeyChar);
                }
            }
        }
    }
}