using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
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
            DrawBox(0, 0, 1919, 1079, GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]), TRUE);
            TextureLoad.SongSelect_Background.Draw(0, 0);

            #if DEBUG
            DrawString(0, 0, $"File:{PlayData.Data.PlayFile}", 0xffffff);
            DrawString(0, 20, $"Course:{(ECourse)PlayData.Data.PlayCourse[0]}" + (PlayData.Data.IsPlay2P ? $"/{(ECourse)PlayData.Data.PlayCourse[1]}" : ""), 0xffffff);
            if (PlayData.Data.Auto[0])
            {
                DrawString(0, 40, "1P AUTO", 0xffffff);
            }
            if (PlayData.Data.Auto[1] && PlayData.Data.IsPlay2P)
            {
                DrawString(80, 40, "2P AUTO", 0xffffff);
            }
            DrawString(0, 60, $"Gauge:{(EGauge)PlayData.Data.GaugeType[0]}" + (PlayData.Data.IsPlay2P ? $"/{(EGauge)PlayData.Data.GaugeType[1]}" : ""), 0xffffff);
            DrawString(0, 80, $"GAS:{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[0]}" + (PlayData.Data.IsPlay2P ? $"/{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[1]}" : ""), 0xffffff);
            DrawString(0, 100, $"GASmin:{(EGauge)PlayData.Data.GaugeAutoShiftMin[0]}" + (PlayData.Data.IsPlay2P ? $"/{(EGauge)PlayData.Data.GaugeAutoShiftMin[1]}" : ""), 0xffffff);
            DrawString(0, 120, $"Hazard:{PlayData.Data.Hazard[0]}" + (PlayData.Data.IsPlay2P ? $"/{PlayData.Data.Hazard[1]}" : ""), 0xffffff);

            DrawString(0, 140, "PRESS ENTER", 0xffffff);
            #endif
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(KEY_INPUT_RETURN))
            {
                if (Key.IsPushing(KEY_INPUT_LSHIFT))
                {
                    Replay[0] = true;
                }
                else
                {
                    Replay[0] = false;
                }
                Program.SceneChange(new Game.Game());
            }
            if (Key.IsPushed(KEY_INPUT_ESCAPE))
            {
                Program.SceneChange(new Title.Title());
            }
            if (Key.IsPushing(KEY_INPUT_LSHIFT) && Key.IsPushing(KEY_INPUT_RSHIFT) && Key.IsPushing(KEY_INPUT_DELETE))
            {
                Program.SceneChange(new Game.Game());
            }

            if (Key.IsPushed(KEY_INPUT_F1))
            {
                Program.SceneChange(new Config.Config());
            }
            if (Key.IsPushed(KEY_INPUT_F3))
            {
                PlayData.Data.Auto[0] = !PlayData.Data.Auto[0];
            }
            if (Key.IsPushed(KEY_INPUT_F4) && PlayData.Data.IsPlay2P)
            {
                PlayData.Data.Auto[1] = !PlayData.Data.Auto[1];
            }
            if (Key.IsPushed(KEY_INPUT_F5))
            {
                if (Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT) && PlayData.Data.IsPlay2P)
                {
                    GaugeChange(1);
                }
                else
                {
                    GaugeChange(0);
                }
            }
            if (Key.IsPushed(KEY_INPUT_F6))
            {
                if (Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT) && PlayData.Data.IsPlay2P)
                {
                    GASChange(1);
                }
                else
                {
                    GASChange(0);
                }
            }
            if (Key.IsPushed(KEY_INPUT_F7))
            {
                PlayData.Data.IsPlay2P = !PlayData.Data.IsPlay2P;
            }
            if (Key.IsPushed(KEY_INPUT_F8))
            {
                PlayData.Data.ShowResultScreen = !PlayData.Data.ShowResultScreen;
            }
            if (Key.IsPushed(KEY_INPUT_F9))
            {
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.Data.IsPlay2P)
                {
                    PlayData.Data.Hazard[1]--;
                }
                else
                {
                    PlayData.Data.Hazard[0]--;
                }
            }
            if (Key.IsPushed(KEY_INPUT_F10))
            {
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.Data.IsPlay2P)
                {
                    PlayData.Data.Hazard[1]++;
                }
                else
                {
                    PlayData.Data.Hazard[0]++;
                }
            }

            if (Key.IsPushed(KEY_INPUT_LEFT))
            {
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.Data.IsPlay2P)
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
                if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.Data.IsPlay2P)
                {
                    CourseChange(false, 1);
                }
                else
                {
                    CourseChange(false, 0);
                }
            }

            if (Key.IsPushed(KEY_INPUT_F12))
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.FileName = "水天神術・時雨.tja";
                ofd.InitialDirectory = @"Songs\";
                ofd.Filter = "TJAファイル(*.tja)|*.tja|すべてのファイル(*.*)|*.*";
                ofd.FilterIndex = 1;
                ofd.Title = "再生する譜面の選択";
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    PlayData.Data.PlayFile = ofd.InitialDirectory + ofd.FileName;
                }
                ofd.Dispose();
            }


            base.Update();
        }

        public static void CourseChange(bool isdim, int player)
        {
            if (!isdim)
            {
                #region [ 上げる ]
                if (PlayData.Data.PlayCourse[player] < 4)
                    PlayData.Data.PlayCourse[player]++;
                else
                    PlayData.Data.PlayCourse[player] = 0;
                #endregion
            }
            else
            {
                #region [ 下げる ]
                if (PlayData.Data.PlayCourse[player] > 0)
                    PlayData.Data.PlayCourse[player]--;
                else
                    PlayData.Data.PlayCourse[player] = 4;
                #endregion
            }

        }

        public static void GaugeChange(int player)
        {
            if (PlayData.Data.GaugeType[player] < (int)EGauge.EXHard)
                PlayData.Data.GaugeType[player]++;
            else
                PlayData.Data.GaugeType[player] = (int)EGauge.Normal;
        }
        public static void GASChange(int player)
        {
            if (PlayData.Data.GaugeAutoShift[player] < (int)EGaugeAutoShift.LessBest)
                PlayData.Data.GaugeAutoShift[player]++;
            else
                PlayData.Data.GaugeAutoShift[player] = (int)EGaugeAutoShift.None;
        }

        public static bool[] Replay = new bool[2];
    }
}
