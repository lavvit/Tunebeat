using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
            if (Create.Selecting)
            {

                if (Create.Cursor != 11)
                {
                    if (Key.IsPushed(KEY_INPUT_RETURN))
                    {
                        Create.Selecting = !Create.Selecting;
                        #region HeaderLoad
                        switch (Create.Cursor)
                        {
                            case 0:
                                Create.File.Title = Input.Text;
                                break;
                            case 1:
                                Create.File.SubTitle = Input.Text;
                                break;
                            case 2:
                                Create.File.Wave = Input.Text;
                                if (!string.IsNullOrEmpty(Create.File.Wave)) Game.MainSong = new Sound($"{Path.GetDirectoryName(Game.MainTJA[0].TJAPath)}/{Create.File.Wave}");
                                break;
                            case 3:
                                Create.File.BGImage = Input.Text;
                                if (!string.IsNullOrEmpty(Create.File.BGImage)) Game.MainImage = new Texture($"{Path.GetDirectoryName(Game.MainTJA[0].TJAPath)}/{Create.File.BGImage}");
                                break;
                            case 4:
                                Create.File.BGMovie = Input.Text;
                                string movie = $"{Path.GetDirectoryName(Game.MainTJA[0].TJAPath)}/{Create.File.BGMovie}";
                                movie = movie.Replace(".wmv", ".mp4");
                                if (!string.IsNullOrEmpty(movie)) Game.MainMovie = new Movie(movie);
                                break;
                            case 5:
                                if (double.TryParse(Input.Text, out double d)) Create.File.Bpm = double.Parse(Input.Text);
                                break;
                            case 6:
                                if (double.TryParse(Input.Text, out double dd)) Create.File.Offset = double.Parse(Input.Text);
                                break;
                            case 7:
                                if (int.TryParse(Input.Text, out int s)) Create.File.SongVol = int.Parse(Input.Text);
                                break;
                            case 8:
                                if (int.TryParse(Input.Text, out int ss)) Create.File.SeVol = int.Parse(Input.Text);
                                break;
                            case 9:
                                if (double.TryParse(Input.Text, out double de)) Create.File.DemoStart = double.Parse(Input.Text);
                                break;
                            case 10:
                                Create.File.Genre = Input.Text;
                                break;
                            case 12:
                                if (int.TryParse(Input.Text, out int le)) Create.File.Level[Game.Course[0]] = int.Parse(Input.Text);
                                break;
                            case 13:
                                if (double.TryParse(Input.Text, out double to)) Create.File.Total[Game.Course[0]] = double.Parse(Input.Text);
                                break;
                        }
                        #endregion
                        Input.End();
                    }
                    if (Key.IsPushed(KEY_INPUT_ESCAPE))
                    {
                        Create.Selecting = !Create.Selecting;
                        Input.End();
                    }
                    #region Timer
                    if (Key.IsPushed(PlayData.Data.MeasureUp))
                    {
                        Game.PushedTimer[0].Start();
                    }
                    if (Key.IsLeft(PlayData.Data.MeasureUp))
                    {
                        Game.PushedTimer[0].Stop();
                        Game.PushedTimer[0].Reset();
                        Game.PushingTimer[0].Stop();
                        Game.PushingTimer[0].Reset();
                    }
                    if (Key.IsPushed(PlayData.Data.MeasureDown))
                    {
                        Game.PushedTimer[1].Start();
                    }
                    if (Key.IsLeft(PlayData.Data.MeasureDown))
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
                    #endregion
                    if ((Key.IsPushed(PlayData.Data.MeasureUp) || (Game.PushingTimer[0].Value == Game.PushingTimer[0].End)))
                    {
                        Game.MeasureUp();
                        Game.PushingTimer[0].Reset();
                    }
                    if ((Key.IsPushed(PlayData.Data.MeasureDown) || (Game.PushingTimer[1].Value == Game.PushingTimer[1].End)))
                    {
                        Game.MeasureDown();
                        Game.PushingTimer[1].Reset();
                    }
                    if (Key.IsPushed(PlayData.Data.JunpEnd))
                    {
                        Game.MeasureUp(true);
                    }
                    if (Key.IsPushed(PlayData.Data.JunpHome))
                    {
                        Game.MeasureDown(true);
                    }
                }
                else
                {
                    if (Key.IsPushed(KEY_INPUT_RIGHT))
                    {
                        if (Game.Course[0] < 4)
                            Game.Course[0]++;
                        else
                            Game.Course[0] = 0;
                        Game.Reset();
                    }
                    if (Key.IsPushed(KEY_INPUT_LEFT))
                    {
                        if (Game.Course[0] > 0)
                            Game.Course[0]--;
                        else
                            Game.Course[0] = 4;
                        Game.Reset();
                    }
                    if (Key.IsPushed(KEY_INPUT_RETURN))
                    {
                        if (!Game.MainTJA[0].Courses[Game.Course[0]].IsEnable)
                        {
                            Create.Save(Game.TJAPath);
                            Game.Reset();
                        }
                        Create.Selecting = !Create.Selecting;
                    }
                }
            }
            else if (Create.CommandLayer == 2)
            {
                if (Key.IsPushed(KEY_INPUT_RETURN))
                {
                    if (!Create.DeleteMode) Create.AddCommand(Input.Text);
                    else Create.DeleteCommand(Input.Text);
                    Create.Save(Game.TJAPath);
                    Game.Reset();
                    Input.End();
                    Create.CommandLayer = 0;
                }
                if (Key.IsPushed(KEY_INPUT_ESCAPE))
                {
                    Create.CommandLayer = 0;
                    Input.End();
                }
            }
            else
            {
                if (Key.IsPushed(PlayData.Data.ChangeAuto) && !Game.IsReplay[0])
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
                if (Key.IsPushed(PlayData.Data.ChangeAuto2P) && Game.Play2P && !Game.IsReplay[1])
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

#if DEBUG
                if ((Key.IsPushed(KEY_INPUT_RBRACKET) && PlayData.Data.AutoRoll > 0) || (Key.IsPushing(KEY_INPUT_RBRACKET) && PlayData.Data.AutoRoll > 20))
                {
                    PlayData.Data.AutoRoll--;
                    for (int i = 0; i < 5; i++)
                    {
                        ProcessAuto.RollTimer[i] = new Counter((long)0.0, (long)(1000.0 / PlayData.Data.AutoRoll), (long)1000.0, false);
                    }
                }
                if (Key.IsPushed(KEY_INPUT_LBRACKET) || (Key.IsPushing(KEY_INPUT_LBRACKET) && PlayData.Data.AutoRoll > 20 && PlayData.Data.AutoRoll < 1000))
                {
                    PlayData.Data.AutoRoll++;
                    for (int i = 0; i < 5; i++)
                    {
                        ProcessAuto.RollTimer[i] = new Counter((long)0.0, (long)(1000.0 / PlayData.Data.AutoRoll), (long)1000.0, false);
                    }
                }
#endif

                if (Key.IsPushed(PlayData.Data.PlayReset))
                {
                    Game.Reset();
                }

                if (!Auto1P && !Failed1P && !Game.IsReplay[0])
                {
                    if (ListPushed(PlayData.Data.LEFTDON))
                    {
                        Process(true, true, 0);
                        PlayMemory.AddData(0, Game.MainTimer.Value, true, true);
                    }
                    if (ListPushed(PlayData.Data.RIGHTDON))
                    {
                        Process(true, false, 0);
                        PlayMemory.AddData(0, Game.MainTimer.Value, true, false);
                    }
                    if (ListPushed(PlayData.Data.LEFTKA))
                    {
                        Process(false, true, 0);
                        PlayMemory.AddData(0, Game.MainTimer.Value, false, true);
                    }
                    if (ListPushed(PlayData.Data.RIGHTKA))
                    {
                        Process(false, false, 0);
                        PlayMemory.AddData(0, Game.MainTimer.Value, false, false);
                    }
                }
                if (!Auto2P && !Failed2P && Game.Play2P && !Game.IsReplay[1])
                {
                    if (ListPushed(PlayData.Data.LEFTDON2P))
                    {
                        Process(true, true, 1);
                        PlayMemory.AddData(1, Game.MainTimer.Value, true, true);
                    }
                    if (ListPushed(PlayData.Data.RIGHTDON2P))
                    {
                        Process(true, false, 1);
                        PlayMemory.AddData(1, Game.MainTimer.Value, true, false);
                    }
                    if (ListPushed(PlayData.Data.LEFTKA2P))
                    {
                        Process(false, true, 1);
                        PlayMemory.AddData(1, Game.MainTimer.Value, false, true);
                    }
                    if (ListPushed(PlayData.Data.RIGHTKA2P))
                    {
                        Process(false, false, 1);
                        PlayMemory.AddData(1, Game.MainTimer.Value, false, false);
                    }
                }

                if (Game.MainTimer.State == 0 && !Game.IsSongPlay)
                {
                    #region 開始前
                    if (Key.IsPushed(PlayData.Data.DisplaySudden) && !Game.IsReplay[0])
                    {
                        if (Game.SuddenTimer[0].State == 0)
                        {
                            Game.SuddenTimer[0].Reset();
                            Game.SuddenTimer[0].Start();
                        }
                        else
                        {
                            PlayData.Data.UseSudden[0] = !PlayData.Data.UseSudden[0];
                            Game.SuddenTimer[0].Stop();
                        }
                    }
                    if (Key.IsPushed(PlayData.Data.DisplaySudden2P) && Game.Play2P && !Game.IsReplay[1])
                    {
                        if (Game.SuddenTimer[1].State == 0)
                        {
                            Game.SuddenTimer[1].Reset();
                            Game.SuddenTimer[1].Start();
                        }
                        else
                        {
                            PlayData.Data.UseSudden[1] = !PlayData.Data.UseSudden[1];
                            Game.SuddenTimer[1].Stop();
                        }
                    }
                    if (Key.IsPushing(PlayData.Data.DisplaySudden) && !Game.IsReplay[0])
                    {
                        #region Sudden1P
                        if (ListPushed(PlayData.Data.LEFTKA) || ListPushed(PlayData.Data.RIGHTKA))
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
                        if (ListPushed(PlayData.Data.LEFTDON) || ListPushed(PlayData.Data.RIGHTDON))
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
                        if (Key.IsPushed(KEY_INPUT_LEFT))
                        {
                            Game.Adjust[0] += 0.5;
                        }
                        if (Key.IsPushed(KEY_INPUT_RIGHT))
                        {
                            Game.Adjust[0] -= 0.5;
                        }
                        if (Key.IsPushing(PlayData.Data.SuddenPlus) && PlayData.Data.UseSudden[0] && PlayData.Data.SuddenNumber[0] < 1000)
                        {
                            Notes.SetSudden(0, true, true);
                        }
                        if (Key.IsPushing(PlayData.Data.SuddenMinus) && PlayData.Data.UseSudden[0] && PlayData.Data.SuddenNumber[0] > 0)
                        {
                            Notes.SetSudden(0, false, true);
                        }
                        if (Key.IsPushed(PlayData.Data.ChangeFHS))
                        {
                            PlayData.Data.FloatingHiSpeed[0] = !PlayData.Data.FloatingHiSpeed[0];
                            Notes.PreGreen[0] = Notes.GetGreenNumber(0);
                        }
                        #endregion
                    }
                    if (Key.IsPushing(PlayData.Data.DisplaySudden2P) && Game.Play2P && !Game.IsReplay[1])
                    {
                        #region Sudden2P
                        if (ListPushed(PlayData.Data.LEFTKA2P) || ListPushed(PlayData.Data.RIGHTKA2P))
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
                        if (ListPushed(PlayData.Data.LEFTDON2P) || ListPushed(PlayData.Data.RIGHTDON2P))
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
                        if (Key.IsPushed(KEY_INPUT_LEFT))
                        {
                            Game.Adjust[1] += 0.5;
                        }
                        if (Key.IsPushed(KEY_INPUT_RIGHT))
                        {
                            Game.Adjust[1] -= 0.5;
                        }
                        if (Key.IsPushing(PlayData.Data.SuddenPlus2P) && PlayData.Data.UseSudden[1] && PlayData.Data.SuddenNumber[1] < 1000)
                        {
                            Notes.SetSudden(1, true, true);
                        }
                        if (Key.IsPushing(PlayData.Data.SuddenMinus2P) && PlayData.Data.UseSudden[1] && PlayData.Data.SuddenNumber[1] > 0)
                        {
                            Notes.SetSudden(1, false, true);
                        }
                        if (Key.IsPushed(PlayData.Data.ChangeFHS2P))
                        {
                            PlayData.Data.FloatingHiSpeed[1] = !PlayData.Data.FloatingHiSpeed[1];
                            Notes.PreGreen[1] = Notes.GetGreenNumber(1);
                        }
                        #endregion
                    }

                    #region Timer
                    if (Key.IsPushed(PlayData.Data.MeasureUp))
                    {
                        Game.PushedTimer[0].Start();
                    }
                    if (Key.IsLeft(PlayData.Data.MeasureUp))
                    {
                        Game.PushedTimer[0].Stop();
                        Game.PushedTimer[0].Reset();
                        Game.PushingTimer[0].Stop();
                        Game.PushingTimer[0].Reset();
                    }
                    if (Key.IsPushed(PlayData.Data.MeasureDown))
                    {
                        Game.PushedTimer[1].Start();
                    }
                    if (Key.IsLeft(PlayData.Data.MeasureDown))
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
                    #endregion
                    if ((Key.IsPushed(PlayData.Data.MeasureUp) || (Game.PushingTimer[0].Value == Game.PushingTimer[0].End)))
                    {
                        Game.MeasureUp();
                        Game.PushingTimer[0].Reset();
                    }
                    if ((Key.IsPushed(PlayData.Data.MeasureDown) || (Game.PushingTimer[1].Value == Game.PushingTimer[1].End)))
                    {
                        Game.MeasureDown();
                        Game.PushingTimer[1].Reset();
                    }
                    if (Key.IsPushed(PlayData.Data.JunpEnd))
                    {
                        Game.MeasureUp(true);
                    }
                    if (Key.IsPushed(PlayData.Data.JunpHome))
                    {
                        Game.MeasureDown(true);
                    }
                    #endregion
                }
                else if (Game.MainTimer.State != 0)
                {
                    #region プレイ中
                    if (Key.IsPushed(PlayData.Data.DisplaySudden) && !Game.IsReplay[0])
                    {
                        if (Game.SuddenTimer[0].State == 0)
                        {
                            Game.SuddenTimer[0].Reset();
                            Game.SuddenTimer[0].Start();
                        }
                        else
                        {
                            Notes.UseSudden[0] = !Notes.UseSudden[0];
                            PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                            Game.SuddenTimer[0].Stop();
                        }
                    }
                    if (Key.IsPushed(PlayData.Data.DisplaySudden2P) && Game.Play2P && !Game.IsReplay[1])
                    {
                        if (Game.SuddenTimer[1].State == 0)
                        {
                            Game.SuddenTimer[1].Reset();
                            Game.SuddenTimer[1].Start();
                        }
                        else
                        {
                            Notes.UseSudden[1] = !Notes.UseSudden[1];
                            PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                            Game.SuddenTimer[1].Stop();
                        }
                    }
                    if (Key.IsPushing(PlayData.Data.DisplaySudden) && !Game.IsReplay[0])
                    {
                        #region Sudden1P
                        if (ListPushed(PlayData.Data.LEFTKA) || ListPushed(PlayData.Data.RIGHTKA))
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
                        if (ListPushed(PlayData.Data.LEFTDON) || ListPushed(PlayData.Data.RIGHTDON))
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
                        if (Key.IsPushed(KEY_INPUT_LEFT))
                        {
                            Game.Adjust[0] += 0.5;
                            PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                        }
                        if (Key.IsPushed(KEY_INPUT_RIGHT))
                        {
                            Game.Adjust[0] -= 0.5;
                            PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                        }
                        if (Key.IsPushing(PlayData.Data.SuddenPlus) && Key.IsPushing(PlayData.Data.SuddenMinus) && Notes.UseSudden[0])
                        {
                            Notes.SetSudden(0, true, false, true);
                            PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                        }
                        else if (Key.IsPushing(PlayData.Data.SuddenPlus) && Notes.UseSudden[0] && Notes.Sudden[0] < 1000)
                        {
                            Notes.SetSudden(0, true);
                            PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);
                        }
                        else if (Key.IsPushing(PlayData.Data.SuddenMinus) && Notes.UseSudden[0] && Notes.Sudden[0] > 0)
                        {
                            Notes.SetSudden(0, false);
                            PlayMemory.AddSetting(0, Game.MainTimer.Value, Notes.Scroll[0], Notes.Sudden[0], Notes.UseSudden[0], Game.Adjust[0]);

                        }
                        if (Key.IsPushed(PlayData.Data.ChangeFHS))
                        {
                            PlayData.Data.FloatingHiSpeed[0] = !PlayData.Data.FloatingHiSpeed[0];
                        }
                        #endregion
                    }
                    if (Key.IsPushing(PlayData.Data.DisplaySudden2P) && Game.Play2P && !Game.IsReplay[1])
                    {
                        #region Sudden2P
                        if (ListPushed(PlayData.Data.LEFTKA2P) || ListPushed(PlayData.Data.RIGHTKA2P))
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
                        if (ListPushed(PlayData.Data.LEFTDON2P) || ListPushed(PlayData.Data.RIGHTDON2P))
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
                        if (Key.IsPushed(KEY_INPUT_LEFT))
                        {
                            Game.Adjust[1] += 0.5;
                            PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                        }
                        if (Key.IsPushed(KEY_INPUT_RIGHT))
                        {
                            Game.Adjust[1] -= 0.5;
                            PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                        }
                        if (Key.IsPushing(PlayData.Data.SuddenPlus2P) && Key.IsPushing(PlayData.Data.SuddenMinus2P) && Notes.UseSudden[1])
                        {
                            Notes.SetSudden(1, true, false, true);
                            PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                        }
                        else if (Key.IsPushing(PlayData.Data.SuddenPlus2P) && Notes.UseSudden[1] && Notes.Sudden[1] < 1000)
                        {
                            Notes.SetSudden(1, true);
                            PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                        }
                        else if (Key.IsPushing(PlayData.Data.SuddenMinus2P) && Notes.UseSudden[1] && Notes.Sudden[1] > 0)
                        {
                            Notes.SetSudden(1, false);
                            PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                        }
                        if (Key.IsPushed(PlayData.Data.ChangeFHS2P))
                        {
                            PlayData.Data.FloatingHiSpeed[1] = !PlayData.Data.FloatingHiSpeed[1];
                            PlayMemory.AddSetting(1, Game.MainTimer.Value, Notes.Scroll[1], Notes.Sudden[1], Notes.UseSudden[1], Game.Adjust[1]);
                        }
                        #endregion
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Game.PushedTimer[i].Stop();
                        Game.PushedTimer[i].Reset();
                        Game.PushingTimer[i].Stop();
                        Game.PushingTimer[i].Reset();
                    }
                    #endregion
                }

                if (Key.IsPushed(PlayData.Data.SaveReplay) && Game.IsSongPlay && !Game.MainSong.IsPlaying && Game.PlayMeasure == 0)
                {
                    if (!Game.IsReplay[0] && !Key.IsPushing(KEY_INPUT_LSHIFT)) PlayMemory.SaveData(0);
                    if (Game.Play2P && !Game.IsReplay[1] & Key.IsPushing(KEY_INPUT_LSHIFT)) PlayMemory.SaveData(1);
                }

                #region Create
                int[] wid = new int[8] { 0, 29, 60, 75, 84, 90, 97, 104 };
                if (Key.IsPushed(PlayData.Data.MoveCreate))
                {
                    SoundLoad.Ka[0].Play();
                    Create.CreateMode = !Create.CreateMode;
                    Create.InfoMenu = false;
                    if (Key.IsPushing(KEY_INPUT_RSHIFT))
                    {
                        Create.BarInit(Game.Course[0]);
                        Create.Save(Game.TJAPath);
                    }
                    else Create.BarLoad(Game.Course[0]);
                    Game.Reset();
                    if (Create.CreateMode && Game.MainTimer.State == 0)
                    {
                        Game.TimeRemain = -wid[Create.NowInput];
                    }
                    else if (Game.MainTimer.State == 0)
                    {
                        Game.TimeRemain = wid[Create.NowInput];
                    }
                }
                if (Create.CreateMode)
                {
                    if (Key.IsPushed(KEY_INPUT_RSHIFT) || Key.IsLeft(KEY_INPUT_RSHIFT))
                    {
                        Create.Preview = !Create.Preview;
                        if (Create.Preview && Game.MainTimer.State == 0)
                        {
                            Game.TimeRemain = -wid[Create.NowInput];
                        }
                        else if (Game.MainTimer.State == 0)
                        {
                            Game.TimeRemain = wid[Create.NowInput];
                        }
                    }
                    int[] inputlist = new int[8] { 4, 8, 12, 16, 20, 24, 32, 48 };
                    if (Key.IsPushed(KEY_INPUT_DIVIDE))
                    {
                        SoundLoad.Ka[0].Play();
                        if (Create.NowInput > 0) Game.TimeRemain = wid[Create.NowInput] - wid[Create.NowInput - 1];
                        if (Create.NowInput-- <= 0) Create.NowInput = 0;
                        Create.InputType = inputlist[Create.NowInput];
                    }
                    if (Key.IsPushed(KEY_INPUT_MULTIPLY))
                    {
                        SoundLoad.Ka[0].Play();
                        if (Create.NowInput < 7) Game.TimeRemain = wid[Create.NowInput] - wid[Create.NowInput + 1];
                        if (Create.NowInput++ >= 7) Create.NowInput = 7;
                        Create.InputType = inputlist[Create.NowInput];
                    }
                    if (Create.CommandLayer > 0)
                    {
                        if (ListPushed(PlayData.Data.LEFTDON) || ListPushed(PlayData.Data.RIGHTDON))
                        {
                            BarLine bar = Create.File.Bar[Game.Course[0]][Game.NowMeasure - 1];
                            Create.SelectedChip = (null, null);
                            Create.SelectedChip = (bar, bar.Chip[0]);
                            Input.Init();
                            Create.CommandLayer++;
                        }
                    }
                    if (Mouse.X >= Notes.NotesP[0].X - 22 && Mouse.Y >= Notes.NotesP[0].Y && Mouse.Y < Notes.NotesP[0].Y + 195 && Game.NowMeasure > 0 && Game.MainTimer.State == 0)
                    {
                        int chiprange = Create.File.Bar[Game.Course[0]][Game.NowMeasure - 1].Chip.Count;
                        int amount = Create.InputType;
                        double width = (1920 - (Notes.NotesP[0].X - 22)) / (double)amount;
                        double x;
                        for (int i = 0; i < amount; i++)
                        {
                            x = Notes.NotesP[0].X - 22 + i * width;
                            int nownote = i * (chiprange / Create.InputType);
                            Chip chip = Create.File.Bar[Game.Course[0]][Game.NowMeasure - 1].Chip[nownote];
                            if (Mouse.X >= x && Mouse.X < x + width)
                            {
                                DrawString(1100, 500, $"Time:{chip.Time}", 0xffffff);
                                if (Create.CommandLayer > 0)
                                {
                                    if (Mouse.IsPushed(MouseButton.Left))
                                    {
                                        Create.DeleteMode = false;
                                        Create.SelectedChip = (null, null);
                                        Create.SelectedChip = (Create.File.Bar[Game.Course[0]][Game.NowMeasure - 1], chip);
                                        Input.Init();
                                        Create.CommandLayer++;
                                    }
                                    if (Mouse.IsPushed(MouseButton.Right))
                                    {
                                        Create.DeleteMode = true;
                                        Create.SelectedChip = (null, null);
                                        Create.SelectedChip = (Create.File.Bar[Game.Course[0]][Game.NowMeasure - 1], chip);
                                        Input.Init();
                                        Create.CommandLayer++;
                                    }
                                }
                                else
                                {
                                    ENote color = Create.NowColor == 7 ? ENote.Kusudama : (ENote)Create.NowColor + 1;
                                    if (Mouse.IsPushing(MouseButton.Left) && !Create.RollEnd && chip.ENote < ENote.RollStart)
                                    {
                                        Create.Edited = true;
                                        if (chip.ENote != color)
                                        {
                                            chip.ENote = color;
                                        }
                                    }
                                    if (Mouse.IsPushing(MouseButton.Right))
                                    {
                                        Create.Edited = true;
                                        chip.ENote = ENote.Space;
                                        chip.RollEnd = null;
                                    }

                                    if (Mouse.IsPushed(MouseButton.Left))
                                    {
                                        if (Create.NowColor == 1 || Create.NowColor == 3)
                                            SoundLoad.Ka[0].Play();
                                        else
                                            SoundLoad.Don[0].Play();
                                        if (Create.RollEnd)
                                        {
                                            chip.ENote = ENote.RollEnd;
                                            Create.RollBegin.RollEnd = chip;
                                            Create.RollEnd = !Create.RollEnd;
                                        }
                                        else if (Create.NowColor >= 4)
                                        {
                                            if (Create.NowColor >= 4) Create.RollBegin = chip;
                                            Create.RollEnd = !Create.RollEnd;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Create.AllText != null)
                    {
                        if ((Key.IsPushing(KEY_INPUT_SUBTRACT) || (Mouse.X >= 1280 && Mouse.Wheel > 0)) && Create.NowScroll > 0)
                        {
                            Create.NowScroll--;
                        }
                        if ((Key.IsPushing(KEY_INPUT_ADD) || (Mouse.X >= 1280 && Mouse.Wheel < 0)) && Create.NowScroll + 29 < Create.Course[Game.Course[0]].Count)
                        {
                            Create.NowScroll++;
                        }
                    }
                    if (Key.IsPushed(PlayData.Data.RealTimeMapping))
                    {
                        Create.Mapping = !Create.Mapping;
                        if (Create.Mapping)
                        {
                            DrawLog.Draw("マッピングを開始します…");
                        }
                        if (!Create.Mapping)
                        {
                            DrawLog.Draw("マッピングをセーブしました!");
                            Create.Save(Game.TJAPath);
                            Game.Reset();
                        }
                    }
                    if (Create.Mapping)
                    {
                        if (ListPushed(PlayData.Data.LEFTDON) || ListPushed(PlayData.Data.RIGHTDON))
                        {
                            SoundLoad.Don[0].Play();
                            Create.InputN(Game.Course[0], Game.MainTimer.Value, Game.NowMeasure, true);
                            Flash(true, ListPushed(PlayData.Data.LEFTDON), 0);
                        }
                        if (ListPushed(PlayData.Data.LEFTKA) || ListPushed(PlayData.Data.RIGHTKA))
                        {
                            SoundLoad.Ka[0].Play();
                            Create.InputN(Game.Course[0], Game.MainTimer.Value, Game.NowMeasure, false);
                            Flash(false, ListPushed(PlayData.Data.LEFTKA), 0);
                        }
                    }
                    if (Key.IsPushed(PlayData.Data.SaveFile))
                    {
                        DrawLog.Draw("セーブしました!");
                        Create.Save(Game.TJAPath);
                    }
                    if (Key.IsPushed(PlayData.Data.InfoMenu))
                    {
                        Create.InfoMenu = !Create.InfoMenu;
                    }
                    if (Key.IsPushed(PlayData.Data.AddMeasure))
                    {
                        Create.AddMeasure();
                        Create.Save(Game.TJAPath);
                        Game.Reset();
                    }
                    if (Key.IsPushed(PlayData.Data.AddCommand))
                    {
                        if (Create.CommandLayer > 0) Create.CommandLayer = 0;
                        else Create.CommandLayer++;
                    }
                    if (!Create.InfoMenu)
                    {
                        if (Key.IsPushed(KEY_INPUT_UP))
                        {
                            SoundLoad.Ka[0].Play();
                            if (Create.NowColor-- <= 0) Create.NowColor = 7;
                        }
                        if (Key.IsPushed(KEY_INPUT_DOWN))
                        {
                            SoundLoad.Ka[0].Play();
                            if (Create.NowColor++ >= 7) Create.NowColor = 0;
                        }
                    }
                    else
                    {
                        if (Key.IsPushed(KEY_INPUT_UP))
                        {
                            if (Create.Cursor-- <= 0) Create.Cursor = 13;
                        }
                        if (Key.IsPushed(KEY_INPUT_DOWN))
                        {
                            if (Create.Cursor++ >= 13) Create.Cursor = 0;
                        }
                        if (Key.IsPushed(KEY_INPUT_RETURN))
                        {
                            Create.Selecting = !Create.Selecting;
                            if (Create.Cursor != 11) Input.Init();
                            switch (Create.Cursor)
                            {
                                case 0:
                                    if (!string.IsNullOrEmpty(Create.File.Title)) Input.Text = Create.File.Title;
                                    break;
                                case 1:
                                    if (!string.IsNullOrEmpty(Create.File.SubTitle)) Input.Text = Create.File.SubTitle;
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(Create.File.Wave)) Input.Text = Create.File.Wave;
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(Create.File.BGImage)) Input.Text = Create.File.BGImage;
                                    break;
                                case 4:
                                    if (!string.IsNullOrEmpty(Create.File.BGMovie)) Input.Text = Create.File.BGMovie;
                                    break;
                                case 5:
                                    Input.Text = $"{Create.File.Bpm}";
                                    break;
                                case 6:
                                    Input.Text = $"{Create.File.Offset}";
                                    break;
                                case 7:
                                    Input.Text = $"{Create.File.SongVol}";
                                    break;
                                case 8:
                                    Input.Text = $"{Create.File.SeVol}";
                                    break;
                                case 9:
                                    Input.Text = $"{Create.File.DemoStart}";
                                    break;
                                case 10:
                                    if (!string.IsNullOrEmpty(Create.File.Genre)) Input.Text = Create.File.Genre;
                                    break;
                                case 12:
                                    Input.Text = $"{Create.File.Level[Game.Course[0]]}";
                                    break;
                                case 13:
                                    Input.Text = $"{Create.File.Total[Game.Course[0]]}";
                                    break;
                            }
                        }
                    }
                }
            }
            #endregion
        }

        public static bool ListPushed(List<int> keylist)
        {
            foreach (int key in keylist)
            {
                if (Key.IsPushed(key))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ListPushing(List<int> keylist)
        {
            foreach (int key in keylist)
            {
                if (Key.IsPushing(key))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ListLeft(List<int> keylist)
        {
            foreach (int key in keylist)
            {
                if (Key.IsLeft(key))
                {
                    return true;
                }
            }
            return false;
        }

        public static void Process(bool isDon, bool isLeft, int player)
        {
            Chip chip = GetNotes.GetNearNote(Game.MainTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value - Game.Adjust[player]);
            Chip nowchip = GetNotes.GetNowNote(Game.MainTJA[player].Courses[Game.Course[player]].ListChip, Game.MainTimer.Value - Game.Adjust[player]);
            if (Create.CreateMode)
            {
                List<Chip> list = new List<Chip>();
                for (int i = 0; i < Create.File.Bar[Game.Course[player]].Count; i++)
                {
                    foreach (Chip c in Create.File.Bar[Game.Course[player]][i].Chip)
                        list.Add(c);
                }
                chip = GetNotes.GetNearNote(list, Game.MainTimer.Value - Game.Adjust[player]);
                nowchip = GetNotes.GetNowNote(list, Game.MainTimer.Value - Game.Adjust[player]);
            }
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

            Flash(isDon, isLeft, player);
        }
        public static void Flash(bool isDon, bool isLeft, int player)
        {
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
