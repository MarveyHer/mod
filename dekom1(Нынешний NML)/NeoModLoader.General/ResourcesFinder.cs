using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeoModLoader.General;

public static class ResourcesFinder
{
	private static Dictionary<Type, Dictionary<string, Object>> objects_cache = new Dictionary<Type, Dictionary<string, Object>>();

	public static T[] FindResources<T>(string name) where T : Object
	{
		T[] array = Resources.FindObjectsOfTypeAll<T>();
		List<T> list = new List<T>(array.Length / 16);
		string text = name.ToLower();
		T[] array2 = array;
		foreach (T val in array2)
		{
			if (((Object)val).name.ToLower() == text)
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}

	public static T FindResource<T>(string name) where T : Object
	{
		string text = name.ToLower();
		if (objects_cache.TryGetValue(typeof(T), out var value))
		{
			if (value.TryGetValue(text, out var value2))
			{
				return (T)(object)value2;
			}
		}
		else
		{
			value = new Dictionary<string, Object>();
			objects_cache.Add(typeof(T), value);
		}
		T[] array = Resources.FindObjectsOfTypeAll<T>();
		T[] array2 = array;
		foreach (T val in array2)
		{
			if (((Object)val).name.ToLower() == text)
			{
				T val2 = Object.Instantiate<T>(val, WorldBoxMod.InactiveTransform);
				((Object)val2).name = ((Object)val).name;
				value.Add(text, (Object)(object)val2);
				return val;
			}
		}
		return default(T);
	}
}
