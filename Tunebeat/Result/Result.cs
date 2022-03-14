using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using SeaDrop;

namespace Tunebeat
{
    public class Result : Scene
    {
        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            PlayMemory.Dispose();
            base.Disable();
        }
        public override void Draw()
        {
            Drawing.Box(0, 0, 1919, 257, Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
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
            ReplayData data = PlayMemory.BestData;
            if (data != null)
            {
                TextureLoad.Result_Rank.Draw(363 - 165, 461, new Rectangle(0, 45 * (int)Score.GetRank(data.Score, 0), 161, 45));
                Score.DrawNumber(377 - 165, 541, $"{data.Score,5}", 0);
                Score.DrawNumber(377 - 165, 541 + 73, $"{data.Bad + data.Poor,5}", 0);

                int plusscore = (Score.EXScore[0] > 0 ? Score.EXScore[0] : Score.Auto[0] * 2) - data.Score;
                Score.DrawMiniNumber(377 + 40, 573, plusscore < 0 ? $"{plusscore,5}" : $"{"+" + plusscore,5}", plusscore < 0 ? 1 : 0);
                int plusmiss = Score.Bad[0] + Score.Poor[0] - (data.Bad + data.Poor);
                Score.DrawMiniNumber(377 + 40, 573 + 73, plusmiss < 0 ? $"{plusmiss,5}" : $"{"+" + plusmiss,5}", plusmiss < 0 ? 0 : 1);
            }

            Score.DrawNumber(171, 750, $"{(Score.EXScore[0] > 0 ? Score.Perfect[0] : Score.Auto[0]),4}", Score.EXScore[0] == 0 && Score.Auto[0] > 0 ? 5 : 0);
            Score.DrawNumber(171, 750 + 32, $"{Score.Great[0],4}", 0);
            Score.DrawNumber(171, 750 + 32 * 2, $"{Score.Good[0],4}", 0);
            Score.DrawNumber(171, 750 + 32 * 3, $"{Score.Bad[0],4}", 0);
            Score.DrawNumber(171, 750 + 32 * 4, $"{Score.Poor[0],4}", 0);
            List<ChipData> chipdata = PlayMemory.ChipData;
            int pgfast = 0, pgrate = 0, grfast = 0, grrate = 0, gdfast = 0, gdrate = 0, bdfast = 0, bdrate = 0, prfast = 0, prrate = 0;
            for (int i = 0; i < chipdata.Count; i++)
            {
                switch (chipdata[i].judge)
                {
                    case EJudge.Perfect:
                    case EJudge.Auto:
                        if (chipdata[i].Time < chipdata[i].Chip.Time) pgfast++;
                        else pgrate++;
                        break;
                    case EJudge.Great:
                        if (chipdata[i].Time < chipdata[i].Chip.Time) grfast++;
                        else grrate++;
                        break;
                    case EJudge.Good:
                        if (chipdata[i].Time < chipdata[i].Chip.Time) gdfast++;
                        else gdrate++;
                        break;
                    case EJudge.Bad:
                        if (chipdata[i].Time < chipdata[i].Chip.Time) bdfast++;
                        else bdrate++;
                        break;
                    case EJudge.Poor:
                        if (chipdata[i].Time < chipdata[i].Chip.Time) prfast++;
                        else prrate++;
                        break;

                }
            }
            Score.DrawNumber(171 + 120, 750, $"{pgfast,4}", 6);
            Score.DrawNumber(171 + 120, 750 + 32, $"{grfast,4}", 6);
            Score.DrawNumber(171 + 120, 750 + 32 * 2, $"{gdfast,4}", 6);
            Score.DrawNumber(171 + 120, 750 + 32 * 3, $"{bdfast,4}", 6);
            Score.DrawNumber(171 + 120, 750 + 32 * 4, $"{prfast,4}", 6);
            Score.DrawNumber(171 + 120 * 2, 750, $"{pgrate,4}", 7);
            Score.DrawNumber(171 + 120 * 2, 750 + 32, $"{grrate,4}", 7);
            Score.DrawNumber(171 + 120 * 2, 750 + 32 * 2, $"{gdrate,4}", 7);
            Score.DrawNumber(171 + 120 * 2, 750 + 32 * 3, $"{bdrate,4}", 7);
            Score.DrawNumber(171 + 120 * 2, 750 + 32 * 4, $"{prrate,4}", 7);

            int fast = pgfast + grfast + gdfast + bdfast + prfast;
            int rate = pgrate + grrate + gdrate + bdrate + prrate;
            double perfast = fast + rate > 0 ? fast / (double)(fast + rate) : 0;
            double perrate = fast + rate > 0 ? rate / (double)(fast + rate) : 0;
            TextureLoad.Result_FastRate.Draw(119, 949, new Rectangle(0, 0, (int)(200.0 * perfast), 16));
            TextureLoad.Result_FastRate.Draw(119, 969, new Rectangle(0, 16, (int)(200.0 * perrate), 16));

            Score.DrawNumber(398, 960, $"{(Score.EXScore[0] > 0 ? Score.Roll[0] : Score.AutoRoll[0]),4}", Score.EXScore[0] == 0 && Score.AutoRoll[0] > 0 ? 5 : 0);

            if (Game.Play2P)
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
            if (Key.IsPushed(EKey.Esc) || Key.IsPushed(EKey.Enter) || Key.IsPushed(PlayData.Data.LEFTDON) || Key.IsPushed(PlayData.Data.RIGHTDON) || Mouse.IsPushed(MouseButton.Left))
            {
                SoundLoad.Don[0].Volume = PlayData.Data.SE / 100.0;
                SoundLoad.Don[0].Play();
                Program.SceneChange(new SongSelect());
            }
            if (Key.IsPushed(PlayData.Data.PlayReset))
            {
                SoundLoad.Ka[0].Volume = PlayData.Data.SE / 100.0;
                SoundLoad.Ka[0].Play();
                Program.SceneChange(new Game());
            }

            if (Key.IsPushed(PlayData.Data.SaveReplay) && Game.PlayMeasure == 0)
            {
                if (!Game.IsReplay[0]) PlayMemory.SaveData(0);
                if (Game.Play2P && !Game.IsReplay[1]) PlayMemory.SaveData(1);
            }

            base.Update();
        }
    }
}
