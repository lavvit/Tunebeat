using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amaoto;
using TJAParse;
using Tunebeat.Common;
using Tunebeat.Game;

namespace Tunebeat.Game
{
    public class ProcessReplay
    {
        public static void Update(bool isReplay, int player)
        {
            if (!isReplay) return;
            if (Game.MainTimer.State == 0) return;

            List<InputData> data = player == 0 ? PlayMemory.ReplayData.Data : PlayMemory.ReplayData2P.Data;
            if (data == null) return;

            foreach (InputData input in data)
            {
                if (Game.MainTimer.Value + 6 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit && Math.Abs(Game.MainTimer.Value - input.Time) < 60)
                {
                    KeyInput.Process(input.IsDon, input.IsLeft, player);
                    input.Hit = true;
                }
            }

            List<InputSetting> setting = player == 0 ? PlayMemory.ReplayData.Setting : PlayMemory.ReplayData2P.Setting;
            if (setting == null) return;

            foreach (InputSetting input in setting)
            {
                if (Game.MainTimer.Value + 6 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit)
                {
                    Notes.Scroll[player] = input.Scroll;
                    Notes.UseSudden[player] = input.Sudden;
                    Notes.Sudden[player] = input.SuddenNumber;
                    Game.Adjust[player] = input.Adjust;
                    input.Hit = true;
                }
            }
        }
    }
}
