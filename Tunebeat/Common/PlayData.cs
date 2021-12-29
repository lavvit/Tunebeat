using Amaoto;
using static DxLibDLL.DX;

namespace Tunebeat.Common
{
    class PlayData
    {
        public static void Init()
        {
            SkinName = "Default";
            SoundName = "Default";
            BGMName = "Default";
            PlayFile = @"Songs/水天神術・時雨.tja";
            PlayCourse[0] = 3;
            PlayCourse[1] = 4;
            IsPlay2P = true;

            Auto[0] = true;
            Auto[1] = true;
            AutoRoll = 15;

            GaugeType[0] = 0;
            GaugeAutoShift[0] = 0;
            GaugeAutoShiftMin[0] = 1;
            Hazard[0] = 0;
            GaugeType[1] = 0;
            GaugeAutoShift[1] = 0;
            GaugeAutoShiftMin[1] = 1;
            Hazard[1] = 0;

            JudgeType = 1;
            JudgePerfect = 10;
            JudgeGreat = 25;
            JudgeGood = 75;
            JudgeBad = 100;
            JudgePoor = 120;
            Just = false;

            LEFTDON = KEY_INPUT_F;
            RIGHTDON = KEY_INPUT_J;
            LEFTKA = KEY_INPUT_D;
            RIGHTKA = KEY_INPUT_K;

            LEFTDON2P = KEY_INPUT_V;
            RIGHTDON2P = KEY_INPUT_M;
            LEFTKA2P = KEY_INPUT_C;
            RIGHTKA2P = KEY_INPUT_COMMA;
        }

        public static string SkinName, SoundName, BGMName, PlayFile;
        public static int[] PlayCourse = new int[2];
        public static bool IsPlay2P;

        public static bool[] Auto = new bool[2];
        public static int AutoRoll;

        public static int[] GaugeType = new int[2], GaugeAutoShift = new int[2], GaugeAutoShiftMin = new int[2], Hazard = new int[2];
        public static int JudgeType, JudgePerfect, JudgeGreat, JudgeGood, JudgeBad, JudgePoor;
        public static bool Just;

        public static int LEFTDON, RIGHTDON, LEFTKA, RIGHTKA, LEFTDON2P, RIGHTDON2P, LEFTKA2P, RIGHTKA2P;
    }
}