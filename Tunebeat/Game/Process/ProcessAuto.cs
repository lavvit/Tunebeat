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

            if ((chip.ENote == ENote.Don || chip.ENote == ENote.DON) && time + 8 >= chip.Time && !chip.IsHit && !chip.IsMiss)
            {
                KeyInput.Process(true, left[player], player);
                left[player] = !left[player];
            }

            if ((chip.ENote == ENote.Ka || chip.ENote == ENote.KA) && time + 8 >= chip.Time && !chip.IsHit && !chip.IsMiss)
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
                    if (RollTimer.Value == RollTimer.End)
                    {
                        KeyInput.Process(true, left[0], 0);
                        left[0] = !left[0];
                        RollTimer.Value = RollTimer.Begin;
                    }
                    if (player == 1 && RollTimer2P.Value == RollTimer2P.End)
                    {
                        KeyInput.Process(true, left[1], 1);
                        left[1] = !left[1];
                        RollTimer2P.Value = RollTimer2P.Begin;
                    }
                }
            }
        }
        public static Counter RollTimer, RollTimer2P;

        public static bool[] left = new bool[2];
    }
}
