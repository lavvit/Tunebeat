using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;

namespace BMSParse
{
    public class Chip
    {
        public int Channel;
        public string ID;
        public double Time;//判定
        public double PushTime;//判定
        //public EJudge Judge;
        public double Bpm;
        public double Measure;
        public (int, int) Place;
        public bool IsHit;
        public bool IsMiss;
        public string SoundPath;
        public Sound Sound;
        public Chip LongEnd;
        public bool Pushing;
        public bool LongMiss;
        public bool TopPushed;
        public int RanNum;

        public string Draw()
        {
            return $"{(int)Time,4} {ID} {Bpm} {Place}{(IsHit ? " Hit" : (IsMiss ? " Miss" : ""))} {(LongEnd != null ? ($"Long:{LongEnd.Draw()} {(TopPushed && Pushing ? "Push" : "")}") : "")}";
        }
    }

    public class Wave
    {
        public string ID;
        public string Path;
    }
    public class Bitmap
    {
        public string ID;
        public string Path;
    }
    public class BPM
    {
        public string ID;
        public double Value;
    }
    public class Stop
    {
        public string ID;
        public double Value;
    }
    public class BPMList
    {
        public double Time;
        public (int, int, int) Place;
        public double Value;
    }
    public class StopList
    {
        public double Time;
        public (int, int, int) Place;
        public double Value;
    }

    public class Bar
    {
        public Bar(int num, double bpm, double measure, double time)
        {
            Number = num + 1;
            BPM = bpm;
            Measure = measure;
            Time = time;
            Chip = new List<Chip>();
        }
        public int Number;
        public double Time, BPM, Measure;
        public List<Chip> Chip;
    }
    public class Channel
    {
        public int Number;
        public List<Chip> Chips;
    }
}
