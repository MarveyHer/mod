using System;
using System.Collections.Generic;
using db;
using UnityEngine;

public class Culture : MetaObjectWithTraits<CultureData, CultureTrait>
{
	public const int MAX_LEVEL = 5;

	public readonly List<City> cities = new List<City>();

	public readonly List<Kingdom> kingdoms = new List<Kingdom>();

	public readonly BooksHandler books = new BooksHandler();

	private readonly List<string> _preferred_weapons_craft_subtypes = new List<string>();

	private readonly List<EquipmentAsset> _preferred_weapons_craft_assets = new List<EquipmentAsset>();

	private NameSetAsset _name_set_asset;

	private readonly List<CultureTrait> _traits_town_plan_zones = new List<CultureTrait>();

	private readonly Dictionary<MetaType, OnomasticsData> _onomastics_data = new Dictionary<MetaType, OnomasticsData>();

	protected override MetaType meta_type => MetaType.Culture;

	public override BaseSystemManager manager => World.world.cultures;

	protected override AssetLibrary<CultureTrait> trait_library => AssetManager.culture_traits;

	protected override List<string> default_traits => getActorAsset().default_culture_traits;

	protected override List<string> saved_traits => data.saved_traits;

	protected override string species_id => data.original_actor_asset;

	public void createCulture(Actor pActor, bool pAddDefaultTraits)
	{
		string tName = pActor.generateName(MetaType.Culture, getID());
		setName(tName);
		data.name_culture_id = pActor.culture?.getID() ?? (-1);
		data.original_actor_asset = pActor.asset.id;
		data.creator_city_name = pActor.city?.name;
		data.creator_city_id = pActor.city?.getID() ?? (-1);
		data.creator_name = pActor.getName();
		data.creator_id = pActor.getID();
		data.creator_species_id = pActor.asset.id;
		data.creator_subspecies_name = pActor.subspecies.name;
		data.creator_subspecies_id = pActor.subspecies.getID();
		data.creator_clan_name = pActor.clan?.name;
		data.creator_clan_id = pActor.clan?.getID() ?? (-1);
		data.creator_kingdom_id = pActor.kingdom?.getID() ?? (-1);
		data.creator_kingdom_name = pActor.kingdom?.name;
		data.name_template_set = pActor.asset.name_template_sets.GetRandom();
		books.setMeta(this);
		generateNewMetaObject(pAddDefaultTraits);
		setDirty();
	}

	public void cloneAndEvolveOnomastics(Culture pFrom)
	{
		CultureData tCultureData = pFrom.data;
		data.parent_culture_id = pFrom.id;
		data.name_template_set = tCultureData.name_template_set;
		pFrom.loadAllOnomasticsData();
		_onomastics_data.Clear();
		foreach (KeyValuePair<MetaType, OnomasticsData> tPair in pFrom._onomastics_data)
		{
			OnomasticsData tCopy = new OnomasticsData();
			tCopy.loadFromShortTemplate(tPair.Value.getShortTemplate());
			OnomasticsEvolver.scramble(tCopy);
			_onomastics_data.Add(tPair.Key, tCopy);
		}
	}

	protected override void recalcBaseStats()
	{
		base.recalcBaseStats();
		recalcPreferredWeapons();
		recalcTownPlanTraits();
	}

	private void recalcTownPlanTraits()
	{
		_traits_town_plan_zones.Clear();
		foreach (CultureTrait tTrait in getTraits())
		{
			if (tTrait.town_layout_plan)
			{
				_traits_town_plan_zones.Add(tTrait);
			}
		}
	}

	public void countConversion()
	{
		addRenown(1);
	}

	protected sealed override void setDefaultValues()
	{
		base.setDefaultValues();
	}

	public override void increaseBirths()
	{
		throw new NotImplementedException(GetType().Name);
	}

	public int countCities()
	{
		return cities.Count;
	}

	public int countKingdoms()
	{
		return kingdoms.Count;
	}

	protected override ColorLibrary getColorLibrary()
	{
		return AssetManager.culture_colors_library;
	}

	public bool canUseRoads()
	{
		if (hasTrait("roads"))
		{
			return true;
		}
		return false;
	}

	public void testDebugNewBook()
	{
		if (base.units.Count != 0)
		{
			Actor tActor = base.units.GetRandom();
			if (tActor.getCity() != null && tActor.city.hasBookSlots())
			{
				World.world.books.generateNewBook(tActor);
			}
		}
	}

