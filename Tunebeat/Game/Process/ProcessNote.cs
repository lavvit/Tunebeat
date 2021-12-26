using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAParse;
using static DxLibDLL.DX;
using Amaoto;

namespace Tunebeat.Game
{
    public class ProcessNote
    {
        public static void Process(EJudge judge, Chip chip)
        {
            if(judge != EJudge.Through)
            {
                chip.IsHit = true;
                if(judge != EJudge.Miss)
                {

                }
                else
                {
                    chip.IsMiss = true;
                }
            }
            else
            {
                chip.IsMiss = true;
            }
        }

        public static void PassNote(Chip chip, double time)
        {
            if (!chip.IsHit && time < -100 && chip.EChip == EChip.Note && chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                if (!chip.IsMiss)
                    if (GetNotes.GetJudge(chip, time) == EJudge.Through)
                    {
                        Process(EJudge.Through, chip);
                        chip.IsMiss = true;
                    }
        }
    }
}
