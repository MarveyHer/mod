using System;
using System.Collections.Generic;
using UnityPools;

public class KingdomOpinion : IDisposable
{
	public readonly Dictionary<OpinionAsset, int> results = UnsafeCollectionPool<Dictionary<OpinionAsset, int>, KeyValuePair<OpinionAsset, int>>.Get();

	public int total;

	public Kingdom main;

	public Kingdom target;

	public KingdomOpinion(Kingdom k1, Kingdom k2)
	{
		main = k1;
		target = k2;
	}

	internal void calculate(Kingdom pMain, Kingdom pTarget, DiplomacyRelation pRelation)
	{
		clear();
		foreach (OpinionAsset tAsset in AssetManager.opinion_library.list)
		{
			int tResult = tAsset.calc(pMain, pTarget);
			total += tResult;
			if (tResult != 0)
			{
				results.Add(tAsset, tResult);
			}
		}
	}

	private void clear()
	{
		total = 0;
		results.Clear();
	}

	public void Dispose()
	{
		clear();
		UnsafeCollectionPool<Dictionary<OpinionAsset, int>, KeyValuePair<OpinionAsset, int>>.Release(results);
		main = null;
		target = null;
	}
}
