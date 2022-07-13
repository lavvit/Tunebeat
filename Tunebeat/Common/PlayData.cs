using System.Collections.Generic;
using System.IO;
using SeaDrop;

namespace Tunebeat
{
    class PlayData
    {
        public static void Init()
        {
            Data = new Data();
            if (!File.Exists("Config.json"))
                ConfigJson.SaveConfig(Data, "Config.json");

            Data = ConfigJson.GetConfig<Data>(@"Config.json");
            if (Data.PlayFolder.Count > 1) Data.PlayFolder.RemoveAt(0);
            if (Data.LEFTDON.Count > 1) Data.LEFTDON.RemoveAt(0);
            if (Data.LEFTKA.Count > 1) Data.LEFTKA.RemoveAt(0);
            if (Data.RIGHTDON.Count > 1) Data.RIGHTDON.RemoveAt(0);
            if (Data.RIGHTKA.Count > 1) Data.RIGHTKA.RemoveAt(0);
            if (Data.LEFTDON2P.Count > 1) Data.LEFTDON2P.RemoveAt(0);
            if (Data.LEFTKA2P.Count > 1) Data.LEFTKA2P.RemoveAt(0);
            if (Data.RIGHTDON2P.Count > 1) Data.RIGHTDON2P.RemoveAt(0);
            if (Data.RIGHTKA2P.Count > 1) Data.RIGHTKA2P.RemoveAt(0);
        }
        public static void End()
        {
            ConfigJson.SaveConfig(Data, "Config.json");
        }
        public static Data Data { get; set; }
    }
    class Data
    {
        public string Memo = "Tunebeatの設定ファイルです。値の項目は数値、true/falseはON/OFF、””で囲われた所は文字で入力してください(￥は使えません。お手数ですが\\か/に変換してください。)";
        public string PlayerName = "";
        public string SkinName = "Default";
        public string SoundName = "Default";
        public string BGMName = "Default";
        public int[] SkinColor = new int[3] { 128, 128, 128 };
        public bool FullScreen = false;
        public (int, int) Size = (1920, 1080);
        public bool FontRendering = true;
        public string FontName = "";
        public bool VSync = false;

        public bool PreviewSong = true;
        public int SystemBGM = 100;
        public int GameBGM = 100;
        public int SE = 100;

        public List<string> PlayFolder = new List<string>() { "Songs" };
        public bool IgnoreFolder = false;
        public int PreviewType = 0;
        public int[] PlayCourse = new int[2] { 3, 3 };
        public bool IsPlay2P = false;
        public bool ShowImage = true;
        public bool PlayMovie = true;
        public bool QuickStart = false;
        public int ShowComboNotes = 10;
        public bool ShowResultScreen = false;
        public bool PlayList = false;
        public bool SaveScore = false;

        public bool ShowGraph = false;
        public bool ShowBestScore = true;
        public string BestScore = "";
        public int RivalType = 0;
        public double RivalPercent = 0;
        public int RivalRank = 0;
        public string RivalScore = "";

        public double PlaySpeed = 1.0;
        public bool ChangeSESpeed = true;
        public bool[] Random = new bool[2] { false, false };
        public int RandomRate = 100;
        public bool[] Mirror = new bool[2] { false, false };
        public bool[] Stelth = new bool[2] { false, false };
        public int[] NotesChange = new int[2] { 0, 0 };

        public int[] ScrollType = new int[2] { 0, 0 };
        public double[] ScrollSpeed = new double[2] { 1.0, 1.0 };
        public bool[] FloatingHiSpeed = new bool[2] { false, false };
        public int[] GreenNumber = new int[2] { 1200, 1200 };
        public bool[] NormalHiSpeed = new bool[2] { false, false };
        public int[] NHSSpeed = new int[2] { 0, 0 };
        public bool[] UseSudden = new bool[2] { false, false };
        public int[] SuddenNumber = new int[2] { 0, 0 };

        public bool[] Auto = new bool[2] { false, false };
        public int AutoRoll = 15;
        public string[] Replay = new string[2] { "", "" };

        public int[] GaugeType = new int[2] { 0, 0 };
        public int[] GaugeAutoShift = new int[2] { 0, 0 };
        public int[] GaugeAutoShiftMin = new int[2] { 0, 0 };
        public int[] Hazard = new int[2] { 0, 0 };

        public int JudgeType = 0;
        public double JudgePerfect = 10;
        public double JudgeGreat = 25;
        public double JudgeGood = 75;
        public double JudgeBad = 100;
        public double JudgePoor = 120;
        public bool Just = false;
        public double[] InputAdjust = new double[2] { 0, 0 };
        public bool[] AutoAdjust = new bool[2] { false, false };

        public bool RandomDanSave = false;
        public int DanFailedType = 2;
        public DanOrder DanSetting = new DanOrder();

        public List<string> Template = new List<string>();

