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
            ScoreData scoredata = new BestScore(Game.Path).ScoreData;
            Scores score = scoredata != null ? scoredata.Score[Game.Course[0]] : null;

            if (Game.IsReplay[0] && !string.IsNullOrEmpty(SongSelect.ReplayScore[0]))
            {
                ReplayData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.Path)}/{Path.GetFileNameWithoutExtension(Game.Path)}.{(ECourse)Game.Course[0]}.{SongSelect.ReplayScore[0]}.tbr");
            }
            if (scoredata != null && !string.IsNullOrEmpty(score.BestScore) && PlayData.Data.ShowGraph && PlayData.Data.ShowBestScore)
            {
                BestData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.Path)}/{Path.GetFileNameWithoutExtension(Game.Path)}.{(ECourse)Game.Course[0]}.{PlayData.Data.PlayerName}.{score.BestScore}.tbr");
            }
            if (scoredata != null)
            {
                BestData.Score = score.Score;
                BestData.Perfect = score.Perfect;
                BestData.Great = score.Great;
                BestData.Good = score.Good;
                BestData.Bad = score.Bad;
                BestData.Poor = score.Poor;
                BestData.Roll = score.Roll;
                BestData.MaxCombo = score.MaxCombo;
            }
            if (!string.IsNullOrEmpty(SongSelect.RivalScore) && PlayData.Data.ShowGraph && PlayData.Data.RivalType == (int)ERival.PlayScore)
            {
                RivalData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.Path)}/{Path.GetFileNameWithoutExtension(Game.Path)}.{(ECourse)Game.Course[0]}.{SongSelect.RivalScore}.tbr");
            }
        }
        public static void SetColor()
        {
            if (Game.Play2P && Game.IsReplay[1] && !string.IsNullOrEmpty(SongSelect.ReplayScore[1]))
            {
                ReplayData2P = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(Game.Path)}/{Path.GetFileNameWithoutExtension(Game.Path)}.{(ECourse)Game.Course[1]}.{SongSelect.ReplayScore[1]}.tbr");
            }

            if (ReplayData != null && ReplayData.Chip != null)
            {
                foreach (ChipData chipdata in ReplayData.Chip)
                {
                    foreach (Chip chip in SongData.NowTJA[0].Courses[Game.Course[0]].ListChip)
                    {
                        if (chipdata.Chip.Time == chip.Time)
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
                        if (chipdata.Chip.Time == chip.Time)
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
            if (player >= 2) return;
            if (Saved) return;
            if (Game.IsReplay[player]) return;

            DateTime time = DateTime.Now;
            strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";

            ReplayData replaydata;
            List<InputData> inputdata = player == 0 ? InputData : InputData2P;
            List<InputSetting> inputsetting = player == 0 ? InputSetting : InputSetting2P;
            List<ChipData> chipdata = player == 0 ? ChipData : ChipData2P;
            List<string> option = new List<string>();
            if (PlayData.Data.PlaySpeed != 1.0) option.Add($"{PlayData.Data.PlaySpeed}x Speed");
            if (PlayData.Data.Random[player]) option.Add($"{PlayData.Data.RandomRate}% Random");
            if (PlayData.Data.Mirror[player]) option.Add($"Mirror");
            if (PlayData.Data.Stelth[player]) option.Add($"Stelth");
            if (PlayData.Data.NotesChange[player] > 0) option.Add($"{(EChange)PlayData.Data.NotesChange[player]}");
            if (PlayData.Data.ScrollType[player] > 0) option.Add($"{(EScroll)PlayData.Data.ScrollType[player]}");
            if (PlayData.Data.JudgeType == 2) option.Add($"HardMord");
            else if (PlayData.Data.JudgeType == 0) option.Add($"Custom {(EScroll)PlayData.Data.JudgePerfect}/{(EScroll)PlayData.Data.JudgeGreat}/{(EScroll)PlayData.Data.JudgeGood}/{(EScroll)PlayData.Data.JudgeBad}/{(EScroll)PlayData.Data.JudgePoor}");
            string alloption = "";
            if (option.Count > 0)
            {
                alloption = option[0];
                for (int i = 1; i < option.Count; i++)
                {
                    alloption += $",{option[i]}";
                }
            }

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
                Chip = chipdata,
                Option = alloption,
                Speed = PlayData.Data.PlaySpeed
            };

            ConfigJson.SaveConfig(replaydata, $"{Path.GetDirectoryName(SongData.NowTJA[player].Path)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr");
            TextLog.Draw($"スコアが保存されました! : {Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr", 2000);
            Saved = true;
        }

        public static void SaveScore(int player, int course)
        {
            if (player >= 2) return;
            if (Saved) return;
            if (Game.IsReplay[player]) return;
            if (Score.Auto[player] + Score.AutoRoll[player] > 0) return;

            DateTime time = DateTime.Now;
            strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";

            ScoreData best = new BestScore(Game.Path).ScoreData;
            Scores[] scores = best != null ? best.Score : new Scores[5] { new Scores(){ Course = 0 }, new Scores(){ Course = 1 }, new Scores(){ Course = 2 }, new Scores(){ Course = 3 }, new Scores(){ Course = 4 } };
            bool save = Score.EXScore[player] > scores[course].Score;
            if (save)
            {
                scores[course] = new Scores()
                {
                    Course = Game.Course[player],
                    PlayCount = scores[course].PlayCount + 1,
                    Time = $"{time:G}",
                    ClearLamp = (int)NowClear(player) > scores[course].ClearLamp ? (int)NowClear(player) : scores[course].ClearLamp,
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
            else if ((int)NowClear(player) > scores[course].ClearLamp)
            {
                scores[course].ClearLamp = (int)NowClear(player);
                scores[course].PlayCount++;
            }

            if (string.IsNullOrEmpty(scores[course].BestScore) || !File.Exists($"{Path.GetDirectoryName(SongData.NowTJA[player].Path)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{scores[course].BestScore}.tbr"))
            {
                scores[course].BestScore = $"{Score.EXScore[player]}.{strTime}";
            }
            else
            {
                List<string> list = SongData.NowSong.ScoreList[Game.Course[player]];
                string b = "";
                int num = 0;
                if (list != null)
                {
                    foreach (string s in list)
                    {
                        string[] split = s.Split('.');
                        if (split[0] == PlayData.Data.PlayerName)
                        {
                            if (int.Parse(split[1]) > num)
                            {
                                num = int.Parse(split[1]);
                                b = $"{split[1]}.{split[2]}";
                            }
                        }
                    }
                    scores[course].BestScore = b;
                }
                else scores[course].BestScore = null;
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
            List<string> option = new List<string>();
            if (PlayData.Data.PlaySpeed != 1.0) option.Add($"{PlayData.Data.PlaySpeed}x Speed");
            if (PlayData.Data.Random[player]) option.Add($"{PlayData.Data.RandomRate}% Random");
            if (PlayData.Data.Mirror[player]) option.Add($"Mirror");
            if (PlayData.Data.Stelth[player]) option.Add($"Stelth");
            if (PlayData.Data.NotesChange[player] > 0) option.Add($"{(EChange)PlayData.Data.NotesChange[player]}");
            if (PlayData.Data.ScrollType[player] > 0) option.Add($"{(EScroll)PlayData.Data.ScrollType[player]}");
            if (PlayData.Data.JudgeType == 2) option.Add($"HardMord");
            else if (PlayData.Data.JudgeType == 0) option.Add($"Custom {(EScroll)PlayData.Data.JudgePerfect}/{(EScroll)PlayData.Data.JudgeGreat}/{(EScroll)PlayData.Data.JudgeGood}/{(EScroll)PlayData.Data.JudgeBad}/{(EScroll)PlayData.Data.JudgePoor}");
            string alloption = "";
            if (option.Count > 0)
            {
                alloption = option[0];
                for (int i = 1; i < option.Count; i++)
                {
                    alloption += $",{option[i]}";
                }
            }

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
                Chip = chipdata,
                Option = alloption,
                Speed = PlayData.Data.PlaySpeed
            };

            SongData.NowSong = SongLoad.ReLoad(SongData.NowSong);
            ConfigJson.SaveConfig(bestscore, $"{Path.GetDirectoryName(SongData.NowTJA[player].Path)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{PlayData.Data.PlayerName}.tbs");
            if (save)
            {
                ConfigJson.SaveConfig(replaydata, $"{Path.GetDirectoryName(SongData.NowTJA[player].Path)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr");
                TextLog.Draw($"スコアが保存されました! : {Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{(ECourse)Game.Course[player]}.{PlayData.Data.PlayerName}.{Score.EXScore[player]}.{strTime}.tbr", 2000);
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
}
