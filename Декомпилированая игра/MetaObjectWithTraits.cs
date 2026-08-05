using System;
using System.Collections.Generic;
using UnityEngine;

public class MetaObjectWithTraits<TData, TBaseTrait> : MetaObject<TData>, ITraitsOwner<TBaseTrait> where TData : MetaObjectData where TBaseTrait : BaseTrait<TBaseTrait>
{
	private readonly HashSet<TBaseTrait> _traits = new HashSet<TBaseTrait>();

	public readonly BaseStats base_stats = new BaseStats();

	public readonly BaseStats base_stats_meta = new BaseStats();

	private ActorAsset _species_asset;

	public readonly List<BaseAugmentationAsset> all_actions_actor_special_effect = new List<BaseAugmentationAsset>();

	public AttackAction all_actions_actor_attack_target;

	public GetHitAction all_actions_actor_get_hit;

	public WorldAction all_actions_actor_death;

	public WorldAction all_actions_actor_growth;

	public WorldAction all_actions_actor_birth;

	public readonly List<DecisionAsset> decisions_assets = new List<DecisionAsset>();

	public readonly CombatActionHolder combat_actions = new CombatActionHolder();

	public readonly SpellHolder spells = new SpellHolder();

	protected virtual AssetLibrary<TBaseTrait> trait_library
	{
		get
		{
			throw new NotImplementedException(GetType().Name);
		}
	}

	protected virtual List<string> default_traits => null;

	protected virtual List<string> saved_traits => null;

	protected virtual string species_id => "human";

	public override void loadData(TData pData)
	{
		base.loadData(pData);
		loadTraits();
	}

	private void resetStatsAndCallbacks()
	{
		all_actions_actor_death = null;
		all_actions_actor_growth = null;
		all_actions_actor_birth = null;
		all_actions_actor_attack_target = null;
		all_actions_actor_get_hit = null;
		all_actions_actor_special_effect.Clear();
		base_stats.clear();
		base_stats_meta.clear();
		decisions_assets.Clear();
		combat_actions.reset();
		spells.reset();
	}

	public void forceRecalcBaseStats()
	{
		recalcBaseStats();
	}

	protected virtual void recalcBaseStats()
	{
		resetStatsAndCallbacks();
		foreach (TBaseTrait tTrait in _traits)
		{
			base_stats.mergeStats(tTrait.base_stats);
			base_stats_meta.mergeStats(tTrait.base_stats_meta);
			all_actions_actor_death = (WorldAction)Delegate.Combine(all_actions_actor_death, tTrait.action_death);
			all_actions_actor_growth = (WorldAction)Delegate.Combine(all_actions_actor_growth, tTrait.action_growth);
			all_actions_actor_birth = (WorldAction)Delegate.Combine(all_actions_actor_birth, tTrait.action_birth);
			all_actions_actor_attack_target = (AttackAction)Delegate.Combine(all_actions_actor_attack_target, tTrait.action_attack_target);
			all_actions_actor_get_hit = (GetHitAction)Delegate.Combine(all_actions_actor_get_hit, tTrait.action_get_hit);
			if (tTrait.action_special_effect != null)
			{
				all_actions_actor_special_effect.Add(tTrait);
			}
			if (tTrait.hasDecisions())
			{
				decisions_assets.AddRange(tTrait.decisions_assets);
			}
			if (tTrait.hasCombatActions())
			{
				combat_actions.mergeWith(tTrait.combat_actions);
			}
			if (!tTrait.hasSpells())
			{
				continue;
			}
			spells.mergeWith(tTrait.spells);
			foreach (SpellAsset tSpell in tTrait.spells)
			{
				if (tSpell.hasDecisions())
				{
					decisions_assets.AddRange(tSpell.decisions_assets);
				}
			}
		}
		setUnitStatsDirty();
	}

