using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.Game
{
    public class Notes : Scene
    {
        public override void Enable()
        {
            ProcessAuto.RollTimer = new Counter((long)0.0, (long)(1000.0 / PlayData.AutoRoll), (long)1000.0, false);
            ProcessAuto.RollTimer2P = new Counter((long)0.0, (long)(1000.0 / PlayData.AutoRoll), (long)1000.0, false);
            base.Enable();
        }

        public override void Disable()
        {
            ProcessAuto.RollTimer.Reset();
            ProcessAuto.RollTimer2P.Reset();
            base.Disable();
        }

        public override void Draw()
        {
            DrawNotes(0);
            if (PlayData.IsPlay2P)
            {
                DrawNotes(1);
                TextureLoad.Game_Base_DP.Draw(0, NotesP[0].Y - 4);
                TextureLoad.Game_Lane_Frame_DP.Draw(NotesP[0].X - 26, NotesP[0].Y - 4);
            }
            else
            {
                TextureLoad.Game_Base.Draw(0, NotesP[0].Y - 4);
                TextureLoad.Game_Lane_Frame.Draw(NotesP[0].X - 26, NotesP[0].Y - 4);
            }
            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }

        public static void DrawNotes(int player)
        {
            TextureLoad.Game_Lane.Draw(NotesP[player].X - 22, NotesP[player].Y);

            TextureLoad.Game_Notes.Draw(NotesP[player].X, NotesP[player].Y, new Rectangle(0, 0, 195, 195));

            for (int i = 0; i < Game.MainTJA.Courses[Game.Course[player]].ListChip.Count; i++)
            {
                Chip chip = Game.MainTJA.Courses[Game.Course[player]].ListChip[i];
                double time = chip.Time - Game.MainTimer.Value;
                float x = (float)NotesX(chip.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);
                if (chip.EChip == EChip.Measure && chip.IsShow && x <= 1500 && x >= -715)
                {
                    TextureLoad.Game_Bar.Draw(NotesP[player].X + 96 + x, NotesP[player].Y);
                }

                //オートの処理呼び出し
                ProcessAuto.Update(Game.IsAuto[player], chip, Game.MainTimer.Value, player);
                //ノーツが通り過ぎた時の処理
                ProcessNote.PassNote(chip, time, chip.ENote == ENote.Ka || chip.ENote == ENote.KA ? false : true, player);
            }
            //連打のタイマー　なんでここ？？
            Chip nowchip = GetNotes.GetNowNote(Game.MainTJA.Courses[Game.Course[player]].ListChip, Game.MainTimer.Value);
            ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
            ProcessAuto.RollTimer.Tick();
            ProcessAuto.RollTimer2P.Tick();
            if (player == 0)
            {
                if (roll != ERoll.None)
                {
                    ProcessAuto.RollTimer.Start();
                }
                else
                {
                    ProcessAuto.RollTimer.Reset();
                }
            }
            else
            {
                if (roll != ERoll.None)
                {
                    ProcessAuto.RollTimer2P.Start();
                }
                else
                {
                    ProcessAuto.RollTimer2P.Reset();
                }
            }

            for (int i = Game.MainTJA.Courses[Game.Course[player]].ListChip.Count - 1; i >= 0; i--)
            {
                Chip chip = Game.MainTJA.Courses[Game.Course[player]].ListChip[i];
                float x = (float)NotesX(chip.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);

                if (chip.EChip == EChip.Note && x <= 1500 && !chip.IsHit)
                {
                    switch (chip.ENote)
                    {
                        case ENote.Don:
                        case ENote.Ka:
                        case ENote.DON:
                        case ENote.KA:
                            if (x >= -715)
                                TextureLoad.Game_Notes.Draw(NotesP[player].X + x, NotesP[player].Y, new Rectangle((int)chip.ENote * 195, 0, 195, 195));
                            break;
                        case ENote.RollStart:
                        case ENote.ROLLStart:
                            double rollx = NotesX(chip.RollEnd != null ? chip.RollEnd.Time : chip.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);
                            if (rollx >= -715)
                            {
                                TextureLoad.Game_Notes.ScaleX = (float)(rollx - x);
                                TextureLoad.Game_Notes.Draw(NotesP[player].X + x - 3 + 112, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 6 : 9) + 10, 0, 1, 195)); //連打の中身
                                TextureLoad.Game_Notes.ScaleX = 1f;
                                TextureLoad.Game_Notes.Draw(NotesP[player].X + rollx - 4.5f + 112, NotesP[player].Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 7 : 10), 0, 195, 195)); //連打の末端の顔
                                TextureLoad.Game_Notes.Draw(NotesP[player].X + x, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 5 : 8), 0, 195, 195)); //連打の先頭の顔
                            }
                            break;
                        case ENote.Balloon:
                        case ENote.Kusudama:
                            double ballx = NotesX(chip.RollEnd != null ? chip.RollEnd.Time : chip.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);
                            if (ballx >= -715)
                            {
                                if (x > 0)
                                    TextureLoad.Game_Notes.Draw(NotesP[player].X + x, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                else if (ballx > 0)
                                    TextureLoad.Game_Notes.Draw(NotesP[player].X, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                else
                                {
                                    TextureLoad.Game_Notes.Draw(NotesP[player].X + ballx, NotesP[player].Y, new Rectangle(195 * (chip.ENote == ENote.Balloon ? 11 : 13), 0, 390, 195));
                                    ProcessNote.BalloonList[player]++;
                                }
                            }
                            break;
                    }
                }
            }

        }

        public static double NotesX(double chiptime, double timer, double bpm, double scroll)
        {
            double time = chiptime - timer;
            return time * bpm * scroll * 2.0 / 335.2396;
        }

        public static Point[] NotesP = new Point[2] { new Point(521, 290), new Point(521, 552) };
    }
}
