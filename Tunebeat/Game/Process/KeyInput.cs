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
        public static void Update(bool Auto1P, bool Auto2P)
        {
            if(Key.IsPushed(KEY_INPUT_F1))
                Game.IsAuto[0] = !Game.IsAuto[0];
            if (Key.IsPushed(KEY_INPUT_F2) && PlayData.IsPlay2P)
                Game.IsAuto[1] = !Game.IsAuto[1];
            if (Key.IsPushed(KEY_INPUT_F3) && PlayData.AutoRoll > 0)
                if (PlayData.AutoRoll > 120) PlayData.AutoRoll = 120;
                else PlayData.AutoRoll--;
            if (Key.IsPushed(KEY_INPUT_F4))
                if (PlayData.AutoRoll >= 120) PlayData.AutoRoll = 1000;
                else PlayData.AutoRoll++;

            if (!Auto1P)
            {
                if (Key.IsPushed(PlayData.LEFTDON))
                    Process(true, true, 0);
                if (Key.IsPushed(PlayData.RIGHTDON))
                    Process(true, false, 0);
                if (Key.IsPushed(PlayData.LEFTKA))
                    Process(false, true, 0);
                if (Key.IsPushed(PlayData.RIGHTKA))
                    Process(false, false, 0);
            }

            if (!Auto2P && PlayData.IsPlay2P)
            {
                if (Key.IsPushed(PlayData.LEFTDON2P))
                    Process(true, true, 1);
                if (Key.IsPushed(PlayData.RIGHTDON2P))
                    Process(true, false, 1);
                if (Key.IsPushed(PlayData.LEFTKA2P))
                    Process(false, true, 1);
                if (Key.IsPushed(PlayData.RIGHTKA2P))
                    Process(false, false, 1);
            }
        }

        public static void Process(bool isDon, bool isLeft, int player)
        {
            Chip chip = GetNotes.GetNearNote(Game.MainTJA.Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            Chip nowchip = GetNotes.GetNowNote(Game.MainTJA.Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
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
                if (chip != null && ((chip.ENote == ENote.DON || chip.ENote == ENote.KA) && (judge <= EJudge.Great || judge == EJudge.Auto)) || roll == ERoll.ROLL)
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
                if (PlayData.IsPlay2P)
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
                if (chip != null && ((chip.ENote == ENote.DON || chip.ENote == ENote.KA) && (judge <= EJudge.Great || judge == EJudge.Auto)) || roll == ERoll.ROLL)
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
        }
    }
}
