using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using static DxLibDLL.DX;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class NewCreate
    {
        public static int Size, Count, Line, DisplayLine, NowInput, InputType, mg, mb;
        public static int[] StartLine = new int[5];
        public static bool Enable, Mapping;
        public static List<string> AllText, Header;
        public static List<string>[] CourseText = new List<string>[5], CourseHeader = new List<string>[5];
        public static List<int>[] MeasureList = new List<int>[5];
        public static Handle Handle;
        public static Counter MetoFlash;
        public static DateTime FileTime;

        public static void Init()
        {
            if (NewGame.Dan != null) return;

            Count = 0;
            Line = 0;
            DisplayLine = 0;
            Size = 24;
            NowInput = 0;
            InputType = 4;
            Enable = false;
            Handle = new Handle(Size);
            MetoFlash = new Counter(0, 100, 1000, false);
            Read();
        }
        public static void Read()
        {
            FileTime = DateTime.Now;

            AllText = new List<string>();
            Header = new List<string>();
            using (StreamReader sr = new StreamReader(NewGame.Path, Encoding.GetEncoding("SHIFT_JIS")))
            {
                while (sr.Peek() > -1)
                {
                    AllText.Add(sr.ReadLine());
                }
            }
            for (int i = 0; i < 5; i++)
            {
                MeasureList[i] = new List<int>();
                CourseHeader[i] = new List<string>();
                CourseText[i] = new List<string>();
            }
            for (int i = 0; i < AllText.Count; i++)
            {
                string[] split = AllText[i].Split(':');
                if (split.Length >= 2)
                {
                    switch (split[0])
                    {
                        case "COURSE":
                            DCourse = (ECourse)Course.GetCourse(split[1]);
                            break;
                    }
                }
                else
                {
                    if (AllText[i].StartsWith("#START") || AllText[i].StartsWith("#BMSCROLL") || AllText[i].StartsWith("#HBSCROLL"))
                    {
                        DParse = true;
                    }
                    else if (AllText[i].StartsWith("#END"))
                    {
                        DParse = false;
                        CourseText[(int)DCourse].Add(AllText[i]);
                    }
                }

                if (DParse) CourseText[(int)DCourse].Add(AllText[i]);
                else if (!string.IsNullOrEmpty(AllText[i]))
                {
                    if (split[0] == "COURSE" || split[0] == "LEVEL" || split[0] == "BALLOON" || split[0] == "TOTAL" || split[0] == "SCOREINIT" || split[0] == "SCOREDIFF" || split[0].StartsWith("DIFPLUS") || split[0].StartsWith("NOTESDESIGNER"))
                        CourseHeader[(int)DCourse].Add(AllText[i]);
                    else if (!AllText[i].StartsWith("#END")) Header.Add(AllText[i]);
                }
                
            }
            for (int i = 0; i < CourseText.Length; i++)
            {
                for (int j = 0; j < CourseText[i].Count; j++)
                {
                    if (CourseText[i][j].Contains(","))
                    {
                        MeasureList[i].Add(j);
                    }
                }
            }
        }
        public static ECourse DCourse;
        public static bool DParse;

        public static void Draw()
        {
            if (!Enable) return;

            Drawing.Text(300, 200, "Create Mode", 0xa06000);

            Drawing.Text(472, 40, $"{InputType,2}", 0xffffff);
            if (Mapping) Drawing.Circle(478, 20, 8, 0xff0000);

            if (MetoFlash.State != 0)
            {
                Drawing.Circle(470, 175, mb == 0 ? 24 : 16, mb == 0 ? 0x00ff00 : 0xffff00, (int)(255 - 255 * ((double)MetoFlash.Value / MetoFlash.End)));
            }

            int height = (int)(Size * 1.25);
            int disp = ((1080 - (200 + height * CourseHeader[NewGame.Course[0]].Count)) / height);
            for (int i = 0; i < (disp > Header.Count ? Header.Count : disp); i++)
            {
                string text = Header[i];
                if (200 + 20 * i < 1080) Drawing.Text(970, 250 + height * i, text, Handle);
            }
            for (int i = 0; i < (disp > CourseHeader[NewGame.Course[0]].Count ? CourseHeader[NewGame.Course[0]].Count : disp); i++)
            {
                string text = CourseHeader[NewGame.Course[0]][i];
                if (200 + 20 * i < 1080) Drawing.Text(10, 200 + height * i, text, Handle);
            }
            for (int i = 0; i < (disp > CourseText[NewGame.Course[0]].Count ? CourseText[NewGame.Course[0]].Count : disp); i++)
            {
                string text = CourseText[NewGame.Course[0]][i + DisplayLine];
                int start = 200 + height * CourseHeader[NewGame.Course[0]].Count;
                if (start + 20 * i < 1080) Drawing.Text(10, start + height * i, text, Handle);
            }
            Drawing.Text(600, 250, $"{Count},{Line}", Handle);
            char ch = Count < CourseText[NewGame.Course[0]][Line].Length ? CourseText[NewGame.Course[0]][Line][Count] : ' ';
            Drawing.Box(10 + Drawing.TextWidth(CourseText[NewGame.Course[0]][Line], Count, Handle), 200 + (height - 4) + height * CourseHeader[NewGame.Course[0]].Count + height * (Line - DisplayLine), Drawing.TextWidth($"{ch}", Handle), 4, 0xffff00);

#if DEBUG
            for (int i = 0; i < MeasureList[NewGame.Course[0]].Count; i++)
            {
                if (MeasureList[NewGame.Course[0]][i] > Line)
                {
                    Drawing.Text(800, 300, $"{i},{MeasureList[NewGame.Course[0]][i]}");
                    break;
                }
            }
#endif
        }

        public static void DrawLaneGrid()
        {
            if (!Enable) return;

            if (NewGame.NowState == EState.None)
            {
                Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
                Point Point = Enable ? createpoint[0] : NewNotes.NotesP[0];
                float width = (1920 - (Point.X - 22)) / (float)InputType;
                int[] color = new int[2];
                switch (InputType)
                {
                    case 4:
                        color = new int[2] { 0xff0000, 0x0000ff };
                        break;
                    case 8:
                        color = new int[2] { 0xff0000, 0x0000ff };
                        break;
                    case 12:
                        color = new int[3] { 0xff0000, 0x00ff00, 0x0000ff };
                        break;
                    case 16:
                        color = new int[4] { 0xff0000, 0xffff00, 0x0000ff, 0xffff00 };
                        break;
                    case 20:
                        color = new int[5] { 0xff0000, 0xffff00, 0x00ff00, 0x00ffff, 0x0000ff };
                        break;
                    case 24:
                        color = new int[6] { 0xff0000, 0x00ffff, 0x00ff00, 0x0000ff, 0x00ff00, 0x00ffff };
                        break;
                    case 32:
                        color = new int[8] { 0xff0000, 0xff8000, 0xffff00, 0xff8000, 0x0000ff, 0xff8000, 0xffff00, 0xff8000 };
                        break;
                    case 48:
                        color = new int[12] { 0xff0000, 0xff00ff, 0x00ffff, 0xffff00, 0x00ff00, 0xff00ff, 0x0000ff, 0xff00ff, 0x00ff00, 0xffff00, 0x00ffff, 0xff00ff };
                        break;
                }

                for (int i = 0; i < InputType; i++)
                {
                    float x = Point.X - 22 + i * width;
                    if (Mouse.X >= x && Mouse.X < x + width && Mouse.Y >= Point.Y && Mouse.Y < Point.Y + 195)
                    {
                        Drawing.Box(x, Point.Y, width, 195, color[i % (InputType == 4 ? 2 : InputType / 4)], 64);
                    }
                }
                for (int i = 0; i < InputType; i++)
                {
                    float x = Point.X - 22 + i * width;
                    Drawing.Box(x, Point.Y + 191, width, 4, color[i % (InputType == 4 ? 2 : InputType / 4)]);
                }
            }
        }
        public static void DrawGhost(List<Chip> chips)
        {
            if (!Enable) return;

            if (NewGame.NowState == EState.None)
            {
                Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
                Point Point = Enable ? createpoint[0] : NewNotes.NotesP[0];
                float width = (1920 - (Point.X - 22)) / (float)InputType;
                for (int i = 0; i < InputType; i++)
                {
                    float x = Point.X - 22 + i * width;
                    if (Mouse.X >= x && Mouse.X < x + width && Mouse.Y >= Point.Y && Mouse.Y < Point.Y + 195)
                    {
                        Chip chip = chips[i];
                        double xx = NewNotes.NoteX(chip, 0);
                        Tx.Game_Notes.Draw(Point.X + xx, Point.Y, new Rectangle(195 * 15, 0, 195, 195));
                    }
                }
            }
        }

        public static void Update()
        {
            if (!Enable) return;

            MetoFlash.Tick();
            List<Bar> Bar = NewGame.Bars[0];
            int mej = Process.NowBar(Bar) != null ? Process.NowBar(Bar).Number : 0;
            if (NewGame.NowState == EState.None)
            {
                mg = mej - 1;
                mb = 0;
            }
            else
            {
                if (mej > 0 && Bar.Count > 0)
                {
                    if (mej > mg && NewGame.Timer.Value >= Bar[mej - 1].Time)
                    {
                        Sfx.Metronome_Bar.Play();
                        mg = mej;
                        mb = 0;
                        MetoFlash = new Counter(0, (long)(36000 / Bar[mej - 1].BPM), 1000, false);
                        MetoFlash.Reset();
                        MetoFlash.Start();
                    }
                    for (int j = 1; j < Bar[mej - 1].Measure * 4; j++)
                    {
                        double time = Bar[mej - 1].Time + j * (60000.0 / Bar[mej - 1].BPM);
                        if (j > mb && NewGame.Timer.Value >= time)
                        {
                            Sfx.Metronome.Play();
                            mb = j;
                            MetoFlash = new Counter(0, (long)(24000 / Bar[mej - 1].BPM), 1000, false);
                            MetoFlash.Reset();
                            MetoFlash.Start();
                        }
                    }
                }
            }

            if (Input.IsEnable)
            {
                if (Key.IsPushed(EKey.Enter))
                {
                    CourseText[NewGame.Course[0]][Line] = Input.Text;
                    Input.End();
                    Save();
                }
            }
            else
            {
                if (Key.IsHolding(EKey.Left, 250, 50)) if (Count-- <= 0) Count = 0;
                if (Key.IsHolding(EKey.Right, 250, 50)) { if (Count++ >= CourseText[NewGame.Course[0]][Line].Length) Count = CourseText[NewGame.Course[0]][Line].Length; if (Count < 0) Count = 0; }
                int height = (int)(Size * 1.25);
                int disp = ((1080 - (200 + height * CourseHeader[NewGame.Course[0]].Count)) / height) - 1;
                if (Key.IsHolding(EKey.Up, 250, 50))
                {
                    if (Key.IsPushing(EKey.LCtrl) || Key.IsPushing(EKey.RCtrl))
                    {
                        Line -= disp;
                    }
                    if (Line-- <= 0) Line = 0;
                    if (Count >= CourseText[NewGame.Course[0]][Line].Length) Count = CourseText[NewGame.Course[0]][Line].Length;
                    if (Count < 0) Count = 0;
                    if (Line <= DisplayLine) DisplayLine = Line;

                    for (int i = MeasureList[NewGame.Course[0]].Count - 1; i >= 0; i--)
                    {
                        if (Line <= MeasureList[NewGame.Course[0]][i])
                        {
                            MeasureSet(i + 1);
                        }
                    }
                    if (CourseText[NewGame.Course[0]][Line].StartsWith("#START") || CourseText[NewGame.Course[0]][Line].StartsWith("#BMSCROLL") || CourseText[NewGame.Course[0]][Line].StartsWith("#HBSCROLL"))
                    {
                        MeasureSet(0);
                    }
                }
                if (Key.IsHolding(EKey.Down, 250, 50))
                {
                    if (Key.IsPushing(EKey.LCtrl) || Key.IsPushing(EKey.RCtrl))
                    {
                        Line += disp;
                    }
                    if (Line++ >= CourseText[NewGame.Course[0]].Count - 1) Line = CourseText[NewGame.Course[0]].Count - 1;
                    if (Count >= CourseText[NewGame.Course[0]][Line].Length) Count = CourseText[NewGame.Course[0]][Line].Length;
                    if (Count < 0) Count = 0;
                    if (Line >= DisplayLine + disp) DisplayLine = Line - disp;

                    for (int i = MeasureList[NewGame.Course[0]].Count - 1; i >= 0; i--)
                    {
                        if (Line <= MeasureList[NewGame.Course[0]][i])
                        {
                            MeasureSet(i + 1);
                        }
                    }
                    if (CourseText[NewGame.Course[0]][Line].StartsWith("#START") || CourseText[NewGame.Course[0]][Line].StartsWith("#BMSCROLL") || CourseText[NewGame.Course[0]][Line].StartsWith("#HBSCROLL"))
                    {
                        MeasureSet(0);
                    }
                }

                if (Key.IsPushed(EKey.Enter))
                {
                    if (Key.IsPushing(EKey.LShift) || Key.IsPushing(EKey.RShift))
                    {
                        if (Line >= CourseText[NewGame.Course[0]].Count - 1) CourseText[NewGame.Course[0]].Add("");
                        else CourseText[NewGame.Course[0]].Insert(Line + 1, "");
                        Line++;
                        Save();
                    }
                    else
                    {
                        Input.Init();
                        Input.Text = CourseText[NewGame.Course[0]][Line];
                    }
                }
                if (Key.IsHolding(EKey.Back, 500, 100))
                {
                    Delete();
                }
                if (Key.IsHolding(EKey.Comma, 500, 100))
                {
                    Tipe(",");
                }
                if (Key.IsHolding(EKey.Key_1, 500, 100) || Key.IsHolding(EKey.NumPad_1, 500, 100))
                {
                    Tipe("1");
                }
                if (Key.IsHolding(EKey.Key_2, 500, 100) || Key.IsHolding(EKey.NumPad_2, 500, 100))
                {
                    Tipe("2");
                }
                if (Key.IsHolding(EKey.Key_3, 500, 100) || Key.IsHolding(EKey.NumPad_3, 500, 100))
                {
                    Tipe("3");
                }
                if (Key.IsHolding(EKey.Key_4, 500, 100) || Key.IsHolding(EKey.NumPad_4, 500, 100))
                {
                    Tipe("4");
                }
                if (Key.IsHolding(EKey.Key_5, 500, 100) || Key.IsHolding(EKey.NumPad_5, 500, 100))
                {
                    Tipe("5");
                }
                if (Key.IsHolding(EKey.Key_6, 500, 100) || Key.IsHolding(EKey.NumPad_6, 500, 100))
                {
                    Tipe("6");
                }
                if (Key.IsHolding(EKey.Key_7, 500, 100) || Key.IsHolding(EKey.NumPad_7, 500, 100))
                {
                    Tipe("7");
                }
                if (Key.IsHolding(EKey.Key_8, 500, 100) || Key.IsHolding(EKey.NumPad_8, 500, 100))
                {
                    Tipe("8");
                }
                if (Key.IsHolding(EKey.Key_9, 500, 100) || Key.IsHolding(EKey.NumPad_9, 500, 100))
                {
                    Tipe("9");
                }
                if (Key.IsHolding(EKey.Key_0, 500, 100) || Key.IsHolding(EKey.NumPad_0, 500, 100))
                {
                    Tipe("0");
                }

                int[] inputlist = new int[8] { 4, 8, 12, 16, 20, 24, 32, 48 };
                if (Key.IsPushed(EKey.NumPad_Divide))
                {
                    Sfx.Ka[0].Play();
                    if (NowInput-- <= 0) NowInput = 0;
                    InputType = inputlist[NowInput];
                }
                if (Key.IsPushed(EKey.NumPad_Multiply))
                {
                    Sfx.Ka[0].Play();
                    if (NowInput++ >= 7) NowInput = 7;
                    InputType = inputlist[NowInput];
                }

                if (Key.IsPushed(PlayData.Data.RealTimeMapping))
                {
                    Mapping = !Mapping;
                    if (Mapping)
                    {
                        TextLog.Draw("マッピングを開始します…");
                    }
                    if (!Mapping)
                    {
                        TextLog.Draw("マッピングをセーブしました!");
                        SaveBar();
                    }
                }
                if (Key.IsPushed(PlayData.Data.SaveFile))
                {
                    TextLog.Draw("セーブしました!");
                    Save();
                }
            }

        }

        public static void Tipe(string str)
        {
            if (CourseText[NewGame.Course[0]][Line].Length <= Count) CourseText[NewGame.Course[0]][Line] = CourseText[NewGame.Course[0]][Line] + " ";
            CourseText[NewGame.Course[0]][Line] = CourseText[NewGame.Course[0]][Line].Remove(Count, 1).Insert(Count, str);
            Count++;
            Save();
        }
        public static void Delete()
        {
            if (CourseText[NewGame.Course[0]][Line].Length == 0)
            {
                CourseText[NewGame.Course[0]].RemoveAt(Line);
                if (Line > 0) Line--;
            }
            else
            {
                if (CourseText[NewGame.Course[0]][Line].Length > Count) CourseText[NewGame.Course[0]][Line] = CourseText[NewGame.Course[0]][Line].Remove(Count, 1);
                if (CourseText[NewGame.Course[0]][Line].Length <= Count && CourseText[NewGame.Course[0]][Line].Length > 0) Count--;
            }
            Save();
        }

        public static void Save()
        {
            List<string> strings = new List<string>();
            strings.AddRange(Header);
            for (int i = 4; i >= 0; i--)
            {
                bool add = false;
                if (CourseText[i] != null)
                {
                    for (int j = 0; j < CourseText[i].Count; j++)
                    {
                        if (CourseText[i][j].Contains(",")) { add = true; break; }
                    }
                    if (add)
                    {
                        strings.Add("");
                        strings.AddRange(CourseHeader[i]);
                        strings.Add("");
                        strings.AddRange(CourseText[i]);
                    }
                }
            }
            ConfigIni file = new ConfigIni()
            {
                TextList = strings
            };
            file.SaveConfig(NewGame.Path);
        }

        public static void MeasureSet(int value)
        {
            if (NewGame.NowState > EState.None) return;

            if (value > NewGame.StartMeasure)
            {
                for (int i = NewGame.StartMeasure; i < value; i++)
                {
                    NewGame.MeasureUp();
                }
            }
            if (value < NewGame.StartMeasure)
            {
                for (int i = value; i < NewGame.StartMeasure; i++)
                {
                    NewGame.MeasureDown();
                }
            }
        }

        public static void InputN(bool isDon)
        {
            List<Bar> Bar = NewGame.Bars[0];
            int count = Process.NowBar(Bar) != null ? Process.NowBar(Bar).Number : 0;

            foreach (Bar bar in Bar)
                RangeFix(bar.Chip, InputType == 20 ? 480 : 96);
            for (int i = count - 1; i < count + 3; i++)
            {
                if (i >= 0 && i < Bar.Count)
                {
                    double num = 240000.0 / Bar[i].BPM * Bar[i].Measure;
                    double num2 = NewGame.Timer.Value - Bar[i].Time;
                    int chiprange = Bar[i].Chip.Count;
                    int amount = InputType;
                    for (int j = 0; j < amount; j++)
                    {
                        double num3 = (j - 0.5) * (double)num / amount;
                        double num4 = (j + 0.5) * (double)num / amount;
                        int nownote = j * (chiprange / InputType);
                        if (num2 >= num3 && num2 < num4)
                        {
                            TextLog.Draw($"{nownote}");
                            if (isDon)
                            {
                                if (Bar[i].Chip[nownote].ENote == ENote.Space)
                                {
                                    Bar[i].Chip[nownote].ENote = ENote.Don;
                                }
                                if (Bar[i].Chip[nownote].ENote == ENote.Don && Key.HoldTime(PlayData.Data.LEFTDON) > 0 && Key.HoldTime(PlayData.Data.RIGHTDON) > 0)// && Key.HoldTime(PlayData.Data.LEFTDON) < 500 && Key.HoldTime(PlayData.Data.RIGHTDON) < 500)
                                {
                                    Bar[i].Chip[nownote].ENote = ENote.DON;
                                }
                            }
                            else
                            {
                                if (Bar[i].Chip[nownote].ENote == ENote.Space)
                                {
                                    Bar[i].Chip[nownote].ENote = ENote.Ka;
                                }
                                if (Bar[i].Chip[nownote].ENote == ENote.Ka && Key.HoldTime(PlayData.Data.LEFTKA) > 0 && Key.HoldTime(PlayData.Data.RIGHTKA) > 0)// && Key.HoldTime(PlayData.Data.LEFTKA) < 500 && Key.HoldTime(PlayData.Data.RIGHTKA) < 500)
                                {
                                    Bar[i].Chip[nownote].ENote = ENote.KA;
                                }
                            }
                            Process.PlaySound(Bar[i].Chip[nownote].ENote, 0); 
                        }
                    }
                }
            }
            foreach (Bar bar in Bar)
                RangeFix(bar.Chip, 1);
        }

        /*public static void BarInit(int course)
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
            if (SongData.NowTJA[0].Courses[Game.Course[0]].IsEnable)
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
        }*/

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
                    for (int j = 0; j < NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand.Count; j++)
                    {
                        double time = NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand[j].Time;
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
                    for (int j = 0; j < NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand.Count; j++)
                    {
                        double time = NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand[j].Time;
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
                    for (int j = 0; j < NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand.Count; j++)
                    {
                        double time = NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand[j].Time;
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

        /*public static void AddMeasure()
        {
            BarLine lastbar = File.Bar[Game.Course[0]][File.Bar[Game.Course[0]].Count - 1];
            double num = lastbar.Time + 240000.0 / lastbar.BPM / lastbar.Measure;
            BarLine bar = new BarLine(num, lastbar.BPM, lastbar.Measure, 480);
            File.Bar[Game.Course[0]].Add(bar);
            TextLog.Draw("小節を追加しました。");
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
                            TextLog.Draw("命令を追加しました。");
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
            if (success) TextLog.Draw("命令を削除しました。");
            else TextLog.Draw("記入された命令が見つかりませんでした。");
        }*/

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

        public static void SaveBar()
        {
            List<string> strings = new List<string>();
            strings.AddRange(Header);
            for (int i = 4; i >= 0; i--)
            {
                if (i == NewGame.Course[0])
                {
                    if (NewGame.Bars[0].Count > 0)
                    {
                        strings.Add("");
                        strings.AddRange(CourseHeader[i]);
                        strings.Add("");

                        strings.Add("#START");
                        int count = 0;
                        foreach (Bar bar in NewGame.Bars[0])
                        {
                            RangeFix(bar.Chip, 1);
                            string line = "";
                            for (int j = 0; j < bar.Chip.Count; j++)
                            {
                                if (count < NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand.Count && NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand[count].Time <= bar.Chip[j].Time)
                                {
                                    for (int m = 0; m < 20; m++)
                                    {
                                        if (count < NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand.Count && NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand[count].Time <= bar.Chip[j].Time)
                                        {
                                            line += NewGame.TJA[0].Courses[NewGame.Course[0]].ListCommand[count].Name;
                                            count++;
                                        }
                                        else break;
                                    }
                                }
                                line += $"{(int)bar.Chip[j].ENote}";
                            }
                            strings.Add(line + ",");
                        }
                        strings.Add("");
                        strings.Add("#END");
                        strings.Add("");
                    }
                }
                else
                {
                    bool add = false;
                    if (CourseText[i] != null)
                    {
                        for (int j = 0; j < CourseText[i].Count; j++)
                        {
                            if (CourseText[i][j].Contains(",")) { add = true; break; }
                        }
                        if (add)
                        {
                            strings.Add("");
                            strings.AddRange(CourseHeader[i]);
                            strings.Add("");
                            strings.AddRange(CourseText[i]);
                        }
                    }
                }
            }
            ConfigIni file = new ConfigIni()
            {
                TextList = strings
            };
            file.SaveConfig(NewGame.Path);
        }
    }

    public class Create : Scene
    {
        public override void Enable()
        {
            CreateMode = SongSelect.PlayMode == 1 ? true : false;
            Preview = true;
            InfoMenu = false;
            Selecting = false;
            Mapping = false;
            Metro = false;
            MetroL = false;
            Edited = false;
            File = new TJAFile(SongData.NowTJA[0]);
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
            using (StreamReader sr = new StreamReader(SongData.NowTJA[0].Path, Encoding.GetEncoding("Shift_JIS")))
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
                //Tx.Game_Notes.Draw(195 * 4 + 98, 520, new Rectangle(195 * 6, 0, 195 * 2, 195));
                //Tx.Game_Notes.Draw(0, 520, new Rectangle(195, 0, 195 * 5, 195));
                //Tx.Game_Notes.Draw(98, 520 + 195, new Rectangle(195 * 9, 0, 195 * 2, 195));
                //Tx.Game_Notes.Draw(0, 520 + 195, new Rectangle(195 * 8, 0, 195, 195));
                //Tx.Game_Notes.Draw(195 * 2, 520 + 195, new Rectangle(195 * 11, 0, 195 * 4, 195));

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
                        Drawing.Box(1916, y, 1920, y + (width < 4 ? 4 : width), 0xffff00);
                    }
                    for (int i = 0; i < (Course[Game.Course[0]].Count > 29 ? 29 : Course[Game.Course[0]].Count); i++)
                    {
                        Drawing.Text(1280, 495 + 20 * i, Course[Game.Course[0]][i + NowScroll], 0xffffff);
                    }
                }

                Drawing.Text(472, 350, $"{InputType,2}", 0xffffff);
                if (Mapping) DrawCircleAA(478, 330, 8, 256, 0xff0000);

                Drawing.Box(0, 0, 496, Notes.NotesP[0].Y - 4, 0x000000);
                Drawing.Text(40, 10, $"Title:{(Input.IsEnable && Cursor == 0 ? Input.Text : File.Title)}", Selecting && Cursor == 0 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 30, $"SubTitle:{(Input.IsEnable && Cursor == 1 ? Input.Text : File.SubTitle)}", Selecting && Cursor == 1 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 50, $"Wave:{(Input.IsEnable && Cursor == 2 ? Input.Text : File.Wave)}", Selecting && Cursor == 2 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 70, $"BGImage:{(Input.IsEnable && Cursor == 3 ? Input.Text : File.BGImage)}", Selecting && Cursor == 3 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 90, $"BGMovie:{(Input.IsEnable && Cursor == 4 ? Input.Text : File.BGMovie)}", Selecting && Cursor == 4 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 110, $"Bpm:{(Input.IsEnable && Cursor == 5 ? Input.Text : File.Bpm.ToString())}", Selecting && Cursor == 5 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 130, $"Offset:{(Input.IsEnable && Cursor == 6 ? Input.Text : File.Offset.ToString())}", Selecting && Cursor == 6 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 150, $"SongVol:{(Input.IsEnable && Cursor == 7 ? Input.Text : File.SongVol.ToString())}", Selecting && Cursor == 7 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 170, $"SeVol:{(Input.IsEnable && Cursor == 8 ? Input.Text : File.SeVol.ToString())}", Selecting && Cursor == 8 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 190, $"DemoStart:{(Input.IsEnable && Cursor == 9 ? Input.Text : File.DemoStart.ToString())}", Selecting && Cursor == 9 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 210, $"Genre:{(Input.IsEnable && Cursor == 10 ? Input.Text : File.Genre)}", Selecting && Cursor == 10 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 230, $"NowCourse:{(ECourse)Game.Course[0]}", Selecting && Cursor == 11 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 250, $"Level:{(Input.IsEnable && Cursor == 12 ? Input.Text : File.Level[Game.Course[0]].ToString())}", Selecting && Cursor == 12 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 270, $"Total:{(Input.IsEnable && Cursor == 13 ? Input.Text : File.Total[Game.Course[0]].ToString())}", Selecting && Cursor == 13 ? 0xffff00 : 0xffffff);
                Drawing.Text(40, 310, $"Notes:{SongData.NowTJA[0].Courses[Game.Course[0]].TotalNotes}", 0xffffff);
                if (InfoMenu)
                {
                    Drawing.Text(10, 10 + 20 * Cursor, ">", 0xff0000);
                }
                if (Selecting && Cursor == 11)
                {
                    Drawing.Text(30, 10 + 20 * Cursor, "<", 0x0000ff);
                    Drawing.Text(40 + Drawing.TextWidth($"NowCourse:{(ECourse)Game.Course[0]}", $"NowCourse:{(ECourse)Game.Course[0]}".Length), 10 + 20 * Cursor, ">", 0x0000ff);
                }
                if (CommandLayer == 1)
                {
                    Drawing.Box(0, Notes.NotesP[0].Y + 199, 400, Notes.NotesP[0].Y + 239, 0x000000);
                    Drawing.Text(10, Notes.NotesP[0].Y + 209, "命令文を追加/削除するノーツを選択してください。", 0xffffff);
                }
                if (CommandLayer == 2)
                {
                    Drawing.Box(0, Notes.NotesP[0].Y + 199, 240, Notes.NotesP[0].Y + 239, 0x000000);
                    Drawing.Text(10, Notes.NotesP[0].Y + 209, "命令文を記入してください。", 0xffffff);
                }
                if (RollEnd) Drawing.Text(800, 520, "Roll...", 0xffffff);

#if DEBUG
                Drawing.Text(240, 20, $"Length:{len}", 0xffffff);
                Drawing.Text(240, 40, $"Cursor:{cur}", 0xffffff);
                Drawing.Text(240, 60, $"Bar:{File.Bar[Game.Course[0]].Count}", 0xffffff);
                Drawing.Text(240, 80, $"Song:{Game.MainSong.Length}", 0xffffff);
                Drawing.Text(240, 100, $"Off:{-File.Offset * 1000.0}", 0xffffff);
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
                            Drawing.Text(20 + 9 * j, 500 + 20 * i, $"{(int)chip[j].ENote}", 0xffffff);
                        }
                        Drawing.Text(20 + 9 * chip.Count, 500 + 20 * i, ",", 0xffffff);
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
                if (NowCommand != null) Drawing.Text(900 - 36, 500, $"Now:{(int)NowCommand.Time,6}:{NowCommand.Name}", 0xffffff);
                else Drawing.Text(900 - 36, 500, $"Now:{0,6}:None", 0xffffff);
                for (int i = 0; i < ListCommand.Count; i++)
                {
                    if (i < 29) Drawing.Text(900, 540 + 20 * i, $"{(int)(File.Command[Game.Course[0]][i].Time - Game.MainTimer.Value),6}:{File.Command[Game.Course[0]][i].Name}", 0xffffff);
                }
                if (Game.NowMeasure > 0)
                {
                    if (Game.NowMeasure <= File.Bar[Game.Course[0]].Count)
                    {
                        BarLine bar = File.Bar[Game.Course[0]][Game.NowMeasure - 1];
                        /*double num = 240000.0 / bar.BPM * bar.Measure;
                        double num2 = Game.MainTimer.Value - bar.Time;
                        Drawing.Text(400, 500, $"num:{num}", 0xffffff);
                        Drawing.Text(400, 520, $"num2:{num2}", 0xffffff);
                        int count = bar.Chip.Count;
                        Drawing.Text(600, 500, $"Count:{count}", 0xffffff);
                        for (int j = 0; j < count; j++)
                        {
                            Drawing.Text(200, 500 + 20 * j, $"{(int)bar.Chip[j].Time,6},{(int)bar.Chip[j].ENote},{bar.Chip[j].Bpm},{bar.Chip[j].Measure}", 0xffffff);
                            double num3 = (j - 0.5) * (double)num / count;
                            double num4 = (j + 0.5) * (double)num / count;
                            Drawing.Text(400, 540 + 20 * j, $"{num3}", 0xffffff);
                            Drawing.Text(600, 540 + 20 * j, $"{num4}", 0xffffff);
                        }*/
                        Chip chip = GetNotes.GetNowNote(bar.Chip, Game.MainTimer.Value, true);
                        if (chip != null)
                        {
                            Drawing.Text(800, 500, $"{4 / chip.Measure}", 0xffffff);

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
                            Sfx.Metronome_Bar.Play();
                            mg = Game.NowMeasure;
                            mb = 0;
                        }
                        for (int j = 1; j < Bar[Game.NowMeasure - 1].Measure * 4; j++)
                        {
                            double time = Bar[Game.NowMeasure - 1].Time + j * (60000.0 / Bar[Game.NowMeasure - 1].BPM);
                            if (j > mb && Game.MainTimer.Value >= time)
                            {
                                Sfx.Metronome.Play();
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
                                if (File.Bar[course][i].Chip[nownote].ENote == ENote.Don && Key.IsPushing(PlayData.Data.LEFTDON) && Key.IsPushing(PlayData.Data.RIGHTDON))
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
                                if (File.Bar[course][i].Chip[nownote].ENote == ENote.Ka && Key.IsPushing(PlayData.Data.LEFTKA) && Key.IsPushing(PlayData.Data.RIGHTKA))
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
            if (SongData.NowTJA[0].Courses[Game.Course[0]].IsEnable)
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
                        if (chip.ENote == ENote.RollEnd)
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
            TextLog.Draw("小節を追加しました。");
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
                            TextLog.Draw("命令を追加しました。");
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
            if (success) TextLog.Draw("命令を削除しました。");
            else TextLog.Draw("記入された命令が見つかりませんでした。");
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
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.GetEncoding("Shift_JIS")))
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
        public TJAFile(TJA parse)
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