	private void recalcPreferredWeapons()
	{
		_preferred_weapons_craft_subtypes.Clear();
		_preferred_weapons_craft_assets.Clear();
		foreach (CultureTrait tTrait in getTraits())
		{
			if (!tTrait.is_weapon_trait)
			{
				continue;
			}
			if (tTrait.related_weapon_subtype_ids != null)
			{
				foreach (string tSubtypeID in tTrait.related_weapon_subtype_ids)
				{
					_preferred_weapons_craft_subtypes.Add(tSubtypeID);
				}
			}
			if (tTrait.related_weapons_ids == null)
			{
				continue;
			}
			foreach (string tWeaponID in tTrait.related_weapons_ids)
			{
				EquipmentAsset tAsset = AssetManager.items.get(tWeaponID);
				if (tAsset != null)
				{
					_preferred_weapons_craft_assets.Add(tAsset);
				}
			}
		}
	}

	public bool hasPreferredWeaponsToCraft()
	{
		return _preferred_weapons_craft_assets.Count > 0;
	}

	public List<EquipmentAsset> getPreferredWeaponAssets()
	{
		return _preferred_weapons_craft_assets;
	}

	public string getPreferredWeaponSubtypeIDs()
	{
		if (_preferred_weapons_craft_subtypes.Count == 0)
		{
			return null;
		}
		return _preferred_weapons_craft_subtypes.GetRandom();
	}

	public float chanceToGiveTraits()
	{
		return 0.5f;
	}

	public void clearListCities()
	{
		cities.Clear();
	}

	public void clearListKingdoms()
	{
		kingdoms.Clear();
	}

	public void listCity(City pCity)
	{
		cities.Add(pCity);
	}

	public void listKingdom(Kingdom pKingdom)
	{
		kingdoms.Add(pKingdom);
	}

	public override void generateBanner()
	{
		data.banner_decor_id = AssetManager.culture_banners_library.getNewIndexBackground();
		data.banner_element_id = AssetManager.culture_banners_library.getNewIndexIcon();
	}

	public override void save()
	{
		base.save();
		data.saved_traits = getTraitsAsStrings();
		if (_onomastics_data.Count > 0)
		{
			data.onomastics = new Dictionary<MetaType, string>(_onomastics_data.Count);
			{
				foreach (MetaType tType in _onomastics_data.Keys)
				{
					OnomasticsData tData = _onomastics_data[tType];
					data.onomastics[tType] = tData.getShortTemplate();
				}
				return;
			}
		}
		data.onomastics?.Clear();
		data.onomastics = null;
	}

	public override void loadData(CultureData pData)
	{
		base.loadData(pData);
		if (!string.IsNullOrEmpty(data.name_template_set) && !AssetManager.name_sets.has(data.name_template_set))
		{
			data.name_template_set = null;
			_name_set_asset = null;
		}
		books.setDirty();
		books.setMeta(this);
		_onomastics_data.Clear();
		if (pData.onomastics == null)
		{
			return;
		}
		foreach (KeyValuePair<MetaType, string> tPair in pData.onomastics)
		{
			OnomasticsData tData = new OnomasticsData();
			tData.loadFromShortTemplate(tPair.Value);
			_onomastics_data.Add(tPair.Key, tData);
		}
	}

	public override void updateDirty()
	{
		foreach (City city in cities)
		{
			city.setStatusDirty();
		}
	}

	public void debug(DebugTool pTool)
	{
		pTool.setText("id:", base.id, 0f, pShowBar: false, 0L);
		pTool.setText("name:", name, 0f, pShowBar: false, 0L);
		pTool.setText("followers:", countUnits(), 0f, pShowBar: false, 0L);
		pTool.setText("cities:", countCities(), 0f, pShowBar: false, 0L);
	}

	internal void updateTitleCenter()
	{
	}

	public Sprite getElementSprite()
	{
		return AssetManager.culture_banners_library.getSpriteIcon(data.banner_element_id);
	}

	public Sprite getDecorSprite()
	{
		return AssetManager.culture_banners_library.getSpriteBackground(data.banner_decor_id);
	}

	public List<long> getBooks()
	{
		return books.getList();
	}

	public override bool isReadyForRemoval()
	{
		if (books.hasBooks())
		{
			return false;
		}
		return base.isReadyForRemoval();
	}

	public override void convertSameSpeciesAroundUnit(Actor pActorMain, bool pOverrideExisting = false)
	{
		foreach (Actor tActor in getUnitFromChunkForConversion(pActorMain))
		{
			if (pOverrideExisting || !tActor.hasCulture())
			{
				tActor.setCulture(this);
			}
		}
	}

