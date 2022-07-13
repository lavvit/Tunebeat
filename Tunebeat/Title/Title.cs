using System.Drawing;
using SeaDrop;

namespace Tunebeat
{
    public class Title : Scene
    {
        public override void Enable()
        {
            ToConfig = false;
            Config.SoundPitch();
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }
        public override void Draw()
        {
            Drawing.Box(0, 0, 1919, 1079, Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
            Tx.Title_Background.Draw(0, 0);

            Tx.Title_Text.Draw(270, 350);
            Tx.Title_Text_Color.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
            Tx.Title_Text_Color.Draw(270, 350);

            #if DEBUG
            Drawing.Text(0, 0, $"RGB:{PlayData.Data.SkinColor[0]},{PlayData.Data.SkinColor[1]},{PlayData.Data.SkinColor[2]}", 0xffffff);
            #endif

            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(EKey.Enter) || Key.IsPushed(PlayData.Data.LEFTDON) || Key.IsPushed(PlayData.Data.RIGHTDON) || Mouse.IsPushed(MouseButton.Left))
            {
                Sfx.Don[0].Volume = PlayData.Data.SE / 100.0;
                Sfx.Don[0].Play();
                SongLoad.Init();
                TextLog.Draw(LoadPath);
                Program.SceneChange(new SongSelect());
            }
            if (Key.IsPushed(EKey.Esc))
            {
                Program.End();
            }
            if (Key.IsPushed(PlayData.Data.MoveConfig))
            {
                ToConfig = true;
                Program.SceneChange(new Config());
            }

            #if DEBUG
            if (Key.IsPushing(EKey.Key_1))
            {
                if (Key.IsHolding(EKey.Up, 250, 10) && PlayData.Data.SkinColor[0] < 255)
                {
                    PlayData.Data.SkinColor[0]++;
                }
                if (Key.IsHolding(EKey.Down, 250, 10) && PlayData.Data.SkinColor[0] > 0)
                {
                    PlayData.Data.SkinColor[0]--;
                }
            }
            if (Key.IsPushing(EKey.Key_2))
            {
                if (Key.IsHolding(EKey.Up, 250, 10) && PlayData.Data.SkinColor[1] < 255)
                {
                    PlayData.Data.SkinColor[1]++;
                }
                if (Key.IsHolding(EKey.Down, 250, 10) && PlayData.Data.SkinColor[1] > 0)
                {
                    PlayData.Data.SkinColor[1]--;
                }
            }
            if (Key.IsPushing(EKey.Key_3))
            {
                if (Key.IsHolding(EKey.Up, 250, 20) && PlayData.Data.SkinColor[2] < 255)
                {
                    PlayData.Data.SkinColor[2]++;
                }
                if (Key.IsHolding(EKey.Down, 250, 20) && PlayData.Data.SkinColor[2] > 0)
                {
                    PlayData.Data.SkinColor[2]--;
                }
            }
            #endif

            base.Update();
        }

        public static bool ToConfig;
        public static string LoadPath;
    }
}
