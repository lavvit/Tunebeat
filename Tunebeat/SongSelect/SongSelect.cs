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
            if (File.Exists(PlayData.Data.PlayFile))
            {
                NowTJA = new TJAParse.TJAParse(PlayData.Data.PlayFile, PlayData.Data.PlaySpeed, 0, false, 0);
            }
            FontFamily family = new FontFamily("Arial");
            TitleFont = new FontRender(family, 56, 2, FontStyle.Regular);
            SubTitleFont = new FontRender(family, 32, 2, FontStyle.Regular);
            Title = new Texture();
            SubTitle = new Texture();
            Alart = new Counter(0, 1000, 1000, false);
            base.Enable();
        }

        public override void Disable()
        {
            NowTJA = null;
            TitleFont = null;
            SubTitleFont = null;
            Title = null;
            SubTitle = null;
            Alart.Reset();
            base.Disable();
        }
        public override void Draw()
        {
            DrawBox(0, 0, 1919, 1079, GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]), TRUE);
            TextureLoad.SongSelect_Background.Draw(0, 0);

            TextureLoad.SongSelect_Bar.Draw(1180, -90 + 60 * 10);//表示するだけ
            for (int i = 0; i < 10; i++)
            {
                TextureLoad.SongSelect_Bar.Draw(1212, -90 + 60 * i);//表示するだけ
            }
            for (int i = 11; i < 21; i++)
            {
                TextureLoad.SongSelect_Bar.Draw(1212, -90 + 60 * i);//表示するだけ
            }

            int[] difXY = new int[2] { 72, 500 - 60 };
            TextureLoad.SongSelect_Difficulty_Base.Draw(difXY[0], difXY[1]);
            if (PlayData.Data.IsPlay2P)
            {
                TextureLoad.SongSelect_Difficulty_Base.Draw(difXY[0], difXY[1] + 163, new Rectangle(0, 132, 814, 31));
            }
            DrawString(80, 200, $"{PlayData.Data.PlayFile}", 0xffffff);
            if (NowTJA != null)
            {
                if (!string.IsNullOrEmpty(NowTJA.Header.GENRE))
                {
                    DrawString(80, 280, NowTJA.Header.GENRE, 0xffffff);
                }
                if (!string.IsNullOrEmpty(NowTJA.Header.TITLE))
                {
                    DrawString(80, 320, NowTJA.Header.TITLE, 0xffffff);
                    //Title = TitleFont.GetTexture(NowTJA.Header.TITLE, 900, true);
                    //Title.Draw(80, 200);
                }
                if (!string.IsNullOrEmpty(NowTJA.Header.SUBTITLE))
                {
                    DrawString(80, 360, NowTJA.Header.SUBTITLE, 0xffffff);
                    //SubTitle = TitleFont.GetTexture(NowTJA.Header.SUBTITLE, 900, true);
                    //SubTitle.Draw(80, 200);
                }
                DrawString(800, 360, $"BPM:{NowTJA.Header.BPM}", 0xffffff);
                for (int i = 0; i < 5; i++)
                {
                    if (i == PlayData.Data.PlayCourse[0] || (PlayData.Data.IsPlay2P && i == PlayData.Data.PlayCourse[1]))
                    {
                        switch (i)
                        {
                            case (int)ECourse.Easy:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0xff5f3f);
                                break;
                            case (int)ECourse.Normal:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x9fff3f);
                                break;
                            case (int)ECourse.Hard:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x3fbfff);
                                break;
                            case (int)ECourse.Oni:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0xff3f9f);
                                break;
                            case (int)ECourse.Edit:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x9f3fff);
                                break;
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case (int)ECourse.Easy:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x7f2f1f);
                                break;
                            case (int)ECourse.Normal:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x4f7f1f);
                                break;
                            case (int)ECourse.Hard:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x1f5f7f);
                                break;
                            case (int)ECourse.Oni:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x7f1f4f);
                                break;
                            case (int)ECourse.Edit:
                                TextureLoad.SongSelect_Difficulty.Color = Color.FromArgb(0x4f1f7f);
                                break;
                        }
                    }
                    if (NowTJA.Courses[i].ListChip.Count > 0)
                    {
                        TextureLoad.SongSelect_Difficulty.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 8);
                        TextureLoad.SongSelect_Difficulty_Course.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 100);
                        Score.DrawNumber(difXY[0] + 94 + 156 * i - 12 * Score.Digit(NowTJA.Courses[i].LEVEL), difXY[1] + 42, $"{NowTJA.Courses[i].LEVEL}", 0);
                    }
                    if (i == PlayData.Data.PlayCourse[0] || (PlayData.Data.IsPlay2P && i == PlayData.Data.PlayCourse[1]))
                    {
                        TextureLoad.SongSelect_Difficulty_Cursor.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 8);
                    }
                }
                if (NowTJA.Courses[PlayData.Data.PlayCourse[0]].LEVEL > 0)
                {
                    int lev = NowTJA.Courses[PlayData.Data.PlayCourse[0]].LEVEL < 12 ? NowTJA.Courses[PlayData.Data.PlayCourse[0]].LEVEL : 12;
                    for (int i = 0; i < lev; i++)
                    {
                        switch (PlayData.Data.PlayCourse[0])
                        {
                            case (int)ECourse.Easy:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0xff5f3f);
                                break;
                            case (int)ECourse.Normal:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x9fff3f);
                                break;
                            case (int)ECourse.Hard:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x3fbfff);
                                break;
                            case (int)ECourse.Oni:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0xff3f9f);
                                break;
                            case (int)ECourse.Edit:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x9f3fff);
                                break;
                        }
                        TextureLoad.SongSelect_Difficulty_Level.Draw(difXY[0] + 12 + 66 * i, difXY[1] + 136);
                        if (NowTJA.Courses[PlayData.Data.PlayCourse[0]].LEVEL > 12)
                        {

                            for (int j = 0; j < NowTJA.Courses[PlayData.Data.PlayCourse[0]].LEVEL - 12; j++)
                            {
                                switch (PlayData.Data.PlayCourse[0])
                                {
                                    case (int)ECourse.Easy:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x7f2f1f);
                                        break;
                                    case (int)ECourse.Normal:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x4f7f1f);
                                        break;
                                    case (int)ECourse.Hard:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x1f5f7f);
                                        break;
                                    case (int)ECourse.Oni:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x7f1f4f);
                                        break;
                                    case (int)ECourse.Edit:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x4f1f7f);
                                        break;
                                }
                                TextureLoad.SongSelect_Difficulty_Level.Draw(difXY[0] + 12 + 66 * j, difXY[1] + 136);
                            }
                        }
                    }
                }
                if (PlayData.Data.IsPlay2P && NowTJA.Courses[PlayData.Data.PlayCourse[1]].LEVEL > 0)
                {
                    int lev = NowTJA.Courses[PlayData.Data.PlayCourse[1]].LEVEL < 12 ? NowTJA.Courses[PlayData.Data.PlayCourse[1]].LEVEL : 12;
                    for (int i = 0; i < lev; i++)
                    {
                        switch (PlayData.Data.PlayCourse[1])
                        {
                            case (int)ECourse.Easy:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0xff5f3f);
                                break;
                            case (int)ECourse.Normal:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x9fff3f);
                                break;
                            case (int)ECourse.Hard:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x3fbfff);
                                break;
                            case (int)ECourse.Oni:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0xff3f9f);
                                break;
                            case (int)ECourse.Edit:
                                TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x9f3fff);
                                break;
                        }
                        TextureLoad.SongSelect_Difficulty_Level.Draw(difXY[0] + 12 + 66 * i, difXY[1] + 136 + 31);
                        if (NowTJA.Courses[PlayData.Data.PlayCourse[1]].LEVEL > 12)
                        {

                            for (int j = 0; j < NowTJA.Courses[PlayData.Data.PlayCourse[1]].LEVEL - 12; j++)
                            {
                                switch (PlayData.Data.PlayCourse[1])
                                {
                                    case (int)ECourse.Easy:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x7f2f1f);
                                        break;
                                    case (int)ECourse.Normal:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x4f7f1f);
                                        break;
                                    case (int)ECourse.Hard:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x1f5f7f);
                                        break;
                                    case (int)ECourse.Oni:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x7f1f4f);
                                        break;
                                    case (int)ECourse.Edit:
                                        TextureLoad.SongSelect_Difficulty_Level.Color = Color.FromArgb(0x4f1f7f);
                                        break;
                                }
                                TextureLoad.SongSelect_Difficulty_Level.Draw(difXY[0] + 12 + 66 * j, difXY[1] + 136 + 31);
                            }
                        }
                    }
                }

                DrawString(480, 640, "REPLAYDATA MENU (Beta mode)", 0xffffff);
                if (!string.IsNullOrEmpty(PlayData.Data.BestScore) && File.Exists($@"{Path.GetDirectoryName(NowTJA.TJAPath)}\{Path.GetFileNameWithoutExtension(NowTJA.TJAPath)}.{(ECourse)PlayData.Data.PlayCourse[0]}.{PlayData.Data.BestScore}.replaydata"))
                {
                    DrawString(480, 680, $"BestScore:{PlayData.Data.BestScore}", 0xffffff);
                }
                else
                {
                    DrawString(480, 680, "BestScore:None", 0xffffff);
                }
                if (!string.IsNullOrEmpty(PlayData.Data.RivalScore) && File.Exists($@"{Path.GetDirectoryName(NowTJA.TJAPath)}\{Path.GetFileNameWithoutExtension(NowTJA.TJAPath)}.{(ECourse)PlayData.Data.PlayCourse[0]}.{PlayData.Data.RivalScore}.replaydata"))
                {
                    DrawString(480, 700, $"RivalScore:{PlayData.Data.RivalScore}", 0xffffff);
                }
                else
                {
                    DrawString(480, 700, "RivalScore:None", 0xffffff);
                }
                if (!string.IsNullOrEmpty(PlayData.Data.Replay[0]) && File.Exists($@"{Path.GetDirectoryName(NowTJA.TJAPath)}\{Path.GetFileNameWithoutExtension(NowTJA.TJAPath)}.{(ECourse)PlayData.Data.PlayCourse[0]}.{PlayData.Data.Replay[0]}.replaydata"))
                {
                    DrawString(480, 720, $"ReplayScore1P:{PlayData.Data.Replay[0]}", 0xffffff);
                    DrawString(800, 720, "Press LSHIFT & ENTER to playback", 0xffffff);
                }
                else
                {
                    DrawString(480, 720, "ReplayScore1P:None", 0xffffff);
                }
                if (PlayData.Data.IsPlay2P)
                {
                    if (!string.IsNullOrEmpty(PlayData.Data.Replay[1]) && File.Exists($@"{Path.GetDirectoryName(NowTJA.TJAPath)}\{Path.GetFileNameWithoutExtension(NowTJA.TJAPath)}.{(ECourse)PlayData.Data.PlayCourse[1]}.{PlayData.Data.Replay[1]}.replaydata"))
                    {
                        DrawString(480, 740, $"ReplayScore2P:{PlayData.Data.Replay[1]}", 0xffffff);
                        DrawString(800, 740, "Press RSHIFT & ENTER to playback", 0xffffff);
                    }
                    else
                    {
                        DrawString(480, 740, "ReplayScore2P:None", 0xffffff);
                    }
                }
            }
            TextureLoad.SongSelect_Difficulty_TJA.Draw(difXY[0], difXY[1]);

            DrawString(80, 640, "OPTION MENU (Beta mode)", 0xffffff);
            DrawString(80, 680, "1P", 0xffffff);
            if (PlayData.Data.Auto[0])
            {
                DrawString(110, 680, "AUTO", 0xffffff);
            }
            DrawString(80, 700, $"Gauge:{(EGauge)PlayData.Data.GaugeType[0]}", 0xffffff);
            DrawString(80, 720, $"GAS:{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[0]}", 0xffffff);
            DrawString(80, 740, $"GASmin:{(EGauge)PlayData.Data.GaugeAutoShiftMin[0]}", 0xffffff);
            DrawString(80, 760, $"Hazard:{PlayData.Data.Hazard[0]}", 0xffffff);
            if (PlayData.Data.IsPlay2P)
            {
                DrawString(240, 680, "2P", 0xffffff);
                if (PlayData.Data.Auto[1])
                {
                    DrawString(270, 680, "AUTO", 0xffffff);
                }
                DrawString(240, 700, $"Gauge:{(EGauge)PlayData.Data.GaugeType[1]}", 0xffffff);
                DrawString(240, 720, $"GAS:{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[1]}", 0xffffff);
                DrawString(240, 740, $"GASmin:{(EGauge)PlayData.Data.GaugeAutoShiftMin[1]}", 0xffffff);
                DrawString(240, 760, $"Hazard:{PlayData.Data.Hazard[1]}", 0xffffff);
            }


            if (Alart.State != 0)
            {
                DrawBox(0, 1040, 410, 1080, 0x000000, TRUE);
                switch (AlartType)
                {
                    case 0:
                        DrawString(20, 1052, $"TJAが見つかりません!パスを確認してください。", 0xffffff);
                        break;
                    case 1:
                        DrawString(20, 1052, $"譜面がありません!難易度を確認してください。", 0xffffff);
                        break;
                }
            }

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
            Alart.Tick();
            if (Alart.Value == Alart.End) Alart.Stop();

            if (Key.IsPushed(KEY_INPUT_RETURN))
            {
                if (File.Exists(PlayData.Data.PlayFile))
                {
                    if (NowTJA.Courses[PlayData.Data.PlayCourse[0]].ListChip.Count > 0)
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
                    else
                    {
                        AlartType = 1;
                        Alart.Reset();
                        Alart.Start();
                    }
                }
                else
                {
                    AlartType = 0;
                    Alart.Reset();
                    Alart.Start();
                }
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
            if (Key.IsPushed(KEY_INPUT_F2))
            {
                PlayData.Init();
                NowTJA = new TJAParse.TJAParse(PlayData.Data.PlayFile, PlayData.Data.PlaySpeed, 0, false, 0);
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
        public static FontRender TitleFont, SubTitleFont;
        public static Texture Title, SubTitle;
        public static TJAParse.TJAParse NowTJA;
        public static Counter Alart;
        public static int AlartType;
    }
}
