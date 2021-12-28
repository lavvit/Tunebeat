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
            foreach (Scene scene in ChildScene)
                scene?.Disable();
            base.Disable();
        }

        public override void Draw()
        {
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
                DrawString(520, 80, $"{chip[0].ENote}", 0xffffff);
                DrawString(520, 100, $"{chip[0].Time}", 0xffffff);
                DrawString(520, 120, $"{ProcessNote.RollState(chip[0])}", 0xffffff);
                DrawString(520, 160, $"{chip[0].RollCount}", 0xffffff);
                DrawString(520, 180, $"{ProcessNote.BalloonRemain[0]}", 0xffffff);
                DrawString(520, 240, $"{chip[0].ENote}", 0xffffff);
            }
            if (PlayData.IsPlay2P && chip[1] != null)
            {
                DrawString(520, 820, $"{chip[1].ENote}", 0xffffff);
                DrawString(520, 840, $"{chip[1].Time}", 0xffffff);
                DrawString(520, 860, $"{ProcessNote.RollState(chip[1])}", 0xffffff);
                DrawString(520, 880, $"{chip[1].RollCount}", 0xffffff);
                DrawString(520, 900, $"{ProcessNote.BalloonRemain[1]}", 0xffffff);
                DrawString(520, 780, $"{chip[1].ENote}", 0xffffff);
            }

            if (IsSongPlay && !MainSong.IsPlaying) DrawString(0, 160, "PRESS ENTER", 0xffffff);
            #endif

            foreach (Scene scene in ChildScene)
                scene?.Draw();
            base.Draw();
        }

        public override void Update()
        {
            MainTimer.Tick();
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
    }
}
