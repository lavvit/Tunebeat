using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;
using BMSParse;

namespace Tunebeat
{
    public class BSudden
    {
        #region Function
        public static int Showms = 174000;
        public static bool[] UseSudden = new bool[2];
        public static int[] SuddenNumber = new int[2], GreenNumber = new int[2], PreGreen = new int[2], NHSNumber = new int[2];
        public static int[] NHSTargetGNum = new int[20] { 1200, 1000, 800, 700, 650, 600, 550, 500, 480, 460,
            440, 420, 400, 380, 360, 340, 320, 300, 280, 260 };
        public static Counter[] STimer = new Counter[2];
        #endregion
        public static void Init()
        {
            for (int i = 0; i < 2; i++)
            {
                UseSudden[i] = PlayData.Data.BMSUseSudden[i];
                SuddenNumber[i] = PlayData.Data.BMSSuddenNumber[i];
                PreGreen[i] = PlayData.Data.BMSGreenNumber[i];
                NHSNumber[i] = PlayData.Data.BMSNHSSpeed[i];
                if (PlayData.Data.BMSFloatingHiSpeed[i]) SetScroll(i);
                else if (PlayData.Data.BMSNormalHiSpeed[i]) SetNHSScroll(i);
                else BNotes.Scroll[i] = PlayData.Data.BMSScrollSpeed[i];
                STimer[i] = new Counter(0, 249, 1000, false);
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < 2; i++)
            {
                int[] x = new int[2] { 75, 1413 };
                if (UseSudden[i])
                {
                    if (i == 0 && BGame.NowCourse[0].Player == 2)
                    {
                        Drawing.Box(x[0] + 384, 0, 432, 722 * (SuddenNumber[0] / 1000.0), Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
                        Drawing.Box(x[1] - 384, 0, 432, 722 * (SuddenNumber[0] / 1000.0), Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
                    }
                    else
                    {
                        Drawing.Box((i == 0 && PlayData.Data.Is2PSide ? x[1] : x[i]), 0, 432, 722 * (SuddenNumber[i] / 1000.0), Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
                    }
                }

                if (i == 0 || BGame.Play2P)
                {
                    Drawing.Text(0, 400, $"Sud:{SuddenNumber[i]}");
                    Drawing.Text(0, 440, $"Scr:{BNotes.Scroll[i]:0.00}");
                    Drawing.Text(0, 480, $"{GreenNumber[i]}", 0x00ff00);
                }
            }
        }

        public static void Update()
        {
            for (int i = 0; i < 2; i++)
            {
                STimer[i].Tick();
                GreenNumber[i] = GetGreenNumber(i);
                if (BGame.NowState == EState.None)
                {
                    PlayData.Data.BMSUseSudden[i] = UseSudden[i];
                    PlayData.Data.BMSSuddenNumber[i] = SuddenNumber[i];
                    if (!PlayData.Data.BMSFloatingHiSpeed[i] && !PlayData.Data.BMSNormalHiSpeed[i]) PlayData.Data.BMSScrollSpeed[i] = BNotes.Scroll[i];
                    if (PlayData.Data.BMSFloatingHiSpeed[i]) PlayData.Data.BMSGreenNumber[i] = PreGreen[i];
                    PlayData.Data.BMSNHSSpeed[i] = NHSNumber[i];
                    PlayData.Data.BMSInputAdjust[i] = BGame.Adjust[i];
                }

            }
            if (BGame.NowState < EState.End && BGame.Playmode[0] < EAuto.Replay)
            {
                if (BProcess.Pushed(8, 0))
                {
                    if (STimer[0].State == 0)
                    {
                        STimer[0].Reset();
                        STimer[0].Start();
                    }
                    else
                    {
                        UseSudden[0] = !UseSudden[0];
                        if (PlayData.Data.FloatingHiSpeed[0] && BGame.NowState == EState.None) PreGreen[0] = GetGreenNumber(0);
                        //Add(0);
                        STimer[0].Stop();
                    }
                }
                if (BProcess.Pushed(8, 1) && BGame.Play2P)
                {
                    if (STimer[1].State == 0)
                    {
                        STimer[1].Reset();
                        STimer[1].Start();
                    }
                    else
                    {

                        UseSudden[1] = !UseSudden[1];
                        if (PlayData.Data.FloatingHiSpeed[1] && BGame.NowState == EState.None) PreGreen[1] = GetGreenNumber(1);
                        //Add(1);
                        STimer[1].Stop();
                    }
                }
                if (BProcess.Pushing(8, 0)) SuddenUpdate(0);
                if (BProcess.Pushing(8, 1) && BGame.Play2P) SuddenUpdate(1);
            }
        }

        public static void SuddenUpdate(int player)
        {
            if (BProcess.Pushed(2, player) || BProcess.Pushed(4, player) || BProcess.Pushed(6, player))
            {
                if (PlayData.Data.BMSNormalHiSpeed[player] && !PlayData.Data.BMSFloatingHiSpeed[player])
                {
                    if (NHSNumber[player] < 19) NHSNumber[player]++;
                    SetNHSScroll(player);
                }
                else
                {
                    BNotes.Scroll[player] += 0.25;
                    BGame.ScrollRemain[player] += 0.25;
                    if (PlayData.Data.BMSFloatingHiSpeed[player] && BGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                }
                //Add(player);
            }
            if (BProcess.Pushed(1, player) || BProcess.Pushed(3, player) || BProcess.Pushed(5, player) || BProcess.Pushed(7, player))
            {
                if (PlayData.Data.BMSNormalHiSpeed[player] && !PlayData.Data.BMSFloatingHiSpeed[player])
                {
                    if (NHSNumber[player] > 0) NHSNumber[player]--;
                    SetNHSScroll(player);
                }
                else
                {
                    BNotes.Scroll[player] -= 0.25;
                    BGame.ScrollRemain[player] -= 0.25;
                    if (PlayData.Data.BMSFloatingHiSpeed[player] && BGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                }
                //Add(player);
            }
            if (Key.IsPushed(EKey.Left))
            {
                BGame.Adjust[player] += 0.5;
                //Add(player);
            }
            if (Key.IsPushed(EKey.Right))
            {
                BGame.Adjust[player] -= 0.5;
                //Add(player);
            }
            if (IsHolding(11, player))
            {
                if (UseSudden[player])
                {
                    if (SuddenNumber[player] < 1000)
                    {
                        SetSudden(player, true);
                        //Add(player);
                    }
                }
                else if (PlayData.Data.BMSFloatingHiSpeed[player])
                {
                    BNotes.Scroll[player] += 0.01;
                    BGame.ScrollRemain[player] += 0.01;
                    BNotes.Scroll[player] = Math.Round(BNotes.Scroll[player], 2, MidpointRounding.AwayFromZero);
                    if (BGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                    //Add(player);
                }
            }
            if (IsHolding(10, player))
            {
                if (UseSudden[player])
                {
                    if (SuddenNumber[player] > 0)
                    {
                        SetSudden(player, false);
                        //Add(player);
                    }
                }
                else if (PlayData.Data.BMSFloatingHiSpeed[player])
                {
                    BNotes.Scroll[player] -= 0.01;
                    BGame.ScrollRemain[player] -= 0.01;
                    BNotes.Scroll[player] = Math.Round(BNotes.Scroll[player], 2, MidpointRounding.AwayFromZero);
                    if (BGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                    //Add(player);
                }
            }
        }

        public static int GetGreenNumber(int player)
        {
            double bpm = BProcess.NowBPM(BGame.BMS[player].Course.ListCBPM);
            int sudden = UseSudden[player] ? SuddenNumber[player] : 0;
            double suddenrate = 1000.0 / (1000 - sudden);
            double scroll = BNotes.Scroll[player];
            return (int)Math.Round(Showms / (bpm * scroll * suddenrate), 0, MidpointRounding.AwayFromZero);
        }

        public static void SetNHSScroll(int player)
        {
            double bpm = BProcess.NowBPM(BGame.BMS[player].Course.ListCBPM);
            double prescroll = BNotes.Scroll[player];
            BNotes.Scroll[player] = Showms / (NHSTargetGNum[NHSNumber[player]] * bpm);
            BGame.ScrollRemain[player] += BNotes.Scroll[player] - prescroll;
        }

        public static void SetScroll(int player)
        {
            double bpm = BProcess.NowBPM(BGame.BMS[player].Course.ListCBPM);
            int sudden = UseSudden[player] ? SuddenNumber[player] : 0;
            double suddenrate = 1000.0 / (1000 - sudden);
            double prescroll = BNotes.Scroll[player];
            BNotes.Scroll[player] = Showms / (PreGreen[player] * bpm * suddenrate);
            BGame.ScrollRemain[player] += BNotes.Scroll[player] - prescroll;
        }

        public static void SetSudden(int player, bool isPlus, bool Reset = false)
        {
            if (Reset)
            {
                SuddenNumber[player] = PlayData.Data.BMSSuddenNumber[player];
            }
            else
            {
                if (isPlus)
                    SuddenNumber[player]++;
                else SuddenNumber[player]--;
            }

            if (PlayData.Data.BMSFloatingHiSpeed[player]) SetScroll(player);
        }

        public static bool IsHolding(int key, int player)
        {
            counter.Tick();
            if (BProcess.Pushed(key, player))
            {
                counter.Start();
                return true;
            }
            else if (BProcess.Pushing(key, player))
            {
                if (Firsted)
                {
                    if (counter.Value > 20)
                    {
                        counter.Reset();
                        return true;
                    }
                }
                else
                {
                    if (counter.Value > 250)
                    {
                        counter.Reset();
                        Firsted = true;
                        return true;
                    }
                }
            }
            else if (BProcess.Left(key, player))
            {
                counter.Stop();
                counter.Reset();
                Firsted = false;
            }
            return false;
        }
        private static bool Firsted;
        private static Counter counter = new Counter(0, 1000, 1000, false);
    }
}
