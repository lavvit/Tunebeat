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
        public static void Update(bool isAuto)
        {
            if(Key.IsPushed(KEY_INPUT_F1))
                Game.IsAuto = !Game.IsAuto;
            if (Key.IsPushed(KEY_INPUT_F3) && PlayData.AutoRoll > 0)
                if (PlayData.AutoRoll > 120) PlayData.AutoRoll = 120;
                else PlayData.AutoRoll--;
            if (Key.IsPushed(KEY_INPUT_F4))
                if (PlayData.AutoRoll >= 120) PlayData.AutoRoll = 1000;
                else PlayData.AutoRoll++;

            if (isAuto) return;

            if (Key.IsPushed(LEFTDON))
                Process(true, true);
            if (Key.IsPushed(RIGHTDON))
                Process(true, false);
            if (Key.IsPushed(LEFTKA))
                Process(false, true);
            if (Key.IsPushed(RIGHTKA))
                Process(false, false);
        }

        public static void Process(bool isDon, bool isLeft)
        {
            Chip chip = GetNotes.GetNearNote(Game.MainTJA.Courses[Game.Course].ListChip, Game.MainTimer.Value);
            Chip nowchip = GetNotes.GetNowNote(Game.MainTJA.Courses[Game.Course].ListChip, Game.MainTimer.Value);
            EJudge judge;
            ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
            if (Game.IsAuto)
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
                ProcessNote.Process(judge, chip, isDon);
            }
            else
            {
                ProcessNote.RollProcess(nowchip, isDon);
            }

            Sound[] taiko = new Sound[2];
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

            if (isDon) taiko[0].Play();
            else taiko[1].Play();
        }

        public const int LEFTDON = KEY_INPUT_F;
        public const int RIGHTDON = KEY_INPUT_J;
        public const int LEFTKA = KEY_INPUT_D;
        public const int RIGHTKA = KEY_INPUT_K;
    }
}
