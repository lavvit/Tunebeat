using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TJAParse;
using Amaoto;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    class PlayMemory
    {
        public static void Init()
        {
            ReplayData = new ReplayData();
            ReplayData2P = new ReplayData();
            InputData = new List<InputData>();
            InputSetting = new List<InputSetting>();
            InputData2P = new List<InputData>();
            InputSetting2P = new List<InputSetting>();
            BestData = new ReplayData();
            RivalData = new ReplayData();

            if (!string.IsNullOrEmpty(PlayData.Data.Replay[0]))
            {
                ReplayData = ConfigManager.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.MainTJA[0].TJAPath)}/{Path.GetFileNameWithoutExtension(Game.MainTJA[0].TJAPath)}.{(ECourse)Game.Course[0]}.{PlayData.Data.Replay[0]}.replaydata");
            }
            if (Game.Play2P && !string.IsNullOrEmpty(PlayData.Data.Replay[1]))
            {
                ReplayData2P = ConfigManager.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.MainTJA[1].TJAPath)}/{Path.GetFileNameWithoutExtension(Game.MainTJA[1].TJAPath)}.{(ECourse)Game.Course[1]}.{PlayData.Data.Replay[1]}.replaydata");
            }
            if (!string.IsNullOrEmpty(PlayData.Data.BestScore) && PlayData.Data.ShowGraph && PlayData.Data.ShowBestScore)
            {
                BestData = ConfigManager.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.MainTJA[0].TJAPath)}/{Path.GetFileNameWithoutExtension(Game.MainTJA[0].TJAPath)}.{(ECourse)Game.Course[0]}.{PlayData.Data.BestScore}.replaydata");
            }
            if (!string.IsNullOrEmpty(PlayData.Data.RivalScore) && PlayData.Data.ShowGraph && PlayData.Data.RivalType == (int)ERival.PlayScore)
            {
                RivalData = ConfigManager.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.MainTJA[0].TJAPath)}/{Path.GetFileNameWithoutExtension(Game.MainTJA[0].TJAPath)}.{(ECourse)Game.Course[0]}.{PlayData.Data.RivalScore}.replaydata");
            }
        }

        public static void Dispose()
        {
            InputData = null;
            InputSetting = null;
            ReplayData = null;
            InputData2P = null;
            InputSetting2P = null;
            ReplayData2P = null;
            BestData = null;
            RivalData = null;
        }

        public static void AddData(int player, double time, bool isdon, bool isleft)
        {
            if (Game.IsReplay[player]) return;

            List<InputData> data = player == 0 ? InputData : InputData2P;
            InputData inputdata = new InputData()
            {
                Time = time,
                IsDon = isdon,
                IsLeft = isleft
            };
            if (Game.MainTimer.State != 0)
                data.Add(inputdata);
        }

        public static void InitSetting(int player, double time, double scroll, int sudden, bool issudden, double adjust)
        {
            if (Game.IsReplay[player]) return;

            List<InputSetting> setting = player == 0 ? InputSetting : InputSetting2P;
            InputSetting inputsetting = new InputSetting()
            {
                Time = time,
                Scroll = scroll,
                Sudden = issudden,
                SuddenNumber = sudden,
                Adjust = adjust
            };
            setting.Add(inputsetting);
        }

        public static void AddSetting(int player, double time, double scroll, int sudden, bool issudden, double adjust)
        {
            if (Game.IsReplay[player]) return;

            List<InputSetting> setting = player == 0 ? InputSetting : InputSetting2P;
            InputSetting inputsetting = new InputSetting()
            {
                Time = time,
                Scroll = scroll,
                Sudden = issudden,
                SuddenNumber = sudden,
                Adjust = adjust
            };
            if (Game.MainTimer.State != 0) setting.Add(inputsetting);
        }
        public static void SaveData(int player)
        {
            if (Game.IsReplay[player]) return;

            if (Score.EXScore[player] > 0 && !PlayData.Data.Auto[player])
            {
                DateTime time = DateTime.Now;
                string strtime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";

                ReplayData replaydata;
                List<InputData> inputdata = player == 0 ? InputData : InputData2P;
                List<InputSetting> inputsetting = player == 0 ? InputSetting : InputSetting2P;
                replaydata = new ReplayData()
                {
                    Title = Game.MainTJA[player].Header.TITLE,
                    Course = Game.Course[player],
                    Time = $"{time:G}",
                    Score = Score.EXScore[player],
                    MaxCombo = Score.MaxCombo[player],
                    GaugeType = (int)Score.GaugeType[player],
                    Gauge = Score.Gauge[player],
                    Perfect = Score.Perfect[player],
                    Great = Score.Great[player],
                    Good = Score.Good[player],
                    Bad = Score.Bad[player],
                    Poor = Score.Poor[player],
                    Roll = Score.Roll[player],
                    Data = inputdata,
                    Setting = inputsetting
                };

                ConfigManager.SaveConfig(replaydata, $"{Path.GetDirectoryName(Game.MainTJA[player].TJAPath)}/{Path.GetFileNameWithoutExtension(Game.MainTJA[player].TJAPath)}.{(ECourse)Game.Course[player]}.{Score.EXScore[player]}.{strtime}{(player == 1 ? ".2P" : "")}.replaydata");
                PlayData.Data.Replay[0] = $"{Score.EXScore[player]}.{strtime}";
            }
        }

        public static List<InputData> InputData { get; set; }
        public static List<InputSetting> InputSetting { get; set; }
        public static ReplayData ReplayData { get; set; }
        public static List<InputData> InputData2P { get; set; }
        public static List<InputSetting> InputSetting2P { get; set; }
        public static ReplayData ReplayData2P { get; set; }
        public static ReplayData BestData { get; set; }
        public static ReplayData RivalData { get; set; }
    }

    class ReplayData
    {
        public string Title;
        public int Course;
        public string Time;
        public int Score;
        public int MaxCombo;
        public int GaugeType;
        public double Gauge;
        public int Perfect;
        public int Great;
        public int Good;
        public int Bad;
        public int Poor;
        public int Roll;
        public List<InputData> Data;
        public List<InputSetting> Setting;
    }

    class InputData
    {
        public double Time;
        public bool IsDon;
        public bool IsLeft;
        public bool Hit;
    }

    class InputSetting
    {
        public double Time;
        public double Scroll;
        public bool Sudden;
        public int SuddenNumber;
        public double Adjust;
        public bool Hit;
    }
}
