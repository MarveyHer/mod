using ai.behaviours;

public class BehTrySleep : BehaviourActionActor
{
	private bool _sleep_outside;

	public BehTrySleep(bool pSleepOutside = false)
	{
		_sleep_outside = pSleepOutside;
	}

	public override BehResult execute(Actor pActor)
	{
		float tWaitTimer = getWaitTimer(pActor);
		pActor.makeSleep(tWaitTimer);
		if (pActor.hasCity() && !pActor.hasHouse() && pActor.isSapient())
		{
			pActor.changeHappiness("slept_outside");
		}
		return BehResult.Continue;
	}

	private float getWaitTimer(Actor pActor)
	{
		if (!pActor.hasSubspecies())
		{
			return 20f;
		}
		WorldAgeAsset tWorldAge = BehaviourActionBase<Actor>.world.era_manager.getCurrentAge();
		Subspecies tSubspecies = pActor.subspecies;
		float tSleepTimer = 0f;
		bool tShouldHibernate = false;
		if (tWorldAge.flag_winter && tSubspecies.hasTrait("winter_slumberers"))
		{
			tShouldHibernate = true;
		}
		else if (tWorldAge.flag_night && tSubspecies.hasTrait("nocturnal_dormancy"))
		{
			tShouldHibernate = true;
		}
		else if (!tWorldAge.flag_chaos && tSubspecies.hasTrait("chaos_driven"))
		{
			tShouldHibernate = true;
		}
		else if (tWorldAge.flag_light_age && tSubspecies.hasTrait("circadian_drift"))
		{
			tShouldHibernate = true;
		}
		if (tShouldHibernate)
		{
			tSleepTimer = 100f;
		}
		else
		{
			float tMin = 20f;
			float tMax = 60f;
			if (tSubspecies.hasTrait("monophasic_sleep"))
			{
				tMin = 40f;
				tMax = 90f;
			}
			tSleepTimer = Randy.randomFloat(tMin, tMax);
			if (tSubspecies.hasTrait("prolonged_rest"))
			{
				tSleepTimer += Randy.randomFloat(tMin, tMax);
			}
		}
		return tSleepTimer;
	}
}
