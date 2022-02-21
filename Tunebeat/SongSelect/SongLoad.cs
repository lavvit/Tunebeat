using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.SongSelect
{
    public class SongLoad
    {
        public static void Init()
        {
            SongData = new List<SongData>();
            SongList = new List<SongData>();
            FolderData = new List<string>();
            if (!string.IsNullOrEmpty(SongSelect.NowPath) && FolderFloor != 0)
            {
                Load(SongData, SongSelect.NowPath);
            }
            else
            {
                FolderFloor = 0;
                Load(SongData, PlayData.Data.PlayFolder);
            }
        }
        public static void Dispose()
        {
            SongData = null;
            FolderData = null;
        }
        public static bool DoubledCheck(string path, List<string> folder, int limit = 0)//仕方なく分ける2
        {
            if (folder.Count == 0) return false;
            for (int i = 0; i < folder.Count; i++)
            {
                if (path.Contains(folder[i])) return true;
            }
            return false;
        }
        public static void Load(List<SongData> data, List<string> allpath)
        {
            Course[] r = new Course[5];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new Course();
            }
            bool[] t = new bool[5];
            SongData New = new SongData()
            {
                Path = "",
                Title = "新しく譜面を作成",
                FontColor = Color.White,
                BackColor = Color.FromArgb((int)GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.New,
                Enable = t,
            };
            data.Add(New);

            SongData random = new SongData()
            {
                Path = "",
                Title = "ランダムに曲を選ぶ",
                FontColor = Color.White,
                BackColor = Color.FromArgb((int)GetColor(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.Random,
                Enable = t,
            };
            data.Add(random);

            List<string> list = new List<string>();
            foreach (string path in allpath)
            {
                string ppath = path.Replace("/", "\\");
                foreach (string directory in Directory.EnumerateDirectories(ppath, "*", SearchOption.AllDirectories))
                {
                    if (Directory.EnumerateFiles(directory, "genre.ini", SearchOption.TopDirectoryOnly).ToList().Count > 0 || Directory.EnumerateFiles(directory, "*.tja", SearchOption.TopDirectoryOnly).ToList().Count > 0)
                        list.Add(directory);

                    foreach (string item in Directory.EnumerateFiles(directory, "*.tja", SearchOption.TopDirectoryOnly))
                    {
                        TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                        bool[] enable = new bool[5];
                        for (int i = 0; i < 5; i++)
                        {
                            enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                        }
                        SongData songdata = new SongData()
                        {
                            Path = item,
                            Title = parse.Header.TITLE,
                            Time = File.GetLastWriteTime(item),
                            Header = parse.Header,
                            Course = parse.Courses,
                            Type = EType.Score,
                            Enable = enable,
                        };
                        SongList.Add(songdata);
                    }
                }
            }

            foreach (string directory in list)
            {
                foreach (string item in Directory.EnumerateFiles(directory, "genre.ini", SearchOption.AllDirectories))
                {
                    if (DoubledCheck(Path.GetDirectoryName(item), FolderData)) break;
                    Folder folder = new Folder(item);
                    int p;
                    Course[] c = new Course[5];
                    for (int i = 0; i < c.Length; i++)
                    {
                        c[i] = new Course();
                    }
                    SongData songdata = new SongData()
                    {
                        Path = Path.GetDirectoryName(item),
                        Title = !string.IsNullOrEmpty(folder.Name) ? folder.Name : Path.GetFileName(Path.GetDirectoryName(item)),
                        FontColor = !string.IsNullOrEmpty(folder.FontColor) && int.TryParse(folder.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(folder.FontColor.Substring(0, 7)) : Color.White,
                        BackColor = !string.IsNullOrEmpty(folder.BackColor) && int.TryParse(folder.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(folder.BackColor.Substring(0, 7)) : Color.Gray,
                        Course = c,
                        Type = EType.Folder,
                        Enable = t,
                    };
                    data.Add(songdata);
                    FolderData.Add(songdata.Path);
                }
            }
            foreach (string directory in list)
            {
                foreach (string item in Directory.EnumerateFiles(directory, "*.tja", SearchOption.AllDirectories))
                {
                    if (DoubledCheck(item, FolderData)) break;
                    Folder folder;
                    foreach (string dir in FolderData)
                    {
                        if (item.Contains(dir)) folder = new Folder(dir);
                    }
                    TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                    List<string>[] slist = new List<string>[5];
                    for (int i = 0; i < 5; i++)
                    {
                        slist[i] = new List<string>();
                    }
                    foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(item), "*.tbr", SearchOption.TopDirectoryOnly))
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (parse != null && Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.", "").StartsWith($"{(ECourse)i}"))
                            {
                                slist[i].Add(Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.{(ECourse)i}.", ""));
                            }
                        }
                    }
                    bool[] enable = new bool[5];
                    int[] lamp = new int[5], notes = new int[5], score = new int[5];
                    double[] rate = new double[5];
                    for (int i = 0; i < 5; i++)
                    {
                        enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                        notes[i] = parse.Courses != null ? parse.Courses[i].TotalNotes : 0;
                        lamp[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].ClearLamp : 0;
                        score[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].Score : 0;
                        rate[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? (1.01 * new BestScore(item).ScoreData.Score[i].Perfect + 1.0 * new BestScore(item).ScoreData.Score[i].Great + 0.5 * new BestScore(item).ScoreData.Score[i].Good) / notes[i] * 100000.0 : 0;
                    }
                    SongData songdata = new SongData()
                    {
                        Path = item,
                        Title = parse.Header.TITLE,
                        Time = File.GetLastWriteTime(item),
                        Header = parse.Header,
                        Course = parse.Courses,
                        FontColor = Color.White,
                        BackColor = Color.Black,
                        Type = EType.Score,
                        Score = new BestScore(item).ScoreData,
                        ScoreList = slist,
                        Lamp = lamp,
                        Enable = enable,
                        EXScore = score,
                        Notes = notes,
                        Rate = rate,
                    };
                    data.Add(songdata);
                }
            }
            foreach (string path in allpath)
            {
                string ppath = path.Replace("/", "\\");
                foreach (string item in Directory.EnumerateFiles(ppath, "*.tja", SearchOption.TopDirectoryOnly))//root
                {
                    if (DoubledCheck(item, FolderData)) break;
                    TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                    List<string>[] slist = new List<string>[5];
                    for (int i = 0; i < 5; i++)
                    {
                        slist[i] = new List<string>();
                    }
                    foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(item), "*.tbr", SearchOption.TopDirectoryOnly))
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (parse != null && Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.", "").StartsWith($"{(ECourse)i}"))
                            {
                                slist[i].Add(Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.{(ECourse)i}.", ""));
                            }
                        }
                    }
                    bool[] enable = new bool[5];
                    int[] lamp = new int[5], notes = new int[5], score = new int[5];
                    double[] rate = new double[5];
                    for (int i = 0; i < 5; i++)
                    {
                        enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                        notes[i] = parse.Courses != null ? parse.Courses[i].TotalNotes : 0;
                        lamp[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].ClearLamp : 0;
                        score[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].Score : 0;
                        rate[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? (1.01 * new BestScore(item).ScoreData.Score[i].Perfect + 1.0 * new BestScore(item).ScoreData.Score[i].Great + 0.5 * new BestScore(item).ScoreData.Score[i].Good) / (double)notes[i] * 100000.0 : 0;
                    }
                    SongData songdata = new SongData()
                    {
                        Path = item,
                        Title = parse.Header.TITLE,
                        Time = File.GetLastWriteTime(item),
                        Header = parse.Header,
                        Course = parse.Courses,
                        FontColor = Color.White,
                        BackColor = Color.Black,
                        Type = EType.Score,
                        Score = new BestScore(item).ScoreData,
                        ScoreList = slist,
                        Lamp = lamp,
                        Enable = enable,
                        EXScore = score,
                        Notes = notes,
                        Rate = rate,
                    };
                    data.Add(songdata);
                    SongList.Add(songdata);
                }
            }
            int s = 0;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Type == EType.Score)
                    s++;
            }
            if (s > 0)
            {
                data.Sort(SortSystem[(int)NowSort]);
                SongList.Sort(SortSystem[(int)NowSort]);
            }

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Prev = data[(i + (data.Count - 1)) % data.Count];
                data[i].Next = data[(i + 1) % data.Count];
            }
        }
        public static void Load(List<SongData> data, string path)//folder
        {
            if (FolderFloor == 0)
            {
                Init();
                data.Sort(SortSystem[(int)NowSort]);
                SongList.Sort(SortSystem[(int)NowSort]);
                return;
            }
            Folder fol = new Folder(path + @"\genre.ini");
            int p = 0;
            Course[] c = new Course[5];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = new Course();
            }
            bool[] t = new bool[5];
            SongData root = new SongData()
            {
                Path = Path.GetDirectoryName(path),
                Title = !string.IsNullOrEmpty(fol.Name) ? fol.Name : Path.GetFileName(path),
                Course = c,
                FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Gray,
                Type = EType.Back,
                Enable = t,
            };
            data.Add(root);

            Course[] r = new Course[5];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new Course();
            }
            SongData random = new SongData()
            {
                Path = path,
                Title = "ランダムに曲を選ぶ",
                FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Gray,
                Course = r,
                Type = EType.Random,
                Enable = t,
            };
            data.Add(random);

            List<string> list = new List<string>();
            string ppath = path.Replace("/", "\\");
            foreach (string directory in Directory.EnumerateDirectories(ppath, "*", SearchOption.AllDirectories))
            {
                if (Directory.EnumerateFiles(directory, "genre.ini", SearchOption.TopDirectoryOnly).ToList().Count > 0 || Directory.EnumerateFiles(directory, "*.tja", SearchOption.TopDirectoryOnly).ToList().Count > 0)
                    list.Add(directory);
                foreach (string item in Directory.EnumerateFiles(directory, "*.tja", SearchOption.TopDirectoryOnly))
                {
                    TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                    bool[] enable = new bool[5];
                    for (int i = 0; i < 5; i++)
                    {
                        enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                    }
                    SongData songdata = new SongData()
                    {
                        Path = item,
                        Title = parse.Header.TITLE,
                        Time = File.GetLastWriteTime(item),
                        Header = parse.Header,
                        Course = parse.Courses,
                        Type = EType.Score,
                        Enable = enable,
                    };
                    SongList.Add(songdata);
                }
            }
            foreach (string directory in list)
            {
                foreach (string item in Directory.EnumerateFiles(directory, "genre.ini", SearchOption.AllDirectories))
                {
                    if (DoubledCheck(Path.GetDirectoryName(item), FolderData)) break;
                    Folder folder = new Folder(item);
                    SongData songdata = new SongData()
                    {
                        Path = Path.GetDirectoryName(item),
                        Title = !string.IsNullOrEmpty(folder.Name) ? folder.Name : Path.GetFileName(Path.GetDirectoryName(item)),
                        Course = c,
                        FontColor = !string.IsNullOrEmpty(folder.FontColor) && int.TryParse(folder.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(folder.FontColor.Substring(0, 7)) : Color.White,
                        BackColor = !string.IsNullOrEmpty(folder.BackColor) && int.TryParse(folder.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(folder.BackColor.Substring(0, 7)) : Color.Gray,
                        Type = EType.Folder,
                        Enable = t,
                    };
                    if (DoubledCheck(songdata.Path, FolderData, 1)) break;
                    data.Add(songdata);
                }
            }
            foreach (string directory in list)
            {
                foreach (string item in Directory.EnumerateFiles(directory, "*.tja", SearchOption.AllDirectories))
                {
                    if (DoubledCheck(item, FolderData)) break;
                    TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                    List<string>[] slist = new List<string>[5];
                    for (int i = 0; i < 5; i++)
                    {
                        slist[i] = new List<string>();
                    }
                    foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(item), "*.tbr", SearchOption.TopDirectoryOnly))
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (parse != null && Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.", "").StartsWith($"{(ECourse)i}"))
                            {
                                slist[i].Add(Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.{(ECourse)i}.", ""));
                            }
                        }
                    }
                    bool[] enable = new bool[5];
                    int[] lamp = new int[5], notes = new int[5], score = new int[5];
                    double[] rate = new double[5];
                    for (int i = 0; i < 5; i++)
                    {
                        enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                        notes[i] = parse.Courses != null ? parse.Courses[i].TotalNotes : 0;
                        lamp[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].ClearLamp : 0;
                        score[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].Score : 0;
                        rate[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? (1.01 * new BestScore(item).ScoreData.Score[i].Perfect + 1.0 * new BestScore(item).ScoreData.Score[i].Great + 0.5 * new BestScore(item).ScoreData.Score[i].Good) / (double)notes[i] * 100000.0 : 0;
                    }
                    SongData songdata = new SongData()
                    {
                        Path = item,
                        Title = parse.Header.TITLE,
                        Time = File.GetLastWriteTime(item),
                        Header = parse.Header,
                        Course = parse.Courses,
                        FontColor = root.FontColor,
                        BackColor = root.BackColor,
                        Type = EType.Score,
                        Score = new BestScore(item).ScoreData,
                        ScoreList = slist,
                        Lamp = lamp,
                        Enable = enable,
                        EXScore = score,
                        Notes = notes,
                        Rate = rate,
                    };
                    data.Add(songdata);
                }
            }
            foreach (string item in Directory.EnumerateFiles(ppath, "*.tja", SearchOption.TopDirectoryOnly))//root
            {
                if (DoubledCheck(item, FolderData)) break;
                TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
                List<string>[] slist = new List<string>[5];
                for (int i = 0; i < 5; i++)
                {
                    slist[i] = new List<string>();
                }
                foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(item), "*.tbr", SearchOption.TopDirectoryOnly))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (parse != null && Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.", "").StartsWith($"{(ECourse)i}"))
                        {
                            slist[i].Add(Path.GetFileNameWithoutExtension(file).Replace($"{parse.Header.TITLE}.{(ECourse)i}.", ""));
                        }
                    }
                }
                bool[] enable = new bool[5];
                int[] lamp = new int[5], notes = new int[5], score = new int[5];
                double[] rate = new double[5];
                for (int i = 0; i < 5; i++)
                {
                    enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                    notes[i] = parse.Courses != null ? parse.Courses[i].TotalNotes : 0;
                    lamp[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].ClearLamp : 0;
                    score[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? new BestScore(item).ScoreData.Score[i].Score : 0;
                    rate[i] = new BestScore(item).ScoreData != null && new BestScore(item).ScoreData.Score[i].Score > 0 ? (1.01 * new BestScore(item).ScoreData.Score[i].Perfect + 1.0 * new BestScore(item).ScoreData.Score[i].Great + 0.5 * new BestScore(item).ScoreData.Score[i].Good) / (double)notes[i] * 100000.0 : 0;
                }
                SongData songdata = new SongData()
                {
                    Path = item,
                    Title = parse.Header.TITLE,
                    Time = File.GetLastWriteTime(item),
                    Header = parse.Header,
                    Course = parse.Courses,
                    FontColor = root.FontColor,
                    BackColor = root.BackColor,
                    Type = EType.Score,
                    Score = new BestScore(item).ScoreData,
                    ScoreList = slist,
                    Lamp = lamp,
                    Enable = enable,
                    EXScore = score,
                    Notes = notes,
                    Rate = rate,
                };
                data.Add(songdata);
            }
            data.Sort(SortSystem[(int)NowSort]);
            SongList.Sort(SortSystem[(int)NowSort]);

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Prev = data[(i + (data.Count - 1)) % data.Count];
                data[i].Next = data[(i + 1) % data.Count];
            }
        }

        public static void Sort(List<SongData> data, ESort comp)
        {
            if (data == null) return;
            data.Sort(SortSystem[(int)comp]);

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Prev = data[(i + (data.Count - 1)) % data.Count];
                data[i].Next = data[(i + 1) % data.Count];
            }
        }

        public static int FolderFloor;
        public static ESort NowSort;
        public static List<SongData> SongData { get; set; }
        public static List<SongData> SongList { get; set; }
        public static List<string> FolderData { get; set; }

        public static Comparison<SongData>[] SortSystem = new Comparison<SongData>[15]
            {
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    return resulta != 0 ? resulta : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    return resulta != 0 ? resulta : a.Title.CompareTo(b.Title); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    return resulta != 0 ? resulta : b.Title.CompareTo(a.Title); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Course[SongSelect.EnableCourse(a.Course, 0)].LEVEL - b.Course[SongSelect.EnableCourse(b.Course, 0)].LEVEL;
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Course[SongSelect.EnableCourse(b.Course, 0)].LEVEL - a.Course[SongSelect.EnableCourse(a.Course, 0)].LEVEL;
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Time.CompareTo(a.Time);
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Time.CompareTo(b.Time);
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Lamp[SongSelect.EnableCourse(b.Course, 0)] - a.Lamp[SongSelect.EnableCourse(a.Course, 0)];
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Lamp[SongSelect.EnableCourse(a.Course, 0)] - b.Lamp[SongSelect.EnableCourse(b.Course, 0)];
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Notes[SongSelect.EnableCourse(b.Course, 0)] - a.Notes[SongSelect.EnableCourse(a.Course, 0)];
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Notes[SongSelect.EnableCourse(a.Course, 0)] - b.Notes[SongSelect.EnableCourse(b.Course, 0)];
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : b.EXScore[SongSelect.EnableCourse(b.Course, 0)] - a.EXScore[SongSelect.EnableCourse(a.Course, 0)];
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    int resultb = resulta != 0 ? resulta : a.EXScore[SongSelect.EnableCourse(a.Course, 0)] - b.EXScore[SongSelect.EnableCourse(b.Course, 0)];
                    return resultb != 0 ? resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    double resultb = resulta != 0 ? resulta : b.Rate[SongSelect.EnableCourse(b.Course, 0)] - a.Rate[SongSelect.EnableCourse(a.Course, 0)];
                    return resultb != 0 ? (int)resultb : a.Path.CompareTo(b.Path); },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b.Course, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a.Course, 0)]);
                    double resultb = resulta != 0 ? resulta : a.Rate[SongSelect.EnableCourse(a.Course, 0)] - b.Rate[SongSelect.EnableCourse(b.Course, 0)];
                    return resultb != 0 ? (int)resultb : a.Path.CompareTo(b.Path); },
            };
    }

    public class SongData
    {
        public string Title;
        public string Path;
        public DateTime Time;
        public Header Header;
        public Course[] Course = new Course[5];
        public EType Type;
        public SongData Prev;
        public SongData Next;
        public ScoreData Score;
        public bool[] Enable = new bool[5];
        public int[] Lamp = new int[5];
        public int[] Notes = new int[5];
        public int[] EXScore = new int[5];
        public double[] Rate = new double[5];
        public List<string>[] ScoreList = new List<string>[5];
        public Color FontColor;
        public Color BackColor;
    }

    public enum EType
    {
        Back,
        New,
        Folder,
        Random,
        Score
    }

    public enum ESort
    {
        None,
        Title_ABC,
        Title_ZYX,
        Level_123,
        Level_987,
        Time_New,
        Time_Old,
        Clear_High,
        Clear_Low,
        Notes_High,
        Notes_Low,
        Score_High,
        Score_Low,
        Rate_High,
        Rate_Low
    }

    public class Folder
    {
        public string Name, BackColor, FontColor;
        public Folder(string path)
        {
            Name = path;
            if (File.Exists(path))
            {
                string str;
                using (StreamReader reader = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
                {
                    str = reader.ReadToEnd();
                }
                Load(str);
            }
        }

        public void Load(string strAll)
        {
            string[] delimiter = { "\n" };
            string[] strSingleLine = strAll.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in strSingleLine)
            {
                string str = s.Replace('\t', ' ').TrimStart(new char[] { '\t', ' ' });
                if ((str.Length != 0) && (str[0] != ';'))
                {
                    string str3;
                    string str4;
                    string[] strArray = str.Split(new char[] { '=' });
                    if (strArray.Length == 2)
                    {
                        str3 = strArray[0].Trim();
                        str4 = strArray[1].Trim();
                        if (str3.Equals("GenreName"))
                        {
                            Name = str4;
                        }
                        else if (str3.Equals("GenreColor"))
                        {
                            BackColor = str4;
                        }
                        else if (str3.Equals("FontColor"))
                        {
                            FontColor = str4;
                        }
                        continue;
                    }
                }
            }
        }
    }
}
