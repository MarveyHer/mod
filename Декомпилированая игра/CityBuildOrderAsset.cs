using System;
using System.Collections.Generic;
using UnityPools;

[Serializable]
public class CityBuildOrderAsset : Asset
{
	[NonSerialized]
	public string[] list_for_generation;

	public List<BuildOrder> list = new List<BuildOrder>();

	public void addUpgrade(string pID, int pLimitType = 0, int pPop = 0, int pBuildings = 0, bool pCheckFullVillage = false, bool pZonesCheck = false, int pMinZones = 0)
	{
		addBuilding(pID, pLimitType, pPop, pBuildings, pCheckFullVillage, pZonesCheck, pMinZones).upgrade = true;
	}

	public BuildOrder addBuilding(string pID, int pLimitType = 0, int pPop = 0, int pBuildings = 0, bool pCheckFullVillage = false, bool pCheckHouseLimit = false, int pMinZones = 0)
	{
		BuildOrder tAsset = new BuildOrder();
		tAsset.id = pID;
		tAsset.limit_type = pLimitType;
		tAsset.required_pop = pPop;
		tAsset.required_buildings = pBuildings;
		tAsset.check_full_village = pCheckFullVillage;
		tAsset.check_house_limit = pCheckHouseLimit;
		tAsset.min_zones = pMinZones;
		list.Add(tAsset);
		BuildOrderLibrary.b = tAsset;
		return tAsset;
	}

	public void prepareForAssetGeneration()
	{
		HashSet<string> tTempHashset = UnsafeCollectionPool<HashSet<string>, string>.Get();
		foreach (BuildOrder tBuildOrder in list)
		{
			tTempHashset.Add(tBuildOrder.id);
			if (tBuildOrder.requirements_orders != null)
			{
				tTempHashset.UnionWith(tBuildOrder.requirements_orders);
			}
		}
		list_for_generation = tTempHashset.ToArray();
		UnsafeCollectionPool<HashSet<string>, string>.Release(tTempHashset);
	}
}
