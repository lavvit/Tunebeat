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
            Gauge = 0;
            SetGauge();

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
            DrawString(0, 300, $"SC:{EXScore}", 0xffffff);
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying) DrawString(80, 300, Remain > 0 ? $"MAX-{Remain}" : "MAX+0", 0xffffff);
            DrawString(0, 320, $"PG:{Perfect}", 0xffffff);
            DrawString(0, 340, $"GR:{Great}", 0xffffff);
            DrawString(0, 360, $"GD:{Good}", 0xffffff);
            DrawString(0, 380, $"BD:{Bad}", 0xffffff);
            DrawString(0, 400, $"PR:{Poor}", 0xffffff);
            DrawString(0, 420, $"AT:{Auto}", 0xffffff);
            DrawString(0, 440, $"RL:{Roll}({RollYellow},{RollBalloon})", 0xffffff);

            DrawString(200, 300, $"{Gauge}", 0xffffff);
            DrawString(200, 320, $"Total:{Total}", 0xffffff);

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
            Hit = Perfect + Great + Good + Bad + Poor + Auto;
            Remain = Hit * 2 - EXScore;
            JudgeCounter.Tick();
            base.Update();
        }

        public static void AddScore(EJudge judge)
        {
            AddGauge(judge);
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

        public static void AddGauge(EJudge judge)
        {
            double gauge = Total / Game.MainTJA.Courses[Game.Course].TotalNotes;
            switch (judge)
            {
                case EJudge.Good:
                    gauge *= GoodRate;
                    break;
                case EJudge.Bad:
                    gauge *= -BadRate;
                    break;
                case EJudge.Poor:
                    gauge *= -PoorRate;
                    break;
            }
            Gauge += gauge;

            if (Gauge >= 100.0)
            {
                Gauge = 100.0;
            }
            if (Gauge <= 0.0)
            {
                Gauge = 0.0;
            }
        }

        public static void SetGauge()
        {
            double total = 135;
            double goodrate = 0.5;
            double badrate = 2;
            double poorrate;
            switch (Game.Course)
            {
                case (int)ECourse.Easy:
                    {
                        goodrate = 0.75;
                        badrate = 0.5;
                        switch (Game.MainTJA.Courses[Game.Course].LEVEL)
                        {
                            case 1:
                                total = 166.666; //0.6
                                break;
                            case 2:
                            case 3:
                                total = 157.9; //0.63333
                                break;
                            case 4:
                            case 5:
                            default:
                                total = 136.36; //0.73333
                                break;
                        }

                    }
                    break;
                case (int)ECourse.Normal:
                    {
                        goodrate = 0.75;
                        switch (Game.MainTJA.Courses[Game.Course].LEVEL)
                        {
                            case 1:
                            case 2:
                                badrate = 0.5;
                                total = 152.333; //0.65
                                break;
                            case 3:
                                badrate = 0.5;
                                total = 143.8; //0.695
                                break;
                            case 4:
                                badrate = 0.75;
                                total = 142.2; //0.70333
                                break;
                            case 5:
                            case 6:
                            case 7:
                            default:
                                badrate = 1;
                                total = 133.333; //0.75
                                break;
                        }
                    }
                    break;
                case (int)ECourse.Hard:
                    {
                        goodrate = 0.75;
                        switch (Game.MainTJA.Courses[Game.Course].LEVEL)
                        {
                            case 1:
                            case 2:
                                badrate = 0.75;
                                total = 129; //0.775
                                break;
                            case 3:
                                badrate = 1;
                                total = 138; //0.725
                                break;
                            case 4:
                                badrate = 1.166;
                                total = 144.5; //0.691
                                break;
                            case 5:
                                badrate = 1.25;
                                total = 148.15; //0.675
                                break;
                            case 6:
                            case 7:
                            case 8:
                            default:
                                badrate = 1.25;
                                total = 145.45; //0.6875
                                break;
                        }
                    }
                    break;
                case (int)ECourse.Oni:
                case (int)ECourse.Edit:
                    {
                        goodrate = 0.5;
                        switch (Game.MainTJA.Courses[Game.Course].LEVEL)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                badrate = 1.6;
                                total = 141.4; //0.7075
                                break;
                            case 8:
                                badrate = 2;
                                total = 142.85; //0.7
                                break;
                            case 9:
                            case 10:
                            default:
                                badrate = 2;
                                total = 133.333; //0.75
                                break;
                        }
                    }
                    break;
            }
            poorrate = badrate;

            Total = Game.MainTJA.Courses[Game.Course].TOTAL > 0.0 ? Game.MainTJA.Courses[Game.Course].TOTAL : total;
            GoodRate = goodrate;
            BadRate = badrate;
            PoorRate = poorrate;
        }

        public static int EXScore, Perfect, Great, Good, Bad, Poor, Auto, Hit, Roll, RollYellow, RollBalloon, Remain;
        public static double msJudge, Gauge, Total, GoodRate, BadRate, PoorRate;
        private static EJudge DisplayJudge;
        private static Counter JudgeCounter;
    }
}
