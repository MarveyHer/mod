using System.Collections.Generic;
using UnityEngine;

public static class HotkeysLocalized
{
	private static Dictionary<KeyCode, string> _dictionary;

	private static void init()
	{
		_dictionary = new Dictionary<KeyCode, string>();
		_dictionary.Add(KeyCode.Alpha0, "0");
		_dictionary.Add(KeyCode.Alpha1, "1");
		_dictionary.Add(KeyCode.Alpha2, "2");
		_dictionary.Add(KeyCode.Alpha3, "3");
		_dictionary.Add(KeyCode.Alpha4, "4");
		_dictionary.Add(KeyCode.Alpha5, "5");
		_dictionary.Add(KeyCode.Alpha6, "6");
		_dictionary.Add(KeyCode.Alpha7, "7");
		_dictionary.Add(KeyCode.Alpha8, "8");
		_dictionary.Add(KeyCode.Alpha9, "9");
		_dictionary.Add(KeyCode.Keypad0, "0");
		_dictionary.Add(KeyCode.Keypad1, "1");
		_dictionary.Add(KeyCode.Keypad2, "2");
		_dictionary.Add(KeyCode.Keypad3, "3");
		_dictionary.Add(KeyCode.Keypad4, "4");
		_dictionary.Add(KeyCode.Keypad5, "5");
		_dictionary.Add(KeyCode.Keypad6, "6");
		_dictionary.Add(KeyCode.Keypad7, "7");
		_dictionary.Add(KeyCode.Keypad8, "8");
		_dictionary.Add(KeyCode.Keypad9, "9");
		_dictionary.Add(KeyCode.Space, "SPACE");
		_dictionary.Add(KeyCode.LeftShift, "SHIFT");
		_dictionary.Add(KeyCode.RightShift, "SHIFT");
		_dictionary.Add(KeyCode.LeftAlt, "ALT");
		_dictionary.Add(KeyCode.RightAlt, "ALT");
		_dictionary.Add(KeyCode.LeftControl, "CONTROL");
		_dictionary.Add(KeyCode.RightControl, "CONTROL");
		_dictionary.Add(KeyCode.LeftMeta, "");
		_dictionary.Add(KeyCode.RightMeta, "");
	}

	public static string getLocalizedKey(KeyCode pCode)
	{
		if (_dictionary == null)
		{
			init();
		}
		if (pCode == KeyCode.None)
		{
			return string.Empty;
		}
		string tResult = string.Empty;
		tResult = ((!_dictionary.ContainsKey(pCode)) ? pCode.ToString() : _dictionary[pCode]);
		if (string.IsNullOrEmpty(tResult))
		{
			return string.Empty;
		}
		return Toolbox.coloredText(tResult, "#95DD5D");
	}
}
