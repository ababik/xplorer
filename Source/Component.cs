using System;

namespace Xplorer
{
    public abstract class Component<TModel>
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private int PrevTop { get; set; }
        private int PrevLeft { get; set; }
        private int PrevWidth { get; set; }
        private int PrevHeight { get; set; }
        protected ITheme Theme { get; }
        protected TModel Model { get; set; }

        public Component(ITheme theme)
        {
            Theme = theme;
        }

        public abstract void Render();

        public virtual void Render(TModel model)
        {
            if ((object.Equals(model, Model) == false) ||  (Top != PrevTop) || (Left != PrevLeft) || (Width != PrevWidth || (Height != PrevHeight)))
            {
                Model = model;
                
                PrevTop = Top;
                PrevLeft = Left;
                PrevWidth = Width;
                PrevHeight = Height;

                Render();
            }
        }

        public void Position(int top, int left, int width, int height)
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
        }

        protected void Write(string value, int padding = 0)
        {
            Console.Write((value ?? string.Empty).PadRight(Width - padding));
        }
    }
}