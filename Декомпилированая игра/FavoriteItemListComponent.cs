using System.Collections.Generic;

public class FavoriteItemListComponent : ComponentListBase<FavoriteItemListElement, Item, ItemData, FavoriteItemListComponent>
{
	private List<NanoObject> _meta_objects = new List<NanoObject>();

	protected override MetaType meta_type => MetaType.Item;

	protected override void setupSortingTabs()
	{
		sorting_tab.tryAddButton("ui/Icons/iconAge", "sort_by_age", show, delegate
		{
			current_sort = sortByAge;
		});
		sorting_tab.tryAddButton("ui/Icons/iconKills", "sort_by_kills", show, delegate
		{
			current_sort = sortByKills;
		});
		sorting_tab.tryAddButton("ui/Icons/iconDamage", "sort_by_damage", show, delegate
		{
			current_sort = sortByDamage;
		});
		sorting_tab.tryAddButton("ui/Icons/iconArmor", "sort_by_armor", show, delegate
		{
			current_sort = sortByArmor;
		});
		sorting_tab.tryAddButton("ui/Icons/iconItemType", "sort_by_type", show, delegate
		{
			current_sort = sortByType;
		});
		sorting_tab.tryAddButton("ui/Icons/iconItemQuality", "sort_by_quality", show, delegate
		{
			current_sort = sortByQuality;
		});
		sorting_tab.tryAddButton("ui/Icons/iconCity", "sort_by_city", show, delegate
		{
			current_sort = sortByCity;
		});
		sorting_tab.tryAddButton("ui/Icons/iconHumans", "sort_by_owner", show, delegate
		{
			current_sort = sortByOwner;
		});
	}

	protected override IEnumerable<Item> getObjectsList()
	{
		_meta_objects.Clear();
		foreach (Item tItem in World.world.items)
		{
			if (!tItem.isRekt() && tItem.isFavorite())
			{
				_meta_objects.Add(tItem);
				if (tItem.hasCity())
				{
					_meta_objects.Add(tItem.getCity());
				}
				if (tItem.hasActor())
				{
					_meta_objects.Add(tItem.getActor());
				}
				yield return tItem;
			}
		}
	}

	public static int sortByAge(Item pItem1, Item pItem2)
	{
		return -pItem2.data.created_time.CompareTo(pItem1.data.created_time);
	}

	public static int sortByKills(Item pItem1, Item pItem2)
	{
		return pItem2.data.kills.CompareTo(pItem1.data.kills);
	}

	public static int sortByType(Item pItem1, Item pItem2)
	{
		return pItem2.getAsset().equipment_type.CompareTo(pItem1.getAsset().equipment_type);
	}

	public static int sortByQuality(Item pItem1, Item pItem2)
	{
		return pItem2.getQuality().CompareTo(pItem1.getQuality());
	}

	public static int sortByCity(Item pItem1, Item pItem2)
	{
		int tCityCompare = pItem1.hasCity().CompareTo(pItem2.hasCity());
		if (tCityCompare != 0)
		{
			return tCityCompare;
		}
		if (pItem1.hasCity() && pItem2.hasCity())
		{
			int tKingdomCompare = pItem2.getCity().kingdom.CompareTo(pItem1.getCity().kingdom);
			if (tKingdomCompare != 0)
			{
				return tKingdomCompare;
			}
			return pItem2.getCity().name.CompareTo(pItem1.getCity().name);
		}
		return pItem2.name.CompareTo(pItem1.name);
	}

	public static int sortByOwner(Item pItem1, Item pItem2)
	{
		int tActorCompare = pItem1.hasActor().CompareTo(pItem2.hasActor());
		if (tActorCompare != 0)
		{
			return tActorCompare;
		}
		if (pItem1.hasActor() && pItem2.hasActor())
		{
			Actor tActor1 = pItem1.getActor();
			Actor tActor2 = pItem2.getActor();
			int tKingdomCompare = tActor1.kingdom.CompareTo(tActor2.kingdom);
			if (tKingdomCompare != 0)
			{
				return tKingdomCompare;
			}
			int tCityCompare = tActor1.hasCity().CompareTo(tActor2.hasCity());
			if (tCityCompare != 0)
			{
				return tCityCompare;
			}
			if (tActor1.hasCity() && tActor2.hasCity())
			{
				int tCityCompare2 = tActor1.getCity().name.CompareTo(tActor2.getCity().name);
				if (tCityCompare2 != 0)
				{
					return tCityCompare2;
				}
			}
			return pItem2.getActor().name.CompareTo(pItem1.getActor().name);
		}
		return pItem2.name.CompareTo(pItem1.name);
	}

	public static int sortByDamage(Item pItem1, Item pItem2)
	{
		return pItem2.getFullStats()["damage"].CompareTo(pItem1.getFullStats()["damage"]);
	}

	public static int sortByArmor(Item pItem1, Item pItem2)
	{
		return pItem2.getFullStats()["armor"].CompareTo(pItem1.getFullStats()["armor"]);
	}

	public override void clear()
	{
		base.clear();
		_meta_objects.Clear();
	}

	public override bool checkRefreshWindow()
	{
		foreach (NanoObject meta_object in _meta_objects)
		{
			if (meta_object.isRekt())
			{
				return true;
			}
		}
		return base.checkRefreshWindow();
	}
}
