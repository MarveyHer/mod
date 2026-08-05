using ai.behaviours;

public class BehCheckCanRepairEquipment : BehCityActor
{
	public override BehResult execute(Actor pActor)
	{
		if (!pActor.hasEquipment())
		{
			return BehResult.Stop;
		}
		bool tCanRepairAny = false;
		foreach (ActorEquipmentSlot tSlot in pActor.equipment)
		{
			if (tSlot.getItem().needRepair())
			{
				int tRepairCost = (int)((float)tSlot.getItem().getAsset().cost_gold * SimGlobals.m.item_repair_cost_multiplier);
				if (pActor.money >= tRepairCost)
				{
					tCanRepairAny = true;
				}
			}
		}
		if (!tCanRepairAny)
		{
			return BehResult.Stop;
		}
		return BehResult.Continue;
	}
}