	private void setUnitStatsDirty()
	{
		List<Actor> tUnits = base.units;
		int tLength = tUnits.Count;
		for (int i = 0; i < tLength; i++)
		{
			tUnits[i].setStatsDirty();
		}
	}

	private void loadTraits()
	{
		if (saved_traits == null)
		{
			return;
		}
		fillTraitAssetsFromStringList(saved_traits);
		foreach (TBaseTrait tTrait in _traits)
		{
			tTrait.action_on_augmentation_load?.Invoke(this, tTrait);
		}
	}

	protected void fillTraitAssetsFromStringList(List<string> pList)
	{
		foreach (string tID in pList.LoopRandom())
		{
			TBaseTrait tTrait = trait_library.get(tID);
			if (tTrait != null && !hasOppositeTrait(tTrait))
			{
				_traits.Add(tTrait);
			}
		}
		recalcBaseStats();
	}

	protected override void generateNewMetaObject()
	{
		base.generateNewMetaObject();
		if (default_traits != null)
		{
			fillTraitAssetsFromStringList(default_traits);
		}
	}

	protected override void generateNewMetaObject(bool pAddDefaultTraits)
	{
		base.generateNewMetaObject();
		if (default_traits != null && pAddDefaultTraits)
		{
			fillTraitAssetsFromStringList(default_traits);
		}
	}

	public List<string> getTraitsAsStrings()
	{
		return Toolbox.getListForSave(_traits);
	}

	public string getTraitsAsLocalizedString()
	{
		string tResult = "";
		foreach (TBaseTrait tBaseTrait in _traits)
		{
			tResult = tResult + tBaseTrait.getTranslatedName() + ", ";
		}
		return tResult;
	}

	public void copyTraits(IReadOnlyCollection<TBaseTrait> pTraitsToCopy)
	{
		foreach (TBaseTrait tTrait in pTraitsToCopy)
		{
			if (!hasOppositeTrait(tTrait))
			{
				_traits.Add(tTrait);
			}
		}
		recalcBaseStats();
	}

	protected void clearTraits()
	{
		if (_traits.Count != 0)
		{
			_traits.Clear();
		}
	}

	public IReadOnlyCollection<TBaseTrait> getTraits()
	{
		return _traits;
	}

	public void sortTraits(IReadOnlyCollection<TBaseTrait> pTraits)
	{
		if (!_traits.SetEquals(pTraits))
		{
			return;
		}
		_traits.Clear();
		foreach (TBaseTrait tTrait in pTraits)
		{
			_traits.Add(tTrait);
		}
	}

	public virtual void traitModifiedEvent()
	{
	}

	public override void triggerOnRemoveObject()
	{
		base.triggerOnRemoveObject();
		foreach (TBaseTrait tTrait in _traits)
		{
			tTrait.action_on_object_remove?.Invoke(this, tTrait);
		}
	}

	public void removeTrait(string pTraitID)
	{
		TBaseTrait tTrait = trait_library.get(pTraitID);
		removeTrait(tTrait);
	}

	public bool hasTrait(string pTrait)
	{
		TBaseTrait tTrait = trait_library.get(pTrait);
		return hasTrait(tTrait);
	}

	public bool hasMetaTag(string pTag)
	{
		return base_stats_meta.hasTag(pTag);
	}

	public bool hasTraits()
	{
		return _traits.Count > 0;
	}

	public bool hasTrait(TBaseTrait pTrait)
	{
		if (_traits.Contains(pTrait))
		{
			return true;
		}
		return false;
	}

	public void removeTraits(ListPool<string> pTraits)
	{
		bool tAnyRemoved = false;
		foreach (ref string pTrait in pTraits)
		{
			string tTraitID = pTrait;
			TBaseTrait tTrait = trait_library.get(tTraitID);
			if (_traits.Remove(tTrait))
			{
				tTrait.action_on_augmentation_remove?.Invoke(this, tTrait);
				tAnyRemoved = true;
			}
		}
		if (tAnyRemoved)
		{
			recalcBaseStats();
		}
	}

