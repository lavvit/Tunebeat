using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using SeaDrop;
using TJAParse;
using Tunebeat.Common;
using Tunebeat.Config;
using Tunebeat.SongSelect;

namespace Tunebeat.Game
{
    public class Game : Scene
    {
        public override void Enable()
        {
            TJAPath = SongSelect.SongSelect.PlayMode > 0 ? $@"{SongSelect.SongSelect.FolderName}\{SongSelect.SongSelect.FileName}.tja" : SongSelect.SongSelect.NowTJA.Path;
            if (!File.Exists(TJAPath))
            {
                if (!Directory.Exists(SongSelect.SongSelect.FolderName))
                {
                    Directory.CreateDirectory(SongSelect.SongSelect.FolderName);
                }
                using (StreamWriter streamWriter = new StreamWriter(TJAPath, false, Encoding.GetEncoding("Shift_JIS")))
                {
                    streamWriter.WriteLine($"TITLE:{SongSelect.SongSelect.FileName}");
                    streamWriter.WriteLine("SUBTITLE:--");
                    streamWriter.WriteLine("BPM:120");
                    streamWriter.WriteLine($"WAVE:{SongSelect.SongSelect.FileName}.ogg");
                    streamWriter.WriteLine("OFFSET:-0");
                    streamWriter.WriteLine("SONGVOL:100");
                    streamWriter.WriteLine("SEVOL:100");
                    streamWriter.WriteLine("DEMOSTART:0");
                    streamWriter.WriteLine($"COURSE:{SongSelect.SongSelect.NowTJA.Course}");
                    streamWriter.WriteLine("LEVEL:0");
                    streamWriter.WriteLine("#START");
                    streamWriter.WriteLine("#END");
                }
            }
            MainTimer = new Counter(-2000, int.MaxValue, 1000, false);
            if (PlayData.Data.PreviewType == 3)
            {
                MainTJA = new TJAParse.TJAParse[5];
                for (int i = 0; i < 2; i++)
                {
                    MainTJA[i] = new TJAParse.TJAParse(TJAPath, PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
                for (int i = 2; i < 5; i++)
                {
                    MainTJA[i] = new TJAParse.TJAParse(TJAPath, PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
                }
            }
            else
            {
                MainTJA = new TJAParse.TJAParse[2];
                for (int i = 0; i < 2; i++)
                {
                    MainTJA[i] = new TJAParse.TJAParse(TJAPath, PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
            }
            MainSong = new Sound($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.WAVE}");
            MainImage = new Texture($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGIMAGE}");
            MainMovie = new Movie($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGMOVIE}");

            if (PlayData.Data.FontRendering)
            {
                string font = !string.IsNullOrEmpty(PlayData.Data.FontName) ? PlayData.Data.FontName : "MS UI Gothic";
                Title = FontRender.GetTexture(MainTJA[0].Header.TITLE, 48, font, 8);
                SubTitle = FontRender.GetTexture(MainTJA[0].Header.SUBTITLE, 20, font, 8);
            }

            RandomCourse = SongSelect.SongSelect.Course;
            for (int i = 0; i < 2; i++)
            {
                IsAuto[i] = PlayData.Data.PreviewType == 3 ? true : PlayData.Data.Auto[i];
                IsReplay[i] = SongSelect.SongSelect.Replay[i] && !string.IsNullOrEmpty(SongSelect.SongSelect.ReplayScore[i]) ? true : false;
                Course[i] = SongSelect.SongSelect.Random && PlayData.Data.PlayCourse[i] == 4 ? RandomCourse : SongSelect.SongSelect.EnableCourse(SongSelect.SongSelect.NowTJA.Course, i);
                Failed[i] = false;
                ProcessNote.BalloonList[i] = 0;
                PushedTimer[i] = new Counter(0, 499, 1000, false);
                PushingTimer[i] = new Counter(0, 99, 1000, false);
                SuddenTimer[i] = new Counter(0, 499, 1000, false);
                Adjust[i] = !PlayData.Data.Auto[i] ? PlayData.Data.InputAdjust[i] : 0;

                if (IsReplay[i]) IsAuto[i] = false;
            }
            Adjust[2] = Adjust[3] = PlayData.Data.InputAdjust[0];
            Play2P = MainTJA[1].Courses[Course[1]].ListChip.Count > 0 ? PlayData.Data.IsPlay2P : false;

            if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
            {
                int count = 0;
                for (int i = 4; i >= 0; i--)
                {
                    if (MainTJA[count].Courses[i].ListChip.Count > 0)
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

            PlayMemory.Init();

            for (int i = 0; i < 5; i++)
            {
                SoundLoad.Don[i].Volume = (PlayData.Data.SE / 100.0) * (MainTJA[0].Header.SEVOL / 100.0);
                SoundLoad.DON[i].Volume = (PlayData.Data.SE / 100.0) * (MainTJA[0].Header.SEVOL / 100.0);
                SoundLoad.Ka[i].Volume = (PlayData.Data.SE / 100.0) * (MainTJA[0].Header.SEVOL / 100.0);
                SoundLoad.KA[i].Volume = (PlayData.Data.SE / 100.0) * (MainTJA[0].Header.SEVOL / 100.0);
                SoundLoad.Balloon[i].Volume = (PlayData.Data.SE / 100.0) * (MainTJA[0].Header.SEVOL / 100.0);
                SoundLoad.Kusudama[i].Volume = (PlayData.Data.SE / 100.0) * (MainTJA[0].Header.SEVOL / 100.0);
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
            MainMovie.Dispose();
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
            for (int i = 0; i < MainTJA[0].Courses[Course[0]].ListChip.Count; i++)
            {
                if (MainTJA[0].Courses[Course[0]].ListChip[i].EChip == EChip.Measure)
                {
                    MeasureList.Add(MainTJA[0].Courses[Course[0]].ListChip[i]);
                }
            }
        }

        public static void SetBalloon()
        {
            for (int player = 0; player < 2; player++)
            {
                int amount = 0;
                foreach (Chip chip in MainTJA[player].Courses[Course[player]].ListChip)
                {
                    if (chip.ENote == ENote.Balloon || chip.ENote == ENote.Kusudama)
                    {
                        if (chip.RollEnd != null && chip.RollEnd.Time < StartTime)
                        {
                            amount++;
                        }
                    }
                }
                ProcessNote.BalloonList[player] = amount; 
            }
        }

        public static void Reset()
        {
            MainTimer.Stop();
            MainSong.Stop();
            MainMovie.Stop();
            if (PlayMeasure == 0) MainTimer.Reset();
            else MainTimer.Value = (int)StartTime;
            IsSongPlay = false;
            for (int i = 0; i < 2; i++)
            {
                Failed[i] = false;
            }
            if (PlayData.Data.PreviewType == 3)
            {
                MainTJA = new TJAParse.TJAParse[5];
                for (int i = 0; i < 2; i++)
                {
                    MainTJA[i] = new TJAParse.TJAParse(TJAPath, PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
                for (int i = 2; i < 5; i++)
                {
                    MainTJA[i] = new TJAParse.TJAParse(TJAPath, PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
                }
            }
            else
            {
                MainTJA = new TJAParse.TJAParse[2];
                for (int i = 0; i < 2; i++)
                {
                    MainTJA[i] = new TJAParse.TJAParse(TJAPath, PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
            }
            if (File.Exists(MainSong.FileName)) MainSong.Time = StartTime / 1000;
            if (PlayData.Data.PlayMovie && File.Exists(MainMovie.FileName)) MainMovie.Time = StartTime;

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
            SetBalloon();

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
                if (PlayData.Data.PlayMovie && File.Exists(MainMovie.FileName)) MainMovie.Time = 0;
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
                        if (PlayData.Data.PlayMovie && MainMovie.IsEnable) MainMovie.Time = Math.Ceiling(MeasureList[i].Time);
                        break;
                    }
                }
            }

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
                if (PlayData.Data.PlayMovie && MainMovie.IsEnable) MainMovie.Time = Math.Ceiling(MeasureList[MeasureList.Count - 1].Time);
                SetBalloon();
            }
            else if (PlayMeasure < MeasureList.Count)
            {
                PlayMeasure++;
                TimeRemain += MainTimer.Value - (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                StartTime = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                MainTimer.Value = (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                if (PlayData.Data.PlayMovie && MainMovie.IsEnable) MainMovie.Time = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                SetBalloon();
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
                if (PlayData.Data.PlayMovie && File.Exists(MainMovie.FileName)) MainMovie.Time = 0;
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
                        foreach (Chip chip in MainTJA[i].Courses[Course[i]].ListChip)
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
                SetBalloon();
            }
            else if (PlayMeasure > 1)
            {
                PlayMeasure--;
                TimeRemain += MainTimer.Value - (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                StartTime = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                MainTimer.Value = (int)Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                if (PlayData.Data.PlayMovie && File.Exists(MainMovie.FileName)) MainMovie.Time = Math.Ceiling(MeasureList[PlayMeasure - 1].Time);
                for (int i = 0; i < 2; i++)
                {
                    foreach (Chip chip in MainTJA[i].Courses[Course[i]].ListChip)
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
                SetBalloon();
            }
        }

        public override void Draw()
        {
            if (PlayData.Data.PlayMovie && File.Exists(MainMovie.FileName))
            {
                MainMovie.Volume = 0;
                MainMovie.Draw(0, 0);
            }
            else if (PlayData.Data.ShowImage && File.Exists($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGIMAGE}"))
            {
                MainImage.Draw(960 - (MainImage.TextureSize.Width / 2), 960 - (MainImage.TextureSize.Height / 2));
            }
            else
            {
                DrawBox(0, 0, 1919, 1079, GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]), TRUE);
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
                    if (MainTJA[i].Courses[Course[i]].ListChip.Count > 0)
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
                DrawString(720, 356, $"{PlayMeasure}/{MeasureList.Count}", 0xffffff);
                DrawString(720, 376, $"PRESS {((EKey)PlayData.Data.PlayStart).ToString().ToUpper()} KEY", 0xffffff);
            }

            if ((PlayData.Data.PreviewType >= (int)EPreviewType.Down) || (PlayData.Data.PreviewType == (int)EPreviewType.Normal && !(Play2P || PlayData.Data.ShowGraph)))
            {
                if (PlayData.Data.FontRendering)
                {
                    Title.Draw(1920 - Title.ActualSize.Width - 20, 40);
                    SubTitle.Draw(1920 - SubTitle.ActualSize.Width - 20, 90);
                }
                else
                {
                    DrawString(1920 - GetDrawStringWidth(MainTJA[0].Header.TITLE, MainTJA[0].Header.TITLE.Length) - 20, 40, MainTJA[0].Header.TITLE, 0xffffff);
                    DrawString(1920 - GetDrawStringWidth(MainTJA[0].Header.SUBTITLE, MainTJA[0].Header.SUBTITLE.Length) - 20, 90, MainTJA[0].Header.SUBTITLE, 0xffffff);
                }
            }
            else 
            {
                if (PlayData.Data.FontRendering)
                {
                    Title.Draw(1920 - Title.ActualSize.Width - 20, 990);
                    SubTitle.Draw(1920 - SubTitle.ActualSize.Width - 20, 1040);
                }
                else
                {
                    DrawString(1920 - GetDrawStringWidth(MainTJA[0].Header.TITLE, MainTJA[0].Header.TITLE.Length) - 20, 990, MainTJA[0].Header.TITLE, 0xffffff);
                    DrawString(1920 - GetDrawStringWidth(MainTJA[0].Header.SUBTITLE, MainTJA[0].Header.SUBTITLE.Length) - 20, 1040, MainTJA[0].Header.SUBTITLE, 0xffffff);
                }
            }

            if (PlayData.Data.PreviewType == 3)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (MainTJA[i].Courses[Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Course[i] == Course[i - 1]) break;
                        count++;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    string Lv = $"{MainTJA[i].Courses[Course[i]].COURSE} Lv.{MainTJA[i].Courses[Course[i]].LEVEL}";
                    DrawString(413 - GetDrawStringWidth(Lv, Lv.Length) / 2, Notes.NotesP[i].Y + 6, Lv, 0xffffff);
                    if (MainTJA[i].Courses[Course[i]].ScrollType != EScroll.Normal)
                    {
                        DrawString(378, Notes.NotesP[i].Y + 26, $"{MainTJA[i].Courses[Course[i]].ScrollType}", 0xffffff);
                    }
                }
            }
            else
            {
                string Lv1P = $"{MainTJA[0].Courses[Course[0]].COURSE} Lv.{MainTJA[0].Courses[Course[0]].LEVEL}";
                DrawString(413 - GetDrawStringWidth(Lv1P, Lv1P.Length) / 2, Notes.NotesP[0].Y + 6, Lv1P, 0xffffff);
                if (MainTJA[0].Courses[Course[0]].ScrollType != EScroll.Normal)
                {
                    DrawString(378, Notes.NotesP[0].Y + 26, $"{MainTJA[0].Courses[Course[0]].ScrollType}", 0xffffff);
                }
                if (IsAuto[0]) DrawString(258, Notes.NotesP[0].Y + 6, "AUTO PLAY", 0xffffff);
                if (Play2P)
                {
                    string Lv2P = $"{MainTJA[1].Courses[Course[1]].COURSE} Lv.{MainTJA[1].Courses[Course[1]].LEVEL}";
                    DrawString(413 - GetDrawStringWidth(Lv2P, Lv2P.Length) / 2, Notes.NotesP[0].Y + 416, Lv2P, 0xffffff);
                    if (MainTJA[1].Courses[Course[1]].ScrollType != EScroll.Normal)
                    {
                        DrawString(378, Notes.NotesP[0].Y + 436, $"{MainTJA[1].Courses[Course[1]].ScrollType}", 0xffffff);
                    }
                    if (IsAuto[1]) DrawString(258, Notes.NotesP[0].Y + 416, "AUTO PLAY", 0xffffff);
                }
            }

            Chip nchip = GetNotes.GetNowNote(MainTJA[0].Courses[Course[0]].ListChip, MainTimer.Value, true);
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
                    if (!string.IsNullOrEmpty(nchip.Lyric)) DrawString(960 - GetDrawStringWidth(nchip.Lyric, nchip.Lyric.Length) / 2, 1000, nchip.Lyric, 0x0000ff);
                }
            }

            if (Input.IsEnable)
            {
                DrawBox(0, 1040, GetDrawStringWidth(Input.Text, Input.Text.Length) + 40, 1080, 0x000000, TRUE);
                DrawBox(20 + 9 * Input.Selection.Start, 1040, 20 + 9 * Input.Selection.End, 1080, 0x0000ff, TRUE);
                DrawString(20, 1052, Input.Text, 0xffffff);
                DrawString(16 + GetDrawStringWidth(Input.Text, Input.Position), 1052, "|", 0xffff00);
            }

            #if DEBUG
            DrawString(0, 0, $"{MainTimer.Value}", 0xffffff); if (IsSongPlay && !MainSong.IsPlaying) DrawString(60, 0, "Stoped", 0xffffff);
            DrawString(0, 20, $"{MainTJA[0].Header.TITLE}", 0xffffff);
            DrawString(0, 40, $"{MainTJA[0].Header.SUBTITLE}", 0xffffff);
            DrawString(0, 60, $"{MainTJA[0].Header.BPM}", 0xffffff);

            DrawString(0, 80, $"{MainTJA[0].Courses[Course[0]].COURSE}" + (Play2P ? $"/{MainTJA[0].Courses[Course[1]].COURSE}" : ""), 0xffffff);
            DrawString(0, 100, $"{MainTJA[0].Courses[Course[0]].LEVEL}" + (Play2P ? $"/{MainTJA[0].Courses[Course[1]].LEVEL}" : ""), 0xffffff);
            DrawString(0, 120, $"{MainTJA[0].Courses[Course[0]].TotalNotes}" + (Play2P ? $"/{MainTJA[0].Courses[Course[1]].TotalNotes}" : ""), 0xffffff);
            DrawString(0, 140, $"{MainTJA[0].Courses[Course[0]].ScrollType}" + (Play2P ? $"/{MainTJA[0].Courses[Course[1]].ScrollType}" : ""), 0xffffff);
            DrawString(200, 0, $"{PlayData.Data.AutoRoll}", 0xffffff);
            DrawString(300, 0, $"{PlayMemory.ChipData.Count}", 0xffffff);

            Chip[] chip = new Chip[2] { GetNotes.GetNowNote(MainTJA[0].Courses[Course[0]].ListChip, MainTimer.Value), GetNotes.GetNowNote(MainTJA[0].Courses[Course[1]].ListChip, MainTimer.Value) };
            if (chip[0] != null)
            {
                DrawString(520, 160, $"{chip[0].ENote}", 0xffffff);
                DrawString(520, 180, $"{chip[0].Time}", 0xffffff);
                DrawString(520, 200, $"{ProcessNote.RollState(chip[0])}", 0xffffff);
                DrawString(520, 220, $"{chip[0].RollCount}", 0xffffff);
                DrawString(520, 240, $"{ProcessNote.BalloonRemain[0]}", 0xffffff);
                DrawString(520, 260, $"{ProcessNote.BalloonList[0]}", 0xffffff);
            }
            if (Play2P && chip[1] != null)
            {
                DrawString(520, 780, $"{chip[1].ENote}", 0xffffff);
                DrawString(520, 800, $"{chip[1].Time}", 0xffffff);
                DrawString(520, 820, $"{ProcessNote.RollState(chip[1])}", 0xffffff);
                DrawString(520, 840, $"{chip[1].RollCount}", 0xffffff);
                DrawString(520, 860, $"{ProcessNote.BalloonRemain[1]}", 0xffffff);
                DrawString(520, 880, $"{ProcessNote.BalloonList[1]}", 0xffffff);
            }

            DrawString(700, 160, $"NowAdjust:{Adjust[0]}", 0xffffff);
            DrawString(700, 180, $"Average:{Score.msAverage[0]}", 0xffffff);
            if (Play2P && chip[1] != null)
            {
                DrawString(700, 780, $"NowAdjust:{Adjust[1]}", 0xffffff);
                DrawString(700, 800, $"Average:{Score.msAverage[1]}", 0xffffff);
            }
            DrawString(720, 320, $"{NowMeasure}/{MeasureList.Count}", 0xffffff);

            if (IsSongPlay && !MainSong.IsPlaying) DrawString(0, 160, "PRESS ENTER", 0xffffff);
            #endif

            base.Draw();
        }

        public static void GenerateLyric(Chip chip)
        {
            string font = !string.IsNullOrEmpty(PlayData.Data.FontName) ? PlayData.Data.FontName : "MS UI Gothic";
            Lyric = FontRender.GetTexture(chip.Lyric, 56, font, 4, 0xffffff, 0x0000ff);
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
                    MainSong.PlaySpeed = PlayData.Data.PlaySpeed;
                    MainSong.Volume = (PlayData.Data.GameBGM / 100.0) * (MainTJA[0].Header.SONGVOL / 100.0);
                }
                IsSongPlay = true;
                if (PlayData.Data.PlayMovie && MainMovie.IsEnable)
                {
                    MainMovie.Play(PlayMeasure == 0 ? true : false);
                }
            }
            if (IsSongPlay && !MainSong.IsPlaying)
            {
                if (PlayData.Data.SaveScore && !Play2P &&  PlayMeasure == 0 && MainTimer.State != 0) PlayMemory.SaveScore(0, Course[0]);
                MainTimer.Stop();
                MainMovie.Stop();
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
                        if (SongSelect.SongSelect.Random)
                        {
                            for (int i = 0; i < 100000000; i++)
                            {
                                Random random = new Random();
                                int r = random.Next(SongLoad.SongList.Count);
                                if (PlayData.Data.PlayCourse[0] == (int)ECourse.Edit)
                                {
                                    Random difran = new Random();
                                    int d = difran.Next(0, 2);
                                    RandomCourse = 3 + d;
                                    if (SongLoad.SongList[r] != null && SongLoad.SongList[r].Course[RandomCourse].IsEnable && TJAPath != SongLoad.SongList[r].Path)
                                    {
                                        TJAPath = SongLoad.SongList[r].Path;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (SongLoad.SongList[r] != null && SongLoad.SongList[r].Course[PlayData.Data.PlayCourse[0]].IsEnable && TJAPath != SongLoad.SongList[r].Path)
                                    {
                                        TJAPath = SongLoad.SongList[r].Path;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < SongLoad.SongList.Count; i++)
                            {
                                if (SongLoad.SongList[i] != null && TJAPath == SongLoad.SongList[i].Path)
                                {
                                    TJAPath = SongLoad.SongList[i == SongLoad.SongList.Count - 1 ? 0 : i + 1].Path;
                                    break;
                                }
                            }
                        }
                        Reset();
                        if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
                        {
                            int count = 0;
                            for (int i = 4; i >= 0; i--)
                            {
                                if (MainTJA[count].Courses[i].ListChip.Count > 0)
                                {
                                    Course[count] = i;
                                    count++;
                                }
                            }
                        }

                        if (PlayData.Data.FontRendering)
                        {
                            string font = !string.IsNullOrEmpty(PlayData.Data.FontName) ? PlayData.Data.FontName : "MS UI Gothic";
                            Title = FontRender.GetTexture(MainTJA[0].Header.TITLE, 48, font, 8);
                            SubTitle = FontRender.GetTexture(MainTJA[0].Header.SUBTITLE, 20, font, 8);
                        }
                        Notes.SetNotesP();
                        PlayMeasure = 0;
                        StartTime = 0;
                        MeasureList = new List<Chip>();
                        MeasureCount();
                        MainTimer.Value = -2000;
                        MainSong = new Sound($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.WAVE}");
                        MainImage = new Texture($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGIMAGE}");
                        MainMovie = new Movie($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGMOVIE}");

                        TextLog.Draw($"Now:{MainTJA[0].TJAPath}", 2000);
                    }
                    else
                    {
                        if (PlayData.Data.ShowResultScreen)
                        {
                            Program.SceneChange(new Result.Result());
                        }
                        if (Key.IsPushed(EKey.Enter) || Key.IsPushed((EKey)PlayData.Data.PlayStart))
                        {
                            PlayMemory.Dispose();
                            Program.SceneChange(new SongSelect.SongSelect());
                        }
                    }
                }
            }
            if (Key.IsPushed(EKey.Esc) && Create.CommandLayer < 2)
            {
                PlayMemory.Dispose();
                Program.SceneChange(new SongSelect.SongSelect());
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
                        MainMovie.Stop();
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

        public static TJAParse.TJAParse[] MainTJA;
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
    }
}
