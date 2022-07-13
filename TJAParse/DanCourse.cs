using System.Collections.Generic;
using System.Linq;
using System.IO;
using SeaDrop;

namespace TJAParse
{
    public class DanCourse
    {
        public string Title, SubTitle;
        public int Level = 0, TotalNotes;
        public List<DanSong> Courses;
        public GaugeExam Gauge;
        public List<Exam> Exams;

        public string Path;
        public bool isEnable;
        public static int SongNumber;

        public DanCourse(){}
        public DanCourse(string path)
        {
            Path = path;
            if (File.Exists(path))
            {
                isEnable = true;
                SongNumber = 0;
                Courses = new List<DanSong>();
                Exams = new List<Exam>();

                ConfigIni file = new ConfigIni();
                file.Load(path);
                foreach (string str in file.TextList)
                {
                    if (str.Length > 0)
                    {
                        string[] split = str.Split(':');
                        if (split.Length >= 2)
                        {
                            string allsplit = split[1];
                            if (split.Length > 2)
                            {
                                for (int i = 2; i < split.Length; i++)
                                {
                                    allsplit = allsplit + ":" + split[i];
                                }
                            }
                            switch (split[0].ToUpper())
                            {
                                case "TITLE":
                                    Title = allsplit;
                                    break;
                                case "SUBTITLE":
                                    SubTitle = allsplit;
                                    break;
                                case "LEVEL":
                                    Level = int.Parse(split[1]);
                                    break;
                                case "SONG":
                                    string[] split2 = allsplit.Split(',');
                                    Courses.Add(new DanSong() { Path = split2[0].Contains(":") ? split2[0] : $@"{System.IO.Path.GetDirectoryName(path)}\{split2[0]}", Course = int.Parse(split2[1]) });
                                    break;
                                case "GAUGE":
                                    split2 = split[1].Split(',');
                                    Gauge = new GaugeExam()
                                    {
                                        RedNumber = int.Parse(split2[0]),
                                        GoldNumber = int.Parse(split2[1]),
                                        GaugeType = SetGaugeType(split2[2]),
                                        Total = int.Parse(split2[3]),
                                    };
                                    break;
                                case "EXAM":
                                    split2 = split[1].Split(',');
                                    Exams.Add(new Exam()
                                    {
                                        Name = SetExamName(split2[0]),
                                        RedNumber = SetValue(split2[1]),
                                        GoldNumber = SetValue(split2[2]),
                                        RedNumberSong = SetValues(split2[1]),
                                        GoldNumberSong = SetValues(split2[2]),
                                        isLess = SetML(split2[3]),
                                        Success = ESuccess.Gold
                                    });
                                    break;
                            }
                        }
                    }
                }

                TotalNotes = 0;
                foreach (DanSong song in Courses)
                {
                    TJA tja = new TJA(song.Path, 1.0, 0, false, 0);
                    song.Title = tja.Header.TITLE;
                    song.Level = tja.Courses[song.Course].LEVEL;
                    TotalNotes += tja.Courses[song.Course].TotalNotes;
                }
            }
        }

        public static int SetGaugeType(string str)
        {
            switch (str.ToLower())
            {
                case "0":
                case "normal":
                default:
                    return 0;
                case "1":
                case "hard":
                    return 1;
                case "2":
                case "exhard":
                    return 2;
            }
        }

        public static EExam SetExamName(string str)
        {
            switch (str.ToLower())
            {
                case "0":
                case "p":
                case "perfect":
                default:
                    return EExam.Perfect;
                case "1":
                case "gr":
                case "great":
                    return EExam.Great;
                case "2":
                case "g":
                case "jg":
                case "good":
                    return EExam.Good;
                case "3":
                case "b":
                case "bad":
                    return EExam.Bad;
                case "4":
                case "pr":
                case "poor":
                    return EExam.Poor;
                case "5":
                case "pg":
                case "perfectgreat":
                case "jp":
                case "l":
                case "light":
                    return EExam.Light;
                case "6":
                case "bp":
                case "badpoor":
                case "jb":
                case "m":
                case "miss":
                    return EExam.Miss;
                case "7":
                case "e":
                case "score":
                    return EExam.Score;
                case "8":
                case "s":
                case "oldscore":
                    return EExam.OldScore;
                case "9":
                case "r":
                case "roll":
                    return EExam.Roll;
                case "10":
                case "h":
                case "hit":
                    return EExam.Hit;
                case "11":
                case "c":
                case "combo":
                    return EExam.Combo;
            }
        }

        public static int SetValue(string str)
        {
            if (str.Contains('.')) return 0;
            else return int.Parse(str);
        }
        public static List<int> SetValues(string str)
        {
            if (str.Contains('.'))
            {
                string[] arr = str.Split('.');
                List<int> result = new List<int>();
                for (int i = 0; i < arr.Length; i++)
                {
                    result.Add(int.Parse(arr[i]));
                }
                return result;
            }
            else return null;
        }

        public static bool SetML(string str)
        {
            switch (str.ToLower())
            {
                case "0":
                case "m":
                case "more":
                default:
                    return false;
                case "1":
                case "l":
                case "less":
                    return true;
            }
        }
    }

    public class DanSong
    {
        public string Path;
        public int Course;
        public string Title;
        public int Level;
    }

    public class Exam
    {
        public EExam Name;
        public int RedNumber;
        public int GoldNumber;
        public List<int> RedNumberSong;
        public List<int> GoldNumberSong;
        public bool isLess;
        public ESuccess Success;
    }
    public class GaugeExam
    {
        public int GaugeType;//0:Normal,1:Hard,2:EXHard
        public double RedNumber;
        public double GoldNumber;
        public double Total;
    }

    public enum EExam
    {
        Perfect,
        Great,
        Good,
        Bad,
        Poor,
        Light,//Perfect+Great
        Miss,//Bad+Poor
        Score,
        OldScore,
        Roll,
        Hit,
        Combo
    }

    public enum ESuccess
    {
        Failed,
        None,
        Red,
        Gold
    }
}
