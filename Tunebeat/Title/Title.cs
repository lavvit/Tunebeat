using System.Drawing;
using static DxLibDLL.DX;
using SeaDrop;

namespace Tunebeat
{
    public class Title : Scene
    {
        public override void Enable()
        {
            Config = false;
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }
        public override void Draw()
        {
            DrawBox(0, 0, 1919, 1079, GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]), TRUE);
            TextureLoad.Title_Background.Draw(0, 0);

            TextureLoad.Title_Text.Draw(270, 350);
            TextureLoad.Title_Text_Color.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
            TextureLoad.Title_Text_Color.Draw(270, 350);

            #if DEBUG
            DrawString(0, 0, $"RGB:{PlayData.Data.SkinColor[0]},{PlayData.Data.SkinColor[1]},{PlayData.Data.SkinColor[2]}", 0xffffff);
            #endif

            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(EKey.Enter) || KeyInput.ListPushed(PlayData.Data.LEFTDON) || KeyInput.ListPushed(PlayData.Data.RIGHTDON) || Mouse.IsPushed(MouseButton.Left))
            {
                SoundLoad.Don[0].Volume = PlayData.Data.SE / 100.0;
                SoundLoad.Don[0].Play();
                Program.SceneChange(new SongSelect());
            }
            if (Key.IsPushed(EKey.Esc))
            {
                Program.End();
            }
            if (Key.IsPushed(PlayData.Data.MoveConfig))
            {
                Config = true;
                Program.SceneChange(new Config());
            }

            #if DEBUG
            if (Key.IsPushing(EKey.Key_1))
            {
                if (Key.IsPushing(EKey.Up) && PlayData.Data.SkinColor[0] < 255)
                {
                    PlayData.Data.SkinColor[0]++;
                }
                if (Key.IsPushing(EKey.Down) && PlayData.Data.SkinColor[0] > 0)
                {
                    PlayData.Data.SkinColor[0]--;
                }
            }
            if (Key.IsPushing(EKey.Key_2))
            {
                if (Key.IsPushing(EKey.Up) && PlayData.Data.SkinColor[1] < 255)
                {
                    PlayData.Data.SkinColor[1]++;
                }
                if (Key.IsPushing(EKey.Down) && PlayData.Data.SkinColor[1] > 0)
                {
                    PlayData.Data.SkinColor[1]--;
                }
            }
            if (Key.IsPushing(EKey.Key_3))
            {
                if (Key.IsPushing(EKey.Up) && PlayData.Data.SkinColor[2] < 255)
                {
                    PlayData.Data.SkinColor[2]++;
                }
                if (Key.IsPushing(EKey.Down) && PlayData.Data.SkinColor[2] > 0)
                {
                    PlayData.Data.SkinColor[2]--;
                }
            }
            #endif

            base.Update();
        }
        public static bool Config;
    }
}
