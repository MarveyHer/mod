using ai.behaviours;

public class CityBehCheckFarms : BehaviourActionCity
{
	public override bool shouldRetry(City pCity)
	{
		return false;
	}

	public override BehResult execute(City pCity)
	{
		check(pCity);
		return BehResult.Continue;
	}

	public static void check(City pCity)
	{
		pCity.calculated_place_for_farms.Clear();
		pCity.calculated_grown_wheat.Clear();
		pCity.calculated_farm_fields.Clear();
		pCity.calculated_crops.Clear();
		behFindTileForFarm(pCity);
		pCity.calculated_place_for_farms.checkAddRemove();
		pCity.calculated_farm_fields.checkAddRemove();
		pCity.calculated_crops.checkAddRemove();
		behCheckWheat(pCity);
		pCity.calculated_grown_wheat.checkAddRemove();
	}

	private static void behCheckWheat(City pCity)
	{
		foreach (WorldTile tTile in pCity.calculated_crops)
		{
			if (tTile.hasBuilding() && tTile.building.asset.wheat && tTile.building.component_wheat.isMaxLevel())
			{
				pCity.calculated_grown_wheat.Add(tTile);
			}
		}
	}

	private static void behFindTileForFarm(City pCity)
	{
		Building tBuilding = pCity.getBuildingOfType("type_windmill");
		if (tBuilding != null)
		{
			checkRegion(tBuilding.current_tile.region, tBuilding, pCity);
			for (int i = 0; i < tBuilding.current_tile.region.neighbours.Count; i++)
			{
				checkRegion(tBuilding.current_tile.region.neighbours[i], tBuilding, pCity);
			}
		}
	}

	private static void checkRegion(MapRegion pRegion, Building pBuilding, City pCity)
	{
		MapChunk tChunk = pRegion.chunk;
		for (int i = 0; i < tChunk.zones.Count; i++)
		{
			checkZone(tChunk.zones[i], pBuilding, pCity);
		}
	}

	private static void checkZone(TileZone pZone, Building pBuilding, City pCity)
	{
		if (!pZone.isSameCityHere(pCity))
		{
			return;
		}
		WorldTile[] tTiles = pZone.tiles;
		int tCount = tTiles.Length;
		for (int i = 0; i < tCount; i++)
		{
			WorldTile tTile = tTiles[i];
			if ((float)Toolbox.SquaredDistTile(pBuilding.current_tile, tTile) > 81f)
			{
				continue;
			}
			if (tTile.Type.can_be_farm)
			{
				pCity.calculated_place_for_farms.Add(tTile);
			}
			if (tTile.Type.farm_field)
			{
				pCity.calculated_farm_fields.Add(tTile);
				if (tTile.hasBuilding() && tTile.building.asset.wheat)
				{
					pCity.calculated_crops.Add(tTile);
				}
			}
		}
	}
}
