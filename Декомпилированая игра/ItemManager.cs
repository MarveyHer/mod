using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPools;

public class ItemManager : CoreSystemManager<Item, ItemData>
{
	private HashSet<string> unique_legendary_names = new HashSet<string>();

	private List<Item> _to_remove = new List<Item>();

	private bool _dirty;

	public ItemManager()
	{
		type_id = "item";
		MapBox.on_world_loaded = (Action)Delegate.Combine(MapBox.on_world_loaded, (Action)delegate
		{
			diagnostic();
		});
	}

	public bool isDirty()
	{
		return _dirty;
	}

	public void setDirty()
	{
		_dirty = true;
	}

	public Item newItem(EquipmentAsset pAsset)
	{
		Item item = newObject();
		item.newItem(pAsset);
		return item;
	}

	public void diagnostic()
	{
		Dictionary<Item, int> tDictCities = UnsafeCollectionPool<Dictionary<Item, int>, KeyValuePair<Item, int>>.Get();
		Dictionary<Item, int> tDictUnits = UnsafeCollectionPool<Dictionary<Item, int>, KeyValuePair<Item, int>>.Get();
		foreach (City city in World.world.cities)
		{
			foreach (List<long> allEquipmentList in city.data.equipment.getAllEquipmentLists())
			{
				foreach (long tItemID in allEquipmentList)
				{
					Item tItem = get(tItemID);
					if (tItem != null)
					{
						if (!tDictCities.ContainsKey(tItem))
						{
							tDictCities.Add(tItem, 0);
						}
						tDictCities[tItem]++;
					}
				}
			}
		}
		foreach (Actor tUnit in World.world.units)
		{
			if (!tUnit.hasEquipment())
			{
				continue;
			}
			foreach (ActorEquipmentSlot item in tUnit.equipment)
			{
				Item tItem2 = item.getItem();
				if (tItem2 != null)
				{
					if (!tDictUnits.ContainsKey(tItem2))
					{
						tDictUnits.Add(tItem2, 0);
					}
					tDictUnits[tItem2]++;
				}
			}
		}
		foreach (Item tItem3 in list)
		{
			if (tDictCities.ContainsKey(tItem3) && tDictUnits.ContainsKey(tItem3))
			{
				Debug.LogError("Item Error. Item in city and in unit " + tItem3.id);
			}
		}
		UnsafeCollectionPool<Dictionary<Item, int>, KeyValuePair<Item, int>>.Release(tDictCities);
		UnsafeCollectionPool<Dictionary<Item, int>, KeyValuePair<Item, int>>.Release(tDictUnits);
	}

	public override Item loadObject(ItemData pData)
	{
		if (AssetManager.items.get(pData.asset_id) == null)
		{
			return null;
		}
		return base.loadObject(pData);
	}

	private List<ItemModAsset> getModPool(EquipmentType pType)
	{
		switch (pType)
		{
		case EquipmentType.Ring:
		case EquipmentType.Amulet:
			return AssetManager.items_modifiers.pools["accessory"];
		case EquipmentType.Weapon:
			return AssetManager.items_modifiers.pools["weapon"];
		default:
			return AssetManager.items_modifiers.pools["armor"];
		}
	}

	private ItemModAsset getRandomModFromPool(EquipmentType pType)
	{
		return getModPool(pType).GetRandom();
	}

	public void generateModsFor(Item pItem, int pTries = 1, Actor pActor = null, bool pAddName = true)
	{
		EquipmentAsset tAsset = pItem.getAsset();
		using ListPool<string> tNewNames = new ListPool<string>();
		for (int i = 0; i < pTries; i++)
		{
			if (Randy.randomBool())
			{
				continue;
			}
			ItemModAsset tModAsset = getRandomModFromPool(tAsset.equipment_type);
			if (tModAsset.mod_can_be_given)
			{
				bool tModAdded = tryToAddMod(pItem, tModAsset);
				if (pAddName && tModAdded && checkModName(pItem, tModAsset, tAsset, pActor, out var tName))
				{
					tNewNames.Add(tName);
				}
			}
		}
		if (tAsset.item_modifiers != null)
		{
			for (int j = 0; j < tAsset.item_modifiers.Length; j++)
			{
				ItemModAsset tModAsset2 = tAsset.item_modifiers[j];
				bool tModAdded2 = tryToAddMod(pItem, tModAsset2);
				if (pAddName && tModAdded2 && checkModName(pItem, tModAsset2, tAsset, pActor, out var tName2))
				{
					tNewNames.Add(tName2);
				}
			}
		}
		tNewNames.RemoveAll((string value) => string.IsNullOrEmpty(value));
		if (tNewNames.Count > 0)
		{
			pItem.setName(Randy.getRandom(tNewNames));
		}
	}

	public Item generateItem(EquipmentAsset pItemAsset, Kingdom pKingdom = null, string pWho = null, int pTries = 1, Actor pActor = null, int pFakeCreationYear = 0, bool pByPlayer = false)
	{
		Item tNewItem = newItem(pItemAsset);
		generateModsFor(tNewItem, pTries, pActor);
		tNewItem.data.asset_id = pItemAsset.id;
		tNewItem.data.by = pWho;
		if (!pByPlayer && !pActor.isRekt() && pActor.name == pWho)
		{
			tNewItem.data.creator_id = pActor.getID();
		}
		else
		{
			tNewItem.data.creator_id = -1L;
		}
		tNewItem.created_time_unscaled = Time.time;
		tNewItem.data.created_time -= (float)pFakeCreationYear * 60f;
		tNewItem.data.created_by_player = pByPlayer;
		if (pKingdom != null)
		{
			tNewItem.data.byColor = pKingdom.getColor().color_text;
			tNewItem.data.creator_kingdom_id = pKingdom.id;
			tNewItem.data.from = pKingdom.name;
			tNewItem.data.fromColor = pKingdom.getColor().color_text;
		}
		tNewItem.initItem();
		return tNewItem;
	}

	public override void removeObject(Item pObject)
	{
		base.removeObject(pObject);
		pObject.setShouldBeRemoved();
	}

	public override void clear()
	{
		base.clear();
		unique_legendary_names.Clear();
	}

	private bool tryToAddMod(Item pItem, ItemModAsset pModAsset)
	{
		return pItem.addMod(pModAsset);
	}

	private bool checkModName(Item pItem, ItemModAsset pModAsset, EquipmentAsset pItemAsset, Actor pActor, out string pName)
	{
		pName = null;
		if (pModAsset.quality == Rarity.R3_Legendary)
		{
			int loop = 0;
			while (string.IsNullOrEmpty(pName) || unique_legendary_names.Contains(pName))
			{
				string tNameTemplate = pItemAsset.getRandomNameTemplate(pActor);
				NameGeneratorAsset tNameAsset = AssetManager.name_generator.get(tNameTemplate);
				pName = NameGenerator.generateNameFromTemplate(tNameAsset, pActor);
				if (++loop > 100)
				{
					unique_legendary_names.Clear();
				}
			}
			return true;
		}
		return false;
	}

	public override void checkDeadObjects()
	{
		base.checkDeadObjects();
		if (!isDirty())
		{
			return;
		}
		using (IEnumerator<Item> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Item tItem = enumerator.Current;
				if (tItem.isReadyForRemoval())
				{
					_to_remove.Add(tItem);
				}
			}
		}
		foreach (Item tItem2 in _to_remove)
		{
			removeObject(tItem2);
		}
		_to_remove.Clear();
		_dirty = false;
	}
}
