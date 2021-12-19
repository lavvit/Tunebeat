using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAParse
{
    public class Header
    {
        public string TITLE, SUBTITLE, WAVE, GENRE;
        public double BPM, OFFSET, SONGVOL, SEVOL, DEMOSTART, SCOREMODE;

        public static void Load(string str, Header header)
        {
            if (str.Length <= 0) return;
            var split = str.Split(':');
            if (split.Length < 2) return;
            switch (split[0])
            {
                case "TITLE":
                    header.TITLE = split[1];
                    break;
                case "SUBTITLE":
                    if (split[1].StartsWith("++") || split[1].StartsWith("--"))
                        split[1] = split[1].Substring(2);
                    header.SUBTITLE = split[1];
                    break;
                case "WAVE":
                    header.WAVE = split[1];
                    break;
                case "GENRE":
                    header.GENRE = split[1];
                    break;
                case "BPM":
                    header.BPM = double.Parse(split[1]);
                    break;
                case "OFFSET":
                    header.OFFSET = double.Parse(split[1]);
                    break;
                case "SONGVOL":
                    header.SONGVOL = double.Parse(split[1]);
                    break;
                case "SEVOL":
                    header.SEVOL = double.Parse(split[1]);
                    break;
                case "DEMOSTART":
                    header.DEMOSTART = double.Parse(split[1]);
                    break;
                case "SCOREMODE":
                    header.SCOREMODE = double.Parse(split[1]);
                    break;
            }
        }
    }
}
