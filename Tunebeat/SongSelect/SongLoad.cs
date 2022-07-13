using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using SeaDrop;
using TJAParse;
using BMSParse;

namespace Tunebeat
{
    public class SongLoad
    {
        public static void Init()
        {
            SongSelect.StopPreview();
            SongSelect.Cursor = 0;
            SongData.FolderFloor = 0;
            SongData.AllSong = new List<Song>();
            SongData.FolderSong = new List<Song>();
            SongData.AllFolder = new List<Folder>();
            SongData.FolderPath = new List<string>();
            SongData.Ini = new List<string>();
            foreach (string path in PlayData.Data.PlayFolder)
            {
                Title.LoadPath = path;
                LoadFolderPath(SongData.FolderPath, path);
                LoadInt(SongData.Ini, path);
                LoadSong(SongData.AllSong, path);
                LoadFolder(SongData.AllFolder, path);
            }
            Sort(SongData.AllSong, NowSort);
            if (PlayData.Data.PlayList) SongData.FolderSong = SongData.AllSong;

            SongData.Song = new List<Song>();
            RootSet(SongData.Song);
            foreach (string path in PlayData.Data.PlayFolder)
            {
                LoadSong(SongData.Song, SongData.AllSong, path);
                LoadFolder(SongData.Song, SongData.AllFolder, path);
            }
            foreach (string path in SongSelect.AddSongs)
            {
                AddSong(SongData.Song, path);
            }
            Sort(SongData.Song, NowSort);

            SongData.NowSong = SongData.Song[0];
        }
        public static void Load(string path)
        {
            SongData.Song = new List<Song>();
            SongData.FolderSong = new List<Song>();
            if (SongData.FolderFloor == 0)
            {
                RootSet(SongData.Song);
                foreach (string ppath in PlayData.Data.PlayFolder)
                {
                    LoadSong(SongData.Song, SongData.AllSong, ppath);
                    LoadFolder(SongData.Song, SongData.AllFolder, ppath);
                }
                foreach (string ppath in SongSelect.AddSongs)
                {
                    AddSong(SongData.Song, ppath);
                }
                if (PlayData.Data.PlayList) SongData.FolderSong = SongData.AllSong;
            }
            else
            {
                FolderSet(SongData.Song, path);
                LoadSong(SongData.Song, SongData.AllSong, path);
                LoadFolder(SongData.Song, SongData.AllFolder, path);
                if (PlayData.Data.PlayList) LoadSong(SongData.FolderSong, path);
            }
            Sort(SongData.Song, NowSort);
            if (PlayData.Data.PlayList) Sort(SongData.FolderSong, NowSort);
        }
        public static void AllLoad(bool isDifficulty)
        {
            SongData.Song = new List<Song>();
            SongData.FolderSong = new List<Song>();
            FolderSet(SongData.Song, isDifficulty);
            foreach (string path in PlayData.Data.PlayFolder)
            {
                LoadSong(SongData.Song, path, isDifficulty);
                if (PlayData.Data.PlayList) LoadSong(SongData.FolderSong, path, isDifficulty);
            }
            Sort(SongData.Song, NowSort);
        }

