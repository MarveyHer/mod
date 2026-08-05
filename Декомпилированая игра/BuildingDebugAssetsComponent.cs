using System.Collections.Generic;

public class BuildingDebugAssetsComponent : BaseDebugAssetsComponent<BuildingAsset, BuildingDebugAssetElement, BuildingAssetElementPlace>
{
	protected override List<BuildingAsset> getAssetsList()
	{
		return AssetManager.buildings.list;
	}

	protected override void init()
	{
		sorting_tab.addButton("ui/Icons/iconHealth", "sort_by_health", base.setDataResorted, delegate
		{
			list_assets_sorted = list_assets_sorting;
			list_assets_sorted.Sort(sortByHealth);
			checkReverseSort();
		});
		sorting_tab.addButton("ui/Icons/iconDamage", "sort_by_damage", base.setDataResorted, delegate
		{
			list_assets_sorted = list_assets_sorting;
			list_assets_sorted.Sort(sortByDamage);
			checkReverseSort();
		});
		sorting_tab.addButton("ui/Icons/iconPopulationAttackers", "sort_by_targets", base.setDataResorted, delegate
		{
			list_assets_sorted = list_assets_sorting;
			list_assets_sorted.Sort(sortByTargets);
			checkReverseSort();
		});
		sorting_tab.addButton("effects/circle132", "sort_by_area_of_effect", base.setDataResorted, delegate
		{
			list_assets_sorted = list_assets_sorting;
			list_assets_sorted.Sort(sortByAreaOfEffect);
			checkReverseSort();
		});
		base.init();
	}

	private int sortByHealth(BuildingAsset pObject1, BuildingAsset pObject2)
	{
		return -pObject1.base_stats["health"].CompareTo(pObject2.base_stats["health"]);
	}

	private int sortByDamage(BuildingAsset pObject1, BuildingAsset pObject2)
	{
		return -pObject1.base_stats["damage"].CompareTo(pObject2.base_stats["damage"]);
	}

	private int sortByTargets(BuildingAsset pObject1, BuildingAsset pObject2)
	{
		return -pObject1.base_stats["targets"].CompareTo(pObject2.base_stats["targets"]);
	}

	private int sortByAreaOfEffect(BuildingAsset pObject1, BuildingAsset pObject2)
	{
		return -pObject1.base_stats["area_of_effect"].CompareTo(pObject2.base_stats["area_of_effect"]);
	}

	protected override List<BuildingAsset> getListCivsSort()
	{
		bool tShow = sorting_tab.getCurrentButton().getState() == SortButtonState.Up;
		List<BuildingAsset> tResult = new List<BuildingAsset>();
		foreach (BuildingAsset tAsset in getAssetsList())
		{
			bool tStringEmpty = string.IsNullOrEmpty(tAsset.civ_kingdom);
			if (!(tStringEmpty && tShow) && (tStringEmpty || tShow))
			{
				tResult.Add(tAsset);
			}
		}
		return tResult;
	}
}
