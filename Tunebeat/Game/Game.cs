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

namespace Tunebeat.Game
{
    public class Game : Scene
    {
        public override void Enable()
        {
            MainTimer = new Counter(-2000, int.MaxValue, 1000, false);
            
            for (int i = 0; i < 2; i++)
            {
                MainTJA[i] = new TJAParse.TJAParse(PlayData.Data.PlayFile, PlayData.Data.PlaySpeed);
                IsAuto[i] = PlayData.Data.Auto[i];
                Course[i] = PlayData.Data.PlayCourse[i];
                Failed[i] = false;
                ProcessNote.BalloonList[i] = 0;
                PushingTimer[i] = new Counter(0, int.MaxValue, 1000, false);
            }
            MainSong = new Sound($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.WAVE}");
            MainImage = new Texture($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGIMAGE}");
            MainMovie = new Movie($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGMOVIE}");

            for (int i = 0; i < 4; i++)
            {
                HitTimer[i] = new Counter(0, 100, 1000, false);
                HitTimer2P[i] = new Counter(0, 100, 1000, false);
            }

            PlayMeasure = 0;
            StartTime = 0;
            MeasureCount();

            #region AddChildScene
            AddChildScene(new Notes());
            AddChildScene(new Score());
            #endregion
            base.Enable();
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

        public static void Reset()
        {
            MainTimer.Stop();
            MainSong.Stop();
            MainMovie.Stop();
            if (PlayMeasure == 0) MainTimer.Reset();
            else MainTimer.Value = (int)StartTime;
            MainSong.Time = StartTime / 1000;
            MainMovie.Time = StartTime;
            IsSongPlay = false;
            for (int i = 0; i < 2; i++)
            {
                MainTJA[i] = new TJAParse.TJAParse(PlayData.Data.PlayFile, PlayData.Data.PlaySpeed);
                Failed[i] = false;
            }

            for (int i = 0; i < 2; i++)
            {
                Score.EXScore[i] = 0;
                Score.Perfect[i] = 0;
                Score.Great[i] = 0;
                Score.Good[i] = 0;
                Score.Bad[i] = 0;
                Score.Poor[i] = 0;
                Score.Auto[i] = 0;
                Score.Roll[i] = 0;
                Score.AutoRoll[i] = 0;
                Score.RollYellow[i] = 0;
                Score.RollBalloon[i] = 0;
                Score.Gauge[i] = 0;
                Score.SetGauge(i);
                Score.Combo[i] = 0;
                Score.MaxCombo[i] = 0;
            }

            for (int i = 0; i < 2; i++)
            {
                Notes.UseSudden[i] = PlayData.Data.UseSudden[i];
                Notes.Sudden[i] = PlayData.Data.SuddenNumber[i];
                Notes.Scroll[i] = PlayData.Data.ScrollSpeed[i];
            }
        }

        public static void MeasureUp()
        {
            if (PlayMeasure < MeasureList.Count)
            {
                PlayMeasure++;
                StartTime = MeasureList[PlayMeasure - 1].Time;
                MainTimer.Value = (int)MeasureList[PlayMeasure - 1].Time;
                if (MainSong != null) MainSong.Time = MeasureList[PlayMeasure - 1].Time / 1000;
                if (MainMovie != null) MainMovie.Time = MeasureList[PlayMeasure - 1].Time;
            }
        }
        public static void MeasureDown()
        {
            if (PlayMeasure > 1)
            {
                PlayMeasure--;
                StartTime = MeasureList[PlayMeasure - 1].Time;
                MainTimer.Value = (int)MeasureList[PlayMeasure - 1].Time;
                if (MainSong != null) MainSong.Time = MeasureList[PlayMeasure - 1].Time / 1000;
                if (MainMovie != null) MainMovie.Time = MeasureList[PlayMeasure - 1].Time;
                for (int i = 0; i < 2; i++)
                {
                    foreach (Chip chip in MainTJA[i].Courses[Course[i]].ListChip)
                    {
                        if (chip.Time >= StartTime && chip.IsMiss)
                        {
                            chip.IsMiss = false;
                            Score.Poor[i]--;
                        }
                    }
                }
            }
            else if(PlayMeasure == 1)
            {
                PlayMeasure = 0;
                StartTime = 0;
                MainTimer.Value = -2000;
                if (MainSong != null) MainSong.Time = 0;
                if (MainMovie != null) MainMovie.Time = 0;
            }
        }

        public override void Disable()
        {
            MainSong.Dispose();
            MainImage.Dispose();
            MainMovie.Dispose();
            MainTimer.Reset();
            IsSongPlay = false;
            for (int i = 0; i < 4; i++)
            {
                HitTimer[i].Reset();
                HitTimer2P[i].Reset();
            }
            foreach (Scene scene in ChildScene)
                scene?.Disable();
            base.Disable();
        }

        public override void Draw()
        {
            if (PlayData.Data.PlayMovie && File.Exists($"{Path.GetDirectoryName(MainTJA[0].TJAPath)}/{MainTJA[0].Header.BGMOVIE}"))
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

            if (HitTimer[0].State == TimerState.Started)
            {
                TextureLoad.Game_Don[0].Opacity = 1.0 - ((double)HitTimer[0].Value / HitTimer[0].End);
                TextureLoad.Game_Don[0].Draw(362, 337);
            }
            if (HitTimer[1].State == TimerState.Started)
            {
                TextureLoad.Game_Don[1].Opacity = 1.0 - ((double)HitTimer[1].Value / HitTimer[1].End);
                TextureLoad.Game_Don[1].Draw(362, 337);
            }
            if (HitTimer[2].State == TimerState.Started)
            {
                TextureLoad.Game_Ka[0].Opacity = 1.0 - ((double)HitTimer[2].Value / HitTimer[2].End);
                TextureLoad.Game_Ka[0].Draw(362, 337);
            }
            if (HitTimer[3].State == TimerState.Started)
            {
                TextureLoad.Game_Ka[1].Opacity = 1.0 - ((double)HitTimer[3].Value / HitTimer[3].End);
                TextureLoad.Game_Ka[1].Draw(362, 337);
            }
            if (HitTimer2P[0].State == TimerState.Started)
            {
                TextureLoad.Game_Don2P[0].Opacity = 1.0 - ((double)HitTimer2P[0].Value / HitTimer2P[0].End);
                TextureLoad.Game_Don2P[0].Draw(362, 337 + 262);
            }
            if (HitTimer2P[1].State == TimerState.Started)
            {
                TextureLoad.Game_Don2P[1].Opacity = 1.0 - ((double)HitTimer2P[1].Value / HitTimer2P[1].End);
                TextureLoad.Game_Don2P[1].Draw(362, 337 + 262);
            }
            if (HitTimer2P[2].State == TimerState.Started)
            {
                TextureLoad.Game_Ka2P[0].Opacity = 1.0 - ((double)HitTimer2P[2].Value / HitTimer2P[2].End);
                TextureLoad.Game_Ka2P[0].Draw(362, 337 + 262);
            }
            if (HitTimer2P[3].State == TimerState.Started)
            {
                TextureLoad.Game_Ka2P[1].Opacity = 1.0 - ((double)HitTimer2P[3].Value / HitTimer2P[3].End);
                TextureLoad.Game_Ka2P[1].Draw(362, 337 + 262);
            }
            Score.DrawCombo(0);
            if (PlayData.Data.IsPlay2P)
            {
                Score.DrawCombo(1);
            }

            if (MainTimer.State == 0 && !IsSongPlay)
            {
                DrawString(720, 376, "PRESS SPACE KEY", 0xffffff);
            }

            Chip nchip = GetNotes.GetNowNote(MainTJA[0].Courses[Course[0]].ListChip, MainTimer.Value, true);
            if (nchip != null && nchip.Lyric != null) DrawString(960, 1000, nchip.Lyric, 0x0000ff);


            #if DEBUG
            DrawString(0, 0, $"{MainTimer.Value}", 0xffffff); if (IsSongPlay && !MainSong.IsPlaying) DrawString(60, 0, "Stoped", 0xffffff);
            DrawString(0, 20, $"{MainTJA[0].Header.TITLE}", 0xffffff);
            DrawString(0, 40, $"{MainTJA[0].Header.SUBTITLE}", 0xffffff);
            DrawString(0, 60, $"{MainTJA[0].Header.BPM}", 0xffffff);

            DrawString(0, 80, $"{MainTJA[0].Courses[Course[0]].COURSE}" + (PlayData.Data.IsPlay2P ? $"/{MainTJA[0].Courses[Course[1]].COURSE}" : ""), 0xffffff);
            DrawString(0, 100, $"{MainTJA[0].Courses[Course[0]].LEVEL}" + (PlayData.Data.IsPlay2P ? $"/{MainTJA[0].Courses[Course[1]].LEVEL}" : ""), 0xffffff);
            DrawString(0, 120, $"{MainTJA[0].Courses[Course[0]].TotalNotes}" + (PlayData.Data.IsPlay2P ? $"/{MainTJA[0].Courses[Course[1]].TotalNotes}" : ""), 0xffffff);
            DrawString(0, 140, $"{MainTJA[0].Courses[Course[0]].ScrollType}" + (PlayData.Data.IsPlay2P ? $"/{MainTJA[0].Courses[Course[1]].ScrollType}" : ""), 0xffffff);
            DrawString(200, 0, $"{PlayData.Data.AutoRoll}", 0xffffff);

            Chip[] chip = new Chip[2] { GetNotes.GetNowNote(MainTJA[0].Courses[Course[0]].ListChip, MainTimer.Value), GetNotes.GetNowNote(MainTJA[0].Courses[Course[1]].ListChip, MainTimer.Value) };
            if (chip[0] != null)
            {
                DrawString(520, 160, $"{chip[0].ENote}", 0xffffff);
                DrawString(520, 180, $"{chip[0].Time}", 0xffffff);
                DrawString(520, 200, $"{ProcessNote.RollState(chip[0])}", 0xffffff);
                DrawString(520, 220, $"{chip[0].RollCount}", 0xffffff);
                DrawString(520, 240, $"{ProcessNote.BalloonRemain[0]}", 0xffffff);
            }
            if (PlayData.Data.IsPlay2P && chip[1] != null)
            {
                DrawString(520, 780, $"{chip[1].ENote}", 0xffffff);
                DrawString(520, 800, $"{chip[1].Time}", 0xffffff);
                DrawString(520, 820, $"{ProcessNote.RollState(chip[1])}", 0xffffff);
                DrawString(520, 840, $"{chip[1].RollCount}", 0xffffff);
                DrawString(520, 860, $"{ProcessNote.BalloonRemain[1]}", 0xffffff);
            }

            DrawString(520, 260, $"{ProcessNote.BalloonList[0]}", 0xffffff);

            if (IsSongPlay && !MainSong.IsPlaying) DrawString(0, 160, "PRESS ENTER", 0xffffff);
            #endif

            base.Draw();
        }

        public override void Update()
        {
            MainTimer.Tick();
            for (int i = 0; i < 4; i++)
            {
                HitTimer[i].Tick();
                HitTimer2P[i].Tick();
            }
            for (int i = 0; i < 2; i++)
            {
                PushingTimer[i].Tick();
            }
            if (MainTimer.State == 0 && (Key.IsPushed(KEY_INPUT_SPACE) || PlayData.Data.QuickStart)) MainTimer.Start();
            if (MainTimer.Value >= 0 && MainTimer.State != 0 && !MainSong.IsPlaying && !IsSongPlay) { MainSong.Play(PlayMeasure == 0 ? true : false); if (PlayData.Data.PlayMovie) { MainMovie.Play(PlayMeasure == 0 ? true : false); } IsSongPlay = true;  MainSong.PlaySpeed = (PlayData.Data.PlaySpeed); }
            if (IsSongPlay && !MainSong.IsPlaying)
            {
                MainTimer.Stop();
                if (PlayData.Data.ShowResultScreen)
                {
                    Program.SceneChange(new Result.Result());
                }
                if (Key.IsPushed(KEY_INPUT_RETURN))
                {
                    Program.SceneChange(new SongSelect.SongSelect());
                }
            }
            if (Key.IsPushed(KEY_INPUT_ESCAPE))
            {
                Program.SceneChange(new SongSelect.SongSelect());
            }
            if (Key.IsPushing(KEY_INPUT_LSHIFT) && Key.IsPushing(KEY_INPUT_RSHIFT) && Key.IsPushing(KEY_INPUT_DELETE))
            {
                Program.SceneChange(new SongSelect.SongSelect());
            }

            foreach (Scene scene in ChildScene)
                scene?.Update();

            KeyInput.Update(IsAuto[0], IsAuto[1], Failed[0], Failed[1]);
          

            base.Update();
        }

        public static void AddChildScene(Scene scene)
        {
            scene?.Enable();
            ChildScene.Add(scene);
        }

        public static TJAParse.TJAParse[] MainTJA = new TJAParse.TJAParse[2];
        public static Counter MainTimer;
        public static Sound MainSong;
        public static Texture MainImage;
        public static Movie MainMovie;
        public static List<Scene> ChildScene = new List<Scene>();
        public static bool IsSongPlay;
        public static bool[] IsAuto = new bool[2], Failed = new bool[2];
        public static int[] Course = new int[2];
        public static int PlayMeasure;
        public static double StartTime; 
        public static List<Chip> MeasureList = new List<Chip>();
        public static Counter[] HitTimer = new Counter[4];
        public static Counter[] HitTimer2P = new Counter[4];
        public static Counter[] PushingTimer = new Counter[2];
    }
}
