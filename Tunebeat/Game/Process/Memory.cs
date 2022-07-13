using System;
using System.Collections.Generic;
using System.IO;
using TJAParse;
using SeaDrop;

namespace Tunebeat
{
    class Memory
    {
        public static void Init(string path)
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
            SaveBest = false;
            SaveReplay = false;
            ScoreData scoredata = new BestScore(path).ScoreData;
            BestScores = scoredata != null ? scoredata.Score : new Scores[5] { new Scores() { Course = 0 }, new Scores() { Course = 1 }, new Scores() { Course = 2 }, new Scores() { Course = 3 }, new Scores() { Course = 4 } };
            Scores score = BestScores[NewGame.Course[0]];

            if (NewGame.Playmode[0] == EAuto.Replay && !string.IsNullOrEmpty(SongSelect.ReplayScore[0]))
            {
                ReplayData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}.{(ECourse)NewGame.Course[0]}.{SongSelect.ReplayScore[0]}.tbr");
            }
            if (NewGame.Playmode[1] == EAuto.Replay && !string.IsNullOrEmpty(SongSelect.ReplayScore[1]))
            {
                ReplayData2P = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}.{(ECourse)NewGame.Course[1]}.{SongSelect.ReplayScore[1]}.tbr");
            }
            if (scoredata != null && !string.IsNullOrEmpty(score.BestScore) && PlayData.Data.ShowGraph && PlayData.Data.ShowBestScore)
            {
                BestData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}.{(ECourse)NewGame.Course[0]}.{PlayData.Data.PlayerName}.{score.BestScore}.tbr");
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
                RivalData = ConfigJson.GetConfig<ReplayData>($"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}.{(ECourse)NewGame.Course[0]}.{SongSelect.RivalScore}.tbr");
            }
        }

        public static void SetColor()
        {
            if (ReplayData != null && ReplayData.Chip != null)
            {
                foreach (ChipData chipdata in ReplayData.Chip)
                {
                    foreach (Chip chip in NewGame.Chips[0])
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
                    foreach (Chip chip in NewGame.Chips[1])
                    {
                        if ((int)chipdata.Chip.Time == (int)chip.Time)
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

        public static void Reset()
        {
            SaveBest = false;
            SaveReplay = false;
            InputData = new List<InputData>();
            InputSetting = new List<InputSetting>();
            ChipData = new List<ChipData>();
            InputData2P = new List<InputData>();
            InputSetting2P = new List<InputSetting>();
            ChipData2P = new List<ChipData>();
            if (ReplayData != null)
            {
                if (ReplayData.Data != null)
                {
                    foreach (InputData data in ReplayData.Data)
                    {
                        data.Hit = false;
                    }
                }
                if (ReplayData.Setting != null)
                {
                    foreach (InputSetting data in ReplayData.Setting)
                    {
                        data.Hit = false;
                    }
                }
            }
            if (BestData != null && BestData.Chip != null)
            {
                foreach (ChipData chip in BestData.Chip)
                {
                    chip.Hit = false;
                }
            }
            if (RivalData != null && RivalData.Chip != null)
            {
                foreach (ChipData chip in RivalData.Chip)
                {
                    chip.Hit = false;
                }
            }
        }

        public static void Update()
        {
            for (int i = 0; i < 2; i++)
            {
                if (NewGame.Playmode[i] == EAuto.Replay)
                {
                    ReplayData data = i == 0 ? ReplayData : ReplayData2P;
                    if (data != null && (NewGame.NowState == EState.Start || NewGame.NowState == EState.Play))
                    {
                        if (data.Data != null)
                        {
                            foreach (InputData input in data.Data)
                            {
                                if (input.Time - NewGame.Timer.Value < 100)
                                {
                                    if (NewGame.Timer.Value >= input.Time / PlayData.Data.PlaySpeed && !input.Hit && Math.Abs(NewGame.Timer.Value - input.Time) < Process.GetRange(4))
                                    {
                                        if (input.IsDon)
                                        {
                                            Chip chip = Process.NearChip(NewGame.Chips[i], true);
                                            Process.PlaySound(true, chip, i);
                                            if (chip != null && (chip.ENote == ENote.Don || chip.ENote == ENote.DON))
                                            {
                                                chip.IsHit = true;
                                                NewScore.AddScore(Process.GetJudge(chip), i);
                                                NewScore.DrawJudge(chip.ENote, i);
                                                NewScore.msJudge[i] = NewGame.Timer.Value - NewGame.Adjust[i] - chip.Time;
                                                if (NewGame.Timer.State != 0 && Process.GetJudge(chip) < EJudge.Poor)
                                                {
                                                    NewScore.Active.Reset();
                                                    NewScore.Active.Start();
                                                    NewScore.msSum[i] += NewScore.msJudge[i];
                                                    NewScore.Hit[i]++;
                                                }
                                            }
                                            Chip nchip = Process.NowChip(NewGame.Chips[i], false);
                                            Process.RollProcess(nchip, i);
                                            Process.BalloonProcess(nchip, i);
                                            if (input.IsLeft)
                                            {
                                                Process.DonTimer[i][0].Reset();
                                                Process.DonTimer[i][0].Start();
                                            }
                                            else
                                            {
                                                Process.DonTimer[i][1].Reset();
                                                Process.DonTimer[i][1].Start();
                                            }
                                        }
                                        else
                                        {
                                            Chip chip = Process.NearChip(NewGame.Chips[i], false);
                                            Process.PlaySound(false, chip, i);
                                            if (chip != null && (chip.ENote == ENote.Ka || chip.ENote == ENote.KA))
                                            {
                                                chip.IsHit = true;
                                                NewScore.AddScore(Process.GetJudge(chip), i);
                                                NewScore.DrawJudge(chip.ENote, i);
                                                NewScore.msJudge[i] = NewGame.Timer.Value - NewGame.Adjust[i] - chip.Time;
                                                if (NewGame.Timer.State != 0 && Process.GetJudge(chip) < EJudge.Poor)
                                                {
                                                    NewScore.Active.Reset();
                                                    NewScore.Active.Start();
                                                    NewScore.msSum[i] += NewScore.msJudge[i];
                                                    NewScore.Hit[i]++;
                                                }
                                            }
                                            Chip nchip = Process.NowChip(NewGame.Chips[i], false);
                                            Process.RollProcess(nchip, i);
                                            if (input.IsLeft)
                                            {
                                                Process.DonTimer[i][2].Reset();
                                                Process.DonTimer[i][2].Start();
                                            }
                                            else
                                            {
                                                Process.DonTimer[i][3].Reset();
                                                Process.DonTimer[i][3].Start();
                                            }
                                        }
                                        input.Hit = true;
                                    }
                                }
                            }
                        }
                        if (data.Setting != null)
                        {
                            foreach (InputSetting input in data.Setting)
                            {
                                if (input.Time - NewGame.Timer.Value < 100)
                                {
                                    if (NewGame.Timer.Value >= input.Time / PlayData.Data.PlaySpeed && !input.Hit)
                                    {
                                        NewNotes.Scroll[i] = input.Scroll;
                                        Sudden.UseSudden[i] = input.Sudden;
                                        Sudden.SuddenNumber[i] = input.SuddenNumber;
                                        NewGame.Adjust[i] = input.Adjust;
                                        input.Hit = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (BestData != null)
            {
                List<ChipData> data = BestData.Chip;
                if (data != null && NewGame.NowState == EState.Play)
                {
                    foreach (ChipData chip in data)
                    {
                        if (chip.Time - NewGame.Timer.Value < 100)
                        {
                            if (NewGame.Timer.Value >= chip.Time / PlayData.Data.PlaySpeed && !chip.Hit)
                            {
                                NewScore.AddScore(chip.judge, 2);
                                chip.Hit = true;
                            }
                        }
                    }
                }
            }
            if (RivalData != null)
            {
                List<ChipData> rdata = RivalData.Chip;
                if (rdata != null && NewGame.NowState == EState.Play)
                {
                    foreach (ChipData chip in rdata)
                    {
                        if (chip.Time - NewGame.Timer.Value < 100)
                        {
                            if (NewGame.Timer.Value >= chip.Time / PlayData.Data.PlaySpeed && !chip.Hit)
                            {
                                NewScore.AddScore(chip.judge, 3);
                                chip.Hit = true;
                            }
                        }
                    }
                }
            }
        }

        public static void AddData(int player, bool isdon, bool isleft)
        {
            if ((player < 2 && NewGame.Playmode[player] == EAuto.Replay) || player >= 2) return;

            List<InputData> data = player == 0 ? InputData : InputData2P;
            InputData inputdata = new InputData()
            {
                Time = NewGame.Timer.Value,
                IsDon = isdon,
                IsLeft = isleft
            };
            if (NewGame.Timer.State != 0)
                data.Add(inputdata);
        }

        public static void AddChip(int player, Chip chip, EJudge judge)
        {
            if (player >= 2) return;

            List<ChipData> data = player == 0 ? ChipData : ChipData2P;
            ChipData chipdata = new ChipData()
            {
                Time = NewGame.Timer.Value,
                Chip = chip,
                judge = judge
            };
            if (NewGame.Timer.State != 0)
                data.Add(chipdata);
        }

        public static void InitSetting(int player, double scroll, int sudden, bool issudden, double adjust)
        {
            if ((player < 2 && NewGame.Playmode[player] == EAuto.Replay) || player >= 2) return;

            List<InputSetting> setting = player == 0 ? InputSetting : InputSetting2P;
            InputSetting inputsetting = new InputSetting()
            {
                Time = NewGame.Timer.Begin,
                Scroll = scroll,
                Sudden = issudden,
                SuddenNumber = sudden,
                Adjust = adjust
            };
            setting.Add(inputsetting);
        }

        public static void AddSetting(int player, double scroll, int sudden, bool issudden, double adjust)
        {
            if ((player < 2 && NewGame.Playmode[player] == EAuto.Replay) || player >= 2) return;

            List<InputSetting> setting = player == 0 ? InputSetting : InputSetting2P;
            InputSetting inputsetting = new InputSetting()
            {
                Time = NewGame.Timer.Value,
                Scroll = scroll,
                Sudden = issudden,
                SuddenNumber = sudden,
                Adjust = adjust
            };
            if (NewGame.Timer.State != 0) setting.Add(inputsetting);
        }
        public static void SaveScore(int player)
        {
            if (player > 1) return;
            if (SaveBest) return;
            if (NewGame.Playmode[player] == EAuto.Replay) return;
            if (NewGame.StartMeasure > 0) return;
            if (NewScore.Scores[player].Auto + NewScore.Scores[player].AutoRoll > 0) return;
            if (NewScore.Scores[player].EXScore == 0) return;
            if (NewGame.Dan != null) return;

            DateTime time = DateTime.Now;
            strTime = $"{time.Year:0000}{time.Month:00}{time.Day:00}{time.Hour:00}{time.Minute:00}{time.Second:00}";

            ScoreBoard board = NewGame.Dan != null ? NewScore.DanScore[DanCourse.SongNumber] : NewScore.Scores[player];
            int course = NewGame.Course[player];
            bool save = NewScore.Scores[player].EXScore > BestScores[course].Score;
            if (save)
            {
                BestScores[course] = new Scores()
                {
                    Course = NewGame.Course[player],
                    PlayCount = BestScores[course].PlayCount + 1,
                    Time = $"{time:G}",
                    ClearLamp = (int)NewScore.GetClear(player) > BestScores[course].ClearLamp ? (int)NewScore.GetClear(player) : BestScores[course].ClearLamp,
                    Score = board.EXScore,
                    MaxCombo = board.MaxCombo,
                    GaugeType = (int)NewScore.GaugeType[player],
                    Gauge = NewScore.Gauge[player],
                    Perfect = board.Perfect,
                    Great = board.Great,
                    Good = board.Good,
                    Bad = board.Bad,
                    Poor = board.Poor,
                    Roll = board.Roll,
                    BestScore = $"{board.EXScore}.{strTime}"
                };
                SaveData(player);
            }
            else if ((int)NewScore.GetClear(player) > BestScores[course].ClearLamp)
            {
                BestScores[course].ClearLamp = (int)NewScore.GetClear(player);
                BestScores[course].PlayCount++;
            }

            /*if (string.IsNullOrEmpty(BestScores[course].BestScore) || !File.Exists($"{Path.GetDirectoryName(SongData.NowTJA[player].Path)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{(ECourse)NewGame.Course[player]}.{PlayData.Data.PlayerName}.{BestScores[course].BestScore}.tbr"))
            {
                BestScores[course].BestScore = $"{NewScore.EXScore[player]}.{strTime}";
            }
            else
            {
                List<string> list = SongData.NowSong.ScoreList[NewGame.Course[player]];
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
                    BestScores[course].BestScore = b;
                }
                else BestScores[course].BestScore = null;
            }*/

            ScoreData bestscore;
            bestscore = new ScoreData()
            {
                Title = SongData.NowTJA[player].Header.TITLE,
                Score = BestScores,
            };

            ConfigJson.SaveConfig(bestscore, $"{Path.GetDirectoryName(SongData.NowTJA[player].Path)}/{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{PlayData.Data.PlayerName}.tbs");
            SongData.NowSong = SongLoad.ReLoad(SongData.NowSong);
            SaveBest = true;
        }
        public static void SaveData(int player)
        {
            if (player > 1) return;
            if (SaveReplay) return;
            if (NewGame.Playmode[player] == EAuto.Replay) return;
            if (NewGame.StartMeasure > 0) return;
            if (NewScore.Scores[player].Auto + NewScore.Scores[player].AutoRoll > 0) return;
            if (NewScore.Scores[player].EXScore == 0) return;

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
                Course = NewGame.Course[player],
                Time = $"{time:G}",
                Score = NewScore.Scores[player].EXScore,
                MaxCombo = NewScore.Scores[player].MaxCombo,
                GaugeType = (int)NewScore.GaugeType[player],
                Gauge = NewScore.Gauge[player],
                Perfect = NewScore.Scores[player].Perfect,
                Great = NewScore.Scores[player].Great,
                Good = NewScore.Scores[player].Good,
                Bad = NewScore.Scores[player].Bad,
                Poor = NewScore.Scores[player].Poor,
                Roll = NewScore.Scores[player].Roll,
                Data = inputdata,
                Setting = inputsetting,
                Chip = chipdata,
                Option = alloption,
                Speed = PlayData.Data.PlaySpeed
            };

            string path = $"{Path.GetFileNameWithoutExtension(SongData.NowTJA[player].Path)}.{(ECourse)NewGame.Course[player]}.{PlayData.Data.PlayerName}.{NewScore.Scores[player].EXScore}.{strTime}.tbr";
            ConfigJson.SaveConfig(replaydata, $"{Path.GetDirectoryName(SongData.NowTJA[player].Path)}/{path}");
            TextLog.Draw($"スコアが保存されました! : {path}", 2000);
            SongData.NowSong = SongLoad.ReLoad(SongData.NowSong);
            SaveReplay = true;
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

        public static bool SaveBest, SaveReplay;
        public static string strTime;
        public static Scores[] BestScores = new Scores[5];
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
        public double Speed;
        public string Option;
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
