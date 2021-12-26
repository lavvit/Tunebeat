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
            NowCourse = PlayData.PlayCourse;
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
            DrawString(0, 20, $"{NowCourse}", 0xffffff);
            if (PlayData.Auto)
            {
                DrawString(20, 20, "AUTO PLAY", 0xffffff);
            }
            DrawString(0, 40, "PRESS ENTER", 0xffffff);
            #endif
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(KEY_INPUT_RETURN))
            {
                PlayData.PlayCourse = NowCourse;
                Program.SceneChange(new Game.Game());
            }
            if (Key.IsPushed(KEY_INPUT_ESCAPE))
            {
                Program.End();
            }

            if (Key.IsPushed(KEY_INPUT_F3))
            {
                PlayData.Auto = !PlayData.Auto;
            }
            if (Key.IsPushed(KEY_INPUT_LEFT))
            {
                CourseChange(true);
            }
            if (Key.IsPushed(KEY_INPUT_RIGHT))
            {
                CourseChange(false);
            }
            base.Update();
        }

        public static void CourseChange(bool isdim)
        {
            if (!isdim)
            {
                #region [ 上げる ]
                if (NowCourse < 4)
                    NowCourse++;
                else
                    NowCourse = 0;
                #endregion
            }
            else
            {
                #region [ 下げる ]
                if (NowCourse > 0)
                    NowCourse--;
                else
                    NowCourse = 4;
                #endregion
            }

        }

        private static int NowCourse;
    }
}
