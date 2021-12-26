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
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            TextureLoad.Game_Notes.Draw(NotesP.X, NotesP.Y, new Rectangle(0, 0, 195, 195));

            for (int i = 0; i < Game.MainTJA.Courses[Game.Course].ListChip.Count; i++)
            {
                var chip = Game.MainTJA.Courses[Game.Course].ListChip[i];
                double time = chip.Time - Game.MainTimer.Value;
                float x = (float)NotesX(chip.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);
                if (chip.EChip == EChip.Measure && chip.IsShow && x <= 1500 && x >= -400)
                {
                    TextureLoad.Game_Bar.Draw(NotesP.X + x, NotesP.Y, new Rectangle(0, 0, 195, 195));
                }

                //オートの処理呼び出し
                ProcessAuto.Update(Game.IsAuto, chip, Game.MainTimer.Value);
                //ノーツが通り過ぎた時の処理
                ProcessNote.PassNote(chip, time, chip.ENote == ENote.Ka || chip.ENote == ENote.KA ? false : true);
            }

            for (int i = Game.MainTJA.Courses[Game.Course].ListChip.Count - 1; i >= 0; i--)
            {
                var chip = Game.MainTJA.Courses[Game.Course].ListChip[i];
                float x = (float)NotesX(chip.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);

                if (chip.EChip == EChip.Note && x <= 1500 && !chip.IsHit)
                {
                    switch (chip.ENote)
                    {
                        case ENote.Don:
                        case ENote.Ka:
                        case ENote.DON:
                        case ENote.KA:
                            if (x >= -400)
                                TextureLoad.Game_Notes.Draw(NotesP.X + x, NotesP.Y, new Rectangle((int)chip.ENote * 195, 0, 195, 195));
                            break;
                        case ENote.RollStart:
                        case ENote.ROLLStart:
                            var rollx = NotesX(chip.RollEnd.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);
                            if (rollx >= -400)
                            {
                                TextureLoad.Game_Notes.ScaleX = (float)(rollx - x);
                                TextureLoad.Game_Notes.Draw(NotesP.X + x - 3 + 112, NotesP.Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 6 : 9) + 10, 0, 1, 195)); //連打の中身
                                TextureLoad.Game_Notes.ScaleX = 1f;
                                TextureLoad.Game_Notes.Draw(NotesP.X + rollx - 4.5f + 112, NotesP.Y + 1, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 7 : 10), 0, 195, 195)); //連打の末端の顔
                                TextureLoad.Game_Notes.Draw(NotesP.X + x, NotesP.Y, new Rectangle(195 * (chip.ENote == ENote.RollStart ? 5 : 8), 0, 195, 195)); //連打の先頭の顔
                            }
                            break;
                        case ENote.Balloon:
                        case ENote.Kusudama:
                            var ballx = NotesX(chip.RollEnd.Time, Game.MainTimer.Value, chip.Bpm, chip.Scroll);
                            if (ballx >= -400)
                            {
                                if (x > 0)
                                    TextureLoad.Game_Notes.Draw(NotesP.X + x, NotesP.Y, new Rectangle(195 * 11, 0, 390, 195));
                                else if (ballx > 0)
                                    TextureLoad.Game_Notes.Draw(NotesP.X, NotesP.Y, new Rectangle(195 * 11, 0, 390, 195));
                                else
                                    TextureLoad.Game_Notes.Draw(NotesP.X + ballx, NotesP.Y, new Rectangle(195 * 11, 0, 390, 195));
                            }
                            break;
                    }
                }
            }
            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }

        public static double NotesX(double chiptime, double timer, double bpm, double scroll)
        {
            var time = chiptime - timer;
            return time * bpm * scroll * 2.0 / 335.2396;
        }

        public static Point NotesP = new Point(520, 288);
    }
}
