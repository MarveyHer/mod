using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Building : BaseSimObject, IEquatable<Building>, IComparable<Building>, ILoadable<BuildingData>
{
	public BatchBuildings batch;

	internal bool positionDirty;

	internal bool sprite_dirty;

	internal bool tiles_dirty;

	private Sprite _last_colored_sprite;

	private ColorAsset _last_color_asset;

	internal Sprite last_main_sprite;

	internal BuildingData data;

	internal BuildingAsset asset;

	public bool flip_x;

	internal readonly List<WorldTile> tiles = new List<WorldTile>();

	public BuildingAnimationData animData;

	public int animData_index;

	private float _shake_timer;

	private float _shake_intensity_x;

	private float _shake_intensity_y;

	internal float lastAngle;

	private Vector2 _shake_offset;

	internal readonly List<TileZone> zones = new List<TileZone>();

	internal BuildingAnimationState animation_state;

	internal BuildingOwnershipState state_ownership;

	internal ListPool<BaseBuildingComponent> components_list;

	internal Docks component_docks;

	internal Wheat component_wheat;

	internal BuildingFruitGrowth component_fruit_growth;

	internal UnitSpawner component_unit_spawner;

	internal BuildingSpreadBiome component_biome_spreader;

	internal BuildingMonolith component_monolith;

	internal BuildingWaypoint component_waypoint;

	internal BuildingBiomeFoodProducer component_food_producer;

	internal Beehive component_beehive;

	internal readonly BuildingTweenScaleHelper scale_helper = new BuildingTweenScaleHelper();

	internal bool chopped;

	internal bool is_visible;

	internal bool check_spawn_animation;

	private float _timer_shake_resource;

	private float _auto_remove_timer;

	public HashSet<long> residents = new HashSet<long>();

	private Vector3 _last_scale = Vector3.zero;

	public Material material;

	protected override MetaType meta_type => MetaType.Building;

	internal WorldTile door_tile
	{
		get
		{
			if (!current_tile.has_tile_down)
			{
				return current_tile;
			}
			return current_tile.tile_down;
		}
	}

	public City city => current_tile.zone.city;

	public CityResources resources => data.resources;

	internal bool isBurnable()
	{
		if (!hasHealth())
		{
			return false;
		}
		if (hasCity())
		{
			City tCity = getCity();
			if (tCity.hasReligion() && tCity.getReligion().hasMetaTag("building_immunity_fire"))
			{
				return false;
			}
		}
		return asset.burnable;
	}

	public float getExistenceTime()
	{
		return World.world.getWorldTimeElapsedSince(data.created_time);
	}

	public float getExistenceMonths()
	{
		return getExistenceTime() / 5f;
	}

	public void setAnimData(int pIndex)
	{
		if (pIndex >= asset.building_sprites.animation_data.Count || pIndex < 0)
		{
			pIndex = 0;
		}
		animData = asset.building_sprites.animation_data[pIndex];
		animData_index = pIndex;
	}

	internal void stopFire()
	{
		finishStatusEffect("burning");
	}

	internal override void create()
	{
		base.create();
		setObjectType(MapObjectType.Building);
		startShake(0.3f);
	}

	protected sealed override void setDefaultValues()
	{
		base.setDefaultValues();
		flip_x = false;
		positionDirty = false;
		sprite_dirty = false;
		tiles_dirty = false;
		_last_colored_sprite = null;
		_last_color_asset = null;
		_shake_timer = 0f;
		lastAngle = 0f;
		residents.Clear();
		_shake_offset = Vector2.zero;
		animation_state = BuildingAnimationState.Normal;
		state_ownership = BuildingOwnershipState.None;
		chopped = false;
		is_visible = false;
		check_spawn_animation = false;
	}

	private T addComponent<T>() where T : BaseBuildingComponent, new()
	{
		T pObject = World.world.buildings.component_pool.get<T>();
		if (components_list == null)
		{
			components_list = new ListPool<BaseBuildingComponent>();
		}
		components_list.Add(pObject);
		pObject.create(this);
		batch.c_components.Add(this);
		return pObject;
	}

	public bool hasBooks()
	{
		if (data.books == null)
		{
			return false;
		}
		return data.books.hasAny();
	}

	public bool hasFreeBookSlot()
	{
		if (asset.book_slots == 0)
		{
			return false;
		}
		return asset.book_slots > data.books.totalBooks();
	}

	public void addBook(Book pBook)
	{
		data.books.addBook(pBook);
	}

	public bool isState(BuildingState pState)
	{
		return data.state == pState;
	}

	internal void setBuilding(WorldTile pTile, BuildingAsset pAsset, BuildingData pData)
	{
		current_tile = pTile;
		current_tile.zone.addBuildingMain(this);
		if (pData == null)
		{
			setTemplate(pAsset);
			data.mainX = pTile.pos.x;
			data.mainY = pTile.pos.y;
			setState(BuildingState.Normal);
			updateStats();
			setMaxHealth();
			if (asset.has_resources_grown_to_collect)
			{
				setHaveResourcesToCollect(asset.has_resources_grown_to_collect_on_spawn);
			}
		}
		else
		{
			setData(pData);
			setTemplate(pAsset);
		}
		setStatsDirty();
		current_position = current_tile.pos;
		current_scale.x = asset.scale_base.x;
		current_scale.y = asset.scale_base.y;
		fillTiles();
		if (!string.IsNullOrEmpty(asset.kingdom))
		{
			Kingdom tKingdom = World.world.kingdoms_wild.get(asset.kingdom);
			setKingdom(tKingdom);
		}
		if (!isUnderConstruction())
		{
			int tFrame = -1;
			if (pData != null)
			{
				tFrame = pData.frameID;
			}
			initAnimationData();
			if (tFrame != -1)
			{
				setAnimData(tFrame);
			}
		}
		checkMaterial();
		setPositionDirty();
		updatePosition();
		if (pAsset.storage && data.resources == null)
		{
			data.resources = new CityResources();
		}
		if (pAsset.book_slots > 0 && data.books == null)
		{
			data.books = new StorageBooks();
		}
		if (pAsset.smoke)
		{
			addComponent<BuildingSmokeEffect>();
		}
		if (pAsset.building_type == BuildingType.Building_Poops)
		{
			batch.c_poop.Add(this);
		}
		if (pAsset.spread)
		{
			switch (pAsset.flora_type)
			{
			case FloraType.Fungi:
				batch.c_spread_fungi.Add(this);
				break;
			case FloraType.Plant:
				batch.c_spread_plants.Add(this);
				break;
			case FloraType.Tree:
				batch.c_spread_trees.Add(this);
				break;
			}
		}
		if (pAsset.produce_biome_food)
		{
			component_food_producer = addComponent<BuildingBiomeFoodProducer>();
		}
		if (pAsset.spawn_drops)
		{
			addComponent<BuildingEffectSpawnDrop>();
		}
		if (pAsset.id == "monolith")
		{
			component_monolith = addComponent<BuildingMonolith>();
		}
		if (pAsset.waypoint)
		{
			component_waypoint = pAsset.kingdom switch
			{
				"alien_mold" => addComponent<BuildingWaypointAlienMold>(), 
				"computer" => addComponent<BuildingWaypointComputer>(), 
				"golden_egg" => addComponent<BuildingWaypointGoldenEgg>(), 
				"harp" => addComponent<BuildingWaypointHarp>(), 
				_ => throw new ArgumentOutOfRangeException(pAsset.kingdom + " is not a valid kingdom for a waypoint"), 
			};
		}
		if (pAsset.grow_creep)
		{
			addComponent<BuildingCreepHUB>();
		}
		if (pAsset.wheat)
		{
			component_wheat = addComponent<Wheat>();
		}
		if (pAsset.building_type == BuildingType.Building_Fruits)
		{
			component_fruit_growth = addComponent<BuildingFruitGrowth>();
		}
		if (pAsset.ice_tower)
		{
			addComponent<IceTower>();
		}
		if (pAsset.id == "poop")
		{
			addComponent<Poop>();
		}
		if (pAsset.spawn_units)
		{
			component_unit_spawner = addComponent<UnitSpawner>();
		}
		if (pAsset.spread_biome)
		{
			component_biome_spreader = addComponent<BuildingSpreadBiome>();
		}
		if (pAsset.beehive)
		{
			component_beehive = addComponent<Beehive>();
		}
		if (pAsset.docks)
		{
			component_docks = addComponent<Docks>();
		}
		if (pAsset.tower)
		{
			addComponent<BuildingTower>();
		}
		if (pData == null && !pAsset.city_building)
		{
			setAnimationState(BuildingAnimationState.Normal);
			this.setScaleTween();
		}
		if (isRuin())
		{
			makeRuins();
		}
		else if (asset.city_building && hasCity())
		{
			setKingdom(current_tile.zone_city.kingdom);
		}
		else if (asset.city_building && !hasCity() && isAbandoned())
		{
			makeAbandoned();
		}
	}

	private void debugCheckResourcesOnSpawn(BuildingAsset pAsset)
	{
	}

	public override void setStatsDirty()
	{
		base.setStatsDirty();
		if (isAlive())
		{
			batch.c_stats_dirty.Add(this);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void setPositionDirty()
	{
		positionDirty = true;
		batch.c_position_dirty.Add(this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BaseObjectData getData()
	{
		return data;
	}

	public void setData(BuildingData pData)
	{
		data = pData;
	}

	public void loadData(BuildingData pData)
	{
		setData(pData);
		pData.load();
	}

	public void loadBuilding(BuildingData pData)
	{
		if (!isUnderConstruction())
		{
			setAnimData(pData.frameID);
		}
		if (data.resources != null)
		{
			resources.loadFromSave();
		}
	}

	internal void setHaveResourcesToCollect(bool pValue)
	{
		if (pValue)
		{
			data.addFlag("has_resources");
		}
		else
		{
			data.removeFlag("has_resources");
		}
	}

	public bool hasResourcesToCollect()
	{
		if (asset.has_resources_grown_to_collect)
		{
			return data.hasFlag("has_resources");
		}
		if (chopped)
		{
			return false;
		}
		return asset.has_resources_to_collect;
	}

	internal bool canBeUpgraded()
	{
		if (isUnderConstruction())
		{
			return false;
		}
		if (asset.city_building && !isCiv())
		{
			return false;
		}
		return asset.can_be_upgraded;
	}

	internal bool upgradeBuilding()
	{
		if (!canBeUpgraded())
		{
			return false;
		}
		BuildingAsset tNewTemplate = AssetManager.buildings.get(asset.upgrade_to);
		if ((tNewTemplate.fundament.left != asset.fundament.left || tNewTemplate.fundament.right != asset.fundament.right || tNewTemplate.fundament.top != asset.fundament.top || tNewTemplate.fundament.bottom != asset.fundament.bottom) && !checkTilesForUpgrade(current_tile, tNewTemplate))
		{
			return false;
		}
		makeZoneDirty();
		setTemplate(tNewTemplate);
		initAnimationData();
		updateStats();
		setMaxHealth();
		fillTiles();
		return true;
	}

	private void setTemplate(BuildingAsset pTemplate)
	{
		asset = pTemplate;
		data.asset_id = asset.id;
		asset.buildings.Add(this);
		if (asset.canBeOccupied())
		{
			World.world.buildings.occupied_buildings.Add(this);
		}
		asset.checkSpritesAreLoaded();
	}

	internal void setMaterial(string pMaterialID)
	{
		material = LibraryMaterials.instance.dict[pMaterialID];
	}

	internal void setKingdomCiv(Kingdom pKingdom)
	{
		if (kingdom != pKingdom || !hasKingdom())
		{
			setKingdom(pKingdom);
		}
	}

	internal void makeRuins()
	{
		setKingdom(World.world.kingdoms_wild.get("ruins"));
		setState(BuildingState.Ruins);
	}

	public void makeAbandoned()
	{
		setKingdom(WildKingdomsManager.abandoned);
		if (isUnderConstruction())
		{
			startDestroyBuilding();
		}
		else if (!asset.can_be_abandoned)
		{
			if (asset.has_ruin_state)
			{
				startMakingRuins();
			}
			else
			{
				startDestroyBuilding();
			}
		}
	}

	public void setKingdom(Kingdom pKingdom)
	{
		if (kingdom != pKingdom)
		{
			if (kingdom != pKingdom)
			{
				makeZoneDirty();
			}
			checkKingdom();
			kingdom = pKingdom;
			checkKingdom();
			if (isKingdomCiv())
			{
				setOwnershipState(BuildingOwnershipState.Civilization);
			}
			else
			{
				setOwnershipState(BuildingOwnershipState.World);
			}
			setTilesDirty();
			World.world.sim_object_zones.setBuildingsDirty(base.chunk);
		}
	}

	private void checkKingdom()
	{
		if (hasKingdom())
		{
			if (kingdom.wild)
			{
				World.world.kingdoms_wild.setDirtyBuildings();
			}
			else
			{
				World.world.kingdoms.setDirtyBuildings();
			}
		}
	}

	public bool hasHousingLogic()
	{
		if (asset.canBeOccupied())
		{
			return true;
		}
		return false;
	}

	private void setState(BuildingState pState)
	{
		if (hasHousingLogic())
		{
			World.world.buildings.event_houses = true;
		}
		if (isRemoved())
		{
			return;
		}
		if (pState == BuildingState.Ruins && !isRuin())
		{
			bool tIsOnLava = false;
			if (tIsOnLava)
			{
				foreach (WorldTile tile in tiles)
				{
					if (tile.Type.lava)
					{
						tIsOnLava = true;
						break;
					}
				}
			}
			if (tIsOnLava)
			{
				setHealth(getMaxHealthPercent(0.5f));
			}
			else
			{
				setMaxHealth();
			}
			stats["health"] = getHealth();
		}
		data.state = pState;
		checkAutoRemove();
		checkMaterial();
		clearZones();
		if (!isRemoved())
		{
			fillTiles();
		}
		setTilesDirty();
		World.world.sim_object_zones.setBuildingsDirty(base.chunk);
	}

	public void checkMaterial()
	{
		if (data.state == BuildingState.Ruins)
		{
			setMaterial(BuildingRendererSettings.cur_default_material);
		}
		else if (BuildingRendererSettings.wobbly_material_enabled)
		{
			setMaterial(asset.material);
		}
		else
		{
			setMaterial(BuildingRendererSettings.cur_default_material);
		}
	}

	internal void updateKingdomColors()
	{
		setTilesDirty();
	}

	internal bool checkTilesForUpgrade(WorldTile pTile, BuildingAsset pTemplate)
	{
		WorldTile tTile = null;
		int tX = pTile.pos.x - pTemplate.fundament.left;
		int tY = pTile.pos.y - pTemplate.fundament.bottom;
		int tWidth = pTemplate.fundament.right + pTemplate.fundament.left + 1;
		int tHeight = pTemplate.fundament.top + pTemplate.fundament.bottom + 1;
		for (int xx = 0; xx < tWidth; xx++)
		{
			for (int yy = 0; yy < tHeight; yy++)
			{
				tTile = World.world.GetTile(tX + xx, tY + yy);
				if (tTile == null)
				{
					return false;
				}
				if (!tTile.Type.can_build_on)
				{
					return false;
				}
				if (tTile.zone.city != city)
				{
					return false;
				}
				Building tBuilding = tTile.building;
				if (tBuilding != null && tBuilding != this)
				{
					if (tBuilding.asset.priority >= asset.priority)
					{
						return false;
					}
					if (tBuilding.asset.upgrade_level >= asset.upgrade_level)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	internal void debugConstructions()
	{
		if (!(asset.building_sprites.construction == null))
		{
			setUnderConstruction();
		}
	}

	private void initAnimationData()
	{
		asset.checkSpritesAreLoaded();
		int tRandomVariation = Randy.randomInt(0, asset.building_sprites.animation_data.Count);
		setAnimData(tRandomVariation);
		if (asset.random_flip && !asset.shadow)
		{
			flip_x = Randy.randomBool();
		}
		this.setScaleTween();
	}

	private void fillTiles()
	{
		if (tiles.Count != 0)
		{
			clearTiles();
		}
		int tX = current_tile.pos.x - asset.fundament.left;
		int tY = current_tile.pos.y - asset.fundament.bottom;
		int tWidth = asset.fundament.right + asset.fundament.left + 1;
		int tHeight = asset.fundament.top + asset.fundament.bottom + 1;
		int tStartY = 0;
		for (int xx = 0; xx < tWidth; xx++)
		{
			for (int yy = tStartY; yy < tHeight; yy++)
			{
				WorldTile tTile = World.world.GetTile(tX + xx, tY + yy);
				if (tTile != null)
				{
					setBuildingTile(tTile, xx, yy);
				}
			}
		}
		setTilesDirty();
	}

	internal void checkDirtyTiles()
	{
		if (tiles_dirty)
		{
			tiles_dirty = false;
			for (int i = 0; i < tiles.Count; i++)
			{
				WorldTile tTile = tiles[i];
				World.world.setTileDirty(tTile);
			}
			batch?.c_tiles_dirty.Remove(this);
		}
	}

	private void setTilesDirty()
	{
		tiles_dirty = true;
		batch?.c_tiles_dirty.Add(this);
	}

	private void forceUpdateTilesDirty()
	{
		setTilesDirty();
		checkDirtyTiles();
	}

	private void setBuildingTile(WorldTile pTile, int pX, int pY)
	{
		if (pTile.hasBuilding() && pTile.building != this)
		{
			pTile.building.startDestroyBuilding();
		}
		pTile.building = this;
		pTile.minimap_building_x = pX;
		pTile.minimap_building_y = pY;
		if (!tiles.Contains(pTile))
		{
			tiles.Add(pTile);
			if (!zones.Contains(pTile.zone))
			{
				zones.Add(pTile.zone);
			}
		}
		TileType tType = null;
		TopTileType tTopType = null;
		if (asset.transform_tiles_to_tile_type != null)
		{
			tType = AssetManager.tiles.get(asset.transform_tiles_to_tile_type);
		}
		if (asset.transform_tiles_to_top_tiles != null)
		{
			tTopType = AssetManager.top_tiles.get(asset.transform_tiles_to_top_tiles);
		}
		if (tType != null || tTopType != null)
		{
			if (tType == null)
			{
				tType = pTile.main_type;
			}
			if (tType.can_be_biome)
			{
				MapAction.terraformTile(pTile, tType, tTopType, TerraformLibrary.nothing);
			}
		}
	}

	public void setOwnershipState(BuildingOwnershipState pState)
	{
		if (state_ownership != pState)
		{
			makeZoneDirty();
		}
		state_ownership = pState;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isRuin()
	{
		return data.state == BuildingState.Ruins;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isRemoved()
	{
		return data.state == BuildingState.Removed;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isNormal()
	{
		return data.state == BuildingState.Normal;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isAbandoned()
	{
		if (state_ownership == BuildingOwnershipState.World)
		{
			return asset.city_building;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isCiv()
	{
		return state_ownership == BuildingOwnershipState.Civilization;
	}

	public void prepareForSave()
	{
		if (hasCity())
		{
			data.cityID = city.data.id;
		}
		else
		{
			data.cityID = -1L;
		}
		resources?.save();
		data.frameID = animData_index;
		data.save();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isUsable()
	{
		if (!isAlive())
		{
			return false;
		}
		if (isRuin())
		{
			return false;
		}
		if (isOnRemove())
		{
			return false;
		}
		if (isRemoved())
		{
			return false;
		}
		return true;
	}

	internal void startDestroyBuilding()
	{
		if (!isOnRemove())
		{
			if (asset.has_ruins_graphics && !isUnderConstruction())
			{
				setState(BuildingState.Ruins);
			}
			startRemove();
		}
	}

	private void clearZones()
	{
		zones.Clear();
	}

	internal void kill()
	{
		if (!isAlive())
		{
			return;
		}
		clearZones();
		setAlive(pValue: false);
		if (asset.city_building)
		{
			World.world.map_stats.housesDestroyed++;
		}
		if (!hasBooks())
		{
			return;
		}
		foreach (long tBookID in data.books.list_books)
		{
			Book tBook = World.world.books.get(tBookID);
			World.world.books.burnBook(tBook);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override City getCity()
	{
		return city;
	}

	internal override void updateStats()
	{
		base.updateStats();
		stats.clear();
		stats.mergeStats(asset.base_stats);
		if (getHealth() > getMaxHealth())
		{
			setMaxHealth();
		}
		batch.c_stats_dirty.Remove(this);
	}

	internal void chopTree()
	{
		if (!chopped && ((!asset.become_alive_when_chopped && !WorldLawLibrary.world_law_bark_bites_back.isEnabled()) || !Randy.randomChance(0.2f) || !ActionLibrary.tryToMakeFloraAlive(this)))
		{
			finishAllStatusEffects();
			MusicBox.playSound("event:/SFX/NATURE/TreeFall", current_tile, pGameViewOnly: true, pVisibleOnly: true);
			chopped = true;
			setHaveResourcesToCollect(pValue: false);
			float tAngle = (Randy.randomBool() ? 90 : (-90));
			scale_helper.doRotateTween(tAngle, 1f, finishChop);
			batch.c_angle.Add(this);
		}
	}

	private void finishChop()
	{
		startRemove();
	}

	private void startRemove()
	{
		if (!isOnRemove())
		{
			if (!isUnderConstruction() && asset.has_sound_destroyed)
			{
				MusicBox.playSound(asset.sound_destroyed, current_tile, pGameViewOnly: true, pVisibleOnly: true);
			}
			setAnimationState(BuildingAnimationState.OnRemove);
			clearTiles();
			clearComponents();
			setHaveResourcesToCollect(pValue: false);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isAnimationState(BuildingAnimationState pState)
	{
		return animation_state == pState;
	}

	internal void startMakingRuins()
	{
		if (!asset.has_ruin_state)
		{
			startRemove();
		}
		else if (!isAnimationState(BuildingAnimationState.OnRuin) && data.state != BuildingState.Ruins)
		{
			setAnimationState(BuildingAnimationState.OnRuin);
			makeRuins();
		}
	}

	internal void removeBuildingFinal()
	{
		setState(BuildingState.Removed);
		clearZones();
		clearTiles();
		kill();
		current_tile.zone.removeBuildingMain(this);
		World.world.buildings.scheduleDestroyOnPlay(this);
	}

	internal void clearTiles()
	{
		forceUpdateTilesDirty();
		for (int i = 0; i < tiles.Count; i++)
		{
			tiles[i].building = null;
		}
		tiles.Clear();
	}

	private void clearComponents()
	{
		if (asset.flora_type == FloraType.Tree)
		{
			batch.c_spread_trees.Remove(this);
		}
		if (asset.flora_type == FloraType.Fungi)
		{
			batch.c_spread_fungi.Remove(this);
		}
		if (asset.flora_type == FloraType.Plant)
		{
			batch.c_spread_plants.Remove(this);
		}
		if (asset.building_type == BuildingType.Building_Poops)
		{
			batch.c_poop.Remove(this);
		}
		if (components_list != null)
		{
			batch.c_components.Remove(this);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isOnRemove()
	{
		return animation_state == BuildingAnimationState.OnRemove;
	}

	internal void setAnimationState(BuildingAnimationState pState)
	{
		if (!isOnRemove())
		{
			animation_state = pState;
			this.checkTweens();
		}
	}

	internal void completeMakingRuin()
	{
		setState(BuildingState.Ruins);
		setAnimationState(BuildingAnimationState.Normal);
		this.setScaleTween();
	}

	private void checkAutoRemove()
	{
		if (batch != null)
		{
			if (asset.auto_remove_ruin && isRuin() && !isCiv())
			{
				batch.c_auto_remove.Add(this);
			}
			else
			{
				batch.c_auto_remove.Remove(this);
			}
		}
	}

	internal void updateAutoRemove(float pElapsed)
	{
		if (_auto_remove_timer < 300f)
		{
			_auto_remove_timer += pElapsed;
			return;
		}
		_auto_remove_timer = 0f;
		batch.c_auto_remove.Remove(this);
		startDestroyBuilding();
	}

	internal void updateTimerShakeResources(float pElapsed)
	{
		if (_timer_shake_resource > 0f)
		{
			_timer_shake_resource -= pElapsed;
			if (_timer_shake_resource <= 0f)
			{
				batch.c_resource_shaker.Remove(this);
			}
		}
	}

	internal void updateComponents(float pElapsed)
	{
		for (int i = 0; i < components_list.Count; i++)
		{
			components_list[i].update(pElapsed);
		}
	}

	public void updatePosition()
	{
		if (positionDirty)
		{
			positionDirty = false;
			batch.c_position_dirty.Remove(this);
			cur_transform_position = current_tile.posV3;
			if (cur_transform_position.z < 0f)
			{
				cur_transform_position.z = 0f;
			}
			cur_transform_position.x += _shake_offset.x;
			cur_transform_position.y += _shake_offset.y;
			cur_transform_position.z = -0.2f + asset.bonus_z;
		}
	}

	internal void spawnBurstSpecial(int pAmount = 1)
	{
		WorldTile tTile = World.world.GetTile(current_tile.pos.x, current_tile.pos.y);
		if (tTile == null)
		{
			tTile = current_tile;
		}
		for (int i = 0; i < pAmount; i++)
		{
			World.world.drop_manager.spawnParabolicDrop(tTile, asset.spawn_drop_id, asset.spawn_drop_start_height, asset.spawn_drop_min_height, asset.spawn_drop_max_height, asset.spawn_drop_min_radius, asset.spawn_drop_max_radius);
		}
	}

	internal bool updateBuild(int pProgress = 1)
	{
		data.change("construction_progress", pProgress);
		startShake(0.3f);
		bool tFinished = false;
		if (getConstructionProgress() > asset.construction_progress_needed)
		{
			tFinished = true;
			completeConstruction();
			if (asset.has_sound_built)
			{
				MusicBox.playSound(asset.sound_built, current_tile, pGameViewOnly: true, pVisibleOnly: true);
			}
			initAnimationData();
			this.setScaleTween(0.25f);
		}
		else
		{
			this.setScaleTween(0.75f);
		}
		return tFinished;
	}

	private void makeZoneDirty()
	{
		current_tile.zone.setDirty(pValue: true);
		if (hasHousingLogic())
		{
			World.world.buildings.event_houses = true;
		}
	}

	public bool hasResidentSlots()
	{
		if (!asset.hasHousingSlots())
		{
			return false;
		}
		if (asset.housing_slots > countResidents())
		{
			return true;
		}
		return false;
	}

	public int countResidents()
	{
		return residents.Count;
	}

	public bool hasResidents()
	{
		return countResidents() > 0;
	}

	public void startShake(float pDuration, float pIntensityX = 0.1f, float pIntensityY = 0.1f)
	{
		_shake_timer = pDuration;
		_shake_intensity_x = pIntensityX;
		_shake_intensity_y = pIntensityY;
		batch?.c_shake.Add(this);
	}

	internal void resourceGathering(float pElapsed)
	{
		if (!(_timer_shake_resource > 0f))
		{
			batch.c_resource_shaker.Add(this);
			startShake(0.3f);
			_timer_shake_resource = 1f;
		}
	}

	public void updateShake(float pElapsed)
	{
		if (_shake_timer > 0f)
		{
			_shake_timer -= pElapsed;
			if (_shake_timer < 0f)
			{
				_shake_offset = Vector2.zero;
				batch.c_shake.Remove(this);
			}
			else
			{
				_shake_offset.x = batch.rnd.NextFloat(0f - _shake_intensity_x, _shake_intensity_x);
				_shake_offset.y = batch.rnd.NextFloat(0f - _shake_intensity_y, _shake_intensity_y);
			}
			setPositionDirty();
		}
	}

	internal override void getHitFullHealth(AttackType pAttackType)
	{
		getHit(getHealth(), pFlash: false, pAttackType, null, pSkipIfShake: false, pMetallicWeapon: false, pCheckDamageReduction: false);
	}

	internal override void getHit(float pDamage, bool pFlash = true, AttackType pAttackType = AttackType.Other, BaseSimObject pAttacker = null, bool pSkipIfShake = true, bool pMetallicWeapon = false, bool pCheckDamageReduction = true)
	{
		if (!isAnimationState(BuildingAnimationState.Normal))
		{
			return;
		}
		changeHealth((int)(0f - pDamage));
		if (pAttackType == AttackType.Weapon && asset.has_sound_hit)
		{
			MusicBox.playSound(asset.sound_hit, current_tile, pGameViewOnly: true, pVisibleOnly: true);
		}
		startShake(0.3f);
		if (!hasHealth())
		{
			if (data.state == BuildingState.Ruins)
			{
				startDestroyBuilding();
			}
			else
			{
				startMakingRuins();
			}
		}
		else
		{
			this.setScaleTween(0.75f);
		}
	}

	internal void extractResources(Actor pBy)
	{
		this.setScaleTween(0.75f);
		switch (asset.building_type)
		{
		case BuildingType.Building_Wheat:
		case BuildingType.Building_Plant:
			startDestroyBuilding();
			break;
		case BuildingType.Building_Tree:
			chopTree();
			break;
		case BuildingType.Building_Poops:
		case BuildingType.Building_Mineral:
			startRemove();
			break;
		case BuildingType.Building_Fruits:
			component_fruit_growth.reset();
			setHaveResourcesToCollect(pValue: false);
			if (Randy.randomChance(0.2f))
			{
				startDestroyBuilding();
			}
			break;
		case BuildingType.Building_Hives:
			component_beehive.honey = 0;
			setHaveResourcesToCollect(pValue: false);
			break;
		}
	}

	internal Color32 getColorForMinimap(WorldTile pTile)
	{
		if (Config.EVERYTHING_MAGIC_COLOR)
		{
			return Toolbox.EVERYTHING_MAGIC_COLOR32;
		}
		return asset.building_sprites.map_icon.getColor(pTile.minimap_building_x, pTile.minimap_building_y, this);
	}

	public WorldTile getConstructionTile()
	{
		if (asset.docks)
		{
			var (tZones, tLength) = Toolbox.getAllZonesFromTile(current_tile);
			foreach (TileZone tTileZone in tZones.LoopRandom(tLength))
			{
				using IEnumerator<WorldTile> enumerator2 = checkZoneForDockConstruction(tTileZone).GetEnumerator();
				if (enumerator2.MoveNext())
				{
					return enumerator2.Current;
				}
			}
		}
		return Randy.getRandom(tiles);
	}

	public int getConstructionProgress()
	{
		data.get("construction_progress", out var tResult, 0);
		return tResult;
	}

	public void completeConstruction()
	{
		data.removeInt("construction_progress");
		data.removeFlag("under_construction");
		makeZoneDirty();
	}

	public bool isUnderConstruction()
	{
		if (!asset.has_sprite_construction)
		{
			return false;
		}
		return data.hasFlag("under_construction");
	}

	public void setUnderConstruction()
	{
		if (asset.has_sprite_construction)
		{
			data.addFlag("under_construction");
		}
	}

	public bool canRemoveForFarms()
	{
		return asset.flora;
	}

	internal IEnumerable<WorldTile> checkZoneForDockConstruction(TileZone pZone)
	{
		if (pZone.city == null || pZone.city != city)
		{
			yield break;
		}
		foreach (WorldTile iTile in pZone.tiles.LoopRandom())
		{
			if (iTile.Type.ground && Toolbox.SquaredDistTile(current_tile, iTile) <= 49)
			{
				yield return iTile;
			}
		}
	}

	internal void checkStartSpawnAnimation()
	{
		Sprite[] spawn = animData.spawn;
		if (spawn != null && spawn.Length != 0)
		{
			check_spawn_animation = true;
		}
	}

	public Sprite calculateMainSprite()
	{
		bool tAnimationAllowed = true;
		Sprite[] tSprites = null;
		bool tIsRuin = isRuin();
		if (tIsRuin)
		{
			tAnimationAllowed = false;
		}
		if (isUnderConstruction())
		{
			last_main_sprite = asset.building_sprites.construction;
			return last_main_sprite;
		}
		if (asset.has_special_animation_state)
		{
			tSprites = ((!hasResourcesToCollect()) ? animData.special : animData.main);
		}
		else if (tIsRuin && asset.has_ruins_graphics)
		{
			tAnimationAllowed = false;
			tSprites = animData.ruins;
		}
		else if (asset.spawn_drops && data.hasFlag("stop_spawn_drops"))
		{
			tSprites = animData.main_disabled;
		}
		else if (asset.can_be_abandoned && isAbandoned())
		{
			Sprite[] main_disabled = animData.main_disabled;
			tSprites = ((main_disabled == null || main_disabled.Length == 0) ? animData.main : animData.main_disabled);
			tAnimationAllowed = false;
		}
		else
		{
			tSprites = animData.main;
			if (asset.get_override_sprites_main != null)
			{
				Sprite[] tOverride = asset.get_override_sprites_main(this);
				if (tOverride != null)
				{
					tSprites = tOverride;
				}
			}
		}
		Sprite tMainSprite = null;
		if (check_spawn_animation)
		{
			return getSpawnFrameSprite();
		}
		if (!tAnimationAllowed || tSprites.Length == 1)
		{
			return tSprites[0];
		}
		return AnimationHelper.getSpriteFromList(GetHashCode(), tSprites, asset.animation_speed);
	}

	public bool isColoredSpriteNeedsCheck(Sprite pMainSprite)
	{
		if ((object)last_main_sprite == null || last_main_sprite.GetHashCode() != pMainSprite.GetHashCode() || _last_color_asset != kingdom.getColor())
		{
			return true;
		}
		return false;
	}

	public Sprite calculateColoredSprite(Sprite pMainSprite)
	{
		if (isColoredSpriteNeedsCheck(pMainSprite))
		{
			_last_colored_sprite = DynamicSprites.getRecoloredBuilding(pMainSprite, kingdom.getColor(), asset.atlas_asset);
			last_main_sprite = pMainSprite;
			_last_color_asset = kingdom.getColor();
		}
		return _last_colored_sprite;
	}

	public Sprite getLastColoredSprite()
	{
		return _last_colored_sprite;
	}

	public void clearSprites()
	{
		last_main_sprite = null;
		_last_colored_sprite = null;
		_last_color_asset = null;
	}

	public Sprite checkSpriteToRender()
	{
		Sprite tMainSprite = calculateMainSprite();
		return calculateColoredSprite(tMainSprite);
	}

	public Vector3 getCurrentScale()
	{
		float tTweenBuildingsValue = World.world.quality_changer.getTweenBuildingsValue();
		float tScaleY = current_scale.y * tTweenBuildingsValue;
		float tScaleX = current_scale.x * tTweenBuildingsValue;
		if (_last_scale.y != tScaleY || _last_scale.x != tScaleX)
		{
			_last_scale.Set(tScaleX, tScaleY, 1f);
		}
		return _last_scale;
	}

	public bool isFullyGrown()
	{
		if (!asset.can_be_grown)
		{
			return true;
		}
		if (asset.wheat)
		{
			return component_wheat.isMaxLevel();
		}
		return false;
	}

	private Sprite getSpawnFrameSprite()
	{
		Sprite[] tSpawnSprites = animData.spawn;
		float tSpawnTime = World.world.getWorldTimeElapsedSince(data.created_time);
		float tTotalAnimationTime = (float)tSpawnSprites.Length * asset.animation_speed / 60f;
		Sprite tResult;
		if (tTotalAnimationTime > tSpawnTime)
		{
			int tSpawnIndex = (int)(tSpawnTime / tTotalAnimationTime * (float)tSpawnSprites.Length);
			tResult = tSpawnSprites[tSpawnIndex];
		}
		else
		{
			tResult = tSpawnSprites.Last();
			check_spawn_animation = false;
		}
		return tResult;
	}

	public int takeResource(string pResourceID, int pAmount)
	{
		return resources.change(pResourceID, -pAmount);
	}

	public int getResourcesAmount(string pResourceID)
	{
		return resources.get(pResourceID);
	}

	public int addResources(string pResourceID, int pAmount)
	{
		return resources.change(pResourceID, pAmount);
	}

	public bool hasSpaceForResource(ResourceAsset pResourceAsset)
	{
		return resources.hasSpaceForResource(pResourceAsset);
	}

	public bool hasResourcesForNewItems()
	{
		return resources.hasResourcesForNewItems();
	}

	public int countFood()
	{
		return resources.countFood();
	}

	public ResourceAsset getRandomSuitableFood(Subspecies pSubspecies, string pFavoriteFood = null)
	{
		return resources.getRandomSuitableFood(pSubspecies, pFavoriteFood);
	}

	public override void Dispose()
	{
		kingdom = null;
		_last_colored_sprite = null;
		_last_color_asset = null;
		last_main_sprite = null;
		batch = null;
		data = null;
		asset = null;
		tiles.Clear();
		animData = null;
		zones.Clear();
		if (components_list != null)
		{
			for (int i = 0; i < components_list.Count; i++)
			{
				BaseBuildingComponent tComponent = components_list[i];
				tComponent.Dispose();
				World.world.buildings.component_pool.release(tComponent);
			}
			components_list.Clear();
			components_list.Dispose();
			components_list = null;
		}
		component_docks = null;
		component_wheat = null;
		component_fruit_growth = null;
		component_unit_spawner = null;
		component_biome_spreader = null;
		component_monolith = null;
		component_waypoint = null;
		component_food_producer = null;
		component_beehive = null;
		scale_helper.reset();
		base.Dispose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Building pObject)
	{
		return GetHashCode() == pObject.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Building pTarget)
	{
		return GetHashCode().CompareTo(pTarget.GetHashCode());
	}

	public void checkVegetationSpread(float pElapsed)
	{
		BuildingAsset tBuildingAsset = asset;
		if (Randy.randomChance(tBuildingAsset.spread_chance))
		{
			WorldTile tRandomTile = current_tile.neighboursAll.GetRandom();
			for (int i = 0; (float)i < tBuildingAsset.spread_steps; i++)
			{
				tRandomTile = tRandomTile.neighboursAll.GetRandom();
			}
			string tRandomSpreadId = tBuildingAsset.spread_ids.GetRandom();
			BuildingAsset tRandomSpreadAsset = AssetManager.buildings.get(tRandomSpreadId);
			tryToGrowOnTile(tRandomTile, tRandomSpreadAsset);
		}
	}

	private bool tryToGrowOnTile(WorldTile pTile, BuildingAsset pAsset, bool pCheckLimit = true)
	{
		if (pCheckLimit && pTile.zone.hasReachedBuildingLimit(pTile, pAsset))
		{
			return false;
		}
		if (!World.world.buildings.canBuildFrom(pTile, pAsset, null, BuildPlacingType.New, pFloraGrowth: true))
		{
			return false;
		}
		World.world.buildings.addBuilding(pAsset, pTile);
		if (pAsset.flora_type == FloraType.Tree)
		{
			World.world.game_stats.data.treesGrown++;
		}
		else if (pAsset.flora_type == FloraType.Plant || pAsset.flora_type == FloraType.Fungi)
		{
			World.world.game_stats.data.floraGrown++;
		}
		return true;
	}
}
