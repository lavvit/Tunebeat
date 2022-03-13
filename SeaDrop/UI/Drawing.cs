using System.Collections.Generic;
using System.Drawing;
using static DxLibDLL.DX;

namespace SeaDrop
{
    public class Drawing
    {
        /// <summary>
        /// RGB値からint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color(int red, int green, int blue)
        {
            return (int)GetColor(red, green, blue);
        }
        /// <summary>
        /// カラーコードからint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color(string color)
        {
            return ColorTranslator.ToWin32(ColorTranslator.FromHtml(color));
        }
        /// <summary>
        /// Colorからint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color(Color color)
        {
            return ColorTranslator.ToWin32(color);
        }
        /// <summary>
        /// 点を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="color">色</param>
        public static void Pixel(int x, int y, int color = 0xffffff)
        {
            DrawPixel(x, y, (uint)color);
        }
        /// <summary>
        /// 直線を描画します。
        /// </summary>
        /// <param name="x">始点X座標</param>
        /// <param name="y">始点Y座標</param>
        /// <param name="rangeX">終点X相対座標</param>
        /// <param name="rangeY">終点Y相対座標</param>
        /// <param name="color">色</param>
        /// <param name="antiAging">アンチエイジングをするか</param>
        public static void Line(double x, double y, double rangeX, double rangeY, int color = 0xffffff, bool antiAging = true)
        {
            if (antiAging) DrawLineAA((float)x, (float)y, (float)(x + rangeX), (float)(y + rangeY), (uint)color);
            else DrawLine((int)x, (int)y, (int)(x + rangeX), (int)(y + rangeY), (uint)color);
        }
        /// <summary>
        /// 四角を描画します。
        /// </summary>
        /// <param name="x">始点X座標</param>
        /// <param name="y">始点Y座標</param>
        /// <param name="rangeX">終点X相対座標</param>
        /// <param name="rangeY">終点Y相対座標</param>
        /// <param name="fill">塗りつぶすか</param>
        /// <param name="color">色</param>
        /// <param name="antiAging">アンチエイジングをするか</param>
        public static void Box(double x, double y, double rangeX, double rangeY, int color = 0xffffff, bool fill = true, bool antiAging = true)
        {
            if (antiAging) DrawBoxAA((float)x, (float)y, (float)(x + rangeX), (float)(y + rangeY), (uint)color, fill ? TRUE : FALSE);
            else DrawBox((int)x, (int)y, (int)(x + rangeX), (int)(y + rangeY), (uint)color, fill ? TRUE : FALSE);
        }
        /// <summary>
        /// 三角を描画します。
        /// </summary>
        /// <param name="x1">X座標1</param>
        /// <param name="y1">Y座標1</param>
        /// <param name="x2">X座標2</param>
        /// <param name="y2">Y座標2</param>
        /// <param name="x3">X座標3</param>
        /// <param name="y3">Y座標3</param>
        /// <param name="rangeX">終点X相対座標</param>
        /// <param name="rangeY">終点Y相対座標</param>
        /// <param name="fill">塗りつぶすか</param>
        /// <param name="color">色</param>
        /// <param name="antiAging">アンチエイジングをするか</param>
        public static void Triangle(double x1, double y1, double x2, double y2, double x3, double y3, int color = 0xffffff, bool fill = true, bool antiAging = true)
        {
            if (antiAging) DrawTriangleAA((float)x1, (float)y1, (float)x2, (float)y2, (float)x3, (float)y3, (uint)color, fill ? TRUE : FALSE);
            else DrawTriangle((int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, (uint)color, fill ? TRUE : FALSE);
        }
        /// <summary>
        /// 円を描画します。
        /// </summary>
        /// <param name="x">中心X座標</param>
        /// <param name="y">中心Y座標</param>
        /// <param name="r">半径</param>
        /// <param name="fill">塗りつぶすか</param>
        /// <param name="color">色</param>
        /// <param name="antiAging">アンチエイジングをするか</param>
        /// <param name="posnum">アンチエイジング時の円の細かさ</param>
        public static void Circle(double x, double y, double r, int color = 0xffffff, bool fill = true, bool antiAging = true, int posnum = 64)
        {
            if (antiAging) DrawCircleAA((float)x, (float)y, (float)r, posnum, (uint)color, fill ? TRUE : FALSE);
            else DrawCircle((int)x, (int)y, (int)r, (uint)color, fill ? TRUE : FALSE);
        }
        /// <summary>
        /// 楕円を描画します。
        /// </summary>
        /// <param name="x">中心X座標</param>
        /// <param name="y">中心Y座標</param>
        /// <param name="rX">X半径</param>
        /// <param name="rY">Y半径</param>
        /// <param name="fill">塗りつぶすか</param>
        /// <param name="color">色</param>
        /// <param name="antiAging">アンチエイジングをするか</param>
        /// <param name="posnum">アンチエイジング時の円の細かさ</param>
        public static void Oval(double x, double y, double rX, double rY, int color = 0xffffff, bool fill = true, bool antiAging = true, int posnum = 64)
        {
            if (antiAging) DrawOvalAA((float)x, (float)y, (float)rX, (float)rY, posnum, (uint)color, fill ? TRUE : FALSE);
            else DrawOval((int)x, (int)y, (int)rX, (int)rY, (uint)color, fill ? TRUE : FALSE);
        }
        /// <summary>
        /// 文字を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="text">テキスト</param>
        /// <param name="color">色</param>
        public static void Text(int x, int y, string text, int color = 0xffffff)
        {
            DrawString(x, y, text, (uint)color);
        }
        /// <summary>
        /// 文字を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="text">テキスト</param>
        /// <param name="color">色</param>
        /// <param name="size">大きさ</param>
        public static void Text(int x, int y, int size, string text, int color = 0xffffff)
        {
            SetFontSize(size);
            DrawString(x, y, text, (uint)color);
            SetFontSize(16);
        }
        /// <summary>
        /// 文字を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="text">テキスト</param>
        /// <param name="color">色</param>
        /// <param name="font">フォント</param>
        public static void Text(int x, int y, string text, string font, int color = 0xffffff)
        {
            ChangeFont(font);
            DrawString(x, y, text, (uint)color);
            ChangeFont("");
        }
        /// <summary>
        /// 文字を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="text">テキスト</param>
        /// <param name="color">色</param>
        /// <param name="size">大きさ</param>
        /// <param name="font">フォント</param>
        public static void Text(int x, int y, int size, string text, string font, int color = 0xffffff)
        {
            SetFontSize(size);
            ChangeFont(font);
            DrawString(x, y, text, (uint)color);
            SetFontSize(16);
            ChangeFont("");
        }
        /// <summary>
        /// 文字を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="text">テキスト</param>
        /// <param name="color">色</param>
        /// <param name="handle">ハンドル</param>
        public static void Text(int x, int y, string text, Handle handle, int color = 0xffffff)
        {
            DrawStringToHandle(x, y, text, (uint)color, handle.ID);
        }
        /// <summary>
        /// 文字の幅を計算します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>テキストの幅</returns>
        public static int TextWidth(string text, Handle handle = null)
        {
            if (handle != null) return GetDrawStringWidthToHandle(text, text.Length, handle.ID);
            return GetDrawStringWidth(text, text.Length);
        }
        /// <summary>
        /// 文字の幅を計算します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="length">最初からの文字数</param>
        /// <returns>テキストの幅</returns>
        public static int TextWidth(string text, int length, Handle handle = null)
        {
            if (handle != null) return GetDrawStringWidthToHandle(text, length, handle.ID);
            return GetDrawStringWidth(text, length);
        }
        /// <summary>
        /// 文字の幅を計算します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="start">始点</param>
        /// <param name="length">始点からの文字数</param>
        /// <returns>テキストの幅</returns>
        public static int TextWidth(string text, int start, int length, Handle handle = null)
        {
            string str = text.Substring(start, length);
            if (handle != null) return GetDrawStringWidthToHandle(str, str.Length, handle.ID);
            return GetDrawStringWidth(str, str.Length);
        }
        /// <summary>
        /// 文字の最大幅を計算します。
        /// </summary>
        /// <param name="text">テキスト群</param>
        /// <returns>テキストの幅</returns>
        public static int TextWidth(string[] text, Handle handle = null)
        {
            int size = 0;
            for (int i = 0; i < text.Length; i++)
            {
                int s = handle != null ? GetDrawStringWidthToHandle(text[i], text[i].Length, handle.ID) : GetDrawStringWidth(text[i], text[i].Length);
                if (s > size) size = s;
            }
            return size;
        }
        /// <summary>
        /// 文字の最大幅を計算します。
        /// </summary>
        /// <param name="text">テキスト群</param>
        /// <returns>テキストの幅</returns>
        public static int TextWidth(List<string> text, Handle handle = null)
        {
            int size = 0;
            for (int i = 0; i < text.Count; i++)
            {
                int s = handle != null ? GetDrawStringWidthToHandle(text[i], text[i].Length, handle.ID) : GetDrawStringWidth(text[i], text[i].Length);
                if (s > size) size = s;
            }
            return size;
        }
    }

    public class Handle
    {
        public Handle(string font)
        {
            ID = CreateFontToHandle(font, -1, -1);
        }
        public Handle(int size)
        {
            ID = CreateFontToHandle(null, size, -1);
        }
        public Handle(string font, int size)
        {
            ID = CreateFontToHandle(font, size, -1);
        }
        public Handle(int size, string font)
        {
            ID = CreateFontToHandle(font, size, -1);
        }

        public void Dispose()
        {
            DeleteFontToHandle(ID);
        }
        public static void End()
        {
            InitFontToHandle();
        }

        public int ID;
    }
}
