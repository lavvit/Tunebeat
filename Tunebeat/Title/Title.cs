using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using Amaoto;
using Tunebeat.Common;
using Tunebeat.Game;

namespace Tunebeat.Title
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
            if (Key.IsPushed(KEY_INPUT_RETURN) || KeyInput.ListPushed(PlayData.Data.LEFTDON) || KeyInput.ListPushed(PlayData.Data.RIGHTDON) || Mouse.IsPushed(MouseButton.Left))
            {
                SoundLoad.Don[0].Volume = PlayData.Data.SE / 100.0;
                SoundLoad.Don[0].Play();
                Program.SceneChange(new SongSelect.SongSelect());
            }
            if (Key.IsPushed(KEY_INPUT_ESCAPE))
            {
                Program.End();
            }

            if (Key.IsPushing(KEY_INPUT_1))
            {
                if (Key.IsPushing(KEY_INPUT_UP) && PlayData.Data.SkinColor[0] < 255)
                {
                    PlayData.Data.SkinColor[0]++;
                }
                if (Key.IsPushing(KEY_INPUT_DOWN) && PlayData.Data.SkinColor[0] > 0)
                {
                    PlayData.Data.SkinColor[0]--;
                }
            }
            if (Key.IsPushing(KEY_INPUT_2))
            {
                if (Key.IsPushing(KEY_INPUT_UP) && PlayData.Data.SkinColor[1] < 255)
                {
                    PlayData.Data.SkinColor[1]++;
                }
                if (Key.IsPushing(KEY_INPUT_DOWN) && PlayData.Data.SkinColor[1] > 0)
                {
                    PlayData.Data.SkinColor[1]--;
                }
            }
            if (Key.IsPushing(KEY_INPUT_3))
            {
                if (Key.IsPushing(KEY_INPUT_UP) && PlayData.Data.SkinColor[2] < 255)
                {
                    PlayData.Data.SkinColor[2]++;
                }
                if (Key.IsPushing(KEY_INPUT_DOWN) && PlayData.Data.SkinColor[2] > 0)
                {
                    PlayData.Data.SkinColor[2]--;
                }
            }
            if (Key.IsPushed(PlayData.Data.MOVECONFIG))
            {
                Config = true;
                Program.SceneChange(new Config.Config());
            }

            base.Update();
        }
        public static bool Config;
    }
}
