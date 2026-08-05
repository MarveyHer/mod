using ai.behaviours;

public class BehTryFindTargetWithStatusNearby : BehaviourActionActor
{
	private string[] _status_ids;

	public BehTryFindTargetWithStatusNearby(params string[] pStatusIDs)
	{
		_status_ids = pStatusIDs;
	}

	public override BehResult execute(Actor pActor)
	{
		Actor tTarget = getClosestActorWithStatus(pActor, _status_ids);
		if (tTarget == null)
		{
			WorldTile tTile = Finder.findTileInChunk(pActor.current_tile, TileFinderType.FreeTile);
			if (tTile == null)
			{
				return BehResult.Stop;
			}
			pActor.beh_tile_target = tTile;
			return BehResult.Continue;
		}
		pActor.beh_tile_target = tTarget.current_tile.getTileAroundThisOnSameIsland(pActor.current_tile);
		pActor.beh_actor_target = tTarget;
		return BehResult.Continue;
	}

	private Actor getClosestActorWithStatus(Actor pSelf, string[] pStatusIDs)
	{
		bool tRandomShuffle = Randy.randomBool();
		int tBestDist = int.MaxValue;
		Actor tBest = null;
		foreach (Actor tTarget in Finder.getUnitsFromChunk(pSelf.current_tile, 1, 0f, tRandomShuffle))
		{
			if (tTarget == pSelf)
			{
				continue;
			}
			int tDist = Toolbox.SquaredDistTile(tTarget.current_tile, pSelf.current_tile);
			if (tDist >= tBestDist || !pSelf.isSameIslandAs(tTarget) || !tTarget.hasAnyStatusEffect())
			{
				continue;
			}
			bool tHasAnyStatusEffect = false;
			foreach (string tStatusID in pStatusIDs)
			{
				if (tTarget.hasStatus(tStatusID))
				{
					tHasAnyStatusEffect = true;
					break;
				}
			}
			if (tHasAnyStatusEffect)
			{
				tBestDist = tDist;
				tBest = tTarget;
				if (tRandomShuffle || Randy.randomBool())
				{
					break;
				}
			}
		}
		return tBest;
	}
}
