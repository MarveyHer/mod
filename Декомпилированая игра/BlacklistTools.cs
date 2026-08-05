using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public static class BlacklistTools
{
	private static string[] _profanities;

	public static string[] getProfanities()
	{
		if (_profanities != null)
		{
			return _profanities;
		}
		TextAsset obj = Resources.Load("blacklisted_names") as TextAsset;
		string tContent = obj.text;
		Resources.UnloadAsset(obj);
		string[] tProfanityArr = Regex.Split(tContent, "\r\n?|\n", RegexOptions.Singleline);
		using ListPool<string> tListPool = new ListPool<string>(tProfanityArr.Length);
		for (int i = 0; i < tProfanityArr.Length; i++)
		{
			string lString = tProfanityArr[i].Trim().ToLower();
			if (lString.Length != 0)
			{
				tListPool.Add(lString);
			}
		}
		_profanities = tListPool.ToArray();
		return _profanities;
	}

	public static void loadProfanityFilter(Dictionary<char, string[]> pProfanity, HashSet<char> pUnique)
	{
		if (pProfanity != null && pProfanity.Count > 0)
		{
			return;
		}
		try
		{
			Dictionary<char, List<string>> tProfanity = new Dictionary<char, List<string>>();
			string[] profanities = getProfanities();
			foreach (string tString in profanities)
			{
				pUnique.Clear();
				pUnique.UnionWith(tString);
				pUnique.RemoveWhere((char pChar) => !char.IsLetter(pChar));
				foreach (char tChar in pUnique)
				{
					if (!tProfanity.ContainsKey(tChar))
					{
						tProfanity[tChar] = new List<string>();
					}
					tProfanity[tChar].Add(tString);
				}
			}
			foreach (char tChar2 in tProfanity.Keys)
			{
				pProfanity[tChar2] = tProfanity[tChar2].ToArray();
			}
		}
		catch (Exception message)
		{
			Debug.Log("Error when loading blacklist");
			Debug.LogError(message);
		}
	}

	public static void loadProfanityFilter(Dictionary<char, char[][]> pProfanity, HashSet<char> pUnique)
	{
		if (pProfanity != null && pProfanity.Count > 0)
		{
			return;
		}
		try
		{
			Dictionary<char, List<char[]>> tProfanity = new Dictionary<char, List<char[]>>();
			string[] profanities = getProfanities();
			foreach (string tString in profanities)
			{
				pUnique.Clear();
				pUnique.UnionWith(tString);
				pUnique.RemoveWhere((char pChar) => !char.IsLetter(pChar));
				char[] tCharArray = tString.ToCharArray();
				foreach (char tChar in pUnique)
				{
					if (!tProfanity.ContainsKey(tChar))
					{
						tProfanity[tChar] = new List<char[]>();
					}
					tProfanity[tChar].Add(tCharArray);
				}
			}
			foreach (char tChar2 in tProfanity.Keys)
			{
				pProfanity[tChar2] = tProfanity[tChar2].ToArray();
			}
		}
		catch (Exception message)
		{
			Debug.Log("Error when loading blacklist");
			Debug.LogError(message);
		}
	}

	public static void loadProfanityFilter(Dictionary<int, HashSet<int>> pProfanity, ref int pMinLength, ref int pMaxLength)
	{
		if (pProfanity != null && pProfanity.Count > 0)
		{
			return;
		}
		try
		{
			string[] profanities = getProfanities();
			foreach (string tString in profanities)
			{
				if (tString.Length < pMinLength)
				{
					pMinLength = tString.Length;
				}
				if (tString.Length > pMaxLength)
				{
					pMaxLength = tString.Length;
				}
				if (!pProfanity.ContainsKey(tString.Length))
				{
					pProfanity.Add(tString.Length, new HashSet<int>());
				}
				if (!pProfanity[tString.Length].Add(getCharHashCode(tString.ToCharArray())))
				{
					Debug.Log("Duplicate profanity: " + tString);
				}
			}
		}
		catch (Exception message)
		{
			Debug.Log("Error when loading blacklist");
			Debug.LogError(message);
		}
	}

	public static void loadProfanityFilter(Dictionary<string, string[]> pProfanity, int pIndexLength = 3)
	{
		if (pProfanity != null && pProfanity.Count > 0)
		{
			return;
		}
		try
		{
			Dictionary<string, HashSet<string>> tProfanity = new Dictionary<string, HashSet<string>>();
			string[] profanities = getProfanities();
			foreach (string tString in profanities)
			{
				string tID = tString.Substring(0, pIndexLength);
				if (!tProfanity.ContainsKey(tID))
				{
					tProfanity.Add(tID, new HashSet<string>());
				}
				if (!tProfanity[tID].Add(tString))
				{
					Debug.Log("Duplicate profanity: " + tString);
				}
			}
			foreach (KeyValuePair<string, HashSet<string>> tPair in tProfanity)
			{
				pProfanity.Add(tPair.Key, tPair.Value.ToArray());
			}
		}
		catch (Exception message)
		{
			Debug.Log("Error when loading blacklist");
			Debug.LogError(message);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int getCharHashCode(char[] pChar)
	{
		return ((IStructuralEquatable)pChar).GetHashCode((IEqualityComparer)EqualityComparer<char>.Default);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string cleanString(string pString)
	{
		if (string.IsNullOrEmpty(pString))
		{
			return pString;
		}
		string tString = pString[0].ToString();
		for (int j = 0; j < pString.Length - 1; j++)
		{
			if (!pString[j].Equals(pString[j + 1]))
			{
				tString += pString[j + 1];
			}
		}
		return tString;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string cleanStringAsSpan(string pString)
	{
		if (string.IsNullOrEmpty(pString))
		{
			return pString;
		}
		ReadOnlySpan<char> tSpan = MemoryExtensions.AsSpan(pString);
		Span<char> buffer = stackalloc char[tSpan.Length];
		int length = 0;
		buffer[length++] = tSpan[0];
		for (int i = 1; i < tSpan.Length; i++)
		{
			if (tSpan[i] != tSpan[i - 1])
			{
				buffer[length++] = tSpan[i];
			}
		}
		return new string(buffer.Slice(0, length));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> cleanSpan(ReadOnlySpan<char> pSpan)
	{
		if (pSpan.Length == 0)
		{
			return pSpan;
		}
		Span<char> buffer = new char[pSpan.Length];
		int length = 0;
		buffer[length++] = pSpan[0];
		for (int i = 1; i < pSpan.Length; i++)
		{
			if (pSpan[i] != pSpan[i - 1])
			{
				buffer[length++] = pSpan[i];
			}
		}
		return buffer.Slice(0, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool contains(ReadOnlySpan<char> pText, ReadOnlySpan<char> pSearchPattern)
	{
		if (pSearchPattern.Length == 0)
		{
			return true;
		}
		if (pSearchPattern.Length > pText.Length)
		{
			return false;
		}
		char tFirstChar = pSearchPattern[0];
		for (int i = 0; i <= pText.Length - pSearchPattern.Length; i++)
		{
			if (pText[i] != tFirstChar)
			{
				continue;
			}
			bool tMatch = true;
			for (int j = 1; j < pSearchPattern.Length; j++)
			{
				if (pText[i + j] != pSearchPattern[j])
				{
					tMatch = false;
					break;
				}
			}
			if (tMatch)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool contains(ReadOnlySpan<char> pText, ReadOnlySpan<char> pSearchPattern, int pStartIndex)
	{
		if (pSearchPattern.Length == 0)
		{
			return true;
		}
		if (pSearchPattern.Length > pText.Length)
		{
			return false;
		}
		char tFirstChar = pSearchPattern[0];
		for (int i = pStartIndex; i <= pText.Length - pSearchPattern.Length; i++)
		{
			if (pText[i] != tFirstChar)
			{
				continue;
			}
			bool tMatch = true;
			for (int j = 1; j < pSearchPattern.Length; j++)
			{
				if (pText[i + j] != pSearchPattern[j])
				{
					tMatch = false;
					break;
				}
			}
			if (tMatch)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool contains(ref ReadOnlySpan<char> pText, ref ReadOnlySpan<char> pSearchPattern)
	{
		if (pSearchPattern.Length == 0)
		{
			return true;
		}
		if (pSearchPattern.Length > pText.Length)
		{
			return false;
		}
		char tFirstChar = pSearchPattern[0];
		for (int i = 0; i <= pText.Length - pSearchPattern.Length; i++)
		{
			if (pText[i] != tFirstChar)
			{
				continue;
			}
			bool tMatch = true;
			for (int j = 1; j < pSearchPattern.Length; j++)
			{
				if (pText[i + j] != pSearchPattern[j])
				{
					tMatch = false;
					break;
				}
			}
			if (tMatch)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool contains2(ref ReadOnlySpan<char> pText, ref ReadOnlySpan<char> pSearchPattern)
	{
		int tSearchLength = pSearchPattern.Length;
		if (tSearchLength == 0)
		{
			return true;
		}
		int tTextLength = pText.Length;
		if (tSearchLength > tTextLength)
		{
			return false;
		}
		int i = 0;
		for (int length = tTextLength - tSearchLength; i <= length; i++)
		{
			if (pText.Slice(i, tSearchLength).SequenceEqual(pSearchPattern))
			{
				return true;
			}
		}
		return false;
	}
}
