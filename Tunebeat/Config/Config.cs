using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;
using Amaoto;
using TJAParse;
using Tunebeat.Common;

namespace Tunebeat.Config
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
            DrawString(80, 200 + 40 * Layer, ">", 0xff0000);
            DrawString(100, 200, "System", Layer == (int)ELayer.System ? 0xff0000 : (InLayer ? 0x808080 : (uint)0xffffff));
            DrawString(100, 240, "Play TJA", Layer == (int)ELayer.PlayTJA ? 0xff0000 : (InLayer ? 0x808080 : (uint)0xffffff));
            DrawString(100, 280, "Play TJA(2P)", Layer == (int)ELayer.PlayTJA2P ? 0xff0000 : (InLayer ? 0x808080 : (uint)0xffffff));
            DrawString(100, 320, "Play BMS", Layer == (int)ELayer.PlayBMS ? 0xff0000 : (InLayer ? 0x808080 : (uint)0xffffff));
            DrawString(100, 360, "Play BMS(2P)", Layer == (int)ELayer.PlayBMS2P ? 0xff0000 : (InLayer ? 0x808080 : (uint)0xffffff));
            DrawString(100, 400, "KEY CONFIG", Layer == (int)ELayer.KeyConfig ? 0xff0000 : (InLayer ? 0x808080 : (uint)0xffffff));
            DrawString(100, 440, "Back to Menu", Layer == (int)ELayer.Back ? 0xff0000 : (InLayer ? 0x808080 : (uint)0xffffff));

            if (OptionList != null)
            {
                for (int i = 0; i < OptionList.Count; i++)
                {
                    Option list = OptionList[i];
                    DrawString(300, 200 + 20 * i, $"{list.Name}", !InLayer ? 0x808080 : (Cursor == i ? 0xff0000 : (uint)0xffffff));
                    if (InLayer && Cursor == i)
                    {
                        DrawString(280, 200 + 20 * i, ">", 0xff0000);
                        DrawString(600, 200, $"{list.Info}", 0xffffff);
                        if (list.objAmount() != null)
                        {
                            DrawString(600, 240, $"Now:{list.objAmount()}", Selecting && list.Type < OptionType.String ? 0xffff00 : (uint)0xffffff);
                            if (Input.IsEnable)
                            {
                                DrawString(632 + GetDrawStringWidth(Input.Text, Input.Position), 240, "|", 0xffff00);
                            }
                        }

                        if ((ELayer)Layer == ELayer.System && i >= 1 && i <= 3)
                        {
                            DrawCircleAA(1450, 640, 200, 256, GetColor(OptionList[1].GetIndex(), OptionList[2].GetIndex(), OptionList[3].GetIndex()), TRUE);
                        }
                    }
                }
            }

            #if DEBUG
            DrawString(0, 0, $"Layer:{Layer}", 0xffffff);
            if (InLayer) DrawString(0, 20, $"Cursor:{Cursor}", 0xffffff);
            if (Selecting) DrawString(0, 60, "Selecting...", 0xffffff);

            DrawString(0, 40, $"ListCount:{OptionList.Count}", 0xffffff);
            #endif
            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(KEY_INPUT_UP))
            {
                PushedTimer[0].Start();
            }
            if (Key.IsLeft(KEY_INPUT_UP))
            {
                PushedTimer[0].Stop();
                PushedTimer[0].Reset();
                PushingTimer[0].Stop();
                PushingTimer[0].Reset();
            }
            if (Key.IsPushed(KEY_INPUT_DOWN))
            {
                PushedTimer[1].Start();
            }
            if (Key.IsLeft(KEY_INPUT_DOWN))
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

            if (Key.IsPushing(KEY_INPUT_UP) && Key.IsPushing(KEY_INPUT_DOWN) && Selecting && OptionList != null)
            {
                OptionList[Cursor].Reset();
            }
            else
            {
                if (Key.IsPushed(KEY_INPUT_UP) || (PushingTimer[0].Value == PushingTimer[0].End))
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
                if (Key.IsPushed(KEY_INPUT_DOWN) || (PushingTimer[1].Value == PushingTimer[1].End))
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
                if (Key.IsPushed(KEY_INPUT_RETURN))
                {
                    if (Selecting)
                    {
                        Selecting = false;
                        if (OptionList[Cursor].Type == OptionType.String || OptionList[Cursor].Type == OptionType.StrList)
                        {
                            OptionList[Cursor].Enter();
                            if (OptionList[Cursor].Name == "PlayFolder")
                                SongSelect.SongSelect.NowSongNumber = 0;
                        }
                        UpdateConfig();
                    }
                    else if (InLayer)
                    {
                        if (OptionList[Cursor].Type == OptionType.Int || OptionList[Cursor].Type == OptionType.List || OptionList[Cursor].Type == OptionType.Double)
                        {
                            Selecting = true;
                        }
                        else if (OptionList[Cursor].Type == OptionType.String || OptionList[Cursor].Type == OptionType.StrList)
                        {
                            Selecting = true;
                            OptionList[Cursor].Enter();
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
                            if (Title.Title.Config) Program.SceneChange(new Title.Title());
                            else Program.SceneChange(new SongSelect.SongSelect());
                        }
                        else if (OptionList.Count > 0)
                        {
                            InLayer = true;
                        }
                    }
                }
                if (Key.IsPushed(KEY_INPUT_ESCAPE))
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
                        if (Title.Title.Config) Program.SceneChange(new Title.Title());
                        else Program.SceneChange(new SongSelect.SongSelect());
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
                    SkinName = new OptionString("SkinName", PlayData.Data.SkinName, "使用する画像のフォルダを変更します。"); OptionList.Add(SkinName);
                    SoundName = new OptionString("SoundName", PlayData.Data.SoundName, "使用する効果音のフォルダを変更します。"); OptionList.Add(SoundName);
                    BGMName = new OptionString("BGMName", PlayData.Data.BGMName, "使用するBGMのフォルダを変更します。"); OptionList.Add(BGMName);
                    PlayFolder = new OptionStrList("PlayFolder", PlayData.Data.PlayFolder, "読み込むフォルダを変更します。(セミコロン(;)で区切ることにより複数のパスを指定できます。)"); OptionList.Add(PlayFolder);
                    SkinColorR = new OptionInt("SkinColor - R", PlayData.Data.SkinColor[0], 0, 255, "スキンの色を変更します。(Red)"); OptionList.Add(SkinColorR);
                    SkinColorG = new OptionInt("SkinColor - G", PlayData.Data.SkinColor[1], 0, 255, "スキンの色を変更します。(Green)"); OptionList.Add(SkinColorG);
                    SkinColorB = new OptionInt("SkinColor - B", PlayData.Data.SkinColor[2], 0, 255, "スキンの色を変更します。(Blue)"); OptionList.Add(SkinColorB);
                    PreviewSong = new OptionBool("PreviewSong", PlayData.Data.PreviewSong, "曲のプレビューを再生します。"); OptionList.Add(PreviewSong);
                    SystemBGM = new OptionInt("Volume-SystemBGM", PlayData.Data.SystemBGM, 0, 100, "システムのBGMの音量を調節します。"); OptionList.Add(SystemBGM);
                    GameBGM = new OptionInt("Volume-GameBGM", PlayData.Data.GameBGM, 0, 100, "演奏中の曲の音量を調節します。"); OptionList.Add(GameBGM);
                    SE = new OptionInt("Volume-SE", PlayData.Data.SystemBGM, 0, 100, "効果音の音量を調節します。"); OptionList.Add(SE);
                    FontRendering = new OptionBool("FontRendering", PlayData.Data.FontRendering, "文字を指定したフォントで読み込みます。(エラーが出る場合はOFFにしてください)"); OptionList.Add(FontRendering);
                    FontName = new OptionString("FontName", PlayData.Data.FontName, "使用するフォントを変更します。(空白の場合デフォルトの設定を使用します。)"); OptionList.Add(FontName);
                    ShowImage = new OptionBool("ShowImage", PlayData.Data.ShowImage, "BGIMAGEに記述された画像を背景に表示します。"); OptionList.Add(ShowImage);
                    PlayMovie = new OptionBool("PlayMovie", PlayData.Data.PlayMovie, "BGMOVIEに記述された動画を背景に表示します。"); OptionList.Add(PlayMovie);
                    QuickStart = new OptionBool("QuickStart", PlayData.Data.QuickStart, "自動的に譜面を再生します。"); OptionList.Add(QuickStart);
                    ShowResultScreen = new OptionBool("ShowResultScreen", PlayData.Data.ShowResultScreen, "リザルト画面を表示します。"); OptionList.Add(ShowResultScreen);
                    PlayList = new OptionBool("PlayList", PlayData.Data.PlayList, "プレイが終了したらすぐ次の譜面を再生します。"); OptionList.Add(PlayList);
                    SaveScore = new OptionBool("SaveScore", PlayData.Data.SaveScore, "記録を更新した時にスコアを保存します。"); OptionList.Add(SaveScore);

                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayTJA:
                    PreviewType = new OptionList("PreviewType", PlayData.Data.PreviewType, "再生の仕方を変更します。",
                        new string[] { "Normal", "Up", "Down", "AllCourses" }); OptionList.Add(PreviewType);
                    PlayCourse = new OptionList("PlayCourse", PlayData.Data.PlayCourse[0], "再生する難易度を変更します。",
                        new string[] { "Easy", "Normal", "Hard", "Oni", "Edit" }); OptionList.Add(PlayCourse);
                    ShowGraph = new OptionBool("ShowGraph", PlayData.Data.ShowGraph, "自分やライバルの記録と比較できるグラフを表示します。"); OptionList.Add(ShowGraph);
                    ShowBestScore = new OptionBool("ShowBestScore", PlayData.Data.ShowBestScore, "自己ベスト記録のグラフを表示します。(重い場合は非表示にしてください)"); OptionList.Add(ShowBestScore);
                    RivalType = new OptionList("RivalType", PlayData.Data.RivalType, "ライバルを設定します。",
                        new string[] { "None", "Percent", "Rank", "PlayScore" }); OptionList.Add(RivalType);
                    RivalPercent = new OptionDouble("RivalPercent", PlayData.Data.RivalPercent, 0, 100, "Percent 使用時の目標スコアを設定します。"); OptionList.Add(RivalPercent);
                    RivalRank = new OptionList("RivalPercent", PlayData.Data.RivalRank, "Rank 使用時の目標スコアを設定します。",
                        new string[] { "F", "E", "D", "C", "B", "A", "AA", "AAA", "MAX" }); OptionList.Add(RivalRank);
                    PlaySpeed = new OptionDouble("PlaySpeed", PlayData.Data.PlaySpeed, 0, 1000, "曲の再生速度を変更します。"); OptionList.Add(PlaySpeed);
                    ChangeSESpeed = new OptionBool("ChangeSESpeed", PlayData.Data.ChangeSESpeed, "曲の再生速度に合わせて効果音のピッチを調節します。"); OptionList.Add(ChangeSESpeed);
                    Random = new OptionBool("Random", PlayData.Data.Random[0], "ノーツの色をランダムに変更します。"); OptionList.Add(Random);
                    RandomRate = new OptionInt("RandomRate", PlayData.Data.RandomRate, 0, 100, "ランダムの割合を調節します。"); OptionList.Add(RandomRate);
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
                    AutoRoll = new OptionInt("AutoRoll", PlayData.Data.AutoRoll, 0, 1000, "オート時の連打の秒間打数を設定します。"); OptionList.Add(AutoRoll);
                    GaugeType = new OptionList("GaugeType", PlayData.Data.GaugeType[0], "ゲージの種類を変更します。",
                        new string[] { "Normal", "AssistEasy", "Easy", "Hard", "EX-Hard" }); OptionList.Add(GaugeType);
                    GaugeAutoShift = new OptionList("GaugeAutoShift", PlayData.Data.GaugeAutoShift[0], "ゲージを自動的に変更したりする機能です。",
                        new string[] { "None", "Continue", "Retry", "ToNormal", "Best", "LessBest" }); OptionList.Add(GaugeAutoShift);
                    GaugeAutoShiftMin = new OptionList("GaugeAutoShiftMin", PlayData.Data.GaugeAutoShiftMin[0], "Bestなどで下がる際のゲージの下限を変更します。",
                        new string[] { "Normal", "AssistEasy", "Easy" }); OptionList.Add(GaugeAutoShiftMin);
                    Hazard = new OptionInt("Hazard", PlayData.Data.Hazard[0], 0, 10000, "指定個数BadやPoorを出すと終了します。(1以上で有効)"); OptionList.Add(Hazard);
                    JudgeType = new OptionList("JudgeType", PlayData.Data.JudgeType, "ソフトで設定された判定を使用します。",
                        new string[] { "Custom", "Standard", "HardMode" }); OptionList.Add(JudgeType);
                    JudgePerfect = new OptionDouble("JudgePerfect", PlayData.Data.JudgePerfect, 0, 10000, "Perfect の範囲を設定します。"); OptionList.Add(JudgePerfect);
                    JudgeGreat = new OptionDouble("JudgeGreat", PlayData.Data.JudgeGreat, 0, 10000, "Great の範囲を設定します。"); OptionList.Add(JudgeGreat);
                    JudgeGood = new OptionDouble("JudgeGood", PlayData.Data.JudgeGood, 0, 10000, "Good の範囲を設定します。"); OptionList.Add(JudgeGood);
                    JudgeBad = new OptionDouble("JudgeBad", PlayData.Data.JudgeBad, 0, 10000, "Bad の範囲を設定します。"); OptionList.Add(JudgeBad);
                    JudgePoor = new OptionDouble("JudgePoor", PlayData.Data.JudgePoor, 0, 10000, "Poor の範囲を設定します。"); OptionList.Add(JudgePoor);
                    Just = new OptionBool("Just", PlayData.Data.Just, "Goodが強制的にBadになります。"); OptionList.Add(Just);
                    InputAdjust = new OptionDouble("InputAdjust", PlayData.Data.InputAdjust[0], -10000, 10000, "判定の位置を調整します。"); OptionList.Add(InputAdjust);
                    AutoAdjust = new OptionBool("AutoAdjust", PlayData.Data.AutoAdjust[0], "自動で判定の位置を調整する機能です。"); OptionList.Add(AutoAdjust);

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


                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.PlayBMS2P:


                    Back = new Option("<< Back to List", "前の項目に戻ります。"); OptionList.Add(Back);
                    break;
                case ELayer.KeyConfig:


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
                    PlayData.Data.SkinName = SkinName.Text;
                    TextureLoad.Init();
                    PlayData.Data.SoundName = SoundName.Text;
                    PlayData.Data.BGMName = BGMName.Text;
                    SoundLoad.Init();
                    PlayData.Data.PreviewSong = PreviewSong.ON;
                    PlayData.Data.SystemBGM = SystemBGM.Value;
                    PlayData.Data.GameBGM = GameBGM.Value;
                    PlayData.Data.SE = SE.Value;
                    PlayData.Data.FontRendering = FontRendering.ON;
                    PlayData.Data.FontName = FontName.Text;
                    PlayData.Data.PlayFolder = PlayFolder.Text;
                    PlayData.Data.SkinColor = new int[3] { SkinColorR.Value, SkinColorG.Value, SkinColorB.Value };
                    PlayData.Data.ShowImage = ShowImage.ON;
                    PlayData.Data.PlayMovie = PlayMovie.ON;
                    PlayData.Data.QuickStart = QuickStart.ON;
                    PlayData.Data.ShowResultScreen = ShowResultScreen.ON;
                    PlayData.Data.PlayList = PlayList.ON;
                    PlayData.Data.SaveScore = SaveScore.ON;
                    break;
                case ELayer.PlayTJA:
                    PlayData.Data.PreviewType = PreviewType.Value;
                    PlayData.Data.PlayCourse[0] = PlayCourse.Value;
                    PlayData.Data.ShowGraph = ShowGraph.ON;
                    PlayData.Data.ShowBestScore = ShowBestScore.ON;
                    PlayData.Data.RivalType = RivalType.Value;
                    PlayData.Data.RivalPercent = RivalPercent.Value;
                    PlayData.Data.RivalRank = RivalRank.Value;
                    PlayData.Data.PlaySpeed = PlaySpeed.Value;
                    PlayData.Data.ChangeSESpeed = ChangeSESpeed.ON;
                    PlayData.Data.Random[0] = Random.ON;
                    PlayData.Data.RandomRate = RandomRate.Value;
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
                    PlayData.Data.AutoRoll = AutoRoll.Value;
                    PlayData.Data.GaugeType[0] = GaugeType.Value;
                    PlayData.Data.GaugeAutoShift[0] = GaugeAutoShift.Value;
                    PlayData.Data.GaugeAutoShiftMin[0] = GaugeAutoShiftMin.Value;
                    PlayData.Data.Hazard[0] = Hazard.Value;
                    PlayData.Data.JudgeType = JudgeType.Value;
                    PlayData.Data.JudgePerfect = JudgePerfect.Value;
                    PlayData.Data.JudgeGreat = JudgeGreat.Value;
                    PlayData.Data.JudgeGood = JudgeGood.Value;
                    PlayData.Data.JudgeBad = JudgeBad.Value;
                    PlayData.Data.JudgePoor = JudgePoor.Value;
                    PlayData.Data.Just = Just.ON;
                    PlayData.Data.InputAdjust[0] = InputAdjust.Value;
                    PlayData.Data.AutoAdjust[0] = AutoAdjust.ON;
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
                    break;
                case ELayer.KeyConfig:
                    break;
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
            PlayTJA2P,
            PlayBMS,
            PlayBMS2P,
            KeyConfig,
            Back
        }
        public static Option  Back, LEFTDON, RIGHTDON, LEFTKA, RIGHTKA, LEFTDON2P, RIGHTDON2P, LEFTKA2P, RIGHTKA2P;
        public static OptionBool FullScreen, PreviewSong, FontRendering, IsPlay2P, ShowImage, PlayMovie, QuickStart, ShowResultScreen, PlayList, SaveScore, ShowGraph, ShowBestScore, ChangeSESpeed, Random, Mirror, Stelth, Random2P, Mirror2P, Stelth2P,
            FloatingHiSpeed, NormalHiSpeed, UseSudden, FloatingHiSpeed2P, NormalHiSpeed2P, UseSudden2P, Auto, Auto2P, Just, AutoAdjust, AutoAdjust2P;
        public static OptionInt SkinColorR, SkinColorG, SkinColorB, SystemBGM, GameBGM, SE, RandomRate, GreenNumber, NHSSpeed, SuddenNumber, GreenNumber2P, NHSSpeed2P, SuddenNumber2P,
            AutoRoll, Hazard, Hazard2P;
        public static OptionList PreviewType, PlayCourse, PlayCourse2P, RivalType, RivalRank, NotesChange, NotesChange2P, ScrollType, ScrollType2P, GaugeType, GaugeAutoShift, GaugeAutoShiftMin, GaugeType2P, GaugeAutoShift2P, GaugeAutoShiftMin2P, JudgeType;
        public static OptionDouble RivalPercent, PlaySpeed, ScrollSpeed, ScrollSpeed2P, JudgePerfect, JudgeGreat, JudgeGood, JudgeBad, JudgePoor, InputAdjust, InputAdjust2P;
        public static OptionString PlayerName, SkinName, SoundName, BGMName, FontName, PlayFile, BestScore, RivalScore, Replay, Replay2P;
        public static OptionStrList PlayFolder;
    }
}
