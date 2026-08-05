using System.Collections.Generic;
using UnityEngine;

public class NameplateLibrary : AssetLibrary<NameplateAsset>
{
	public readonly Dictionary<MetaType, NameplateAsset> map_modes_nameplates = new Dictionary<MetaType, NameplateAsset>();

	private NameplateAsset _plate_kingdom;

	private NameplateAsset _plate_city;

	private const int OFFSET_UNIT_Y = -2;

	public override void init()
	{
		base.init();
		add(new NameplateAsset
		{
			id = "plate_subspecies",
			path_sprite = "ui/nameplates/nameplate_subspecies",
			padding_left = 11,
			padding_right = 13,
			banner_only_mode_scale = 1.8f,
			map_mode = MetaType.Subspecies,
			overlap_for_fluid_mode = true,
			action_main = actionSubspecies
		});
		add(new NameplateAsset
		{
			id = "plate_army",
			path_sprite = "ui/nameplates/nameplate_army",
			padding_left = 26,
			padding_right = 18,
			padding_top = -2,
			banner_only_mode_scale = 1.5f,
			map_mode = MetaType.Army,
			action_main = actionArmy
		});
		add(new NameplateAsset
		{
			id = "plate_family",
			path_sprite = "ui/nameplates/nameplate_family",
			padding_left = 11,
			padding_right = 13,
			banner_only_mode_scale = 1.5f,
			map_mode = MetaType.Family,
			overlap_for_fluid_mode = true,
			action_main = actionFamily
		});
		add(new NameplateAsset
		{
			id = "plate_religion",
			path_sprite = "ui/nameplates/nameplate_religion",
			padding_left = 11,
			padding_right = 13,
			map_mode = MetaType.Religion,
			action_main = actionReligion
		});
		add(new NameplateAsset
		{
			id = "plate_culture",
			path_sprite = "ui/nameplates/nameplate_culture",
			padding_left = 11,
			padding_right = 13,
			map_mode = MetaType.Culture,
			action_main = actionCulture
		});
		add(new NameplateAsset
		{
			id = "plate_language",
			path_sprite = "ui/nameplates/nameplate_language",
			padding_left = 11,
			padding_right = 13,
			map_mode = MetaType.Language,
			action_main = actionLanguage
		});
		add(new NameplateAsset
		{
			id = "plate_alliance",
			path_sprite = "ui/nameplates/nameplate_alliance",
			map_mode = MetaType.Alliance,
			banner_only_mode_scale = 3f,
			padding_left = 14,
			padding_top = 2,
			action_main = actionAlliance
		});
		_plate_kingdom = add(new NameplateAsset
		{
			id = "plate_kingdom",
			path_sprite = "ui/nameplates/nameplate_kingdom",
			padding_left = 26,
			padding_right = 26,
			padding_top = -2,
			banner_only_mode_scale = 2.5f,
			map_mode = MetaType.Kingdom,
			action_main = actionKingdom
		});
		_plate_city = add(new NameplateAsset
		{
			id = "plate_city",
			path_sprite = "ui/nameplates/nameplate_city",
			map_mode = MetaType.City,
			banner_only_mode_scale = 2.5f,
			padding_left = 6,
			padding_right = 7,
			padding_top = -2,
			action_main = actionCity
		});
		add(new NameplateAsset
		{
			id = "plate_clan",
			path_sprite = "ui/nameplates/nameplate_clan",
			map_mode = MetaType.Clan,
			padding_left = 17,
			padding_right = 24,
			action_main = actionClan
		});
	}

	private bool isWithinCamera(Vector2 pVector)
	{
		return World.world.move_camera.isWithinCameraViewNotPowerBar(pVector);
	}

	public override NameplateAsset add(NameplateAsset pAsset)
	{
		map_modes_nameplates.Add(pAsset.map_mode, pAsset);
		return base.add(pAsset);
	}

