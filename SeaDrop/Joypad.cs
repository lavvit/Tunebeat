using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;

namespace SeaDrop
{
    public class Joypad
    {
        public static void Init()
        {
            for (int i = 0; i < 4; i++)
            {
                Pads[i] = new Pad();
                Pads[i].Buttons = new int[32];
            }
        }

        public static void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                int pad = GetJoypadInputState(i + 1);
                for (int j = 0; j < Pads[i].Buttons.Length; j++)
                {
                    if ((pad & (int)GetButtonFromIndex(j)) != 0)
                    {
                        // 押してる
                        if (Pads[i].Buttons[j] <= 0)
                        {
                            // 押してない状態から、押してる状態になった
                            Pads[i].Buttons[j] = 1;
                        }
                        else
                        {
                            // 押してる状態が継続してる
                            Pads[i].Buttons[j] = 2;
                        }
                    }
                    else
                    {
                        // 押してない
                        if (Pads[i].Buttons[j] >= 1)
                        {
                            // 押してる状態から、離した状態になった
                            Pads[i].Buttons[j] = -1;
                        }
                        else
                        {
                            // 押してない状態が継続してる
                            Pads[i].Buttons[j] = 0;
                        }
                    }
                }
                
            }
        }

        public static int Amount()
        {
            return GetJoypadNum();
        }

        public static bool IsPushed(int number, JoypadButton pad)
        {
            if (Pads[number] != null && Pads[number].Buttons != null)
                return Pads[number].Buttons[GetIndexFromButton(pad)] == 1;
            else return false;
        }

        public static bool IsPushing(int number, JoypadButton pad)
        {
            if (Pads[number] != null && Pads[number].Buttons != null)
                return Pads[number].Buttons[GetIndexFromButton(pad)] > 0;
            else return false;
        }

        public static bool IsLeft(int number, JoypadButton pad)
        {
            if (Pads[number] != null && Pads[number].Buttons != null)
                return Pads[number].Buttons[GetIndexFromButton(pad)] == -1;
            else return false;
        }

        public static int GetIndexFromButton(JoypadButton button)
        {
            switch (button)
            {
                case JoypadButton.Down:
                    return 0;
                case JoypadButton.Left:
                    return 1;
                case JoypadButton.Right:
                    return 2;
                case JoypadButton.Up:
                    return 3;
                case JoypadButton.Pad_1:
                    return 4;
                case JoypadButton.Pad_2:
                    return 5;
                case JoypadButton.Pad_3:
                    return 6;
                case JoypadButton.Pad_4:
                    return 7;
                case JoypadButton.Pad_5:
                    return 8;
                case JoypadButton.Pad_6:
                    return 9;
                case JoypadButton.Pad_7:
                    return 10;
                case JoypadButton.Pad_8:
                    return 11;
                case JoypadButton.Pad_9:
                    return 12;
                case JoypadButton.Pad_10:
                    return 13;
                case JoypadButton.Pad_11:
                    return 14;
                case JoypadButton.Pad_12:
                    return 15;
                case JoypadButton.Pad_13:
                    return 16;
                case JoypadButton.Pad_14:
                    return 17;
                case JoypadButton.Pad_15:
                    return 18;
                case JoypadButton.Pad_16:
                    return 19;
                case JoypadButton.Pad_17:
                    return 20;
                case JoypadButton.Pad_18:
                    return 21;
                case JoypadButton.Pad_19:
                    return 22;
                case JoypadButton.Pad_20:
                    return 23;
                case JoypadButton.Pad_21:
                    return 24;
                case JoypadButton.Pad_22:
                    return 25;
                case JoypadButton.Pad_23:
                    return 26;
                case JoypadButton.Pad_24:
                    return 27;
                case JoypadButton.Pad_25:
                    return 28;
                case JoypadButton.Pad_26:
                    return 29;
                case JoypadButton.Pad_27:
                    return 30;
                case JoypadButton.Pad_28:
                    return 31;

                default:
                    return 0;
            }
        }
        public static JoypadButton GetButtonFromIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return JoypadButton.Down;
                case 1:
                    return JoypadButton.Left;
                case 2:
                    return JoypadButton.Right;
                case 3:
                    return JoypadButton.Up;
                case 4:
                    return JoypadButton.Pad_1;
                case 5:
                    return JoypadButton.Pad_2;
                case 6:
                    return JoypadButton.Pad_3;
                case 7:
                    return JoypadButton.Pad_4;
                case 8:
                    return JoypadButton.Pad_5;
                case 9:
                    return JoypadButton.Pad_6;
                case 10:
                    return JoypadButton.Pad_7;
                case 11:
                    return JoypadButton.Pad_8;
                case 12:
                    return JoypadButton.Pad_9;
                case 13:
                    return JoypadButton.Pad_10;
                case 14:
                    return JoypadButton.Pad_11;
                case 15:
                    return JoypadButton.Pad_12;
                case 16:
                    return JoypadButton.Pad_13;
                case 17:
                    return JoypadButton.Pad_14;
                case 18:
                    return JoypadButton.Pad_15;
                case 19:
                    return JoypadButton.Pad_16;
                case 20:
                    return JoypadButton.Pad_17;
                case 21:
                    return JoypadButton.Pad_18;
                case 22:
                    return JoypadButton.Pad_19;
                case 23:
                    return JoypadButton.Pad_20;
                case 24:
                    return JoypadButton.Pad_21;
                case 25:
                    return JoypadButton.Pad_22;
                case 26:
                    return JoypadButton.Pad_23;
                case 27:
                    return JoypadButton.Pad_24;
                case 28:
                    return JoypadButton.Pad_25;
                case 29:
                    return JoypadButton.Pad_26;
                case 30:
                    return JoypadButton.Pad_27;
                case 31:
                    return JoypadButton.Pad_28;

                default:
                    return 0;
            }
        }

        public static Pad[] Pads = new Pad[4];
    }

    public class Pad
    {
        public int Name;
        public int[] Buttons;

    }

    public enum JoypadButton
    {
        Down = PAD_INPUT_DOWN,
        Left = PAD_INPUT_LEFT,
        Right = PAD_INPUT_RIGHT,
        Up = PAD_INPUT_UP,
        Pad_1 = PAD_INPUT_1,
        Pad_2 = PAD_INPUT_2,
        Pad_3 = PAD_INPUT_3,
        Pad_4 = PAD_INPUT_4,
        Pad_5 = PAD_INPUT_5,
        Pad_6 = PAD_INPUT_6,
        Pad_7 = PAD_INPUT_7,
        Pad_8 = PAD_INPUT_8,
        Pad_9 = PAD_INPUT_9,
        Pad_10 = PAD_INPUT_10,
        Pad_11 = PAD_INPUT_11,
        Pad_12 = PAD_INPUT_12,
        Pad_13 = PAD_INPUT_13,
        Pad_14 = PAD_INPUT_14,
        Pad_15 = PAD_INPUT_15,
        Pad_16 = PAD_INPUT_16,
        Pad_17 = PAD_INPUT_17,
        Pad_18 = PAD_INPUT_18,
        Pad_19 = PAD_INPUT_19,
        Pad_20 = PAD_INPUT_20,
        Pad_21 = PAD_INPUT_21,
        Pad_22 = PAD_INPUT_22,
        Pad_23 = PAD_INPUT_23,
        Pad_24 = PAD_INPUT_24,
        Pad_25 = PAD_INPUT_25,
        Pad_26 = PAD_INPUT_26,
        Pad_27 = PAD_INPUT_27,
        Pad_28 = PAD_INPUT_28,
    }
}
