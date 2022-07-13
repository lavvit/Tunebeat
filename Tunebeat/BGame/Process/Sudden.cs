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
                        Drawing.Box(x[i], 0, 432, 722 * (SuddenNumber[i] / 1000.0), Drawing.Color(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]));
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
                if (Key.IsPushed(EKey.W))
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
                if (Key.IsPushed(EKey.P) && BGame.Play2P)
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
                //if (Key.IsPushing(EKey.W)) SuddenUpdate(0);
                //if (Key.IsPushing(EKey.P)) SuddenUpdate(1);
            }
        }

        public static int GetGreenNumber(int player)
        {
            double bpm = BProcess.NowBPM(BGame.BMS[player].Course.ListCBPM);
            int sudden = UseSudden[player] ? SuddenNumber[player] : 0;
            double suddenrate = 1000.0 / (1000 - sudden);
            double scroll = BNotes.Scroll[player];
            return (int)(Showms / (bpm * scroll * suddenrate));
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
                SuddenNumber[player] = PlayData.Data.SuddenNumber[player];
            }
            else
            {
                if (isPlus)
                    SuddenNumber[player]++;
                else SuddenNumber[player]--;
            }

            if (PlayData.Data.FloatingHiSpeed[player]) SetScroll(player);
        }
    }
}
