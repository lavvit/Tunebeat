using DxLibDLL;
using System.Collections.Generic;

namespace SeaDrop
{
    /// <summary>
    /// キーボードを管理するクラス。
    /// </summary>
    public static class Key
    {
        /// <summary>
        /// キーボード入力の状態をチェックします。毎フレーム呼び出す必要があります。
        /// </summary>
        public static void Update()
        {
            DX.GetHitKeyStateAll(Buffer);
            for (int i = 0; i < 256; i++)
            {
                if (Buffer[i] == 1)
                {
                    // 現在押している
                    if (!IsPushing((EKey)i))
                    {
                        // 押していない状態から押している状態になった
                        Keys[i] = 1;
                    }
                    else
                    {
                        // 押している状態から押している状態になった。
                        Keys[i] = 2;
                    }
                }
                else
                {
                    // 現在押していない
                    if (IsPushing((EKey)i))
                    {
                        // 押している状態から押していない状態になった
                        Keys[i] = -1;
                    }
                    else
                    {
                        // 押していない状態から押していない状態になった。
                        Keys[i] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// そのキーを押したかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>押したかどうか。</returns>
        public static bool IsPushed(int key)
        {
            return Keys[key] == 1;
        }

        /// <summary>
        /// そのキーを押しているかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>押しているかどうか。</returns>
        public static bool IsPushing(int key)
        {
            return Keys[key] > 0;
        }

        /// <summary>
        /// そのキーを離したかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>離したかどうか。</returns>
        public static bool IsLeft(int key)
        {
            return Keys[key] == -1;
        }
        /// <summary>
        /// そのキーを押したかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>押したかどうか。</returns>
        public static bool IsPushed(EKey key)
        {
            return Keys[(int)key] == 1;
        }

        /// <summary>
        /// そのキーを押しているかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>押しているかどうか。</returns>
        public static bool IsPushing(EKey key)
        {
            return Keys[(int)key] > 0;
        }

        /// <summary>
        /// そのキーを離したかどうかチェックします。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns>離したかどうか。</returns>
        public static bool IsLeft(EKey key)
        {
            return Keys[(int)key] == -1;
        }

        /// <summary>
        /// キーコードに対応する文字列を取得します。
        /// </summary>
        /// <param name="key">キーコード。</param>
        /// <returns><c>key</c> に対応する文字列。正しくないキーコードの場合は <c>UNKNOWN</c> が返る。</returns>
        public static string GetKeyCodeString(int key)
        {
            if (KeyCodeString.TryGetValue(key, out var result))
            {
                return result;
            }
            else
            {
                return "UNKNOWN";
            }
        }

        private static readonly int[] Keys = new int[256];
        private static readonly byte[] Buffer = new byte[256];

        /// <summary>
        /// キーコードの文字列。
        /// </summary>
        public static readonly Dictionary<int, string> KeyCodeString
            = new Dictionary<int, string>()
            {
                { 4, "3" },
                { 5, "4" },
                { 6, "5" },
                { 7, "6" },
                { 8, "7" },
                { 9, "8" },
                { 10, "9" },
                { 3, "2" },
                { 2, "1" },
                { 11, "0" },
                { 44, "Z" },
                { 32, "D" },
                { 18, "E" },
                { 33, "F" },
                { 34, "G" },
                { 35, "H" },
                { 23, "I" },
                { 36, "J" },
                { 37, "K" },
                { 38, "L" },
                { 50, "M" },
                { 49, "N" },
                { 25, "P" },
                { 16, "Q" },
                { 19, "R" },
                { 31, "S" },
                { 20, "T" },
                { 22, "U" },
                { 47, "V" },
                { 17, "W" },
                { 45, "X" },
                { 21, "Y" },
                { 24, "O" },
                { 46, "C" },
                { 48, "B" },
                { 88, "F12" },
                { 14, "BACK" },
                { 15, "TAB" },
                { 28, "RETURN" },
                { 42, "LSHIFT" },
                { 54, "RSHIFT" },
                { 29, "LCONTROL" },
                { 157, "RCONTROL" },
                { 1, "ESCAPE" },
                { 57, "SPACE" },
                { 201, "PGUP" },
                { 209, "PGDN" },
                { 207, "END" },
                { 199, "HOME" },
                { 203, "LEFT" },
                { 200, "UP" },
                { 205, "RIGHT" },
                { 208, "DOWN" },
                { 210, "INSERT" },
                { 211, "DELETE" },
                { 12, "MINUS" },
                { 125, "YEN" },
                { 30, "A" },
                { 144, "PREVTRACK" },
                { 53, "SLASH" },
                { 77, "NUMPAD6" },
                { 71, "NUMPAD7" },
                { 72, "NUMPAD8" },
                { 73, "NUMPAD9" },
                { 55, "MULTIPLY" },
                { 78, "ADD" },
                { 74, "SUBTRACT" },
                { 83, "DECIMAL" },
                { 181, "DIVIDE" },
                { 156, "NUMPADENTER" },
                { 59, "F1" },
                { 60, "F2" },
                { 61, "F3" },
                { 62, "F4" },
                { 63, "F5" },
                { 64, "F6" },
                { 65, "F7" },
                { 66, "F8" },
                { 67, "F9" },
                { 68, "F10" },
                { 87, "F11" },
                { 76, "NUMPAD5" },
                { 75, "NUMPAD4" },
                { 81, "NUMPAD3" },
                { 80, "NUMPAD2" },
                { 56, "LALT" },
                { 184, "RALT" },
                { 70, "SCROLL" },
                { 39, "SEMICOLON" },
                { 146, "COLON" },
                { 26, "LBRACKET" },
                { 27, "RBRACKET" },
                { 145, "AT" },
                { 43, "BACKSLASH" },
                { 51, "COMMA" },
                { 52, "PERIOD" },
                { 148, "KANJI" },
                { 123, "NOCONVERT" },
                { 112, "KANA" },
                { 221, "APPS" },
                { 58, "CAPSLOCK" },
                { 183, "SYSRQ" },
                { 197, "PAUSE" },
                { 219, "LWIN" },
                { 220, "RWIN" },
                { 82, "NUMPAD0" },
                { 79, "NUMPAD1" },
                { 121, "CONVERT" },
                { 69, "NUMLOCK" }
            };
    }

    public enum EKey
    {
        Key_1 = 2,
        Key_2 = 3,
        Key_3 = 4,
        Key_4 = 5,
        Key_5 = 6,
        Key_6 = 7,
        Key_7 = 8,
        Key_8 = 9,
        Key_9 = 10,
        Key_0 = 11,
        A = 30,
        B = 48,
        C = 46,
        D = 32,
        E = 18,
        F = 33,
        G = 34,
        H = 35,
        I = 23,
        J = 36,
        K = 37,
        L = 38,
        M = 50,
        N = 49,
        O = 24,
        P = 25,
        Q = 16,
        R = 19,
        S = 31,
        T = 20,
        U = 22,
        V = 47,
        W = 17,
        X = 45,
        Y = 21,
        Z = 44,
        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,
        F6 = 64,
        F7 = 65,
        F8 = 66,
        F9 = 67,
        F10 = 68,
        F11 = 87,
        F12 = 88,
        Back = 14,
        Tab = 15,
        Enter = 28,
        LShift = 42,
        RShift = 54,
        LCtrl = 29,
        RCtrl = 157,
        Esc = 1,
        Space = 57,
        PgUp = 201,
        PgDn = 209,
        Home = 199,
        End = 207,
        Up = 200,
        Down = 208,
        Left = 203,
        Right = 205,
        Insert = 210,
        Delete = 211,
        Minus = 12,
        Yen = 125,
        Prevtrack = 144,
        Period = 52,
        Slash = 53,
        LAlt = 56,
        RAlt = 184,
        Scroll = 70,
        SemiColon = 39,
        Colon = 146,
        LBracket = 26,
        RBracket = 27,
        At = 145,
        BackSlash = 43,
        Comma = 51,
        漢字 = 148,
        変換 = 121,
        無変換 = 123,
        かな = 112,
        Apps = 221,
        CapsLock = 58,
        SysRQ = 183,
        Pause = 197,
        LWindows = 219,
        RWindows = 220,
        NumPad_NumLock = 69,
        NumPad_0 = 82,
        NumPad_1 = 79,
        NumPad_2 = 80,
        NumPad_3 = 81,
        NumPad_4 = 75,
        NumPad_5 = 76,
        NumPad_6 = 77,
        NumPad_7 = 71,
        NumPad_8 = 72,
        NumPad_9 = 73,
        NumPad_Multiply = 55,
        NumPad_Add = 78,
        NumPad_Subtract = 74,
        NumPad_Decimal = 83,
        NumPad_Divide = 181,
        NumPad_Enter = 156
    }
}