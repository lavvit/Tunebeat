using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;

namespace Tunebeat.Game
{
    public class Score : Scene
    {
        public override void Enable()
        {
            EXScore = 0;
            Perfect = 0;
            Great = 0;
            Good = 0;
            Bad = 0;
            Poor = 0;
            Auto = 0;
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }
        public override void Draw()
        {
            #if DEBUG
            DrawString(0, 300, "SC:" + $"{EXScore}", 0xffffff);
            DrawString(0, 320, "PG:" + $"{Perfect}", 0xffffff);
            DrawString(0, 340, "GR:" + $"{Great}", 0xffffff);
            DrawString(0, 360, "GD:" + $"{Good}", 0xffffff);
            DrawString(0, 380, "BD:" + $"{Bad}", 0xffffff);
            DrawString(0, 400, "PR:" + $"{Poor}", 0xffffff);
            DrawString(0, 420, "AT:" + $"{Auto}", 0xffffff);
            #endif
            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }

        public static void AddScore(EJudge judge)
        {
            switch (judge)
            {
                case EJudge.Perfect:
                    Perfect++;
                    EXScore += 2;
                    break;
                case EJudge.Great:
                    Great++;
                    EXScore += 2;
                    break;
                case EJudge.Good:
                    Good++;
                    EXScore++;
                    break;
                case EJudge.Bad:
                    Bad++;
                    break;
                case EJudge.Poor:
                case EJudge.Through:
                    Poor++;
                    break;
                case EJudge.Auto:
                    Auto++;
                    break;
                default:
                    break;
            }
        }

        public static int EXScore, Perfect, Great, Good, Bad, Poor, Auto;
    }
}
