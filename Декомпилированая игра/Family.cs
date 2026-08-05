using System.Collections.Generic;
using db;
using UnityEngine;

public class Family : MetaObject<FamilyData>, ISapient
{
	private Actor _cached_alpha;

	private ActorAsset _cached_species;

	private Actor _founder_1;

	private Actor _founder_2;

	private bool _founders_dirty;

	private double _timestamp_hungry_check;

	private bool _cached_hungry_check_result;

	protected override MetaType meta_type => MetaType.Family;

	public override BaseSystemManager manager => World.world.families;

	public void newFamily(Actor pActor1, Actor pActor2, WorldTile pTile)
	{
		data.species_id = pActor1.asset.id;
		generateNewMetaObject();
		data.founder_actor_name_1 = pActor1.getName();
		data.founder_actor_name_2 = pActor2?.getName();
		if (pActor1.hasSubspecies())
		{
			data.subspecies_id = pActor1.subspecies.id;
			data.subspecies_name = pActor1.subspecies.name;
		}
		data.founder_city_id = pActor1.city?.getID() ?? (-1);
		data.founder_city_name = pActor1.city?.name;
		if (pActor1.kingdom.isCiv())
		{
			data.founder_kingdom_id = pActor1.kingdom.getID();
			data.founder_kingdom_name = pActor1.kingdom.data.name;
		}
		data.main_founder_id_1 = pActor1.getID();
		if (pActor2 != null)
		{
			data.main_founder_id_2 = pActor2.getID();
		}
		generateName(pActor1);
	}

	public bool areMostUnitsHungry()
	{
		if (World.world.getWorldTimeElapsedSince(_timestamp_hungry_check) < 5f)
		{
			return _cached_hungry_check_result;
		}
		if (countHungry() >= countUnits() / 2)
		{
			_cached_hungry_check_result = true;
		}
		else
		{
			_cached_hungry_check_result = false;
		}
		_timestamp_hungry_check = World.world.getCurWorldTime();
		return _cached_hungry_check_result;
	}

	public bool isFull()
	{
		return base.units.Count > getActorAsset().family_limit;
	}

	public bool isAlpha(Actor pActor)
	{
		Actor tAlpha = _cached_alpha;
		return pActor == tAlpha;
	}

	private void removeAlpha()
	{
		data.alpha_id = -1L;
		_cached_alpha = null;
	}

	public void checkAlpha()
	{
		if (_cached_alpha != null)
		{
			if (!_cached_alpha.isAlive())
			{
				removeAlpha();
			}
			else if (_cached_alpha.family != this)
			{
				removeAlpha();
			}
		}
		if (_cached_alpha == null)
		{
			Actor tActor = findAlpha();
			if (tActor != null)
			{
				setAlpha(tActor, pNew: true);
			}
		}
	}

	public Actor getAlpha()
	{
		return _cached_alpha;
	}

	public Actor findAlpha()
	{
		Actor tResult = null;
		double tBestTime = double.MaxValue;
		for (int i = 0; i < base.units.Count; i++)
		{
			Actor tActor = base.units[i];
			if (tActor.isAlive() && !(tActor.data.created_time > tBestTime))
			{
				tBestTime = tActor.data.created_time;
				tResult = tActor;
			}
		}
		return tResult;
	}

	private void setAlpha(Actor pActor, bool pNew)
	{
		data.alpha_id = pActor.getID();
		if (pNew)
		{
			pActor.changeHappiness("become_alpha");
		}
		_cached_alpha = pActor;
	}

	public void saveOriginFamily1(long pFamilyID)
	{
		data.original_family_1 = pFamilyID;
	}

	public void saveOriginFamily2(long pFamilyID)
	{
		if (pFamilyID != data.original_family_1)
		{
			data.original_family_2 = pFamilyID;
		}
	}

	public IEnumerable<Family> getOriginFamilies()
	{
		if (data.original_family_1.hasValue())
		{
			Family tFamily1 = World.world.families.get(data.original_family_1);
			if (!tFamily1.isRekt())
			{
				yield return tFamily1;
			}
		}
		if (data.original_family_2.hasValue())
		{
			Family tFamily2 = World.world.families.get(data.original_family_2);
			if (!tFamily2.isRekt())
			{
				yield return tFamily2;
			}
		}
	}

