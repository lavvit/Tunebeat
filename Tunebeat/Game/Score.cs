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
            Roll = 0;
            RollYellow = 0;
            RollBalloon = 0;
            JudgeCounter = new Counter(0, 200, 1000, false);
            base.Enable();
        }

        public override void Disable()
        {
            JudgeCounter.Reset();
            base.Disable();
        }
        public override void Draw()
        {
            #if DEBUG
            DrawString(0, 300, "SC:" + $"{EXScore}", 0xffffff);
            int maxremain = Game.MainTJA.Courses[Game.Course].TotalNotes - EXScore;
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying) DrawString(60, 300, maxremain > 0 ? $"MAX-{maxremain}" : "MAX+0", 0xffffff);
            DrawString(0, 320, "PG:" + $"{Perfect}", 0xffffff);
            DrawString(0, 340, "GR:" + $"{Great}", 0xffffff);
            DrawString(0, 360, "GD:" + $"{Good}", 0xffffff);
            DrawString(0, 380, "BD:" + $"{Bad}", 0xffffff);
            DrawString(0, 400, "PR:" + $"{Poor}", 0xffffff);
            DrawString(0, 420, "AT:" + $"{Auto}", 0xffffff);
            DrawString(0, 440, "RL:" + $"{Roll}({RollYellow},{RollBalloon})", 0xffffff);

            if (JudgeCounter.State == TimerState.Started)
            {
                DrawString(600, 260, $"{DisplayJudge}", 0xffffff);
                DrawString(600, 280, $"{msJudge}", 0xffffff);
            }
            #endif
            base.Draw();
        }

        public override void Update()
        {
            JudgeCounter.Tick();
            base.Update();
        }

        public static void AddScore(EJudge judge)
        {
            DisplayJudge = judge;
            JudgeCounter.Reset();
            JudgeCounter.Start();
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

        public static void AddRoll()
        {
            Roll++;
            RollYellow++;
        }
        public static void AddBalloon()
        {
            Roll++;
            RollBalloon++;
        }

        public static int EXScore, Perfect, Great, Good, Bad, Poor, Auto, Roll, RollYellow, RollBalloon;
        public static double msJudge;
        private static EJudge DisplayJudge;
        private static Counter JudgeCounter;
    }
}
