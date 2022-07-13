using System.Collections.Generic;
using System.Text;
using System.IO;
using SeaDrop;

namespace BMSParse
{
    public class BMS
    {
        public BMS(string path, double playspeed = 1.0, int longtype = 0, int random = 0)
        {
            Path = path;
            Dir = System.IO.Path.GetDirectoryName(path);
            if (File.Exists(path))
            {
                isEnable = true;
                Course = new BCourse();

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
                    if (alltext[i].StartsWith("*"))
                        alltext[i] = alltext[i].Substring(0, alltext[i].IndexOf("*"));
                }
                BCourse.NowTime = 0;
                foreach (string line in alltext)
                    BCourse.GetBPM(line, Course, playspeed);
                foreach (string line in alltext)
                    BCourse.Load(line, Course);
                BCourse.SetChip(Course);
                BCourse.SetLong(Course, Course.ListChip, longtype);
                BCourse.SetOption(Course.ListChip, (EOption)random, Course);
            }
        }

        public string Path;
        public bool isEnable;
        public BCourse Course;

        public static string Dir;
    }
}
