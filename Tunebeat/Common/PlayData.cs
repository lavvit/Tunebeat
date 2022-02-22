using System.Collections.Generic;
using System.IO;
using Amaoto;
using static DxLibDLL.DX;

namespace Tunebeat.Common
{
    class PlayData
    {
        public static void Init()
        {
            Data = new Data();
            if (!File.Exists("Config.json"))
                ConfigManager.SaveConfig(Data, "Config.json");

            Data = ConfigManager.GetConfig<Data>(@"Config.json");
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
            ConfigManager.SaveConfig(Data, "Config.json");
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
        public bool FontRendering = true;
        public string FontName = "";

        public bool PreviewSong = true;
        public int SystemBGM = 100;
        public int GameBGM = 100;
        public int SE = 100;

        public List<string> PlayFolder = new List<string>() { "Songs" };
        public int PreviewType = 0;
        public int[] PlayCourse = new int[2] { 3, 3 };
        public bool IsPlay2P = false;
        public bool ShowImage = true;
        public bool PlayMovie = true;
        public bool QuickStart = false;
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

        public List<string> Template = new List<string>();

        public List<int> LEFTDON = new List<int>() { KEY_INPUT_F };
        public List<int> RIGHTDON = new List<int>() { KEY_INPUT_J };
        public List<int> LEFTKA = new List<int>() { KEY_INPUT_D };
        public List<int> RIGHTKA = new List<int>() { KEY_INPUT_K };
        public List<int> LEFTDON2P = new List<int>() { KEY_INPUT_V };
        public List<int> RIGHTDON2P = new List<int>() { KEY_INPUT_M };
        public List<int> LEFTKA2P = new List<int>() { KEY_INPUT_C };
        public List<int> RIGHTKA2P = new List<int>() { KEY_INPUT_COMMA };

        public int ScreenShot = KEY_INPUT_F12;
        public int MoveConfig = KEY_INPUT_F1;
        public int OpenOption = KEY_INPUT_F2;
        public int PlayStart = KEY_INPUT_SPACE;
        public int PlayReset = KEY_INPUT_Q;
        public int DisplaySudden = KEY_INPUT_LSHIFT;
        public int SuddenPlus = KEY_INPUT_Z;
        public int SuddenMinus = KEY_INPUT_X;
        public int ChangeFHS = KEY_INPUT_LCONTROL;
        public int DisplaySudden2P = KEY_INPUT_RSHIFT;
        public int SuddenPlus2P = KEY_INPUT_SLASH;
        public int SuddenMinus2P = KEY_INPUT_BACKSLASH;
        public int ChangeFHS2P = KEY_INPUT_RCONTROL;
        public int MeasureUp = KEY_INPUT_PGUP;
        public int MeasureDown = KEY_INPUT_PGDN;
        public int JunpHome = KEY_INPUT_HOME;
        public int JunpEnd = KEY_INPUT_END;
        public int ChangeAuto = KEY_INPUT_F1;
        public int ChangeAuto2P = KEY_INPUT_F2;
        public int MoveCreate = KEY_INPUT_F3;
        public int InfoMenu = KEY_INPUT_F4;
        public int OpenText = KEY_INPUT_F5;
        public int AddCommand = KEY_INPUT_F6;
        public int OpenTenplate = KEY_INPUT_F7;
        public int RealTimeMapping = KEY_INPUT_F8;
        public int SaveFile = KEY_INPUT_F9;
        public int SaveReplay = KEY_INPUT_F11;
        //public int MoveTraining = KEY_INPUT_F9;
    }
}