using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;
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
                if (Game.MainTimer.Value + 8.0 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit && Math.Abs(Game.MainTimer.Value - input.Time) < GetNotes.range[4])
                {
                    KeyInput.Process(input.IsDon, input.IsLeft, player);
                    input.Hit = true;
                }
            }

            List<InputSetting> setting = player == 0 ? PlayMemory.ReplayData.Setting : PlayMemory.ReplayData2P.Setting;
            if (setting == null) return;

            foreach (InputSetting input in setting)
            {
                if (Game.MainTimer.Value + 8.0 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit)
                {
                    Notes.Scroll[player] = input.Scroll;
                    Notes.UseSudden[player] = input.Sudden;
                    Notes.Sudden[player] = input.SuddenNumber;
                    Game.Adjust[player] = input.Adjust;
                    input.Hit = true;
                }
            }
        }

        public static void UnderUpdate()
        {
            if (Game.Play2P) return;

            List<ChipData> data = PlayMemory.BestData.Chip;
            if (data != null)
            {
                foreach (ChipData chip in data)
                {
                    if (Game.MainTimer.Value + 8.0 >= chip.Time / PlayData.Data.PlaySpeed && !chip.Hit)
                    {
                        Score.AddScore(chip.judge, 2);
                        chip.Hit = true;
                    }
                }
            }

            List<ChipData> rdata = PlayMemory.RivalData.Chip;
            if (rdata != null)
            {
                foreach (ChipData chip in rdata)
                {
                    if (Game.MainTimer.Value + 8.0 >= chip.Time / PlayData.Data.PlaySpeed && !chip.Hit)
                    {
                        Score.AddScore(chip.judge, 3);
                        chip.Hit = true;
                    }
                }
            }
        }

        public static void Back()
        {
            if (Game.Play2P) return;

            if (PlayMemory.BestData.Chip != null)
            {
                foreach (ChipData data in PlayMemory.BestData.Chip)
                {
                    if (data.Chip.Time >= Game.StartTime - 1 && data.Hit)
                    {
                        switch (data.judge)
                        {
                            case EJudge.Perfect:
                                Score.EXScore[2] -= 2;
                                break;
                            case EJudge.Great:
                                Score.EXScore[2] -= 1;
                                break;
                        }
                        data.Hit = false;
                    }
                }
            }

            if (PlayMemory.RivalData.Chip != null)
            {
                foreach (ChipData data in PlayMemory.RivalData.Chip)
                {
                    if (data.Chip.Time >= Game.StartTime - 1 && data.Hit)
                    {
                        switch (data.judge)
                        {
                            case EJudge.Perfect:
                                Score.EXScore[3] -= 2;
                                break;
                            case EJudge.Great:
                                Score.EXScore[3] -= 1;
                                break;
                        }
                        data.Hit = false;
                    }
                }
            }
        }
    }
}
