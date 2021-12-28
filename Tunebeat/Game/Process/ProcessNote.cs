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
                    Score.msJudge[player] = (Game.MainTimer.Value - chip.Time);
                    chip.IsHit = true;
                    if (judge == EJudge.Bad || judge == EJudge.Poor)
                    {
                        chip.IsMiss = true;
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
            if (!chip.IsHit && time < -100 && chip.EChip == EChip.Note && chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                if (!chip.IsMiss)
                    if (GetNotes.GetJudge(chip, time) == EJudge.Through)
                    {
                        Score.AddScore(EJudge.Through, player);
                        Process(EJudge.Through, chip, isDon, player);
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
                    int balloonamount = Game.MainTJA.Courses[Game.Course[player]].BALLOON.Count > BalloonList[player] ? Game.MainTJA.Courses[Game.Course[player]].BALLOON[BalloonList[player]] : 5;
                    if (chip.RollCount == 0)
                    {
                        BalloonRemain[player] = balloonamount;
                    }
                    if (isDon && !chip.IsHit)
                    {
                        chip.RollCount++;
                        BalloonRemain[player]--;
                        Score.AddBalloon(player);
                        if (chip.RollCount == balloonamount)
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
