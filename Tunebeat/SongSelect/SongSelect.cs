using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;
using Tunebeat.Common;
using Tunebeat.Game;

namespace Tunebeat.SongSelect
{
    public class SongSelect : Scene
    {
        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }
        public override void Draw()
        {
            #if DEBUG
            DrawString(0, 0, $"File:{PlayData.PlayFile}", 0xffffff);
            DrawString(0, 20, $"Course:{(ECourse)PlayData.PlayCourse[0]}" + (PlayData.IsPlay2P ? $"/{(ECourse)PlayData.PlayCourse[1]}" : ""), 0xffffff);
            if (PlayData.Auto[0])
            {
                DrawString(0, 40, "1P AUTO", 0xffffff);
            }
            if (PlayData.Auto[1] && PlayData.IsPlay2P)
            {
                DrawString(80, 40, "2P AUTO", 0xffffff);
            }
            DrawString(0, 60, $"Gauge:{(EGauge)PlayData.GaugeType[0]}" + (PlayData.IsPlay2P ? $"/{(EGauge)PlayData.GaugeType[1]}" : ""), 0xffffff);
            DrawString(0, 80, $"GAS:{(EGaugeAutoShift)PlayData.GaugeAutoShift[0]}" + (PlayData.IsPlay2P ? $"/{(EGaugeAutoShift)PlayData.GaugeAutoShift[1]}" : ""), 0xffffff);
            DrawString(0, 100, $"GASmin:{(EGauge)PlayData.GaugeAutoShiftMin[0]}" + (PlayData.IsPlay2P ? $"/{(EGauge)PlayData.GaugeAutoShiftMin[1]}" : ""), 0xffffff);
            DrawString(0, 120, $"Hazard:{PlayData.Hazard[0]}" + (PlayData.IsPlay2P ? $"/{PlayData.Hazard[1]}" : ""), 0xffffff);

            DrawString(0, 140, "PRESS ENTER", 0xffffff);
            #endif
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(KEY_INPUT_RETURN))
            {
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
            if (Key.IsPushed(KEY_INPUT_F3))
            {
                if (Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT))
                {
                    GASChange(0);
                }
                else
                {
                    GaugeChange(0);
                }
            }
            if (Key.IsPushed(KEY_INPUT_F4) && PlayData.IsPlay2P)
            {
                if (Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT))
                {
                    GASChange(1);
                }
                else
                {
                    GaugeChange(1);
                }
            }
            if (Key.IsPushed(KEY_INPUT_F5))
            {
                PlayData.IsPlay2P = !PlayData.IsPlay2P;
            }
            if (Key.IsPushed(KEY_INPUT_F7))
            {
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.IsPlay2P)
                {
                    PlayData.Hazard[1]--;
                }
                else
                {
                    PlayData.Hazard[0]--;
                }
            }
            if (Key.IsPushed(KEY_INPUT_F8))
            {
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.IsPlay2P)
                {
                    PlayData.Hazard[1]++;
                }
                else
                {
                    PlayData.Hazard[0]++;
                }
            }

            if (Key.IsPushed(KEY_INPUT_LEFT))
            {
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.IsPlay2P)
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
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.IsPlay2P)
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
                if (PlayData.PlayCourse[player] < 4)
                    PlayData.PlayCourse[player]++;
                else
                    PlayData.PlayCourse[player] = 0;
                #endregion
            }
            else
            {
                #region [ 下げる ]
                if (PlayData.PlayCourse[player] > 0)
                    PlayData.PlayCourse[player]--;
                else
                    PlayData.PlayCourse[player] = 4;
                #endregion
            }

        }

        public static void GaugeChange(int player)
        {
            if (PlayData.GaugeType[player] < (int)EGauge.EXHard)
                PlayData.GaugeType[player]++;
            else
                PlayData.GaugeType[player] = (int)EGauge.Normal;
        }
        public static void GASChange(int player)
        {
            if (PlayData.GaugeAutoShift[player] < (int)EGaugeAutoShift.LessBest)
                PlayData.GaugeAutoShift[player]++;
            else
                PlayData.GaugeAutoShift[player] = (int)EGaugeAutoShift.None;
        }
    }
}
