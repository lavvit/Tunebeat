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
            Preview =  CreateMode;
            InfoMenu = false;
            Selecting = false;
            Read();
            File = new TJAFile(Game.MainTJA[0]);
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
                TextureLoad.Game_Notes.Draw(195 * 2, 520 + 195, new Rectangle(195 * 11, 0, 195 * 5, 195));

                double len = (Mouse.X - (Notes.NotesP[0].X - 22)) / (1421f / InputType);
                int cur = (int)Math.Floor(len);
                int[] wid = new int[8] { 0, 29, 60, 75, 84, 90, 97, 104 };
                Rectangle[] rec = new Rectangle[8] { new Rectangle(195 * 1, 0, 195 * 1, 195), new Rectangle(195 * 2, 0, 195 * 1, 195), new Rectangle(195 * 3, 0, 195 * 1, 195),
                new Rectangle(195 * 4, 0, 195 * 1, 195), new Rectangle(195 * 5, 0, 195 * 1, 195), new Rectangle(195 * 8, 0, 195 * 1, 195),
                new Rectangle(195 * 11, 0, 195 * 2, 195), new Rectangle(195 * 13, 0, 195 * 2, 195) };
                if (Mouse.X >= Notes.NotesP[0].X - 22 && Mouse.Y >= Notes.NotesP[0].Y && Mouse.Y < Notes.NotesP[0].Y + 195 && Game.MainTimer.State == 0)
                {
                    double width = (1920 - (Notes.NotesP[0].X - 22)) / (double)InputType;
                    TextureLoad.Game_Notes_Ghost.Opacity = 0.5;
                    TextureLoad.Game_Notes_Ghost.Draw(Notes.NotesP[0].X - wid[NowInput] + cur * width, Notes.NotesP[0].Y, rec[NowColor]);
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
                DrawString(40, 20, $"Title:{(Input.IsEnable && Cursor == 0 ? Input.Text : File.Title)}", Selecting && Cursor == 0 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 40, $"SubTitle:{(Input.IsEnable && Cursor == 1 ? Input.Text : File.SubTitle)}", Selecting && Cursor == 1 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 60, $"Wave:{(Input.IsEnable && Cursor == 2 ? Input.Text : File.Wave)}", Selecting && Cursor == 2 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 80, $"BGImage:{(Input.IsEnable && Cursor == 3 ? Input.Text : File.BGImage)}", Selecting && Cursor == 3 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 100, $"BGMovie:{(Input.IsEnable && Cursor == 4 ? Input.Text : File.BGMovie)}", Selecting && Cursor == 4 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 120, $"Bpm:{(Input.IsEnable && Cursor == 5 ? Input.Text : File.Bpm.ToString())}", Selecting && Cursor == 5 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 140, $"Offset:{(Input.IsEnable && Cursor == 6 ? Input.Text : File.Offset.ToString())}", Selecting && Cursor == 6 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 160, $"DemoStart:{(Input.IsEnable && Cursor == 7 ? Input.Text : File.DemoStart.ToString())}", Selecting && Cursor == 7 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 180, $"NowCourse:{(ECourse)Game.Course[0]}", Selecting && Cursor == 8 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 200, $"Level:{(Input.IsEnable && Cursor == 9 ? Input.Text : File.Level[Game.Course[0]].ToString())}", Selecting && Cursor == 9 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 220, $"Total:{(Input.IsEnable && Cursor == 10 ? Input.Text : File.Total[Game.Course[0]].ToString())}", Selecting && Cursor == 10 ? (uint)0xffff00 : 0xffffff);
                DrawString(40, 260, $"Notes:{Game.MainTJA[0].Courses[Game.Course[0]].TotalNotes}", 0xffffff);
                if (InfoMenu)
                {
                    DrawString(10, 20 + 20 * Cursor, ">", 0xff0000);
                }
                if (Selecting && Cursor == 8)
                {
                    DrawString(30, 20 + 20 * Cursor, "<", 0x0000ff);
                    DrawString(40 + GetDrawStringWidth($"NowCourse:{(ECourse)Game.Course[0]}", $"NowCourse:{(ECourse)Game.Course[0]}".Length), 20 + 20 * Cursor, ">", 0x0000ff);
                }

#if DEBUG
                DrawString(240, 20, $"Length:{len}", 0xffffff);
                DrawString(240, 40, $"Cursor:{cur}", 0xffffff);
#endif
            }
            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }

        public static bool CreateMode, Preview, InfoMenu, Selecting, ad;
        public static List<string> AllText, Course;
        public static int NowScroll, Cursor, InputType, NowInput, NowColor;
        public static TJAFile File;
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
            DemoStart = parse.Header.DEMOSTART;
            for (int i = 0; i < 5; i++)
            {
                Level[i] = parse.Courses[i].LEVEL;
                Total[i] = parse.Courses[i].TOTAL;
            }
        }
        public string Title, SubTitle, Wave, BGImage, BGMovie, Genre;
        public double Bpm, Offset, DemoStart;
        public int[] Level = new int[5];
        public double[] Total = new double[5];
    }
}
