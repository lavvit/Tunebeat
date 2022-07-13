using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;
using BMSParse;

namespace Tunebeat
{
    public class BScore
    {
        public static BScoreBoard[] ScoreBoard = new BScoreBoard[2];
        public static EJudge[] Judge = new EJudge[2];
        public static EGauge[] GaugeType = new EGauge[2];
        public static double[] Total = new double[2], Pushms = new double[2];
        public static double[][] GaugeList = new double[2][], ClearRate = new double[2][];


        public static void Init()
        {
            for (int i = 0; i < 2; i++)
            {
                ScoreBoard[i] = new BScoreBoard();
                Pushms[i] = 0;
                SetGauge(i);
            }
        }

        public static void Draw()
        {
            int[] ScoreX = new int[2] { 540 + (BGame.NowCourse[0].Player == 2 ? 384 : 0), 1280 };
            for (int i = 0; i < 2; i++)
            {
                if (i == 0 || BGame.Play2P)
                {
                    int p = i == 0 && PlayData.Data.Is2PSide ? 1 : i;
                    Drawing.Text(p == 1 ? 1600 : 240, 620, $"{Pushms[i]}", Pushms[i] >= 16 ? 0xff0000 : (Pushms[i] <= -16 ? 0x0000ff : 0xffffff));
                    Drawing.Text(p == 1 ? 1600 : 240, 640, $"{Judge[i]} {ScoreBoard[i].Combo}  {ScoreBoard[i].Score}");

                    DrawGauge(i);

                    Drawing.Text(ScoreX[p], 720, $"AT:{ScoreBoard[i].Auto}");
                    Drawing.Text(ScoreX[p], 740, $"PR:{ScoreBoard[i].Perfect}");
                    Drawing.Text(ScoreX[p], 760, $"GR:{ScoreBoard[i].Great}");
                    Drawing.Text(ScoreX[p], 780, $"GD:{ScoreBoard[i].Good}");
                    Drawing.Text(ScoreX[p], 800, $"BD:{ScoreBoard[i].Bad}");
                    Drawing.Text(ScoreX[p], 820, $"PR:{ScoreBoard[i].Through}");
                    Drawing.Text(ScoreX[p], 840, $"EP:{ScoreBoard[i].Poor}");
                    Drawing.Text(ScoreX[p], 880, $"BP:{ScoreBoard[i].BadPoor()}");
                    Drawing.Text(ScoreX[p], 900, $"CB:{ScoreBoard[i].ComboBreak()}");
                    Drawing.Text(ScoreX[p], 940, $"HT:{ScoreBoard[i].Hit()}");

                    Drawing.Text(ScoreX[p], 480, $"TO:{Total[i]}");

                    for (int j = 0; j < 6; j++)
                    {
                        Drawing.Text(ScoreX[p], 520 + 20 * j, $"{(EGauge)j}:{GaugeList[i][j]}");
                    }
                }
            }
        }

        public static void Update()
        {
            for (int i = 0; i < 2; i++)
            {
                ScoreBoard[i].Gauge = Math.Round(ScoreBoard[i].Gauge, 8, MidpointRounding.AwayFromZero);
                if (ScoreBoard[i].Gauge > 100.0) ScoreBoard[i].Gauge = 100;
                if (ScoreBoard[i].Gauge < 0.0) ScoreBoard[i].Gauge = 0;
            }
        }

        public static void DrawGauge(int player)
        {
            int p = player == 0 && PlayData.Data.Is2PSide ? 1 : player;
            int[] GaugeX = new int[2] { 39 + (BGame.NowCourse[0].Player == 2 ? 384 : 0), 1431 };
            Drawing.Box(GaugeX[p] - 4, 885, 458, 39, 0);
            Drawing.Box(GaugeX[p], 889, 450, 31, 0x400000);
            int[] Color = new int[6] { 0x00AAFF, 0x5500ff, 0x00ff00, 0xff0000, 0xffaa00, 0xc0c0c0};
            Drawing.Box(GaugeX[p], 889, 9 * (ScoreBoard[player].Gauge / 2), 31, Color[(int)GaugeType[player]]);
            Drawing.Text(GaugeX[p], 869, $"{ScoreBoard[player].Gauge:0.00}%");
        }

