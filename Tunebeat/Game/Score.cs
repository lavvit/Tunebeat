﻿using System;
using System.Drawing;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class NewScore
    {
        #region Function
        public static EGauge[] GaugeType = new EGauge[2];
        public static ScoreBoard[] Scores = new ScoreBoard[5];
        public static ScoreBoard[] DanScore;
        public static int[] Hit = new int[2], Remain = new int[5], Combo = new int[5], NowRoll = new int[5];
        public static double[] Gauge = new double[2], msJudge = new double[2], Total = new double[2], GoodRate = new double[2], BadRate = new double[2], PoorRate = new double[2], msSum = new double[2], msAverage = new double[2];
        public static double[][] GaugeList = new double[2][], ClearRate = new double[2][];
        public static bool[] Cleared = new bool[2];
        public static bool RollReset;
        private static EJudge[] DisplayJudge = new EJudge[2];
        public static ERank[] Rank = new ERank[2];
        public static Counter JudgeCounter, JudgeCounterBig, JudgeCounter2P, JudgeCounterBig2P, BombAnimation, Active;
        public static Counter[] RollCounter = new Counter[5];
        #endregion
        public static void Init()
        {
            Reset();

            JudgeCounter = new Counter(0, 200, 1000, false);
            JudgeCounterBig = new Counter(0, 500, 1000, false);
            JudgeCounter2P = new Counter(0, 200, 1000, false);
            JudgeCounterBig2P = new Counter(0, 500, 1000, false);
            BombAnimation = new Counter(0, 1023, 1000, false);
            for (int i = 0; i < 5; i++)
            {
                RollCounter[i] = new Counter(0, 500, 1000, false);
            }
            Active = new Counter(0, 1000, 1000, false);
        }
        public static void Reset()
        {
            for (int i = 0; i < 5; i++)
            {
                Scores[i] = new ScoreBoard();
                Combo[i] = 0;
            }
            for (int i = 0; i < 2; i++)
            {
                Gauge[i] = 0;
                if (NewGame.Dan != null) SetDanGauge(i);
                else SetGauge(i);
                msSum[i] = 0;
                Hit[i] = 0;
            }
            if (NewGame.Dan != null)
            {
                DanScore = new ScoreBoard[NewGame.Dan.Courses.Count];
                for (int i = 0; i < NewGame.Dan.Courses.Count; i++)
                {
                    DanScore[i] = new ScoreBoard();
                }
            }

            RollReset = false;
        }

        public static void Draw()
        {
            Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
            Point Point = NewCreate.Enable ? createpoint[0] : NewNotes.NotesP[0];
            int y = NewCreate.Enable ? Point.Y + 195 + 4 : Point.Y - 4 - 48;
            Tx.Game_Gauge_Base.Draw(495, y, new Rectangle(0, NewCreate.Enable ? 48 : 0, 1425, 48));

            int[] Color = new int[100];
            int[] Color2P = new int[100];
            for (int i = 0; i < 100; i++)
            {
                Color[i] = i >= ClearRate[0][(int)GaugeType[0]] - 1 ? (GaugeType[0] >= EGauge.Hard ? 10 * (int)GaugeType[0] : 30) : 10 * (int)GaugeType[0];
                Color2P[i] = i >= ClearRate[1][(int)GaugeType[1]] - 1 ? (GaugeType[1] >= EGauge.Hard ? 10 * (int)GaugeType[1] : 30) : 10 * (int)GaugeType[1];
            }

            for (int i = 0; i < 100; i++)
            {
                Tx.Game_Gauge.Draw(495 + 179 + 12 * i, y + (NewCreate.Enable ? 2 : 6), new Rectangle(Color[i], (int)Gauge[0] > i ? 40 : 0, 10, 40));
            }
            NewGame.GameNumber.Draw(495 + 15, y + (NewCreate.Enable ? 8 : 12), $"{Gauge[0],5:F1}%", 0);
            if (NewGame.Play2P && PlayData.Data.PreviewType < 3)
            {
                Tx.Game_Gauge_Base.Draw(495, Point.Y - 4 + 465, new Rectangle(0, 48, 1425, 48));
                for (int i = 0; i < 100; i++)
                {
                    Tx.Game_Gauge.Draw(495 + 179 + 12 * i, Point.Y - 4 + 465 + 2, new Rectangle(Color2P[i], (int)Gauge[1] > i ? 40 : 0, 10, 40));
                }
                NewGame.GameNumber.Draw(495 + 15, Point.Y - 4 + 465 + 8, $"{Gauge[1],5:F1}%", 0);
            }

            if (!NewGame.Play2P && PlayData.Data.ShowGraph && !NewCreate.Enable)
            {
                DrawGraph();
            }

            if (!NewCreate.Enable) DrawJudge();

            DrawCombo(0);
            if (NewGame.Play2P) DrawCombo(1);

            if (PlayData.Data.PreviewType == 3)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (SongData.NowTJA[i].Courses[Game.Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Game.Course[i] == Game.Course[i - 1]) break;
                        count++;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    NewGame.GameNumber.Draw(0, NewNotes.NotesP[i].Y + 2, $"{(Scores[i].EXScore > 0 ? Scores[i].EXScore : Scores[i].Auto * 2),9}", Scores[i].EXScore > 0 || Scores[i].Auto == 0 ? 0 : 5);
                    NewGame.SmallNumber.Draw(136, NewNotes.NotesP[i].Y + 29, $"+{(Scores[i].EXScore > 0 ? Scores[i].Roll : Scores[i].AutoRoll),4}", Scores[i].EXScore > 0 || Scores[i].Auto == 0 ? 0 : 1);
                    Chip nowchip = Process.NowChip(NewGame.Chips[i]);
                    NewGame.GameNumber.Draw(0, NewNotes.NotesP[i].Y + 2 + 52, $"{(nowchip != null ? nowchip.Scroll * NewNotes.Scroll[i] : NewNotes.Scroll[i]),9:F2}", 0);//HS
                    NewGame.GameNumber.Draw(0, NewNotes.NotesP[i].Y + 2 + 92, $"{(nowchip != null ? nowchip.Bpm : NewGame.TJA[i].Header.BPM),9:F1}", 0);
                }
            }
            else
            {
                Chip nowchip = Process.NowChip(NewGame.Chips[0]);
                NewGame.GameNumber.Draw(0, Point.Y + 2, $"{(Scores[0].EXScore > 0 ? Scores[0].EXScore : Scores[0].Auto * 2),9}", Scores[0].EXScore > 0 || Scores[0].Auto == 0 ? 0 : 5);
                NewGame.SmallNumber.Draw(136, Point.Y + 29, $"+{(Scores[0].EXScore > 0 ? Scores[0].Roll : Scores[0].AutoRoll),4}", Scores[0].EXScore > 0 || Scores[0].Auto == 0 ? 0 : 1);
                NewGame.GameNumber.Draw(0, Point.Y + 2 + 52, $"{(nowchip != null ? nowchip.Scroll * NewNotes.Scroll[0] : NewNotes.Scroll[0]),9:F2}", 0);//HS
                NewGame.GameNumber.Draw(0, Point.Y + 2 + 92, $"{(nowchip != null ? nowchip.Bpm : NewGame.TJA[0].Header.BPM),9:F1}", 0);
                if (NewGame.Play2P)
                {
                    Chip nowchip2p = Process.NowChip(NewGame.Chips[1]);
                    NewGame.GameNumber.Draw(0, Point.Y + 2 + 331, $"{(Scores[1].EXScore > 0 ? Scores[1].EXScore : Scores[1].Auto * 2),9}", Scores[1].EXScore > 0 || Scores[1].Auto == 0 ? 0 : 5);
                    NewGame.SmallNumber.Draw(136, Point.Y + 29 + 331, $"+{(Scores[1].EXScore > 0 ? Scores[1].Roll : Scores[1].AutoRoll),4}", Scores[1].EXScore > 0 || Scores[1].Auto == 0 ? 0 : 1);
                    NewGame.GameNumber.Draw(72, Point.Y + 2 + 331 + 52, $"{(nowchip2p != null ? nowchip2p.Scroll * NewNotes.Scroll[1] : NewNotes.Scroll[1]),6:F2}", 0);//HS
                    NewGame.GameNumber.Draw(0, Point.Y + 2 + 331 + 92, $"{(nowchip2p != null ? nowchip2p.Bpm : NewGame.TJA[1].Header.BPM),9:F1}", 0);
                }
            }

            DrawScore();

            int[] rankvalue = new int[] { 0, NewGame.NowCourse[0].TotalNotes * 4 / 9,
                NewGame.NowCourse[0].TotalNotes * 6 / 9, NewGame.NowCourse[0].TotalNotes * 8 / 9,
                NewGame.NowCourse[0].TotalNotes * 10 / 9, NewGame.NowCourse[0].TotalNotes * 12 / 9,
                NewGame.NowCourse[0].TotalNotes * 14 / 9, NewGame.NowCourse[0].TotalNotes * 16 / 9,
                NewGame.NowCourse[0].TotalNotes * 2};
            int[] rankvalue2p = new int[] { 0, NewGame.NowCourse[1].TotalNotes * 4 / 9,
                NewGame.NowCourse[1].TotalNotes * 6 / 9, NewGame.NowCourse[1].TotalNotes * 8 / 9,
                NewGame.NowCourse[1].TotalNotes * 10 / 9, NewGame.NowCourse[1].TotalNotes * 12 / 9,
                NewGame.NowCourse[1].TotalNotes * 14 / 9, NewGame.NowCourse[1].TotalNotes * 16 / 9,
                NewGame.NowCourse[1].TotalNotes * 2};
            if (NewGame.NowState == EState.End && !PlayData.Data.ShowResultScreen && !NewCreate.Enable)
            {
                ERank rank = Rank[0];
                if (Math.Abs(Scores[0].EXScore + Scores[0].Auto * 2 - rankvalue[(int)Rank[0]]) > Math.Abs(Scores[0].EXScore + Scores[0].Auto * 2 - rankvalue[(int)Rank[0] + 1]))
                {
                    rank = Rank[0] + 1;
                }
                Drawing.Text(NewNotes.NotesP[0].X + 200, NewNotes.NotesP[0].Y + 86, $"SC:{Scores[0].EXScore}", 0xffffff);
                Drawing.Text(NewNotes.NotesP[0].X + 300, NewNotes.NotesP[0].Y + 86, $"PG:{Scores[0].Perfect + Scores[0].Auto}", Scores[0].Auto > 0 ? 0x00ff00 : 0xffffff);
                Drawing.Text(NewNotes.NotesP[0].X + 400, NewNotes.NotesP[0].Y + 86, $"GR:{Scores[0].Great}", 0xffffff);
                Drawing.Text(NewNotes.NotesP[0].X + 500, NewNotes.NotesP[0].Y + 86, $"GD:{Scores[0].Good}", 0xffffff);
                Drawing.Text(NewNotes.NotesP[0].X + 600, NewNotes.NotesP[0].Y + 86, $"BD:{Scores[0].Bad}", 0xffffff);
                Drawing.Text(NewNotes.NotesP[0].X + 700, NewNotes.NotesP[0].Y + 86, $"PR:{Scores[0].Poor}", 0xffffff);
                Drawing.Text(NewNotes.NotesP[0].X + 800, NewNotes.NotesP[0].Y + 86, $"RL:{Scores[0].Roll}", 0xffffff);
                Drawing.Text(NewNotes.NotesP[0].X + 200, NewNotes.NotesP[0].Y + 106, $"Rank:{Rank[0]}", 0xffffff);
                int rankremain = Scores[0].EXScore + Scores[0].Auto * 2 - rankvalue[(int)rank];
                Drawing.Text(NewNotes.NotesP[0].X + 300, NewNotes.NotesP[0].Y + 106, $"{rank}{((Scores[0].EXScore + Scores[0].Auto == NewGame.NowCourse[0].TotalNotes) || (rankremain >= 0) ? "+" : "")}{rankremain}", 0xffffff);
                if (Memory.SaveBest) Drawing.Text(NewNotes.NotesP[0].X + 400, NewNotes.NotesP[0].Y + 106, "New Record", 0xffffff);
                if (NewGame.Dan != null) Drawing.Text(NewNotes.NotesP[0].X + 500, NewNotes.NotesP[0].Y + 106, $"{DanC.ExamSuccess(NewGame.Dan.Exams)}", DanC.CrearColor(DanC.ExamSuccess(NewGame.Dan.Exams)));
                if (NewGame.Play2P)
                {
                    ERank rank2p = Rank[1];
                    if (Math.Abs(Scores[0].EXScore + Scores[0].Auto * 2 - rankvalue2p[(int)Rank[1]]) > Math.Abs(Scores[0].EXScore + Scores[0].Auto * 2 - rankvalue2p[(int)Rank[1] + 1]))
                    {
                        rank2p = Rank[1] + 1;
                    }
                    Drawing.Text(NewNotes.NotesP[1].X + 200, NewNotes.NotesP[1].Y + 86, $"SC:{Scores[1].EXScore}", 0xffffff);
                    Drawing.Text(NewNotes.NotesP[1].X + 300, NewNotes.NotesP[1].Y + 86, $"PG:{Scores[1].Perfect + Scores[1].Auto}", Scores[1].Auto > 0 ? 0x00ff00 : 0xffffff);
                    Drawing.Text(NewNotes.NotesP[1].X + 400, NewNotes.NotesP[1].Y + 86, $"GR:{Scores[1].Great}", 0xffffff);
                    Drawing.Text(NewNotes.NotesP[1].X + 500, NewNotes.NotesP[1].Y + 86, $"GD:{Scores[1].Good}", 0xffffff);
                    Drawing.Text(NewNotes.NotesP[1].X + 600, NewNotes.NotesP[1].Y + 86, $"BD:{Scores[1].Bad}", 0xffffff);
                    Drawing.Text(NewNotes.NotesP[1].X + 700, NewNotes.NotesP[1].Y + 86, $"PR:{Scores[1].Poor}", 0xffffff);
                    Drawing.Text(NewNotes.NotesP[1].X + 800, NewNotes.NotesP[1].Y + 86, $"RL:{Scores[1].Roll}", 0xffffff);
                    Drawing.Text(NewNotes.NotesP[1].X + 200, NewNotes.NotesP[1].Y + 106, $"Rank:{Rank[1]}", 0xffffff);
                    int rankremain2p = Scores[1].EXScore + Scores[1].Auto * 2 - rankvalue2p[(int)rank2p];
                    Drawing.Text(NewNotes.NotesP[1].X + 300, NewNotes.NotesP[1].Y + 106, $"{rank2p}{((Scores[1].EXScore + Scores[1].Auto == NewGame.NowCourse[1].TotalNotes) || (rankremain2p >= 0) ? "+" : "")}{rankremain2p}", 0xffffff);
                }
            }

            if (PlayData.Data.PreviewType == 3)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (NewGame.Chips[i].Count > 0)
                    {
                        if (i > 0 && NewGame.Course[i] == NewGame.Course[i - 1]) break;
                        count++;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    DrawRoll(i);
                }
            }
            else
            {
                DrawRoll(0);
                if (NewGame.Play2P)
                {
                    DrawRoll(1);
                }
            }


