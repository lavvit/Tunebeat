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

namespace Tunebeat.Title
{
    public class Title : Scene
    {
        public override void Enable()
        {
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
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(KEY_INPUT_RETURN))
            {
                Program.SceneChange(new SongSelect.SongSelect());
            }
            if (Key.IsPushed(KEY_INPUT_ESCAPE))
            {
                Program.End();
            }

            base.Update();
        }
    }
}
