using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPools;

public static class GraphHelpers
{
	public static string getCategoryName(string pCategory)
	{
		if (!pCategory.Contains('|'))
		{
			return pCategory;
		}
		return pCategory.Split('|')[0];
	}

	public static ListPool<string> bestCategories(Dictionary<string, MinMax> pCategoryStats)
	{
		Dictionary<string, AvgStats> tCategoryStats = UnsafeCollectionPool<Dictionary<string, AvgStats>, KeyValuePair<string, AvgStats>>.Get();
		foreach (KeyValuePair<string, MinMax> tEntry in pCategoryStats)
		{
			string tCategoryName = getCategoryName(tEntry.Key);
			MinMax tMinMax = tEntry.Value;
			if (!tCategoryStats.TryGetValue(tCategoryName, out var tStats))
			{
				tStats = new AvgStats(0.0, 0, tCategoryName);
			}
			tCategoryStats[tCategoryName] = tStats.add(tMinMax.max);
		}
		using ListPool<AvgStats> tSortedCategories = new ListPool<AvgStats>(tCategoryStats.Values);
		UnsafeCollectionPool<Dictionary<string, AvgStats>, KeyValuePair<string, AvgStats>>.Release(tCategoryStats);
		tSortedCategories.Sort(delegate(AvgStats a, AvgStats b)
		{
			int num = b.count.CompareTo(a.count);
			return (num == 0) ? b.avg.CompareTo(a.avg) : num;
		});
		int tLimit = Math.Min(3, tSortedCategories.Count);
		ListPool<string> tTopThreeCategories = new ListPool<string>(tLimit);
		for (int i = 0; i < tLimit; i++)
		{
			if (i <= 0 || (!(tSortedCategories[i].avg <= 3.0) && tSortedCategories[i].count >= tSortedCategories[0].count))
			{
				tTopThreeCategories.Add(tSortedCategories[i].name);
			}
		}
		return tTopThreeCategories;
	}

	public static string horizontalFormatYears(double pValue, int pDigits)
	{
		return Toolbox.formatNumber((long)(pValue - (double)Date.getCurrentYear()) * -1) + "\n" + pValue.ToText();
	}

	public static string verticalFormat(double pValue, int pDigits)
	{
		MinMax tMinMax = GraphController.min_max;
		double num = Math.Abs(pValue);
		string tResult = null;
		tResult = ((!(num < 1000.0)) ? Toolbox.formatNumber((long)pValue) : pValue.ToString("N" + pDigits));
		if (pValue == 0.0)
		{
			return Toolbox.coloredText(tResult, "#FFBC66");
		}
		if (pValue < 0.0)
		{
			string tColor = Toolbox.colorBetween(pValue, tMinMax.min, 0.0, "#FF637D", "#FFBC66");
			return Toolbox.coloredText(tResult, tColor);
		}
		string tColor2 = Toolbox.colorBetween(pValue, 0.0, tMinMax.max, "#FFBC66", "#F3961F");
		return Toolbox.coloredText(tResult, tColor2);
	}

	public static long calculateNiceMaxAxisSize(double pLargestValue)
	{
		if (pLargestValue < 5.0)
		{
			return 5L;
		}
		if (pLargestValue < 8.0)
		{
			return 8L;
		}
		if (pLargestValue < 10.0)
		{
			return 10L;
		}
		if (pLargestValue < 20.0)
		{
			return 20L;
		}
		if (pLargestValue < 30.0)
		{
			return 30L;
		}
		if (pLargestValue < 40.0)
		{
			return 40L;
		}
		if (pLargestValue < 50.0)
		{
			return 50L;
		}
		if (pLargestValue < 60.0)
		{
			return 60L;
		}
		if (pLargestValue < 80.0)
		{
			return 80L;
		}
		if (pLargestValue < 100.0)
		{
			return 100L;
		}
		if (pLargestValue < 120.0)
		{
			return 120L;
		}
		if (pLargestValue < 140.0)
		{
			return 140L;
		}
		if (pLargestValue < 160.0)
		{
			return 160L;
		}
		if (pLargestValue < 180.0)
		{
			return 180L;
		}
		if (pLargestValue < 200.0)
		{
			return 200L;
		}
		if (pLargestValue < 240.0)
		{
			return 240L;
		}
		if (pLargestValue < 280.0)
		{
			return 280L;
		}
		if (pLargestValue < 300.0)
		{
			return 300L;
		}
		if (pLargestValue < 340.0)
		{
			return 340L;
		}
		if (pLargestValue < 380.0)
		{
			return 380L;
		}
		if (pLargestValue < 400.0)
		{
			return 400L;
		}
		if (pLargestValue < 500.0)
		{
			return 500L;
		}
		if (pLargestValue < 600.0)
		{
			return 600L;
		}
		if (pLargestValue < 700.0)
		{
			return 700L;
		}
		if (pLargestValue < 800.0)
		{
			return 800L;
		}
		if (pLargestValue < 900.0)
		{
			return 900L;
		}
		if (pLargestValue < 1000.0)
		{
			return 1000L;
		}
		double tOrderOfMagnitude = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10((float)pLargestValue)));
		double tFractionOfMagnitude = pLargestValue / tOrderOfMagnitude;
		double tNiceFraction = ((tFractionOfMagnitude <= 1.5) ? 1.5 : ((tFractionOfMagnitude <= 2.0) ? 2.0 : ((tFractionOfMagnitude <= 3.0) ? 3.0 : ((!(tFractionOfMagnitude <= 5.0)) ? 10.0 : 5.0))));
		return (long)(tNiceFraction * tOrderOfMagnitude);
	}

	public static int findVerticalDivision(long pValue)
	{
		if (canDivideIntoWholeNumbers(pValue, 4))
		{
			return 4;
		}
		if (canDivideIntoWholeNumbers(pValue, 5))
		{
			return 5;
		}
		if (canDivideIntoWholeNumbers(pValue, 3))
		{
			return 3;
		}
		if (canDivideIntoWholeNumbers(pValue, 6))
		{
			return 6;
		}
		if (canDivideIntoWholeNumbers(pValue, 2))
		{
			return 2;
		}
		return 4;
	}

	private static bool canDivideIntoWholeNumbers(long pTotalValue, int pSegments)
	{
		for (int tStep = 1; tStep <= pSegments; tStep++)
		{
			if ((double)pTotalValue / (double)pSegments * (double)tStep % 1.0 > 0.0)
			{
				return false;
			}
		}
		return true;
	}
}
