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
        public static void Process(EJudge judge, Chip chip, bool isDon)
        {
            if ((isDon && (chip.ENote == ENote.Don || chip.ENote == ENote.DON)) || (!isDon && (chip.ENote == ENote.Ka || chip.ENote == ENote.KA)))
            {
                if (judge != EJudge.Through)
                {
                    Score.AddScore(judge);
                    Score.msJudge = (Game.MainTimer.Value - chip.Time);
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

        public static void PassNote(Chip chip, double time, bool isDon)
        {
            if (!chip.IsHit && time < -100 && chip.EChip == EChip.Note && chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                if (!chip.IsMiss)
                    if (GetNotes.GetJudge(chip, time) == EJudge.Through)
                    {
                        Score.AddScore(EJudge.Through);
                        Process(EJudge.Through, chip, isDon);
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

        public static void RollProcess(Chip chip, bool isDon)
        {
            switch (RollState(chip))
            {
                case ERoll.Roll:
                case ERoll.ROLL:
                    if (chip.RollCount == 0)
                    {
                        NowRoll = 0;
                    }
                    chip.RollCount++;
                    NowRoll++;
                    Score.AddRoll();
                    break;
                case ERoll.Balloon:
                case ERoll.Kusudama:
                    int balloonamount = Game.MainTJA.Courses[Game.Course].BALLOON.Count > BalloonList ? Game.MainTJA.Courses[Game.Course].BALLOON[BalloonList] : 5;
                    if (chip.RollCount == 0)
                    {
                        BalloonRemain = balloonamount;
                    }
                    if (isDon && !chip.IsHit)
                    {
                        chip.RollCount++;
                        BalloonRemain--;
                        Score.AddBalloon();
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
                            BalloonList++;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public static int NowRoll, BalloonRemain, BalloonList;
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
