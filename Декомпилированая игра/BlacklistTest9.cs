using System;
using System.Collections.Generic;

public class BlacklistTest9
{
	private static readonly Dictionary<string, string[]> _profanity = new Dictionary<string, string[]>();

	private const int INDEX_LENGTH = 3;

	private static bool _initiated = false;

	public static void init()
	{
		if (!_initiated)
		{
			_initiated = true;
			BlacklistTools.loadProfanityFilter(_profanity);
		}
	}

	internal static bool checkBlackList(string pName)
	{
		ReadOnlySpan<char> tNameSpan = MemoryExtensions.AsSpan(pName.ToLower());
		Dictionary<string, string[]> tProfanity = _profanity;
		for (int j = 0; j < tNameSpan.Length - 3 + 1; j++)
		{
			string tCheckString = tNameSpan.Slice(j, 3).ToString();
			if (!tProfanity.TryGetValue(tCheckString, out var tBlacklisted))
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
			}
		}
		ReadOnlySpan<char> tCleanSpan = BlacklistTools.cleanSpan(tNameSpan);
		if (tCleanSpan == tNameSpan || tCleanSpan.Length <= 2)
		{
			return false;
		}
		for (int k = 0; k < tCleanSpan.Length - 3 + 1; k++)
		{
			string tCheckString2 = tCleanSpan.Slice(k, 3).ToString();
			if (!tProfanity.TryGetValue(tCheckString2, out var tBlacklisted2))
			{
				continue;
			}
			for (int l = 0; l < tBlacklisted2.Length; l++)
			{
				ReadOnlySpan<char> tProfaneWord2 = MemoryExtensions.AsSpan(tBlacklisted2[l]);
				if (BlacklistTools.contains(tCleanSpan, tProfaneWord2))
				{
					return true;
				}
			}
		}
		return false;
	}
}