#if DEBUG
            if (!NewCreate.Enable)
            {
                Drawing.Text(0, 300, $"SC:{Scores[0].EXScore}", 0xffffff);
                if (NewGame.NowState == EState.End) Drawing.Text(80, 300, Remain[0] > 0 ? $"MAX-{Remain[0]}" : "MAX+0", 0xffffff);
                Drawing.Text(0, 320, $"PG:{Scores[0].Perfect}", 0xffffff);
                Drawing.Text(0, 340, $"GR:{Scores[0].Great}", 0xffffff);
                Drawing.Text(0, 360, $"GD:{Scores[0].Good}", 0xffffff);
                Drawing.Text(0, 380, $"BD:{Scores[0].Bad}", 0xffffff);
                Drawing.Text(0, 400, $"PR:{Scores[0].Poor}", 0xffffff);
                Drawing.Text(0, 420, $"AT:{Scores[0].Auto}", 0xffffff);
                Drawing.Text(0, 440, $"RL:{Scores[0].Roll + Scores[0].AutoRoll}{Scores[0].RollDetail}", 0xffffff);

                Drawing.Text(200, 300, $"{Gauge[0]}", 0xffffff);
                Drawing.Text(200, 320, $"Total:{Total[0]}", 0xffffff);
                if (NewGame.NowState == EState.End) Drawing.Text(200, 340, Cleared[0] ? "Cleared" : "Failed", 0xffffff);
                Drawing.Text(200, 360, $"Combo:{Scores[0].MaxCombo}", 0xffffff);
                Drawing.Text(200, 380, $"Rank:{Rank[0]}", 0xffffff);
                Drawing.Text(200, 400, $"Now:{GetClear(0)}", 0xffffff);

                if (JudgeCounter.State == TimerState.Started || JudgeCounterBig.State == TimerState.Started)
                {
                    Drawing.Text(600, 220, $"{DisplayJudge[0]}", 0xffffff);
                    Drawing.Text(600, 240, $"{Math.Round(msJudge[0], 2, MidpointRounding.AwayFromZero)}", 0xffffff);
                }

                if (NewGame.Play2P)
                {
                    Drawing.Text(0, 560, $"SC:{Scores[1].EXScore}", 0xffffff);
                    if (NewGame.NowState == EState.End) Drawing.Text(80, 560, Remain[1] > 0 ? $"MAX-{Remain[1]}" : "MAX+0", 0xffffff);
                    Drawing.Text(0, 580, $"PG:{Scores[1].Perfect}", 0xffffff);
                    Drawing.Text(0, 600, $"GR:{Scores[1].Great}", 0xffffff);
                    Drawing.Text(0, 620, $"GD:{Scores[1].Good}", 0xffffff);
                    Drawing.Text(0, 640, $"BD:{Scores[1].Bad}", 0xffffff);
                    Drawing.Text(0, 660, $"PR:{Scores[1].Poor}", 0xffffff);
                    Drawing.Text(0, 680, $"AT:{Scores[1].Auto}", 0xffffff);
                    Drawing.Text(0, 700, $"RL:{Scores[1].Roll + Scores[1].AutoRoll}{Scores[1].RollDetail}", 0xffffff);

                    Drawing.Text(200, 560, $"{Gauge[1]}", 0xffffff);
                    Drawing.Text(200, 580, $"Total:{Total[1]}", 0xffffff);
                    if (NewGame.NowState == EState.End) Drawing.Text(200, 600, Cleared[1] ? "Cleared" : "Failed", 0xffffff);
                    Drawing.Text(200, 620, $"Combo:{Scores[1].MaxCombo}", 0xffffff);
                    Drawing.Text(200, 640, $"Rank:{Rank[1]}", 0xffffff);
                    Drawing.Text(200, 680, $"Now:{GetClear(1)}", 0xffffff);

                    if (JudgeCounter2P.State == TimerState.Started || JudgeCounterBig2P.State == TimerState.Started)
                    {
                        Drawing.Text(600, 480, $"{DisplayJudge[1]}", 0xffffff);
                        Drawing.Text(600, 500, $"{Math.Round(msJudge[1], 2, MidpointRounding.AwayFromZero)}", 0xffffff);
                    }
                }
            }
