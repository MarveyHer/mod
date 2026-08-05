using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using db;
using JetBrains.Annotations;
using UnityEngine;

public class City : MetaObject<CityData>
{
	private static readonly HashSet<City> _connected_checked = new HashSet<City>();

	private static readonly HashSet<City> _connected_next_wave = new HashSet<City>();

	private static readonly HashSet<City> _connected_current_wave = new HashSet<City>();

	private readonly Dictionary<string, CityStorageSlot> _total_resource_slots = new Dictionary<string, CityStorageSlot>();

	private readonly Dictionary<UnitProfession, List<Actor>> _professions_dict = new Dictionary<UnitProfession, List<Actor>>();

	private readonly List<Actor> _boats = new List<Actor>();

	private readonly Dictionary<string, long> _species = new Dictionary<string, long>();

	public readonly List<Building> buildings = new List<Building>();

	public readonly Dictionary<string, List<Building>> buildings_dict_type = new Dictionary<string, List<Building>>();

	public readonly Dictionary<string, List<Building>> buildings_dict_id = new Dictionary<string, List<Building>>();

	public readonly CityTasksData tasks = new CityTasksData();

	public readonly CitizenJobs jobs = new CitizenJobs();

	public readonly CityStatus status = new CityStatus();

	public float mark_scale_effect;

	[NonSerialized]
	internal Kingdom kingdom;

	public Culture culture;

	public Language language;

	public Religion religion;

	public Actor leader;

	public Army army;

	internal readonly List<TileZone> zones = new List<TileZone>();

	internal readonly HashSet<TileZone> neighbour_zones = new HashSet<TileZone>();

	internal readonly HashSet<TileZone> border_zones = new HashSet<TileZone>();

	internal readonly HashSet<City> neighbours_cities = new HashSet<City>();

	internal readonly HashSet<City> neighbours_cities_kingdom = new HashSet<City>();

	internal readonly HashSet<Kingdom> neighbours_kingdoms = new HashSet<Kingdom>();

	internal Building under_construction_building;

	internal readonly List<Building> stockpiles = new List<Building>();

	internal readonly List<Building> storages = new List<Building>();

	internal float timer_build_boat;

	internal float timer_build;

	public float timer_action;

	private float _timer_capture;

	private float _timer_warrior;

	internal readonly List<WorldTile> road_tiles_to_build = new List<WorldTile>();

	private readonly List<WorldTile> tiles_to_remove = new List<WorldTile>();

	internal TileZone target_attack_zone;

	internal City target_attack_city;

	internal WorldTile _city_tile;

	internal string _debug_last_possible_build_orders;

	internal string _debug_last_possible_build_orders_no_resources;

	internal string _debug_last_build_order_try;

	internal Kingdom being_captured_by;

	private float _capture_ticks;

	public int last_visual_capture_ticks;

	private bool _dirty_citizens;

	private bool _dirty_city_status;

	private bool _dirty_abandoned_zones;

	internal Vector2 city_center;

	internal Vector2 last_city_center;

	public readonly WorldTileContainer calculated_place_for_farms = new WorldTileContainer();

	public readonly WorldTileContainer calculated_farm_fields = new WorldTileContainer();

	public readonly WorldTileContainer calculated_crops = new WorldTileContainer();

	public readonly WorldTileContainer calculated_grown_wheat = new WorldTileContainer();

	private readonly Dictionary<Kingdom, int> _capturing_units = new Dictionary<Kingdom, int>();

	internal readonly HashSet<TileZone> danger_zones = new HashSet<TileZone>();

	public AiSystemCity ai;

	private int _current_total_food;

	private int _last_checked_job_id;

	private double _loyalty_last_time;

	private int _loyalty_cached;

	private readonly List<long> _cached_book_ids = new List<long>();

	private readonly List<Building> _cached_buildings_with_book_slots = new List<Building>();

	public double timestamp_shrink;

	private int _storage_version;

	protected override MetaType meta_type => MetaType.City;

	public override BaseSystemManager manager => World.world.cities;

	protected override bool track_death_types => true;

	public int amount_wood => getResourcesAmount("wood");

	public int amount_gold => getResourcesAmount("gold");

	public int amount_stone => getResourcesAmount("stone");

	public int amount_common_metals => getResourcesAmount("common_metals");

	public int getStorageVersion()
	{
		return _storage_version;
	}

	public override void increaseBirths()
	{
		base.increaseBirths();
		addRenown(1);
	}

	public void increaseLeft()
	{
		if (isAlive())
		{
			data.left++;
		}
	}

	public void increaseJoined()
	{
		if (isAlive())
		{
			data.joined++;
			addRenown(1);
		}
	}

	public void increaseMoved()
	{
		if (isAlive())
		{
			data.moved++;
			addRenown(2);
		}
	}

	public void increaseMigrants()
	{
		if (isAlive())
		{
			data.migrated++;
		}
	}

	public long getTotalLeft()
	{
		return data.left;
	}

	public long getTotalJoined()
	{
		return data.joined;
	}

	public long getTotalMoved()
	{
		return data.moved;
	}

	public long getTotalMigrated()
	{
		return data.migrated;
	}

	public bool isZoneToClaimStillGood(Actor pActor, TileZone pZone, WorldTile pCityTile)
	{
		if (!pZone.canBeClaimedByCity(this))
		{
			return false;
		}
		if (!pZone.checkCanSettleInThisBiomes(pActor.subspecies))
		{
			return false;
		}
		TileZone[] tNeighbours = pZone.neighbours;
		foreach (TileZone tZone in tNeighbours)
		{
			if (tZone.hasCity() && tZone.city == this)
			{
				return true;
			}
		}
		return false;
	}

	internal override void clearListUnits()
	{
		base.clearListUnits();
		_boats.Clear();
		_species.Clear();
	}

	public override ActorAsset getActorAsset()
	{
		if (hasLeader())
		{
			return leader.getActorAsset();
		}
		return getFounderSpecies();
	}

	public ActorAsset getFounderSpecies()
	{
		return AssetManager.actor_library.get(data.original_actor_asset);
	}

	public CityLayoutTilePlacement getTilePlacementFromZone()
	{
		if (hasCulture())
		{
			if (culture.hasTrait("city_layout_the_grand_arrangement"))
			{
				return CityLayoutTilePlacement.CenterTile;
			}
			if (culture.hasTrait("city_layout_tile_wobbly_pattern"))
			{
				return CityLayoutTilePlacement.CenterTileDrunk;
			}
			if (culture.hasTrait("city_layout_tile_moonsteps"))
			{
				return CityLayoutTilePlacement.Moonsteps;
			}
		}
		return CityLayoutTilePlacement.Random;
	}

	public string getSpecies()
	{
		return getActorAsset().id;
	}

	public override bool isReadyForRemoval()
	{
		if (zones.Count != 0)
		{
			return false;
		}
		return true;
	}

	public void clearBuildingList()
	{
		buildings.Clear();
		foreach (List<Building> value in buildings_dict_type.Values)
		{
			value.Clear();
		}
		foreach (List<Building> value2 in buildings_dict_id.Values)
		{
			value2.Clear();
		}
		stockpiles.Clear();
		storages.Clear();
		_cached_book_ids.Clear();
		_cached_buildings_with_book_slots.Clear();
	}

	public override void listUnit(Actor pActor)
	{
		if (pActor.asset.is_boat)
		{
			_boats.Add(pActor);
			return;
		}
		base.units.Add(pActor);
		if (pActor.hasSubspecies())
		{
			_species[pActor.asset.id] = pActor.subspecies.id;
		}
	}

	public Subspecies getSubspecies(string pSpeciesId)
	{
		long tSubspeciesId = getSubspeciesId(pSpeciesId);
		return World.world.subspecies.get(tSubspeciesId);
	}

	public long getSubspeciesId(string pSpeciesId)
	{
		if (_species.TryGetValue(pSpeciesId, out var tSubspeciesId))
		{
			return tSubspeciesId;
		}
		return -1L;
	}

	public bool hasFreeHouseSlots()
	{
		if (status.housing_free == 0)
		{
			return false;
		}
		return true;
	}

	public bool hasReachedWorldLawLimit()
	{
		if (WorldLawLibrary.world_law_civ_limit_population_100.isEnabled() && getPopulationPeople() >= 100)
		{
			return true;
		}
		return false;
	}

	public void listBuilding(Building pBuilding)
	{
		buildings.Add(pBuilding);
		BuildingAsset asset = pBuilding.asset;
		if (asset.type == "type_stockpile")
		{
			stockpiles.Add(pBuilding);
		}
		if (asset.storage)
		{
			storages.Add(pBuilding);
		}
		if (asset.book_slots > 0)
		{
			_cached_buildings_with_book_slots.Add(pBuilding);
			if (pBuilding.data.books != null)
			{
				_cached_book_ids.AddRange(pBuilding.data.books.list_books);
			}
		}
		setBuildingDictType(pBuilding);
		setBuildingDictID(pBuilding);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CanBeNull]
	public WorldTile getTile(bool pForceRecalc = false)
	{
		if (_city_tile == null || pForceRecalc)
		{
			recalculateCityTile();
		}
		return _city_tile;
	}

	internal void recalculateCityTile()
	{
		_city_tile = null;
		Building tMainBuilding = getBuildingOfType("type_bonfire");
		if (tMainBuilding != null)
		{
			_city_tile = tMainBuilding.current_tile;
			return;
		}
		foreach (Building tBuilding in buildings.LoopRandom())
		{
			if (!tBuilding.asset.docks && !tBuilding.current_tile.Type.ocean)
			{
				if (tMainBuilding == null)
				{
					tMainBuilding = tBuilding;
				}
				else if (tBuilding.asset.priority > tMainBuilding.asset.priority)
				{
					tMainBuilding = tBuilding;
				}
			}
		}
		if (tMainBuilding != null)
		{
			_city_tile = tMainBuilding.current_tile;
			return;
		}
		List<TileZone> tZones = zones;
		if (tZones.Count == 0)
		{
			return;
		}
		for (int i = 0; i < tZones.Count; i++)
		{
			TileZone tZone = tZones[i];
			if (!tZone.centerTile.Type.ocean)
			{
				_city_tile = tZone.centerTile;
				break;
			}
		}
	}

