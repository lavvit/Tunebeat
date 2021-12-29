using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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
            TextureLoad.Game_Gauge_Base.Draw(495, 286 - 48, new Rectangle(0, 0, 1425, 48));

            int[] Color = new int[100];
            int[] Color2P = new int[100];
            for (int i = 0; i < 100; i++)
            {
                Color[i] = i >= ClearRate[0][(int)GaugeType[0]] - 1 ? (GaugeType[0] >= EGauge.Hard ? 10 * (int)GaugeType[0] : 30) : 10 * (int)GaugeType[0];
                Color2P[i] = i >= ClearRate[1][(int)GaugeType[1]] - 1 ? (GaugeType[1] >= EGauge.Hard ? 10 * (int)GaugeType[1] : 30) : 10 * (int)GaugeType[1];
            }

            for (int i = 0; i < 100; i++)
            {
                TextureLoad.Game_Gauge.Draw(495 + 179 + 12 * i, 286 - 48 + 6, new Rectangle(Color[i], (int)Gauge[0] > i ? 40 : 0, 10, 40));
            }
            if (PlayData.IsPlay2P)
            {
                TextureLoad.Game_Gauge_Base.Draw(495, 286 + 465, new Rectangle(0, 48, 1425, 48));
                for (int i = 0; i < 100; i++)
                {
                    TextureLoad.Game_Gauge.Draw(495 + 179 + 12 * i, 286 + 465 + 2, new Rectangle(Color2P[i], (int)Gauge[1] > i ? 40 : 0, 10, 40));
                }
            }

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
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying) DrawString(200, 340, Cleared[0] ? "Cleared" : "Failed", 0xffffff);

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
                if (Game.IsSongPlay && !Game.MainSong.IsPlaying) DrawString(200, 600, Cleared[0] ? "Cleared" : "Failed", 0xffffff);

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

            Gauge[0] = GaugeList[0][(int)GaugeType[0]];
            if (PlayData.IsPlay2P)
            {
                Gauge[1] = GaugeList[1][(int)GaugeType[1]];
            }

            for (int player = 0; player < 2; player++)
            {
                Cleared[player] = Gauge[player] >= ClearRate[player][(int)GaugeType[player]] ? true : false;
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
                    EXScore[player] += 1;
                    break;
                case EJudge.Good:
                    Good[player]++;
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
            double[] gaugepernote = new double[2] { Total[0] / Game.MainTJA.Courses[Game.Course[0]].TotalNotes, Total[1] / Game.MainTJA.Courses[Game.Course[1]].TotalNotes };
            double[] gauge = new double[6];
            int Notes = PlayData.Hazard[player] > Game.MainTJA.Courses[Game.Course[player]].TotalNotes ? Game.MainTJA.Courses[Game.Course[player]].TotalNotes : PlayData.Hazard[player];
            switch (judge)
            {
                case EJudge.Perfect:
                case EJudge.Great:
                case EJudge.Auto:
                    for (int i = 0; i < 6; i++)
                    {
                        gauge[i] += gaugepernote[player];
                    }
                    gauge[3] = GaugeList[player][3] > 0.0 ? 0.16 : 0; gauge[4] = GaugeList[player][4] > 0.0 ? 0.16 : 0; gauge[5] = 0;
                    break;

                case EJudge.Good:
                    gaugepernote[player] *= GoodRate[player];
                    for (int i = 0; i < 6; i++)
                    {
                        gauge[i] += gaugepernote[player];
                    }
                    gauge[3] = 0; gauge[4] = 0; gauge[5] = 0;
                    break;

                case EJudge.Bad:
                    gaugepernote[player] *= -BadRate[player];
                    for (int i = 0; i < 6; i++)
                    {
                        gauge[i] += gaugepernote[player];
                    }
                    gauge[1] *= 0.5; gauge[2] *= 0.8; gauge[3] = GaugeList[player][3] < 30.0 ? -4.8 : -9.6; gauge[4] = -20.0; gauge[5] = -100.0 / Notes;
                    break;

                case EJudge.Poor:
                case EJudge.Through:
                    gaugepernote[player] *= -PoorRate[player];
                    for (int i = 0; i < 6; i++)
                    {
                        gauge[i] += gaugepernote[player];
                    }
                    gauge[1] *= 0.5; gauge[2] *= 0.8; gauge[3] = GaugeList[player][3] < 30.0 ? -4.8 : -9.6; gauge[4] = -20.0; gauge[5] = -100.0 / Notes;
                    break;
            }
            for (int i = 0; i < 6; i++)
            {
                GaugeList[player][i] += gauge[i];
                if (GaugeList[player][i] >= 100.0)
                {
                    GaugeList[player][i] = 100.0;
                }
                if (GaugeList[player][i] <= 0.0)
                {
                    GaugeList[player][i] = 0.0;
                }
                if (GaugeList[player][5] < 1.0)
                {
                    GaugeList[player][5] = 0.0;
                }
            }
            
            switch ((EGaugeAutoShift)PlayData.GaugeAutoShift[player])
            {
                case EGaugeAutoShift.ToNormal:
                    if ((PlayData.GaugeType[player] >= (int)EGauge.Hard && PlayData.GaugeType[player] <= (int)EGauge.Hazard) && GaugeList[player][(int)GaugeType[player]] <= 0.0)
                    {
                        GaugeType[player] = EGauge.Normal;
                    }
                    break;
                case EGaugeAutoShift.Best:
                    if ((GaugeType[player] == EGauge.Hazard) && GaugeList[player][(int)GaugeType[player]] <= 0.0)
                    {
                        GaugeType[player] = EGauge.EXHard;
                    }
                    if ((GaugeType[player] == EGauge.Hard || GaugeType[player] == EGauge.EXHard) && GaugeList[player][(int)GaugeType[player]] <= 0.0)
                    {
                        GaugeType[player] -= 1;
                    }
                    if (GaugeType[player] < EGauge.Hard)
                    {
                        switch (PlayData.GaugeAutoShiftMin[player])
                        {
                            case 0:
                                GaugeType[player] = EGauge.Normal;
                                break;
                            case 1:
                                if (GaugeList[player][0] >= 80.0)
                                {
                                    GaugeType[player] = EGauge.Normal;
                                }
                                else if (GaugeList[player][2] >= 80.0)
                                {
                                    GaugeType[player] = EGauge.Easy;
                                }
                                else
                                {
                                    GaugeType[player] = EGauge.Assist;
                                }
                                break;
                            case 2:
                                if (GaugeList[player][0] >= 80.0)
                                {
                                    GaugeType[player] = EGauge.Normal;
                                }
                                else
                                {
                                    GaugeType[player] = EGauge.Easy;
                                }

                                break;
                        }
                    }
                    break;
                case EGaugeAutoShift.LessBest:
                    if ((GaugeType[player] == EGauge.Hazard) && GaugeList[player][(int)GaugeType[player]] <= 0.0)
                    {
                        GaugeType[player] = (EGauge)PlayData.GaugeType[player];
                    }
                    if ((GaugeType[player] == EGauge.Hard || GaugeType[player] == EGauge.EXHard) && GaugeList[player][(int)GaugeType[player]] <= 0.0)
                    {
                        GaugeType[player] -= 1;
                    }
                    if (GaugeType[player] < EGauge.Hard)
                    {
                        switch (PlayData.GaugeAutoShiftMin[player])
                        {
                            case 0:
                                GaugeType[player] = EGauge.Normal;
                                break;
                            case 1:
                                if (GaugeList[player][0] >= 80.0 && PlayData.GaugeType[player] != (int)EGauge.Assist && PlayData.GaugeType[player] != (int)EGauge.Easy)
                                {
                                    GaugeType[player] = EGauge.Normal;
                                }
                                else if (GaugeList[player][2] >= 80.0 && PlayData.GaugeType[player] != (int)EGauge.Assist)
                                {
                                    GaugeType[player] = EGauge.Easy;
                                }
                                else
                                {
                                    GaugeType[player] = EGauge.Assist;
                                }
                                break;
                            case 2:
                                if (GaugeList[player][0] >= 80.0 && PlayData.GaugeType[player] != (int)EGauge.Assist && PlayData.GaugeType[player] != (int)EGauge.Easy)
                                {
                                    GaugeType[player] = EGauge.Normal;
                                }
                                else
                                {
                                    GaugeType[player] = EGauge.Easy;
                                }
                                
                                break;
                        }
                    }
                    break;
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

            EGauge min = (EGauge)PlayData.GaugeAutoShiftMin[player];
            GaugeType[player] = (EGauge)PlayData.GaugeType[player];
            switch ((EGaugeAutoShift)PlayData.GaugeAutoShift[player])
            {
                case EGaugeAutoShift.Best:
                    GaugeType[player] = EGauge.EXHard;
                    break;
                case EGaugeAutoShift.LessBest:
                    if (PlayData.GaugeType[player] >= (int)EGauge.Normal && PlayData.GaugeType[player] <= (int)EGauge.Easy)
                    {
                        GaugeType[player] = min;
                    }
                    break;
            }
            GaugeType[player] = PlayData.Hazard[player] >= 1 ? EGauge.Hazard : GaugeType[player];
            GaugeList[player] = new double[6] { 0.0, 0.0, 0.0, 100.0, 100.0, 100.0 };
            ClearRate[player] = new double[6] { 80.0, 60.0, 80.0, 0.01, 0.01, 0.01 };

            if (PlayData.GaugeType[player] >= (int)EGauge.Hard && PlayData.GaugeType[player] <= (int)EGauge.Hazard)
            {
                Gauge[player] = 100.0;
            }
            else
            {
                Gauge[player] = 0;
            }
        }

        public static EGauge[] GaugeType = new EGauge[2];
        public static int[] EXScore = new int[2], Perfect = new int[2], Great = new int[2], Good = new int[2], Bad = new int[2], Poor = new int[2], Auto = new int[2],
            Hit = new int[2], Roll = new int[2], RollYellow = new int[2], RollBalloon = new int[2], Remain = new int[2];
        public static double[] msJudge = new double[2], Gauge = new double[2], Total = new double[2], GoodRate = new double[2], BadRate = new double[2], PoorRate = new double[2];
        public static double[][] GaugeList = new double[2][], ClearRate = new double[2][];
        public static bool[] Cleared = new bool[2];
        private static EJudge[] DisplayJudge = new EJudge[2];
        private static Counter JudgeCounter, JudgeCounter2P;
    }

    public enum EGauge
    {
        Normal,
        Assist,
        Easy,
        Hard,
        EXHard,
        Hazard
    }

    public enum EGaugeAutoShift
    {
        None,
        Continue,
        Retry,
        ToNormal,
        Best,
        LessBest
    }
}
