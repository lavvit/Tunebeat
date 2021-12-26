using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAParse;

namespace Tunebeat.Game
{
    public class ProcessAuto
    {
        public static void Update(bool isAuto, Chip chip, double time)
        {
            if (!isAuto) return;

            if ((chip.ENote == ENote.Don || chip.ENote == ENote.DON) && time >= chip.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(true, true);
            }

            if ((chip.ENote == ENote.Ka || chip.ENote == ENote.KA) && time >= chip.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(false, true);
            }

            if (chip.RollEnd != null && time >= chip.Time && time < chip.RollEnd.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(true, true);
            }
        }
    }
}
