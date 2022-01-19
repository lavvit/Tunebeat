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
            TextureLoad.Result_Panel.Draw(0, 0, new Rectangle(0, 0, 960, 1080));

            int[] Color = new int[100];
            int[] Color2P = new int[100];
            for (int i = 0; i < 100; i++)
            {
                Color[i] = i >= Score.ClearRate[0][(int)Score.GaugeType[0]] - 1 ? (Score.GaugeType[0] >= EGauge.Hard ? 3 * (int)Score.GaugeType[0] : 9) : 3 * (int)Score.GaugeType[0];
                Color2P[i] = i >= Score.ClearRate[1][(int)Score.GaugeType[1]] - 1 ? (Score.GaugeType[1] >= EGauge.Hard ? 3 * (int)Score.GaugeType[1] : 9) : 3 * (int)Score.GaugeType[1];
            }

            for (int i = 0; i < 100; i++)
            {
                TextureLoad.Result_Gauge.Draw(159 + 4 * i, 279, new Rectangle(Color[i], (int)Score.Gauge[0] > i ? 48 : 0, 3, 48));
            }
            TextureLoad.Result_Rank.Draw(363, 461, new Rectangle(0, 45 * (int)Score.Rank[0], 161, 45));

            Score.DrawNumber(52, 290, $"{(int)Score.Gauge[0],3}%", 0);
            Score.DrawNumber(377, 541, $"{(Score.EXScore[0] > 0 ? Score.EXScore[0] : Score.Auto[0] * 2),5}", Score.EXScore[0] > 0 ? 6 : 7);
            Score.DrawNumber(377, 541 + 73, $"{Score.Bad[0] + Score.Poor[0],5}", 6);

            Score.DrawNumber(171, 750, $"{(Score.EXScore[0] > 0 ? Score.Perfect[0] : Score.Auto[0]),4}", Score.EXScore[0] == 0 && Score.Auto[0] > 0 ? 5 : 0);
            Score.DrawNumber(171, 750 + 32, $"{Score.Great[0],4}", 0);
            Score.DrawNumber(171, 750 + 32 * 2, $"{Score.Good[0],4}", 0);
            Score.DrawNumber(171, 750 + 32 * 3, $"{Score.Bad[0],4}", 0);
            Score.DrawNumber(171, 750 + 32 * 4, $"{Score.Poor[0],4}", 0);

            Score.DrawNumber(398, 960, $"{(Score.EXScore[0] > 0 ? Score.Roll[0] : Score.AutoRoll[0]),4}", Score.EXScore[0] == 0 && Score.AutoRoll[0] > 0 ? 5 : 0);

            if (PlayData.Data.IsPlay2P)
            {
                TextureLoad.Result_Panel.Draw(960, 0, new Rectangle(960, 0, 960, 1080));

                for (int i = 0; i < 100; i++)
                {
                    TextureLoad.Result_Gauge.Draw(1319 + 159 + 4 * i, 279, new Rectangle(Color2P[i], (int)Score.Gauge[1] > i ? 48 : 0, 3, 48));
                }
                TextureLoad.Result_Rank.Draw(1353 + 363, 461, new Rectangle(0, 45 * (int)Score.Rank[1], 161, 45));

                Score.DrawNumber(1319 + 52, 290, $"{(int)Score.Gauge[1],3}%", 0);
                Score.DrawNumber(1353 + 377, 541, $"{(Score.EXScore[1] > 0 ? Score.EXScore[1] : Score.Auto[1] * 2),5}", Score.EXScore[1] > 0 ? 6 : 7);
                Score.DrawNumber(1353 + 377, 541 + 73, $"{Score.Bad[1] + Score.Poor[1],5}", 6);

                Score.DrawNumber(1353 + 171, 750, $"{(Score.EXScore[1] > 0 ? Score.Perfect[1] : Score.Auto[1]),4}", Score.EXScore[1] == 0 && Score.Auto[1] > 0 ? 5 : 0);
                Score.DrawNumber(1353 + 171, 750 + 32, $"{Score.Great[1],4}", 0);
                Score.DrawNumber(1353 + 171, 750 + 32 * 2, $"{Score.Good[1],4}", 0);
                Score.DrawNumber(1353 + 171, 750 + 32 * 3, $"{Score.Bad[1],4}", 0);
                Score.DrawNumber(1353 + 171, 750 + 32 * 4, $"{Score.Poor[1],4}", 0);

                Score.DrawNumber(1353 + 398, 960, $"{(Score.EXScore[1] > 0 ? Score.Roll[1] : Score.AutoRoll[1]),4}", Score.EXScore[1] == 0 && Score.AutoRoll[1] > 0 ? 5 : 0);
            }

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
