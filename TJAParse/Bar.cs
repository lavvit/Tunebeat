using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAParse
{
    public class Bar
    {
        public Bar(int number, double time, double bpm, double beat, int measure)
        {
            Number = number;
            Time = time;
            BPM = bpm;
            Scroll = 1.0;
            Measure = beat;
            Amount = measure;
            IsShow = true;
            Chip = new List<Chip>();
            double num = time;
            for (int i = 0; i < Amount; i++)
            {
                Chip chip = new Chip()
                {
                    Time = num,
                    Bpm = BPM,
                    Scroll = Scroll,
                    Measure = Measure,
                };
                Chip.Add(chip);
                num += (long)(240000.0 / Amount / BPM);
            }
            foreach (Chip chip in Chip)
                chip.Time = Math.Round(chip.Time, 4, MidpointRounding.AwayFromZero);
        }

        public Bar(int num, double time, double bpm, double scroll, double beat, int measure, bool show, List<Chip> chip)
        {
            Number = num;
            Time = time;
            BPM = bpm;
            Scroll = scroll;
            Measure = beat;
            Amount = measure;
            IsShow = show;
            Chip = chip;
        }

        public double Time, BPM, Scroll, Measure;
        public int Number, Amount;
        public bool IsShow;
        public List<Chip> Chip;
    }
}
