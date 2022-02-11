using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class Create : Scene
    {
        public override void Enable()
        {
            CreateMode = false;
            AllText = new List<string>();
            using (StreamReader sr = new StreamReader(Game.MainTJA[0].TJAPath, Encoding.GetEncoding("SHIFT_JIS")))
            {
                while (sr.Peek() > -1)
                {
                    AllText.Add(sr.ReadLine());
                }
            }
            Course = new List<string>();
            string dif = ($"{(ECourse)Game.Course[0]}").ToLower();
            foreach (string s in AllText)
            {
                if (ad)
                {
                    Course.Add(s);
                }
                if (s.ToLower().StartsWith($"course:{dif}"))
                {
                    ad = true;
                    Course.Add(s);
                }
                else if (ad && s.StartsWith("#END"))
                {
                    ad = false;
                }
            }
            NowScroll = 0;
            base.Enable();
        }

        public override void Disable()
        {
            AllText = null;
            Course = null;
            base.Disable();
        }
        public override void Draw()
        {
            DrawString(1000, 0, $"{CreateMode}", 0xffffff);
            if (CreateMode)
            {
                TextureLoad.Game_Notes.Draw(195 * 4 + 98, 520, new Rectangle(195 * 6, 0, 195 * 2, 195));
                TextureLoad.Game_Notes.Draw(0, 520, new Rectangle(195, 0, 195 * 5, 195));
                TextureLoad.Game_Notes.Draw(98, 520 + 195, new Rectangle(195 * 9, 0, 195 * 2, 195));
                TextureLoad.Game_Notes.Draw(0, 520 + 195, new Rectangle(195 * 8, 0, 195, 195));
                TextureLoad.Game_Notes.Draw(195 * 2, 520 + 195, new Rectangle(195 * 11, 0, 195 * 5, 195));
                if (Course != null)
                {
                    for (int i = 0; i < (Course.Count > 54 ? 54 : Course.Count); i++)
                    {
                        DrawString(1280, 20 * i, Course[i + NowScroll], 0xffffff);
                    }
                }
            }
            base.Draw();
        }

        public override void Update()
        {
            if (CreateMode && Mouse.X >= 1280 && AllText != null)
            {
                if (Mouse.Wheel > 0 && NowScroll > 0)
                {
                    NowScroll--;
                }
                if (Mouse.Wheel < 0 && NowScroll + 54 < Course.Count)
                {
                    NowScroll++;
                }
            }
            base.Update();
        }

        public static bool CreateMode, ad;
        public static List<string> AllText, Course;
        public static int NowScroll;
    }
}
