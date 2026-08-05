using System;
using System.Collections.Generic;

public class BlacklistTest8
{
	private static readonly Dictionary<int, HashSet<int>> _profanity = new Dictionary<int, HashSet<int>>();

	private static int _min_length = int.MaxValue;

	private static int _max_length = int.MinValue;

	private static readonly Dictionary<int, char[]> _char_arrays = new Dictionary<int, char[]>();

	private static bool _initiated = false;

	public static void init()
	{
		if (!_initiated)
		{
			_initiated = true;
			BlacklistTools.loadProfanityFilter(_profanity, ref _min_length, ref _max_length);
			for (int i = _min_length; i <= _max_length; i++)
			{
				_char_arrays[i] = new char[i];
			}
		}
	}

	private static int getCharHashCode(char[] pChar)
	{
		return BlacklistTools.getCharHashCode(pChar);
	}

	internal static bool checkBlackList(string pName)
	{
		ReadOnlySpan<char> tNameSpan = MemoryExtensions.AsSpan(pName.ToLower());
		ReadOnlySpan<char> tCleanSpan = BlacklistTools.cleanSpan(tNameSpan);
		bool tDoubleCheck = !(tCleanSpan == tNameSpan);
		for (int i = _min_length; i <= _max_length; i++)
		{
			char[] tCheck = _char_arrays[i];
			HashSet<int> tProfanity = _profanity[i];
			for (int j = 0; j < tNameSpan.Length - i + 1; j++)
			{
				tNameSpan.Slice(j, i).CopyTo(tCheck);
				int tHash = getCharHashCode(tCheck);
				if (tProfanity.Contains(tHash))
				{
					return true;
				}
				if (tDoubleCheck && tCleanSpan.Length >= j + i)
				{
					tCleanSpan.Slice(j, i).CopyTo(tCheck);
					tHash = getCharHashCode(tCheck);
					if (tProfanity.Contains(tHash))
					{
						return true;
					}
				}
			}
		}
		return false;
	}
}
