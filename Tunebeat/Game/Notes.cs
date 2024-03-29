﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static DxLibDLL.DX;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class NewNotes
    {
        #region Function
        public static double[] Scroll = new double[5];
        public static EScroll[] ScrollType = new EScroll[2];
        public static Point[] NotesP = new Point[5];
        #endregion
        public static void Init()
        {
            for (int i = 0; i < 2; i++)
            {
                ScrollType[i] = SongData.NowTJA[i].Courses[Game.Course[i]].ScrollType > EScroll.Normal ? SongData.NowTJA[i].Courses[Game.Course[i]].ScrollType : (EScroll)PlayData.Data.ScrollType[i];
                //if (ScrollType[i] > EScroll.Normal) HBSCROLL.Init(i);
            }
            SetNotesP();
        }

        public static void Draw()
        {
            if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (SongData.NowTJA[i].Courses[NewGame.Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && NewGame.Course[i] == NewGame.Course[i - 1]) break;
                        DrawNotes(i);

                        if ((PlayData.Data.PlayMovie && NewGame.TJA[0].Movie != null && NewGame.TJA[0].Movie.IsEnable) || (PlayData.Data.ShowImage && NewGame.TJA[0].Image != null && NewGame.TJA[0].Image.IsEnable))
                        {
                            Tx.Game_Base.Opacity = 0.75;
                            Tx.Game_Lane_Frame.Opacity = 0.75;
                            Tx.Game_Gauge_Base.Opacity = 0.75;
                        }
                        else
                        {
                            Tx.Game_Base.Opacity = 1.0;
                            Tx.Game_Lane_Frame.Opacity = 1.0;
                            Tx.Game_Gauge_Base.Opacity = 1.0;
                        }
                        Tx.Game_Base.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
                        Tx.Game_Base.Draw(0, NotesP[i].Y - 4);
                        Tx.Game_Base_Info.Draw(0, NotesP[i].Y - 4);
                        Tx.Game_Lane_Frame.Draw(495, NotesP[i].Y - 4);
                    }
                }
            }
            else
            {
                if (ScrollType[0] == EScroll.Normal)
                {
                    DrawNotes(0);
                }
                else
                {
                    DrawNotes(0);
                    //DrawNotesHBS(0);
                }
                if (NewGame.Play2P)
                {
                    if (ScrollType[1] == EScroll.Normal)
                    {
                        DrawNotes(1);
                    }
                    else
                    {
                        DrawNotes(1);
                        //DrawNotesHBS(1);
                    }
                }

                if ((PlayData.Data.PlayMovie && NewGame.TJA[0].Movie != null && NewGame.TJA[0].Movie.IsEnable) || (PlayData.Data.ShowImage && NewGame.TJA[0].Image != null && NewGame.TJA[0].Image.IsEnable))
                {
                    Tx.Game_Base_DP.Opacity = 0.75;
                    Tx.Game_Base.Opacity = 0.75;
                    Tx.Game_Lane.Opacity = 0.75;
                    Tx.Game_Lane_Gogo.Opacity = 0.25;
                    Tx.Game_Lane_Frame_DP.Opacity = 0.75;
                    Tx.Game_Lane_Frame.Opacity = 0.75;
                    Tx.Game_Gauge_Base.Opacity = 0.75;
                }
                else
                {
                    Tx.Game_Base_DP.Opacity = 1.0;
                    Tx.Game_Base.Opacity = 1.0;
                    Tx.Game_Lane.Opacity = 1.0;
                    Tx.Game_Lane_Gogo.Opacity = 0.5;
                    Tx.Game_Lane_Frame_DP.Opacity = 1.0;
                    Tx.Game_Lane_Frame.Opacity = 1.0;
                    Tx.Game_Gauge_Base.Opacity = 1.0;
                }

                if (NewGame.Play2P)
                {
                    Tx.Game_Base_DP.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
                    Tx.Game_Base_DP.Draw(0, NotesP[0].Y - 4);
                    Tx.Game_Base_Info_DP.Draw(0, NotesP[0].Y - 4);
                    Tx.Game_Lane_Frame_DP.Draw(495, NotesP[0].Y - 4);
                }
                else
                {
                    Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
                    Point Point = NewCreate.Enable ? createpoint[0] : NotesP[0];
                    Tx.Game_Base.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
                    Tx.Game_Base.Draw(0, Point.Y - 4);
                    Tx.Game_Base_Info.Draw(0, Point.Y - 4);
                    Tx.Game_Lane_Frame.Draw(495, Point.Y - 4);
                }
            }
        }
        public static void DrawNotes(int player)
        {
            Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
            Point Point = NewCreate.Enable ? createpoint[player] :NotesP[player];
            Tx.Game_Lane.Draw(499, Point.Y);
            Chip nowchip = Process.NowChip(NewGame.Chips[player]);
            if (nowchip != null && nowchip.IsGogo && NewGame.NowState < EState.End) Tx.Game_Lane_Gogo.Draw(499, Point.Y);
            if (player == 0) NewCreate.DrawLaneGrid();

            Tx.Game_Notes.Draw(Point.X, Point.Y, 1.0, 1.0, new Rectangle(0, 0, 195, 195));

            if (NewGame.Bars[player] != null)
            {
                for (int i = 0; i < NewGame.Bars[player].Count; i++)
                {
                    Bar bar = NewGame.Bars[player][i];
                    float x = (float)NoteX(bar, player);
                    if (x <= 1500 && x >= -715)
                    {
                        if (bar.IsShow) Drawing.Box(Point.X + 96 + x, Point.Y, 3, 195);
                        if (NewCreate.Enable) Drawing.Text((int)(Point.X + 96 + x), Point.Y + 200, $"{bar.Number}", 0);
                    }
                }
            }
            if (NewCreate.Enable)
            {
                for (int i = NewGame.Bars[player].Count - 1; i >= 0; i--)
                {
                    DrawNotes(NewGame.Bars[player][i].Chip, player);
                    //NewCreate.DrawGhost(NewGame.Bars[player][i].Chip);
                }
            }
            else
            {
                DrawNotes(NewGame.Chips[player], player);
            }
        }
        public static void DrawNotes(List<Chip> listchip, int player)
        {
            Point[] createpoint = new Point[2] { new Point(521, 4), new Point(521, 1080 - 199) };
            Point Point = NewCreate.Enable ? createpoint[player] : NotesP[player];
            if (listchip != null)
            {
                for (int i = listchip.Count - 1; i >= 0; i--)
                {
                    Chip chip = listchip[i];
                    float x = (float)NoteX(chip, player);
                    if (!chip.IsHit && (chip.Sudden[0] == 0.0 || NewGame.Timer.Value > chip.Time - chip.Sudden[0]))
                    {
                        double X = Point.X + x;
                        bool minus = chip.Bpm * chip.Scroll * Scroll[player] < 0;
                        switch (chip.ENote)
                        {
                            case ENote.Don:
                            case ENote.Ka:
                            case ENote.DON:
                            case ENote.KA:
                                if (X >= -195 && X <= 2175)
                                    Tx.Game_Notes.Draw(X, Point.Y, 1.0, 1.0, new Rectangle((int)chip.ENote * 195, 0, 195, 195));
                                break;
                            case ENote.RollStart:
                            case ENote.ROLLStart:
                                double rollx = NoteX(chip.RollEnd != null ? chip.RollEnd : chip, player);
                                int xx = rollx + 1 < x ? 2 : 0;
                                double rX = Point.X + rollx - 4.5f + 112 + xx;
                                bool inbox = !(X < -195 && rX < -195 && X > 2175 && rX > 2175);
                                if (inbox)
                                {
                                    Tx.Game_Notes.Draw(X - 3 + 112, Point.Y + 1, rollx - x, 1.0, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 6 : 9) + 10, 0, 1, 195)); //連打の中身
                                    Tx.Game_Notes.Draw(rX, Point.Y + 1, rollx + 1 < x ? -1.0 : 1.0, 1.0, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 7 : 10), 0, 195, 195)); //連打の末端の顔
                                    Tx.Game_Notes.Draw(X, Point.Y, 1.0, 1.0, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 5 : 8), 0, 195, 195)); //連打の先頭の顔
                                }
                                break;
                            case ENote.Balloon:
                            case ENote.Kusudama:
                                double ballx = NoteX(chip.RollEnd != null ? chip.RollEnd : chip, player);
                                int mir = minus ? 193 : 0;
                                double bX = Point.X + ballx;
                                if ((!minus && bX >= -390 && X <= 2370) || (minus && X >= -390 && bX <= 2370))
                                {
                                    if (NewGame.Timer.Value < chip.Time)
                                        Tx.Game_Notes.Draw(X + mir, Point.Y, minus ? -1.0 : 1.0, 1.0, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                    else if (NewGame.Timer.Value < (chip.RollEnd != null ? chip.RollEnd.Time : chip.Time))
                                        Tx.Game_Notes.Draw(Point.X + mir, Point.Y, minus ? -1.0 : 1.0, 1.0, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                    else
                                    {
                                        Tx.Game_Notes.Draw(bX + mir, Point.Y, minus ? -1.0 : 1.0, 1.0, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        public static void SetNotesP()
        {
            switch ((EPreviewType)PlayData.Data.PreviewType)
            {
                case EPreviewType.Up:
                    NotesP = new Point[2] { new Point(521, 52), new Point(521, 552 - 290 + 52) };
                    break;
                case EPreviewType.Down:
                    NotesP = new Point[2] { new Point(521, NewGame.Play2P ? 1080 - 461 - 48 : 1080 - 199), new Point(521, 1080 - 199 - 48) };
                    break;
                case EPreviewType.Normal:
                default:
                    NotesP = new Point[2] { new Point(521, 290), new Point(521, 552) };
                    break;
                case EPreviewType.AllCourses:
                    for (int i = 1; i < 5; i++)
                    {
                        Scroll[i] = PlayData.Data.ScrollSpeed[0];
                    }
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (SongData.NowTJA[i].Courses[NewGame.Course[i]].ListChip.Count > 0)
                        {
                            if (i > 0 && NewGame.Course[i] == NewGame.Course[i - 1]) break;
                            count++;
                        }
                    }
                    switch (count)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            NotesP = new Point[3] { new Point(521, 290), new Point(521, 290 + 199), new Point(521, 290 + 199 * 2) };
                            break;
                        case 4:
                            NotesP = new Point[4] { new Point(521, 85 + 199), new Point(521, 85 + 199 * 2), new Point(521, 85 + 199 * 3), new Point(521, 85 + 199 * 4) };
                            break;
                        case 5:
                            NotesP = new Point[5] { new Point(521, 85), new Point(521, 85 + 199), new Point(521, 85 + 199 * 2), new Point(521, 85 + 199 * 3), new Point(521, 85 + 199 * 4) };
                            break;
                    }
                    break;
            }
        }

        public static double NoteX(Chip chip, int player)
        {

            double time = chip.Time - (NewGame.Timer.Value + NewGame.TimeRemain);
            return time * chip.Bpm * chip.Scroll / 168.895 * (Scroll[player] - NewGame.ScrollRemain[player]);
        }
        public static double NoteX(Bar bar, int player)
        {
            double time = bar.Time - (NewGame.Timer.Value + NewGame.TimeRemain);
            return time * bar.BPM * bar.Scroll / 168.895 * (Scroll[player] - NewGame.ScrollRemain[player]);
        }
    }
    public class Notes : Scene
    {
        public override void Enable()
        {
            for (int i = 0; i < 2; i++)
            {
                UseSudden[i] = PlayData.Data.UseSudden[i];
                Sudden[i] = PlayData.Data.SuddenNumber[i];
                Scroll[i] = PlayData.Data.ScrollSpeed[i];
                PreGreen[i] = PlayData.Data.GreenNumber[i];
                NHSNumber[i] = PlayData.Data.NHSSpeed[i];
                ScrollType[i] = SongData.NowTJA[i].Courses[Game.Course[i]].ScrollType > EScroll.Normal ? SongData.NowTJA[i].Courses[Game.Course[i]].ScrollType : (EScroll)PlayData.Data.ScrollType[i];
                if (PlayData.Data.NormalHiSpeed[i]) SetNHSScroll(i, true);
                if (PlayData.Data.FloatingHiSpeed[i]) SetScroll(i, true);
                //if (ScrollType[i] > EScroll.Normal) HBSCROLL.Init(i);
            }
            SetNotesP();
            for (int i = 0; i < 5; i++)
            {
                ProcessAuto.RollTimer[i] = new Counter((long)0.0, (long)(1000.0 / PlayData.Data.AutoRoll), (long)1000.0, false);
            }
            base.Enable();
        }

        public override void Disable()
        {
            for (int i = 0; i < 5; i++)
            {
                ProcessAuto.RollTimer[i].Reset();
            }
            base.Disable();
        }

        public override void Draw()
        {
            if ((EPreviewType)PlayData.Data.PreviewType == EPreviewType.AllCourses)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (SongData.NowTJA[i].Courses[Game.Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Game.Course[i] == Game.Course[i - 1]) break;
                        DrawNotes(i);

                        if ((Game.MainMovie != null && Game.MainMovie.IsEnable) || (PlayData.Data.ShowImage && File.Exists(Game.MainImage.FileName)))
                        {
                            Tx.Game_Base.Opacity = 0.75;
                            Tx.Game_Lane_Frame.Opacity = 0.75;
                            Tx.Game_Gauge_Base.Opacity = 0.75;
                        }
                        else
                        {
                            Tx.Game_Base.Opacity = 1.0;
                            Tx.Game_Lane_Frame.Opacity = 1.0;
                            Tx.Game_Gauge_Base.Opacity = 1.0;
                        }
                        Tx.Game_Base.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
                        Tx.Game_Base.Draw(0, NotesP[i].Y - 4);
                        Tx.Game_Base_Info.Draw(0, NotesP[i].Y - 4);
                        Tx.Game_Lane_Frame.Draw(495, NotesP[i].Y - 4);
                    }
                }
            }
            else
            {
                if (ScrollType[0] == EScroll.Normal)
                {
                    DrawNotes(0);
                }
                else
                {
                    DrawNotes(0);
                    //DrawNotesHBS(0);
                }
                if (Game.Play2P)
                {
                    if (ScrollType[1] == EScroll.Normal)
                    {
                        DrawNotes(1);
                    }
                    else
                    {
                        DrawNotes(1);
                        //DrawNotesHBS(1);
                    }
                }

                Tx.Game_Sudden.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
                if (UseSudden[0])
                {
                    Tx.Game_Sudden.Draw(NotesP[0].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - Sudden[0]) / 1000), NotesP[0].Y);
                }
                if (UseSudden[1] && Game.Play2P)
                {
                    Tx.Game_Sudden.Draw(NotesP[1].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - Sudden[1]) / 1000), NotesP[1].Y);
                }

                if (Key.IsPushing(EKey.LShift))
                {
                    int type = 0;
                    if (PlayData.Data.FloatingHiSpeed[0]) type = 1;
                    else if (PlayData.Data.NormalHiSpeed[0]) type = 2;
                    Tx.Game_HiSpeed.Draw(NotesP[0].X - 24, NotesP[0].Y - 2, new Rectangle(0, 56 * type, 195, 56));
                    Score.DrawNumber(PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0] ? NotesP[0].X + 16 : NotesP[0].X - 4, NotesP[0].Y + 18, $"{Scroll[0],6:F2}", 0);
                    if (PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0]) Score.DrawNumber(NotesP[0].X - 16, NotesP[0].Y + 18, $"{NHSNumber[0] + 1,2}", 0);
                    Score.DrawNumber(Sudden[0] < 54 ? 1842 : NotesP[0].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - Sudden[0]) / 1000), 348, $"{Sudden[0]}", 0);
                    Score.DrawNumber(Sudden[0] < 54 ? 1842 : NotesP[0].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - Sudden[0]) / 1000), 388, $"{(GreenNumber[0] > 0 ? GreenNumber[0] : 0)}", 5);
                }
                if (Key.IsPushing(EKey.RShift) && Game.Play2P)
                {
                    int type = 0;
                    if (PlayData.Data.FloatingHiSpeed[1]) type = 1;
                    else if (PlayData.Data.NormalHiSpeed[1]) type = 2;
                    Tx.Game_HiSpeed.Draw(NotesP[1].X - 24, NotesP[1].Y - 2, new Rectangle(0, 56 * type, 195, 56));
                    Score.DrawNumber(PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1] ? NotesP[1].X + 16 : NotesP[1].X - 4, NotesP[1].Y + 18, $"{Scroll[1],6:F2}", 0);
                    if (PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1]) Score.DrawNumber(NotesP[1].X - 16, NotesP[1].Y + 18, $"{NHSNumber[1] + 1,2}", 0);
                    Score.DrawNumber(Sudden[1] < 54 ? 1842 : NotesP[1].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - Sudden[1]) / 1000), 616, $"{Sudden[1]}", 0);
                    Score.DrawNumber(Sudden[1] < 54 ? 1842 : NotesP[1].X - 22 + (Tx.Game_Lane.TextureSize.Width * (1000 - Sudden[1]) / 1000), 656, $"{(GreenNumber[1] > 0 ? GreenNumber[1] : 0)}", 5);
                }
                if ((Game.MainMovie != null && Game.MainMovie.IsEnable) || (PlayData.Data.ShowImage && File.Exists(Game.MainImage.FileName)))
                {
                    Tx.Game_Base_DP.Opacity = 0.75;
                    Tx.Game_Base.Opacity = 0.75;
                    Tx.Game_Lane_Frame_DP.Opacity = 0.75;
                    Tx.Game_Lane_Frame.Opacity = 0.75;
                    Tx.Game_Gauge_Base.Opacity = 0.75;
                }
                else
                {
                    Tx.Game_Base_DP.Opacity = 1.0;
                    Tx.Game_Base.Opacity = 1.0;
                    Tx.Game_Lane_Frame_DP.Opacity = 1.0;
                    Tx.Game_Lane_Frame.Opacity = 1.0;
                    Tx.Game_Gauge_Base.Opacity = 1.0;
                }

                if (Game.Play2P)
                {
                    Tx.Game_Base_DP.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
                    Tx.Game_Base_DP.Draw(0, NotesP[0].Y - 4);
                    Tx.Game_Base_Info_DP.Draw(0, NotesP[0].Y - 4);
                    Tx.Game_Lane_Frame_DP.Draw(495, NotesP[0].Y - 4);
                }
                else
                {
                    Tx.Game_Base.Color = Color.FromArgb(PlayData.Data.SkinColor[0], PlayData.Data.SkinColor[1], PlayData.Data.SkinColor[2]);
                    Tx.Game_Base.Draw(0, NotesP[0].Y - 4);
                    Tx.Game_Base_Info.Draw(0, NotesP[0].Y - 4);
                    Tx.Game_Lane_Frame.Draw(495, NotesP[0].Y - 4);
                }
            }

