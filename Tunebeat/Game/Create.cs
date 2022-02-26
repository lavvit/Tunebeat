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
            CreateMode = SongSelect.SongSelect.PlayMode == 1 ? true : false;
            Preview = true;
            InfoMenu = false;
            Selecting = false;
            Mapping = false;
            Metro = false;
            MetroL = false;
            Edited = false;
            File = new TJAFile(Game.MainTJA[0]);
            Read();
            MeasureCount = 0;
            InputType = 4;
            NowInput = 0;
            NowColor = 0;
            CommandLayer = 0;
            SelectedChip = (null, null);
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
            for (int i = 0; i < 5; i++)
            {
                Course[i] = new List<string>();
                string dif = $"{(ECourse)i}".ToLower();
                foreach (string s in AllText)
                {
                    if (ad)
                    {
                        Course[i].Add(s);
                    }
                    if (s.ToLower().StartsWith($"course:{dif}") || s.ToLower().StartsWith($"course:{i}"))
                    {
                        ad = true;
                        Course[i].Add(s);
                    }
                    else if (ad && s.StartsWith("#END"))
                    {
                        ad = false;
                    }
                }
                if (Course[i].Count > 0) BarLoad(i);
            }
            
            NowScroll = 0;
        }

        public override void Disable()
        {
            AllText = null;
            for (int i = 0; i < 5; i++)
            {
                Course[i] = null;
            }
            base.Disable();
        }
        public override void Draw()
        {
            if (CreateMode)
            {
                //TextureLoad.Game_Notes.Draw(195 * 4 + 98, 520, new Rectangle(195 * 6, 0, 195 * 2, 195));
                //TextureLoad.Game_Notes.Draw(0, 520, new Rectangle(195, 0, 195 * 5, 195));
                //TextureLoad.Game_Notes.Draw(98, 520 + 195, new Rectangle(195 * 9, 0, 195 * 2, 195));
                //TextureLoad.Game_Notes.Draw(0, 520 + 195, new Rectangle(195 * 8, 0, 195, 195));
                //TextureLoad.Game_Notes.Draw(195 * 2, 520 + 195, new Rectangle(195 * 11, 0, 195 * 4, 195));

                double len = (Mouse.X - (Notes.NotesP[0].X - 22)) / (1421f / InputType);
                int cur = (int)Math.Floor(len);
                if (Course[Game.Course[0]] != null)
                {
                    if (Course[Game.Course[0]].Count > 29)
                    {
                        float count = Course[Game.Course[0]].Count - 28;
                        float width = (1080 - 490) / count;
                        if (width < 4) width = (1076 - 490) / count;
                        float y = 490 + NowScroll * width;
                        DrawBoxAA(1916, y, 1920, y + (width < 4 ? 4 : width), 0xffff00, TRUE);
                    }
                    for (int i = 0; i < (Course[Game.Course[0]].Count > 29 ? 29 : Course[Game.Course[0]].Count); i++)
                    {
                        DrawString(1280, 495 + 20 * i, Course[Game.Course[0]][i + NowScroll], 0xffffff);
                    }
                }

                DrawString(472, 350, $"{InputType,2}", 0xffffff);
                if (Mapping) DrawCircleAA(478, 330, 8, 256, 0xff0000, TRUE);

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
                if (CommandLayer == 1)
                {
                    DrawBox(0, Notes.NotesP[0].Y + 199, 400, Notes.NotesP[0].Y + 239, 0x000000, TRUE);
                    DrawString(10, Notes.NotesP[0].Y + 209, "命令文を追加/削除するノーツを選択してください。", 0xffffff);
                }
                if (CommandLayer == 2)
                {
                    DrawBox(0, Notes.NotesP[0].Y + 199, 240, Notes.NotesP[0].Y + 239, 0x000000, TRUE);
                    DrawString(10, Notes.NotesP[0].Y + 209, "命令文を記入してください。", 0xffffff);
                }
                if (RollEnd) DrawString(800, 520, "Roll...", 0xffffff);

#if DEBUG
                DrawString(240, 20, $"Length:{len}", 0xffffff);
                DrawString(240, 40, $"Cursor:{cur}", 0xffffff);
                DrawString(240, 60, $"Bar:{File.Bar[Game.Course[0]].Count}", 0xffffff);
                DrawString(240, 80, $"Song:{GetSoundTotalTime(Game.MainSong.ID)}", 0xffffff);
                DrawString(240, 100, $"Off:{-File.Offset * 1000.0}", 0xffffff);
                for (int i = 0; i < File.Bar[Game.Course[0]].Count; i++)
                {
                    List<Chip> chip = new List<Chip>();
                    foreach (Chip c in File.Bar[Game.Course[0]][i].Chip)
                        chip.Add(c);
                    RangeFix(chip, 1);
                    if (i < 29)
                    {
                        for (int j = 0; j < chip.Count; j++)
                        {
                            DrawString(20 + 9 * j, 500 + 20 * i, $"{(int)chip[j].ENote}", 0xffffff);
                        }
                        DrawString(20 + 9 * chip.Count, 500 + 20 * i, ",", 0xffffff);
                    }
                }
                if (Game.MainTimer.State == 0)
                {
                    ListCommand = File.Command[Game.Course[0]];
                }
                else
                {
                    if (ListCommand.Count > 0 && Game.MainTimer.Value >= ListCommand[0].Time)
                    {
                        NowCommand = ListCommand[0];
                        ListCommand.RemoveAt(0);
                    }
                }
                if (NowCommand != null) DrawString(900 - 36, 500, $"Now:{(int)NowCommand.Time,6}:{NowCommand.Name}", 0xffffff);
                else DrawString(900 - 36, 500, $"Now:{0,6}:None", 0xffffff);
                for (int i = 0; i < ListCommand.Count; i++)
                {
                    if (i < 29) DrawString(900, 540 + 20 * i, $"{(int)(File.Command[Game.Course[0]][i].Time - Game.MainTimer.Value),6}:{File.Command[Game.Course[0]][i].Name}", 0xffffff);
                }
                if (Game.NowMeasure > 0)
                {
                    if (Game.NowMeasure <= File.Bar[Game.Course[0]].Count)
                    {
                        BarLine bar = File.Bar[Game.Course[0]][Game.NowMeasure - 1];
                        /*double num = 240000.0 / bar.BPM * bar.Measure;
                        double num2 = Game.MainTimer.Value - bar.Time;
                        DrawString(400, 500, $"num:{num}", 0xffffff);
                        DrawString(400, 520, $"num2:{num2}", 0xffffff);
                        int count = bar.Chip.Count;
                        DrawString(600, 500, $"Count:{count}", 0xffffff);
                        for (int j = 0; j < count; j++)
                        {
                            DrawString(200, 500 + 20 * j, $"{(int)bar.Chip[j].Time,6},{(int)bar.Chip[j].ENote},{bar.Chip[j].Bpm},{bar.Chip[j].Measure}", 0xffffff);
                            double num3 = (j - 0.5) * (double)num / count;
                            double num4 = (j + 0.5) * (double)num / count;
                            DrawString(400, 540 + 20 * j, $"{num3}", 0xffffff);
                            DrawString(600, 540 + 20 * j, $"{num4}", 0xffffff);
                        }*/
                        Chip chip = GetNotes.GetNowNote(bar.Chip, Game.MainTimer.Value, true);
                        if (chip != null)
                        {
                            DrawString(800, 500, $"{4 / chip.Measure}", 0xffffff);

                        }
                        
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
                    if (Game.NowMeasure > 0 && Bar.Count > 0)
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
                    int chiprange = File.Bar[course][i].Chip.Count;
                    int amount = InputType;
                    for (int j = 0; j < amount; j++)
                    {
                        double num3 = (j - 0.5) * (double)num / amount;
                        double num4 = (j + 0.5) * (double)num / amount;
                        int nownote = j * (chiprange / InputType);
                        if (num2 >= num3 && num2 < num4)
                        {
                            if (isDon)
                            {
                                if (File.Bar[course][i].Chip[nownote].ENote == ENote.Space)
                                {
                                    File.Bar[course][i].Chip[nownote].ENote = ENote.Don;
                                }
                                if (File.Bar[course][i].Chip[nownote].ENote == ENote.Don && KeyInput.ListPushing(PlayData.Data.LEFTDON) && KeyInput.ListPushing(PlayData.Data.RIGHTDON))
                                {
                                    File.Bar[course][i].Chip[nownote].ENote = ENote.DON;
                                }
                            }
                            else
                            {
                                if (File.Bar[course][i].Chip[nownote].ENote == ENote.Space)
                                {
                                    File.Bar[course][i].Chip[nownote].ENote = ENote.Ka;
                                }
                                if (File.Bar[course][i].Chip[nownote].ENote == ENote.Ka && KeyInput.ListPushing(PlayData.Data.LEFTKA) && KeyInput.ListPushing(PlayData.Data.RIGHTKA))
                                {
                                    File.Bar[course][i].Chip[nownote].ENote = ENote.KA;
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
            File.Command[course].Clear();
            ListMeasureCount = new List<int>();
            ListAllChip = new List<Chip>();
            double num = -File.Offset * 1000.0;
            while (num < GetSoundTotalTime(Game.MainSong.ID))
            {
                BarLine bar = new BarLine(num, File.Bpm, 1.0, 480);
                File.Bar[course].Add(bar);
                num += (long)(240000.0 / File.Bpm);
            }
        }

        public static void BarLoad(int course)
        {
            File.Bar[course].Clear();
            File.Command[course].Clear();
            ListMeasureCount = new List<int>();
            ListAllChip = new List<Chip>();
            if (Game.MainTJA[0].Courses[Game.Course[0]].IsEnable)
            {
                foreach (string str in Course[course])
                    GetMeasureCount(str);
                List<List<Chip>> ListChip = new List<List<Chip>>();
                foreach (string str in Course[course])
                    Parse(str, File.Bar[course], File.Command[course], ListChip);
                foreach (List<Chip> list1 in ListChip)
                    ListAllChip.AddRange(list1.ToArray());
                for (int i = 0; i < File.Bar[course].Count; i++)
                {
                    //TJAParse.Course.RollDoubledCheck(File.Bar[course][i].Chip);
                    RangeFix(File.Bar[course][i].Chip, 480);
                }
            }
            else BarInit(course);
        }
        public static void Parse(string str, List<BarLine> bar, List<Command> command, List<List<Chip>> listchip)
        {
            if (str.Length <= 0) return;
            if (str.StartsWith("#"))
            {
                Command c = new Command()
                {
                    Name = str,
                    Time = CreateParse.Time
                };
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
                    command.Add(c);

                }
                else if (str.StartsWith("#BPMCHANGE"))
                {
                    CreateParse.Bpm = double.Parse(str.Replace("#BPMCHANGE", "").Trim()) * PlayData.Data.PlaySpeed;
                    command.Add(c);
                }
                else if (str.StartsWith("#DELAY"))
                {
                    CreateParse.Time += double.Parse(str.Replace("#DELAY", "").Trim()) * 1000.0;
                    command.Add(c);
                }
                else if (str.StartsWith("#MEASURE"))
                {
                    var SplitSlash = str.Replace("#MEASURE", "").Trim().Split('/');
                    if (SplitSlash.Length > 1) CreateParse.Measure = double.Parse(SplitSlash[1]) / double.Parse(SplitSlash[0]);
                    command.Add(c);
                }
                else if (str.StartsWith("#BARLINEON"))
                {
                    CreateParse.ShowBarLine = true;
                    command.Add(c);
                }
                else if (str.StartsWith("#BARLINEOFF"))
                {
                    CreateParse.ShowBarLine = false;
                    command.Add(c);
                }
                else if (str.StartsWith("#GOGOSTART"))
                {
                    CreateParse.IsGogo = true;
                    command.Add(c);
                }
                else if (str.StartsWith("#GOGOEND"))
                {
                    CreateParse.IsGogo = false;
                    command.Add(c);
                }
                else if (str.StartsWith("#LYRIC"))
                {
                    CreateParse.LyricText = str.Replace("#LYRIC", "").Trim();
                    command.Add(c);
                }
                else if (str.StartsWith("#SUDDEN"))
                {
                    var SplitSlash = str.Replace("#SUDDEN", "").Trim().Split(' ');
                    if (SplitSlash.Length > 1) CreateParse.Sudden = new double[2] { double.Parse(SplitSlash[0]) * 1000.0, double.Parse(SplitSlash[1]) * 1000.0 };
                    else CreateParse.Sudden = new double[2] { double.Parse(SplitSlash[0]) * 1000.0, double.Parse(SplitSlash[0]) * 1000.0 };
                    command.Add(c);
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
            foreach (Command com in command)
                com.Time = Math.Round(com.Time, 4, MidpointRounding.AwayFromZero);
            foreach (List<Chip> list1 in listchip)
                foreach (Chip chip in list1)
                {
                    chip.Time = Math.Round(chip.Time, 4, MidpointRounding.AwayFromZero);
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

        public static void RangeFix(List<Chip> listchip, int range)
        {
            if (listchip.Count > range)
            {
                RangeRemove(listchip);
            }
            if (listchip.Count < range)
            {
                RangeAdd(listchip, range);
            }
            ReTime(listchip);
        }
        public static void RangeRemove(List<Chip> listchip)
        {
            for (int w = 0; w < 10; w++)
            {
                int count = 0;
                for (int i = 0; i < listchip.Count; i++)
                {
                    bool b = false;
                    for (int j = 0; j < File.Command[Game.Course[0]].Count; j++)
                    {
                        double time = File.Command[Game.Course[0]][j].Time;
                        if (listchip[i].Time == time)
                        {
                            b = true;
                            break;
                        }
                    }
                    if (i % 2 > 0 && listchip[i].ENote == ENote.Space && !b)
                    {
                        count++;
                    }
                }
                if (count >= listchip.Count / 2)
                {
                    for (int i = listchip.Count - 1; i >= 0; i--)
                    {
                        if (i % 2 == 1)
                        {
                            listchip.RemoveAt(i);
                        }
                    }
                }
                else break;
            }
            for (int w = 0; w < 10; w++)
            {
                int count = 0;
                for (int i = 0; i < listchip.Count; i++)
                {
                    bool b = false;
                    for (int j = 0; j < File.Command[Game.Course[0]].Count; j++)
                    {
                        double time = File.Command[Game.Course[0]][j].Time;
                        if (listchip[i].Time == time)
                        {
                            b = true;
                            break;
                        }
                    }
                    if (i % 3 > 0 && listchip[i].ENote == ENote.Space && !b)
                    {
                        count++;
                    }
                }
                if (count >= listchip.Count * 2 / 3)
                {
                    for (int i = listchip.Count - 1; i >= 0; i--)
                    {
                        if (i % 3 > 0)
                        {
                            listchip.RemoveAt(i);
                        }
                    }
                }
                else break;
            }
            for (int w = 0; w < 10; w++)
            {
                int count = 0;
                for (int i = 0; i < listchip.Count; i++)
                {
                    bool b = false;
                    for (int j = 0; j < File.Command[Game.Course[0]].Count; j++)
                    {
                        double time = File.Command[Game.Course[0]][j].Time;
                        if (listchip[i].Time == time)
                        {
                            b = true;
                            break;
                        }
                    }
                    if (i % 5 > 0 && listchip[i].ENote == ENote.Space && !b)
                    {
                        count++;
                    }
                }
                if (count >= listchip.Count * 4 / 5)
                {
                    for (int i = listchip.Count - 1; i >= 0; i--)
                    {
                        if (i % 5 > 0)
                        {
                            listchip.RemoveAt(i);
                        }
                    }
                }
                else break;
            }
        }
        public static void RangeAdd(List<Chip> listchip, int range)
        {
            Chip chip = new Chip()
            {
                Time = listchip[0].Time,
                Bpm = listchip[0].Bpm,
                Scroll = 1.0,
                Measure = listchip[0].Measure,
                EChip = EChip.Note,
                ENote = ENote.Space,
                CanShow = true,
                Sudden = new double[2] { 0.0, 0.0 }
            };
            for (int w = 0; w < 20; w++)
            {
                int num = range / listchip.Count;
                if (listchip.Count < range)
                {
                    if (num % 5 == 0)
                    {
                        for (int i = listchip.Count; i >= 1; i--)
                        {
                            for (int m = 0; m < 4; m++)
                            {
                                listchip.Insert(i, chip);
                            }
                        }
                    }
                    else if (num % 3 == 0)
                    {
                        for (int i = listchip.Count; i >= 1; i--)
                        {
                            for (int m = 0; m < 2; m++)
                            {
                                listchip.Insert(i, chip);
                            }
                        }
                    }
                    else if (num % 2 == 0)
                    {
                        for (int i = listchip.Count; i >= 1; i--)
                        {
                            listchip.Insert(i, chip);
                        }
                    }
                }
                else break;
            }
        }
        public static void ReTime(List<Chip> listchip)
        {
            if (listchip.Count == 0) return;
            double time = listchip[0].Time;
            for (int i = 0; i < listchip.Count; i++)
            {
                Chip chip = new Chip()
                {
                    Time = time,
                    Bpm = listchip[i].Bpm,
                    Scroll = listchip[i].Scroll,
                    Measure = listchip[i].Measure,
                    EChip = EChip.Note,
                    ENote = listchip[i].ENote,
                    CanShow = true,
                    Sudden = new double[2] { 0.0, 0.0 }
                };
                listchip[i] = chip;
                time += (240000.0 / listchip.Count / listchip[i].Bpm / listchip[i].Measure);
            }
            foreach (Chip chip in listchip)
            {
                chip.Time = Math.Round(chip.Time, 4, MidpointRounding.AwayFromZero);
            }
        }

        public static void AddMeasure()
        {
            BarLine lastbar = File.Bar[Game.Course[0]][File.Bar[Game.Course[0]].Count - 1];
            double num = lastbar.Time + 240000.0 / lastbar.BPM / lastbar.Measure;
            BarLine bar = new BarLine(num, lastbar.BPM, lastbar.Measure, 480);
            File.Bar[Game.Course[0]].Add(bar);
            DrawLog.Draw("小節を追加しました。");
        }
        public static void AddCommand(string str)
        {
            foreach (BarLine bar in File.Bar[Game.Course[0]])
            {
                if (bar == SelectedChip.Item1)
                {
                    foreach (Chip chip in bar.Chip)
                    {
                        if (chip.Time == SelectedChip.Item2.Time)
                        {
                            Command com = new Command()
                            {
                                Time = chip.Time,
                                Name = str
                            };
                            File.Command[Game.Course[0]].Add(com);
                            DrawLog.Draw("命令を追加しました。");
                        }
                    }
                }
            }
            if (File.Command[Game.Course[0]].Count > 0) File.Command[Game.Course[0]].Sort((a, b) => { int result = (int)a.Time - (int)b.Time; return result != 0 ? result : SortCommand(a.Name) - SortCommand(b.Name); });
        }
        public static void DeleteCommand(string str)
        {
            double time = 0;
            foreach (BarLine bar in File.Bar[Game.Course[0]])
            {
                if (bar == SelectedChip.Item1)
                {
                    foreach (Chip chip in bar.Chip)
                    {
                        if (chip.Time == SelectedChip.Item2.Time)
                        {
                            time = chip.Time;
                        }
                    }
                }
            }
            bool success = false;
            for (int i = File.Command[Game.Course[0]].Count - 1; i >= 0; i--)
            {
                Command command = File.Command[Game.Course[0]][i];
                if (command.Time == time && command.Name.StartsWith(str))
                {
                    File.Command[Game.Course[0]].RemoveAt(i);
                    success = true;
                }
            }
            if (success) DrawLog.Draw("命令を削除しました。");
            else DrawLog.Draw("記入された命令が見つかりませんでした。");
        }

        public static int SortCommand(string str)
        {
            if (str.StartsWith("#BPMCHANGE")) return 0;
            else if (str.StartsWith("#MEASURE")) return 1;
            else if (str.StartsWith("#GOGOSTART") || str.StartsWith("#GOGOEND")) return 2;
            else if (str.StartsWith("#BARLINEON") || str.StartsWith("#BARLINEOFF")) return 3;
            else if (str.StartsWith("#DELAY")) return 4;
            else if (str.StartsWith("#SCROLL")) return 5;
            else if (str.StartsWith("#SUDDEN")) return 6;
            else if (str.StartsWith("#LYRIC")) return 7;
            return 8;
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
                if (!string.IsNullOrEmpty(File.BGImage)) streamWriter.WriteLine($"BGIMAGE:{File.BGImage}");
                if (!string.IsNullOrEmpty(File.BGMovie)) streamWriter.WriteLine($"BGMOVIE:{File.BGImage}");
                streamWriter.WriteLine($"SONGVOL:{File.SongVol}");
                streamWriter.WriteLine($"SEVOL:{File.SeVol}");
                streamWriter.WriteLine($"DEMOSTART:{File.DemoStart}");
                if (!string.IsNullOrEmpty(File.Genre)) streamWriter.WriteLine($"GENRE:{File.Genre}");
                streamWriter.WriteLine("");

                for (int i = 4; i >= 0; i--)
                {
                    if (File.Bar[i].Count > 0)
                    {
                        streamWriter.WriteLine($"COURSE:{i}");
                        streamWriter.WriteLine($"LEVEL:{File.Level[i]}");
                        if (File.Total[i] > 0) streamWriter.WriteLine($"{File.Total[i]}");
                        streamWriter.WriteLine("");

                        streamWriter.WriteLine("#START");
                        int count = 0;
                        foreach (BarLine bar in File.Bar[i])
                        {
                            RangeFix(bar.Chip, 1);
                            for (int j = 0; j < bar.Chip.Count; j++)
                            {
                                if (count < File.Command[i].Count && File.Command[i][count].Time <= bar.Chip[j].Time)
                                {
                                    streamWriter.WriteLine("");
                                    for (int m = 0; m < 20; m++)
                                    {
                                        if (count < File.Command[i].Count && File.Command[i][count].Time <= bar.Chip[j].Time)
                                        {
                                            streamWriter.WriteLine(File.Command[i][count].Name);
                                            count++;
                                        }
                                        else break;
                                    }
                                }
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
        }

        public static bool CreateMode, Preview, InfoMenu, Selecting, Mapping, RollEnd, ad, MetroL, Metro, Edited, DeleteMode;
        public static List<string> AllText;
        public static List<string>[] Course = new List<string>[5];
        public static List<int> ListMeasureCount;
        public static List<Chip> ListAllChip;
        public static List<Command> ListCommand;
        public static Command NowCommand;
        public static int NowScroll, Cursor, InputType, NowInput, NowColor, MeasureCount, CommandLayer, mg, mb;
        public static Chip RollBegin;
        public static (BarLine, Chip) SelectedChip;
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
                Command[i] = new List<Command>();
            }
        }

        public string Title, SubTitle, Wave, BGImage, BGMovie, Genre;
        public double Bpm, Offset, DemoStart;
        public int SongVol, SeVol;
        public int[] Level = new int[5];
        public double[] Total = new double[5];
        public List<BarLine>[] Bar = new List<BarLine>[5];
        public List<Command>[] Command = new List<Command>[5];
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
                    CanShow = true,
                    Sudden = new double[2] { 0.0, 0.0 }
                };
                Chip.Add(chip);
                num += (long)(240000.0 / Amount / BPM);
            }
            foreach (Chip chip in Chip)
                chip.Time = Math.Round(chip.Time, 4, MidpointRounding.AwayFromZero);
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

    public class Command
    {
        public double Time;
        public string Name;
    }
}