	internal int countInHouses()
	{
		int ii = 0;
		List<Actor> tUnits = base.units;
		for (int i = 0; i < tUnits.Count; i++)
		{
			if (tUnits[i].is_inside_building)
			{
				ii++;
			}
		}
		return ii;
	}

	public int countBookSlots()
	{
		int tSlotsTotal = 0;
		for (int i = 0; i < _cached_buildings_with_book_slots.Count; i++)
		{
			Building tBuilding = _cached_buildings_with_book_slots[i];
			tSlotsTotal += tBuilding.asset.book_slots;
		}
		return tSlotsTotal;
	}

	public bool hasBookSlots()
	{
		int tSlots = countBookSlots();
		if (countBooks() >= tSlots)
		{
			return false;
		}
		return true;
	}

	public Building getBuildingWithBookSlot()
	{
		foreach (Building tBuilding in _cached_buildings_with_book_slots)
		{
			if (tBuilding.hasFreeBookSlot())
			{
				return tBuilding;
			}
		}
		return null;
	}

	public int countBooks()
	{
		return _cached_book_ids.Count;
	}

	private void setKingdomTimestamp()
	{
		data.timestamp_kingdom = World.world.getCurWorldTime();
	}

	public override ColorAsset getColor()
	{
		return kingdom.getColor();
	}

	internal void setKingdom(Kingdom pKingdom, bool pFromLoad = false)
	{
		World.world.kingdoms.setDirtyCities();
		if (isCapitalCity())
		{
			kingdom.clearCapital();
		}
		kingdom = pKingdom;
		if (kingdom != null && kingdom != WildKingdomsManager.neutral)
		{
			data.last_kingdom_id = kingdom.id;
		}
		if (!pFromLoad)
		{
			checkArmyExistence();
			if (hasArmy())
			{
				army.checkCity();
			}
		}
	}

	internal void newForceKingdomEvent(List<Actor> pUnits, List<Actor> pBoats, Kingdom pKingdom, string pHappinessEvent)
	{
		setKingdomTimestamp();
		forceUnitsIntoThisKingdom(pUnits, pKingdom, pBoats: false, pHappinessEvent);
		forceUnitsIntoThisKingdom(pBoats, pKingdom, pBoats: true);
	}

	internal void forceBuildingsToKingdom(List<Building> pBuildings, Kingdom pKingdom)
	{
		for (int i = 0; i < pBuildings.Count; i++)
		{
			pBuildings[i].setKingdom(pKingdom);
		}
	}

	internal void forceUnitsIntoThisKingdom(List<Actor> pActors, Kingdom pKingdom, bool pBoats, string pHappinessEvent = null)
	{
		if (pBoats)
		{
			for (int i = 0; i < pActors.Count; i++)
			{
				Actor tActor = pActors[i];
				if (!tActor.isRekt())
				{
					tActor.joinKingdom(pKingdom);
				}
			}
			return;
		}
		for (int j = 0; j < pActors.Count; j++)
		{
			Actor tActor2 = pActors[j];
			if (tActor2.isRekt())
			{
				continue;
			}
			if (tActor2.isKing())
			{
				if (tActor2.city != this || tActor2.kingdom == pKingdom)
				{
					continue;
				}
				tActor2.kingdom.kingLeftEvent();
			}
			tActor2.joinKingdom(pKingdom);
			if (pHappinessEvent != null)
			{
				tActor2.changeHappiness(pHappinessEvent);
			}
		}
	}

	internal Building getStorageNear(WorldTile pTile, bool pOnlyFood = false)
	{
		Building tBest = null;
		int tDistBest = int.MaxValue;
		List<Building> tBuildings = storages;
		for (int i = 0; i < tBuildings.Count; i++)
		{
			Building tBuilding = tBuildings[i];
			if (!tBuilding.isUsable() || !tBuilding.current_tile.isSameIsland(pTile))
			{
				continue;
			}
			if (pOnlyFood && tBuilding.asset.storage_only_food)
			{
				tBest = tBuilding;
				continue;
			}
			int tDist = Toolbox.SquaredDistVec2(tBuilding.current_tile.pos, pTile.pos);
			if (tDist < tDistBest)
			{
				tDistBest = tDist;
				tBest = tBuilding;
			}
		}
		return tBest;
	}

	internal Building getStorageWithFoodNear(WorldTile pTile)
	{
		Building tBest = null;
		int tDistBest = int.MaxValue;
		List<Building> tBuildings = storages;
		for (int i = 0; i < tBuildings.Count; i++)
		{
			Building tBuilding = tBuildings[i];
			if (tBuilding.isUsable() && tBuilding.current_tile.isSameIsland(pTile) && tBuilding.countFood() != 0)
			{
				int tDist = Toolbox.SquaredDistVec2(tBuilding.current_tile.pos, pTile.pos);
				if (tDist < tDistBest)
				{
					tDistBest = tDist;
					tBest = tBuilding;
				}
			}
		}
		return tBest;
	}

	internal bool hasStorageBuilding()
	{
		List<Building> tBuildings = storages;
		for (int i = 0; i < tBuildings.Count; i++)
		{
			if (!tBuildings[i].isUnderConstruction())
			{
				return true;
			}
		}
		return false;
	}

	public WorldTile getRoadTileToBuild(Actor pBuilder)
	{
		tiles_to_remove.Clear();
		for (int i = 0; i < road_tiles_to_build.Count; i++)
		{
			WorldTile tTile = road_tiles_to_build[i];
			if (tTile.Type.road)
			{
				tiles_to_remove.Add(tTile);
			}
		}
		for (int j = 0; j < tiles_to_remove.Count; j++)
		{
			WorldTile tTile2 = tiles_to_remove[j];
			road_tiles_to_build.Remove(tTile2);
		}
		tiles_to_remove.Clear();
		if (road_tiles_to_build.Count > 0)
		{
			return road_tiles_to_build[0];
		}
		return null;
	}

	internal void init()
	{
		createAI();
		setStatusDirty();
	}

	private void createAI()
	{
		if (Globals.AI_TEST_ACTIVE)
		{
			if (ai == null)
			{
				ai = new AiSystemCity(this);
			}
			ai.next_job_delegate = getNextJob;
			ai.jobs_library = AssetManager.job_city;
			ai.task_library = AssetManager.tasks_city;
			ai.addSingleTask("build");
			ai.addSingleTask("check_loyalty");
			ai.addSingleTask("check_destruction");
		}
	}

	protected sealed override void setDefaultValues()
	{
		base.setDefaultValues();
		mark_scale_effect = 1f;
		timer_build_boat = 10f;
		timer_build = 0f;
		timer_action = 0f;
		_timer_capture = 0f;
		_timer_warrior = 0f;
		_capture_ticks = 0f;
		last_visual_capture_ticks = 0;
		_dirty_citizens = true;
		_dirty_city_status = false;
		_dirty_abandoned_zones = false;
		_current_total_food = 0;
		_last_checked_job_id = 0;
		_loyalty_last_time = -1.0;
		_loyalty_cached = -1;
	}

	private string getNextJob()
	{
		return "city";
	}

	public bool isValidTargetForWar()
	{
		if (!hasZones())
		{
			return false;
		}
		return true;
	}

	public bool hasZones()
	{
		return zones.Count > 0;
	}

	public bool needSettlers()
	{
		int tCurrentPop = getPopulationPeople();
		if (getAge() < 5)
		{
			return true;
		}
		if (tCurrentPop >= 22)
		{
			return false;
		}
		if (tCurrentPop < 22 && status.housing_free == 0 && getAge() > 10 && getHouseCurrent() > 2)
		{
			return false;
		}
		return true;
	}

	internal void generateName(Actor pActor)
	{
		string tName = pActor.generateName(MetaType.City, getID());
		setName(tName);
		data.name_culture_id = culture?.id ?? (-1);
	}

	public void loadLeader()
	{
		if (data.leaderID.hasValue())
		{
			Actor tActor = World.world.units.get(data.leaderID);
			setLeader(tActor, pNew: false);
		}
	}

	public void newCityEvent(Actor pActor)
	{
		recalculateCityTile();
		generateName(pActor);
	}

	private void loadCityZones(List<ZoneData> pZoneData)
	{
		if (pZoneData == null)
		{
			return;
		}
		for (int j = 0; j < pZoneData.Count; j++)
		{
			ZoneData tZoneData = pZoneData[j];
			TileZone tZone = World.world.zone_calculator.getZone(tZoneData.x, tZoneData.y);
			if (tZone != null)
			{
				addZone(tZone);
			}
		}
	}

	public void loadCity(CityData pData)
	{
		loadCityZones(pData.zones);
		setData(pData);
		if (data.id_culture.hasValue())
		{
			setCulture(World.world.cultures.get(data.id_culture));
		}
		if (data.id_language.hasValue())
		{
			setLanguage(World.world.languages.get(data.id_language));
		}
		if (data.id_religion.hasValue())
		{
			setReligion(World.world.religions.get(data.id_religion));
		}
		if (data.equipment == null)
		{
			data.equipment = new CityEquipment();
		}
		else
		{
			data.equipment.loadFromSave(this);
		}
		Kingdom tKingdom = ((!pData.kingdomID.hasValue() || pData.kingdomID == 0L) ? WildKingdomsManager.neutral : World.world.kingdoms.get(pData.kingdomID));
		setKingdom(tKingdom, pFromLoad: true);
	}

