using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunebeat.Common;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;

namespace Tunebeat.Game
{
    public class KeyInput
    {
        public static void Update(bool Auto1P, bool Auto2P, bool Failed1P, bool Failed2P)
        {
            if (Key.IsPushed(KEY_INPUT_F1) && !Game.IsReplay[0])
            {
                Game.IsAuto[0] = !Game.IsAuto[0];
                if (Game.MainTimer.State == 0 && !Game.IsSongPlay)
                {
                    PlayData.Data.Auto[0] = Game.IsAuto[0];
                    int poor = Score.Poor[0];
                    Score.Poor[0] = Score.Auto[0];
                    Score.Auto[0] = poor;
                }
            }
            if (Key.IsPushed(KEY_INPUT_F2) && Game.Play2P && !Game.IsReplay[1])
            {
                Game.IsAuto[1] = !Game.IsAuto[1];
                if (Game.MainTimer.State == 0 && !Game.IsSongPlay)
                {
                    PlayData.Data.Auto[1] = Game.IsAuto[1];
                    int poor = Score.Poor[1];
                    Score.Poor[1] = Score.Auto[1];
                    Score.Auto[1] = poor;
                }
            }

            if ((Key.IsPushed(KEY_INPUT_F3) && PlayData.Data.AutoRoll > 0) || (Key.IsPushing(KEY_INPUT_F3) && PlayData.Data.AutoRoll > 20))
            {
                PlayData.Data.AutoRoll--;
                for (int i = 0; i < 5; i++)
                {
                    ProcessAuto.RollTimer[i] = new Counter((long)0.0, (long)(1000.0 / PlayData.Data.AutoRoll), (long)1000.0, false);
                }
            }
            if (Key.IsPushed(KEY_INPUT_F4) || (Key.IsPushing(KEY_INPUT_F4) && PlayData.Data.AutoRoll > 20 && PlayData.Data.AutoRoll < 1000))
            {
                PlayData.Data.AutoRoll++;
                for (int i = 0; i < 5; i++)
                {
                    ProcessAuto.RollTimer[i] = new Counter((long)0.0, (long)(1000.0 / PlayData.Data.AutoRoll), (long)1000.0, false);
                }
            }

            if (Key.IsPushed(KEY_INPUT_F8))
            {
                SoundLoad.Ka[0].Play();
                Create.CreateMode = !Create.CreateMode;
            }

            if (Key.IsPushed(KEY_INPUT_Q) && Game.Wait.State == 0)
            {
                Game.Reset();
            }

            if (!Auto1P && !Failed1P && !Game.IsReplay[0])
            {
                if (Key.IsPushed(PlayData.Data.LEFTDON))
                {
                    Process(true, true, 0);
                    PlayMemory.AddData(0, Game.MainTimer.Value, true, true);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTDON))
                {
                    Process(true, false, 0);
                    PlayMemory.AddData(0, Game.MainTimer.Value, true, false);
                }
                if (Key.IsPushed(PlayData.Data.LEFTKA))
                {
                    Process(false, true, 0);
                    PlayMemory.AddData(0, Game.MainTimer.Value, false, true);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTKA))
                {
                    Process(false, false, 0);
                    PlayMemory.AddData(0, Game.MainTimer.Value, false, false);
                }
            }

