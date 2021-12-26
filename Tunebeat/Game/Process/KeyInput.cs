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
            if (Game.IsAuto)
            {
                judge = EJudge.Auto;
            }
            else
            {
                judge = GetNotes.GetJudge(chip, Game.MainTimer.Value);
            }
            if (ProcessNote.RollState(nowchip) == ProcessNote.ERoll.None)
            {
                ProcessNote.Process(judge, chip, isDon);
            }
            else
            {
                ProcessNote.RollProcess(nowchip, isDon);
            }

            if (chip.ENote == ENote.DON || chip.ENote == ENote.KA || ProcessNote.RollState(nowchip) == ProcessNote.ERoll.ROLL)
            {
                SoundLoad.Don.Volume = 1.5;
                SoundLoad.Ka.Volume = 1.5;
            }
            else
            {
                SoundLoad.Don.Volume = 1.0;
                SoundLoad.Ka.Volume = 1.0;
            }

            if (isDon) SoundLoad.Don.Play();
            else SoundLoad.Ka.Play();
        }

        public const int LEFTDON = KEY_INPUT_F;
        public const int RIGHTDON = KEY_INPUT_J;
        public const int LEFTKA = KEY_INPUT_D;
        public const int RIGHTKA = KEY_INPUT_K;
    }
}
