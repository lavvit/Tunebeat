using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DxLibDLL.DX;

namespace SeaDrop
{
    public class TextDebug
    {
        public static void Draw()
        {
            DrawBox(0, 1040, GetDrawStringWidth(Input.Text, Input.Text.Length) + 40, 1080, 0x000000, TRUE);
            DrawBox(20 + 9 * Input.Selection.Start, 1040, 20 + 9 * Input.Selection.End, 1080, 0x0000ff, TRUE);
            DrawString(20, 1052, Input.Text, 0xffffff);
            DrawString(16 + GetDrawStringWidth(Input.Text, Input.Position), 1052, "|", 0xffff00);
        }
        public static void Update()
        {
            if (Input.IsEnable)
            {
                Draw();

                if (Input.Text.Contains(""))
                {
                    Input.Text = Input.Text.Substring(0, Input.Text.IndexOf(""));
                    Input.Selection = (0, Input.Text.Length);
                }
            }
        }
    }
}
