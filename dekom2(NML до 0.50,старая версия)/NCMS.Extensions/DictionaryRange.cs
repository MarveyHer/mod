using System;
using System.Collections.Generic;
using MonoMod.Utils;

namespace NCMS.Extensions;

public static class DictionaryRange
{
	public static void AddRangeOverride<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
	{
		foreach (TKey key in dicToAdd.Keys)
		{
			dic[key] = dicToAdd[key];
		}
	}

	public static void AddRangeNewOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
	{
		foreach (TKey key in dicToAdd.Keys)
		{
			if (!dic.ContainsKey(key))
			{
				dic[key] = dicToAdd[key];
			}
		}
	}

	public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
	{
		Extensions.AddRange<TKey, TValue>(dic, dicToAdd);
	}

	public static bool ContainsKeys<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
	{
		foreach (TKey key in keys)
		{
			if (!dic.ContainsKey(key))
			{
				return false;
			}
		}
		return true;
	}

	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T item in source)
		{
			action(item);
		}
	}

	public static void ForEachOrBreak<T>(this IEnumerable<T> source, Func<T, bool> func)
	{
		foreach (T item in source)
		{
			if (func(item))
			{
				break;
			}
		}
	}
}
