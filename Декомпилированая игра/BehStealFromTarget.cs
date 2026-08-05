using ai.behaviours;

public class BehStealFromTarget : BehaviourActionActor
{
	protected override void setupErrorChecks()
	{
		base.setupErrorChecks();
		null_check_actor_target = true;
	}

	public override BehResult execute(Actor pActor)
	{
		Actor tTarget = pActor.beh_actor_target.a;
		if (tTarget == null || !tTarget.isAlive() || tTarget.isInsideSomething())
		{
			return BehResult.Stop;
		}
		if (pActor.distanceToActorTile(tTarget) > 2f)
		{
			return BehResult.Stop;
		}
		bool tSteal = false;
		float tWaitTimerHimself = 0.5f;
		float tStunnedTimer = 1f;
		bool tAddAggro = false;
		if (tTarget.canSeeTileBasedOnDirection(pActor.current_tile))
		{
			if (Randy.randomChance(0.4f))
			{
				tSteal = true;
				tStunnedTimer = 1f;
				tWaitTimerHimself = 0.9f;
				tAddAggro = true;
			}
		}
		else if (Randy.randomChance(0.7f))
		{
			tSteal = true;
			tStunnedTimer = 5f;
			tWaitTimerHimself = 1f;
		}
		else
		{
			pActor.makeWait(1f);
		}
		if (tSteal)
		{
			pActor.spawnSlashTalk(tTarget.current_position);
			pActor.punchTargetAnimation(tTarget.current_position, pFlip: false, pReverse: false, -20f);
			pActor.stealActionFrom(tTarget, tStunnedTimer, tWaitTimerHimself, tAddAggro);
			return BehResult.Continue;
		}
		return BehResult.Stop;
	}
}