            if (!Auto2P && !Failed2P && Game.Play2P && !Game.IsReplay[1])
            {
                if (Key.IsPushed(PlayData.Data.LEFTDON2P))
                {
                    Process(true, true, 1);
                    PlayMemory.AddData(1, Game.MainTimer.Value, true, true);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTDON2P))
                {
                    Process(true, false, 1);
                    PlayMemory.AddData(1, Game.MainTimer.Value, true, false);
                }
                if (Key.IsPushed(PlayData.Data.LEFTKA2P))
                {
                    Process(false, true, 1);
                    PlayMemory.AddData(1, Game.MainTimer.Value, false, true);
                }
                if (Key.IsPushed(PlayData.Data.RIGHTKA2P))
                {
                    Process(false, false, 1);
                    PlayMemory.AddData(1, Game.MainTimer.Value, false, false);
                }
            }

            if (Game.MainTimer.State == 0 && !Game.IsSongPlay)
            {
                #region 開始前
                if (Key.IsPushing(KEY_INPUT_LSHIFT) && !Game.IsReplay[0])
                {
                    if (Key.IsPushed(PlayData.Data.LEFTKA) || Key.IsPushed(PlayData.Data.RIGHTKA))
                    {
                        if (PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0])
                        {
                            if (PlayData.Data.NHSSpeed[0] < 19) PlayData.Data.NHSSpeed[0]++;
                        }
                        else
                        {
                            PlayData.Data.ScrollSpeed[0] += 0.25;
                            Game.ScrollRemain[0] += 0.25;
                        }
                        Notes.PreGreen[0] = Notes.GetGreenNumber(0, 0.25);
                    }
                    if (Key.IsPushed(PlayData.Data.LEFTDON) || Key.IsPushed(PlayData.Data.RIGHTDON))
                    {
                        if (PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0])
                        {
                            if (PlayData.Data.NHSSpeed[0] > 0) PlayData.Data.NHSSpeed[0]--;
                        }
                        else
                        {
                            PlayData.Data.ScrollSpeed[0] -= 0.25;
                            Game.ScrollRemain[0] -= 0.25;
                        }
                        Notes.PreGreen[0] = Notes.GetGreenNumber(0, -0.25);
                    }
                    if (Key.IsPushed(KEY_INPUT_LCONTROL))
                    {
                        PlayData.Data.FloatingHiSpeed[0] = !PlayData.Data.FloatingHiSpeed[0];
                        Notes.PreGreen[0] = Notes.GetGreenNumber(0);
                    }
                    if (Key.IsPushed(KEY_INPUT_LEFT))
                    {
                        Game.Adjust[0] += 0.5;
                    }
                    if (Key.IsPushed(KEY_INPUT_RIGHT))
                    {
                        Game.Adjust[0] -= 0.5;
                    }
                }
                if (Key.IsPushing(KEY_INPUT_RSHIFT) && Game.Play2P && !Game.IsReplay[1])
                {
                    if (Key.IsPushed(PlayData.Data.LEFTKA2P) || Key.IsPushed(PlayData.Data.RIGHTKA2P))
                    {
                        if (PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1])
                        {
                            if (PlayData.Data.NHSSpeed[1] < 19) PlayData.Data.NHSSpeed[1]++;
                        }
                        else
                        {
                            PlayData.Data.ScrollSpeed[1] += 0.25;
                            Game.ScrollRemain[1] += 0.25;
                        }
                        Notes.PreGreen[1] = Notes.GetGreenNumber(1, 0.25);
                    }
                    if (Key.IsPushed(PlayData.Data.LEFTDON2P) || Key.IsPushed(PlayData.Data.RIGHTDON2P))
                    {
                        if (PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1])
                        {
                            if (PlayData.Data.NHSSpeed[1] > 0) PlayData.Data.NHSSpeed[1]--;
                        }
                        else
                        {
                            PlayData.Data.ScrollSpeed[1] -= 0.25;
                            Game.ScrollRemain[1] -= 0.25;
                        }
                        Notes.PreGreen[1] = Notes.GetGreenNumber(1, -0.25);
                    }
                    if (Key.IsPushed(KEY_INPUT_RCONTROL))
                    {
                        PlayData.Data.FloatingHiSpeed[1] = !PlayData.Data.FloatingHiSpeed[1];
                        Notes.PreGreen[1] = Notes.GetGreenNumber(1);
                    }
                    if (Key.IsPushed(KEY_INPUT_LEFT))
                    {
                        Game.Adjust[1] += 0.5;
                    }
                    if (Key.IsPushed(KEY_INPUT_RIGHT))
                    {
                        Game.Adjust[1] -= 0.5;
                    }
                }
                if (!Key.IsPushing(KEY_INPUT_LSHIFT) && Key.IsPushed(KEY_INPUT_LCONTROL) && !Game.IsReplay[0])
                {
                    PlayData.Data.UseSudden[0] = !PlayData.Data.UseSudden[0];
                }
                if (!Key.IsPushing(KEY_INPUT_RSHIFT) && Key.IsPushed(KEY_INPUT_RCONTROL) && Game.Play2P && !Game.IsReplay[1])
                {
                    PlayData.Data.UseSudden[1] = !PlayData.Data.UseSudden[1];
                }
                if (Key.IsPushing(KEY_INPUT_Z) && PlayData.Data.UseSudden[0] && PlayData.Data.SuddenNumber[0] < 1000 && !Game.IsReplay[0])
                {
                    Notes.SetSudden(0, true, true);
                }
                if (Key.IsPushing(KEY_INPUT_X) && PlayData.Data.UseSudden[0] && PlayData.Data.SuddenNumber[0] > 0 && !Game.IsReplay[0])
                {
                    Notes.SetSudden(0, false, true);
                }
                if (Key.IsPushing(KEY_INPUT_SLASH) && PlayData.Data.UseSudden[1] && PlayData.Data.SuddenNumber[1] < 1000 && Game.Play2P && !Game.IsReplay[1])
                {
                    Notes.SetSudden(1, true, true);
                }
                if (Key.IsPushing(KEY_INPUT_BACKSLASH) && PlayData.Data.UseSudden[1] && PlayData.Data.SuddenNumber[1] > 0 && Game.Play2P && !Game.IsReplay[1])
                {
                    Notes.SetSudden(1, false, true);
                }
                #endregion
                if (Key.IsPushed(KEY_INPUT_PGUP))
                {
                    Game.PushedTimer[0].Start();
                }
                if (Key.IsLeft(KEY_INPUT_PGUP))
                {
                    Game.PushedTimer[0].Stop();
                    Game.PushedTimer[0].Reset();
                    Game.PushingTimer[0].Stop();
                    Game.PushingTimer[0].Reset();
                }
                if (Key.IsPushed(KEY_INPUT_PGDN))
                {
                    Game.PushedTimer[1].Start();
                }
                if (Key.IsLeft(KEY_INPUT_PGDN))
                {
                    Game.PushedTimer[1].Stop();
                    Game.PushedTimer[1].Reset();
                    Game.PushingTimer[1].Stop();
                    Game.PushingTimer[1].Reset();
                }
                for (int i = 0; i < 2; i++)
                {
                    if (Game.PushedTimer[i].Value == Game.PushedTimer[i].End)
                    {
                        Game.PushingTimer[i].Start();
                    }
                }
                if ((Key.IsPushed(KEY_INPUT_PGUP) || (Game.PushingTimer[0].Value == Game.PushingTimer[0].End)) && Game.Wait.State == 0)
                {
                    Game.MeasureUp();
                    Game.PushingTimer[0].Reset();
                }
                if ((Key.IsPushed(KEY_INPUT_PGDN) || (Game.PushingTimer[1].Value == Game.PushingTimer[1].End)) && Game.Wait.State == 0)
                {
                    Game.MeasureDown();
                    Game.PushingTimer[1].Reset();
                }
                if (Key.IsPushed(KEY_INPUT_END) && Game.Wait.State == 0)
                {
                    Game.MeasureUp(true);
                }
                if (Key.IsPushed(KEY_INPUT_HOME) && Game.Wait.State == 0)
                {
                    Game.MeasureDown(true);
                }
            }
            else if (Game.MainTimer.State != 0)
            {
                #region プレイ中
                if (Key.IsPushing(KEY_INPUT_LSHIFT) && !Game.IsReplay[0])
                {
                    if (Key.IsPushed(PlayData.Data.LEFTKA) || Key.IsPushed(PlayData.Data.RIGHTKA))
                    {
                        if (PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0])
                        {
                            if (Notes.NHSNumber[0] < 19) Notes.NHSNumber[0]++;
                        }
                        else
                        {
                            Notes.Scroll[0] += 0.25;
                            Game.ScrollRemain[0] += 0.25;
                        }
                        PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                    }
                    if (Key.IsPushed(PlayData.Data.LEFTDON) || Key.IsPushed(PlayData.Data.RIGHTDON))
                    {
                        if (PlayData.Data.NormalHiSpeed[0] && !PlayData.Data.FloatingHiSpeed[0])
                        {
                            if (Notes.NHSNumber[0] > 0) Notes.NHSNumber[0]--;
                        }
                        else
                        {
                            Notes.Scroll[0] -= 0.25;
                            Game.ScrollRemain[0] -= 0.25;
                        }
                        PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                    }
                    if (Key.IsPushed(KEY_INPUT_LCONTROL))
                    {
                        PlayData.Data.FloatingHiSpeed[0] = !PlayData.Data.FloatingHiSpeed[0];
                    }
                }
                if (Key.IsPushing(KEY_INPUT_RSHIFT) && Game.Play2P && !Game.IsReplay[1])
                {
                    if (Key.IsPushed(PlayData.Data.LEFTKA2P) || Key.IsPushed(PlayData.Data.RIGHTKA2P))
                    {
                        if (PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1])
                        {
                            if (Notes.NHSNumber[1] < 19) Notes.NHSNumber[1]++;
                        }
                        else
                        {
                            Notes.Scroll[1] += 0.25;
                            Game.ScrollRemain[1] += 0.25;
                        }
                        PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                    }
                    if (Key.IsPushed(PlayData.Data.LEFTDON2P) || Key.IsPushed(PlayData.Data.RIGHTDON2P))
                    {
                        if (PlayData.Data.NormalHiSpeed[1] && !PlayData.Data.FloatingHiSpeed[1])
                        {
                            if (Notes.NHSNumber[1] > 0) Notes.NHSNumber[1]--;
                        }
                        else
                        {
                            Notes.Scroll[1] -= 0.25;
                            Game.ScrollRemain[1] -= 0.25;
                        }
                        PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                    }
                    if (Key.IsPushed(KEY_INPUT_RCONTROL))
                    {
                        PlayData.Data.FloatingHiSpeed[1] = !PlayData.Data.FloatingHiSpeed[1];
                        PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                    }
                }
                if (!Key.IsPushing(KEY_INPUT_LSHIFT) && Key.IsPushed(KEY_INPUT_LCONTROL) && !Game.IsReplay[0])
                {
                    Notes.UseSudden[0] = !Notes.UseSudden[0];
                    PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                }
                if (!Key.IsPushing(KEY_INPUT_RSHIFT) && Key.IsPushed(KEY_INPUT_RCONTROL) && Game.Play2P && !Game.IsReplay[1])
                {
                    Notes.UseSudden[1] = !Notes.UseSudden[1];
                    PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                }
                if (Key.IsPushing(KEY_INPUT_Z) && Key.IsPushing(KEY_INPUT_X) && Notes.UseSudden[0] && !Game.IsReplay[0])
                {
                    Notes.SetSudden(0, true, false, true);
                    PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                }
                else if (Key.IsPushing(KEY_INPUT_Z) && Notes.UseSudden[0] && Notes.Sudden[0] < 1000 && !Game.IsReplay[0])
                {
                    Notes.SetSudden(0, true);
                    PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                }
                else if (Key.IsPushing(KEY_INPUT_X) && Notes.UseSudden[0] && Notes.Sudden[0] > 0 && !Game.IsReplay[0])
                {
                    Notes.SetSudden(0, false);
                    PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);

                }
                if (Key.IsPushing(KEY_INPUT_SLASH) && Key.IsPushing(KEY_INPUT_BACKSLASH) && Notes.UseSudden[1] && Game.Play2P && !Game.IsReplay[1])
                {
                    Notes.SetSudden(1, true, false, true);
                    PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                }
                else if (Key.IsPushing(KEY_INPUT_SLASH) && Notes.UseSudden[1] && Notes.Sudden[1] < 1000 && Game.Play2P && !Game.IsReplay[1])
                {
                    Notes.SetSudden(1, true);
                    PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                }
                else if (Key.IsPushing(KEY_INPUT_BACKSLASH) && Notes.UseSudden[1] && Notes.Sudden[1] > 0 && Game.Play2P && !Game.IsReplay[1])
                {
                    Notes.SetSudden(1, false);
                    PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                }
                #endregion
                for (int i = 0; i < 2; i++)
                {
                    Game.PushedTimer[i].Stop();
                    Game.PushedTimer[i].Reset();
                    Game.PushingTimer[i].Stop();
                    Game.PushingTimer[i].Reset();
                }
            }

            if (Key.IsPushed(KEY_INPUT_F11) && Game.IsSongPlay && !Game.MainSong.IsPlaying)
            {
                if (!Game.IsReplay[0] && !Key.IsPushing(KEY_INPUT_LSHIFT)) PlayMemory.SaveData(0);
                if (Game.Play2P && !Game.IsReplay[1] & Key.IsPushing(KEY_INPUT_LSHIFT)) PlayMemory.SaveData(1);
            }
        }

        public static void Process(bool isDon, bool isLeft, int player)
        {
            Chip chip = GetNotes.GetNearNote(Game.MainTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value - Game.Adjust[player]);
            Chip nowchip = GetNotes.GetNowNote(Game.MainTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value - Game.Adjust[player]);
            EJudge judge;
            ERoll roll = nowchip != null ? ProcessNote.RollState(nowchip) : ERoll.None;
            if (PlayData.Data.PreviewType == 3 || Game.IsAuto[player])
            {
                judge = EJudge.Auto;
            }
            else if (chip == null)
            {
                judge = EJudge.Through;
            }
            else
            {
                judge = GetNotes.GetJudge(chip, Game.MainTimer.Value - Game.Adjust[player]);
            }
            if (Game.MainTimer.State != 0)
            {
                if (chip != null && roll == ERoll.None)
                {

                    ProcessNote.Process(judge, chip, isDon, player);
                }
                else
                {
                    ProcessNote.RollProcess(nowchip, isDon, player);
                }
            }

            Sound[][] taiko = new Sound[5][];
            for (int i = 0; i < 5; i++)
            {
                taiko[i] = new Sound[2];
                taiko[i][0] = SoundLoad.Don[i];
                taiko[i][1] = SoundLoad.Ka[i];
            }
            if (chip != null && Math.Abs(Game.MainTimer.Value - Game.Adjust[player] - chip.Time) <= 32 && ((chip.ENote == ENote.DON || chip.ENote == ENote.KA) && (judge <= EJudge.Great || judge == EJudge.Auto)) || roll == ERoll.ROLL)
            {
                taiko[player][0] = SoundLoad.DON[player];
                taiko[player][0].Volume = (PlayData.Data.SE / 100.0) * (Game.MainTJA[0].Header.SEVOL / 100.0) * 1.5;
                taiko[player][1] = SoundLoad.KA[player];
                taiko[player][1].Volume = (PlayData.Data.SE / 100.0) * (Game.MainTJA[0].Header.SEVOL / 100.0) * 1.5;
            }
            else
            {
                taiko[player][0] = SoundLoad.Don[player];
                taiko[player][0].Volume = (PlayData.Data.SE / 100.0) * (Game.MainTJA[0].Header.SEVOL / 100.0);
                taiko[player][1] = SoundLoad.Ka[player];
                taiko[player][1].Volume = (PlayData.Data.SE / 100.0) * (Game.MainTJA[0].Header.SEVOL / 100.0);
            }

            if (PlayData.Data.PreviewType == 3)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (Game.MainTJA[i].Courses[Game.Course[i]].ListChip.Count > 0)
                    {
                        if (i > 0 && Game.Course[i] == Game.Course[i - 1]) break;
                        count++;
                    }
                }
                switch (count)
                {
                    case 1:
                        taiko[0][0].Pan = taiko[0][1].Pan = 0;
                        break;
                    case 2:
                        taiko[0][0].Pan = taiko[0][1].Pan = -255;
                        taiko[1][0].Pan = taiko[1][1].Pan = 255;
                        break;
                    case 3:
                        taiko[0][0].Pan = taiko[0][1].Pan = -255;
                        taiko[1][0].Pan = taiko[1][1].Pan = 0;
                        taiko[2][0].Pan = taiko[2][1].Pan = 255;
                        break;
                    case 4:
                        taiko[0][0].Pan = taiko[0][1].Pan = -255;
                        taiko[1][0].Pan = taiko[1][1].Pan = -80;
                        taiko[2][0].Pan = taiko[2][1].Pan = 80;
                        taiko[3][0].Pan = taiko[3][1].Pan = 255;
                        break;
                    case 5:
                        taiko[0][0].Pan = taiko[0][1].Pan = -255;
                        taiko[1][0].Pan = taiko[1][1].Pan = -128;
                        taiko[2][0].Pan = taiko[2][1].Pan = 0;
                        taiko[3][0].Pan = taiko[3][1].Pan = 128;
                        taiko[4][0].Pan = taiko[4][1].Pan = 255;
                        break;
                }
            }
            else
            {
                if (Game.Play2P)
                {
                    if (player == 0) taiko[0][0].Pan = taiko[0][1].Pan = -255;
                    if (player == 1) taiko[1][0].Pan = taiko[1][1].Pan = 255;
                }
                else
                {
                    taiko[0][0].Pan = taiko[0][1].Pan = 0;
                }
            }
            

            if (PlayData.Data.ChangeSESpeed)
            {
                taiko[player][0].PlaySpeed = PlayData.Data.PlaySpeed;
                taiko[player][1].PlaySpeed = PlayData.Data.PlaySpeed;
            }
            else
            {
                taiko[player][0].PlaySpeed = 1.0;
                taiko[player][1].PlaySpeed = 1.0;
            }

            if (isDon) taiko[player][0].Play();
            else taiko[player][1].Play();

            if (isDon)
            {
                if (isLeft)
                {
                    Game.HitTimer[player][0].Reset();
                    Game.HitTimer[player][0].Start();
                }
                else
                {
                    Game.HitTimer[player][1].Reset();
                    Game.HitTimer[player][1].Start();
                }
            }
            else
            {
                if (isLeft)
                {
                    Game.HitTimer[player][2].Reset();
                    Game.HitTimer[player][2].Start();
                }
                else
                {
                    Game.HitTimer[player][3].Reset();
                    Game.HitTimer[player][3].Start();
                }
            }
        }
    }
}
