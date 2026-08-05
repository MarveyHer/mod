using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using db;
using UnityEngine;

public class Kingdom : MetaObjectWithTraits<KingdomData, KingdomTrait>
{
	public static KingdomCheckCache cache_enemy_check = new KingdomCheckCache();

	public KingdomAsset asset;

	public bool wild;

	public float timer_action;

	public Actor king;

	public City capital;

	public Culture culture;

	public Language language;

	public Religion religion;

	public readonly List<Building> buildings = new List<Building>();

	public readonly List<City> cities = new List<City>();

	public int power;

	public AiSystemKingdom ai;

	public Vector3 location;

	private float _cached_tax_local;

	private float _cached_tax_tribute;

	private bool _has_boats;

	protected override MetaType meta_type => MetaType.Kingdom;

	public override BaseSystemManager manager => World.world.kingdoms;

	protected override bool track_death_types => true;

	protected override AssetLibrary<KingdomTrait> trait_library => AssetManager.kingdoms_traits;

	protected override List<string> default_traits => getActorAsset().default_kingdom_traits;

	protected override List<string> saved_traits => data.saved_traits;

	[Obsolete("use .getColor() instead", false)]
	public ColorAsset kingdomColor => getColor();

	protected override void recalcBaseStats()
	{
		base.recalcBaseStats();
		_cached_tax_local = SimGlobals.m.base_tax_rate_local;
		_cached_tax_tribute = SimGlobals.m.base_tax_rate_tribute;
		foreach (KingdomTrait tTrait in getTraits())
		{
			if (tTrait.is_local_tax_trait)
			{
				_cached_tax_local = tTrait.tax_rate;
			}
			if (tTrait.is_tribute_tax_trait)
			{
				_cached_tax_tribute = tTrait.tax_rate;
			}
		}
	}

	protected sealed override void setDefaultValues()
	{
		base.setDefaultValues();
		power = 1;
		timer_action = 5f;
	}

	protected override ColorLibrary getColorLibrary()
	{
		return AssetManager.kingdom_colors_library;
	}

	public void clearListCities()
	{
		cities.Clear();
	}

	public void clearBuildingList()
	{
		buildings.Clear();
	}

	public override void increaseDeaths(AttackType pType)
	{
		if (isAlive())
		{
			base.increaseDeaths(pType);
			if (hasAlliance())
			{
				getAlliance().increaseDeaths(pType);
			}
		}
	}

	public override void increaseKills()
	{
		if (isAlive())
		{
			base.increaseKills();
			if (hasAlliance())
			{
				getAlliance().increaseKills();
			}
		}
	}

