using ai.behaviours;

public class BehAffectDreams : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		Actor tActorTarget = getRandomDreamingActor(pActor);
		if (tActorTarget == null)
		{
			return BehResult.Stop;
		}
		tActorTarget.tryToConvertActorToMetaFromActor(pActor);
		return BehResult.Continue;
	}

	private Actor getRandomDreamingActor(Actor pActor)
	{
		BehaviourActionBase<Actor>.world.units.checkSleepingUnits();
		if (BehaviourActionBase<Actor>.world.units.cached_sleeping_units.Count == 0)
		{
			return null;
		}
		foreach (Actor tActor in BehaviourActionBase<Actor>.world.units.cached_sleeping_units.LoopRandom())
		{
			if (tActor.isAlive() && tActor.hasSubspecies() && tActor.hasStatus("sleeping") && (tActor.subspecies.has_advanced_memory || tActor.subspecies.has_advanced_communication))
			{
				return tActor;
			}
		}
		return null;
	}
}
