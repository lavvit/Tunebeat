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

        public List<int> LEFTDON = new List<int>() { (int)EKey.F };
        public List<int> RIGHTDON = new List<int>() { (int)EKey.J };
        public List<int> LEFTKA = new List<int>() { (int)EKey.D };
        public List<int> RIGHTKA = new List<int>() { (int)EKey.K };
        public List<int> LEFTDON2P = new List<int>() { (int)EKey.V };
        public List<int> RIGHTDON2P = new List<int>() { (int)EKey.M };
        public List<int> LEFTKA2P = new List<int>() { (int)EKey.C };
        public List<int> RIGHTKA2P = new List<int>() { (int)EKey.Comma };

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
}