	public override void generateBanner()
	{
		data.banner_background_id = AssetManager.family_banners_library.getNewIndexBackground();
		ActorAsset tActorAsset = getActorAsset();
		int tIndex;
		if (tActorAsset.family_banner_frame_only_inclusion)
		{
			tIndex = AssetManager.family_banners_library.main.frames.IndexOf(tActorAsset.family_banner_frame_generation_inclusion);
		}
		else
		{
			using ListPool<string> tFrames = new ListPool<string>(AssetManager.family_banners_library.main.frames);
			string tExclusion = tActorAsset.family_banner_frame_generation_exclusion;
			if (!string.IsNullOrEmpty(tExclusion))
			{
				tFrames.Remove(tExclusion);
			}
			string tInclusion = tActorAsset.family_banner_frame_generation_exclusion;
			if (!string.IsNullOrEmpty(tInclusion))
			{
				tFrames.Remove(tInclusion);
			}
			string tFrame = tFrames.GetRandom();
			tIndex = AssetManager.family_banners_library.main.frames.IndexOf(tFrame);
		}
		data.banner_frame_id = tIndex;
	}

	public Sprite getSpriteBackground()
	{
		return AssetManager.family_banners_library.getSpriteBackground(data.banner_background_id);
	}

	public Sprite getSpriteFrame()
	{
		return AssetManager.family_banners_library.getSpriteFrame(data.banner_frame_id);
	}

	public override ActorAsset getActorAsset()
	{
		if (_cached_species == null)
		{
			_cached_species = AssetManager.actor_library.get(data.species_id);
		}
		return _cached_species;
	}

	public bool isSapient()
	{
		return World.world.subspecies.get(data.subspecies_id)?.isSapient() ?? false;
	}

	public bool isMainFounder(Actor pActor)
	{
		if (pActor.data.id == data.main_founder_id_1)
		{
			return true;
		}
		if (pActor.data.id == data.main_founder_id_2)
		{
			return true;
		}
		return false;
	}

	public bool hasFounders()
	{
		checkFounders();
		if (_founder_1 == null)
		{
			return _founder_2 != null;
		}
		return true;
	}

	public Actor getRandomFounder()
	{
		checkFounders();
		if (!hasFounders())
		{
			return null;
		}
		if (_founder_1 != null && _founder_2 == null)
		{
			return _founder_1;
		}
		if (_founder_2 != null && _founder_1 == null)
		{
			return _founder_2;
		}
		if (Randy.randomBool())
		{
			return _founder_1;
		}
		return _founder_2;
	}

	public Actor getFounderFirst()
	{
		return _founder_1;
	}

	public Actor getFounderSecond()
	{
		return _founder_2;
	}

	private void checkFounders()
	{
		if (!_founders_dirty)
		{
			return;
		}
		clearFounders();
		_founders_dirty = false;
		foreach (Actor tActor in base.units)
		{
			if (tActor.data.id == data.main_founder_id_1)
			{
				_founder_1 = tActor;
			}
			if (tActor.data.id == data.main_founder_id_2)
			{
				_founder_2 = tActor;
			}
		}
	}

	private void clearFounders()
	{
		_founder_1 = null;
		_founder_2 = null;
		_founders_dirty = true;
	}

	protected override ColorLibrary getColorLibrary()
	{
		return AssetManager.families_colors_library;
	}

	public bool isSameSpecies(string pAssetID)
	{
		return data.species_id == pAssetID;
	}

	private void generateName(Actor pActor)
	{
		string tName = pActor.generateName(MetaType.Family, getID());
		setName(tName);
		data.name_culture_id = pActor.culture?.getID() ?? (-1);
	}

	public override void loadData(FamilyData pData)
	{
		base.loadData(pData);
		if (data.alpha_id.hasValue())
		{
			Actor tLoadedAlpha = World.world.units.get(data.alpha_id);
			if (tLoadedAlpha != null)
			{
				setAlpha(tLoadedAlpha, pNew: false);
			}
		}
	}

	public override void updateDirty()
	{
		base.updateDirty();
		clearFounders();
	}

	public override void Dispose()
	{
		DBInserter.deleteData(getID(), "family");
		_cached_alpha = null;
		_cached_species = null;
		_timestamp_hungry_check = 0.0;
		_cached_hungry_check_result = false;
		_founder_1 = null;
		_founder_2 = null;
		_founders_dirty = true;
		base.Dispose();
	}
}
