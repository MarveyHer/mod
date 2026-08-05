using ai.behaviours;

public class BehActorGiveTax : BehCitizenActionCity
{
	public override BehResult execute(Actor pActor)
	{
		if (pActor.isKing())
		{
			pActor.takeAllOwnLoot();
			return BehResult.Continue;
		}
		if (!pActor.city.hasLeader())
		{
			pActor.takeAllOwnLoot();
			return BehResult.Continue;
		}
		Actor tLeader = pActor.city.leader;
		if (pActor.isCityLeader())
		{
			if (!pActor.kingdom.hasKing())
			{
				pActor.takeAllOwnLoot();
			}
			else
			{
				payTributeToKing(pActor, pActor.kingdom.king, pActor.kingdom.getTaxRateTribute());
			}
		}
		else
		{
			payTaxToLeader(pActor, tLeader, pActor.kingdom.getTaxRateLocal());
		}
		return BehResult.Continue;
	}

	private void payTributeToKing(Actor pActor, Actor pKing, float pTaxRate)
	{
		if (pActor.loot > 0)
		{
			int loot = pActor.loot;
			int tToKing = (int)((float)loot * pTaxRate);
			int tToMe = loot - tToKing;
			int tToCity = (int)((float)tToMe * 0.5f);
			tToMe -= tToCity;
			pActor.city.addResourcesToRandomStockpile("gold", tToCity);
			pActor.addMoney(tToMe);
			pKing.addLoot(tToKing);
			pActor.paidTax(pTaxRate, "fx_money_paid_tribute");
		}
	}

	private void payTaxToLeader(Actor pActor, Actor pTarget, float pTaxRate)
	{
		if (pActor.loot > 0)
		{
			int loot = pActor.loot;
			int pLeader = (int)((float)loot * pTaxRate);
			int tToMe = loot - pLeader;
			pActor.addMoney(tToMe);
			pTarget.addLoot(pLeader);
			pActor.paidTax(pTaxRate, "fx_money_paid_tax");
		}
	}
}
