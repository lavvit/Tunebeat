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

        public static void UnderUpdate()
        {
            if (Game.MainTimer.State == 0) return;
            if (PlayData.Data.IsPlay2P) return;

            List<InputData> data = PlayMemory.BestData.Data;
            if (data != null)
            {
                foreach (InputData input in data)
                {
                    if (Game.MainTimer.Value + 6 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit && Math.Abs(Game.MainTimer.Value - input.Time) < 60)
                    {
                        Chip chip = GetNotes.GetNearNote(Game.MainTJA[5].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value - Game.Adjust[2]);
                        Chip nowchip = GetNotes.GetNowNote(Game.MainTJA[5].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value - Game.Adjust[2]);
                        EJudge judge;
                        ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
                        if (chip == null)
                        {
                            judge = EJudge.Through;
                        }
                        else
                        {
                            judge = GetNotes.GetJudge(chip, Game.MainTimer.Value - Game.Adjust[2]);
                        }
                        if (Game.MainTimer.State != 0)
                        {
                            if (chip != null && roll == ERoll.None)
                            {

                                ProcessNote.Process(judge, chip, input.IsDon, 2);
                            }
                        }
                        input.Hit = true;
                    }
                }

                List<InputSetting> setting = PlayMemory.BestData.Setting;
                if (setting != null)
                {
                    foreach (InputSetting input in setting)
                    {
                        if (Game.MainTimer.Value + 6 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit)
                        {
                            Game.Adjust[2] = input.Adjust;
                            input.Hit = true;
                        }
                    }
                }
            }

            List<InputData> rdata = PlayMemory.RivalData.Data;
            if (rdata != null)
            {
                foreach (InputData input in rdata)
                {
                    if (Game.MainTimer.Value + 6 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit && Math.Abs(Game.MainTimer.Value - input.Time) < 60)
                    {
                        Chip chip = GetNotes.GetNearNote(Game.MainTJA[6].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value - Game.Adjust[3]);
                        Chip nowchip = GetNotes.GetNowNote(Game.MainTJA[6].Courses[Game.Course[0]].ListChip, Game.MainTimer.Value - Game.Adjust[3]);
                        EJudge judge;
                        ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
                        if (chip == null)
                        {
                            judge = EJudge.Through;
                        }
                        else
                        {
                            judge = GetNotes.GetJudge(chip, Game.MainTimer.Value - Game.Adjust[3]);
                        }
                        if (Game.MainTimer.State != 0)
                        {
                            if (chip != null && roll == ERoll.None)
                            {

                                ProcessNote.Process(judge, chip, input.IsDon, 3);
                            }
                        }
                        input.Hit = true;
                    }
                }

                List<InputSetting> setting = PlayMemory.RivalData.Setting;
                if (setting != null)
                {
                    foreach (InputSetting input in setting)
                    {
                        if (Game.MainTimer.Value + 6 >= input.Time / PlayData.Data.PlaySpeed && !input.Hit)
                        {
                            Game.Adjust[3] = input.Adjust;
                            input.Hit = true;
                        }
                    }
                }
            }
        }
    }
}
