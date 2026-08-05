using System;
using System.Collections.Generic;
using UnityEngine;

public class Bench
{
	public static bool bench_enabled = false;

	public static bool bench_ai_enabled = false;

	private static Dictionary<string, BenchmarkGroup> dict = new Dictionary<string, BenchmarkGroup>();

	private static float _timer_flatten = 0f;

	public static void update()
	{
		if (bench_enabled)
		{
			finishSplitBenchmarkGroupAI();
			finishSplitBenchmarkGroup("effects_traits");
			finishSplitBenchmarkGroup("effects_items");
			if (_timer_flatten > 0f)
			{
				_timer_flatten -= Time.deltaTime;
				return;
			}
			_timer_flatten = 0.05f;
			flatten("effects_traits");
			flatten("effects_items");
		}
	}

	private static void flatten(string pID)
	{
		if (dict.TryGetValue(pID, out var tGroup))
		{
			tGroup.flatten();
		}
	}

	private static void finishSplitBenchmarkGroupAI()
	{
		DebugConfig.isOn(DebugOption.BenchAiEnabled);
	}

	private static void finishSplitBenchmarkGroup(string pID)
	{
		if (!dict.TryGetValue(pID, out var tGroup))
		{
			return;
		}
		double tTotal = 0.0;
		foreach (ToolBenchmarkData tData in tGroup.dict_data.Values)
		{
			tTotal += tData.latest_result;
			tData.saveAverageCounter();
		}
		benchSaveSplit(pID, tTotal, 1, "game_total");
	}

	public static void saveAverageCounter(string pID, string pGroup)
	{
		get(pID, pGroup).saveAverageCounter();
	}

	public static BenchmarkGroup getGroup(string pID)
	{
		if (dict.ContainsKey(pID))
		{
			return dict[pID];
		}
		BenchmarkGroup tGroup = new BenchmarkGroup();
		tGroup.id = pID;
		dict.Add(pID, tGroup);
		return tGroup;
	}

	private static ToolBenchmarkData get(string pID, string pGroupID = "main", bool pNew = true)
	{
		if (!dict.TryGetValue(pGroupID, out var tGroup))
		{
			tGroup = new BenchmarkGroup();
			tGroup.id = pGroupID;
			dict.Add(pGroupID, tGroup);
		}
		if (!tGroup.dict_data.TryGetValue(pID, out var tData) && pNew)
		{
			tData = new ToolBenchmarkData();
			tData.id = pID;
			tGroup.dict_data.Add(pID, tData);
		}
		return tData;
	}

	public static void clearBenchmarkEntrySkipMultiple(string pGroupID = "main", params string[] pEntries)
	{
		foreach (string pID in pEntries)
		{
			bench(pID, pGroupID);
			benchEnd(pID, pGroupID, pSaveCounter: false, 0L);
		}
	}

	public static void clearBenchmarkEntrySkip(string pID, string pGroupID = "main")
	{
		bench(pID, pGroupID);
		benchEnd(pID, pGroupID, pSaveCounter: false, 0L);
	}

	public static double bench(string pID, string pGroupID = "main", bool pForce = false)
	{
		if (!(bench_enabled || pForce))
		{
			return 0.0;
		}
		ToolBenchmarkData toolBenchmarkData = get(pID, pGroupID);
		double tTime = Time.realtimeSinceStartupAsDouble;
		toolBenchmarkData.start(tTime);
		return tTime;
	}

	public static double benchEnd(string pID, string pGroupID = "main", bool pSaveCounter = false, long pCounter = 0L, bool pForce = false)
	{
		if (!(bench_enabled || pForce))
		{
			return 0.0;
		}
		ToolBenchmarkData tData = get(pID, pGroupID);
		double tTime = Time.realtimeSinceStartupAsDouble - tData.latest_time;
		tData.end(tTime);
		if (pSaveCounter)
		{
			tData.newCount(pCounter);
			tData.saveAverageCounter();
		}
		return tTime;
	}

	public static void benchSet(string pID, double pVal, int pCounter, string pGroupID = "main")
	{
		if (bench_enabled)
		{
			benchSave(pID, pVal, pCounter, pGroupID);
			saveAverageCounter(pID, pGroupID);
		}
	}

	public static void benchSetValue(string pID, int pValue, string pGroupID = "main")
	{
		if (bench_enabled)
		{
			get(pID, pGroupID).newValue(pValue);
		}
	}