#if DEBUG

            if (UseSudden[0]) Drawing.Text(1700, 360, $"{Sudden[0]}", 0xffffff);
            Drawing.Text(1700, 400, $"{GreenNumber[0]}", 0x00ff00);
            if (PlayData.Data.NormalHiSpeed[0]) Drawing.Text(1600, 380, $"{NHSNumber[0] + 1}", 0x0000ff);
            if (PlayData.Data.FloatingHiSpeed[0]) Drawing.Text(1600, 360, "FHS", 0xffffff);
            if (Game.Play2P)
            {
                if (UseSudden[1]) Drawing.Text(1700, 620, $"{Sudden[1]}", 0xffffff);
                Drawing.Text(1700, 660, $"{GreenNumber[1]}", 0x00ff00);
                if (PlayData.Data.NormalHiSpeed[1]) Drawing.Text(1600, 640, $"{NHSNumber[1] + 1}", 0x0000ff);
                if (PlayData.Data.FloatingHiSpeed[1]) Drawing.Text(1600, 620, "FHS", 0xffffff);
            }
#endif

            base.Draw();
        }

        public override void Update()
        {
            for (int i = 0; i < 2; i++)
            {
                GreenNumber[i] = GetGreenNumber(i);
                Chip chip = GetNotes.GetNowNote(SongData.NowTJA[i].Courses[Game.Course[i]].ListChip, Game.MainTimer.Value - Game.Adjust[i]);
                if (chip != null && chip.RollCount == 0 && chip.ENote >= ENote.RollStart && chip.ENote != ENote.RollEnd)
                {
                    switch (chip.ENote)
                    {
                        case ENote.RollStart:
                        case ENote.ROLLStart:
                            ProcessNote.BalloonRemain[i] = 0;
                            break;
                        case ENote.Balloon:
                        case ENote.Kusudama:
                            ProcessNote.BalloonRemain[i] = chip.Balloon;
                            break;
                    }
                }
                if (PlayData.Data.NormalHiSpeed[i] && !PlayData.Data.FloatingHiSpeed[i]) SetNHSScroll(i, Game.MainTimer.State == 0);
            }

            if (Game.MainTimer.State == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Scroll[i] = PlayData.Data.ScrollSpeed[i];
                    UseSudden[i] = PlayData.Data.UseSudden[i];
                    Sudden[i] = PlayData.Data.SuddenNumber[i];
                    NHSNumber[i] = PlayData.Data.NHSSpeed[i];
                    if (PlayData.Data.FloatingHiSpeed[i]) PlayData.Data.GreenNumber[i] = PreGreen[i];
                    else PlayData.Data.GreenNumber[i] = GreenNumber[i];
                }
            }

            base.Update();
        }

        public static void DrawNotes(int player)
        {
            #region Lane
            if ((Game.MainMovie != null && Game.MainMovie.IsEnable) || (PlayData.Data.ShowImage && File.Exists(Game.MainImage.FileName)))
            {
                Tx.Game_Lane.Opacity = 0.5;
            }
            else
            {
                Tx.Game_Lane.Opacity = 1.0;
            }
            Tx.Game_Lane.Draw(NotesP[player].X - 22, NotesP[player].Y);
            Chip nchip = GetNotes.GetNowNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value, true);
            if (Create.CreateMode) nchip = GetNotes.GetNowNote(Create.ListAllChip, Game.MainTimer.Value, true);
            if (nchip != null && nchip.IsGogo)
            {
                if ((Game.MainMovie != null && Game.MainMovie.IsEnable) || (PlayData.Data.ShowImage && File.Exists(Game.MainImage.FileName)))
                {
                    Tx.Game_Lane_Gogo.Opacity = 0.25;
                }
                else
                {
                    Tx.Game_Lane_Gogo.Opacity = 0.5;
                }
                Tx.Game_Lane_Gogo.BlendMode = BlendMode.Add;
                Tx.Game_Lane_Gogo.Draw(NotesP[player].X - 22, NotesP[player].Y);
            }
            #endregion

            if (Create.CreateMode)
            {
                #region Create
                if (Game.MainTimer.State == 0)
                {
                    float width = (1920 - (NotesP[0].X - 22)) / (float)Create.InputType;
                    float x;
                    int[] color = new int[2];
                    switch (Create.InputType)
                    {
                        case 4:
                            color = new int[2] { 0xff0000, 0x0000ff };
                            break;
                        case 8:
                            color = new int[2] { 0xff0000, 0x0000ff };
                            break;
                        case 12:
                            color = new int[3] { 0xff0000, 0x00ff00, 0x0000ff };
                            break;
                        case 16:
                            color = new int[4] { 0xff0000, 0xffff00, 0x0000ff, 0xffff00 };
                            break;
                        case 20:
                            color = new int[5] { 0xff0000, 0xffff00, 0x00ff00, 0x00ffff, 0x0000ff };
                            break;
                        case 24:
                            color = new int[6] { 0xff0000, 0x00ffff, 0x00ff00, 0x0000ff, 0x00ff00, 0x00ffff };
                            break;
                        case 32:
                            color = new int[8] { 0xff0000, 0xff8000, 0xffff00, 0xff8000, 0x0000ff, 0xff8000, 0xffff00, 0xff8000 };
                            break;
                        case 48:
                            color = new int[12] { 0xff0000, 0xff00ff, 0x00ffff, 0xffff00, 0x00ff00, 0xff00ff, 0x0000ff, 0xff00ff, 0x00ff00, 0xffff00, 0x00ffff, 0xff00ff };
                            break;
                    }
                    SetDrawBlendMode(DX_BLENDMODE_ALPHA, 64);
                    for (int i = 0; i < Create.InputType; i++)
                    {
                        x = NotesP[0].X - 22 + i * width;
                        Drawing.Box(x, NotesP[0].Y, x + width, NotesP[0].Y + 195, color[i % (Create.InputType == 4 ? 2 : Create.InputType / 4)]);
                        if (Mouse.X >= x && Mouse.X < x + width && Mouse.Y >= NotesP[0].Y && Mouse.Y < NotesP[0].Y + 195)
                        {
                            Drawing.Box(x, NotesP[0].Y, x + width, NotesP[0].Y + 195);
                        }
                    }
                    SetDrawBlendMode(DX_BLENDMODE_ALPHA, 255);
                }

                int[] wid = new int[8] { 0, 29, 60, 75, 84, 90, 97, 104 };
                if (Create.Preview && Game.MainTimer.State == 0)
                {
                    Tx.Game_Notes.Draw(NotesP[player].X - wid[Create.NowInput], NotesP[player].Y, new Rectangle(0, 0, 195, 195));
                }
                else Tx.Game_Notes.Draw(NotesP[player].X, NotesP[player].Y, new Rectangle(0, 0, 195, 195));

                List<BarLine> Bar = Create.File.Bar[Game.Course[player]];
                for (int i = Bar.Count - 1; i >= 0; i--)
                {
                    if (Bar[i].IsShow && Game.MainTimer.Value > Bar[i].Time - 30000)
                    {
                        float x = (float)NotesX(Bar[i].Time, Game.MainTimer.Value + Game.TimeRemain, Bar[i].BPM, Bar[i].Scroll, player);
                        if (Create.Preview && Game.MainTimer.State == 0)
                        {
                            x = (float)NotesX(Bar[i].Time, Game.MainTimer.Value + Game.TimeRemain, Bar[i].BPM, Bar[i].Scroll, player) - wid[Create.NowInput];
                        }
                        if (x <= 1500 && x >= -715) Tx.Game_Bar.Draw(NotesP[player].X + 96 + x, NotesP[player].Y);
                    }
                    for (int j = 0; j < Bar[i].Measure * 4; j++)
                    {
                        #region function
                        double time = Bar[i].Time;
                        double bpm = Bar[i].BPM;
                        double scroll = Bar[i].Scroll;
                        bool[] doubt = new bool[1000];
                        for (int c = Bar[i].Chip.Count - 1; c >= 0; c--)
                        {
                            Chip chip = Bar[i].Chip[c];
                            if (time == chip.Time)
                            {
                                bpm = chip.Bpm;
                                scroll = chip.Scroll;
                                break;
                            }
                        }
                        for (int c = Bar[i].Chip.Count - 1; c >= 0; c--)
                        {
                            Chip chip = Bar[i].Chip[c];
                            if (chip.Time == Bar[i].Time + j * (60000.0 / Bar[i].BPM) && chip.ENote != ENote.Space && !chip.IsHit)
                            {
                                doubt[j] = true;
                            }
                        }
                        float x = (float)NotesX(time, Game.MainTimer.Value + Game.TimeRemain, bpm, scroll, player);
                        if (Create.Preview && Game.MainTimer.State == 0)
                        {
                            x = (float)NotesX(time, Game.MainTimer.Value + Game.TimeRemain, bpm, scroll, player) - wid[Create.NowInput];
                        }
                        double num = j * (1421.0 / 4) * Bar[i].Scroll;
                        double width = (1920 - (NotesP[0].X - 22)) / (double)Create.InputType;
                        double xx;
                        int select = -1;
                        for (int v = 0; v < Create.InputType; v++)
                        {
                            xx = NotesP[0].X - 22 + v * width;
                            if (Mouse.X >= xx && Mouse.X < xx + width && Mouse.Y >= NotesP[0].Y && Mouse.Y < NotesP[0].Y + 195 && Game.MainTimer.State == 0)
                            {
                                select = v;
                            }
                        }
                        #endregion
                        if (x <= 1500 && select != j && !doubt[j] && Game.MainTimer.Value > Bar[i].Time - 30000)
                        {
                            Tx.Game_Notes.Opacity = 0.5;
                            Tx.Game_Notes.Draw(NotesP[player].X + x + num, NotesP[player].Y, new Rectangle(195 * 15, 0, 195, 195));
                            Tx.Game_Notes.Opacity = 1.0;
                        }
                    }
                    for (int j = Bar[i].Chip.Count - 1; j >= 0; j--)
                    {
                        Chip chip = Bar[i].Chip[j];
                        float x = (float)NotesX(chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.Scroll, player);
                        if (Create.Preview && Game.MainTimer.State == 0)
                        {
                            x = (float)NotesX(chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.Scroll, player) - wid[Create.NowInput];
                        }
                        if (x <= 1500)
                        {
                            if (chip.ENote != ENote.Space && !chip.IsHit && (chip.Sudden[0] == 0.0 || Game.MainTimer.Value + Game.TimeRemain > chip.Time - chip.Sudden[0]))
                            {
                                switch (chip.ENote)
                                {
                                    case ENote.Don:
                                    case ENote.Ka:
                                    case ENote.DON:
                                    case ENote.KA:
                                        if (x >= -715)
                                            Tx.Game_Notes.Draw(NotesP[player].X + x, NotesP[player].Y, new Rectangle((int)chip.ENote * 195, 0, 195, 195));
                                        break;
                                    case ENote.RollStart:
                                    case ENote.ROLLStart:
                                        double rollx = NotesX(chip.RollEnd != null ? chip.RollEnd.Time : chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.RollEnd != null ? chip.RollEnd.Scroll : chip.Scroll, player);
                                        if (rollx >= -715)
                                        {
                                            Tx.Game_Notes.ScaleX = (float)(rollx - x);
                                            Tx.Game_Notes.Draw(NotesP[player].X + x - 3 + 112, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 6 : 9) + 10, 0, 1, 195)); //連打の中身
                                            Tx.Game_Notes.ScaleX = 1f;
                                            Tx.Game_Notes.ScaleX = rollx + 1 < x ? -1f : 1f;
                                            int xx = rollx + 1 < x ? 2 : 0;
                                            Tx.Game_Notes.Draw(NotesP[player].X + rollx - 4.5f + 112 + xx, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 7 : 10), 0, 195, 195)); //連打の末端の顔
                                            Tx.Game_Notes.Draw(NotesP[player].X + x, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 5 : 8), 0, 195, 195)); //連打の先頭の顔
                                        }
                                        break;
                                    case ENote.Balloon:
                                    case ENote.Kusudama:
                                        double ballx = NotesX(chip.RollEnd != null ? chip.RollEnd.Time : chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.Scroll, player);
                                        if (ballx >= -715)
                                        {
                                            if (Create.Preview)
                                            {
                                                Tx.Game_Notes.ScaleX = (float)(ballx - x);
                                                Tx.Game_Notes.Opacity = 0.5;
                                                Tx.Game_Notes.Draw(NotesP[player].X + x - 3 + 112, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 6 : 9) + 10, 0, 1, 195)); //連打の中身
                                                Tx.Game_Notes.ScaleX = ballx + 1 < x ? -1f : 1f;
                                                int xx = ballx + 1 < x ? 2 : 0;
                                                Tx.Game_Notes.Draw(NotesP[player].X + ballx - 4.5f + 112 + xx, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 7 : 10), 0, 195, 195)); //連打の末端の顔
                                                Tx.Game_Notes.ScaleX = 1f;
                                                Tx.Game_Notes.Opacity = 1.0;
                                            }
                                            int mir = 0;
                                            if (chip.Scroll * chip.Bpm < 0)
                                            {
                                                Tx.Game_Notes.ScaleX = -1f;
                                                mir = 195;
                                            }
                                            if (x > 0)
                                                Tx.Game_Notes.Draw(NotesP[player].X + x + mir, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                            else if (ballx > 0)
                                                Tx.Game_Notes.Draw(NotesP[player].X + mir, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                            else
                                            {
                                                Tx.Game_Notes.Draw(NotesP[player].X + ballx + mir, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                            }
                                            Tx.Game_Notes.ScaleX = 1f;
                                        }
                                        break;
                                }
                            }
                        }
                        double time = Bar[i].Chip[j].Time - Game.MainTimer.Value;
                        if (!Create.Mapping)
                        {
                            //オートの処理呼び出し
                            ProcessAuto.Update(Game.IsAuto[player], Bar[i].Chip[j], Game.MainTimer.Value, player);
                            //ノーツが通り過ぎた時の処理
                            ProcessNote.PassNote(Bar[i].Chip[j], time, Bar[i].Chip[j].ENote == ENote.Ka || Bar[i].Chip[j].ENote == ENote.KA ? false : true, player);
                            
                        }
                    }
                    if (!Create.Mapping)
                    {
                        //連打のタイマー　なんでここ？？
                        Chip nowchip = GetNotes.GetNowNote(Create.ListAllChip, Game.MainTimer.Value);
                        ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
                        for (int r = 0; r < 5; r++)
                        {
                            ProcessAuto.RollTimer[r].Tick();
                        }
                        if (roll != ERoll.None)
                        {
                            ProcessAuto.RollTimer[player].Start();
                        }
                        else
                        {
                            ProcessAuto.RollTimer[player].Reset();
                        }
                    }
                }
                if (Mouse.X >= NotesP[0].X - 22 && Mouse.Y >= NotesP[0].Y && Mouse.Y < NotesP[0].Y + 195 && Game.MainTimer.State == 0)
                {
                    int cur = (int)Math.Floor((Mouse.X - (NotesP[0].X - 22)) / (1421f / Create.InputType));
                    Rectangle[] rec = new Rectangle[8] { new Rectangle(195 * 1, 0, 195 * 1, 195), new Rectangle(195 * 2, 0, 195 * 1, 195), new Rectangle(195 * 3, 0, 195 * 1, 195),
                    new Rectangle(195 * 4, 0, 195 * 1, 195), new Rectangle(195 * 5, 0, 195 * 1, 195), new Rectangle(195 * 8, 0, 195 * 1, 195),
                    new Rectangle(195 * 11, 0, 195 * 2, 195), new Rectangle(195 * 13, 0, 195 * 2, 195) };
                    double width = (1920 - (NotesP[0].X - 22)) / (double)Create.InputType;
                    Tx.Game_Notes.Opacity = 0.5;
                    if (Create.RollEnd)
                    {
                        double x = (float)NotesX(Create.RollBegin.Time, Game.MainTimer.Value + Game.TimeRemain, Create.RollBegin.Bpm, Create.RollBegin.Scroll, player);
                        double rollx = cur * width;
                        Tx.Game_Notes.ScaleX = (float)(rollx - x);
                        Tx.Game_Notes.Draw(NotesP[player].X - wid[Create.NowInput] + x - 3 + 112, NotesP[player].Y, new Rectangle(195 * (Create.RollBegin.ENote == ENote.RollStart || Create.RollBegin.ENote == ENote.Balloon ? 6 : 9) + 10, 0, 1, 195)); //連打の中身
                        Tx.Game_Notes.ScaleX = rollx + 1 < x ? -1f : 1f;
                        int xx = rollx + 1 < x ? 2 : 0;
                        Tx.Game_Notes.Draw(NotesP[player].X - wid[Create.NowInput] + rollx - 4.5f + 112 + xx, NotesP[player].Y, new Rectangle(195 * (Create.RollBegin.ENote == ENote.RollStart || Create.RollBegin.ENote == ENote.Balloon ? 7 : 10), 0, 195, 195)); //連打の末端の顔
                        Tx.Game_Notes.ScaleX = 1f;
                    }
                    else Tx.Game_Notes.Draw(NotesP[player].X - wid[Create.NowInput] + cur * width, NotesP[player].Y, rec[Create.NowColor]);
                    Tx.Game_Notes.Opacity = 1.0;
                }
