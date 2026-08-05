using ai.behaviours;

public class BehSpawnSlashYellTowardTileTarget : BehaviourActionActor
{
	protected override void setupErrorChecks()
	{
		base.setupErrorChecks();
		null_check_tile_target = true;
	}

	public override BehResult execute(Actor pActor)
	{
		pActor.spawnSlashYell(pActor.beh_tile_target.pos);
		return BehResult.Continue;
	}
}
