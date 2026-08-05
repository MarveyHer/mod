using ai.behaviours;

public class BehSpawnSlashTalkTowardTileTarget : BehaviourActionActor
{
	protected override void setupErrorChecks()
	{
		base.setupErrorChecks();
		null_check_tile_target = true;
	}

	public override BehResult execute(Actor pActor)
	{
		pActor.spawnSlashTalk(pActor.beh_tile_target.pos);
		return BehResult.Continue;
	}
}
