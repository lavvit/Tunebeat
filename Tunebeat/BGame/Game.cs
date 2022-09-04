using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SeaDrop;
using BMSParse;

namespace Tunebeat
{
    public class BGame : Scene
    {
        #region Function
        public static string Path;
        public static BMS[] BMS;
        public static BCourse[] NowCourse;
        public static List<Chip>[] Chips;
        public static List<Bar>[] Bars;
        public static Counter Timer;
        public static Sound[][] KeySound = new Sound[2][], ChargeSound = new Sound[2][];
        public static int StartMeasure;
        public static bool Play2P;
        public static bool[] Failed = new bool[2];
        public static double TimeRemain;
        public static double[] AutoTime = new double[2];
        public static double[] Adjust = new double[5], ScrollRemain = new double[5];
        public static EState NowState;
        public static EAuto[] Playmode = new EAuto[2];
        public static Texture Title, Genre, Artist, Lyric;
        public static Counter[][] HitTimer = new Counter[5][];
        public static Handle LyricHandle, TitleHandle, GenreHandle, ArtistHandle;
        public static Number GameNumber, SmallNumber;
        public static DateTime FileTime;
        #endregion

        public override void Enable()
        {
            Timer = new Counter(-2000, int.MaxValue, 1000, false);
            Path = SongSelect.PlayMode > 0 ? $@"{SongSelect.FolderName}\{SongSelect.FileName}" : SongData.NowSong.Path;

            for (int i = 0; i < 2; i++)
            {
                Adjust[i] = !PlayData.Data.Auto[i] ? PlayData.Data.BMSInputAdjust[i] : 0;
            }
            Adjust[2] = Adjust[3] = PlayData.Data.BMSInputAdjust[0];

            StartMeasure = 0;
            BMSInit();
            Play2P = NowCourse[0].Player == 2 ? false : PlayData.Data.IsPlay2P;// Dan == null && Chips[1] != null && Chips[1].Count > 0 ? PlayData.Data.IsPlay2P : false;
            Init();

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
            //    $"{SongData.NowBMS[0].Header.TITLE}:{PlayMemory.NowClear(0)} {Score.EXScore[0]}", Discord.GetUnixTime() + (long)SongData.NowBMS[0].Courses[Course[0]].ListChip[SongData.NowBMS[0].Courses[Course[0]].ListChip.Count].Time / 1000);

            for (int i = 0; i < 5; i++)
            {
                HitTimer[i] = new Counter[4];
                for (int j = 0; j < 4; j++)
                {
                    HitTimer[i][j] = new Counter(0, 100, 1000, false);
                }
            }
            base.Enable();
        }

        public static void Init()
        {
            NowState = EState.None;
            AutoTime[0] = AutoTime[1] = 0;
            if (Timer.State != 0) Timer.Stop();
            Timer.Value = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;

            for (int i = 0; i < 2; i++)
            {
                Playmode[i] = EAuto.Normal;
                if (PlayData.Data.Auto[i] || PlayData.Data.PreviewType == 3) Playmode[i] = EAuto.Auto;
                if (SongSelect.Replay[i] && !string.IsNullOrEmpty(SongSelect.ReplayScore[i])) Playmode[i] = EAuto.Replay;

                Failed[i] = false;
                KeySound[i] = new Sound[16];
                ChargeSound[i] = new Sound[16];
            }
            BScore.Init();
            BSudden.Init();

            for (int p = 0; p < BMS.Length; p++)
            {
                if (BMS[p] != null && BMS[p].Course.ListChip != null)
                {
                    for (int i = 0; i < BMS[p].Course.ListChip.Count; i++)
                    {
                        Chip chip = BMS[p].Course.ListChip[i];
                        chip.IsHit = false;
                        chip.IsMiss = false;
                        chip.Pushing = false;
                        chip.TopPushed = false;
                        if (chip.LongEnd != null) chip.LongEnd.IsMiss = false;
                        if (chip.Sound != null) chip.Sound.Stop();
                    }
                    BCourse.SetOption(BMS[p].Course.ListChip, (EOption)PlayData.Data.Option[p], BMS[p].Course);
                }
            }
        }
        public static void BMSInit()
        {
            int lane = PlayData.Data.PreviewType == 3 ? 5 : 2;
            if (Chips != null)
            {
                for (int i = 0; i < Chips.Length; i++)
                {
                    if (Chips[i] != null) BCourse.DeleteSound(Chips[i]);
                }
            }
            SongData.NowBMS = new BMS[2];
            for (int i = 0; i < 2; i++)
            {
                ReplayData replay = i > 0 ? Memory.ReplayData2P : Memory.ReplayData;
                SongData.NowBMS[i] = new BMS(Path, Playmode[i] == EAuto.Replay && replay != null ? (replay.Speed > 0 ? replay.Speed : 1) : PlayData.Data.PlaySpeed,
                    PlayData.Data.LNType, PlayData.Data.Option[i]);
            }
            BMS = SongData.NowBMS;
            FileTime = DateTime.Now;

            NowCourse = new BCourse[lane];
            Chips = new List<Chip>[lane];
            Bars = new List<Bar>[lane];
            for (int i = 0; i < lane; i++)
            {
                NowCourse[i] = BMS[i].Course;
                Chips[i] = NowCourse[i].ListChip;
                Bars[i] = NowCourse[i].ListBar;

                BCourse.SetSound(Chips[i]);
            }

            if (PlayData.Data.FontRendering)
            {
                Title = FontRender.GetTexture(BMS[0].Course.Title, 48, 6, PlayData.Data.FontName);
                Genre = FontRender.GetTexture(BMS[0].Course.Genre, 20, 4, PlayData.Data.FontName);
                Artist = FontRender.GetTexture(BMS[0].Course.Artist, 20, 4, PlayData.Data.FontName);
            }
        }

