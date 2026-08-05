using ai.behaviours;

public class BehTryToEatCityFood : BehCityActor
{
	public override BehResult execute(Actor pActor)
	{
		City tCity = pActor.city;
		if (!tCity.hasSuitableFood(pActor.subspecies))
		{
			return BehResult.Stop;
		}
		ResourceAsset tFoodItem = tCity.getFoodItem(pActor.subspecies, pActor.data.favorite_food);
		bool tNeedToPay = !pActor.isFoodFreeForThisPerson();
		if (tFoodItem != null)
		{
			if (tNeedToPay && !pActor.hasEnoughMoney(tFoodItem.money_cost))
			{
				return BehResult.Stop;
			}
			eatFood(pActor, tCity, tFoodItem, tNeedToPay);
			if (pActor.hasTrait("gluttonous"))
			{
				tFoodItem = tCity.getFoodItem(pActor.subspecies, pActor.data.favorite_food);
				if (tFoodItem != null && tNeedToPay && pActor.hasEnoughMoney(tFoodItem.money_cost))
				{
					eatFood(pActor, tCity, tFoodItem, pNeedToPay: true);
				}
			}
		}
		return BehResult.Continue;
	}

	private void eatFood(Actor pActor, City pCity, ResourceAsset pFoodItem, bool pNeedToPay)
	{
		if (pNeedToPay)
		{
			pActor.spendMoney(pFoodItem.money_cost);
		}
		pCity.eatFoodItem(pFoodItem.id);
		pActor.consumeFoodResource(pFoodItem);
	}
}