	public virtual bool removeTrait(TBaseTrait pTrait)
	{
		bool num = _traits.Remove(pTrait);
		if (num)
		{
			pTrait.action_on_augmentation_remove?.Invoke(this, pTrait);
			recalcBaseStats();
		}
		return num;
	}

	private void removeOppositeTraits(TBaseTrait pTrait)
	{
		if (!pTrait.hasOppositeTraits())
		{
			return;
		}
		foreach (TBaseTrait tTrait in pTrait.opposite_traits)
		{
			removeTrait(tTrait);
		}
	}

	public virtual bool addTrait(string pTraitID, bool pRemoveOpposites = false)
	{
		TBaseTrait tTrait = trait_library.get(pTraitID);
		if (tTrait == null)
		{
			return false;
		}
		return addTrait(tTrait, pRemoveOpposites);
	}

	public virtual bool addTrait(TBaseTrait pTrait, bool pRemoveOpposites = false)
	{
		if (hasTrait(pTrait))
		{
			return false;
		}
		if (pRemoveOpposites)
		{
			removeOppositeTraits(pTrait);
		}
		if (hasOppositeTrait(pTrait))
		{
			return false;
		}
		_traits.Add(pTrait);
		pTrait.action_on_augmentation_add?.Invoke(this, pTrait);
		recalcBaseStats();
		return true;
	}

	public override Sprite getTopicSprite()
	{
		if (_traits.Count == 0)
		{
			return null;
		}
		return _traits.GetRandom().getSprite();
	}

	internal bool hasOppositeTrait(TBaseTrait pTrait)
	{
		return pTrait.hasOppositeTrait(_traits);
	}

	public override ActorAsset getActorAsset()
	{
		if (_species_asset == null)
		{
			string tSpeciesAsset = species_id;
			_species_asset = AssetManager.actor_library.get(tSpeciesAsset);
		}
		return _species_asset;
	}

	public bool isSameActorAsset(ActorAsset pAsset)
	{
		return getActorAsset() == pAsset;
	}

	public void addRandomTraitFromBiome<T>(WorldTile pTile, List<string> pTraitList, AssetLibrary<T> pTraitLibrary) where T : BaseTrait<TBaseTrait>
	{
		if (!pTile.Type.is_biome || pTraitList == null || pTraitList.Count == 0)
		{
			return;
		}
		int tTries = pTraitList.Count;
		for (int i = 0; i < tTries; i++)
		{
			if (!Randy.randomBool())
			{
				string tRandomTraitID = pTraitList.GetRandom();
				Asset tTrait = pTraitLibrary.get(tRandomTraitID);
				addTrait((TBaseTrait)tTrait, pRemoveOpposites: true);
			}
		}
	}

	public void addTraitFromBiome<T>(WorldTile pTile, List<string> pTraitList, AssetLibrary<T> pTraitLibrary) where T : BaseTrait<TBaseTrait>
	{
		if (pTile.Type.is_biome && pTraitList != null && pTraitList.Count != 0)
		{
			for (int i = 0; i < pTraitList.Count; i++)
			{
				Asset tTrait = pTraitLibrary.get(pTraitList[i]);
				addTrait((TBaseTrait)tTrait, pRemoveOpposites: true);
			}
		}
	}

	public TBaseTrait getTraitForBook()
	{
		IReadOnlyCollection<TBaseTrait> tTraits = getTraits();
		using ListPool<TBaseTrait> tList = new ListPool<TBaseTrait>(tTraits.Count);
		foreach (TBaseTrait tTrait in tTraits)
		{
			if (tTrait.can_be_in_book)
			{
				tList.Add(tTrait);
			}
		}
		if (tList.Count == 0)
		{
			return null;
		}
		return tList.GetRandom();
	}

	public override void Dispose()
	{
		_species_asset = null;
		clearTraits();
		resetStatsAndCallbacks();
		base.Dispose();
	}
}
