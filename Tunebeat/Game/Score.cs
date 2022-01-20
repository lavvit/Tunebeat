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
            for (int i = 0; i < 4; i++)
            {
                EXScore[i] = 0;
            }
            for (int i = 0; i < 2; i++)
            {
                Perfect[i] = 0;
                Great[i] = 0;
                Good[i] = 0;
                Bad[i] = 0;
                Poor[i] = 0;
                Auto[i] = 0;
                Roll[i] = 0;
                AutoRoll[i] = 0;
                RollYellow[i] = 0;
                RollBalloon[i] = 0;
                Gauge[i] = 0;
                SetGauge(i);
                Combo[i] = 0;
                MaxCombo[i] = 0;
                msSum[i] = 0;
                Hit[i] = 0;
            }

            JudgeCounter = new Counter(0, 200, 1000, false);
            JudgeCounterBig = new Counter(0, 500, 1000, false);
            JudgeCounter2P = new Counter(0, 200, 1000, false);
            JudgeCounterBig2P = new Counter(0, 500, 1000, false);
            BombAnimation = new Counter(0, 1023, 1000, false);
            RollCounter = new Counter(0, 200, 1000, false);
            RollCounter2P = new Counter(0, 200, 1000, false);
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
            RollCounter.Reset();
            RollCounter2P.Reset();
            Active.Reset();
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
            DrawNumber(495 + 15, 286 - 48 + 12, $"{Gauge[0],5:F1}%", 0);
            if (PlayData.Data.IsPlay2P)
            {
                TextureLoad.Game_Gauge_Base.Draw(495, 286 + 465, new Rectangle(0, 48, 1425, 48));
                for (int i = 0; i < 100; i++)
                {
                    TextureLoad.Game_Gauge.Draw(495 + 179 + 12 * i, 286 + 465 + 2, new Rectangle(Color2P[i], (int)Gauge[1] > i ? 40 : 0, 10, 40));
                }
                DrawNumber(495 + 15, 286 + 465 + 8, $"{Gauge[1],5:F1}%", 0);
            }

            if (!PlayData.Data.IsPlay2P && PlayData.Data.ShowGraph)
            {
                DrawGraph();
            }

            DrawJudge();

            DrawNumber(0, 292, $"{(EXScore[0] > 0 ? EXScore[0] : Auto[0] * 2), 9}", EXScore[0] > 0 || Auto[0] == 0 ? 0 : 5);
            DrawMiniNumber(136, 319, $"+{(EXScore[0] > 0 ? Roll[0] : AutoRoll[0]),4}", EXScore[0] > 0 || Auto[0] == 0 ? 0 : 1);
            Chip nowchip = GetNotes.GetNowNote(Game.MainTJA[0].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value, true);
            Chip nowchip2p = GetNotes.GetNowNote(Game.MainTJA[1].Courses[Game.Course[1]].ListChip, Game.MainTimer.Value, true);
            DrawNumber(0, 292 + 52, $"{(nowchip != null ? nowchip.Scroll * Notes.Scroll[0] : Notes.Scroll[0]),9:F2}", 0);//HS
            DrawNumber(0, 292 + 92, $"{(nowchip != null ? nowchip.Bpm : Game.MainTJA[0].Header.BPM),9:F1}", 0);
            if (PlayData.Data.IsPlay2P)
            {
                DrawNumber(0, 292 + 331, $"{(EXScore[1] > 0 ? EXScore[1] : Auto[1] * 2),9}", EXScore[1] > 0 || Auto[1] == 0 ? 0 : 5);
                DrawMiniNumber(136, 319 + 331, $"+{(EXScore[1] > 0 ? Roll[1] : AutoRoll[1]),4}", EXScore[1] > 0 || Auto[1] == 0 ? 0 : 1);
                DrawNumber(72, 292 + 331 + 52, $"{(nowchip2p != null ? nowchip2p.Scroll * Notes.Scroll[1] : Notes.Scroll[1]),6:F2}", 0);//HS
                DrawNumber(0, 292 + 331 + 92, $"{(nowchip2p != null ? nowchip2p.Bpm : Game.MainTJA[1].Header.BPM),9:F1}", 0);
            }

            int[] rankvalue = new int[] { 0, Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 4 / 9,
                Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 6 / 9, Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 8 / 9,
                Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 10 / 9, Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 12 / 9,
                Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 14 / 9, Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 16 / 9,
                Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 2};
            int[] rankvalue2p = new int[] { 0, Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 4 / 9,
                Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 6 / 9, Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 8 / 9,
                Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 10 / 9, Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 12 / 9,
                Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 14 / 9, Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 16 / 9,
                Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes * 2};
            if (Game.IsSongPlay && !Game.MainSong.IsPlaying && !PlayData.Data.ShowResultScreen)
            {
                ERank rank = Rank[0];
                if (Math.Abs(EXScore[0] + Auto[0] * 2 - rankvalue[(int)Rank[0]]) > Math.Abs(EXScore[0] + Auto[0] * 2 - rankvalue[(int)Rank[0] + 1]))
                {
                    rank = Rank[0] + 1;
                }
                DrawString(Notes.NotesP[0].X + 200, Notes.NotesP[0].Y + 86, $"SC:{EXScore[0]}", 0xffffff);
                DrawString(Notes.NotesP[0].X + 300, Notes.NotesP[0].Y + 86, $"PG:{Perfect[0] + Auto[0]}", Auto[0] > 0 ? 0x00ff00 : (uint)0xffffff);
                DrawString(Notes.NotesP[0].X + 400, Notes.NotesP[0].Y + 86, $"GR:{Great[0]}", 0xffffff);
                DrawString(Notes.NotesP[0].X + 500, Notes.NotesP[0].Y + 86, $"GD:{Good[0]}", 0xffffff);
                DrawString(Notes.NotesP[0].X + 600, Notes.NotesP[0].Y + 86, $"BD:{Bad[0]}", 0xffffff);
                DrawString(Notes.NotesP[0].X + 700, Notes.NotesP[0].Y + 86, $"PR:{Poor[0]}", 0xffffff);
                DrawString(Notes.NotesP[0].X + 800, Notes.NotesP[0].Y + 86, $"RL:{Roll[0]}", 0xffffff);
                DrawString(Notes.NotesP[0].X + 200, Notes.NotesP[0].Y + 106, $"Rank:{Rank[0]}", 0xffffff);
                DrawString(Notes.NotesP[0].X + 300, Notes.NotesP[0].Y + 106, $"{rank}{((EXScore[0] + Auto[0] == Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes) || (rank == Rank[0]) ? "+" : "")}{EXScore[0] + Auto[0] * 2 - rankvalue[(int)rank]}", 0xffffff);
                if (PlayData.Data.IsPlay2P)
                {
                    ERank rank2p = Rank[1];
                    if (Math.Abs(EXScore[1] + Auto[1] * 2 - rankvalue2p[(int)Rank[1]]) > Math.Abs(EXScore[1] + Auto[1] * 2 - rankvalue2p[(int)Rank[1] + 1]))
                    {
                        rank2p = Rank[1] + 1;
                    }
                    DrawString(Notes.NotesP[1].X + 200, Notes.NotesP[1].Y + 86, $"SC:{EXScore[1]}", 0xffffff);
                    DrawString(Notes.NotesP[1].X + 300, Notes.NotesP[1].Y + 86, $"PG:{Perfect[1] + Auto[1]}", Auto[1] > 0 ? 0x00ff00 : (uint)0xffffff);
                    DrawString(Notes.NotesP[1].X + 400, Notes.NotesP[1].Y + 86, $"GR:{Great[1]}", 0xffffff);
                    DrawString(Notes.NotesP[1].X + 500, Notes.NotesP[1].Y + 86, $"GD:{Good[1]}", 0xffffff);
                    DrawString(Notes.NotesP[1].X + 600, Notes.NotesP[1].Y + 86, $"BD:{Bad[1]}", 0xffffff);
                    DrawString(Notes.NotesP[1].X + 700, Notes.NotesP[1].Y + 86, $"PR:{Poor[1]}", 0xffffff);
                    DrawString(Notes.NotesP[1].X + 800, Notes.NotesP[1].Y + 86, $"RL:{Roll[1]}", 0xffffff);
                    DrawString(Notes.NotesP[1].X + 200, Notes.NotesP[1].Y + 106, $"Rank:{Rank[1]}", 0xffffff);
                    DrawString(Notes.NotesP[1].X + 300, Notes.NotesP[1].Y + 106, $"{rank2p}{((EXScore[1] + Auto[1] == Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes) || (rank2p == Rank[1]) ? "+" : "")}{EXScore[1] + Auto[1] * 2 - rankvalue2p[(int)rank2p]}", 0xffffff);
                }
            }

            Chip rnowchip = GetNotes.GetNowNote(Game.MainTJA[0].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value - Game.Adjust[0]);
            Chip rnowchip2p = GetNotes.GetNowNote(Game.MainTJA[1].Courses[Game.Course[1]].ListChip, Game.MainTimer.Value - Game.Adjust[1]);
            ERoll roll = rnowchip != null ? ProcessNote.RollState(rnowchip) : ERoll.None;
            ERoll roll2p = rnowchip2p != null ? ProcessNote.RollState(rnowchip2p) : ERoll.None;
            if (roll != ERoll.None && !rnowchip.IsHit) { RollCounter.Reset(); RollCounter.Start(); }
            if (roll2p != ERoll.None && !rnowchip2p.IsHit) { RollCounter2P.Reset(); RollCounter2P.Start(); }
            if (RollCounter.State != 0)
            {
                if ((roll == ERoll.Roll || roll == ERoll.ROLL)) NowRoll[0] = Game.MainTimer.State == 0 ? 0 : ProcessNote.NowRoll[0];
                else if ((roll == ERoll.Balloon || roll == ERoll.Kusudama)) NowRoll[0] = ProcessNote.BalloonRemain[0];

                DrawNumber(Notes.NotesP[0].X + 160, Notes.NotesP[0].Y + 82, $"{NowRoll[0]}", 0);
            }
            if (PlayData.Data.IsPlay2P && RollCounter2P.State != 0)
            {
                if ((roll2p == ERoll.Roll || roll2p == ERoll.ROLL)) NowRoll[1] = Game.MainTimer.State == 0 ? 0 : ProcessNote.NowRoll[1];
                else if ((roll2p == ERoll.Balloon || roll2p == ERoll.Kusudama)) NowRoll[1] = ProcessNote.BalloonRemain[1];
                DrawNumber(Notes.NotesP[1].X + 160, Notes.NotesP[1].Y + 82, $"{NowRoll[1]}", 0);
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
            DrawString(200, 360, $"Combo:{MaxCombo[0]}", 0xffffff);
            DrawString(200, 380, $"Rank:{Rank[0]}", 0xffffff);

            if (JudgeCounter.State == TimerState.Started || JudgeCounterBig.State == TimerState.Started)
            {
                DrawString(600, 260, $"{DisplayJudge[0]}", 0xffffff);
                DrawString(600, 280, $"{msJudge[0]}", 0xffffff);
            }

            if (PlayData.Data.IsPlay2P)
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
                if (Game.IsSongPlay && !Game.MainSong.IsPlaying) DrawString(200, 600, Cleared[1] ? "Cleared" : "Failed", 0xffffff);
                DrawString(200, 620, $"Combo:{MaxCombo[1]}", 0xffffff);
                DrawString(200, 640, $"Rank:{Rank[1]}", 0xffffff);

                if (JudgeCounter2P.State == TimerState.Started || JudgeCounterBig2P.State == TimerState.Started)
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
            Remain[0] = Hit[0] * 2 - EXScore[0];
            Gauge[0] = GaugeList[0][(int)GaugeType[0]];
            Cleared[0] = Gauge[0] >= ClearRate[0][(int)GaugeType[0]] ? true : false;
            Rank[0] = GetRank(EXScore[0] + Auto[0] * 2, 0);
            msAverage[0] = Hit[0] > 0 ? msSum[0] / Hit[0] : 0;
            if (PlayData.Data.IsPlay2P)
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
            if (PlayData.Data.IsPlay2P && GaugeType[1] >= EGauge.Hard && Gauge[1] == 0 && (PlayData.Data.GaugeAutoShift[1] == (int)EGaugeAutoShift.None || PlayData.Data.GaugeAutoShift[1] == (int)EGaugeAutoShift.Retry))
            {
                Game.Failed[1] = true;
            }

            JudgeCounter.Tick();
            JudgeCounter2P.Tick();
            JudgeCounterBig.Tick();
            JudgeCounterBig2P.Tick();
            BombAnimation.Tick();
            BombAnimation.Start();
            RollCounter.Tick();
            RollCounter2P.Tick();
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
                        TextureLoad.Game_Bomb[0].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        TextureLoad.Game_Bomb[0].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        TextureLoad.Game_Bomb[4].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        TextureLoad.Game_Bomb[4].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        TextureLoad.Game_Bomb[5].Opacity = 1.0 - ((double)JudgeCounter.Value / JudgeCounter.End);
                        TextureLoad.Game_Bomb[5].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter.Value / JudgeCounter.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
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
                        TextureLoad.Game_Bomb[6].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        TextureLoad.Game_Bomb[6].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        TextureLoad.Game_Bomb[10].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        TextureLoad.Game_Bomb[10].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        TextureLoad.Game_Bomb[11].Opacity = 1.0 - ((double)JudgeCounterBig.Value / JudgeCounterBig.End);
                        TextureLoad.Game_Bomb[11].Draw(Notes.NotesP[0].X - 97.5, Notes.NotesP[0].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig.Value / JudgeCounter.End));
                        if (JudgeCounterBig.Value < JudgeCounter.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[0].X + 30.5, Notes.NotesP[0].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
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
                        TextureLoad.Game_Bomb[0].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        TextureLoad.Game_Bomb[0].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        TextureLoad.Game_Bomb[4].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        TextureLoad.Game_Bomb[4].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        TextureLoad.Game_Bomb[5].Opacity = 1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End);
                        TextureLoad.Game_Bomb[5].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounter2P.Value / JudgeCounter2P.End));
                        TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
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
                        TextureLoad.Game_Bomb[6].Opacity = 1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        TextureLoad.Game_Bomb[6].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 0, 134, 42));
                        break;
                    case EJudge.Great:
                        TextureLoad.Game_Bomb[10].Opacity = 1.0 - (JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        TextureLoad.Game_Bomb[10].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 1, 134, 42));
                        break;
                    case EJudge.Good:
                        TextureLoad.Game_Bomb[11].Opacity = 1.0 - (JudgeCounterBig2P.Value / JudgeCounterBig2P.End);
                        TextureLoad.Game_Bomb[11].Draw(Notes.NotesP[1].X - 97.5, Notes.NotesP[1].Y - 97.5);
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 2, 134, 42));
                        break;
                    case EJudge.Bad:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 3, 134, 42));
                        break;
                    case EJudge.Poor:
                        TextureLoad.Game_Judge.Opacity = 3.0 * (1.0 - ((double)JudgeCounterBig2P.Value / JudgeCounter2P.End));
                        if (JudgeCounterBig2P.Value < JudgeCounter2P.End) TextureLoad.Game_Judge.Draw(Notes.NotesP[1].X + 30.5, Notes.NotesP[1].Y - 18, new Rectangle(0, 42 * 4, 134, 42));
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
                DrawNumber(411 - 12 * Digit(Combo[player]), 372 + 262 * player, $"{Combo[player]}", color);
        }

        public static void DrawGraph()
        {
            TextureLoad.Game_Graph_Base.Draw(495, 0);

            double allnotes = Perfect[0] + Great[0] + Good[0] + Bad[0] + Poor[0] + Auto[0];
            double hitnotes = EXScore[0] > 0 ? EXScore[0] / 2.0 : Auto[0];
            double percent = allnotes > 0 ? hitnotes / allnotes : 0;
            double hitpercent = hitnotes / Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes;
            double allpercent = allnotes / Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes;
            int score = (int)(Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * percent * 2);
            ERank rank = GetRank(score, 0);

            TextureLoad.Game_Graph.Draw(499, 163, new Rectangle((int)(1000 - 1000 * hitpercent), 64 * 2, (int)(1000 * hitpercent), 64));

            if (PlayMemory.BestData.Score > 0)
            {
                double bestallnotes = PlayMemory.BestData.Score;
                double bestnotes = EXScore[2];
                double bestpercent = bestnotes / (Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                double bestallpercent = bestallnotes / (Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                TextureLoad.Game_Graph.Draw(499, 163 - 76, new Rectangle((int)(1000 - 1000 * bestallpercent), 64 * 3, (int)(1000 * bestallpercent), 64));
                TextureLoad.Game_Graph.Draw(499, 163 - 76, new Rectangle((int)(1000 - 1000 * bestpercent), 64, (int)(1000 * bestpercent), 64));
                int num = (int)hitnotes * 2 - (int)bestnotes;
                DrawMiniNumber(499 + 8, 183 - 76, $"{(num >= 0 ? "+" : "")}{num}", num < 0 ? 1 : 0);
            }

            switch ((ERival)PlayData.Data.RivalType)
            {
                case ERival.Percent:
                    double allvalue = PlayData.Data.RivalPercent / 100.0;
                    double value = allvalue * allpercent;
                    TextureLoad.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * allvalue), 64 * 3, (int)(1000 * allvalue), 64));
                    TextureLoad.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * value), 0, (int)(1000 * value), 64));
                    int num = (int)hitnotes * 2 - (int)(Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 2 * value);
                    DrawMiniNumber(499 + 8, 183 - 76 * 2, $"{(num >= 0 ? "+" : "")}{num}", num < 0 ? 1 : 0);
                    DrawNumber(499 - 178, 181 - 76 * 2, $"{PlayData.Data.RivalPercent,6:F2}%", 0);
                    break;
                case ERival.Rank:
                    double rpercent = GetRankToPercent((ERank)PlayData.Data.RivalRank, 0);
                    double rvalue = rpercent * allpercent;
                    TextureLoad.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rpercent), 64 * 3, (int)(1000 * rpercent), 64));
                    TextureLoad.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rvalue), 0, (int)(1000 * rvalue), 64));
                    int rnum = (int)hitnotes * 2 - (int)(Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 2 * rvalue);
                    DrawMiniNumber(499 + 8, 183 - 76 * 2, $"{(rnum >= 0 ? "+" : "")}{rnum}", rnum < 0 ? 1 : 0);
                    TextureLoad.Result_Rank.Draw(499 - 152, 175 - 76 * 2, new Rectangle(0, PlayData.Data.RivalRank > 7 ? 45 * 7 : 45 * PlayData.Data.RivalRank, 161, 45));
                    break;
                case ERival.PlayScore:
                    double rivalallnotes = PlayMemory.RivalData.Score;
                    double rivalnotes = EXScore[3];
                    double rivalpercent = rivalnotes / (Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                    double rivalallpercent = rivalallnotes / (Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes * 2);
                    TextureLoad.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rivalallpercent), 64 * 3, (int)(1000 * rivalallpercent), 64));
                    TextureLoad.Game_Graph.Draw(499, 163 - 76 * 2, new Rectangle((int)(1000 - 1000 * rivalpercent), 0, (int)(1000 * rivalpercent), 64));
                    int rivalnum = (int)hitnotes * 2 - (int)rivalnotes;
                    DrawMiniNumber(499 + 8, 183 - 76 * 2, $"{(rivalnum >= 0 ? "+" : "")}{rivalnum}", rivalnum < 0 ? 1 : 0);
                    break;
            }

            DrawNumber(1536, 196, $"{score, 4}", 0);
            TextureLoad.Game_Rank.Draw(1622, 152, new Rectangle(0, 90 * (int)rank, 161 * 2, 45 * 2));
        }

        public static void AddScore(EJudge judge, int player)
        {
            if (player < 2)
            {
                AddGauge(judge, player);
                DisplayJudge[player] = judge;
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
            switch (judge)
            {
                case EJudge.Perfect:
                    EXScore[player] += 2;
                    break;
                case EJudge.Great:
                    EXScore[player] += 1;
                    break;
            }
        }

        public static void AddRoll(int player)
        {
            if (Game.IsAuto[player])
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
            if (Game.IsAuto[player])
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
            if (Game.MainTimer.State == 0) return;

            double[] gaugepernote = new double[2] { Total[0] / Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes, Total[1] / Game.MainTJA[1].Courses[Game.Course[1]].TotalNotes };
            double[] gauge = new double[6];
            int Notes = PlayData.Data.Hazard[player] > Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes ? Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes : PlayData.Data.Hazard[player];
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
                        switch (Game.MainTJA[player].Courses[Game.Course[player]].LEVEL)
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
                        switch (Game.MainTJA[player].Courses[Game.Course[player]].LEVEL)
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
                        switch (Game.MainTJA[player].Courses[Game.Course[player]].LEVEL)
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
                        switch (Game.MainTJA[player].Courses[Game.Course[player]].LEVEL)
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

            Total[player] = Game.MainTJA[player].Courses[Game.Course[player]].TOTAL > 0.0 ? Game.MainTJA[player].Courses[Game.Course[player]].TOTAL : total[player];
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
            if (value >= Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes * 16 / 9)
            {
                return ERank.AAA;
            }
            else if (value >= Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes * 14 / 9)
            {
                return ERank.AA;
            }
            else if (value >= Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes * 12 / 9)
            {
                return ERank.A;
            }
            else if (value >= Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes * 10 / 9)
            {
                return ERank.B;
            }
            else if (value >= Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes * 8 / 9)
            {
                return ERank.C;
            }
            else if (value >= Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes * 6 / 9)
            {
                return ERank.D;
            }
            else if (value >= Game.MainTJA[player].Courses[Game.Course[player]].TotalNotes * 4 / 9)
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
                        TextureLoad.Game_Number.Draw(x, y, new Rectangle(stNumber[i].X, 28 * type, 26, 28));
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
                        TextureLoad.Game_Number_Mini.Draw(x, y, new Rectangle(stMiniNumber[i].X, 18 * type, 18, 18));
                        break;
                    }
                }
                x += 16;
            }
        }

        public static EGauge[] GaugeType = new EGauge[2];
        public static int[] EXScore = new int[4], Perfect = new int[2], Great = new int[2], Good = new int[2], Bad = new int[2], Poor = new int[2], Auto = new int[2],
            Hit = new int[2], Roll = new int[2], RollYellow = new int[2], RollBalloon = new int[2], AutoRoll = new int[2], Remain = new int[2], Combo = new int[2], MaxCombo = new int[2], NowRoll = new int[2];
        public static double[] msJudge = new double[2], Gauge = new double[2], Total = new double[2], GoodRate = new double[2], BadRate = new double[2], PoorRate = new double[2], msSum = new double[2], msAverage = new double[2];
        public static double[][] GaugeList = new double[2][], ClearRate = new double[2][];
        public static bool[] Cleared = new bool[2];
        private static EJudge[] DisplayJudge = new EJudge[2];
        public static ERank[] Rank = new ERank[2];
        public static Counter JudgeCounter, JudgeCounterBig, JudgeCounter2P, JudgeCounterBig2P, BombAnimation, RollCounter, RollCounter2P, Active;

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
