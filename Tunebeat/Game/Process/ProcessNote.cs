using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAParse;
using static DxLibDLL.DX;
using Amaoto;

namespace Tunebeat.Game
{
    public class ProcessNote
    {
        public static void Process(EJudge judge, Chip chip)
        {
            if(judge != EJudge.Through)
            {
                chip.IsHit = true;
            }
        }
    }
}
