using System;
using Xplorer.Actions;
using Xplorer.Components;

namespace Xplorer;

public class Application
{
    public static void Main(string[] args)
    {
        var application = new Application();
        application.Run();
    }

    public static event Action OnResize;

    private Context Context { get; }

    private Application()
    {
        Context = new Context();
        Context.State = NavigationActions.Init(Context);
        Context.Component = new MasterComponent(Context.Theme);
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

        var width = Console.WindowWidth;
        var height = Console.WindowHeight;

        while (true)
        {
            if (Context.State != null)
            {
                Context.State = NavigationActions.UpdateToolbarActions(Context, Context.State);
                Context.Component.Render(Context.State);
            }

            Console.SetCursorPosition(0, 0);

            var input = Console.ReadKey(true);

            if (Console.WindowWidth != width || Console.WindowHeight != height)
            {
                Console.Clear();
                Console.CursorVisible = false;

                width = Console.WindowWidth;
                height = Console.WindowHeight;
                
                OnResize?.Invoke();
                Context.State = NavigationActions.Resize(Context, Context.State);
            }

            if (input.Key == ConsoleKey.Spacebar)
            {
                Context.State = NavigationActions.Reveal(Context, Context.State);
                continue;
            }

            Context.State = NavigationActions.InterceptTransactionAction(Context, Context.State);

            if (Context.Model.Transaction != null)
            {
                continue;
            }

            switch (input.Key)
            {
                case ConsoleKey.DownArrow: 
                    Context.State = NavigationActions.MoveDown(Context, Context.State);
                    break;
                case ConsoleKey.UpArrow: 
                    Context.State = NavigationActions.MoveUp(Context, Context.State);
                    break;
                case ConsoleKey.LeftArrow: 
                    Context.State = NavigationActions.MoveLeft(Context, Context.State);
                    break;
                case ConsoleKey.RightArrow:
                    Context.State = NavigationActions.MoveRight(Context, Context.State);
                    break;
                case ConsoleKey.Enter: 
                    Context.State = NavigationActions.Navigate(Context, Context.State);
                    break;
                case ConsoleKey.Escape:
                    Context.State = NavigationActions.Escape(Context, Context.State);
                    break;
                case ConsoleKey.Backspace:
                    Context.State = NavigationActions.Back(Context, Context.State);
                    break;
                case ConsoleKey.F5:
                    Context.State = NavigationActions.Copy(Context, Context.State);
                    break;
            }

            switch (input.KeyChar)
            {
                case ']': Context.State = NavigationActions.ToggleSecondaryNavigation(Context, Context.State); break;
                case '[': Context.State = NavigationActions.TogglePrimaryNavigation(Context, Context.State); break;
                case '/': Context.State = NavigationActions.ToggleSelectedItem(Context, Context.State); break;
                case '?': Context.State = NavigationActions.RevertSelectedItems(Context, Context.State); break;
            }

            if (char.IsLetterOrDigit(input.KeyChar))
            {
                Context.State = NavigationActions.ApplyFilter(Context, Context.State, input.KeyChar);
            }
        }
    }
}