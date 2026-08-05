using ai.behaviours;

public class BehChildFindRandomFamilyParent : BehaviourActionActor
{
	public override BehResult execute(Actor pBabyActor)
	{
		if (!pBabyActor.family.hasFounders())
		{
			return BehResult.Stop;
		}
		Actor tParent = pBabyActor.family.getRandomFounder();
		if (pBabyActor.inOwnCityBorders() && !tParent.inOwnCityBorders())
		{
			return BehResult.Stop;
		}
		pBabyActor.beh_actor_target = tParent;
		return BehResult.Continue;
	}
}