	public void forceDoChecks()
	{
		updateTotalFood();
		updateCitizens();
		updateCityStatus();
	}

	public void executeAllActionsForCity()
	{
		AssetManager.tasks_city.get("do_initial_load_check").executeAllActionsForCity(this);
	}

	public void eventUnitAdded(Actor pActor)
	{
		if (!pActor.asset.is_boat)
		{
			setCitizensDirty();
		}
		setStatusDirty();
	}

	public void eventUnitRemoved(Actor pActor)
	{
		setStatusDirty();
		setCitizensDirty();
		if (pActor.isCityLeader())
		{
			removeLeader();
		}
	}

	public void setAbandonedZonesDirty()
	{
		_dirty_abandoned_zones = true;
	}

	public void setCitizensDirty()
	{
		_dirty_citizens = true;
	}

	public void setStatusDirty()
	{
		_dirty_city_status = true;
	}

	private void sortZonesByDistanceToCenter()
	{
		WorldTile tCenter = getTile();
		if (tCenter != null)
		{
			Vector2Int tCenterPos = tCenter.pos;
			zones.Sort(delegate(TileZone a, TileZone b)
			{
				int num = Toolbox.SquaredDistVec2(a.centerTile.pos, tCenterPos);
				int value = Toolbox.SquaredDistVec2(b.centerTile.pos, tCenterPos);
				return num.CompareTo(value);
			});
		}
	}

	private void updateCityStatus()
	{
		_dirty_city_status = false;
		status.clear();
		recalculateCityTile();
		sortZonesByDistanceToCenter();
		recalculateNeighbourZones();
		recalculateNeighbourCities();
		List<Building> tBuildings = buildings;
		int tPopBabies = countPopulationChildren();
		status.population = getPopulationPeople();
		status.population_adults = status.population - tPopBabies;
		status.population_children = tPopBabies;
		MetaObject<CityData>._family_counter.Clear();
		List<Actor> tUnits = base.units;
		for (int i = 0; i < tUnits.Count; i++)
		{
			Actor tActor = tUnits[i];
			if (tActor.isHungry())
			{
				status.hungry++;
			}
			if (tActor.isSexMale())
			{
				status.males++;
			}
			else
			{
				status.females++;
			}
			if (tActor.hasFamily())
			{
				MetaObject<CityData>._family_counter.Add(tActor.family);
			}
			if (tActor.isSick())
			{
				status.sick++;
			}
			if (tActor.hasHouse())
			{
				status.housed++;
			}
			else
			{
				status.homeless++;
			}
		}
		status.families = MetaObject<CityData>._family_counter.Count;
		MetaObject<CityData>._family_counter.Clear();
		for (int j = 0; j < tBuildings.Count; j++)
		{
			Building tBuilding = tBuildings[j];
			if (!tBuilding.isUnderConstruction() && tBuilding.asset.hasHousingSlots())
			{
				status.housing_total += tBuilding.asset.housing_slots;
			}
		}
		if (status.population > status.housing_total)
		{
			status.housing_occupied = status.housing_total;
		}
		else
		{
			status.housing_occupied = status.population;
		}
		status.housing_free = status.housing_total - status.housing_occupied;
		status.maximum_items = 15;
		recalculateMaxHouses();
		status.warrior_slots = jobs.countCurrentJobs(CitizenJobLibrary.attacker);
		status.warriors_current = countProfession(UnitProfession.Warrior);
		CityBehCheckFarms.check(this);
	}

	private void recalculateMaxHouses()
	{
		if (DebugConfig.isOn(DebugOption.CityUnlimitedHouses))
		{
			status.houses_max = 9999;
			return;
		}
		float tHouseMax = zones.Count;
		if (hasCulture())
		{
			if (culture.hasTrait("dense_dwellings"))
			{
				tHouseMax = zones.Count * 2;
			}
			if (culture.hasTrait("solitude_seekers"))
			{
				tHouseMax = (float)zones.Count / 3f;
			}
			if (culture.hasTrait("hive_society"))
			{
				tHouseMax = (float)zones.Count * 3f;
			}
		}
		foreach (Building tBuilding in buildings)
		{
			tHouseMax += (float)tBuilding.asset.max_houses;
		}
		status.houses_max = (int)tHouseMax;
	}

	public bool hasBooksToRead(Actor pActor)
	{
		if (pActor.hasTag("can_read_any_book"))
		{
			return countBooks() > 0;
		}
		if (!pActor.hasLanguage())
		{
			return false;
		}
		if (!hasBooksOfLanguage(pActor.language))
		{
			return false;
		}
		return true;
	}

	public bool hasBooksOfLanguage(Language pLanguage)
	{
		int i = 0;
		for (int tLength = countBooks(); i < tLength; i++)
		{
			long tBookID = _cached_book_ids[i];
			Book tBook = World.world.books.get(tBookID);
			if (!tBook.isRekt() && tBook.isReadyToBeRead())
			{
				Language tBookLanguage = tBook.getLanguage();
				if (tBookLanguage.id == pLanguage.id || tBookLanguage.hasTrait("magic_words"))
				{
					return true;
				}
			}
		}
		return false;
	}

	public Book getRandomBookOfLanguage(Language pLanguage)
	{
		using ListPool<Book> tBooks = new ListPool<Book>();
		int i = 0;
		for (int tLength = countBooks(); i < tLength; i++)
		{
			long tBookID = _cached_book_ids[i];
			Book tBook = World.world.books.get(tBookID);
			if (!tBook.isRekt() && tBook.isReadyToBeRead())
			{
				Language tBookLanguage = tBook.getLanguage();
				if (tBookLanguage.id == pLanguage.id || tBookLanguage.hasTrait("magic_words"))
				{
					tBooks.Add(tBook);
				}
			}
		}
		if (tBooks.Count == 0)
		{
			return null;
		}
		return tBooks.GetRandom();
	}

	public Book getRandomBook()
	{
		using ListPool<Book> tBooks = new ListPool<Book>();
		int i = 0;
		for (int tLength = countBooks(); i < tLength; i++)
		{
			long tBookID = _cached_book_ids[i];
			Book tBook = World.world.books.get(tBookID);
			if (!tBook.isRekt() && tBook.isReadyToBeRead())
			{
				tBooks.Add(tBook);
			}
		}
		if (tBooks.Count == 0)
		{
			return null;
		}
		return tBooks.GetRandom();
	}

	public List<long> getBooks()
	{
		return _cached_book_ids;
	}

	public int getHouseCurrent()
	{
		return countBuildingsType("type_house", pCountOnlyFinished: false);
	}

	public int getHouseLimit()
	{
		return status.houses_max;
	}

	public bool isConnectedToCapital()
	{
		if (!kingdom.hasCapital())
		{
			return false;
		}
		recalculateNeighbourCities();
		if (neighbours_cities_kingdom.Contains(this))
		{
			return true;
		}
		kingdom.calculateNeighbourCities();
		_connected_checked.Clear();
		_connected_next_wave.Clear();
		_connected_current_wave.Clear();
		_connected_next_wave.UnionWith(kingdom.capital.neighbours_cities_kingdom);
		int iii = 0;
		while (_connected_next_wave.Count > 0)
		{
			_connected_current_wave.UnionWith(_connected_next_wave);
			_connected_next_wave.Clear();
			iii++;
			foreach (City tCity in _connected_current_wave)
			{
				if (tCity == this)
				{
					return true;
				}
				_connected_checked.Add(tCity);
				foreach (City iCity in tCity.neighbours_cities_kingdom)
				{
					if (!_connected_checked.Contains(iCity))
					{
						_connected_next_wave.Add(iCity);
					}
				}
			}
			if (iii > 30)
			{
				break;
			}
		}
		return false;
	}

	public void recalculateNeighbourCities()
	{
		neighbours_cities.Clear();
		neighbours_cities_kingdom.Clear();
		neighbours_kingdoms.Clear();
		foreach (TileZone neighbour_zone in neighbour_zones)
		{
			City tZoneCity = neighbour_zone.city;
			if (tZoneCity != this && tZoneCity != null)
			{
				neighbours_cities.Add(tZoneCity);
				if (tZoneCity.kingdom == kingdom)
				{
					neighbours_cities_kingdom.Add(tZoneCity);
				}
				else
				{
					neighbours_kingdoms.Add(tZoneCity.kingdom);
				}
			}
		}
	}

	public void recalculateNeighbourZones()
	{
		border_zones.Clear();
		neighbour_zones.Clear();
		List<TileZone> tZones = zones;
		for (int i = 0; i < tZones.Count; i++)
		{
			TileZone tParentZone = tZones[i];
			TileZone[] tNeighbourZones = tParentZone.neighbours_all;
			foreach (TileZone tNeighbourZone in tNeighbourZones)
			{
				if (tNeighbourZone.city != this)
				{
					border_zones.Add(tParentZone);
					neighbour_zones.Add(tNeighbourZone);
				}
			}
		}
	}

	internal void setCulture(Culture pCulture)
	{
		if (culture != pCulture)
		{
			culture = pCulture;
			World.world.cultures.setDirtyCities();
		}
	}

	public Culture getCulture()
	{
		return culture;
	}

	public Language getLanguage()
	{
		return language;
	}

	public Religion getReligion()
	{
		return religion;
	}

	public void checkAbandon()
	{
		if (_dirty_abandoned_zones)
		{
			_dirty_abandoned_zones = false;
			World.world.city_zone_helper.city_abandon.check(this);
		}
	}

