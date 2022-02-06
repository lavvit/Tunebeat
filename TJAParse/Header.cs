using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAParse
{
    public class Header
    {
        public string TITLE, SUBTITLE, WAVE, GENRE, BGIMAGE, BGMOVIE;
        public double BPM, OFFSET, SONGVOL, SEVOL, DEMOSTART, SCOREMODE;

        public static void Load(string str, Header header, double playspeed)
        {
            if (str.Length <= 0) return;
            var split = str.Split(':');
            if (split.Length < 2) return;
            string allsplit;
            switch (split[0])
            {
                case "TITLE":
                    allsplit = split[1];
                    if (split.Length > 2)
                    {
                        for (int i = 2; i < split.Length; i++)
                        {
                            allsplit = allsplit + split[i];
                        }
                    }
                    header.TITLE = allsplit;
                    break;
                case "SUBTITLE":
                    if (split[1].StartsWith("++") || split[1].StartsWith("--"))
                        split[1] = split[1].Substring(2);
                    allsplit = split[1];
                    if (split.Length > 2)
                    {
                        for (int i = 2; i < split.Length; i++)
                        {
                            allsplit = allsplit + ":" + split[i];
                        }
                    }
                    header.SUBTITLE = allsplit;
                    break;
                case "WAVE":
                    header.WAVE = split[1];
                    break;
                case "BGIMAGE":
                    header.BGIMAGE = split[1];
                    break;
                case "BGMOVIE":
                    header.BGMOVIE = split[1];
                    break;
                case "GENRE":
                    header.GENRE = split[1];
                    break;
                case "BPM":
                    header.BPM = !string.IsNullOrEmpty(split[1]) ? double.Parse(split[1]) * playspeed : 120;
                    break;
                case "OFFSET":
                    header.OFFSET = !string.IsNullOrEmpty(split[1]) ? double.Parse(split[1]) / playspeed : 0;
                    break;
                case "SONGVOL":
                    header.SONGVOL = !string.IsNullOrEmpty(split[1]) ? double.Parse(split[1]) : 100;
                    break;
                case "SEVOL":
                    header.SEVOL = !string.IsNullOrEmpty(split[1]) ? double.Parse(split[1]) : 100;
                    break;
                case "DEMOSTART":
                    header.DEMOSTART = !string.IsNullOrEmpty(split[1]) ? double.Parse(split[1]) : 0;
                    break;
            }
        }
    }
}
