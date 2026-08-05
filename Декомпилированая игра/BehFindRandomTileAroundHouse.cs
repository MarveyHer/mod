using ai.behaviours;

public class BehFindRandomTileAroundHouse : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		Building tHomeBuilding = pActor.getHomeBuilding();
		if (tHomeBuilding == null)
		{
			return BehResult.Stop;
		}
		if (!tHomeBuilding.current_tile.isSameIsland(pActor.current_tile))
		{
			return BehResult.Stop;
		}
		MapRegion tRegion = tHomeBuilding.current_tile.region;
		if (Randy.randomChance(0.2f) && tRegion.neighbours.Count > 0)
		{
			tRegion = tRegion.neighbours.GetRandom();
		}
		WorldTile tResultTile = tRegion.tiles.GetRandom();
		pActor.beh_tile_target = tResultTile;
		return BehResult.Continue;
	}
}
