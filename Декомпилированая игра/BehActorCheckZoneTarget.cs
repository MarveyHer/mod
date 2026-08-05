using ai.behaviours;

public class BehActorCheckZoneTarget : BehCityActor
{
	public override BehResult execute(Actor pActor)
	{
		City tCity = pActor.city;
		TileZone tZoneToClaim = BehaviourActionBase<Actor>.world.city_zone_helper.city_growth.getZoneToClaim(pActor, pActor.city);
		if (tZoneToClaim == null)
		{
			return BehResult.Stop;
		}
		if (tZoneToClaim.city == tCity)
		{
			return BehResult.Stop;
		}
		WorldTile tTargetTile = null;
		if (tZoneToClaim.centerTile.isSameIsland(pActor.current_tile))
		{
			tTargetTile = tZoneToClaim.centerTile;
		}
		else
		{
			foreach (WorldTile tTile in tZoneToClaim.tiles.LoopRandom())
			{
				if (tTile.isSameIsland(pActor.current_tile))
				{
					tTargetTile = tTile;
					break;
				}
			}
		}
		if (tTargetTile == null)
		{
			return BehResult.Stop;
		}
		pActor.beh_tile_target = tTargetTile;
		return BehResult.Continue;
	}
}