        public static void ScoreSet(Chip chip, int player, int type = 0)
        {
            if (chip == null) return;
            if (chip.IsHit || chip.IsMiss) return;

            double dif = BGame.Timer.Value - (type == 2 ? chip.LongEnd.Time + BGame.Adjust[player] : chip.Time + BGame.Adjust[player]);
            if (type == 3) dif = chip.PushTime - (chip.Time + BGame.Adjust[player]);
            EJudge judge = BProcess.GetJudge(dif);

            ScoreSet(judge, player, type);
            Pushms[player] = Math.Round(dif, 2, MidpointRounding.AwayFromZero);
            if (chip.LongEnd == null || PlayData.Data.LNType > 0 || judge < EJudge.Good)
            {
                //if (type == 2) chip.LongEnd.Judge = judge;
                //else chip.Judge = judge;

                switch (judge)
                {
                    case EJudge.Perfect:
                    case EJudge.Great:
                    case EJudge.Good:
                    case EJudge.Bad:
                        if (type != 1) chip.IsHit = true;
                        break;
                    case EJudge.Poor:
                        if (type == 2)
                        {
                            chip.IsMiss = true;
                            chip.LongEnd.IsMiss = true;
                            BGame.ChargeSound[player][BCourse.GetLane(chip)].Stop();
                        }
                        break;
                }
            }

        }
        public static void ScoreSet(EJudge judge, int player, int type = 0)
        {
            Judge[player] = judge;
            switch (judge)
            {
                case EJudge.Auto:
                    ScoreBoard[player].Auto++;
                    ScoreBoard[player].Score += 2;
                    ScoreBoard[player].Combo++;
                    break;
                case EJudge.Perfect:
                    ScoreBoard[player].Perfect++;
                    ScoreBoard[player].Score += 2;
                    ScoreBoard[player].Combo++;
                    break;
                case EJudge.Great:
                    ScoreBoard[player].Great++;
                    ScoreBoard[player].Score++;
                    ScoreBoard[player].Combo++;
                    break;
                case EJudge.Good:
                    ScoreBoard[player].Good++;
                    ScoreBoard[player].Combo++;
                    break;
                case EJudge.Bad:
                    ScoreBoard[player].Bad++;
                    ScoreBoard[player].Combo = 0;
                    break;
                case EJudge.Poor:
                    if (type == 2)
                    {
                        Judge[player] = EJudge.Through;
                        ScoreBoard[player].Through++;
                        ScoreBoard[player].Combo = 0;
                    }
                    else ScoreBoard[player].Poor++;
                    break;
                case EJudge.Through:
                    ScoreBoard[player].Through++;
                    ScoreBoard[player].Combo = 0;
                    break;
            }
            AddGauge(judge, player);
        }