#endif
        }

        public static void DrawJudge(ENote note, int player)
        {
            if (player == 0)
            {
                if (note == ENote.DON || note == ENote.KA)
                {
                    JudgeCounterBig.Reset();
                    JudgeCounterBig.Start();
                }
                else
                {
                    JudgeCounter.Reset();
                    JudgeCounter.Start();
                }

            }
            else
            {
                if (note == ENote.DON || note == ENote.KA)
                {
                    JudgeCounterBig2P.Reset();
                    JudgeCounterBig2P.Start();
                }
                else
                {
                    JudgeCounter2P.Reset();
                    JudgeCounter2P.Start();
                }
            }
        }
        public static void DrawJudge()
        {
            if (JudgeCounter.State == TimerState.Started)
            {
                if (DisplayJudge[0] != EJudge.Auto)
                    NewGame.SmallNumber.Draw(NewNotes.NotesP[0].X + 178, NewNotes.NotesP[0].Y - 6, msJudge[0] >= 1.0 ? $"+{(int)msJudge[0]}" : $"{(int)msJudge[0]}", msJudge[0] > GetNotes.range[0] ? 1 : (msJudge[0] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[0])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[0].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        Tx.Game_Bomb[0].Draw(NewNotes.NotesP[0].X - 97.5, NewNotes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[4].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        Tx.Game_Bomb[4].Draw(NewNotes.NotesP[0].X - 97.5, NewNotes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[5].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        Tx.Game_Bomb[5].Draw(NewNotes.NotesP[0].X - 97.5, NewNotes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
            if (JudgeCounterBig.State == TimerState.Started)
            {
                if (DisplayJudge[0] != EJudge.Auto)
                    NewGame.SmallNumber.Draw(NewNotes.NotesP[0].X + 178, NewNotes.NotesP[0].Y - 6, msJudge[0] >= 1.0 ? $"+{(int)msJudge[0]}" : $"{(int)msJudge[0]}", msJudge[0] > GetNotes.range[0] ? 1 : (msJudge[0] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[0])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[6].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        Tx.Game_Bomb[6].Draw(NewNotes.NotesP[0].X - 97.5, NewNotes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[10].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        Tx.Game_Bomb[10].Draw(NewNotes.NotesP[0].X - 97.5, NewNotes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[11].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        Tx.Game_Bomb[11].Draw(NewNotes.NotesP[0].X - 97.5, NewNotes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(NewNotes.NotesP[0].X + 30.5, NewNotes.NotesP[0].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
            if (JudgeCounter2P.State == TimerState.Started)
            {
                if (DisplayJudge[1] != EJudge.Auto)
                    NewGame.SmallNumber.Draw(NewNotes.NotesP[1].X + 178, NewNotes.NotesP[1].Y - 6, msJudge[1] >= 1.0 ? $"+{(int)msJudge[1]}" : $"{(int)msJudge[1]}", msJudge[1] > GetNotes.range[0] ? 1 : (msJudge[1] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[1])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[0].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        Tx.Game_Bomb[0].Draw(NewNotes.NotesP[1].X - 97.5, NewNotes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[4].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        Tx.Game_Bomb[4].Draw(NewNotes.NotesP[1].X - 97.5, NewNotes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[5].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        Tx.Game_Bomb[5].Draw(NewNotes.NotesP[1].X - 97.5, NewNotes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
            if (JudgeCounterBig2P.State == TimerState.Started)
            {
                if (DisplayJudge[1] != EJudge.Auto)
                    NewGame.SmallNumber.Draw(NewNotes.NotesP[1].X + 178, NewNotes.NotesP[1].Y - 6, msJudge[1] >= 1.0 ? $"+{(int)msJudge[1]}" : $"{(int)msJudge[1]}", msJudge[1] > GetNotes.range[0] ? 1 : (msJudge[1] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[1])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[6].Opacity = 1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        Tx.Game_Bomb[6].Draw(NewNotes.NotesP[1].X - 97.5, NewNotes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[10].Opacity = 1.0 - (JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        Tx.Game_Bomb[10].Draw(NewNotes.NotesP[1].X - 97.5, NewNotes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[11].Opacity = 1.0 - (JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        Tx.Game_Bomb[11].Draw(NewNotes.NotesP[1].X - 97.5, NewNotes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(NewNotes.NotesP[1].X + 30.5, NewNotes.NotesP[1].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
        }

        public static void DrawRoll(int player)
        {
            Chip nowchip = Process.NowChip(NewGame.Chips[player], false);
            ERoll roll = nowchip != null ? nowchip.ERoll : ERoll.None;
            if (roll >= ERoll.Roll && roll < ERoll.Balloon) { RollCounter[player].Reset(); RollCounter[player].Start(); }
            if ((RollCounter[player].State != 0 || roll > ERoll.None) && (nowchip != null && !nowchip.IsHit))
            {
                Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
                Point Point = NewCreate.Enable ? createpoint[player] : NewNotes.NotesP[player];
                NewGame.GameNumber.Draw(Point.X + 160, Point.Y + 82, NowRoll[player], 0);
            }
        }

        public static int Digit(int num)
        {
            int digit = 1;
            for (int i = num; i >= 10; i /= 10)
            {
                digit++;
            }
            return digit;
        }
        public static void DrawCombo(int player)
        {
            int color = 0;
            switch (NewGame.Course[player])
            {
                case (int)ECourse.Easy:
                    if (Combo[player] >= 200)
                        color = 4;
                    else if (Combo[player] >= 30)
                        color = 3;
                    else if (Combo[player] >= 10)
                        color = 2;
                    break;
                case (int)ECourse.Normal:
                    if (Combo[player] >= 300)
                        color = 4;
                    else if (Combo[player] >= 30)
                        color = 3;
                    else if (Combo[player] >= 10)
                        color = 2;
                    break;
                case (int)ECourse.Hard:
                    if (Combo[player] >= 500)
                        color = 4;
                    else if (Combo[player] >= 50)
                        color = 3;
                    else if (Combo[player] >= 30)
                        color = 2;
                    break;
                case (int)ECourse.Oni:
                case (int)ECourse.Edit:
                    if (Combo[player] >= 1000)
                        color = 4;
                    else if (Combo[player] >= 100)
                        color = 3;
                    else if (Combo[player] >= 50)
                        color = 2;
                    break;
            }
            if (Combo[player] >= PlayData.Data.ShowComboNotes)
            {
                if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
                {
                    NewGame.GameNumber.Draw(411 - 12 * Digit(Combo[player]), NewNotes.NotesP[player].Y + 82, $"{Combo[player]}", color);
                }
                else
                {
                    Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
                    Point Point = NewCreate.Enable ? createpoint[0] : NewNotes.NotesP[0];
                    NewGame.GameNumber.Draw(411 - 12 * Digit(Combo[player]), Point.Y + 82 + 262 * player, $"{Combo[player]}", color);
                }
            }
        }

        public static void DrawScore()
        {
            if (!NewCreate.Enable)
            {
                switch ((EPreviewType)PlayData.Data.PreviewType)
                {
                    case EPreviewType.Up:
                        for (int i = 0; i < 6; i++)
                        {
                            Tx.Game_Judge_Data.Draw(20, 24 + 32 * i + 512, new Rectangle(0, 42 * i, 134, 42));
                        }
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 0 + 512, $"{Scores[0].Perfect + Scores[0].Auto}", Scores[0].Auto > 0 ? 1 : 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 1 + 512, $"{Scores[0].Great}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 2 + 512, $"{Scores[0].Good}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 3 + 512, $"{Scores[0].Bad}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 4 + 512, $"{Scores[0].Poor}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 5 + 512, $"{Scores[0].Roll + Scores[0].AutoRoll}", Scores[0].AutoRoll > 0 ? 1 : 0);
                        if (NewGame.Play2P)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Tx.Game_Judge_Data.Draw(20, 824 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                            }
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 0, $"{Scores[1].Perfect + Scores[1].Auto}", Scores[1].Auto > 0 ? 1 : 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 1, $"{Scores[1].Great}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 2, $"{Scores[1].Good}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 3, $"{Scores[1].Bad}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 4, $"{Scores[1].Poor}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 5, $"{Scores[1].Roll + Scores[1].AutoRoll}", Scores[1].AutoRoll > 0 ? 1 : 0);
                        }
                        break;
                    case EPreviewType.Down:
                        for (int i = 0; i < 6; i++)
                        {
                            Tx.Game_Judge_Data.Draw(20, 24 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                        }
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 0, $"{Scores[0].Perfect + Scores[0].Auto}", Scores[0].Auto > 0 ? 1 : 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 1, $"{Scores[0].Great}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 2, $"{Scores[0].Good}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 3, $"{Scores[0].Bad}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 4, $"{Scores[0].Poor}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 5, $"{Scores[0].Roll + Scores[0].AutoRoll}", Scores[0].AutoRoll > 0 ? 1 : 0);
                        if (NewGame.Play2P)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Tx.Game_Judge_Data.Draw(20, 824 + 32 * i - 512, new Rectangle(0, 42 * i, 134, 42));
                            }
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 0 - 512, $"{Scores[1].Perfect + Scores[1].Auto}", Scores[1].Auto > 0 ? 1 : 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 1 - 512, $"{Scores[1].Great}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 2 - 512, $"{Scores[1].Good}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 3 - 512, $"{Scores[1].Bad}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 4 - 512, $"{Scores[1].Poor}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 5 - 512, $"{Scores[1].Roll + Scores[1].AutoRoll}", Scores[1].AutoRoll > 0 ? 1 : 0);
                        }
                        break;
                    case EPreviewType.Normal:
                        for (int i = 0; i < 6; i++)
                        {
                            Tx.Game_Judge_Data.Draw(20, 24 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                        }
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 0, $"{Scores[0].Perfect + Scores[0].Auto}", Scores[0].Auto > 0 ? 1 : 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 1, $"{Scores[0].Great}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 2, $"{Scores[0].Good}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 3, $"{Scores[0].Bad}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 4, $"{Scores[0].Poor}", 0);
                        NewGame.SmallNumber.Draw(160, 36 + 32 * 5, $"{Scores[0].Roll + Scores[0].AutoRoll}", Scores[0].AutoRoll > 0 ? 1 : 0);
                        if (NewGame.Play2P)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Tx.Game_Judge_Data.Draw(20, 824 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                            }
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 0, $"{Scores[1].Perfect + Scores[1].Auto}", Scores[1].Auto > 0 ? 1 : 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 1, $"{Scores[1].Great}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 2, $"{Scores[1].Good}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 3, $"{Scores[1].Bad}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 4, $"{Scores[1].Poor}", 0);
                            NewGame.SmallNumber.Draw(160, 836 + 32 * 5, $"{Scores[1].Roll + Scores[1].AutoRoll}", Scores[1].AutoRoll > 0 ? 1 : 0);
                        }
                        break;
                }
            }
        }

        public static void DrawGraph()
        {
            if (PlayData.Data.PreviewType > 0) return;

            Tx.Game_Graph_Base.Draw(495, 0);

            ScoreBoard score1 = NewGame.Dan != null ? DanScore[DanCourse.SongNumber] : Scores[0];
            double allnotes = score1.Perfect + score1.Great + score1.Good + score1.Bad + score1.Poor + score1.Auto;
            double hitnotes = score1.EXScore > 0 ? score1.EXScore / 2.0 : score1.Auto;
            double percent = allnotes > 0 ? hitnotes / allnotes : 0;
            double totalnote = NewGame.NowCourse[0].TotalNotes;
            double hitpercent = hitnotes / totalnote;
            double allpercent = allnotes / totalnote;
            int score = (int)(totalnote * percent * 2);
            ERank rank = GetRank(score, 0);

            Tx.Game_Graph.Draw(499, 163, new Rectangle((int)(1000 - 1000 * hitpercent), 64 * 2, (int)(1000 * hitpercent), 64));

            if (Memory.BestData != null && Memory.BestData.Score > 0)
            {
                double bestallnotes = Memory.BestData.Score;
                double bestnotes = Scores[2].EXScore;
                double bestpercent = bestnotes / (totalnote * 2);
                double bestallpercent = bestallnotes / (totalnote * 2);
                Tx.Game_Graph.Draw(499, 163 - 76, new Rectangle((int)(1000 - 1000 * bestallpercent), 64 * 3, (int)(1000 * bestallpercent), 64));
                if (Memory.BestData.Chip != null)
                {
                    Tx.Game_Graph.Draw(499, 163 - 76, new Rectangle((int)(1000 - 1000 * bestpercent), 64, (int)(1000 * bestpercent), 64));
                    int num = (int)hitnotes * 2 - (int)bestnotes;
                    NewGame.SmallNumber.Draw(499 + 8, 183 - 76, $"{(num >= 0 ? "+" : "")}{num}", num < 0 ? 1 : 0);
                }
            }

            switch ((ERival)PlayData.Data.RivalType)
            {
                case ERival.Percent:
                    double allvalue = PlayData.Data.RivalPercent / 100.0;
                    double value = allvalue * allpercent;
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * allvalue), 64 * 3, (int)(1000 * allvalue), 64));
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * value), 0, (int)(1000 * value), 64));
                    int num = (int)hitnotes * 2 - (int)(totalnote * 2 * value);
                    NewGame.SmallNumber.Draw(499 + 8, 183 - 76 * 2, $"{(num >= 0 ? "+" : "")}{num}", num < 0 ? 1 : 0);
                    NewGame.GameNumber.Draw(499 - 178, 181 - 76 * 2, $"{PlayData.Data.RivalPercent,6:F2}%", 0);
                    break;
                case ERival.Rank:
                    double rpercent = GetRankToPercent((ERank)PlayData.Data.RivalRank, 0);
                    double rvalue = rpercent * allpercent;
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rpercent), 64 * 3, (int)(1000 * rpercent), 64));
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rvalue), 0, (int)(1000 * rvalue), 64));
                    int rnum = (int)hitnotes * 2 - (int)(totalnote * 2 * rvalue);
                    NewGame.SmallNumber.Draw(499 + 8, 183 - 76 * 2, $"{(rnum >= 0 ? "+" : "")}{rnum}", rnum < 0 ? 1 : 0);
                    Tx.Result_Rank.Draw(499 - 152, 175 - 76 * 2, new Rectangle(0, PlayData.Data.RivalRank > 7 ? 45 * 7 : 45 * PlayData.Data.RivalRank, 161, 45));
                    break;
                case ERival.PlayScore:
                    if (Memory.RivalData.Score > 0)
                    {
                        double rivalallnotes = Memory.RivalData.Score;
                        double rivalnotes = Scores[3].EXScore;
                        double rivalpercent = rivalnotes / (totalnote * 2);
                        double rivalallpercent = rivalallnotes / (totalnote * 2);
                        Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rivalallpercent), 64 * 3, (int)(1000 * rivalallpercent), 64));
                        if (Memory.RivalData.Chip != null)
                        {
                            Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rivalpercent), 0, (int)(1000 * rivalpercent), 64));
                            int rivalnum = (int)hitnotes * 2 - (int)rivalnotes;
                            NewGame.SmallNumber.Draw(499 + 8, 183 - 76 * 2, $"{(rivalnum >= 0 ? "+" : "")}{rivalnum}", rivalnum < 0 ? 1 : 0);
                        }
                    }
                    break;
            }

            NewGame.GameNumber.Draw(1536, 196, $"{score,4}", 0);
            Tx.Game_Rank.Draw(1622, 152, new Rectangle(0, 90 * (int)rank, 161 * 2, 45 * 2));
        }

        public static void Update()
        {
            Remain[0] = Hit[0] * 2 - Scores[0].EXScore;
            Gauge[0] = GaugeList[0][(int)GaugeType[0]];
            Cleared[0] = Gauge[0] >= ClearRate[0][(int)GaugeType[0]] ? true : false;
            Rank[0] = GetRank(Scores[0].EXScore + Scores[0].Auto * 2, 0);
            msAverage[0] = Hit[0] > 0 ? msSum[0] / Hit[0] : 0;
            if (NewGame.Play2P)
            {
                Remain[1] = Hit[1] * 2 - Scores[1].EXScore;
                Gauge[1] = GaugeList[1][(int)GaugeType[1]];
                Cleared[1] = Gauge[1] >= ClearRate[1][(int)GaugeType[1]] ? true : false;
                Rank[1] = GetRank(Scores[1].EXScore + Scores[1].Auto * 2, 1);
                msAverage[1] = Hit[1] > 0 ? msSum[1] / Hit[1] : 0;
            }

            Chip nchip = Process.NowChip(NewGame.Chips[0], false);
            if (NewGame.NowState == EState.None)
            {
                NowRoll[0] = nchip != null ? nchip.Balloon : 0;
            }
            else
            {
                if (nchip != null)
                {
                    switch (nchip.ENote)
                    {
                        case ENote.RollStart:
                        case ENote.ROLLStart:
                            if (!RollReset)
                            {
                                NowRoll[0] = 0;
                                RollReset = true;
                            }
                            break;
                        case ENote.Balloon:
                        case ENote.Kusudama:
                            if (!RollReset)
                            {
                                NowRoll[0] = nchip.Balloon;
                                RollReset = true;
                            }
                            break;
                        case ENote.RollEnd:
                            if (RollReset)
                            {
                                RollReset = false;
                            }
                            break;
                    }
                }
            }

            if (GaugeType[0] >= EGauge.Hard && Gauge[0] == 0 && (PlayData.Data.GaugeAutoShift[0] == (int)EGaugeAutoShift.None || PlayData.Data.GaugeAutoShift[0] == (int)EGaugeAutoShift.Retry || NewGame.Dan != null))
            {
                NewGame.Failed[0] = true;
            }
            if (NewGame.Play2P && GaugeType[1] >= EGauge.Hard && Gauge[1] == 0 && (PlayData.Data.GaugeAutoShift[1] == (int)EGaugeAutoShift.None || PlayData.Data.GaugeAutoShift[1] == (int)EGaugeAutoShift.Retry || NewGame.Dan != null))
            {
                NewGame.Failed[1] = true;
            }

            JudgeCounter.Tick();
            JudgeCounter2P.Tick();
            JudgeCounterBig.Tick();
            JudgeCounterBig2P.Tick();
            BombAnimation.Tick();
            BombAnimation.Start();
            for (int i = 0; i < 5; i++)
            {
                RollCounter[i].Tick();
            }
            Active.Tick();
        }

        public static void AddScore(EJudge judge, int player)
        {
            if (player < 2)
            {
                if (NewGame.Dan != null) AddDanGauge(judge, player);
                else AddGauge(judge, player);
                DisplayJudge[player] = judge;
            }
            switch (judge)
            {
                case EJudge.Perfect:
                    Scores[player].EXScore += 2;
                    if (NewGame.Dan != null && player < 2) DanScore[DanCourse.SongNumber].EXScore += 2;
                    break;
                case EJudge.Great:
                    Scores[player].EXScore += 1;
                    if (NewGame.Dan != null && player < 2) DanScore[DanCourse.SongNumber].EXScore += 1;
                    break;
            }
            if (PlayData.Data.PreviewType == 3 || (PlayData.Data.PreviewType < 3 && player < 2))
            {
                switch (judge)
                {
                    case EJudge.Perfect:
                        Scores[player].Perfect++;
                        Combo[player]++;
                        if (Combo[player] > Scores[player].MaxCombo) Scores[player].MaxCombo++;
                        if (NewGame.Dan != null)
                        {
                            DanScore[DanCourse.SongNumber].Perfect++;
                            DanScore[DanCourse.SongNumber].MaxCombo++;
                        }
                        break;
                    case EJudge.Great:
                        Scores[player].Great++;
                        Combo[player]++;
                        if (Combo[player] > Scores[player].MaxCombo) Scores[player].MaxCombo++;
                        if (NewGame.Dan != null)
                        {
                            DanScore[DanCourse.SongNumber].Great++;
                            DanScore[DanCourse.SongNumber].MaxCombo++;
                        }
                        break;
                    case EJudge.Good:
                        Scores[player].Good++;
                        Combo[player]++;
                        if (Combo[player] > Scores[player].MaxCombo) Scores[player].MaxCombo++;
                        if (NewGame.Dan != null)
                        {
                            DanScore[DanCourse.SongNumber].Good++;
                            DanScore[DanCourse.SongNumber].MaxCombo++;
                        }
                        break;
                    case EJudge.Bad:
                        Scores[player].Bad++;
                        Combo[player] = 0;
                        if (NewGame.Dan != null)
                        {
                            DanScore[DanCourse.SongNumber].Bad++;
                        }
                        break;
                    case EJudge.Poor:
                    case EJudge.Through:
                        Scores[player].Poor++;
                        Combo[player] = 0;
                        if (NewGame.Dan != null)
                        {
                            DanScore[DanCourse.SongNumber].Poor++;
                        }
                        break;
                    case EJudge.Auto:
                        Scores[player].Auto++;
                        Combo[player]++;
                        if (Combo[player] > Scores[player].MaxCombo) Scores[player].MaxCombo++;
                        if (NewGame.Dan != null)
                        {
                            DanScore[DanCourse.SongNumber].Auto++;
                            DanScore[DanCourse.SongNumber].MaxCombo++;
                        }
                        break;
                }
            }
        }

        public static void AddRoll(int player)
        {
            if (PlayData.Data.PreviewType == 3 || Game.IsAuto[player])
            {
                Scores[player].AutoRoll++;
                if (NewGame.Dan != null)
                {
                    DanScore[DanCourse.SongNumber].AutoRoll++;
                }
            }
            else
            {
                Scores[player].Roll++;
                Scores[player].RollDetail.Item1++;
                if (NewGame.Dan != null)
                {
                    DanScore[DanCourse.SongNumber].Roll++;
                    DanScore[DanCourse.SongNumber].RollDetail.Item1++;
                }
            }
        }
        public static void AddBalloon(int player)
        {
            if (PlayData.Data.PreviewType == 3 || Game.IsAuto[player])
            {
                Scores[player].AutoRoll++;
                if (NewGame.Dan != null)
                {
                    DanScore[DanCourse.SongNumber].AutoRoll++;
                }
            }
            else
            {
                Scores[player].Roll++;
                Scores[player].RollDetail.Item2++;
                if (NewGame.Dan != null)
                {
                    DanScore[DanCourse.SongNumber].Roll++;
                    DanScore[DanCourse.SongNumber].RollDetail.Item1++;
                }
            }
        }

        public static void AddGauge(EJudge judge, int player)
        {
            if (NewGame.NowState != EState.Play && GaugeType[player] >= EGauge.Hard) return;

            double[] gaugepernote = new double[2];
            for (int i = 0; i < gaugepernote.Length; i++)
            {
                if (NewGame.NowCourse[i] != null) gaugepernote[i] = Total[i] / NewGame.NowCourse[i].TotalNotes;
            }
            double[] gauge = new double[6];
            int t = NewGame.NowCourse[player].TotalNotes;
            int Notes = PlayData.Data.Hazard[player] > t ? t : PlayData.Data.Hazard[player];
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
                case EJudge.Poor:
                    gaugepernote[player] *= -BadRate[player];
                    for (int i = 0; i < 6; i++)
                    {
                        gauge[i] += gaugepernote[player];
                    }
                    gauge[1] *= 0.5; gauge[2] *= 0.8; gauge[3] = GaugeList[player][3] < 30.0 ? -2.5 : -5.0; gauge[4] = -10.0; gauge[5] = -100.0 / Notes;
                    break;

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
        }
        public static void AddDanGauge(EJudge judge, int player)
        {
            if (NewGame.NowState != EState.Play && GaugeType[player] >= EGauge.Hard) return;

            double[] gaugepernote = new double[2];
            for (int i = 0; i < gaugepernote.Length; i++)
            {
                if (NewGame.NowCourse[i] != null) gaugepernote[i] = Total[i] / NewGame.Dan.TotalNotes;
            }
            double[] gauge = new double[6];
            int t = NewGame.Dan.TotalNotes;
            int Notes = PlayData.Data.Hazard[player] > t ? t : PlayData.Data.Hazard[player];
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
                    gauge[3] = 0.02; gauge[4] = 0.02; gauge[5] = 0;
                    break;

                case EJudge.Bad:
                    gaugepernote[player] *= -BadRate[player];
                    for (int i = 0; i < 6; i++)
                    {
                        gauge[i] += gaugepernote[player];
                    }
                    gauge[1] *= 0.5; gauge[2] *= 0.8; gauge[3] = GaugeList[player][3] < 30.0 ? -2.4 : -4.8; gauge[4] = -20.0; gauge[5] = -100.0 / Notes;
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

        public static void SetGauge(int player)
        {
            double[] total = new double[2] { 135, 135 };
            double[] goodrate = new double[2] { 0.5, 0.5 };
            double[] badrate = new double[2] { 2.0, 2.0 };
            double[] poorrate = new double[2];
            switch (NewGame.Course[player])
            {
                case (int)ECourse.Easy:
                    {
                        goodrate[player] = 0.75;
                        badrate[player] = 0.5;
                        switch (NewGame.NowCourse[player].LEVEL)
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
                        switch (NewGame.NowCourse[player].LEVEL)
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
                        switch (NewGame.NowCourse[player].LEVEL)
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
                        switch (NewGame.NowCourse[player].LEVEL)
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

            Total[player] = NewGame.NowCourse[player].TOTAL > 0.0 ? NewGame.NowCourse[player].TOTAL : total[player];
            GoodRate[player] = goodrate[player];
            BadRate[player] = badrate[player];
            PoorRate[player] = poorrate[player];

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
            GaugeList[player] = new double[6] { 0.0, 0.0, 0.0, 100.0, 100.0, 100.0 };
            ClearRate[player] = new double[6] { 80.0, 60.0, 80.0, 0.01, 0.01, 0.01 };

            if (PlayData.Data.GaugeType[player] >= (int)EGauge.Hard && PlayData.Data.GaugeType[player] <= (int)EGauge.Hazard)
            {
                Gauge[player] = 100.0;
            }
            else
            {
                Gauge[player] = 0;
            }
        }
        public static void SetDanGauge(int player)
        {
            Total[player] = NewGame.Dan.Gauge.Total > 0.0 ? NewGame.Dan.Gauge.Total : 133.333;
            GoodRate[player] = 0.5;
            BadRate[player] = 2;
            PoorRate[player] = 2;

            GaugeType[player] = NewGame.Dan.Gauge.GaugeType == 0 ? EGauge.Normal : (EGauge)NewGame.Dan.Gauge.GaugeType + 2;
            GaugeType[player] = PlayData.Data.Hazard[player] >= 1 ? EGauge.Hazard : GaugeType[player];
            GaugeList[player] = new double[6] { 0.0, 0.0, 0.0, 100.0, 100.0, 100.0 };
            ClearRate[player] = new double[6] { 80.0, 60.0, 80.0, 0.01, 0.01, 0.01 };

            if (PlayData.Data.GaugeType[player] >= (int)EGauge.Hard && PlayData.Data.GaugeType[player] <= (int)EGauge.Hazard)
            {
                Gauge[player] = 100.0;
            }
            else
            {
                Gauge[player] = 0;
            }
        }

        public static ERank GetRank(int value, int player)
        {
            if (value >= NewGame.NowCourse[player].TotalNotes * 16 / 9)
            {
                return ERank.AAA;
            }
            else if (value >= NewGame.NowCourse[player].TotalNotes * 14 / 9)
            {
                return ERank.AA;
            }
            else if (value >= NewGame.NowCourse[player].TotalNotes * 12 / 9)
            {
                return ERank.A;
            }
            else if (value >= NewGame.NowCourse[player].TotalNotes * 10 / 9)
            {
                return ERank.B;
            }
            else if (value >= NewGame.NowCourse[player].TotalNotes * 8 / 9)
            {
                return ERank.C;
            }
            else if (value >= NewGame.NowCourse[player].TotalNotes * 6 / 9)
            {
                return ERank.D;
            }
            else if (value >= NewGame.NowCourse[player].TotalNotes * 4 / 9)
            {
                return ERank.E;
            }
            else
            {
                return ERank.F;
            }
        }

        public static double GetRankToPercent(ERank rank, int player)
        {
            switch (rank)
            {
                case ERank.MAX:
                    return 1.0;
                case ERank.AAA:
                    return 8.0 / 9;
                case ERank.AA:
                    return 7.0 / 9;
                case ERank.A:
                    return 6.0 / 9;
                case ERank.B:
                    return 5.0 / 9;
                case ERank.C:
                    return 4.0 / 9;
                case ERank.D:
                    return 3.0 / 9;
                case ERank.E:
                    return 2.0 / 9;
                case ERank.F:
                default:
                    return 0;
            }
        }

        public static EClear GetClear(int player)
        {
            if (Scores[player].Bad + Scores[player].Poor == 0)
            {
                if (Scores[player].Good == 0)
                {
                    if (Scores[player].Great == 0) return EClear.AllPerfect;
                    else return EClear.AllGreat;
                }
                else return EClear.FullCombo;
            }
            else
            {
                switch (GaugeType[player])
                {
                    case EGauge.Hazard:
                    case EGauge.EXHard:
                        if (Gauge[player] > 0) return EClear.EXHardClear;
                        break;
                    case EGauge.Hard:
                        if (Gauge[player] > 0) return EClear.HardClear;
                        break;
                    case EGauge.Normal:
                        if (Gauge[player] >= 80) return EClear.Clear;
                        break;
                    case EGauge.Easy:
                        if (Gauge[player] >= 80) return EClear.EasyClear;
                        break;
                    case EGauge.Assist:
                        if (Gauge[player] >= 60) return EClear.AssistClear;
                        break;
                }
            }
            return EClear.Failed;
        }
    }

    public class Score : Scene
    {
        public override void Enable()
        {
            for (int i = 0; i < 5; i++)
            {
                EXScore[i] = 0;
                Poor[i] = 0;
                Auto[i] = 0;
                AutoRoll[i] = 0;
                Combo[i] = 0;
                MaxCombo[i] = 0;
            }
            for (int i = 0; i < 2; i++)
            {
                Perfect[i] = 0;
                Great[i] = 0;
                Good[i] = 0;
                Bad[i] = 0;
                Roll[i] = 0;
                RollYellow[i] = 0;
                RollBalloon[i] = 0;
                Gauge[i] = 0;
                SetGauge(i);
                msSum[i] = 0;
                Hit[i] = 0;
            }

            JudgeCounter = new Counter(0, 200, 1000, false);
            JudgeCounterBig = new Counter(0, 500, 1000, false);
            JudgeCounter2P = new Counter(0, 200, 1000, false);
            JudgeCounterBig2P = new Counter(0, 500, 1000, false);
            BombAnimation = new Counter(0, 1023, 1000, false);
            for (int i = 0; i < 5; i++)
            {
                RollCounter[i] = new Counter(0, 200, 1000, false);
            }
            Active = new Counter(0, 1000, 1000, false);
            base.Enable();
        }

        public override void Disable()
        {
            JudgeCounter.Reset();
            JudgeCounterBig.Reset();
            JudgeCounter2P.Reset();
            JudgeCounterBig2P.Reset();
            BombAnimation.Reset();
            for (int i = 0; i < 5; i++)
            {
                RollCounter[i].Reset();
            }
            Active.Reset();
            base.Disable();
        }
        public override void Draw()
        {
            Tx.Game_Gauge_Base.Draw(495, Notes.NotesP[0].Y - 4 - 48, new Rectangle(0, 0, 1425, 48));

            int[] Color = new int[100];
            int[] Color2P = new int[100];
            for (int i = 0; i < 100; i++)
            {
                Color[i] = i >= ClearRate[0][(int)GaugeType[0]] - 1 ? (GaugeType[0] >= EGauge.Hard ? 10 * (int)GaugeType[0] : 30) : 10 * (int)GaugeType[0];
                Color2P[i] = i >= ClearRate[1][(int)GaugeType[1]] - 1 ? (GaugeType[1] >= EGauge.Hard ? 10 * (int)GaugeType[1] : 30) : 10 * (int)GaugeType[1];
            }

            for (int i = 0; i < 100; i++)
            {
                Tx.Game_Gauge.Draw(495 + 179 + 12 * i, Notes.NotesP[0].Y - 4 - 48 + 6, new Rectangle(Color[i], (int)Gauge[0] > i ? 40 : 0, 10, 40));
            }
            DrawNumber(495 + 15, Notes.NotesP[0].Y - 4 - 48 + 12, $"{Gauge[0],5:F1}%", 0);
            if (Game.Play2P && PlayData.Data.PreviewType < 3)
            {
                Tx.Game_Gauge_Base.Draw(495, Notes.NotesP[0].Y - 4 + 465, new Rectangle(0, 48, 1425, 48));
                for (int i = 0; i < 100; i++)
                {
                    Tx.Game_Gauge.Draw(495 + 179 + 12 * i, Notes.NotesP[0].Y - 4 + 465 + 2, new Rectangle(Color2P[i], (int)Gauge[1] > i ? 40 : 0, 10, 40));
                }
                DrawNumber(495 + 15, Notes.NotesP[0].Y - 4 + 465 + 8, $"{Gauge[1],5:F1}%", 0);
            }

            if (!Game.Play2P && PlayData.Data.ShowGraph)
            {
                DrawGraph();
            }

            DrawJudge();

            if (PlayData.Data.PreviewType == 3)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (SongData.NowTJA[i].Courses[Game.Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Game.Course[i] == Game.Course[i - 1]) break;
                        count++;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    DrawNumber(0, Notes.NotesP[i].Y + 2, $"{(EXScore[i] > 0 ? EXScore[i] : Auto[i] * 2),9}", EXScore[i] > 0 || Auto[i] == 0 ? 0 : 5);
                    DrawMiniNumber(136, Notes.NotesP[i].Y + 29, $"+{(EXScore[i] > 0 ? Roll[i] : AutoRoll[i]),4}", EXScore[i] > 0 || Auto[i] == 0 ? 0 : 1);
                    Chip nowchip = GetNotes.GetNowNote(SongData.NowTJA[i].Courses[Game.Course[i]].ListChip, Game.MainTimer.Value, true);
                    DrawNumber(0, Notes.NotesP[i].Y + 2 + 52, $"{(nowchip != null ? nowchip.Scroll * Notes.Scroll[i] : Notes.Scroll[i]),9:F2}", 0);//HS
                    DrawNumber(0, Notes.NotesP[i].Y + 2 + 92, $"{(nowchip != null ? nowchip.Bpm : SongData.NowTJA[i].Header.BPM),9:F1}", 0);
                }
            }
            else
            {
                DrawNumber(0, Notes.NotesP[0].Y + 2, $"{(EXScore[0] > 0 ? EXScore[0] : Auto[0] * 2),9}", EXScore[0] > 0 || Auto[0] == 0 ? 0 : 5);
                DrawMiniNumber(136, Notes.NotesP[0].Y + 29, $"+{(EXScore[0] > 0 ? Roll[0] : AutoRoll[0]),4}", EXScore[0] > 0 || Auto[0] == 0 ? 0 : 1);
                Chip nowchip = GetNotes.GetNowNote(SongData.NowTJA[0].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value, true);
                Chip nowchip2p = GetNotes.GetNowNote(SongData.NowTJA[1].Courses[Game.Course[1]].ListChip, Game.MainTimer.Value, true);
                DrawNumber(0, Notes.NotesP[0].Y + 2 + 52, $"{(nowchip != null ? nowchip.Scroll * Notes.Scroll[0] : Notes.Scroll[0]),9:F2}", 0);//HS
                DrawNumber(0, Notes.NotesP[0].Y + 2 + 92, $"{(nowchip != null ? nowchip.Bpm : SongData.NowTJA[0].Header.BPM),9:F1}", 0);
                if (Game.Play2P)
                {
                    DrawNumber(0, Notes.NotesP[0].Y + 2 + 331, $"{(EXScore[1] > 0 ? EXScore[1] : Auto[1] * 2),9}", EXScore[1] > 0 || Auto[1] == 0 ? 0 : 5);
                    DrawMiniNumber(136, Notes.NotesP[0].Y + 29 + 331, $"+{(EXScore[1] > 0 ? Roll[1] : AutoRoll[1]),4}", EXScore[1] > 0 || Auto[1] == 0 ? 0 : 1);
                    DrawNumber(72, Notes.NotesP[0].Y + 2 + 331 + 52, $"{(nowchip2p != null ? nowchip2p.Scroll * Notes.Scroll[1] : Notes.Scroll[1]),6:F2}", 0);//HS
                    DrawNumber(0, Notes.NotesP[0].Y + 2 + 331 + 92, $"{(nowchip2p != null ? nowchip2p.Bpm : SongData.NowTJA[1].Header.BPM),9:F1}", 0);
                }
            }

            if (!Create.CreateMode)
            {
                switch ((EPreviewType)PlayData.Data.PreviewType)
                {
                    case EPreviewType.Up:
                        for (int i = 0; i < 6; i++)
                        {
                            Tx.Game_Judge_Data.Draw(20, 24 + 32 * i + 512, new Rectangle(0, 42 * i, 134, 42));
                        }
                        DrawMiniNumber(160, 36 + 32 * 0 + 512, $"{Perfect[0] + Auto[0]}", Auto[0] > 0 ? 1 : 0);
                        DrawMiniNumber(160, 36 + 32 * 1 + 512, $"{Great[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 2 + 512, $"{Good[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 3 + 512, $"{Bad[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 4 + 512, $"{Poor[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 5 + 512, $"{Roll[0] + AutoRoll[0]}", AutoRoll[0] > 0 ? 1 : 0);
                        if (Game.Play2P)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Tx.Game_Judge_Data.Draw(20, 824 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                            }
                            DrawMiniNumber(160, 836 + 32 * 0, $"{Perfect[1] + Auto[1]}", Auto[1] > 0 ? 1 : 0);
                            DrawMiniNumber(160, 836 + 32 * 1, $"{Great[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 2, $"{Good[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 3, $"{Bad[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 4, $"{Poor[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 5, $"{Roll[1] + AutoRoll[1]}", AutoRoll[1] > 0 ? 1 : 0);
                        }
                        break;
                    case EPreviewType.Down:
                        for (int i = 0; i < 6; i++)
                        {
                            Tx.Game_Judge_Data.Draw(20, 24 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                        }
                        DrawMiniNumber(160, 36 + 32 * 0, $"{Perfect[0] + Auto[0]}", Auto[0] > 0 ? 1 : 0);
                        DrawMiniNumber(160, 36 + 32 * 1, $"{Great[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 2, $"{Good[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 3, $"{Bad[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 4, $"{Poor[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 5, $"{Roll[0] + AutoRoll[0]}", AutoRoll[0] > 0 ? 1 : 0);
                        if (Game.Play2P)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Tx.Game_Judge_Data.Draw(20, 824 + 32 * i - 512, new Rectangle(0, 42 * i, 134, 42));
                            }
                            DrawMiniNumber(160, 836 + 32 * 0 - 512, $"{Perfect[1] + Auto[1]}", Auto[1] > 0 ? 1 : 0);
                            DrawMiniNumber(160, 836 + 32 * 1 - 512, $"{Great[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 2 - 512, $"{Good[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 3 - 512, $"{Bad[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 4 - 512, $"{Poor[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 5 - 512, $"{Roll[1] + AutoRoll[1]}", AutoRoll[1] > 0 ? 1 : 0);
                        }
                        break;
                    case EPreviewType.Normal:
                        for (int i = 0; i < 6; i++)
                        {
                            Tx.Game_Judge_Data.Draw(20, 24 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                        }
                        DrawMiniNumber(160, 36 + 32 * 0, $"{Perfect[0] + Auto[0]}", Auto[0] > 0 ? 1 : 0);
                        DrawMiniNumber(160, 36 + 32 * 1, $"{Great[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 2, $"{Good[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 3, $"{Bad[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 4, $"{Poor[0]}", 0);
                        DrawMiniNumber(160, 36 + 32 * 5, $"{Roll[0] + AutoRoll[0]}", AutoRoll[0] > 0 ? 1 : 0);
                        if (Game.Play2P)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Tx.Game_Judge_Data.Draw(20, 824 + 32 * i, new Rectangle(0, 42 * i, 134, 42));
                            }
                            DrawMiniNumber(160, 836 + 32 * 0, $"{Perfect[1] + Auto[1]}", Auto[1] > 0 ? 1 : 0);
                            DrawMiniNumber(160, 836 + 32 * 1, $"{Great[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 2, $"{Good[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 3, $"{Bad[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 4, $"{Poor[1]}", 0);
                            DrawMiniNumber(160, 836 + 32 * 5, $"{Roll[1] + AutoRoll[1]}", AutoRoll[1] > 0 ? 1 : 0);
                        }
                        break;
                }
            }

            int[] rankvalue = new int[] { 0, SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 4 / 9,
                SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 6 / 9, SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 8 / 9,
                SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 10 / 9, SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 12 / 9,
                SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 14 / 9, SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 16 / 9,
                SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 2};
            int[] rankvalue2p = new int[] { 0, SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 4 / 9,
                SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 6 / 9, SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 8 / 9,
                SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 10 / 9, SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 12 / 9,
                SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 14 / 9, SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 16 / 9,
                SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes * 2};
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying && !PlayData.Data.ShowResultScreen)
            {
                ERank rank = Rank[0];
                if (Math.Abs(EXScore[0] + Auto[0] * 2 - rankvalue[(int)Rank[0]]) > Math.Abs(EXScore[0] + Auto[0] * 2 - rankvalue[(int)Rank[0] + 1]))
                {
                    rank = Rank[0] + 1;
                }
                Drawing.Text(Notes.NotesP[0].X + 200, Notes.NotesP[0].Y + 86, $"SC:{EXScore[0]}", 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 300, Notes.NotesP[0].Y + 86, $"PG:{Perfect[0] + Auto[0]}", Auto[0] > 0 ? 0x00ff00 : 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 400, Notes.NotesP[0].Y + 86, $"GR:{Great[0]}", 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 500, Notes.NotesP[0].Y + 86, $"GD:{Good[0]}", 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 600, Notes.NotesP[0].Y + 86, $"BD:{Bad[0]}", 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 700, Notes.NotesP[0].Y + 86, $"PR:{Poor[0]}", 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 800, Notes.NotesP[0].Y + 86, $"RL:{Roll[0]}", 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 200, Notes.NotesP[0].Y + 106, $"Rank:{Rank[0]}", 0xffffff);
                Drawing.Text(Notes.NotesP[0].X + 300, Notes.NotesP[0].Y + 106, $"{rank}{((EXScore[0] + Auto[0] == SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes) || (rank == Rank[0]) ? "+" : "")}{EXScore[0] + Auto[0] * 2 - rankvalue[(int)rank]}", 0xffffff);
                if (!Game.Play2P && EXScore[0] > PlayMemory.BestData.Score) Drawing.Text(Notes.NotesP[0].X + 400, Notes.NotesP[0].Y + 106, "New Record", 0xffffff);
                if (Game.Play2P)
                {
                    ERank rank2p = Rank[1];
                    if (Math.Abs(EXScore[1] + Auto[1] * 2 - rankvalue2p[(int)Rank[1]]) > Math.Abs(EXScore[1] + Auto[1] * 2 - rankvalue2p[(int)Rank[1] + 1]))
                    {
                        rank2p = Rank[1] + 1;
                    }
                    Drawing.Text(Notes.NotesP[1].X + 200, Notes.NotesP[1].Y + 86, $"SC:{EXScore[1]}", 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 300, Notes.NotesP[1].Y + 86, $"PG:{Perfect[1] + Auto[1]}", Auto[1] > 0 ? 0x00ff00 : 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 400, Notes.NotesP[1].Y + 86, $"GR:{Great[1]}", 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 500, Notes.NotesP[1].Y + 86, $"GD:{Good[1]}", 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 600, Notes.NotesP[1].Y + 86, $"BD:{Bad[1]}", 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 700, Notes.NotesP[1].Y + 86, $"PR:{Poor[1]}", 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 800, Notes.NotesP[1].Y + 86, $"RL:{Roll[1]}", 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 200, Notes.NotesP[1].Y + 106, $"Rank:{Rank[1]}", 0xffffff);
                    Drawing.Text(Notes.NotesP[1].X + 300, Notes.NotesP[1].Y + 106, $"{rank2p}{((EXScore[1] + Auto[1] == SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes) || (rank2p == Rank[1]) ? "+" : "")}{EXScore[1] + Auto[1] * 2 - rankvalue2p[(int)rank2p]}", 0xffffff);
                }
            }

            if (PlayData.Data.PreviewType == 3)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (SongData.NowTJA[i].Courses[Game.Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Game.Course[i] == Game.Course[i - 1]) break;
                        count++;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    Chip rnowchip = GetNotes.GetNowNote(SongData.NowTJA[i].Courses[Game.Course[i]].ListChip, Game.MainTimer.Value - Game.Adjust[i]);
                    ERoll roll = rnowchip != null ? ProcessNote.RollState(rnowchip) : ERoll.None;
                    if (roll != ERoll.None && !rnowchip.IsHit) { RollCounter[i].Reset(); RollCounter[i].Start(); }
                    if (RollCounter[i].State != 0)
                    {
                        if ((roll == ERoll.Roll || roll == ERoll.ROLL)) NowRoll[i] = Game.MainTimer.State == 0 ? 0 : ProcessNote.NowRoll[i];
                        else if ((roll == ERoll.Balloon || roll == ERoll.Kusudama)) NowRoll[i] = ProcessNote.BalloonRemain[i];

                        DrawNumber(Notes.NotesP[i].X + 160, Notes.NotesP[i].Y + 82, $"{NowRoll[i]}", 0);
                    }
                }
            }
            else
            {
                Chip rnowchip = GetNotes.GetNowNote(SongData.NowTJA[0].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value - Game.Adjust[0]);
                Chip rnowchip2p = GetNotes.GetNowNote(SongData.NowTJA[1].Courses[Game.Course[1]].ListChip, Game.MainTimer.Value - Game.Adjust[1]);
                ERoll roll = rnowchip != null ? ProcessNote.RollState(rnowchip) : ERoll.None;
                ERoll roll2p = rnowchip2p != null ? ProcessNote.RollState(rnowchip2p) : ERoll.None;
                if (roll != ERoll.None && !rnowchip.IsHit) { RollCounter[0].Reset(); RollCounter[0].Start(); }
                if (roll2p != ERoll.None && !rnowchip2p.IsHit) { RollCounter[1].Reset(); RollCounter[1].Start(); }
                if (RollCounter[0].State != 0)
                {
                    if ((roll == ERoll.Roll || roll == ERoll.ROLL)) NowRoll[0] = Game.MainTimer.State == 0 ? 0 : ProcessNote.NowRoll[0];
                    else if ((roll == ERoll.Balloon || roll == ERoll.Kusudama)) NowRoll[0] = ProcessNote.BalloonRemain[0];

                    DrawNumber(Notes.NotesP[0].X + 160, Notes.NotesP[0].Y + 82, $"{NowRoll[0]}", 0);
                }
                if (Game.Play2P && RollCounter[1].State != 0)
                {
                    if ((roll2p == ERoll.Roll || roll2p == ERoll.ROLL)) NowRoll[1] = Game.MainTimer.State == 0 ? 0 : ProcessNote.NowRoll[1];
                    else if ((roll2p == ERoll.Balloon || roll2p == ERoll.Kusudama)) NowRoll[1] = ProcessNote.BalloonRemain[1];
                    DrawNumber(Notes.NotesP[1].X + 160, Notes.NotesP[1].Y + 82, $"{NowRoll[1]}", 0);
                }
            }


            #if DEBUG
            Drawing.Text(0, 300, $"SC:{EXScore[0]}", 0xffffff);
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying) Drawing.Text(80, 300, Remain[0] > 0 ? $"MAX-{Remain[0]}" : "MAX+0", 0xffffff);
            Drawing.Text(0, 320, $"PG:{Perfect[0]}", 0xffffff);
            Drawing.Text(0, 340, $"GR:{Great[0]}", 0xffffff);
            Drawing.Text(0, 360, $"GD:{Good[0]}", 0xffffff);
            Drawing.Text(0, 380, $"BD:{Bad[0]}", 0xffffff);
            Drawing.Text(0, 400, $"PR:{Poor[0]}", 0xffffff);
            Drawing.Text(0, 420, $"AT:{Auto[0]}", 0xffffff);
            Drawing.Text(0, 440, $"RL:{Roll[0] + AutoRoll[0]}({RollYellow[0]},{RollBalloon[0]})", 0xffffff);

            Drawing.Text(200, 300, $"{Gauge[0]}", 0xffffff);
            Drawing.Text(200, 320, $"Total:{Total[0]}", 0xffffff);
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying) Drawing.Text(200, 340, Cleared[0] ? "Cleared" : "Failed", 0xffffff);
            Drawing.Text(200, 360, $"Combo:{MaxCombo[0]}", 0xffffff);
            Drawing.Text(200, 380, $"Rank:{Rank[0]}", 0xffffff);

            if (JudgeCounter.State == TimerState.Started || JudgeCounterBig.State == TimerState.Started)
            {
                Drawing.Text(600, 260, $"{DisplayJudge[0]}", 0xffffff);
                Drawing.Text(600, 280, $"{Math.Round(msJudge[0], 2, MidpointRounding.AwayFromZero)}", 0xffffff);
            }

            if (Game.Play2P)
            {
                Drawing.Text(0, 560, $"SC:{EXScore[1]}", 0xffffff);
                if (Game.IsSongPlay && !Game.MainSong.IsPlaying) Drawing.Text(80, 560, Remain[1] > 0 ? $"MAX-{Remain[1]}" : "MAX+0", 0xffffff);
                Drawing.Text(0, 580, $"PG:{Perfect[1]}", 0xffffff);
                Drawing.Text(0, 600, $"GR:{Great[1]}", 0xffffff);
                Drawing.Text(0, 620, $"GD:{Good[1]}", 0xffffff);
                Drawing.Text(0, 640, $"BD:{Bad[1]}", 0xffffff);
                Drawing.Text(0, 660, $"PR:{Poor[1]}", 0xffffff);
                Drawing.Text(0, 680, $"AT:{Auto[1]}", 0xffffff);
                Drawing.Text(0, 700, $"RL:{Roll[1] + AutoRoll[1]}({RollYellow[1]},{RollBalloon[1]})", 0xffffff);

                Drawing.Text(200, 560, $"{Gauge[1]}", 0xffffff);
                Drawing.Text(200, 580, $"Total:{Total[1]}", 0xffffff);
                if (Game.IsSongPlay && !Game.MainSong.IsPlaying) Drawing.Text(200, 600, Cleared[1] ? "Cleared" : "Failed", 0xffffff);
                Drawing.Text(200, 620, $"Combo:{MaxCombo[1]}", 0xffffff);
                Drawing.Text(200, 640, $"Rank:{Rank[1]}", 0xffffff);

                if (JudgeCounter2P.State == TimerState.Started || JudgeCounterBig2P.State == TimerState.Started)
                {
                    Drawing.Text(600, 520, $"{DisplayJudge[1]}", 0xffffff);
                    Drawing.Text(600, 540, $"{Math.Round(msJudge[1], 2, MidpointRounding.AwayFromZero)}", 0xffffff);
                }
            }
            #endif
            base.Draw();
        }

        public override void Update()
        {
            Remain[0] = Hit[0] * 2 - EXScore[0];
            Gauge[0] = GaugeList[0][(int)GaugeType[0]];
            Cleared[0] = Gauge[0] >= ClearRate[0][(int)GaugeType[0]] ? true : false;
            Rank[0] = GetRank(EXScore[0] + Auto[0] * 2, 0);
            msAverage[0] = Hit[0] > 0 ? msSum[0] / Hit[0] : 0;
            if (Game.Play2P)
            {
                Remain[1] = Hit[1] * 2 - EXScore[1];
                Gauge[1] = GaugeList[1][(int)GaugeType[1]];
                Cleared[1] = Gauge[1] >= ClearRate[1][(int)GaugeType[1]] ? true : false;
                Rank[1] = GetRank(EXScore[1] + Auto[1] * 2, 1);
                msAverage[1] = Hit[1] > 0 ? msSum[1] / Hit[1] : 0;
            }

            if (GaugeType[0] >= EGauge.Hard && Gauge[0] == 0 && (PlayData.Data.GaugeAutoShift[0] == (int)EGaugeAutoShift.None || PlayData.Data.GaugeAutoShift[0] == (int)EGaugeAutoShift.Retry))
            {
                Game.Failed[0] = true;
            }
            if (Game.Play2P && GaugeType[1] >= EGauge.Hard && Gauge[1] == 0 && (PlayData.Data.GaugeAutoShift[1] == (int)EGaugeAutoShift.None || PlayData.Data.GaugeAutoShift[1] == (int)EGaugeAutoShift.Retry))
            {
                Game.Failed[1] = true;
            }

            JudgeCounter.Tick();
            JudgeCounter2P.Tick();
            JudgeCounterBig.Tick();
            JudgeCounterBig2P.Tick();
            BombAnimation.Tick();
            BombAnimation.Start();
            for (int i = 0; i < 5; i++)
            {
                RollCounter[i].Tick();
            }
            Active.Tick();
            base.Update();
        }

        public static void DrawJudge(int player, bool isBig)
        {
            if (player == 0)
            {
                if (isBig)
                {
                    JudgeCounterBig.Reset();
                    JudgeCounterBig.Start();
                }
                else
                {
                    JudgeCounter.Reset();
                    JudgeCounter.Start();
                }

            }
            else
            {
                if (isBig)
                {
                    JudgeCounterBig2P.Reset();
                    JudgeCounterBig2P.Start();
                }
                else
                {
                    JudgeCounter2P.Reset();
                    JudgeCounter2P.Start();
                }
            }
        }
        public static void DrawJudge()
        {
            if (JudgeCounter.State == TimerState.Started)
            {
                if (DisplayJudge[0] != EJudge.Auto)
                    DrawMiniNumber(Notes.NotesP[0].X + 178, Notes.NotesP[0].Y - 6, msJudge[0] >= 1.0 ? $"+{(int)msJudge[0]}" : $"{(int)msJudge[0]}", msJudge[0] > GetNotes.range[0] ? 1 : (msJudge[0] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[0])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[0].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        Tx.Game_Bomb[0].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[4].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        Tx.Game_Bomb[4].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[5].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        Tx.Game_Bomb[5].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
            if (JudgeCounterBig.State == TimerState.Started)
            {
                if (DisplayJudge[0] != EJudge.Auto)
                    DrawMiniNumber(Notes.NotesP[0].X + 178, Notes.NotesP[0].Y - 6, msJudge[0] >= 1.0 ? $"+{(int)msJudge[0]}" : $"{(int)msJudge[0]}", msJudge[0] > GetNotes.range[0] ? 1 : (msJudge[0] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[0])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[6].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        Tx.Game_Bomb[6].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[10].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        Tx.Game_Bomb[10].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[11].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        Tx.Game_Bomb[11].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) Tx.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
            if (JudgeCounter2P.State == TimerState.Started)
            {
                if (DisplayJudge[1] != EJudge.Auto)
                    DrawMiniNumber(Notes.NotesP[1].X + 178, Notes.NotesP[1].Y - 6, msJudge[1] >= 1.0 ? $"+{(int)msJudge[1]}" : $"{(int)msJudge[1]}", msJudge[1] > GetNotes.range[0] ? 1 : (msJudge[1] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[1])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[0].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        Tx.Game_Bomb[0].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[4].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        Tx.Game_Bomb[4].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[5].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        Tx.Game_Bomb[5].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
            if (JudgeCounterBig2P.State == TimerState.Started)
            {
                if (DisplayJudge[1] != EJudge.Auto)
                    DrawMiniNumber(Notes.NotesP[1].X + 178, Notes.NotesP[1].Y - 6, msJudge[1] >= 1.0 ? $"+{(int)msJudge[1]}" : $"{(int)msJudge[1]}", msJudge[1] > GetNotes.range[0] ? 1 : (msJudge[1] < -GetNotes.range[0] ? 2 : 0));
                switch (DisplayJudge[1])
                {
                    case EJudge.Perfect:
                        Tx.Game_Bomb[6].Opacity = 1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        Tx.Game_Bomb[6].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        Tx.Game_Bomb[10].Opacity = 1.0 - (JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        Tx.Game_Bomb[10].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        Tx.Game_Bomb[11].Opacity = 1.0 - (JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        Tx.Game_Bomb[11].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        Tx.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) Tx.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
                        break;
                }
            }
        }

        public static int Digit(int num)
        {
            int digit = 1;
            for (int i = num; i >= 10; i /= 10)
            {
                digit++;
            }
            return digit;
        }
        public static void DrawCombo(int player)
        {
            int color = 0;
            switch (Game.Course[player])
            {
                case (int)ECourse.Easy:
                    if (Combo[player] >= 200)
                        color = 4;
                    else if (Combo[player] >= 30)
                        color = 3;
                    else if (Combo[player] >= 10)
                        color = 2;
                    break;
                case (int)ECourse.Normal:
                    if (Combo[player] >= 300)
                        color = 4;
                    else if (Combo[player] >= 30)
                        color = 3;
                    else if (Combo[player] >= 10)
                        color = 2;
                    break;
                case (int)ECourse.Hard:
                    if (Combo[player] >= 500)
                        color = 4;
                    else if (Combo[player] >= 50)
                        color = 3;
                    else if (Combo[player] >= 30)
                        color = 2;
                    break;
                case (int)ECourse.Oni:
                case (int)ECourse.Edit:
                    if (Combo[player] >= 1000)
                        color = 4;
                    else if (Combo[player] >= 100)
                        color = 3;
                    else if (Combo[player] >= 50)
                        color = 2;
                    break;
            }
            //if (Combo[player] >= 10)
            if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
            {
                DrawNumber(411 - 12 * Digit(Combo[player]), Notes.NotesP[player].Y + 82, $"{Combo[player]}", color);
            }
            else
            {
                DrawNumber(411 - 12 * Digit(Combo[player]), Notes.NotesP[0].Y + 82 + 262 * player, $"{Combo[player]}", color);
            }
        }

        public static void DrawGraph()
        {
            if (PlayData.Data.PreviewType > 0) return;

            Tx.Game_Graph_Base.Draw(495, 0);

            double allnotes = Perfect[0] + Great[0] + Good[0] + Bad[0] + Poor[0] + Auto[0];
            double hitnotes = EXScore[0] > 0 ? EXScore[0] / 2.0 : Auto[0];
            double percent = allnotes > 0 ? hitnotes / allnotes : 0;
            double hitpercent = hitnotes / SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes;
            double allpercent = allnotes / SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes;
            int score = (int)(SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * percent * 2);
            ERank rank = GetRank(score, 0);

            Tx.Game_Graph.Draw(499, 163, new Rectangle((int)(1000 - 1000 * hitpercent), 64 * 2, (int)(1000 * hitpercent), 64));

            if (PlayMemory.BestData.Score > 0)
            {
                double bestallnotes = PlayMemory.BestData.Score;
                double bestnotes = EXScore[2];
                double bestpercent = bestnotes / (SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                double bestallpercent = bestallnotes / (SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                Tx.Game_Graph.Draw(499, 163 - 76, new Rectangle((int)(1000 - 1000 * bestallpercent), 64 * 3, (int)(1000 * bestallpercent), 64));
                if (PlayMemory.BestData.Chip != null)
                {
                    Tx.Game_Graph.Draw(499, 163 - 76, new Rectangle((int)(1000 - 1000 * bestpercent), 64, (int)(1000 * bestpercent), 64));
                    int num = (int)hitnotes * 2 - (int)bestnotes;
                    DrawMiniNumber(499 + 8, 183 - 76, $"{(num >= 0 ? "+" : "")}{num}", num < 0 ? 1 : 0);
                }
            }

            switch ((ERival)PlayData.Data.RivalType)
            {
                case ERival.Percent:
                    double allvalue = PlayData.Data.RivalPercent / 100.0;
                    double value = allvalue * allpercent;
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * allvalue), 64 * 3, (int)(1000 * allvalue), 64));
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * value), 0, (int)(1000 * value), 64));
                    int num = (int)hitnotes * 2 - (int)(SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 2 * value);
                    DrawMiniNumber(499 + 8, 183 - 76 * 2, $"{(num >= 0 ? "+" : "")}{num}", num < 0 ? 1 : 0);
                    DrawNumber(499 - 178, 181 - 76 * 2, $"{PlayData.Data.RivalPercent,6:F2}%", 0);
                    break;
                case ERival.Rank:
                    double rpercent = GetRankToPercent((ERank)PlayData.Data.RivalRank, 0);
                    double rvalue = rpercent * allpercent;
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rpercent), 64 * 3, (int)(1000 * rpercent), 64));
                    Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rvalue), 0, (int)(1000 * rvalue), 64));
                    int rnum = (int)hitnotes * 2 - (int)(SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 2 * rvalue);
                    DrawMiniNumber(499 + 8, 183 - 76 * 2, $"{(rnum >= 0 ? "+" : "")}{rnum}", rnum < 0 ? 1 : 0);
                    Tx.Result_Rank.Draw(499 - 152, 175 - 76 * 2, new Rectangle(0, PlayData.Data.RivalRank > 7 ? 45 * 7 : 45 * PlayData.Data.RivalRank, 161, 45));
                    break;
                case ERival.PlayScore:
                    if (PlayMemory.RivalData.Score > 0)
                    {
                        double rivalallnotes = PlayMemory.RivalData.Score;
                        double rivalnotes = EXScore[3];
                        double rivalpercent = rivalnotes / (SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                        double rivalallpercent = rivalallnotes / (SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                        Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rivalallpercent), 64 * 3, (int)(1000 * rivalallpercent), 64));
                        if (PlayMemory.RivalData.Chip != null)
                        {
                            Tx.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rivalpercent), 0, (int)(1000 * rivalpercent), 64));
                            int rivalnum = (int)hitnotes * 2 - (int)rivalnotes;
                            DrawMiniNumber(499 + 8, 183 - 76 * 2, $"{(rivalnum >= 0 ? "+" : "")}{rivalnum}", rivalnum < 0 ? 1 : 0);
                        }
                    }
                    break;
            }

            DrawNumber(1536, 196, $"{score, 4}", 0);
            Tx.Game_Rank.Draw(1622, 152, new Rectangle(0, 90 * (int)rank, 161 * 2, 45 * 2));
        }

        public static void AddScore(EJudge judge, int player)
        {
            if (player < 2)
            {
                AddGauge(judge, player);
                DisplayJudge[player] = judge;
            }
            switch (judge)
            {
                case EJudge.Perfect:
                    EXScore[player] += 2;
                    break;
                case EJudge.Great:
                    EXScore[player] += 1;
                    break;
            }
            if (PlayData.Data.PreviewType == 3 || (PlayData.Data.PreviewType < 3 && player < 2))
            {
                switch (judge)
                {
                    case EJudge.Perfect:
                        Perfect[player]++;
                        Combo[player]++;
                        if (Combo[player] > MaxCombo[player]) MaxCombo[player]++;
                        break;
                    case EJudge.Great:
                        Great[player]++;
                        Combo[player]++;
                        if (Combo[player] > MaxCombo[player]) MaxCombo[player]++;
                        break;
                    case EJudge.Good:
                        Good[player]++;
                        Combo[player]++;
                        if (Combo[player] > MaxCombo[player]) MaxCombo[player]++;
                        break;
                    case EJudge.Bad:
                        Bad[player]++;
                        Combo[player] = 0;
                        break;
                    case EJudge.Poor:
                    case EJudge.Through:
                        Poor[player]++;
                        Combo[player] = 0;
                        break;
                    case EJudge.Auto:
                        Auto[player]++;
                        Combo[player]++;
                        if (Combo[player] > MaxCombo[player]) MaxCombo[player]++;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void AddRoll(int player)
        {
            if (PlayData.Data.PreviewType == 3 || Game.IsAuto[player])
            {
                AutoRoll[player]++;
            }
            else
            {
                Roll[player]++;
                RollYellow[player]++;
            }
        }
        public static void AddBalloon(int player)
        {
            if (PlayData.Data.PreviewType == 3 || Game.IsAuto[player])
            {
                AutoRoll[player]++;
            }
            else
            {
                Roll[player]++;
                RollBalloon[player]++;
            }
        }

        public static void AddGauge(EJudge judge, int player)
        {
            if (Game.MainTimer.State == 0 && GaugeType[player] >= EGauge.Hard) return;

            double[] gaugepernote = new double[2] { Total[0] / SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes, Total[1] / SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes };
            double[] gauge = new double[6];
            int Notes = PlayData.Data.Hazard[player] > SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes ? SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes : PlayData.Data.Hazard[player];
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
                GaugeList[player][i] = Math.Round(GaugeList[player][i], 4, MidpointRounding.AwayFromZero);
            }
            ShiftGauge(player);
        }


        public static void DeleteGauge(int player)
        {
            double[] gaugepernote = new double[2] { Total[0] / SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes, Total[1] / SongData.NowTJA[1].Courses[Game.Course[1]].TotalNotes };
            double[] gauge = new double[6];

            for (int i = 0; i < 6; i++)
            {
                if ((100.0 / gaugepernote[player]) >= Auto[player])
                {
                    gauge[i] = -gaugepernote[player];
                }
            }
            gauge[3] = 0; gauge[4] = 0; gauge[5] = 0;
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

            ShiftGauge(player);
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
                        switch (SongData.NowTJA[player].Courses[Game.Course[player]].LEVEL)
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
                        switch (SongData.NowTJA[player].Courses[Game.Course[player]].LEVEL)
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
                        switch (SongData.NowTJA[player].Courses[Game.Course[player]].LEVEL)
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
                        switch (SongData.NowTJA[player].Courses[Game.Course[player]].LEVEL)
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

            Total[player] = SongData.NowTJA[player].Courses[Game.Course[player]].TOTAL > 0.0 ? SongData.NowTJA[player].Courses[Game.Course[player]].TOTAL : total[player];
            GoodRate[player] = goodrate[player];
            BadRate[player] = badrate[player];
            PoorRate[player] = poorrate[player];

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
            GaugeList[player] = new double[6] { 0.0, 0.0, 0.0, 100.0, 100.0, 100.0 };
            ClearRate[player] = new double[6] { 80.0, 60.0, 80.0, 0.01, 0.01, 0.01 };

            if (PlayData.Data.GaugeType[player] >= (int)EGauge.Hard && PlayData.Data.GaugeType[player] <= (int)EGauge.Hazard)
            {
                Gauge[player] = 100.0;
            }
            else
            {
                Gauge[player] = 0;
            }
        }

        public static ERank GetRank(int value, int player)
        {
            if (value >= SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes * 16 / 9)
            {
                return ERank.AAA;
            }
            else if (value >= SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes * 14 / 9)
            {
                return ERank.AA;
            }
            else if (value >= SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes * 12 / 9)
            {
                return ERank.A;
            }
            else if (value >= SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes * 10 / 9)
            {
                return ERank.B;
            }
            else if (value >= SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes * 8 / 9)
            {
                return ERank.C;
            }
            else if (value >= SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes * 6 / 9)
            {
                return ERank.D;
            }
            else if (value >= SongData.NowTJA[player].Courses[Game.Course[player]].TotalNotes * 4 / 9)
            {
                return ERank.E;
            }
            else
            {
                return ERank.F;
            }
        }

        public static double GetRankToPercent(ERank rank, int player)
        {
            switch (rank)
            {
                case ERank.MAX:
                    return 1.0;
                case ERank.AAA:
                    return 8.0 / 9;
                case ERank.AA:
                    return 7.0 / 9;
                case ERank.A:
                    return 6.0 / 9;
                case ERank.B:
                    return 5.0/ 9;
                case ERank.C:
                    return 4.0 / 9;
                case ERank.D:
                    return 3.0 / 9;
                case ERank.E:
                    return 2.0 / 9;
                case ERank.F:
                default:
                    return 0;
            }
        }

        public static void DrawNumber(double x, double y, string num, int type)
        {
            foreach (char ch in num)
            {
                for (int i = 0; i < stNumber.Length; i++)
                {
                    if (ch == ' ')
                    {
                        break;
                    }
                    if (stNumber[i].ch == ch)
                    {
                        Tx.Game_Number.Draw(x, y, new Rectangle(stNumber[i].X, 28 * type, 26, 28));
                        break;
                    }
                }
                x += 24;
            }
        }

        public static void DrawMiniNumber(double x, double y, string num, int type)
        {
            foreach (char ch in num)
            {
                for (int i = 0; i < stMiniNumber.Length; i++)
                {
                    if (ch == ' ')
                    {
                        break;
                    }
                    if (stMiniNumber[i].ch == ch)
                    {
                        Tx.Game_Number_Mini.Draw(x, y, new Rectangle(stMiniNumber[i].X, 18 * type, 18, 18));
                        break;
                    }
                }
                x += 16;
            }
        }

        public static EGauge[] GaugeType = new EGauge[2];
        public static int[] EXScore = new int[5], Perfect = new int[2], Great = new int[2], Good = new int[2], Bad = new int[2], Poor = new int[5], Auto = new int[5],
            Hit = new int[5], Roll = new int[2], RollYellow = new int[2], RollBalloon = new int[2], AutoRoll = new int[5], Remain = new int[5], Combo = new int[5], MaxCombo = new int[5], NowRoll = new int[5];
        public static double[] msJudge = new double[2], Gauge = new double[2], Total = new double[2], GoodRate = new double[2], BadRate = new double[2], PoorRate = new double[2], msSum = new double[2], msAverage = new double[2];
        public static double[][] GaugeList = new double[2][], ClearRate = new double[2][];
        public static bool[] Cleared = new bool[2];
        private static EJudge[] DisplayJudge = new EJudge[2];
        public static ERank[] Rank = new ERank[2];
        public static Counter JudgeCounter, JudgeCounterBig, JudgeCounter2P, JudgeCounterBig2P, BombAnimation, Active;
        public static Counter[] RollCounter = new Counter[5];

        private struct STNumber
        {
            public char ch;
            public int X;
        }
        private static STNumber[] stNumber = new STNumber[13]
        { new STNumber(){ ch = '0', X = 0 },new STNumber(){ ch = '1', X = 26 },new STNumber(){ ch = '2', X = 26 * 2 },new STNumber(){ ch = '3', X = 26 * 3 },new STNumber(){ ch = '4', X = 26 * 4 },
        new STNumber(){ ch = '5', X = 26 * 5 },new STNumber(){ ch = '6', X = 26 * 6 },new STNumber(){ ch = '7', X = 26 * 7 },new STNumber(){ ch = '8', X = 26 * 8 },new STNumber(){ ch = '9', X = 26 * 9 },
        new STNumber(){ ch = '.', X = 26 * 10 },new STNumber(){ ch = '%', X = 26 * 11 },new STNumber(){ ch = '-', X = 26 * 12 } };
        private static STNumber[] stMiniNumber = new STNumber[13]
        { new STNumber(){ ch = '0', X = 0 },new STNumber(){ ch = '1', X = 18 },new STNumber(){ ch = '2', X = 18 * 2 },new STNumber(){ ch = '3', X = 18 * 3 },new STNumber(){ ch = '4', X = 18 * 4 },
        new STNumber(){ ch = '5', X = 18 * 5 },new STNumber(){ ch = '6', X = 18 * 6 },new STNumber(){ ch = '7', X = 18 * 7 },new STNumber(){ ch = '8', X = 18 * 8 },new STNumber(){ ch = '9', X = 18 * 9 },
        new STNumber(){ ch = '.', X = 18 * 10 },new STNumber(){ ch = '+', X = 18 * 11 },new STNumber(){ ch = '-', X = 18 * 12 } };
    }

    public class ScoreBoard
    {
        public int EXScore;
        public int Perfect;
        public int Great;
        public int Good;
        public int Bad;
        public int Poor;
        public int Auto;
        public int Roll;
        public (int, int) RollDetail;
        public int AutoRoll;
        public int MaxCombo;

        public int Hit()
        {
            return Auto + Perfect + Great + Good;
        }
        public int BadPoor()
        {
            return Bad + Poor;
        }
    }

    public enum EGauge
    {
        Normal,
        Assist,
        Easy,
        Hard,
        EXHard,
        Hazard,
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

    public enum ERank
    {
        F,
        E,
        D,
        C,
        B,
        A,
        AA,
        AAA,
        MAX
    }

    public enum ERival
    {
        None,
        Percent,
        Rank,
        PlayScore
    }
}
