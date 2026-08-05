using ai.behaviours;
using UnityEngine;

public class BehGenerateLootFromHouse : BehCityActor
{
	public override BehResult execute(Actor pActor)
	{
		if (!pActor.hasHouse())
		{
			return BehResult.Stop;
		}
		Building homeBuilding = pActor.getHomeBuilding();
		int tCoinsFromHouse = homeBuilding.asset.loot_generation;
		int tCoinsFromBiome = 0;
		BiomeAsset tBiomeAsset = homeBuilding.current_tile.getBiome();
		if (tBiomeAsset != null)
		{
			tCoinsFromBiome = tBiomeAsset.loot_generation;
		}
		int tTotalCoins = tCoinsFromHouse + tCoinsFromBiome;
		tTotalCoins = Mathf.Max(1, tTotalCoins);
		pActor.addLoot(tTotalCoins);
		return BehResult.Continue;
	}
}
