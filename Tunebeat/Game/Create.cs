using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class Create : Scene
    {
        public override void Enable()
        {
            CreateMode = !string.IsNullOrEmpty(SongSelect.SongSelect.FileName) ? true : false;
            Preview = CreateMode;
            InfoMenu = false;
            Selecting = false;
            Mapping = false;
            Metro = false;
            MetroL = false;
            Read();
            File = new TJAFile(Game.MainTJA[0]);
            ListMeasureCount = new List<int>();
            MeasureCount = 0;
            InputType = 4;
            NowInput = 0;
            NowColor = 0;
            base.Enable();
        }
        public static void Read()
        {
            AllText = new List<string>();
            using (StreamReader sr = new StreamReader(Game.MainTJA[0].TJAPath, Encoding.GetEncoding("SHIFT_JIS")))
            {
                while (sr.Peek() > -1)
                {
                    AllText.Add(sr.ReadLine());
                }
            }
            for (int i = 0; i < AllText.Count; i++)
            {
                //コメント削除
                if (AllText[i].Contains("//"))
                    AllText[i] = AllText[i].Substring(0, AllText[i].IndexOf("//"));
                //バグ対策
                if (AllText.Count > 0 && AllText[i] == ",")
                    AllText[i] = AllText[i].Replace(",", "0,");
            }
            Course = new List<string>();
            string dif = ($"{(ECourse)Game.Course[0]}").ToLower();
            foreach (string s in AllText)
            {
                if (ad)
                {
                    Course.Add(s);
                }
                if (s.ToLower().StartsWith($"course:{dif}") || s.ToLower().StartsWith($"course:{Game.Course[0]}"))
                {
                    ad = true;
                    Course.Add(s);
                }
                else if (ad && s.StartsWith("#END"))
                {
                    ad = false;
                }
            }
            NowScroll = 0;
        }

        public override void Disable()
        {
            AllText = null;
            Course = null;
            base.Disable();
        }
        public override void Draw()
        {
            if (CreateMode)
            {
                TextureLoad.Game_Notes.Draw(195 * 4 + 98, 520, new Rectangle(195 * 6, 0, 195 * 2, 195));
                TextureLoad.Game_Notes.Draw(0, 520, new Rectangle(195, 0, 195 * 5, 195));
                TextureLoad.Game_Notes.Draw(98, 520 + 195, new Rectangle(195 * 9, 0, 195 * 2, 195));
                TextureLoad.Game_Notes.Draw(0, 520 + 195, new Rectangle(195 * 8, 0, 195, 195));
                TextureLoad.Game_Notes.Draw(195 * 2, 520 + 195, new Rectangle(195 * 11, 0, 195 * 4, 195));

                double len = (Mouse.X - (Notes.NotesP[0].X - 22)) / (1421f / InputType);
                int cur = (int)Math.Floor(len);
                int[] wid = new int[8] { 0, 29, 60, 75, 84, 90, 97, 104 };
                Rectangle[] rec = new Rectangle[8] { new Rectangle(195 * 1, 0, 195 * 1, 195), new Rectangle(195 * 2, 0, 195 * 1, 195), new Rectangle(195 * 3, 0, 195 * 1, 195),
                new Rectangle(195 * 4, 0, 195 * 1, 195), new Rectangle(195 * 5, 0, 195 * 1, 195), new Rectangle(195 * 8, 0, 195 * 1, 195),
                new Rectangle(195 * 11, 0, 195 * 2, 195), new Rectangle(195 * 13, 0, 195 * 2, 195) };
                if (Mouse.X >= Notes.NotesP[0].X - 22 && Mouse.Y >= Notes.NotesP[0].Y && Mouse.Y < Notes.NotesP[0].Y + 195 && Game.MainTimer.State == 0)
                {
                    double width = (1920 - (Notes.NotesP[0].X - 22)) / (double)InputType;
                    TextureLoad.Game_Notes.Opacity = 0.5;
                    TextureLoad.Game_Notes.Draw(Notes.NotesP[0].X - wid[NowInput] + cur * width, Notes.NotesP[0].Y, rec[NowColor]);
                    TextureLoad.Game_Notes.Opacity = 1.0;
                }

                if (Course != null)
                {
                    if (Course.Count > 54)
                    {
                        float count = Course.Count - 53;
                        float width = 1080 / count;
                        if (width < 4) width = 1076 / count;
                        float y = NowScroll * width;
                        DrawBoxAA(1916, y, 1920, y + (width < 4 ? 4 : width), 0xffff00, TRUE);
                    }
                    for (int i = 0; i < (Course.Count > 54 ? 54 : Course.Count); i++)
                    {
                        DrawString(1280, 20 * i, Course[i + NowScroll], 0xffffff);
                    }
                }
                DrawBox(0, 0, 496, Notes.NotesP[0].Y - 4, 0x000000, TRUE);
                DrawString(40, 10, $"Title:{(Input.IsEnable && Cursor == 0 ? Input.Text : File.Title)}", Selecting && Cursor == 0 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 30, $"SubTitle:{(Input.IsEnable && Cursor == 1 ? Input.Text : File.SubTitle)}", Selecting && Cursor == 1 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 50, $"Wave:{(Input.IsEnable && Cursor == 2 ? Input.Text : File.Wave)}", Selecting && Cursor == 2 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 70, $"BGImage:{(Input.IsEnable && Cursor == 3 ? Input.Text : File.BGImage)}", Selecting && Cursor == 3 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 90, $"BGMovie:{(Input.IsEnable && Cursor == 4 ? Input.Text : File.BGMovie)}", Selecting && Cursor == 4 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 110, $"Bpm:{(Input.IsEnable && Cursor == 5 ? Input.Text : File.Bpm.ToString())}", Selecting && Cursor == 5 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 130, $"Offset:{(Input.IsEnable && Cursor == 6 ? Input.Text : File.Offset.ToString())}", Selecting && Cursor == 6 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 150, $"SongVol:{(Input.IsEnable && Cursor == 7 ? Input.Text : File.SongVol.ToString())}", Selecting && Cursor == 7 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 170, $"SeVol:{(Input.IsEnable && Cursor == 8 ? Input.Text : File.SeVol.ToString())}", Selecting && Cursor == 8 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 190, $"DemoStart:{(Input.IsEnable && Cursor == 9 ? Input.Text : File.DemoStart.ToString())}", Selecting && Cursor == 9 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 210, $"Genre:{(Input.IsEnable && Cursor == 10 ? Input.Text : File.Genre)}", Selecting && Cursor == 10 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 230, $"NowCourse:{(ECourse)Game.Course[0]}", Selecting && Cursor == 11 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 250, $"Level:{(Input.IsEnable && Cursor == 12 ? Input.Text : File.Level[Game.Course[0]].ToString())}", Selecting && Cursor == 12 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 270, $"Total:{(Input.IsEnable && Cursor == 13 ? Input.Text : File.Total[Game.Course[0]].ToString())}", Selecting && Cursor == 13 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 310, $"Notes:{Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes}", 0xffffff);
                if (InfoMenu)
                {
                    DrawString(10, 10 + 20 * Cursor, ">", 0xff0000);
                }
                if (Selecting && Cursor == 11)
                {
                    DrawString(30, 10 + 20 * Cursor, "<", 0x0000ff);
                    DrawString(40 + GetDrawStringWidth($"NowCourse:{(ECourse)Game.Course[0]}", $"NowCourse:{(ECourse)Game.Course[0]}".Length), 10 + 20 * Cursor, ">", 0x0000ff);
                }

#if DEBUG
                DrawString(240, 20, $"Length:{len}", 0xffffff);
                DrawString(240, 40, $"Cursor:{cur}", 0xffffff);
                DrawString(240, 60, $"Bar:{File.Bar[Game.Course[0]].Count}", 0xffffff);
                DrawString(240, 80, $"Song:{GetSoundTotalTime(Game.MainSong.ID)}", 0xffffff);
                DrawString(240, 100, $"Off:{-File.Offset * 1000.0}", 0xffffff);
                for (int i = 0; i < File.Bar[Game.Course[0]].Count; i++)
                {
                    int count = File.Bar[Game.Course[0]][i].Chip.Count;
                    for (int j = 0; j < count; j++)
                    {
                        DrawString(100 + 9 * j, 500 + 20 * i, $"{(int)File.Bar[Game.Course[0]][i].Chip[j].ENote}", 0xffffff);
                    }
                    DrawString(100 + 9 * count, 500 + 20 * i, ",", 0xffffff);
                }
                int me = Game.NowMeasure - 1;
                if (me >= 0 && me < File.Bar[Game.Course[0]].Count)
                {
                    double num = 240000.0 / File.Bar[Game.Course[0]][me].BPM * File.Bar[Game.Course[0]][me].Measure;
                    double num2 = Game.MainTimer.Value - File.Bar[Game.Course[0]][me].Time;
                    DrawString(400, 500, $"num:{num}", 0xffffff);
                    DrawString(400, 520, $"num2:{num2}", 0xffffff);
                    int count = File.Bar[Game.Course[0]][me].Chip.Count;
                    DrawString(600, 500, $"Count:{count}", 0xffffff);
                    for (int j = 0; j < count; j++)
                    {
                        double num3 = (j - 0.5) * (double)num / count;
                        double num4 = (j + 0.5) * (double)num / count;
                        DrawString(400, 540 + 20 * j, $"{num3}", 0xffffff);
                        DrawString(600, 540 + 20 * j, $"{num4}", 0xffffff);
                    }
                    Chip chip = GetNotes.GetNowNote(File.Bar[Game.Course[0]][me].Chip, Game.MainTimer.Value, true);
                    if (chip != null)
                    {
                        DrawString(800, 500, $"{1 / chip.Measure}", 0xffffff);
                    }
                }
#endif
            }
            base.Draw();
        }
        public override void Update()
        {
            if (CreateMode)
            {
                List<BarLine> Bar = File.Bar[Game.Course[0]];
                if (Game.MainTimer.State == 0)
                {
                    mg = Game.NowMeasure - 1;
                    mb = 0;
                }
                else
                {
                    if (Game.NowMeasure > 0)
                    {
                        if (Game.NowMeasure > mg && Game.MainTimer.Value >= Bar[Game.NowMeasure - 1].Time)
                        {
                            SoundLoad.Metronome_Bar.Play();
                            mg = Game.NowMeasure;
                            mb = 0;
                        }
                        for (int j = 1; j < Bar[Game.NowMeasure - 1].Measure * 4; j++)
                        {
                            double time = Bar[Game.NowMeasure - 1].Time + j * (60000.0 / Bar[Game.NowMeasure - 1].BPM);
                            if (j > mb && Game.MainTimer.Value >= time)
                            {
                                SoundLoad.Metronome.Play();
                                mb = j;
                            }
                        }
                    }
                }
            }
            base.Update();
        }

        public static void InputN(int course, double time, int count, bool isDon)
        {
            for (int i = count - 1; i < count + 3; i++)
            {
                if (i >= 0 && i < File.Bar[course].Count)
                {
                    double num = 240000.0 / File.Bar[course][i].BPM * File.Bar[course][i].Measure;
                    double num2 = time - File.Bar[course][i].Time;
                    int noteamount = File.Bar[course][i].Chip.Count;
                    for (int j = 0; j < noteamount; j++)
                    {
                        double num3 = (j - 0.5) * (double)num / noteamount;
                        double num4 = (j + 0.5) * (double)num / noteamount;
                        if (num2 >= num3 && num2 < num4)
                        {
                            if (isDon)
                            {
                                if (File.Bar[course][i].Chip[j].ENote == ENote.Space)
                                {
                                    File.Bar[course][i].Chip[j].ENote = ENote.Don;
                                }
                                if (File.Bar[course][i].Chip[j].ENote == ENote.Don && KeyInput.ListPushing(PlayData.Data.LEFTDON) && KeyInput.ListPushing(PlayData.Data.RIGHTDON))
                                {
                                    File.Bar[course][i].Chip[j].ENote = ENote.DON;
                                }
                            }
                            else
                            {
                                if (File.Bar[course][i].Chip[j].ENote == ENote.Space)
                                {
                                    File.Bar[course][i].Chip[j].ENote = ENote.Ka;
                                }
                                if (File.Bar[course][i].Chip[j].ENote == ENote.Ka && KeyInput.ListPushing(PlayData.Data.LEFTKA) && KeyInput.ListPushing(PlayData.Data.RIGHTKA))
                                {
                                    File.Bar[course][i].Chip[j].ENote = ENote.KA;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void BarInit(int course)
        {
            File.Bar[course].Clear();
            double num = -File.Offset * 1000.0;
            while (num < GetSoundTotalTime(Game.MainSong.ID))
            {
                BarLine chip = new BarLine(num, File.Bpm, 1.0, InputType);
                File.Bar[course].Add(chip);
                num += (long)(240000.0 / File.Bpm);
            }
        }

        public static void BarLoad(int course)
        {
            File.Bar[course].Clear();
            if (Game.MainTJA[0].Courses[Game.Course[0]].IsEnable)
            {
                foreach (string str in Course)
                    GetMeasureCount(str);
                List<List<Chip>> ListChip = new List<List<Chip>>();
                foreach (string str in Course)
                    Parse(str, File.Bar[course], ListChip);
            }
            else BarInit(course);
        }

        public static void Save(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.GetEncoding("SHIFT_JIS")))
            {
                streamWriter.WriteLine($"TITLE:{File.Title}");
                streamWriter.WriteLine($"SUBTITLE:--{File.SubTitle}");
                streamWriter.WriteLine($"BPM:{File.Bpm}");
                streamWriter.WriteLine($"WAVE:{File.Wave}");
                streamWriter.WriteLine($"OFFSET:{File.Offset}");
                streamWriter.WriteLine($"SONGVOL:{File.SongVol}");
                streamWriter.WriteLine($"SEVOL:{File.SeVol}");
                streamWriter.WriteLine($"DEMOSTART:{File.DemoStart}");
                streamWriter.WriteLine($"GENRE:{File.Genre}");
                streamWriter.WriteLine("");

                for (int i = 4; i >= 0; i--)
                {
                    streamWriter.WriteLine($"COURSE:{i}");
                    streamWriter.WriteLine($"LEVEL:{File.Level[i]}");
                    if (File.Total[i] > 0) streamWriter.WriteLine($"{File.Total[i]}");
                    streamWriter.WriteLine("");

                    streamWriter.WriteLine("#START");
                    streamWriter.WriteLine("");
                    foreach (BarLine bar in File.Bar[i])
                    {
                        for (int j = 0; j < bar.Chip.Count; j++)
                        {
                            streamWriter.Write($"{(int)bar.Chip[j].ENote}");
                        }
                        streamWriter.WriteLine(",");
                    }
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("#END");
                    streamWriter.WriteLine("");
                }
            }
        }

        public static void Parse(string str, List<BarLine> bar, List<List<Chip>> listchip)
        {
            if (str.Length <= 0) return;
            if (str.StartsWith("#"))
            {
                /*if (str.StartsWith("#BMSCROLL"))
                {
                    courses[NowCourse].ScrollType = EScroll.BMSCROLL;
                }
                else if (str.StartsWith("#HBSCROLL"))
                {
                    courses[NowCourse].ScrollType = EScroll.HBSCROLL;
                }
                else*/ if (str.StartsWith("#START"))
                {
                    CreateParse.StartParse = true;
                    CreateParse.Time = (long)(File.Offset * -1000.0) / PlayData.Data.PlaySpeed;
                    CreateParse.Scroll = 1.0;
                    CreateParse.Bpm = File.Bpm * PlayData.Data.PlaySpeed;
                    CreateParse.Measure = 1.0;
                    CreateParse.MeasureCount = 0;
                    CreateParse.ShowBarLine = true;
                    CreateParse.AddMeasure = true;
                    CreateParse.RollBegin = null;
                    CreateParse.LyricText = null;
                    CreateParse.Sudden = new double[2] { 0.0, 0.0 };
                    listchip.Add(new List<Chip>());
                }
                else if (str.StartsWith("#END"))
                {
                    CreateParse.StartParse = false;
                }
                else if (str.StartsWith("#SCROLL"))
                {
                    CreateParse.Scroll = double.Parse(str.Replace("#SCROLL", "").Trim());
                }
                else if (str.StartsWith("#BPMCHANGE"))
                {
                    CreateParse.Bpm = double.Parse(str.Replace("#BPMCHANGE", "").Trim()) * PlayData.Data.PlaySpeed;
                }
                else if (str.StartsWith("#DELAY"))
                {
                    CreateParse.Time += double.Parse(str.Replace("#DELAY", "").Trim()) * 1000.0;
                }
                else if (str.StartsWith("#MEASURE"))
                {
                    var SplitSlash = str.Replace("#MEASURE", "").Trim().Split('/');
                    if (SplitSlash.Length > 1) CreateParse.Measure = double.Parse(SplitSlash[1]) / double.Parse(SplitSlash[0]);
                }
                else if (str.StartsWith("#BARLINEON"))
                {
                    CreateParse.ShowBarLine = true;
                }
                else if (str.StartsWith("#BARLINEOFF"))
                {
                    CreateParse.ShowBarLine = false;
                }
                else if (str.StartsWith("#GOGOSTART"))
                {
                    CreateParse.IsGogo = true;
                }
                else if (str.StartsWith("#GOGOEND"))
                {
                    CreateParse.IsGogo = false;
                }
                else if (str.StartsWith("#LYRIC"))
                {
                    CreateParse.LyricText = str.Replace("#LYRIC", "").Trim();
                }
                else if (str.StartsWith("#SUDDEN"))
                {
                    var SplitSlash = str.Replace("#SUDDEN", "").Trim().Split(' ');
                    if (SplitSlash.Length > 1) CreateParse.Sudden = new double[2] { double.Parse(SplitSlash[0]) * 1000.0, double.Parse(SplitSlash[1]) * 1000.0 };
                    else CreateParse.Sudden = new double[2] { double.Parse(SplitSlash[0]) * 1000.0, double.Parse(SplitSlash[0]) * 1000.0 };
                }
            }
            else
            {
                if (!((str[0] >= '0' && str[0] <= '9') || (str[str.Length - 1] >= '0' && str[str.Length - 1] <= '9') || str[0] == ',' || str[str.Length - 1] == ',')) return;
                if (!CreateParse.StartParse) return;

                //List<Chip> listchip = new List<Chip>();
                if (CreateParse.AddMeasure)
                {
                    int measure = CreateParse.MeasureCount;
                    List<Chip> chips = listchip[measure];
                    BarLine chip = new BarLine(CreateParse.Time, CreateParse.Bpm, CreateParse.Scroll, 1 / CreateParse.Measure, chips.Count, CreateParse.ShowBarLine, chips);
                    bar.Add(chip);
                    CreateParse.AddMeasure = false;
                }
                for (int i = 0; i < str.Length; i++)
                {
                    var num = str[i];
                    if (num == ',')
                    {
                        CreateParse.MeasureCount++;
                        listchip.Add(new List<Chip>());
                        CreateParse.AddMeasure = true;
                    }
                    if (num >= '0' && num <= '9')
                    {
                        Chip chip = new Chip()
                        {
                            Time = CreateParse.Time,
                            Bpm = CreateParse.Bpm,
                            Scroll = CreateParse.Scroll,
                            Measure = CreateParse.Measure,
                            IsGogo = CreateParse.IsGogo,
                            EChip = EChip.Note,
                            ENote = (ENote)int.Parse(num.ToString()),
                            CanShow = true,
                            Lyric = CreateParse.LyricText,
                            Sudden = CreateParse.Sudden
                        };

                        if (chip.ENote == ENote.Balloon || chip.ENote == ENote.RollStart || chip.ENote == ENote.ROLLStart || chip.ENote == ENote.Kusudama)
                        {
                            if (CreateParse.RollBegin == null)
                                CreateParse.RollBegin = chip;
                        }
                        if (chip.EChip == EChip.Note && chip.ENote == ENote.RollEnd)
                        {
                            if (CreateParse.RollBegin != null)
                            {
                                CreateParse.RollBegin.RollEnd = chip;
                                chip.RollEnd = chip;
                            }
                            CreateParse.RollBegin = null;
                        }

                        //if (chip.ENote != ENote.Space)
                        listchip[CreateParse.MeasureCount].Add(chip);

                        int count = CreateParse.MeasureCount < ListMeasureCount.Count ? CreateParse.MeasureCount : ListMeasureCount.Count - 1;
                        CreateParse.Time += 15000d / CreateParse.Bpm / CreateParse.Measure * (16d / ListMeasureCount[count]);
                    }
                }
            }
        }
        public static void GetMeasureCount(string str)
        {
            if (str.Length <= 0) return;
            if (str.StartsWith("#"))
            {
                if (str.StartsWith("#START"))
                {
                    CreateParse.StartParse = true;
                }
                else if (str.StartsWith("#END"))
                {
                    CreateParse.StartParse = false;
                }
            }
            else
            {
                if (!((str[0] >= '0' && str[0] <= '9') || (str[str.Length - 1] >= '0' && str[str.Length - 1] <= '9') || str[0] == ',' || str[str.Length - 1] == ',')) return;
                if (!CreateParse.StartParse) return;
                for (int i = 0; i < str.Length; i++)
                {
                    var num = str[i];
                    if (num == ',')
                    {
                        ListMeasureCount.Add(MeasureCount);
                        MeasureCount = 0;
                    }
                    else if (num >= '0' && num <= '9')
                    {
                        MeasureCount++;
                    }

                }
            }
        }

        public static bool CreateMode, Preview, InfoMenu, Selecting, Mapping, ad, MetroL, Metro;
        public static List<string> AllText, Course;
        public static List<int> ListMeasureCount;
        public static int NowScroll, Cursor, InputType, NowInput, NowColor, MeasureCount, mg, mb;
        public static TJAFile File { get; set; }
    }

    public class TJAFile
    {
        public TJAFile(TJAParse.TJAParse parse)
        {
            Title = parse.Header.TITLE;
            SubTitle = parse.Header.SUBTITLE;
            Wave = parse.Header.WAVE;
            BGImage = parse.Header.BGIMAGE;
            BGMovie = parse.Header.BGMOVIE;
            Genre = parse.Header.GENRE;
            Bpm = parse.Header.BPM;
            Offset = parse.Header.OFFSET;
            SongVol = (int)parse.Header.SONGVOL;
            SeVol = (int)parse.Header.SEVOL;
            DemoStart = parse.Header.DEMOSTART;
            for (int i = 0; i < 5; i++)
            {
                Level[i] = parse.Courses[i].LEVEL;
                Total[i] = parse.Courses[i].TOTAL;
                Bar[i] = new List<BarLine>();
            }
        }

        public string Title, SubTitle, Wave, BGImage, BGMovie, Genre;
        public double Bpm, Offset, DemoStart;
        public int SongVol, SeVol;
        public int[] Level = new int[5];
        public double[] Total = new double[5];
        public List<BarLine>[] Bar = new List<BarLine>[5];
    }
    public class BarLine
    {
        public BarLine(double time, double bpm, double beat, int measure)
        {
            Time = time;
            BPM = bpm;
            Scroll = 1.0;
            Measure = beat;
            Amount = measure;
            IsShow = true;
            Chip = new List<Chip>();
            double num = time;
            for (int i = 0; i < Amount; i++)
            {
                Chip chip = new Chip()
                {
                    Time = num,
                    Bpm = BPM,
                    Scroll = 1.0,
                    Measure = Measure,
                    EChip = EChip.Note,
                    ENote = ENote.Space,
                    CanShow = true
                };
                Chip.Add(chip);
                num += (long)(240000.0 / Amount / BPM * (4.0 / 4.0));
            }
        }

        public BarLine(double time, double bpm, double scroll, double beat, int measure, bool show, List<Chip> chip)
        {
            Time = time;
            BPM = bpm;
            Scroll = scroll;
            Measure = beat;
            Amount = measure;
            IsShow = show;
            Chip = chip;
        }

        public double Time, BPM, Scroll, Measure;
        public int Amount;
        public bool IsShow;
        public List<Chip> Chip;
    }

    public class CreateParse
    {
        public static bool StartParse;
        public static double Time;
        public static double Scroll = 1.0;
        public static double Bpm = 120.0;
        public static double Measure = 1.0;
        public static int MeasureCount = 0;
        public static bool IsGogo = false;
        public static bool ShowBarLine = true;
        public static bool AddMeasure = false;
        public static Chip RollBegin = null;
        public static string LyricText = null;
        public static double[] Sudden = new double[2];
    }
}
