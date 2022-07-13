using System;
using System.Collections.Generic;
using System.IO;
using SeaDrop;

namespace BMSParse
{
    public class BCourse
    {
        public int Player, Rank, TotalNotes;
        public double BPM, Level, Total;
        public string Title, Genre, Artist;
        public string LNOBJ;
        public EBCourse Difficulty = EBCourse.Beginner;
        public List<Wave> ListWave = new List<Wave>();
        public List<Bitmap> ListBitmap = new List<Bitmap>();
        public List<BPM> ListBPM = new List<BPM>();
        public List<Bar> ListBar = new List<Bar>();
        public List<Chip> ListChip = new List<Chip>();
        public List<BPMList> ListCBPM = new List<BPMList>();
        public List<Stop> ListStop = new List<Stop>();
        public List<StopList> ListStops = new List<StopList>();
        public bool IsEnable = false;
        public int[] RanMap = new int[8];

        public static double NowTime, NowBPM;
        public static Chip[] LongBegin = new Chip[8];
        

        public static void Load(string str, BCourse course)
        {
            if (str.StartsWith("#"))
            {
                if (str.StartsWith("#GENRE"))
                {
                    course.Genre = str.Replace("#GENRE", "").Trim();
                }
                else if (str.StartsWith("#TITLE"))
                {
                    course.Title = str.Replace("#TITLE", "").Trim();
                    if (course.Title.ToUpper().Contains("BEGINNER")) course.Difficulty = EBCourse.Beginner;
                    if (course.Title.ToUpper().Contains("NORMAL")) course.Difficulty = EBCourse.Normal;
                    if (course.Title.ToUpper().Contains("HYPER")) course.Difficulty = EBCourse.Hyper;
                    if (course.Title.ToUpper().Contains("ANOTHER")) course.Difficulty = EBCourse.Another;
                    if (course.Title.ToUpper().Contains("INSAME") || course.Title.ToUpper().Contains("LEGENDARIA") || course.Title.ToUpper().Contains("BLACK ANOTHER")) course.Difficulty = EBCourse.Insame;
                }
                else if (str.StartsWith("#ARTIST"))
                {
                    course.Artist = str.Replace("#ARTIST", "").Trim();
                }
                else if (str.StartsWith("#PLAYLEVEL"))
                {
                    course.Level = double.Parse(str.Replace("#PLAYLEVEL", "").Trim());
                }
                else if (str.StartsWith("#RANK"))
                {
                    course.Rank = int.Parse(str.Replace("#RANK", "").Trim());
                }
                else if (str.StartsWith("#DIFFICULTY"))
                {
                    course.Difficulty = (EBCourse)int.Parse(str.Replace("#DIFFICULTY", "").Trim()) - 1;
                }
                else if (str.StartsWith("#TOTAL"))
                {
                    course.Total = double.TryParse(str.Replace("#TOTAL", "").Trim(), out double p) ? double.Parse(str.Replace("#TOTAL", "").Trim()) : 0;
                }
                else if (str.StartsWith("#WAV"))
                {
                    Wave wav = new Wave()
                    {
                        ID = str.Substring(4, 2),
                        Path = str.Substring(7),
                    };
                    course.ListWave.Add(wav);
                }
                else if (str.StartsWith("#BMP"))
                {
                    Bitmap bmp = new Bitmap()
                    {
                        ID = str.Substring(4, 2),
                        Path = str.Substring(7),
                    };
                    course.ListBitmap.Add(bmp);
                }
                else if (str.StartsWith("#LNOBJ"))
                {
                    course.LNOBJ = str.Substring(7, 2);
                }
                else
                {
                    string s = str.Length > 5 ? str.Substring(1, 5) : "";
                    if (int.TryParse(s, out int p))
                    {
                        string[] array = str.Substring(1).Split(':');
                        if (array.Length == 2)
                        {
                            string ar = array[0];
                            int mej = int.Parse(ar.Substring(0, 3));
                            if (mej < course.ListBar.Count)
                            {
                                int ch = int.Parse(array[0].Substring(3, 2));
                                switch (ch)
                                {
                                    case 1://empty note
                                           //Normal
                                    case 16://sc
                                    case 11://1
                                    case 12://2
                                    case 13://3
                                    case 14://4
                                    case 15://5
                                    case 18://6
                                    case 19://7
                                            //Normal 2P
                                    case 26://sc
                                    case 21://1
                                    case 22://2
                                    case 23://3
                                    case 24://4
                                    case 25://5
                                    case 28://6
                                    case 29://7
                                            //Long
                                    case 56://bss
                                    case 51://1
                                    case 52://2
                                    case 53://3
                                    case 54://4
                                    case 55://5
                                    case 58://6
                                    case 59://7
                                            //Long 2P
                                    case 66://bss
                                    case 61://1
                                    case 62://2
                                    case 63://3
                                    case 64://4
                                    case 65://5
                                    case 68://6
                                    case 69://7
                                        int amount = array[1].Length / 2;
                                        for (int i = 0; i < amount; i++)
                                        {
                                            string num = array[1].Substring(2 * i, 2);
                                            string se = "";
                                            foreach (Wave wave in course.ListWave)
                                            {
                                                if (num == wave.ID)
                                                {
                                                    se = $@"{BMS.Dir}\{wave.Path}";
                                                    if (!File.Exists(se))
                                                    {
                                                        if (File.Exists(se.Replace(".wav", ".ogg")))
                                                        {
                                                            se = se.Replace(".wav", ".ogg");
                                                        }
                                                    }
                                                }
                                            }
                                            //if (ch > 50) ch -= 40;
                                            Chip chip = new Chip()
                                            {
                                                Channel = ch,
                                                ID = num,
                                                SoundPath = num != "00" ? se : "",
                                                Place = (amount, i)
                                            };
                                            course.ListBar[mej].Chip.Add(chip); //if (num != "00") 
                                            if (!course.IsEnable) course.IsEnable = true;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void GetBPM(string str, BCourse course, double playspeed)
        {
            if (str.StartsWith("#"))
            {
                if (str.StartsWith("#BPM"))
                {
                    if (double.TryParse(str.Substring(4), out double p))
                    {
                        course.BPM = double.Parse(str.Replace("#BPM", "").Trim()) * playspeed;
                        NowBPM = course.BPM * playspeed;
                    }
                    else
                    {
                        BPM bpm = new BPM()
                        {
                            ID = str.Substring(4, 2),
                            Value = double.Parse(str.Substring(7)) * playspeed,
                        };
                        course.ListBPM.Add(bpm);
                    }
                }
                else if (str.StartsWith("#STOP"))
                {
                    Stop stop = new Stop()
                    {
                        ID = str.Substring(5, 2),
                        Value = double.Parse(str.Substring(8)),
                    };
                    course.ListStop.Add(stop);
                }
                else
                {
                    string s = str.Length > 5 ? str.Substring(1, 5) : "";
                    if (int.TryParse(s, out int p))
                    {
                        string[] array = str.Substring(1).Split(':');
                        if (array.Length == 2)
                        {
                            string ar = array[0];
                            int mej = int.Parse(ar.Substring(0, 3));
                            while (mej >= course.ListBar.Count)
                            {
                                course.ListBar.Add(new Bar(course.ListBar.Count, NowBPM, 1.0, NowTime));
                                if (mej >= course.ListBar.Count) NowTime += 240000 * course.ListBar[course.ListBar.Count - 1].Measure / NowBPM;
                            }

                            if (mej < course.ListBar.Count)
                            {
                                int ch = int.Parse(array[0].Substring(3, 2));
                                switch (ch)
                                {
                                    case 2:
                                        course.ListBar[mej].Measure = double.Parse(array[1]);
                                        break;
                                    case 3:
                                        int amount = array[1].Length / 2;
                                        double t = 0;
                                        for (int i = 0; i < amount; i++)
                                        {
                                            string num = array[1].Substring(2 * i, 2);
                                            if (num != "00") NowBPM = int.Parse(num, System.Globalization.NumberStyles.HexNumber) * playspeed;
                                            BPMList chip = new BPMList()
                                            {
                                                Time = NowTime + t,
                                                Value = NowBPM * playspeed,
                                                Place = (mej, amount, i)
                                            };
                                            if (num != "00") course.ListCBPM.Add(chip);
                                            if (i == 0)
                                            {
                                                double m = course.ListBar[mej].Measure;
                                                course.ListBar[mej] = new Bar(mej, NowBPM, m, NowTime);
                                            }
                                            t += 240000 * course.ListBar[mej].Measure / NowBPM / amount;
                                        }
                                        NowTime += t;
                                        break;
                                    case 8:
                                        amount = array[1].Length / 2;
                                        t = 0;
                                        for (int i = 0; i < amount; i++)
                                        {
                                            string num = array[1].Substring(2 * i, 2);
                                            foreach (BPM bpm in course.ListBPM)
                                                if (num == bpm.ID) NowBPM = bpm.Value * playspeed;
                                            BPMList chip = new BPMList()
                                            {
                                                Time = NowTime + t,
                                                Value = NowBPM * playspeed,
                                                Place = (mej, amount, i)
                                            };
                                            if (num != "00") course.ListCBPM.Add(chip);
                                            if (i == 0)
                                            {
                                                double m = course.ListBar[mej].Measure;
                                                course.ListBar[mej] = new Bar(mej, NowBPM, m, NowTime);
                                            }
                                            t += 240000 * course.ListBar[mej].Measure / NowBPM / amount;
                                        }
                                        NowTime += t;
                                        break;
                                    case 9:
                                        amount = array[1].Length / 2;
                                        t = 0;
                                        double v = 0;
                                        for (int i = 0; i < amount; i++)
                                        {
                                            string num = array[1].Substring(2 * i, 2);
                                            foreach (Stop stop in course.ListStop)
                                                if (num == stop.ID) v = stop.Value * playspeed;
                                            StopList chip = new StopList()
                                            {
                                                Time = NowTime + t,
                                                Value = v,
                                                Place = (mej, amount, i)
                                            };
                                            if (num != "00") course.ListStops.Add(chip);
                                            t += 240000 * course.ListBar[mej].Measure / NowBPM / amount;
                                        }
                                        NowTime += t;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SetChip(BCourse course)
        {
            double bartime = 0;
            double time, btime;
            double bpm = course.BPM;
            double bbpm = course.BPM;
            double fbpm = course.BPM;
            foreach (Bar bar in course.ListBar)
            {
                time = 0;
                btime = 0;
                bar.Time = bartime;
                fbpm = bpm;

                //(int)(a.Place.Item1 - b.Place.Item1)
                course.ListCBPM.Sort((a, b) => { int q = a.Place.Item1 - b.Place.Item1; return q != 0 ? q : (int)(((double)a.Place.Item3 * 1000 / a.Place.Item2) - ((double)b.Place.Item3 * 1000 / b.Place.Item2)); });

                List<BPMList> list = new List<BPMList>();
                for (int b = course.ListCBPM.Count - 1; b >= 0; b--)
                {
                    BPMList chip = course.ListCBPM[b];
                    if (b < course.ListCBPM.Count - 1 && chip.Place == course.ListCBPM[b + 1].Place) course.ListCBPM.RemoveAt(b);
                    else if (chip.Place.Item1 == bar.Number - 1)
                    {
                        list.Add(chip);
                    }
                }

                course.ListStops.Sort((a, b) => { int q = a.Place.Item1 - b.Place.Item1; return q != 0 ? q : (int)(((double)a.Place.Item3 * 1000 / a.Place.Item2) - ((double)b.Place.Item3 * 1000 / b.Place.Item2)); });
                List<StopList> slist = new List<StopList>();
                for (int b = course.ListStops.Count - 1; b >= 0; b--)
                {
                    StopList chip = course.ListStops[b];
                    if (b < course.ListStops.Count - 1 && chip.Place == course.ListStops[b + 1].Place) course.ListStops.RemoveAt(b);
                    else if (chip.Place.Item1 == bar.Number - 1)
                    {
                        slist.Add(chip);
                    }
                }

                list.Sort((a, b) => { return (int)(((double)a.Place.Item3 * 1000 / a.Place.Item2) - ((double)b.Place.Item3 * 1000 / b.Place.Item2)); });
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0) btime += 240000 * bar.Measure / bpm / list[i].Place.Item2 * list[i].Place.Item3;
                    list[i].Time = bartime + btime;
                    bbpm = list[i].Value;
                    double mej = (double)(list[i].Place.Item2 - list[i].Place.Item3) / list[i].Place.Item2;
                    if (list.Count == 1 && list[i].Place.Item3 == 0) mej = 1;
                    if (i < list.Count - 1 && (double)list[i + 1].Place.Item3 / list[i + 1].Place.Item2 > (double)list[i].Place.Item3 / list[i].Place.Item2)
                        mej = (double)list[i + 1].Place.Item3 / list[i + 1].Place.Item2 - (double)list[i].Place.Item3 / list[i].Place.Item2;
                    double v = 240000 * bar.Measure / bbpm * mej;
                    btime += v;
                    for (int b = 0; b < slist.Count; b++)
                    {
                        if (bartime + btime >= slist[b].Time)
                        {
                            btime += slist[b].Value;
                        }
                    }
                }

                
                slist.Sort((a, b) => { return (int)(((double)a.Place.Item3 * 1000 / a.Place.Item2) - ((double)b.Place.Item3 * 1000 / b.Place.Item2)); });
                double sbpm = bpm, stime = 0;
                for (int i = 0; i < slist.Count; i++)
                {
                    for (int b = 0; b < list.Count; b++)
                    {
                        if (bartime + time >= list[b].Time)
                        {
                            sbpm = list[b].Value;
                        }
                    }
                    if (i == 0) stime += 240000 * bar.Measure / sbpm / slist[i].Place.Item2 * slist[i].Place.Item3;
                    slist[i].Time = bartime + stime;
                    double mej = (double)(slist[i].Place.Item2 - slist[i].Place.Item3) / slist[i].Place.Item2;
                    if (slist.Count == 1 && slist[i].Place.Item3 == 0) mej = 1;
                    if (i < slist.Count - 1 && (double)slist[i + 1].Place.Item3 / slist[i + 1].Place.Item2 > (double)slist[i].Place.Item3 / slist[i].Place.Item2)
                        mej = (double)slist[i + 1].Place.Item3 / slist[i + 1].Place.Item2 - (double)slist[i].Place.Item3 / slist[i].Place.Item2;
                    double v = 240000 * bar.Measure / sbpm * mej;
                    stime += v;
                }

                for (int i = 0; i < bar.Chip.Count; i++)
                {
                    Chip chip = bar.Chip[i];
                    if (chip.Place.Item2 == 0) time = 0;

                    string str = chip.Draw();
                    bpm = fbpm;
                    for (int b = 0; b < list.Count; b++)
                    {
                        if (bartime + time >= list[b].Time)
                        {
                            bpm = list[b].Value;
                        }
                    }
                    chip.Time = Math.Round(bartime + time, 4, MidpointRounding.AwayFromZero);
                    chip.Bpm = bpm;
                    time += 240000 * bar.Measure / bpm / chip.Place.Item1;
                    int nu = bar.Number - 1;
                    for (int b = 0; b < slist.Count; b++)
                    {
                        if (bartime + time >= slist[b].Time)
                        {
                            time += slist[b].Value;
                        }
                    }
                }
                double t = time;
                if (bar.Chip.Count == 0) t = 240000 * bar.Measure / bpm;
                if (list.Count > 0) t = btime;
                for (int b = 0; b < list.Count; b++)
                {
                    if (bartime >= list[b].Time)
                    {
                        fbpm = list[b].Value;
                    }
                }
                bar.BPM = fbpm;
                if (list.Count > 0) bpm = list[list.Count - 1].Value;
                bartime += t;

                foreach (Chip chip in bar.Chip)
                {
                    switch (chip.Channel)
                    {
                        case 1://empty note
                            if (chip.ID != "00")
                            {
                                course.ListChip.Add(chip);
                            }
                            break;
                        case 16:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 18:
                        case 19:
                        case 26:
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                        case 28:
                        case 29:
                        case 56:
                        case 51:
                        case 52:
                        case 53:
                        case 54:
                        case 55:
                        case 58:
                        case 59:
                        case 66:
                        case 61:
                        case 62:
                        case 63:
                        case 64:
                        case 65:
                        case 68:
                        case 69:
                            if (chip.ID != "00")
                            {
                                course.ListChip.Add(chip);
                                course.TotalNotes++;
                                if (chip.Channel / 10 == 1 && course.Player == 0) course.Player = 1;
                                if (chip.Channel / 10 == 2 && course.Player == 1) course.Player = 2;
                            }
                            break;
                    }
                }

                course.ListCBPM.Sort((a, b) => { int n = (a != null ? 1 : 0) - (b != null ? 1 : 0); return n != 0 ? n : (int)(a.Time - b.Time); });
                bar.Chip.Sort((a, b) => { int n = (a != null ? 1 : 0) - (b != null ? 1 : 0); int q = n != 0 ? n : (int)(a.Time - b.Time); return q != 0 ? q : GetLane(a) - GetLane(b); });

                bartime = Math.Round(bartime, 4, MidpointRounding.AwayFromZero);
            }
        }

        public static void SetLong(BCourse course, List<Chip> chips, int type)
        {
            List<int> ends = new List<int>();
            for (int i = 0; i < chips.Count; i++)
            {
                Chip chip = chips[i];
                if (chip.Channel >= 50 && chip.Channel < 70)
                {
                    chip.Channel -= 40;
                    for (int j = 0; j < 8; j++)
                    {
                        if (chip.Channel == GetChannel(j) || chip.Channel == GetChannel(j, true))
                        {
                            if (LongBegin[j] != null)
                            {
                                LongBegin[j].LongEnd = chip;
                                LongBegin[j] = null;
                                ends.Add(i);
                            }
                            else
                            {
                                LongBegin[j] = chip;
                            }
                            break;
                        }
                    }
                }
                else if (chip.ID == course.LNOBJ)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (chip.Channel == GetChannel(j))
                        {
                            PreLong(chips, chip).LongEnd = chip;
                            ends.Add(i);
                            break;
                        }
                    }
                }
                chip.RanNum = chip.Channel;
            }
            for (int i = chips.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < ends.Count; j++)
                {
                    if (i == ends[j])
                    {
                        chips.RemoveAt(i);
                        if (type == 0)
                            course.TotalNotes--;
                    }
                }
            }
        }

        public static Chip PreLong(List<Chip> chips, Chip LNChip)
        {
            for (int i = chips.Count - 1; i >= 0; i--)
            {
                Chip chip = chips[i];
                if (chip.Channel == LNChip.Channel && chip.Time < LNChip.Time) return chip;
            }
            return null;
        }

        public static void SetSound(List<Chip> chips)
        {
            for (int i = 0; i < chips.Count; i++)
            {
                Chip chip = chips[i];
                chip.Sound = new Sound(chip.SoundPath);
            }
        }
        public static void DeleteSound(List<Chip> chips)
        {
            for (int i = 0; i < chips.Count; i++)
            {
                Chip chip = chips[i];
                if (chip.Sound != null)
                {
                    chip.Sound.Dispose();
                    chip.Sound = null;
                }
            }
        }

        public static void SetOption(List<Chip> chips, EOption option, BCourse course)
        {
            Random r = new Random();
            int ran = r.Next(0, 7);
            int mir = r.Next(0, 2);
            List<int> rancount = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            List<int> ranlane = new List<int>();
            while (rancount.Count > 0)
            {
                int index = r.Next(0, rancount.Count);
                ranlane.Add(rancount[index]);
                rancount.RemoveAt(index);
            }
            double[] used = new double[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
            List<Chip> chipList = new List<Chip>();
            foreach (Chip chip in chips)
            {
                if (chip != null && GetLane(chip) >= 0)
                {
                    chipList.Add(chip);
                }
            }
            if (chipList.Count > 1) chipList.Sort((a, b) => { int q = (int)(a.Time - b.Time); return q != 0 ? q : GetLane(a) - GetLane(b); });
            for (int i = 0; i < chipList.Count; i++)
            {
                Chip chip = chipList[i];
                switch (option)
                {
                    case EOption.None:
                        chip.Channel = chip.RanNum;
                        //RanMap = new int[] { 1, 2, 3, 4, 5, 6, 7 };
                        break;
                    case EOption.Mirror:
                        for (int j = 1; j < 8; j++)
                        {
                            if (chip.RanNum == GetChannel(j))
                            {
                                chip.Channel = GetChannel(8 - j);
                                break;
                            }
                        }
                        //RanMap = new int[] { 7, 6, 5, 4, 3, 2, 1 };
                        break;
                    case EOption.Random:
                        for (int j = 1; j < 8; j++)
                        {
                            if (chip.RanNum == GetChannel(j))
                            {
                                chip.Channel = GetChannel(ranlane[j - 1]);
                                break;
                            }
                        }
                        break;
                    case EOption.RRandom:
                        for (int j = 1; j < 8; j++)
                        {
                            if (chip.RanNum == GetChannel(j))
                            {
                                chip.Channel = GetChannel((j + ran - 1) % 7 + 1);
                                break;
                            }
                        }
                        if (mir == 1)
                        {
                            for (int j = 1; j < 8; j++)
                            {
                                if (chip.Channel == GetChannel(j))
                                {
                                    chip.Channel = GetChannel(8 - j);
                                    break;
                                }
                            }
                        }
                        break;
                    case EOption.SRandom:
                        if (GetLane(chip) > 0)
                        {
                            while (true)
                            {
                                int ch = chip.Channel;
                                int sl = r.Next(1, 8);
                                if (chip.Time > used[sl])
                                {
                                    chip.Channel = GetChannel(sl);
                                    used[sl] = chip.LongEnd != null ? chip.LongEnd.Time : chip.Time;
                                    break;
                                }
                            }
                            for (int j = 0; j < used.Length; j++)
                            {
                                if (chip.Time > used[j])
                                {
                                    used[j] = -1;
                                }
                            }
                        }
                        break;
                    case EOption.AllScrach:
                        if (GetLane(chip) > 0)
                        {
                            while (true)
                            {
                                int ch = chip.Channel;
                                int sl = r.Next(1, 8);
                                if (!IncludeScrach(chipList, chip) && chip.Time > used[0])
                                {
                                    chip.Channel = GetChannel(0);
                                    used[0] = chip.LongEnd != null ? chip.LongEnd.Time : chip.Time;
                                    break;
                                }
                                else if (chip.Time > used[sl])
                                {
                                    chip.Channel = GetChannel(sl);
                                    used[sl] = chip.LongEnd != null ? chip.LongEnd.Time : chip.Time;
                                    break;
                                }
                            }
                            for (int j = 0; j < used.Length; j++)
                            {
                                if (chip.Time > used[j])
                                {
                                    used[j] = -1;
                                }
                            }
                        }
                        break;
                }
            }
            for (int i = 0; i < chips.Count; i++)
            {
                Chip chip = chips[i];
                for (int j = 0; j < 8; j++)
                {
                    if (chip.RanNum == GetChannel(j))
                    {
                        course.RanMap[GetLane(chip)] = j;
                        break;
                    }
                }
            }
        }

        public static bool IncludeScrach(List<Chip> chips, Chip timechip)
        {
            double time = timechip.Time;
            double end = timechip.LongEnd != null ? timechip.LongEnd.Time : timechip.Time;
            for (int i = 0; i < chips.Count; i++)
            {
                Chip chip = chips[i];
                if (GetLane(chip) == 0 && chip.Time >= time && chip.Time <= end) return true;
            }
            return false;
        }

        public static int GetLane(Chip chip)
        {
            switch (chip.Channel)
            {
                case 16:
                case 26:
                    return 0;
                case 11:
                case 21:
                    return 1;
                case 12:
                case 22:
                    return 2;
                case 13:
                case 23:
                    return 3;
                case 14:
                case 24:
                    return 4;
                case 15:
                case 25:
                    return 5;
                case 18:
                case 28:
                    return 6;
                case 19:
                case 29:
                    return 7;
                default:
                    return -1;
            }
        }
        public static int GetLane2P(Chip chip)
        {
            switch (chip.Channel)
            {
                case 16:
                case 26:
                    return 7;
                case 11:
                case 21:
                    return 0;
                case 12:
                case 22:
                    return 1;
                case 13:
                case 23:
                    return 2;
                case 14:
                case 24:
                    return 3;
                case 15:
                case 25:
                    return 4;
                case 18:
                case 28:
                    return 5;
                case 19:
                case 29:
                    return 6;
                default:
                    return -1;
            }
        }
        public static int GetLane(Chip chip, bool is2PLane)
        {
            if (is2PLane)
            {
                switch (chip.Channel)
                {
                    case 26:
                        return 0;
                    case 21:
                        return 1;
                    case 22:
                        return 2;
                    case 23:
                        return 3;
                    case 24:
                        return 4;
                    case 25:
                        return 5;
                    case 28:
                        return 6;
                    case 29:
                        return 7;
                    default:
                        return -1;
                }
            }
            else
            {
                switch (chip.Channel)
                {
                    case 16:
                        return 0;
                    case 11:
                        return 1;
                    case 12:
                        return 2;
                    case 13:
                        return 3;
                    case 14:
                        return 4;
                    case 15:
                        return 5;
                    case 18:
                        return 6;
                    case 19:
                        return 7;
                    default:
                        return -1;
                }
            }
        }
        public static int GetLane2P(Chip chip, bool is2PLane)
        {
            if (is2PLane)
            {
                switch (chip.Channel)
                {
                    case 26:
                        return 7;
                    case 21:
                        return 0;
                    case 22:
                        return 1;
                    case 23:
                        return 2;
                    case 24:
                        return 3;
                    case 25:
                        return 4;
                    case 28:
                        return 5;
                    case 29:
                        return 6;
                    default:
                        return -1;
                }
            }
            else
            {
                switch (chip.Channel)
                {
                    case 16:
                        return 7;
                    case 11:
                        return 0;
                    case 12:
                        return 1;
                    case 13:
                        return 2;
                    case 14:
                        return 3;
                    case 15:
                        return 4;
                    case 18:
                        return 5;
                    case 19:
                        return 6;
                    default:
                        return -1;
                }
            }
        }
        public static int GetChannel(int value, bool is2PLane = false)
        {
            int channel = 0;
            switch (value)
            {
                case 0:
                    channel = 16;
                    break;
                case 1:
                    channel = 11;
                    break;
                case 2:
                    channel = 12;
                    break;
                case 3:
                    channel = 13;
                    break;
                case 4:
                    channel = 14;
                    break;
                case 5:
                    channel = 15;
                    break;
                case 6:
                    channel = 18;
                    break;
                case 7:
                    channel = 19;
                    break;
            }
            return is2PLane ? channel + 10 : channel;
        }
        public static int GetChannel2P(int value, bool is2PLane = false)
        {
            int channel = 0;
            switch (value)
            {
                case 0:
                    channel = 11;
                    break;
                case 1:
                    channel = 12;
                    break;
                case 2:
                    channel = 13;
                    break;
                case 3:
                    channel = 14;
                    break;
                case 4:
                    channel = 15;
                    break;
                case 5:
                    channel = 18;
                    break;
                case 6:
                    channel = 19;
                    break;
                case 7:
                    channel = 16;
                    break;
            }
            return is2PLane ? channel + 10 : channel;
        }
    }

    public enum EBCourse
    {
        Beginner,
        Normal,
        Hyper,
        Another,
        Insame,
        None = -1
    }

    public enum EOption
    {
        None,
        Mirror,
        Random,
        RRandom,
        SRandom,
        AllScrach
    }
}