	public void update(float pElapsed)
	{
		if (timer_build > 0f)
		{
			timer_build -= pElapsed;
		}
		updateTotalFood();
		if (data.timer_supply > 0f)
		{
			data.timer_supply -= pElapsed;
		}
		if (data.timer_trade > 0f)
		{
			data.timer_trade -= pElapsed;
		}
		if (_timer_warrior > 0f)
		{
			_timer_warrior -= pElapsed;
		}
		if (isDirtyUnits())
		{
			return;
		}
		if (!kingdom.wild && !hasUnits())
		{
			turnCityToNeutral();
			return;
		}
		if (_dirty_city_status)
		{
			updateCityStatus();
		}
		if (_dirty_citizens)
		{
			updateCitizens();
		}
		if (World.world.isPaused())
		{
			return;
		}
		if (timer_build_boat > 0f)
		{
			timer_build_boat -= pElapsed;
		}
		if (ai != null)
		{
			if (timer_action > 0f)
			{
				timer_action -= pElapsed;
			}
			else
			{
				ai.update();
			}
			ai.updateSingleTasks(pElapsed);
		}
		updateCapture(pElapsed);
	}

	private void turnCityToNeutral()
	{
		makeBoatsAbandonCity();
		setKingdom(WildKingdomsManager.neutral);
		forceBuildingsToKingdom(buildings, WildKingdomsManager.neutral);
	}

	private void makeBoatsAbandonCity()
	{
		if (countBoats() == 0)
		{
			return;
		}
		foreach (Actor tBoat in _boats)
		{
			if (!tBoat.isRekt())
			{
				tBoat.setCity(null);
			}
		}
	}

	private void updateTotalFood()
	{
		_current_total_food = countFoodTotal();
	}

	private void updateCapture(float pElapsed)
	{
		if (last_visual_capture_ticks == 0 && !isGettingCaptured())
		{
			return;
		}
		if ((int)_capture_ticks != last_visual_capture_ticks)
		{
			if ((int)_capture_ticks > last_visual_capture_ticks)
			{
				last_visual_capture_ticks++;
			}
			else
			{
				last_visual_capture_ticks--;
			}
		}
		last_visual_capture_ticks = Mathf.Clamp(last_visual_capture_ticks, 0, 100);
		if (_timer_capture > 0f)
		{
			_timer_capture -= pElapsed;
			return;
		}
		_timer_capture = 0.1f;
		int tTowers = countBuildingsType("type_watch_tower");
		if (tTowers > 0)
		{
			addCapturePoints(kingdom, 10 * tTowers);
		}
		Kingdom tDominating = null;
		foreach (Kingdom iKingdom in _capturing_units.Keys)
		{
			if (tDominating == null)
			{
				tDominating = iKingdom;
			}
			else if (_capturing_units[iKingdom] > _capturing_units[tDominating])
			{
				tDominating = iKingdom;
			}
		}
		if (tDominating == null)
		{
			_capture_ticks -= 0.5f;
			if (_capture_ticks <= 0f)
			{
				clearCapture();
			}
			return;
		}
		bool haveDefenders = false;
		if (_capturing_units.ContainsKey(kingdom) && _capturing_units[kingdom] > 0 && countWarriors() > 0)
		{
			haveDefenders = true;
		}
		if (being_captured_by != null && !being_captured_by.isAlive())
		{
			being_captured_by = null;
		}
		bool tCaptureGoDown = false;
		if (kingdom == tDominating)
		{
			tCaptureGoDown = true;
		}
		if (haveDefenders && _capturing_units.Count == 1)
		{
			tCaptureGoDown = true;
		}
		if (tCaptureGoDown)
		{
			_capture_ticks -= 1f;
			if (_capture_ticks <= 0f)
			{
				clearCapture();
			}
		}
		else
		{
			if (!tDominating.isEnemy(kingdom) || (haveDefenders && !(_capture_ticks < 5f)))
			{
				return;
			}
			if (being_captured_by == null || being_captured_by == tDominating)
			{
				_capture_ticks += 1f + 1f * pElapsed;
				being_captured_by = tDominating;
				if (_capture_ticks >= 100f)
				{
					finishCapture(tDominating);
				}
			}
			else if (tDominating.isEnemy(being_captured_by))
			{
				_capture_ticks -= 0.5f;
				if (_capture_ticks <= 0f)
				{
					clearCapture();
				}
			}
			else
			{
				_capture_ticks += 1f + 1f * pElapsed;
				if (_capture_ticks >= 100f)
				{
					finishCapture(being_captured_by);
				}
			}
		}
	}

	public bool isGettingCaptured()
	{
		if (_capturing_units.Count == 0)
		{
			return false;
		}
		if (_capturing_units.Count == 1 && _capturing_units.ContainsKey(kingdom))
		{
			return false;
		}
		return true;
	}

	public bool isGettingCapturedBy(Kingdom pKingdom)
	{
		if (_capturing_units.TryGetValue(pKingdom, out var tCount) && tCount > 0)
		{
			return true;
		}
		return false;
	}

	public Kingdom getCapturingKingdom()
	{
		return being_captured_by;
	}

	private void clearCapture()
	{
		_capture_ticks = 0f;
		being_captured_by = null;
	}

	public float getCaptureTicks()
	{
		return _capture_ticks;
	}

	private void prepareProfessionDicts()
	{
		if (_professions_dict.Count == 0)
		{
			for (int i = 0; i < ProfessionLibrary.list_enum_profession_ids.Length; i++)
			{
				UnitProfession tPro = ProfessionLibrary.list_enum_profession_ids[i];
				_professions_dict.Add(tPro, new List<Actor>());
			}
		}
	}

	private void updateCitizens()
	{
		_dirty_citizens = false;
		prepareProfessionDicts();
		foreach (List<Actor> value in _professions_dict.Values)
		{
			value.Clear();
		}
		List<Actor> tUnits = base.units;
		for (int i = 0; i < tUnits.Count; i++)
		{
			Actor tActor = tUnits[i];
			if (tActor != null && tActor.isAlive())
			{
				_professions_dict[tActor.getProfession()].Add(tActor);
			}
		}
	}

	public bool canGrowZones()
	{
		if (!DebugConfig.isOn(DebugOption.SystemZoneGrowth))
		{
			return false;
		}
		if (_dirty_abandoned_zones)
		{
			return false;
		}
		if (getPopulationPeople() == 0)
		{
			return false;
		}
		return true;
	}

	internal int countProfession(UnitProfession pType)
	{
		if (_professions_dict.TryGetValue(pType, out var tList))
		{
			return tList.Count;
		}
		return 0;
	}

	public void destroyCity()
	{
		removeLeader();
		disbandArmy();
		foreach (TileZone zone in zones)
		{
			zone.setCity(null);
		}
		foreach (Actor tActor in World.world.units)
		{
			if (tActor.city == this)
			{
				tActor.setCity(null);
			}
		}
		data.equipment.clearItems();
		base.units.Clear();
		_boats.Clear();
		zones.Clear();
		if (hasKingdom())
		{
			removeFromCurrentKingdom();
		}
	}

	public override void Dispose()
	{
		DBInserter.deleteData(getID(), "city");
		_connected_checked.Clear();
		_connected_next_wave.Clear();
		_connected_current_wave.Clear();
		stockpiles.Clear();
		storages.Clear();
		_cached_book_ids.Clear();
		_cached_buildings_with_book_slots.Clear();
		base.units.Clear();
		_boats.Clear();
		buildings.Clear();
		buildings_dict_id.Clear();
		buildings_dict_type.Clear();
		zones.Clear();
		road_tiles_to_build.Clear();
		calculated_place_for_farms.Clear();
		calculated_farm_fields.Clear();
		calculated_crops.Clear();
		calculated_grown_wheat.Clear();
		_professions_dict.Clear();
		neighbour_zones.Clear();
		border_zones.Clear();
		neighbours_cities.Clear();
		neighbours_cities_kingdom.Clear();
		neighbours_kingdoms.Clear();
		tiles_to_remove.Clear();
		danger_zones.Clear();
		_capturing_units.Clear();
		_city_tile = null;
		target_attack_zone = null;
		target_attack_city = null;
		army = null;
		tasks.clear();
		jobs.clear();
		status.clear();
		under_construction_building = null;
		culture = null;
		language = null;
		religion = null;
		kingdom = null;
		leader = null;
		being_captured_by = null;
		_debug_last_possible_build_orders = null;
		_debug_last_possible_build_orders_no_resources = null;
		_debug_last_build_order_try = null;
		timestamp_shrink = 0.0;
		ai.reset();
		base.Dispose();
	}

	public bool hasAttackZoneOrder()
	{
		return target_attack_zone != null;
	}

	internal void spendResourcesForBuildingAsset(ConstructionCost pCost)
	{
		takeResource("wood", pCost.wood);
		takeResource("gold", pCost.gold);
		takeResource("stone", pCost.stone);
		takeResource("common_metals", pCost.common_metals);
	}

	internal bool hasEnoughResourcesFor(ConstructionCost pCost)
	{
		if (DebugConfig.isOn(DebugOption.CityInfiniteResources))
		{
			return true;
		}
		if (amount_wood < pCost.wood)
		{
			return false;
		}
		if (amount_common_metals < pCost.common_metals)
		{
			return false;
		}
		if (amount_stone < pCost.stone)
		{
			return false;
		}
		if (amount_gold < pCost.gold)
		{
			return false;
		}
		return true;
	}

	internal Building getBuildingToBuild()
	{
		if (under_construction_building != null && (!under_construction_building.isAlive() || !under_construction_building.isUnderConstruction()))
		{
			under_construction_building = null;
		}
		return under_construction_building;
	}

	internal bool hasBuildingToBuild()
	{
		if (under_construction_building != null)
		{
			if (!under_construction_building.isAlive() || !under_construction_building.isUnderConstruction())
			{
				under_construction_building = null;
				return false;
			}
			return true;
		}
		return false;
	}

