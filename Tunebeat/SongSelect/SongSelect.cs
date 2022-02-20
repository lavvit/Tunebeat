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
using Tunebeat.Game;

namespace Tunebeat.SongSelect
{
    public class SongSelect : Scene
    {
        public override void Enable()
        {
            SongLoad.Init();
            if (SongLoad.SongData.Count > 0) NowTJA = SongLoad.SongData[NowSongNumber];
            Title = new Texture();
            SubTitle = new Texture();
            Genre = new Texture();
            BPM = new Texture();
            if (PlayData.Data.FontRendering) FontLoad();
            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i] = new Counter(0, 199, 1000, false);
                PushingTimer[i] = new Counter(0, 49, 1000, false);
            }
            SoundLoad.Don[0].Pan = 0;
            SoundLoad.Don[0].Volume = PlayData.Data.SE / 100.0;
            if (PlayData.Data.PlaySpeed != 1.0 && PlayData.Data.ChangeSESpeed) SoundLoad.Don[0].PlaySpeed = PlayData.Data.PlaySpeed;
            SoundLoad.Ka[0].Pan = 0;
            SoundLoad.Ka[0].Volume = PlayData.Data.SE / 100.0;
            if (PlayData.Data.PlaySpeed != 1.0 && PlayData.Data.ChangeSESpeed) SoundLoad.Ka[0].PlaySpeed = PlayData.Data.PlaySpeed;
            SetScore();
            if (PlayData.Data.PreviewSong)
            {
                if (NowTJA.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(NowTJA.Path)}/{new TJAParse.TJAParse(NowTJA.Path).Header.WAVE}");
            }
            base.Enable();
        }

        public override void Disable()
        {
            NowTJA = null;
            Title = null;
            SubTitle = null;
            Genre = null;
            BPM = null;
            Preview = null;
            SongLoad.Dispose();

            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i].Reset();
                PushingTimer[i].Reset();
            }
            base.Disable();
        }
        public override void Draw()
        {
            DrawBox(0, 0, 1919, 1079, GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]), TRUE);
            TextureLoad.SongSelect_Background.Draw(0, 0);

            if (NowTJA != null)
            {
                switch (NowTJA.Type)
                {
                    case EType.Score:
                        TextureLoad.SongSelect_Bar_Color.Color = NowTJA.BackColor;
                        TextureLoad.SongSelect_Bar_Color.Opacity = 0.75;
                        TextureLoad.SongSelect_Bar_Color.Draw(1180, -90 + 60 * 10);
                        TextureLoad.SongSelect_Bar.Draw(1180, -90 + 60 * 10);
                        DrawString(1300, -90 + 60 * 10 + 22, NowTJA.Header.TITLE, (uint)ColorTranslator.ToWin32(NowTJA.FontColor));
                        if (NowTJA.Course[EnableCourse(NowTJA.Course, 0)].IsEnable)
                        {
                            if (NowTJA.Score != null) TextureLoad.SongSelect_Clear.Draw(1180 + 2, -90 + 2 + 60 * 10, new Rectangle(29 * NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].ClearLamp, 0, 29, 56));
                            DrawNumber(1242 - 14 * Score.Digit(NowTJA.Course[EnableCourse(NowTJA.Course, 0)].LEVEL), -90 + 60 * 10 + 14, $"{NowTJA.Course[EnableCourse(NowTJA.Course, 0)].LEVEL}", EnableCourse(NowTJA.Course, 0) + 1);
                        }

                        break;
                    default:
                        TextureLoad.SongSelect_Bar_Folder_Color.Color = NowTJA.BackColor;
                        TextureLoad.SongSelect_Bar_Folder_Color.Draw(1180, -90 + 60 * 10);
                        TextureLoad.SongSelect_Bar_Folder.Draw(1180, -90 + 60 * 10);
                        DrawString(1300 - 60, -90 + 60 * 10 + 22, NowTJA.Title, (uint)ColorTranslator.ToWin32(NowTJA.FontColor));
                        break;
                }

                var prev = NowTJA;
                for (int i = 9; i >= 0; i--)
                {
                    prev = prev.Prev;
                    switch (prev.Type)
                    {
                        case EType.Score:
                            TextureLoad.SongSelect_Bar_Color.Color = prev.BackColor;
                            TextureLoad.SongSelect_Bar_Color.Opacity = 0.75;
                            TextureLoad.SongSelect_Bar_Color.Draw(1212, -90 + 60 * i);
                            TextureLoad.SongSelect_Bar.Draw(1212, -90 + 60 * i);
                            DrawString(1332, -90 + 60 * i + 22, prev.Header.TITLE, (uint)ColorTranslator.ToWin32(prev.FontColor));
                            if (prev.Course[EnableCourse(prev.Course, 0)].IsEnable)
                            {
                                if (prev.Score != null) TextureLoad.SongSelect_Clear.Draw(1212 + 2, -90 + 2 + 60 * i, new Rectangle(29 * prev.Score.Score[EnableCourse(prev.Course, 0)].ClearLamp, 0, 29, 56));
                                DrawNumber(1242 + 32 - 14 * Score.Digit(prev.Course[EnableCourse(prev.Course, 0)].LEVEL), -90 + 60 * i + 14, $"{prev.Course[EnableCourse(prev.Course, 0)].LEVEL}", EnableCourse(prev.Course, 0) + 1);
                            }

                            break;
                        default:
                            TextureLoad.SongSelect_Bar_Folder_Color.Color = prev.BackColor;
                            TextureLoad.SongSelect_Bar_Folder_Color.Draw(1212, -90 + 60 * i);
                            TextureLoad.SongSelect_Bar_Folder.Draw(1212, -90 + 60 * i);
                            DrawString(1332 - 60, -90 + 60 * i + 22, prev.Title, (uint)ColorTranslator.ToWin32(prev.FontColor));
                            break;
                    }

                }
                var next = NowTJA;
                for (int i = 11; i < 21; i++)
                {
                    next = next.Next;
                    switch (next.Type)
                    {
                        case EType.Score:
                            TextureLoad.SongSelect_Bar_Color.Color = next.BackColor;
                            TextureLoad.SongSelect_Bar_Color.Opacity = 0.75;
                            TextureLoad.SongSelect_Bar_Color.Draw(1212, -90 + 60 * i);
                            TextureLoad.SongSelect_Bar.Draw(1212, -90 + 60 * i);
                            DrawString(1332, -90 + 60 * i + 22, next.Header.TITLE, (uint)ColorTranslator.ToWin32(next.FontColor));
                            if (next.Course[EnableCourse(next.Course, 0)].IsEnable)
                            {
                                if (next.Score != null) TextureLoad.SongSelect_Clear.Draw(1212 + 2, -90 + 2 + 60 * i, new Rectangle(29 * next.Score.Score[EnableCourse(next.Course, 0)].ClearLamp, 0, 29, 56));
                                DrawNumber(1242 + 32 - 14 * Score.Digit(next.Course[EnableCourse(next.Course, 0)].LEVEL), -90 + 60 * i + 14, $"{next.Course[EnableCourse(next.Course, 0)].LEVEL}", EnableCourse(next.Course, 0) + 1);
                            }

                            break;
                        default:
                            TextureLoad.SongSelect_Bar_Folder_Color.Color = next.BackColor;
                            TextureLoad.SongSelect_Bar_Folder_Color.Draw(1212, -90 + 60 * i);
                            TextureLoad.SongSelect_Bar_Folder.Draw(1212, -90 + 60 * i);
                            DrawString(1332 - 60, -90 + 60 * i + 22, next.Title, (uint)ColorTranslator.ToWin32(next.FontColor));
                            break;
                    }
                }
                double len = (Mouse.Y - 540.0) / TextureLoad.SongSelect_Bar.ActualSize.Height;
                int cur = (int)Math.Round(len, 0, MidpointRounding.AwayFromZero);
                if (Mouse.X >= 1212 || (cur == 0 && Mouse.X >= 1180))
                {
                    TextureLoad.SongSelect_Bar_Cursor.Draw(cur == 0 ? 1180 : 1212, -90 + 60 * (10 + cur));
                }

                int[] difXY = new int[2] { 72, 500 - 60 };
                TextureLoad.SongSelect_Difficulty_Base.Draw(difXY[0], difXY[1]);
                if (PlayData.Data.IsPlay2P)
                {
                    TextureLoad.SongSelect_Difficulty_Base.Draw(difXY[0], difXY[1] + 163, new Rectangle(0, 132, 814, 31));
                }

                switch (NowTJA.Type)
                {
                    case EType.Score:
                        if (PlayData.Data.FontRendering)
                        {
                            Genre.ScaleX = Genre.TextureSize.Width > 814.0 ? (814.0 / Genre.TextureSize.Width) : 1.0;
                            Title.ScaleX = Title.TextureSize.Width > 814.0 ? (814.0 / Title.TextureSize.Width) : 1.0;
                            SubTitle.ScaleX = SubTitle.TextureSize.Width > 814.0 ? (814.0 / SubTitle.TextureSize.Width) : 1.0;
                            Genre.Draw(difXY[0] + 407 - (Genre.ActualSize.Width / 2), 190);
                            Title.Draw(difXY[0] + 407 - (Title.ActualSize.Width / 2), 240);
                            SubTitle.Draw(difXY[0] + 407 - (SubTitle.ActualSize.Width / 2), 340);
                            BPM.Draw(difXY[0] + 407 - (BPM.ActualSize.Width / 2), 390);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(NowTJA.Header.GENRE)) DrawString(difXY[0] + 407 - (GetDrawStringWidth(NowTJA.Header.GENRE, NowTJA.Header.GENRE.Length) / 2), 190, NowTJA.Header.GENRE, 0xffffff);
                            if (!string.IsNullOrEmpty(NowTJA.Header.TITLE)) DrawString(difXY[0] + 407 - (GetDrawStringWidth(NowTJA.Header.TITLE, NowTJA.Header.TITLE.Length) / 2), 240, NowTJA.Header.TITLE, 0xffffff);
                            if (!string.IsNullOrEmpty(NowTJA.Header.SUBTITLE)) DrawString(difXY[0] + 407 - (GetDrawStringWidth(NowTJA.Header.SUBTITLE, NowTJA.Header.SUBTITLE.Length) / 2), 340, NowTJA.Header.SUBTITLE, 0xffffff);
                            string bpm = $"{Math.Round(NowTJA.Header.BPM, 0, MidpointRounding.AwayFromZero)} BPM";
                            DrawString(difXY[0] + 407 - (GetDrawStringWidth(bpm, bpm.Length) / 2), 390, bpm, 0xffffff);
                        }
                        bool select = Mouse.X >= difXY[0] + 22 && Mouse.X < difXY[0] + 795 && Mouse.Y >= difXY[1] + 8 && Mouse.Y < difXY[1] + 126;
                        double widgh = (Mouse.X - (difXY[0] + 22 + 74.5)) / 156;
                        int wcursor = (int)Math.Round(widgh, 0, MidpointRounding.AwayFromZero);
                        for (int i = 0; i < 5; i++)
                        {
                            if (i == (select && !Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 0)) || (PlayData.Data.IsPlay2P && i == (select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1))) || PlayData.Data.PreviewType == 3)
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
                            if (NowTJA.Course[i].IsEnable)
                            {
                                TextureLoad.SongSelect_Difficulty.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 8);
                                TextureLoad.SongSelect_Difficulty_Course.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 100);
                                Score.DrawNumber(difXY[0] + 94 + 156 * i - 12 * Score.Digit(NowTJA.Course[i].LEVEL), difXY[1] + 42, $"{NowTJA.Course[i].LEVEL}", 0);
                            }
                        }
                        if (NowTJA.Course[select && !Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 0)].LEVEL > 0)
                        {
                            int lev = NowTJA.Course[select && !Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 0)].LEVEL < 12 ? NowTJA.Course[select && !Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 0)].LEVEL : 12;
                            for (int i = 0; i < lev; i++)
                            {
                                switch (select && !Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 0))
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
                                if (NowTJA.Course[EnableCourse(NowTJA.Course, 0)].LEVEL > 12)
                                {

                                    for (int j = 0; j < NowTJA.Course[select && !Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 0)].LEVEL - 12; j++)
                                    {
                                        switch (select && !Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 0))
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
                        if (PlayData.Data.IsPlay2P && NowTJA.Course[select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1)].LEVEL > 0)
                        {
                            int lev = NowTJA.Course[select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1)].LEVEL < 12 ? NowTJA.Course[select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1)].LEVEL : 12;
                            for (int i = 0; i < lev; i++)
                            {
                                switch (select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1))
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
                                if (NowTJA.Course[select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1)].LEVEL > 12)
                                {

                                    for (int j = 0; j < NowTJA.Course[select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1)].LEVEL - 12; j++)
                                    {
                                        switch (select && Key.IsPushing(KEY_INPUT_LSHIFT) ? wcursor : EnableCourse(NowTJA.Course, 1))
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

                        DrawString(300, 640, "BESTSCORE (Beta mode)", 0xffffff);
                        if (NowTJA.Score != null)
                        {
                            DrawString(300, 660, $"Clear:{(EClear)NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].ClearLamp}", 0xffffff);
                            DrawString(300, 680, $"GaugeType:{(EGauge)NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].GaugeType}", 0xffffff);
                            DrawString(300, 700, $"Gauge:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Gauge}", 0xffffff);
                            DrawString(300, 720, $"Score:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Score}", 0xffffff);
                            DrawString(300, 740, $"Perfect:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Perfect}", 0xffffff);
                            DrawString(300, 760, $"Great:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Great}", 0xffffff);
                            DrawString(300, 780, $"Good:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Good}", 0xffffff);
                            DrawString(300, 800, $"Bad:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Bad}", 0xffffff);
                            DrawString(300, 820, $"Poor:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Poor}", 0xffffff);
                            DrawString(300, 840, $"Roll:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Roll}", 0xffffff);
                            DrawString(300, 860, $"MaxCombo:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].MaxCombo}", 0xffffff);
                            DrawString(300, 880, $"Rate:{Math.Round(NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Score > 0 ? (1.01 * NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Perfect + 1.0 * NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Great + 0.5 * NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].Good) / NowTJA.Course[EnableCourse(NowTJA.Course, 0)].TotalNotes * 100.0 : 0, 4, MidpointRounding.AwayFromZero)}", 0xffffff);
                        }

                        DrawString(520, 640, "REPLAYDATA MENU (Beta mode)", 0xffffff);
                        if (NowTJA.Score != null && !string.IsNullOrEmpty(NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].BestScore)
                            && File.Exists($@"{Path.GetDirectoryName(NowTJA.Path)}\{Path.GetFileNameWithoutExtension(NowTJA.Path)}.{(ECourse)EnableCourse(NowTJA.Course, 0)}.{PlayData.Data.PlayerName}.{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].BestScore}.tbr"))
                        {
                            DrawString(520, 660, $"BestScore:{NowTJA.Score.Score[EnableCourse(NowTJA.Course, 0)].BestScore}", 0xffffff);
                        }
                        else
                        {
                            DrawString(520, 660, "BestScore:None", 0xffffff);
                        }
                        if (!string.IsNullOrEmpty(RivalScore) && File.Exists($@"{Path.GetDirectoryName(NowTJA.Path)}\{Path.GetFileNameWithoutExtension(NowTJA.Path)}.{(ECourse)EnableCourse(NowTJA.Course, 0)}.{RivalScore}.tbr"))
                        {
                            DrawString(520, 680, $"RivalScore:{RivalScore}", 0xffffff);
                        }
                        else
                        {
                            DrawString(520, 680, "RivalScore:None", 0xffffff);
                        }
                        if (!string.IsNullOrEmpty(ReplayScore[0]) && File.Exists($@"{ Path.GetDirectoryName(NowTJA.Path)}\{ Path.GetFileNameWithoutExtension(NowTJA.Path)}.{ (ECourse)EnableCourse(NowTJA.Course, 0)}.{ReplayScore[0]}.tbr"))
                        {
                            DrawString(520, 700, $"ReplayScore1P:{ReplayScore[0]}", 0xffffff);
                            DrawString(540 + GetDrawStringWidth($"ReplayScore1P:{ReplayScore[0]}", $"ReplayScore1P:{ReplayScore[0]}".Length), 700, "Press LSHIFT & ENTER to play", 0xffffff);
                        }
                        else
                        {
                            DrawString(520, 700, "ReplayScore1P:None", 0xffffff);
                        }
                        if (PlayData.Data.IsPlay2P)
                        {
                            if (!string.IsNullOrEmpty(ReplayScore[1]) && File.Exists($@"{Path.GetDirectoryName(NowTJA.Path)}\{Path.GetFileNameWithoutExtension(NowTJA.Path)}.{(ECourse)EnableCourse(NowTJA.Course, 0)}.{ReplayScore[1]}.tbr"))
                            {
                                DrawString(520, 720, $"ReplayScore2P:{ReplayScore[1]}", 0xffffff);
                                DrawString(540 + GetDrawStringWidth($"ReplayScore2P:{ReplayScore[1]}", $"ReplayScore2P:{ReplayScore[1]}".Length), 720, "Press RSHIFT & ENTER to play", 0xffffff);
                            }
                            else
                            {
                                DrawString(520, 720, "ReplayScore2P:None", 0xffffff);
                            }
                        }
                        if (NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)] != null && NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count > 0)
                        {
                            DrawString(520, 740, "List:", 0xffffff);
                            for (int i = 0; i < NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count; i++)
                            {
                                DrawString(520, 760 + 20 * i, NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)][i], 0xffffff);
                            }
                        }
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < 5; i++)
                {
                    if ((i == PlayData.Data.PlayCourse[0] || (PlayData.Data.IsPlay2P && i == PlayData.Data.PlayCourse[1])) && PlayData.Data.PreviewType < 3)
                    {
                        TextureLoad.SongSelect_Difficulty_Cursor.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 8);
                    }
                }
                TextureLoad.SongSelect_Difficulty_TJA.Draw(difXY[0], difXY[1]);

                DrawString(80, 640, "OPTION MENU (Beta mode)", 0xffffff);
                DrawString(80, 660, "1P", 0xffffff);
                if (PlayData.Data.Auto[0]) DrawString(110, 660, "AUTO", 0xffffff);
                DrawString(80, 680, $"Gauge:{(EGauge)PlayData.Data.GaugeType[0]}", 0xffffff);
                DrawString(80, 700, $"GAS:{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[0]}", 0xffffff);
                DrawString(80, 720, $"GASmin:{(EGauge)PlayData.Data.GaugeAutoShiftMin[0]}", 0xffffff);
                DrawString(80, 740, $"Hazard:{PlayData.Data.Hazard[0]}", 0xffffff);
                if (PlayData.Data.Random[0]) DrawString(80, 760, $"{PlayData.Data.RandomRate}% Random", 0xffffff);

                if (PlayData.Data.IsPlay2P)
                {
                    DrawString(80, 800, "2P", 0xffffff);
                    if (PlayData.Data.Auto[1])
                    {
                        DrawString(110, 800, "AUTO", 0xffffff);
                    }
                    DrawString(80, 820, $"Gauge:{(EGauge)PlayData.Data.GaugeType[1]}", 0xffffff);
                    DrawString(80, 840, $"GAS:{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[1]}", 0xffffff);
                    DrawString(80, 860, $"GASmin:{(EGauge)PlayData.Data.GaugeAutoShiftMin[1]}", 0xffffff);
                    DrawString(80, 880, $"Hazard:{PlayData.Data.Hazard[1]}", 0xffffff);
                    if (PlayData.Data.Random[1]) DrawString(80, 900, $"{PlayData.Data.RandomRate}% Random", 0xffffff);
                }
            }
            else
            {
                DrawBox(0, 308, 530, 348, 0x000000, TRUE);
                DrawString(80, 320, "TJAがありません。パスを確認し、ロードしてください。", 0xffffff);
            }

            if (!string.IsNullOrEmpty(PlayData.Data.PlayerName)) DrawString(80, 1000, $"Player:{PlayData.Data.PlayerName}", 0xffffff);

            if (Input.IsEnable)
            {
                DrawBox(0, 1040, GetDrawStringWidth(Input.Text, Input.Text.Length) + 40, 1080, 0x000000, TRUE);
                DrawBox(20 + 9 * Input.Selection.Start, 1040, 20 + 9 * Input.Selection.End, 1080, 0x0000ff, TRUE);
                DrawString(20, 1052, Input.Text, 0xffffff);
                DrawString(16 + GetDrawStringWidth(Input.Text, Input.Position), 1052, "|", 0xffff00);
            }

            #if DEBUG
            if (NowTJA != null)DrawString(0, 0, $"File:{NowTJA.Path}", 0xffffff);
            DrawString(200, 20, $"SongCount:{SongLoad.SongData.Count},{SongLoad.SongList.Count}", 0xffffff);
            DrawString(200, 40, $"SongNumber:{NowSongNumber}", 0xffffff);
            DrawString(0, 20, $"Course:{(ECourse)EnableCourse(NowTJA.Course, 0)}" + (PlayData.Data.IsPlay2P ? $"/{(ECourse)EnableCourse(NowTJA.Course, 1)}" : ""), 0xffffff);
            if (PlayData.Data.Auto[0])
            {
                DrawString(0, 40, "1P AUTO", 0xffffff);
            }
            if (PlayData.Data.Auto[1] && PlayData.Data.IsPlay2P)
            {
                DrawString(80, 40, "2P AUTO", 0xffffff);
            }
            double length = (Mouse.Y - 540.0) / TextureLoad.SongSelect_Bar.ActualSize.Height;
            int cursor = (int)Math.Round(length, 0, MidpointRounding.AwayFromZero);
            DrawString(200, 60, $"NowY:{length}", 0xffffff);
            DrawString(200, 80, $"NowYCursor:{cursor}", 0xffffff);
            DrawString(200, 100, $"NowWheel:{Mouse.Wheel}", 0xffffff);
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
            if (Preview != null && Preview.IsEnable && !Preview.IsPlaying)
            {
                Preview.Volume = PlayData.Data.SystemBGM / 100.0;
                Preview.Play();
            }
            if (Preview != null && Preview.IsPlaying && Preview.Time < new TJAParse.TJAParse(NowTJA.Path).Header.DEMOSTART)
            {
                Preview.Time = new TJAParse.TJAParse(NowTJA.Path).Header.DEMOSTART;
            }
            if (Input.IsEnable)
            {
                if (Key.IsPushed(KEY_INPUT_RETURN))
                {
                    Input.End();
                }
            }
            else
            {
                if (KeyInput.ListPushed(PlayData.Data.LEFTKA))
                {
                    PushedTimer[0].Start();
                }
                if (KeyInput.ListLeft(PlayData.Data.LEFTKA))
                {
                    if (PlayData.Data.FontRendering) FontLoad();
                    PushedTimer[0].Stop();
                    PushedTimer[0].Reset();
                    PushingTimer[0].Stop();
                    PushingTimer[0].Reset();

                }
                if (KeyInput.ListPushed(PlayData.Data.RIGHTKA))
                {
                    PushedTimer[1].Start();
                }
                if (KeyInput.ListLeft(PlayData.Data.RIGHTKA))
                {
                    if (PlayData.Data.FontRendering) FontLoad();
                    PushedTimer[1].Stop();
                    PushedTimer[1].Reset();
                    PushingTimer[1].Stop();
                    PushingTimer[1].Reset();
                }
                for (int i = 0; i < 2; i++)
                {
                    if (PushedTimer[i].Value == PushedTimer[i].End)
                    {
                        PushingTimer[i].Start();
                    }
                }

                if ((KeyInput.ListPushed(PlayData.Data.LEFTKA) || (PushingTimer[0].Value == PushingTimer[0].End) || Mouse.Wheel > 0) && NowTJA != null)
                {
                    SoundLoad.Ka[0].Play();
                    NowTJA = NowTJA.Prev;
                    if (NowSongNumber <= 0) NowSongNumber = SongLoad.SongData.Count - 1;
                    else NowSongNumber--;
                    SetScore();
                    if (PlayData.Data.PreviewSong)
                    {
                        if (Preview != null) { Preview.Stop(); Preview = null; }
                        if (NowTJA.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(NowTJA.Path)}/{new TJAParse.TJAParse(NowTJA.Path).Header.WAVE}");
                    }
                    PushingTimer[0].Reset();
                }
                if ((KeyInput.ListPushed(PlayData.Data.RIGHTKA) || (PushingTimer[1].Value == PushingTimer[1].End) || Mouse.Wheel < 0) && NowTJA != null)
                {
                    SoundLoad.Ka[0].Play();
                    NowTJA = NowTJA.Next;
                    if (NowSongNumber >= SongLoad.SongData.Count - 1) NowSongNumber = 0;
                    else NowSongNumber++;
                    SetScore();
                    if (PlayData.Data.PreviewSong)
                    {
                        if (Preview != null) { Preview.Stop(); Preview = null; }
                        if (NowTJA.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(NowTJA.Path)}/{new TJAParse.TJAParse(NowTJA.Path).Header.WAVE}");
                    }
                    PushingTimer[1].Reset();
                }
                if (((KeyInput.ListPushing(PlayData.Data.LEFTKA) && KeyInput.ListPushed(PlayData.Data.RIGHTKA)) || (KeyInput.ListPushed(PlayData.Data.LEFTKA) && KeyInput.ListPushing(PlayData.Data.RIGHTKA))) && SongLoad.FolderFloor > 0)
                {
                    NowTJA = SongLoad.SongData[0];
                    Back();
                }

                if ((Key.IsPushed(KEY_INPUT_RETURN) || KeyInput.ListPushed(PlayData.Data.LEFTDON) || KeyInput.ListPushed(PlayData.Data.RIGHTDON)) && NowTJA != null)
                {
                    SoundLoad.Don[0].Play();
                    Enter();
                }

                int difx = 72, dify = 500 - 60;
                double length = (Mouse.Y - 540.0) / TextureLoad.SongSelect_Bar.ActualSize.Height;
                int cursor = (int)Math.Round(length, 0, MidpointRounding.AwayFromZero);
                double widgh = (Mouse.X - (difx + 22 + 74.5)) / 156;
                int wcursor = (int)Math.Round(widgh, 0, MidpointRounding.AwayFromZero);
                if (Mouse.IsPushed(MouseButton.Left) && NowTJA != null)
                {
                    if (Mouse.X >= 1212 || (cursor == 0 && Mouse.X >= 1180))
                    {
                        if (cursor < 0)
                        {
                            SoundLoad.Ka[0].Play();
                            for (int i = 0; i < -cursor; i++)
                            {
                                if (NowSongNumber <= 0) NowSongNumber = SongLoad.SongData.Count - 1;
                                else NowSongNumber--;
                            }
                            NowTJA = SongLoad.SongData[NowSongNumber];
                            SetScore();
                            if (PlayData.Data.PreviewSong)
                            {
                                if (Preview != null) { Preview.Stop(); Preview = null; }
                                if (NowTJA.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(NowTJA.Path)}/{new TJAParse.TJAParse(NowTJA.Path).Header.WAVE}");
                            }
                        }
                        else if (cursor > 0)
                        {
                            SoundLoad.Ka[0].Play();
                            for (int i = 0; i < cursor; i++)
                            {
                                if (NowSongNumber >= SongLoad.SongData.Count - 1) NowSongNumber = 0;
                                else NowSongNumber++;
                            }
                            NowTJA = SongLoad.SongData[NowSongNumber];
                            SetScore();
                            if (PlayData.Data.PreviewSong)
                            {
                                if (Preview != null) { Preview.Stop(); Preview = null; }
                                if (NowTJA.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(NowTJA.Path)}/{new TJAParse.TJAParse(NowTJA.Path).Header.WAVE}");
                            }
                        }
                        else
                        {
                            SoundLoad.Don[0].Play();
                            Enter();
                        }
                    }
                    else if (Mouse.X >= difx + 22 && Mouse.X < difx + 795 && Mouse.Y >= dify + 8 && Mouse.Y < dify + 126)
                    {
                        SoundLoad.Ka[0].Play();
                        if (Key.IsPushing(KEY_INPUT_LSHIFT) && PlayData.Data.IsPlay2P)
                        {
                            PlayData.Data.PlayCourse[1] = wcursor;
                        }
                        else
                        {
                            PlayData.Data.PlayCourse[0] = wcursor;
                        }
                        SetScore();
                        string title = NowTJA.Title;
                        SongLoad.Sort(SongLoad.SongData, SongLoad.NowSort);
                        SongLoad.Sort(SongLoad.SongList, SongLoad.NowSort);

                        for (int i = 0; i < SongLoad.SongData.Count; i++)
                        {
                            if (SongLoad.SongData[i].Title == title)
                            {
                                NowSongNumber = i;
                                break;
                            }
                        }
                        NowTJA = SongLoad.SongData[NowSongNumber];
                    }
                    else
                    {
                        if (SongLoad.FolderFloor > 0)
                        {
                            SoundLoad.Ka[0].Play();
                            NowTJA = SongLoad.SongData[0];
                            Back();
                        }
                    }
                }

                if (Key.IsPushed(KEY_INPUT_SPACE))
                {
                    SoundLoad.Ka[0].Play();
                    string title = NowTJA.Title;
                    if (SongLoad.NowSort == ESort.Rate_Low) SongLoad.NowSort = ESort.None;
                    else SongLoad.NowSort++;
                    SongLoad.Sort(SongLoad.SongData, SongLoad.NowSort);
                    SongLoad.Sort(SongLoad.SongList, SongLoad.NowSort);

                    for (int i = 0; i < SongLoad.SongData.Count; i++)
                    {
                        if (SongLoad.SongData[i].Title == title)
                        {
                            NowSongNumber = i;
                            break;
                        }
                    }
                    NowTJA = SongLoad.SongData[NowSongNumber];
                    if (PlayData.Data.FontRendering) FontLoad();
                    string str;
                    if (SongLoad.NowSort != ESort.None)
                    {
                        str = $"譜面を並び替えました! Sort:{SongLoad.NowSort}...";
                    }
                    else
                    {
                        str = "譜面の並び順をデフォルトに戻しました!";
                    }
                    DrawLog.Draw(str);
                }

                if (Key.IsPushed(KEY_INPUT_ESCAPE))
                {
                    SoundLoad.Ka[0].Play();
                    if (SongLoad.FolderFloor > 0)
                    {
                        NowTJA = SongLoad.SongData[0];
                        Back();
                    }
                    else
                    {
                        if (Preview != null) { Preview.Stop(); Preview = null; }
                        Program.SceneChange(new Title.Title());
                    }
                }

                if (Key.IsPushed(KEY_INPUT_1))
                {
                    if (NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)] != null && NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count > 0)
                    {
                        SoundLoad.Ka[0].Play();
                        if (NowRivalScore >= NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count - 1) NowRivalScore = 0;
                        else NowRivalScore++;
                        RivalScore = NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)][NowRivalScore];
                    }
                }
                if (Key.IsPushed(KEY_INPUT_2))
                {
                    if (NowTJA.ScoreList != null && NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count > 0)
                    {
                        SoundLoad.Ka[0].Play();
                        if (NowReplay[0] >= NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count - 1) NowReplay[0] = 0;
                        else NowReplay[0]++;
                        ReplayScore[0] = NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)][NowReplay[0]];
                    }
                }
                if (Key.IsPushed(KEY_INPUT_3))
                {
                    if (NowTJA.ScoreList != null && NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count > 0 && PlayData.Data.IsPlay2P)
                    {
                        SoundLoad.Ka[0].Play();
                        if (NowReplay[1] >= NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count - 1) NowReplay[1] = 0;
                        else NowReplay[1]++;
                        ReplayScore[1] = NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)][NowReplay[1]];
                    }
                }

                if (Key.IsPushed(PlayData.Data.MoveConfig))
                {
                    if (Preview != null) { Preview.Stop(); Preview = null; }
                    Program.SceneChange(new Config.Config());
                }
                if (Key.IsPushed(KEY_INPUT_F2))
                {
                    PlayData.Init();
                    SongLoad.Init();
                    NowTJA = SongLoad.SongData[NowSongNumber];
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
                    PlayData.Data.IsPlay2P = !PlayData.Data.IsPlay2P;
                }
                if (Key.IsPushed(KEY_INPUT_F6))
                {
                    PlayData.Data.ShowGraph = !PlayData.Data.ShowGraph;
                }
                if (Key.IsPushed(KEY_INPUT_F7))
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
                if (Key.IsPushed(KEY_INPUT_F8))
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
                    SoundLoad.Ka[0].Play();
                    if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.Data.IsPlay2P)
                    {
                        CourseChange(true, 1);
                        SetScore();
                    }
                    else
                    {
                        CourseChange(true, 0);
                        NowReplay[0] = 0;
                        SetScore();
                    }
                }
                if (Key.IsPushed(KEY_INPUT_RIGHT))
                {
                    SoundLoad.Ka[0].Play();
                    if ((Key.IsPushing(KEY_INPUT_LSHIFT) || Key.IsPushing(KEY_INPUT_RSHIFT)) && PlayData.Data.IsPlay2P)
                    {
                        CourseChange(false, 1);
                    }
                    else
                    {
                        CourseChange(false, 0);
                    }
                }
                if (Key.IsPushed(KEY_INPUT_SLASH))
                {
                    Input.Init();
                }
            }

            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i].Tick();
                PushingTimer[i].Tick();
            }
            base.Update();
        }

        public static void Enter()
        {
            switch (NowTJA.Type)
            {
                case EType.Score:
                    if (File.Exists(NowTJA.Path))
                    {
                        if (NowTJA.Course[EnableCourse(NowTJA.Course, 0)].IsEnable || PlayData.Data.PreviewType == 3)
                        {
                            if (Key.IsPushing(KEY_INPUT_LSHIFT))
                            {
                                Replay[0] = true;
                            }
                            else
                            {
                                Replay[0] = false;
                            }
                            Random = false;
                            NowSListNumber = NowSongNumber;
                            for (int i = 0; i < SongLoad.SongData.Count; i++)
                            {
                                if (SongLoad.SongData[i].Type != EType.Score)
                                    NowSListNumber--;
                            }
                            if (Preview != null) { Preview.Stop(); Preview = null; }
                            Program.SceneChange(new Game.Game());
                        }
                        else
                        {
                            DrawLog.Draw("譜面がありません!難易度を確認してください。");
                        }
                    }
                    else
                    {
                        DrawLog.Draw("TJAが見つかりません!パスを確認してください。");
                    }
                    break;
                case EType.Random:
                    for (int i = 0; i < 100000000; i++)
                    {
                        Random random = new Random();
                        int r = random.Next(SongLoad.SongList.Count);
                        if (PlayData.Data.PlayCourse[0] == (int)ECourse.Edit)
                        {
                            Random difran = new Random();
                            int d = difran.Next(0, 2);
                            Course = 3 + d;
                            if (SongLoad.SongList[r] != null && SongLoad.SongList[r].Course[Course].IsEnable)
                            {
                                NowTJA = SongLoad.SongList[r];
                                break;
                            }
                        }
                        else
                        {
                            if (SongLoad.SongList[r] != null && SongLoad.SongList[r].Course[PlayData.Data.PlayCourse[0]].IsEnable)
                            {
                                NowTJA = SongLoad.SongList[r];
                                break;
                            }
                        }
                    }
                    if (Preview != null) { Preview.Stop(); Preview = null; }
                    if (File.Exists(NowTJA.Path))
                    {
                        if (NowTJA.Course[EnableCourse(NowTJA.Course, 0)].IsEnable || PlayData.Data.PreviewType == 3)
                        {
                            if (Key.IsPushing(KEY_INPUT_LSHIFT))
                            {
                                Replay[0] = true;
                            }
                            else
                            {
                                Replay[0] = false;
                            }
                            Random = true;
                            if (Preview != null) { Preview.Stop(); Preview = null; }
                            Program.SceneChange(new Game.Game());
                        }
                        else
                        {
                            DrawLog.Draw("譜面がありません!難易度を確認してください。");
                        }
                    }
                    else
                    {
                        DrawLog.Draw("TJAが見つかりません!パスを確認してください。");
                    }
                    break;
                case EType.Folder:
                    SongLoad.FolderFloor++;
                    SongLoad.SongData = new List<SongData>();
                    SongLoad.SongList = new List<SongData>();
                    SongLoad.FolderData = new List<string>();
                    NowPath = NowTJA.Path;
                    SongLoad.Load(SongLoad.SongData, NowTJA.Path);
                    NowSongNumber = 0;
                    NowTJA = SongLoad.SongData[NowSongNumber];
                    if (PlayData.Data.FontRendering) FontLoad();
                    SetScore();
                    if (PlayData.Data.PreviewSong)
                    {
                        if (Preview != null) { Preview.Stop(); Preview = null; }
                        if (NowTJA.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(NowTJA.Path)}/{new TJAParse.TJAParse(NowTJA.Path).Header.WAVE}");
                    }
                    break;
                case EType.Back:
                    Back();
                    break;
            }
        }
        public static void Back()
        {
            string title = NowTJA.Title;
            SongLoad.FolderFloor--;
            SongLoad.SongData = new List<SongData>();
            SongLoad.SongList = new List<SongData>();
            SongLoad.FolderData = new List<string>();
            NowPath = NowTJA.Path;
            SongLoad.Load(SongLoad.SongData, NowTJA.Path);
            for (int i = 0; i < SongLoad.SongData.Count; i++)
            {
                if (SongLoad.SongData[i].Title == title)
                {
                    NowSongNumber = i;
                    break;
                }
            }
            NowTJA = SongLoad.SongData[NowSongNumber];
            if (PlayData.Data.FontRendering) FontLoad();
            SetScore();
            if (PlayData.Data.PreviewSong)
            {
                if (Preview != null) { Preview.Stop(); Preview = null; }
                if (NowTJA.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(NowTJA.Path)}/{new TJAParse.TJAParse(NowTJA.Path).Header.WAVE}");
            }
        }

        public static void SetScore()
        {
            NowRivalScore = 0;
            NowReplay[0] = 0;
            NowReplay[1] = 0;
            if (NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)] != null && NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Count > 0)
            {
                NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)].Sort((a, b) => b.Substring(b.Length - 15, 14).CompareTo(a.Substring(a.Length - 15, 14)));
                RivalScore = NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)][0];
                for (int j = 0; j < 2; j++)
                {
                    ReplayScore[j] = NowTJA.ScoreList[EnableCourse(NowTJA.Course, 0)][0];
                }
            }
            else
            {
                RivalScore = null;
                for (int i = 0; i < 2; i++)
                {
                    ReplayScore[i] = null;
                }
            }
            if (PlayData.Data.FontRendering) FontLoad();
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
            string title = NowTJA.Title;
            SongLoad.Sort(SongLoad.SongData, SongLoad.NowSort);
            SongLoad.Sort(SongLoad.SongList, SongLoad.NowSort);

            for (int i = 0; i < SongLoad.SongData.Count; i++)
            {
                if (SongLoad.SongData[i].Title == title)
                {
                    NowSongNumber = i;
                    break;
                }
            }
            NowTJA = SongLoad.SongData[NowSongNumber];
        }

        public static int EnableCourse(Course[] course, int player)
        {
            if (course != null)
            {
                int dif = 0;
                int[] lane = new int[5];
                switch (PlayData.Data.PlayCourse[player])
                {
                    case 0:
                        lane = new int[5] { 0, 1, 2, 3, 4 };
                        break;
                    case 1:
                        lane = new int[5] { 1, 0, 2, 3, 4 };
                        break;
                    case 2:
                        lane = new int[5] { 2, 1, 0, 3, 4 };
                        break;
                    case 3:
                        lane = new int[5] { 3, 4, 2, 1, 0 };
                        break;
                    case 4:
                        lane = new int[5] { 4, 3, 2, 1, 0 };
                        break;
                }
                for (int i = 0; i < 5; i++)
                {
                    if (course[lane[dif]].IsEnable)
                    {
                        return lane[dif];
                    }
                    dif++;
                }
            }
            return PlayData.Data.PlayCourse[player];
        }

        public static void FontLoad()
        {
            if (NowTJA.Header == null) return;

            FontFamily family = new FontFamily(!string.IsNullOrEmpty(PlayData.Data.FontName) ? PlayData.Data.FontName : "MS UI Gothic");
            if (!string.IsNullOrEmpty(NowTJA.Header.TITLE))
            {
                Title = DrawFont.GetTexture(NowTJA.Header.TITLE, family, 96, 4, 0, Color.White, Color.Black);
            }
            else Title = new Texture();
            FontFamily familyb = new FontFamily(!string.IsNullOrEmpty(PlayData.Data.FontName) ? PlayData.Data.FontName : "Arial Black");
            if (!string.IsNullOrEmpty(NowTJA.Header.GENRE))
            {
                Genre = DrawFont.GetTexture(NowTJA.Header.GENRE, familyb, 32, 4, 0, Color.White, Color.Black);
            }
            else Genre = new Texture();
            if (!string.IsNullOrEmpty(NowTJA.Header.SUBTITLE))
            {
                SubTitle = DrawFont.GetTexture(NowTJA.Header.SUBTITLE, familyb, 32, 4, 0, Color.White, Color.Black);
            }
            else SubTitle = new Texture();
            FontFamily familyc = new FontFamily(!string.IsNullOrEmpty(PlayData.Data.FontName) ? PlayData.Data.FontName : "Arial Black");
            BPM = DrawFont.GetTexture($"{Math.Round(NowTJA.Header.BPM, 0, MidpointRounding.AwayFromZero)} BPM", familyc, 32, 6, 0, Color.White, Color.Black);
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

        public static void DrawNumber(double x, double y, string num, int type)
        {
            foreach (char ch in num)
            {
                for (int i = 0; i < stNumber.Length; i++)
                {
                    if (ch == ' ')
                    {
                        break;
                    }
                    if (stNumber[i].ch == ch)
                    {
                        TextureLoad.SongSelect_Number.Draw(x, y, new Rectangle(stNumber[i].X, 30 * type, 30, 30));
                        break;
                    }
                }
                x += 28;
            }
        }

        public static bool[] Replay = new bool[2];
        public static Texture Title, SubTitle, Genre, BPM;
        public static SongData NowTJA;
        public static Counter[] PushedTimer = new Counter[2], PushingTimer = new Counter[2];
        public static Sound Preview;
        public static int NowSongNumber, NowSListNumber, NowRivalScore, Course;
        public static int[] NowReplay = new int[2];
        public static bool Random;
        public static string NowPath, RivalScore;
        public static string[] ReplayScore = new string[2];

        private struct STNumber
        {
            public char ch;
            public int X;
        }
        private static STNumber[] stNumber = new STNumber[11]
        { new STNumber(){ ch = '0', X = 0 },new STNumber(){ ch = '1', X = 30 },new STNumber(){ ch = '2', X = 30 * 2 },new STNumber(){ ch = '3', X = 30 * 3 },new STNumber(){ ch = '4', X = 30 * 4 },
        new STNumber(){ ch = '5', X = 30 * 5 },new STNumber(){ ch = '6', X = 30 * 6 },new STNumber(){ ch = '7', X = 30 * 7 },new STNumber(){ ch = '8', X = 30 * 8 },new STNumber(){ ch = '9', X = 30 * 9 },
        new STNumber(){ ch = '+', X = 30 * 10 } };
    }
}
