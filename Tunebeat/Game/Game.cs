﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class Game : Scene
    {
        public override void Enable()
        {
            TJAPath = SongSelect.PlayMode > 0 ? $@"{SongSelect.FolderName}\{SongSelect.FileName}.tja" : SongData.NowSong.Path;
            if (!File.Exists(TJAPath))
            {
                if (!Directory.Exists(SongSelect.FolderName)) Directory.CreateDirectory(SongSelect.FolderName);
                string[] tja = new string[]
                {
                    $"TITLE:{SongSelect.FileName}",
                    "SUBTITLE:--",
                    "BPM:120",
                    $"WAVE:{SongSelect.FileName}.ogg",
                    "OFFSET:-0",
                    "DEMOSTART:0",
                    $"COURSE:{SongData.NowSong.Course[0]}",
                    "LEVEL:0",
                    "#START",
                    "#END"
                };
                ConfigIni ini = new ConfigIni();
                ini.AddList(tja);
                ini.SaveConfig(TJAPath);
            }
            MainTimer = new Counter(-2000, int.MaxValue, 1000, false);

            for (int i = 0; i < 2; i++)
            {
                IsAuto[i] = PlayData.Data.PreviewType == 3 ? true : PlayData.Data.Auto[i];
                IsReplay[i] = SongSelect.Replay[i] && !string.IsNullOrEmpty(SongSelect.ReplayScore[i]) ? true : false;
                Course[i] = SongSelect.Random ? SongSelect.Course : SongSelect.EnableCourse(SongData.NowSong, i);
                Failed[i] = false;
                PushedTimer[i] = new Counter(0, 499, 1000, false);
                PushingTimer[i] = new Counter(0, 99, 1000, false);
                SuddenTimer[i] = new Counter(0, 499, 1000, false);
                Adjust[i] = !PlayData.Data.Auto[i] ? PlayData.Data.InputAdjust[i] : 0;

                if (IsReplay[i]) IsAuto[i] = false;
            }
            Adjust[2] = Adjust[3] = PlayData.Data.InputAdjust[0];
            PlayMemory.Init();

            if (PlayData.Data.PreviewType == 3)
            {
                SongData.NowTJA = new TJA[5];
                for (int i = 0; i < 2; i++)
                {
                    ReplayData replay = i > 0 ? PlayMemory.ReplayData2P : PlayMemory.ReplayData;
                    SongData.NowTJA[i] = new TJA(TJAPath, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
                for (int i = 2; i < 5; i++)
                {
                    SongData.NowTJA[i] = new TJA(TJAPath, IsReplay[0] ? (PlayMemory.ReplayData.Speed > 0 ? PlayMemory.ReplayData.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
                }
            }
            else
            {
                SongData.NowTJA = new TJA[2];
                for (int i = 0; i < 2; i++)
                {
                    ReplayData replay = i > 0 ? PlayMemory.ReplayData2P : PlayMemory.ReplayData;
                    SongData.NowTJA[i] = new TJA(TJAPath, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
            }
            MainSong = new Sound($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{SongData.NowTJA[0].Header.WAVE}");
            MainImage = new Texture($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{SongData.NowTJA[0].Header.BGIMAGE}");
            string path = $"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{SongData.NowTJA[0].Header.BGMOVIE}";
            string mp4path = path.Replace("wmv", "mp4");
            if (PlayData.Data.PlayMovie) MainMovie = new Movie(File.Exists(mp4path) ? mp4path : path);
            if (PlayData.Data.FontRendering)
            {
                Title = FontRender.GetTexture(SongData.NowTJA[0].Header.TITLE, 48, 6, PlayData.Data.FontName);
                SubTitle = FontRender.GetTexture(SongData.NowTJA[0].Header.SUBTITLE, 20, 4, PlayData.Data.FontName);
            }
            Play2P = SongData.NowTJA[1].Courses[Course[1]].ListChip.Count > 0 ? PlayData.Data.IsPlay2P : false;
            PlayMemory.SetColor();

            if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
            {
                int count = 0;
                for (int i = 4; i >= 0; i--)
                {
                    if (SongData.NowTJA[count].Courses[i].ListChip.Count > 0)
                    {
                        Course[count] = i;
                        count++;
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                HitTimer[i] = new Counter[4];
                for (int j = 0; j < 4; j++)
                {
                    HitTimer[i][j] = new Counter(0, 100, 1000, false);
                }
            }

            PlayMeasure = 0;
            StartTime = 0;
            MeasureList = new List<Chip>();
            MeasureCount();
            LyricHandle = new Handle(PlayData.Data.FontName, 48);

            for (int i = 0; i < 5; i++)
            {
                SoundLoad.Don[i].Volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
                SoundLoad.DON[i].Volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
                SoundLoad.Ka[i].Volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
                SoundLoad.KA[i].Volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
                SoundLoad.Balloon[i].Volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
                SoundLoad.Kusudama[i].Volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            }
            
            #region AddChildScene
            AddChildScene(new Notes());
            AddChildScene(new Score());
            AddChildScene(new Create());
            #endregion
            base.Enable();
        }

        public override void Disable()
        {
            MainSong.Dispose();
            MainImage.Dispose();
            if (MainMovie != null) MainMovie.Dispose();
            MainTimer.Reset();
            IsSongPlay = false;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    HitTimer[i][j].Reset();
                }
            }
            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i].Reset();
                PushingTimer[i].Reset();
                SuddenTimer[i].Reset();
            }
            MeasureList = null;

            foreach (Scene scene in ChildScene)
                scene?.Disable();
            base.Disable();
        }

        public static void MeasureCount()
        {
            for (int i = 0; i < SongData.NowTJA[0].Courses[Course[0]].ListChip.Count; i++)
            {
                if (SongData.NowTJA[0].Courses[Course[0]].ListChip[i].EChip == EChip.Measure)
                {
                    MeasureList.Add(SongData.NowTJA[0].Courses[Course[0]].ListChip[i]);
                }
            }
        }

        public static void Reset()
        {
            MainTimer.Stop();
            MainSong.Stop();
            if (MainMovie != null && MainMovie.IsEnable) MainMovie.Stop();
            if (PlayMeasure == 0) MainTimer.Reset();
            else MainTimer.Value = (int)StartTime;
            IsSongPlay = false;
            for (int i = 0; i < 2; i++)
            {
                Failed[i] = false;
            }
            if (PlayData.Data.PreviewType == 3)
            {
                SongData.NowTJA = new TJA[5];
                for (int i = 0; i < 2; i++)
                {
                    ReplayData replay = i > 0 ? PlayMemory.ReplayData2P : PlayMemory.ReplayData;
                    SongData.NowTJA[i] = new TJA(TJAPath, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
                for (int i = 2; i < 5; i++)
                {
                    SongData.NowTJA[i] = new TJA(TJAPath, IsReplay[0] ? (PlayMemory.ReplayData.Speed > 0 ? PlayMemory.ReplayData.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
                }
            }
            else
            {
                SongData.NowTJA = new TJA[2];
                for (int i = 0; i < 2; i++)
                {
                    ReplayData replay = i > 0 ? PlayMemory.ReplayData2P : PlayMemory.ReplayData;
                    SongData.NowTJA[i] = new TJA(TJAPath, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
            }
            if (File.Exists(MainSong.FileName)) MainSong.Time = StartTime / 1000;
            if (MainMovie != null && MainMovie.IsEnable) MainMovie.Time = StartTime;

            for (int i = 0; i < 5; i++)
            {
                Score.EXScore[i] = 0;
                Score.Poor[i] = 0;
                Score.Auto[i] = 0;
                Score.AutoRoll[i] = 0;
                Score.Combo[i] = 0;
                Score.MaxCombo[i] = 0;
            }
            for (int i = 0; i < 2; i++)
            {
                Score.Perfect[i] = 0;
                Score.Great[i] = 0;
                Score.Good[i] = 0;
                Score.Bad[i] = 0;
                Score.Roll[i] = 0;
                Score.RollYellow[i] = 0;
                Score.RollBalloon[i] = 0;
                Score.Gauge[i] = 0;
                Score.SetGauge(i);
                Score.msSum[i] = 0;
                Score.Hit[i] = 0;
            }

            for (int i = 0; i < 2; i++)
            {
                Notes.UseSudden[i] = PlayData.Data.UseSudden[i];
                Notes.Sudden[i] = PlayData.Data.SuddenNumber[i];
                if (PlayData.Data.PreviewType < 3) Notes.Scroll[i] = PlayData.Data.ScrollSpeed[i];
            }

            if (Create.Edited)
            {
                TextLog.Draw("セーブしました!");
                Create.Save(TJAPath);
                Create.Edited = false;
            }
            Create.Read();
            Create.BarLoad(Course[0]);

            MeasureList = new List<Chip>();
            MeasureCount();
            if (MeasureList.Count == 0)
            {
                PlayMeasure = 0;
                TimeRemain += MainTimer.Value + 2000;
                StartTime = 0;
                MainTimer.Value = -2000;
                if (File.Exists(MainSong.FileName)) MainSong.Time = 0;
                if (MainMovie != null && MainMovie.IsEnable) MainMovie.Time = 0;
                Score.Auto[0] = 0;
                Score.Combo[0] = 0;
                Score.MaxCombo[0] = 0;
                Score.GaugeList[0] = new double[6] { 0.0, 0.0, 0.0, 100.0, 100.0, 100.0 };
                Score.Poor[0] = 0;
                Score.EXScore[2] = 0;
                Score.EXScore[3] = 0;
            }
            else if (PlayMeasure > MeasureList.Count || StartTime > MeasureList[MeasureList.Count - 1].Time)
            {
                MeasureUp(true);
            }
            else
            {
                for (int i = 1; i < MeasureList.Count; i++)
                {
                    if (StartTime == MeasureList[i].Time)
                    {
                        PlayMeasure = i + 1;
                        TimeRemain += MainTimer.Value - (int)Math.Ceiling(MeasureList[i].Time);
                        StartTime = Math.Ceiling(MeasureList[i].Time);
                        MainTimer.Value = (int)Math.Ceiling(MeasureList[i].Time);
                        if (MainMovie != null && MainMovie.IsEnable) MainMovie.Time = Math.Ceiling(MeasureList[i].Time);
                        break;
                    }
                }
            }

            PlayMemory.Init();
            if (PlayMemory.BestData.Chip != null)
            {
                foreach (ChipData data in PlayMemory.BestData.Chip)
                {
                    data.Hit = false;
                }
            }
            if (PlayMemory.RivalData.Chip != null)
            {
                foreach (ChipData data in PlayMemory.RivalData.Chip)
                {
                    data.Hit = false;
                }
            }
        }

        public static void MeasureUp(bool end = false)
        {
            if (MeasureList.Count == 0) return;
            if (end)
            {
                PlayMeasure = MeasureList.Count;
                TimeRemain += MainTimer.Value - (int)Math.Ceiling(MeasureList[MeasureList.Count - 1].Time);
                StartTime = Math.Ceiling(MeasureList[MeasureList.Count - 1].Time);
                MainTimer.Value = (int)Math.Ceiling(MeasureList[MeasureList.Count - 1].Time);
                if (MainMovie != null && MainMovie.IsEnable) MainMovie.Time = Math.Ceiling(MeasureList[MeasureList.Count - 1].Time);
            }
            else if (PlayMeasure < MeasureList.Count)
            {
                PlayMeasure++;
                TimeRemain += MainTimer.Value - (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                StartTime = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                MainTimer.Value = (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                if (MainMovie != null && MainMovie.IsEnable) MainMovie.Time = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
            }
        }
        public static void MeasureDown(bool home = false)
        {
            if (MeasureList.Count == 0) return;
            if (home || PlayMeasure == 1)
            {
                PlayMeasure = 0;
                TimeRemain += MainTimer.Value + 2000;
                StartTime = 0;
                MainTimer.Value = -2000;
                if (MainMovie != null && MainMovie.IsEnable) MainMovie.Time = 0;
                for (int i = 0; i < 2; i++)
                {
                    if (Create.CreateMode)
                    {
                        for (int j = 0; j < Create.File.Bar[Course[i]].Count; j++)
                        {
                            foreach (Chip chip in Create.File.Bar[Course[i]][j].Chip)
                            {
                                if (chip.Time >= StartTime - 1 && chip.IsMiss)
                                {
                                    chip.IsMiss = false;
                                    if (IsAuto[i])
                                    {
                                        Score.Auto[i]--;
                                        Score.Combo[i]--;
                                        Score.MaxCombo[i]--;
                                        Score.DeleteGauge(i);
                                    }
                                    else
                                    {
                                        Score.Poor[i]--;
                                    }
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        foreach (Chip chip in SongData.NowTJA[i].Courses[Course[i]].ListChip)
                        {
                            if (chip.Time >= StartTime - 1 && chip.IsMiss)
                            {
                                chip.IsMiss = false;
                                if (IsAuto[i])
                                {
                                    Score.Auto[i]--;
                                    Score.Combo[i]--;
                                    Score.MaxCombo[i]--;
                                    Score.DeleteGauge(i);
                                }
                                else
                                {
                                    Score.Poor[i]--;
                                }
                            }
                        }
                    }
                }
                ProcessReplay.Back();
            }
            else if (PlayMeasure > 1)
            {
                PlayMeasure--;
                TimeRemain += MainTimer.Value - (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                StartTime = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                MainTimer.Value = (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                if (MainMovie != null && MainMovie.IsEnable) MainMovie.Time = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                for (int i = 0; i < 2; i++)
                {
                    foreach (Chip chip in SongData.NowTJA[i].Courses[Course[i]].ListChip)
                    {
                        if (chip.Time >= StartTime - 1 && chip.IsMiss)
                        {
                            chip.IsMiss = false;
                            if (IsAuto[i])
                            {
                                Score.Auto[i]--;
                                Score.Combo[i]--;
                                Score.MaxCombo[i]--;
                                Score.DeleteGauge(i);
                            }
                            else
                            {
                                Score.Poor[i]--;
                            }
                        }
                    }
                }
                ProcessReplay.Back();
            }
        }

        public override void Draw()
        {
            if (MainMovie != null && MainMovie.IsEnable)
            {
                MainMovie.Draw(0, 0);
            }
            else if (PlayData.Data.ShowImage && File.Exists($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{SongData.NowTJA[0].Header.BGIMAGE}"))
            {
                MainImage.Draw(960 - (MainImage.TextureSize.Width / 2), 960 - (MainImage.TextureSize.Height / 2));
            }
            else
            {
                Drawing.Box(0, 0, 1919, 1079, Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
                TextureLoad.Game_Background.Draw(0, 0);
            }


            foreach (Scene scene in ChildScene)
                scene?.Draw();

            for (int i = 0; i < 5; i++)
            {
                if (HitTimer[i][0].State == TimerState.Started)
                {
                    TextureLoad.Game_Don[i][0].Opacity = 1.0 - ((double)HitTimer[i][0].Value / HitTimer[i][0].End);
                    TextureLoad.Game_Don[i][0].Draw(362, Notes.NotesP[i].Y + 47);
                }
                if (HitTimer[i][1].State == TimerState.Started)
                {
                    TextureLoad.Game_Don[i][1].Opacity = 1.0 - ((double)HitTimer[i][1].Value / HitTimer[i][1].End);
                    TextureLoad.Game_Don[i][1].Draw(362, Notes.NotesP[i].Y + 47);
                }
                if (HitTimer[i][2].State == TimerState.Started)
                {
                    TextureLoad.Game_Ka[i][0].Opacity = 1.0 - ((double)HitTimer[i][2].Value / HitTimer[i][2].End);
                    TextureLoad.Game_Ka[i][0].Draw(362, Notes.NotesP[i].Y + 47);
                }
                if (HitTimer[i][3].State == TimerState.Started)
                {
                    TextureLoad.Game_Ka[i][1].Opacity = 1.0 - ((double)HitTimer[i][3].Value / HitTimer[i][3].End);
                    TextureLoad.Game_Ka[i][1].Draw(362, Notes.NotesP[i].Y + 47);
                }
            }

            if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (SongData.NowTJA[i].Courses[Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Course[i] == Course[i - 1]) break;
                        Score.DrawCombo(i);
                    }
                }
            }
            else
            {
                Score.DrawCombo(0);
                if (Play2P)
                {
                    Score.DrawCombo(1);
                }
            }

            if (MainTimer.State == 0 && !IsSongPlay)
            {
                Drawing.Text(720, 356, $"{PlayMeasure}/{MeasureList.Count}", 0xffffff);
                Drawing.Text(720, 376, $"PRESS {((EKey)PlayData.Data.PlayStart).ToString().ToUpper()} KEY", 0xffffff);
            }

            if ((PlayData.Data.PreviewType >= (int)EPreviewType.Down) || (PlayData.Data.PreviewType == (int)EPreviewType.Normal && !(Play2P || PlayData.Data.ShowGraph)))
            {
                if (PlayData.Data.FontRendering)
                {
                    Title.Draw(1920 - Title.ActualSize.Width - 20, 20);
                    SubTitle.Draw(1920 - SubTitle.ActualSize.Width - 20, 80);
                }
                else
                {
                    Drawing.Text(1920 - Drawing.TextWidth(SongData.NowTJA[0].Header.TITLE, SongData.NowTJA[0].Header.TITLE.Length) - 20, 40, SongData.NowTJA[0].Header.TITLE, 0xffffff);
                    Drawing.Text(1920 - Drawing.TextWidth(SongData.NowTJA[0].Header.SUBTITLE, SongData.NowTJA[0].Header.SUBTITLE.Length) - 20, 90, SongData.NowTJA[0].Header.SUBTITLE, 0xffffff);
                }
            }
            else 
            {
                if (PlayData.Data.FontRendering)
                {
                    Title.Draw(1920 - Title.ActualSize.Width - 20, 980);
                    SubTitle.Draw(1920 - SubTitle.ActualSize.Width - 20, 1040);
                }
                else
                {
                    Drawing.Text(1920 - Drawing.TextWidth(SongData.NowTJA[0].Header.TITLE, SongData.NowTJA[0].Header.TITLE.Length) - 20, 990, SongData.NowTJA[0].Header.TITLE, 0xffffff);
                    Drawing.Text(1920 - Drawing.TextWidth(SongData.NowTJA[0].Header.SUBTITLE, SongData.NowTJA[0].Header.SUBTITLE.Length) - 20, 1040, SongData.NowTJA[0].Header.SUBTITLE, 0xffffff);
                }
            }

            if (PlayData.Data.PreviewType == 3)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (SongData.NowTJA[i].Courses[Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Course[i] == Course[i - 1]) break;
                        count++;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    string Lv = $"{SongData.NowTJA[i].Courses[Course[i]].COURSE} Lv.{SongData.NowTJA[i].Courses[Course[i]].LEVEL}";
                    Drawing.Text(413 - Drawing.TextWidth(Lv, Lv.Length) / 2, Notes.NotesP[i].Y + 6, Lv, 0xffffff);
                    if (SongData.NowTJA[i].Courses[Course[i]].ScrollType != EScroll.Normal)
                    {
                        Drawing.Text(378, Notes.NotesP[i].Y + 26, $"{SongData.NowTJA[i].Courses[Course[i]].ScrollType}", 0xffffff);
                    }
                }
            }
            else
            {
                string Lv1P = $"{SongData.NowTJA[0].Courses[Course[0]].COURSE} Lv.{SongData.NowTJA[0].Courses[Course[0]].LEVEL}";
                Drawing.Text(413 - Drawing.TextWidth(Lv1P) / 2, Notes.NotesP[0].Y + 6, Lv1P, 0xffffff);
                if (SongData.NowTJA[0].Courses[Course[0]].ScrollType != EScroll.Normal)
                {
                    string scr = $"{SongData.NowTJA[0].Courses[Course[0]].ScrollType}";
                    Drawing.Text(413 - Drawing.TextWidth(scr) / 2, Notes.NotesP[0].Y + 26, scr, 0xffffff);
                }
                if (IsAuto[0] || IsReplay[0]) Drawing.Text(258, Notes.NotesP[0].Y + 6, "AUTO PLAY", 0xffffff);
                if (Play2P)
                {
                    string Lv2P = $"{SongData.NowTJA[1].Courses[Course[1]].COURSE} Lv.{SongData.NowTJA[1].Courses[Course[1]].LEVEL}";
                    Drawing.Text(413 - Drawing.TextWidth(Lv2P) / 2, Notes.NotesP[0].Y + 416, Lv2P, 0xffffff);
                    if (SongData.NowTJA[1].Courses[Course[1]].ScrollType != EScroll.Normal)
                    {
                        string scr = $"{SongData.NowTJA[1].Courses[Course[1]].ScrollType}";
                        Drawing.Text(413 - Drawing.TextWidth(scr) / 2, Notes.NotesP[0].Y + 436, scr, 0xffffff);
                    }
                    if (IsAuto[1] || IsReplay[1]) Drawing.Text(258, Notes.NotesP[0].Y + 416, "AUTO PLAY", 0xffffff);
                }
            }

            Chip nchip = GetNotes.GetNowNote(SongData.NowTJA[0].Courses[Course[0]].ListChip, MainTimer.Value, true);
            if (nchip != null && nchip.Lyric != null)
            {
                if (PlayData.Data.FontRendering)
                {
                    if (lyric != nchip.Lyric)
                    {
                        GenerateLyric(nchip);
                        lyric = nchip.Lyric;
                    }
                    Lyric.Draw(960 - Lyric.ActualSize.Width / 2, 1000);
                }
                else
                {
                    if (!string.IsNullOrEmpty(nchip.Lyric)) Drawing.Text(960 - Drawing.TextWidth(nchip.Lyric, LyricHandle) / 2, 1000, nchip.Lyric, LyricHandle, 0x0000ff);
                }
            }

            TextDebug.Update();

            #if DEBUG
            Drawing.Text(0, 0, $"{MainTimer.Value}", 0xffffff); if (IsSongPlay && !MainSong.IsPlaying) Drawing.Text(60, 0, "Stoped", 0xffffff);
            Drawing.Text(0, 20, $"{SongData.NowTJA[0].Header.TITLE}", 0xffffff);
            Drawing.Text(0, 40, $"{SongData.NowTJA[0].Header.SUBTITLE}", 0xffffff);
            Drawing.Text(0, 60, $"{SongData.NowTJA[0].Header.BPM}", 0xffffff);

            Drawing.Text(0, 80, $"{SongData.NowTJA[0].Courses[Course[0]].COURSE}" + (Play2P ? $"/{SongData.NowTJA[0].Courses[Course[1]].COURSE}" : ""), 0xffffff);
            Drawing.Text(0, 100, $"{SongData.NowTJA[0].Courses[Course[0]].LEVEL}" + (Play2P ? $"/{SongData.NowTJA[0].Courses[Course[1]].LEVEL}" : ""), 0xffffff);
            Drawing.Text(0, 120, $"{SongData.NowTJA[0].Courses[Course[0]].TotalNotes}" + (Play2P ? $"/{SongData.NowTJA[0].Courses[Course[1]].TotalNotes}" : ""), 0xffffff);
            Drawing.Text(0, 140, $"{SongData.NowTJA[0].Courses[Course[0]].ScrollType}" + (Play2P ? $"/{SongData.NowTJA[0].Courses[Course[1]].ScrollType}" : ""), 0xffffff);
            Drawing.Text(200, 0, $"{PlayData.Data.AutoRoll}", 0xffffff);
            Drawing.Text(300, 0, $"{PlayMemory.ChipData.Count}", 0xffffff);

            Chip[] chip = new Chip[2] { GetNotes.GetNowNote(SongData.NowTJA[0].Courses[Course[0]].ListChip, MainTimer.Value), GetNotes.GetNowNote(SongData.NowTJA[0].Courses[Course[1]].ListChip, MainTimer.Value) };
            if (chip[0] != null)
            {
                Drawing.Text(520, 160, $"{chip[0].ENote}", 0xffffff);
                Drawing.Text(520, 180, $"{chip[0].Time}", 0xffffff);
                Drawing.Text(520, 200, $"{ProcessNote.RollState(chip[0])}", 0xffffff);
                Drawing.Text(520, 220, $"{chip[0].RollCount}", 0xffffff);
                Drawing.Text(520, 240, $"{chip[0].Balloon}", 0xffffff);
            }
            if (Play2P && chip[1] != null)
            {
                Drawing.Text(520, 780, $"{chip[1].ENote}", 0xffffff);
                Drawing.Text(520, 800, $"{chip[1].Time}", 0xffffff);
                Drawing.Text(520, 820, $"{ProcessNote.RollState(chip[1])}", 0xffffff);
                Drawing.Text(520, 840, $"{chip[1].RollCount}", 0xffffff);
                Drawing.Text(520, 860, $"{chip[1].Balloon}", 0xffffff);
            }

            Drawing.Text(700, 160, $"NowAdjust:{Adjust[0]}", 0xffffff);
            Drawing.Text(700, 180, $"Average:{Score.msAverage[0]}", 0xffffff);
            if (Play2P && chip[1] != null)
            {
                Drawing.Text(700, 780, $"NowAdjust:{Adjust[1]}", 0xffffff);
                Drawing.Text(700, 800, $"Average:{Score.msAverage[1]}", 0xffffff);
            }
            Drawing.Text(720, 320, $"{NowMeasure}/{MeasureList.Count}", 0xffffff);

            if (IsSongPlay && !MainSong.IsPlaying) Drawing.Text(0, 160, "PRESS ENTER", 0xffffff);
            #endif

            base.Draw();
        }

        public static void GenerateLyric(Chip chip)
        {
            Lyric = FontRender.GetTexture(chip.Lyric, 56, 4, Color.White, Color.Blue, PlayData.Data.FontName);
            return;
        }
        public override void Update()
        {
            MainTimer.Tick();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    HitTimer[i][j].Tick();
                }
            }
            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i].Tick();
                PushingTimer[i].Tick();
                SuddenTimer[i].Tick();
            }
            if (MainTimer.State == 0 && Create.CommandLayer == 0 && ((!(Create.CreateMode && Create.InfoMenu) && Key.IsPushed(EKey.Enter)) || Key.IsPushed((EKey)PlayData.Data.PlayStart) || PlayData.Data.QuickStart))
            {
                MainTimer.Start();
                if (IsAuto[0]) PlayData.Data.InputAdjust[0] = Adjust[0];
                PlayMemory.InitSetting(0, MainTimer.Begin, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Adjust[0]);
                if (Play2P) { PlayMemory.InitSetting(1, MainTimer.Begin, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Adjust[1]); if (IsAuto[1]) PlayData.Data.InputAdjust[1] = Adjust[1]; }

            }
            if (MainTimer.Value >= 0 && MainTimer.State != 0 && !MainSong.IsPlaying && !IsSongPlay)
            {
                if (MainSong.IsEnable && File.Exists(MainSong.FileName))
                {
                    MainSong.Play(PlayMeasure == 0 ? true : false);
                    if (PlayMeasure > 0) MainSong.Time = Math.Ceiling(MeasureList[PlayMeasure - 1].Time) / 1000;
                    MainSong.PlaySpeed = IsReplay[0] ? PlayMemory.ReplayData.Speed : PlayData.Data.PlaySpeed;
                    MainSong.Volume = (PlayData.Data.GameBGM / 100.0) * (SongData.NowTJA[0].Header.SONGVOL / 100.0);
                }
                IsSongPlay = true;
                if (MainMovie != null && MainMovie.IsEnable)
                {
                    MainMovie.PlayGraph(PlayMeasure == 0 ? true : false);
                    MainMovie.PlaySpeed = PlayData.Data.PlaySpeed;
                }
            }
            if (IsSongPlay && !MainSong.IsPlaying)
            {
                if (PlayData.Data.SaveScore && !Play2P && PlayMeasure == 0 && MainTimer.State != 0)
                {
                    PlayMemory.SaveScore(0, Course[0]);
                }

                //MainTimer.Stop();
                //MainMovie.Stop();
                PlayData.End();
                if (Create.CreateMode)
                {
                    if (Create.Mapping)
                    {
                        Create.Mapping = !Create.Mapping;
                        TextLog.Draw("マッピングをセーブしました!");
                        Create.Save(TJAPath);
                    }
                    Reset();
                }
                else
                {
                    if (PlayData.Data.PlayList)
                    {
                        List<Song> list = SongData.FolderFloor > 0 ? SongData.FolderSong : SongData.AllSong;
                        if (SongSelect.Random)
                        {
                            while(true)
                            {
                                Random random = new Random();
                                int r = random.Next(list.Count);
                                int d = random.Next(0, 3);
                                if (SongData.NowSong.DisplayDif > 0)
                                {
                                    if (list[r] != null && list[r].DisplayDif - 1 <= PlayData.Data.PlayCourse[0])
                                    {
                                        TJAPath = list[r].Path;
                                        for (int i = 0; i < 2; i++)
                                        {
                                            Course[i] = list[r].DisplayDif - 1;
                                        }
                                        break;
                                    }
                                }
                                else
                                {
                                    if (PlayData.Data.PlayCourse[0] == (int)ECourse.Edit)
                                    {
                                        if (list[r] != null && TJAPath != list[r].Path)
                                        {
                                            if (list[r].Course[4].IsEnable)
                                            {
                                                if (list[r].Course[3].IsEnable)
                                                {
                                                    RandomCourse = d > 0 ? 4 : 3;
                                                    if (list[r].Course[RandomCourse].IsEnable)
                                                    {
                                                        TJAPath = list[r].Path;
                                                        for (int i = 0; i < 2; i++)
                                                        {
                                                            Course[i] = RandomCourse;
                                                        }
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    TJAPath = list[r].Path;
                                                    for (int i = 0; i < 2; i++)
                                                    {
                                                        Course[i] = 4;
                                                    }
                                                    break;
                                                }
                                            }
                                            else if (list[r].Course[3].IsEnable)
                                            {
                                                TJAPath = list[r].Path;
                                                for (int i = 0; i < 2; i++)
                                                {
                                                    Course[i] = 3;
                                                }
                                                break;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (list[r] != null && list[r].Course[PlayData.Data.PlayCourse[0]].IsEnable && TJAPath != list[r].Path)
                                        {
                                            TJAPath = list[r].Path;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            int count = 0;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i] != null && TJAPath == list[i].Path)
                                {
                                    count = i;
                                    break;
                                }
                            }
                            while (true)
                            {
                                if (count++ >= list.Count - 1) count = 0;
                                TJAPath = list[count].Path;
                                if (list[count].Course[PlayData.Data.PlayCourse[0]].IsEnable) break;
                            }
                        }
                        Reset();
                        if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
                        {
                            int count = 0;
                            for (int i = 4; i >= 0; i--)
                            {
                                if (SongData.NowTJA[count].Courses[i].ListChip.Count > 0)
                                {
                                    Course[count] = i;
                                    count++;
                                }
                            }
                        }

                        if (PlayData.Data.FontRendering)
                        {
                            Title = FontRender.GetTexture(SongData.NowTJA[0].Header.TITLE, 48, 6, PlayData.Data.FontName);
                            SubTitle = FontRender.GetTexture(SongData.NowTJA[0].Header.SUBTITLE, 20, 4, PlayData.Data.FontName);
                        }
                        Notes.SetNotesP();
                        PlayMeasure = 0;
                        StartTime = 0;
                        MeasureList = new List<Chip>();
                        MeasureCount();
                        MainTimer.Value = -2000;
                        MainSong = new Sound($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{SongData.NowTJA[0].Header.WAVE}");
                        MainImage = new Texture($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{SongData.NowTJA[0].Header.BGIMAGE}");
                        string path = $"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{SongData.NowTJA[0].Header.BGMOVIE}";
                        string mp4path = path.Replace("wmv", "mp4");
                        if (PlayData.Data.PlayMovie) MainMovie = new Movie(File.Exists(mp4path) ? mp4path : path);

                        TextLog.Draw($"Now:{SongData.NowTJA[0].TJAPath}", 2000);
                    }
                    else
                    {
                        if (PlayData.Data.ShowResultScreen)
                        {
                            Program.SceneChange(new Result());
                        }
                        if (Key.IsPushed(EKey.Enter) || Key.IsPushed((EKey)PlayData.Data.PlayStart))
                        {
                            PlayMemory.Dispose();
                            Program.SceneChange(new SongSelect());
                        }
                    }
                }
            }
            if (Key.IsPushed(EKey.Esc) && Create.CommandLayer < 2)
            {
                PlayMemory.Dispose();
                Program.SceneChange(new SongSelect());
            }

            if (MainTimer.State != 0)
            {
                TimeRemain = 0;
            }
            else
            {
                if (Math.Abs(TimeRemain) < 1)
                {
                    TimeRemain = 0;
                }
                if (TimeRemain != 0)
                {
                    TimeRemain /= 1.2;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (Math.Abs(ScrollRemain[i]) < 0.0001)
                {
                    ScrollRemain[i] = 0;
                }
                if (ScrollRemain[i] != 0)
                {
                    ScrollRemain[i] /= 1.25;
                }
            }

            if (MeasureList != null)
            {
                for (int i = MeasureList.Count - 1; i >= 0; i--)
                {
                    if (MeasureList[i].Time <= MainTimer.Value)
                    {
                        NowMeasure = i + 1;
                        break;
                    }
                    NowMeasure = 0;
                }
            }
            else NowMeasure = 0;

            if (MainTimer.Value % 2000 <= 10 && MainTimer.Value > 0 && Score.Active.State != 0 && Score.Active.Value < Score.Active.End)
            {
                if (Score.msAverage[0] > 0.5 && PlayData.Data.AutoAdjust[0] && !IsReplay[0])
                {
                    Adjust[0] += 0.5;
                    PlayMemory.AddSetting(0, MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Adjust[0]);
                }
                if (Score.msAverage[0] < -0.5 && PlayData.Data.AutoAdjust[0] && !IsReplay[0])
                {
                    Adjust[0] -= 0.5;
                    PlayMemory.AddSetting(0, MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Adjust[0]);
                }
                if (Score.msAverage[1] > 0.5 && PlayData.Data.AutoAdjust[1] && !IsReplay[1] && Play2P)
                {
                    Adjust[1] += 0.5;
                    PlayMemory.AddSetting(1, MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Adjust[1]);
                }
                if (Score.msAverage[1] < -0.5 && PlayData.Data.AutoAdjust[1] && !IsReplay[1] && Play2P)
                {
                    Adjust[1] -= 0.5;
                    PlayMemory.AddSetting(1, MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Adjust[1]);
                }
            }

            foreach (Scene scene in ChildScene)
                scene?.Update();

            KeyInput.Update(IsAuto[0], IsAuto[1], Failed[0], Failed[1]);

            if (!Play2P && Failed[0])
            {
                switch ((EGaugeAutoShift)PlayData.Data.GaugeAutoShift[0])
                {
                    case EGaugeAutoShift.None:
                        MainSong.Stop();
                        if (MainMovie != null && MainMovie.IsEnable) MainMovie.Stop();
                        break;
                    case EGaugeAutoShift.Retry:
                        Reset();
                        break;
                }
            }

            base.Update();
        }

        public static void AddChildScene(Scene scene)
        {
            scene?.Enable();
            ChildScene.Add(scene);
        }

        public static Counter MainTimer;
        public static Sound MainSong;
        public static Texture MainImage, Title, SubTitle, Lyric;
        public static Movie MainMovie;
        public static List<Scene> ChildScene = new List<Scene>();
        public static string TJAPath, lyric;
        public static bool IsSongPlay, Play2P;
        public static bool[] IsAuto = new bool[2], Failed = new bool[2], IsReplay = new bool[2];
        public static int[] Course = new int[5];
        public static double[] Adjust = new double[5], ScrollRemain = new double[5];
        public static int PlayMeasure, RandomCourse, NowMeasure;
        public static double StartTime, TimeRemain;
        public static List<Chip> MeasureList = new List<Chip>();
        public static Counter[] PushedTimer = new Counter[2], PushingTimer = new Counter[2], SuddenTimer = new Counter[2];
        public static Counter[][] HitTimer = new Counter[5][];
        public static Handle LyricHandle;
    }
}
