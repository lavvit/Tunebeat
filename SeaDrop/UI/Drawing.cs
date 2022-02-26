using static DxLibDLL.DX;

namespace SeaDrop
{
    public class Drawing
    {
        /// <summary>
        /// RGB値からuint形式の色を取得します。
        /// </summary>
        public static uint Color(int red, int green, int blue)
        {
            return GetColor(red, green, blue);
        }
        /// <summary>
        /// 点を描画します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="color">色</param>
        public static void Pixel(int x, int y, uint color = 0xffffff)
        {
            DrawPixel(x, y, color);
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
        public static void Line(double x, double y, double rangeX, double rangeY, uint color = 0xffffff, bool antiAging = true)
        {
            if (antiAging) DrawLineAA((float)x, (float)y, (float)(x + rangeX), (float)(y + rangeY), color);
            else DrawLine((int)x, (int)y, (int)(x + rangeX), (int)(y + rangeY), color);
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
        public static void Box(double x, double y, double rangeX, double rangeY, bool fill = true, uint color = 0xffffff, bool antiAging = true)
        {
            if (antiAging) DrawBoxAA((float)x, (float)y, (float)(x + rangeX), (float)(y + rangeY), color, fill ? TRUE : FALSE);
            else DrawBox((int)x, (int)y, (int)(x + rangeX), (int)(y + rangeY), color, fill ? TRUE : FALSE);
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
        public static void Triangle(double x1, double y1, double x2, double y2, double x3, double y3, bool fill = true, uint color = 0xffffff, bool antiAging = true)
        {
            if (antiAging) DrawTriangleAA((float)x1, (float)y1, (float)x2, (float)y2, (float)x3, (float)y3, color, fill ? TRUE : FALSE);
            else DrawTriangle((int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, color, fill ? TRUE : FALSE);
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
        public static void Circle(double x, double y, double r, bool fill = true, uint color = 0xffffff, bool antiAging = true, int posnum = 64)
        {
            if (antiAging) DrawCircleAA((float)x, (float)y, (float)r, posnum, color, fill ? TRUE : FALSE);
            else DrawCircle((int)x, (int)y, (int)r, color, fill ? TRUE : FALSE);
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
        public static void Oval(double x, double y, double rX, double rY, bool fill = true, uint color = 0xffffff, bool antiAging = true, int posnum = 64)
        {
            if (antiAging) DrawOvalAA((float)x, (float)y, (float)rX, (float)rY, posnum, color, fill ? TRUE : FALSE);
            else DrawOval((int)x, (int)y, (int)rX, (int)rY, color, fill ? TRUE : FALSE);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="text">テキスト</param>
        /// <param name="color">色</param>
        public static void Text(int x, int y, string text, uint color = 0xffffff)
        {
            DrawString(x, y, text, color);
        }
    }
}
