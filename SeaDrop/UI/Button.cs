using System.Drawing;

namespace SeaDrop
{
    /// <summary>
    /// マウスで操作するボタン。
    /// </summary>
    public class Button
    {
        public Button(string path, bool visible = true)
        {
            Texture = new Texture(path);
            Visible = visible;
        }
        public Button(Texture texture, bool visible = true)
        {
            Texture = texture;
            Visible = visible;
        }

        public void Draw(double x, double y, Rectangle? rectangle = null, double rangeX = 0, double rangeY = 0)
        {
            if (Visible) Texture.Draw(x, y, rectangle);
            X = x;
            Y = y;
            RangeX = x + Texture.ActualSize.Width + rangeX;
            RangeY = y + Texture.ActualSize.Height + rangeY;

            Update();
        }
        public void Draw(double x, double y, double opacity, Rectangle? rectangle = null, double rangeX = 0, double rangeY = 0)
        {
            if (Visible) Texture.Draw(x, y, opacity, rectangle);
            X = x;
            Y = y;
            RangeX = x + Texture.ActualSize.Width + rangeX;
            RangeY = y + Texture.ActualSize.Height + rangeY;

            Update();
        }
        public void Draw(double x, double y, double scaleX, double scaleY, Rectangle? rectangle = null, double rangeX = 0, double rangeY = 0)
        {
            if (Visible) Texture.Draw(x, y, scaleX, scaleY, rectangle);
            X = x;
            Y = y;
            RangeX = x + Texture.ActualSize.Width + rangeX;
            RangeY = y + Texture.ActualSize.Height + rangeY;

            Update();
        }
        public void Draw(double x, double y, double opacity, double scaleX, double scaleY, Rectangle? rectangle = null, double rangeX = 0, double rangeY = 0)
        {
            if (Visible) Texture.Draw(x, y, opacity, scaleX, scaleY, rectangle);
            X = x;
            Y = y;
            RangeX = x + Texture.ActualSize.Width + rangeX;
            RangeY = y + Texture.ActualSize.Height + rangeY;

            Update();
        }

        public void Update()
        {
            if (Mouse.X >= X && Mouse.X < RangeX && Mouse.Y >= Y && Mouse.Y < RangeY)
            {
                Cursoring = true;
                if (Mouse.IsPushed(MouseButton.Left)) LPushed = true;
                else LPushed = false;
                if (Mouse.IsPushing(MouseButton.Left)) LPushing = true;
                else LPushing = false;
                if (Mouse.IsLeft(MouseButton.Left)) LLeft = true;
                else LLeft = false;
                if (Mouse.IsPushed(MouseButton.Right)) RPushed = true;
                else RPushed = false;
                if (Mouse.IsPushing(MouseButton.Right)) RPushing = true;
                else RPushing = false;
                if (Mouse.IsLeft(MouseButton.Right)) RLeft = true;
                else RLeft = false;
            }
            else Cursoring = false;
        }

        public Texture Texture;
        public double X, Y, RangeX, RangeY;
        public bool Cursoring, LPushed, LPushing, LLeft, RPushed, RPushing, RLeft, Visible;
    }
}
