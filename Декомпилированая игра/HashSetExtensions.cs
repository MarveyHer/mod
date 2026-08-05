using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public static class HashSetExtensions
{
	[CanBeNull]
	public static T GetRandom<T>(this HashSet<T> pHashSet)
	{
		int tRandomIndex = Randy.randomInt(0, pHashSet.Count);
		int tIndex = 0;
		foreach (T item in pHashSet)
		{
			if (tIndex++ == tRandomIndex)
			{
				return item;
			}
		}
		return default(T);
	}

	public static T[] ToArray<T>(this HashSet<T> pHashSet)
	{
		T[] tArray = new T[pHashSet.Count];
		pHashSet.CopyTo(tArray);
		return tArray;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Any<T>(this HashSet<T> pHashSet)
	{
		if (pHashSet == null)
		{
			return false;
		}
		return pHashSet.Count > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool RemoveAll<T>(this HashSet<T> pHashSet, ICollection<T> pToRemove)
	{
		if (pToRemove == null)
		{
			throw new ArgumentNullException("pToRemove");
		}
		if (pToRemove.Count == 0 || pHashSet.Count == 0)
		{
			return false;
		}
		int count = pHashSet.Count;
		pHashSet.ExceptWith(pToRemove);
		int tNewCount = pHashSet.Count;
		return count != tNewCount;
	}

	public static T Pop<T>(this HashSet<T> pHashSet)
	{
		if (pHashSet == null)
		{
			throw new ArgumentNullException("pHashSet");
		}
		if (pHashSet.Count == 0)
		{
			throw new InvalidOperationException("Cannot pop from an empty HashSet.");
		}
		int tLastIndex = pHashSet.Count - 1;
		int tIndex = 0;
		foreach (T tItem in pHashSet)
		{
			if (tIndex++ == tLastIndex)
			{
				pHashSet.Remove(tItem);
				return tItem;
			}
		}
		throw new InvalidOperationException("Unexpected error: HashSet is empty after iteration.");
	}

	public static T Shift<T>(this HashSet<T> pHashSet)
	{
		if (pHashSet == null)
		{
			throw new ArgumentNullException("pHashSet");
		}
		if (pHashSet.Count == 0)
		{
			throw new InvalidOperationException("Cannot shift from an empty HashSet.");
		}
		using (HashSet<T>.Enumerator enumerator = pHashSet.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				T tItem = enumerator.Current;
				pHashSet.Remove(tItem);
				return tItem;
			}
		}
		throw new InvalidOperationException("Unexpected error: HashSet is empty after iteration.");
	}
}
