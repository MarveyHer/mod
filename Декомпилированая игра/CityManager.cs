using System.Collections.Generic;

public class CityManager : MetaSystemManager<City, CityData>
{
	private bool _dirty_buildings;

	public CityManager()
	{
		type_id = "city";
	}

	public City newCity(Kingdom pKingdom, TileZone pZone, Actor pOriginalActor)
	{
		World.world.game_stats.data.citiesCreated++;
		World.world.map_stats.citiesCreated++;
		City tCity = newObject();
		tCity.data.founder_id = pOriginalActor.getID();
		tCity.data.founder_name = pOriginalActor.name;
		tCity.data.original_actor_asset = pOriginalActor.asset.id;
		tCity.data.equipment = new CityEquipment();
		tCity.setKingdom(pKingdom);
		tCity.addZone(pZone);
		TileZone[] tNeighbours = pZone.neighbours_all;
		foreach (TileZone tZone in tNeighbours)
		{
			if (tZone.city == null)
			{
				tCity.addZone(tZone);
			}
		}
		World.world.city_zone_helper.city_place_finder.setDirty();
		return tCity;
	}

	public City buildNewCity(Actor pActor, TileZone pZone)
	{
		Kingdom tKingdom = pActor.kingdom;
		City city = World.world.cities.newCity(tKingdom, pZone, pActor);
		city.setUnitMetas(pActor);
		city.newCityEvent(pActor);
		WorldLog.logNewCity(city);
		return city;
	}

	public bool tryToCreateCity(Actor pActor, ListPool<Building> pBuildingList)
	{
		if (pActor.current_tile.zone.hasCity())
		{
			return false;
		}
		return true;
	}

	public bool canStartNewCityCivilizationHere(Actor pActor)
	{
		if (pActor.kingdom.asset.is_forced_by_trait)
		{
			return false;
		}
		if (!pActor.canBuildNewCity())
		{
			return false;
		}
		KingdomAsset tPossibleKingdomAsset = AssetManager.kingdoms.get(pActor.asset.kingdom_id_civilization);
		if (tPossibleKingdomAsset == null || !tPossibleKingdomAsset.civ)
		{
			return false;
		}
		WorldTile tActorTile = pActor.current_tile;
		TileZone tActorZone = tActorTile.zone;
		TileZone[] tNeighbours = tActorZone.neighbours;
		foreach (TileZone tNeighbourZone in tNeighbours)
		{
			if (tNeighbourZone.hasCity())
			{
				WorldTile tTile = tNeighbourZone.city.getTile();
				if (tTile != null && tTile.isSameIsland(tActorTile))
				{
					tNeighbourZone.city.addZone(tActorZone);
					return false;
				}
			}
		}
		return true;
	}

	public City buildFirstCivilizationCity(Actor pActor)
	{
		City tNewCity = buildNewCity(pActor, pActor.current_zone);
		pActor.joinCity(tNewCity);
		tNewCity.setUnitMetas(pActor);
		tNewCity.convertSameSpeciesAroundUnit(pActor);
		return tNewCity;
	}

	protected override void updateDirtyUnits()
	{
		List<Actor> tActorList = World.world.units.units_only_alive;
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tUnit = tActorList[i];
			City tCity = tUnit.city;
			if (tCity != null && tCity.isDirtyUnits())
			{
				tCity.listUnit(tUnit);
			}
		}
	}

	public void beginChecksBuildings()
	{
		if (_dirty_buildings)
		{
			updateDirtyBuildings();
		}
		_dirty_buildings = false;
	}

	private void updateDirtyBuildings()
	{
		clearAllBuildingLists();
		using IEnumerator<City> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			City tCity = enumerator.Current;
			Kingdom tKingdomCiv = tCity.kingdom;
			for (int i = 0; i < tCity.zones.Count; i++)
			{
				foreach (Building tBuilding in tCity.zones[i].buildings_all)
				{
					if (tBuilding.asset.city_building && tBuilding.isUsable())
					{
						tBuilding.setKingdomCiv(tKingdomCiv);
						tCity.listBuilding(tBuilding);
					}
				}
			}
		}
	}

	public void setDirtyBuildings(City pCity)
	{
		_dirty_buildings = true;
		World.world.kingdoms.setDirtyBuildings();
	}

	private void clearAllBuildingLists()
	{
		using IEnumerator<City> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.clearBuildingList();
		}
	}

	protected override void addObject(City pObject)
	{
		pObject.init();
		base.addObject(pObject);
	}

	public override City loadObject(CityData pData)
	{
		City city = base.loadObject(pData);
		city.loadCity(pData);
		return city;
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		using IEnumerator<City> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			City current = enumerator.Current;
			current.update(pElapsed);
			current.clearCursorOver();
		}
	}

	public void updateAge()
	{
		using IEnumerator<City> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.updateAge();
		}
	}

	public override List<CityData> save(List<City> pList = null)
	{
		List<CityData> tSavingList = new List<CityData>();
		using IEnumerator<City> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			City tCity = enumerator.Current;
			tCity.save();
			tSavingList.Add(tCity.data);
		}
		return tSavingList;
	}

	private void checkForCityErrors(SavedMap pSaveData)
	{
		List<CityData> tCityData = new List<CityData>();
		for (int i = 0; i < pSaveData.cities.Count; i++)
		{
			CityData tData = pSaveData.cities[i];
			if (tData.zones.Count != 0)
			{
				TileZone tFirstZone = World.world.zone_calculator.getZone(tData.zones[0].x, tData.zones[0].y);
				if (pSaveData.saveVersion < 7)
				{
					tFirstZone = findZoneViaBuilding(tData.id, pSaveData.buildings);
				}
				if (tFirstZone != null)
				{
					tCityData.Add(tData);
				}
			}
		}
		pSaveData.cities = tCityData;
	}

	public void loadCities(SavedMap pSaveData)
	{
		checkForCityErrors(pSaveData);
		for (int i = 0; i < pSaveData.cities.Count; i++)
		{
			CityData tData = pSaveData.cities[i];
			City tCity = loadObject(tData);
			if (tCity == null || pSaveData.saveVersion < 7)
			{
				continue;
			}
			for (int j = 0; j < tData.zones.Count; j++)
			{
				ZoneData tZoneData = tData.zones[j];
				TileZone tZone = World.world.zone_calculator.getZone(tZoneData.x, tZoneData.y);
				if (tZone != null)
				{
					tCity.addZone(tZone);
				}
			}
		}
	}

	public override void removeObject(City pObject)
	{
		World.world.game_stats.data.citiesDestroyed++;
		World.world.map_stats.citiesDestroyed++;
		WorldLog.logCityDestroyed(pObject);
		pObject.destroyCity();
		base.removeObject(pObject);
		World.world.city_zone_helper.city_place_finder.setDirty();
		World.world.cultures.setDirtyCities();
		World.world.kingdoms.setDirtyCities();
		World.world.languages.setDirtyCities();
		World.world.religions.setDirtyCities();
	}

	private TileZone findZoneViaBuilding(long pID, List<BuildingData> pList)
	{
		for (int i = 0; i < pList.Count; i++)
		{
			BuildingData tData = pList[i];
			if (tData.cityID == pID)
			{
				return World.world.GetTileSimple(tData.mainX, tData.mainY).zone;
			}
		}
		return null;
	}

	public override bool isLocked()
	{
		if (isUnitsDirty())
		{
			return true;
		}
		if (World.world.kingdoms.hasDirtyCities())
		{
			return true;
		}
		return false;
	}
}
