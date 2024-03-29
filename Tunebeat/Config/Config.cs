﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;
using SeaDrop;
using TJAParse;

namespace Tunebeat
{
    public class Config : Scene
    {
        public override void Enable()
        {
            OptionList = new List<Option>();

            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i] = new Counter(0, 499, 1000, false);
                PushingTimer[i] = new Counter(0, 24, 1000, false);
            }
                
            Change();
            base.Enable();
        }

        public override void Disable()
        {
            OptionList = null;

            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i].Reset();
                PushingTimer[i].Reset();
            }
            base.Disable();
        }
        public override void Draw()
        {
            Drawing.Text(80, 120 + 40 * Layer, ">", 0xff0000);
            Drawing.Text(100, 120, "System", Layer == (int)ELayer.System ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 160, "Play TJA", Layer == (int)ELayer.PlayTJA ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 200, "Play TJA(1P)", Layer == (int)ELayer.PlayTJA1P ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 240, "Play TJA(2P)", Layer == (int)ELayer.PlayTJA2P ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 280, "Play BMS", Layer == (int)ELayer.PlayBMS ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 320, "Play BMS(1P)", Layer == (int)ELayer.PlayBMS1P ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 360, "Play BMS(2P)", Layer == (int)ELayer.PlayBMS2P ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 400, "KEY CONFIG", Layer == (int)ELayer.KeyConfig ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 440, "KEY CONFIG(BMS)", Layer == (int)ELayer.BMSKeyConfig ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 480, "SKIN", Layer == (int)ELayer.Skin ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));
            Drawing.Text(100, 520, "Back to Menu", Layer == (int)ELayer.Back ? 0xff0000 : (InLayer ? 0x808080 : 0xffffff));

            if (OptionList != null)
            {
                for (int i = 0; i < OptionList.Count; i++)
                {
                    Option list = OptionList[i];
                    Drawing.Text(300, 120 + 20 * i, $"{list.Name}", !InLayer ? 0x808080 : (Cursor == i ? 0xff0000 : 0xffffff));
                    if (InLayer && Cursor == i)
                    {
                        Drawing.Text(280, 120 + 20 * i, ">", 0xff0000);
                        Drawing.Text(600, 120, $"{list.Info}", 0xffffff);
                        if (list.objAmount() != null)
                        {
                            Drawing.Text(600, 160, $"Now:{list.objAmount()}", Selecting && (list.Type < OptionType.String || list.Type >= OptionType.Key) ? 0xffff00 : 0xffffff);
                            if (Input.IsEnable)
                            {
                                Drawing.Text(632 + Drawing.TextWidth(Input.Text, Input.Position), 160, "|", 0xffff00);
                            }
                        }

                        if ((ELayer)Layer == ELayer.Skin && i >= 3 && i <= 5)
                        {
                            Drawing.Circle(1450, 640, 120, Drawing.Color(OptionList[3].GetIndex(), OptionList[4].GetIndex(), OptionList[5].GetIndex()));
                        }
                    }
                }
            }
            TextDebug.Update();

            #if DEBUG
            Drawing.Text(0, 0, $"Layer:{Layer}", 0xffffff);
            if (InLayer) Drawing.Text(0, 20, $"Cursor:{Cursor}", 0xffffff);
            if (Selecting) Drawing.Text(0, 60, "Selecting...", 0xffffff);

            Drawing.Text(0, 40, $"ListCount:{OptionList.Count}", 0xffffff);
            #endif
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(EKey.Up))
            {
                PushedTimer[0].Start();
            }
            if (Key.IsLeft(EKey.Up))
            {
                PushedTimer[0].Stop();
                PushedTimer[0].Reset();
                PushingTimer[0].Stop();
                PushingTimer[0].Reset();
            }
            if (Key.IsPushed(EKey.Down))
            {
                PushedTimer[1].Start();
            }
            if (Key.IsLeft(EKey.Down))
            {
                PushedTimer[1].Stop();
                PushedTimer[1].Reset();
                PushingTimer[1].Stop();
                PushingTimer[1].Reset();
            }
            for (int i = 0; i < 2; i++)
            {
                if (PushedTimer[i].Value == PushedTimer[i].End)
                {
                    PushingTimer[i].Start();
                }
            }

            if (Key.IsPushing(EKey.Up) && Key.IsPushing(EKey.Down) && Selecting && OptionList != null)
            {
                OptionList[Cursor].Reset();
            }
            else
            {
                if (Key.IsPushed(EKey.Up) || (PushingTimer[0].Value == PushingTimer[0].End))
                {
                    if (Selecting && OptionList != null)
                    {
                        OptionList[Cursor].Up();

                    }
                    else if (InLayer)
                    {
                        if (Cursor-- <= 0) Cursor = OptionList.Count - 1;
                    }
                    else
                    {
                        if (Layer-- <= 0) Layer = (int)ELayer.Back;
                        Change();
                    }
                    PushingTimer[0].Reset();
                }
                if (Key.IsPushed(EKey.Down) || (PushingTimer[1].Value == PushingTimer[1].End))
                {
                    if (Selecting && OptionList != null)
                    {
                        OptionList[Cursor].Down();
                    }
                    else if (InLayer)
                    {
                        if (Cursor++ >= OptionList.Count - 1) Cursor = 0;
                    }
                    else
                    {
                        if (Layer++ >= (int)ELayer.Back) Layer = 0;
                        Change();
                    }
                    PushingTimer[1].Reset();
                }
                for (int i = 0; i < 256; i++)
                {
                    if (Key.IsPushed((EKey)i) && Selecting)
                    {
                        if (OptionList[Cursor].Type == OptionType.Key)
                        {
                            Selecting = false;
                            OptionList[Cursor].Enter();
                            UpdateConfig();
                        }
                        else if (OptionList[Cursor].Type == OptionType.EKey)
                        {
                            OptionList[Cursor].Add();
                        }
                    }
                }
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (Joypad.IsPushed(j, Joypad.GetButtonFromIndex(i)) && Selecting)
                        {
                            if (OptionList[Cursor].Type == OptionType.Pad)
                            {
                                Selecting = false;
                                OptionList[Cursor].Enter();
                                UpdateConfig();
                            }
                        }
                    }
                }
                if (Key.IsPushed(EKey.Enter))
                {
                    if (Selecting)
                    {
                        if (OptionList[Cursor].Type != OptionType.Key)
                        {
                            Selecting = false;
                            if (OptionList[Cursor].Type == OptionType.String || OptionList[Cursor].Type == OptionType.StrList)
                            {
                                OptionList[Cursor].Enter();
                            }
                            UpdateConfig();
                            if (OptionList[Cursor].Name == "PlayFolder") SongLoad.Init();
                        }
                    }
                    else if (InLayer)
                    {
                        if (OptionList[Cursor].Type > OptionType.Bool)
                        {
                            Selecting = true;
                            if (OptionList[Cursor].Type == OptionType.String || OptionList[Cursor].Type == OptionType.StrList)
                            {
                                OptionList[Cursor].Enter();
                            }
                            if (OptionList[Cursor].Type == OptionType.EKey)
                            {
                                OptionList[Cursor].Start();
                            }
                        }
                        else if (Cursor == OptionList.Count - 1)
                        {
                            InLayer = false;
                            Cursor = 0;
                        }
                        else
                        {
                            OptionList[Cursor].Enter();
                            UpdateConfig();
                        }
                    }
                    else
                    {
                        if (Layer == (int)ELayer.Back)
                        {
                            if (Title.ToConfig) Program.SceneChange(new Title());
                            else Program.SceneChange(new SongSelect());
                        }
                        else if (OptionList.Count > 0)
                        {
                            InLayer = true;
                        }
                    }
                }
                if (Key.IsPushed(EKey.Esc))
                {
                    if (Selecting)
                    {
                        Selecting = false;
                        OptionList[Cursor].Reset();
                    }
                    else if (InLayer)
                    {
                        InLayer = false;
                        Cursor = 0;
                    }
                    else
                    {
                        //設定の保存
                        PlayData.End();
                        if (Title.ToConfig) Program.SceneChange(new Title());
                        else Program.SceneChange(new SongSelect());
                    }
                }
            }
            

            for (int i = 0; i < 2; i++)
            {
                PushedTimer[i].Tick();
                PushingTimer[i].Tick();
            }
            base.Update();
        }

        public static void Change()
        {
            if (OptionList != null) OptionList.Clear();
            switch ((ELayer)Layer)
            {
                case ELayer.System:
                    FullScreen = new OptionBool("FullScreen", PlayData.Data.FullScreen, "全画面表示とウィンドウ表示を切り替えます。"); OptionList.Add(FullScreen);
                    PlayerName = new OptionString("PlayerName", PlayData.Data.PlayerName, "スコア等で使用する名前を変更します。"); OptionList.Add(PlayerName);
                    PlayFolder = new OptionStrList("PlayFolder", PlayData.Data.PlayFolder, "読み込むフォルダを変更します。(セミコロン(;)で区切ることにより複数のパスを指定できます。)"); OptionList.Add(PlayFolder);
                    PreviewSong = new OptionBool("PreviewSong", PlayData.Data.PreviewSong, "曲のプレビューを再生します。"); OptionList.Add(PreviewSong);
                    SystemBGM = new OptionInt("Volume-SystemBGM", PlayData.Data.SystemBGM, 0, 100, "システムのBGMの音量を調節します。"); OptionList.Add(SystemBGM);
                    GameBGM = new OptionInt("Volume-GameBGM", PlayData.Data.GameBGM, 0, 100, "演奏中の曲の音量を調節します。"); OptionList.Add(GameBGM);
                    SE = new OptionInt("Volume-SE", PlayData.Data.SE, 0, 100, "効果音の音量を調節します。"); OptionList.Add(SE);
                    VSync = new OptionBool("VSync", PlayData.Data.VSync, "垂直同期を有効にします。(適用するにはソフトを再起動してください)"); OptionList.Add(VSync);
                    ShowImage = new OptionBool("ShowImage", PlayData.Data.ShowImage, "BGIMAGEに記述された画像を背景に表示します。"); OptionList.Add(ShowImage);
                    PlayMovie = new OptionBool("PlayMovie", PlayData.Data.PlayMovie, "BGMOVIEに記述された動画を背景に表示します。"); OptionList.Add(PlayMovie);
                    QuickStart = new OptionBool("QuickStart", PlayData.Data.QuickStart, "自動的に譜面を再生します。"); OptionList.Add(QuickStart);
                    ShowResultScreen = new OptionBool("ShowResultScreen", PlayData.Data.ShowResultScreen, "リザルト画面を表示します。"); OptionList.Add(ShowResultScreen);
                    PlayList = new OptionBool("PlayList", PlayData.Data.PlayList, "プレイが終了したらすぐ次の譜面を再生します。"); OptionList.Add(PlayList);
                    SaveScore = new OptionBool("SaveScore", PlayData.Data.SaveScore, "記録を更新した時に自動的にスコアを保存します。"); OptionList.Add(SaveScore);
                    RandomDanSave = new OptionBool("RandomDanSave", PlayData.Data.RandomDanSave, "ランダム段位で生成した段位を保存します。"); OptionList.Add(RandomDanSave);
                    DanFailedType = new OptionList("DanFailedType", PlayData.Data.DanFailedType, "段位不合格確定時の処理を設定します。",
                        new string[] { "End", "Retry", "Song", "All" }); OptionList.Add(DanFailedType);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayTJA:
                    PreviewType = new OptionList("PreviewType", PlayData.Data.PreviewType, "再生の仕方を変更します。",
                        new string[] { "Normal", "Up", "Down", "AllCourses" }); OptionList.Add(PreviewType);
                    PlaySpeed = new OptionDouble("PlaySpeed", PlayData.Data.PlaySpeed, 0, 1000, "曲の再生速度を変更します。"); OptionList.Add(PlaySpeed);
                    ChangeSESpeed = new OptionBool("ChangeSESpeed", PlayData.Data.ChangeSESpeed, "曲の再生速度に合わせて効果音のピッチを調節します。"); OptionList.Add(ChangeSESpeed);
                    RandomRate = new OptionInt("RandomRate", PlayData.Data.RandomRate, 0, 100, "ランダムの割合を調節します。"); OptionList.Add(RandomRate);
                    AutoRoll = new OptionInt("AutoRoll", PlayData.Data.AutoRoll, 0, 1000, "オート時の連打の秒間打数を設定します。"); OptionList.Add(AutoRoll);
                    JudgeType = new OptionList("JudgeType", PlayData.Data.JudgeType, "ソフトで設定された判定を使用します。",
                        new string[] { "Custom", "Standard", "HardMode" }); OptionList.Add(JudgeType);
                    JudgePerfect = new OptionDouble("JudgePerfect", PlayData.Data.JudgePerfect, 0, 10000, "Perfect の範囲を設定します。"); OptionList.Add(JudgePerfect);
                    JudgeGreat = new OptionDouble("JudgeGreat", PlayData.Data.JudgeGreat, 0, 10000, "Great の範囲を設定します。"); OptionList.Add(JudgeGreat);
                    JudgeGood = new OptionDouble("JudgeGood", PlayData.Data.JudgeGood, 0, 10000, "Good の範囲を設定します。"); OptionList.Add(JudgeGood);
                    JudgeBad = new OptionDouble("JudgeBad", PlayData.Data.JudgeBad, 0, 10000, "Bad の範囲を設定します。"); OptionList.Add(JudgeBad);
                    JudgePoor = new OptionDouble("JudgePoor", PlayData.Data.JudgePoor, 0, 10000, "Poor の範囲を設定します。"); OptionList.Add(JudgePoor);
                    Just = new OptionBool("Just", PlayData.Data.Just, "Goodが強制的にBadになります。"); OptionList.Add(Just);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayTJA1P:
                    PlayCourse = new OptionList("PlayCourse", PlayData.Data.PlayCourse[0], "再生する難易度を変更します。",
                        new string[] { "Easy", "Normal", "Hard", "Oni", "Edit" }); OptionList.Add(PlayCourse);
                    Random = new OptionBool("Random", PlayData.Data.Random[0], "ノーツの色をランダムに変更します。"); OptionList.Add(Random);
                    Mirror = new OptionBool("Mirror", PlayData.Data.Mirror[0], "ノーツの色を反転します。"); OptionList.Add(Mirror);
                    Stelth = new OptionBool("Stelth", PlayData.Data.Stelth[0], "ノーツを見えなくします。"); OptionList.Add(Stelth);
                    NotesChange = new OptionList("NotesChange", PlayData.Data.NotesChange[0], "ノーツの色を単色にします。",
                        new string[] { "None", "RedOnly", "BlueOnly", "AllRed", "AllBlue" }); OptionList.Add(NotesChange);
                    ScrollType = new OptionList("ScrollType", PlayData.Data.ScrollType[0], "譜面が特殊な動きをするようになります。",
                        new string[] { "Normal", "BMSCROLL", "HBSCROLL" }); OptionList.Add(ScrollType);
                    ScrollSpeed = new OptionDouble("ScrollSpeed", PlayData.Data.ScrollSpeed[0], 0, 255, "譜面の流れる速さを変更します。"); OptionList.Add(ScrollSpeed);
                    UseSudden = new OptionBool("UseSudden", PlayData.Data.UseSudden[0], "画面の一部を隠します。"); OptionList.Add(UseSudden);
                    SuddenNumber = new OptionInt("SuddenNumber", PlayData.Data.SuddenNumber[0], 0, 1000, "Suddenの幅を変更します。"); OptionList.Add(SuddenNumber);
                    FloatingHiSpeed = new OptionBool("FloatingHiSpeed", PlayData.Data.FloatingHiSpeed[0], "見える時間(緑数字)を固定します。"); OptionList.Add(FloatingHiSpeed);
                    GreenNumber = new OptionInt("GreenNumber", PlayData.Data.GreenNumber[0], 0, 10000, "FHS時に固定する緑数字を設定します。"); OptionList.Add(GreenNumber);
                    NormalHiSpeed = new OptionBool("NormalHiSpeed", PlayData.Data.NormalHiSpeed[0], "決まった緑数字に合わせてスクロールを調節します。"); OptionList.Add(NormalHiSpeed);
                    NHSSpeed = new OptionInt("NHSSpeed", PlayData.Data.NHSSpeed[0], 0, 20, "NHS時のスクロールを設定します。"); OptionList.Add(NHSSpeed);
                    Auto = new OptionBool("Auto", PlayData.Data.Auto[0], "自動で譜面をプレイします。"); OptionList.Add(Auto);
                    GaugeType = new OptionList("GaugeType", PlayData.Data.GaugeType[0], "ゲージの種類を変更します。",
                        new string[] { "Normal", "AssistEasy", "Easy", "Hard", "EX-Hard" }); OptionList.Add(GaugeType);
                    GaugeAutoShift = new OptionList("GaugeAutoShift", PlayData.Data.GaugeAutoShift[0], "ゲージを自動的に変更したりする機能です。",
                        new string[] { "None", "Continue", "Retry", "ToNormal", "Best", "LessBest" }); OptionList.Add(GaugeAutoShift);
                    GaugeAutoShiftMin = new OptionList("GaugeAutoShiftMin", PlayData.Data.GaugeAutoShiftMin[0], "Bestなどで下がる際のゲージの下限を変更します。",
                        new string[] { "Normal", "AssistEasy", "Easy" }); OptionList.Add(GaugeAutoShiftMin);
                    Hazard = new OptionInt("Hazard", PlayData.Data.Hazard[0], 0, 10000, "指定個数BadやPoorを出すと終了します。(1以上で有効)"); OptionList.Add(Hazard);
                    InputAdjust = new OptionDouble("InputAdjust", PlayData.Data.InputAdjust[0], -10000, 10000, "判定の位置を調整します。"); OptionList.Add(InputAdjust);
                    AutoAdjust = new OptionBool("AutoAdjust", PlayData.Data.AutoAdjust[0], "自動で判定の位置を調整する機能です。"); OptionList.Add(AutoAdjust);
                    ShowGraph = new OptionBool("ShowGraph", PlayData.Data.ShowGraph, "自分やライバルの記録と比較できるグラフを表示します。"); OptionList.Add(ShowGraph);
                    ShowBestScore = new OptionBool("ShowBestScore", PlayData.Data.ShowBestScore, "自己ベスト記録のグラフを表示します。(重い場合は非表示にしてください)"); OptionList.Add(ShowBestScore);
                    RivalType = new OptionList("RivalType", PlayData.Data.RivalType, "ライバルを設定します。",
                        new string[] { "None", "Percent", "Rank", "PlayScore" }); OptionList.Add(RivalType);
                    RivalPercent = new OptionDouble("RivalPercent", PlayData.Data.RivalPercent, 0, 100, "Percent 使用時の目標スコアを設定します。"); OptionList.Add(RivalPercent);
                    RivalRank = new OptionList("RivalRank", PlayData.Data.RivalRank, "Rank 使用時の目標スコアを設定します。",
                        new string[] { "F", "E", "D", "C", "B", "A", "AA", "AAA", "MAX" }); OptionList.Add(RivalRank);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayTJA2P:
                    IsPlay2P = new OptionBool("Play2P", PlayData.Data.IsPlay2P, "2Pを使用します。"); OptionList.Add(IsPlay2P);
                    PlayCourse2P = new OptionList("PlayCourse", PlayData.Data.PlayCourse[1], "再生する難易度を変更します。",
                        new string[] { "Easy", "Normal", "Hard", "Oni", "Edit" }); OptionList.Add(PlayCourse2P);
                    Random2P = new OptionBool("Random", PlayData.Data.Random[1], "ノーツの色をランダムに変更します。"); OptionList.Add(Random2P);
                    Mirror2P = new OptionBool("Mirror", PlayData.Data.Mirror[1], "ノーツの色を反転します。"); OptionList.Add(Mirror2P);
                    Stelth2P = new OptionBool("Stelth", PlayData.Data.Stelth[1], "ノーツを見えなくします。"); OptionList.Add(Stelth2P);
                    NotesChange2P = new OptionList("NotesChange", PlayData.Data.NotesChange[1], "ノーツの色を単色にします。",
                        new string[] { "None", "RedOnly", "BlueOnly", "AllRed", "AllBlue" }); OptionList.Add(NotesChange2P);
                    ScrollType2P = new OptionList("ScrollType", PlayData.Data.ScrollType[1], "譜面が特殊な動きをするようになります。",
                        new string[] { "Normal", "BMSCROLL", "HBSCROLL" }); OptionList.Add(ScrollType2P);
                    ScrollSpeed2P = new OptionDouble("ScrollSpeed", PlayData.Data.ScrollSpeed[1], 0, 255, "譜面の流れる速さを変更します。"); OptionList.Add(ScrollSpeed2P);
                    UseSudden2P = new OptionBool("UseSudden", PlayData.Data.UseSudden[1], "画面の一部を隠します。"); OptionList.Add(UseSudden2P);
                    SuddenNumber2P = new OptionInt("SuddenNumber", PlayData.Data.SuddenNumber[1], 0, 1000, "Suddenの幅を変更します。"); OptionList.Add(SuddenNumber2P);
                    FloatingHiSpeed2P = new OptionBool("FloatingHiSpeed", PlayData.Data.FloatingHiSpeed[1], "見える時間(緑数字)を固定します。"); OptionList.Add(FloatingHiSpeed2P);
                    GreenNumber2P = new OptionInt("GreenNumber", PlayData.Data.GreenNumber[1], 0, 10000, "FHS時に固定する緑数字を設定します。"); OptionList.Add(GreenNumber2P);
                    NormalHiSpeed2P = new OptionBool("NormalHiSpeed", PlayData.Data.NormalHiSpeed[1], "決まった緑数字に合わせてスクロールを調節します。"); OptionList.Add(NormalHiSpeed2P);
                    NHSSpeed2P = new OptionInt("NHSSpeed", PlayData.Data.NHSSpeed[1], 0, 20, "NHS時のスクロールを設定します。"); OptionList.Add(NHSSpeed2P);
                    Auto2P = new OptionBool("Auto", PlayData.Data.Auto[1], "自動で譜面をプレイします。"); OptionList.Add(Auto2P);
                    GaugeType2P = new OptionList("GaugeType", PlayData.Data.GaugeType[1], "ゲージの種類を変更します。",
                        new string[] { "Normal", "AssistEasy", "Easy", "Hard", "EX-Hard" }); OptionList.Add(GaugeType2P);
                    GaugeAutoShift2P = new OptionList("GaugeAutoShift", PlayData.Data.GaugeAutoShift[1], "ゲージを自動的に変更したりする機能です。",
                        new string[] { "None", "Continue", "Retry", "ToNormal", "Best", "LessBest" }); OptionList.Add(GaugeAutoShift2P);
                    GaugeAutoShiftMin2P = new OptionList("GaugeAutoShiftMin", PlayData.Data.GaugeAutoShiftMin[1], "Bestなどで下がる際のゲージの下限を変更します。",
                        new string[] { "Normal", "AssistEasy", "Easy" }); OptionList.Add(GaugeAutoShiftMin2P);
                    Hazard2P = new OptionInt("Hazard", PlayData.Data.Hazard[1], 0, 10000, "指定個数BadやPoorを出すと終了します。(1以上で有効)"); OptionList.Add(Hazard2P);
                    InputAdjust2P = new OptionDouble("InputAdjust", PlayData.Data.InputAdjust[1], -10000, 10000, "判定の位置を調整します。"); OptionList.Add(InputAdjust2P);
                    AutoAdjust2P = new OptionBool("AutoAdjust", PlayData.Data.AutoAdjust[1], "自動で判定の位置を調整する機能です。"); OptionList.Add(AutoAdjust2P);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayBMS:
                    LNType = new OptionList("LNType", PlayData.Data.LNType, "ロングノーツの種類を設定します。",
                        new string[] { "LN", "CN", "HCN" }); OptionList.Add(LNType);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayBMS1P:
                    Is2PSide = new OptionBool("Is2PSide", PlayData.Data.Is2PSide, "2P側でプレイします。"); OptionList.Add(Is2PSide);
                    Option = new OptionList("Option", PlayData.Data.Option[0], "ノーツの配置を変更します。",
                        new string[] { "None", "Mirror", "Random", "R-Random", "S-Random", "All-Scr" }); OptionList.Add(Option);
                    BMSScrollSpeed = new OptionDouble("ScrollSpeed", PlayData.Data.BMSScrollSpeed[0], 0, 255, "譜面の流れる速さを変更します。"); OptionList.Add(BMSScrollSpeed);
                    BMSUseSudden = new OptionBool("UseSudden", PlayData.Data.BMSUseSudden[0], "画面の一部を隠します。"); OptionList.Add(BMSUseSudden);
                    BMSSuddenNumber = new OptionInt("SuddenNumber", PlayData.Data.BMSSuddenNumber[0], 0, 1000, "Suddenの幅を変更します。"); OptionList.Add(BMSSuddenNumber);
                    BMSFloatingHiSpeed = new OptionBool("FloatingHiSpeed", PlayData.Data.BMSFloatingHiSpeed[0], "見える時間(緑数字)を固定します。"); OptionList.Add(BMSFloatingHiSpeed);
                    BMSGreenNumber = new OptionInt("GreenNumber", PlayData.Data.BMSGreenNumber[0], 0, 10000, "FHS時に固定する緑数字を設定します。"); OptionList.Add(BMSGreenNumber);
                    BMSNormalHiSpeed = new OptionBool("NormalHiSpeed", PlayData.Data.BMSNormalHiSpeed[0], "決まった緑数字に合わせてスクロールを調節します。"); OptionList.Add(BMSNormalHiSpeed);
                    BMSNHSSpeed = new OptionInt("NHSSpeed", PlayData.Data.BMSNHSSpeed[0], 0, 20, "NHS時のスクロールを設定します。"); OptionList.Add(BMSNHSSpeed);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayBMS2P:
                    Option2P = new OptionList("Option", PlayData.Data.Option[1], "ノーツの配置を変更します。",
                        new string[] { "None", "Mirror", "Random", "R-Random", "S-Random", "All-Scr" }); OptionList.Add(Option2P);
                    BMSScrollSpeed2P = new OptionDouble("ScrollSpeed", PlayData.Data.BMSScrollSpeed[1], 0, 255, "譜面の流れる速さを変更します。"); OptionList.Add(BMSScrollSpeed2P);
                    BMSUseSudden2P = new OptionBool("UseSudden", PlayData.Data.BMSUseSudden[1], "画面の一部を隠します。"); OptionList.Add(BMSUseSudden2P);
                    BMSSuddenNumber2P = new OptionInt("SuddenNumber", PlayData.Data.BMSSuddenNumber[1], 0, 1000, "Suddenの幅を変更します。"); OptionList.Add(BMSSuddenNumber2P);
                    BMSFloatingHiSpeed2P = new OptionBool("FloatingHiSpeed", PlayData.Data.BMSFloatingHiSpeed[1], "見える時間(緑数字)を固定します。"); OptionList.Add(BMSFloatingHiSpeed2P);
                    BMSGreenNumber2P = new OptionInt("GreenNumber", PlayData.Data.BMSGreenNumber[1], 0, 10000, "FHS時に固定する緑数字を設定します。"); OptionList.Add(BMSGreenNumber2P);
                    BMSNormalHiSpeed2P = new OptionBool("NormalHiSpeed", PlayData.Data.BMSNormalHiSpeed[1], "決まった緑数字に合わせてスクロールを調節します。"); OptionList.Add(BMSNormalHiSpeed2P);
                    BMSNHSSpeed2P = new OptionInt("NHSSpeed", PlayData.Data.BMSNHSSpeed[1], 0, 20, "NHS時のスクロールを設定します。"); OptionList.Add(BMSNHSSpeed2P);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.BMSKeyConfig:
                    Key1 = new OptionKey("1Key", PlayData.Data.BMSKey.Key1, "[BMS] 1Pの1鍵"); OptionList.Add(Key1);
                    Key2 = new OptionKey("2Key", PlayData.Data.BMSKey.Key2, "[BMS] 1Pの2鍵"); OptionList.Add(Key2);
                    Key3 = new OptionKey("3Key", PlayData.Data.BMSKey.Key3, "[BMS] 1Pの3鍵"); OptionList.Add(Key3);
                    Key4 = new OptionKey("4Key", PlayData.Data.BMSKey.Key4, "[BMS] 1Pの4鍵"); OptionList.Add(Key4);
                    Key5 = new OptionKey("5Key", PlayData.Data.BMSKey.Key5, "[BMS] 1Pの5鍵"); OptionList.Add(Key5);
                    Key6 = new OptionKey("6Key", PlayData.Data.BMSKey.Key6, "[BMS] 1Pの6鍵"); OptionList.Add(Key6);
                    Key7 = new OptionKey("7Key", PlayData.Data.BMSKey.Key7, "[BMS] 1Pの7鍵"); OptionList.Add(Key7);
                    Scr_F = new OptionKey("Scr_F", PlayData.Data.BMSKey.Scr_F, "[BMS] 1Pの皿(押し)"); OptionList.Add(Scr_F);
                    Scr_R = new OptionKey("Scr_R", PlayData.Data.BMSKey.Scr_R, "[BMS] 1Pの皿(引き)"); OptionList.Add(Scr_R);
                    Start = new OptionKey("Start", PlayData.Data.BMSKey.Start, "[BMS] 1Pのスタートボタン"); OptionList.Add(Start);
                    Select = new OptionKey("Select", PlayData.Data.BMSKey.Select, "[BMS] 1Pのセレクトボタン"); OptionList.Add(Select);

                    Controller = new OptionInt("Controller", PlayData.Data.Controller[0], 0, 3, "1Pで使用するコントローラー"); OptionList.Add(Controller);
                    PKey1 = new OptionPad("Pad_1Key", PlayData.Data.BMSKey.Pad_Key1, "[BMS] 1Pの1鍵", 0); OptionList.Add(PKey1);
                    PKey2 = new OptionPad("Pad_2Key", PlayData.Data.BMSKey.Pad_Key2, "[BMS] 1Pの2鍵", 0); OptionList.Add(PKey2);
                    PKey3 = new OptionPad("Pad_3Key", PlayData.Data.BMSKey.Pad_Key3, "[BMS] 1Pの3鍵", 0); OptionList.Add(PKey3);
                    PKey4 = new OptionPad("Pad_4Key", PlayData.Data.BMSKey.Pad_Key4, "[BMS] 1Pの4鍵", 0); OptionList.Add(PKey4);
                    PKey5 = new OptionPad("Pad_5Key", PlayData.Data.BMSKey.Pad_Key5, "[BMS] 1Pの5鍵", 0); OptionList.Add(PKey5);
                    PKey6 = new OptionPad("Pad_6Key", PlayData.Data.BMSKey.Pad_Key6, "[BMS] 1Pの6鍵", 0); OptionList.Add(PKey6);
                    PKey7 = new OptionPad("Pad_7Key", PlayData.Data.BMSKey.Pad_Key7, "[BMS] 1Pの7鍵", 0); OptionList.Add(PKey7);
                    PScr_F = new OptionPad("Pad_Scr_F", PlayData.Data.BMSKey.Pad_Scr_F, "[BMS] 1Pの皿(押し)", 0); OptionList.Add(PScr_F);
                    PScr_R = new OptionPad("Pad_Scr_R", PlayData.Data.BMSKey.Pad_Scr_R, "[BMS] 1Pの皿(引き)", 0); OptionList.Add(PScr_R);
                    PStart = new OptionPad("Pad_Start", PlayData.Data.BMSKey.Pad_Start, "[BMS] 1Pのスタートボタン", 0); OptionList.Add(PStart);
                    PSelect = new OptionPad("Pad_Select", PlayData.Data.BMSKey.Pad_Select, "[BMS] 1Pのセレクトボタン", 0); OptionList.Add(PSelect);

                    Key1_2P = new OptionKey("1Key_2P", PlayData.Data.BMSKey2P.Key1, "[BMS] 2Pの1鍵"); OptionList.Add(Key1_2P);
                    Key2_2P = new OptionKey("2Key_2P", PlayData.Data.BMSKey2P.Key2, "[BMS] 2Pの2鍵"); OptionList.Add(Key2_2P);
                    Key3_2P = new OptionKey("3Key_2P", PlayData.Data.BMSKey2P.Key3, "[BMS] 2Pの3鍵"); OptionList.Add(Key3_2P);
                    Key4_2P = new OptionKey("4Key_2P", PlayData.Data.BMSKey2P.Key4, "[BMS] 2Pの4鍵"); OptionList.Add(Key4_2P);
                    Key5_2P = new OptionKey("5Key_2P", PlayData.Data.BMSKey2P.Key5, "[BMS] 2Pの5鍵"); OptionList.Add(Key5_2P);
                    Key6_2P = new OptionKey("6Key_2P", PlayData.Data.BMSKey2P.Key6, "[BMS] 2Pの6鍵"); OptionList.Add(Key6_2P);
                    Key7_2P = new OptionKey("7Key_2P", PlayData.Data.BMSKey2P.Key7, "[BMS] 2Pの7鍵"); OptionList.Add(Key7_2P);
                    Scr_F_2P = new OptionKey("Scr_F_2P", PlayData.Data.BMSKey2P.Scr_F, "[BMS] 2Pの皿(押し)"); OptionList.Add(Scr_F_2P);
                    Scr_R_2P = new OptionKey("Scr_R_2P", PlayData.Data.BMSKey2P.Scr_R, "[BMS] 2Pの皿(引き)"); OptionList.Add(Scr_R_2P);
                    Start_2P = new OptionKey("Start_2P", PlayData.Data.BMSKey2P.Start, "[BMS] 2Pのスタートボタン"); OptionList.Add(Start_2P);
                    Select_2P = new OptionKey("Select_2P", PlayData.Data.BMSKey2P.Select, "[BMS] 2Pのセレクトボタン"); OptionList.Add(Select_2P);

                    Controller2P = new OptionInt("Controller_2P", PlayData.Data.Controller[1], 0, 3, "2Pで使用するコントローラー"); OptionList.Add(Controller2P);
                    PKey1_2P = new OptionPad("Pad_1Key_2P", PlayData.Data.BMSKey2P.Pad_Key1, "[BMS] 2Pの1鍵", 1); OptionList.Add(PKey1_2P);
                    PKey2_2P = new OptionPad("Pad_2Key_2P", PlayData.Data.BMSKey2P.Pad_Key2, "[BMS] 2Pの2鍵", 1); OptionList.Add(PKey2_2P);
                    PKey3_2P = new OptionPad("Pad_3Key_2P", PlayData.Data.BMSKey2P.Pad_Key3, "[BMS] 2Pの3鍵", 1); OptionList.Add(PKey3_2P);
                    PKey4_2P = new OptionPad("Pad_4Key_2P", PlayData.Data.BMSKey2P.Pad_Key4, "[BMS] 2Pの4鍵", 1); OptionList.Add(PKey4_2P);
                    PKey5_2P = new OptionPad("Pad_5Key_2P", PlayData.Data.BMSKey2P.Pad_Key5, "[BMS] 2Pの5鍵", 1); OptionList.Add(PKey5_2P);
                    PKey6_2P = new OptionPad("Pad_6Key_2P", PlayData.Data.BMSKey2P.Pad_Key6, "[BMS] 2Pの6鍵", 1); OptionList.Add(PKey6_2P);
                    PKey7_2P = new OptionPad("Pad_7Key_2P", PlayData.Data.BMSKey2P.Pad_Key7, "[BMS] 2Pの7鍵", 1); OptionList.Add(PKey7_2P);
                    PScr_F_2P = new OptionPad("Pad_Scr_F_2P", PlayData.Data.BMSKey2P.Pad_Scr_F, "[BMS] 2Pの皿(押し)", 1); OptionList.Add(PScr_F_2P);
                    PScr_R_2P = new OptionPad("Pad_Scr_R_2P", PlayData.Data.BMSKey2P.Pad_Scr_R, "[BMS] 2Pの皿(引き)", 1); OptionList.Add(PScr_R_2P);
                    PStart_2P = new OptionPad("Pad_Start_2P", PlayData.Data.BMSKey2P.Pad_Start, "[BMS] 2Pのスタートボタン", 1); OptionList.Add(PStart_2P);
                    PSelect_2P = new OptionPad("Pad_Select_2P", PlayData.Data.BMSKey2P.Pad_Select, "[BMS] 2Pのセレクトボタン", 1); OptionList.Add(PSelect_2P);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.KeyConfig:
                    LeftDon = new OptionEKey("LeftDon", PlayData.Data.LEFTDON, "[TJA] 1P左側の面"); OptionList.Add(LeftDon);
                    RightDon = new OptionEKey("RightDon", PlayData.Data.RIGHTDON, "[TJA] 1P右側の面"); OptionList.Add(RightDon);
                    LeftKa = new OptionEKey("LeftKa", PlayData.Data.LEFTKA, "[TJA] 1P左側の縁"); OptionList.Add(LeftKa);
                    RightKa = new OptionEKey("RightKa", PlayData.Data.RIGHTKA, "[TJA] 1P右側の縁"); OptionList.Add(RightKa);
                    LeftDon2P = new OptionEKey("LeftDon2P", PlayData.Data.LEFTDON2P, "[TJA] 2P左側の面"); OptionList.Add(LeftDon2P);
                    RightDon2P = new OptionEKey("RightDon2P", PlayData.Data.RIGHTDON2P, "[TJA] 2P右側の面"); OptionList.Add(RightDon2P);
                    LeftKa2P = new OptionEKey("LeftKa2P", PlayData.Data.LEFTKA2P, "[TJA] 2P左側の縁"); OptionList.Add(LeftKa2P);
                    RightKa2P = new OptionEKey("RightKa2P", PlayData.Data.RIGHTKA2P, "[TJA] 2P右側の縁"); OptionList.Add(RightKa2P);

                    ScreenShot = new OptionKey("ScreenShot", PlayData.Data.ScreenShot, "[共通] スクリーンショットを撮影する"); OptionList.Add(ScreenShot);
                    MoveConfig = new OptionKey("MoveConfig", PlayData.Data.MoveConfig, "[タイトル・選曲] この画面に移行する"); OptionList.Add(MoveConfig);
                    OpenOption = new OptionKey("OpenOption", PlayData.Data.OpenOption, "[選曲] オプションを開く"); OptionList.Add(OpenOption);
                    PlayStart = new OptionKey("PlayStart", PlayData.Data.PlayStart, "[プレイ] 譜面の再生を開始する"); OptionList.Add(PlayStart);
                    PlayReset = new OptionKey("PlayReset", PlayData.Data.PlayReset, "[プレイ] 譜面を停止し再読み込みする"); OptionList.Add(PlayReset);
                    MeasureUp = new OptionKey("MeasureUp", PlayData.Data.MeasureUp, "[プレイ] 小節を1つ進める"); OptionList.Add(MeasureUp);
                    MeasureDown = new OptionKey("MeasureDown", PlayData.Data.MeasureDown, "[プレイ] 小節を1つ戻す"); OptionList.Add(MeasureDown);
                    JunpHome = new OptionKey("JunpHome", PlayData.Data.JunpHome, "[プレイ] 最初の小節に戻る"); OptionList.Add(JunpHome);
                    JunpEnd = new OptionKey("JunpEnd", PlayData.Data.JunpEnd, "[プレイ] 最後の小節に進む"); OptionList.Add(JunpEnd);
                    ChangeAuto = new OptionKey("ChangeAuto", PlayData.Data.ChangeAuto, "[プレイ] オートのON/OFFを切り替える"); OptionList.Add(ChangeAuto);
                    DisplaySudden = new OptionKey("DisplaySudden", PlayData.Data.DisplaySudden, "[プレイ] Suddenの情報を表示、2回押しでSuddenを表示/非表示する"); OptionList.Add(DisplaySudden);
                    SuddenPlus = new OptionKey("SuddenPlus", PlayData.Data.SuddenPlus, "[プレイ] 隠す範囲を広げる"); OptionList.Add(SuddenPlus);
                    SuddenMinus = new OptionKey("SuddenMinus", PlayData.Data.SuddenMinus, "[プレイ] 隠す範囲を狭める"); OptionList.Add(SuddenMinus);
                    ChangeFHS = new OptionKey("ChangeFHS", PlayData.Data.ChangeFHS, "[プレイ] フローティングハイスピードを切り替える"); OptionList.Add(ChangeFHS);
                    ChangeAuto2P = new OptionKey("ChangeAuto2P", PlayData.Data.ChangeAuto2P, "[プレイ] 2PのオートのON/OFFを切り替える"); OptionList.Add(ChangeAuto2P);
                    DisplaySudden2P = new OptionKey("DisplaySudden2P", PlayData.Data.DisplaySudden2P, "[プレイ] 2PのSuddenの情報を表示、2回押しでSuddenを表示/非表示する"); OptionList.Add(DisplaySudden2P);
                    SuddenPlus2P = new OptionKey("SuddenPlus2P", PlayData.Data.SuddenPlus2P, "[プレイ] 2Pの隠す範囲を広げる"); OptionList.Add(SuddenPlus2P);
                    SuddenMinus2P = new OptionKey("SuddenMinus2P", PlayData.Data.SuddenMinus2P, "[プレイ] 2Pの隠す範囲を狭める"); OptionList.Add(SuddenMinus2P);
                    ChangeFHS2P = new OptionKey("ChangeFHS2P", PlayData.Data.ChangeFHS2P, "[プレイ] 2Pのフローティングハイスピードを切り替える"); OptionList.Add(ChangeFHS2P);
                    SaveReplay = new OptionKey("SaveReplay", PlayData.Data.SaveReplay, "[プレイ・リザルト] プレイデータを保存する"); OptionList.Add(SaveReplay);
                    MoveCreate = new OptionKey("MoveCreate", PlayData.Data.MoveCreate, "[プレイ] 編集モードを起動する(開発中)"); OptionList.Add(MoveCreate);
                    InfoMenu = new OptionKey("InfoMenu", PlayData.Data.InfoMenu, "[編集] 譜面情報を編集する"); OptionList.Add(InfoMenu);
                    AddMeasure = new OptionKey("AddMeasure", PlayData.Data.AddMeasure, "[編集] 小節を追加する"); OptionList.Add(AddMeasure);
                    AddCommand = new OptionKey("AddCommand", PlayData.Data.AddCommand, "[編集] 命令文を追加する"); OptionList.Add(AddCommand);
                    OpenTenplate = new OptionKey("OpenTenplate", PlayData.Data.OpenTenplate, "[編集] テンプレートに登録したテキストを追加する"); OptionList.Add(OpenTenplate);
                    RealTimeMapping = new OptionKey("RealTimeMapping", PlayData.Data.RealTimeMapping, "[編集] 押されたキーで譜面を追加する"); OptionList.Add(RealTimeMapping);
                    SaveFile = new OptionKey("SaveFile", PlayData.Data.SaveFile, "[編集] 譜面を保存する"); OptionList.Add(SaveFile);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.Skin:
                    SkinName = new OptionString("SkinName", PlayData.Data.SkinName, "使用する画像のフォルダを変更します。"); OptionList.Add(SkinName);
                    SoundName = new OptionString("SoundName", PlayData.Data.SoundName, "使用する効果音のフォルダを変更します。"); OptionList.Add(SoundName);
                    BGMName = new OptionString("BGMName", PlayData.Data.BGMName, "使用するBGMのフォルダを変更します。"); OptionList.Add(BGMName);
                    SkinColorR = new OptionInt("SkinColor - R", PlayData.Data.SkinColor[0], 0, 255, "スキンの色を変更します。(Red)"); OptionList.Add(SkinColorR);
                    SkinColorG = new OptionInt("SkinColor - G", PlayData.Data.SkinColor[1], 0, 255, "スキンの色を変更します。(Green)"); OptionList.Add(SkinColorG);
                    SkinColorB = new OptionInt("SkinColor - B", PlayData.Data.SkinColor[2], 0, 255, "スキンの色を変更します。(Blue)"); OptionList.Add(SkinColorB);
                    FontRendering = new OptionBool("FontRendering", PlayData.Data.FontRendering, "文字を指定したフォントで読み込みます。(エラーが出る場合はOFFにしてください)"); OptionList.Add(FontRendering);
                    FontName = new OptionString("FontName", PlayData.Data.FontName, "使用するフォントを変更します。(空白の場合デフォルトの設定を使用します。)"); OptionList.Add(FontName);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
            }
        }

        public static void UpdateConfig()
        {
            switch ((ELayer)Layer)
            {
                case ELayer.System:
                    PlayData.Data.FullScreen = FullScreen.ON;
                    PlayData.Data.PlayerName = PlayerName.Text;
                    ChangeWindowMode(PlayData.Data.FullScreen ? FALSE : TRUE);  //ウィンドウモード切替
                    Tx.Init();
                    PlayData.Data.PreviewSong = PreviewSong.ON;
                    PlayData.Data.SystemBGM = SystemBGM.Value;
                    PlayData.Data.GameBGM = GameBGM.Value;
                    PlayData.Data.SE = SE.Value;
                    PlayData.Data.VSync = VSync.ON;
                    PlayData.Data.PlayFolder = PlayFolder.Text;
                    PlayData.Data.ShowImage = ShowImage.ON;
                    PlayData.Data.PlayMovie = PlayMovie.ON;
                    PlayData.Data.QuickStart = QuickStart.ON;
                    PlayData.Data.ShowResultScreen = ShowResultScreen.ON;
                    PlayData.Data.PlayList = PlayList.ON;
                    PlayData.Data.SaveScore = SaveScore.ON;
                    PlayData.Data.RandomDanSave = RandomDanSave.ON;
                    PlayData.Data.DanFailedType = DanFailedType.Value;
                    break;
                case ELayer.PlayTJA:
                    PlayData.Data.PreviewType = PreviewType.Value;
                    PlayData.Data.PlaySpeed = PlaySpeed.Value;
                    PlayData.Data.ChangeSESpeed = ChangeSESpeed.ON;
                    SoundPitch();
                    PlayData.Data.RandomRate = RandomRate.Value;
                    PlayData.Data.AutoRoll = AutoRoll.Value;
                    PlayData.Data.JudgeType = JudgeType.Value;
                    PlayData.Data.JudgePerfect = JudgePerfect.Value;
                    PlayData.Data.JudgeGreat = JudgeGreat.Value;
                    PlayData.Data.JudgeGood = JudgeGood.Value;
                    PlayData.Data.JudgeBad = JudgeBad.Value;
                    PlayData.Data.JudgePoor = JudgePoor.Value;
                    PlayData.Data.Just = Just.ON;
                    break;
                case ELayer.PlayTJA1P:
                    PlayData.Data.PlayCourse[0] = PlayCourse.Value;
                    PlayData.Data.Random[0] = Random.ON;
                    PlayData.Data.Mirror[0] = Mirror.ON;
                    PlayData.Data.Stelth[0] = Stelth.ON;
                    PlayData.Data.NotesChange[0] = NotesChange.Value;
                    PlayData.Data.ScrollType[0] = ScrollType.Value;
                    PlayData.Data.ScrollSpeed[0] = ScrollSpeed.Value;
                    PlayData.Data.UseSudden[0] = UseSudden.ON;
                    PlayData.Data.SuddenNumber[0] = SuddenNumber.Value;
                    PlayData.Data.FloatingHiSpeed[0] = FloatingHiSpeed.ON;
                    PlayData.Data.GreenNumber[0] = GreenNumber.Value;
                    PlayData.Data.NormalHiSpeed[0] = NormalHiSpeed.ON;
                    PlayData.Data.NHSSpeed[0] = NHSSpeed.Value;
                    PlayData.Data.Auto[0] = Auto.ON;
                    PlayData.Data.GaugeType[0] = GaugeType.Value;
                    PlayData.Data.GaugeAutoShift[0] = GaugeAutoShift.Value;
                    PlayData.Data.GaugeAutoShiftMin[0] = GaugeAutoShiftMin.Value;
                    PlayData.Data.Hazard[0] = Hazard.Value;
                    PlayData.Data.InputAdjust[0] = InputAdjust.Value;
                    PlayData.Data.AutoAdjust[0] = AutoAdjust.ON;
                    PlayData.Data.ShowGraph = ShowGraph.ON;
                    PlayData.Data.ShowBestScore = ShowBestScore.ON;
                    PlayData.Data.RivalType = RivalType.Value;
                    PlayData.Data.RivalPercent = RivalPercent.Value;
                    PlayData.Data.RivalRank = RivalRank.Value;
                    break;
                case ELayer.PlayTJA2P:
                    PlayData.Data.IsPlay2P = IsPlay2P.ON;
                    PlayData.Data.PlayCourse[1] = PlayCourse2P.Value;
                    PlayData.Data.Random[1] = Random2P.ON;
                    PlayData.Data.Mirror[1] = Mirror2P.ON;
                    PlayData.Data.Stelth[1] = Stelth2P.ON;
                    PlayData.Data.NotesChange[1] = NotesChange2P.Value;
                    PlayData.Data.ScrollType[1] = ScrollType2P.Value;
                    PlayData.Data.ScrollSpeed[1] = ScrollSpeed2P.Value;
                    PlayData.Data.UseSudden[1] = UseSudden2P.ON;
                    PlayData.Data.SuddenNumber[1] = SuddenNumber2P.Value;
                    PlayData.Data.FloatingHiSpeed[1] = FloatingHiSpeed2P.ON;
                    PlayData.Data.GreenNumber[1] = GreenNumber2P.Value;
                    PlayData.Data.NormalHiSpeed[1] = NormalHiSpeed2P.ON;
                    PlayData.Data.NHSSpeed[1] = NHSSpeed2P.Value;
                    PlayData.Data.Auto[1] = Auto2P.ON;
                    PlayData.Data.GaugeType[1] = GaugeType2P.Value;
                    PlayData.Data.GaugeAutoShift[1] = GaugeAutoShift2P.Value;
                    PlayData.Data.GaugeAutoShiftMin[1] = GaugeAutoShiftMin2P.Value;
                    PlayData.Data.Hazard[1] = Hazard2P.Value;
                    PlayData.Data.InputAdjust[1] = InputAdjust2P.Value;
                    PlayData.Data.AutoAdjust[1] = AutoAdjust2P.ON;
                    break;
                case ELayer.PlayBMS:
                    PlayData.Data.LNType = LNType.Value;
                    break;
                case ELayer.PlayBMS1P:
                    PlayData.Data.Is2PSide = Is2PSide.ON;
                    PlayData.Data.Option[0] = Option.Value;
                    PlayData.Data.BMSScrollSpeed[0] = BMSScrollSpeed.Value;
                    PlayData.Data.BMSUseSudden[0] = BMSUseSudden.ON;
                    PlayData.Data.BMSSuddenNumber[0] = BMSSuddenNumber.Value;
                    PlayData.Data.BMSFloatingHiSpeed[0] = BMSFloatingHiSpeed.ON;
                    PlayData.Data.BMSGreenNumber[0] = BMSGreenNumber.Value;
                    PlayData.Data.BMSNormalHiSpeed[0] = BMSNormalHiSpeed.ON;
                    PlayData.Data.BMSNHSSpeed[0] = BMSNHSSpeed.Value;
                    break;
                case ELayer.PlayBMS2P:
                    PlayData.Data.Option[1] = Option2P.Value;
                    PlayData.Data.BMSScrollSpeed[1] = BMSScrollSpeed2P.Value;
                    PlayData.Data.BMSUseSudden[1] = BMSUseSudden2P.ON;
                    PlayData.Data.BMSSuddenNumber[1] = BMSSuddenNumber2P.Value;
                    PlayData.Data.BMSFloatingHiSpeed[1] = BMSFloatingHiSpeed2P.ON;
                    PlayData.Data.BMSGreenNumber[1] = BMSGreenNumber2P.Value;
                    PlayData.Data.BMSNormalHiSpeed[1] = BMSNormalHiSpeed2P.ON;
                    PlayData.Data.BMSNHSSpeed[1] = BMSNHSSpeed2P.Value;
                    break;
                case ELayer.KeyConfig:
                    PlayData.Data.LEFTDON = LeftDon.Value;
                    PlayData.Data.LEFTKA = LeftKa.Value;
                    PlayData.Data.RIGHTDON = RightDon.Value;
                    PlayData.Data.RIGHTKA = RightKa.Value;
                    PlayData.Data.LEFTDON2P = LeftDon2P.Value;
                    PlayData.Data.LEFTKA2P = LeftKa2P.Value;
                    PlayData.Data.RIGHTDON2P = RightDon2P.Value;
                    PlayData.Data.RIGHTKA2P = RightKa2P.Value;
                    PlayData.Data.ScreenShot = ScreenShot.Value;
                    PlayData.Data.MoveConfig = MoveConfig.Value;
                    PlayData.Data.OpenOption = OpenOption.Value;
                    PlayData.Data.PlayStart = PlayStart.Value;
                    PlayData.Data.PlayReset = PlayReset.Value;
                    PlayData.Data.DisplaySudden = DisplaySudden.Value;
                    PlayData.Data.SuddenPlus = SuddenPlus.Value;
                    PlayData.Data.SuddenMinus = SuddenMinus.Value;
                    PlayData.Data.ChangeFHS = ChangeFHS.Value;
                    PlayData.Data.DisplaySudden2P = DisplaySudden2P.Value;
                    PlayData.Data.SuddenPlus2P = SuddenPlus2P.Value;
                    PlayData.Data.SuddenMinus2P = SuddenMinus2P.Value;
                    PlayData.Data.ChangeFHS2P = ChangeFHS2P.Value;
                    PlayData.Data.MeasureUp = MeasureUp.Value;
                    PlayData.Data.MeasureDown = MeasureDown.Value;
                    PlayData.Data.JunpHome = JunpHome.Value;
                    PlayData.Data.JunpEnd = JunpEnd.Value;
                    PlayData.Data.ChangeAuto = ChangeAuto.Value;
                    PlayData.Data.ChangeAuto2P = ChangeAuto2P.Value;
                    PlayData.Data.MoveCreate = MoveCreate.Value;
                    PlayData.Data.InfoMenu = InfoMenu.Value;
                    PlayData.Data.AddMeasure = AddMeasure.Value;
                    PlayData.Data.AddCommand = AddCommand.Value;
                    PlayData.Data.OpenTenplate = OpenTenplate.Value;
                    PlayData.Data.RealTimeMapping = RealTimeMapping.Value;
                    PlayData.Data.SaveFile = SaveFile.Value;
                    PlayData.Data.SaveReplay = SaveReplay.Value;
                    break;
                case ELayer.BMSKeyConfig:
                    PlayData.Data.Controller = new int[2] { Controller.Value, Controller2P.Value };
                    PlayData.Data.BMSKey = new BMSKey()
                    {
                        Key1 = Key1.Value,
                        Key2 = Key2.Value,
                        Key3 = Key3.Value,
                        Key4 = Key4.Value,
                        Key5 = Key5.Value,
                        Key6 = Key6.Value,
                        Key7 = Key7.Value,
                        Scr_F = Scr_F.Value,
                        Scr_R = Scr_R.Value,
                        Start = Start.Value,
                        Select = Select.Value,
                        Pad_Key1 = PKey1.Value,
                        Pad_Key2 = PKey2.Value,
                        Pad_Key3 = PKey3.Value,
                        Pad_Key4 = PKey4.Value,
                        Pad_Key5 = PKey5.Value,
                        Pad_Key6 = PKey6.Value,
                        Pad_Key7 = PKey7.Value,
                        Pad_Scr_F = PScr_F.Value,
                        Pad_Scr_R = PScr_R.Value,
                        Pad_Start = PStart.Value,
                        Pad_Select = PSelect.Value,
                    };
                    PlayData.Data.BMSKey2P = new BMSKey()
                    {
                        Key1 = Key1_2P.Value,
                        Key2 = Key2_2P.Value,
                        Key3 = Key3_2P.Value,
                        Key4 = Key4_2P.Value,
                        Key5 = Key5_2P.Value,
                        Key6 = Key6_2P.Value,
                        Key7 = Key7_2P.Value,
                        Scr_F = Scr_F_2P.Value,
                        Scr_R = Scr_R_2P.Value,
                        Start = Start_2P.Value,
                        Select = Select_2P.Value,
                        Pad_Key1 = PKey1_2P.Value,
                        Pad_Key2 = PKey2_2P.Value,
                        Pad_Key3 = PKey3_2P.Value,
                        Pad_Key4 = PKey4_2P.Value,
                        Pad_Key5 = PKey5_2P.Value,
                        Pad_Key6 = PKey6_2P.Value,
                        Pad_Key7 = PKey7_2P.Value,
                        Pad_Scr_F = PScr_F_2P.Value,
                        Pad_Scr_R = PScr_R_2P.Value,
                        Pad_Start = PStart_2P.Value,
                        Pad_Select = PSelect_2P.Value,
                    };
                    break;
                case ELayer.Skin:
                    PlayData.Data.SkinName = SkinName.Text;
                    Tx.Init();
                    PlayData.Data.SoundName = SoundName.Text;
                    PlayData.Data.BGMName = BGMName.Text;
                    Sfx.Init();
                    PlayData.Data.SkinColor = new int[3] { SkinColorR.Value, SkinColorG.Value, SkinColorB.Value };
                    PlayData.Data.FontRendering = FontRendering.ON;
                    PlayData.Data.FontName = FontName.Text;
                    break;
            }
        }

        public static void SoundPitch()
        {
            if (!PlayData.Data.ChangeSESpeed)
            {
                for (int i = 0; i < 5; i++)
                {
                    Sfx.Don[i].PlaySpeed = 1.0;
                    Sfx.Ka[i].PlaySpeed = 1.0;
                    Sfx.DON[i].PlaySpeed = 1.0;
                    Sfx.KA[i].PlaySpeed = 1.0;
                    Sfx.Balloon[i].PlaySpeed = 1.0;
                    Sfx.Kusudama[i].PlaySpeed = 1.0;
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Sfx.Don[i].PlaySpeed = PlayData.Data.PlaySpeed;
                    Sfx.Ka[i].PlaySpeed = PlayData.Data.PlaySpeed;
                    Sfx.DON[i].PlaySpeed = PlayData.Data.PlaySpeed;
                    Sfx.KA[i].PlaySpeed = PlayData.Data.PlaySpeed;
                    Sfx.Balloon[i].PlaySpeed = PlayData.Data.PlaySpeed;
                    Sfx.Kusudama[i].PlaySpeed = PlayData.Data.PlaySpeed;
                }
            }
        }

        public static int Layer, Cursor;
        public static bool InLayer, Selecting, FromTitle;
        public static List<Option> OptionList;
        public static Counter[] PushedTimer = new Counter[2], PushingTimer = new Counter[2];

        public enum ELayer
        {
            System,
            PlayTJA,
            PlayTJA1P,
            PlayTJA2P,
            PlayBMS,
            PlayBMS1P,
            PlayBMS2P,
            KeyConfig,
            BMSKeyConfig,
            Skin,
            Back
        }
        public static Option  Back;
        public static OptionBool FullScreen, PreviewSong, FontRendering, VSync, IsPlay2P, ShowImage, PlayMovie, QuickStart, ShowResultScreen, PlayList, SaveScore, RandomDanSave, ShowGraph, ShowBestScore,
            ChangeSESpeed, Random, Mirror, Stelth, Random2P, Mirror2P, Stelth2P, Is2PSide,
            FloatingHiSpeed, NormalHiSpeed, UseSudden, FloatingHiSpeed2P, NormalHiSpeed2P, UseSudden2P, Auto, Auto2P, Just, AutoAdjust, AutoAdjust2P,
            BMSFloatingHiSpeed, BMSNormalHiSpeed, BMSUseSudden, BMSFloatingHiSpeed2P, BMSNormalHiSpeed2P, BMSUseSudden2P;
        public static OptionInt SkinColorR, SkinColorG, SkinColorB, SystemBGM, GameBGM, SE, RandomRate, GreenNumber, NHSSpeed, SuddenNumber, GreenNumber2P, NHSSpeed2P, SuddenNumber2P,
            AutoRoll, Hazard, Hazard2P, BMSGreenNumber, BMSNHSSpeed, BMSSuddenNumber, BMSGreenNumber2P, BMSNHSSpeed2P, BMSSuddenNumber2P, Controller, Controller2P;
        public static OptionList PreviewType, DanFailedType, PlayCourse, PlayCourse2P, RivalType, RivalRank, NotesChange, NotesChange2P, ScrollType, ScrollType2P, GaugeType, GaugeAutoShift, GaugeAutoShiftMin,
            GaugeType2P, GaugeAutoShift2P, GaugeAutoShiftMin2P, JudgeType, LNType, Option, Option2P;
        public static OptionDouble RivalPercent, PlaySpeed, ScrollSpeed, ScrollSpeed2P, JudgePerfect, JudgeGreat, JudgeGood, JudgeBad, JudgePoor, InputAdjust, InputAdjust2P,
            BMSScrollSpeed, BMSScrollSpeed2P;
        public static OptionString PlayerName, SkinName, SoundName, BGMName, FontName, PlayFile, BestScore, RivalScore, Replay, Replay2P;
        public static OptionStrList PlayFolder;
        public static OptionKey ScreenShot, MoveConfig, OpenOption, PlayStart, PlayReset, DisplaySudden, SuddenPlus, SuddenMinus, ChangeFHS, DisplaySudden2P, SuddenPlus2P, SuddenMinus2P, ChangeFHS2P,
            MeasureUp, MeasureDown, JunpHome, JunpEnd, ChangeAuto, ChangeAuto2P, MoveCreate, InfoMenu, AddMeasure, AddCommand, OpenTenplate, RealTimeMapping, SaveFile, SaveReplay,
            Key1, Key2, Key3, Key4, Key5, Key6, Key7, Scr_F, Scr_R, Start, Select,
            Key1_2P, Key2_2P, Key3_2P, Key4_2P, Key5_2P, Key6_2P, Key7_2P, Scr_F_2P, Scr_R_2P, Start_2P, Select_2P;
        public static OptionPad PKey1, PKey2, PKey3, PKey4, PKey5, PKey6, PKey7, PScr_F, PScr_R, PStart, PSelect,
            PKey1_2P, PKey2_2P, PKey3_2P, PKey4_2P, PKey5_2P, PKey6_2P, PKey7_2P, PScr_F_2P, PScr_R_2P, PStart_2P, PSelect_2P;
        public static OptionEKey LeftDon, LeftKa, RightDon, RightKa, LeftDon2P, LeftKa2P, RightDon2P, RightKa2P;
    }
}
