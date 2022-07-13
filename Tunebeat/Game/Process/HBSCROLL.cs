using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAParse;

namespace Tunebeat
{
    public class HBSCROLL
    {
        public static void Init(int player)
        {
            /*List<Chip> ListChip = SongData.NowTJA[player].Courses[Game.Course[player]].ListChip;
            int count = ListChip.Count;
            int lastbpm = ListChip[count - 1].BPMID + 1;
            List<Chip>[] ChipList = new List<Chip>[lastbpm];
            Chips[player] = new List<BPMChip>();
            for (int i = 0; i < lastbpm; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    int id = ListChip[j].BPMID;
                    if (i == id)
                    {
                        ChipList[j].Add(ListChip[j]);
                    }
                }
                BPMChip chip = new BPMChip()
                {
                    BPM = ChipList[i][0].Bpm,
                    Chips = ChipList[i]
                };
                Chips[player].Add(chip);
            }*/
        }

        public double GetNowPBMTime()
        {
            float bpm_time = 0;
            //int last_input = 0;
            //float last_bpm_change_time;
            double play_time = Game.MainTimer.Value;

            /*for (int i = 1; ; i++)
            {
                //BPMCHANGEの数越えた
                if (i >= tja.listBPM.Count)
                {
                    CDTX.CBPM cBPM = tja.listBPM[last_input];
                    bpm_time = (float)cBPM.bpm_change_bmscroll_time + ((play_time - (float)cBPM.bpm_change_time) * (float)cBPM.dbBPM値 / 15000.0f);
                    last_bpm_change_time = (float)cBPM.bpm_change_time;
                    break;
                }
                for (; i < tja.listBPM.Count; i++)
                {
                    CDTX.CBPM cBPM = tja.listBPM[i];
                    if (cBPM.bpm_change_time == 0 || cBPM.bpm_change_course == n現在のコース[0])
                    {
                        break;
                    }
                }
                if (i == tja.listBPM.Count)
                {
                    i = tja.listBPM.Count - 1;
                    continue;
                }

                if (play_time < tja.listBPM[i].bpm_change_time)
                {
                    CDTX.CBPM cBPM = tja.listBPM[last_input];
                    bpm_time = (float)cBPM.bpm_change_bmscroll_time + ((play_time - (float)cBPM.bpm_change_time) * (float)cBPM.dbBPM値 / 15000.0f);
                    last_bpm_change_time = (float)cBPM.bpm_change_time;
                    break;
                }
                else
                {
                    last_input = i;
                }
            }*/

            return bpm_time;
        }

        public static List<BPMChip>[] Chips = new List<BPMChip>[2];//player,bpm,chip
    }

    public class BPMChip
    {
        public double BPM;
        public List<Chip> Chips;
    }
}
