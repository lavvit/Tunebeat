using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TJAParse;
using SeaDrop;

namespace Tunebeat
{
    public class Sudden
    {
        #region Function
        public static int[] Showms = new int[2] { 256300, -37000 };
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
                UseSudden[i] = PlayData.Data.UseSudden[i];
                SuddenNumber[i] = PlayData.Data.SuddenNumber[i];
                PreGreen[i] = PlayData.Data.GreenNumber[i];
                NHSNumber[i] = PlayData.Data.NHSSpeed[i];
                if (PlayData.Data.FloatingHiSpeed[i]) SetScroll(i);
                else if (PlayData.Data.NormalHiSpeed[i]) SetNHSScroll(i);
                else NewNotes.Scroll[i] = PlayData.Data.ScrollSpeed[i];
                STimer[i] = new Counter(0, 249, 1000, false);
            }

        }

        public static void Draw()
        {
            Tx.Game_Sudden.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
            if (UseSudden[0])
            {
                Tx.Game_Sudden.Draw(NewNotes.NotesP[0].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - SuddenNumber[0]) / 1000), NewNotes.NotesP[0].Y);
            }
            if (UseSudden[1] && NewGame.Play2P)
            {
                Tx.Game_Sudden.Draw(NewNotes.NotesP[1].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - SuddenNumber[1]) / 1000), NewNotes.NotesP[1].Y);
            }

            if (Key.IsPushing(PlayData.Data.DisplaySudden))
            {
                int type = 0;
                if (PlayData.Data.FloatingHiSpeed[0]) type = 1;
                else if (PlayData.Data.NormalHiSpeed[0]) type = 2;
                Tx.Game_HiSpeed.Draw(NewNotes.NotesP[0].X - 24, NewNotes.NotesP[0].Y - 2, new Rectangle(0, 56 * type, 195, 56));
                NewGame.GameNumber.Draw(PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0] ? NewNotes.NotesP[0].X + 16 : NewNotes.NotesP[0].X - 4, NewNotes.NotesP[0].Y + 18, $"{NewNotes.Scroll[0],6:F2}", 0);
                if (PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0]) NewGame.GameNumber.Draw(NewNotes.NotesP[0].X - 16, NewNotes.NotesP[0].Y + 18, $"{NHSNumber[0] + 1,2}", 0);
                NewGame.GameNumber.Draw(SuddenNumber[0] < 54 ? 1842 : NewNotes.NotesP[0].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - SuddenNumber[0]) / 1000), 348, $"{SuddenNumber[0]}", 0);
                int g = PlayData.Data.FloatingHiSpeed[0] && NewGame.NowState == EState.None ? PreGreen[0] : GreenNumber[0];
                NewGame.GameNumber.Draw(SuddenNumber[0] < 54 ? 1842 : NewNotes.NotesP[0].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - SuddenNumber[0]) / 1000), 388, $"{g}", 5);
            }
            if (Key.IsPushing(PlayData.Data.DisplaySudden2P) && NewGame.Play2P)
            {
                int type = 0;
                if (PlayData.Data.FloatingHiSpeed[1]) type = 1;
                else if (PlayData.Data.NormalHiSpeed[1]) type = 2;
                Tx.Game_HiSpeed.Draw(NewNotes.NotesP[1].X - 24, NewNotes.NotesP[1].Y - 2, new Rectangle(0, 56 * type, 195, 56));
                NewGame.GameNumber.Draw(PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1] ? NewNotes.NotesP[1].X + 16 : NewNotes.NotesP[1].X - 4, NewNotes.NotesP[1].Y + 18, $"{NewNotes.Scroll[1],6:F2}", 0);
                if (PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1]) NewGame.GameNumber.Draw(NewNotes.NotesP[1].X - 16, NewNotes.NotesP[1].Y + 18, $"{NHSNumber[1] + 1,2}", 0);
                NewGame.GameNumber.Draw(SuddenNumber[1] < 54 ? 1842 : NewNotes.NotesP[1].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - SuddenNumber[1]) / 1000), 616, $"{SuddenNumber[1]}", 0);
                double g = PlayData.Data.FloatingHiSpeed[1] && NewGame.NowState == EState.None ? PreGreen[1] : GreenNumber[1];
                NewGame.GameNumber.Draw(SuddenNumber[1] < 54 ? 1842 : NewNotes.NotesP[1].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - SuddenNumber[1]) / 1000), 656, $"{g}", 5);
            }
