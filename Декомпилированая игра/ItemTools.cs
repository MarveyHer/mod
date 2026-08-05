using System;

public class ItemTools
{
	public static int s_value;

	public static Rarity s_quality;

	public static void calcItemValues(Item pItem, BaseStats pStats)
	{
		pStats.clear();
		s_value = 0;
		s_quality = Rarity.R0_Normal;
		pItem.action_attack_target = null;
		mergeStatsWithItem(pStats, pItem);
	}

	public static void mergeStatsWithItem(BaseStats pStats, Item pItem, bool pCalcGlobalValue = true, float pMultiplier = 1f)
	{
		EquipmentAsset tItemAsset = pItem.getAsset();
		mergeStats(pStats, tItemAsset, pCalcGlobalValue, pMultiplier);
		pItem.action_attack_target = null;
		addAction(pItem, tItemAsset.action_attack_target);
		foreach (ref string modifier in pItem.data.modifiers)
		{
			string tModID = modifier;
			ItemModAsset tModAsset = AssetManager.items_modifiers.get(tModID);
			mergeStats(pStats, tModAsset, pCalcGlobalValue, pMultiplier);
			addAction(pItem, tModAsset.action_attack_target);
		}
	}

	private static void addAction(Item pItem, AttackAction pAction)
	{
		if (pAction != null)
		{
			if (pItem.action_attack_target == null)
			{
				pItem.action_attack_target = pAction;
			}
			else
			{
				pItem.action_attack_target = (AttackAction)Delegate.Combine(pItem.action_attack_target, pAction);
			}
		}
	}

	public static void mergeStats(BaseStats pStats, ItemAsset pAsset, bool pCalcGlobalValue = true, float pMultiplier = 1f)
	{
		if (pAsset == null)
		{
			return;
		}
		pStats.mergeStats(pAsset.base_stats, pMultiplier);
		if (pCalcGlobalValue)
		{
			if (pAsset.quality > s_quality)
			{
				s_quality = pAsset.quality;
			}
			s_value += pAsset.equipment_value + pAsset.mod_rank * 2;
		}
	}

	public static void getTooltipTitle(EquipmentAsset pAsset, out string pName, out string pMaterial)
	{
		string tName = pAsset.getLocaleID().Localize();
		string tMat = "";
		if (!string.IsNullOrEmpty(pAsset.material) && pAsset.material != "basic")
		{
			tMat = tMat + "(" + LocalizedTextManager.getText(pAsset.getMaterialID()) + ") ";
		}
		pName = tName;
		pMaterial = tMat;
	}

	public static void getAssetIds(string pItemIdWithMaterial, out string pItemAssetId, out string pMaterialAssetId)
	{
		pItemAssetId = null;
		pMaterialAssetId = null;
		int tIndex = pItemIdWithMaterial.LastIndexOf('_');
		if (tIndex != -1)
		{
			pItemAssetId = pItemIdWithMaterial.Substring(0, tIndex);
			pMaterialAssetId = pItemIdWithMaterial.Substring(tIndex + 1);
		}
	}
}
