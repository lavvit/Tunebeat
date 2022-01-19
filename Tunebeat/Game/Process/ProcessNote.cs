using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAParse;
using static DxLibDLL.DX;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class ProcessNote
    {
        public static void Process(EJudge judge, Chip chip, bool isDon, int player)
        {
            if ((isDon && (chip.ENote == ENote.Don || chip.ENote == ENote.DON)) || (!isDon && (chip.ENote == ENote.Ka || chip.ENote == ENote.KA)))
            {
                if (judge != EJudge.Through)
                {
                    Score.AddScore(judge, player);
                    Score.DrawJudge(player, chip.ENote == ENote.DON || chip.ENote == ENote.KA ? true : false);
                    Score.msJudge[player] = (Game.MainTimer.Value - PlayData.Data.InputAdjust[player] - chip.Time);
                    chip.IsHit = true;
                    if (judge == EJudge.Bad || judge == EJudge.Poor)
                    {
                        chip.IsMiss = true;
                    }
                    else if (Game.MainTimer.State != 0 && judge != EJudge.Auto)
                    {
                        Score.Active.Reset();
                        Score.Active.Start();
                        Score.msSum[player] += (Game.MainTimer.Value - PlayData.Data.InputAdjust[player] - chip.Time);
                        Score.Hit[player]++;
                    }
                }
                else
                {
                    chip.IsMiss = true;
                }
            }
        }

        public static void PassNote(Chip chip, double time, bool isDon, int player)
        {
            double cuttime = Game.MainTimer.State == 0 ? -1 : -100;
            if (!chip.IsHit && time < cuttime && chip.EChip == EChip.Note && chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                if (!chip.IsMiss)
                    if (GetNotes.GetJudge(chip, time) == EJudge.Through)
                    {
                        EJudge judge = Game.IsAuto[player] ? EJudge.Auto : EJudge.Through;
                        if (!Game.IsAuto[player]) Score.AddScore(judge, player);
                        Process(judge, chip, isDon, player);
                        chip.IsHit = false;
                        chip.IsMiss = true;
                    }
        }

        public static ERoll RollState(Chip chip)
        {
            if (chip != null && chip.EChip == EChip.Note)
            {
                switch (chip.ENote)
                {
                    case ENote.RollStart:
                        return ERoll.Roll;
                    case ENote.ROLLStart:
                        return ERoll.ROLL;
                    case ENote.Balloon:
                        return ERoll.Balloon;
                    case ENote.Kusudama:
                        return ERoll.Kusudama;
                    default:
                        return ERoll.None;
                }
            }
            else
            {
                return ERoll.None;
            }
        }

        public static void RollProcess(Chip chip, bool isDon, int player)
        {
            switch (RollState(chip))
            {
                case ERoll.Roll:
                case ERoll.ROLL:
                    if (chip.RollCount == 0)
                    {
                        NowRoll[player] = 0;
                    }
                    chip.RollCount++;
                    NowRoll[player]++;
                    Score.AddRoll(player);
                    break;
                case ERoll.Balloon:
                case ERoll.Kusudama:
                    if (isDon && !chip.IsHit)
                    {
                        chip.RollCount++;
                        BalloonRemain[player]--;
                        Score.AddBalloon(player);
                        if (chip.RollCount == BalloonAmount(player))
                        {
                            if (RollState(chip) == ERoll.Balloon)
                            {
                                SoundLoad.Balloon.Play();
                            }
                            else
                            {
                                SoundLoad.Kusudama.Play();
                            }
                            chip.IsHit = true;
                            BalloonList[player]++;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public static int BalloonAmount(int player)
        {
            return Game.MainTJA[player].Courses[Game.Course[player]].BALLOON.Count > BalloonList[player] ? Game.MainTJA[player].Courses[Game.Course[player]].BALLOON[BalloonList[player]] : 5;
        }

        public static int[] NowRoll = new int[2], BalloonRemain = new int[2], BalloonList = new int[2];
    }

    public enum ERoll
    {
        None,
        Roll,
        ROLL,
        Balloon,
        Kusudama
    };
}
