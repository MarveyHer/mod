using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BaseSimObject : NanoObject, IEquatable<BaseSimObject>
{
	public float position_height;

	public WorldTile current_tile;

	public Vector2 current_position;

	public Vector3 current_scale;

	internal Vector3 current_rotation;

	private HashSet<long> _targets_to_ignore;

	[NonSerialized]
	public Kingdom kingdom;

	private bool _stats_dirty;

	internal bool event_full_stats;

	internal readonly BaseStats stats = new BaseStats();

	internal Actor a;

	internal Building b;

	private MapObjectType _object_type;

	private readonly Dictionary<string, Status> _active_status_dict = new Dictionary<string, Status>();

	private bool _has_any_status_cached;

	private bool _has_any_status_to_render;

	internal Vector3 cur_transform_position;

	public TileIsland current_island => current_tile.region.island;

	public TileZone current_zone => current_tile.zone;

	public MapChunk current_chunk => current_tile.chunk;

	public MapRegion current_region => current_tile.region;

	public MapChunk chunk => current_tile.chunk;

	internal virtual void create()
	{
	}

	public int countStatusEffects()
	{
		return _active_status_dict.Count;
	}

	public Dictionary<string, Status>.ValueCollection getStatuses()
	{
		return _active_status_dict.Values;
	}

	public Dictionary<string, Status>.KeyCollection getStatusesIds()
	{
		return _active_status_dict.Keys;
	}

	public IReadOnlyDictionary<string, Status> getStatusesDict()
	{
		return _active_status_dict;
	}

	protected override void setDefaultValues()
	{
		base.setDefaultValues();
		_stats_dirty = true;
		event_full_stats = false;
		current_rotation = default(Vector3);
		position_height = 0f;
		_has_any_status_cached = false;
		_has_any_status_to_render = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasCity()
	{
		return getCity() != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual City getCity()
	{
		return null;
	}

	internal bool addStatusEffect(string pID, float pOverrideTimer = 0f, bool pColorEffect = true)
	{
		StatusAsset tStatusAsset = AssetManager.status.get(pID);
		if (tStatusAsset == null)
		{
			return false;
		}
		return addStatusEffect(tStatusAsset, pOverrideTimer, pColorEffect);
	}

	internal virtual bool addStatusEffect(StatusAsset pStatusAsset, float pOverrideTimer = 0f, bool pColorEffect = true)
	{
		if (!isAlive())
		{
			return false;
		}
		bool tIsActor = isActor();
		if (tIsActor && a.asset.allowed_status_tiers < pStatusAsset.tier)
		{
			return false;
		}
		bool tHasAnyStatus = hasAnyStatusEffectRaw();
		if (tHasAnyStatus && hasStatus(pStatusAsset.id))
		{
			if (!pStatusAsset.allow_timer_reset && pOverrideTimer == 0f)
			{
				return false;
			}
			Status tStatusEffect = _active_status_dict[pStatusAsset.id];
			float tResetTimer = pStatusAsset.duration;
			if (pOverrideTimer != 0f)
			{
				tResetTimer = pOverrideTimer;
			}
			if (tStatusEffect.getRemainingTime() < (double)tResetTimer)
			{
				tStatusEffect.setDuration(tResetTimer);
			}
			return true;
		}
		if (!canAddStatus(pStatusAsset, tIsActor, pColorEffect))
		{
			return false;
		}
		addNewStatusEffect(pStatusAsset, pOverrideTimer, pColorEffect, tIsActor, tHasAnyStatus);
		return true;
	}

	private bool canAddStatus(StatusAsset pStatusAsset, bool pIsActor, bool pHasAnyStatus)
	{
		if (pIsActor)
		{
			if (pStatusAsset.opposite_traits != null)
			{
				for (int i = 0; i < pStatusAsset.opposite_traits.Length; i++)
				{
					string tTraitID = pStatusAsset.opposite_traits[i];
					if (a.hasTrait(tTraitID))
					{
						return false;
					}
				}
			}
			if (pStatusAsset.opposite_tags != null && a.stats.hasTags() && a.stats.hasTags(pStatusAsset.opposite_tags))
			{
				return false;
			}
		}
		if (pStatusAsset.opposite_status != null && pHasAnyStatus)
		{
			for (int j = 0; j < pStatusAsset.opposite_status.Length; j++)
			{
				string tStatusID = pStatusAsset.opposite_status[j];
				if (hasStatus(tStatusID))
				{
					return false;
				}
			}
		}
		return true;
	}

	private void addNewStatusEffect(StatusAsset pStatusAsset, float pOverrideTimer, bool pColorEffect, bool pIsActor, bool pHasAnyStatus)
	{
		Status tNewStatus = World.world.statuses.newStatus(this, pStatusAsset, pOverrideTimer);
		setStatsDirty();
		_active_status_dict.Add(pStatusAsset.id, tNewStatus);
		_has_any_status_cached = true;
		if (pIsActor && pStatusAsset.cancel_actor_job && pColorEffect)
		{
			a.cancelAllBeh();
			a.startColorEffect();
		}
		if (pStatusAsset.remove_status != null && pHasAnyStatus)
		{
			for (int i = 0; i < pStatusAsset.remove_status.Length; i++)
			{
				string tStatusToFinish = pStatusAsset.remove_status[i];
				finishStatusEffect(tStatusToFinish);
			}
		}
		if (pIsActor)
		{
			pStatusAsset.action_on_receive?.Invoke(this);
		}
	}

	internal void finishAllStatusEffects()
	{
		foreach (Status value in _active_status_dict.Values)
		{
			value.finish();
			setStatsDirty();
		}
		_active_status_dict.Clear();
		_has_any_status_cached = false;
		_has_any_status_to_render = false;
	}

	public void finishStatusEffect(string pID)
	{
		if (hasAnyStatusEffect() && _active_status_dict.TryGetValue(pID, out var tStatusEffect))
		{
			tStatusEffect.finish();
			setStatsDirty();
		}
	}

	public virtual void setStatsDirty()
	{
		_stats_dirty = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isActor()
	{
		return _object_type == MapObjectType.Actor;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isBuilding()
	{
		return _object_type == MapObjectType.Building;
	}

	public void setObjectType(MapObjectType pType)
	{
		_object_type = pType;
		if (_object_type == MapObjectType.Actor)
		{
			a = (Actor)this;
		}
		else
		{
			b = (Building)this;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool hasStatus(string pID)
	{
		return _active_status_dict.ContainsKey(pID);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool hasAnyStatusEffect()
	{
		return _has_any_status_cached;
	}

	internal bool hasAnyStatusEffectRaw()
	{
		return _active_status_dict.Count > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool hasAnyStatusEffectToRender()
	{
		return _has_any_status_to_render;
	}

	public void removeFinishedStatusEffect(Status pStatusData)
	{
		_active_status_dict.Remove(pStatusData.asset.id);
		_has_any_status_cached = hasAnyStatusEffectRaw();
		setStatsDirty();
	}

	internal virtual void updateStats()
	{
		_stats_dirty = false;
		stats_dirty_version++;
		updateCachedStatusEffects();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isStatsDirty()
	{
		return _stats_dirty;
	}

	private void updateCachedStatusEffects()
	{
		_has_any_status_cached = hasAnyStatusEffectRaw();
		_has_any_status_to_render = false;
		if (!_has_any_status_cached)
		{
			return;
		}
		foreach (Status tStatusEffect in _active_status_dict.Values)
		{
			if (!tStatusEffect.is_finished && tStatusEffect.asset.need_visual_render)
			{
				_has_any_status_to_render = true;
				break;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isInLiquid()
	{
		return current_tile.Type.liquid;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isInWater()
	{
		return current_tile.Type.ocean;
	}

	public bool isTouchingLiquid()
	{
		if (isInLiquid())
		{
			return !isInAir();
		}
		return false;
	}

	internal virtual bool isInAir()
	{
		return false;
	}

	internal virtual bool isFlying()
	{
		return false;
	}

	internal virtual float getHeight()
	{
		return 0f;
	}

	internal virtual void getHit(float pDamage, bool pFlash = true, AttackType pAttackType = AttackType.Other, BaseSimObject pAttacker = null, bool pSkipIfShake = true, bool pMetallicWeapon = false, bool pCheckDamageReduction = true)
	{
	}

	internal virtual void getHitFullHealth(AttackType pAttackType)
	{
	}

	internal BaseSimObject findEnemyObjectTarget(bool pAttackBuildings)
	{
		EnemyFinderData tEnemyData = EnemiesFinder.findEnemiesFrom(current_tile, kingdom);
		if (tEnemyData.isEmpty())
		{
			return null;
		}
		bool tFindClosest = true;
		if (tEnemyData.list.Count > 50)
		{
			tFindClosest = Randy.randomChance(0.6f);
		}
		IEnumerable<BaseSimObject> pList;
		if (!tFindClosest)
		{
			pList = tEnemyData.list.LoopRandom();
		}
		else
		{
			IEnumerable<BaseSimObject> list = tEnemyData.list;
			pList = list;
		}
		return checkObjectList(pList, pAttackBuildings, tFindClosest, pIgnoreStunned: false);
	}

	protected BaseSimObject checkObjectList(IEnumerable<BaseSimObject> pList, bool pAttackBuildings, bool pFindClosest, bool pIgnoreStunned, int pMaxDist = int.MaxValue)
	{
		int tDist = int.MaxValue;
		BaseSimObject tBestObject = null;
		long tBestDist = ((pMaxDist == int.MaxValue) ? pMaxDist : (pMaxDist * pMaxDist + 1));
		bool tHasMelee = isActor() && a.hasMeleeAttack();
		WorldTile tCurrentTile = current_tile;
		Vector2Int tCurrentPos = tCurrentTile.pos;
		foreach (BaseSimObject tObject in pList)
		{
			if (!tObject.isAlive() || tObject == this)
			{
				continue;
			}
			WorldTile tObjectTile = tObject.current_tile;
			if (pFindClosest)
			{
				tDist = Toolbox.SquaredDistVec2(tObjectTile.pos, tCurrentPos);
				if (tDist >= tBestDist)
				{
					continue;
				}
			}
			if ((!pIgnoreStunned || !tObject.isActor() || !tObject.a.hasStatusStunned()) && canAttackTarget(tObject, pCheckForFactions: true, pAttackBuildings) && (!tHasMelee || tObjectTile.isSameIsland(tCurrentTile) || (!tObjectTile.Type.block && tCurrentTile.region.island.isConnectedWith(tObjectTile.region.island))) && (!tObject.isBuilding() || !isKingdomCiv() || !tObject.b.asset.city_building || tObject.b.asset.tower || !(tObject.kingdom.getSpecies() == kingdom.getSpecies())) && !shouldIgnoreTarget(tObject))
			{
				if (!pFindClosest)
				{
					return tObject;
				}
				if (tDist <= 4)
				{
					return tObject;
				}
				tBestObject = tObject;
				tBestDist = tDist;
			}
		}
		return tBestObject;
	}

	internal void ignoreTarget(BaseSimObject pTarget)
	{
		if (_targets_to_ignore == null)
		{
			_targets_to_ignore = new HashSet<long>();
		}
		_targets_to_ignore.Add(pTarget.getID());
	}

	internal bool shouldIgnoreTarget(BaseSimObject pTarget)
	{
		return _targets_to_ignore?.Contains(pTarget.getID()) ?? false;
	}

	internal void clearIgnoreTargets()
	{
		_targets_to_ignore?.Clear();
	}

	internal int countTargetsToIgnore()
	{
		return _targets_to_ignore?.Count ?? 0;
	}

	internal bool canAttackTarget(BaseSimObject pTarget, bool pCheckForFactions = true, bool pAttackBuildings = true)
	{
		if (!isAlive())
		{
			return false;
		}
		if (!pTarget.isAlive())
		{
			return false;
		}
		bool tThisIsActor = isActor();
		if (pTarget.isBuilding() && !pAttackBuildings)
		{
			if (!tThisIsActor || !a.asset.unit_zombie)
			{
				return false;
			}
			if (!pTarget.kingdom.asset.brain)
			{
				return false;
			}
		}
		string tSpeciesID;
		WeaponType tAttackType;
		if (tThisIsActor)
		{
			if (a.asset.skip_fight_logic)
			{
				return false;
			}
			tSpeciesID = a.asset.id;
			tAttackType = a._attack_asset.attack_type;
		}
		else
		{
			tSpeciesID = b.kingdom.getSpecies();
			tAttackType = WeaponType.Range;
		}
		if (pTarget.isActor())
		{
			Actor tActorTarget = pTarget.a;
			if (!tActorTarget.asset.can_be_killed_by_stuff)
			{
				return false;
			}
			if (tActorTarget.isInsideSomething())
			{
				return false;
			}
			if (tActorTarget.isFlying() && tAttackType == WeaponType.Melee)
			{
				return false;
			}
			if (tActorTarget.ai.action != null && tActorTarget.ai.action.special_prevent_can_be_attacked)
			{
				return false;
			}
			if (tActorTarget.isInMagnet())
			{
				return false;
			}
			if (pCheckForFactions && areFoes(pTarget) && tActorTarget.isKingdomCiv() && isKingdomCiv() && !hasStatusTantrum() && !tActorTarget.hasStatusTantrum())
			{
				bool tXenophobicAny = (tThisIsActor && a.hasXenophobic()) || tActorTarget.hasXenophobic();
				bool tXenophileAny = (tThisIsActor && a.hasXenophiles()) || tActorTarget.hasXenophiles();
				bool tSameCulture = tThisIsActor && a.culture == tActorTarget.culture;
				bool tSameSpecies = tSpeciesID == tActorTarget.asset.id;
				bool tIgnoreCivilians = ((tSameSpecies || tXenophileAny) && !tXenophobicAny) || (tSameCulture && tSameSpecies);
				if (!WorldLawLibrary.world_law_angry_civilians.isEnabled())
				{
					if (tActorTarget.profession_asset.is_civilian && tIgnoreCivilians)
					{
						return false;
					}
					if (tThisIsActor && a.profession_asset.is_civilian && tIgnoreCivilians)
					{
						return false;
					}
				}
			}
			if (pCheckForFactions && tThisIsActor && a.hasCannibalism() && a.isSameSpecies(tActorTarget))
			{
				Family tFamilyThis = a.family;
				Family tFamilyTarget = tActorTarget.family;
				if (tFamilyTarget == null || tFamilyThis == null)
				{
					return false;
				}
				if (a.hasFamily())
				{
					if (tFamilyTarget == tFamilyThis)
					{
						return false;
					}
					if (!tFamilyTarget.areMostUnitsHungry() && !tFamilyThis.areMostUnitsHungry())
					{
						return false;
					}
				}
			}
		}
		else
		{
			Building tBuildingTarget = pTarget.b;
			if (isKingdomCiv() && tBuildingTarget.asset.city_building && tBuildingTarget.asset.tower && !tBuildingTarget.isCiv() && tThisIsActor && a.profession_asset.is_civilian && !WorldLawLibrary.world_law_angry_civilians.isEnabled() && tBuildingTarget.kingdom.getSpecies() == kingdom.getSpecies())
			{
				return false;
			}
		}
		if (tThisIsActor)
		{
			ActorAsset tActorAsset = a.asset;
			if (!a.isWaterCreature() || !a.hasRangeAttack())
			{
				if (a.isWaterCreature() && !tActorAsset.force_land_creature)
				{
					if (!pTarget.isInLiquid())
					{
						return false;
					}
					if (!pTarget.current_tile.isSameIsland(current_tile))
					{
						return false;
					}
				}
				else if (tAttackType == WeaponType.Melee && pTarget.isInLiquid() && !a.isWaterCreature())
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool areFoes(BaseSimObject pTarget)
	{
		return kingdom.isEnemy(pTarget.kingdom);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void setHealth(int pValue, bool pClamp = true)
	{
		BaseObjectData data = getData();
		if (pClamp)
		{
			pValue = Mathf.Clamp(pValue, 1, getMaxHealth());
		}
		data.health = pValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void setMaxHealth()
	{
		setHealth(getMaxHealth());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void changeHealth(int pValue)
	{
		BaseObjectData data = getData();
		int tNewValue = data.health + pValue;
		data.health = Mathf.Clamp(tNewValue, 0, getMaxHealth());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int getHealth()
	{
		return getData().health;
	}

	public int getMaxHealthPercent(float pPercent)
	{
		int tResult = (int)((float)getMaxHealth() * pPercent);
		return Mathf.Max(1, tResult);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasHealth()
	{
		return getHealth() > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(BaseSimObject pObject)
	{
		return _hashcode == pObject.GetHashCode();
	}

	public int getMaxHealth()
	{
		return (int)stats["health"];
	}

	public override void Dispose()
	{
		current_tile = null;
		kingdom = null;
		stats.reset();
		clearIgnoreTargets();
		_targets_to_ignore = null;
		disposeStatusEffects();
		current_tile = null;
		base.Dispose();
	}

	private void disposeStatusEffects()
	{
		foreach (Status value in _active_status_dict.Values)
		{
			value.finish();
		}
		_active_status_dict.Clear();
		_has_any_status_cached = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isKingdomCiv()
	{
		return kingdom.isCiv();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isKingdomMob()
	{
		return kingdom.isMobs();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasKingdom()
	{
		return kingdom != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual BaseObjectData getData()
	{
		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override long getID()
	{
		return getData().id;
	}

	public override double getFoundedTimestamp()
	{
		return getData().created_time;
	}

	public virtual bool hasStatusTantrum()
	{
		return false;
	}

	public bool isSameIsland(WorldTile pTile)
	{
		return current_tile.isSameIsland(pTile);
	}

	public bool isSameIslandAs(BaseSimObject pTarget)
	{
		return current_tile.isSameIsland(pTarget.current_tile);
	}
}
