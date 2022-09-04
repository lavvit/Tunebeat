using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;
using BMSParse;

namespace Tunebeat
{
    public class BProcess
    {
        public static void Draw()
        {
            bool[][] autoflash = new bool[2][];
            if (BGame.NowCourse[0].Player == 2)
            {
                for (int p = 0; p < 2; p++)
                {
                    int[] x = new int[2] { 75 + 384, 1413 - 384 };
                    int[] xx = p == 1 ? new int[8] { 0, 54, 96, 150, 192, 246, 288, 342 } : new int[8] { 0, 93, 147, 189, 243, 285, 339, 381 };
                    int[] width = p == 1 ? new int[8] { 51, 39, 51, 39, 51, 39, 51, 90 } : new int[8] { 90, 51, 39, 51, 39, 51, 39, 51 };
                    int[] keycolorlong = p == 1 ? new int[8] { 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00, 0xff8800 } :
                        new int[8] { 0xff8800, 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00 };
                    int[] vcolor = p == 1 ? new int[8] { 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x800000 } :
                        new int[8] { 0x800000, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0 };
                    int[] kcolor = p == 1 ? new int[8] { 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff, 0xff4400 } :
                        new int[8] { 0xff4400, 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff };
                    autoflash[p] = new bool[16];
                    int[] bup = p == 1 ? new int[8] { 0, 20, 0, 20, 0, 20, 0, 0 } : new int[8] { 0, 0, 20, 0, 20, 0, 20, 0 };

                    foreach (Chip chip in BGame.BMS[0].Course.ListChip)
                    {
                        int a = BCourse.GetLane(chip);
                        if (BGame.Playmode[0] == EAuto.Auto && BGame.Timer.Value >= chip.Time && BGame.Timer.Value < chip.Time + 100 && BCourse.GetLane(chip) >= 0)
                        {
                            int l = p == 1 ? BCourse.GetLane2P(chip, true) : BCourse.GetLane(chip, false);
                            if (l >= 0) autoflash[p][l] = true;
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Drawing.Box(x[p] + xx[i] - 2, 780 - bup[i] - 2, width[i] + 4, 44, 0);
                        Drawing.Box(x[p] + xx[i], 780 - bup[i], width[i], 40, vcolor[i]);
                        if (BGame.Playmode[0] > EAuto.Normal)
                        {
                            if (autoflash[p][i])
                            {
                                Drawing.Box(x[p] + xx[i], 780 - bup[i], width[i], 40, kcolor[i]);
                            }
                        }
                        else
                        {
                            if (Pushing(i, p))
                            {
                                if (kcolor[i] == 0xff4400)
                                {
                                    if (Pushing(10, p)) Drawing.Box(x[p] + xx[i], 780 - bup[i], width[i], 20, kcolor[i]);
                                    else Drawing.Box(x[p] + xx[i], 800 - bup[i], width[i], 20, kcolor[i]);
                                }
                                else Drawing.Box(x[p] + xx[i], 780 - bup[i], width[i], 40, kcolor[i]);
                            }
                        }

                        bool[] isCharge = new bool[16];
                        for (int j = 0; j < BGame.BMS[0].Course.ListChip.Count; j++)
                        {
                            Chip chip = BGame.BMS[0].Course.ListChip[j];
                            if (chip.Pushing)
                            {
                                int l = p == 1 ? BCourse.GetLane2P(chip, true) : BCourse.GetLane(chip, false);
                                if (l >= 0) isCharge[l] = true;
                            }
                        }
                        if (isCharge[i])
                        {
                            Drawing.Box(x[p] + xx[i], 780 - bup[i], width[i], 40, keycolorlong[i]);
                        }
                    }
                }
            }
            else
            {
                for (int p = 0; p < 2; p++)
                {
                    if (p == 0 || BGame.Play2P)
                    {
                        int ps = p == 0 && PlayData.Data.Is2PSide ? 1 : p;
                        int[] x = new int[2] { 75, 1413 };
                        int[] xx = ps == 1 ? new int[8] { 0, 54, 96, 150, 192, 246, 288, 342 } : new int[8] { 0, 93, 147, 189, 243, 285, 339, 381 };
                        int[] width = ps == 1 ? new int[8] { 51, 39, 51, 39, 51, 39, 51, 90 } : new int[8] { 90, 51, 39, 51, 39, 51, 39, 51 };
                        int[] keycolorlong = ps == 1 ? new int[8] { 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00, 0xff8800 } :
                            new int[8] { 0xff8800, 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00, 0x00ffff, 0xffff00 };
                        int[] vcolor = ps == 1 ? new int[8] { 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x800000 } :
                            new int[8] { 0x800000, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0, 0x404040, 0xc0c0c0 };
                        int[] kcolor = ps == 1 ? new int[8] { 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff, 0xff4400 } :
                            new int[8] { 0xff4400, 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff, 0x0088ff, 0x88ffff };
                        autoflash[p] = new bool[8];
                        int[] bup = ps == 1 ? new int[8] { 0, 20, 0, 20, 0, 20, 0, 0 } : new int[8] { 0, 0, 20, 0, 20, 0, 20, 0 };

                        foreach (Chip chip in BGame.BMS[p].Course.ListChip)
                        {
                            if (BGame.Playmode[p] == EAuto.Auto && BGame.Timer.Value >= chip.Time && BGame.Timer.Value < chip.Time + 100 && chip.Channel >= 10 && chip.Channel < 20)
                            {
                                autoflash[p][ps == 1 ? BCourse.GetLane2P(chip) : BCourse.GetLane(chip)] = true;
                            }
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            Drawing.Box(x[ps] + xx[i] - 2, 780 - bup[i] - 2, width[i] + 4, 44, 0);
                            Drawing.Box(x[ps] + xx[i], 780 - bup[i], width[i], 40, vcolor[i]);
                            if (BGame.Playmode[p] > EAuto.Normal)
                            {
                                if (autoflash[p][i])
                                {
                                    Drawing.Box(x[ps] + xx[i], 780 - bup[i], width[i], 40, kcolor[i]);
                                }
                            }
                            else
                            {
                                if (Pushing(i, ps, p))
                                {
                                    if (kcolor[i] == 0xff4400)
                                    {
                                        if (Pushing(10, ps)) Drawing.Box(x[ps] + xx[i], 780 - bup[i], width[i], 20, kcolor[i]);
                                        else Drawing.Box(x[ps] + xx[i], 800 - bup[i], width[i], 20, kcolor[i]);
                                    }
                                    else Drawing.Box(x[ps] + xx[i], 780 - bup[i], width[i], 40, kcolor[i]);
                                }
                            }

                            bool[] isCharge = new bool[8];
                            for (int j = 0; j < BGame.BMS[p].Course.ListChip.Count; j++)
                            {
                                Chip chip = BGame.BMS[p].Course.ListChip[j];
                                if (chip.Pushing)
                                {
                                    isCharge[ps == 1 ? BCourse.GetLane2P(chip) : BCourse.GetLane(chip)] = true;
                                }
                            }
                            if (isCharge[i])
                            {
                                Drawing.Box(x[ps] + xx[i], 780 - bup[i], width[i], 40, keycolorlong[i]);
                            }
                        }
                    }
                }
            }
        }

        public static void Update()
        {
            for (int p = 0; p < 2; p++)
            {
                if (BGame.NowCourse[0].Player == 2)
                {
                    if (BGame.NowState > EState.None && BGame.Chips[0] != null)
                    {
                        double time = BGame.AutoTime[0] > 0 ? BGame.AutoTime[0] : (BGame.StartMeasure == 0 ? BGame.Timer.Begin : (long)BGame.Bars[0][BGame.StartMeasure - 1].Time);
                        #region BGM
                        for (int i = 0; i < BGame.Chips[0].Count; i++)
                        {
                            Chip chip = BGame.Chips[0][i];
                            if (BGame.Timer.Value >= chip.Time && !chip.IsHit && chip.Channel == 1 && chip.Time >= time)
                            {
                                chip.IsHit = true;
                                chip.Sound.Volume = PlayData.Data.GameBGM / 100.0;
                                chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                chip.Sound.Play();
                            }
                        }
                        #endregion
                        if (BGame.NowState < EState.End)
                        {
                            switch (BGame.Playmode[0])
                            {
                                case EAuto.Normal:
                                    #region Normal
                                    for (int i = 0; i < BGame.Chips[0].Count; i++)
                                    {
                                        Chip chip = BGame.Chips[0][i];
                                        int l = p == 1 ? BCourse.GetLane2P(chip, true) : BCourse.GetLane(chip, false);
                                        if (chip.LongEnd != null)
                                        {
                                            if (BCourse.GetLane(chip) > 0 || (!PlayData.Data.AutoSclatch[0] && BCourse.GetLane(chip) == 0))
                                            {
                                                if (!chip.IsHit)
                                                {
                                                    if (PlayData.Data.LNType == 0)
                                                    {
                                                        if (!chip.IsMiss)
                                                        {
                                                            if (chip.TopPushed)
                                                            {
                                                                if (!chip.Pushing)
                                                                {
                                                                    if (GetJudge(BGame.Timer.Value - (chip.LongEnd.Time + BGame.Adjust[p])) < EJudge.Poor)
                                                                    {
                                                                        BScore.ScoreSet(chip, 0, 3);
                                                                        chip.IsHit = true;
                                                                        chip.Pushing = false;
                                                                    }
                                                                    else
                                                                    {
                                                                        BScore.ScoreSet(EJudge.Through, 0);
                                                                        chip.IsMiss = true;
                                                                        BGame.ChargeSound[p][l].Stop();
                                                                    }
                                                                }
                                                                if (chip.Pushing && BGame.Timer.Value > chip.LongEnd.Time)
                                                                {
                                                                    BScore.ScoreSet(chip, 0, 3);
                                                                    chip.IsHit = true;
                                                                    chip.Pushing = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (chip.Time + 216.6666 < BGame.Timer.Value)
                                                                {
                                                                    BScore.ScoreSet(EJudge.Through, 0);
                                                                    chip.IsMiss = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        if (chip.TopPushed)
                                                        {
                                                            if (chip.LongEnd.Time + 216.6666 < BGame.Timer.Value && !chip.LongEnd.IsMiss && !chip.IsMiss)
                                                            {
                                                                BScore.ScoreSet(EJudge.Through, 0);
                                                                chip.IsMiss = true;
                                                                chip.LongEnd.IsMiss = true;
                                                                chip.Pushing = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (chip.LongEnd.Time < BGame.Timer.Value && !chip.LongEnd.IsMiss && chip.IsMiss)
                                                            {
                                                                BScore.ScoreSet(EJudge.Through, 0);
                                                                chip.IsMiss = true;
                                                                chip.LongEnd.IsMiss = true;
                                                                chip.Pushing = false;
                                                            }
                                                            if (chip.Time + 216.6666 < BGame.Timer.Value && !chip.IsMiss)
                                                            {
                                                                BScore.ScoreSet(EJudge.Through, 0);
                                                                chip.IsMiss = true;
                                                            }
                                                        }
                                                        if (chip.LongEnd.Time < BGame.Timer.Value && chip.LongEnd.IsMiss && chip.Pushing)
                                                        {
                                                            chip.Pushing = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((BGame.NowState == EState.None ? time < BGame.Timer.Value : chip.Time + 216.6666 < BGame.Timer.Value) && !chip.IsHit && !chip.IsMiss && l >= 0)
                                            {
                                                BScore.ScoreSet(EJudge.Through, 0);
                                                chip.IsMiss = true;
                                            }
                                        }
                                    }


                                    for (int i = PlayData.Data.AutoSclatch[0] ? 1 : 0; i < 8; i++)
                                    {
                                        int ii = p == 1 ? 8 + i : i;
                                        Chip chip = NearChip(BGame.Chips[0], BCourse.GetChannel(i, p == 1));
                                        if (chip != null)
                                        {
                                            if (chip.Sound != null)
                                            {
                                                if (chip.LongEnd != null)
                                                {
                                                    BGame.ChargeSound[0][ii] = chip.Sound;
                                                }

                                                else
                                                {
                                                    BGame.KeySound[0][ii] = chip.Sound;
                                                    BGame.ChargeSound[0][ii] = null;
                                                }
                                            }

                                            if (chip.LongEnd != null)
                                            {
                                                if (PlayData.Data.LNType == 0)
                                                {
                                                    if (chip.Pushing)
                                                    {
                                                        if (Left(i, p))
                                                        {
                                                            chip.Pushing = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Pushed(i, p) && GetJudge(BGame.Timer.Value - (chip.Time + BGame.Adjust[0])) < EJudge.Poor && !chip.IsMiss)
                                                        {
                                                            PlayKey(i);
                                                            if (BGame.ChargeSound[0][ii] != null)
                                                            {
                                                                BGame.ChargeSound[0][ii].Volume = PlayData.Data.SE / 100.0;
                                                                BGame.ChargeSound[0][ii].PlaySpeed = PlayData.Data.PlaySpeed;
                                                                BGame.ChargeSound[0][ii].Play();
                                                            }
                                                            chip.Pushing = true;
                                                            chip.TopPushed = true;
                                                            chip.PushTime = BGame.Timer.Value;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (chip.Pushing)
                                                    {
                                                        if (Left(i, p))
                                                        {
                                                            BScore.ScoreSet(chip, 0, 2);
                                                            chip.Pushing = false;
                                                            if (PlayData.Data.LNType == 2)
                                                            {
                                                                if (!chip.IsMiss) chip.LongEnd.PushTime = BGame.Timer.Value;
                                                                else BGame.ChargeSound[0][ii].Stop();
                                                            }
                                                            else chip.LongEnd.PushTime = BGame.Timer.Value;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Pushed(i, p) && GetJudge(BGame.Timer.Value - (chip.Time + BGame.Adjust[0])) < EJudge.Poor && !chip.IsMiss)
                                                        {
                                                            PlayKey(i);
                                                            if (BGame.ChargeSound[0][ii] != null)
                                                            {
                                                                BGame.ChargeSound[0][ii].Volume = PlayData.Data.SE / 100.0;
                                                                BGame.ChargeSound[0][ii].PlaySpeed = PlayData.Data.PlaySpeed;
                                                                BGame.ChargeSound[0][ii].Play();
                                                            }
                                                            BScore.ScoreSet(chip, 0, 1);
                                                            chip.Pushing = true;
                                                            if (PlayData.Data.LNType < 2 || !chip.IsMiss)
                                                            {
                                                                chip.TopPushed = true;
                                                                chip.PushTime = BGame.Timer.Value;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Pushed(i, p))
                                                {
                                                    PlayKey(i);
                                                    if (BGame.KeySound[0][ii] != null)
                                                    {
                                                        BGame.KeySound[0][ii].Volume = PlayData.Data.SE / 100.0;
                                                        BGame.KeySound[0][ii].PlaySpeed = PlayData.Data.PlaySpeed;
                                                        BGame.KeySound[0][ii].Play();
                                                    }
                                                    BScore.ScoreSet(chip, 0);
                                                    chip.PushTime = BGame.Timer.Value;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Pushed(i, p))
                                            {
                                                PlayKey(i);
                                                if (BGame.KeySound[0][ii] != null)
                                                {
                                                    BGame.KeySound[0][ii].Volume = PlayData.Data.SE / 100.0;
                                                    BGame.KeySound[0][ii].PlaySpeed = PlayData.Data.PlaySpeed;
                                                    BGame.KeySound[0][ii].Play();
                                                }
                                            }
                                        }
                                    }
                                    if (PlayData.Data.AutoSclatch[0])
                                    {
                                        for (int i = 0; i < BGame.Chips[0].Count; i++)
                                        {
                                            Chip chip = BGame.Chips[0][i];
                                            if (BGame.Timer.Value >= chip.Time && !chip.IsHit && chip.Channel == BCourse.GetChannel(0, p == 1) && BGame.NowState > EState.None)
                                            {
                                                if (chip.LongEnd != null)
                                                {
                                                    if (chip.Pushing)
                                                    {
                                                        if (BGame.Timer.Value >= chip.LongEnd.Time)
                                                        {
                                                            chip.IsHit = true;
                                                            BScore.ScoreSet(EJudge.Auto, 0);
                                                            chip.Pushing = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PlayKey(i);
                                                        chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                        chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                        if (chip.Time >= time) chip.Sound.Play();
                                                        if (PlayData.Data.LNType > 0) BScore.ScoreSet(EJudge.Auto, 0);
                                                        chip.Pushing = true;
                                                    }
                                                }
                                                else
                                                {
                                                    chip.IsHit = true;
                                                    PlayKey(i);
                                                    chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                    chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                    if (chip.Time >= time) chip.Sound.Play();
                                                    BScore.ScoreSet(EJudge.Auto, 0);
                                                }

                                            }
                                        }
                                    }
                                    #endregion
                                    break;
                                case EAuto.Auto:
                                    #region Auto
                                    for (int i = 0; i < BGame.Chips[0].Count; i++)
                                    {
                                        Chip chip = BGame.Chips[0][i];
                                        if (BGame.Timer.Value >= chip.Time && !chip.IsHit && BCourse.GetLane(chip) >= 0 && BGame.NowState > EState.None)
                                        {
                                            if (chip.LongEnd != null)
                                            {
                                                if (chip.Pushing)
                                                {
                                                    if (BGame.Timer.Value >= chip.LongEnd.Time)
                                                    {
                                                        chip.IsHit = true;
                                                        BScore.ScoreSet(EJudge.Auto, 0);
                                                        chip.Pushing = false;
                                                    }
                                                }
                                                else
                                                {
                                                    PlayKey(BCourse.GetLane(chip));
                                                    chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                    chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                    if (chip.Time >= time) chip.Sound.Play();
                                                    if (PlayData.Data.LNType > 0) BScore.ScoreSet(EJudge.Auto, 0);
                                                    chip.Pushing = true;
                                                }
                                            }
                                            else
                                            {
                                                chip.IsHit = true;
                                                PlayKey(BCourse.GetLane(chip));
                                                chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                if (chip.Time >= time) chip.Sound.Play();
                                                BScore.ScoreSet(EJudge.Auto, 0);
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        else
                        {
                            #region Key
                            if (BGame.Playmode[0] == EAuto.Normal)
                            {
                                for (int i = PlayData.Data.AutoSclatch[0] ? 1 : 0; i < 8; i++)
                                {
                                    int ii = p == 1 ? 8 + i : i;
                                    if (Pushed(i, p))
                                    {
                                        PlayKey(i);
                                        if (BGame.KeySound[0][ii] != null)
                                        {
                                            BGame.KeySound[0][ii].Volume = PlayData.Data.SE / 100.0;
                                            BGame.KeySound[0][ii].PlaySpeed = PlayData.Data.PlaySpeed;
                                            BGame.KeySound[0][ii].Play();
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    if (BGame.NowState > EState.None && BGame.Chips[p] != null && (p == 0 || BGame.Play2P))
                    {
                        double time = BGame.AutoTime[p] > 0 ? BGame.AutoTime[p] : (BGame.StartMeasure == 0 ? BGame.Timer.Begin : (long)BGame.Bars[p][BGame.StartMeasure - 1].Time);
                            #region BGM
                            for (int i = 0; i < BGame.Chips[p].Count; i++)
                            {
                                Chip chip = BGame.Chips[p][i];
                                if (BGame.Timer.Value >= chip.Time && !chip.IsHit && chip.Channel == 1 && chip.Time >= time)
                                {
                                    chip.IsHit = true;
                                    chip.Sound.Volume = PlayData.Data.GameBGM / 100.0;
                                    chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                    chip.Sound.Pan = SetPan(p);
                                    chip.Sound.Play();
                                }
                            }
                            #endregion
                        if (BGame.NowState < EState.End)
                        {
                            int ps = p == 0 && PlayData.Data.Is2PSide ? 1 : p;
                            switch (BGame.Playmode[p])
                            {
                                case EAuto.Normal:
                                    #region Normal
                                    for (int i = 0; i < BGame.Chips[p].Count; i++)
                                    {
                                        Chip chip = BGame.Chips[p][i];
                                        int l = ps == 1 ? BCourse.GetLane2P(chip) : BCourse.GetLane(chip);
                                        if (chip.LongEnd != null)
                                        {
                                            if (BCourse.GetLane(chip) > 0 || (!PlayData.Data.AutoSclatch[p] && BCourse.GetLane(chip) == 0))
                                            {
                                                if (!chip.IsHit)
                                                {
                                                    if (PlayData.Data.LNType == 0)
                                                    {
                                                        if (!chip.IsMiss)
                                                        {
                                                            if (chip.TopPushed)
                                                            {
                                                                if (!chip.Pushing)
                                                                {
                                                                    if (GetJudge(BGame.Timer.Value - (chip.LongEnd.Time + BGame.Adjust[p])) < EJudge.Poor)
                                                                    {
                                                                        BScore.ScoreSet(chip, p, 3);
                                                                        chip.IsHit = true;
                                                                        chip.Pushing = false;
                                                                    }
                                                                    else
                                                                    {
                                                                        BScore.ScoreSet(EJudge.Through, p);
                                                                        chip.IsMiss = true;
                                                                        BGame.ChargeSound[p][l].Stop();
                                                                    }
                                                                }
                                                                if (chip.Pushing && BGame.Timer.Value > chip.LongEnd.Time)
                                                                {
                                                                    BScore.ScoreSet(chip, p, 3);
                                                                    chip.IsHit = true;
                                                                    chip.Pushing = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (chip.Time + 216.6666 < BGame.Timer.Value)
                                                                {
                                                                    BScore.ScoreSet(EJudge.Through, p);
                                                                    chip.IsMiss = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        if (chip.TopPushed)
                                                        {
                                                            if (chip.LongEnd.Time + 216.6666 < BGame.Timer.Value && !chip.LongEnd.IsMiss && !chip.IsMiss)
                                                            {
                                                                BScore.ScoreSet(EJudge.Through, p);
                                                                chip.IsMiss = true;
                                                                chip.LongEnd.IsMiss = true;
                                                                chip.Pushing = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (chip.LongEnd.Time < BGame.Timer.Value && !chip.LongEnd.IsMiss && chip.IsMiss)
                                                            {
                                                                BScore.ScoreSet(EJudge.Through, p);
                                                                chip.IsMiss = true;
                                                                chip.LongEnd.IsMiss = true;
                                                                chip.Pushing = false;
                                                            }
                                                            if (chip.Time + 216.6666 < BGame.Timer.Value && !chip.IsMiss)
                                                            {
                                                                BScore.ScoreSet(EJudge.Through, p);
                                                                chip.IsMiss = true;
                                                            }
                                                        }
                                                        if (chip.LongEnd.Time < BGame.Timer.Value && chip.LongEnd.IsMiss && chip.Pushing)
                                                        {
                                                            chip.Pushing = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((BGame.NowState == EState.None ? time < BGame.Timer.Value : chip.Time + 216.6666 < BGame.Timer.Value) && !chip.IsHit && !chip.IsMiss && l >= 0)
                                            {
                                                BScore.ScoreSet(EJudge.Through, p);
                                                chip.IsMiss = true;
                                            }
                                        }
                                    }


                                    for (int i = PlayData.Data.AutoSclatch[p] ? 1 : 0; i < 8; i++)
                                    {
                                        Chip chip = NearChip(BGame.Chips[p], BCourse.GetChannel(i));
                                        if (chip != null)
                                        {
                                            if (chip.Sound != null)
                                            {
                                                if (chip.LongEnd != null)
                                                {
                                                    BGame.ChargeSound[p][i] = chip.Sound;
                                                }

                                                else
                                                {
                                                    BGame.KeySound[p][i] = chip.Sound;
                                                    BGame.ChargeSound[p][i] = null;
                                                }
                                            }

                                            if (chip.LongEnd != null)
                                            {
                                                if (PlayData.Data.LNType == 0)
                                                {
                                                    if (chip.Pushing)
                                                    {
                                                        if (Left(i, ps, p))
                                                        {
                                                            chip.Pushing = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Pushed(i, ps, p) && GetJudge(BGame.Timer.Value - (chip.Time + BGame.Adjust[p])) < EJudge.Poor && !chip.IsMiss)
                                                        {
                                                            PlayKey(i);
                                                            if (BGame.ChargeSound[p][i] != null)
                                                            {
                                                                BGame.ChargeSound[p][i].Volume = PlayData.Data.SE / 100.0;
                                                                BGame.ChargeSound[p][i].PlaySpeed = PlayData.Data.PlaySpeed;
                                                                BGame.ChargeSound[p][i].Pan = SetPan(p);
                                                                BGame.ChargeSound[p][i].Play();
                                                            }
                                                            chip.Pushing = true;
                                                            chip.TopPushed = true;
                                                            chip.PushTime = BGame.Timer.Value;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (chip.Pushing)
                                                    {
                                                        if (Left(i, ps, p))
                                                        {
                                                            BScore.ScoreSet(chip, p, 2);
                                                            chip.Pushing = false;
                                                            if (PlayData.Data.LNType == 2)
                                                            {
                                                                if (!chip.IsMiss) chip.LongEnd.PushTime = BGame.Timer.Value;
                                                                else BGame.ChargeSound[0][i].Stop();
                                                            }
                                                            else chip.LongEnd.PushTime = BGame.Timer.Value;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Pushed(i, ps, p) && GetJudge(BGame.Timer.Value - (chip.Time + BGame.Adjust[p])) < EJudge.Poor && !chip.IsMiss)
                                                        {
                                                            PlayKey(i);
                                                            if (BGame.ChargeSound[p][i] != null)
                                                            {
                                                                BGame.ChargeSound[p][i].Volume = PlayData.Data.SE / 100.0;
                                                                BGame.ChargeSound[p][i].PlaySpeed = PlayData.Data.PlaySpeed;
                                                                BGame.ChargeSound[p][i].Pan = SetPan(p);
                                                                BGame.ChargeSound[p][i].Play();
                                                            }
                                                            BScore.ScoreSet(chip, p, 1);
                                                            chip.Pushing = true;
                                                            if (PlayData.Data.LNType < 2 || !chip.IsMiss)
                                                            {
                                                                chip.TopPushed = true;
                                                                chip.PushTime = BGame.Timer.Value;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Pushed(i, ps, p))
                                                {
                                                    PlayKey(i);
                                                    if (BGame.KeySound[p][i] != null)
                                                    {
                                                        BGame.KeySound[p][i].Volume = PlayData.Data.SE / 100.0;
                                                        BGame.KeySound[p][i].PlaySpeed = PlayData.Data.PlaySpeed;
                                                        BGame.KeySound[p][i].Pan = SetPan(p);
                                                        BGame.KeySound[p][i].Play();
                                                    }
                                                    BScore.ScoreSet(chip, p);
                                                    chip.PushTime = BGame.Timer.Value;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Pushed(i, ps, p))
                                            {
                                                PlayKey(i);
                                                if (BGame.KeySound[p][i] != null)
                                                {
                                                    BGame.KeySound[p][i].Volume = PlayData.Data.SE / 100.0;
                                                    BGame.KeySound[p][i].PlaySpeed = PlayData.Data.PlaySpeed;
                                                    BGame.KeySound[p][i].Pan = SetPan(p);
                                                    BGame.KeySound[p][i].Play();
                                                }
                                            }
                                        }
                                    }
                                    if (PlayData.Data.AutoSclatch[p])
                                    {
                                        for (int i = 0; i < BGame.Chips[p].Count; i++)
                                        {
                                            Chip chip = BGame.Chips[p][i];
                                            if (BGame.Timer.Value >= chip.Time && !chip.IsHit && chip.Channel == BCourse.GetChannel(0) && BGame.NowState > EState.None)
                                            {
                                                if (chip.LongEnd != null)
                                                {
                                                    if (chip.Pushing)
                                                    {
                                                        if (BGame.Timer.Value >= chip.LongEnd.Time)
                                                        {
                                                            chip.IsHit = true;
                                                            BScore.ScoreSet(EJudge.Auto, p);
                                                            chip.Pushing = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PlayKey(i);
                                                        chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                        chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                        chip.Sound.Pan = SetPan(p);
                                                        if (chip.Time >= time) chip.Sound.Play();
                                                        if (PlayData.Data.LNType > 0) BScore.ScoreSet(EJudge.Auto, p);
                                                        chip.Pushing = true;
                                                    }
                                                }
                                                else
                                                {
                                                    chip.IsHit = true;
                                                    PlayKey(i);
                                                    chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                    chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                    chip.Sound.Pan = SetPan(p);
                                                    if (chip.Time >= time) chip.Sound.Play();
                                                    BScore.ScoreSet(EJudge.Auto, p);
                                                }

                                            }
                                        }
                                    }
                                    #endregion
                                    break;
                                case EAuto.Auto:
                                    #region Auto
                                    for (int i = 0; i < BGame.Chips[p].Count; i++)
                                    {
                                        Chip chip = BGame.Chips[p][i];
                                        if (BGame.Timer.Value >= chip.Time && !chip.IsHit && BCourse.GetLane(chip) >= 0 && BGame.NowState > EState.None)
                                        {
                                            if (chip.LongEnd != null)
                                            {
                                                if (chip.Pushing)
                                                {
                                                    if (BGame.Timer.Value >= chip.LongEnd.Time)
                                                    {
                                                        chip.IsHit = true;
                                                        BScore.ScoreSet(EJudge.Auto, p);
                                                        chip.Pushing = false;
                                                    }
                                                }
                                                else
                                                {
                                                    PlayKey(BCourse.GetLane(chip));
                                                    chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                    chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                    chip.Sound.Pan = SetPan(p);
                                                    if (chip.Time >= time) chip.Sound.Play();
                                                    if (PlayData.Data.LNType > 0) BScore.ScoreSet(EJudge.Auto, p);
                                                    chip.Pushing = true;
                                                }
                                            }
                                            else
                                            {
                                                PlayKey(BCourse.GetLane(chip));
                                                chip.IsHit = true;
                                                chip.Sound.Volume = PlayData.Data.SE / 100.0;
                                                chip.Sound.PlaySpeed = PlayData.Data.PlaySpeed;
                                                chip.Sound.Pan = SetPan(p);
                                                if (chip.Time >= time) chip.Sound.Play();
                                                BScore.ScoreSet(EJudge.Auto, p);
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        else
                        {
                            #region Key
                            if (BGame.Playmode[p] == EAuto.Normal)
                            {
                                int ps = p == 0 && PlayData.Data.Is2PSide ? 1 : p;
                                for (int i = PlayData.Data.AutoSclatch[p] ? 1 : 0; i < 8; i++)
                                {
                                    if (Pushed(i, ps, p))
                                    {
                                        PlayKey(i);
                                        if (BGame.KeySound[p][i] != null)
                                        {
                                            BGame.KeySound[p][i].Volume = PlayData.Data.SE / 100.0;
                                            BGame.KeySound[p][i].PlaySpeed = PlayData.Data.PlaySpeed;
                                            BGame.KeySound[p][i].Pan = SetPan(p);
                                            BGame.KeySound[p][i].Play();
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
        }

        public static void PlayKey(int lane)
        {
            if (PlayData.Data.Hitsound)
            {
                Sfx.KeySound[lane].Volume = PlayData.Data.SE / 100.0;
                Sfx.KeySound[lane].Stop();
                Sfx.KeySound[lane].Play();
            }
        }

        public static int SetPan(int player)
        {
            if (BGame.Play2P)
            {
                if (player == 1) return 255;
                else return -255;
            }
            return 0;
        }

        public static EJudge GetJudge(double time)
        {
            time = Math.Abs(time);
            if (time <= 16.6666) return EJudge.Perfect;
            else if (time <= 33.3333) return EJudge.Great;
            else if (time <= 116.6666) return EJudge.Good;
            else if (time <= 216.6666) return EJudge.Bad;
            else return EJudge.Poor;
        }

        public static Chip NearChip(List<Chip> list, int lane)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Chip chip = list[i];
                if (chip.Channel == lane && !chip.IsHit && (!chip.IsMiss || (chip.LongEnd != null && PlayData.Data.LNType == 2)))
                {
                    if (chip.LongEnd != null)
                    {
                        if (chip.Time < BGame.Timer.Value && chip.LongEnd.Time + 216.6666 > BGame.Timer.Value)
                        {
                            return chip;
                        }
                    }
                }
            }

            List<Chip> chipList = new List<Chip>();
            foreach (Chip chip in list)
            {
                if (chip.Channel == lane && GetJudge(BGame.Timer.Value - chip.Time) < EJudge.Poor)
                {
                    chipList.Add(chip);
                }
            }
            chipList.Sort((a, b) => { int r = (int)(Math.Abs(BGame.Timer.Value - a.Time) - Math.Abs(BGame.Timer.Value - b.Time)); return r; });
            for (int c = 0; c < chipList.Count; c++)
            {
                Chip cchip = chipList[c];
                for (int i = 0; i < list.Count; i++)
                {
                    Chip chip = list[i];
                    if (chip.Time == cchip.Time && chip.Channel == cchip.Channel)
                    {
                        return chip;
                    }
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                Chip chip = list[i];
                if (chip.Channel == lane && !chip.IsHit && (!chip.IsMiss || (chip.LongEnd != null && PlayData.Data.LNType == 2)))
                {
                    if (chip.LongEnd != null)
                    {
                        if (chip.Time - 533.3333 < BGame.Timer.Value && chip.LongEnd.Time + 216.6666 > BGame.Timer.Value)
                        {
                            return chip;
                        }
                    }
                    else
                    {
                        if (chip.Time - 533.3333 < BGame.Timer.Value && chip.Time + 216.6666 > BGame.Timer.Value)
                        {
                            return chip;
                        }
                    }
                }
            }
            return null;
        }
        public static Chip NowChip(List<Chip> list, int lane)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Chip chip = list[i];
                if (chip.Time - 1 <= BGame.Timer.Value && chip.Channel == lane)
                {
                    return chip;
                }

            }
            return null;
        }
        public static Bar NowBar(List<Bar> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Bar chip = list[i];
                if (chip.Time - 1 <= BGame.Timer.Value)
                {
                    return chip;
                }
            }
            return null;
        }
        public static double NowBPM(List<BPMList> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                BPMList chip = list[i];
                if (chip.Time - 1 <= BGame.Timer.Value)
                {
                    return chip.Value;
                }
            }
            return BGame.BMS[0].Course.BPM;
        }

        public static bool Finished()
        {
            bool fin = true;
            for (int i = 0; i < 8; i++)
            {
                Chip c = null;
                for (int j = BGame.BMS[0].Course.ListChip.Count - 1; j >= 0; j--)
                {
                    Chip chip = BGame.BMS[0].Course.ListChip[j];
                    if (chip.Channel == BCourse.GetChannel(i) || chip.Channel == BCourse.GetChannel(i, true))
                    {
                        c = chip;
                        break;
                    }
                }
                if (c != null && ((!c.IsHit && !c.IsMiss) || BGame.Timer.Value <= c.Time + 216.6666)) fin = false;
            }
            return fin;
        }

        public static bool Pushed(int value, int player, int trueplayer = -1)
        {
            BMSKey key = player == 1 ? PlayData.Data.BMSKey2P : PlayData.Data.BMSKey;
            int joyp = trueplayer >= 0 ? trueplayer : player;
            BMSKey jkey = joyp == 1 ? PlayData.Data.BMSKey2P : PlayData.Data.BMSKey;
            switch (value)
            {
                case 0:
                    return Key.IsPushed(key.Scr_F) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F)
                        || Key.IsPushed(key.Scr_R) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
                case 1:
                    return Key.IsPushed(key.Key1) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key1);
                case 2:
                    return Key.IsPushed(key.Key2) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)(jkey.Pad_Key2));
                case 3:
                    return Key.IsPushed(key.Key3) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key3);
                case 4:
                    return Key.IsPushed(key.Key4) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key4);
                case 5:
                    return Key.IsPushed(key.Key5) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key5);
                case 6:
                    return Key.IsPushed(key.Key6) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key6);
                case 7:
                    return Key.IsPushed(key.Key7) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key7);
                case 8:
                    return Key.IsPushed(key.Start) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Start);
                case 9:
                    return Key.IsPushed(key.Select) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Select);
                case 10:
                    return Key.IsPushed(key.Scr_F) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F);
                case 11:
                    return Key.IsPushed(key.Scr_R) || Joypad.IsPushed(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
            }
            return false;
        }
        public static bool Pushing(int value, int player, int trueplayer = -1)
        {
            BMSKey key = player == 1 ? PlayData.Data.BMSKey2P : PlayData.Data.BMSKey;
            int joyp = trueplayer >= 0 ? trueplayer : player;
            BMSKey jkey = joyp == 1 ? PlayData.Data.BMSKey2P : PlayData.Data.BMSKey;
            if (player == 1)
            {
                switch (value)
                {
                    case 7:
                        return Key.IsPushing(key.Scr_F) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F)
                            || Key.IsPushing(key.Scr_R) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
                    case 0:
                        return Key.IsPushing(key.Key1) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key1);
                    case 1:
                        return Key.IsPushing(key.Key2) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key2);
                    case 2:
                        return Key.IsPushing(key.Key3) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key3);
                    case 3:
                        return Key.IsPushing(key.Key4) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key4);
                    case 4:
                        return Key.IsPushing(key.Key5) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key5);
                    case 5:
                        return Key.IsPushing(key.Key6) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key6);
                    case 6:
                        return Key.IsPushing(key.Key7) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key7);
                    case 8:
                        return Key.IsPushing(key.Start) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Start);
                    case 9:
                        return Key.IsPushing(key.Select) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Select);
                    case 10:
                        return Key.IsPushing(key.Scr_F) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F);
                    case 11:
                        return Key.IsPushing(key.Scr_R) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
                }
            }
            else
            {
                switch (value)
                {
                    case 0:
                        return Key.IsPushing(key.Scr_F) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F)
                            || Key.IsPushing(key.Scr_R) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
                    case 1:
                        return Key.IsPushing(key.Key1) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key1);
                    case 2:
                        return Key.IsPushing(key.Key2) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key2);
                    case 3:
                        return Key.IsPushing(key.Key3) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key3);
                    case 4:
                        return Key.IsPushing(key.Key4) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key4);
                    case 5:
                        return Key.IsPushing(key.Key5) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key5);
                    case 6:
                        return Key.IsPushing(key.Key6) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key6);
                    case 7:
                        return Key.IsPushing(key.Key7) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key7);
                    case 8:
                        return Key.IsPushing(key.Start) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Start);
                    case 9:
                        return Key.IsPushing(key.Select) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Select);
                    case 10:
                        return Key.IsPushing(key.Scr_F) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F);
                    case 11:
                        return Key.IsPushing(key.Scr_R) || Joypad.IsPushing(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
                }
            }
            return false;
        }
        public static bool Left(int value, int player, int trueplayer = -1)
        {
            int joyp = trueplayer >= 0 ? trueplayer : player;
            BMSKey key = player == 1 ? PlayData.Data.BMSKey2P : PlayData.Data.BMSKey;
            BMSKey jkey = joyp == 1 ? PlayData.Data.BMSKey2P : PlayData.Data.BMSKey;
            switch (value)
            {
                case 0:
                    return Key.IsLeft(key.Scr_F) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F)
                        || Key.IsLeft(key.Scr_R) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
                case 1:
                    return Key.IsLeft(key.Key1) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key1);
                case 2:
                    return Key.IsLeft(key.Key2) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)(JoypadButton)jkey.Pad_Key2);
                case 3:
                    return Key.IsLeft(key.Key3) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key3);
                case 4:
                    return Key.IsLeft(key.Key4) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key4);
                case 5:
                    return Key.IsLeft(key.Key5) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key5);
                case 6:
                    return Key.IsLeft(key.Key6) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key6);
                case 7:
                    return Key.IsLeft(key.Key7) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Key7);
                case 10:
                    return Key.IsLeft(key.Scr_F) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_F);
                case 11:
                    return Key.IsLeft(key.Scr_R) || Joypad.IsLeft(PlayData.Data.Controller[joyp], (JoypadButton)jkey.Pad_Scr_R);
            }
            return false;
        }
    }
}