	private void actionAlliance(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		foreach (Alliance tAlliance in World.world.alliances)
		{
			City tBestCity = null;
			Kingdom tBestKingdom = null;
			foreach (Kingdom tKingdom in tAlliance.kingdoms_hashset)
			{
				if (tKingdom.hasCapital() && isWithinCamera(tKingdom.capital.city_center) && (tBestKingdom == null || tBestKingdom.power < tKingdom.power))
				{
					tBestKingdom = tKingdom;
				}
			}
			if (tBestKingdom != null && tBestKingdom.hasCapital())
			{
				tBestCity = tBestKingdom.capital;
			}
			if (tBestCity != null)
			{
				pManager.prepareNext(pAsset, tAlliance).showTextAlliance(tAlliance, tBestCity);
			}
		}
		foreach (Kingdom tKingdom2 in World.world.kingdoms)
		{
			if (tKingdom2.hasCapital() && !tKingdom2.hasAlliance() && isWithinCamera(tKingdom2.capital.city_center))
			{
				if (tCurrent >= pAsset.max_nameplate_count)
				{
					break;
				}
				tCurrent++;
				pManager.prepareNext(_plate_kingdom, tKingdom2).showTextKingdom(tKingdom2, tKingdom2.capital.city_center);
			}
		}
	}

	private void actionReligion(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		switch (MetaTypeLibrary.religion.getZoneOptionState())
		{
		case 0:
		{
			foreach (Kingdom tKingdom in World.world.kingdoms)
			{
				if (tKingdom.hasReligion() && tKingdom.hasCapital() && isWithinCamera(tKingdom.capital.city_center))
				{
					pManager.prepareNext(pAsset, tKingdom.religion).showTextReligion(tKingdom.religion, tKingdom.capital.city_center);
				}
			}
			return;
		}
		case 1:
		{
			foreach (City tCity in World.world.cities)
			{
				if (tCity.hasReligion())
				{
					Religion tMeta = tCity.getReligion();
					if (isWithinCamera(tCity.city_center))
					{
						pManager.prepareNext(pAsset, tMeta).showTextReligion(tMeta, tCity.city_center);
					}
				}
			}
			return;
		}
		}
		foreach (Religion tMeta2 in World.world.religions)
		{
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			if (getPositionForMeta(tMeta2, out var tPosition))
			{
				pManager.prepareNext(pAsset, tMeta2).showTextReligion(tMeta2, tPosition);
				tCurrent++;
			}
		}
	}

	private void actionLanguage(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		switch (MetaTypeLibrary.language.getZoneOptionState())
		{
		case 0:
		{
			foreach (Kingdom tKingdom in World.world.kingdoms)
			{
				if (tKingdom.hasLanguage() && tKingdom.hasCapital() && isWithinCamera(tKingdom.capital.city_center))
				{
					pManager.prepareNext(pAsset, tKingdom.language).showTextLanguage(tKingdom.language, tKingdom.capital.city_center);
				}
			}
			return;
		}
		case 1:
		{
			foreach (City tCity in World.world.cities)
			{
				if (tCity.hasLanguage())
				{
					Language tMeta = tCity.getLanguage();
					if (isWithinCamera(tCity.city_center))
					{
						pManager.prepareNext(pAsset, tMeta).showTextLanguage(tMeta, tCity.city_center);
					}
				}
			}
			return;
		}
		}
		foreach (Language tMeta2 in World.world.languages)
		{
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			if (getPositionForMeta(tMeta2, out var tPosition))
			{
				pManager.prepareNext(pAsset, tMeta2).showTextLanguage(tMeta2, tPosition);
				tCurrent++;
			}
		}
	}

	private void actionCulture(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		switch (MetaTypeLibrary.culture.getZoneOptionState())
		{
		case 0:
		{
			foreach (Kingdom tKingdom in World.world.kingdoms)
			{
				if (tKingdom.hasCulture() && tKingdom.hasCapital() && isWithinCamera(tKingdom.capital.city_center))
				{
					pManager.prepareNext(pAsset, tKingdom.culture).showTextCulture(tKingdom.culture, tKingdom.capital.city_center);
				}
			}
			return;
		}
		case 1:
		{
			foreach (City tCity in World.world.cities)
			{
				if (tCity.hasCulture())
				{
					Culture tMeta = tCity.getCulture();
					if (isWithinCamera(tCity.city_center))
					{
						pManager.prepareNext(pAsset, tMeta).showTextCulture(tMeta, tCity.city_center);
					}
				}
			}
			return;
		}
		}
		foreach (Culture tMeta2 in World.world.cultures)
		{
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			if (getPositionForMeta(tMeta2, out var tPosition))
			{
				pManager.prepareNext(pAsset, tMeta2).showTextCulture(tMeta2, tPosition);
				tCurrent++;
			}
		}
	}