#if DEBUG

            if (UseSudden[0]) Drawing.Text(1700, 360, $"{SuddenNumber[0]}", 0xffffff);
            Drawing.Text(1700, 400, $"{GreenNumber[0]},{PreGreen[0]}", 0x00ff00);
            if (PlayData.Data.NormalHiSpeed[0]) Drawing.Text(1600, 380, $"{NHSNumber[0] + 1}", 0x0000ff);
            if (PlayData.Data.FloatingHiSpeed[0]) Drawing.Text(1600, 360, "FHS", 0xffffff);
            if (Game.Play2P)
            {
                if (UseSudden[1]) Drawing.Text(1700, 620, $"{SuddenNumber[1]}", 0xffffff);
                Drawing.Text(1700, 660, $"{GreenNumber[1]},{PreGreen[1]}", 0x00ff00);
                if (PlayData.Data.NormalHiSpeed[1]) Drawing.Text(1600, 640, $"{NHSNumber[1] + 1}", 0x0000ff);
                if (PlayData.Data.FloatingHiSpeed[1]) Drawing.Text(1600, 620, "FHS", 0xffffff);
            }
#endif

        }

        public static void Update()
        {
            for (int i = 0; i < 2; i++)
            {
                STimer[i].Tick();
                GreenNumber[i] = GetGreenNumber(i);
                if (NewGame.NowState == EState.None)
                {
                    PlayData.Data.UseSudden[i] = UseSudden[i];
                    PlayData.Data.SuddenNumber[i] = SuddenNumber[i];
                    if (!PlayData.Data.FloatingHiSpeed[i] && !PlayData.Data.NormalHiSpeed[i]) PlayData.Data.ScrollSpeed[i] = NewNotes.Scroll[i];
                    if (PlayData.Data.FloatingHiSpeed[i]) PlayData.Data.GreenNumber[i] = PreGreen[i];
                    PlayData.Data.NHSSpeed[i] = NHSNumber[i];
                    PlayData.Data.InputAdjust[i] = NewGame.Adjust[i];
                }
                    
            }
            if (NewGame.NowState < EState.End && NewGame.Playmode[0] < EAuto.Replay)
            {
                if (Key.IsPushed(PlayData.Data.DisplaySudden))
                {
                    if (STimer[0].State == 0)
                    {
                        STimer[0].Reset();
                        STimer[0].Start();
                    }
                    else
                    {
                        UseSudden[0] = !UseSudden[0];
                        if (PlayData.Data.FloatingHiSpeed[0] && NewGame.NowState == EState.None) PreGreen[0] = GetGreenNumber(0);
                        Add(0);
                        STimer[0].Stop();
                    }
                }
                if (Key.IsPushed(PlayData.Data.DisplaySudden2P) && NewGame.Play2P)
                {
                    if (STimer[1].State == 0)
                    {
                        STimer[1].Reset();
                        STimer[1].Start();
                    }
                    else
                    {

                        UseSudden[1] = !UseSudden[1];
                        if (PlayData.Data.FloatingHiSpeed[1] && NewGame.NowState == EState.None) PreGreen[1] = GetGreenNumber(1);
                        Add(1);
                        STimer[1].Stop();
                    }
                }
                if (Key.IsPushing(PlayData.Data.DisplaySudden)) SuddenUpdate(0);
                if (Key.IsPushing(PlayData.Data.DisplaySudden2P)) SuddenUpdate(1);
            }
        }

        public static void SuddenUpdate(int player)
        {
            if (Key.IsPushed(PlayData.Data.LEFTKA) || Key.IsPushed(PlayData.Data.RIGHTKA))
            {
                if (PlayData.Data.NormalHiSpeed[player] && !PlayData.Data.FloatingHiSpeed[player])
                {
                    if (NHSNumber[player] < 19) NHSNumber[player]++;
                    SetNHSScroll(player);
                }
                else
                {
                    NewNotes.Scroll[player] += 0.25;
                    NewGame.ScrollRemain[player] += 0.25;
                    if (PlayData.Data.FloatingHiSpeed[player] && NewGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                }
                Add(player);
            }
            if (Key.IsPushed(PlayData.Data.LEFTDON) || Key.IsPushed(PlayData.Data.RIGHTDON))
            {
                if (PlayData.Data.NormalHiSpeed[player] && !PlayData.Data.FloatingHiSpeed[player])
                {
                    if (NHSNumber[player] > 0) NHSNumber[player]--;
                    SetNHSScroll(player);
                }
                else
                {
                    NewNotes.Scroll[player] -= 0.25;
                    NewGame.ScrollRemain[player] -= 0.25;
                    if (PlayData.Data.FloatingHiSpeed[player] && NewGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                }
                Add(player);
            }
            if (Key.IsPushed(EKey.Left))
            {
                NewGame.Adjust[player] += 0.5;
                Add(player);
            }
            if (Key.IsPushed(EKey.Right))
            {
                NewGame.Adjust[player] -= 0.5;
                Add(player);
            }
            if (Key.IsPushing(player == 0 ? PlayData.Data.SuddenPlus : PlayData.Data.SuddenPlus2P) && Key.IsPushing(player == 0 ? PlayData.Data.SuddenMinus : PlayData.Data.SuddenMinus2P) && UseSudden[player] && NewGame.NowState > EState.None)
            {
                SetSudden(player, true, true);
                Add(player);
            }
            else if (Key.IsHolding(player == 0 ? PlayData.Data.SuddenPlus : PlayData.Data.SuddenPlus2P, 200, 10))
            {
                if (UseSudden[player])
                {
                    if (SuddenNumber[player] < 1000)
                    {
                        SetSudden(player, true);
                        Add(player);
                    }
                }
                else if (PlayData.Data.FloatingHiSpeed[player])
                {
                    NewNotes.Scroll[player] += 0.01;
                    NewGame.ScrollRemain[player] += 0.01;
                    NewNotes.Scroll[player] = Math.Round(NewNotes.Scroll[player], 2, MidpointRounding.AwayFromZero);
                    if (NewGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                    Add(player);
                }
                
            }
            else if (Key.IsHolding(player == 0 ? PlayData.Data.SuddenMinus : PlayData.Data.SuddenMinus2P, 200, 10))
            {
                if (UseSudden[player])
                {
                    if (SuddenNumber[player] > 0)
                    {
                        SetSudden(player, false);
                        Add(player);
                    }
                }
                else if (PlayData.Data.FloatingHiSpeed[player])
                {
                    NewNotes.Scroll[player] -= 0.01;
                    NewGame.ScrollRemain[player] -= 0.01;
                    NewNotes.Scroll[player] = Math.Round(NewNotes.Scroll[player], 2, MidpointRounding.AwayFromZero);
                    if (NewGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                    Add(player);
                }

            }
            if (Key.IsPushed(player == 0 ? PlayData.Data.ChangeFHS : PlayData.Data.ChangeFHS2P))
            {
                if (!PlayData.Data.FloatingHiSpeed[player] && NewGame.NowState == EState.None) PreGreen[player] = GetGreenNumber(player);
                PlayData.Data.FloatingHiSpeed[player] = !PlayData.Data.FloatingHiSpeed[player];
            }
        }

        public static void Add(int player)
        {
            if (NewGame.NowState == EState.Start || NewGame.NowState == EState.Play)
            Memory.AddSetting(player, NewNotes.Scroll[player], SuddenNumber[player], UseSudden[player], NewGame.Adjust[player]);
        }

        public static int GetGreenNumber(int player, double plusminus = 0)
        {
            Chip chip = Process.NowChip(NewGame.Chips[player]);
            if (chip == null) chip = Process.NextChip(NewGame.Chips[player]);
            double bpm = chip != null ? chip.Bpm : SongData.NowTJA[player].Header.BPM;
            double scroll = chip != null ? chip.Scroll * (NewNotes.Scroll[player] + plusminus) : (NewNotes.Scroll[player] + plusminus);
            int ms = scroll > 0 ? Showms[0] : Showms[1];
            int sudden = UseSudden[player] ? SuddenNumber[player] : 0;
            double suddenrate = 1000.0 / (1000 - sudden);
            return (int)(ms / (bpm * scroll * 2.0 * suddenrate));
        }

        public static void SetNHSScroll(int player)
        {
            Chip chip = Process.NowChip(NewGame.Chips[player]);
            if (chip == null) chip = Process.NextChip(NewGame.Chips[player]);
            double bpm = chip != null ? chip.Bpm : SongData.NowTJA[player].Header.BPM;
            double scroll = chip != null ? chip.Scroll : 1.0;
            double prescroll = NewNotes.Scroll[player];
            NewNotes.Scroll[player] = Showms[0] / (NHSTargetGNum[NHSNumber[player]] * bpm * scroll * 2.0);
            NewGame.ScrollRemain[player] += NewNotes.Scroll[player] - prescroll;
        }

        public static void SetScroll(int player)
        {
            Chip chip = Process.NowChip(NewGame.Chips[player]);
            if (chip == null) chip = Process.NextChip(NewGame.Chips[player]);
            double bpm = chip != null ? chip.Bpm : NewGame.TJA[player].Header.BPM;
            double scroll = chip != null ? chip.Scroll : 1.0;
            int sudden = UseSudden[player] ? SuddenNumber[player] : 0;
            double suddenrate = 1000.0 / (1000 - sudden);
            double prescroll = NewNotes.Scroll[player];
            NewNotes.Scroll[player] = Showms[0] / (PreGreen[player] * bpm * scroll * 2.0 * suddenrate);
            NewGame.ScrollRemain[player] += NewNotes.Scroll[player] - prescroll;
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