	public override void increaseBirths()
	{
		if (isAlive())
		{
			base.increaseBirths();
			if (hasAlliance())
			{
				getAlliance().increaseBirths();
			}
			addRenown(1);
		}
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

	public override bool isReadyForRemoval()
	{
		if (buildings.Count > 0)
		{
			return false;
		}
		if (getPopulationTotal() > 0)
		{
			return false;
		}
		if (hasCities())
		{
			return false;
		}
		if (World.world.projectiles.hasActiveProjectiles(this))
		{
			return false;
		}
		if (!base.isReadyForRemoval())
		{
			return false;
		}
		return true;
	}

	public bool hasBuildings()
	{
		return buildings.Count > 0;
	}

	public void addBuildings(List<Building> pListBuildings)
	{
		buildings.AddRange(pListBuildings);
	}

	public void listCity(City pCity)
	{
		cities.Add(pCity);
	}

	public void listBuilding(Building pBuilding)
	{
		buildings.Add(pBuilding);
	}

	public Subspecies getMainSubspecies()
	{
		if (hasKing())
		{
			return king.subspecies;
		}
		if (base.units.Count == 0)
		{
			return null;
		}
		return base.units[0].subspecies;
	}

	public void createWildKingdom()
	{
		asset.default_kingdom_color.initColor();
		wild = true;
	}

	public void createAI()
	{
		if (Globals.AI_TEST_ACTIVE)
		{
			if (ai == null)
			{
				ai = new AiSystemKingdom(this);
			}
			ai.next_job_delegate = getNextJob;
			ai.jobs_library = AssetManager.job_kingdom;
			ai.task_library = AssetManager.tasks_kingdom;
		}
	}

	public bool isOpinionTowardsKingdomGood(Kingdom pKingdom)
	{
		if (this == pKingdom)
		{
			return true;
		}
		if (World.world.diplomacy.getOpinion(this, pKingdom).total >= 0)
		{
			return true;
		}
		return false;
	}

	public string getNextJob()
	{
		return "kingdom";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isCiv()
	{
		return asset.civ;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isMobs()
	{
		return asset.mobs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isNeutral()
	{
		return asset.neutral;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isNature()
	{
		return asset.nature;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isNomads()
	{
		return asset.nomads;
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
		if (hasKing())
		{
			data.kingID = king.data.id;
		}
		else
		{
			data.kingID = -1L;
		}
		data.saved_traits = getTraitsAsStrings();
	}

	public IEnumerable<War> getWars(bool pRandom = false)
	{
		return World.world.wars.getWars(this, pRandom);
	}

	public bool isAttacker()
	{
		foreach (War war in getWars())
		{
			if (war.isAttacker(this))
			{
				return true;
			}
		}
		return false;
	}

	public bool isDefender()
	{
		foreach (War war in getWars())
		{
			if (war.isDefender(this))
			{
				return true;
			}
		}
		return false;
	}

	public bool isInWarWith(Kingdom pKingdom)
	{
		return World.world.wars.isInWarWith(this, pKingdom);
	}

	public bool isInWarOnSameSide(Kingdom pKingdom)
	{
		foreach (War war in getWars())
		{
			if (war.onTheSameSide(pKingdom, this))
			{
				return true;
			}
		}
		return false;
	}

	public bool isEnemy(Kingdom pKingdomTarget)
	{
		if (pKingdomTarget == null)
		{
			return true;
		}
		long tHashCode = cache_enemy_check.getHash(this, pKingdomTarget);
		if (cache_enemy_check.dict.TryGetValue(tHashCode, out var tCacheResult))
		{
			return tCacheResult;
		}
		if (isCiv() && pKingdomTarget.isCiv())
		{
			if (pKingdomTarget == this)
			{
				cache_enemy_check.dict[tHashCode] = false;
				return false;
			}
			if (World.world.wars.isInWarWith(this, pKingdomTarget))
			{
				cache_enemy_check.dict[tHashCode] = true;
				return true;
			}
			cache_enemy_check.dict[tHashCode] = false;
			return false;
		}
		if (asset.isFoe(pKingdomTarget.asset))
		{
			cache_enemy_check.dict[tHashCode] = true;
			return true;
		}
		cache_enemy_check.dict[tHashCode] = false;
		return false;
	}

	public bool isGettingCaptured()
	{
		foreach (City city in getCities())
		{
			if (city.isGettingCaptured())
			{
				return true;
			}
		}
		return false;
	}

	public override ColorAsset getColor()
	{
		if (isCiv())
		{
			return base.getColor();
		}
		return asset.default_kingdom_color;
	}

	internal void newCivKingdom(Actor pActor)
	{
		asset = AssetManager.kingdoms.get(pActor.asset.kingdom_id_civilization);
		data.original_actor_asset = pActor.asset.id;
		string tName = pActor.generateName(MetaType.Kingdom, getID());
		setName(tName);
		data.name_culture_id = culture?.id ?? (-1);
		generateNewMetaObject();
	}

	public override ActorAsset getActorAsset()
	{
		if (hasKing())
		{
			return king.getActorAsset();
		}
		return getFounderSpecies();
	}

	public ActorAsset getFounderSpecies()
	{
		return AssetManager.actor_library.get(data.original_actor_asset);
	}

	public string getSpecies()
	{
		if (string.IsNullOrEmpty(data.original_actor_asset))
		{
			return null;
		}
		return getActorAsset()?.id;
	}

	public void trySetRoyalClan()
	{
		if (hasKing() && king.hasClan() && king.clan.id != data.royal_clan_id)
		{
			long tOldClanID = data.royal_clan_id;
			Clan tOldClan = World.world.clans.get(tOldClanID);
			if (tOldClan != null && tOldClan.isAlive())
			{
				logNewRoyalClanChanged(tOldClan, king.clan);
			}
			else if (king.clan.getRenown() >= 10)
			{
				logNewRoyalClan(king.clan);
			}
			data.royal_clan_id = king.clan.id;
		}
	}

	public void checkEndWar()
	{
		data.timestamp_last_war = World.world.getCurWorldTime();
	}

	public void madePeace(War pWar)
	{
		int tRenown = (int)((float)pWar.getRenown() * 0.25f);
		addRenown(tRenown);
		foreach (Actor unit in getUnits())
		{
			unit.madePeace(pWar);
		}
		if (hasAlliance())
		{
			getAlliance().addRenown(tRenown);
		}
	}

	public void wonWar(War pWar)
	{
		addRenown(pWar.getRenown());
		foreach (Actor unit in getUnits())
		{
			unit.warWon(pWar);
		}
		if (hasAlliance())
		{
			getAlliance().addRenown(pWar.getRenown());
		}
	}

	public void lostWar(War pWar)
	{
		int tRenown = (int)((float)pWar.getRenown() * 0.1f);
		addRenown(tRenown);
		foreach (Actor unit in getUnits())
		{
			unit.warLost(pWar);
		}
		if (hasAlliance())
		{
			getAlliance().addRenown(tRenown);
		}
	}

	internal void updateCiv(float pElapsed)
	{
		if (data.timer_new_king > 0f)
		{
			data.timer_new_king -= pElapsed;
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
		}
	}

	public void setCapital(City pCity)
	{
		capital = pCity;
		if (capital != null && capital.isAlive())
		{
			KingdomData kingdomData = data;
			long capitalID = (data.last_capital_id = pCity.data.id);
			kingdomData.capitalID = capitalID;
			location = capital.city_center;
		}
		else
		{
			data.capitalID = -1L;
		}
	}

	public void setKing(Actor pActor, bool pFromLoad = false)
	{
		king = pActor;
		king.setProfession(UnitProfession.King);
		if (!pFromLoad)
		{
			data.total_kings++;
			addRuler(pActor);
			data.timestamp_king_rule = World.world.getCurWorldTime();
			king.changeHappiness("become_king");
		}
		trySetRoyalClan();
	}

	internal void kingLeftEvent()
	{
		if (hasKing())
		{
			if (king.isAlive())
			{
				king.changeHappiness("lost_crown");
			}
			logKingLeft(king);
			removeKing();
		}
	}

	internal void kingFledCity()
	{
		if (hasKing())
		{
			if (king.city.isCapitalCity())
			{
				logKingFledCapital(king);
			}
			else
			{
				logKingFledCity(king);
			}
			king.setCity(null);
		}
	}

	internal void removeKing()
	{
		if (!king.isRekt())
		{
			king.setProfession(UnitProfession.Unit);
		}
		rulerLeft();
		king = null;
		data.timer_new_king = Randy.randomFloat(5f, 20f);
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
		KingdomData kingdomData = data;
		if (kingdomData.past_rulers == null)
		{
			kingdomData.past_rulers = new List<LeaderEntry>();
		}
		rulerLeft();
		data.past_rulers.Add(new LeaderEntry
		{
			id = pActor.getID(),
			name = pActor.name,
			color_id = data.color_id,
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

	public void logKingDead(Actor pActor)
	{
		if (!pActor.attackedBy.isRekt() && pActor.attackedBy.isActor())
		{
			WorldLog.logKingMurder(this, pActor, pActor.attackedBy.a);
		}
		else
		{
			WorldLog.logKingDead(this, pActor);
		}
	}

	public void logKingFledCapital(Actor pActor)
	{
		WorldLog.logKingFledCapital(this, pActor);
	}

	public void logKingFledCity(Actor pActor)
	{
		WorldLog.logKingFledCity(this, pActor);
	}

	public void logKingLeft(Actor pActor)
	{
		WorldLog.logKingLeft(this, pActor);
	}

	public void logNewRoyalClanChanged(Clan pOldClan, Clan pNewClan)
	{
		WorldLog.logRoyalClanChanged(this, pOldClan, pNewClan);
	}

	public void logNewRoyalClan(Clan pClan)
	{
		WorldLog.logRoyalClanNew(this, pClan);
	}

	public void logRoyalClanLost(Clan pClan)
	{
		WorldLog.logRoyalClanNoMore(this, pClan);
	}

	internal void checkClearCapital(City pCity)
	{
		if (pCity.isCapitalCity())
		{
			clearCapital();
		}
	}

	public void clearCapital()
	{
		data.capitalID = -1L;
		capital = null;
	}

	public bool hasNearbyKingdoms()
	{
		foreach (City city in getCities())
		{
			if (city.neighbours_kingdoms.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void capturedFrom(Kingdom pKingdom)
	{
		World.world.diplomacy.getRelation(this, pKingdom);
	}

	public virtual string getMotto()
	{
		if (string.IsNullOrEmpty(data.motto))
		{
			data.motto = NameGenerator.getName("kingdom_mottos");
		}
		return data.motto;
	}

	public override void generateBanner()
	{
		BannerAsset tAsset = AssetManager.kingdom_banners_library.get(getActorAsset().banner_id);
		data.banner_icon_id = Randy.randomInt(0, tAsset.icons.Count);
		data.banner_background_id = Randy.randomInt(0, tAsset.backgrounds.Count);
	}

	public override void loadData(KingdomData pData)
	{
		base.loadData(pData);
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
		ActorAsset tAsset = getActorAsset();
		asset = AssetManager.kingdoms.get(tAsset.kingdom_id_civilization);
	}

	internal void load2()
	{
		City tLoadedCapital = World.world.cities.get(data.capitalID);
		if (tLoadedCapital != null)
		{
			setCapital(tLoadedCapital);
		}
		if (data.kingID.hasValue())
		{
			Actor tLoadedUnitForKing = World.world.units.get(data.kingID);
			if (tLoadedUnitForKing != null)
			{
				setKing(tLoadedUnitForKing, pFromLoad: true);
				tLoadedUnitForKing.setProfession(UnitProfession.King);
			}
		}
	}

	public override bool updateColor(ColorAsset pColor)
	{
		bool tResult = base.updateColor(pColor);
		if (tResult)
		{
			foreach (Building building in buildings)
			{
				building.updateKingdomColors();
			}
		}
		return tResult;
	}

	public static float distanceBetweenKingdom(Kingdom pKingdom, Kingdom pTarget)
	{
		if (!pKingdom.hasCities() || !pTarget.hasCities())
		{
			return -1f;
		}
		float tBestFastDist = float.MaxValue;
		using ListPool<Vector2> tKingdomCenters = new ListPool<Vector2>();
		using ListPool<Vector2> tTargetCenters = new ListPool<Vector2>();
		foreach (City tCity in pKingdom.getCities())
		{
			tKingdomCenters.Add(tCity.city_center);
		}
		foreach (City tCity2 in pTarget.getCities())
		{
			tTargetCenters.Add(tCity2.city_center);
		}
		foreach (ref Vector2 item in tKingdomCenters)
		{
			Vector2 tCity3 = item;
			foreach (ref Vector2 item2 in tTargetCenters)
			{
				Vector2 tCity4 = item2;
				float tFastDist = Toolbox.SquaredDistVec2Float(tCity3, tCity4);
				if (tFastDist < tBestFastDist)
				{
					tBestFastDist = tFastDist;
				}
			}
		}
		return tBestFastDist;
	}

	public override IEnumerable<City> getCities()
	{
		if (World.world.kingdoms.hasDirtyCities())
		{
			foreach (City tCity in World.world.cities)
			{
				if (!tCity.isRekt() && tCity.kingdom == this)
				{
					yield return tCity;
				}
			}
			yield break;
		}
		foreach (City tCity2 in cities)
		{
			if (!tCity2.isRekt())
			{
				yield return tCity2;
			}
		}
	}

	public void clear()
	{
		buildings.Clear();
		cities.Clear();
		base.units.Clear();
		cache_enemy_check.clear();
		clearCapital();
	}

	public override void Dispose()
	{
		DBInserter.deleteData(getID(), "kingdom");
		clear();
		asset = null;
		king = null;
		capital = null;
		culture = null;
		language = null;
		religion = null;
		ai?.reset();
		base.Dispose();
	}

	public bool hasEnemies()
	{
		return World.world.wars.hasWars(this);
	}

	public ListPool<Kingdom> getEnemiesKingdoms()
	{
		return World.world.wars.getEnemiesOf(this);
	}

	public void makeSurvivorsToNomads()
	{
		if (base.units.Count == 0)
		{
			return;
		}
		for (int i = 0; i < base.units.Count; i++)
		{
			Actor tActor = base.units[i];
			if (tActor.isAlive())
			{
				if (tActor.asset.is_boat)
				{
					tActor.getHitFullHealth(AttackType.None);
					continue;
				}
				tActor.cancelAllBeh();
				tActor.removeFromPreviousFaction();
				tActor.joinKingdom(World.world.kingdoms_wild.get(tActor.asset.kingdom_id_wild));
			}
		}
		base.units.Clear();
	}

	public void clearKingData()
	{
		king = null;
	}

	public void updateAge()
	{
		if (hasKing() && king.hasClan())
		{
			king.clan.addRenown(1);
		}
	}

	public override int countCouples()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countCouples();
		}
		return tResult;
	}

	public override int countSingleMales()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countSingleMales();
		}
		return tResult;
	}

	public override int countSingleFemales()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countSingleFemales();
		}
		return tResult;
	}

	public int countZones()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countZones();
		}
		return tResult;
	}

	public int countBuildings()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countBuildings();
		}
		return tResult;
	}

	public int countCities()
	{
		if (!World.world.kingdoms.hasDirtyCities())
		{
			return cities.Count;
		}
		int tResult = 0;
		foreach (City city in getCities())
		{
			_ = city;
			tResult++;
		}
		return tResult;
	}

	public override int getPopulationPeople()
	{
		if (!_has_boats)
		{
			return base.units.Count;
		}
		int tResult = 0;
		int tBoats = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.getPopulationPeople();
			tBoats += tCity.countBoats();
		}
		if (tResult + tBoats == base.units.Count)
		{
			return tResult;
		}
		tResult = 0;
		foreach (Actor unit in getUnits())
		{
			if (!unit.asset.is_boat)
			{
				tResult++;
			}
		}
		return tResult;
	}

	public override int countUnits()
	{
		return getPopulationPeople();
	}

	public override IEnumerable<Actor> getUnits()
	{
		foreach (Actor tActor in base.units)
		{
			if (tActor.isAlive() && !tActor.asset.is_boat && tActor.kingdom == this)
			{
				yield return tActor;
			}
		}
	}

	public override Actor getRandomUnit()
	{
		foreach (Actor tActor in base.units.LoopRandom())
		{
			if (tActor.isAlive() && !tActor.asset.is_boat && tActor.kingdom == this)
			{
				return tActor;
			}
		}
		return null;
	}

	public int getPopulationTotal()
	{
		return base.units.Count;
	}

	public int countBoats()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countBoats();
		}
		return tResult;
	}

	public int getPopulationTotalPossible()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.getPopulationMaximum();
		}
		return tResult;
	}

	public int countWeapons()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countWeapons();
		}
		return tResult;
	}

	public int countTotalFood()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.getTotalFood();
		}
		return tResult;
	}

	public int countTotalWarriors()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.countWarriors();
		}
		return tResult;
	}

	public int countWarriorsMax()
	{
		int tResult = 0;
		foreach (City tCity in getCities())
		{
			tResult += tCity.getMaxWarriors();
		}
		return tResult;
	}

	public int getMaxCities()
	{
		int tResult = getActorAsset().civ_base_cities;
		if (hasKing())
		{
			tResult += (int)king.stats["cities"];
		}
		if (tResult < 1)
		{
			tResult = 1;
		}
		return tResult;
	}

	public bool diceAgressionSuccess()
	{
		if (!hasKing())
		{
			return false;
		}
		int tCountCities = countCities();
		if (tCountCities < getMaxCities())
		{
			return true;
		}
		if (tCountCities >= getMaxCities() && Randy.randomChance(king.stats["personality_aggression"]))
		{
			return true;
		}
		return false;
	}

	public bool isSupreme()
	{
		return DiplomacyManager.kingdom_supreme == this;
	}

	public bool isSecondBest()
	{
		return DiplomacyManager.kingdom_second == this;
	}

	public bool hasAlliance()
	{
		return getAlliance() != null;
	}

	public Alliance getAlliance()
	{
		if (!data.allianceID.hasValue())
		{
			return null;
		}
		Alliance alliance = World.world.alliances.get(data.allianceID);
		if (alliance == null)
		{
			data.allianceID = -1L;
		}
		return alliance;
	}

	public void allianceLeave(Alliance pAlliance)
	{
		data.allianceID = -1L;
		data.timestamp_alliance = World.world.getCurWorldTime();
	}

	public void allianceJoin(Alliance pAlliance)
	{
		data.allianceID = pAlliance.data.id;
		data.timestamp_alliance = World.world.getCurWorldTime();
	}

	public void calculateNeighbourCities()
	{
		foreach (City city in getCities())
		{
			city.recalculateNeighbourCities();
		}
	}

	public Culture getCulture()
	{
		return culture;
	}

	public void setCulture(Culture pCulture)
	{
		if (culture != pCulture)
		{
			culture = pCulture;
			World.world.cultures.setDirtyKingdoms();
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

	public void setLanguage(Language pLanguage)
	{
		language = pLanguage;
		World.world.languages.setDirtyKingdoms();
	}

	public Language getLanguage()
	{
		return language;
	}

	public bool hasLanguage()
	{
		if (language != null && !language.isAlive())
		{
			setLanguage(null);
		}
		return language != null;
	}

	public void setReligion(Religion pReligion)
	{
		if (religion != pReligion)
		{
			religion = pReligion;
			World.world.religions.setDirtyKingdoms();
		}
	}

	public Religion getReligion()
	{
		return religion;
	}

	public bool hasReligion()
	{
		if (religion != null && !religion.isAlive())
		{
			setReligion(null);
		}
		return religion != null;
	}

	public bool isEnemyAroundZone(TileZone pZone)
	{
		TileZone[] neighbours = pZone.neighbours;
		foreach (TileZone tZone in neighbours)
		{
			if (tZone.city == null)
			{
				return true;
			}
			Kingdom tKingdom = tZone.city.kingdom;
			if (tKingdom != this)
			{
				return true;
			}
			if (tKingdom != this && tKingdom.isEnemy(this))
			{
				return true;
			}
		}
		return false;
	}

	public override bool hasCities()
	{
		using (IEnumerator<City> enumerator = getCities().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				return true;
			}
		}
		return false;
	}

	public bool hasCapital()
	{
		return capital != null;
	}

	public bool hasKing()
	{
		if (king == null)
		{
			return false;
		}
		if (!king.isAlive())
		{
			removeKing();
			return false;
		}
		return true;
	}

	public void affectKingByPowers()
	{
		if (hasKing())
		{
			king.addStatusEffect("voices_in_my_head");
		}
	}

	public int countUnhappyCities()
	{
		int tResult = 0;
		foreach (City city in getCities())
		{
			if (!city.isHappy())
			{
				tResult++;
			}
		}
		return tResult;
	}

	public Sprite getSpeciesIcon()
	{
		return getActorAsset().getSpriteIcon();
	}

	public Sprite getElementIcon()
	{
		return AssetManager.kingdom_banners_library.getSpriteIcon(data.banner_icon_id, getActorAsset().banner_id);
	}

	public Sprite getElementBackground()
	{
		return AssetManager.kingdom_banners_library.getSpriteBackground(data.banner_background_id, getActorAsset().banner_id);
	}

	public void increaseHappinessFromNewCityCapture()
	{
		foreach (Actor tActor in getUnits())
		{
			if (!tActor.hasHappinessEntry("was_conquered", 400f))
			{
				tActor.changeHappiness("conquered_city");
			}
		}
	}

	public void increaseHappinessFromDestroyingCity()
	{
		foreach (Actor tActor in getUnits())
		{
			if (!tActor.hasHappinessEntry("was_conquered", 400f))
			{
				tActor.changeHappiness("destroyed_city");
			}
		}
	}

	public void decreaseHappinessFromLostCityCapture(City pCity)
	{
		foreach (Actor tActor in base.units)
		{
			if (!tActor.hasHappinessEntry("was_conquered", 400f))
			{
				if (pCity.isCapitalCity())
				{
					tActor.changeHappiness("lost_capital");
				}
				else
				{
					tActor.changeHappiness("lost_city");
				}
			}
		}
	}

	public void decreaseHappinessFromRazedCity(City pCity)
	{
		foreach (Actor tActor in base.units)
		{
			if (!tActor.hasHappinessEntry("was_conquered", 400f))
			{
				if (pCity.isCapitalCity())
				{
					tActor.changeHappiness("razed_capital");
				}
				else
				{
					tActor.changeHappiness("razed_city");
				}
			}
		}
	}

	public int getLootMin()
	{
		return 5;
	}

	public float getTaxRateTribute()
	{
		return _cached_tax_tribute;
	}

	public float getTaxRateLocal()
	{
		return _cached_tax_local;
	}

	public void copyMetasFromOtherKingdom(Kingdom pKingdom)
	{
		if (pKingdom.hasCulture())
		{
			setCulture(pKingdom.culture);
		}
		if (pKingdom.hasLanguage())
		{
			setLanguage(pKingdom.language);
		}
		if (pKingdom.hasReligion())
		{
			setReligion(pKingdom.religion);
		}
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

	public void setCityMetas(City pCity)
	{
		if (pCity.hasCulture())
		{
			setCulture(pCity.culture);
		}
		if (pCity.hasLanguage())
		{
			setLanguage(pCity.language);
		}
		if (pCity.hasReligion())
		{
			setReligion(pCity.religion);
		}
	}

	public Clan getKingClan()
	{
		if (hasKing() && king.hasClan())
		{
			return king.clan;
		}
		return null;
	}

	public override void listUnit(Actor pActor)
	{
		if (pActor.asset.is_boat)
		{
			_has_boats = true;
		}
		base.listUnit(pActor);
	}

	internal override void clearListUnits()
	{
		_has_boats = false;
		base.clearListUnits();
	}

	public override string ToString()
	{
		if (data == null)
		{
			return "[Kingdom is null]";
		}
		using StringBuilderPool tBuilder = new StringBuilderPool();
		tBuilder.Append($"[Kingdom:{base.id} ");
		if (!isAlive())
		{
			tBuilder.Append("[DEAD] ");
		}
		tBuilder.Append("\"" + name + "\" ");
		tBuilder.Append($"Cities:{cities.Count} ");
		if (World.world.kingdoms.hasDirtyCities())
		{
			tBuilder.Append($" [Dirty:{countCities()}] ");
		}
		tBuilder.Append($"Units:{base.units.Count} ");
		if (isDirtyUnits())
		{
			tBuilder.Append("[Dirty] ");
		}
		if (hasKing())
		{
			tBuilder.Append($"King:{king.id} ");
		}
		return tBuilder.ToString().Trim() + "]";
	}
}
