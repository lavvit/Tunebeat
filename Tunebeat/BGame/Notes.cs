using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SeaDrop;
using BMSParse;

namespace Tunebeat
{
    public class BNotes
    {
        #region Function
        public static double[] Scroll = new double[5];
        #endregion
        public static void Draw()
        {
            if (BGame.NowCourse[0].Player == 2)
            {
                DrawNotesDP();
            }
            else
            {
                DrawNotes(0);
                if (BGame.Play2P) DrawNotes(1);
            }
        }
        public static void DrawNotes(int player)
        {
            int[] x = new int[2] { 70, 1408 };
            int p = player == 0 && PlayData.Data.Is2PSide ? 1 : player;
            Tx.BMS_Lane.Draw(x[p], 0, new Rectangle(442 * p, 0, 442, 726));

            int[] lx = new int[2] { 75, 1413 };
            DrawBar(lx[p], BGame.Bars[player], p, player == 0);
            DrawNotes(lx[p], BGame.Chips[player], p, player == 0);
        }
        public static void DrawNotesDP()
        {
            int[] x = new int[2] { 70 + 384, 1408 - 384 };
            Tx.BMS_Lane.Draw(x[0], 0, new Rectangle(0, 0, 442, 726));
            Tx.BMS_Lane.Draw(x[1], 0, new Rectangle(442, 0, 442, 726));

            int[] lx = new int[2] { 75 + 384, 1413 - 384 };
            DrawBar(lx[0], BGame.Bars[0], 0, true);
            DrawBar(lx[1], BGame.Bars[0], 1, true);
            DrawNotes(lx[0], BGame.Chips[0], 0, true, false);
            DrawNotes(lx[1], BGame.Chips[0], 1, true, true);
        }
        public static void DrawBar(int x, List<Bar> listbar, int player, bool is1P)
        {
            if (listbar != null)
            {
                Drawing.Box(x, 722, 432, -8, 0xff0000);
                Drawing.Text(x + 4, 700, $"{(BProcess.NowBar(listbar) != null ? BProcess.NowBar(listbar).Number : 0)}");
                for (int i = 0; i < listbar.Count; i++)
                {
                    Bar bar = listbar[i];
                    double time = bar.Time - BGame.Timer.Value;
                    double y = 722 - (time * bar.BPM / 332.42 * Scroll[is1P ? 0 : player]);
                    if (y >= 0 && y < 726)
                    {
                        Drawing.Box(x, y, 432, -4, 0xc0c0c0);
                        //Drawing.Text(x[p] + 450, (int)y, $"{bar.BPM}");
                    }
                }
            }
        }
        public static void DrawNotes(int x, List<Chip> listchip, int player, bool is1P, bool is2PLane = false)
        {
            int length = 12;
            int[] xx = player == 1 ? new int[8] { 0, 54, 96, 150, 192, 246, 288, 342 } : new int[8] { 0, 93, 147, 189, 243, 285, 339, 381 };
            int hcl = PlayData.Data.LNType == 2 ? 72 : 0;

            if (listchip != null)
            {
                for (int j = listchip.Count - 1; j >= 0; j--)
                {
                    Chip chip = listchip[j];
                    double cy = 722 - length - ((chip.Time - BGame.Timer.Value) * chip.Bpm / 332.42 * Scroll[is1P ? 0 : player]);
                    cy = cy > 722 - length ? 722 - length : cy;
                    if (chip.ID != "00")
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            bool miss = chip.LongEnd != null ? (PlayData.Data.LNType == 0 ? (chip.IsMiss && BGame.Timer.Value > chip.LongEnd.Time) : (chip.LongEnd.IsMiss && BGame.Timer.Value > chip.LongEnd.Time)) : chip.IsMiss;
                            if ((player == 1 ? BCourse.GetLane2P(chip, is2PLane) : BCourse.GetLane(chip, is2PLane)) == i && !chip.IsHit && !miss && (BGame.NowState > EState.None || BGame.Timer.Value <= chip.Time))
                            {
                                if (chip.LongEnd != null)
                                {
                                    double ly = 722 - length + 12 - ((chip.LongEnd.Time - BGame.Timer.Value) * chip.LongEnd.Bpm / 332.42 * Scroll[is1P ? 0 : player]);
                                    //ly = ly > 722 - length - 18 ? 722 - length - 18 : ly;
                                    if (cy >= 0 && ly < 1080 + length)
                                    {
                                        if (Tx.BMS_Long.IsEnable && Tx.BMS_Long_Charge.IsEnable && Tx.BMS_Long_Hell.IsEnable && Tx.BMS_Long_Hell_Charge.IsEnable)
                                        {
                                            Texture tx;
                                            if (PlayData.Data.LNType < 2)
                                            {
                                                if (!chip.Pushing) tx = Tx.BMS_Long;
                                                else tx = Tx.BMS_Long_Charge;
                                            }
                                            else
                                            {
                                                if (!chip.Pushing) tx = Tx.BMS_Long_Hell;
                                                else tx = Tx.BMS_Long_Hell_Charge;
                                            }
                                            tx.Draw(x + xx[i], ly + 18, 1, (cy - ly - 18) / 18.0, new Rectangle(RectX(chip, player).Item1, 0, RectX(chip, player).Item2, 18));
                                        }
                                        else Tx.BMS_Notes.Draw(x + xx[i], ly + 18, 1, (cy - ly - 18) / 18.0, new Rectangle(RectX(chip, player).Item1, (chip.Pushing ? 48 : 30) + hcl, RectX(chip, player).Item2, 18));
                                        Tx.BMS_Notes.Draw(x + xx[i], cy - 6, new Rectangle(RectX(chip, player).Item1, 66 + hcl, RectX(chip, player).Item2, 18));//start
                                        if (PlayData.Data.LNType > 0) Tx.BMS_Notes.Draw(x + xx[i], ly, new Rectangle(RectX(chip, player).Item1, 12 + hcl, RectX(chip, player).Item2, 18));//end
                                        //Drawing.Text(x[i], (int)cy, $"{chip.LongEnd.Time}");
                                    }
                                }
                                else
                                {
                                    if (cy >= 0 && cy < 1080 + length)
                                    {
                                        Tx.BMS_Notes.Draw(x + xx[i], cy, new Rectangle(RectX(chip, player).Item1, 0, RectX(chip, player).Item2, 12));
                                    }
                                }
                                //Drawing.Text(x[i], (int)cy, $"{chip.Bpm}");
                            }
                        }
                    }
                }
                for (int i = 0; i < listchip.Count; i++)
                {
                    //Drawing.Text(1000, 20 * i, $"{listchip[i].Time,6} {listchip[i].Channel} {listchip[i].Place} {listchip[i].ID} {(listchip[i].LongEnd != null ? "Charge" : "")}");
                }
            }
        }

        public static (int, int) RectX(Chip chip, int player)
        {
            switch (BCourse.GetLane(chip))
            {
                case 0:
                    if (BGame.AutoScratch[player]) return (0, 90);
                    else return (90, 90);
                case 1:
                case 3:
                case 5:
                case 7:
                    return (180, 51);
                case 2:
                case 4:
                case 6:
                    return (231, 39);
                default:
                    return (0, 0);
            }
        }
    }
}
