using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.SongSelect
{
    public class SongLoad
    {
        public static void Init()
        {
            SongData = new List<SongData>();
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
            List<string> list = new List<string>();
            foreach (string path in allpath)
            {
                string ppath = path.Replace("/", "\\");
                foreach (string directory in Directory.EnumerateDirectories(ppath, "*", SearchOption.AllDirectories))
                {
                    if (Directory.EnumerateFiles(directory, "genre.ini", SearchOption.TopDirectoryOnly).ToList().Count > 0 || Directory.EnumerateFiles(directory, "*.tja", SearchOption.TopDirectoryOnly).ToList().Count > 0)
                        list.Add(directory);
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
                        Title = folder.Name,
                        FontColor = ColorTranslator.FromHtml(folder.FontColor),
                        BackColor = ColorTranslator.FromHtml(folder.BackColor),
                        Type = EType.Folder,
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
                    Folder folder = new Folder(Path.GetDirectoryName(item) + @"\genre.ini");
                    foreach (string dir in FolderData)
                    {
                        if (item.Contains(dir)) folder = new Folder(dir);
                    }
                    TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
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
                    };
                    data.Add(songdata);
                }
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
                return;
            }
            Folder fol = new Folder(path + @"\genre.ini");
            SongData root = new SongData()
            {
                Path = Path.GetDirectoryName(path),
                Title = fol.Name,
                FontColor = ColorTranslator.FromHtml(fol.FontColor),
                BackColor = ColorTranslator.FromHtml(fol.BackColor),
                Type = EType.Back,
            };
            data.Add(root);

            List<string> list = new List<string>();
            string ppath = path.Replace("/", "\\");
            foreach (string directory in Directory.EnumerateDirectories(ppath, "*", SearchOption.AllDirectories))
            {
                if (Directory.EnumerateFiles(directory, "genre.ini", SearchOption.TopDirectoryOnly).ToList().Count > 0 || Directory.EnumerateFiles(directory, "*.tja", SearchOption.TopDirectoryOnly).ToList().Count > 0)
                    list.Add(directory);
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
                        Title = folder.Name,
                        FontColor = ColorTranslator.FromHtml(folder.FontColor),
                        BackColor = ColorTranslator.FromHtml(folder.BackColor),
                        Type = EType.Folder,
                    };
                    if (DoubledCheck(songdata.Path, FolderData, 1)) break;
                    data.Add(songdata);
                    FolderData.Add(songdata.Path);
                }
            }
            foreach (string directory in list)
            {
                foreach (string item in Directory.EnumerateFiles(directory, "*.tja", SearchOption.AllDirectories))
                {
                    if (DoubledCheck(item, FolderData)) break;
                    TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
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
                    };
                    data.Add(songdata);
                }
            }
            foreach (string item in Directory.EnumerateFiles(ppath, "*.tja", SearchOption.TopDirectoryOnly))//root
            {
                if (DoubledCheck(item, FolderData)) break;
                TJAParse.TJAParse parse = new TJAParse.TJAParse(item);
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
                };
                data.Add(songdata);
            }

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Prev = data[(i + (data.Count - 1)) % data.Count];
                data[i].Next = data[(i + 1) % data.Count];
            }
        }

        public static void Sort(List<SongData> data, Comparison<SongData> comp)
        {
            data.Sort(comp);

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Prev = data[(i + (data.Count - 1)) % data.Count];
                data[i].Next = data[(i + 1) % data.Count];
            }
        }

        public static int FolderFloor;
        public static List<SongData> SongData { get; set; }
        public static List<string> FolderData { get; set; }
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
        public Color FontColor;
        public Color BackColor;
    }

    public enum EType
    {
        Back,
        Folder,
        Random,
        Score
    }

    public class Folder
    {
        public string Name, BackColor, FontColor;
        public Folder(string path)
        {
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