        public static void SetGauge(int player)
        {
            double total = 260;
            if (BGame.BMS[player].Course.TotalNotes < 1)
            {
                total = 0;
            }
            else if (BGame.BMS[player].Course.TotalNotes < 400)
            {
                total = 200 + (BGame.BMS[player].Course.TotalNotes / 5);
            }
            else if (BGame.BMS[player].Course.TotalNotes < 600)
            {
                total = 280 + ((BGame.BMS[player].Course.TotalNotes - 400) / 2.5);
            }
            else if (BGame.BMS[player].Course.TotalNotes >= 600)
            {
                total = 360 + ((BGame.BMS[player].Course.TotalNotes - 600) / 5);
            }
            //total = 7.605 * BGame.BMS[player].Course.TotalNotes / (0.01 * BGame.BMS[player].Course.TotalNotes + 6.5);

            Total[player] = BGame.BMS[player].Course.Total > 0.0 ? BGame.BMS[player].Course.Total : total;

            EGauge min = (EGauge)PlayData.Data.GaugeAutoShiftMin[player];
            GaugeType[player] = (EGauge)PlayData.Data.GaugeType[player];
            switch ((EGaugeAutoShift)PlayData.Data.GaugeAutoShift[player])
            {
                case EGaugeAutoShift.Best:
                    GaugeType[player] = EGauge.EXHard;
                    break;
                case EGaugeAutoShift.LessBest:
                    if (PlayData.Data.GaugeType[player] >= (int)EGauge.Normal && PlayData.Data.GaugeType[player] <= (int)EGauge.Easy)
                    {
                        GaugeType[player] = min;
                    }
                    break;
            }
            GaugeType[player] = PlayData.Data.Hazard[player] >= 1 ? EGauge.Hazard : GaugeType[player];
            GaugeList[player] = new double[6] { 20.0, 20.0, 20.0, 100.0, 100.0, 100.0 };
            ClearRate[player] = new double[6] { 80.0, 60.0, 80.0, 0.01, 0.01, 0.01 };
            ScoreBoard[player].Gauge = GaugeList[player][(int)GaugeType[player]];
        }
        public static void AddGauge(EJudge judge, int player)
        {
            if (BGame.NowState != EState.Play && GaugeType[player] >= EGauge.Hard) return;

            double gaugepernote = Total[player] / BGame.BMS[player].Course.TotalNotes;
            double[] gauge = new double[6];
            int t = BGame.BMS[player].Course.TotalNotes;
            int Notes = PlayData.Data.Hazard[player] > t ? t : PlayData.Data.Hazard[player];
            switch (judge)
            {
                case EJudge.Perfect:
                case EJudge.Great:
                case EJudge.Auto:
                    for (int i = 0; i < 3; i++)
                    {
                        gauge[i] = gaugepernote;
                    }
                    gauge[3] = GaugeList[player][3] > 0.0 ? 0.16 : 0; gauge[4] = GaugeList[player][4] > 0.0 ? 0.16 : 0; gauge[5] = 0;
                    break;

                case EJudge.Good:
                    for (int i = 0; i < 3; i++)
                    {
                        gauge[i] = gaugepernote * 0.5;
                    }
                    gauge[3] = 0; gauge[4] = 0; gauge[5] = 0;
                    break;

                case EJudge.Bad:
                case EJudge.Poor:
                    gauge[0] = -1.6; gauge[1] = -0.8; gauge[2] = -1.6; gauge[3] = GaugeList[player][3] < 30.0 ? -2.5 : -5.0; gauge[4] = -10.0; gauge[5] = -100.0 / Notes;
                    break;

                case EJudge.Through:
                    gauge[0] = -6.0; gauge[1] = -3.0; gauge[2] = -4.8; gauge[3] = GaugeList[player][3] < 30.0 ? -4.8 : -9.6; gauge[4] = -20.0; gauge[5] = -100.0 / Notes;
                    break;
            }
            for (int i = 0; i < 6; i++)
            {
                if (i < (int)EGauge.Hard || GaugeList[player][i] > 0.0) GaugeList[player][i] += gauge[i];
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
                GaugeList[player][i] = Math.Round(GaugeList[player][i], 4, MidpointRounding.AwayFromZero);
            }
            ShiftGauge(player);
            ScoreBoard[player].Gauge = GaugeList[player][(int)GaugeType[player]];
        }
        public static void ShiftGauge(int player)
        {
            switch ((EGaugeAutoShift)PlayData.Data.GaugeAutoShift[player])
            {
                case EGaugeAutoShift.ToNormal:
                    if ((PlayData.Data.GaugeType[player] >= (int)EGauge.Hard && PlayData.Data.GaugeType[player] <= (int)EGauge.Hazard) && GaugeList[player][(int)GaugeType[player]] <= 0.0)
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
                        switch (PlayData.Data.GaugeAutoShiftMin[player])
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
                        GaugeType[player] = (EGauge)PlayData.Data.GaugeType[player];
                    }
                    if ((GaugeType[player] == EGauge.Hard || GaugeType[player] == EGauge.EXHard) && GaugeList[player][(int)GaugeType[player]] <= 0.0)
                    {
                        GaugeType[player] -= 1;
                    }
                    if (GaugeType[player] < EGauge.Hard)
                    {
                        switch (PlayData.Data.GaugeAutoShiftMin[player])
                        {
                            case 0:
                                GaugeType[player] = EGauge.Normal;
                                break;
                            case 1:
                                if (GaugeList[player][0] >= 80.0 && PlayData.Data.GaugeType[player] != (int)EGauge.Assist && PlayData.Data.GaugeType[player] != (int)EGauge.Easy)
                                {
                                    GaugeType[player] = EGauge.Normal;
                                }
                                else if (GaugeList[player][2] >= 80.0 && PlayData.Data.GaugeType[player] != (int)EGauge.Assist)
                                {
                                    GaugeType[player] = EGauge.Easy;
                                }
                                else
                                {
                                    GaugeType[player] = EGauge.Assist;
                                }
                                break;
                            case 2:
                                if (GaugeList[player][0] >= 80.0 && PlayData.Data.GaugeType[player] != (int)EGauge.Assist && PlayData.Data.GaugeType[player] != (int)EGauge.Easy)
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
    }

    public class BScoreBoard
    {
        public int Auto, Perfect, Great, Good, Bad, Poor, Through, Score, Combo;
        public double Gauge = 100;


        public int Hit()
        {
            return Auto + Perfect + Great + Good + Bad + Through;
        }
        public int BadPoor()
        {
            return Bad + Poor + Through;
        }
        public int ComboBreak()
        {
            return Bad + Through;
        }
    }
}
