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
        public static void Update(bool isAuto, Chip chip, double time, int player)
        {
            if (!isAuto) return;
            if (Game.MainTimer.State == 0) return;

            if ((chip.ENote == ENote.Don || chip.ENote == ENote.DON) && time + 8.5 >= chip.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(true, left[player], player);
                left[player] = !left[player];
            }

            if ((chip.ENote == ENote.Ka || chip.ENote == ENote.KA) && time + 8.5 >= chip.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(false, left[player], player);
                left[player] = !left[player];
            }

            if (chip.RollEnd != null && time >= chip.Time && time < chip.RollEnd.Time && !chip.IsHit && !chip.IsMiss)
            {
                if (PlayData.Data.AutoRoll > 0)
                {
                    if (chip.RollCount == 0)
                    {
                        KeyInput.Process(true, left[player], player);
                        left[player] = !left[player];

                    }
                    if (RollTimer[player].Value == RollTimer[player].End)
                    {
                        KeyInput.Process(true, left[player], player);
                        left[player] = !left[player];
                        RollTimer[player].Value = RollTimer[player].Begin;
                    }
                }
            }
        }
        public static Counter[] RollTimer = new Counter[5];

        public static bool[] left = new bool[5];
    }
}
