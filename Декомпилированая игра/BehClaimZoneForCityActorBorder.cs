using ai.behaviours;

public class BehClaimZoneForCityActorBorder : BehCityActor
{
	public override BehResult execute(Actor pActor)
	{
		return tryClaimZone(pActor);
	}

	public static BehResult tryClaimZone(Actor pActor)
	{
		TileZone tCurZone = pActor.current_tile.zone;
		City tCity = pActor.city;
		WorldTile tCityTile = tCity.getTile();
		if (tCityTile == null)
		{
			return BehResult.Stop;
		}
		if (!tCity.isZoneToClaimStillGood(pActor, tCurZone, tCityTile))
		{
			return BehResult.Stop;
		}
		bool tGrabZonesAround = pActor.hasCultureTrait("expansionists") || DebugConfig.isOn(DebugOption.CityFastZonesGrowth);
		bool num = tCurZone.city != null && tCurZone.city != tCity;
		tCity.addZone(tCurZone);
		if (num)
		{
			tGrabZonesAround = false;
		}
		if (tGrabZonesAround)
		{
			TileZone[] tNeighbours = tCurZone.neighbours_all;
			foreach (TileZone tZone in tNeighbours)
			{
				if (!tZone.hasCity() && tZone.centerTile.isSameIsland(tCityTile) && tCity.isZoneToClaimStillGood(pActor, tZone, tCityTile))
				{
					tCity.addZone(tZone);
				}
			}
		}
		pActor.addLoot(SimGlobals.m.coins_for_zone);
		return BehResult.Continue;
	}
}