	internal void setBuildingDictType(Building pBuilding)
	{
		List<Building> tList = getBuildingListOfType(pBuilding.asset.type);
		if (tList == null)
		{
			tList = new List<Building>();
			buildings_dict_type.Add(pBuilding.asset.type, tList);
		}
		tList.Add(pBuilding);
	}

	internal List<Building> getBuildingListOfID(string pBuildingID)
	{
		buildings_dict_id.TryGetValue(pBuildingID, out var tList);
		return tList;
	}

	public int countZones()
	{
		return zones.Count;
	}

	public int countBuildings()
	{
		return buildings.Count;
	}

	public int countBuildingsOfID(string pBuildingID)
	{
		return getBuildingListOfID(pBuildingID)?.Count ?? 0;
	}

	internal void setBuildingDictID(Building pBuilding)
	{
		if (!buildings_dict_id.TryGetValue(pBuilding.asset.id, out var tList))
		{
			buildings_dict_id.Add(pBuilding.asset.id, tList = new List<Building>());
		}
		tList.Add(pBuilding);
	}

	public int countBuildingsType(string pBuildingTypeID, bool pCountOnlyFinished = true)
	{
		List<Building> tList = getBuildingListOfType(pBuildingTypeID);
		if (tList == null)
		{
			return 0;
		}
		if (pCountOnlyFinished)
		{
			int tCount = 0;
			{
				foreach (Building item in tList)
				{
					if (!item.isUnderConstruction())
					{
						tCount++;
					}
				}
				return tCount;
			}
		}
		return tList.Count;
	}

