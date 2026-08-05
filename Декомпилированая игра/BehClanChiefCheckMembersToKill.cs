using ai.behaviours;

public class BehClanChiefCheckMembersToKill : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		Clan tClan = pActor.clan;
		for (int i = 0; i < tClan.units.Count; i++)
		{
			Actor tActorTarget = tClan.units[i];
			if (tActorTarget != pActor && pActor.areFoes(tActorTarget))
			{
				tActorTarget.getHitFullHealth(AttackType.Divine);
			}
		}
		return BehResult.Continue;
	}
}
