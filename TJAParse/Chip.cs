using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAParse
{
    public class Chip
    {
        public ENote ENote;
        public EChip EChip;
        public double Time;//判定
        //public double DrawTime;//描画
        public double Bpm;
        public double Scroll;
        public double Measure;
        public int RollCount;
        public int Balloon;
        public bool IsHit;
        public bool IsGogo;
        public bool IsMiss;
        public bool IsShow;
        public bool IsDummy;
        public bool CanShow;
        public Chip RollEnd;
        public string Lyric;
        public double[] Sudden;
        public int BPMID;//HBS用
    }

    public class BPM
    {

    }

    public enum EChip
    {
        Note,
        Measure,
        GoGoStart,
        GoGoEnd
    }

    public enum ENote
    {
        Space,
        Don,
        Ka,
        DON,
        KA,
        RollStart,
        ROLLStart,
        Balloon,
        RollEnd,
        Kusudama
    }
}
