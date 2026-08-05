using System.Collections.Generic;

public static class ConsonantSeparator
{
	private static HashSet<char> _consonants = new HashSet<char>
	{
		'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm',
		'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'
	};

	public static void addRandomVowels(StringBuilderPool pString, string[] pPartsToInsert)
	{
		if (pString.Length < 2)
		{
			return;
		}
		pString.ToLowerInvariant();
		int tLastWord = pString.LastIndexOfAny(' ', ',') + 2;
		using ListPool<int> tPossibleLocations = new ListPool<int>(pString.Length);
		for (int i = tLastWord; i < pString.Length; i++)
		{
			if (isConsonant(pString[i - 1]) && isConsonant(pString[i]))
			{
				tPossibleLocations.Add(i);
			}
		}
		if (tPossibleLocations.Count != 0)
		{
			int tRandomLocation = OnomasticsLibrary.GetRandom(tPossibleLocations);
			string tNewPartToInsert = OnomasticsLibrary.GetRandom(pPartsToInsert);
			pString.Insert(tRandomLocation, tNewPartToInsert);
		}
	}

	public static ListPool<int> findAllConsonants(StringBuilderPool pString, int pStart, int pLength)
	{
		ListPool<int> tConsonants = new ListPool<int>(pLength);
		for (int i = pStart; i < pStart + pLength; i++)
		{
			if (isConsonant(pString[i]))
			{
				tConsonants.Add(i);
			}
		}
		return tConsonants;
	}

	public static ListPool<int> findAllSingleConsonants(StringBuilderPool pString, int pStart, int pLength)
	{
		ListPool<int> tConsonants = new ListPool<int>(pLength);
		for (int i = pStart; i < pStart + pLength; i++)
		{
			if (isConsonant(pString[i]) && (i <= 0 || !isConsonant(pString[i - 1])) && (i >= pString.Length - 1 || !isConsonant(pString[i + 1])))
			{
				tConsonants.Add(i);
			}
		}
		return tConsonants;
	}

	public static bool isConsonant(char pChar)
	{
		pChar = char.ToLowerInvariant(pChar);
		if (_consonants.Contains(pChar))
		{
			return true;
		}
		if (!char.IsLetter(pChar))
		{
			return false;
		}
		return !VowelSeparator.isVowel(pChar);
	}
}
