using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SeaDrop;

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
        public List<Bar> ListBar = new List<Bar>();
        public List<Command> ListCommand = new List<Command>();
        public bool IsEnable = false;

        public static int NowCourse = (int)ECourse.Oni;
        public static int MeasureCount, BalloonCount, BarCount;
        public static List<List<Chip>> WChip = new List<List<Chip>>();

        public static void Load(string str, Course[] courses, Header header, double playspeed, bool FullLoad = true)
        {
            if (str.Length <= 0) return;
            var split = str.Split(':');
            if (split.Length >= 2)
            {
                HeaderLoad(split, courses, FullLoad);
            }
            else
            {
                if (str.StartsWith("#"))
                {
                    CommandLoad(str, courses, header, playspeed, FullLoad);
                }
                else
                {
                    ChipLoad(str, courses, FullLoad);
                }
            }
        }

        public static void HeaderLoad(string[] split, Course[] courses, bool FullLoad)
        {
            switch (split[0])
            {
                case "COURSE":
                    NowCourse = GetCourse(split[1]);
                    courses[NowCourse].COURSE = (ECourse)NowCourse;
                    break;
                case "LEVEL":
                    courses[NowCourse].LEVEL = !string.IsNullOrEmpty(split[1]) && double.TryParse(split[1], out double i) ? (int)double.Parse(split[1]) : 0;
                    break;
            }
            if (FullLoad)
            {
                switch (split[0])
                {
                    case "BALLOON":
                        var splitballoon = split[1].Split(',');
                        foreach (var sb in splitballoon)
                            if (sb.Trim() != "" && splitballoon.Length >= 1)
                                courses[NowCourse].BALLOON.Add(int.Parse(sb.Trim()));
                        break;
                    case "TOTAL":
                        courses[NowCourse].TOTAL = !string.IsNullOrEmpty(split[1]) && double.TryParse(split[1], out double i) ? double.Parse(split[1]) : 0;
                        break;
                }
            }
        }
        public static void CommandLoad(string str, Course[] courses, Header header, double playspeed, bool FullLoad)
        {
            Command c = new Command()
            {
                Name = str,
                Time = NowInfo.Time
            };
            if (str.StartsWith("#BMSCROLL"))
            {
                courses[NowCourse].ScrollType = EScroll.BMSCROLL;
            }
            else if (str.StartsWith("#HBSCROLL"))
            {
                courses[NowCourse].ScrollType = EScroll.HBSCROLL;
            }
            if (FullLoad)
            {
                if (str.StartsWith("#START"))
                {
                    NowInfo.StartParse = true;
                    NowInfo.Time = (long)(header.OFFSET * -1000.0) / playspeed;
                    NowInfo.Scroll = 1.0;
                    NowInfo.Bpm = header.BPM;
                    NowInfo.Measure = 1.0;
                    NowInfo.MeasureCount = 0;
                    NowInfo.ShowBarLine = true;
                    NowInfo.AddMeasure = true;
                    NowInfo.RollBegin = null;
                    NowInfo.LyricText = null;
                    NowInfo.Sudden = new double[2] { 0.0, 0.0 };
                    NowInfo.RollState = ERoll.None;
                    WChip = new List<List<Chip>>() { new List<Chip>() };
                    BalloonCount = 0;
                    BarCount = 1;
                }
                else if (str.StartsWith("#END"))
                {
                    NowInfo.StartParse = false;
                }
                else if (str.StartsWith("#SCROLL"))
                {
                    NowInfo.Scroll = double.Parse(str.Replace("#SCROLL", "").Trim());
                    courses[NowCourse].ListCommand.Add(c);

                }
                else if (str.StartsWith("#BPMCHANGE"))
                {
                    NowInfo.Bpm = double.Parse(str.Replace("#BPMCHANGE", "").Trim()) * playspeed;
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#DELAY"))
                {
                    NowInfo.Time += double.Parse(str.Replace("#DELAY", "").Trim()) * 1000.0 / playspeed;
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#MEASURE"))
                {
                    var SplitSlash = str.Replace("#MEASURE", "").Trim().Split('/');
                    if (SplitSlash.Length > 1) NowInfo.Measure = double.Parse(SplitSlash[1]) / double.Parse(SplitSlash[0]);
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#BARLINEON"))
                {
                    NowInfo.ShowBarLine = true;
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#BARLINEOFF"))
                {
                    NowInfo.ShowBarLine = false;
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#GOGOSTART"))
                {
                    NowInfo.IsGogo = true;
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#GOGOEND"))
                {
                    NowInfo.IsGogo = false;
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#LYRIC"))
                {
                    NowInfo.LyricText = str.Replace("#LYRIC", "").Trim();
                    courses[NowCourse].ListCommand.Add(c);
                }
                else if (str.StartsWith("#SUDDEN"))
                {
                    var SplitSlash = str.Replace("#SUDDEN", "").Trim().Split(' ');
                    if (SplitSlash.Length > 1) NowInfo.Sudden = new double[2] { double.Parse(SplitSlash[0]) * 1000.0, double.Parse(SplitSlash[1]) * 1000.0 };
                    else NowInfo.Sudden = new double[2] { double.Parse(SplitSlash[0]) * 1000.0, double.Parse(SplitSlash[0]) * 1000.0 };
                    courses[NowCourse].ListCommand.Add(c);
                }
            }
        }
        public static void ChipLoad(string str, Course[] courses, bool FullLoad)
        {
            if (FullLoad)
            {
                if (!((str[0] >= '0' && str[0] <= '9') || (str[str.Length - 1] >= '0' && str[str.Length - 1] <= '9') || str[0] == ',' || str[str.Length - 1] == ',')) return;
                if (!NowInfo.StartParse) return;
                //if (courses[NowCourse].ListMeasureCount.Count == 0) return;

                if (NowInfo.AddMeasure)
                {
                    int measure = NowInfo.MeasureCount;
                    List<Chip> chips = WChip[measure];
                    if (chips.Count > 0)
                    {
                        foreach (Chip cchip in chips)
                        {
                            cchip.Time = Math.Round(cchip.Time, 4, MidpointRounding.AwayFromZero);
                        }
                    }
                    Bar bar = new Bar(BarCount, Math.Round(NowInfo.Time, 4, MidpointRounding.AwayFromZero), NowInfo.Bpm, NowInfo.Scroll, 1 / NowInfo.Measure, chips.Count, NowInfo.ShowBarLine, chips);
                    courses[NowCourse].ListBar.Add(bar);
                    BarCount++;
                    NowInfo.AddMeasure = false;
                }
            }

            for (int i = 0; i < str.Length; i++)
            {
                var num = str[i];
                if (num == ',')
                {
                    if (!courses[NowCourse].IsEnable) courses[NowCourse].IsEnable = true;
                    NowInfo.MeasureCount++;
                    WChip.Add(new List<Chip>());
                    NowInfo.AddMeasure = true;
                }
                if (num >= '0' && num <= '9')
                {
                    if (FullLoad)
                    {
                        Chip chip = new Chip()
                        {
                            Time = NowInfo.Time,
                            Bpm = NowInfo.Bpm,
                            Scroll = NowInfo.Scroll,
                            Measure = NowInfo.Measure,
                            IsGogo = NowInfo.IsGogo,
                            ENote = (ENote)int.Parse(num.ToString()),
                            ERanNote = (ENote)int.Parse(num.ToString()),
                            ERoll = NowInfo.RollState,
                            CanShow = true,
                            Lyric = NowInfo.LyricText,
                            Sudden = NowInfo.Sudden
                        };

                        if (chip.ENote == ENote.Balloon || chip.ENote == ENote.RollStart || chip.ENote == ENote.ROLLStart || chip.ENote == ENote.Kusudama)
                        {
                            NowInfo.RollState = RollState(chip);
                            chip.ERoll = RollState(chip);
                            if (NowInfo.RollBegin == null)
                                NowInfo.RollBegin = chip;
                        }
                        if (chip.ENote == ENote.RollEnd)
                        {
                            NowInfo.RollState = ERoll.None;
                            chip.ERoll = ERoll.None;
                            if (NowInfo.RollBegin != null)
                            {
                                NowInfo.RollBegin.RollEnd = chip;
                                chip.RollEnd = chip;
                            }
                            NowInfo.RollBegin = null;
                        }

                        if (chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                            courses[NowCourse].TotalNotes++;

                        if (chip.ENote == ENote.Balloon || chip.ENote == ENote.Kusudama)
                        {
                            chip.Balloon = BalloonCount < courses[NowCourse].BALLOON.Count ? courses[NowCourse].BALLOON[BalloonCount] : 5;
                            BalloonCount++;
                        }

                        //if (chip.ENote != ENote.Space)
                        WChip[NowInfo.MeasureCount].Add(chip);
                        courses[NowCourse].ListChip.Add(chip);

                        int measure = NowInfo.MeasureCount < courses[NowCourse].ListMeasureCount.Count ? NowInfo.MeasureCount : courses[NowCourse].ListMeasureCount.Count - 1;
                        NowInfo.Time += 15000.0 / NowInfo.Bpm / NowInfo.Measure * (16.0 / courses[NowCourse].ListMeasureCount[measure]);
                    }
                    else
                    {
                        if (int.Parse(num.ToString()) >= (int)ENote.Don && int.Parse(num.ToString()) <= (int)ENote.KA)
                        {
                            courses[NowCourse].TotalNotes++;
                        }
                    }
                }
            }
        }

        public static void TimeRound(List<Chip> listchip, List<Bar> listbar, List<Command> listcom)
        {
            if (listchip.Count > 0)
            {
                foreach (Chip chip in listchip)
                {
                    chip.Time = Math.Round(chip.Time, 4, MidpointRounding.AwayFromZero);
                }
            }
            if (listbar.Count > 0)
            {
                foreach (Bar bar in listbar)
                {
                    bar.Time = Math.Round(bar.Time, 4, MidpointRounding.AwayFromZero);
                }
            }
            if (listcom.Count > 0)
            {
                foreach (Command com in listcom)
                {
                    com.Time = Math.Round(com.Time, 4, MidpointRounding.AwayFromZero);
                }
            }
        }
        public static void RollDoubledCheck(List<Chip> listchip)//仕方なく分ける
        {
            if (listchip.Count == 0) return;

            for (int i = listchip.Count - 1; i > 0; i--)
            {
                if (listchip[i].ENote == listchip[i - 1].ENote)
                {
                    if (listchip[i].ENote == ENote.Balloon || listchip[i].ENote == ENote.RollStart || listchip[i].ENote == ENote.ROLLStart || listchip[i].ENote == ENote.Kusudama)
                    {
                        listchip.RemoveAt(i);
                    }
                }
            }
        }
        public static void SetBalloon(Course courses, List<Chip> listchip)
        {
            if (listchip.Count == 0) return;

            int b = 0;
            for (int i = 0; i < listchip.Count; i++)
            {
                if (listchip[i].ENote == ENote.Balloon || listchip[i].ENote == ENote.Kusudama)
                {
                    listchip[i].Balloon = b < courses.BALLOON.Count ? courses.BALLOON[b] : 5;
                    b++;
                }
            }
        }

        public static void RandomizeChip(List<Chip> listchip, int random, bool mirror, int change)
        {
            if (listchip.Count == 0) return;

            foreach (Chip chip in listchip)
            {
                chip.ENote = chip.ERanNote;
            }

            if (change > 0)
            {
                for (int i = listchip.Count - 1; i > 0; i--)
                {
                    switch (change)
                    {
                        case 1:
                            if (listchip[i].ENote == ENote.Ka || listchip[i].ENote == ENote.KA)
                                listchip.RemoveAt(i);
                            break;
                        case 2:
                            if (listchip[i].ENote == ENote.Don || listchip[i].ENote == ENote.DON)
                                listchip.RemoveAt(i);
                            break;
                        case 3:
                            if (listchip[i].ENote == ENote.Ka)
                                listchip[i].ENote = ENote.Don;
                            if (listchip[i].ENote == ENote.KA)
                                listchip[i].ENote = ENote.DON;
                            break;
                        case 4:
                            if (listchip[i].ENote == ENote.Don)
                                listchip[i].ENote = ENote.Ka;
                            if (listchip[i].ENote == ENote.DON)
                                listchip[i].ENote = ENote.KA;
                            break;
                    }
                }
            }

            if (random > 0)
            {
                Random ran = new Random();
                for (int i = 0; i < listchip.Count; i++)
                {
                    int value = ran.Next(100);
                    if (value < random)
                    {
                        int half = ran.Next(100);
                        if (half > 49)
                        {
                            switch (listchip[i].ENote)
                            {
                                case ENote.Don:
                                    listchip[i].ENote = ENote.Ka;
                                    break;
                                case ENote.Ka:
                                    listchip[i].ENote = ENote.Don;
                                    break;
                                case ENote.DON:
                                    listchip[i].ENote = ENote.KA;
                                    break;
                                case ENote.KA:
                                    listchip[i].ENote = ENote.DON;
                                    break;
                            }
                        }
                    }
                }
            }
            if (mirror)
            {
                for (int i = 0; i < listchip.Count; i++)
                {
                    switch (listchip[i].ENote)
                    {
                        case ENote.Don:
                            listchip[i].ENote = ENote.Ka;
                            break;
                        case ENote.Ka:
                            listchip[i].ENote = ENote.Don;
                            break;
                        case ENote.DON:
                            listchip[i].ENote = ENote.KA;
                            break;
                        case ENote.KA:
                            listchip[i].ENote = ENote.DON;
                            break;
                    }
                }
            }
        }

        public static void DummyCheck(List<Chip> listchip, string tjapath, string songpath)
        {
            Sound sound = new Sound($"{Path.GetDirectoryName(tjapath)}/{songpath}");
            if (sound == null) return;
            for (int i = listchip.Count - 1; i > 0; i--)
            {
                Chip chip = listchip[i];
                
                if (chip.Time <= sound.Length) break;
                chip.IsDummy = true;
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
        public static ERoll RollState(Chip chip)
        {
            switch (chip.ENote)
            {
                case ENote.RollStart:
                    return ERoll.Roll;
                case ENote.ROLLStart:
                    return ERoll.ROLL;
                case ENote.Balloon:
                    return ERoll.Balloon;
                case ENote.Kusudama:
                    return ERoll.Kusudama;
                default:
                    return ERoll.None;
            }
        }
    }

    public enum ECourse
    {
        Easy,
        Normal,
        Hard,
        Oni,
        Edit,
        Tower,
        Dan
    }

    public enum EScroll
    {
        Normal,
        BMSCROLL,
        HBSCROLL
    }

    public enum EChange
    {
        None,
        RedOnly,
        BlueOnly,
        AllRed,
        AllBlue,
    }
}
