using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaDrop
{
    public class MathCheck
    {
        public static bool IsRange(double value, double min, double max, bool includeMin = true, bool includeMax = false)
        {
            if (includeMin ? value >= min : value > min)
            {
                if (includeMax ? value <= max : value < max)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInclude(int value, int[] list)
        {
            foreach (int i in list)
            {
                if (i == value) return true;
            }
            return false;
        }
        public static bool IsInclude(double value, double[] list)
        {
            foreach (int i in list)
            {
                if (i == value) return true;
            }
            return false;
        }
    }
}
