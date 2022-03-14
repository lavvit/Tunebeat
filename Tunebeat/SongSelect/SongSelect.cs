using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class SongSelect : Scene
    {
        public override void Enable()
        {
            Number = new Number(TextureLoad.SongSelect_Number, 30, 30, stNumber, -2);
            for (int i = 0; i < 21; i++)
            {
                SongBar[i] = new Button(TextureLoad.SongSelect_Bar, false);
            }
            for (int i = 0; i < 5; i++)
            {
                CourseBar[i] = new Button(TextureLoad.SongSelect_Difficulty, false);
            }
            Title = new Texture();
            SubTitle = new Texture();
            Genre = new Texture();
            BPM = new Texture();
            if (PlayData.Data.PreviewSong)
            {
                if (SongData.NowSong.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(SongData.NowSong.Path)}/{new TJAParse.TJAParse(SongData.NowSong.Path).Header.WAVE}");
            }
            SetSong();
            CreateStep = 0;

            SoundLoad.Don[0].Pan = 0;
            SoundLoad.Don[0].Volume = PlayData.Data.SE / 100.0;
            if (PlayData.Data.PlaySpeed != 1.0 && PlayData.Data.ChangeSESpeed) SoundLoad.Don[0].PlaySpeed = PlayData.Data.PlaySpeed;
            SoundLoad.Ka[0].Pan = 0;
            SoundLoad.Ka[0].Volume = PlayData.Data.SE / 100.0;
            if (PlayData.Data.PlaySpeed != 1.0 && PlayData.Data.ChangeSESpeed) SoundLoad.Ka[0].PlaySpeed = PlayData.Data.PlaySpeed;
            base.Enable();
        }

        public override void Disable()
        {
            Title = null;
            SubTitle = null;
            Genre = null;
            BPM = null;
            Preview = null;
            base.Disable();
        }
        public override void Draw()
        {
            Drawing.Box(0, 0, 1919, 1079, Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
            TextureLoad.SongSelect_Background.Draw(0, 0);

            DrawBar();

            if (SongData.NowSong == null)
            {
                Drawing.Box(0, 308, 530, 348, 0x000000);
                Drawing.Text(80, 320, "TJAがありません。パスを確認し、ロードしてください。");
            }

            TextureLoad.SongSelect_Difficulty_Base.Draw(difXY[0], difXY[1]);
            if (PlayData.Data.IsPlay2P)
            {
                TextureLoad.SongSelect_Difficulty_Base.Draw(difXY[0], difXY[1] + 163, new Rectangle(0, 132, 814, 31));
            }

            switch (SongData.NowSong.Type)
            {
                case EType.Score:
                    DrawSong();
                    DrawDifficulty();
                    DrawMenu();
                    break;
                default:
                    break;
            }

            for (int i = 0; i < 5; i++)
            {
                CourseBar[i].Draw(difXY[0] + 22 + 156 * i, difXY[1] + 8);
                if ((i == PlayData.Data.PlayCourse[0] || (PlayData.Data.IsPlay2P && i == PlayData.Data.PlayCourse[1])) && PlayData.Data.PreviewType < 3)
                {
                    TextureLoad.SongSelect_Difficulty_Cursor.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 8);
                }
            }
            TextureLoad.SongSelect_Difficulty_TJA.Draw(difXY[0], difXY[1]);

            Drawing.Text(80, 640, "OPTION MENU (Beta mode)", 0xffffff);
            Drawing.Text(80, 660, "1P", 0xffffff);
            if (PlayData.Data.Auto[0]) Drawing.Text(110, 660, "AUTO", 0xffffff);
            Drawing.Text(80, 680, $"Gauge:{(EGauge)PlayData.Data.GaugeType[0]}", 0xffffff);
            Drawing.Text(80, 700, $"GAS:{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[0]}", 0xffffff);
            Drawing.Text(80, 720, $"GASmin:{(EGauge)PlayData.Data.GaugeAutoShiftMin[0]}", 0xffffff);
            Drawing.Text(80, 740, $"Hazard:{PlayData.Data.Hazard[0]}", 0xffffff);
            if (PlayData.Data.Random[0]) Drawing.Text(80, 760, $"{PlayData.Data.RandomRate}% Random", 0xffffff);

            if (PlayData.Data.IsPlay2P)
            {
                Drawing.Text(80, 800, "2P", 0xffffff);
                if (PlayData.Data.Auto[1])
                {
                    Drawing.Text(110, 800, "AUTO", 0xffffff);
                }
                Drawing.Text(80, 820, $"Gauge:{(EGauge)PlayData.Data.GaugeType[1]}", 0xffffff);
                Drawing.Text(80, 840, $"GAS:{(EGaugeAutoShift)PlayData.Data.GaugeAutoShift[1]}", 0xffffff);
                Drawing.Text(80, 860, $"GASmin:{(EGauge)PlayData.Data.GaugeAutoShiftMin[1]}", 0xffffff);
                Drawing.Text(80, 880, $"Hazard:{PlayData.Data.Hazard[1]}", 0xffffff);
                if (PlayData.Data.Random[1]) Drawing.Text(80, 900, $"{PlayData.Data.RandomRate}% Random", 0xffffff);
            }

            if (!string.IsNullOrEmpty(PlayData.Data.PlayerName)) Drawing.Text(80, 1000, $"Player:{PlayData.Data.PlayerName}", 0xffffff);

            switch (CreateStep)
            {
                case 1:
                    Drawing.Box(0, 1000, 240, 1040, 0x000000);
                    Drawing.Text(20, 1012, "作成する譜面のフォルダを入力してください。", 0xffffff);
                    break;
                case 2:
                    Drawing.Box(0, 1000, 240, 1040, 0x000000);
                    Drawing.Text(20, 1012, "作成する譜面のファイル名を入力してください。", 0xffffff);
                    break;
            }
            TextDebug.Update();

#if DEBUG
            Drawing.Text(0, 0, $"AllSong:{(SongData.AllSong != null ? $"{SongData.AllSong.Count}" : "Null")}");
            Drawing.Text(0, 20, $"FolderSong:{(SongData.Song != null ? $"{SongData.Song.Count}" : "Null")}");
            Drawing.Text(0, 40, $"Cursor:{SongData.FolderFloor},{Cursor}");
            if (SongData.NowSong != null)
            {
                Drawing.Text(200, 0, $"{SongData.NowSong.Title} , {SongData.NowSong.Path}");
                Drawing.Text(200, 20, $"{SongData.NowSong.Folder}");
            }
            Drawing.Text(1180, 20 * Cursor, ">", 0xff0000);
            int y = 0;
            foreach (Song song in SongData.Song)
            {
                if (y >= 1080) break;
                Drawing.Text(1200, y, song.Title);
                y += 20;
            }
            y = 80;
            foreach (string path in SongData.Ini)
            {
                if (y >= 1080) break;
                Drawing.Text(200, y, path);
                y += 20;
            }
            y = 0;
            #endif
            base.Draw();
        }

        public static void DrawBar()
        {
            if (SongData.NowSong == null) return;
            SongBar[10].Draw(1180, -90 + 60 * 10);
            switch (SongData.NowSong.Type)
            {
                case EType.Score:
                    TextureLoad.SongSelect_Bar_Color.Color = SongData.NowSong.BackColor;
                    TextureLoad.SongSelect_Bar_Color.Opacity = 0.75;
                    TextureLoad.SongSelect_Bar_Color.Draw(1180, -90 + 60 * 10);
                    TextureLoad.SongSelect_Bar.Draw(1180, -90 + 60 * 10);
                    Drawing.Text(1300, -90 + 60 * 10 + 22, SongData.NowSong.Header.TITLE, ColorTranslator.ToWin32(SongData.NowSong.FontColor));
                    if (SongData.NowSong.Course[EnableCourse(SongData.NowSong.Course, 0)].IsEnable)
                    {
                        if (SongData.NowSong.Score != null) TextureLoad.SongSelect_Clear.Draw(1180 + 2, -90 + 2 + 60 * 10, new Rectangle(29 * SongData.NowSong.Score.Score[EnableCourse(SongData.NowSong.Course, 0)].ClearLamp, 0, 29, 56));
                        Number.Draw(1242 - 14 * Score.Digit(SongData.NowSong.Course[EnableCourse(SongData.NowSong.Course, 0)].LEVEL), -90 + 60 * 10 + 14, SongData.NowSong.Course[EnableCourse(SongData.NowSong.Course, 0)].LEVEL, EnableCourse(SongData.NowSong.Course, 0) + 1);
                    }

                    break;
                default:
                    TextureLoad.SongSelect_Bar_Folder_Color.Color = SongData.NowSong.BackColor;
                    TextureLoad.SongSelect_Bar_Folder_Color.Draw(1180, -90 + 60 * 10);
                    TextureLoad.SongSelect_Bar_Folder.Draw(1180, -90 + 60 * 10);
                    Drawing.Text(1300 - 60, -90 + 60 * 10 + 22, SongData.NowSong.Title, ColorTranslator.ToWin32(SongData.NowSong.FontColor));
                    break;
            }

            var prev = SongData.NowSong;
            for (int i = 9; i >= 0; i--)
            {
                SongBar[i].Draw(1212, -90 + 60 * i);
                prev = prev.Prev;
                switch (prev.Type)
                {
                    case EType.Score:
                        TextureLoad.SongSelect_Bar_Color.Color = prev.BackColor;
                        TextureLoad.SongSelect_Bar_Color.Opacity = 0.75;
                        TextureLoad.SongSelect_Bar_Color.Draw(1212, -90 + 60 * i);
                        TextureLoad.SongSelect_Bar.Draw(1212, -90 + 60 * i);
                        Drawing.Text(1332, -90 + 60 * i + 22, prev.Header.TITLE, ColorTranslator.ToWin32(prev.FontColor));
                        if (prev.Course[EnableCourse(prev.Course, 0)].IsEnable)
                        {
                            if (prev.Score != null) TextureLoad.SongSelect_Clear.Draw(1212 + 2, -90 + 2 + 60 * i, new Rectangle(29 * prev.Score.Score[EnableCourse(prev.Course, 0)].ClearLamp, 0, 29, 56));
                            Number.Draw(1242 + 32 - 14 * Score.Digit(prev.Course[EnableCourse(prev.Course, 0)].LEVEL), -90 + 60 * i + 14, prev.Course[EnableCourse(prev.Course, 0)].LEVEL, EnableCourse(prev.Course, 0) + 1);
                        }

                        break;
                    default:
                        TextureLoad.SongSelect_Bar_Folder_Color.Color = prev.BackColor;
                        TextureLoad.SongSelect_Bar_Folder_Color.Draw(1212, -90 + 60 * i);
                        TextureLoad.SongSelect_Bar_Folder.Draw(1212, -90 + 60 * i);
                        Drawing.Text(1332 - 60, -90 + 60 * i + 22, prev.Title, ColorTranslator.ToWin32(prev.FontColor));
                        break;
                }

            }
            var next = SongData.NowSong;
            for (int i = 11; i < 21; i++)
            {
                SongBar[i].Draw(1212, -90 + 60 * i);
                next = next.Next;
                switch (next.Type)
                {
                    case EType.Score:
                        TextureLoad.SongSelect_Bar_Color.Color = next.BackColor;
                        TextureLoad.SongSelect_Bar_Color.Opacity = 0.75;
                        TextureLoad.SongSelect_Bar_Color.Draw(1212, -90 + 60 * i);
                        TextureLoad.SongSelect_Bar.Draw(1212, -90 + 60 * i);
                        Drawing.Text(1332, -90 + 60 * i + 22, next.Header.TITLE, ColorTranslator.ToWin32(next.FontColor));
                        if (next.Course[EnableCourse(next.Course, 0)].IsEnable)
                        {
                            if (next.Score != null) TextureLoad.SongSelect_Clear.Draw(1212 + 2, -90 + 2 + 60 * i, new Rectangle(29 * next.Score.Score[EnableCourse(next.Course, 0)].ClearLamp, 0, 29, 56));
                            Number.Draw(1242 + 32 - 14 * Score.Digit(next.Course[EnableCourse(next.Course, 0)].LEVEL), -90 + 60 * i + 14, next.Course[EnableCourse(next.Course, 0)].LEVEL, EnableCourse(next.Course, 0) + 1);
                        }

                        break;
                    default:
                        TextureLoad.SongSelect_Bar_Folder_Color.Color = next.BackColor;
                        TextureLoad.SongSelect_Bar_Folder_Color.Draw(1212, -90 + 60 * i);
                        TextureLoad.SongSelect_Bar_Folder.Draw(1212, -90 + 60 * i);
                        Drawing.Text(1332 - 60, -90 + 60 * i + 22, next.Title, ColorTranslator.ToWin32(next.FontColor));
                        break;
                }
            }
            for (int i = 0; i < 21; i++)
            {
                if (SongBar[i].Cursoring)
                {
                    TextureLoad.SongSelect_Bar_Cursor.Draw(SongBar[i].X, SongBar[i].Y);
                }
            }
        }
        public static void DrawSong()
        {
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
                if (!string.IsNullOrEmpty(SongData.NowSong.Header.GENRE)) Drawing.Text(difXY[0] + 407 - (Drawing.TextWidth(SongData.NowSong.Header.GENRE) / 2), 190, SongData.NowSong.Header.GENRE);
                if (!string.IsNullOrEmpty(SongData.NowSong.Header.TITLE)) Drawing.Text(difXY[0] + 407 - (Drawing.TextWidth(SongData.NowSong.Header.TITLE) / 2), 240, SongData.NowSong.Header.TITLE);
                if (!string.IsNullOrEmpty(SongData.NowSong.Header.SUBTITLE)) Drawing.Text(difXY[0] + 407 - (Drawing.TextWidth(SongData.NowSong.Header.SUBTITLE) / 2), 340, SongData.NowSong.Header.SUBTITLE);
                string bpm = $"{Math.Round(SongData.NowSong.Header.BPM, 0, MidpointRounding.AwayFromZero)} BPM";
                Drawing.Text(difXY[0] + 407 - (Drawing.TextWidth(bpm, bpm.Length) / 2), 390, bpm, 0xffffff);
            }
        }
        public static void DrawDifficulty()
        {
            bool select = Mouse.X >= difXY[0] + 22 && Mouse.X < difXY[0] + 795 && Mouse.Y >= difXY[1] + 8 && Mouse.Y < difXY[1] + 126;
            double widgh = (Mouse.X - (difXY[0] + 22 + 74.5)) / 156;
            int wcursor = (int)Math.Round(widgh, 0, MidpointRounding.AwayFromZero);
            int course = EnableCourse(SongData.NowSong.Course, 0);
            int course2 = EnableCourse(SongData.NowSong.Course, 1);
            for (int i = 0; i < 5; i++)
            {
                if (i == (select && !Key.IsPushing(EKey.LShift) ? wcursor : course) || (PlayData.Data.IsPlay2P && i == (select && Key.IsPushing(EKey.LShift) ? wcursor : course2)) || PlayData.Data.PreviewType == 3)
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
                if (SongData.NowSong.Course[i].IsEnable)
                {
                    TextureLoad.SongSelect_Difficulty.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 8);
                    TextureLoad.SongSelect_Difficulty_Course.Draw(difXY[0] + 22 + 156 * i, difXY[1] + 100);
                    Score.DrawNumber(difXY[0] + 94 + 156 * i - 12 * Score.Digit(SongData.NowSong.Course[i].LEVEL), difXY[1] + 42, $"{SongData.NowSong.Course[i].LEVEL}", 0);
                }
            }
            if (SongData.NowSong.Course[select && !Key.IsPushing(EKey.LShift) ? wcursor : course].LEVEL > 0)
            {
                int lev = SongData.NowSong.Course[select && !Key.IsPushing(EKey.LShift) ? wcursor : course].LEVEL < 12 ? SongData.NowSong.Course[select && !Key.IsPushing(EKey.LShift) ? wcursor : course].LEVEL : 12;
                for (int i = 0; i < lev; i++)
                {
                    switch (select && !Key.IsPushing(EKey.LShift) ? wcursor : course)
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
                    if (SongData.NowSong.Course[course].LEVEL > 12)
                    {

                        for (int j = 0; j < SongData.NowSong.Course[select && !Key.IsPushing(EKey.LShift) ? wcursor : course].LEVEL - 12; j++)
                        {
                            switch (select && !Key.IsPushing(EKey.LShift) ? wcursor : course)
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
            if (PlayData.Data.IsPlay2P && SongData.NowSong.Course[select && Key.IsPushing(EKey.LShift) ? wcursor : course2].LEVEL > 0)
            {
                int lev = SongData.NowSong.Course[select && Key.IsPushing(EKey.LShift) ? wcursor : course2].LEVEL < 12 ? SongData.NowSong.Course[select && Key.IsPushing(EKey.LShift) ? wcursor : course2].LEVEL : 12;
                for (int i = 0; i < lev; i++)
                {
                    switch (select && Key.IsPushing(EKey.LShift) ? wcursor : course2)
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
                    if (SongData.NowSong.Course[select && Key.IsPushing(EKey.LShift) ? wcursor : course2].LEVEL > 12)
                    {

                        for (int j = 0; j < SongData.NowSong.Course[select && Key.IsPushing(EKey.LShift) ? wcursor : course2].LEVEL - 12; j++)
                        {
                            switch (select && Key.IsPushing(EKey.LShift) ? wcursor : course2)
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
        }
        public static void DrawMenu()
        {
            Drawing.Text(300, 640, "BESTSCORE (Beta mode)", 0xffffff);
            if (SongData.NowSong.Score != null)
            {
                Scores score = SongData.NowSong.Score.Score[EnableCourse(SongData.NowSong.Course, 0)];
                Drawing.Text(300, 660, $"Clear:{(EClear)score.ClearLamp}", 0xffffff);
                Drawing.Text(300, 680, $"GaugeType:{(EGauge)score.GaugeType}", 0xffffff);
                Drawing.Text(300, 700, $"Gauge:{score.Gauge}", 0xffffff);
                Drawing.Text(300, 720, $"Score:{score.Score}", 0xffffff);
                Drawing.Text(300, 740, $"Perfect:{score.Perfect}", 0xffffff);
                Drawing.Text(300, 760, $"Great:{score.Great}", 0xffffff);
                Drawing.Text(300, 780, $"Good:{score.Good}", 0xffffff);
                Drawing.Text(300, 800, $"Bad:{score.Bad}", 0xffffff);
                Drawing.Text(300, 820, $"Poor:{score.Poor}", 0xffffff);
                Drawing.Text(300, 840, $"Roll:{score.Roll}", 0xffffff);
                Drawing.Text(300, 860, $"MaxCombo:{score.MaxCombo}", 0xffffff);
                Drawing.Text(300, 880, $"Rate:{Math.Round(score.Score > 0 ? (1.01 * score.Perfect + 1.0 * score.Great + 0.5 * score.Good) / SongData.NowSong.Course[EnableCourse(SongData.NowSong.Course, 0)].TotalNotes * 100.0 : 0, 4, MidpointRounding.AwayFromZero)}", 0xffffff);
            }

            Drawing.Text(520, 640, "REPLAYDATA MENU (Beta mode)", 0xffffff);
            if (SongData.NowSong.Score != null && !string.IsNullOrEmpty(SongData.NowSong.Score.Score[EnableCourse(SongData.NowSong.Course, 0)].BestScore)
                && File.Exists($@"{Path.GetDirectoryName(SongData.NowSong.Path)}\{Path.GetFileNameWithoutExtension(SongData.NowSong.Path)}.{(ECourse)EnableCourse(SongData.NowSong.Course, 0)}.{PlayData.Data.PlayerName}.{SongData.NowSong.Score.Score[EnableCourse(SongData.NowSong.Course, 0)].BestScore}.tbr"))
            {
                Drawing.Text(520, 660, $"BestScore:{SongData.NowSong.Score.Score[EnableCourse(SongData.NowSong.Course, 0)].BestScore}", 0xffffff);
            }
            else
            {
                Drawing.Text(520, 660, "BestScore:None", 0xffffff);
            }
            if (!string.IsNullOrEmpty(RivalScore) && File.Exists($@"{Path.GetDirectoryName(SongData.NowSong.Path)}\{Path.GetFileNameWithoutExtension(SongData.NowSong.Path)}.{(ECourse)EnableCourse(SongData.NowSong.Course, 0)}.{RivalScore}.tbr"))
            {
                Drawing.Text(520, 680, $"RivalScore:{RivalScore}", 0xffffff);
            }
            else
            {
                Drawing.Text(520, 680, "RivalScore:None", 0xffffff);
            }
            if (!string.IsNullOrEmpty(ReplayScore[0]) && File.Exists($@"{ Path.GetDirectoryName(SongData.NowSong.Path)}\{ Path.GetFileNameWithoutExtension(SongData.NowSong.Path)}.{ (ECourse)EnableCourse(SongData.NowSong.Course, 0)}.{ReplayScore[0]}.tbr"))
            {
                Drawing.Text(520, 700, $"ReplayScore1P:{ReplayScore[0]}", 0xffffff);
                Drawing.Text(540 + Drawing.TextWidth($"ReplayScore1P:{ReplayScore[0]}"), 700, "Press LShift & ENTER to play", 0xffffff);
            }
            else
            {
                Drawing.Text(520, 700, "ReplayScore1P:None", 0xffffff);
            }
            if (PlayData.Data.IsPlay2P)
            {
                if (!string.IsNullOrEmpty(ReplayScore[1]) && File.Exists($@"{Path.GetDirectoryName(SongData.NowSong.Path)}\{Path.GetFileNameWithoutExtension(SongData.NowSong.Path)}.{(ECourse)EnableCourse(SongData.NowSong.Course, 0)}.{ReplayScore[1]}.tbr"))
                {
                    Drawing.Text(520, 720, $"ReplayScore2P:{ReplayScore[1]}", 0xffffff);
                    Drawing.Text(540 + Drawing.TextWidth($"ReplayScore2P:{ReplayScore[1]}"), 720, "Press RShift & ENTER to play", 0xffffff);
                }
                else
                {
                    Drawing.Text(520, 720, "ReplayScore2P:None", 0xffffff);
                }
            }
            if (SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)] != null && SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count > 0)
            {
                Drawing.Text(520, 740, "List:", 0xffffff);
                for (int i = 0; i < SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count; i++)
                {
                    Drawing.Text(520, 760 + 20 * i, SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)][i], 0xffffff);
                }
            }
        }

        public override void Update()
        {
            if (Preview != null && Preview.IsEnable && !Preview.IsPlaying)
            {

                Preview.PlayLoop(new TJAParse.TJAParse(SongData.NowSong.Path).Header.DEMOSTART, true);
                Preview.Volume = PlayData.Data.SystemBGM / 100.0;
                Preview.PlaySpeed = PlayData.Data.PlaySpeed;
            }

            if (Input.IsEnable)
            {
                if (Key.IsPushed(EKey.Enter))
                {
                    if (SongData.NowSong.Type == EType.New && CreateStep > 0)
                    {
                        switch (CreateStep)
                        {
                            case 1:
                                CreateStep++;
                                FolderName = Input.Text;
                                Input.Init();
                                break;
                            case 2:
                                FileName = Input.Text;
                                Input.End();
                                SoundLoad.Don[0].Play();
                                PlayMode = 1;
                                if (Preview != null) { Preview.Stop(); Preview = null; }
                                Program.SceneChange(new Game());
                                break;
                        }
                    }
                    else
                    {
                        if (Input.Text.ToLower().StartsWith("/boot"))
                        {
                            string file = Input.Text.Substring(5).Trim();
                            if (File.Exists(file))
                            {
                                FolderName = Path.GetDirectoryName(file);
                                FileName = Path.GetFileNameWithoutExtension(file);
                                Input.End();
                                SoundLoad.Don[0].Play();
                                PlayMode = 2;
                                if (Preview != null) { Preview.Stop(); Preview = null; }
                                Program.SceneChange(new Game());
                            }
                            else Input.End();
                        }
                        else Input.End();
                    }
                }
                if (Key.IsPushed(EKey.Esc))
                {
                    CreateStep = 0;
                    Input.End();
                }
            }
            else
            {
                if (Key.IsPushed(EKey.Esc))
                {
                    SoundLoad.Ka[0].Play();
                    if (SongData.FolderFloor > 0) Back();
                    else { if (Preview != null) { Preview.Stop(); Preview = null; } Program.SceneChange(new Title()); }
                }
                if (Key.IsPushed(EKey.Enter) || Key.IsPushed(PlayData.Data.LEFTDON) || Key.IsPushed(PlayData.Data.RIGHTDON))
                {
                    SoundLoad.Don[0].Play();
                    Enter();
                }
                if (Key.IsPushed(PlayData.Data.LEFTKA))
                {
                    SoundLoad.Ka[0].Play();
                    Select(true);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTKA))
                {
                    SoundLoad.Ka[0].Play();
                    Select(false);
                }
                if (Key.IsPushed(PlayData.Data.LEFTKA) && Key.IsPushed(PlayData.Data.RIGHTKA))
                {
                    if (SongData.FolderFloor > 0) Back();
                }

                for (int i = 0; i < 21; i++)
                {
                    if (SongBar[i].LPushed)
                    {
                        int cur = i - 10;
                        if (cur < 0)
                        {
                            SoundLoad.Ka[0].Play();
                            for (int j = 0; j < -cur; j++)
                            {
                                Select(true);
                            }
                        }
                        else if (cur > 0)
                        {
                            SoundLoad.Ka[0].Play();
                            for (int j = 0; j < cur; j++)
                            {
                                Select(false);
                            }
                        }
                        else
                        {
                            SoundLoad.Don[0].Play();
                            Enter();
                        }
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    if (CourseBar[i].LPushed)
                    {
                        SoundLoad.Ka[0].Play();
                        if (Key.IsPushing(EKey.LShift) && PlayData.Data.IsPlay2P)
                        {
                            PlayData.Data.PlayCourse[1] = i;
                        }
                        else
                        {
                            PlayData.Data.PlayCourse[0] = i;
                        }
                        SetScore();
                        ReloadSong();
                    }
                }

                if (Key.IsPushed(EKey.Space))
                {
                    SoundLoad.Ka[0].Play();
                    if (SongLoad.NowSort == ESort.Rate_Low) SongLoad.NowSort = ESort.None;
                    else SongLoad.NowSort++;
                    ReloadSong();
                    
                    string str;
                    if (SongLoad.NowSort != ESort.None)
                    {
                        str = $"譜面を並び替えました! Sort:{SongLoad.NowSort}...";
                    }
                    else
                    {
                        str = "譜面の並び順をデフォルトに戻しました!";
                    }
                    TextLog.Draw(str);
                }

                if (Key.IsPushed(EKey.Key_1))
                {
                    if (SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)] != null && SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count > 0)
                    {
                        SoundLoad.Ka[0].Play();
                        if (NowRivalScore >= SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count - 1) NowRivalScore = 0;
                        else NowRivalScore++;
                        RivalScore = SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)][NowRivalScore];
                    }
                }
                if (Key.IsPushed(EKey.Key_2))
                {
                    if (SongData.NowSong.ScoreList != null && SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count > 0)
                    {
                        SoundLoad.Ka[0].Play();
                        if (NowReplay[0] >= SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count - 1) NowReplay[0] = 0;
                        else NowReplay[0]++;
                        ReplayScore[0] = SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)][NowReplay[0]];
                    }
                }
                if (Key.IsPushed(EKey.Key_3))
                {
                    if (SongData.NowSong.ScoreList != null && SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count > 0 && PlayData.Data.IsPlay2P)
                    {
                        SoundLoad.Ka[0].Play();
                        if (NowReplay[1] >= SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count - 1) NowReplay[1] = 0;
                        else NowReplay[1]++;
                        ReplayScore[1] = SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)][NowReplay[1]];
                    }
                }

                if (Key.IsPushed(PlayData.Data.MoveConfig))
                {
                    if (Preview != null) { Preview.Stop(); Preview = null; }
                    Program.SceneChange(new Config());
                }
                if (Key.IsPushed(EKey.F2))
                {
                    PlayData.Init();
                    SongLoad.Init();
                }
                if (Key.IsPushed(EKey.F3))
                {
                    PlayData.Data.Auto[0] = !PlayData.Data.Auto[0];
                }
                if (Key.IsPushed(EKey.F4) && PlayData.Data.IsPlay2P)
                {
                    PlayData.Data.Auto[1] = !PlayData.Data.Auto[1];
                }
                if (Key.IsPushed(EKey.F5))
                {
                    PlayData.Data.IsPlay2P = !PlayData.Data.IsPlay2P;
                }
                if (Key.IsPushed(EKey.F6))
                {
                    PlayData.Data.ShowGraph = !PlayData.Data.ShowGraph;
                }
                if (Key.IsPushed(EKey.F7))
                {
                    if (Key.IsPushing(EKey.LShift) || Key.IsPushing(EKey.RShift) && PlayData.Data.IsPlay2P)
                    {
                        GaugeChange(1);
                    }
                    else
                    {
                        GaugeChange(0);
                    }
                }
                if (Key.IsPushed(EKey.F8))
                {
                    if (Key.IsPushing(EKey.LShift) || Key.IsPushing(EKey.RShift) && PlayData.Data.IsPlay2P)
                    {
                        GASChange(1);
                    }
                    else
                    {
                        GASChange(0);
                    }
                }
                if (Key.IsPushed(EKey.F9))
                {
                    if ((Key.IsPushing(EKey.LShift) || Key.IsPushing(EKey.RShift)) && PlayData.Data.IsPlay2P)
                    {
                        PlayData.Data.Hazard[1]--;
                    }
                    else
                    {
                        PlayData.Data.Hazard[0]--;
                    }
                }
                if (Key.IsPushed(EKey.F10))
                {
                    if ((Key.IsPushing(EKey.LShift) || Key.IsPushing(EKey.RShift)) && PlayData.Data.IsPlay2P)
                    {
                        PlayData.Data.Hazard[1]++;
                    }
                    else
                    {
                        PlayData.Data.Hazard[0]++;
                    }
                }

                if (Key.IsPushed(EKey.Left))
                {
                    SoundLoad.Ka[0].Play();
                    if ((Key.IsPushing(EKey.LShift) || Key.IsPushing(EKey.RShift)) && PlayData.Data.IsPlay2P)
                    {
                        CourseChange(true, 1);
                    }
                    else
                    {
                        CourseChange(true, 0);
                        NowReplay[0] = 0;
                    }
                }
                if (Key.IsPushed(EKey.Right))
                {
                    SoundLoad.Ka[0].Play();
                    if ((Key.IsPushing(EKey.LShift) || Key.IsPushing(EKey.RShift)) && PlayData.Data.IsPlay2P)
                    {
                        CourseChange(false, 1);
                    }
                    else
                    {
                        CourseChange(false, 0);
                        NowReplay[0] = 0;
                    }
                }
                if (Key.IsPushed(EKey.Slash))
                {
                    Input.Init();
                }
            }
            base.Update();
        }

        public static void Select(bool isLeft)
        {
            if (isLeft)
            {
                if (Cursor-- <= 0) Cursor = SongData.Song.Count - 1;
            }
            else
            {
                if (Cursor++ >= SongData.Song.Count - 1) Cursor = 0;
            }
            SetSong();
        }
        public static void ReloadSong()
        {
            string path = SongData.NowSong.Path;
            SongLoad.Sort(SongData.AllSong, SongLoad.NowSort);
            SongLoad.Sort(SongData.Song, SongLoad.NowSort);
            SongLoad.Sort(SongData.FolderSong, SongLoad.NowSort);

            for (int i = 0; i < SongData.Song.Count; i++)
            {
                if (SongData.Song[i].Path == path)
                {
                    Cursor = i;
                    break;
                }
            }
            SongData.NowSong = SongData.Song[Cursor];
        }
        public static void SetSong()
        {
            SongData.NowSong = SongData.Song[Cursor];
            if (PlayData.Data.FontRendering) FontLoad();
            SetScore();
            if (PlayData.Data.PreviewSong)
            {
                if (Preview != null) { Preview.Stop(); Preview = null; }
                if (SongData.NowSong.Type == EType.Score) Preview = new Sound($"{Path.GetDirectoryName(SongData.NowSong.Path)}/{new TJAParse.TJAParse(SongData.NowSong.Path).Header.WAVE}");
            }
        }
        public static void SetScore()
        {
            NowRivalScore = 0;
            NowReplay[0] = 0;
            NowReplay[1] = 0;
            if (SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)] != null && SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Count > 0)
            {
                SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)].Sort((a, b) => b.Substring(b.Length - 15, 14).CompareTo(a.Substring(a.Length - 15, 14)));
                RivalScore = SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)][0];
                for (int j = 0; j < 2; j++)
                {
                    ReplayScore[j] = SongData.NowSong.ScoreList[EnableCourse(SongData.NowSong.Course, 0)][0];
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
        }
        public static void Enter()
        {
            switch (SongData.NowSong.Type)
            {
                case EType.Score:
                    if (File.Exists(SongData.NowSong.Path))
                    {
                        if (SongData.NowSong.Course[EnableCourse(SongData.NowSong.Course, 0)].IsEnable || PlayData.Data.PreviewType == 3)
                        {
                            if (Key.IsPushing(EKey.LShift))
                            {
                                Replay[0] = true;
                            }
                            else
                            {
                                Replay[0] = false;
                            }
                            Random = false;
                            PlayMode = 0;
                            if (Preview != null) { Preview.Stop(); Preview = null; }
                            Program.SceneChange(new Game());
                        }
                        else
                        {
                            TextLog.Draw("譜面がありません!難易度を確認してください。");
                        }
                    }
                    else
                    {
                        TextLog.Draw("TJAが見つかりません!パスを確認してください。");
                    }
                    break;
                case EType.Folder:
                    SongData.FolderFloor++;
                    SongLoad.Load(SongData.NowSong.Path);
                    Cursor = 0;
                    SetSong();
                    break;
                case EType.Random:
                    List<Song> list = SongData.FolderFloor > 0 ? SongData.FolderSong : SongData.AllSong;
                    while (true)
                    {
                        Random random = new Random();
                        int r = random.Next(list.Count);
                        int d = random.Next(0, 3);
                        if (PlayData.Data.PlayCourse[0] == (int)ECourse.Edit)
                        {
                            if (list[r] != null)
                            {
                                if (list[r].Course[4].IsEnable)
                                {
                                    if (list[r].Course[3].IsEnable)
                                    {
                                        Course = d > 0 ? 4 : 3;
                                        if (list[r].Course[Course].IsEnable)
                                        {
                                            SongData.NowSong = list[r];
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Course = 4;
                                        if (list[r].Course[Course].IsEnable)
                                        {
                                            SongData.NowSong = list[r];
                                            break;
                                        }
                                    }
                                }
                                else if (list[r].Course[3].IsEnable)
                                {
                                    Course = 3;
                                    if (list[r].Course[Course].IsEnable)
                                    {
                                        SongData.NowSong = list[r];
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (list[r] != null && list[r].Course[PlayData.Data.PlayCourse[0]].IsEnable)
                            {
                                SongData.NowSong = list[r];
                                break;
                            }
                        }
                    }
                    PlayMode = 0;
                    if (File.Exists(SongData.NowSong.Path))
                    {
                        if (SongData.NowSong.Course[EnableCourse(SongData.NowSong.Course, 0)].IsEnable || PlayData.Data.PreviewType == 3)
                        {
                            if (Key.IsPushing(EKey.LShift))
                            {
                                Replay[0] = true;
                            }
                            else
                            {
                                Replay[0] = false;
                            }
                            Random = true;
                            if (Preview != null) { Preview.Stop(); Preview = null; }
                            Program.SceneChange(new Game());
                        }
                        else
                        {
                            TextLog.Draw("譜面がありません!難易度を確認してください。");
                        }
                    }
                    else
                    {
                        TextLog.Draw("TJAが見つかりません!パスを確認してください。");
                    }
                    break;
                case EType.New:
                    CreateStep++;
                    Input.Init();
                    Input.Text = PlayData.Data.PlayFolder[0];
                    break;
                case EType.Back:
                    Back();
                    break;
                case EType.AllSongs:
                case EType.AllDifficulty:
                    SongData.FolderFloor++;
                    SongLoad.AllLoad();
                    Cursor = 0;
                    SetSong();
                    break;
            }
        }
        public static void Back()
        {
            SongData.NowSong = SongData.Song[0];
            SongData.FolderFloor--;
            SongLoad.Load(SongData.NowSong.Folder);
            Cursor = 0;
            foreach (Song song in SongData.Song)
            {
                if (song.Path == SongData.NowSong.Path) break;
                Cursor++;
            }
            SetSong();
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

            ReloadSong();
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
            if (SongData.NowSong.Header == null) return;

            if (!string.IsNullOrEmpty(SongData.NowSong.Header.TITLE))
            {
                Title = FontRender.GetTexture(SongData.NowSong.Header.TITLE, 96, 6, PlayData.Data.FontName);
            }
            else Title = new Texture();
            if (!string.IsNullOrEmpty(SongData.NowSong.Header.GENRE))
            {
                Genre = FontRender.GetTexture(SongData.NowSong.Header.GENRE, 32, 4, PlayData.Data.FontName);
            }
            else Genre = new Texture();
            if (!string.IsNullOrEmpty(SongData.NowSong.Header.SUBTITLE))
            {
                SubTitle = FontRender.GetTexture(SongData.NowSong.Header.SUBTITLE, 32, 4, PlayData.Data.FontName);
            }
            else SubTitle = new Texture();
            BPM = FontRender.GetTexture($"{Math.Round(SongData.NowSong.Header.BPM, 0, MidpointRounding.AwayFromZero)} BPM", 32, 6, PlayData.Data.FontName);
        }

        public static int Cursor, NowRivalScore, Course, CreateStep, PlayMode;
        public static int[] difXY = new int[2] { 72, 500 - 60 }, NowReplay = new int[2];
        public static bool Random;
        public static bool[] Replay = new bool[2];
        public static string RivalScore, FolderName, FileName;
        public static string[] ReplayScore = new string[2];
        public static Texture Title, SubTitle, Genre, BPM;
        public static Sound Preview;
        public static Button[] SongBar = new Button[21], CourseBar = new Button[5];
        public static Number Number;
        public static STNumber[] stNumber = new STNumber[11]
        { new STNumber(){ ch = '0', X = 0 },new STNumber(){ ch = '1', X = 30 },new STNumber(){ ch = '2', X = 30 * 2 },new STNumber(){ ch = '3', X = 30 * 3 },new STNumber(){ ch = '4', X = 30 * 4 },
        new STNumber(){ ch = '5', X = 30 * 5 },new STNumber(){ ch = '6', X = 30 * 6 },new STNumber(){ ch = '7', X = 30 * 7 },new STNumber(){ ch = '8', X = 30 * 8 },new STNumber(){ ch = '9', X = 30 * 9 },
        new STNumber(){ ch = '+', X = 30 * 10 } };
    }
}
