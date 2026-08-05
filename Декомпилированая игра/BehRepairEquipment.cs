using ai.behaviours;

public class BehRepairEquipment : BehCityActor
{
	public override BehResult execute(Actor pActor)
	{
		bool tAnythingRepaired = false;
		foreach (ActorEquipmentSlot tSlot in pActor.equipment)
		{
			if (tSlot.isEmpty())
			{
				continue;
			}
			Item tItem = tSlot.getItem();
			if (tItem.needRepair())
			{
				int tRepairCost = (int)((float)tSlot.getItem().getAsset().cost_gold * SimGlobals.m.item_repair_cost_multiplier);
				if (pActor.hasEnoughMoney(tRepairCost))
				{
					pActor.spendMoney(tRepairCost);
					tItem.fullRepair();
					tAnythingRepaired = true;
				}
			}
		}
		if (tAnythingRepaired)
		{
			pActor.setStatsDirty();
		}
		return BehResult.Continue;
	}
}