	private void actionCity(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		using ListPool<City> tSortedCities = new ListPool<City>(World.world.cities.list);
		tSortedCities.Sort(sortByMembers);
		if (MetaTypeLibrary.city.getZoneOptionState() == 0)
		{
			foreach (ref City item in tSortedCities)
			{
				City tCity = item;
				if (tCurrent >= pAsset.max_nameplate_count)
				{
					break;
				}
				if (isWithinCamera(tCity.city_center))
				{
					pManager.prepareNext(_plate_city, tCity).showTextCity(tCity, tCity.city_center);
					tCurrent++;
				}
			}
			return;
		}
		foreach (City tMeta in World.world.cities)
		{
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			Actor tUnit = null;
			if (tMeta.hasLeader() && !tMeta.leader.isRekt() && tMeta.leader.is_visible)
			{
				tUnit = tMeta.leader;
			}
			if (getPositionForMeta(tMeta, out var tPosition, tUnit))
			{
				pManager.prepareNext(pAsset, tMeta).showTextCity(tMeta, tPosition);
				tCurrent++;
			}
		}
	}

	private void actionKingdom(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		if (MetaTypeLibrary.kingdom.getZoneOptionState() == 0)
		{
			foreach (Kingdom tKingdom in World.world.kingdoms)
			{
				if (tKingdom.hasCapital() && isWithinCamera(tKingdom.capital.city_center))
				{
					pManager.prepareNext(pAsset, tKingdom).showTextKingdom(tKingdom, tKingdom.capital.city_center);
				}
			}
			return;
		}
		foreach (Kingdom tMeta in World.world.kingdoms)
		{
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			Actor tUnit = null;
			if (tMeta.hasKing() && !tMeta.king.isRekt() && tMeta.king.is_visible)
			{
				tUnit = tMeta.king;
			}
			if (getPositionForMeta(tMeta, out var tPosition, tUnit))
			{
				pManager.prepareNext(pAsset, tMeta).showTextKingdom(tMeta, tPosition);
				tCurrent++;
			}
		}
	}

	private void actionSubspecies(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		switch (MetaTypeLibrary.subspecies.getZoneOptionState())
		{
		case 0:
		{
			foreach (Kingdom tKingdom in World.world.kingdoms)
			{
				Subspecies tMeta2 = tKingdom.getMainSubspecies();
				if (!tMeta2.isRekt() && tKingdom.hasCapital() && isWithinCamera(tKingdom.capital.city_center))
				{
					pManager.prepareNext(pAsset, tMeta2).showTextSubspecies(tMeta2, tKingdom.capital.city_center);
				}
			}
			return;
		}
		case 1:
		{
			foreach (City tCity in World.world.cities)
			{
				Subspecies tMeta = tCity.getMainSubspecies();
				if (!tMeta.isRekt() && isWithinCamera(tCity.city_center))
				{
					pManager.prepareNext(pAsset, tMeta).showTextSubspecies(tMeta, tCity.city_center);
				}
			}
			return;
		}
		}
		foreach (Subspecies tMeta3 in World.world.subspecies)
		{
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			if (getPositionForMeta(tMeta3, out var tPosition))
			{
				pManager.prepareNext(pAsset, tMeta3).showTextSubspecies(tMeta3, tPosition);
				tCurrent++;
			}
		}
	}

	private bool getPositionForMeta(IMetaObject pMetaObject, out Vector3 pPosition, Actor pForceActor = null)
	{
		if (!pMetaObject.isAlive() || !pMetaObject.hasUnits())
		{
			pPosition = Vector3.zero;
			return false;
		}
		Actor tActorForPosition = pForceActor;
		if (tActorForPosition == null)
		{
			tActorForPosition = pMetaObject.getOldestVisibleUnitForNameplatesCached();
		}
		if (tActorForPosition == null)
		{
			pPosition = Vector3.zero;
			return false;
		}
		Vector3 tPositionResult = tActorForPosition.current_position;
		tPositionResult.y += tActorForPosition.getHeight();
		tPositionResult.y += -2f;
		pPosition = tPositionResult;
		return true;
	}