        public static void LoadFolderPath(List<string> data, string path)
        {
            data.Add(path);
            foreach (string directory in Directory.EnumerateDirectories(path.Replace("/", "\\"), "*", SearchOption.AllDirectories))
            {
                data.Add(directory);
            }
        }
        public static void AddFolder(List<Folder> data, string path)
        {
            Folder folder = new Folder(path);
            data.Add(folder);
        }
        public static void AddFolder(List<Song> data, Folder folder)
        {
            int p;
            Course[] c = new Course[5];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = new Course();
            }
            bool[] t = new bool[5];
            Song songdata = new Song()
            {
                Path = folder.Path,
                Folder = folder.Directory,
                Title = !string.IsNullOrEmpty(folder.Name) ? folder.Name : Path.GetFileName(folder.Path),
                FontColor = !string.IsNullOrEmpty(folder.FontColor) && int.TryParse(folder.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(folder.FontColor.Substring(0, 7)) : Color.White,
                BackColor = !string.IsNullOrEmpty(folder.BackColor) && int.TryParse(folder.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(folder.BackColor.Substring(0, 7)) : Color.Black,
                Course = c,
                Type = EType.Folder,
                Enable = t,
            };
            data.Add(songdata);
        }
        public static void LoadFolder(List<Folder> data, string path)
        {
            foreach (string directory in Directory.EnumerateDirectories(path.Replace("/", "\\"), "*", SearchOption.AllDirectories))
            {
                if (File.Exists(directory + @"\genre.ini")) AddFolder(data, directory);
            }
        }
        public static void LoadFolder(List<Song> data, List<Folder> list, string path)
        {
            foreach (Folder folder in list)
            {
                if (folder.Directory == path)
                {
                    AddFolder(data, folder);
                }
            }
        }
        public static void LoadInt(List<string> data, string path)
        {
            data.Add(path);
            foreach (string directory in Directory.EnumerateDirectories(path.Replace("/", "\\"), "*", SearchOption.AllDirectories))
            {
                if (File.Exists(directory + @"\genre.ini")) data.Add(directory);
            }
        }
        public static void RootSet(List<Song> data)
        {
            Course[] r = new Course[5];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new Course();
            }
            bool[] t = new bool[5];
            Song New = new Song()
            {
                Path = "",
                Folder = "",
                Title = "新しく譜面を作成",
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.New,
                Enable = t,
            };
            data.Add(New);

            Song random = new Song()
            {
                Path = "",
                Folder = "",
                Title = "ランダムに曲を選ぶ",
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.Random,
                Enable = t,
            };
            data.Add(random);
            Song randomd = new Song()
            {
                Path = "",
                Folder = "",
                Title = "ランダム段位",
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.Random_Dan,
                Enable = t,
            };
            data.Add(randomd);

            Song all = new Song()
            {
                Path = "AllSongs",
                Folder = "",
                Title = "全譜面",
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.AllSongs,
                Enable = t,
            };
            data.Add(all);

            Song allD = new Song()
            {
                Path = "AllDifficulty",
                Folder = "",
                Title = "全難易度",
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.AllDifficulty,
                Enable = t,
            };
            data.Add(allD);
        }
        public static void FolderSet(List<Song> data, bool isDifficulty)
        {
            Course[] r = new Course[5];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new Course();
            }
            bool[] t = new bool[5];
            Song root = new Song()
            {
                Path = "",
                Folder = "",
                Title = "戻る",
                Course = r,
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Type = EType.Back,
                Enable = t,
            };
            data.Add(root);

