using ai.behaviours;

public class BehFindRandomTileNearBuildingTarget : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		if (pActor.beh_building_target == null)
		{
			return BehResult.Stop;
		}
		if (!pActor.beh_building_target.current_tile.isSameIsland(pActor.current_tile))
		{
			return BehResult.Stop;
		}
		MapRegion tRegion = pActor.beh_building_target.current_tile.region;
		if (Randy.randomChance(0.2f) && tRegion.neighbours.Count > 0)
		{
			tRegion = tRegion.neighbours.GetRandom();
		}
		WorldTile tResultTile = tRegion.tiles.GetRandom();
		pActor.beh_tile_target = tResultTile;
		return BehResult.Continue;
	}
}
