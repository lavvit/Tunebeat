using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAParse
{
    public class Course
    {
        public ECourse COURSE = ECourse.Oni;
        public EScroll ScrollType = EScroll.Normal;
        public int LEVEL = 0, SCOREINIT, SCOREDIFF, TotalNotes;
        public double TOTAL;
        public List<int> BALLOON = new List<int>();
        public List<int> ListMeasureCount = new List<int>();
        public List<Chip> ListChip = new List<Chip>();

        public static int NowCourse;
        public static int MeasureCount;

        public static void Load(string str, Course[] courses, Header header)
        {
            if (str.Length <= 0) return;
            var split = str.Split(':');
            if (split.Length >= 2)
            {
                switch (split[0])
                {
                    case "COURSE":
                        NowCourse = GetCourse(split[1]);
                        courses[NowCourse].COURSE = (ECourse)NowCourse;
                        break;
                    case "LEVEL":
                        courses[NowCourse].LEVEL = int.Parse(split[1]);
                        break;
                    case "BALLOON":
                        var splitballoon = split[1].Split(',');
                        foreach (var sb in splitballoon)
                            if (sb != "" && splitballoon.Length >= 1)
                                courses[NowCourse].BALLOON.Add(int.Parse(sb));
                        break;
                    case "TOTAL":
                        courses[NowCourse].TOTAL = double.Parse(split[1]);
                        break;
                }
            }
            else
            {
                if (str.StartsWith("#"))
                {
                    if (str.StartsWith("#BMSCROLL"))
                    {
                        NowInfo.ScrollType = EScroll.BMSCROLL;
                    }
                    else if (str.StartsWith("#HBSCROLL"))
                    {
                        NowInfo.ScrollType = EScroll.HBSCROLL;
                    }
                    else if (str.StartsWith("#START"))
                    {
                        NowInfo.StartParse = true;
                        NowInfo.Time = (long)(header.OFFSET * -1000.0);
                        NowInfo.Scroll = 1.0;
                        NowInfo.Bpm = header.BPM;
                        NowInfo.Measure = 1.0;
                        NowInfo.MeasureCount = 0;
                        NowInfo.ShowBarLine = true;
                        NowInfo.AddMeasure = true;
                        NowInfo.RollBegin = null;
                    }
                    else if (str.StartsWith("#END"))
                    {
                        NowInfo.StartParse = false;
                    }
                    else if (str.StartsWith("#SCROLL"))
                    {
                        NowInfo.Scroll = double.Parse(str.Replace("#SCROLL", "").Trim());
                    }
                    else if (str.StartsWith("#BPMCHANGE"))
                    {
                        NowInfo.Bpm = double.Parse(str.Replace("#BPMCHANGE", "").Trim());
                    }
                    else if (str.StartsWith("#DELAY"))
                    {
                        NowInfo.Time += double.Parse(str.Replace("#DELAY", "").Trim()) * 1000.0;
                    }
                    else if (str.StartsWith("#MEASURE"))
                    {
                        var SplitSlash = str.Replace("#MEASURE", "").Trim().Split('/');
                        if (SplitSlash.Length > 1) NowInfo.Measure = (double.Parse(SplitSlash[1], System.Globalization.NumberStyles.AllowExponent) / double.Parse(SplitSlash[0], System.Globalization.NumberStyles.AllowExponent));
                    }
                    else if (str.StartsWith("#BARLINEON"))
                    {
                        NowInfo.ShowBarLine = true;
                    }
                    else if (str.StartsWith("#BARLINEOFF"))
                    {
                        NowInfo.ShowBarLine = false;
                    }
                }
                else
                {

                    if (!((str[0] >= '0' && str[0] <= '9') || (str[str.Length - 1] >= '0' && str[str.Length - 1] <= '9') || str[0] == ',' || str[str.Length - 1] == ',')) return;
                    if (!NowInfo.StartParse) return;

                    if (NowInfo.AddMeasure)
                    {
                        AddChip(NowInfo.Time, ENote.Space, EChip.Measure, courses[NowCourse].ListChip, ref courses[NowCourse], NowInfo.ShowBarLine);
                        NowInfo.AddMeasure = false;
                    }

                    for (int i = 0; i < str.Length; i++)
                    {
                        var num = str[i];
                        if (num == ',')
                        {
                            NowInfo.MeasureCount++;
                            NowInfo.AddMeasure = true;
                        }
                        if (num >= '0' && num <= '9')
                        {
                            Chip chip = new Chip()
                            {
                                Time = NowInfo.Time,
                                Bpm = NowInfo.Bpm,
                                Scroll = NowInfo.Scroll,
                                Measure = NowInfo.Measure,
                                EChip = EChip.Note,
                                ENote = (ENote)int.Parse(num.ToString()),
                                EScroll = NowInfo.ScrollType,
                                CanShow = true
                            };

                            courses[NowCourse].ScrollType = chip.EScroll;

                            if (chip.ENote == ENote.Balloon || chip.ENote == ENote.RollStart || chip.ENote == ENote.ROLLStart || chip.ENote == ENote.Kusudama)
                            {
                                if (NowInfo.RollBegin == null)
                                    NowInfo.RollBegin = chip;
                            }
                            if (chip.EChip == EChip.Note && chip.ENote == ENote.RollEnd)
                            {
                                if (NowInfo.RollBegin != null)
                                {
                                    NowInfo.RollBegin.RollEnd = chip;
                                    chip.RollEnd = chip;
                                }
                                NowInfo.RollBegin = null;
                            }

                            if (chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                                courses[NowCourse].TotalNotes++;

                            if (chip.ENote != ENote.Space)
                                courses[NowCourse].ListChip.Add(chip);

                            NowInfo.Time += 15000d / NowInfo.Bpm / NowInfo.Measure * (16d / courses[NowCourse].ListMeasureCount[NowInfo.MeasureCount < courses[NowCourse].ListMeasureCount.Count ? NowInfo.MeasureCount : courses[NowCourse].ListMeasureCount.Count - 1]);
                        }
                    }
                }
            }


        }

        public static void RollDoubledCheck(Course[] courses)//仕方なく分ける
        {
            for (int i = courses[NowCourse].ListChip.Count - 1; i > 0; i--)
            {
                if (courses[NowCourse].ListChip[i].ENote == courses[NowCourse].ListChip[i - 1].ENote)
                {
                    if (courses[NowCourse].ListChip[i].ENote == ENote.Balloon || courses[NowCourse].ListChip[i].ENote == ENote.RollStart || courses[NowCourse].ListChip[i].ENote == ENote.ROLLStart || courses[NowCourse].ListChip[i].ENote == ENote.Kusudama)
                    {
                        courses[NowCourse].ListChip.RemoveAt(i);
                    }
                }
            }
        }

        public static void GetMeasureCount(string str, Course[] courses)
        {
            if (str.Length <= 0) return;
            string[] split = str.Split(':');
            if (split.Length >= 2)
            {
                switch (split[0])
                {
                    case "COURSE":
                        NowCourse = GetCourse(split[1]);
                        courses[NowCourse].COURSE = (ECourse)NowCourse;
                        break;
                    case "LEVEL":
                        courses[NowCourse].LEVEL = int.Parse(split[1]);
                        break;
                    case "BALLOON":
                        var splitballoon = split[1].Split(',');
                        foreach (var sb in splitballoon)
                            if (sb != "" && splitballoon.Length >= 1)
                                courses[NowCourse].BALLOON.Add(int.Parse(sb));
                        break;
                }
            }
            else
            {
                if (str.StartsWith("#"))
                {
                    if (str.StartsWith("#START"))
                    {
                        NowInfo.StartParse = true;
                    }
                    else if (str.StartsWith("#END"))
                    {
                        NowInfo.StartParse = false;
                    }
                }
                else
                {
                    if (!((str[0] >= '0' && str[0] <= '9') || (str[str.Length - 1] >= '0' && str[str.Length - 1] <= '9') || str[0] == ',' || str[str.Length - 1] == ',')) return;
                    if (!NowInfo.StartParse) return;

                    for (int i = 0; i < str.Length; i++)
                    {
                        var num = str[i];

                        if (num == ',')
                        {
                            courses[NowCourse].ListMeasureCount.Add(MeasureCount);
                            MeasureCount = 0;
                        }
                        else if (num >= '0' && num <= '9')
                        {
                            MeasureCount++;
                        }
                    }
                }
            }
        }

        public static void AddChip(double time, ENote note, EChip channel, List<Chip> listChip, ref Course course, bool isShow = true)
        {
            var Chip = new Chip()
            {
                Time = time,
                Scroll = NowInfo.Scroll,
                Bpm = NowInfo.Bpm,
                EChip = channel,
                Measure = NowInfo.Measure,
                ENote = note,
                IsShow = isShow,
            };

            course.ListChip.Add(Chip);
        }

        public static int GetCourse(string str)
        {
            var ret = int.TryParse(str, out int nCourse);
            if (ret) return nCourse;
            switch (str.ToLower())
            {
                case "easy": return 0;
                case "normal": return 1;
                case "hard": return 2;
                case "oni": return 3;
                case "edit": return 4;
                default: return 3;
            }
        }
    }

    public enum ECourse
    {
        Easy,
        Normal,
        Hard,
        Oni,
        Edit
    }

    public enum EScroll
    {
        Normal,
        BMSCROLL,
        HBSCROLL
    }
}
