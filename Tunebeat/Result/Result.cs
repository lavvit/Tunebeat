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

namespace Tunebeat.Result
{
    public class Result : Scene
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
            DrawBox(0, 0, 1919, 257, GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]), TRUE);
            TextureLoad.Result_Background.Draw(0, 0);
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(KEY_INPUT_ESCAPE) || Key.IsPushed(KEY_INPUT_RETURN))
            {
                Program.SceneChange(new SongSelect.SongSelect());
            }
            base.Update();
        }
    }
}
