using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TJAParse
{
    public class TJA
    {
        public TJA(string path, double playspeed, int random, bool mirror, int change)
        {
            TJAPath = path;
            Header = new Header();
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
                Header.Load(str, Header, playspeed);

            foreach (string str in alltext)
                Course.GetMeasureCount(str, Courses);
            //譜面読み込み
            foreach (string str in alltext)
                Course.Load(str, Courses, Header, playspeed);

            for (int i = 0; i < Courses.Length; i++)
            {
                Course.RandomizeChip(Courses[i].ListChip, random, mirror, change);
                Course.RollDoubledCheck(Courses[i].ListChip);
            }

        }

        public TJA(string path)//Load
        {
            TJAPath = path;
            Header = new Header();
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
                Header.Load(str, Header, 1.0, false);
            foreach (string str in alltext)
                Course.Load(str, Courses, Header, 1.0, false);
        }

        public string TJAPath;
        public Header Header;
        public Course[] Courses = new Course[5];
    }
}
