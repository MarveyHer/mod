using System.Collections.Generic;

public class BlacklistTest10
{
	private static readonly Dictionary<char, string[]> _profanity = new Dictionary<char, string[]>();

	private static readonly HashSet<char> _unique = new HashSet<char>();

	private static bool _initiated = false;

	public static void init()
	{
		if (!_initiated)
		{
			_initiated = true;
			BlacklistTools.loadProfanityFilter(_profanity, _unique);
		}
	}

	public static bool checkBlackList(string pName)
	{
		string tName = pName.ToLower();
		_unique.Clear();
		_unique.UnionWith(tName);
		_unique.RemoveWhere((char pChar) => !char.IsLetter(pChar));
		string tClean = BlacklistTools.cleanString(tName);
		bool tDoubleCheck = !(tClean == tName);
		Dictionary<char, string[]> tProfanity = _profanity;
		foreach (char tChar in _unique)
		{
			if (!tProfanity.ContainsKey(tChar))
			{
				continue;
			}
			for (int j = 0; j < tProfanity[tChar].Length; j++)
			{
				if (tName.Contains(tProfanity[tChar][j]))
				{
					return true;
				}
				if (tDoubleCheck && tClean.Contains(tProfanity[tChar][j]))
				{
					return true;
				}
			}
		}
		return false;
	}
}