        public override void Disable()
        {
            for (int i = 0; i < Chips.Length; i++)
            {
                if (Chips[i] != null) BCourse.DeleteSound(Chips[i]);
            }
            Timer.Reset();
            NowState = EState.None;
            for (int i = 0; i < HitTimer.Length; i++)
            {
                for (int j = 0; j < HitTimer[i].Length; j++)
                {
                    HitTimer[i][j].Reset();
                }
            }

            base.Disable();
        }

        public override void Draw()
        {
            Drawing.Box(0, 0, 1919, 1079, Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
            Tx.Game_Background.Draw(0, 0);

            BNotes.Draw();
            BProcess.Draw();
            BSudden.Draw();
            BScore.Draw();
            if (BMS[0] != null)
            {
                Drawing.Text(0, 0, $"{BMS[0].Course.Genre}");
                Drawing.Text(0, 20, $"{BMS[0].Course.Title}");
                Drawing.Text(0, 40, $"{BMS[0].Course.Artist}");
                Drawing.Text(0, 60, $"{BMS[0].Course.BPM} BPM ({PlayData.Data.PlaySpeed}x Speed)");
                int[] col = new int[5] { 0x40ff00, 0x0080ff, 0xffc000, 0xff0000, 0xff00c0 };
                Drawing.Text(0, 80, $"{BMS[0].Course.Difficulty} {BMS[0].Course.Level}  {BMS[0].Course.TotalNotes} Notes", col[(int)BMS[0].Course.Difficulty]);
                if (Playmode[0] > EAuto.Normal) Drawing.Text(PlayData.Data.Is2PSide ? 1760 : 80, 640, $"{Playmode[0]}", 0xff0000);
                if (Playmode[1] > EAuto.Normal && Play2P) Drawing.Text(1760, 640, $"{Playmode[1]}", 0x0000ff);

                string map = "";
                for (int i = 0; i < BMS[0].Course.RanMap.Length; i++)
                {
                    map += $"{(BMS[0].Course.RanMap[i] == 0 ? "S" : $"{BMS[0].Course.RanMap[i]}")}" + (i == 7 ? "" : ", ");
                }
                int[] keycolor = new int[8] { (Playmode[0] == EAuto.Normal && PlayData.Data.AutoSclatch[0] ? 0x00ff44 : 0xff0000), 0xffffff, 0x0000ff, 0xffffff, 0x0000ff, 0xffffff, 0x0000ff, 0xffffff };
                Drawing.Text(0, 100, $"{(EOption)PlayData.Data.Option[0]} {map}");
                Drawing.Text(0, 720, $"{NowState}");
                Drawing.Text(0, 740, $"{Timer.Value}");
                Drawing.Text(0, 780, $"BPM:{BProcess.NowBPM(BMS[0].Course.ListCBPM)}");

                for (int i = 0; i < 8; i++)
                {
                    Chip chip = BProcess.NowChip(BMS[0].Course.ListChip, BCourse.GetChannel(i));
                    Chip nchip = BProcess.NearChip(BMS[0].Course.ListChip, BCourse.GetChannel(i));
                    Drawing.Text(540, 100 + 40 * i, chip != null ? $"{i} : " + chip.Draw() : $"{i} : ");
                    Drawing.Text(1100, 100 + 40 * i, nchip != null ? $"Near : " + nchip.Draw() : $"Near : ");
                }
            }

            TextDebug.Update();
            base.Draw();
        }

        public override void Update()
        {
            Timer.Tick();
            bool s = false;
            if (NowState == EState.None && (Key.IsPushed(EKey.Space) || Key.IsPushed(EKey.Enter) || BProcess.Pushed(9, 0)))
            {
                Timer.Start();
                NowState = EState.Start;
                s = true;
            }
            if (NowState == EState.Start && Timer.Value >= 0)
            {
                NowState = EState.Play;
            }
            if (NowState == EState.Play && BProcess.Finished())// && Timer.Value > endtime
            {
                NowState = EState.End;
            }

            SmoothMoving();

            BProcess.Update();
            BScore.Update();
            BSudden.Update();

            switch (NowState)
            {
                case EState.None:
                    if (Key.IsHolding(EKey.PgUp, 200, 25)) MeasureUp();
                    if (Key.IsHolding(EKey.PgDn, 200, 25)) MeasureDown();
                    if (Key.IsPushed(EKey.Home)) MeasureHome();
                    if (Key.IsPushed(EKey.End)) MeasureEnd();
                    break;
            }
            if (Key.IsPushed(EKey.Esc)) Program.SceneChange(new SongSelect());
            if (Key.IsPushed(EKey.Back)) Init();//(!s && Key.IsPushed(EKey.Space)) || 
            for (int i = 0; i < 2; i++)
            {
                if ((NowState > EState.None && BProcess.Pushing(8, i) && BProcess.Pushing(9, i))
                   || (NowState == EState.End && BProcess.Pushing(9, i))) Init();
            }
            if (Key.IsPushed(EKey.F1))
            {
                if (Key.IsPushing(EKey.LShift))
                {
                    PlayData.Data.AutoSclatch[0] = !PlayData.Data.AutoSclatch[0];
                }
                else
                {
                    if (Playmode[0] < EAuto.Replay)
                    {
                        Playmode[0] = Playmode[0] == EAuto.Normal ? EAuto.Auto : EAuto.Normal;
                    }
                    if (Playmode[0] == EAuto.Auto && NowState == EState.Play) AutoTime[0] = Timer.Value;
                    if (NowState == EState.None) PlayData.Data.Auto[0] = Playmode[0] == EAuto.Auto;
                }
                
            }
            if (Key.IsPushed(EKey.F2) && Play2P)
            {
                if (Key.IsPushing(EKey.RShift))
                {
                    PlayData.Data.AutoSclatch[1] = !PlayData.Data.AutoSclatch[1];
                }
                else
                {
                    if (Playmode[1] < EAuto.Replay) Playmode[1] = Playmode[1] == EAuto.Normal ? EAuto.Auto : EAuto.Normal;
                    if (Playmode[1] == EAuto.Auto && NowState == EState.Play) AutoTime[1] = Timer.Value;
                    if (NowState == EState.None) PlayData.Data.Auto[1] = Playmode[1] == EAuto.Auto;
                }
            }
            if (Key.IsPushed(EKey.F3))
            {
                if (PlayData.Data.Option[0]++ >= (int)EOption.AllScrach) PlayData.Data.Option[0] = (int)EOption.None;
                Init();
            }
            if (Key.IsPushed(EKey.F4) && Play2P)
            {
                if (PlayData.Data.Option[1]++ >= (int)EOption.AllScrach) PlayData.Data.Option[1] = (int)EOption.None;
                Init();
            }
            if (Key.IsPushed(EKey.F5))
            {
                double pre = PlayData.Data.PlaySpeed;
                PlayData.Data.PlaySpeed -= 0.05;
                BMSInit();
                Init();
                Timer.Value = (long)(Timer.Value * (pre / PlayData.Data.PlaySpeed));
            }
            if (Key.IsPushed(EKey.F6))
            {
                double pre = PlayData.Data.PlaySpeed;
                PlayData.Data.PlaySpeed += 0.05;
                BMSInit();
                Init();
                Timer.Value = (long)(Timer.Value * (pre / PlayData.Data.PlaySpeed));
            }

            if (!string.IsNullOrEmpty(Path) && File.GetLastWriteTime(Path) > FileTime)
            {
                Init();
                BMSInit();
            }

            base.Update();
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

        public static void MeasureUp()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            if (StartMeasure++ >= Bars[0].Count) StartMeasure = Bars[0].Count;
            Timer.Value = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            TimeRemain -= Timer.Value - prevalue;
        }
        public static void MeasureDown()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            if (StartMeasure-- <= 0) StartMeasure = 0;
            Timer.Value = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            TimeRemain -= Timer.Value - prevalue;
        }
        public static void MeasureHome()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            StartMeasure = 0;
            Timer.Value = Timer.Begin;
            TimeRemain -= Timer.Value - prevalue;
        }
        public static void MeasureEnd()
        {
            double prevalue = StartMeasure == 0 ? Timer.Begin : (long)Bars[0][StartMeasure - 1].Time;
            StartMeasure = Bars[0].Count;
            Timer.Value = (long)Bars[0][Bars[0].Count - 1].Time;
            TimeRemain -= Timer.Value - prevalue;
        }
    }
}
