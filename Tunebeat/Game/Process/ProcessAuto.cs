using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amaoto;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class ProcessAuto
    {
        public static void Update(bool isAuto, Chip chip, double time)
        {
            if (!isAuto) return;

            if ((chip.ENote == ENote.Don || chip.ENote == ENote.DON) && time + 8 >= chip.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(true, true);
            }

            if ((chip.ENote == ENote.Ka || chip.ENote == ENote.KA) && time + 8 >= chip.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(false, true);
            }

            if (chip.RollEnd != null && time >= chip.Time && time < chip.RollEnd.Time && !chip.IsHit && !chip.IsMiss)
            {
                if (PlayData.AutoRoll > 0)
                {
                    if (chip.RollCount == 0)
                    {
                        KeyInput.Process(true, true);
                    }
                    if (RollTimer.Value == RollTimer.End)
                    {
                        KeyInput.Process(true, true);
                        RollTimer.Value = RollTimer.Begin;
                    }
                }
            }
        }
        public static Counter RollTimer;
    }
}
