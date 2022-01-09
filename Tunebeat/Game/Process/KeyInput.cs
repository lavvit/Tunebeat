using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunebeat.Common;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;

namespace Tunebeat.Game
{
    public class KeyInput
    {
        public static void Update(bool Auto1P, bool Auto2P, bool Failed1P, bool Failed2P)
        {
            if(Key.IsPushed(KEY_INPUT_F1))
                Game.IsAuto[0] = !Game.IsAuto[0];
            if (Key.IsPushed(KEY_INPUT_F2) && PlayData.Data.IsPlay2P)
                Game.IsAuto[1] = !Game.IsAuto[1];
            if (Key.IsPushed(KEY_INPUT_F3) && PlayData.Data.AutoRoll > 0)
                if (PlayData.Data.AutoRoll > 120) PlayData.Data.AutoRoll = 120;
                else PlayData.Data.AutoRoll--;
            if (Key.IsPushed(KEY_INPUT_F4))
                if (PlayData.Data.AutoRoll >= 120) PlayData.Data.AutoRoll = 1000;
                else PlayData.Data.AutoRoll++;

            if (!Auto1P && !Failed1P)
            {
                if (Key.IsPushed(PlayData.Data.LEFTDON))
                {
                    Process(true, true, 0);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTDON))
                {
                    Process(true, false, 0);
                }
                if (Key.IsPushed(PlayData.Data.LEFTKA))
                {
                    Process(false, true, 0);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTKA))
                {
                    Process(false, false, 0);
                }
            }

            if (!Auto2P && !Failed2P && PlayData.Data.IsPlay2P)
            {
                if (Key.IsPushed(PlayData.Data.LEFTDON2P))
                {
                    Process(true, true, 1);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTDON2P))
                {
                    Process(true, false, 1);
                }
                if (Key.IsPushed(PlayData.Data.LEFTKA2P))
                {
                    Process(false, true, 1);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTKA2P))
                {
                    Process(false, false, 1);
                }
            }
        }

        public static void Process(bool isDon, bool isLeft, int player)
        {
            Chip chip = GetNotes.GetNearNote(Game.MainTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            Chip nowchip = GetNotes.GetNowNote(Game.MainTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            EJudge judge;
            ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
            if (Game.IsAuto[player])
            {
                judge = EJudge.Auto;
            }
            else if (chip == null)
            {
                judge = EJudge.Through;
            }
            else
            {
                judge = GetNotes.GetJudge(chip, Game.MainTimer.Value);
            }
            if (chip != null && roll == ERoll.None)
            {
                ProcessNote.Process(judge, chip, isDon, player);
            }
            else
            {
                ProcessNote.RollProcess(nowchip, isDon, player);
            }

            Sound[] taiko = new Sound[2];
            Sound[] taiko2P = new Sound[2];
            if (player == 0)
            {
                if (chip != null && Math.Abs(Game.MainTimer.Value - chip.Time) <= 32 && ((chip.ENote == ENote.DON || chip.ENote == ENote.KA) && (judge <= EJudge.Great || judge == EJudge.Auto)) || roll == ERoll.ROLL)
                {
                    taiko[0] = SoundLoad.DON;
                    taiko[0].Volume = 1.5;
                    taiko[1] = SoundLoad.KA;
                    taiko[1].Volume = 1.5;
                }
                else
                {
                    taiko[0] = SoundLoad.Don;
                    taiko[0].Volume = 1.0;
                    taiko[1] = SoundLoad.Ka;
                    taiko[1].Volume = 1.0;
                }
                if (PlayData.Data.IsPlay2P)
                {
                    taiko[0].Pan = -255;
                    taiko[1].Pan = -255;
                }
                else
                {
                    taiko[0].Pan = 0;
                    taiko[1].Pan = 0;
                }

                if (isDon) taiko[0].Play();
                else taiko[1].Play();
            }
            else
            {
                if (chip != null && Math.Abs(Game.MainTimer.Value - chip.Time) <= 32 && ((chip.ENote == ENote.DON || chip.ENote == ENote.KA) && (judge <= EJudge.Great || judge == EJudge.Auto)) || roll == ERoll.ROLL)
                {
                    taiko2P[0] = SoundLoad.DON2P;
                    taiko2P[0].Volume = 1.5;
                    taiko2P[1] = SoundLoad.KA2P;
                    taiko2P[1].Volume = 1.5;
                }
                else
                {
                    taiko2P[0] = SoundLoad.Don2P;
                    taiko2P[0].Volume = 1.0;
                    taiko2P[1] = SoundLoad.Ka2P;
                    taiko2P[1].Volume = 1.0;
                }
                taiko2P[0].Pan = 255;
                taiko2P[1].Pan = 255;

                if (isDon) taiko2P[0].Play();
                else taiko2P[1].Play();
            }

            if (player == 0)
            {
                if (isDon)
                {
                    if (isLeft)
                    {
                        Game.HitTimer[0].Reset();
                        Game.HitTimer[0].Start();
                    }
                    else
                    {
                        Game.HitTimer[1].Reset();
                        Game.HitTimer[1].Start();
                    }
                }
                else
                {
                    if (isLeft)
                    {
                        Game.HitTimer[2].Reset();
                        Game.HitTimer[2].Start();
                    }
                    else
                    {
                        Game.HitTimer[3].Reset();
                        Game.HitTimer[3].Start();
                    }
                }
            }
            else
            {
                if (isDon)
                {
                    if (isLeft)
                    {
                        Game.HitTimer2P[0].Reset();
                        Game.HitTimer2P[0].Start();
                    }
                    else
                    {
                        Game.HitTimer2P[1].Reset();
                        Game.HitTimer2P[1].Start();
                    }
                }
                else
                {
                    if (isLeft)
                    {
                        Game.HitTimer2P[2].Reset();
                        Game.HitTimer2P[2].Start();
                    }
                    else
                    {
                        Game.HitTimer2P[3].Reset();
                        Game.HitTimer2P[3].Start();
                    }
                }
            }
        }
    }
}