	private int sortByMembers(IMetaObject pObject1, IMetaObject pObject2)
	{
		int tFavoriteComparison = pObject2.isFavorite().CompareTo(pObject1.isFavorite());
		if (tFavoriteComparison != 0)
		{
			return tFavoriteComparison;
		}
		int tSelectedComparison = pObject2.isSelected().CompareTo(pObject1.isSelected());
		if (tSelectedComparison != 0)
		{
			return tSelectedComparison;
		}
		return pObject2.countUnits().CompareTo(pObject1.countUnits());
	}

	private void actionArmy(NameplateManager pManager, NameplateAsset pAsset)
	{
		MetaTypeLibrary.army.getZoneOptionState();
		using ListPool<Army> tSortedArmies = new ListPool<Army>(World.world.armies.list);
		tSortedArmies.Sort(sortByMembers);
		int tCurrent = 0;
		foreach (ref Army item in tSortedArmies)
		{
			Army tMeta = item;
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			Actor tUnit = null;
			if (tMeta.hasCaptain())
			{
				tUnit = tMeta.getCaptain();
			}
			if (getPositionForMeta(tMeta, out var tPosition, tUnit))
			{
				pManager.prepareNext(pAsset, tMeta).showTextArmy(tMeta, tPosition);
				tCurrent++;
			}
		}
	}

	private void actionFamily(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		switch (MetaTypeLibrary.family.getZoneOptionState())
		{
		case 0:
		{
			foreach (Kingdom tKingdom in World.world.kingdoms)
			{
				if (tKingdom.hasCapital() && tKingdom.hasKing() && tKingdom.king.hasFamily() && isWithinCamera(tKingdom.capital.city_center))
				{
					pManager.prepareNext(pAsset, tKingdom.king.family).showTextFamily(tKingdom.king.family, tKingdom.capital.city_center);
				}
			}
			return;
		}
		case 1:
		{
			foreach (City tCity in World.world.cities)
			{
				if (tCity.hasLeader() && tCity.leader.hasFamily())
				{
					Family tMeta = tCity.leader.family;
					if (tMeta != null && isWithinCamera(tCity.city_center))
					{
						pManager.prepareNext(pAsset, tMeta).showTextFamily(tMeta, tCity.city_center);
					}
				}
			}
			return;
		}
		}
		using ListPool<Family> tSortedFamilies = new ListPool<Family>(World.world.families.list);
		tSortedFamilies.Sort(sortByMembers);
		foreach (ref Family item in tSortedFamilies)
		{
			Family tMeta2 = item;
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			if (getPositionForMeta(tMeta2, out var tPosition))
			{
				pManager.prepareNext(pAsset, tMeta2).showTextFamily(tMeta2, tPosition);
				tCurrent++;
			}
		}
	}

	private void actionClan(NameplateManager pManager, NameplateAsset pAsset)
	{
		int tCurrent = 0;
		switch (MetaTypeLibrary.clan.getZoneOptionState())
		{
		case 0:
		{
			foreach (Kingdom tKingdom in World.world.kingdoms)
			{
				if (tKingdom.hasCapital() && tKingdom.hasKing() && tKingdom.king.hasClan() && isWithinCamera(tKingdom.capital.city_center))
				{
					pManager.prepareNext(pAsset, tKingdom.king.clan).showTextClanCity(tKingdom.king.clan, tKingdom.capital);
				}
			}
			return;
		}
		case 1:
		{
			foreach (City tCity in World.world.cities)
			{
				Clan tMeta = tCity.getRoyalClan();
				if (tMeta != null && isWithinCamera(tCity.city_center))
				{
					pManager.prepareNext(pAsset, tMeta).showTextClanCity(tMeta, tCity);
				}
			}
			return;
		}
		}
		foreach (Clan tMeta2 in World.world.clans)
		{
			if (tCurrent >= pAsset.max_nameplate_count)
			{
				break;
			}
			if (getPositionForMeta(tMeta2, out var tPosition))
			{
				pManager.prepareNext(pAsset, tMeta2).showTextClanFluid(tMeta2, tPosition);
				tCurrent++;
			}
		}
	}
}
