using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;
using Amaoto;
using Tunebeat.Common;

namespace Tunebeat.SongSelect
{
    public class SongSelect : Scene
    {
        public override void Enable()
        {
            NowCourse[0] = PlayData.PlayCourse[0];
            NowCourse[1] = PlayData.PlayCourse[1];
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }
        public override void Draw()
        {
            #if DEBUG
            DrawString(0, 0, $"{PlayData.PlayFile}", 0xffffff);
            DrawString(0, 20, $"{NowCourse[0]}" + (PlayData.IsPlay2P ? $"/{NowCourse[1]}" : ""), 0xffffff);
            if (PlayData.Auto[0])
            {
                DrawString(0, 40, "1P AUTO", 0xffffff);
            }
            if (PlayData.Auto[1] && PlayData.IsPlay2P)
            {
                DrawString(80, 40, "2P AUTO", 0xffffff);
            }
            DrawString(0, 60, "PRESS ENTER", 0xffffff);
            #endif
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(KEY_INPUT_RETURN))
            {
                PlayData.PlayCourse[0] = NowCourse[0];
                PlayData.PlayCourse[1] = NowCourse[1];
                Program.SceneChange(new Game.Game());
            }
            if (Key.IsPushed(KEY_INPUT_ESCAPE))
            {
                Program.End();
            }

            if (Key.IsPushed(KEY_INPUT_F1))
            {
                PlayData.Auto[0] = !PlayData.Auto[0];
            }
            if (Key.IsPushed(KEY_INPUT_F2) && PlayData.IsPlay2P)
            {
                PlayData.Auto[1] = !PlayData.Auto[1];
            }
            if (Key.IsPushed(KEY_INPUT_F5))
            {
                PlayData.IsPlay2P = !PlayData.IsPlay2P;
            }
            if (Key.IsPushed(KEY_INPUT_LEFT))
            {
                if (Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT))
                {
                    CourseChange(true, 1);
                }
                else
                {
                    CourseChange(true, 0);
                }
            }
            if (Key.IsPushed(KEY_INPUT_RIGHT))
            {
                if (Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT))
                {
                    CourseChange(false, 1);
                }
                else
                {
                    CourseChange(false, 0);
                }
            }
            base.Update();
        }

        public static void CourseChange(bool isdim, int player)
        {
            if (!isdim)
            {
                #region [ 上げる ]
                if (NowCourse[player] < 4)
                    NowCourse[player]++;
                else
                    NowCourse[player] = 0;
                #endregion
            }
            else
            {
                #region [ 下げる ]
                if (NowCourse[player] > 0)
                    NowCourse[player]--;
                else
                    NowCourse[player] = 4;
                #endregion
            }

        }

        private static int[] NowCourse = new int[2];
    }
}
