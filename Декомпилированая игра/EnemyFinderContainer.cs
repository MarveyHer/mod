using System;
using System.Collections.Generic;
using UnityEngine.Pool;

public class EnemyFinderContainer
{
	public Dictionary<int, EnemyFinderData> dict_data = new Dictionary<int, EnemyFinderData>((int)Math.Pow(9.0, SimGlobals.m.unit_chunk_sight_range));

	private Kingdom _kingdom;

	public void setKingdom(Kingdom pKingdom)
	{
		_kingdom = pKingdom;
	}

	public EnemyFinderData getData(MapChunk pChunk, int pRange)
	{
		int t_id = pChunk.id * 10000 + pRange;
		if (!dict_data.TryGetValue(t_id, out var tData))
		{
			tData = UnsafeGenericPool<EnemyFinderData>.Get();
			dict_data.Add(t_id, tData);
			if (!_kingdom.asset.force_look_all_chunks)
			{
				if (pRange == 0)
				{
					findEnemiesOfKingdomInChunk(tData, pChunk, _kingdom);
					return tData;
				}
				if (Randy.randomChance(0.8f))
				{
					findEnemiesOfKingdomInChunk(tData, pChunk, _kingdom);
				}
			}
			if (tData.isEmpty())
			{
				for (int i = 0; i <= pRange; i++)
				{
					checkRange(tData, pChunk, i, i);
					if (!tData.isEmpty() && !_kingdom.asset.force_look_all_chunks)
					{
						break;
					}
				}
			}
			return tData;
		}
		EnemiesFinder.counter_reused++;
		return tData;
	}

	private void checkRange(EnemyFinderData pData, MapChunk pChunk, int pRange, int pSkipLessThan = -1)
	{
		if (pRange == 0)
		{
			findEnemiesOfKingdomInChunk(pData, pChunk, _kingdom);
			return;
		}
		int tStartX = pChunk.x;
		int tStartY = pChunk.y;
		bool tSkipCheck = pSkipLessThan > 0;
		int tMin = pSkipLessThan * -1;
		for (int iX = -pRange; iX <= pRange; iX++)
		{
			for (int iY = -pRange; iY <= pRange; iY++)
			{
				if (!tSkipCheck || iX <= tMin || iX >= pSkipLessThan || iY <= tMin || iY >= pSkipLessThan)
				{
					int xx = tStartX + iX;
					int yy = tStartY + iY;
					MapChunk tChunk = World.world.map_chunk_manager.get(xx, yy);
					if (tChunk != null)
					{
						findEnemiesOfKingdomInChunk(pData, tChunk, _kingdom);
					}
				}
			}
		}
	}

	private static void findEnemiesOfKingdomInChunk(EnemyFinderData pData, MapChunk pChunk, Kingdom pMainKingdom)
	{
		if (pChunk.objects.kingdoms.Count == 0)
		{
			return;
		}
		List<long> tKingdomsIDs = pChunk.objects.kingdoms;
		bool tPeacefulMonsters = WorldLawLibrary.world_law_peaceful_monsters.isEnabled();
		if (pMainKingdom.asset.mobs && tPeacefulMonsters)
		{
			return;
		}
		for (int i = 0; i < tKingdomsIDs.Count; i++)
		{
			long tKingdomID = tKingdomsIDs[i];
			Kingdom iKingdom = World.world.kingdoms.getCivOrWildViaID(tKingdomID);
			if (iKingdom != null && (!tPeacefulMonsters || !iKingdom.asset.mobs) && pMainKingdom.isEnemy(iKingdom))
			{
				pData.addEnemyList(pChunk.objects.getUnits(tKingdomID));
				pData.addEnemyList(pChunk.objects.getBuildings(tKingdomID));
			}
		}
	}

	public void clear()
	{
		foreach (EnemyFinderData value in dict_data.Values)
		{
			value.reset();
			UnsafeGenericPool<EnemyFinderData>.Release(value);
		}
		dict_data.Clear();
	}

	public void disposeAll()
	{
		foreach (EnemyFinderData value in dict_data.Values)
		{
			value.reset();
		}
		_kingdom = null;
	}
}
