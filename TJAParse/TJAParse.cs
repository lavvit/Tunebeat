using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SeaDrop;

namespace TJAParse
{
    public class TJA
    {
        public TJA(string path, double playspeed, int random, bool mirror, int change)
        {
            Path = path;
            if (File.Exists(path))
            {
                isEnable = true;
                Header = new Header();
                Courses = new Course[5];
                for (int i = 0; i < Courses.Length; i++)
                    Courses[i] = new Course();

                //譜面をListに1行づつ読み込む
                List<string> alltext = new List<string>();
                using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("SHIFT_JIS")))
                {
                    while (sr.Peek() > -1)
                    {
                        alltext.Add(sr.ReadLine());
                    }
                }

                for (int i = 0; i < alltext.Count; i++)
                {
                    //コメント削除
                    if (alltext[i].Contains("//"))
                        alltext[i] = alltext[i].Substring(0, alltext[i].IndexOf("//"));
                    //バグ対策
                    if (alltext.Count > 0 && alltext[i] == ",")
                        alltext[i] = alltext[i].Replace(",", "0,");
                }

                foreach (string line in alltext)
                {
                    Course.GetMeasureCount(line, Courses);
                }
                foreach (string line in alltext)
                {
                    Header.Load(line, Header, playspeed);
                    Course.Load(line, Courses, Header, playspeed);
                }

                for (int i = 0; i < Courses.Length; i++)
                {
                    Course.SetBalloon(Courses[i], Courses[i].ListChip);
                    Course.TimeRound(Courses[i].ListChip, Courses[i].ListBar, Courses[i].ListCommand);
                    Course.RandomizeChip(Courses[i].ListChip, random, mirror, change);
                    Course.RollDoubledCheck(Courses[i].ListChip);
                    Course.DummyCheck(Courses[i].ListChip, Path, Header.WAVE);
                }
                string dir = System.IO.Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(Header.WAVE)) Sound = new Sound($@"{dir}\{Header.WAVE}");
                if (!string.IsNullOrEmpty(Header.BGIMAGE)) Image = new Texture($@"{dir}\{Header.BGIMAGE}");
                string wmvpath = $@"{dir}\{Header.BGMOVIE}";
                string mp4path = wmvpath.Replace("wmv", "mp4");
                if (!string.IsNullOrEmpty(Header.BGMOVIE) && File.Exists(mp4path)) Movie = new Movie(mp4path);// (File.Exists(mp4path) ? mp4path : wmvpath);
                Jacket = new Texture($@"{dir}\Album.png");
            }
        }

        public TJA(string path)//Load
        {
            Path = path;
            if (File.Exists(path))
            {
                isEnable = true;
                Header = new Header();
                Courses = new Course[5];
                for (int i = 0; i < Courses.Length; i++)
                    Courses[i] = new Course();

                //譜面をListに1行づつ読み込む
                List<string> alltext = new List<string>();
                using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("SHIFT_JIS")))
                {
                    while (sr.Peek() > -1)
                    {
                        alltext.Add(sr.ReadLine());
                    }
                }

                for (int i = 0; i < alltext.Count; i++)
                {
                    //コメント削除
                    if (alltext[i].Contains("//"))
                        alltext[i] = alltext[i].Substring(0, alltext[i].IndexOf("//"));
                    //バグ対策
                    if (alltext.Count > 0 && alltext[i] == ",")
                        alltext[i] = alltext[i].Replace(",", "0,");
                }

                //ヘッダー読み込み
                foreach (string str in alltext)
                {
                    Header.Load(str, Header, 1.0, false);
                    Course.Load(str, Courses, Header, 1.0, false);
                }
                Jacket = new Texture($"{System.IO.Path.GetDirectoryName(path)}/Album.png");
            }
        }

        public string Path;
        public bool isEnable;
        public Header Header;
        public Course[] Courses;
        public Sound Sound;
        public Texture Image, Jacket;
        public Movie Movie;
    }
}
