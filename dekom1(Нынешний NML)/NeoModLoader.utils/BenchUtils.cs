using System.Collections.Generic;
using UnityEngine;

namespace NeoModLoader.utils;

internal static class BenchUtils
{
	private static Dictionary<string, float> bench = new Dictionary<string, float>();

	public static void Start(string key)
	{
		if (!bench.ContainsKey(key))
		{
			bench.Add(key, 0f);
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		bench[key] = realtimeSinceStartup;
	}

	public static float End(string key)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (bench.TryGetValue(key, out var value))
		{
			return realtimeSinceStartup - value;
		}
		return -1f;
	}
}
