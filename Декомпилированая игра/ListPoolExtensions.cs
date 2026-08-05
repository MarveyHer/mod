using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Pool;

public static class ListPoolExtensions
{
	private static Random rnd => Randy.rnd;

	public static string ToJson(this ListPool<string> list)
	{
		if (list.Count == 0)
		{
			return "[]";
		}
		return "['" + string.Join("','", list) + "']";
	}

	public static void ShuffleHalf<T>(this ListPool<T> list)
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

	public static void ShuffleN<T>(this ListPool<T> list, int pItems)
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
	public static void Shuffle<T>(this ListPool<T> list)
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
	public static void ShuffleOne<T>(this ListPool<T> list)
	{
		if (list.Count >= 2)
		{
			list.Swap(0, rnd.Next(0, list.Count));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleOne<T>(this ListPool<T> list, int nItem)
	{
		if (list.Count >= 2 && list.Count >= nItem + 1)
		{
			list.Swap(nItem, rnd.Next(nItem, list.Count));
		}
	}

	public static void ShuffleLast<T>(this ListPool<T> list)
	{
		if (list.Count >= 2)
		{
			list.Swap(list.Count - 1, rnd.Next(0, list.Count));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Pop<T>(this ListPool<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Shift<T>(this ListPool<T> list)
	{
		T result = list[0];
		list.RemoveAt(0);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T First<T>(this ListPool<T> list)
	{
		return list[0];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T Last<T>(this ListPool<T> list)
	{
		return list[list.Count - 1];
	}

	public static void ShuffleRandomOne<T>(this ListPool<T> list)
	{
		if (list.Count >= 2)
		{
			int i = Randy.randomInt(0, list.Count - 1);
			list.Swap(i, rnd.Next(i, list.Count));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this ListPool<T> list, int i, int j)
	{
		T[] _items = list.GetRawBuffer();
		T tTemp = _items[i];
		_items[i] = _items[j];
		_items[j] = tTemp;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T GetRandom<T>(this ListPool<T> list)
	{
		return list[rnd.Next(0, list.Count)];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveAtSwapBack<T>(this ListPool<T> list, T pObject)
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

	[Pure]
	public static T[] ToArray<T>(this ListPool<T> list)
	{
		T[] tArray = new T[list.Count];
		list.CopyTo(tArray, 0);
		return tArray;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static bool Any<T>(this ListPool<T> list)
	{
		if (list == null)
		{
			return false;
		}
		return list.Count > 0;
	}

	[Pure]
	public static bool SetEquals<T>(this ListPool<T> pList, IEnumerable<T> pOther)
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTimes<T>(this ListPool<T> pList, int pAmount, T pObject)
	{
		for (int i = 0; i < pAmount; i++)
		{
			pList.Add(pObject);
		}
	}

	public static int CountAll<T>(this ListPool<T> pList, Predicate<T> pMatch)
	{
		int tCount = 0;
		for (int i = 0; i < pList.Count; i++)
		{
			if (pMatch(pList[i]))
			{
				tCount++;
			}
		}
		return tCount;
	}

	public static IEnumerable<T> Where<T>(this ListPool<T> pList, Func<T, bool> pPredicate)
	{
		for (int i = 0; i < pList.Count; i++)
		{
			if (pPredicate(pList[i]))
			{
				yield return pList[i];
			}
		}
	}

	[Pure]
	public static bool ValuesEqual<T>(this ListPool<T> pList, ListPool<T> pOther)
	{
		if (pList.Count != pOther.Count)
		{
			return false;
		}
		long longHashCode = pList.GetLongHashCode();
		long tOtherHash = pOther.GetLongHashCode();
		if (longHashCode != tOtherHash)
		{
			return false;
		}
		return true;
	}

	[Pure]
	public static long GetLongHashCode<T>(this ListPool<T> pList)
	{
		long tHash = 0L;
		foreach (ref T p in pList)
		{
			T tItem = p;
			tHash += tItem.GetHashCode();
		}
		return tHash;
	}

	public static string AsString<T>(this ListPool<T> pListPool)
	{
		return pListPool.ToArray().AsString();
	}
}
