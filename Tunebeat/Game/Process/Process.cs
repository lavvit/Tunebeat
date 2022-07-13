using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class Process
    {
        public static bool[] IsLeft = new bool[5];
        public static Counter[] RollTimer = new Counter[5];
        public static Counter[][] DonTimer = new Counter[5][];

        public static void Init()
        {
            for (int i = 0; i < NewGame.Chips.Length; i++)
            {
                foreach (Chip chip in NewGame.Chips[i])
                {
                    chip.IsHit = false;
                    chip.IsMiss = false;
                    chip.RollCount = 0;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                IsLeft[i] = false;
                RollTimer[i] = new Counter(0, (long)(1000.0 / PlayData.Data.AutoRoll), 1000, false);
                DonTimer[i] = new Counter[4];
                for (int j = 0; j < 4; j++)
                {
                    DonTimer[i][j] = new Counter(0, 99, 1000, false);
                }
            }
        }

        public static void Draw()
        {
            int lane = PlayData.Data.PreviewType == 3 ? 5 : 2;
            for (int player = 0; player < lane; player++)
            {
                if (lane == 2 && player == 1 && !NewGame.Play2P) break;

                if (NewCreate.Mapping)
                {
                    if (Key.IsPushing(PlayData.Data.LEFTDON)) DrawTaiko(0, player);
                    if (Key.IsPushing(PlayData.Data.RIGHTDON)) DrawTaiko(1, player);
                    if (Key.IsPushing(PlayData.Data.LEFTKA)) DrawTaiko(2, player);
                    if (Key.IsPushing(PlayData.Data.RIGHTKA)) DrawTaiko(3, player);
                }
                else
                {
                    if (NewGame.Playmode[player] >= EAuto.Auto)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (DonTimer[player][i].State != 0)
                            {
                                DrawTaiko(i, player);
                            }
                        }
                    }
                    else
                    {
                        List<int> ld = player == 0 ? PlayData.Data.LEFTDON : PlayData.Data.LEFTDON2P;
                        List<int> rd = player == 0 ? PlayData.Data.RIGHTDON : PlayData.Data.RIGHTDON2P;
                        List<int> lk = player == 0 ? PlayData.Data.LEFTKA : PlayData.Data.LEFTKA2P;
                        List<int> rk = player == 0 ? PlayData.Data.RIGHTKA : PlayData.Data.RIGHTKA2P;
                        if (Key.IsPushing(ld)) DrawTaiko(0, player);
                        if (Key.IsPushing(rd)) DrawTaiko(1, player);
                        if (Key.IsPushing(lk)) DrawTaiko(2, player);
                        if (Key.IsPushing(rk)) DrawTaiko(3, player);
                    }
                }
            }
#if DEBUG
            Drawing.Text(300, 80, $"LK:{Key.HoldTime(PlayData.Data.LEFTKA)}");
            Drawing.Text(300, 100, $"LD:{Key.HoldTime(PlayData.Data.LEFTDON)}");
            Drawing.Text(300, 120, $"RD:{Key.HoldTime(PlayData.Data.RIGHTDON)}");
            Drawing.Text(300, 140, $"RK:{Key.HoldTime(PlayData.Data.RIGHTKA)}");
#endif
        }
        public static void Update()
        {
            int lane = PlayData.Data.PreviewType == 3 ? 5 : 2;
            for (int i = 0; i < lane; i++)
            {
                if (lane == 2 && i == 1 && !NewGame.Play2P) break;

                if (NewCreate.Mapping)
                {
                    List<int> list1 = new List<int>();
                    list1.AddRange(i == 0 ? PlayData.Data.LEFTDON : PlayData.Data.LEFTDON2P);
                    list1.AddRange(i == 0 ? PlayData.Data.RIGHTDON : PlayData.Data.RIGHTDON2P);
                    if (Key.IsPushed(list1))
                    {
                        NewCreate.InputN(true);
                    }
                    List<int> list2 = new List<int>();
                    list2.AddRange(i == 0 ? PlayData.Data.LEFTKA : PlayData.Data.LEFTKA2P);
                    list2.AddRange(i == 0 ? PlayData.Data.RIGHTKA : PlayData.Data.RIGHTKA2P);
                    if (Key.IsPushed(list2))
                    {
                        NewCreate.InputN(false);
                    }
                }
                else
                {
                    if (NewGame.Playmode[i] == EAuto.Auto)
                    {
                        if (NewGame.NowState == EState.Start || NewGame.NowState == EState.Play)
                        {
                            Chip nowchip = NowChip(NewGame.Chips[i], false);
                            ERoll roll = nowchip != null ? nowchip.ERoll : ERoll.None;
                            if (roll != ERoll.None)
                            {
                                if (RollTimer[i].State == 0)
                                {
                                    RollTimer[i].Reset();
                                    RollTimer[i].Start();
                                    AutoRoll(nowchip, i);
                                }
                            }
                            else
                            {
                                if (RollTimer[i].State != 0)
                                {
                                    RollTimer[i].Stop();
                                    RollTimer[i].Reset();
                                }
                            }

                            if (NewGame.AutoTime[i] > 0)
                            {
                                foreach (Chip chip in NewGame.Chips[i])
                                {
                                    if (chip.Time <= NewGame.Timer.Value && !chip.IsHit)
                                    {
                                        if (chip.Time >= NewGame.AutoTime[i])
                                        {
                                            PlaySound(chip, i);
                                            StartTimer(chip, i);
                                            if (chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                                            {
                                                chip.IsHit = true;
                                                NewScore.AddScore(EJudge.Auto, i);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                double time = NewGame.StartMeasure == 0 ? NewGame.Timer.Begin : (long)NewGame.Bars[i][NewGame.StartMeasure - 1].Time;
                                foreach (Chip chip in NewGame.Chips[i])
                                {
                                    if (chip.Time <= NewGame.Timer.Value && !chip.IsHit)
                                    {
                                        if (chip.Time >= time)
                                        {
                                            PlaySound(chip, i);
                                            StartTimer(chip, i);
                                        }
                                        if (chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                                        {
                                            chip.IsHit = true;
                                            NewScore.AddScore(EJudge.Auto, i);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (NewGame.Playmode[i] == EAuto.Normal)
                    {
                        if (!NewGame.Failed[i] || NewGame.Dan != null || PlayData.Data.GaugeAutoShift[i] == (int)EGaugeAutoShift.Continue)
                        {
                            List<int> list1 = new List<int>();
                            list1.AddRange(i == 0 ? PlayData.Data.LEFTDON : PlayData.Data.LEFTDON2P);
                            list1.AddRange(i == 0 ? PlayData.Data.RIGHTDON : PlayData.Data.RIGHTDON2P);
                            if (Key.IsPushed(list1))
                            {
                                Chip chip = NearChip(NewGame.Chips[i], true);
                                PlaySound(true, chip, i);
                                Memory.AddData(i, true, Key.IsPushed(i == 0 ? PlayData.Data.LEFTDON : PlayData.Data.LEFTDON2P));
                                if (NewGame.NowState == EState.Start || NewGame.NowState == EState.Play)
                                {
                                    if (chip != null && (chip.ENote == ENote.Don || chip.ENote == ENote.DON))
                                    {
                                        ChipProcess(chip, i);
                                    }
                                    Chip nchip = NowChip(NewGame.Chips[i], false);
                                    RollProcess(nchip, i);
                                    BalloonProcess(nchip, i);
                                }
                            }
                            List<int> list2 = new List<int>();
                            list2.AddRange(i == 0 ? PlayData.Data.LEFTKA : PlayData.Data.LEFTKA2P);
                            list2.AddRange(i == 0 ? PlayData.Data.RIGHTKA : PlayData.Data.RIGHTKA2P);
                            if (Key.IsPushed(list2))
                            {
                                Chip chip = NearChip(NewGame.Chips[i], false);
                                PlaySound(false, chip, i);
                                Memory.AddData(i, false, Key.IsPushed(i == 0 ? PlayData.Data.LEFTKA : PlayData.Data.LEFTKA2P));
                                if (NewGame.NowState == EState.Start || NewGame.NowState == EState.Play)
                                {
                                    if (chip != null && (chip.ENote == ENote.Ka || chip.ENote == ENote.KA))
                                    {
                                        ChipProcess(chip, i);
                                    }
                                    Chip nchip = NowChip(NewGame.Chips[i], false);
                                    RollProcess(nchip, i);
                                }
                            }
                        }
                    }
                    foreach (Chip chip in NewGame.Chips[i])
                    {
                        if ((NewGame.NowState == EState.Start || NewGame.NowState == EState.Play) && !chip.IsHit && !chip.IsMiss && NewGame.Timer.Value > chip.Time + GetRange(4))
                        {
                            if (chip != null && chip.ENote >= ENote.Don && chip.ENote <= ENote.KA)
                            {
                                chip.IsMiss = true;
                                NewScore.msJudge[i] = 0;
                                Memory.AddChip(i, chip, EJudge.Through);
                                NewScore.AddScore(EJudge.Through, i);
                                NewScore.DrawJudge(ENote.Space, i);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                RollTimer[i].Tick();
                for (int j = 0; j < 4; j++)
                {
                    DonTimer[i][j].Tick();
                }
            }
        }

        public static void ChipProcess(Chip chip, int player)
        {
            chip.IsHit = true;
            Memory.AddChip(player, chip, GetJudge(chip));
            NewScore.AddScore(GetJudge(chip), player);
            NewScore.DrawJudge(chip.ENote, player);
            NewScore.msJudge[player] = NewGame.Timer.Value - NewGame.Adjust[player] - chip.Time;
            if (NewGame.Timer.State != 0 && GetJudge(chip) < EJudge.Poor)
            {
                NewScore.Active.Reset();
                NewScore.Active.Start();
                NewScore.msSum[player] += NewScore.msJudge[player];
                NewScore.Hit[player]++;
            }
        }

        public static void DrawTaiko(int type, int player)
        {
            Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
            Point Point = NewCreate.Enable ? createpoint[player] : NewNotes.NotesP[player];
            switch (type)
            {
                case 0:
                    Tx.Game_Don[player][0].Opacity = 1.0 - ((double)DonTimer[player][0].Value / DonTimer[player][0].End);
                    Tx.Game_Don[player][0].Draw(362, Point.Y + 47);
                    break;
                case 1:
                    Tx.Game_Don[player][1].Opacity = 1.0 - ((double)DonTimer[player][1].Value / DonTimer[player][1].End);
                    Tx.Game_Don[player][1].Draw(362, Point.Y + 47);
                    break;
                case 2:
                    Tx.Game_Ka[player][0].Opacity = 1.0 - ((double)DonTimer[player][2].Value / DonTimer[player][2].End);
                    Tx.Game_Ka[player][0].Draw(362, Point.Y + 47); break;
                case 3:
                    Tx.Game_Ka[player][1].Opacity = 1.0 - ((double)DonTimer[player][3].Value / DonTimer[player][3].End);
                    Tx.Game_Ka[player][1].Draw(362, Point.Y + 47);
                    break;
            }
        }
        public static void PlaySound(Chip chip, int player)
        {
            double volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            switch (chip.ENote)
            {
                case ENote.Don:
                    Sfx.Don[player].Volume = volume;
                    Sfx.Don[player].Play();
                    break;
                case ENote.Ka:
                    Sfx.Ka[player].Volume = volume;
                    Sfx.Ka[player].Play();
                    break;
                case ENote.DON:
                    Sfx.DON[player].Volume = volume * 1.5;
                    Sfx.DON[player].Play();
                    break;
                case ENote.KA:
                    Sfx.KA[player].Volume = volume * 1.5;
                    Sfx.KA[player].Play();
                    break;
            }
        }
        public static void PlaySound(ENote note, int player)
        {
            double volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            switch (note)
            {
                case ENote.Don:
                    Sfx.Don[player].Volume = volume;
                    Sfx.Don[player].Play();
                    break;
                case ENote.Ka:
                    Sfx.Ka[player].Volume = volume;
                    Sfx.Ka[player].Play();
                    break;
                case ENote.DON:
                    Sfx.DON[player].Volume = volume * 1.5;
                    Sfx.DON[player].Play();
                    break;
                case ENote.KA:
                    Sfx.KA[player].Volume = volume * 1.5;
                    Sfx.KA[player].Play();
                    break;
            }
        }
        public static void PlaySound(bool isRed, Chip chip, int player)
        {
            double volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            if (isRed)
            {
                if (chip != null && (chip.ENote == ENote.DON || chip.ENote == ENote.ROLLStart))
                {
                    Sfx.DON[player].Volume = volume * 1.5;
                    Sfx.DON[player].Play();
                }
                else
                {
                    Sfx.Don[player].Volume = volume;
                    Sfx.Don[player].Play();
                }
            }
            else
            {
                if (chip != null && (chip.ENote == ENote.KA || chip.ENote == ENote.ROLLStart))
                {
                    Sfx.KA[player].Volume = volume * 1.5;
                    Sfx.KA[player].Play();
                }
                else
                {
                    Sfx.Ka[player].Volume = volume;
                    Sfx.Ka[player].Play();
                }
            }
        }
        public static void StartTimer(Chip chip, int player)
        {
            switch (chip.ENote)
            {
                case ENote.Don:
                    DonTimer[player][IsLeft[player] ? 0 : 1].Reset();
                    DonTimer[player][IsLeft[player] ? 0 : 1].Start();
                    break;
                case ENote.Ka:
                    DonTimer[player][IsLeft[player] ? 2 : 3].Reset();
                    DonTimer[player][IsLeft[player] ? 2 : 3].Start();
                    break;
                case ENote.DON:
                    DonTimer[player][0].Reset();
                    DonTimer[player][1].Reset();
                    DonTimer[player][0].Start();
                    DonTimer[player][1].Start();
                    break;
                case ENote.KA:
                    DonTimer[player][2].Reset();
                    DonTimer[player][3].Reset();
                    DonTimer[player][2].Start();
                    DonTimer[player][3].Start();
                    break;
            }
            IsLeft[player] = !IsLeft[player];
        }

        public static void AutoRoll(Chip chip, int player)
        {
            double volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            if (chip == null || chip.IsHit) return;
            switch (chip.ERoll)
            {
                case ERoll.Roll:
                    Sfx.Don[player].Volume = volume;
                    Sfx.Don[player].Play();
                    RollProcess(chip, player);
                    break;
                case ERoll.ROLL:
                    Sfx.DON[player].Volume = volume * 1.5;
                    Sfx.DON[player].Play();
                    RollProcess(chip, player);
                    break;
                case ERoll.Balloon:
                case ERoll.Kusudama:
                    for (int i = 0; i < (chip.Balloon == 2 ? 2 : 1); i++)
                    {
                        Sfx.Don[player].Volume = volume;
                        Sfx.Don[player].Play();
                        BalloonProcess(chip, player);
                    }
                    break;
            }
        }
        public static void RollProcess(Chip chip, int player)
        {
            if (chip == null) return;
            switch (chip.ERoll)
            {
                case ERoll.Roll:
                case ERoll.ROLL:
                    chip.RollCount++;
                    NewScore.NowRoll[player]++;
                    NewScore.AddRoll(player);
                    break;
            }
        }
        public static void BalloonProcess(Chip chip, int player)
        {
            double volume = (PlayData.Data.SE / 100.0) * (SongData.NowTJA[0].Header.SEVOL / 100.0);
            if (chip == null) return;
            switch (chip.ERoll)
            {
                case ERoll.Balloon:
                case ERoll.Kusudama:
                    if (!chip.IsHit)
                    {
                        chip.RollCount++;
                        NewScore.NowRoll[player]--;
                        NewScore.AddBalloon(player);
                        if (chip.RollCount >= chip.Balloon)
                        {
                            if (chip.ERoll == ERoll.Balloon)
                            {
                                Sfx.Balloon[player].Volume = volume;
                                //Sfx.Balloon[player].Play();
                            }
                            else
                            {
                                Sfx.Kusudama[player].Volume = volume;
                                //Sfx.Kusudama[player].Play();
                            }
                            chip.IsHit = true;
                        }
                    }
                    break;
            }
        }

        public static Chip NowChip(List<Chip> list, bool includeEnpty = true)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Chip chip = list[i];
                if (chip.Time - 1 <= NewGame.Timer.Value && (includeEnpty || chip.ENote > ENote.Space))
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
                if (chip.Time - 1 <= NewGame.Timer.Value)
                {
                    return chip;
                }
            }
            return null;
        }
        public static Chip NearChip(List<Chip> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Chip chip = list[i];
                if (!chip.IsHit && chip.ENote > ENote.Space)
                {
                    if (chip.Time - GetRange(4) < NewGame.Timer.Value && chip.Time + GetRange(4) > NewGame.Timer.Value)
                    {
                        return chip;
                    }
                }
            }
            return null;
        }
        public static Chip NearChip(List<Chip> list, bool isRed)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Chip chip = list[i];
                if (!chip.IsHit)
                {
                    if (isRed)
                    {
                        if (chip.ENote == ENote.Don || chip.ENote == ENote.DON)
                        {
                            if (chip.Time - GetRange(4) < NewGame.Timer.Value && chip.Time + GetRange(4) > NewGame.Timer.Value)
                            {
                                return chip;
                            }
                        }
                    }
                    else
                    {
                        if (chip.ENote == ENote.Ka || chip.ENote == ENote.KA)
                        {
                            if (chip.Time - GetRange(4) < NewGame.Timer.Value && chip.Time + GetRange(4) > NewGame.Timer.Value)
                            {
                                return chip;
                            }
                        }
                    }

                }
            }
            return null;
        }
        public static Chip NextChip(List<Chip> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].Time > NewGame.Timer.Value)
                {
                    return list[i];
                }
            }
            return null;
        }

        public static double GetRange(int type)
        {
            double[] range = new double[5];
            switch (PlayData.Data.JudgeType)
            {
                case 0://カスタム
                    range[0] = PlayData.Data.JudgePerfect;
                    range[1] = PlayData.Data.JudgeGreat;
                    range[2] = PlayData.Data.JudgeGood;
                    range[3] = PlayData.Data.JudgeBad;
                    range[4] = PlayData.Data.JudgePoor;
                    break;
                case 1://Spica標準
                    range[0] = 25.0;
                    range[1] = 32.0;
                    range[2] = 90.0;
                    range[3] = 125.0;
                    range[4] = 125.0;
                    break;
                case 2://ハードモード
                    range[0] = 12.0;
                    range[1] = 20.0;
                    range[2] = 78.0;
                    range[3] = 113.0;
                    range[4] = 113.0;
                    break;
            }
            if (PlayData.Data.Just) range[2] = 0;

            return range[type];
        }
        public static EJudge GetJudge(Chip chip)
        {
            double[] range = new double[5];
            for (int i = 0; i < 5; i++)
            {
                range[i] = GetRange(i);
            }

            if (chip != null)
            {
                double Difference = Math.Abs(NewGame.Timer.Value - chip.Time);
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
