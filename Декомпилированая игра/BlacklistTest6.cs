using System;
using System.Collections.Generic;

public class BlacklistTest6
{
	private static readonly Dictionary<char, char[][]> _profanity = new Dictionary<char, char[][]>();

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

	internal static bool checkBlackList(string pName)
	{
		string tName = pName.ToLower();
		_unique.Clear();
		_unique.UnionWith(tName);
		_unique.RemoveWhere((char pChar) => !char.IsLetter(pChar));
		string tClean = BlacklistTools.cleanStringAsSpan(tName);
		bool tDoubleCheck = !(tClean == tName);
		Dictionary<char, char[][]> tProfanity = _profanity;
		ReadOnlySpan<char> tNameSpan = MemoryExtensions.AsSpan(tName);
		ReadOnlySpan<char> tCleanSpan = (tDoubleCheck ? MemoryExtensions.AsSpan(tClean) : ((ReadOnlySpan<char>)null));
		foreach (char tChar in _unique)
		{
			if (!tProfanity.TryGetValue(tChar, out var tBlacklisted))
			{
				continue;
			}
			for (int i = 0; i < tBlacklisted.Length; i++)
			{
				ReadOnlySpan<char> tProfaneWord = MemoryExtensions.AsSpan(tBlacklisted[i]);
				if (BlacklistTools.contains(tNameSpan, tProfaneWord))
				{
					return true;
				}
				if (tDoubleCheck && BlacklistTools.contains(tCleanSpan, tProfaneWord))
				{
					return true;
				}
			}
		}
		return false;
	}
}
