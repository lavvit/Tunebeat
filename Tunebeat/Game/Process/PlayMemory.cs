using System;
using System.Collections.Generic;
using System.IO;
using TJAParse;
using SeaDrop;

namespace Tunebeat
{
    class PlayMemory
    {
        public static void Init()
        {
            ReplayData = new ReplayData();
            ReplayData2P = new ReplayData();
            InputData = new List<InputData>();
            InputSetting = new List<InputSetting>();
            ChipData = new List<ChipData>();
            InputData2P = new List<InputData>();
            InputSetting2P = new List<InputSetting>();
            ChipData2P = new List<ChipData>();
            BestData = new ReplayData();
            RivalData = new ReplayData();
            Saved = false;

            if (Game.IsReplay[0] && !string.IsNullOrEmpty(SongSelect.ReplayScore[0]))
            {
                ReplayData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[0].TJAPath)}.{(ECourse)Game.Course[0]}.{SongSelect.ReplayScore[0]}.tbr");
            }
            if (Game.Play2P && Game.IsReplay[1] && !string.IsNullOrEmpty(SongSelect.ReplayScore[1]))
            {
                ReplayData2P = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(SongData.NowTJA[1].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[1].TJAPath)}.{(ECourse)Game.Course[1]}.{SongSelect.ReplayScore[1]}.tbr");
            }
            if (new BestScore(Game.TJAPath).ScoreData != null && !string.IsNullOrEmpty(new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].BestScore) && PlayData.Data.ShowGraph && PlayData.Data.ShowBestScore)
            {
                BestData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[0].TJAPath)}.{(ECourse)Game.Course[0]}.{PlayData.Data.PlayerName}.{new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].BestScore}.tbr");
            }
            if (new BestScore(Game.TJAPath).ScoreData != null)
            {
                BestData.Score = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].Score;
                BestData.Perfect = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].Perfect;
                BestData.Great = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].Great;
                BestData.Good = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].Good;
                BestData.Bad = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].Bad;
                BestData.Poor = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].Poor;
                BestData.Roll = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].Roll;
                BestData.MaxCombo = new BestScore(Game.TJAPath).ScoreData.Score[Game.Course[0]].MaxCombo;
            }
            if (!string.IsNullOrEmpty(SongSelect.RivalScore) && PlayData.Data.ShowGraph && PlayData.Data.RivalType == (int)ERival.PlayScore)
            {
                RivalData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(SongData.NowTJA[0].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[0].TJAPath)}.{(ECourse)Game.Course[0]}.{SongSelect.RivalScore}.tbr");
            }

            if (ReplayData != null && ReplayData.Chip != null)
            {
                foreach (ChipData chipdata in ReplayData.Chip)
                {
                    foreach (Chip chip in SongData.NowTJA[0].Courses[Game.Course[0]].ListChip)
                    {
                        if (chipdata.Chip.Time == chip.Time && chip.EChip == EChip.Note)
                        {
                            chip.ENote = chipdata.Chip.ENote;
                        }
                    }
                }
            }
            if (ReplayData2P != null && ReplayData2P.Chip != null)
            {
                foreach (ChipData chipdata in ReplayData2P.Chip)
                {
                    foreach (Chip chip in SongData.NowTJA[1].Courses[Game.Course[1]].ListChip)
                    {
                        if (chipdata.Chip.Time == chip.Time && chip.EChip == EChip.Note)
                        {
                            chip.ENote = chipdata.Chip.ENote;
                        }
                    }
                }
            }
        }

        public static void Dispose()
        {
            InputData = null;
            InputSetting = null;
            ReplayData = null;
            ChipData = null;
            InputData2P = null;
            InputSetting2P = null;
            ReplayData2P = null;
            ChipData2P = null;
            BestData = null;
            RivalData = null;
        }

        public static void AddData(int player, double time, bool isdon, bool isleft)
        {
            if ((player < 2 && Game.IsReplay[player]) || player >= 2) return;

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

        public static void AddChip(int player, Chip chip, double time, EJudge judge)
        {
            if (player >= 2) return;

            List<ChipData> data = player == 0 ? ChipData : ChipData2P;
            ChipData chipdata = new ChipData()
            {
                Chip = chip,
                Time = time,
                judge = judge
            };
            if (Game.MainTimer.State != 0)
                data.Add(chipdata);
        }

        public static void InitSetting(int player, double time, double scroll, int sudden, bool issudden, double adjust)
        {
            if ((player < 2 && Game.IsReplay[player]) || player >= 2) return;

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
            if ((player < 2 && Game.IsReplay[player]) || player >= 2) return;

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
            if ((player < 2 && (Game.IsReplay[player] || Score.Auto[player] > 0 || Score.EXScore[player] == 0)) || Saved || player >= 2) return;

            DateTime time = DateTime.Now;
            strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";

            Scores[] scores = new BestScore(Game.TJAPath).ScoreData != null ? new BestScore(Game.TJAPath).ScoreData.Score : new Scores[5]
                { new Scores(), new Scores(), new Scores(), new Scores(), new Scores() };
            ScoreData bestscore;
            bestscore = new ScoreData()
            {
                Title = SongData.NowTJA[player].Header.TITLE,
                Score = scores,
            };

            ReplayData replaydata;
            List<InputData> inputdata = player == 0 ? InputData : InputData2P;
            List<InputSetting> inputsetting = player == 0 ? InputSetting : InputSetting2P;
            List<ChipData> chipdata = player == 0 ? ChipData : ChipData2P;
            replaydata = new ReplayData()
            {
                Title = SongData.NowTJA[player].Header.TITLE,
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
                Setting = inputsetting,
                Chip = chipdata
            };

            ConfigJson.SaveConfig(bestscore, $"{Path.GetDirectoryName(SongData.NowTJA[player].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{PlayData.Data.PlayerName}.tbs");
            ConfigJson.SaveConfig(replaydata, $"{Path.GetDirectoryName(SongData.NowTJA[player].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr");
            TextLog.Draw($"スコアが保存されました! : {Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr", 2000);
            Saved = true;
        }

        public static void SaveScore(int player, int course)
        {
            if ((player < 2 && (Game.IsReplay[player] || Score.Auto[player] > 0 || Score.EXScore[player] == 0)) || Saved || player >= 2) return;

            DateTime time = DateTime.Now;
            strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";

            Scores[] scores = new BestScore(Game.TJAPath).ScoreData != null ? new BestScore(Game.TJAPath).ScoreData.Score : new Scores[5]
                { new Scores(), new Scores(), new Scores(), new Scores(), new Scores() };
            if (Score.EXScore[player] > scores[course].Score)
            {
                int clear = scores[course].ClearLamp;
                int count = scores[course].PlayCount;
                scores[course] = new Scores()
                {
                    Course = Game.Course[player],
                    PlayCount = count,
                    Time = $"{time:G}",
                    ClearLamp = (int)NowClear(player) > clear ? (int)NowClear(player) : clear,
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
                    BestScore = $"{Score.EXScore[player]}.{strTime}"
                };
            }
            scores[course].PlayCount++;
            if (string.IsNullOrEmpty(scores[course].BestScore) || !File.Exists($"{Path.GetDirectoryName(SongData.NowTJA[player].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{scores[course].BestScore}.tbr"))
            {
                scores[course].BestScore = $"{Score.EXScore[player]}.{strTime}";
            }

            ScoreData bestscore;
            bestscore = new ScoreData()
            {
                Title = SongData.NowTJA[player].Header.TITLE,
                Score = scores,
            };
            ReplayData replaydata;
            List<InputData> inputdata = player == 0 ? InputData : InputData2P;
            List<InputSetting> inputsetting = player == 0 ? InputSetting : InputSetting2P;
            List<ChipData> chipdata = player == 0 ? ChipData : ChipData2P;
            replaydata = new ReplayData()
            {
                Title = SongData.NowTJA[player].Header.TITLE,
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
                Setting = inputsetting,
                Chip = chipdata
            };

            ConfigJson.SaveConfig(bestscore, $"{Path.GetDirectoryName(SongData.NowTJA[player].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{PlayData.Data.PlayerName}.tbs");
            if (Score.EXScore[player] > scores[course].Score || !File.Exists($"{Path.GetDirectoryName(SongData.NowTJA[player].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{scores[course].BestScore}.tbr"))
            {
                ConfigJson.SaveConfig(replaydata, $"{Path.GetDirectoryName(SongData.NowTJA[player].TJAPath)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr");
                TextLog.Draw($"スコアが保存されました! : {Path.GetFileNameWithoutExtension(SongData.NowTJA[player].TJAPath)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr", 2000);
                Saved = true;
            }
        }

        public static EClear NowClear(int player)
        {
            if (Score.Bad[player] + Score.Poor[player] + Score.Good[player] + Score.Great[player] == 0) return EClear.AllPerfect;
            else if (Score.Bad[player] + Score.Poor[player] + Score.Good[player] == 0) return EClear.AllGreat;
            else if (Score.Bad[player] + Score.Poor[player] == 0) return EClear.FullCombo;
            else if (Score.GaugeType[player] >= EGauge.EXHard) return EClear.EXHardClear;
            else if (Score.GaugeType[player] == EGauge.Hard) return EClear.HardClear;
            else if (Score.GaugeType[player] == EGauge.Normal) return EClear.Clear;
            else if (Score.GaugeType[player] == EGauge.Easy) return EClear.EasyClear;
            else if (Score.GaugeType[player] == EGauge.Assist) return EClear.AssistClear;
            else return EClear.Failed;
        }

        public static ReplayData ReplayData { get; set; }
        public static ReplayData ReplayData2P { get; set; }
        public static List<InputData> InputData { get; set; }
        public static List<InputData> InputData2P { get; set; }
        public static List<InputSetting> InputSetting { get; set; }
        public static List<InputSetting> InputSetting2P { get; set; }
        public static List<ChipData> ChipData { get; set; }
        public static List<ChipData> ChipData2P { get; set; }
        public static ReplayData BestData { get; set; }
        public static ReplayData RivalData { get; set; }

        public static bool Saved;
        public static string strTime;
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
        public List<ChipData> Chip;
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

    class ChipData
    {
        public Chip Chip;
        public double Time;
        public EJudge judge;
        public bool Hit;
        
    }
}
