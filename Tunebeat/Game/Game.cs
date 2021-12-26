using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static DxLibDLL.DX;
using Amaoto;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class Game : Scene
    {
        public override void Enable()
        {
            MainTJA = new TJAParse.TJAParse(PlayData.PlayFile);
            MainTimer = new Counter(-2000, int.MaxValue, 1000, false);
            MainSong = new Sound($"{Path.GetDirectoryName(MainTJA.TJAPath)}/{MainTJA.Header.WAVE}");
            IsAuto = PlayData.Auto;
            Course = PlayData.PlayCourse;

            #region AddChildScene
            AddChildScene(new Notes());
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

            DrawString(0, 80, $"{MainTJA.Courses[Course].COURSE}", 0xffffff);
            DrawString(0, 100, $"{MainTJA.Courses[Course].LEVEL}", 0xffffff);
            DrawString(0, 120, $"{MainTJA.Courses[Course].TotalNotes}", 0xffffff);
            DrawString(0, 140, $"{MainTJA.Courses[Course].ScrollType}", 0xffffff);
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

            KeyInput.Update(IsAuto);
          

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
        public static bool IsSongPlay, IsAuto = false;
        public static int Course;
    }
}
