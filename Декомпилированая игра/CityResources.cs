using System;
using System.Collections.Generic;

[Serializable]
public class CityResources : IDisposable
{
	[NonSerialized]
	private Dictionary<string, CityStorageSlot> _resources = new Dictionary<string, CityStorageSlot>();

	[NonSerialized]
	private List<CityStorageSlot> _list_food = new List<CityStorageSlot>();

	[NonSerialized]
	private List<CityStorageSlot> _list_other = new List<CityStorageSlot>();

	public List<CityStorageSlot> saved_resources;

	public void loadFromSave()
	{
		if (saved_resources == null)
		{
			return;
		}
		foreach (CityStorageSlot tRes in saved_resources)
		{
			if (AssetManager.resources.get(tRes.id) != null && tRes.amount >= 0)
			{
				tRes.create(tRes.id);
				putToDict(tRes);
			}
		}
	}

	public int get(string pRes)
	{
		if (_resources.TryGetValue(pRes, out var tSlot))
		{
			return tSlot.amount;
		}
		return 0;
	}

	public int change(string pRes, int pAmount = 1)
	{
		int tResult = 0;
		if (DebugConfig.isOn(DebugOption.CityInfiniteResources))
		{
			pAmount = 999;
		}
		if (_resources.TryGetValue(pRes, out var tSlot))
		{
			tSlot.amount += pAmount;
			if (tSlot.amount > tSlot.asset.maximum)
			{
				tSlot.amount = tSlot.asset.maximum;
			}
			return tSlot.amount;
		}
		return addNew(pRes, pAmount);
	}

	private int addNew(string pResID, int pAmount)
	{
		CityStorageSlot tRes = new CityStorageSlot(pResID);
		tRes.amount = pAmount;
		putToDict(tRes);
		return tRes.amount;
	}

	public bool hasSpaceForResource(ResourceAsset pAsset)
	{
		if (get(pAsset.id) < pAsset.storage_max)
		{
			return true;
		}
		return false;
	}

	public bool hasResourcesForNewItems()
	{
		foreach (ResourceAsset tAsset in AssetManager.resources.strategic_resource_assets)
		{
			if (get(tAsset.id) > 10)
			{
				return true;
			}
		}
		return false;
	}

	public void set(string pRes, int pAmount)
	{
		if (_resources.TryGetValue(pRes, out var tSlot))
		{
			tSlot.amount = pAmount;
		}
		else
		{
			addNew(pRes, pAmount);
		}
	}

	private void putToDict(CityStorageSlot pRes)
	{
		if (!_resources.ContainsKey(pRes.id))
		{
			_resources.Add(pRes.id, pRes);
			if (pRes.asset.food)
			{
				_list_food.Add(pRes);
			}
			else
			{
				_list_other.Add(pRes);
			}
		}
	}

	public ResourceAsset getRandomSuitableFood(Subspecies pSubspecies, string pSpecificFood = null)
	{
		if (_list_food.Count == 0)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(pSpecificFood) && get(pSpecificFood) > 0)
		{
			return AssetManager.resources.get(pSpecificFood);
		}
		HashSet<string> tAllowedFood = pSubspecies.getAllowedFoodByDiet();
		ResourceAsset tResult = getAvailableFoodAsset(_list_food, tAllowedFood, pSort: true);
		if (tResult == null)
		{
			tResult = getAvailableFoodAsset(_list_other, tAllowedFood, pSort: false);
		}
		return tResult;
	}

	private ResourceAsset getAvailableFoodAsset(List<CityStorageSlot> pList, HashSet<string> pAllowedFood, bool pSort)
	{
		if (pSort)
		{
			pList.Sort(foodSorter);
		}
		for (int i = 0; i < pList.Count; i++)
		{
			CityStorageSlot tSlot = pList[i];
			if (tSlot.amount != 0 && pAllowedFood.Contains(tSlot.id))
			{
				return tSlot.asset;
			}
		}
		return null;
	}

	public int foodSorter(CityStorageSlot o1, CityStorageSlot o2)
	{
		return o2.amount.CompareTo(o1.amount);
	}

	public int countFood()
	{
		int tResult = 0;
		foreach (CityStorageSlot tSlot in _list_food)
		{
			tResult += tSlot.amount;
		}
		return tResult;
	}

	public ResourceAsset getRandomFoodAsset()
	{
		if (_list_food.Count == 0)
		{
			return null;
		}
		return _list_food.GetRandom().asset;
	}

	public void save()
	{
		saved_resources = new List<CityStorageSlot>();
		foreach (CityStorageSlot tSlot in getSlots())
		{
			if (tSlot.amount != 0)
			{
				saved_resources.Add(tSlot);
			}
		}
	}

	public IEnumerable<string> getKeys()
	{
		return _resources.Keys;
	}

	public IEnumerable<CityStorageSlot> getSlots()
	{
		return _resources.Values;
	}

	public void Dispose()
	{
		_resources.Clear();
		_list_food.Clear();
		_list_other.Clear();
		saved_resources?.Clear();
	}
}
