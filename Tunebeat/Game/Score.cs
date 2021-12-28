using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class Score : Scene
    {
        public override void Enable()
        {
            for (int i = 0; i < 2; i++)
            {
                EXScore[i] = 0;
                Perfect[i] = 0;
                Great[i] = 0;
                Good[i] = 0;
                Bad[i] = 0;
                Poor[i] = 0;
                Auto[i] = 0;
                Roll[i] = 0;
                RollYellow[i] = 0;
                RollBalloon[i] = 0;
                Gauge[i] = 0;
                SetGauge(i);
            }

            JudgeCounter = new Counter(0, 200, 1000, false);
            JudgeCounter2P = new Counter(0, 200, 1000, false);
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
            DrawString(0, 300, $"SC:{EXScore[0]}", 0xffffff);
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying) DrawString(80, 300, Remain[0] > 0 ? $"MAX-{Remain[0]}" : "MAX+0", 0xffffff);
            DrawString(0, 320, $"PG:{Perfect[0]}", 0xffffff);
            DrawString(0, 340, $"GR:{Great[0]}", 0xffffff);
            DrawString(0, 360, $"GD:{Good[0]}", 0xffffff);
            DrawString(0, 380, $"BD:{Bad[0]}", 0xffffff);
            DrawString(0, 400, $"PR:{Poor[0]}", 0xffffff);
            DrawString(0, 420, $"AT:{Auto[0]}", 0xffffff);
            DrawString(0, 440, $"RL:{Roll[0]}({RollYellow[0]},{RollBalloon[0]})", 0xffffff);

            DrawString(200, 300, $"{Gauge[0]}", 0xffffff);
            DrawString(200, 320, $"Total:{Total[0]}", 0xffffff);

            if (JudgeCounter.State == TimerState.Started)
            {
                DrawString(600, 260, $"{DisplayJudge[0]}", 0xffffff);
                DrawString(600, 280, $"{msJudge[0]}", 0xffffff);
            }

            if (PlayData.IsPlay2P)
            {
                DrawString(0, 560, $"SC:{EXScore[1]}", 0xffffff);
                if (Game.IsSongPlay && !Game.MainSong.IsPlaying) DrawString(80, 560, Remain[1] > 0 ? $"MAX-{Remain[1]}" : "MAX+0", 0xffffff);
                DrawString(0, 580, $"PG:{Perfect[1]}", 0xffffff);
                DrawString(0, 600, $"GR:{Great[1]}", 0xffffff);
                DrawString(0, 620, $"GD:{Good[1]}", 0xffffff);
                DrawString(0, 640, $"BD:{Bad[1]}", 0xffffff);
                DrawString(0, 660, $"PR:{Poor[1]}", 0xffffff);
                DrawString(0, 680, $"AT:{Auto[1]}", 0xffffff);
                DrawString(0, 700, $"RL:{Roll[1]}({RollYellow[1]},{RollBalloon[1]})", 0xffffff);

                DrawString(200, 560, $"{Gauge[1]}", 0xffffff);
                DrawString(200, 580, $"Total:{Total[1]}", 0xffffff);

                if (JudgeCounter2P.State == TimerState.Started)
                {
                    DrawString(600, 520, $"{DisplayJudge[1]}", 0xffffff);
                    DrawString(600, 540, $"{msJudge[1]}", 0xffffff);
                }
            }
            #endif
            base.Draw();
        }

        public override void Update()
        {
            Hit[0] = Perfect[0] + Great[0] + Good[0] + Bad[0] + Poor[0] + Auto[0];
            Remain[0] = Hit[0] * 2 - EXScore[0];
            if (PlayData.IsPlay2P)
            {
                Hit[1] = Perfect[1] + Great[1] + Good[1] + Bad[1] + Poor[1] + Auto[1];
                Remain[1] = Hit[1] * 2 - EXScore[1];
            }
            JudgeCounter.Tick();
            JudgeCounter2P.Tick();
            base.Update();
        }

        public static void AddScore(EJudge judge, int player)
        {
            AddGauge(judge, player);
            DisplayJudge[player] = judge;

            if (player == 0)
            {
                JudgeCounter.Reset();
                JudgeCounter.Start();
            }
            else
            {
                JudgeCounter2P.Reset();
                JudgeCounter2P.Start();
            }
            
            switch (judge)
            {
                case EJudge.Perfect:
                    Perfect[player]++;
                    EXScore[player] += 2;
                    break;
                case EJudge.Great:
                    Great[player]++;
                    EXScore[player] += 2;
                    break;
                case EJudge.Good:
                    Good[player]++;
                    EXScore[player]++;
                    break;
                case EJudge.Bad:
                    Bad[player]++;
                    break;
                case EJudge.Poor:
                case EJudge.Through:
                    Poor[player]++;
                    break;
                case EJudge.Auto:
                    Auto[player]++;
                    break;
                default:
                    break;
            }
        }

        public static void AddRoll(int player)
        {
            Roll[player]++;
            RollYellow[player]++;
        }
        public static void AddBalloon(int player)
        {
            Roll[player]++;
            RollBalloon[player]++;
        }

        public static void AddGauge(EJudge judge, int player)
        {
            double[] gauge = new double[2] { Total[0] / Game.MainTJA.Courses[Game.Course[0]].TotalNotes, Total[1] / Game.MainTJA.Courses[Game.Course[1]].TotalNotes };
            switch (judge)
            {
                case EJudge.Good:
                    gauge[player] *= GoodRate[player];
                    break;
                case EJudge.Bad:
                    gauge[player] *= -BadRate[player];
                    break;
                case EJudge.Poor:
                    gauge[player] *= -PoorRate[player];
                    break;
            }
            Gauge[player] += gauge[player];

            if (Gauge[player] >= 100.0)
            {
                Gauge[player] = 100.0;
            }
            if (Gauge[player] <= 0.0)
            {
                Gauge[player] = 0.0;
            }
        }

        public static void SetGauge(int player)
        {
            double[] total = new double[2] { 135, 135 };
            double[] goodrate = new double[2] { 0.5, 0.5 };
            double[] badrate = new double[2] { 2.0, 2.0 };
            double[] poorrate = new double[2];
            switch (Game.Course[player])
            {
                case (int)ECourse.Easy:
                    {
                        goodrate[player] = 0.75;
                        badrate[player] = 0.5;
                        switch (Game.MainTJA.Courses[Game.Course[player]].LEVEL)
                        {
                            case 1:
                                total[player] = 166.666; //0.6
                                break;
                            case 2:
                            case 3:
                                total[player] = 157.9; //0.63333
                                break;
                            case 4:
                            case 5:
                            default:
                                total[player] = 136.36; //0.73333
                                break;
                        }

                    }
                    break;
                case (int)ECourse.Normal:
                    {
                        goodrate[player] = 0.75;
                        switch (Game.MainTJA.Courses[Game.Course[player]].LEVEL)
                        {
                            case 1:
                            case 2:
                                badrate[player] = 0.5;
                                total[player] = 152.333; //0.65
                                break;
                            case 3:
                                badrate[player] = 0.5;
                                total[player] = 143.8; //0.695
                                break;
                            case 4:
                                badrate[player] = 0.75;
                                total[player] = 142.2; //0.70333
                                break;
                            case 5:
                            case 6:
                            case 7:
                            default:
                                badrate[player] = 1;
                                total[player] = 133.333; //0.75
                                break;
                        }
                    }
                    break;
                case (int)ECourse.Hard:
                    {
                        goodrate[player] = 0.75;
                        switch (Game.MainTJA.Courses[Game.Course[player]].LEVEL)
                        {
                            case 1:
                            case 2:
                                badrate[player] = 0.75;
                                total[player] = 129; //0.775
                                break;
                            case 3:
                                badrate[player] = 1;
                                total[player] = 138; //0.725
                                break;
                            case 4:
                                badrate[player] = 1.166;
                                total[player] = 144.5; //0.691
                                break;
                            case 5:
                                badrate[player] = 1.25;
                                total[player] = 148.15; //0.675
                                break;
                            case 6:
                            case 7:
                            case 8:
                            default:
                                badrate[player] = 1.25;
                                total[player] = 145.45; //0.6875
                                break;
                        }
                    }
                    break;
                case (int)ECourse.Oni:
                case (int)ECourse.Edit:
                    {
                        goodrate[player] = 0.5;
                        switch (Game.MainTJA.Courses[Game.Course[player]].LEVEL)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                badrate[player] = 1.6;
                                total[player] = 141.4; //0.7075
                                break;
                            case 8:
                                badrate[player] = 2;
                                total[player] = 142.85; //0.7
                                break;
                            case 9:
                            case 10:
                            default:
                                badrate[player] = 2;
                                total[player] = 133.333; //0.75
                                break;
                        }
                    }
                    break;
            }
            poorrate = badrate;

            Total[player] = Game.MainTJA.Courses[Game.Course[player]].TOTAL > 0.0 ? Game.MainTJA.Courses[Game.Course[player]].TOTAL : total[player];
            GoodRate[player] = goodrate[player];
            BadRate[player] = badrate[player];
            PoorRate[player] = poorrate[player];
        }

        public static int[] EXScore = new int[2], Perfect = new int[2], Great = new int[2], Good = new int[2], Bad = new int[2], Poor = new int[2], Auto = new int[2],
            Hit = new int[2], Roll = new int[2], RollYellow = new int[2], RollBalloon = new int[2], Remain = new int[2];
        public static double[] msJudge = new double[2], Gauge = new double[2], Total = new double[2], GoodRate = new double[2], BadRate = new double[2], PoorRate = new double[2];
        private static EJudge[] DisplayJudge = new EJudge[2];
        private static Counter JudgeCounter, JudgeCounter2P;
    }
}
