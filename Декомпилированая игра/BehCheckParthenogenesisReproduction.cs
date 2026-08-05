using ai.behaviours;

public class BehCheckParthenogenesisReproduction : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		switch (pActor.subspecies.getReproductionStrategy())
		{
		case ReproductiveStrategy.Egg:
		case ReproductiveStrategy.SpawnUnitImmediate:
			BabyMaker.makeBabyViaParthenogenesis(pActor);
			break;
		case ReproductiveStrategy.Pregnancy:
		{
			BabyHelper.babyMakingStart(pActor);
			float tMaturationTime = pActor.getMaturationTimeSeconds();
			pActor.addStatusEffect("pregnant_parthenogenesis", tMaturationTime);
			pActor.subspecies.counterReproduction();
			break;
		}
		}
		return BehResult.Continue;
	}
}
