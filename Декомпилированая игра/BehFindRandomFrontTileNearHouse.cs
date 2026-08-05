using ai.behaviours;

public class BehFindRandomFrontTileNearHouse : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		Building tHomeBuilding = pActor.getHomeBuilding();
		if (tHomeBuilding == null)
		{
			return BehResult.Stop;
		}
		WorldTile tDoorTile = tHomeBuilding.door_tile;
		if (!tDoorTile.isSameIsland(pActor.current_tile))
		{
			if (tHomeBuilding.current_tile.isSameIsland(pActor.current_tile))
			{
				pActor.beh_tile_target = tHomeBuilding.current_tile;
				return BehResult.Continue;
			}
			return BehResult.Stop;
		}
		using ListPool<WorldTile> tFrontTiles = new ListPool<WorldTile>();
		for (int i = 0; i < 3; i++)
		{
			WorldTile tTile = BehaviourActionBase<Actor>.world.GetTile(tDoorTile.x + i, tDoorTile.y);
			if (tTile != null && tDoorTile.isSameIsland(tTile))
			{
				tFrontTiles.Add(tTile);
			}
		}
		for (int j = 0; j < 3; j++)
		{
			WorldTile tTile2 = BehaviourActionBase<Actor>.world.GetTile(tDoorTile.x - j, tDoorTile.y);
			if (tTile2 != null && tDoorTile.isSameIsland(tTile2))
			{
				tFrontTiles.Add(tTile2);
			}
		}
		if (tFrontTiles.Count == 0)
		{
			return BehResult.Stop;
		}
		WorldTile tResultTile = tFrontTiles.GetRandom();
		pActor.beh_tile_target = tResultTile;
		return BehResult.Continue;
	}
}
