using ai.behaviours;

public class BehDoTalk : BehaviourActionActor
{
	public BehDoTalk()
	{
		socialize = true;
	}

	public override BehResult execute(Actor pActor)
	{
		Actor tTarget = pActor.beh_actor_target?.a;
		if (tTarget == null)
		{
			return BehResult.Stop;
		}
		if (!stillCanTalk(tTarget))
		{
			return BehResult.Stop;
		}
		if ((!pActor.hasTelepathicLink() || !tTarget.hasTelepathicLink()) && (float)Toolbox.SquaredDistTile(tTarget.current_tile, pActor.current_tile) > 16f)
		{
			return BehResult.Stop;
		}
		pActor.data.get("socialize", out var tTalksAmount, 0);
		int tMax = Randy.randomInt(5, 10);
		if (tTalksAmount > tMax)
		{
			return BehResult.Continue;
		}
		continueTalk(pActor, tTarget);
		return BehResult.RepeatStep;
	}

	private bool stillCanTalk(Actor pTarget)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (pTarget.isLying())
		{
			return false;
		}
		return true;
	}

	private void continueTalk(Actor pActor, Actor pTarget)
	{
		pActor.data.get("socialize", out var tTalksAmount, 0);
		pActor.data.set("socialize", ++tTalksAmount);
		bool tNewSprite = false;
		if (Randy.randomChance(0.4f))
		{
			pActor.clearLastTopicSprite();
			tNewSprite = true;
		}
		else if (Randy.randomChance(0.4f))
		{
			pTarget.clearLastTopicSprite();
			tNewSprite = true;
		}
		if (!tNewSprite && pTarget.getTopicSpriteTrait() != null && Randy.randomChance(0.45f))
		{
			pActor.cloneTopicSprite(pTarget.getSocializeTopic());
		}
		pActor.lookTowardsPosition(pTarget.current_position);
		pTarget.lookTowardsPosition(pActor.current_position);
		pTarget.setTask("socialize_receiving", pClean: true, pCleanJob: false, pForceAction: true);
		float tAngleMax = 10f;
		if (Randy.randomBool())
		{
			pActor.playIdleSound();
		}
		else
		{
			pTarget.playIdleSound();
		}
		pActor.setTargetAngleZ(Randy.randomFloat(0f - tAngleMax, tAngleMax));
		pTarget.setTargetAngleZ(Randy.randomFloat(0f - tAngleMax, tAngleMax));
		pTarget.timer_action = (pActor.timer_action = Randy.randomFloat(1.1f, 3.3f));
		if (pActor.timestamp_tween_session_social == 0.0)
		{
			pActor.timestamp_tween_session_social = BehaviourActionBase<Actor>.world.getCurSessionTime();
			pTarget.timestamp_tween_session_social = BehaviourActionBase<Actor>.world.getCurSessionTime();
		}
	}
}
