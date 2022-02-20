using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amaoto;

namespace Tunebeat.Config
{
    public class Option
    {
		public OptionType Type;
		
		public string Name;
		public string Info;


		public Option()
		{
			Name = "";
			Info = "";
		}
		public Option(string name, string info)
			: this()
		{
			Init(name, info);
		}

		public virtual void Enter()
		{
		}
		public virtual void Add()
		{
		}
		public virtual void Start()
		{
		}
		public virtual void Up()
		{
		}
		public virtual void Down()
		{
		}
		public virtual void Reset()
		{
		}
		public virtual void Init(string name, string info)
		{
			Name = name;
			Info = info;
		}
		public virtual object objAmount()
		{
			return null;
		}
		public virtual int GetIndex()
		{
			return 0;
		}
		public virtual void SetIndex(object index)
		{
		}
	}
	public enum OptionType
	{
		Bool,
		Int,
		List,
		Double,
		String,
		StrList,
		Key,
		KeyList
	}

	public class OptionBool : Option
    {
		public bool ON;

		public OptionBool()
		{
			Type = OptionType.Bool;
			ON = false;
		}
		public OptionBool(string name, bool start, string info)
			: this()
		{
			BoolInit(name, start, info);
		}
		public override void Enter()
		{
			Up();
		}
		public override void Up()
		{
			ON = !ON;
		}
		public override void Down()
		{
			Up();
		}
		public void BoolInit(string name, bool start, string info)
		{
			Init(name, info);
			ON = start;
		}
		public override object objAmount()
		{
			return ON ? "ON" : "OFF";
		}
		public override int GetIndex()
		{
			return ON ? 1 : 0;
		}
		public override void SetIndex(object index)
		{
			switch (index)
			{
				case 0:
					ON = false;
					break;
				case 1:
					ON = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public class OptionInt : Option
	{
		public int Value, Max, Min, Preset;
		public bool Selecting;

		public OptionInt()
		{
			Type = OptionType.Int;
			Value = 0;
			Max = 0;
			Min = 0;
			Preset = 0;
			Selecting = false;
		}
		public OptionInt(string name, int value, int min, int max, string info)
			: this()
		{
			IntInit(name, value, max, min, info);
		}
		public override void Enter()
		{
			Selecting = !Selecting;
		}
		public override void Up()
		{
			if (++Value > Max)
			{
				Value = Max;
			}
		}
		public override void Down()
		{
			if (--Value < Min)
			{
				Value = Min;
			}
		}
		public override void Reset()
		{
			Value = Preset;
		}
		public void IntInit(string name, int value, int max, int min, string info)
		{
			Init(name, info);
			Value = value;
			Max = max;
			Min = min;
			Preset = value;
			Selecting = false;
		}
		public override object objAmount()
		{
			return Value;
		}
		public override int GetIndex()
		{
			return Value;
		}
		public override void SetIndex(object index)
		{
			Value = (int)index;
		}
	}

	public class OptionList : Option
	{
		public List<string> List;
		public int Value, Preset;
		public bool Selecting;

		public OptionList()
		{
			Type = OptionType.Int;
			Value = 0;
			Preset = 0;
			List = new List<string>();
		}
		public OptionList(string name, int start, string info, params string[] list)
			: this()
		{
			ListInit(name, start, info, list);
		}
		public override void Enter()
		{
			Selecting = !Selecting;
		}
		public override void Up()
		{
			if (--Value < 0)
			{
				Value = List.Count - 1;
			}
		}
		public override void Down()
		{
			if (++Value >= List.Count)
			{
				Value = 0;
			}
		}
		public override void Reset()
		{
			Value = Preset;
		}
		public void ListInit(string name, int start, string info, params string[] list)
		{
			Init(name, info);
			Value = start;
			Preset = start;
			foreach (string str in list)
			{
				List.Add(str);
			}
		}
		public override object objAmount()
		{
			return List[Value];
		}
		public override int GetIndex()
		{
			return Value;
		}
		public override void SetIndex(object index)
		{
			Value = (int)index;
		}
	}

	public class OptionDouble : Option
	{
		public double Value, Max, Min, Preset;
		public bool Selecting;

		public OptionDouble()
		{
			Type = OptionType.Int;
			Value = 0;
			Max = 0;
			Min = 0;
			Preset = 0;
			Selecting = false;
		}
		public OptionDouble(string name, double value, double min, double max, string info)
			: this()
		{
			DoubleInit(name, value, max, min, info);
		}
		public override void Enter()
		{
			Selecting = !Selecting;
		}
		public override void Up()
		{
			Value += 0.01;
			Value = Math.Round(Value, 2, MidpointRounding.AwayFromZero);
			if (Value > Max)
			{
				Value = Max;
			}
		}
		public override void Down()
		{
			Value -= 0.01;
			Value = Math.Round(Value, 2, MidpointRounding.AwayFromZero);
			if (Value < Min)
			{
				Value = Min;
			}
		}
		public override void Reset()
		{
			Value = Preset;
		}
		public void DoubleInit(string name, double value, double max, double min, string info)
		{
			Init(name, info);
			Value = value;
			Max = max;
			Min = min;
			Preset = value;
			Selecting = false;
		}
		public override object objAmount()
		{
			return Value;
		}
		public override int GetIndex()
		{
			return (int)Value;
		}
		public override void SetIndex(object index)
		{
			Value = (double)index;
		}
	}

	public class OptionString : Option
	{
		public string Text, Preset;
		public bool Selecting;

		public OptionString()
		{
			Type = OptionType.String;
			Text = "";
			Preset = "";
			Selecting = false;
		}
		public OptionString(string name, string start, string info)
			: this()
		{
			StringInit(name, start, info);
		}
		public override void Enter()
		{
			Selecting = !Selecting;
			if (Selecting)
            {
				Input.Init();
				Input.Text = Text;
            }
			else
            {
				Text = Input.Text;
				Input.End();
            }
		}
		public override void Up()
		{
		}
		public override void Down()
		{
		}
		public override void Reset()
		{
			Text = Preset;
			Input.End();
		}
		public void StringInit(string name, string start, string info)
		{
			Init(name, info);
			Text = start;
			Preset = start;
			Selecting = false;
		}
		public override object objAmount()
		{
			return Input.IsEnable ? Input.Text : Text;
		}
		public override int GetIndex()
		{
			return !string.IsNullOrEmpty(Text) ? 1 : 0;
		}
		public override void SetIndex(object index)
		{
			Text = $"{index}";
		}
	}

	public class OptionStrList : Option
	{
		public List<string> Text, Preset;
		public string Preview;
		public bool Selecting;
		public int Max;

		public OptionStrList()
		{
			Type = OptionType.StrList;
			Text = new List<string>();
			Preset = new List<string>();
			Preview = "";
			Max = 0;
			Selecting = false;
		}
		public OptionStrList(string name, List<string> start, string info)
			: this()
		{
			StringInit(name, start, info);
		}
		public override void Enter()
		{
			Selecting = !Selecting;
			if (Selecting)
			{
				Input.Init();
				Input.Text = Preview;
			}
			else
			{
				Preview = Input.Text;
				Text = new List<string>();
				string[] strArray = Input.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				if (Max == 0)
                {
					for (int i = 0; i < strArray.Length; i++)
					{
						Text.Add(strArray[i]);
					}
				}
				else
                {
					for (int i = 0; i < strArray.Length && i < Max; i++)
					{
						Text.Add(strArray[i]);
					}
				}
				Input.End();
			}
		}
		public override void Up()
		{
		}
		public override void Down()
		{
		}
		public override void Reset()
		{
			Text = Preset;
			Input.End();
		}
		public void StringInit(string name, List<string> start, string info, int max = 0)
		{
			Init(name, info);
			Text = start;
			Preset = start;
			Max = max;
			foreach (string str in start)
            {
				Preview = Preview + ";" + str;
            }
			Preview = Preview.Remove(0, 1);
			Selecting = false;
		}
		public override object objAmount()
		{
			string str = Input.IsEnable ? Input.Text : Preview;
			//str = str.Replace(";", ";\n    ");
			return str;
		}
		public override int GetIndex()
		{
			return !string.IsNullOrEmpty(Preview) ? 1 : 0;
		}
		public override void SetIndex(object index)
		{
			Preview = $"{index}";
		}
	}

	public class OptionKey : Option
	{
		public int Value, Preset;
		public bool Selecting;

		public OptionKey()
		{
			Type = OptionType.Key;
			Value = 0;
			Preset = 0;
			Selecting = false;
		}
		public OptionKey(string name, int value, string info)
			: this()
		{
			KeyInit(name, value, info);
		}
		public override void Enter()
		{
			for (int i = 0; i < 256; i++)
            {
				if (Key.IsPushing(i) && i != (int)KeyList.Esc && i != (int)KeyList.Enter && i != (int)KeyList.NumPad_Enter)
                {
					Value = i;
				}
            }
			Selecting = !Selecting;
		}
		public override void Up()
		{
		}
		public override void Down()
		{
		}
		public override void Reset()
		{
			Value = Preset;
		}
		public void KeyInit(string name, int value, string info)
		{
			Init(name, info);
			Value = value;
			Preset = value;
			Selecting = false;
		}
		public override object objAmount()
		{
			return (KeyList)Value;
		}
		public override int GetIndex()
		{
			return Value;
		}
		public override void SetIndex(object index)
		{
			Value = (int)index;
		}
	}

	public class OptionKeyList : Option
	{
		public List<int> Value, Preset;
		public bool Selecting;

		public OptionKeyList()
		{
			Type = OptionType.KeyList;
			Value = new List<int>();
			Preset = new List<int>();
			Selecting = false;
		}
		public OptionKeyList(string name, List<int> value, string info)
			: this()
		{
			KeyListInit(name, value, info);
		}
		public override void Enter()
		{
			Selecting = !Selecting;
		}
		public override void Add()
		{
			for (int i = 0; i < 256; i++)
			{
				if (Key.IsPushing(i) && i != (int)KeyList.Esc && i != (int)KeyList.Enter && i != (int)KeyList.NumPad_Enter)
				{
					Value.Add(i);
				}
			}
		}
		public override void Start()
		{
			Value = new List<int>();
		}
		public override void Up()
		{
		}
		public override void Down()
		{
		}
		public override void Reset()
		{
			Value = Preset;
		}
		public void KeyListInit(string name, List<int> value, string info)
		{
			Init(name, info);
			Value = value;
			Preset = value;
			Selecting = false;
		}
		public override object objAmount()
		{
			string str = "";
			if (Value.Count > 0)
            {
				str = $"{(KeyList)Value[0]}";
				if (Value.Count > 1)
				{
					for (int i = 1; i < Value.Count; i++)
					{
						str = $"{str} : {(KeyList)Value[i]}";

					}
				}
			}
			
			return str;
		}
		public override int GetIndex()
		{
			return Value.Count > 0 ? 1 : 0;
		}
		public override void SetIndex(object index)
		{
			Value = (List<int>)index;
		}
	}

	#region KeyList
	public enum KeyList
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
    #endregion
}
