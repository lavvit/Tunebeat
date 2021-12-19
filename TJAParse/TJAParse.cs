using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TJAParse
{
    public class TJAParse
    {
        public TJAParse(string path)
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
                if (alltext[i].Contains("////"))
                    alltext[i] = alltext[i].Replace($"////{alltext[i].Split(new string[] { "////" }, StringSplitOptions.None)[1]}", "");
                if (alltext[i].Contains("//"))
                    alltext[i] = alltext[i].Replace($"//{alltext[i].Split(new string[] { "//" }, StringSplitOptions.None)[1]}", "");
                //バグ対策
                if (alltext.Count > 0 && alltext[i] == ",")
                    alltext[i] = alltext[i].Replace(",", "0,");
            }

            //ヘッダー読み込み
            foreach (string str in alltext)
                Header.Load(str, Header);

            foreach (string str in alltext)
                Course.GetMeasureCount(str, Courses);
            //譜面読み込み
            foreach (string str in alltext)
                Course.Load(str, Courses, Header);
        }

        public string TJAPath;
        public Header Header;
        public Course[] Courses = new Course[5];
    }
}
