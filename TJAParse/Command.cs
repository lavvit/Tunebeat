using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAParse
{
    public class Command
    {
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

        public double Time;
        public string Name;
    }
}
