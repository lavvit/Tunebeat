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

        public int LEFTDON = KEY_INPUT_F;
        public int RIGHTDON = KEY_INPUT_J;
        public int LEFTKA = KEY_INPUT_D;
        public int RIGHTKA = KEY_INPUT_K;

        public int LEFTDON2P = KEY_INPUT_V;
        public int RIGHTDON2P = KEY_INPUT_M;
        public int LEFTKA2P = KEY_INPUT_C;
        public int RIGHTKA2P = KEY_INPUT_COMMA;
    }
}