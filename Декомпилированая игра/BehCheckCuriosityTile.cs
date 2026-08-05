using ai.behaviours;

public class BehCheckCuriosityTile : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		if (pActor.scheduled_tile_target == null)
		{
			return BehResult.Stop;
		}
		WorldTile tTileToInvestigate = pActor.scheduled_tile_target;
		pActor.scheduled_tile_target = null;
		float tChanceToInvestigate = 0.6f;
		if (pActor.hasSubspecies() && pActor.subspecies.has_trait_curious)
		{
			tChanceToInvestigate += 0.3f;
		}
		if (!Randy.randomChance(tChanceToInvestigate))
		{
			return BehResult.Stop;
		}
		WorldTile tTile = tTileToInvestigate.getWalkableTileAround(pActor.current_tile);
		if (tTile == null)
		{
			return BehResult.Stop;
		}
		pActor.beh_tile_target = tTile;
		return BehResult.Continue;
	}
}
