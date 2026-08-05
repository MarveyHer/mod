using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public static class ArrayExtensions
{
	private static System.Random rnd => Randy.rnd;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T First<T>(this T[] pArray)
	{
		return pArray[0];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T Last<T>(this T[] pArray)
	{
		return pArray[^1];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static int IndexOf<T>(this T[] pArray, T pValue)
	{
		return Array.IndexOf(pArray, pValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static bool Contains<T>(this T[] pArray, T pValue)
	{
		return Array.IndexOf(pArray, pValue) > -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static int FreeIndex<T>(this T[] pArray)
	{
		return Array.IndexOf(pArray, null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T GetRandom<T>(this T[] pArray)
	{
		return pArray[rnd.Next(0, pArray.Length)];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T GetRandom<T>(this T[] pArray, int pLength)
	{
		return pArray[rnd.Next(0, pLength)];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this T[] pArray, int pIndex1, int pIndex2)
	{
		T temp = pArray[pIndex1];
		pArray[pIndex1] = pArray[pIndex2];
		pArray[pIndex2] = temp;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle<T>(this T[] pArray)
	{
		if (pArray.Length >= 2)
		{
			int tCount = pArray.Length;
			for (int i = 0; i < tCount; i++)
			{
				pArray.Swap(i, rnd.Next(i, tCount));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle<T>(this T[] pArray, int pCount)
	{
		if (pCount >= 2)
		{
			for (int i = 0; i < pCount; i++)
			{
				pArray.Swap(i, rnd.Next(i, pCount));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleOne<T>(this T[] pArray)
	{
		if (pArray.Length >= 2)
		{
			pArray.Swap(0, rnd.Next(0, pArray.Length));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleOne<T>(this T[] pArray, int pItem)
	{
		if (pArray.Length >= 2 && pArray.Length >= pItem + 1)
		{
			pArray.Swap(pItem, rnd.Next(pItem, pArray.Length));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleOne<T>(this T[] pArray, int pItem, int pCount)
	{
		if (pCount >= 2 && pCount >= pItem + 1)
		{
			pArray.Swap(pItem, rnd.Next(pItem, pCount));
		}
	}

	public static void Clear<T>(this T[] pArray)
	{
		Array.Clear(pArray, 0, pArray.Length);
	}

	public static void Clear<T>(this T[] pArray, int pCount)
	{
		Array.Clear(pArray, 0, pCount);
	}

	[Pure]
	public static bool AnyTrue(this bool[] pArray)
	{
		for (int i = 0; i < pArray.Length; i++)
		{
			if (pArray[i])
			{
				return true;
			}
		}
		return false;
	}

	[Pure]
	public static bool AnyFalse(this bool[] pArray)
	{
		for (int i = 0; i < pArray.Length; i++)
		{
			if (!pArray[i])
			{
				return true;
			}
		}
		return false;
	}

	public static string AsString<T>(this T[] pArray)
	{
		if (pArray == null)
		{
			return "";
		}
		using ListPool<string> tStringToPrint = new ListPool<string>(pArray.Length);
		for (int i = 0; i < pArray.Length; i++)
		{
			T tObject = pArray[i];
			tStringToPrint.Add(tObject?.ToString() ?? "null");
		}
		return string.Join(", ", tStringToPrint.ToArray());
	}

	public static void PrintToConsole<T>(this T[] pArray, string pMessage = null)
	{
		if (pArray != null)
		{
			string tStringToPrint = "";
			for (int i = 0; i < pArray.Length; i++)
			{
				T tObject = pArray[i];
				tStringToPrint = tStringToPrint + tObject.ToString() + ",";
			}
			if (tStringToPrint.Length > 0)
			{
				tStringToPrint = tStringToPrint.TrimEnd(',');
			}
			if (pMessage != null)
			{
				Debug.Log(pMessage + ": [" + tStringToPrint + "]");
			}
			else
			{
				Debug.Log(tStringToPrint);
			}
		}
	}

	public static bool AllTrue(this bool[] pArray)
	{
		return !pArray.AnyFalse();
	}

	public static bool AllFalse(this bool[] pArray)
	{
		return !pArray.AnyTrue();
	}
}
