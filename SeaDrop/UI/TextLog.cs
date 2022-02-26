using static DxLibDLL.DX;

namespace SeaDrop
{
    public class TextLog
    {
        public static void Init()
        {
            Timer = new Counter(0, 1000, 1000, false);
        }
        public static void Draw(string text, long time = 1000, uint textcolor = 0xffffff, uint backcolor = 0x000000)
        {
            Text = text;
            TextColor = textcolor;
            BackColor = backcolor;
            Timer = new Counter(0, time - 1, 1000, false);
            Timer.Reset();
            Timer.Start();
        }
        private static void Draw()
        {
            DrawBox(0, 1040, 40 + GetDrawStringWidth(Text, Text.Length), 1080, BackColor, TRUE);
            DrawString(20, 1052, Text, TextColor);
        }

        public static void Update()
        {
            Timer.Tick();
            if (Timer.Value == Timer.End) Timer.Stop();

            if (Timer.State != 0)
            {
                Draw();
            }
        }

        public static string Text;
        public static uint TextColor, BackColor;
        public static Counter Timer;
    }
}