#endregion
            }
            else
            {
                #region Normal
                Tx.Game_Notes.Draw(NotesP[player].X, NotesP[player].Y, new Rectangle(0, 0, 195, 195));

                for (int i = 0; i < SongData.NowTJA[player].Courses[Game.Course[player]].ListChip.Count; i++)
                {
                    Chip chip = SongData.NowTJA[player].Courses[Game.Course[player]].ListChip[i];
                    double time = chip.Time - Game.MainTimer.Value;
                    float x = (float)NotesX(chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.Scroll, player);
                    if (chip.IsShow && x <= 1500 && x >= -715 && Game.MainTimer.Value > chip.Time - 30000)
                    {
                        Tx.Game_Bar.Draw(NotesP[player].X + 96 + x, NotesP[player].Y);
                    }

                    //オートの処理呼び出し
                    if (chip.Time - Game.MainTimer.Value < 100) ProcessAuto.Update(PlayData.Data.PreviewType == 3 ? true : Game.IsAuto[player], chip, Game.MainTimer.Value, player);
                    
                    //ノーツが通り過ぎた時の処理
                    ProcessNote.PassNote(chip, time, chip.ENote == ENote.Ka || chip.ENote == ENote.KA ? false : true, player);
                }
                if (PlayData.Data.PreviewType < 3)
                {
                    ProcessReplay.Update(Game.IsReplay[player], player);
                    ProcessReplay.UnderUpdate();
                }
                //連打のタイマー　なんでここ？？
                Chip nowchip = GetNotes.GetNowNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
                ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
                for (int i = 0; i < 5; i++)
                {
                    ProcessAuto.RollTimer[i].Tick();
                }
                if (roll != ERoll.None)
                {
                    ProcessAuto.RollTimer[player].Start();
                }
                else
                {
                    ProcessAuto.RollTimer[player].Reset();
                }

                for (int i = SongData.NowTJA[player].Courses[Game.Course[player]].ListChip.Count - 1; i >= 0; i--)
                {
                    Chip chip = SongData.NowTJA[player].Courses[Game.Course[player]].ListChip[i];
                    float x = (float)NotesX(chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.Scroll, player);
                    if (x <= 1500 && !chip.IsHit && (chip.Sudden[0] == 0.0 || Game.MainTimer.Value + Game.TimeRemain > chip.Time - chip.Sudden[0]) && (PlayData.Data.PreviewType == 3 || (player < 2 && !PlayData.Data.Stelth[player])))
                    {
                        switch (chip.ENote)
                        {
                            case ENote.Don:
                            case ENote.Ka:
                            case ENote.DON:
                            case ENote.KA:
                                if (x >= -715)
                                    Tx.Game_Notes.Draw(NotesP[player].X + x, NotesP[player].Y, new Rectangle((int)chip.ENote * 195, 0, 195, 195));
                                break;
                            case ENote.RollStart:
                            case ENote.ROLLStart:
                                double rollx = NotesX(chip.RollEnd != null ? chip.RollEnd.Time : chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.RollEnd != null ? chip.RollEnd.Scroll : chip.Scroll, player);
                                if (rollx >= -715)
                                {
                                    Tx.Game_Notes.ScaleX = (float)(rollx - x);
                                    Tx.Game_Notes.Draw(NotesP[player].X + x - 3 + 112, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 6 : 9) + 10, 0, 1, 195)); //連打の中身
                                    Tx.Game_Notes.ScaleX = 1f;
                                    Tx.Game_Notes.ScaleX = rollx + 1 < x ? -1f : 1f;
                                    int xx = rollx + 1 < x ? 2 : 0;
                                    Tx.Game_Notes.Draw(NotesP[player].X + rollx - 4.5f + 112 + xx, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 7 : 10), 0, 195, 195)); //連打の末端の顔
                                    Tx.Game_Notes.Draw(NotesP[player].X + x, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 5 : 8), 0, 195, 195)); //連打の先頭の顔
                                }
                                break;
                            case ENote.Balloon:
                            case ENote.Kusudama:
                                double ballx = NotesX(chip.RollEnd != null ? chip.RollEnd.Time : chip.Time, Game.MainTimer.Value + Game.TimeRemain, chip.Bpm, chip.Scroll, player);
                                if (ballx >= -715)
                                {
                                    int mir = 0;
                                    if (chip.Scroll < 0)
                                    {
                                        Tx.Game_Notes.ScaleX = -1f;
                                        mir = 195;
                                    }
                                    if (x > 0)
                                        Tx.Game_Notes.Draw(NotesP[player].X + x + mir, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                    else if (ballx > 0)
                                        Tx.Game_Notes.Draw(NotesP[player].X + mir, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                    else
                                    {
                                        Tx.Game_Notes.Draw(NotesP[player].X + ballx + mir, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                    }
                                    Tx.Game_Notes.ScaleX = 1f;
                                }
                                break;
                        }
                    }
                }
                #endregion
            }
        }
        public static void DrawNotesHBS(int player)
        {
            #region Lane
            if ((Game.MainMovie != null && Game.MainMovie.IsEnable) || (PlayData.Data.ShowImage && File.Exists(Game.MainImage.FileName)))
            {
                Tx.Game_Lane.Opacity = 0.5;
            }
            else
            {
                Tx.Game_Lane.Opacity = 1.0;
            }
            Tx.Game_Lane.Draw(NotesP[player].X - 22, NotesP[player].Y);
            Chip nchip = GetNotes.GetNowNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value, true);
            if (Create.CreateMode) nchip = GetNotes.GetNowNote(Create.ListAllChip, Game.MainTimer.Value, true);
            if (nchip != null && nchip.IsGogo)
            {
                if ((Game.MainMovie != null && Game.MainMovie.IsEnable) || (PlayData.Data.ShowImage && File.Exists(Game.MainImage.FileName)))
                {
                    Tx.Game_Lane_Gogo.Opacity = 0.25;
                }
                else
                {
                    Tx.Game_Lane_Gogo.Opacity = 0.5;
                }
                Tx.Game_Lane_Gogo.BlendMode = BlendMode.Add;
                Tx.Game_Lane_Gogo.Draw(NotesP[player].X - 22, NotesP[player].Y);
            }
            #endregion

            Tx.Game_Notes.Draw(NotesP[player].X, NotesP[player].Y, new Rectangle(0, 0, 195, 195));

            for (int i = 0; i < SongData.NowTJA[player].Courses[Game.Course[player]].ListChip.Count; i++)
            {
                Chip chip = SongData.NowTJA[player].Courses[Game.Course[player]].ListChip[i];
                double time = chip.Time - Game.MainTimer.Value;
                //オートの処理呼び出し
                if (chip.Time - Game.MainTimer.Value < 100) ProcessAuto.Update(PlayData.Data.PreviewType == 3 ? true : Game.IsAuto[player], chip, Game.MainTimer.Value, player);

                //ノーツが通り過ぎた時の処理
                ProcessNote.PassNote(chip, time, chip.ENote == ENote.Ka || chip.ENote == ENote.KA ? false : true, player);
            }
            if (PlayData.Data.PreviewType < 3)
            {
                ProcessReplay.Update(Game.IsReplay[player], player);
                ProcessReplay.UnderUpdate();
            }
            //連打のタイマー　なんでここ？？
            Chip nowchip = GetNotes.GetNowNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
            for (int i = 0; i < 5; i++)
            {
                ProcessAuto.RollTimer[i].Tick();
            }
            if (roll != ERoll.None)
            {
                ProcessAuto.RollTimer[player].Start();
            }
            else
            {
                ProcessAuto.RollTimer[player].Reset();
            }

            //for (int i = 0; i < HBSCROLL.Chips[player].Length; i++)
            {

            }
        }

        public static double NotesX(double chiptime, double timer, double bpm, double scroll, int player)
        {
            double time = chiptime - timer;
            double x = time * bpm * scroll * 2.0 * (Scroll[player] - Game.ScrollRemain[player]) / 337.79;//1421
            return x;
        }

        public static int GetGreenNumber(int player, double plusminus = 0)
        {
            Chip chip = GetNotes.GetNowNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            if (chip == null) chip = GetNotes.GetNearNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            double bpm = chip != null ? chip.Bpm : SongData.NowTJA[player].Header.BPM;
            double scroll = chip != null ? chip.Scroll * (Scroll[player] + plusminus) : (Scroll[player] + plusminus);
            int ms = scroll > 0 ? Showms[0] : Showms[1];
            int sudden = UseSudden[player] ? Sudden[player] : 0;
            double suddenrate = 1000.0 / (1000 - sudden);
            return (int)(ms / (bpm * scroll * 2.0 * suddenrate));
        }

        public static void SetNHSScroll(int player, bool isLoad = false)
        {
            Chip chip = new Chip();
            for (int i = 0; i < SongData.NowTJA[player].Courses[Game.Course[player]].ListChip.Count; i++)
            {
                if (SongData.NowTJA[player].Courses[Game.Course[player]].ListChip[i].ENote >= ENote.Don)
                {
                    chip = SongData.NowTJA[player].Courses[Game.Course[player]].ListChip[i];
                    break;
                }
            }
            double bpm = chip != null ? chip.Bpm : SongData.NowTJA[player].Header.BPM;
            double scroll = chip != null ? chip.Scroll : 1.0;
            double prescroll = Scroll[player];
            Scroll[player] = Math.Round(Showms[0] / (NHSTargetGNum[NHSNumber[player]] * bpm * scroll * 2), 2, MidpointRounding.AwayFromZero);
            Game.ScrollRemain[player] += Scroll[player] - prescroll;

            if (isLoad) PlayData.Data.ScrollSpeed[player] = Scroll[player];
        }

        public static void SetScroll(int player, bool isLoad = false)
        {
            Chip chip = GetNotes.GetNowNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            if (chip == null) chip = GetNotes.GetNearNote(SongData.NowTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            double bpm = chip != null ? chip.Bpm : SongData.NowTJA[player].Header.BPM;
            double scroll = chip != null ? chip.Scroll : 1.0;
            int sudden = UseSudden[player] ? Sudden[player] : 0;
            double suddenrate = 1000.0 / (1000 - sudden);
            double prescroll = Scroll[player];
            Scroll[player] = Math.Round(Showms[0] / (PreGreen[player] * bpm * scroll * 2.0 * suddenrate), 2, MidpointRounding.AwayFromZero);
            Game.ScrollRemain[player] += Scroll[player] - prescroll;

            if (isLoad) PlayData.Data.ScrollSpeed[player] = Scroll[player];
        }

        public static void SetSudden(int player, bool isPlus, bool isLoad = false, bool Reset = false)
        {
            if (!isLoad)
            {
                if (Reset)
                {
                    Sudden[player] = PlayData.Data.SuddenNumber[player];
                }
                else
                {
                    if (isPlus)
                        Sudden[player] += 2;
                    else Sudden[player] -= 2;
                }
            }
            else
            {
                if (isPlus)
                    PlayData.Data.SuddenNumber[player] += 2;
                else PlayData.Data.SuddenNumber[player] -= 2;
            }
            
            if (PlayData.Data.FloatingHiSpeed[player]) SetScroll(player, isLoad);
        }

        public static void SetNotesP()
        {
            switch ((EPreviewType)PlayData.Data.PreviewType)
            {
                case EPreviewType.Up:
                    NotesP = new Point[2] { new Point(521, 52), new Point(521, 552 - 290 + 52) };
                    break;
                case EPreviewType.Down:
                    NotesP = new Point[2] { new Point(521, Game.Play2P ? 1080 - 461 - 48 : 1080 - 199), new Point(521, 1080 - 199 - 48) };
                    break;
                case EPreviewType.Normal:
                default:
                    NotesP = new Point[2] { new Point(521, 290), new Point(521, 552) };
                    break;
                case EPreviewType.AllCourses:
                    for (int i = 1; i < 5; i++)
                    {
                        Scroll[i] = PlayData.Data.ScrollSpeed[0];
                    }
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (SongData.NowTJA[i].Courses[Game.Course[i]].ListChip.Count > 0)
                        {
                            if (i > 0 && Game.Course[i] == Game.Course[i - 1]) break;
                            count++;
                        }
                    }
                    switch (count)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            NotesP = new Point[3] { new Point(521, 290), new Point(521, 290 + 199), new Point(521, 290 + 199 * 2) };
                            break;
                        case 4:
                            NotesP = new Point[4] { new Point(521, 85 + 199), new Point(521, 85 + 199 * 2), new Point(521, 85 + 199 * 3), new Point(521, 85 + 199 * 4) };
                            break;
                        case 5:
                            NotesP = new Point[5] { new Point(521, 85), new Point(521, 85 + 199), new Point(521, 85 + 199 * 2), new Point(521, 85 + 199 * 3), new Point(521, 85 + 199 * 4) };
                            break;
                    }
                    break;
            }
        }

        public static Point[] NotesP = new Point[5];
        public static int[] Showms = new int[2] { 256300, -37000 };
        public static double[] Scroll = new double[5];
        public static bool[] UseSudden = new bool[2];
        public static int[] Sudden = new int[2], GreenNumber = new int[2], PreGreen = new int[2], NHSNumber = new int[2];
        public static int[] NHSTargetGNum = new int[20] { 1200, 1000, 800, 700, 650, 600, 550, 500, 480, 460,
            440, 420, 400, 380, 360, 340, 320, 300, 280, 260 };
        public static EScroll[] ScrollType = new EScroll[2];
    }

    public enum EPreviewType
    {
        Normal,
        Up,
        Down,
        AllCourses
    }
}
