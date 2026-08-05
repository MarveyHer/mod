using System.Collections.Generic;

public static class ItemCrafting
{
	private static readonly EquipmentType[] list_equipments = new EquipmentType[5]
	{
		EquipmentType.Helmet,
		EquipmentType.Armor,
		EquipmentType.Boots,
		EquipmentType.Amulet,
		EquipmentType.Ring
	};

	public static bool tryToCraftRandomWeapon(Actor pActor, City pCity)
	{
		int tTries = pActor.asset.item_making_skill;
		if (pActor.hasCulture() && pActor.culture.hasTrait("weaponsmith_mastery"))
		{
			tTries += CultureTraitLibrary.getValue("weaponsmith_mastery");
		}
		return craftItem(pActor, pActor.getName(), EquipmentType.Weapon, tTries, pCity);
	}

	public static bool tryToCraftRandomArmor(Actor pActor, City pCity)
	{
		int tTries = pActor.asset.item_making_skill;
		if (pActor.hasCulture() && pActor.culture.hasTrait("armorsmith_mastery"))
		{
			tTries += CultureTraitLibrary.getValue("armorsmith_mastery");
		}
		EquipmentType tType = list_equipments.GetRandom();
		return craftItem(pActor, pActor.getName(), tType, tTries, pCity);
	}

	public static bool tryToCraftRandomEquipment(Actor pActor, City pCity)
	{
		bool num = tryToCraftRandomArmor(pActor, pCity);
		bool tNewWeapon = tryToCraftRandomWeapon(pActor, pCity);
		return num || tNewWeapon;
	}

	public static bool craftItem(Actor pActor, string pCreatorName, EquipmentType pType, int pTries, City pCity)
	{
		string tEquipmentSubtype = null;
		if (pType == EquipmentType.Weapon)
		{
			if (pActor.hasCulture())
			{
				tEquipmentSubtype = pActor.culture.getPreferredWeaponSubtypeIDs();
			}
			if (string.IsNullOrEmpty(tEquipmentSubtype))
			{
				tEquipmentSubtype = ItemLibrary.default_weapon_pool.GetRandom();
			}
		}
		else
		{
			tEquipmentSubtype = AssetManager.items.getEquipmentType(pType);
		}
		EquipmentAsset tItemAssetToCraft = null;
		ActorEquipmentSlot tActorSlot = pActor.equipment.getSlot(pType);
		Item tCurrentItem = tActorSlot.getItem();
		if (tCurrentItem != null && tCurrentItem.isCursed())
		{
			return false;
		}
		int tCurrentItemValue = tCurrentItem?.asset.equipment_value ?? 0;
		if (pType == EquipmentType.Weapon && pActor.hasCulture() && pActor.culture.hasPreferredWeaponsToCraft() && Randy.randomBool())
		{
			tItemAssetToCraft = getItemAssetToCraft(pActor, pActor.culture.getPreferredWeaponAssets(), pCity, tCurrentItemValue, pShuffle: true);
		}
		if (tItemAssetToCraft == null)
		{
			List<EquipmentAsset> tItemsOfType = AssetManager.items.equipment_by_subtypes[tEquipmentSubtype];
			tItemAssetToCraft = getItemAssetToCraft(pActor, tItemsOfType, pCity, tCurrentItemValue);
		}
		if (tItemAssetToCraft == null)
		{
			return false;
		}
		Item tItem = World.world.items.generateItem(tItemAssetToCraft, pActor.kingdom, pCreatorName, pTries, pActor);
		if (tActorSlot.isEmpty())
		{
			tActorSlot.setItem(tItem, pActor);
		}
		else
		{
			Item tOldItem = tActorSlot.getItem();
			tActorSlot.takeAwayItem();
			pCity.tryToPutItem(tOldItem);
			tActorSlot.setItem(tItem, pActor);
		}
		pActor.spendMoney(tItemAssetToCraft.get_total_cost);
		if (tItemAssetToCraft.cost_resource_id_1 != "none")
		{
			pCity.takeResource(tItemAssetToCraft.cost_resource_id_1, tItemAssetToCraft.cost_resource_1);
		}
		if (tItemAssetToCraft.cost_resource_id_2 != "none")
		{
			pCity.takeResource(tItemAssetToCraft.cost_resource_id_2, tItemAssetToCraft.cost_resource_2);
		}
		return true;
	}

	public static EquipmentAsset getItemAssetToCraft(Actor pActor, List<EquipmentAsset> pItemList, City pCity, int pCurrentItemValue, bool pShuffle = false)
	{
		if (pShuffle)
		{
			pItemList.Shuffle();
		}
		for (int i = pItemList.Count - 1; i >= 0; i--)
		{
			EquipmentAsset tItemAsset = pItemList[i];
			if (tItemAsset.equipment_value > pCurrentItemValue && hasEnoughResourcesToCraft(pActor, tItemAsset, pCity))
			{
				return tItemAsset;
			}
		}
		return null;
	}

	private static bool hasEnoughResourcesToCraft(Actor pActor, EquipmentAsset pAsset, City pCity)
	{
		int tTotalCost = pAsset.get_total_cost;
		if (!pActor.hasEnoughMoney(tTotalCost))
		{
			return false;
		}
		if (pAsset.cost_resource_id_1 != "none" && pAsset.cost_resource_1 > pCity.getResourcesAmount(pAsset.cost_resource_id_1))
		{
			return false;
		}
		if (pAsset.cost_resource_id_2 != "none" && pAsset.cost_resource_2 > pCity.getResourcesAmount(pAsset.cost_resource_id_2))
		{
			return false;
		}
		return true;
	}
}
