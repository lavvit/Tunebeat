using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static DxLibDLL.DX;
using Amaoto;

namespace Tunebeat.Game
{
    public class Game : Scene
    {
        public override void Enable()
        {
            MainTJA = new TJAParse.TJAParse(@"Songs/水天神術・時雨.tja");
            MainTimer = new Counter(-2000, int.MaxValue, 1000, false);
            MainSong = new Sound($"{Path.GetDirectoryName(MainTJA.TJAPath)}/{MainTJA.Header.WAVE}");
            Course = 4;

            #region AddChildScene
            AddChildScene(new Notes());
            #endregion
            base.Enable();
        }

        public override void Disable()
        {
            MainSong.Dispose();
            foreach (Scene scene in ChildScene)
                scene?.Disable();
            base.Disable();
        }

        public override void Draw()
        {
            foreach (Scene scene in ChildScene)
                scene?.Draw();
            base.Draw();
        }

        public override void Update()
        {
            DrawString(0, 0, $"{MainTimer.Value}", 0xffffff);
            MainTimer.Tick();
            if (MainTimer.State == 0) MainTimer.Start();
            if (MainTimer.Value >= 0 && MainTimer.State != 0 && !MainSong.IsPlaying && !IsSongPlay) { MainSong.Play(); IsSongPlay = true; }

            foreach (Scene scene in ChildScene)
                scene?.Update();

            KeyInput.Update(false);

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
        public static int Course;
    }
}
