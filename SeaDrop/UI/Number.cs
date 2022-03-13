using System.Drawing;

namespace SeaDrop
{
    public class Number
    {
        public Number(string path, int width, int height, int space = 0)
        {
            Texture = new Texture(path);
            Width = width;
            Height = height;
            Space = space;
            stNumber = new STNumber[10]
            {
                new STNumber(){ ch = '0', X = 0 }, new STNumber(){ ch = '1', X = width }, new STNumber(){ ch = '2', X = width * 2 }, new STNumber(){ ch = '3', X = width * 3 },
                new STNumber(){ ch = '4', X = width * 4 }, new STNumber(){ ch = '5', X = width * 5 }, new STNumber(){ ch = '6', X = width * 6 },
                new STNumber(){ ch = '7', X = width * 7 }, new STNumber(){ ch = '8', X = width * 8 }, new STNumber(){ ch = '9', X = width * 9 }
            };
        }
        public Number(Texture texture, int width, int height, int space = 0)
        {
            Texture = texture;
            Width = width;
            Height = height;
            Space = space;
            stNumber = new STNumber[10]
            {
                new STNumber(){ ch = '0', X = 0 }, new STNumber(){ ch = '1', X = width }, new STNumber(){ ch = '2', X = width * 2 }, new STNumber(){ ch = '3', X = width * 3 },
                new STNumber(){ ch = '4', X = width * 4 }, new STNumber(){ ch = '5', X = width * 5 }, new STNumber(){ ch = '6', X = width * 6 },
                new STNumber(){ ch = '7', X = width * 7 }, new STNumber(){ ch = '8', X = width * 8 }, new STNumber(){ ch = '9', X = width * 9 }
            };
        }
        public Number(string path, int width, int height, STNumber[] stnum, int space = 0)
        {
            Texture = new Texture(path);
            Width = width;
            Height = height;
            Space = space;
            stNumber = stnum;
        }
        public Number(Texture texture, int width, int height, STNumber[] stnum, int space = 0)
        {
            Texture = texture;
            Width = width;
            Height = height;
            Space = space;
            stNumber = stnum;
        }

        public void Draw(double x, double y, object num)
        {
            foreach (char ch in $"{num}")
            {
                for (int i = 0; i < stNumber.Length; i++)
                {
                    if (ch == ' ')
                    {
                        break;
                    }
                    if (stNumber[i].ch == ch)
                    {
                        Texture.Draw(x, y, new Rectangle(stNumber[i].X, 0, Width, Height));
                        break;
                    }
                }
                x += Width + Space;
            }
        }
        public void Draw(double x, double y, object num, int type)
        {
            foreach (char ch in $"{num}")
            {
                for (int i = 0; i < stNumber.Length; i++)
                {
                    if (ch == ' ')
                    {
                        break;
                    }
                    if (stNumber[i].ch == ch)
                    {
                        Texture.Draw(x, y, new Rectangle(stNumber[i].X, Height * type, Width, Height));
                        break;
                    }
                }
                x += Width + Space;
            }
        }

        public static Texture Texture;
        public static int Width, Height, Space;
        public static STNumber[] stNumber;
    }

    public struct STNumber
    {
        public char ch;
        public int X;
    }
}