        public int LNType = 1;
        public bool Is2PSide = true;
        public bool Hitsound = true;
        public int[] Option = new int[2] { 0, 0 };
        public double[] BMSScrollSpeed = new double[2] { 1.0, 1.0 };
        public bool[] BMSFloatingHiSpeed = new bool[2] { false, false };
        public int[] BMSGreenNumber = new int[2] { 1200, 1200 };
        public bool[] BMSNormalHiSpeed = new bool[2] { false, false };
        public int[] BMSNHSSpeed = new int[2] { 0, 0 };
        public bool[] BMSUseSudden = new bool[2] { false, false };
        public int[] BMSSuddenNumber = new int[2] { 0, 0 };
        public double[] BMSInputAdjust = new double[2] { 0, 0 };

        public int[] Controller = new int[2] { 0, 0 };

        public List<int> LEFTDON = new List<int>() { (int)EKey.F };
        public List<int> RIGHTDON = new List<int>() { (int)EKey.J };
        public List<int> LEFTKA = new List<int>() { (int)EKey.D };
        public List<int> RIGHTKA = new List<int>() { (int)EKey.K };
        public List<int> LEFTDON2P = new List<int>() { (int)EKey.V };
        public List<int> RIGHTDON2P = new List<int>() { (int)EKey.M };
        public List<int> LEFTKA2P = new List<int>() { (int)EKey.C };
        public List<int> RIGHTKA2P = new List<int>() { (int)EKey.Comma };

        public BMSKey BMSKey = new BMSKey()
        {
            Key1 = (int)EKey.Z,
            Key2 = (int)EKey.S,
            Key3 = (int)EKey.X,
            Key4 = (int)EKey.D,
            Key5 = (int)EKey.C,
            Key6 = (int)EKey.F,
            Key7 = (int)EKey.V,
            Scr_F = (int)EKey.LShift,
            Scr_R = (int)EKey.LCtrl,
            Start = (int)EKey.Q,
            Select = (int)EKey.W,
        };
        public BMSKey BMSKey2P = new BMSKey()
        {
            Key1 = (int)EKey.Comma,
            Key2 = (int)EKey.L,
            Key3 = (int)EKey.Period,
            Key4 = (int)EKey.SemiColon,
            Key5 = (int)EKey.Slash,
            Key6 = (int)EKey.Colon,
            Key7 = (int)EKey.BackSlash,
            Scr_F = (int)EKey.RShift,
            Scr_R = (int)EKey.RCtrl,
            Start = (int)EKey.At,
            Select = (int)EKey.LBracket,
        };

        public int ScreenShot = (int)EKey.F12;
        public int MoveConfig = (int)EKey.F1;
        public int OpenOption = (int)EKey.F2;
        public int PlayStart = (int)EKey.Space;
        public int PlayReset = (int)EKey.Q;
        public int DisplaySudden = (int)EKey.LShift;
        public int SuddenPlus = (int)EKey.Z;
        public int SuddenMinus = (int)EKey.X;
        public int ChangeFHS = (int)EKey.LCtrl;
        public int DisplaySudden2P = (int)EKey.RShift;
        public int SuddenPlus2P = (int)EKey.Slash;
        public int SuddenMinus2P = (int)EKey.BackSlash;
        public int ChangeFHS2P = (int)EKey.RCtrl;
        public int MeasureUp = (int)EKey.PgUp;
        public int MeasureDown = (int)EKey.PgDn;
        public int JunpHome = (int)EKey.Home;
        public int JunpEnd = (int)EKey.End;
        public int ChangeAuto = (int)EKey.F1;
        public int ChangeAuto2P = (int)EKey.F2;
        public int MoveCreate = (int)EKey.F3;
        public int InfoMenu = (int)EKey.F4;
        public int AddMeasure = (int)EKey.F5;
        public int AddCommand = (int)EKey.F6;
        public int OpenTenplate = (int)EKey.F7;
        public int RealTimeMapping = (int)EKey.F8;
        public int SaveFile = (int)EKey.F9;
        public int SaveReplay = (int)EKey.F11;
        //public int MoveTraining = EKey.F9;
    }

    public class BMSKey
    {
        public int Key1;
        public int Key2;
        public int Key3;
        public int Key4;
        public int Key5;
        public int Key6;
        public int Key7;
        public int Scr_F;
        public int Scr_R;
        public int Start;
        public int Select;

        public int Pad_Key1 = (int)JoypadButton.Pad_1;
        public int Pad_Key2 = (int)JoypadButton.Pad_2;
        public int Pad_Key3 = (int)JoypadButton.Pad_3;
        public int Pad_Key4 = (int)JoypadButton.Pad_4;
        public int Pad_Key5 = (int)JoypadButton.Pad_5;
        public int Pad_Key6 = (int)JoypadButton.Pad_6;
        public int Pad_Key7 = (int)JoypadButton.Pad_7;
        public int Pad_Scr_F = (int)JoypadButton.Up;
        public int Pad_Scr_R = (int)JoypadButton.Down;
        public int Pad_Start = (int)JoypadButton.Pad_9;
        public int Pad_Select = (int)JoypadButton.Pad_10;
    }
}