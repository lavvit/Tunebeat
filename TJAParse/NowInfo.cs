using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAParse
{
    class NowInfo
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
        public static int BPMID = 0;
        public static ERoll RollState = ERoll.None;
    }
}