	internal bool hasBuildingType(string pBuildingTypeID, bool pCountOnlyFinished = true, TileIsland pLimitIsland = null)
	{
		List<Building> tList = getBuildingListOfType(pBuildingTypeID);
		if (tList == null)
		{
			return false;
		}
		if (tList.Count == 0)
		{
			return false;
		}
		bool tLimitIsland = pLimitIsland != null;
		foreach (Building tBuilding in tList)
		{
			if ((!pCountOnlyFinished || (!tBuilding.isUnderConstruction() && tBuilding.isUsable())) && (!tLimitIsland || tBuilding.current_island == pLimitIsland))
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal List<Building> getBuildingListOfType(string pType)
	{
		buildings_dict_type.TryGetValue(pType, out var tList);
		return tList;
	}

	internal Building getBuildingOfType(string pBuildingTypeID, bool pCountOnlyFinished = true, bool pRandom = false, bool pOnlyFreeTile = false, TileIsland pLimitIsland = null)
	{
		List<Building> tList = getBuildingListOfType(pBuildingTypeID);
		if (tList == null)
		{
			return null;
		}
		if (tList.Count == 0)
		{
			return null;
		}
		bool tLimitIsland = pLimitIsland != null;
		IEnumerable<Building> enumerable2;
		if (!pRandom)
		{
			IEnumerable<Building> enumerable = tList;
			enumerable2 = enumerable;
		}
		else
		{
			enumerable2 = tList.LoopRandom();
		}
		foreach (Building tBuilding in enumerable2)
		{
			if ((!pCountOnlyFinished || (!tBuilding.isUnderConstruction() && tBuilding.isUsable())) && (!pOnlyFreeTile || !tBuilding.current_tile.isTargeted()) && (!tLimitIsland || tBuilding.current_island == pLimitIsland))
			{
				return tBuilding;
			}
		}
		return null;
	}

	public void addRoads(List<WorldTile> pTiles)
	{
		for (int i = 0; i < pTiles.Count; i++)
		{
			WorldTile tTile = pTiles[i];
			if (!tTile.Type.road && !road_tiles_to_build.Contains(tTile))
			{
				road_tiles_to_build.Add(tTile);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool isArmyFull()
	{
		if (status.warriors_current >= status.warrior_slots)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool isArmyOverLimit()
	{
		if (status.warriors_current > status.warrior_slots)
		{
			return true;
		}
		return false;
	}

	private bool tryToMakeWarrior(Actor pActor)
	{
		if (!checkCanMakeWarrior(pActor))
		{
			return false;
		}
		makeWarrior(pActor);
		_timer_warrior = 15f;
		if (hasLeader())
		{
			float tDecrease = leader.stats["warfare"] / 2f;
			_timer_warrior -= tDecrease;
			if (_timer_warrior < 1f)
			{
				_timer_warrior = 1f;
			}
		}
		if (hasBuildingType("type_barracks"))
		{
			_timer_warrior /= 2f;
		}
		return true;
	}

	public bool checkCanMakeWarrior(Actor pActor)
	{
		if (isArmyFull())
		{
			return false;
		}
		if (pActor.isBaby())
		{
			return false;
		}
		if (hasCulture())
		{
			if (pActor.isSexFemale() && culture.hasTrait("conscription_male_only"))
			{
				return false;
			}
			if (pActor.isSexMale() && culture.hasTrait("conscription_female_only"))
			{
				return false;
			}
		}
		return true;
	}

	public void makeWarrior(Actor pActor)
	{
		pActor.setProfession(UnitProfession.Warrior);
		if (pActor.equipment.weapon.isEmpty())
		{
			giveItem(pActor, getEquipmentList(EquipmentType.Weapon), this);
		}
		status.warriors_current++;
	}

	public bool checkIfWarriorStillOk(Actor pActor)
	{
		bool tIsOk = true;
		if (isArmyOverLimit())
		{
			tIsOk = false;
		}
		else if (!hasEnoughFoodForArmy())
		{
			tIsOk = false;
		}
		if (!tIsOk)
		{
			pActor.stopBeingWarrior();
			_timer_warrior = 30f;
		}
		return tIsOk;
	}

	public void setCitizenJob(Actor pActor)
	{
		if ((!isGettingCaptured() && _timer_warrior <= 0f && pActor.isProfession(UnitProfession.Unit) && getResourcesAmount("gold") > 10 && hasEnoughFoodForArmy() && tryToMakeWarrior(pActor)) || checkCitizenJobList(AssetManager.citizen_job_library.list_priority_high, pActor) || (!hasAnyFood() && checkCitizenJobList(AssetManager.citizen_job_library.list_priority_high_food, pActor)))
		{
			return;
		}
		List<CitizenJobAsset> tJobList = AssetManager.citizen_job_library.list_priority_normal;
		for (int i = 0; i < tJobList.Count; i++)
		{
			_last_checked_job_id++;
			if (_last_checked_job_id > tJobList.Count - 1)
			{
				_last_checked_job_id = 0;
			}
			CitizenJobAsset tCitizenJobAsset = tJobList[_last_checked_job_id];
			if ((tCitizenJobAsset.ok_for_king || !pActor.isKing()) && (tCitizenJobAsset.ok_for_leader || !pActor.isCityLeader()) && checkCitizenJob(tCitizenJobAsset, this, pActor))
			{
				break;
			}
		}
	}

	private bool checkCitizenJobList(List<CitizenJobAsset> pList, Actor pActor)
	{
		for (int i = 0; i < pList.Count; i++)
		{
			CitizenJobAsset tAsset = pList[i];
			if (checkCitizenJob(tAsset, this, pActor))
			{
				return true;
			}
		}
		return false;
	}

	private bool checkCitizenJob(CitizenJobAsset pJobAsset, City pCity, Actor pActor)
	{
		if (pJobAsset.only_leaders && !pActor.isKing() && !pActor.isCityLeader())
		{
			return false;
		}
		if (pJobAsset.should_be_assigned != null && !pJobAsset.should_be_assigned(pActor))
		{
			return false;
		}
		if (jobs.hasJob(pJobAsset))
		{
			jobs.takeJob(pJobAsset);
			pActor.setCitizenJob(pJobAsset);
			return true;
		}
		return false;
	}

	public bool hasSuitableFood(Subspecies pSubspecies)
	{
		HashSet<string> tAllowedFood = pSubspecies.getAllowedFoodByDiet();
		foreach (Building tStorage in storages)
		{
			if (!tStorage.isUsable())
			{
				continue;
			}
			foreach (string tAllowedFoodID in tAllowedFood)
			{
				if (tStorage.getResourcesAmount(tAllowedFoodID) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	internal ResourceAsset getFoodItem(Subspecies pSubspecies, string pFavoriteFood = null)
	{
		if (!string.IsNullOrEmpty(pFavoriteFood) && getResourcesAmount(pFavoriteFood) > 0)
		{
			return AssetManager.resources.get(pFavoriteFood);
		}
		return getRandomSuitableFood(pSubspecies);
	}

	internal void eatFoodItem(string pItem)
	{
		if (pItem != null)
		{
			takeResource(pItem, 1);
			data.total_food_consumed++;
		}
	}

	internal void removeZone(TileZone pZone)
	{
		setAbandonedZonesDirty();
		if (zones.Remove(pZone))
		{
			pZone.setCity(null);
			World.world.city_zone_helper.city_place_finder.setDirty();
		}
		updateCityCenter();
		setStatusDirty();
	}

	internal void addZone(TileZone pZone)
	{
		if (!zones.Contains(pZone))
		{
			if (pZone.city != null)
			{
				pZone.city.removeZone(pZone);
			}
			zones.Add(pZone);
			pZone.setCity(this);
			updateCityCenter();
			if (World.world.city_zone_helper.city_place_finder.hasPossibleZones())
			{
				World.world.city_zone_helper.city_place_finder.setDirty();
			}
			setStatusDirty();
		}
	}

	public int getLoyalty(bool pForceRecalc = false)
	{
		if (kingdom.isNeutral())
		{
			_loyalty_cached = 0;
		}
		else if (World.world.getWorldTimeElapsedSince(_loyalty_last_time) > 3f || pForceRecalc)
		{
			_loyalty_cached = LoyaltyCalculator.calculate(this);
			_loyalty_last_time = World.world.getCurWorldTime();
		}
		return _loyalty_cached;
	}

	public int getCachedLoyalty()
	{
		return _loyalty_cached;
	}

	public bool isCapitalCity()
	{
		if (kingdom == null)
		{
			return false;
		}
		return this == kingdom.capital;
	}

	internal void updateAge()
	{
		if (hasLeader() && leader.hasClan())
		{
			leader.addRenown(1);
		}
	}

	private void updateCityCenter()
	{
		if (!hasZones())
		{
			city_center = Globals.POINT_IN_VOID_2;
			return;
		}
		float x = 0f;
		float y = 0f;
		float tBestDistance = float.MaxValue;
		TileZone tBestCenterZone = null;
		for (int i = 0; i < zones.Count; i++)
		{
			TileZone tZone = zones[i];
			x += tZone.centerTile.posV3.x;
			y += tZone.centerTile.posV3.y;
		}
		city_center.x = x / (float)zones.Count;
		city_center.y = y / (float)zones.Count;
		for (int j = 0; j < zones.Count; j++)
		{
			TileZone tZone2 = zones[j];
			float tDistance = Toolbox.SquaredDist(tZone2.centerTile.x, tZone2.centerTile.y, city_center.x, city_center.y);
			if (tDistance < tBestDistance)
			{
				tBestCenterZone = tZone2;
				tBestDistance = tDistance;
			}
		}
		city_center.x = tBestCenterZone.centerTile.posV3.x;
		city_center.y = tBestCenterZone.centerTile.posV3.y + 2f;
		last_city_center = city_center;
	}

	internal void removeFromCurrentKingdom()
	{
		kingdom.checkClearCapital(this);
	}

	internal void switchedKingdom()
	{
		List<Building> tBuildings = buildings;
		for (int i = 0; i < tBuildings.Count; i++)
		{
			Building tBuilding = tBuildings[i];
			if (!tBuilding.isRemoved())
			{
				tBuilding.setKingdomCiv(kingdom);
			}
		}
		World.world.zone_calculator.setDrawnZonesDirty();
	}

	internal void useInspire(Actor pActor)
	{
		Kingdom tOldKingdom = kingdom;
		makeOwnKingdom(pActor, pRebellion: true);
		World.world.diplomacy.startWar(tOldKingdom, kingdom, WarTypeLibrary.inspire, pLog: false);
	}

	internal void clearCurrentCaptureAmounts()
	{
		_capturing_units.Clear();
	}

	internal void clearDangerZones()
	{
		danger_zones.Clear();
	}

	public bool isInDanger()
	{
		if (danger_zones.Count > 0)
		{
			return true;
		}
		return false;
	}

	internal void updateConquest(Actor pActor)
	{
		if (pActor.isKingdomCiv() && (pActor.kingdom == kingdom || pActor.kingdom.isEnemy(kingdom)))
		{
			addCapturePoints(pActor, 1);
		}
	}

	public void addCapturePoints(BaseSimObject pObject, int pValue)
	{
		addCapturePoints(pObject.kingdom, pValue);
	}

	public void addCapturePoints(Kingdom pKingdom, int pValue)
	{
		_capturing_units.TryGetValue(pKingdom, out var tCurrentCount);
		_capturing_units[pKingdom] = tCurrentCount + pValue;
	}

	public void debugCaptureUnits(DebugTool pTool)
	{
		pTool.setText("capture units:", _capturing_units.Count, 0f, pShowBar: false, 0L);
		pTool.setText("isGettingCaptured()", isGettingCaptured(), 0f, pShowBar: false, 0L);
		foreach (Kingdom iKingdom in _capturing_units.Keys)
		{
			pTool.setText("-" + iKingdom.name, _capturing_units[iKingdom], 0f, pShowBar: false, 0L);
		}
	}

	internal void finishCapture(Kingdom pNewKingdom)
	{
		if (kingdom.hasKing() && kingdom.king.city == this)
		{
			kingdom.kingFledCity();
		}
		if (World.world.cities.isLocked())
		{
			return;
		}
		clearCapture();
		recalculateNeighbourCities();
		pNewKingdom.increaseHappinessFromNewCityCapture();
		kingdom.decreaseHappinessFromLostCityCapture(this);
		using ListPool<War> tListWars = new ListPool<War>(pNewKingdom.getWars());
		Kingdom tKingdomToJoin = findKingdomToJoinAfterCapture(pNewKingdom, tListWars);
		if (!checkRebelWar(tKingdomToJoin, tListWars))
		{
			tKingdomToJoin.data.timestamp_new_conquest = World.world.getCurWorldTime();
		}
		removeSoldiers();
		joinAnotherKingdom(tKingdomToJoin, pCaptured: true);
	}

	private Kingdom findKingdomToJoinAfterCapture(Kingdom pKingdom, ListPool<War> pWars)
	{
		Kingdom tResultKingdom = null;
		for (int i = 0; i < pWars.Count; i++)
		{
			War tWar = pWars[i];
			if (tWar.isTotalWar() || !tWar.hasKingdom(kingdom) || !tWar.isInWarWith(pKingdom, kingdom))
			{
				continue;
			}
			if (tWar.isMainAttacker(pKingdom) || tWar.isMainDefender(pKingdom))
			{
				break;
			}
			if (tWar.isAttacker(kingdom))
			{
				Kingdom tMainDefender = tWar.main_defender;
				if (!tMainDefender.isRekt())
				{
					tResultKingdom = ((!neighbours_kingdoms.Contains(tMainDefender)) ? ((!neighbours_kingdoms.Contains(pKingdom)) ? tMainDefender : pKingdom) : tMainDefender);
					break;
				}
			}
			if (tWar.isDefender(kingdom))
			{
				Kingdom tMainAttacker = tWar.main_attacker;
				if (!tMainAttacker.isRekt())
				{
					tResultKingdom = ((!neighbours_kingdoms.Contains(tMainAttacker)) ? ((!neighbours_kingdoms.Contains(pKingdom)) ? tMainAttacker : pKingdom) : tMainAttacker);
					break;
				}
			}
		}
		if (tResultKingdom == null)
		{
			tResultKingdom = pKingdom;
		}
		else if (tResultKingdom.getSpecies() != kingdom.getSpecies())
		{
			tResultKingdom = pKingdom;
		}
		return tResultKingdom;
	}

	private bool checkRebelWar(Kingdom pKingdomToJoin, ListPool<War> pWars)
	{
		foreach (ref War pWar in pWars)
		{
			War tWar = pWar;
			if (tWar.getAsset().rebellion && tWar.isMainAttacker(pKingdomToJoin) && tWar.isInWarWith(pKingdomToJoin, kingdom))
			{
				return true;
			}
		}
		return false;
	}

	private void removeSoldiers()
	{
		foreach (Actor item in _professions_dict[UnitProfession.Warrior])
		{
			item.setProfession(UnitProfession.Unit);
		}
		disbandArmy();
	}

	public void disbandArmy()
	{
		checkArmyExistence();
		if (hasArmy())
		{
			army.disband();
			checkArmyExistence();
		}
	}

	public void checkArmyExistence()
	{
		if (hasArmy() && (!army.isAlive() || army.countUnits() <= 0))
		{
			setArmy(null);
		}
	}

	public bool hasArmy()
	{
		return army != null;
	}

	public Army getArmy()
	{
		return army;
	}

	public void setArmy(Army pArmy)
	{
		if (army != null && army != pArmy)
		{
			army.clearCity();
		}
		army = pArmy;
	}

	public Actor getRandomWarrior()
	{
		return _professions_dict[UnitProfession.Warrior].GetRandom();
	}

	internal Kingdom makeOwnKingdom(Actor pActor, bool pRebellion = false, bool pFellApart = false)
	{
		string tHappinessEvent = null;
		if (pRebellion)
		{
			World.world.game_stats.data.citiesRebelled++;
			World.world.map_stats.citiesRebelled++;
			tHappinessEvent = "just_rebelled";
		}
		if (pFellApart)
		{
			tHappinessEvent = "kingdom_fell_apart";
		}
		Kingdom tPrevKingdom = kingdom;
		removeFromCurrentKingdom();
		removeLeader();
		Kingdom tNewKingdom = World.world.kingdoms.makeNewCivKingdom(pActor);
		setKingdom(tNewKingdom);
		newForceKingdomEvent(base.units, _boats, tNewKingdom, tHappinessEvent);
		switchedKingdom();
		tNewKingdom.copyMetasFromOtherKingdom(tPrevKingdom);
		tNewKingdom.setCityMetas(this);
		return tNewKingdom;
	}

	public override int getPopulationPeople()
	{
		return countUnits();
	}

	public int getPopulationMaximum()
	{
		if (WorldLawLibrary.world_law_civ_limit_population_100.isEnabled())
		{
			if (status.housing_total >= 100)
			{
				return 100;
			}
			return status.housing_total;
		}
		return status.housing_total;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int getUnitsTotal()
	{
		return countUnits() + countBoats();
	}

	public int countPopulationChildren()
	{
		int tCount = 0;
		foreach (Actor tActor in base.units)
		{
			if (tActor.isAlive() && tActor.isBaby())
			{
				tCount++;
			}
		}
		return tCount;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int countBoats()
	{
		return _boats.Count;
	}

	public void joinAnotherKingdom(Kingdom pNewSetKingdom, bool pCaptured = false, bool pRebellion = false)
	{
		string tHappinessEvent = null;
		if (pCaptured)
		{
			World.world.game_stats.data.citiesConquered++;
			World.world.map_stats.citiesConquered++;
			tHappinessEvent = "was_conquered";
		}
		if (pRebellion)
		{
			World.world.game_stats.data.citiesRebelled++;
			World.world.map_stats.citiesRebelled++;
			tHappinessEvent = "just_rebelled";
		}
		Kingdom tOldKingdom = kingdom;
		removeFromCurrentKingdom();
		setKingdom(pNewSetKingdom);
		newForceKingdomEvent(base.units, _boats, pNewSetKingdom, tHappinessEvent);
		switchedKingdom();
		pNewSetKingdom.capturedFrom(tOldKingdom);
	}

	public int countWeapons()
	{
		return getEquipmentList(EquipmentType.Weapon).Count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int countFoodTotal()
	{
		return countFood();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasEnoughFoodForArmy()
	{
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int getTotalFood()
	{
		return _current_total_food;
	}

	public bool hasAnyFood()
	{
		return _current_total_food > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int countWarriors()
	{
		return countProfession(UnitProfession.Warrior);
	}

	public bool hasAnyWarriors()
	{
		return countWarriors() > 0;
	}

	public bool isHappy()
	{
		if (getCachedLoyalty() >= 0)
		{
			return true;
		}
		return false;
	}

	public float getArmyMaxMultiplier()
	{
		float num = 0f + getActorAsset().civ_base_army_multiplier;
		float tFromleader = getArmyMaxLeaderMultiplier();
		return num + tFromleader;
	}

	public float getArmyMaxLeaderMultiplier()
	{
		float tMultiplier = 0f;
		if (hasLeader())
		{
			tMultiplier += leader.stats["army"];
			float tWarfareBonus = leader.stats["warfare"] * 2f / 100f;
			tMultiplier += tWarfareBonus;
		}
		return tMultiplier;
	}

	public int getMaxWarriors()
	{
		return status.warrior_slots;
	}

	public void removeLeader()
	{
		leader = null;
		data.leaderID = -1L;
		rulerLeft();
	}

	public void setLeader(Actor pActor, bool pNew)
	{
		if (pActor != null && kingdom.king != pActor)
		{
			leader = pActor;
			leader.setProfession(UnitProfession.Leader);
			CityData cityData = data;
			long leaderID = (data.last_leader_id = pActor.data.id);
			cityData.leaderID = leaderID;
			if (pNew)
			{
				data.total_leaders++;
				leader.changeHappiness("become_leader");
				addRuler(pActor);
			}
		}
	}

	public void updateRulers()
	{
		if (data.past_rulers == null || data.past_rulers.Count == 0)
		{
			return;
		}
		foreach (LeaderEntry tEntry in data.past_rulers)
		{
			Actor tRuler = World.world.units.get(tEntry.id);
			if (!tRuler.isRekt())
			{
				tEntry.name = tRuler.name;
			}
		}
	}

	public void addRuler(Actor pActor)
	{
		CityData cityData = data;
		if (cityData.past_rulers == null)
		{
			cityData.past_rulers = new List<LeaderEntry>();
		}
		rulerLeft();
		data.past_rulers.Add(new LeaderEntry
		{
			id = pActor.getID(),
			name = pActor.name,
			color_id = (pActor.kingdom?.data.color_id ?? (-1)),
			timestamp_ago = World.world.getCurWorldTime()
		});
		if (data.past_rulers.Count > 30)
		{
			data.past_rulers.Shift();
		}
	}

	public void rulerLeft()
	{
		if (data.past_rulers != null && data.past_rulers.Count != 0)
		{
			LeaderEntry tLast = data.past_rulers.Last();
			if (!(tLast.timestamp_end >= tLast.timestamp_ago))
			{
				tLast.timestamp_end = World.world.getCurWorldTime();
				updateRulers();
			}
		}
	}

	public static bool nearbyBorders(City pA, City pB)
	{
		City tSmallest;
		City tCheck;
		if (pA.zones.Count > pB.zones.Count)
		{
			tSmallest = pB;
			tCheck = pA;
		}
		else
		{
			tSmallest = pA;
			tCheck = pB;
		}
		for (int i = 0; i < tSmallest.zones.Count; i++)
		{
			TileZone[] tNeighbours = tSmallest.zones[i].neighbours_all;
			for (int j = 0; j < tNeighbours.Length; j++)
			{
				if (tNeighbours[j].city == tCheck)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool giveItem(Actor pActor, List<long> pItems, City pCity)
	{
		if (pItems.Count == 0)
		{
			return false;
		}
		if (!pActor.understandsHowToUseItems())
		{
			return false;
		}
		long tItemID = pItems.GetRandom();
		Item tNewItem = World.world.items.get(tItemID);
		EquipmentAsset tAsset = tNewItem.getAsset();
		ActorEquipmentSlot tActorSlot = pActor.equipment.getSlot(tAsset.equipment_type);
		if (!tActorSlot.isEmpty())
		{
			int tCurItemValue = tActorSlot.getItem().getValue();
			if (tNewItem.getValue() <= tCurItemValue)
			{
				return false;
			}
		}
		Item tPrevItem = null;
		if (!tActorSlot.isEmpty())
		{
			tPrevItem = tActorSlot.getItem();
			tActorSlot.takeAwayItem();
		}
		pItems.Remove(tItemID);
		tActorSlot.setItem(tNewItem, pActor);
		pActor.setStatsDirty();
		if (tPrevItem != null)
		{
			pCity.data.equipment.addItem(pCity, tPrevItem, pItems);
		}
		pCity._storage_version++;
		return true;
	}

	public int getLimitOfBuildingsType(BuildOrder pElement)
	{
		int tResult = pElement.limit_type;
		if (hasCulture())
		{
			string type = pElement.getBuildingAsset(this).type;
			if (!(type == "type_statue"))
			{
				if (type == "type_watch_tower")
				{
					if (culture.hasTrait("tower_lovers"))
					{
						tResult += CultureTraitLibrary.getValue("tower_lovers");
					}
					if (hasLeader())
					{
						tResult += (int)leader.stats["bonus_towers"];
					}
				}
			}
			else if (culture.hasTrait("statue_lovers"))
			{
				tResult += CultureTraitLibrary.getValue("statue_lovers");
			}
		}
		return tResult;
	}

	public Alliance getAlliance()
	{
		return kingdom.getAlliance();
	}

	public Clan getRoyalClan()
	{
		Clan tClan = null;
		if (tClan == null && hasLeader())
		{
			tClan = leader.clan;
		}
		if (tClan == null && kingdom.hasKing())
		{
			tClan = kingdom.king.clan;
		}
		return tClan;
	}

	public bool isOkToSendArmy()
	{
		if (!hasArmy())
		{
			return false;
		}
		float tMaxArmy = getMaxWarriors();
		return (float)army.countUnits() / tMaxArmy >= 0.7f;
	}

	public void tryToPutItem(Item pItem)
	{
		List<long> tCityItemList = data.equipment.getEquipmentList(pItem.getAsset().equipment_type);
		if (tCityItemList.Count >= status.maximum_items)
		{
			tryToPutItemInStorage(pItem);
			return;
		}
		data.equipment.addItem(this, pItem, tCityItemList);
		_storage_version++;
	}

	public void tryToPutItems(IEnumerable<Item> pItems)
	{
		foreach (Item tItem in pItems)
		{
			tryToPutItem(tItem);
		}
	}

	private void tryToPutItemInStorage(Item pNewItem)
	{
		float tNewItemValue = pNewItem.getValue();
		EquipmentType tListType = pNewItem.getAsset().equipment_type;
		List<long> tCityItemList = data.equipment.getEquipmentList(tListType);
		for (int i = 0; i < tCityItemList.Count; i++)
		{
			long tID = tCityItemList[i];
			Item tOldCityItem = World.world.items.get(tID);
			float tCurValue = tOldCityItem.getValue();
			if (tNewItemValue > tCurValue)
			{
				tOldCityItem.clearCity();
				tCityItemList[i] = pNewItem.id;
				pNewItem.setInCityStorage(this);
				_storage_version++;
				break;
			}
		}
	}

	public int getZoneRange(bool pAllowCheat = true)
	{
		if (pAllowCheat && DebugConfig.isOn(DebugOption.CityUnlimitedZoneRange))
		{
			return 999;
		}
		return 13;
	}

	public bool reachableFrom(City pCity)
	{
		WorldTile tTile1 = getTile();
		if (tTile1 == null)
		{
			return false;
		}
		WorldTile tTile2 = pCity.getTile();
		if (tTile2 == null)
		{
			return false;
		}
		return tTile1.reachableFrom(tTile2);
	}

	public bool hasLeader()
	{
		if (leader == null)
		{
			return false;
		}
		if (!leader.isAlive())
		{
			removeLeader();
			return false;
		}
		return true;
	}

	public override void convertSameSpeciesAroundUnit(Actor pActorMain, bool pOverride = false)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pActorMain.current_tile, 2))
		{
			if (!tActor.hasCity() && !tActor.isKingdomCiv() && tActor.isSameSpecies(pActorMain) && tActor.isSapient())
			{
				tActor.joinCity(this);
			}
		}
	}

	public override void forceConvertSameSpeciesAroundUnit(Actor pActorMain)
	{
		convertSameSpeciesAroundUnit(pActorMain, true);
	}

	public void setUnitMetas(Actor pActor)
	{
		if (pActor.hasCulture())
		{
			setCulture(pActor.culture);
		}
		if (pActor.hasLanguage())
		{
			setLanguage(pActor.language);
		}
		if (pActor.hasReligion())
		{
			setReligion(pActor.religion);
		}
	}

	public override void save()
	{
		base.save();
		if (hasCulture())
		{
			data.id_culture = culture.id;
		}
		if (hasReligion())
		{
			data.id_religion = religion.id;
		}
		if (hasLanguage())
		{
			data.id_language = language.id;
		}
		if (kingdom == null)
		{
			data.kingdomID = -1L;
		}
		else
		{
			data.kingdomID = kingdom.id;
		}
		data.zones.Clear();
		foreach (TileZone tZone in zones)
		{
			ZoneData tZoneData = new ZoneData
			{
				x = tZone.x,
				y = tZone.y
			};
			data.zones.Add(tZoneData);
		}
	}

	public bool hasCulture()
	{
		if (culture != null && !culture.isAlive())
		{
			setCulture(null);
		}
		return culture != null;
	}

	public bool hasLanguage()
	{
		if (language != null && !language.isAlive())
		{
			setLanguage(null);
		}
		return language != null;
	}

	internal void setLanguage(Language pLanguage)
	{
		if (language != pLanguage)
		{
			language = pLanguage;
			World.world.languages.setDirtyCities();
		}
	}

	internal void setReligion(Religion pReligion)
	{
		if (religion != pReligion)
		{
			religion = pReligion;
			World.world.religions.setDirtyCities();
		}
	}

	public Subspecies getMainSubspecies()
	{
		if (hasLeader())
		{
			return leader.subspecies;
		}
		if (getPopulationPeople() == 0)
		{
			return null;
		}
		return base.units[0].subspecies;
	}

	public bool hasReligion()
	{
		if (religion != null && !religion.isAlive())
		{
			setReligion(null);
		}
		return religion != null;
	}

	public bool hasStockpiles()
	{
		return stockpiles.Count > 0;
	}

	public bool hasStorages()
	{
		return storages.Count > 0;
	}

	public Building getRandomStockpile()
	{
		if (!hasStockpiles())
		{
			return null;
		}
		foreach (Building tStockpile in stockpiles.LoopRandom())
		{
			if (tStockpile.isUsable())
			{
				return tStockpile;
			}
		}
		return null;
	}

	public void takeResource(string pResourceID, int pAmount)
	{
		if (!hasStorages())
		{
			return;
		}
		int tLeftToTake = pAmount;
		foreach (Building tStorage in storages)
		{
			if (tStorage.isUsable())
			{
				int tAmountCanTake = 0;
				tAmountCanTake = ((tStorage.getResourcesAmount(pResourceID) < tLeftToTake) ? tStorage.getResourcesAmount(pResourceID) : tLeftToTake);
				tStorage.takeResource(pResourceID, tAmountCanTake);
				tLeftToTake -= tAmountCanTake;
				if (tLeftToTake == 0)
				{
					break;
				}
			}
		}
		_storage_version++;
	}

	public int getResourcesAmount(string pResourceID)
	{
		if (!hasStorages())
		{
			return 0;
		}
		int tResult = 0;
		foreach (Building tStorage in storages)
		{
			if (tStorage.isUsable())
			{
				tResult += tStorage.getResourcesAmount(pResourceID);
			}
		}
		return tResult;
	}

	public int addResourcesToRandomStockpile(string pResourceID, int pAmount = 1)
	{
		Building tStockpile = getRandomStockpile();
		if (tStockpile == null)
		{
			return 0;
		}
		_storage_version++;
		return tStockpile.addResources(pResourceID, pAmount);
	}

	public bool hasSpaceForResourceInStockpile(ResourceAsset pResourceAsset)
	{
		if (!hasStockpiles())
		{
			return false;
		}
		foreach (Building tStockpile in stockpiles)
		{
			if (tStockpile.isUsable() && tStockpile.hasSpaceForResource(pResourceAsset))
			{
				return true;
			}
		}
		return false;
	}

	public bool hasResourcesForNewItems()
	{
		if (!hasStorages())
		{
			return false;
		}
		foreach (Building tStorage in storages)
		{
			if (tStorage.isUsable() && tStorage.hasResourcesForNewItems())
			{
				return true;
			}
		}
		return false;
	}

	public ResourceAsset getRandomSuitableFood(Subspecies pSubspecies)
	{
		if (!hasStorages())
		{
			return null;
		}
		foreach (Building tStorage in storages)
		{
			if (tStorage.isUsable())
			{
				ResourceAsset tAsset = tStorage.getRandomSuitableFood(pSubspecies);
				if (tAsset != null)
				{
					return tAsset;
				}
			}
		}
		return null;
	}

	public int countFood()
	{
		if (!hasStorages())
		{
			return 0;
		}
		int tResult = 0;
		foreach (Building tStorage in storages)
		{
			if (tStorage.isUsable())
			{
				tResult += tStorage.countFood();
			}
		}
		return tResult;
	}

	public ListPool<CityStorageSlot> getTotalResourceSlots(ResType[] pResTypes)
	{
		foreach (CityStorageSlot tSlot in _total_resource_slots.Values)
		{
			ResourceAsset tAsset = tSlot.asset;
			if (pResTypes.IndexOf(tAsset.type) != -1)
			{
				tSlot.amount = 0;
			}
		}
		foreach (Building tBuilding in storages)
		{
			if (!tBuilding.isUsable())
			{
				continue;
			}
			foreach (CityStorageSlot tSlot2 in tBuilding.resources.getSlots())
			{
				_total_resource_slots.TryGetValue(tSlot2.id, out var tTotalSlot);
				if (tTotalSlot == null)
				{
					tTotalSlot = new CityStorageSlot(tSlot2.id);
					_total_resource_slots[tSlot2.id] = tTotalSlot;
				}
				tTotalSlot.amount += tSlot2.amount;
			}
		}
		ListPool<CityStorageSlot> tResult = new ListPool<CityStorageSlot>(_total_resource_slots.Count);
		foreach (CityStorageSlot tSlot3 in _total_resource_slots.Values)
		{
			ResourceAsset tAsset2 = tSlot3.asset;
			if (pResTypes.IndexOf(tAsset2.type) != -1 && tSlot3.amount != 0)
			{
				tResult.Add(tSlot3);
			}
		}
		tResult.Sort((CityStorageSlot a, CityStorageSlot b) => a.asset.order.CompareTo(b.asset.order));
		return tResult;
	}

	public bool hasKingdom()
	{
		return kingdom != null;
	}

	public float getTimerForNewWarrior()
	{
		return _timer_warrior;
	}

	public List<long> getEquipmentList(EquipmentType pType)
	{
		return data.equipment.getEquipmentList(pType);
	}

	public bool planAllowsToPlaceBuildingInZone(TileZone pZone, TileZone pCenterZone)
	{
		if (status.housing_total < 10 && zones.Count < 20)
		{
			return true;
		}
		return culture.planAllowsToPlaceBuildingInZone(pZone, pCenterZone);
	}

	public bool hasSpecialTownPlans()
	{
		if (!hasCulture())
		{
			return false;
		}
		return culture.hasSpecialTownPlans();
	}

	public bool isNeutral()
	{
		return kingdom.isNeutral();
	}

	public bool isWelcomedToJoin(Actor pActor)
	{
		if (pActor.kingdom == kingdom)
		{
			return true;
		}
		if (pActor.isSameSubspecies(getMainSubspecies()))
		{
			return true;
		}
		if (!hasCulture())
		{
			return false;
		}
		if (culture.hasTrait("xenophobic"))
		{
			return false;
		}
		if (pActor.hasCultureTrait("xenophobic"))
		{
			return false;
		}
		if (culture.hasTrait("xenophiles"))
		{
			if (!pActor.hasCulture())
			{
				return true;
			}
			if (pActor.hasCultureTrait("xenophiles"))
			{
				return true;
			}
		}
		if (isSameSpeciesAsActor(pActor))
		{
			return true;
		}
		return false;
	}

	public bool isSameSpeciesAsActor(Actor pActor)
	{
		if (pActor.isSameSpecies(getCurrentSpecies()))
		{
			return true;
		}
		return false;
	}

	public string getCurrentSpecies()
	{
		Subspecies tMainSubspecies = getMainSubspecies();
		if (tMainSubspecies != null)
		{
			return tMainSubspecies.getActorAsset().id;
		}
		return getActorAsset().id;
	}

	public Sprite getCurrentSpeciesIcon()
	{
		Subspecies tMainSubspecies = getMainSubspecies();
		if (tMainSubspecies != null)
		{
			return tMainSubspecies.getSpriteIcon();
		}
		return getActorAsset().getSpriteIcon();
	}

	public bool hasTransportBoats()
	{
		foreach (Actor boat in _boats)
		{
			if (boat.asset.is_boat_transport)
			{
				return true;
			}
		}
		return false;
	}

	public bool isCityUnderDangerFire()
	{
		return tasks.fire > 0;
	}

	public bool isPossibleToJoin(Actor pActor)
	{
		if (this == pActor.city)
		{
			return false;
		}
		if (isNeutral())
		{
			return true;
		}
		if (!isWelcomedToJoin(pActor))
		{
			return false;
		}
		if (pActor.city != null)
		{
			if (pActor.isKing())
			{
				return false;
			}
			if (pActor.isCityLeader())
			{
				return false;
			}
			if (pActor.city.getPopulationPeople() < getPopulationPeople())
			{
				return false;
			}
		}
		return true;
	}

	public override string ToString()
	{
		if (data == null)
		{
			return "[City is null]";
		}
		using StringBuilderPool tBuilder = new StringBuilderPool();
		tBuilder.Append($"[City:{base.id} ");
		if (!isAlive())
		{
			tBuilder.Append("[DEAD] ");
		}
		tBuilder.Append("\"" + name + "\" ");
		tBuilder.Append($"Kingdom:{kingdom?.id ?? (-1)} ");
		if (hasArmy())
		{
			tBuilder.Append($"Army:{army.id} ");
		}
		tBuilder.Append($"Units:{base.units.Count} ");
		if (isDirtyUnits())
		{
			tBuilder.Append("[Dirty] ");
		}
		if (!leader.isRekt())
		{
			tBuilder.Append($"Leader:{leader.id} ");
		}
		if (kingdom?.king?.city == this)
		{
			tBuilder.Append($"King:{kingdom.king.id} ");
		}
		return tBuilder.ToString().Trim() + "]";
	}
}
