using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;

public static class ListExtensions
{
	private static System.Random rnd => Randy.rnd;

	public static string ToJson(this List<string> list)
	{
		if (list.Count == 0)
		{
			return "[]";
		}
		return "['" + string.Join("','", list) + "']";
	}

	public static void ShuffleHalf<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			int tCount = list.Count;
			int tHalfLength = tCount / 2 + 1;
			for (int i = 0; i < tHalfLength && i < tCount; i += 2)
			{
				list.Swap(i, rnd.Next(i, tCount));
			}
		}
	}

	public static void ShuffleN<T>(this List<T> list, int pItems)
	{
		if (list.Count >= 2)
		{
			int tCount = ((list.Count < pItems) ? list.Count : pItems);
			for (int i = 0; i < tCount; i++)
			{
				list.Swap(i, rnd.Next(i, tCount));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			int tCount = list.Count;
			for (int i = 0; i < tCount; i++)
			{
				list.Swap(i, rnd.Next(i, tCount));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleOne<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			list.Swap(0, rnd.Next(0, list.Count));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleOne<T>(this List<T> list, int nItem)
	{
		if (list.Count >= 2 && list.Count >= nItem + 1)
		{
			list.Swap(nItem, rnd.Next(nItem, list.Count));
		}
	}

	public static void ShuffleLast<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			list.Swap(list.Count - 1, rnd.Next(0, list.Count));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Pop<T>(this List<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Shift<T>(this List<T> list)
	{
		T result = list[0];
		list.RemoveAt(0);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T First<T>(this List<T> list)
	{
		return list[0];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T Last<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static void ShuffleRandomOne<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			int i = Randy.randomInt(0, list.Count - 1);
			list.Swap(i, rnd.Next(i, list.Count));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this List<T> list, int i, int j)
	{
		T temp = list[i];
		list[i] = list[j];
		list[j] = temp;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T GetRandom<T>(this List<T> list)
	{
		return list[rnd.Next(0, list.Count)];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveAtSwapBack<T>(this List<T> list, T pObject)
	{
		int tIndex = list.IndexOf(pObject);
		if (tIndex != -1)
		{
			int tCount = list.Count - 1;
			list[tIndex] = list[tCount];
			list[tCount] = pObject;
			list.RemoveAt(tCount);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Any<T>(this List<T> list)
	{
		if (list == null)
		{
			return false;
		}
		return list.Count > 0;
	}

	[Pure]
	public static bool SetEquals<T>(this List<T> pList, IEnumerable<T> pOther)
	{
		if (pList == null || pOther == null)
		{
			return false;
		}
		HashSet<T> hashSet = CollectionPool<HashSet<T>, T>.Get();
		HashSet<T> tOther = CollectionPool<HashSet<T>, T>.Get();
		hashSet.UnionWith(pList);
		tOther.UnionWith(pOther);
		bool tEquals = hashSet.SetEquals(tOther);
		tOther.Clear();
		hashSet.Clear();
		CollectionPool<HashSet<T>, T>.Release(hashSet);
		CollectionPool<HashSet<T>, T>.Release(tOther);
		return tEquals;
	}

	public static string ToLineString<T>(this List<T> pList, string pSeparator = ",")
	{
		if (pList == null)
		{
			return string.Empty;
		}
		return string.Join(pSeparator, pList);
	}

	public static void PrintToConsole<T>(this List<T> pList)
	{
		if (pList != null)
		{
			Debug.Log(pList.ToLineString());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTimes<T>(this List<T> pList, int pAmount, T pObject)
	{
		for (int i = 0; i < pAmount; i++)
		{
			pList.Add(pObject);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T LoopNext<T>(this List<T> pList, T pObject)
	{
		int tIndex = pList.IndexOf(pObject);
		if (tIndex == -1)
		{
			return pObject;
		}
		tIndex++;
		if (tIndex >= pList.Count)
		{
			tIndex = 0;
		}
		return pList[tIndex];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this List<T> pList)
	{
		ListAccessHelper.ListDataHelper<T> obj = UnsafeUtility.As<List<T>, ListAccessHelper.ListDataHelper<T>>(ref pList);
		int tSize = obj._size;
		T[] tItems = obj._items;
		if ((uint)tSize > (uint)tItems.Length)
		{
			throw new InvalidOperationException("Concurrent operations are not supported.");
		}
		return new Span<T>(tItems, 0, tSize);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T> pList)
	{
		ListAccessHelper.ListDataHelper<T> obj = UnsafeUtility.As<List<T>, ListAccessHelper.ListDataHelper<T>>(ref pList);
		int tSize = obj._size;
		T[] tItems = obj._items;
		if ((uint)tSize > (uint)tItems.Length)
		{
			throw new InvalidOperationException("Concurrent operations are not supported.");
		}
		return new ReadOnlySpan<T>(tItems, 0, tSize);
	}

	public static string AsString<T>(this List<T> pList)
	{
		return pList.ToArray().AsString();
	}
}