	public override void forceConvertSameSpeciesAroundUnit(Actor pActorMain)
	{
		convertSameSpeciesAroundUnit(pActorMain, pOverrideExisting: true);
	}

	public override void Dispose()
	{
		DBInserter.deleteData(getID(), "culture");
		books.clear();
		_preferred_weapons_craft_subtypes.Clear();
		_preferred_weapons_craft_assets.Clear();
		cities.Clear();
		kingdoms.Clear();
		_traits_town_plan_zones.Clear();
		_name_set_asset = null;
		_onomastics_data.Clear();
		base.Dispose();
	}

	public string getNameTemplate(MetaType pType)
	{
		if (_name_set_asset == null)
		{
			if (string.IsNullOrEmpty(data.name_template_set))
			{
				data.name_template_set = getActorAsset().name_template_sets.GetRandom();
			}
			if (string.IsNullOrEmpty(data.name_template_set))
			{
				_name_set_asset = null;
				return AssetManager.name_generator.get(getActorAsset().getNameTemplate(pType)).id;
			}
		}
		_name_set_asset = AssetManager.name_sets.get(data.name_template_set);
		return _name_set_asset.get(pType);
	}

	public void loadAllOnomasticsData()
	{
		foreach (MetaType tType in NameSetAsset.getTypes())
		{
			getOnomasticData(tType);
		}
	}

	public OnomasticsData getOnomasticData(MetaType pType, bool pReset = false)
	{
		if (!_onomastics_data.TryGetValue(pType, out var tData) || pReset)
		{
			if (tData == null)
			{
				tData = new OnomasticsData();
			}
			else
			{
				tData.clearTemplateData();
			}
			string tNameTemplate = getNameTemplate(pType);
			if (!string.IsNullOrEmpty(tNameTemplate))
			{
				NameGeneratorAsset tNameGeneratorAsset = AssetManager.name_generator.get(tNameTemplate);
				if (tNameGeneratorAsset.hasOnomastics())
				{
					OnomasticsData tOriginalData = OnomasticsCache.getOriginalData(tNameGeneratorAsset.onomastics_templates.GetRandom());
					tData.cloneFrom(tOriginalData);
				}
				else
				{
					Debug.Log($"no onomastics data found for {tNameGeneratorAsset.id} for {pType}");
				}
			}
			if (tData.isEmpty())
			{
				Debug.Log("name set asset " + _name_set_asset.id + " doesn't have " + pType);
				Debug.Log("loading from actor");
				string tActorNameTemplate = getActorAsset().getNameTemplate(pType);
				NameGeneratorAsset tNameGeneratorAsset2 = AssetManager.name_generator.get(tActorNameTemplate);
				if (tNameGeneratorAsset2.hasOnomastics())
				{
					OnomasticsData tOriginalData2 = OnomasticsCache.getOriginalData(tNameGeneratorAsset2.onomastics_templates.GetRandom());
					tData.cloneFrom(tOriginalData2);
				}
				else
				{
					Debug.Log($"no onomastics data found for {tNameGeneratorAsset2.id} for {pType}");
				}
			}
			if (tData.isEmpty())
			{
				tData.setDebugTest();
				Debug.Log($"no onomastics data found for {_name_set_asset.id} for {pType}, defaulting");
			}
			_onomastics_data[pType] = tData;
		}
		return tData;
	}

	public bool planAllowsToPlaceBuildingInZone(TileZone pZone, TileZone pCenterZone)
	{
		foreach (CultureTrait traits_town_plan_zone in _traits_town_plan_zones)
		{
			if (!traits_town_plan_zone.passable_zone_checker(pZone, pCenterZone))
			{
				return false;
			}
		}
		return true;
	}

	public bool hasSpecialTownPlans()
	{
		return _traits_town_plan_zones.Count > 0;
	}

	public bool hasTrueRoots()
	{
		return hasTrait("true_roots");
	}

	public bool isPossibleToConvertToOtherMeta()
	{
		return !hasTrueRoots();
	}

	public override bool hasCities()
	{
		return cities.Count > 0;
	}

	public override IEnumerable<City> getCities()
	{
		return cities;
	}

	public override bool hasKingdoms()
	{
		return kingdoms.Count > 0;
	}

	public override IEnumerable<Kingdom> getKingdoms()
	{
		return kingdoms;
	}
}
