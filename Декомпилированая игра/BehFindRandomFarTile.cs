using ai.behaviours;

public class BehFindRandomFarTile : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		MapRegion tRegion = pActor.current_tile.region;
		for (int i = 0; i < 5; i++)
		{
			if (tRegion.neighbours.Count == 0)
			{
				break;
			}
			tRegion = tRegion.neighbours.GetRandom();
		}
		if (tRegion.tiles.Count > 0)
		{
			pActor.beh_tile_target = tRegion.tiles.GetRandom();
			return BehResult.Continue;
		}
		return BehResult.Stop;
	}
}