            Song random = new Song()
            {
                Path = "",
                Folder = "",
                Title = "ランダムに曲を選ぶ",
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.Random,
                Enable = t,
                DisplayDif = isDifficulty ? 1 : 0
            };
            data.Add(random);
            Song randomd = new Song()
            {
                Path = "",
                Folder = "",
                Title = "ランダム段位",
                FontColor = Color.White,
                BackColor = Color.FromArgb(Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2])),
                Course = r,
                Type = EType.Random_Dan,
                Enable = t,
            };
            data.Add(randomd);
        }
        public static void FolderSet(List<Song> data, string path)
        {
            Folder fol = new Folder(path);
            int p;
            Course[] r = new Course[5];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new Course();
            }
            bool[] t = new bool[5];
            Song root = new Song()
            {
                Path = fol.Path,
                Folder = fol.Directory,
                Title = !string.IsNullOrEmpty(fol.Name) ? fol.Name : Path.GetFileName(fol.Path),
                Course = r,
                FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                Type = EType.Back,
                Enable = t,
            };
            data.Add(root);

            Song random = new Song()
            {
                Path = "",
                Folder = "",
                Title = "ランダムに曲を選ぶ",
                FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                Course = r,
                Type = EType.Random,
                Enable = t,
            };
            data.Add(random);
            Song randomd = new Song()
            {
                Path = "",
                Folder = "",
                Title = "ランダム段位",
                FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                Course = r,
                Type = EType.Random_Dan,
                Enable = t,
            };
            data.Add(randomd);
        }
        public static void AddSong(List<Song> data, string path, bool isDifficulty)
        {
            foreach (string item in Directory.EnumerateFiles(path, "*.tbd", SearchOption.TopDirectoryOnly))
            {
                DanCourse dan = new DanCourse(item);
                bool[] enable = new bool[5];
                for (int i = 0; i < 5; i++)
                {
                    enable[i] = dan.Courses != null;
                }

                int p;
                string dir = "";
                foreach (string str in SongData.Ini)
                {
                    if (path.Contains(str)) dir = str;
                }
                Folder fol = new Folder(dir);
                Song songdata = new Song()
                {
                    Path = item,
                    Folder = dir,
                    Title = dan.Title,
                    Time = File.GetLastWriteTime(item),
                    FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                    BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                    Type = EType.Dan,
                    Enable = enable,
                };
                data.Add(songdata);
            }
            foreach (string item in Directory.EnumerateFiles(path, "*.tja", SearchOption.TopDirectoryOnly))
            {
                TJA parse = new TJA(item);
                List<string>[] slist = new List<string>[5];
                for (int i = 0; i < 5; i++)
                {
                    slist[i] = new List<string>();
                }
                foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(item), "*.tbr", SearchOption.TopDirectoryOnly))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (parse != null && Path.GetFileNameWithoutExtension(file).Replace($"{Path.GetFileNameWithoutExtension(path)}.", "").StartsWith($"{(ECourse)i}"))
                        {
                            slist[i].Add(Path.GetFileNameWithoutExtension(file).Replace($"{Path.GetFileNameWithoutExtension(path)}.{(ECourse)i}.", ""));
                        }
                    }
                }
                bool[] enable = new bool[5];
                int[] lamp = new int[5], notes = new int[5], score = new int[5];
                double[] level = new double[5], rate = new double[5];
                ScoreData scoredata = new BestScore(item).ScoreData;
                for (int i = 0; i < 5; i++)
                {
                    enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                    level[i] = parse.Courses != null ? parse.Courses[i].LEVEL : 0;
                    notes[i] = parse.Courses != null ? parse.Courses[i].TotalNotes : 0;
                    lamp[i] = scoredata != null && scoredata.Score[i].Score > 0 ? scoredata.Score[i].ClearLamp : 0;
                    score[i] = scoredata != null && scoredata.Score[i].Score > 0 ? scoredata.Score[i].Score : 0;
                    rate[i] = scoredata != null && scoredata.Score[i].Score > 0 ? (1.01 * scoredata.Score[i].Perfect + 1.0 * scoredata.Score[i].Great + 0.5 * scoredata.Score[i].Good) / notes[i] * 100000.0 : 0;
                }

                int p;
                string dir = "";
                foreach (string str in SongData.Ini)
                {
                    if (path.Contains(str)) dir = str;
                }
                Folder fol = new Folder(dir);
                if (isDifficulty)
                {
                    List<int> dif = new List<int>();
                    for (int i = 0; i < 5; i++)
                    {
                        if (enable[i]) dif.Add(i);
                    }
                    for (int i = 0; i < dif.Count; i++)
                    {
                        Song songdata = new Song()
                        {
                            Path = item,
                            Folder = dir,
                            Title = parse.Header.TITLE,
                            Time = File.GetLastWriteTime(item),
                            Header = parse.Header,
                            Course = parse.Courses,
                            FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                            BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                            Type = EType.Score,
                            Score = new BestScore(item).ScoreData,
                            ScoreList = slist,
                            DisplayDif = dif[i] + 1,
                            Level = level,
                            Lamp = lamp,
                            Enable = enable,
                            EXScore = score,
                            Notes = notes,
                            Rate = rate,
                        };
                        data.Add(songdata);
                    }
                }
                else
                {
                    Song songdata = new Song()
                    {
                        Path = item,
                        Folder = dir,
                        Title = parse.Header.TITLE,
                        Time = File.GetLastWriteTime(item),
                        Header = parse.Header,
                        Course = parse.Courses,
                        FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                        BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                        Type = EType.Score,
                        Score = new BestScore(item).ScoreData,
                        ScoreList = slist,
                        Level = level,
                        Lamp = lamp,
                        Enable = enable,
                        EXScore = score,
                        Notes = notes,
                        Rate = rate,
                    };
                    data.Add(songdata);
                }
            }
            List<string> bmses = new List<string>();
            foreach (string item in Directory.EnumerateFiles(path, "*.bms", SearchOption.TopDirectoryOnly))
                bmses.Add(item);
            foreach (string item in Directory.EnumerateFiles(path, "*.bme", SearchOption.TopDirectoryOnly))
                bmses.Add(item);
            foreach (string item in Directory.EnumerateFiles(path, "*.bml", SearchOption.TopDirectoryOnly))
                bmses.Add(item);
            foreach (string item in bmses)
            {
                BMS parse = new BMS(item);
                int p;
                string dir = "";
                foreach (string str in SongData.Ini)
                {
                    if (path.Contains(str)) dir = str;
                }
                Folder fol = new Folder(dir);
                double[] level = new double[5];
                for (int i = 0; i < 5; i++)
                {
                    level[i] = parse.Course != null && i == (int)parse.Course.Difficulty ? parse.Course.Level : 0;
                }
                Song songdata = new Song()
                {
                    isBMS = true,
                    Path = item,
                    Folder = dir,
                    Title = parse.Course.Title,
                    Time = File.GetLastWriteTime(item),
                    BCourse = parse.Course,
                    FontColor = !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                    BackColor = !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                    Type = EType.BMScore,
                };
                data.Add(songdata);
            }
        }
        public static void AddSong(List<Song> data, string path)
        {
            if (path.EndsWith(".tbd"))
            {
                DanCourse dan = new DanCourse(path);
                bool[] enable = new bool[5];
                for (int i = 0; i < 5; i++)
                {
                    enable[i] = dan.Courses != null;
                }

                int p;
                string dir = "";
                foreach (string str in SongData.Ini)
                {
                    if (path.Contains(str)) dir = str;
                }
                Folder fol = null;
                if (!string.IsNullOrEmpty(dir)) fol = new Folder(dir);
                Song songdata = new Song()
                {
                    Path = path,
                    Folder = dir,
                    Title = dan.Title,
                    Time = File.GetLastWriteTime(path),
                    FontColor = fol != null && !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                    BackColor = fol != null && !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                    Type = EType.Dan,
                    Enable = enable,
                };
                data.Add(songdata);
            }
            else if (path.EndsWith(".tja"))
            {
                TJA parse = new TJA(path);
                List<string>[] slist = new List<string>[5];
                for (int i = 0; i < 5; i++)
                {
                    slist[i] = new List<string>();
                }
                foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(path), "*.tbr", SearchOption.TopDirectoryOnly))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (parse != null && Path.GetFileNameWithoutExtension(file).Replace($"{Path.GetFileNameWithoutExtension(path)}.", "").StartsWith($"{(ECourse)i}"))
                        {
                            slist[i].Add(Path.GetFileNameWithoutExtension(file).Replace($"{Path.GetFileNameWithoutExtension(path)}.{(ECourse)i}.", ""));
                        }
                    }
                }
                bool[] enable = new bool[5];
                int[] lamp = new int[5], notes = new int[5], score = new int[5];
                double[] level = new double[5], rate = new double[5];
                ScoreData scoredata = new BestScore(path).ScoreData;
                for (int i = 0; i < 5; i++)
                {
                    enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                    level[i] = parse.Courses != null ? parse.Courses[i].LEVEL : 0;
                    notes[i] = parse.Courses != null ? parse.Courses[i].TotalNotes : 0;
                    lamp[i] = scoredata != null && scoredata.Score[i].Score > 0 ? scoredata.Score[i].ClearLamp : 0;
                    score[i] = scoredata != null && scoredata.Score[i].Score > 0 ? scoredata.Score[i].Score : 0;
                    rate[i] = scoredata != null && scoredata.Score[i].Score > 0 ? (1.01 * scoredata.Score[i].Perfect + 1.0 * scoredata.Score[i].Great + 0.5 * scoredata.Score[i].Good) / notes[i] * 100000.0 : 0;
                }

                int p;
                string dir = "";
                foreach (string str in SongData.Ini)
                {
                    if (path.Contains(str)) dir = str;
                }
                Folder fol = null;
                if (!string.IsNullOrEmpty(dir)) fol = new Folder(dir);
                Song songdata = new Song()
                {
                    Path = path,
                    Folder = dir,
                    Title = parse.Header.TITLE,
                    Time = File.GetLastWriteTime(path),
                    Header = parse.Header,
                    Course = parse.Courses,
                    FontColor = fol != null && !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                    BackColor = fol != null && !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                    Type = EType.Score,
                    Score = new BestScore(path).ScoreData,
                    ScoreList = slist,
                    Level = level,
                    Lamp = lamp,
                    Enable = enable,
                    EXScore = score,
                    Notes = notes,
                    Rate = rate,
                };
                data.Add(songdata);
            }

            else if (path.EndsWith(".bms") || path.EndsWith(".bme") || path.EndsWith(".bml"))
            {
                BMS parse = new BMS(path);
                int p;
                string dir = "";
                foreach (string str in SongData.Ini)
                {
                    if (path.Contains(str)) dir = str;
                }
                Folder fol = null;
                if (!string.IsNullOrEmpty(dir)) fol = new Folder(dir);
                double[] level = new double[5];
                for (int i = 0; i < 5; i++)
                {
                    level[i] = parse.Course != null && i == (int)parse.Course.Difficulty ? parse.Course.Level : 0;
                }
                Song songdata = new Song()
                {
                    isBMS = true,
                    Path = path,
                    Folder = dir,
                    Title = parse.Course.Title,
                    Time = File.GetLastWriteTime(path),
                    BCourse = parse.Course,
                    FontColor = fol != null && !string.IsNullOrEmpty(fol.FontColor) && int.TryParse(fol.FontColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.FontColor.Substring(0, 7)) : Color.White,
                    BackColor = fol != null && !string.IsNullOrEmpty(fol.BackColor) && int.TryParse(fol.BackColor.Substring(1, 6), System.Globalization.NumberStyles.HexNumber, null, out p) ? ColorTranslator.FromHtml(fol.BackColor.Substring(0, 7)) : Color.Black,
                    Type = EType.BMScore,
                    Level = level
                };
                data.Add(songdata);
            }
        }
        public static Song ReLoad(Song song)//only
        {
            string path = song.Path;
            TJA parse = new TJA(path);
            List<string>[] slist = new List<string>[5];
            for (int i = 0; i < 5; i++)
            {
                slist[i] = new List<string>();
            }
            foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(path), "*.tbr", SearchOption.TopDirectoryOnly))
            {
                for (int i = 0; i < 5; i++)
                {
                    if (parse != null && Path.GetFileNameWithoutExtension(file).Replace($"{Path.GetFileNameWithoutExtension(path)}.", "").StartsWith($"{(ECourse)i}"))
                    {
                        slist[i].Add(Path.GetFileNameWithoutExtension(file).Replace($"{Path.GetFileNameWithoutExtension(path)}.{(ECourse)i}.", ""));
                    }
                }
            }
            bool[] enable = new bool[5];
            int[] lamp = new int[5], notes = new int[5], score = new int[5];
            double[] rate = new double[5];
            ScoreData scoredata = new BestScore(path).ScoreData;
            for (int i = 0; i < 5; i++)
            {
                enable[i] = parse.Courses != null ? parse.Courses[i].IsEnable : false;
                notes[i] = parse.Courses != null ? parse.Courses[i].TotalNotes : 0;
                lamp[i] = scoredata != null && scoredata.Score[i].Score > 0 ? scoredata.Score[i].ClearLamp : 0;
                score[i] = scoredata != null && scoredata.Score[i].Score > 0 ? scoredata.Score[i].Score : 0;
                rate[i] = scoredata != null && scoredata.Score[i].Score > 0 ? (1.01 * scoredata.Score[i].Perfect + 1.0 * scoredata.Score[i].Great + 0.5 * scoredata.Score[i].Good) / notes[i] * 100000.0 : 0;
            }
            song.Score = scoredata;
            song.ScoreList = slist;
            song.Lamp = lamp;
            song.EXScore = score;
            song.Rate = rate;
            return song;
        }
        public static void LoadSong(List<Song> data, string path, bool isDifficulty = false)//all
        {
            AddSong(data, path, isDifficulty);
            foreach (string directory in Directory.EnumerateDirectories(path.Replace("/", "\\"), "*", SearchOption.AllDirectories))
            {
                AddSong(data, directory, isDifficulty);
            }
        }

        public static void LoadSong(List<Song> data, List<Song> list, string path)//fol
        {
            foreach (Song song in list)
            {
                if (song.Folder == path)
                {
                    data.Add(song);
                }
            }
        }

        public static void Sort(List<Song> data, ESort comp)
        {
            if (data == null) return;

            for (int s = data.Count - 1; s >= 0; s--)
            {
                Song song = data[s];
                if (song != null && song.Type >= EType.Score)
                {
                    int e = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (song.Course[i] != null && song.Course[i].IsEnable) e++;
                        
                    }
                    if (song.BCourse != null && song.BCourse.IsEnable) e++;
                    if (e == 0) data.RemoveAt(s);
                }
            }

            data.Sort(SortSystem[(int)comp]);

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Prev = data[(i + (data.Count - 1)) % data.Count];
                data[i].Next = data[(i + 1) % data.Count];
            }
        }

        public static int FolderFloor;
        public static ESort NowSort;

        public static Comparison<Song>[] SortSystem = new Comparison<Song>[15]
            {
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Path.CompareTo(b.Path);
                    return resultb != 0 ? resultb : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Title.CompareTo(b.Title);
                    return resultb != 0 ? resultb : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Title.CompareTo(a.Title);
                    return resultb != 0 ? resultb : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : (int)a.Level[SongSelect.EnableCourse(a, 0)] - (int)b.Level[SongSelect.EnableCourse(b, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : (int)b.Level[SongSelect.EnableCourse(b, 0)] - (int)a.Level[SongSelect.EnableCourse(a, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Time.CompareTo(a.Time);
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Time.CompareTo(b.Time);
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Lamp[SongSelect.EnableCourse(b, 0)] - a.Lamp[SongSelect.EnableCourse(a, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Lamp[SongSelect.EnableCourse(a, 0)] - b.Lamp[SongSelect.EnableCourse(b, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : b.Notes[SongSelect.EnableCourse(b, 0)] - a.Notes[SongSelect.EnableCourse(a, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : a.Notes[SongSelect.EnableCourse(a, 0)] - b.Notes[SongSelect.EnableCourse(b, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : b.EXScore[SongSelect.EnableCourse(b, 0)] - a.EXScore[SongSelect.EnableCourse(a, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    int resultb = resulta != 0 ? resulta : a.EXScore[SongSelect.EnableCourse(a, 0)] - b.EXScore[SongSelect.EnableCourse(b, 0)];
                    int resultc = resultb != 0 ? resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    double resultb = resulta != 0 ? resulta : b.Rate[SongSelect.EnableCourse(b, 0)] - a.Rate[SongSelect.EnableCourse(a, 0)];
                    int resultc = resultb != 0 ? (int)resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
                (a, b) => { int result = a.Type - b.Type; int resulta = result != 0 ? result : b.Enable[SongSelect.EnableCourse(b, 0)].CompareTo(a.Enable[SongSelect.EnableCourse(a, 0)]);
                    double resultb = resulta != 0 ? resulta : a.Rate[SongSelect.EnableCourse(a, 0)] - b.Rate[SongSelect.EnableCourse(b, 0)];
                    int resultc = resultb != 0 ? (int)resultb : a.Path.CompareTo(b.Path);
                    return resultc != 0 ? resultc : a.DisplayDif - b.DisplayDif; },
            };
    }

    public class Song
    {
        public bool isBMS;
        public string Title;
        public string Path;
        public string Folder;
        public DateTime Time;
        public Header Header;
        public Course[] Course = new Course[5];
        public BCourse BCourse;
        public EType Type;
        public Song Prev;
        public Song Next;
        public ScoreData Score;
        public int DisplayDif;
        public bool[] Enable = new bool[5];
        public double[] Level = new double[5];
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
        Random,
        Random_Dan,
        AllSongs,
        AllDifficulty,
        Folder,
        Dan,
        Score,
        BMScore
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
        public string Name, Path, Directory, BackColor, FontColor;
        public Folder(string path)
        {
            Name = path;
            Path = path;
            string dir = "";
            string s = System.IO.Path.GetDirectoryName(path);
            foreach (string str in SongData.Ini)
            {
                if (s.Contains(str)) dir = str;
            }
            Directory = dir;
            if (File.Exists(path + @"\genre.ini"))
            {
                string str;
                using (StreamReader reader = new StreamReader(path + @"\genre.ini", Encoding.GetEncoding("Shift_JIS")))
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
