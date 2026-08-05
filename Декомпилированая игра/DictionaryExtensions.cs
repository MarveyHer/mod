using System;
using System.Collections.Generic;

public static class DictionaryExtensions
{
	public static int RemoveByValue<TKey, TValue>(this IDictionary<TKey, TValue> pDict, Predicate<TValue> pPredicate)
	{
		using ListPool<TKey> tKeysToRemove = new ListPool<TKey>(pDict.Count);
		foreach (KeyValuePair<TKey, TValue> tPair in pDict)
		{
			if (pPredicate(tPair.Value))
			{
				tKeysToRemove.Add(tPair.Key);
			}
		}
		foreach (ref TKey item in tKeysToRemove)
		{
			TKey tKey = item;
			pDict.Remove(tKey);
		}
		return tKeysToRemove.Count;
	}

	public static int RemoveByKey<TKey, TValue>(this IDictionary<TKey, TValue> pDict, Predicate<TKey> pPredicate)
	{
		using ListPool<TKey> tKeysToRemove = new ListPool<TKey>(pDict.Count);
		foreach (TKey tKey in pDict.Keys)
		{
			if (pPredicate(tKey))
			{
				tKeysToRemove.Add(tKey);
			}
		}
		foreach (ref TKey item in tKeysToRemove)
		{
			TKey tKey2 = item;
			pDict.Remove(tKey2);
		}
		return tKeysToRemove.Count;
	}
}
