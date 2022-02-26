using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaDrop;

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
		EKey
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
				if (Key.IsPushing((EKey)i) && i != (int)EKey.Esc && i != (int)EKey.Enter && i != (int)EKey.NumPad_Enter)
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
			return (EKey)Value;
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

	public class OptionEKey : Option
	{
		public List<int> Value, Preset;
		public bool Selecting;

		public OptionEKey()
		{
			Type = OptionType.EKey;
			Value = new List<int>();
			Preset = new List<int>();
			Selecting = false;
		}
		public OptionEKey(string name, List<int> value, string info)
			: this()
		{
			EKeyInit(name, value, info);
		}
		public override void Enter()
		{
			Selecting = !Selecting;
		}
		public override void Add()
		{
			for (int i = 0; i < 256; i++)
			{
				if (Key.IsPushing((EKey)i) && i != (int)EKey.Esc && i != (int)EKey.Enter && i != (int)EKey.NumPad_Enter)
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
		public void EKeyInit(string name, List<int> value, string info)
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
				str = $"{(EKey)Value[0]}";
				if (Value.Count > 1)
				{
					for (int i = 1; i < Value.Count; i++)
					{
						str = $"{str} : {(EKey)Value[i]}";

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
}
