using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
            MainTJA = new TJAParse.TJAParse(PlayData.PlayFile);
            for (int i = 0; i < 2; i++)
            {
                MainSong = new Sound($"{Path.GetDirectoryName(MainTJA.TJAPath)}/{MainTJA.Header.WAVE}");
                IsAuto[i] = PlayData.Auto[i];
                Course[i] = PlayData.PlayCourse[i];
                ProcessNote.BalloonList[i] = 0;
            }

            for (int i = 0; i < 4; i++)
            {
                HitTimer[i] = new Counter(0, 100, 1000, false);
                HitTimer2P[i] = new Counter(0, 100, 1000, false);
            }

            #region AddChildScene
            AddChildScene(new Notes());
            AddChildScene(new Score());
            #endregion
            base.Enable();
        }

        public override void Disable()
        {
            MainSong.Dispose();
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
            TextureLoad.Game_Background.Draw(0, 0);

            #if DEBUG
            DrawString(0, 0, $"{MainTimer.Value}", 0xffffff); if (IsSongPlay && !MainSong.IsPlaying) DrawString(60, 0, "Stoped", 0xffffff);
            DrawString(0, 20, $"{MainTJA.Header.TITLE}", 0xffffff);
            DrawString(0, 40, $"{MainTJA.Header.SUBTITLE}", 0xffffff);
            DrawString(0, 60, $"{MainTJA.Header.BPM}", 0xffffff);

            DrawString(0, 80, $"{MainTJA.Courses[Course[0]].COURSE}" + (PlayData.IsPlay2P ? $"/{MainTJA.Courses[Course[1]].COURSE}" : ""), 0xffffff);
            DrawString(0, 100, $"{MainTJA.Courses[Course[0]].LEVEL}" + (PlayData.IsPlay2P ? $"/{MainTJA.Courses[Course[1]].LEVEL}" : ""), 0xffffff);
            DrawString(0, 120, $"{MainTJA.Courses[Course[0]].TotalNotes}" + (PlayData.IsPlay2P ? $"/{MainTJA.Courses[Course[1]].TotalNotes}" : ""), 0xffffff);
            DrawString(0, 140, $"{MainTJA.Courses[Course[0]].ScrollType}" + (PlayData.IsPlay2P ? $"/{MainTJA.Courses[Course[1]].ScrollType}" : ""), 0xffffff);
            DrawString(200, 0, $"{PlayData.AutoRoll}", 0xffffff);

            Chip[] chip = new Chip[2] { GetNotes.GetNowNote(MainTJA.Courses[Course[0]].ListChip, MainTimer.Value), GetNotes.GetNowNote(MainTJA.Courses[Course[1]].ListChip, MainTimer.Value) };
            if (chip[0] != null)
            {
                DrawString(520, 160, $"{chip[0].ENote}", 0xffffff);
                DrawString(520, 180, $"{chip[0].Time}", 0xffffff);
                DrawString(520, 200, $"{ProcessNote.RollState(chip[0])}", 0xffffff);
                DrawString(520, 220, $"{chip[0].RollCount}", 0xffffff);
                DrawString(520, 240, $"{ProcessNote.BalloonRemain[0]}", 0xffffff);
            }
            if (PlayData.IsPlay2P && chip[1] != null)
            {
                DrawString(520, 780, $"{chip[1].ENote}", 0xffffff);
                DrawString(520, 800, $"{chip[1].Time}", 0xffffff);
                DrawString(520, 820, $"{ProcessNote.RollState(chip[1])}", 0xffffff);
                DrawString(520, 840, $"{chip[1].RollCount}", 0xffffff);
                DrawString(520, 860, $"{ProcessNote.BalloonRemain[1]}", 0xffffff);
            }

            if (IsSongPlay && !MainSong.IsPlaying) DrawString(0, 160, "PRESS ENTER", 0xffffff);
            #endif

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
            if (MainTimer.State == 0) MainTimer.Start();
            if (MainTimer.Value >= 0 && MainTimer.State != 0 && !MainSong.IsPlaying && !IsSongPlay) { MainSong.Play(); IsSongPlay = true; }
            if (IsSongPlay && !MainSong.IsPlaying)
            {
                MainTimer.Stop();
                if (Key.IsPushed(KEY_INPUT_RETURN))
                {
                    Program.SceneChange(new SongSelect.SongSelect());
                }
            }
            if (Key.IsPushed(KEY_INPUT_ESCAPE))
            {
                Program.SceneChange(new SongSelect.SongSelect());
            }

            foreach (Scene scene in ChildScene)
                scene?.Update();

            KeyInput.Update(IsAuto[0], IsAuto[1]);
          

            base.Update();
        }

        public static void AddChildScene(Scene scene)
        {
            scene?.Enable();
            ChildScene.Add(scene);
        }

        public static TJAParse.TJAParse MainTJA;
        public static Counter MainTimer;
        public static Sound MainSong;
        public static List<Scene> ChildScene = new List<Scene>();
        public static bool IsSongPlay;
        public static bool[] IsAuto = new bool[2];
        public static int[] Course = new int[2];
        public static Counter[] HitTimer = new Counter[4];
        public static Counter[] HitTimer2P = new Counter[4];
    }
}