	public static int getBenchValue(string pID, string pGroupID = "main")
	{
		if (!bench_enabled)
		{
			return 0;
		}
		return (int)get(pID, pGroupID).debug_value;
	}

	public static double benchSave(string pID, double pValue, int pCounter, string pGroupID = "main")
	{
		if (!bench_enabled)
		{
			return 0.0;
		}
		ToolBenchmarkData toolBenchmarkData = get(pID, pGroupID);
		toolBenchmarkData.end(pValue);
		toolBenchmarkData.newCount(pCounter);
		return pValue;
	}

	public static double benchSaveSplit(string pID, double pValue, int pCounter, string pGroupID = "main")
	{
		if (!bench_enabled)
		{
			return 0.0;
		}
		ToolBenchmarkData toolBenchmarkData = get(pID, pGroupID);
		toolBenchmarkData.end(pValue);
		toolBenchmarkData.newCount(pCounter);
		return pValue;
	}

	public static string getBenchResult(string pID, string pGroupID = "main", bool pAverage = true)
	{
		return getBenchResultAsDouble(pID, pGroupID, pAverage).ToString("##,0.#######");
	}

	public static double getBenchResultAsDouble(string pID, string pGroupID = "main", bool pAverage = true)
	{
		ToolBenchmarkData tData = get(pID, pGroupID, pNew: false);
		if (tData == null)
		{
			return -1.0;
		}
		if (pAverage)
		{
			return tData.getAverage();
		}
		return tData.latest_result;
	}

	public static string printableBenchResults(string pGroupID = "main", bool pAverage = false, params string[] pID)
	{
		double[] tResults = new double[pID.Length];
		double tMax = 0.0;
		double tMin = double.MaxValue;
		for (int i = 0; i < pID.Length; i++)
		{
			tResults[i] = getBenchResultAsDouble(pID[i], pGroupID, pAverage);
			if (tResults[i] > tMax)
			{
				tMax = tResults[i];
			}
			if (tResults[i] < tMin)
			{
				tMin = tResults[i];
			}
		}
		Array.Sort(tResults, pID);
		using ListPool<string[]> tRow = new ListPool<string[]>();
		tRow.Add(new string[5] { "ID", "TIME", "PERCENT", "WINNER", "BAR" });
		tRow.Add(new string[0]);
		for (int j = 0; j < pID.Length; j++)
		{
			double tPercent = tResults[j] / tMax;
			bool tWinner = tResults[j].Equals(tMin);
			bool tLoser = tResults[j].Equals(tMax);
			string tPrefix = "";
			string tSuffix = "";
			string tBar = "";
			int tBars = (int)(tPercent * 10.0);
			for (int k = 0; k < tBars; k++)
			{
				tBar += "■";
			}
			tBar = Toolbox.fillRight(tBar, 10);
			if (tWinner || tLoser)
			{
				if (tWinner)
				{
					tPrefix = "<color=green>";
				}
				if (tLoser)
				{
					tPrefix = "<color=red>";
				}
				tSuffix = "</color>";
			}
			string tID = tPrefix + pID[j] + tSuffix;
			string tResult = tPrefix + tResults[j].ToString("F7") + tSuffix;
			string tPercentStr = tPrefix + tPercent.ToString("P0") + tSuffix;
			string tWinnerStr = tPrefix + (tWinner ? "WINNER" : (tLoser ? "SLOWEST" : "")) + tSuffix;
			string tBarStr = tPrefix + tBar + tSuffix;
			tRow.Add(new string[5] { tID, tResult, tPercentStr, tWinnerStr, tBarStr });
		}
		return Toolbox.printRows(tRow);
	}

	public static void printBenchResult(string pID, string pGroupID = "main", bool pAverage = false)
	{
		double tResultFloat = getBenchResultAsDouble(pID, pGroupID, pAverage);
		string tResult = tResultFloat.ToString("##,0.##########");
		if (tResultFloat > 0.3)
		{
			tResult = "<color=red>" + tResult + "</color>";
		}
		else if (tResultFloat > 0.1)
		{
			tResult = "<color=yellow>" + tResult + "</color>";
		}
		Debug.Log("#benchmark: <color=white>" + pID + "</color>: " + tResult);
	}
}
