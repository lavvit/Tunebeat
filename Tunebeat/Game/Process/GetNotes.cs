using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class GetNotes
    {
        public static Chip GetNearNote(List<Chip> listChip, double nTime)
        {
            //sw2.Start();
            nTime += 0;

            int nIndex_InitialPositionSearchingToPast;
            int nTimeDiff;
            int count = listChip.Count;
            if (count <= 0)         // 演奏データとして1個もチップがない場合は
            {
                //sw2.Stop();
                return null;
            }

            int nIndex_NearestChip_Future = nIndex_InitialPositionSearchingToPast = 0;
            if (0 >= count)      // その時点で演奏すべきチップが既に全部無くなっていたら
            {
                nIndex_NearestChip_Future = nIndex_InitialPositionSearchingToPast = count - 1;
            }
            // int nIndex_NearestChip_Future = nIndex_InitialPositionSearchingToFuture;
            //			while ( nIndex_NearestChip_Future < count )	// 未来方向への検索
            for (; nIndex_NearestChip_Future < count; nIndex_NearestChip_Future++)
            {
                if (nIndex_NearestChip_Future < 0)
                    continue;

                Chip chip = listChip[nIndex_NearestChip_Future];
                if (!chip.IsHit && chip.CanShow)
                {
                    if (((chip.ENote >= ENote.Don) && (chip.ENote <= ENote.KA)))
                    {
                        if (chip.Time > nTime)
                        {
                            break;
                        }
                        nIndex_InitialPositionSearchingToPast = nIndex_NearestChip_Future;
                    }
                }
            }
            int nIndex_NearestChip_Past = nIndex_InitialPositionSearchingToPast;
            //			while ( nIndex_NearestChip_Past >= 0 )		// 過去方向への検索
            for (; nIndex_NearestChip_Past >= 0; nIndex_NearestChip_Past--)
            {
                Chip chip = listChip[nIndex_NearestChip_Past];
                //if ( (!chip.bHit && chip.b可視 ) && ( (  0x93 <= chip.nチャンネル番号 ) && ( chip.nチャンネル番号 <= 0x99 ) ) )
                if ((!chip.IsHit && chip.CanShow) && ((chip.ENote >= ENote.Don) && (chip.ENote <= ENote.KA)))
                {
                    break;
                }
            }
            if ((nIndex_NearestChip_Future >= count) && (nIndex_NearestChip_Past < 0))  // 検索対象が過去未来どちらにも見つからなかった場合
            {
                //sw2.Stop();
                return null;
            }
            Chip nearestChip; // = null;	// 以下のifブロックのいずれかで必ずnearestChipには非nullが代入されるので、null初期化を削除
            if (nIndex_NearestChip_Future >= count)                                         // 検索対象が未来方向には見つからなかった(しかし過去方向には見つかった)場合
            {
                nearestChip = listChip[nIndex_NearestChip_Past];
                //				nTimeDiff = Math.Abs( (int) ( nTime - nearestChip.n発声時刻ms ) );
            }
            else if (nIndex_NearestChip_Past < 0)                                               // 検索対象が過去方向には見つからなかった(しかし未来方向には見つかった)場合
            {
                nearestChip = listChip[nIndex_NearestChip_Future];
                //				nTimeDiff = Math.Abs( (int) ( nTime - nearestChip.n発声時刻ms ) );
            }
            else
            {
                int nTimeDiff_Future = Math.Abs((int)(nTime - listChip[nIndex_NearestChip_Future].Time));
                int nTimeDiff_Past = Math.Abs((int)(nTime - listChip[nIndex_NearestChip_Past].Time));
                if (nTimeDiff_Future < nTimeDiff_Past)
                {
                    if (!listChip[nIndex_NearestChip_Past].IsHit && (listChip[nIndex_NearestChip_Past].Time + (108) >= nTime))
                    {
                        nearestChip = listChip[nIndex_NearestChip_Past];
                    }
                    else
                        nearestChip = listChip[nIndex_NearestChip_Future];
                    //					nTimeDiff = Math.Abs( (int) ( nTime - nearestChip.n発声時刻ms ) );
                }
                else
                {
                    nearestChip = listChip[nIndex_NearestChip_Past];
                    //					nTimeDiff = Math.Abs( (int) ( nTime - nearestChip.n発声時刻ms ) );
                }
            }
            nTimeDiff = Math.Abs((int)(nTime - nearestChip.Time));
            int n検索範囲時間ms = 0;
            if ((n検索範囲時間ms > 0) && (nTimeDiff > n検索範囲時間ms))                 // チップは見つかったが、検索範囲時間外だった場合
            {
                //sw2.Stop();
                return null;
            }
            //sw2.Stop();
            return nearestChip;
        }

        public static Chip GetNowNote(List<Chip> listChip, double nTime)
        {
            //sw2.Start();
            nTime += 0;

            int nTimeDiff;
            int count = listChip.Count;
            if (count <= 0)         // 演奏データとして1個もチップがない場合は
            {
                //sw2.Stop();
                return null;
            }

            int nIndex_NowChip = count - 1;
            if (0 >= count)      // その時点で演奏すべきチップが既に全部無くなっていたら
            {
                nIndex_NowChip  = count - 1;
            }
            // int nIndex_NearestChip_Future = nIndex_InitialPositionSearching;
            //			while ( nIndex_NearestChip_Future < count )	// 検索
            for (; nIndex_NowChip >= 0; nIndex_NowChip--)
            {
                Chip chip = listChip[nIndex_NowChip];
                if (chip.ENote >= ENote.Don && chip.ENote <= ENote.Kusudama)
                {
                    if (chip.Time <= nTime)
                    {
                        break;
                    }
                }
                
            }
            if (nIndex_NowChip < 0)  // 検索対象が見つからなかった場合
            {
                return null;
            }
            Chip nowChip = listChip[nIndex_NowChip];
            nTimeDiff = Math.Abs((int)(nTime - nowChip.Time));
            int n検索範囲時間ms = 0;
            if ((n検索範囲時間ms > 0) && (nTimeDiff > n検索範囲時間ms))                 // チップは見つかったが、検索範囲時間外だった場合
            {
                //sw2.Stop();
                return null;
            }
            //sw2.Stop();
            return nowChip;
        }

        public static EJudge GetJudge(Chip chip, double time)
        {
            int[] range = new int[5] { PlayData.Data.JudgePerfect, PlayData.Data.JudgeGreat, PlayData.Data.JudgeGood, PlayData.Data.JudgeBad, PlayData.Data.JudgePoor };
            switch (PlayData.Data.JudgeType)
            {
                case 1://Spica標準
                    range[0] = 16;
                    range[1] = 32;
                    range[2] = 90;
                    range[3] = 125;
                    range[4] = 125;
                    break;
                case 2://ハードモード
                    range[0] = 10;
                    range[1] = 20;
                    range[2] = 78;
                    range[3] = 113;
                    range[4] = 113;
                    break;
            }
            if (PlayData.Data.Just) range[2] = 0;

            if (chip != null)
            {
                double Difference = Math.Abs(time - chip.Time);
                if (Difference <= range[0]) return EJudge.Perfect;
                else if (Difference <= range[1]) return EJudge.Great;
                else if (Difference <= range[2]) return EJudge.Good;
                else if (Difference <= range[3]) return EJudge.Bad;
                else if (Difference <= range[4]) return EJudge.Poor;
                else return EJudge.Through;
            }
            else return EJudge.Through;
        }
    }

    public enum EJudge
    {
        Perfect,
        Great,
        Good,
        Bad,
        Poor,
        Through,
        Auto
    }
}
