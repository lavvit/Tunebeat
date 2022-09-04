using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class NewGame : Scene
    {
        #region Function
        public static string Path;
        public static TJA[] TJA;
        public static DanCourse Dan;
        public static int[] Course = new int[2];
        public static Course[] NowCourse;
        public static List<Chip>[] Chips;
        public static List<Bar>[] Bars;
        public static Counter Timer, FixWait;
        public static int StartMeasure, RandomCourse;
        public static (int, int) StartDan;
        public static int[] AllSongCourse = new int[5];
        public static bool Play2P;
        public static bool[] Failed = new bool[2];
        public static double TimeRemain;
        public static double[] AutoTime = new double[2];
        public static double[] Adjust = new double[5], ScrollRemain = new double[5];
        public static string NowLyric;
        public static EState NowState;
        public static EAuto[] Playmode = new EAuto[2];
        public static Texture Title, SubTitle, Lyric;
        public static Counter[][] HitTimer = new Counter[5][];
        public static Handle LyricHandle, TitleHandle, SubTitleHandle;
        public static Number GameNumber, SmallNumber;
        #endregion

        public override void Enable()
        {
            Timer = new Counter(-2000, int.MaxValue, 1000, false);
            Path = SongSelect.PlayMode > 0 ? $@"{SongSelect.FolderName}\{SongSelect.FileName}" : SongData.NowSong.Path;
            if (!File.Exists(Path) && SongSelect.RandomDan == null)
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
                    "",
                    "#START",
                    "#END"
                };
                ConfigIni ini = new ConfigIni();
                ini.AddList(tja);
                ini.SaveConfig(Path);
            }

            for (int i = 0; i < 2; i++)
            {
                Playmode[i] = EAuto.Normal;
                if (PlayData.Data.Auto[i] || PlayData.Data.PreviewType == 3) Playmode[i] = EAuto.Auto;
                if (SongSelect.Replay[i] && !string.IsNullOrEmpty(SongSelect.ReplayScore[i])) Playmode[i] = EAuto.Replay;
                Course[i] = SongSelect.Random ? SongSelect.Course : SongSelect.EnableCourse(SongData.NowSong, i);
                Failed[i] = false;
                Adjust[i] = !PlayData.Data.Auto[i] ? PlayData.Data.InputAdjust[i] : 0;
            }
            Adjust[2] = Adjust[3] = PlayData.Data.InputAdjust[0];
            if (PlayData.Data.PreviewType == 3)
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

            StartMeasure = 0;
            TJAInit();
            Play2P = Dan == null && Chips[1] != null && Chips[1].Count > 0 ? PlayData.Data.IsPlay2P : false;
            Memory.SetColor();
            Init();
            NewNotes.Init();
            NewScore.Init();
            
            
            NewCreate.Init();

            LyricHandle = new Handle(PlayData.Data.FontName, 48);
            TitleHandle = new Handle(PlayData.Data.FontName, 48);
            STNumber[] stNumber = new STNumber[13]
            { new STNumber(){ ch = '0', X = 0 },new STNumber(){ ch = '1', X = 26 },new STNumber(){ ch = '2', X = 26 * 2 },new STNumber(){ ch = '3', X = 26 * 3 },new STNumber(){ ch = '4', X = 26 * 4 },
                new STNumber(){ ch = '5', X = 26 * 5 },new STNumber(){ ch = '6', X = 26 * 6 },new STNumber(){ ch = '7', X = 26 * 7 },new STNumber(){ ch = '8', X = 26 * 8 },new STNumber(){ ch = '9', X = 26 * 9 },
                new STNumber(){ ch = '.', X = 26 * 10 },new STNumber(){ ch = '%', X = 26 * 11 },new STNumber(){ ch = '-', X = 26 * 12 } };
            STNumber[] stMiniNumber = new STNumber[13]
            { new STNumber(){ ch = '0', X = 0 },new STNumber(){ ch = '1', X = 18 },new STNumber(){ ch = '2', X = 18 * 2 },new STNumber(){ ch = '3', X = 18 * 3 },new STNumber(){ ch = '4', X = 18 * 4 },
                new STNumber(){ ch = '5', X = 18 * 5 },new STNumber(){ ch = '6', X = 18 * 6 },new STNumber(){ ch = '7', X = 18 * 7 },new STNumber(){ ch = '8', X = 18 * 8 },new STNumber(){ ch = '9', X = 18 * 9 },
                new STNumber(){ ch = '.', X = 18 * 10 },new STNumber(){ ch = '+', X = 18 * 11 },new STNumber(){ ch = '-', X = 18 * 12 } };
            GameNumber = new Number(Tx.Game_Number, 26, 28, stNumber, -2);
            SmallNumber = new Number(Tx.Game_Number_Mini, 18, 18, stMiniNumber);

            // Discord Presenceの更新
            //Discord.UpdatePresence(Properties.Discord.Game + (IsAuto[0] ? $"({Properties.Discord.State_Auto})" : "") + (IsAuto[0] ? $"({Properties.Discord.State_Auto})" : ""),
            //    $"{SongData.NowTJA[0].Header.TITLE}:{PlayMemory.NowClear(0)} {Score.EXScore[0]}", Discord.GetUnixTime() + (long)SongData.NowTJA[0].Courses[Course[0]].ListChip[SongData.NowTJA[0].Courses[Course[0]].ListChip.Count].Time / 1000);

            double volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            for (int i = 0; i < 5; i++)
            {
                Sfx.Don[i].Volume = volume;
                Sfx.DON[i].Volume = volume;
                Sfx.Ka[i].Volume = volume;
                Sfx.KA[i].Volume = volume;
                Sfx.Balloon[i].Volume = volume;
                Sfx.Kusudama[i].Volume = volume;
            }

            for (int i = 0; i < 5; i++)
            {
                HitTimer[i] = new Counter[4];
                for (int j = 0; j < 4; j++)
                {
                    HitTimer[i][j] = new Counter(0, 100, 1000, false);
                }
            }
            FixWait = new Counter(0, 100, 1000, false);
            base.Enable();
        }

        public static void Init()
        {
            NowState = EState.None;
            AutoTime[0] = AutoTime[1] = 0;

            for (int i = 0; i < 2; i++)
            {
                Failed[i] = false;
            }

            if (TJA[0].Sound != null && TJA[0].Sound.IsPlaying) TJA[0].Sound.Stop();
            if (TJA[0].Movie != null && TJA[0].Movie.IsPlaying) { TJA[0].Movie.Time = Timer.Value; TJA[0].Movie.Stop(); }

            if (Timer.State != 0) Timer.Stop();
            Timer.Value = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;

            Process.Init();
            NewScore.Reset();
            Memory.Reset();
            Sudden.Init();

            for (int i = 0; i < Chips.Length; i++)
            {
                if (Playmode[i] < EAuto.Replay) TJAParse.Course.RandomizeChip(Chips[i], PlayData.Data.Random[i >= 2 ? 0 : i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i >= 2 ? 0 : i], PlayData.Data.NotesChange[i >= 2 ? 0 : i]);
            }
        }

        public static void TJAInit()
        {
            if (Path.EndsWith("tbd") || SongSelect.RandomDan != null)
            {
                SongData.NowTJA = new TJA[2];
                NowCourse = new Course[2];
                Chips = new List<Chip>[2];
                Bars = new List<Bar>[2];

                DanCourse.SongNumber = 0;
                Dan = SongSelect.RandomDan != null ? SongSelect.RandomDan : new DanCourse(Path);
                DanLoad();
                if (Playmode[0] == EAuto.Replay && Memory.ReplayData != null) DanC.LastChip = new TJA(Dan.Courses[Dan.Courses.Count - 1].Path, Memory.ReplayData.Speed > 0 ? Memory.ReplayData.Speed : 1, 0, false, 0).Courses[Dan.Courses[Dan.Courses.Count - 1].Course].ListChip;
                else DanC.LastChip = new TJA(Dan.Courses[Dan.Courses.Count - 1].Path, PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]).Courses[Dan.Courses[Dan.Courses.Count - 1].Course].ListChip;
            }
            else
            {
                Memory.Init(Path);
                Dan = null;
                if (PlayData.Data.PreviewType == 3)
                {
                    SongData.NowTJA = new TJA[5];
                    for (int i = 0; i < 2; i++)
                    {
                        ReplayData replay = i > 0 ? Memory.ReplayData2P : Memory.ReplayData;
                        SongData.NowTJA[i] = new TJA(Path, Playmode[i] == EAuto.Replay && replay != null ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed,
                            PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                    }
                    for (int i = 2; i < 5; i++)
                    {
                        SongData.NowTJA[i] = new TJA(Path, Playmode[0] == EAuto.Replay && Memory.ReplayData != null ? (Memory.ReplayData.Speed > 0 ? Memory.ReplayData.Speed : 1) : PlayData.Data.PlaySpeed,
                            PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
                    }
                }
                else
                {
                    SongData.NowTJA = new TJA[2];
                    for (int i = 0; i < 2; i++)
                    {
                        ReplayData replay = i > 0 ? Memory.ReplayData2P : Memory.ReplayData;
                        SongData.NowTJA[i] = new TJA(Path, Playmode[i] == EAuto.Replay && replay != null ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed,
                            PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                    }
                }
                TJA = SongData.NowTJA;
                int lane = PlayData.Data.PreviewType == 3 ? 5 : 2;
                NowCourse = new Course[lane];
                Chips = new List<Chip>[lane];
                Bars = new List<Bar>[lane];
                for (int i = 0; i < lane; i++)
                {
                    NowCourse[i] = TJA[i].Courses[Course[i]];
                    Chips[i] = NowCourse[i].ListChip;
                    Bars[i] = NowCourse[i].ListBar;
                }
            }

            if (PlayData.Data.FontRendering)
            {
                Title = FontRender.GetTexture(TJA[0].Header.TITLE, 48, 6, PlayData.Data.FontName);
                SubTitle = FontRender.GetTexture(TJA[0].Header.SUBTITLE, 20, 4, PlayData.Data.FontName);
            }
        }
        public static void DanLoad()
        {
            if (Playmode[0] == EAuto.Replay && Memory.ReplayData != null)
            {
                SongData.NowTJA[0] = SongData.NowTJA[1] = new TJA(Dan.Courses[DanCourse.SongNumber].Path, Memory.ReplayData.Speed > 0 ? Memory.ReplayData.Speed : 1, 0, false, 0);
            }
            else
            {
                SongData.NowTJA[0] = SongData.NowTJA[1] = new TJA(Dan.Courses[DanCourse.SongNumber].Path,
                PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
            }
            TJA = SongData.NowTJA;
            NowCourse[0] = NowCourse[1] = TJA[0].Courses[Dan.Courses[DanCourse.SongNumber].Course];
            Chips[0] = Chips[1] = NowCourse[0].ListChip;
            Bars[0] = Bars[1] = NowCourse[0].ListBar;
            Course[0] = Course[1] = Dan.Courses[DanCourse.SongNumber].Course;

            Memory.Init(Dan.Courses[DanCourse.SongNumber].Path);
        }

        public override void Disable()
        {
            for (int i = 0; i < SongData.NowTJA.Length; i++)
            {
                if (SongData.NowTJA[i].Sound != null) SongData.NowTJA[i].Sound.Dispose();
                if (SongData.NowTJA[i].Image != null) SongData.NowTJA[i].Image.Dispose();
                if (SongData.NowTJA[i].Movie != null) SongData.NowTJA[i].Movie.Dispose();
            }
            
            Timer.Reset();
            NowState = EState.None;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    HitTimer[i][j].Reset();
                }
            }

            Memory.Dispose();

            base.Disable();
        }
        public override void Draw()
        {
            if (PlayData.Data.PlayMovie && TJA[0].Movie != null && TJA[0].Movie.IsEnable)
            {
                TJA[0].Movie.Draw(0, 0);
                Drawing.Text(0, 200, TJA[0].Movie.FileName);
            }
            else if (PlayData.Data.ShowImage && TJA[0].Image != null && TJA[0].Image.IsEnable)
            {
                TJA[0].Image.Draw(960 - (TJA[0].Image.TextureSize.Width / 2), 960 - (TJA[0].Image.TextureSize.Height / 2));
            }
            else
            {
                Drawing.Box(0, 0, 1919, 1079, Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
                Tx.Game_Background.Draw(0, 0);
            }

            NewNotes.Draw();

            double length = TJA[0].Sound != null && TJA[0].Sound.IsEnable && TJA[0].Sound.Frequency.HasValue ? TJA[0].Sound.Length : Chips[0][Chips[0].Count - 1].Time;
            double timer = Timer.Value * PlayData.Data.PlaySpeed;
            double time = timer > length ? length : timer;
            double perlength = time / length;
            Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
            Point Point = NewCreate.Enable ? createpoint[0] : NewNotes.NotesP[0];
            Drawing.Box(0, Point.Y + 195, 1920 * perlength, 4, 0xff6000);

            Chip nowchip = Process.NowChip(Chips[0]);
            if (nowchip != null) Drawing.Text(600, 20, nowchip.Draw());
            Chip nearchip = Process.NearChip(Chips[0]);
            if (nearchip != null) Drawing.Text(600, 40, nearchip.Draw());

            Chip nchip = Process.NowChip(Chips[0]);
            if (nchip != null && nchip.Lyric != null)
            {
                if (PlayData.Data.FontRendering)
                {
                    if (NowLyric != nchip.Lyric)
                    {
                        Lyric = FontRender.GetTexture(nchip.Lyric, 56, 4, Color.White, Color.Blue, PlayData.Data.FontName);
                        NowLyric = nchip.Lyric;
                    }
                    Lyric.Draw(960 - Lyric.ActualSize.Width / 2, 1000);
                }
                else
                {
                    if (!string.IsNullOrEmpty(nchip.Lyric)) Drawing.Text(960 - Drawing.TextWidth(nchip.Lyric, LyricHandle) / 2, 1000, nchip.Lyric, LyricHandle, 0x0000ff);
                }
            }

            Process.Draw();
            NewScore.Draw();
            Sudden.Draw();
            NewCreate.Draw();
            DanC.Draw();

            if (NowState == EState.None)
            {
                Drawing.Text(720, NewNotes.NotesP[0].Y + 70, $"{StartMeasure}/{Bars[0].Count}", 0xffffff);
                if (FixWait.State != 0) Drawing.Text(720, NewNotes.NotesP[0].Y + 90, "Wait a moment...");
                else
                {
                    string key = $"{(EKey)PlayData.Data.PlayStart}";
                    Drawing.Text(720, NewNotes.NotesP[0].Y + 90, $"PRESS {key.ToUpper()} KEY");
                }
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
                    Drawing.Text(1920 - Drawing.TextWidth(TJA[0].Header.TITLE, TJA[0].Header.TITLE.Length) - 20, 40, TJA[0].Header.TITLE, 0xffffff);
                    Drawing.Text(1920 - Drawing.TextWidth(TJA[0].Header.SUBTITLE, TJA[0].Header.SUBTITLE.Length) - 20, 90, TJA[0].Header.SUBTITLE, 0xffffff);
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
                    Drawing.Text(1920 - Drawing.TextWidth(TJA[0].Header.TITLE, TJA[0].Header.TITLE.Length) - 20, 990, TJA[0].Header.TITLE, 0xffffff);
                    Drawing.Text(1920 - Drawing.TextWidth(TJA[0].Header.SUBTITLE, TJA[0].Header.SUBTITLE.Length) - 20, 1040, TJA[0].Header.SUBTITLE, 0xffffff);
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
                    string Lv = $"{NowCourse[i].COURSE} Lv.{NowCourse[i].LEVEL}";
                    Drawing.Text(413 - Drawing.TextWidth(Lv, Lv.Length) / 2, NewNotes.NotesP[i].Y + 6, Lv, 0xffffff);
                    if (NowCourse[i].ScrollType != EScroll.Normal)
                    {
                        Drawing.Text(378, NewNotes.NotesP[i].Y + 26, $"{NowCourse[i].ScrollType}", 0xffffff);
                    }
                }
            }
            else
            {
                string Lv1P = $"{NowCourse[0].COURSE} Lv.{NowCourse[0].LEVEL}";
                Drawing.Text(413 - Drawing.TextWidth(Lv1P) / 2, NewNotes.NotesP[0].Y + 6, Lv1P, 0xffffff);
                if (NowCourse[0].ScrollType != EScroll.Normal)
                {
                    string scr = $"{NowCourse[0].ScrollType}";
                    Drawing.Text(413 - Drawing.TextWidth(scr) / 2, NewNotes.NotesP[0].Y + 26, scr, 0xffffff);
                }
                if (Playmode[0] > EAuto.Normal) Drawing.Text(258, NewNotes.NotesP[0].Y + 6, "AUTO PLAY", 0xffffff);
                if (Play2P)
                {
                    string Lv2P = $"{NowCourse[1].COURSE} Lv.{NowCourse[1].LEVEL}";
                    Drawing.Text(413 - Drawing.TextWidth(Lv2P) / 2, NewNotes.NotesP[0].Y + 416, Lv2P, 0xffffff);
                    if (NowCourse[1].ScrollType != EScroll.Normal)
                    {
                        string scr = $"{NowCourse[1].ScrollType}";
                        Drawing.Text(413 - Drawing.TextWidth(scr) / 2, NewNotes.NotesP[0].Y + 436, scr, 0xffffff);
                    }
                    if (Playmode[1] > EAuto.Normal) Drawing.Text(258, NewNotes.NotesP[0].Y + 416, "AUTO PLAY", 0xffffff);
                }
            }

            TextDebug.Update();

#if DEBUG
            Drawing.Text(0, 0, $"{NowState} {Timer.Value} {TimeRemain}");
            if (Playmode[0] > EAuto.Normal) Drawing.Text(200, 0, $"{Playmode[0]}", 0xff0000);
            if (Playmode[1] > EAuto.Normal) Drawing.Text(280, 0, $"{Playmode[1]}", 0x0000ff);
            if (TJA[0].isEnable)
            {

                Drawing.Text(0, 20, TJA[0].Header.TITLE);
                Drawing.Text(0, 40, TJA[0].Header.SUBTITLE);
                Drawing.Text(0, 60, $"{(ECourse)Course[0]} {NowCourse[0].TotalNotes}Notes");

                if (NowState == EState.None && Bars[0] != null)
                {
                    Drawing.Text(0, 80, $"{StartMeasure}/{Bars[0].Count}");
                }
            }

            /*if (!NewCreate.Enable)
            {
                List<Chip> c = new List<Chip>();
                for (int i = 0; i < Chips[0].Count; i++)
                {
                    if (Chips[0][i] != null && Chips[0][i].Time > Timer.Value && Chips[0][i].ENote > ENote.Space)
                    {
                        c.Add(Chips[0][i]);
                    }
                }
                for (int i = 0; i < c.Count; i++)
                {
                    if (c[i] != null && c[i].Time > Timer.Value && c[i].ENote > ENote.Space)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            if ((i + j) < c.Count) Drawing.Text(0, 500 + 20 * j, ChipData(c[i + j]));
                        }
                        break;
                    }
                }
            }*/
#endif

            base.Draw();
        }

        public override void Update()
        {
            Timer.Tick();
            FixWait.Tick();
            if (NowState == EState.None && Key.IsPushed(EKey.Space))
            {
                Timer.Start();
                NowState = EState.Start;
                Memory.InitSetting(0, NewNotes.Scroll[0], Sudden.SuddenNumber[0], Sudden.UseSudden[0], Adjust[0]);
                if (Play2P) Memory.InitSetting(1, NewNotes.Scroll[1], Sudden.SuddenNumber[1], Sudden.UseSudden[1], Adjust[1]);
            }
            if (NowState == EState.Start && Timer.Value >= 0)
            {
                if (TJA[0].Sound != null)
                {
                    double volume = (PlayData.Data.GameBGM / 100.0) * (SongData.NowTJA[0].Header.SONGVOL / 100.0);
                    TJA[0].Sound.Volume = volume;
                    TJA[0].Sound.PlaySpeed = Playmode[0] == EAuto.Replay && Memory.ReplayData != null ? (Memory.ReplayData.Speed > 0 ? Memory.ReplayData.Speed : 1) : PlayData.Data.PlaySpeed;
                    TJA[0].Sound.Play(StartMeasure > 0 ? Timer.Value : 0);
                }
                if (TJA[0].Movie != null && PlayData.Data.PlayMovie)
                {
                    TJA[0].Movie.PlaySpeed = Playmode[0] == EAuto.Replay && Memory.ReplayData != null ? (Memory.ReplayData.Speed > 0 ? Memory.ReplayData.Speed : 1) : PlayData.Data.PlaySpeed;
                    TJA[0].Movie.PlayGraph(StartMeasure > 0 ? Timer.Value : 0);
                }

                NowState = EState.Play;
            }
            double endtime = TJA[0].Sound != null && TJA[0].Sound.IsEnable ? TJA[0].Sound.Length : Chips[0][Chips[0].Count - 1].Time + 2000;
            if (NowState == EState.Play && Timer.Value > endtime)
            {
                if (Dan != null && DanCourse.SongNumber < Dan.Courses.Count - 1 && (DanC.ExamSuccess(Dan.Exams) != ESuccess.Failed || PlayData.Data.DanFailedType == (int)EDanFailedType.All))
                {
                    Timer.Stop();
                    Timer.Value = -2000;
                    NowState = EState.Start;
                    Memory.SaveScore(0);

                    for (int i = 0; i < Dan.Exams.Count; i++)
                    {
                        DanC.SaveSuccess(Dan.Exams[i]);
                    }
                    for (int i = 2; i < 5; i++)
                    {
                        NewScore.Scores[i] = new ScoreBoard();
                        NewScore.Combo[i] = 0;
                    }

                    DanCourse.SongNumber++;
                    DanLoad();
                    StartMeasure = 0;
                    AutoTime[0] = 0;
                    Timer.Start();
                }
                else NowState = EState.End;
            }

            SmoothMoving();
            AutoAdjust();

            if (Key.IsPushed(EKey.Esc)) Program.SceneChange(new SongSelect());
            if (Key.IsPushed(EKey.Q)) Init();
            if (Key.IsPushed(EKey.F1))
            {
                if (Playmode[0] < EAuto.Replay) Playmode[0] = Playmode[0] == EAuto.Normal ? EAuto.Auto : EAuto.Normal;
                if (Playmode[0] == EAuto.Auto && NowState == EState.Play) AutoTime[0] = Timer.Value;
                if (NowState == EState.None) PlayData.Data.Auto[0] = Playmode[0] == EAuto.Auto;
            }
            if (Key.IsPushed(EKey.F2) && Play2P)
            {
                if (Playmode[1] < EAuto.Replay) Playmode[1] = Playmode[1] == EAuto.Normal ? EAuto.Auto : EAuto.Normal;
                if (Playmode[1] == EAuto.Auto && NowState == EState.Play) AutoTime[1] = Timer.Value;
                if (NowState == EState.None) PlayData.Data.Auto[1] = Playmode[1] == EAuto.Auto;
            }
            if (Key.IsPushed(PlayData.Data.MoveCreate) && Dan == null) NewCreate.Enable = !NewCreate.Enable;

            switch (NowState)
            {
                case EState.None:
                    if (Key.IsHolding(EKey.PgUp, 200, 25)) MeasureUp();
                    if (Key.IsHolding(EKey.PgDn, 200, 25)) MeasureDown();
                    if (Key.IsPushed(EKey.Home)) MeasureHome();
                    if (Key.IsPushed(EKey.End)) MeasureEnd();

                    if (NewCreate.Enable)
                    {
                        int height = (int)(NewCreate.Size * 1.25);
                        int disp = ((1080 - (200 + height * NewCreate.CourseHeader[Course[0]].Count)) / height);
                        if (Key.IsHolding(EKey.PgUp, 200, 25))
                        {
                            NewCreate.Line = StartMeasure <= 1 ? StartMeasure : NewCreate.MeasureList[Course[0]][StartMeasure - 2] + 1;
                            if (NewCreate.Line - NewCreate.DisplayLine >= disp) NewCreate.DisplayLine = NewCreate.Line - disp + 1;
                        }
                        if (Key.IsHolding(EKey.PgDn, 200, 25))
                        {
                            NewCreate.Line = StartMeasure <= 1 ? StartMeasure : NewCreate.MeasureList[Course[0]][StartMeasure - 2] + 1;
                            if (NewCreate.Line - NewCreate.DisplayLine < 0) NewCreate.DisplayLine = NewCreate.Line;
                        }
                        if (Key.IsPushed(EKey.Home))
                        {
                            NewCreate.Line = NewCreate.DisplayLine = 0;
                        }
                        if (Key.IsPushed(EKey.End))
                        {
                            NewCreate.Line = NewCreate.MeasureList[Course[0]][StartMeasure - 2] + 1;
                            NewCreate.DisplayLine = NewCreate.CourseText[Course[0]].Count - disp;
                        }
                    }

                    if (Dan != null) StartDan = (DanCourse.SongNumber, StartMeasure);
                    break;
                case EState.End:
                    Memory.SaveScore(0);
                    if (Key.IsPushed(EKey.F11)) Memory.SaveData(0);
                    break;
            }

            Process.Update();
            NewScore.Update();
            Memory.Update();
            Sudden.Update();
            NewCreate.Update();

            if (Dan != null)
            {
                if (DanC.ExamSuccess(Dan.Exams) == ESuccess.Failed)
                {
                    switch ((EDanFailedType)PlayData.Data.DanFailedType)
                    {
                        case EDanFailedType.End:
                            if (TJA[0].Sound != null && TJA[0].Sound.IsPlaying) TJA[0].Sound.Stop();
                            NowState = EState.End;
                            break;
                        case EDanFailedType.Retry:
                            Init();
                            break;
                    }
                }
            }
            else
            {
                if ((!Play2P && Failed[0]) || Failed[0] && Failed[1])
                {
                    switch ((EGaugeAutoShift)PlayData.Data.GaugeAutoShift[0])
                    {
                        case EGaugeAutoShift.None:
                            if (TJA[0].Sound != null && TJA[0].Sound.IsPlaying) TJA[0].Sound.Stop();
                            NowState = EState.End;
                            break;
                        case EGaugeAutoShift.Retry:
                            Init();
                            break;
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(Path) && File.GetLastWriteTime(Path) > NewCreate.FileTime)
            {
                FixWait.Start();
            }
            if (FixWait.Value == FixWait.End)
            {
                NewCreate.Read();
                Init();
                TJAInit();
                FixWait.Stop();
                FixWait.Reset();
            }
        }

        public static void SmoothMoving()
        {
            if (Math.Abs(TimeRemain) < 1)
            {
                TimeRemain = 0;
            }
            if (TimeRemain != 0)
            {
                TimeRemain /= 1.2;
            }
            for (int i = 0; i < 2; i++)
            {
                if (Math.Abs(ScrollRemain[i]) < 0.0001)
                {
                    ScrollRemain[i] = 0;
                }
                if (ScrollRemain[i] != 0)
                {
                    ScrollRemain[i] /= 1.2;
                }
            }
        }

        public static void AutoAdjust()
        {
            if (Timer.Value % 2000 <= 10 && Timer.Value > 0 && NewScore.Active.State != 0 && NewScore.Active.Value < NewScore.Active.End)
            {
                if (NewScore.msAverage[0] > 0.5 && PlayData.Data.AutoAdjust[0] && Playmode[0] == EAuto.Normal)
                {
                    Adjust[0] += 0.5;
                    Sudden.Add(0);
                }
                if (NewScore.msAverage[0] < -0.5 && PlayData.Data.AutoAdjust[0] && Playmode[0] == EAuto.Normal)
                {
                    Adjust[0] -= 0.5;
                    Sudden.Add(0);
                }
                if (NewScore.msAverage[1] > 0.5 && PlayData.Data.AutoAdjust[1] && Playmode[1] == EAuto.Normal && Play2P)
                {
                    Adjust[1] += 0.5;
                    Sudden.Add(1);
                }
                if (NewScore.msAverage[1] < -0.5 && PlayData.Data.AutoAdjust[1] && Playmode[1] == EAuto.Normal && Play2P)
                {
                    Adjust[1] -= 0.5;
                    Sudden.Add(1);
                }
            }
        }

        public static void MeasureUp()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            int prev = StartMeasure;
            if (StartMeasure++ >= Bars[0].Count) StartMeasure = Bars[0].Count;
            Timer.Value = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            TimeRemain -= Timer.Value - prevalue;
            if (TJA[0].Movie != null) TJA[0].Movie.Time = StartMeasure == 0 ? 0 : Timer.Value;

            if (Dan != null && DanCourse.SongNumber < Dan.Courses.Count - 1 && prev + 1 > Bars[0].Count)
            {
                DanCourse.SongNumber++;
                DanLoad();
                StartMeasure = 0;
                Timer.Value = Timer.Begin;
            }
        }
        public static void MeasureDown()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            int prev = StartMeasure;
            if (StartMeasure-- <= 0) StartMeasure = 0;
            Timer.Value = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            TimeRemain -= Timer.Value - prevalue;
            if (TJA[0].Movie != null) TJA[0].Movie.Time = StartMeasure == 0 ? 0 : Timer.Value;

            if (Dan != null && DanCourse.SongNumber > 0 && prev - 1 < 0)
            {
                DanCourse.SongNumber--;
                DanLoad();
                StartMeasure = Bars[0].Count;
                Timer.Value = (long)Bars[0][Bars[0].Count - 1].Time;
            }
        }
        public static void MeasureHome()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            StartMeasure = 0;
            Timer.Value = Timer.Begin;
            TimeRemain -= Timer.Value - prevalue;
            if (TJA[0].Movie != null) TJA[0].Movie.Time = StartMeasure == 0 ? 0 : Timer.Value;
        }
        public static void MeasureEnd()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            StartMeasure = Bars[0].Count;
            Timer.Value = (long)Bars[0][Bars[0].Count - 1].Time;
            TimeRemain -= Timer.Value - prevalue;
            if (TJA[0].Movie != null) TJA[0].Movie.Time = StartMeasure == 0 ? 0 : Timer.Value;
        }
    }

    public enum EState
    {
        None,
        Start,
        Play,
        End
    }
    public enum EAuto
    {
        Normal,
        Auto,
        Replay
    }

    public class Game : Scene
    {
        public override void Enable()
        {
            Path = SongSelect.PlayMode > 0 ? $@"{SongSelect.FolderName}\{SongSelect.FileName}.tja" : SongData.NowSong.Path;
            if (!File.Exists(Path))
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
                ini.SaveConfig(Path);
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
                    SongData.NowTJA[i] = new TJA(Path, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
                for (int i = 2; i < 5; i++)
                {
                    SongData.NowTJA[i] = new TJA(Path, IsReplay[0] ? (PlayMemory.ReplayData.Speed > 0 ? PlayMemory.ReplayData.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
                }
            }
            else
            {
                SongData.NowTJA = new TJA[2];
                for (int i = 0; i < 2; i++)
                {
                    ReplayData replay = i > 0 ? PlayMemory.ReplayData2P : PlayMemory.ReplayData;
                    SongData.NowTJA[i] = new TJA(Path, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
            }

            MainSong = new Sound($"{System.IO.Path.GetDirectoryName(SongData.NowTJA[0].Path)}/{SongData.NowTJA[0].Header.WAVE}");
            MainImage = new Texture($"{System.IO.Path.GetDirectoryName(SongData.NowTJA[0].Path)}/{SongData.NowTJA[0].Header.BGIMAGE}");
            string path = $"{System.IO.Path.GetDirectoryName(SongData.NowTJA[0].Path)}/{SongData.NowTJA[0].Header.BGMOVIE}";
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

            // Discord Presenceの更新
            //Discord.UpdatePresence(Properties.Discord.Game + (IsAuto[0] ? $"({Properties.Discord.State_Auto})" : "") + (IsAuto[0] ? $"({Properties.Discord.State_Auto})" : ""),
            //    $"{SongData.NowTJA[0].Header.TITLE}:{PlayMemory.NowClear(0)} {Score.EXScore[0]}", Discord.GetUnixTime() + (long)SongData.NowTJA[0].Courses[Course[0]].ListChip[SongData.NowTJA[0].Courses[Course[0]].ListChip.Count].Time / 1000);

            double volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            for (int i = 0; i < 5; i++)
            {
                Sfx.Don[i].Volume = volume;
                Sfx.DON[i].Volume = volume;
                Sfx.Ka[i].Volume = volume;
                Sfx.KA[i].Volume = volume;
                Sfx.Balloon[i].Volume = volume;
                Sfx.Kusudama[i].Volume = volume;
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
                //if (SongData.NowTJA[0].Courses[Course[0]].ListChip[i].EChip == EChip.Measure)
                {
                    //MeasureList.Add(SongData.NowTJA[0].Courses[Course[0]].ListChip[i]);
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
                    SongData.NowTJA[i] = new TJA(Path, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
                }
                for (int i = 2; i < 5; i++)
                {
                    SongData.NowTJA[i] = new TJA(Path, IsReplay[0] ? (PlayMemory.ReplayData.Speed > 0 ? PlayMemory.ReplayData.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[0] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[0], PlayData.Data.NotesChange[0]);
                }
            }
            else
            {
                SongData.NowTJA = new TJA[2];
                for (int i = 0; i < 2; i++)
                {
                    ReplayData replay = i > 0 ? PlayMemory.ReplayData2P : PlayMemory.ReplayData;
                    SongData.NowTJA[i] = new TJA(Path, IsReplay[i] ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed, PlayData.Data.Random[i] ? PlayData.Data.RandomRate : 0, PlayData.Data.Mirror[i], PlayData.Data.NotesChange[i]);
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
                Create.Save(Path);
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
            else if (PlayData.Data.ShowImage && File.Exists($"{System.IO.Path.GetDirectoryName(SongData.NowTJA[0].Path)}/{SongData.NowTJA[0].Header.BGIMAGE}"))
            {
                MainImage.Draw(960 - (MainImage.TextureSize.Width / 2), 960 - (MainImage.TextureSize.Height / 2));
            }
            else
            {
                Drawing.Box(0, 0, 1919, 1079, Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
                Tx.Game_Background.Draw(0, 0);
            }


            foreach (Scene scene in ChildScene)
                scene?.Draw();

            for (int i = 0; i < 5; i++)
            {
                if (HitTimer[i][0].State == TimerState.Started)
                {
                    Tx.Game_Don[i][0].Opacity = 1.0 - ((double)HitTimer[i][0].Value / HitTimer[i][0].End);
                    Tx.Game_Don[i][0].Draw(362, Notes.NotesP[i].Y + 47);
                }
                if (HitTimer[i][1].State == TimerState.Started)
                {
                    Tx.Game_Don[i][1].Opacity = 1.0 - ((double)HitTimer[i][1].Value / HitTimer[i][1].End);
                    Tx.Game_Don[i][1].Draw(362, Notes.NotesP[i].Y + 47);
                }
                if (HitTimer[i][2].State == TimerState.Started)
                {
                    Tx.Game_Ka[i][0].Opacity = 1.0 - ((double)HitTimer[i][2].Value / HitTimer[i][2].End);
                    Tx.Game_Ka[i][0].Draw(362, Notes.NotesP[i].Y + 47);
                }
                if (HitTimer[i][3].State == TimerState.Started)
                {
                    Tx.Game_Ka[i][1].Opacity = 1.0 - ((double)HitTimer[i][3].Value / HitTimer[i][3].End);
                    Tx.Game_Ka[i][1].Draw(362, Notes.NotesP[i].Y + 47);
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
                        Create.Save(Path);
                    }
                    Reset();
                }
                else
                {
                    if (PlayData.Data.PlayList)
                    {
                        List<Song> list = SongData.FolderFloor > 0 ? SongData.FolderSong : SongData.AllSong;
                        string path;
                        if (SongSelect.Random)
                        {
                            path = RandomSongPath(list);
                        }
                        else
                        {
                            path = NextSongPath(list);
                        }
                        PlayNextSong(path);

                        TextLog.Draw($"Now:{SongData.NowTJA[0].Path}", 2000);
                    }
                    else
                    {
                        // Discord Presenceの更新
                        //Discord.UpdatePresence(Properties.Discord.Result + (IsAuto[0] ? $"({Properties.Discord.State_Auto})" : "") + (IsAuto[0] ? $"({Properties.Discord.State_Auto})" : ""),
                        //    $"{SongData.NowTJA[0].Header.TITLE}:{PlayMemory.NowClear(0)} {Score.EXScore[0]}", Program.StartupTime);

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

            SmoothMoving();
            NowMeasureCount();
            AutoAdjust();

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

        public static string RandomSongPath(List<Song> list)
        {
            Random random = new Random();
            while (true)
            {
                int r = random.Next(list.Count);
                int d = random.Next(0, 3);
                if (SongData.NowSong.DisplayDif > 0)
                {
                    if (list[r] != null && list[r].DisplayDif - 1 <= PlayData.Data.PlayCourse[0])
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Course[i] = list[r].DisplayDif - 1;
                        }
                        return list[r].Path;
                    }
                }
                else
                {
                    if (PlayData.Data.PlayCourse[0] == (int)ECourse.Edit)
                    {
                        if (list[r] != null && Path != list[r].Path)
                        {
                            if (list[r].Course[4].IsEnable)
                            {
                                if (list[r].Course[3].IsEnable)
                                {
                                    RandomCourse = d > 0 ? 4 : 3;
                                    if (list[r].Course[RandomCourse].IsEnable)
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            Course[i] = RandomCourse;
                                        }
                                        return list[r].Path;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Course[i] = 4;
                                    }
                                    return list[r].Path;
                                }
                            }
                            else if (list[r].Course[3].IsEnable)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Course[i] = 3;
                                }
                                return list[r].Path;
                            }
                        }

                    }
                    else
                    {
                        if (list[r] != null && list[r].Course[PlayData.Data.PlayCourse[0]].IsEnable && Path != list[r].Path)
                        {
                            return list[r].Path;
                        }
                    }
                }
            }
        }
        public static string NextSongPath(List<Song> list)
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && Path == list[i].Path)
                {
                    count = i;
                    break;
                }
            }
            while (true)
            {
                if (count++ >= list.Count - 1) count = 0;
                if (list[count].Course[PlayData.Data.PlayCourse[0]].IsEnable) return list[count].Path;
            }
        }

        public static void PlayNextSong(string tjapath = null)
        {
            Path = tjapath;
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
            MainSong = new Sound($"{System.IO.Path.GetDirectoryName(SongData.NowTJA[0].Path)}/{SongData.NowTJA[0].Header.WAVE}");
            MainImage = new Texture($"{System.IO.Path.GetDirectoryName(SongData.NowTJA[0].Path)}/{SongData.NowTJA[0].Header.BGIMAGE}");
            string path = $"{System.IO.Path.GetDirectoryName(SongData.NowTJA[0].Path)}/{SongData.NowTJA[0].Header.BGMOVIE}";
            string mp4path = path.Replace("wmv", "mp4");
            if (PlayData.Data.PlayMovie) MainMovie = new Movie(File.Exists(mp4path) ? mp4path : path);
        }
        public static void SmoothMoving()
        {
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
        }
        public static void NowMeasureCount()
        {
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
        }

        public static void AutoAdjust()
        {
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
        public static string Path, lyric;
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
