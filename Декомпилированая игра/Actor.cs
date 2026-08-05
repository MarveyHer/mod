using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ai;
using ai.behaviours;
using UnityEngine;

public class Actor : BaseSimObject, ILoadable<ActorData>, ITraitsOwner<ActorTrait>, IEquatable<Actor>, IComparable<Actor>, IFavoriteable
{
	internal ActorIdleLoopSound idle_loop_sound;

	internal bool is_forced_socialize_icon;

	internal double is_forced_socialize_timestamp;

	internal string ate_last_item_id;

	internal double timestamp_session_ate_food;

	internal double timestamp_tween_session_social;

	private double _last_color_effect_timestamp;

	private double _last_stamina_reduce_timestamp;

	internal double timestamp_profession_set;

	internal List<BaseActorComponent> children_special;

	private Dictionary<Type, BaseActorComponent> _dict_special;

	private List<ActorSimpleComponent> children_pre_behaviour;

	private Dictionary<Type, ActorSimpleComponent> dict_pre_behaviour;

	private UnitProfession _profession;

	public GameObject avatar;

	private double _timestamp_augmentation_effects;

	internal bool show_shadow;

	internal Vector2 current_shadow_position = Globals.POINT_IN_VOID;

	private double[] _decision_cooldowns;

	private bool[] _decision_disabled;

	public DecisionAsset[] decisions = new DecisionAsset[64];

	public int decisions_counter;

	private int _current_children;

	private readonly Queue<HappinessHistory> _last_happiness_history = new Queue<HappinessHistory>();

	private HashSet<long> _aggression_targets = new HashSet<long>();

	private HoverState _hover_state;

	private float _hover_timer;

	public BatchActors batch;

	internal WorldTile beh_tile_target;

	internal Building beh_building_target;

	internal BaseSimObject beh_actor_target;

	internal Book beh_book_target;

	internal Building inside_building;

	internal bool is_inside_building;

	internal Boat inside_boat;

	internal bool is_inside_boat;

	internal BaseSimObject attackedBy;

	public Actor lover;

	public readonly HashSet<ActorTrait> traits = new HashSet<ActorTrait>();

	private readonly CombatActionHolder _combat_actions = new CombatActionHolder();

	private readonly SpellHolder _spells = new SpellHolder();

	private readonly Dictionary<string, bool> _traits_cache = new Dictionary<string, bool>();

	internal ActorData data;

	internal ProfessionAsset profession_asset;

	private bool _state_adult;

	private bool _state_baby;

	private bool _state_egg;

	public ActorAsset asset;

	public Vector2 next_step_position;

	public Vector2 next_step_position_possession;

	internal Vector2 shake_offset;

	public static readonly Vector2 sprite_offset = new Vector2(0.5f, 0.5f);

	public Vector2 move_jump_offset;

	private bool _shake_horizontal;

	private bool _shake_vertical;

	private float _shake_timer;

	private bool _shake_active;

	private float _shake_volume;

	private bool _is_moving;

	private bool _possessed_movement;

	private bool _is_in_liquid;

	internal bool is_visible;

	internal bool last_sprite_renderer_enabled;

	internal AnimationFrameData frame_data;

	internal bool dirty_current_tile;

	internal WorldTile tile_target;

	private WorldTile _next_step_tile;

	public SplitPathStatus split_path;

	public int current_path_index;

	public readonly List<WorldTile> current_path = new List<WorldTile>();

	public List<MapRegion> current_path_global;

	public BaseActionActor callbacks_on_death;

	public BaseActionActor callbacks_landed;

	public BaseActionActor callbacks_cancel_path_movement;

	public BaseActionActor callbacks_magnet_update;

	internal float actor_scale;

	internal float target_scale;

	internal BaseSimObject attack_target;

	internal bool has_attack_target;

	internal float timer_action;

	internal float timer_jump_animation;

	internal float hitbox_bonus_height;

	internal Vector3 velocity;

	internal float velocity_speed;

	internal bool under_forces;

	protected WorldTimer targets_to_ignore_timer;

	private bool _flying;

	internal bool is_in_magnet;

	internal float attack_timer;

	internal double last_attack_timestamp;

	internal EquipmentAsset _attack_asset;

	internal PersonalityAsset s_personality;

	private readonly List<BaseAugmentationAsset> _s_special_effect_augmentations = new List<BaseAugmentationAsset>();

	private readonly Dictionary<BaseAugmentationAsset, double> _s_special_effect_augmentations_timers = new Dictionary<BaseAugmentationAsset, double>();

	internal AttackAction s_action_attack_target;

	internal GetHitAction s_get_hit_action;

	protected static readonly List<BaseAugmentationAsset> _tempAugmentationList = new List<BaseAugmentationAsset>();

	private bool _has_emotions;

	private bool _has_tag_unconscious;

	public bool has_tag_immunity_cold;

	private bool _has_status_strange_urge;

	private bool _has_status_possessed;

	private bool _has_status_sleeping;

	private bool _has_status_tantrum;

	private bool _has_status_drowning;

	private bool _has_status_invincible;

	private bool _cache_check_has_status_removed_on_damage;

	private bool _has_trait_weightless;

	private bool _has_trait_peaceful;

	private bool _has_trait_clone;

	internal bool has_tag_generate_light;

	private bool _has_any_sick_trait;

	internal bool is_immovable;

	internal bool is_ai_frozen;

	private bool _has_stop_idle_animation;

	private bool _ignore_fights;

	protected bool should_check_land_cancel;

	internal WorldTile scheduled_tile_target;

	internal bool _action_wait_after_land;

	internal float _action_wait_after_land_timer;

	internal AiSystemActor ai;

	public CitizenJobAsset citizen_job;

	protected Building _home_building;

	private float _death_timer_color_stage_1;

	private float _death_timer_alpha_stage_2;

	private float _jump_time;

	private float lastX;

	private float lastY;

	public float flip_angle;

	internal bool flip;

	private int _precalc_movement_speed_skips;

	private float _current_combined_movement_speed;

	internal float _timeout_targets;

	internal Vector3 target_angle;

	internal float rotation_cooldown;

	private RotationDirection _rotation_direction;

	private Sprite _last_topic_sprite;

	public Color color;

	internal bool dirty_sprite_main;

	private Sprite _cached_sprite_item;

	private IHandRenderer _cached_hand_renderer_asset;

	internal Sprite cached_sprite_head;

	internal bool dirty_sprite_head;

	internal AnimationContainerUnit animation_container;

	private Sprite _last_main_sprite;

	private Sprite _last_colored_sprite;

	private ColorAsset _last_color_asset;

	private bool _dirty_sprite_item;

	private bool _has_animated_item;

	public SpriteAnimation sprite_animation;

	private const float POSSESSION_ATTACK_SECONDS = 0.5f;

	private double _possession_attack_happened_frame;

	private AttackType _last_attack_type;

	public ActorEquipment equipment;

	public Army army;

	public City city;

	public Clan clan;

	public Culture culture;

	public Family family;

	public Language language;

	public Plot plot;

	public Religion religion;

	public Subspecies subspecies;

	private const float FIND_TILE_SQ_DIST = 4f;

	private const float CUR_SQ_DIST = 0.16000001f;

	private const float NEW_SQ_DIST = 0.09f;

	private bool _beh_skip;

	private bool _update_done;

	private string _last_decision_id;

	public Queue<HappinessHistory> happiness_change_history => _last_happiness_history;

	public string coloredName
	{
		get
		{
			if (data != null)
			{
				if (kingdom?.getColor() != null)
				{
					return "<color=" + kingdom.getColor().color_text + ">" + getName() + "</color>";
				}
				return getName();
			}
			return "";
		}
	}

	public bool is_invincible => _has_status_invincible;

	public override string name
	{
		get
		{
			return getName();
		}
		protected set
		{
			data.name = value;
		}
	}

	public Building home_building => _home_building;

	public int age => getAge();

	public bool is_army_captain
	{
		get
		{
			if (hasArmy())
			{
				return army.getCaptain() == this;
			}
			return false;
		}
	}

	public bool is_profession_nothing => _profession == UnitProfession.Nothing;

	public bool is_profession_king => _profession == UnitProfession.King;

	public bool is_profession_leader => _profession == UnitProfession.Leader;

	public bool is_profession_warrior => _profession == UnitProfession.Warrior;

	public bool is_profession_citizen => _profession == UnitProfession.Unit;

	public bool is_looking_left => !flip;

	public ResourceAsset favorite_food_asset => AssetManager.resources.get(data.favorite_food);

	public int current_children_count => _current_children;

	public bool is_unconscious => _has_tag_unconscious;

	public int loot => data.loot;

	public int money => data.money;

	public int renown => data.renown;

	public int level => data.level;

	public int intelligence => (int)stats["intelligence"];

	public int diplomacy => (int)stats["diplomacy"];

	public int warfare => (int)stats["warfare"];

	public int stewardship => (int)stats["stewardship"];

	protected override MetaType meta_type => MetaType.Unit;

	public bool is_moving
	{
		get
		{
			if (!_is_moving)
			{
				return _possessed_movement;
			}
			return true;
		}
	}

	public bool has_rendered_sprite_head => (object)cached_sprite_head != null;

	public WorldTile debug_next_step_tile => _next_step_tile;

	public ActorBag inventory
	{
		get
		{
			return data.inventory;
		}
		set
		{
			data.inventory = value;
		}
	}

	protected sealed override void setDefaultValues()
	{
		base.setDefaultValues();
		_last_decision_id = string.Empty;
		is_inside_building = false;
		inside_building = null;
		_state_adult = false;
		_state_baby = false;
		_state_egg = false;
		next_step_position = Globals.emptyVector;
		shake_offset = new Vector2(0f, 0f);
		move_jump_offset = new Vector2(0f, 0f);
		_shake_horizontal = true;
		_shake_vertical = true;
		_shake_timer = 0f;
		_shake_active = false;
		_shake_volume = 0.1f;
		_is_moving = false;
		is_visible = false;
		last_sprite_renderer_enabled = false;
		dirty_current_tile = true;
		split_path = SplitPathStatus.Normal;
		current_path_index = 0;
		current_path_global = null;
		actor_scale = 0f;
		target_scale = 0f;
		timer_action = 0f;
		timer_jump_animation = 0f;
		hitbox_bonus_height = 0f;
		velocity = default(Vector3);
		velocity_speed = 0f;
		under_forces = false;
		_flying = false;
		is_in_magnet = false;
		attack_timer = 0f;
		_attack_asset = ItemLibrary.base_attack;
		dirty_sprite_main = true;
		cached_sprite_head = null;
		dirty_sprite_head = true;
		_cached_sprite_item = null;
		_cached_hand_renderer_asset = null;
		s_personality = null;
		_action_wait_after_land = false;
		rotation_cooldown = 0f;
		target_angle = default(Vector3);
		_timeout_targets = 0f;
		_precalc_movement_speed_skips = 0;
		flip_angle = 0f;
		lastX = -10f;
		lastY = -10f;
		_jump_time = 0f;
		_death_timer_color_stage_1 = 1f;
		_death_timer_alpha_stage_2 = 1f;
		color = Color.white;
		ate_last_item_id = string.Empty;
		timestamp_session_ate_food = 0.0;
		timestamp_tween_session_social = 0.0;
		timestamp_profession_set = 0.0;
		_timestamp_augmentation_effects = 0.0;
		show_shadow = false;
		_decision_cooldowns = Toolbox.checkArraySize(_decision_cooldowns, AssetManager.decisions_library.list.Count);
		_decision_disabled = Toolbox.checkArraySize(_decision_disabled, AssetManager.decisions_library.list.Count);
		decisions = Toolbox.checkArraySize(decisions, AssetManager.decisions_library.list.Count);
	}

	public void setShowShadow(bool pShadow)
	{
		show_shadow = pShadow;
	}

	private void updateChildrenList(List<BaseActorComponent> pList, float pElapsed)
	{
		if (pList != null)
		{
			for (int i = 0; i < pList.Count; i++)
			{
				pList[i].update(pElapsed);
			}
		}
	}

	private void updateChildrenListSimple(List<ActorSimpleComponent> pList, float pElapsed)
	{
		if (pList != null)
		{
			for (int i = 0; i < pList.Count; i++)
			{
				pList[i].update(pElapsed);
			}
		}
	}

	public void setAsset(ActorAsset pAsset)
	{
		if (asset != null)
		{
			asset.units.Remove(this);
		}
		asset = pAsset;
		asset.units.Add(this);
		setStatsDirty();
		if (canUseItems() && !hasEquipment())
		{
			equipment = new ActorEquipment();
		}
	}

	internal override void create()
	{
		base.create();
		if (ai == null)
		{
			ai = new AiSystemActor(this);
		}
		ai.jobs_library = AssetManager.job_actor;
		ai.task_library = AssetManager.tasks_actor;
		ai.next_job_delegate = getNextJob;
		ai.clear_action_delegate = clearBeh;
		ai.subscribeToTaskSwitch(setItemSpriteRenderDirty);
		if (targets_to_ignore_timer == null)
		{
			targets_to_ignore_timer = new WorldTimer(3f, base.clearIgnoreTargets);
		}
		_flying = asset.flying;
		setActorScale(asset.base_stats["scale"] * 0.6f);
		if (asset.finish_scale_on_creation)
		{
			target_scale = asset.base_stats["scale"];
			finishScale();
		}
		setObjectType(MapObjectType.Actor);
		setShowShadow(asset.shadow);
		if (asset.has_sound_idle_loop)
		{
			idle_loop_sound = new ActorIdleLoopSound(asset, this);
		}
		if (isHovering())
		{
			move_jump_offset.y = asset.hovering_min;
		}
		addChildren();
		if (asset.kingdom_id_wild.Contains("ants"))
		{
			AchievementLibrary.ant_world.check();
		}
		if (asset.kingdom_id_wild.Contains("monkey"))
		{
			AchievementLibrary.planet_of_apes.check();
		}
		if (asset.cancel_beh_on_land)
		{
			callbacks_landed = (BaseActionActor)Delegate.Combine(callbacks_landed, new BaseActionActor(checkLand));
		}
		callbacks_landed = (BaseActionActor)Delegate.Combine(callbacks_landed, new BaseActionActor(checkDeathOutsideMap));
		callbacks_on_death = (BaseActionActor)Delegate.Combine(callbacks_on_death, new BaseActionActor(playDeathSound));
		callbacks_magnet_update = (BaseActionActor)Delegate.Combine(callbacks_magnet_update, new BaseActionActor(actionMagnetAnimation));
	}

	public bool canSeeTileBasedOnDirection(WorldTile pTile)
	{
		bool tHappenedOnLeft = isTileOnTheLeft(pTile);
		return is_looking_left == tHappenedOnLeft;
	}

	public void setParent1(Actor pParentActor, bool pIncreaseChildren = true)
	{
		data.parent_id_1 = pParentActor.data.id;
		if (pIncreaseChildren)
		{
			pParentActor.increaseChildren();
		}
	}

	public void setParent2(Actor pActor, bool pIncreaseChildren = true)
	{
		data.parent_id_2 = pActor.data.id;
		if (pIncreaseChildren)
		{
			pActor.increaseChildren();
		}
	}

	internal void setProfession(UnitProfession pType, bool pCancelBeh = true)
	{
		_profession = pType;
		profession_asset = AssetManager.professions.get(pType);
		setStatsDirty();
		if (hasCity())
		{
			city.setCitizensDirty();
		}
		if (pCancelBeh)
		{
			cancelAllBeh();
		}
		timestamp_profession_set = World.world.getCurWorldTime();
		clearGraphicsFully();
	}

	private void addChildren()
	{
		if (asset.avatar_prefab != string.Empty)
		{
			GameObject tPrefab = Resources.Load<GameObject>("actors/" + asset.avatar_prefab);
			avatar = UnityEngine.Object.Instantiate(tPrefab, World.world.transform_units);
			if (avatar.HasComponent<SpriteAnimation>())
			{
				sprite_animation = avatar.GetComponent<SpriteAnimation>();
				batch.c_sprite_animations.Add(this);
			}
			if (avatar.HasComponent<Crabzilla>())
			{
				addChild(avatar.GetComponent<Crabzilla>());
			}
			if (avatar.HasComponent<GodFinger>())
			{
				addChild(avatar.GetComponent<GodFinger>());
			}
			if (avatar.HasComponent<Dragon>())
			{
				addChild(avatar.GetComponent<Dragon>());
			}
			if (avatar.HasComponent<UFO>())
			{
				addChild(avatar.GetComponent<UFO>());
			}
		}
		if (asset.is_boat)
		{
			addChildSimple(new Boat());
		}
		if (children_pre_behaviour != null || children_special != null)
		{
			batch.c_update_children.Add(this);
		}
	}

	private void addChild(BaseActorComponent pObject)
	{
		if (children_special == null)
		{
			children_special = new List<BaseActorComponent>();
			_dict_special = new Dictionary<Type, BaseActorComponent>();
		}
		Type tType = pObject.GetType();
		children_special.Add(pObject);
		_dict_special.Add(tType, pObject);
		pObject.create(this);
	}

	private void addChildSimple(ActorSimpleComponent pObject)
	{
		if (children_pre_behaviour == null)
		{
			children_pre_behaviour = new List<ActorSimpleComponent>();
			dict_pre_behaviour = new Dictionary<Type, ActorSimpleComponent>();
		}
		Type tType = pObject.GetType();
		children_pre_behaviour.Add(pObject);
		dict_pre_behaviour.Add(tType, pObject);
		pObject.create(this);
	}

	public T getActorComponent<T>() where T : BaseActorComponent
	{
		if (_dict_special == null)
		{
			return null;
		}
		if (_dict_special.TryGetValue(typeof(T), out var tResult))
		{
			return tResult as T;
		}
		return null;
	}

	public T getSimpleComponent<T>() where T : ActorSimpleComponent
	{
		if (dict_pre_behaviour.TryGetValue(typeof(T), out var tResult))
		{
			return tResult as T;
		}
		return null;
	}

	private void playDeathSound(Actor pActor)
	{
		if (asset.has_sound_death)
		{
			MusicBox.playSound(asset.sound_death, current_tile, pGameViewOnly: true, pVisibleOnly: true);
		}
	}

	public void playIdleSound()
	{
		if (asset.has_sound_idle)
		{
			MusicBox.playIdleSoundVisibleOnly(asset.sound_idle, current_tile);
		}
	}

	public void startShake(float pTimer = 0.3f, float pVol = 0.1f, bool pHorizontal = true, bool pVertical = true)
	{
		_shake_horizontal = pHorizontal;
		_shake_vertical = pVertical;
		_shake_timer = Mathf.Min(pTimer, asset.max_shake_timer);
		_shake_volume = pVol;
		_shake_active = true;
		batch.c_shake.Add(this);
	}

	public Vector3 getThrowStartPosition()
	{
		Vector3 tCurrentActorPos = cur_transform_position;
		Vector3 tActorScale = current_scale;
		Vector3 vector = current_rotation;
		AnimationFrameData tFrameData = getAnimationFrameData();
		float tFrameDataPosX = 0f;
		float tFrameDataPosY = 0f;
		if (tFrameData != null)
		{
			tFrameDataPosX = tFrameData.pos_item.x;
			tFrameDataPosY = tFrameData.pos_item.y;
		}
		float tX = tCurrentActorPos.x + tFrameDataPosX * tActorScale.x;
		float tY = tCurrentActorPos.y + tFrameDataPosY * tActorScale.y;
		Vector3 tItemPosition = new Vector3(tX, tY, -0.01f);
		Vector3 tAngle = vector;
		if (tAngle.y != 0f || tAngle.z != 0f)
		{
			Vector3 t_pivot = new Vector3(tCurrentActorPos.x, tCurrentActorPos.y, 0f);
			tItemPosition = Toolbox.RotatePointAroundPivot(ref tItemPosition, ref t_pivot, ref tAngle);
			tItemPosition.z = -0.01f;
		}
		return tItemPosition;
	}

	public void checkDefaultProfession()
	{
		setProfession(UnitProfession.Unit, pCancelBeh: false);
	}

	public void addAfterglowStatus()
	{
		float tAdjustedTimeout = (float)asset.months_breeding_timeout * 5f;
		addStatusEffect("afterglow", tAdjustedTimeout);
	}

	public void updateHover(float pElapsed)
	{
		if (!isAlive())
		{
			changeMoveJumpOffset((0f - pElapsed) * 10f);
			return;
		}
		if (isOnGround())
		{
			changeMoveJumpOffset((0f - pElapsed) * 3f);
		}
		else if (move_jump_offset.y < asset.hovering_min)
		{
			changeMoveJumpOffset(pElapsed * 3f);
			return;
		}
		if (_hover_timer > 0f)
		{
			_hover_timer -= pElapsed;
			return;
		}
		_hover_timer = 1f + Randy.randomFloat(0f, 4f);
		if (World.world.isPaused())
		{
			return;
		}
		switch (_hover_state)
		{
		case HoverState.Hover:
			if (Randy.randomBool())
			{
				_hover_state = HoverState.Down;
			}
			else
			{
				_hover_state = HoverState.Up;
			}
			break;
		case HoverState.Up:
			_hover_state = HoverState.Hover;
			if (move_jump_offset.y < asset.hovering_max)
			{
				changeMoveJumpOffset(pElapsed * 3f);
			}
			break;
		case HoverState.Down:
			_hover_state = HoverState.Hover;
			if (move_jump_offset.y > asset.hovering_min)
			{
				changeMoveJumpOffset((0f - pElapsed) * 3f);
			}
			break;
		}
	}

	public void updatePollinate(float pElapsed)
	{
		if (!isAlive())
		{
			return;
		}
		if (!is_moving && ai.task?.id == "pollinate")
		{
			setHoverState(HoverState.Down);
			changeMoveJumpOffset((0f - pElapsed) * 3f);
			return;
		}
		setHoverState(HoverState.Up);
		if (move_jump_offset.y < asset.hovering_max)
		{
			changeMoveJumpOffset(pElapsed * 3f);
		}
	}

	private void checkCalibrateTargetPosition()
	{
		if (hasRangeAttack() || beh_actor_target == null)
		{
			return;
		}
		BaseSimObject tTarget = beh_actor_target;
		if (hasTask() && ai.action != null && ai.action.calibrate_target_position && tTarget != null && tTarget.isActor())
		{
			Actor tActorTarget = beh_actor_target.a;
			float num = Toolbox.SquaredDist(tActorTarget.current_tile.x, tActorTarget.current_tile.y, tile_target.x, tile_target.y);
			float tPositionDistance = ai.action.check_actor_target_position_distance * ai.action.check_actor_target_position_distance;
			if (num > tPositionDistance)
			{
				clearPathForCalibration();
				ai.action.startExecute(this);
			}
		}
	}

	internal override bool addStatusEffect(StatusAsset pStatusAsset, float pOverrideTimer = 0f, bool pColorEffect = true)
	{
		if (pStatusAsset.affects_mind && hasTag("strong_mind"))
		{
			return false;
		}
		bool num = base.addStatusEffect(pStatusAsset, pOverrideTimer, pColorEffect);
		if (num && pColorEffect)
		{
			startColorEffect();
		}
		return num;
	}

	public void setTargetAngleZ(float pValue)
	{
		target_angle.z = pValue;
	}

	public void lookTowardsPosition(Vector2 pDirection)
	{
		if (asset.can_flip)
		{
			if (current_position.x < pDirection.x)
			{
				setFlip(pFlip: true);
			}
			else
			{
				setFlip(pFlip: false);
			}
		}
	}

	public override void setStatsDirty()
	{
		if (isAlive())
		{
			batch.c_stats_dirty.Add(this);
		}
		base.setStatsDirty();
		setItemSpriteRenderDirty();
	}

	private void checkRageDemon()
	{
		if (WorldLawLibrary.world_law_disasters_other.isEnabled() && canTurnIntoDemon() && World.world_era.era_disaster_rage_brings_demons && !hasTrait("blessed") && !isFavorite() && hasStatus("rage") && Randy.randomChance(0.1f))
		{
			ActionLibrary.turnIntoDemon(this);
		}
	}

	internal void updateChangeScale(float pElapsed)
	{
		if (actor_scale == target_scale)
		{
			return;
		}
		if (actor_scale > target_scale)
		{
			setActorScale(actor_scale - 0.2f * pElapsed);
			if (actor_scale < target_scale)
			{
				setActorScale(target_scale);
			}
		}
		else
		{
			setActorScale(actor_scale + 0.2f * pElapsed);
			if (actor_scale > target_scale)
			{
				setActorScale(target_scale);
			}
		}
	}

	internal void newCreature()
	{
		changeHappiness("just_born");
		World.world.game_stats.data.creaturesCreated++;
		World.world.map_stats.creaturesCreated++;
		AchievementLibrary.ten_thousands_creatures.check();
		generatePersonality();
		checkShouldBeEgg();
		event_full_stats = true;
		updateStats();
		event_full_stats = true;
		if (needsFood())
		{
			setNutrition(getMaxNutrition());
		}
	}

	public void clearTraits()
	{
		clearTraitCache();
		traits.Clear();
	}

	public override void Dispose()
	{
		WorldBehaviourUnitTemperatures.removeUnit(this);
		clearTraits();
		idle_loop_sound = null;
		checkSimpleComponentListDispose(children_pre_behaviour);
		checkComponentListDispose(children_special);
		_profession = UnitProfession.Nothing;
		sprite_animation = null;
		lover = null;
		idle_loop_sound = null;
		scheduled_tile_target = null;
		_last_main_sprite = null;
		_last_colored_sprite = null;
		_last_color_asset = null;
		_last_topic_sprite = null;
		children_special = null;
		_dict_special = null;
		children_pre_behaviour = null;
		dict_pre_behaviour = null;
		avatar = null;
		clearDecisions();
		if (hasSubspecies())
		{
			World.world.subspecies.unitDied(subspecies);
			subspecies = null;
		}
		if (hasCulture())
		{
			World.world.cultures.unitDied(culture);
			culture = null;
		}
		ai.reset();
		_last_happiness_history.Clear();
		citizen_job = null;
		if (hasCity())
		{
			World.world.cities.unitDied(city);
			city = null;
		}
		if (hasKingdom())
		{
			if (isKing())
			{
				kingdom.removeKing();
			}
			World.world.kingdoms.unitDied(kingdom);
			kingdom = null;
		}
		callbacks_on_death = null;
		callbacks_landed = null;
		callbacks_cancel_path_movement = null;
		callbacks_magnet_update = null;
		s_personality = null;
		_s_special_effect_augmentations.Clear();
		_s_special_effect_augmentations_timers.Clear();
		s_action_attack_target = null;
		targets_to_ignore_timer = null;
		clearOldPath();
		data = null;
		attackedBy = null;
		attack_target = null;
		has_attack_target = false;
		army = null;
		clan = null;
		culture = null;
		family = null;
		language = null;
		plot = null;
		religion = null;
		subspecies = null;
		beh_tile_target = null;
		beh_building_target = null;
		beh_actor_target = null;
		beh_book_target = null;
		exitBuilding();
		inside_boat = null;
		is_inside_boat = false;
		army = null;
		batch = null;
		equipment = null;
		tile_target = null;
		profession_asset = null;
		_next_step_tile = null;
		asset = null;
		frame_data = null;
		animation_container = null;
		_home_building = null;
		cached_sprite_head = null;
		_cached_sprite_item = null;
		_cached_hand_renderer_asset = null;
		_aggression_targets.Clear();
		_current_children = 0;
		is_forced_socialize_icon = false;
		is_forced_socialize_timestamp = 0.0;
		base.Dispose();
	}

	private void checkComponentListDispose(List<BaseActorComponent> pList)
	{
		if (pList != null)
		{
			for (int i = 0; i < pList.Count; i++)
			{
				pList[i].Dispose();
			}
			pList.Clear();
		}
	}

	private void checkSimpleComponentListDispose(List<ActorSimpleComponent> pList)
	{
		if (pList != null)
		{
			for (int i = 0; i < pList.Count; i++)
			{
				pList[i].Dispose();
			}
			pList.Clear();
		}
	}

	public void showTooltip(object pUiObject)
	{
		string tTooltipID = (isKing() ? "actor_king" : ((!isCityLeader()) ? "actor" : "actor_leader"));
		Tooltip.show(pUiObject, tTooltipID, new TooltipData
		{
			actor = this
		});
	}

	public override ColorAsset getColor()
	{
		return kingdom.getColor();
	}

	public void setHoverState(HoverState pState)
	{
		_hover_state = pState;
	}

	public override string ToString()
	{
		if (data == null)
		{
			return "[Actor is null]";
		}
		using StringBuilderPool tBuilder = new StringBuilderPool();
		tBuilder.Append($"[Actor:{base.id} ");
		if (!isAlive())
		{
			tBuilder.Append("[DEAD] ");
		}
		if (!string.IsNullOrEmpty(data.name))
		{
			tBuilder.Append(data.name + " ");
		}
		if (hasCity())
		{
			tBuilder.Append($"City:{city.getID()} ");
			if (city.kingdom != kingdom)
			{
				tBuilder.Append($"CityKingdom:{city.kingdom?.getID() ?? (-1)} ");
			}
			if (city.hasArmy())
			{
				tBuilder.Append($"CityArmy:{city.army.id} ");
			}
		}
		if (hasKingdom())
		{
			tBuilder.Append($"Kingdom:{kingdom.getID()} ");
		}
		if (isKing())
		{
			tBuilder.Append("isKing ");
		}
		if (isCityLeader())
		{
			tBuilder.Append("isCityLeader ");
		}
		if (hasArmy())
		{
			tBuilder.Append($"Army:{army.id} ");
			if (isArmyGroupLeader())
			{
				tBuilder.Append("isArmyGroupLeader ");
			}
			if (isArmyGroupWarrior())
			{
				tBuilder.Append("isArmyGroupWarrior ");
			}
		}
		return tBuilder.ToString().Trim() + "]";
	}

	private int getMaxPossibleLevel()
	{
		return 9999;
	}

	internal void addExperience(int pValue)
	{
		if (pValue == 0 || !asset.can_level_up || !isAlive())
		{
			return;
		}
		if (hasCulture() && culture.hasTrait("fast_learners"))
		{
			pValue *= CultureTraitLibrary.getValue("fast_learners");
		}
		int tMaxLevel = getMaxPossibleLevel();
		if (data.level < tMaxLevel)
		{
			data.experience += pValue;
			if (data.experience >= getExpToLevelup())
			{
				levelUp();
			}
			if (data.level >= tMaxLevel)
			{
				data.experience = getExpToLevelup();
			}
		}
	}

	public void addRenown(int pValue)
	{
		data.renown += pValue;
	}

	public void addRenown(int pAmount, float pPercent)
	{
		int tRenown = (int)((float)pAmount * pPercent);
		addRenown(tRenown);
	}

	internal void updateAge()
	{
		checkGrowthEvent();
		float tAge = getAge();
		if (hasSubspecies())
		{
			subspecies.all_actions_actor_growth?.Invoke(this, current_tile);
			updateAttributes();
		}
		if (hasCity())
		{
			if (isKing())
			{
				addExperience(20);
			}
			if (isCityLeader())
			{
				addExperience(10);
			}
		}
		if (isSapient() && tAge > 300f && hasTrait("immortal") && Randy.randomBool())
		{
			addTrait("evil");
		}
		if (tAge > 40f && Randy.randomChance(0.3f))
		{
			addTrait("wise");
		}
	}

	private void updateAttributes()
	{
		if (Randy.randomChance(0.3f))
		{
			string tAttribute = subspecies.getPossibleAttribute();
			if (!string.IsNullOrEmpty(tAttribute))
			{
				data[tAttribute]++;
			}
		}
	}

	public void setMaxHappiness()
	{
		setHappiness(getMaxHappiness());
	}

	public void setHappiness(int pValue, bool pClamp = true)
	{
		if (pClamp)
		{
			pValue = Math.Clamp(pValue, getMinHappiness(), getMaxHappiness());
		}
		data.happiness = pValue;
	}

	public void restoreHealthPercent(float pVal)
	{
		if (pVal > 0f && !hasMaxHealth())
		{
			int tHealthToRestore = getMaxHealthPercent(pVal);
			restoreHealth(tHealthToRestore);
		}
	}

	public void restoreHealth(int pVal)
	{
		if (!hasMaxHealth())
		{
			changeHealth(pVal);
		}
	}

	public bool changeHappiness(string pID, int pValue = 0)
	{
		if (!hasEmotions())
		{
			return false;
		}
		if (isEgg())
		{
			return false;
		}
		HappinessAsset tHappinessAsset = AssetManager.happiness_library.get(pID);
		if (tHappinessAsset.ignored_by_psychopaths && hasTrait("psychopath"))
		{
			return false;
		}
		int tValueToAdd = pValue + tHappinessAsset.value;
		int tFinalHappiness = getHappiness() + tValueToAdd;
		tFinalHappiness = Mathf.Clamp(tFinalHappiness, getMinHappiness(), getMaxHappiness());
		setHappiness(tFinalHappiness);
		if (tHappinessAsset.show_change_happiness_effect)
		{
			if (tValueToAdd > 0)
			{
				EffectsLibrary.showMetaEventEffect("fx_change_happiness_positive", this);
			}
			else if (tValueToAdd < 0)
			{
				EffectsLibrary.showMetaEventEffect("fx_change_happiness_negative", this);
			}
		}
		_last_happiness_history.Enqueue(new HappinessHistory
		{
			index = tHappinessAsset.index,
			timestamp = World.world.getCurWorldTime(),
			bonus = pValue
		});
		if (_last_happiness_history.Count > 20)
		{
			_last_happiness_history.Dequeue();
		}
		return true;
	}

	public void spendNutritionOnBirth()
	{
		decreaseNutrition(SimGlobals.m.nutrition_cost_new_baby);
	}

	public void addNutritionFromEating(int pVal = 100, bool pSetMaxNutrition = false, bool pSetJustAte = false)
	{
		if (pSetMaxNutrition)
		{
			setNutrition(getMaxNutrition());
		}
		else
		{
			int tNewValue = Math.Min(getMaxNutrition(), data.nutrition + pVal);
			setNutrition(tNewValue);
		}
		if (pSetJustAte)
		{
			justAte();
		}
	}

	public void updateNutritionDecay(bool pDoStarvationDamage = true)
	{
		int tNutritionSpent = subspecies.getMetabolicRate();
		decreaseNutrition(tNutritionSpent);
		if (isStarving())
		{
			setNutrition(0);
			if (pDoStarvationDamage)
			{
				int tDamage = getMaxHealthPercent(SimGlobals.m.starvation_damage_multiplier);
				getHit(tDamage, pFlash: true, AttackType.Starvation);
				if (isAlive())
				{
					addStatusEffect("starving", 0f, pColorEffect: false);
				}
			}
		}
		else
		{
			updateStamina();
			updateMana();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void decreaseNutrition(int pValue = -1)
	{
		setNutrition(getNutrition() - pValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void setNutrition(int pVal, bool pClamp = true)
	{
		if (pClamp)
		{
			pVal = Math.Clamp(pVal, 0, getMaxNutrition());
		}
		data.nutrition = pVal;
	}

	public void updateMana()
	{
		if (!isManaFull())
		{
			addMana(SimGlobals.m.mana_change);
		}
	}

	public void addMana(int pValue)
	{
		int tValueMax = getMaxMana();
		int tValueCurrent = getMana();
		if (tValueCurrent < tValueMax)
		{
			tValueCurrent += pValue;
		}
		tValueCurrent = Math.Clamp(tValueCurrent, 0, tValueMax);
		setMana(tValueCurrent);
	}

	public int getMaxManaPercent(float pPercent)
	{
		int tResult = (int)((float)getMaxMana() * pPercent);
		return Mathf.Max(1, tResult);
	}

	public void restoreManaPercent(float pVal)
	{
		if (pVal > 0f && !hasMaxMana())
		{
			int tManaToRestore = getMaxManaPercent(pVal);
			restoreMana(tManaToRestore);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void changeMana(int pValue)
	{
		int tNewValue = data.mana + pValue;
		data.mana = Mathf.Clamp(tNewValue, 0, getMaxMana());
	}

	public void restoreMana(int pVal)
	{
		if (!hasMaxMana())
		{
			changeMana(pVal);
		}
	}

	public void setMana(int pValue, bool pClamp = true)
	{
		if (pClamp)
		{
			pValue = Math.Clamp(pValue, 0, getMaxMana());
		}
		data.mana = pValue;
	}

	public void spendMana(int pValueSpend)
	{
		if (pValueSpend != 0)
		{
			setMana(getMana() - pValueSpend);
		}
	}

	public int getMaxStaminaPercent(float pPercent)
	{
		int tResult = (int)((float)getMaxStamina() * pPercent);
		return Mathf.Max(1, tResult);
	}

	public void restoreStaminaPercent(float pVal)
	{
		if (pVal > 0f && !isStaminaFull())
		{
			int tStaminaToRestore = getMaxStaminaPercent(pVal);
			restoreStamina(tStaminaToRestore);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void changeStamina(int pValue)
	{
		int tNewValue = data.stamina + pValue;
		data.stamina = Mathf.Clamp(tNewValue, 0, getMaxStamina());
	}

	public void restoreStamina(int pVal)
	{
		if (!isStaminaFull())
		{
			changeStamina(pVal);
		}
	}

	public void updateStamina()
	{
		if (!isStaminaFull())
		{
			addStamina(SimGlobals.m.stamina_change);
		}
	}

	public void addStamina(int pValue)
	{
		int tValueMax = getMaxStamina();
		int tValueCurrent = getStamina();
		if (tValueCurrent < tValueMax)
		{
			tValueCurrent += pValue;
		}
		tValueCurrent = Math.Clamp(tValueCurrent, 0, tValueMax);
		setStamina(tValueCurrent);
	}

	public void setStamina(int pValue, bool pClamp = true)
	{
		if (pClamp)
		{
			pValue = Math.Clamp(pValue, 0, getMaxStamina());
		}
		data.stamina = pValue;
	}

	public void spendStamina(int pValueSpend)
	{
		if (pValueSpend != 0)
		{
			setStamina(getStamina() - pValueSpend);
		}
	}

	public void spendStaminaWithCooldown(int pValueSpend)
	{
		if (pValueSpend != 0 && !isUnderStaminaCooldown())
		{
			_last_stamina_reduce_timestamp = World.world.getCurSessionTime();
			setStamina(getStamina() - pValueSpend);
		}
	}

	public bool hasHappinessEntry(string pID, float pTime = 0f)
	{
		if (!hasHappinessHistory())
		{
			return false;
		}
		foreach (HappinessHistory tHappinessEntry in happiness_change_history)
		{
			if (!(tHappinessEntry.asset.id != pID))
			{
				if (pTime == 0f)
				{
					return true;
				}
				if (tHappinessEntry.elapsedSince() < (double)pTime)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void finishScale()
	{
		setActorScale(target_scale);
	}

	public void setActorScale(float pVal)
	{
		actor_scale = pVal;
		current_scale.Set(actor_scale, actor_scale, 1f);
	}

	public void setData(ActorData pData)
	{
		data = pData;
	}

	public void loadData(ActorData pData)
	{
		setData(pData);
		pData.load();
	}

	public void generateSex()
	{
		if (Randy.randomBool())
		{
			data.sex = ActorSex.Male;
		}
		else
		{
			data.sex = ActorSex.Female;
		}
	}

	protected void generatePersonality()
	{
		if (hasSubspecies())
		{
			foreach (ActorTrait tTrait in subspecies.getActorBirthTraits().getTraits())
			{
				addTrait(tTrait);
			}
			if (subspecies.hasPhenotype())
			{
				generatePhenotypeAndShade();
			}
		}
		else
		{
			generateRandomSpawnTraits(asset);
		}
		if (isAdult())
		{
			checkTraitMutationGrowUp();
		}
		checkTraitMutationOnBirth();
		generateSex();
		setStatsDirty();
	}

	public void calcIsEgg()
	{
		if (hasSubspecies() && subspecies.has_egg_form)
		{
			_state_egg = hasStatus("egg");
		}
	}

	public void calcIsBaby()
	{
		if (hasSubspecies() && asset.has_baby_form && !((float)getAge() >= subspecies.age_adult))
		{
			_state_baby = true;
			clearSprites();
		}
	}

	public void setCheckLanding()
	{
		should_check_land_cancel = true;
	}

	public void addForce(float pX, float pY, float pHeight, bool pCheckLandCancelAllActions = false, bool pIgnorePosHeight = false)
	{
		if (asset.can_be_moved_by_powers)
		{
			if (pCheckLandCancelAllActions)
			{
				setCheckLanding();
			}
			if (pIgnorePosHeight || !(position_height > 0f))
			{
				velocity.x = pX;
				velocity.y = pY;
				velocity.z = pHeight;
				velocity_speed = pHeight;
				under_forces = true;
			}
		}
	}

	public void setFlying(bool pVal)
	{
		_flying = pVal;
		if (pVal)
		{
			hitbox_bonus_height = 8f;
		}
		else
		{
			hitbox_bonus_height = 2f;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void checkIsInLiquid()
	{
		bool tInLiquid = current_tile.is_liquid && move_jump_offset.y == 0f && position_height <= 0f && isAlive();
		_is_in_liquid = tInLiquid;
	}

	private void addDefaultItemAttackActions(ItemAsset pItemAsset)
	{
		addItemActions(pItemAsset);
		if (pItemAsset.action_attack_target != null)
		{
			s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, pItemAsset.action_attack_target);
		}
	}

	private void addItemActions(ItemAsset pItemAsset)
	{
		if (pItemAsset.action_special_effect != null)
		{
			_s_special_effect_augmentations.Add(pItemAsset);
		}
	}

	internal void attackTargetActions(BaseSimObject pTarget, WorldTile pTile)
	{
		s_action_attack_target?.Invoke(this, pTarget, pTile);
	}

	protected void calcAgeStates()
	{
		_state_egg = false;
		_state_baby = false;
		_state_adult = false;
		calcIsEgg();
		if (!isEgg())
		{
			calcIsBaby();
			if (!isBaby())
			{
				_state_adult = true;
				clearSprites();
			}
		}
		else
		{
			_state_baby = true;
			clearSprites();
		}
	}

	internal override void updateStats()
	{
		if (!isStatsDirty())
		{
			return;
		}
		base.updateStats();
		checkGrowthEvent();
		decisions_counter = 0;
		batch.c_stats_dirty.Remove(a);
		if (!isAlive())
		{
			return;
		}
		s_action_attack_target = null;
		s_get_hit_action = null;
		_s_special_effect_augmentations.Clear();
		_s_special_effect_augmentations_timers.Clear();
		stats.clear();
		clearCombatActions();
		clearSpells();
		if (hasSubspecies())
		{
			stats.mergeStats(subspecies.base_stats);
			if (isSexMale())
			{
				stats.mergeStats(subspecies.base_stats_male);
			}
			else
			{
				stats.mergeStats(subspecies.base_stats_female);
			}
		}
		else
		{
			stats.mergeStats(asset.base_stats);
		}
		if (hasClan())
		{
			stats.mergeStats(clan.base_stats);
			if (isSexMale())
			{
				stats.mergeStats(clan.base_stats_male);
			}
			else
			{
				stats.mergeStats(clan.base_stats_female);
			}
		}
		if (hasLanguage())
		{
			stats.mergeStats(language.base_stats);
		}
		if (hasCulture())
		{
			stats.mergeStats(culture.base_stats);
		}
		stats["diplomacy"] += data["diplomacy"];
		stats["stewardship"] += data["stewardship"];
		stats["intelligence"] += data["intelligence"];
		stats["warfare"] += data["warfare"];
		_cache_check_has_status_removed_on_damage = false;
		if (hasAnyStatusEffect())
		{
			foreach (Status tStatus in getStatuses())
			{
				stats.mergeStats(tStatus.asset.base_stats);
				if (tStatus.asset.removed_on_damage)
				{
					_cache_check_has_status_removed_on_damage = true;
				}
				if (!string.IsNullOrEmpty(tStatus.asset.decision_id))
				{
					decisions[decisions_counter++] = tStatus.asset.getDecisionAsset();
				}
			}
		}
		if (!hasWeapon())
		{
			EquipmentAsset tDefaultWeapon = AssetManager.items.get(asset.default_attack);
			if (tDefaultWeapon != null)
			{
				stats.mergeStats(tDefaultWeapon.base_stats);
			}
		}
		checkAttackTypes();
		foreach (ActorTrait tTrait in traits)
		{
			if (!tTrait.only_active_on_era_flag || ((!tTrait.era_active_moon || World.world_era.flag_moon) && (!tTrait.era_active_night || World.world_era.overlay_darkness)))
			{
				if (tTrait.action_get_hit != null)
				{
					s_get_hit_action = (GetHitAction)Delegate.Combine(s_get_hit_action, tTrait.action_get_hit);
				}
				stats.mergeStats(tTrait.base_stats);
			}
		}
		is_forced_socialize_icon = hasStatus("possessed") && hasTag("strong_mind");
		if (hasStatus("budding"))
		{
			stats["diplomacy"] *= 2f;
			stats["stewardship"] *= 2f;
			stats["intelligence"] *= 2f;
			stats["warfare"] *= 2f;
		}
		if (isSapient())
		{
			s_personality = null;
			if (isKing() || isCityLeader())
			{
				string tPersonality = "balanced";
				float tHighStat = stats["diplomacy"];
				if (stats["diplomacy"] > stats["stewardship"])
				{
					tPersonality = "diplomat";
					tHighStat = stats["diplomacy"];
				}
				else if (stats["diplomacy"] < stats["stewardship"])
				{
					tPersonality = "administrator";
					tHighStat = stats["stewardship"];
				}
				if (stats["warfare"] > tHighStat)
				{
					tPersonality = "militarist";
				}
				s_personality = AssetManager.personalities.get(tPersonality);
				stats.mergeStats(s_personality.base_stats);
			}
		}
		float tBonusFromLevelHealth = (float)data.level * SimGlobals.m.level_mod_bonus_health * stats["health"];
		float tBonusFromLevelMana = (float)data.level * SimGlobals.m.level_mod_bonus_mana * stats["mana"];
		float tBonusFromLevelStamina = (float)data.level * SimGlobals.m.level_mod_bonus_stamina * stats["stamina"];
		stats["health"] += tBonusFromLevelHealth;
		stats["mana"] += tBonusFromLevelMana;
		stats["stamina"] += tBonusFromLevelStamina;
		stats["skill_combat"] += (float)(int)(stats["warfare"] / 5f) * 0.01f;
		stats["skill_spell"] += (float)(int)(stats["intelligence"] / 5f) * 0.01f;
		if (data.level > 5)
		{
			stats["skill_combat"] += 0.1f;
			stats["skill_spell"] += 0.1f;
		}
		addSpecialEffectAugmentations(traits);
		checkActionsFromAllMetas();
		recalcCombatActions();
		recalcSpells();
		registerDecisions();
		bool tHadStatusUnconscious = _has_tag_unconscious;
		has_tag_generate_light = hasTag("generate_light");
		_has_tag_unconscious = hasTag("unconscious");
		has_tag_immunity_cold = hasTag("immunity_cold");
		if (_has_tag_unconscious)
		{
			if (!tHadStatusUnconscious)
			{
				if (batch.rnd.NextBool())
				{
					_rotation_direction = RotationDirection.Left;
				}
				else
				{
					_rotation_direction = RotationDirection.Right;
				}
			}
			timer_jump_animation = 0f;
		}
		_has_trait_weightless = hasTrait("weightless");
		_has_status_sleeping = hasStatus("sleeping");
		_has_status_strange_urge = hasStatus("strange_urge");
		_has_status_possessed = hasStatus("possessed");
		_has_status_tantrum = hasStatus("tantrum");
		_has_status_drowning = hasStatus("drowning");
		_has_status_invincible = hasStatus("invincible");
		is_immovable = isImmovable();
		is_ai_frozen = isAiFrozen();
		_has_stop_idle_animation = hasStopIdleAnimation();
		_ignore_fights = isIgnoreFights();
		if (hasSubspecies())
		{
			_has_emotions = subspecies.can_process_emotions;
		}
		else
		{
			_has_emotions = false;
		}
		if (!hasWeapon())
		{
			EquipmentAsset tDefaultItemAttackAsset = AssetManager.items.get(asset.default_attack);
			addDefaultItemAttackActions(tDefaultItemAttackAsset);
			if (tDefaultItemAttackAsset.item_modifiers != null)
			{
				for (int i = 0; i < tDefaultItemAttackAsset.item_modifiers.Length; i++)
				{
					ItemModAsset tModData = tDefaultItemAttackAsset.item_modifiers[i];
					if (tModData != null)
					{
						addDefaultItemAttackActions(tModData);
					}
				}
			}
		}
		if (canUseItems())
		{
			foreach (ActorEquipmentSlot tSlot in equipment)
			{
				if (tSlot.isEmpty())
				{
					continue;
				}
				Item tItem = tSlot.getItem();
				addItemActions(tItem.getAsset());
				if (tItem.action_attack_target != null)
				{
					s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, tItem.action_attack_target);
				}
				foreach (ref string modifier in tItem.data.modifiers)
				{
					string tModID = modifier;
					ItemModAsset tModData2 = AssetManager.items_modifiers.get(tModID);
					addItemActions(tModData2);
				}
			}
		}
		if (_s_special_effect_augmentations.Count == 0)
		{
			batch.c_augmentation_effects.Remove(a);
		}
		else
		{
			batch.c_augmentation_effects.Add(a);
		}
		_has_any_sick_trait = calculateIsSick();
		_has_trait_peaceful = hasTrait("peaceful");
		_has_trait_clone = hasTrait("clone");
		if (canUseItems())
		{
			foreach (ActorEquipmentSlot tSlot2 in equipment)
			{
				if (!tSlot2.isEmpty())
				{
					Item tItem2 = tSlot2.getItem();
					float tStatsMultiplier = 1f;
					if (tItem2.isBroken())
					{
						tStatsMultiplier = 0.5f;
					}
					ItemTools.mergeStatsWithItem(stats, tItem2, pCalcGlobalValue: false, tStatsMultiplier);
				}
			}
		}
		if (asset.only_melee_attack)
		{
			stats["range"] = asset.base_stats["range"];
		}
		stats.normalize();
		stats["cities"] += (int)stats["stewardship"] / 6 + 1;
		stats["bonus_towers"] += (int)(stats["warfare"] / 10f);
		stats["mana"] += (int)(stats["intelligence"] * SimGlobals.m.MANA_PER_INTELLIGENCE);
		stats.checkMultipliers();
		if (isSapient())
		{
			calculateOffspringBasedOnAge();
		}
		if (hasRangeAttack())
		{
			stats["range"] += stats["range"] * World.world_era.range_weapons_multiplier;
		}
		stats["damage"] += stats["warfare"] / 5f;
		if (isBaby())
		{
			stats["damage"] = stats["damage"] * 0.5f;
			stats["health"] = stats["health"] * 0.5f;
		}
		stats.normalize();
		if (getHealth() > getMaxHealth())
		{
			setMaxHealth();
		}
		if (getHappiness() > getMaxHappiness())
		{
			setMaxHappiness();
		}
		if (getStamina() > getMaxStamina())
		{
			setMaxStamina();
		}
		if (getMana() > getMaxMana())
		{
			setMaxMana();
		}
		if (event_full_stats)
		{
			event_full_stats = false;
			setMaxHealth();
			setMaxStamina();
			setMaxMana();
		}
		if (isHovering())
		{
			batch.c_hovering.Add(a);
		}
		else
		{
			move_jump_offset.y = 0f;
			batch.c_hovering.Remove(a);
		}
		if (isPollinator())
		{
			batch.c_pollinating.Add(a);
		}
		else
		{
			batch.c_pollinating.Remove(a);
		}
		target_scale = stats["scale"];
		if (attack_timer > getAttackCooldown())
		{
			attack_timer = getAttackCooldown();
		}
	}

	public void resetAttackTimeout()
	{
		attack_timer = 0f;
	}

	public void setActionTimeout(float pTimeout)
	{
		attack_timer = pTimeout;
	}

	private void addSpecialEffectAugmentations(IEnumerable<BaseAugmentationAsset> pAssets)
	{
		foreach (BaseAugmentationAsset tAugmentation in pAssets)
		{
			if (tAugmentation.action_special_effect != null)
			{
				_s_special_effect_augmentations.Add(tAugmentation);
			}
			if (tAugmentation.action_attack_target != null)
			{
				s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, tAugmentation.action_attack_target);
			}
		}
	}

	private void addSpecialEffectsFromMetas(List<BaseAugmentationAsset> pAugmentations)
	{
		if (pAugmentations != null && pAugmentations.Count != 0)
		{
			_s_special_effect_augmentations.AddRange(pAugmentations);
		}
	}

	private void calculateOffspringBasedOnAge()
	{
		if (!hasTrait("immortal"))
		{
			int tBaseOffspring = (int)stats["offspring"];
			float tMaxAgeRatio = getAgeRatio();
			float tAgeFertilityMod = 1f;
			if (tMaxAgeRatio > 0.9f)
			{
				tAgeFertilityMod = 0.1f;
			}
			else if (tMaxAgeRatio > 0.7f)
			{
				tAgeFertilityMod = 0.2f;
			}
			else if (tMaxAgeRatio > 0.5f)
			{
				tAgeFertilityMod = 0.5f;
			}
			else if (tMaxAgeRatio > 0.3f)
			{
				tAgeFertilityMod = 0.8f;
			}
			stats["offspring"] = (int)Math.Ceiling((float)tBaseOffspring * tAgeFertilityMod);
		}
	}

	internal virtual void updateFall()
	{
		if (position_height < 0f)
		{
			return;
		}
		float tElapsed = World.world.elapsed;
		float tF = SimGlobals.m.gravity * stats.get("mass");
		position_height -= tF * tElapsed;
		if (position_height <= 0f)
		{
			position_height = 0f;
			if (!under_forces)
			{
				stopForce();
			}
		}
	}

	private void stopForce()
	{
		position_height = 0f;
		velocity = Vector3.zero;
		under_forces = false;
		batch.c_action_landed.Add(this);
	}

	internal virtual void actionLanded()
	{
		batch.c_action_landed.Remove(this);
		dirty_current_tile = true;
		callbacks_landed?.Invoke(this);
		if (_action_wait_after_land)
		{
			_action_wait_after_land = false;
			makeWait(_action_wait_after_land_timer);
		}
		checkStepActionForTile(current_tile);
	}

	public void updateShake(float pElapsed)
	{
		if (!_shake_active)
		{
			return;
		}
		_shake_timer -= pElapsed;
		if (_shake_timer <= 0f)
		{
			shake_offset.Set(0f, 0f);
			_shake_active = false;
			batch.c_shake.Remove(this);
			return;
		}
		if (_shake_vertical)
		{
			shake_offset.y = batch.rnd.NextFloat(0f - _shake_volume, _shake_volume);
		}
		if (_shake_horizontal)
		{
			shake_offset.x = batch.rnd.NextFloat(0f - _shake_volume, _shake_volume);
		}
	}

	internal void updateFlipRotation(float pElapsed)
	{
		if (!asset.can_flip)
		{
			return;
		}
		if (flip)
		{
			flip_angle += pElapsed * 600f;
			if (flip_angle > 180f)
			{
				flip_angle = 180f;
			}
		}
		else
		{
			flip_angle -= pElapsed * 600f;
			if (flip_angle < 0f)
			{
				flip_angle = 0f;
			}
		}
		target_angle.y = flip_angle;
	}

	internal bool flipAnimationActive()
	{
		if (!asset.can_flip)
		{
			return false;
		}
		if (flip)
		{
			return flip_angle != 180f;
		}
		return flip_angle != 0f;
	}

	private void updateRotations(float pElapsed)
	{
		if (rotation_cooldown > 0f)
		{
			rotation_cooldown -= pElapsed;
		}
		else if (is_unconscious)
		{
			updateRotationFall(pElapsed);
		}
		else
		{
			updateRotationBack(pElapsed);
		}
	}

	private void updateRotationFall(float pElapsed)
	{
		if (getTextureAsset().prevent_unconscious_rotation)
		{
			return;
		}
		if (current_tile.is_liquid && _is_in_liquid)
		{
			target_angle.z = 0f;
			return;
		}
		if (_rotation_direction == RotationDirection.Left && target_angle.z != -90f)
		{
			target_angle.z -= 230f * pElapsed;
			if (target_angle.z < -90f)
			{
				target_angle.z = -90f;
			}
		}
		if (_rotation_direction == RotationDirection.Right && target_angle.z != 90f)
		{
			target_angle.z += 300f * pElapsed;
			if (target_angle.z > 90f)
			{
				target_angle.z = 90f;
			}
		}
	}

	private void updateRotationBack(float pElapsed)
	{
		if (target_angle.z == 0f)
		{
			return;
		}
		if (target_angle.z < 0f)
		{
			target_angle.z += 300f * pElapsed;
			if (target_angle.z > 0f)
			{
				target_angle.z = 0f;
			}
		}
		if (target_angle.z > 0f)
		{
			target_angle.z -= 300f * pElapsed;
			if (target_angle.z < 0f)
			{
				target_angle.z = 0f;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 updateRotation()
	{
		if (current_rotation.y == target_angle.y && current_rotation.z == target_angle.z)
		{
			return current_rotation;
		}
		current_rotation.Set(target_angle.x, target_angle.y, target_angle.z);
		return current_rotation;
	}

	internal void updateDeadBlackAnimation(float pElapsed)
	{
		if (_death_timer_color_stage_1 > 0f)
		{
			_death_timer_color_stage_1 -= pElapsed;
			if (_death_timer_color_stage_1 <= 0f)
			{
				_death_timer_color_stage_1 = 0f;
			}
		}
		if (_death_timer_color_stage_1 > 0f)
		{
			Color tColor = new Color(_death_timer_color_stage_1, _death_timer_color_stage_1, _death_timer_color_stage_1, 1f);
			color = tColor;
		}
		else if (_death_timer_alpha_stage_2 > 0f)
		{
			_death_timer_alpha_stage_2 -= 1f * pElapsed;
			if (_death_timer_alpha_stage_2 <= 0f)
			{
				die(pDestroy: true, AttackType.None, pCountDeath: false);
				return;
			}
			Color tColor2 = new Color(_death_timer_color_stage_1, _death_timer_color_stage_1, _death_timer_color_stage_1, _death_timer_alpha_stage_2);
			color = tColor2;
		}
	}

	internal virtual void spawnOn(WorldTile pTile, float pZHeight = 0f)
	{
		setCurrentTilePosition(pTile);
		position_height = pZHeight;
		hitbox_bonus_height = asset.default_height;
	}

	public string getName()
	{
		if (string.IsNullOrEmpty(data.name))
		{
			generateNewName();
			AchievementLibrary.child_named_toto.checkBySignal(data.name);
		}
		return data.name;
	}

	public string generateName(MetaType pType, long pSeed, ActorSex pSex = ActorSex.None)
	{
		return NameGenerator.generateName(this, pType, pSeed + World.world.map_stats.life_dna, pSex);
	}

	private void generateNewName()
	{
		ActorSex tSex = (isSapient() ? data.sex : ActorSex.None);
		long tUnitID = getID();
		long tSeed = World.world.map_stats.life_dna + tUnitID * 543;
		string tNewName = NameGenerator.generateName(this, MetaType.Unit, tSeed, tSex);
		setName(tNewName);
		data.name_culture_id = culture?.id ?? (-1);
	}

	public override void trackName(bool pPostChange = false)
	{
		if (!string.IsNullOrEmpty(data.name) && (!pPostChange || (data.past_names != null && data.past_names.Count != 0)))
		{
			ActorData actorData = data;
			if (actorData.past_names == null)
			{
				actorData.past_names = new List<NameEntry>();
			}
			if (data.past_names.Count == 0)
			{
				NameEntry tNewEntry = new NameEntry(data.name, pCustom: false, data.created_time);
				data.past_names.Add(tNewEntry);
			}
			else if (!(data.past_names.Last().name == data.name))
			{
				NameEntry tNewEntry2 = new NameEntry(data.name, data.custom_name);
				data.past_names.Add(tNewEntry2);
			}
		}
	}

	public void setHomeBuilding(Building pBuilding)
	{
		if (_home_building != null)
		{
			clearHomeBuilding();
		}
		_home_building = pBuilding;
		_home_building.residents.Add(data.id);
		World.world.buildings.event_houses = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasHomeBuilding()
	{
		return getHomeBuilding() != null;
	}

	public Building getHomeBuilding()
	{
		checkHomeBuilding();
		return _home_building;
	}

	public void checkHomeBuilding()
	{
		if (_home_building != null)
		{
			if (!_home_building.isUsable() || _home_building.isAbandoned())
			{
				clearHomeBuilding();
				changeHappiness("just_lost_house");
			}
			else if (_home_building.asset.city_building && _home_building.city != city)
			{
				clearHomeBuilding();
				changeHappiness("just_lost_house");
			}
		}
	}

	public void cloneTopicSprite(Sprite pSprite)
	{
		_last_topic_sprite = pSprite;
	}

	public void clearLastTopicSprite()
	{
		_last_topic_sprite = null;
	}

	public Sprite getTopicSpriteTrait()
	{
		if (traits.Count == 0)
		{
			return null;
		}
		return traits.GetRandom().getSprite();
	}

	public Sprite getSocializeTopic()
	{
		if (_last_topic_sprite == null)
		{
			_last_topic_sprite = AssetManager.communication_topic_library.getTopicSprite(this);
		}
		return _last_topic_sprite;
	}

	public void forceSocializeTopic(string pPath)
	{
		_last_topic_sprite = SpriteTextureLoader.getSprite(pPath);
		is_forced_socialize_timestamp = World.world.getCurWorldTime();
	}

	public void clearHomeBuilding()
	{
		_home_building = null;
		World.world.buildings.event_houses = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void setAlive(bool pValue)
	{
		_alive = pValue;
		if (!pValue && data.died_time == 0.0)
		{
			data.died_time = World.world.getCurWorldTime();
		}
		if (!pValue)
		{
			World.world.units.somethingChanged();
		}
	}

	internal bool isProfession(UnitProfession pType)
	{
		return _profession == pType;
	}

	public bool isAnimal()
	{
		if (isSapient())
		{
			return false;
		}
		if (asset.unit_other)
		{
			return false;
		}
		return asset.default_animal;
	}

	public bool isNomad()
	{
		return !isKingdomCiv();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isSapient()
	{
		if (hasSubspecies())
		{
			return subspecies.isSapient();
		}
		return false;
	}

	public bool isPrettyOld()
	{
		int tAge = getAge();
		if (tAge <= 1)
		{
			return false;
		}
		if ((float)tAge < subspecies.age_adult)
		{
			return false;
		}
		return getAgeRatio() > 0.7f;
	}

	public bool isBaby()
	{
		return _state_baby;
	}

	public bool isAdult()
	{
		return _state_adult;
	}

	public bool isBreedingAge()
	{
		if (!hasSubspecies())
		{
			return false;
		}
		return (float)getAge() >= subspecies.age_breeding;
	}

	public bool isEgg()
	{
		return _state_egg;
	}

	public int getAge()
	{
		return data.getAge();
	}

	public string getBirthday()
	{
		return Date.getDate(data.created_time);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isKing()
	{
		if (!hasKingdom())
		{
			return false;
		}
		return kingdom.king == this;
	}

	public float getMaturationTimeSeconds()
	{
		return getMaturationTimeMonths() * 5f;
	}

	public float getMaturationTimeMonths()
	{
		float tResult = 0f;
		if (hasSubspecies())
		{
			tResult += subspecies.getMaturationTimeMonths();
		}
		return tResult;
	}

	public bool isFavorite()
	{
		return data.favorite;
	}

	public void switchFavorite()
	{
		data.favorite = !data.favorite;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override City getCity()
	{
		return city;
	}

	public bool canBuildNewCity()
	{
		if (base.current_zone.hasCity())
		{
			return false;
		}
		if (hasCity())
		{
			return false;
		}
		if (!base.current_zone.isGoodForNewCity(this))
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isCityLeader()
	{
		if (!hasCity())
		{
			return false;
		}
		return city.leader == this;
	}

	public override bool hasDied()
	{
		return data.died_time > 0.0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isPollinator()
	{
		return subspecies?.has_trait_pollinating ?? false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isAffectedByLiquid()
	{
		if (isInAir())
		{
			return false;
		}
		return _is_in_liquid;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool isInAir()
	{
		if (!_flying && !isHovering())
		{
			return isInMagnet();
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool isFlying()
	{
		return _flying;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool ignoresBlocks()
	{
		if (!asset.ignore_blocks && !isFlying())
		{
			return isInMagnet();
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isInMagnet()
	{
		return is_in_magnet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isHovering()
	{
		return subspecies?.has_trait_hovering ?? false;
	}

	public ActorAsset getActorAsset()
	{
		return asset;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IReadOnlyCollection<ActorTrait> getTraits()
	{
		return traits;
	}

	public bool isWaterCreature()
	{
		if (!asset.force_ocean_creature)
		{
			return subspecies?.has_trait_water_creature ?? false;
		}
		return true;
	}

	public bool mustAvoidGround()
	{
		if (isWaterCreature())
		{
			return !asset.force_land_creature;
		}
		return false;
	}

	public bool isInStablePlace()
	{
		if (mustAvoidGround())
		{
			if (current_tile.Type.ground)
			{
				return false;
			}
		}
		else
		{
			if (current_tile.Type.ocean && !isWaterCreature())
			{
				return false;
			}
			if (current_tile.Type.lava && asset.die_in_lava)
			{
				return false;
			}
		}
		return true;
	}

	internal bool hasWeapon()
	{
		if (canUseItems())
		{
			return !equipment.weapon.isEmpty();
		}
		return false;
	}

	internal Item getWeapon()
	{
		if (hasWeapon())
		{
			return equipment.weapon.getItem();
		}
		return null;
	}

	internal EquipmentAsset getWeaponAsset()
	{
		if (hasWeapon())
		{
			return equipment.weapon.getItem().getAsset();
		}
		return AssetManager.items.get(asset.default_attack);
	}

	public bool isWeaponFirearm()
	{
		return getWeapon()?.getAsset().group_id == "firearm";
	}

	public bool isArmyGroupLeader()
	{
		if (hasArmy())
		{
			return army.getCaptain() == this;
		}
		return false;
	}

	public bool isArmyGroupWarrior()
	{
		if (hasArmy())
		{
			return army.getCaptain() != this;
		}
		return false;
	}

	public bool hasTraits()
	{
		return traits.Count > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isSexMale()
	{
		return data.sex == ActorSex.Male;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isSexFemale()
	{
		return data.sex == ActorSex.Female;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasEquipment()
	{
		return equipment != null;
	}

	public bool hasHouse()
	{
		return getHomeBuilding() != null;
	}

	public bool hasLover()
	{
		return lover != null;
	}

	public bool hasBestFriend()
	{
		return getBestFriend() != null;
	}

	public Actor getBestFriend()
	{
		if (data.best_friend_id.hasValue())
		{
			return World.world.units.get(data.best_friend_id);
		}
		return null;
	}

	public bool isChildOf(Actor pActor)
	{
		return isChildOf(pActor.data.id);
	}

	public bool isChildOf(long pID)
	{
		if (data.parent_id_1 == pID)
		{
			return true;
		}
		if (data.parent_id_2 == pID)
		{
			return true;
		}
		return false;
	}

	public bool isParentOf(long pID, Actor pActor)
	{
		if (pID == pActor.data.parent_id_1)
		{
			return true;
		}
		if (pID == pActor.data.parent_id_2)
		{
			return true;
		}
		return false;
	}

	public bool isParentOf(Actor pActor)
	{
		return isParentOf(data.id, pActor);
	}

	public IEnumerable<Actor> getParents()
	{
		Actor tParent1 = World.world.units.get(data.parent_id_1);
		if (tParent1 != null && tParent1.isAlive())
		{
			yield return tParent1;
		}
		Actor tParent2 = World.world.units.get(data.parent_id_2);
		if (tParent2 != null && tParent2.isAlive())
		{
			yield return tParent2;
		}
	}

	public IEnumerable<Actor> getChildren(bool pOnlyCurrentFamily = true)
	{
		if (pOnlyCurrentFamily)
		{
			if (!hasFamily())
			{
				yield break;
			}
			Family tFamily = family;
			foreach (Actor tChild in tFamily.units)
			{
				if (tChild != this && tChild.isChildOf(this))
				{
					yield return tChild;
				}
			}
			yield break;
		}
		int tCurrentLivingChildren = current_children_count;
		if (tCurrentLivingChildren == 0)
		{
			yield break;
		}
		long tParentID = data.id;
		if (!hasSubspecies() || subspecies.isRekt())
		{
			yield break;
		}
		foreach (Actor tChild2 in subspecies.units)
		{
			if (!tChild2.isRekt() && tChild2 != this && tChild2.isChildOf(tParentID))
			{
				tCurrentLivingChildren--;
				yield return tChild2;
				if (tCurrentLivingChildren == 0)
				{
					break;
				}
			}
		}
	}

	public bool hasSuitableBookTraits()
	{
		foreach (ActorTrait trait in getTraits())
		{
			if (trait.group_id == "mind")
			{
				return true;
			}
		}
		return false;
	}

	public bool canBeSurprised(WorldTile pFromTile = null)
	{
		if (!_has_emotions)
		{
			return false;
		}
		if (!asset.can_be_surprised)
		{
			return false;
		}
		if (isFighting())
		{
			return false;
		}
		if (isInsideSomething())
		{
			return false;
		}
		if (is_unconscious)
		{
			return false;
		}
		if (isEgg())
		{
			return false;
		}
		if (hasTask() && ai.task.ignore_fight_check)
		{
			return false;
		}
		return true;
	}

	public bool isTileOnTheLeft(WorldTile pTile)
	{
		return current_tile.x > pTile.x;
	}

	public bool isFighting()
	{
		if (has_attack_target)
		{
			return true;
		}
		return false;
	}

	public UnitProfession getProfession()
	{
		return _profession;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int getNutrition()
	{
		return data.nutrition;
	}

	public bool isHungry()
	{
		if (!needsFood())
		{
			return false;
		}
		return getNutritionRatio() <= SimGlobals.m.nutrition_level_hungry;
	}

	public float getNutritionRatio()
	{
		float num = getNutrition();
		float tMaxHunger = getMaxNutrition();
		return num / tMaxHunger;
	}

	public float getHealthRatio()
	{
		float num = getHealth();
		float tMaxHealth = getMaxHealth();
		return num / tMaxHealth;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasMaxHealth()
	{
		return getHealth() >= getMaxHealth();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasMaxMana()
	{
		return getMana() >= getMaxMana();
	}

	public bool isStarving()
	{
		return getNutrition() == 0;
	}

	public bool hasFavoriteFood()
	{
		return !string.IsNullOrEmpty(data.favorite_food);
	}

	public bool hasEmotions()
	{
		return _has_emotions;
	}

	public bool canHavePrejudice()
	{
		return hasEmotions();
	}

	public bool hasHappinessHistory()
	{
		return _last_happiness_history.Count > 0;
	}

	public bool isUnhappy()
	{
		if (!hasEmotions())
		{
			return false;
		}
		return getHappinessRatio() < 0.3f;
	}

	public int getHappiness()
	{
		return data.happiness;
	}

	public bool isHappy()
	{
		if (!hasEmotions())
		{
			return true;
		}
		return getHappinessRatio() >= 0.6f;
	}

	public int getMinHappiness()
	{
		return -100;
	}

	public int getMaxHappiness()
	{
		return 100;
	}

	public float getHappinessRatio()
	{
		return ((float)getHappiness() + 100f) / 200f;
	}

	internal bool isSameSpecies(string pID)
	{
		return asset.id == pID;
	}

	internal bool isSameSpecies(Actor pActor)
	{
		return isSameSpecies(pActor.asset.id);
	}

	internal bool isSameSubspecies(Subspecies pSubspecies)
	{
		return subspecies == pSubspecies;
	}

	public bool isAllowedToLookForEnemies()
	{
		if (shouldSkipFightCheck())
		{
			return false;
		}
		if (hasTask() && ai.task.ignore_fight_check)
		{
			return false;
		}
		if (_has_trait_peaceful)
		{
			return false;
		}
		if (isInsideSomething())
		{
			return false;
		}
		if (kingdom.asset.units_always_looking_for_enemies)
		{
			return true;
		}
		if (isBaby())
		{
			return false;
		}
		return true;
	}

	private bool shouldSkipFightCheck()
	{
		if (asset.skip_fight_logic)
		{
			return true;
		}
		if (_ignore_fights)
		{
			return true;
		}
		if (asset.is_boat && getSimpleComponent<Boat>().hasPassengers())
		{
			return true;
		}
		return false;
	}

	public bool isInWaterAndCantAttack()
	{
		if (!isWaterCreature())
		{
			return isAffectedByLiquid();
		}
		return false;
	}

	public bool hasReachedOffspringLimit()
	{
		int tMaxOffspring = getMaxOffspring();
		if (current_children_count >= tMaxOffspring)
		{
			return true;
		}
		return false;
	}

	public int getMaxOffspring()
	{
		return (int)Math.Ceiling(stats["offspring"]);
	}

	public bool haveNutritionForNewBaby()
	{
		if (needsFood() && getNutrition() < SimGlobals.m.nutrition_cost_new_baby)
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isInsideSomething()
	{
		if (is_inside_building || is_inside_boat)
		{
			return true;
		}
		return false;
	}

	public bool isOnSameIsland(Actor pActor)
	{
		return current_tile.isSameIsland(pActor.current_tile);
	}

	public bool hasSameCity(Actor pActorTarget)
	{
		if (hasCity())
		{
			return city == pActorTarget.city;
		}
		return false;
	}

	public bool canBreed()
	{
		if (!isAlive())
		{
			return false;
		}
		if (!isBreedingAge())
		{
			return false;
		}
		if (!haveNutritionForNewBaby())
		{
			return false;
		}
		if (hasStatus("pregnant"))
		{
			return false;
		}
		if (hasStatus("afterglow"))
		{
			return false;
		}
		return true;
	}

	public bool canProduceBabies()
	{
		if (hasTrait("infertile"))
		{
			return false;
		}
		return true;
	}

	public bool isPlacePrivateForBreeding()
	{
		int tUnitsInChunk = Toolbox.countUnitsInChunk(current_tile);
		if (hasCity())
		{
			int tEdge = city.getPopulationMaximum() * 2 + 10;
			return city.countUnits() < tEdge;
		}
		return asset.animal_breeding_close_units_limit > tUnitsInChunk;
	}

	public bool isOnGround()
	{
		if (!is_immovable && !is_unconscious)
		{
			if (hasTask())
			{
				return ai.action?.land_if_hovering ?? false;
			}
			return false;
		}
		return true;
	}

	internal bool isInAttackRange(BaseSimObject pObject)
	{
		float tRange = getAttackRange() + pObject.stats["size"];
		tRange *= tRange;
		return Toolbox.SquaredDistVec2Float(current_position, pObject.current_position) < tRange;
	}

	internal bool isAttackReady()
	{
		if (attack_timer > 0f)
		{
			return false;
		}
		return true;
	}

	public float getAttackCooldownRatio()
	{
		float tCooldown = getAttackCooldown();
		if (tCooldown == 0f)
		{
			return 1f;
		}
		return attack_timer / tCooldown;
	}

	internal bool isAttackPossible()
	{
		if (!isAttackReady())
		{
			return false;
		}
		if (current_rotation.z != 0f)
		{
			return false;
		}
		return true;
	}

	public bool canUseSpells()
	{
		if (hasStatus("spell_silence"))
		{
			return false;
		}
		if (hasSpellCastCooldownStatus())
		{
			return false;
		}
		return true;
	}

	public bool hasSpells()
	{
		if (_spells.hasAny())
		{
			return true;
		}
		if (hasSubspecies() && subspecies.spells.hasAny())
		{
			return true;
		}
		if (canUseReligionSpells() && religion.spells.hasAny())
		{
			return true;
		}
		return asset.hasDefaultSpells();
	}

	public bool canUseReligionSpells()
	{
		if (!hasReligion())
		{
			return false;
		}
		if (!religion.spells.hasAny())
		{
			return false;
		}
		if (hasTrait("mute"))
		{
			return false;
		}
		if (hasClan())
		{
			if (clan.hasTrait("void_ban"))
			{
				return false;
			}
			return true;
		}
		if (religion.is_magic_only_clan_members)
		{
			return false;
		}
		return true;
	}

	public SpellAsset getRandomSpell()
	{
		using ListPool<SpellAsset> tSpellPool = new ListPool<SpellAsset>();
		if (_spells.hasAny())
		{
			tSpellPool.Add(_spells.getRandomSpell());
		}
		if (hasSubspecies() && subspecies.spells.hasAny())
		{
			tSpellPool.Add(subspecies.spells.getRandomSpell());
		}
		if (canUseReligionSpells())
		{
			tSpellPool.Add(religion.spells.getRandomSpell());
		}
		if (asset.hasDefaultSpells())
		{
			tSpellPool.Add(asset.spells.getRandomSpell());
		}
		if (tSpellPool.Count == 0)
		{
			return null;
		}
		return tSpellPool.GetRandom();
	}

	internal override float getHeight()
	{
		return position_height + hitbox_bonus_height;
	}

	public float getScaleMod()
	{
		return actor_scale / 0.1f;
	}

	public bool isCameraFollowingUnit()
	{
		return MoveCamera.isCameraFollowingUnit(this);
	}

	internal bool isTargetOkToAttack(Actor pTarget)
	{
		if (pTarget == this)
		{
			return false;
		}
		if (!canAttackTarget(pTarget))
		{
			return false;
		}
		if (!isSameIslandAs(pTarget))
		{
			return false;
		}
		return true;
	}

	private float getLastColorEffectTime()
	{
		return World.world.getRealTimeElapsedSince(_last_color_effect_timestamp);
	}

	private float getLastStaminaReduceTime()
	{
		return World.world.getRealTimeElapsedSince(_last_stamina_reduce_timestamp);
	}

	public bool isUnderDamageCooldown()
	{
		return getLastColorEffectTime() < 0.3f;
	}

	private bool isUnderStaminaCooldown()
	{
		return getLastStaminaReduceTime() < 0.3f;
	}

	private bool haveMetallicArmor()
	{
		return false;
	}

	private bool haveMetallicWeapon()
	{
		if (!hasEquipment())
		{
			return false;
		}
		if (equipment.getSlot(EquipmentType.Weapon).isEmpty())
		{
			return false;
		}
		return equipment.getSlot(EquipmentType.Weapon).getItem().getAsset()
			.metallic;
	}

	public bool isSameKingdomAndAlmostDead(Actor pActor, float pDamage)
	{
		if (isSameKingdom(pActor) && (float)getHealth() - pDamage <= 0f)
		{
			return true;
		}
		return false;
	}

	public bool isSameKingdom(BaseSimObject pSimObject)
	{
		return kingdom == pSimObject.kingdom;
	}

	public bool isInCityIsland()
	{
		if (city.isRekt())
		{
			return false;
		}
		WorldTile tCityTile = city.getTile();
		if (tCityTile == null)
		{
			return false;
		}
		if (current_tile.isSameIsland(tCityTile))
		{
			return true;
		}
		return false;
	}

	public bool isClone()
	{
		return _has_trait_clone;
	}

	public bool isClonedFrom(Actor pActor)
	{
		if (!isClone())
		{
			return false;
		}
		if (data.parent_id_1 != pActor.data.id)
		{
			return false;
		}
		return true;
	}

	public bool isSameClones(Actor pActor)
	{
		if (!isClone())
		{
			return false;
		}
		if (!pActor.isClone())
		{
			return false;
		}
		if (data.parent_id_1 != pActor.data.parent_id_1)
		{
			return false;
		}
		return true;
	}

	public bool isUnitFitToRule()
	{
		if (!isAlive())
		{
			return false;
		}
		if (!isKingdomCiv())
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Actor pObject)
	{
		return GetHashCode() == pObject.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Actor pTarget)
	{
		return GetHashCode().CompareTo(pTarget.GetHashCode());
	}

	public bool canTalkWith(Actor pTarget)
	{
		if (this == pTarget)
		{
			return false;
		}
		if (!pTarget.isReadyToTalk())
		{
			return false;
		}
		if (!isSameIslandAs(pTarget))
		{
			return false;
		}
		if (areFoes(pTarget))
		{
			return false;
		}
		if (isInsideSomething())
		{
			return false;
		}
		if (pTarget.asset.special)
		{
			return false;
		}
		return true;
	}

	public bool canFallInLoveWith(Actor pTarget)
	{
		if (hasLover())
		{
			return false;
		}
		if (!isAdult())
		{
			return false;
		}
		if (!isBreedingAge())
		{
			return false;
		}
		if (!subspecies.needs_mate)
		{
			return false;
		}
		if (!pTarget.subspecies.needs_mate)
		{
			return false;
		}
		if (!isSameSpecies(pTarget))
		{
			return false;
		}
		if (!subspecies.isPartnerSuitableForReproduction(this, pTarget))
		{
			return false;
		}
		if (pTarget.hasLover())
		{
			return false;
		}
		if (!pTarget.isAdult())
		{
			return false;
		}
		if (!pTarget.isBreedingAge())
		{
			return false;
		}
		if (isRelatedTo(pTarget))
		{
			return false;
		}
		return true;
	}

	public bool hasHouseCityInBordersAndSameIsland()
	{
		if (hasCity() && hasHouse() && inOwnCityBorders() && inOwnHouseIsland())
		{
			return true;
		}
		return false;
	}

	public bool inOwnHouseIsland()
	{
		Building tHouse = getHomeBuilding();
		if (tHouse.isRekt())
		{
			return false;
		}
		return current_tile.isSameIsland(tHouse.current_tile);
	}

	public bool inOwnCityBorders()
	{
		if (!hasCity())
		{
			return false;
		}
		return base.current_zone.isSameCityHere(city);
	}

	public bool inOwnCityIsland()
	{
		if (!hasCity())
		{
			return false;
		}
		WorldTile tCityTile = city.getTile();
		if (tCityTile == null)
		{
			return false;
		}
		return current_tile.isSameIsland(tCityTile);
	}

	public bool isReadyToTalk()
	{
		if (!isAlive())
		{
			return false;
		}
		if (!canSocialize())
		{
			return false;
		}
		if (hasTask() && !ai.task.cancellable_by_socialize)
		{
			return false;
		}
		return true;
	}

	public bool canSocialize()
	{
		if (asset.unit_zombie)
		{
			return false;
		}
		if (isEgg())
		{
			return false;
		}
		if (isFighting())
		{
			return false;
		}
		if (hasStatus("recovery_social"))
		{
			return false;
		}
		if (!hasSubspecies())
		{
			return false;
		}
		return true;
	}

	public int getConstructionSpeed()
	{
		int tResult = 2;
		if (hasSubspecies())
		{
			tResult += (int)subspecies.base_stats_meta["construction_speed"];
		}
		return tResult;
	}

	private bool combatActionOnTimeout()
	{
		return hasStatus("recovery_combat_action");
	}

	private bool hasSpellCastCooldownStatus()
	{
		return hasStatus("recovery_spell");
	}

	public bool hasEnoughMana(int pCostMana)
	{
		if (pCostMana != 0)
		{
			return getMana() >= pCostMana;
		}
		return true;
	}

	public int getMana()
	{
		return data.mana;
	}

	public int getMaxMana()
	{
		return (int)stats["mana"];
	}

	public void setMaxMana()
	{
		setMana(getMaxMana());
	}

	public bool isManaFull()
	{
		return getMana() == getMaxMana();
	}

	public bool hasEnoughStamina(int pCostStamina)
	{
		if (pCostStamina != 0)
		{
			return getStamina() >= pCostStamina;
		}
		return true;
	}

	public int getStamina()
	{
		return data.stamina;
	}

	public int getMaxStamina()
	{
		return (int)stats["stamina"];
	}

	public void setMaxStamina()
	{
		setStamina(getMaxStamina());
	}

	public bool isStaminaFull()
	{
		return getStamina() == getMaxStamina();
	}

	public bool isWarrior()
	{
		return profession_asset.profession_id == UnitProfession.Warrior;
	}

	public bool isCarnivore()
	{
		if (hasSubspecies() && subspecies.diet_meat)
		{
			return true;
		}
		return false;
	}

	public bool isHerbivore()
	{
		if (hasSubspecies() && subspecies.diet_vegetation)
		{
			return true;
		}
		return false;
	}

	public bool hasStatusStunned()
	{
		return hasStatus("stunned");
	}

	public bool isLying()
	{
		if (!is_unconscious)
		{
			return _has_status_sleeping;
		}
		return true;
	}

	public override bool hasStatusTantrum()
	{
		return _has_status_tantrum;
	}

	public bool hasAnyCash()
	{
		if (money <= 0)
		{
			return loot > 0;
		}
		return true;
	}

	public bool hasEnoughMoney(int pCost)
	{
		return money >= pCost;
	}

	public int getHappinessPercent()
	{
		int tMax = getMaxHappiness();
		int tMin = getMinHappiness();
		return Mathf.Clamp(Mathf.Clamp(getHappiness() - tMin, 0, tMax - tMin) * 100 / (tMax - tMin), 0, 100);
	}

	public float distanceToObjectTarget(BaseSimObject pBaseSimObject)
	{
		return Toolbox.DistVec2Float(current_position, pBaseSimObject.current_position);
	}

	public float distanceToActorTile(Actor pActor)
	{
		return distanceToActorTile(pActor.current_tile);
	}

	public float distanceToActorTile(WorldTile pTile)
	{
		return current_tile.distanceTo(pTile);
	}

	public bool isRelatedTo(Actor pTarget)
	{
		if (hasFamily() && pTarget.hasFamily() && isSapient() && family == pTarget.family)
		{
			return true;
		}
		if (isChildOf(pTarget))
		{
			return true;
		}
		if (isParentOf(pTarget))
		{
			return true;
		}
		return false;
	}

	public bool isImportantTo(Actor pTarget)
	{
		if (hasLover() && lover == pTarget)
		{
			return true;
		}
		if (hasBestFriend() && getBestFriend() == pTarget)
		{
			return true;
		}
		return false;
	}

	public bool canWork()
	{
		if (isAdult())
		{
			return true;
		}
		if (hasCulture())
		{
			Culture tCulture = culture;
			if (tCulture.hasTrait("tiny_legends"))
			{
				return true;
			}
			if (tCulture.hasTrait("youth_reverence"))
			{
				return false;
			}
		}
		if (getAge() >= SimGlobals.m.child_work_age)
		{
			return true;
		}
		return false;
	}

	public bool hasCultureTrait(string pTraitID)
	{
		if (hasCulture())
		{
			return culture.hasTrait(pTraitID);
		}
		return false;
	}

	public bool canBePossessed()
	{
		return asset.allow_possession;
	}

	public float getAttackRange()
	{
		return stats["range"];
	}

	public float getAttackRangeSquared()
	{
		return stats["range"] * stats["range"];
	}

	public float getStaminaRatio()
	{
		float tMax = getMaxStamina();
		if (tMax == 0f)
		{
			return 0f;
		}
		return (float)getStamina() / tMax;
	}

	public float getManaRatio()
	{
		float tMax = getMaxMana();
		if (tMax == 0f)
		{
			return 0f;
		}
		return (float)getMana() / tMax;
	}

	public bool canGetFoodFromCity()
	{
		if (isFoodFreeForThisPerson())
		{
			return true;
		}
		if (money <= SimGlobals.m.min_coins_before_city_food)
		{
			return false;
		}
		return true;
	}

	public bool isFoodFreeForThisPerson()
	{
		if (isKing())
		{
			return true;
		}
		if (isCityLeader())
		{
			return true;
		}
		if (isBaby())
		{
			return true;
		}
		if (isStarving())
		{
			return true;
		}
		return false;
	}

	public int getMaxNutrition()
	{
		float tVal = asset.nutrition_max;
		if (hasSubspecies())
		{
			tVal += subspecies.base_stats_meta["max_nutrition"];
		}
		return (int)tVal;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int getExpToLevelup()
	{
		return 100 + (data.level - 1) * 20;
	}

	private bool calculateIsSick()
	{
		if (hasTrait("infected"))
		{
			return true;
		}
		if (hasTrait("plague"))
		{
			return true;
		}
		if (hasTrait("mush_spores") && asset.can_turn_into_mush)
		{
			return true;
		}
		if (hasTrait("tumor_infection") && asset.can_turn_into_tumor)
		{
			return true;
		}
		return false;
	}

	public bool isSick()
	{
		return _has_any_sick_trait;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool canTakeItems()
	{
		return asset.take_items;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool understandsHowToUseItems()
	{
		if (!canUseItems())
		{
			return false;
		}
		if (isSapient())
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool canUseItems()
	{
		return asset.use_items;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool canEditEquipment()
	{
		return asset.use_items;
	}

	public bool canTurnIntoColdOne()
	{
		if (isAdult())
		{
			return false;
		}
		if (!asset.can_turn_into_ice_one)
		{
			return false;
		}
		if (!asset.has_soul)
		{
			return false;
		}
		return true;
	}

	public bool canTurnIntoDemon()
	{
		if (isBaby())
		{
			return false;
		}
		return asset.can_turn_into_demon_in_age_of_chaos;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BaseObjectData getData()
	{
		return data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isCarryingResources()
	{
		return inventory.hasResources();
	}

	public bool needsFood()
	{
		if (hasSubspecies())
		{
			return subspecies.needs_food;
		}
		return false;
	}

	public bool isDamagedByRain()
	{
		if (hasSubspecies())
		{
			return subspecies.is_damaged_by_water;
		}
		return false;
	}

	public bool isDamagedByOcean()
	{
		if (hasSubspecies())
		{
			return subspecies.is_damaged_by_water;
		}
		return asset.damaged_by_ocean;
	}

	public int getWaterDamage()
	{
		int tResult = (int)((float)getMaxHealth() * SimGlobals.m.water_damage_multiplier);
		if (tResult < 1)
		{
			tResult = 1;
		}
		return tResult;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasSubspeciesTrait(string pTraitID)
	{
		if (hasSubspecies())
		{
			return subspecies.hasTrait(pTraitID);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasSubspeciesMetaTag(string pTagID)
	{
		if (hasSubspecies())
		{
			return subspecies.hasMetaTag(pTagID);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasTag(string pTag)
	{
		return stats.hasTag(pTag);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isImmuneToFire()
	{
		return hasTag("immunity_fire");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isImmuneToCold()
	{
		return hasTag("immunity_cold");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isImmovable()
	{
		return hasTag("immovable");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isAiFrozen()
	{
		return hasTag("frozen_ai");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isIgnoreFights()
	{
		return hasTag("ignore_fights");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasStopIdleAnimation()
	{
		if (hasSubspecies() && subspecies.hasMetaTag("always_idle_animation"))
		{
			return false;
		}
		return hasTag("stop_idle_animation");
	}

	public bool hasDivineScar()
	{
		return hasTrait("scar_of_divinity");
	}

	public bool hasTelepathicLink()
	{
		if (!hasSubspecies())
		{
			return false;
		}
		return subspecies.hasTrait("telepathic_link");
	}

	public float getResourceThrowDistance()
	{
		return asset.base_throwing_range + stats["throwing_range"];
	}

	internal bool isFalling()
	{
		if (position_height != 0f)
		{
			return true;
		}
		if (move_jump_offset.y != 0f)
		{
			return true;
		}
		return false;
	}

	public float getAgeRatio()
	{
		float tLifespan = stats["lifespan"];
		return (float)getAge() / tLifespan;
	}

	public int getMassKG()
	{
		float tSizeDiff = target_scale / 0.1f;
		int tMassKG = (int)(stats["mass_2"] * tSizeDiff);
		if (isBaby())
		{
			tMassKG = (int)((float)tMassKG * SimGlobals.m.baby_mass_multiplier);
		}
		return tMassKG;
	}

	public IEnumerable<ResourceContainer> getResourcesFromActor()
	{
		if (asset.resources_given == null)
		{
			yield break;
		}
		int tMass = getMassKG();
		foreach (ResourceContainer tContainer in asset.resources_given)
		{
			ResourceAsset tResourceAsset = tContainer.asset;
			int tMultiplier = tMass / tResourceAsset.drop_per_mass + 1;
			int tAmount = tContainer.amount * tMultiplier;
			tAmount = Mathf.Clamp(tAmount, 1, tResourceAsset.drop_max);
			if (tAmount > 0)
			{
				yield return new ResourceContainer(tContainer.id, tAmount);
			}
		}
	}

	public bool hasXenophobic()
	{
		if (hasCulture())
		{
			return culture.hasTrait("xenophobic");
		}
		return false;
	}

	public bool hasXenophiles()
	{
		if (hasCulture())
		{
			return culture.hasTrait("xenophiles");
		}
		return false;
	}

	public bool hasCannibalism()
	{
		if (hasSubspecies())
		{
			return subspecies.hasCannibalism();
		}
		return false;
	}

	public bool isOneCityKingdom()
	{
		if (hasCity() && city.kingdom.countCities() == 1)
		{
			return true;
		}
		return false;
	}

	public bool isImportantPerson()
	{
		if (isKing())
		{
			return true;
		}
		if (isCityLeader())
		{
			return true;
		}
		if (isArmyGroupLeader())
		{
			return true;
		}
		if (isFavorite())
		{
			return true;
		}
		return false;
	}

	public bool canCurrentTaskBeCancelledByReproduction()
	{
		if (!hasTask())
		{
			return true;
		}
		return ai.task.cancellable_by_reproduction;
	}

	public bool isAbleToSkipPriorityLevels()
	{
		if (isWarrior() && hasCity() && city.hasAttackZoneOrder())
		{
			return false;
		}
		return true;
	}

	public void makeSpawnSound(bool pFromUI)
	{
		if (asset.has_sound_spawn)
		{
			if (pFromUI)
			{
				MusicBox.playSoundUI(asset.sound_spawn);
			}
			else
			{
				MusicBox.playSound(asset.sound_spawn, current_tile);
			}
		}
	}

	public void makeSoundAttack()
	{
		if (asset.has_sound_attack)
		{
			MusicBox.playSound(asset.sound_attack, current_tile, pGameViewOnly: true, pVisibleOnly: true);
		}
	}

	public string getTaskText()
	{
		if (!hasTask())
		{
			return "???";
		}
		string localizedText = ai.task.getLocalizedText();
		string tTime = ai.getTaskTime();
		return localizedText + " " + tTime.ColorHex(ColorStyleLibrary.m.color_text_grey_dark);
	}

	public void afterEvolutionEvents()
	{
		clearGraphicsFully();
		makeConfused();
		applyRandomForce();
		increaseEvolutions();
	}

	public void generatePhenotypeAndShade()
	{
		data.phenotype_index = subspecies.getRandomPhenotypeIndex();
		data.phenotype_shade = getRandomPhenotypeShade();
	}

	public static int getRandomPhenotypeShade()
	{
		return Randy.randomInt(0, 4);
	}

	public bool isRendered()
	{
		return current_tile.zone.visible;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool checkHasRenderedItem()
	{
		if (!canUseItems() || _is_in_liquid)
		{
			return false;
		}
		if (isEgg())
		{
			return false;
		}
		if (!equipment.weapon.isEmpty())
		{
			return true;
		}
		if (hasTask() && ai.task.force_hand_tool != string.Empty)
		{
			return true;
		}
		if (isCarryingResources())
		{
			return true;
		}
		return false;
	}

	internal Sprite getSpriteToRender()
	{
		return checkSpriteToRender();
	}

	public bool hasColoredSprite()
	{
		return asset.need_colored_sprite;
	}

	public bool isColoredSpriteNeedsCheck(Sprite pMainSprite)
	{
		if (_last_main_sprite != pMainSprite || _last_color_asset != kingdom.getColor())
		{
			return true;
		}
		return false;
	}

	public Sprite calculateColoredSprite(Sprite pMainSprite, bool pUpdateFrameData = true)
	{
		if (isColoredSpriteNeedsCheck(pMainSprite))
		{
			if (animation_container != null && pUpdateFrameData)
			{
				animation_container.dict_frame_data.TryGetValue(pMainSprite.name, out frame_data);
			}
			checkSpriteHead();
			int tPhenotypeID = data.phenotype_index;
			int tPhenotypeShadeIndex = data.phenotype_shade;
			_last_colored_sprite = DynamicSpriteCreator.getSpriteUnit(frame_data, pMainSprite, this, kingdom.getColor(), tPhenotypeID, tPhenotypeShadeIndex, asset.texture_atlas);
			_last_main_sprite = pMainSprite;
			_last_color_asset = kingdom.getColor();
		}
		return _last_colored_sprite;
	}

	public Sprite getLastColoredSprite()
	{
		return _last_colored_sprite;
	}

	public bool canParallelSetColoredSprite()
	{
		if (asset.has_avatar_prefab)
		{
			return true;
		}
		if (dirty_sprite_main)
		{
			return false;
		}
		return true;
	}

	public Sprite calculateMainSprite()
	{
		if (asset.has_override_sprite)
		{
			return asset.get_override_sprite(this);
		}
		checkAnimationContainer();
		if (ai.action != null && ai.action.force_animation)
		{
			return animation_container.sprites[ai.action.force_animation_id];
		}
		if (!isAlive() || _has_stop_idle_animation)
		{
			if (animation_container.has_swimming && _has_status_drowning)
			{
				return animation_container.swimming.frames[0];
			}
			return animation_container.idle.frames[0];
		}
		float tAnimSpeed = asset.animation_walk_speed;
		bool tSpeedAffectsAnimSpeed = false;
		ActorAnimation tAnimation;
		if (is_moving || timer_jump_animation > 0f || move_jump_offset.y > 0f || is_in_magnet)
		{
			if (animation_container.has_swimming && isAffectedByLiquid())
			{
				tAnimation = animation_container.swimming;
				tAnimSpeed = asset.animation_swim_speed;
			}
			else
			{
				tAnimation = animation_container.walking;
			}
			tSpeedAffectsAnimSpeed = true;
		}
		else if (position_height > 0f)
		{
			tAnimation = animation_container.idle;
		}
		else if (animation_container.has_swimming && isAffectedByLiquid())
		{
			tAnimation = animation_container.swimming;
			tAnimSpeed = asset.animation_swim_speed;
			tSpeedAffectsAnimSpeed = true;
		}
		else
		{
			tAnimation = animation_container.idle;
			tAnimSpeed = asset.animation_idle_speed;
		}
		if (asset.animation_speed_based_on_walk_speed && tSpeedAffectsAnimSpeed)
		{
			tAnimSpeed *= stats["speed"] / 10f;
			tAnimSpeed = Mathf.Clamp(tAnimSpeed, 4f, tAnimSpeed);
		}
		if (tAnimation.frames.Length > 1)
		{
			return AnimationHelper.getSpriteFromList(GetHashCode(), tAnimation.frames, tAnimSpeed);
		}
		return tAnimation.frames[0];
	}

	internal Sprite checkSpriteToRender()
	{
		Sprite tMainSprite = calculateMainSprite();
		if (!asset.need_colored_sprite)
		{
			return tMainSprite;
		}
		return calculateColoredSprite(tMainSprite);
	}

	protected void setItemSpriteRenderDirty()
	{
		_dirty_sprite_item = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Sprite getRenderedItemSprite()
	{
		if (_dirty_sprite_item || _has_animated_item)
		{
			_cached_hand_renderer_asset = getHandRendererAsset();
			_cached_sprite_item = ItemRendering.getItemMainSpriteFrame(_cached_hand_renderer_asset);
			_dirty_sprite_item = false;
		}
		return _cached_sprite_item;
	}

	public IHandRenderer getCachedHandRendererAsset()
	{
		return _cached_hand_renderer_asset;
	}

	public IHandRenderer getHandRendererAsset()
	{
		IHandRenderer tToolItem = getRenderedToolOrItem();
		if (tToolItem != null)
		{
			return tToolItem;
		}
		if (hasWeapon())
		{
			return getWeaponTextureId();
		}
		return null;
	}

	private IHandRenderer getRenderedToolOrItem()
	{
		if (!asset.use_tool_items)
		{
			return null;
		}
		_has_animated_item = false;
		if (has_attack_target && hasWeapon())
		{
			return null;
		}
		if (isCarryingResources())
		{
			return AssetManager.resources.get(inventory.getItemIDToRender());
		}
		if (hasTask())
		{
			UnitHandToolAsset tTaskTool = ai.task.cached_hand_tool_asset;
			if (tTaskTool != null)
			{
				_has_animated_item = tTaskTool.animated;
				return tTaskTool;
			}
		}
		return null;
	}

	public bool isItemInHandAnimated()
	{
		if (isCarryingResources())
		{
			return false;
		}
		if (hasTask())
		{
			UnitHandToolAsset tTaskTool = ai.task.cached_hand_tool_asset;
			if (tTaskTool != null)
			{
				return tTaskTool.animated;
			}
		}
		if (hasWeapon())
		{
			return getWeapon().getAsset().animated;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void clearSprites()
	{
		dirty_sprite_head = true;
		dirty_sprite_main = true;
	}

	public void clearGraphicsFully()
	{
		clearSprites();
		clearLastColorCache();
		animation_container = null;
		frame_data = null;
		animation_container = null;
		_last_main_sprite = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AnimationFrameData getAnimationFrameData()
	{
		return frame_data;
	}

	public Vector3 getHeadOffsetPositionForFunRendering()
	{
		Vector3 tPos = new Vector3(cur_transform_position.x, cur_transform_position.y, 0f);
		AnimationFrameData tFrameData = getAnimationFrameData();
		if (tFrameData != null)
		{
			tPos.x += tFrameData.pos_head.x * current_scale.x;
			tPos.y += tFrameData.pos_head.y * current_scale.y;
		}
		return tPos;
	}

	public IHandRenderer getWeaponTextureId()
	{
		Item tWeapon = getWeapon();
		_has_animated_item = tWeapon.getAsset().animated;
		return tWeapon.asset;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ActorTextureSubAsset getTextureAsset()
	{
		if (hasSubspecies() && subspecies.has_mutation_reskin)
		{
			return subspecies.mutation_skin_asset.texture_asset;
		}
		return asset.texture_asset;
	}

	public string getUnitTexturePath()
	{
		return getTextureAsset().getUnitTexturePath(this);
	}

	internal void checkAnimationContainer()
	{
		if (dirty_sprite_main)
		{
			dirty_sprite_main = false;
			AnimationContainerUnit tContainer = ActorAnimationLoader.getAnimationContainer(getUnitTexturePath(), asset, subspecies?.egg_asset, subspecies?.mutation_skin_asset);
			animation_container = tContainer;
		}
	}

	public SpriteAnimation getSpriteAnimation()
	{
		return sprite_animation;
	}

	public Vector2 getRenderedItemPosition()
	{
		return getAnimationFrameData()?.pos_item ?? Vector2.one;
	}

	public void clearLastColorCache()
	{
		_last_colored_sprite = null;
		_last_color_asset = null;
		cached_sprite_head = null;
	}

	public void startColorEffect(ActorColorEffect pColorType = ActorColorEffect.White)
	{
		if (!asset.effect_damage || !is_visible || isUnderDamageCooldown())
		{
			return;
		}
		_last_color_effect_timestamp = World.world.getCurSessionTime();
		if (World.world.stack_effects.actor_effect_hit.Count <= 1000)
		{
			if (pColorType == ActorColorEffect.Red)
			{
				World.world.stack_effects.actor_effect_hit.Add(new ActorDamageEffectData
				{
					actor = this,
					timestamp = _last_color_effect_timestamp
				});
			}
			else
			{
				World.world.stack_effects.actor_effect_highlight.Add(new ActorHighlightEffectData
				{
					actor = this,
					timestamp = _last_color_effect_timestamp
				});
			}
		}
	}

	protected void checkSpriteHead()
	{
		if (!dirty_sprite_head)
		{
			return;
		}
		dirty_sprite_head = false;
		if (frame_data == null || !frame_data.show_head || animation_container.heads.Length == 0 || isEgg() || (isBaby() && !animation_container.render_heads_for_children))
		{
			return;
		}
		ActorTextureSubAsset tTextureAsset = getTextureAsset();
		if (!tTextureAsset.has_advanced_textures)
		{
			checkHeadID(animation_container.heads);
			setHeadSprite(animation_container.heads[data.head]);
			return;
		}
		bool tSpecial = false;
		string tHeadPath;
		Sprite[] tListHeadsSprites;
		if (isSexMale())
		{
			tHeadPath = tTextureAsset.texture_heads_male;
			tListHeadsSprites = animation_container.heads_male;
		}
		else
		{
			tHeadPath = tTextureAsset.texture_heads_female;
			tListHeadsSprites = animation_container.heads_female;
		}
		if (isSapient())
		{
			if (is_profession_warrior && !equipment.helmet.isEmpty())
			{
				tHeadPath = tTextureAsset.texture_head_warrior;
				tSpecial = true;
			}
			else if (is_profession_king)
			{
				tHeadPath = tTextureAsset.texture_head_king;
				tSpecial = true;
			}
			else if (tTextureAsset.has_old_heads && hasTrait("wise"))
			{
				tHeadPath = ((!isSexMale()) ? tTextureAsset.texture_heads_old_female : tTextureAsset.texture_heads_old_male);
				tSpecial = true;
			}
			else if (isSexMale())
			{
				tHeadPath = tTextureAsset.texture_heads_male;
				tListHeadsSprites = animation_container.heads_male;
			}
			else
			{
				tHeadPath = tTextureAsset.texture_heads_female;
				tListHeadsSprites = animation_container.heads_female;
			}
		}
		if (tSpecial)
		{
			setHeadSprite(ActorAnimationLoader.getHeadSpecial(tHeadPath));
			return;
		}
		checkHeadID(tListHeadsSprites);
		setHeadSprite(ActorAnimationLoader.getHead(tHeadPath, data.head));
	}

	internal void checkHeadID(Sprite[] pListHeads, bool pCheckSavedHead = true)
	{
		if (pCheckSavedHead && data.head > pListHeads.Length - 1)
		{
			data.head = 0;
		}
		if (data.head == -1)
		{
			int tIndex = AnimationHelper.getSpriteIndex(data.id, pListHeads.Length);
			data.head = tIndex;
		}
	}

	private void setHeadSprite(Sprite pSprite)
	{
		cached_sprite_head = pSprite;
	}

	protected void updateDeadAnimation(float pElapsed)
	{
		if (asset.special_dead_animation && asset.action_dead_animation(this, current_tile, pElapsed))
		{
			return;
		}
		if (World.world.quality_changer.isFullLowRes())
		{
			die(pDestroy: true, AttackType.None, pCountDeath: false);
			return;
		}
		if (asset.death_animation_angle && !_has_status_drowning && target_angle.z < 90f)
		{
			target_angle.z = Mathf.Lerp(target_angle.z, 90f, pElapsed * 4f);
			if (target_angle.z > 90f)
			{
				target_angle.z = 90f;
			}
			if (is_visible && Mathf.Abs(current_rotation.z) < 45f)
			{
				return;
			}
		}
		changeMoveJumpOffset(-0.05f);
		if (!isFalling())
		{
			updateDeadBlackAnimation(pElapsed);
		}
	}

	public double[] getDecisionsCooldowns()
	{
		return _decision_cooldowns;
	}

	public bool[] getDecisionsDisabled()
	{
		return _decision_disabled;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isDecisionOnCooldown(int pIndex, double pCooldown)
	{
		double tTimestamp = _decision_cooldowns[pIndex];
		if (tTimestamp == 0.0)
		{
			return false;
		}
		if ((double)World.world.getWorldTimeElapsedSince(tTimestamp) > pCooldown)
		{
			_decision_cooldowns[pIndex] = 0.0;
			return false;
		}
		return true;
	}

	public void setupRandomDecisionCooldowns()
	{
		double tWorldTime = World.world.getCurWorldTime();
		for (int i = 0; i < decisions_counter; i++)
		{
			DecisionAsset tDecisionAsset = decisions[i];
			if (tDecisionAsset.cooldown != 0)
			{
				double tFakeTimestamp = tWorldTime - (double)Randy.randomFloat(0f, (float)tDecisionAsset.cooldown * 0.5f);
				_decision_cooldowns[tDecisionAsset.decision_index] = tFakeTimestamp;
			}
		}
		timer_action = Randy.randomFloat(1f, 5f);
	}

	public void setDecisionCooldown(DecisionAsset pAsset)
	{
		if (pAsset.cooldown != 0)
		{
			_decision_cooldowns[pAsset.decision_index] = World.world.getCurWorldTime();
		}
	}

	public bool isDecisionEnabled(int pIndex)
	{
		return !_decision_disabled[pIndex];
	}

	public bool switchDecisionState(int pIndex)
	{
		_decision_disabled[pIndex] = !_decision_disabled[pIndex];
		return _decision_disabled[pIndex];
	}

	public void setDecisionState(int pIndex, bool pState)
	{
		_decision_disabled[pIndex] = !pState;
	}

	public void setTask(string pTaskId, bool pClean = true, bool pCleanJob = false, bool pForceAction = false)
	{
		ai.setTask(pTaskId, pClean, pCleanJob, pForceAction);
	}

	public void cancelAllBeh()
	{
		ai.clearBeh();
		ai.setTaskBehFinished();
		endJob();
		clearTasks();
	}

	public void endJob()
	{
		ai.clearJob();
		citizen_job = null;
	}

	protected virtual void clearTasks()
	{
		exitBuilding();
		clearAttackTarget();
		timer_action = 0f;
		clearTileTarget();
		stopMovement();
	}

	public void setCitizenJob(CitizenJobAsset pJobAsset)
	{
		citizen_job = pJobAsset;
		ai.setJob(pJobAsset.unit_job_default);
	}

	internal void clearBeh()
	{
		clearTasks();
		beh_tile_target = null;
		beh_building_target = null;
		beh_actor_target = null;
		beh_book_target = null;
	}

	public string getNextJob()
	{
		return nextJobActor(a);
	}

	public static string nextJobActor(Actor pActor)
	{
		if (pActor.isEgg())
		{
			return "egg";
		}
		string tNewJobID = null;
		if (pActor.isSapient())
		{
			if (pActor.isBaby())
			{
				tNewJobID = pActor.asset.job_baby.GetRandom();
			}
			else if (pActor.hasCity())
			{
				tNewJobID = ((!pActor.isProfession(UnitProfession.Warrior)) ? pActor.asset.job_citizen.GetRandom() : pActor.asset.job_attacker.GetRandom());
			}
			else if (pActor.isKingdomCiv())
			{
				tNewJobID = pActor.asset.job_kingdom.GetRandom();
			}
			else if (pActor.asset.job.Length != 0)
			{
				tNewJobID = pActor.asset.job.GetRandom();
			}
		}
		else if (pActor.asset.job.Length != 0)
		{
			tNewJobID = pActor.asset.job.GetRandom();
		}
		return tNewJobID;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isTask(string pID)
	{
		return ai.task?.id == pID;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasTask()
	{
		return ai.hasTask();
	}

	public void clearDecisions()
	{
		_decision_cooldowns.Clear();
		_decision_disabled.Clear();
		decisions.Clear();
		decisions_counter = 0;
		_last_decision_id = string.Empty;
	}

	public void scheduleTask(string pTask, WorldTile pTile)
	{
		ai.scheduleTask(pTask);
		scheduled_tile_target = pTile;
	}

	private void registerDecisions()
	{
		foreach (ActorTrait trait in traits)
		{
			DecisionAsset[] tTraitDecisionAssets = trait.decisions_assets;
			if (tTraitDecisionAssets != null)
			{
				for (int i = 0; i < tTraitDecisionAssets.Length; i++)
				{
					decisions[decisions_counter++] = tTraitDecisionAssets[i];
				}
			}
		}
		Clan obj = clan;
		if (obj != null && obj.decisions_assets.Count > 0)
		{
			List<DecisionAsset> tDecisionsAssets = clan.decisions_assets;
			for (int j = 0; j < tDecisionsAssets.Count; j++)
			{
				decisions[decisions_counter++] = tDecisionsAssets[j];
			}
		}
		Culture obj2 = culture;
		if (obj2 != null && obj2.decisions_assets.Count > 0)
		{
			List<DecisionAsset> tDecisionsAssets2 = culture.decisions_assets;
			for (int k = 0; k < tDecisionsAssets2.Count; k++)
			{
				decisions[decisions_counter++] = tDecisionsAssets2[k];
			}
		}
		Language obj3 = language;
		if (obj3 != null && obj3.decisions_assets.Count > 0)
		{
			List<DecisionAsset> tDecisionsAssets3 = language.decisions_assets;
			for (int l = 0; l < tDecisionsAssets3.Count; l++)
			{
				decisions[decisions_counter++] = tDecisionsAssets3[l];
			}
		}
		Religion obj4 = religion;
		if (obj4 != null && obj4.decisions_assets.Count > 0 && canUseReligionSpells())
		{
			List<DecisionAsset> tDecisionsAssets4 = religion.decisions_assets;
			for (int m = 0; m < tDecisionsAssets4.Count; m++)
			{
				decisions[decisions_counter++] = tDecisionsAssets4[m];
			}
		}
		Subspecies obj5 = subspecies;
		if (obj5 != null && obj5.decisions_assets.Count > 0)
		{
			List<DecisionAsset> tDecisionsAssets5 = subspecies.decisions_assets;
			for (int n = 0; n < tDecisionsAssets5.Count; n++)
			{
				decisions[decisions_counter++] = tDecisionsAssets5[n];
			}
		}
		if (profession_asset != null && profession_asset.hasDecisions())
		{
			DecisionAsset[] tDecisionsAssets6 = profession_asset.decisions_assets;
			for (int num = 0; num < tDecisionsAssets6.Length; num++)
			{
				decisions[decisions_counter++] = tDecisionsAssets6[num];
			}
		}
		if (_spells.hasAny())
		{
			foreach (SpellAsset tSpellAsset in _spells.spells)
			{
				if (tSpellAsset.hasDecisions())
				{
					DecisionAsset[] tSpellDecisions = tSpellAsset.decisions_assets;
					for (int num2 = 0; num2 < tSpellDecisions.Length; num2++)
					{
						decisions[decisions_counter++] = tSpellDecisions[num2];
					}
				}
			}
		}
		if (hasWeapon() && getWeapon().getAsset().hasDecisions())
		{
			DecisionAsset[] tWeaponDecisions = getWeapon().getAsset().decisions_assets;
			for (int num3 = 0; num3 < tWeaponDecisions.Length; num3++)
			{
				decisions[decisions_counter++] = tWeaponDecisions[num3];
			}
		}
		if (hasFamily())
		{
			DecisionAsset[] tDecisionsAssets7 = MetaTypeLibrary.family.decisions_assets;
			for (int num4 = 0; num4 < tDecisionsAssets7.Length; num4++)
			{
				decisions[decisions_counter++] = tDecisionsAssets7[num4];
			}
		}
		if (hasCity() && !asset.is_boat)
		{
			DecisionAsset[] tDecisionsAssets8 = MetaTypeLibrary.city.decisions_assets;
			for (int num5 = 0; num5 < tDecisionsAssets8.Length; num5++)
			{
				decisions[decisions_counter++] = tDecisionsAssets8[num5];
			}
		}
		if (hasPlot())
		{
			DecisionAsset[] tDecisionsAssets9 = MetaTypeLibrary.plot.decisions_assets;
			for (int num6 = 0; num6 < tDecisionsAssets9.Length; num6++)
			{
				decisions[decisions_counter++] = tDecisionsAssets9[num6];
			}
		}
		if (hasClan())
		{
			DecisionAsset[] tDecisionsAssets10 = MetaTypeLibrary.clan.decisions_assets;
			for (int num7 = 0; num7 < tDecisionsAssets10.Length; num7++)
			{
				decisions[decisions_counter++] = tDecisionsAssets10[num7];
			}
		}
	}

	public void debugFav()
	{
	}

	public void clearWait()
	{
		timer_action = 0f;
	}

	public void makeWait(float pValue = 10f)
	{
		stopMovement();
		timer_action = pValue;
	}

	public void stopSleeping()
	{
		finishStatusEffect("sleeping");
	}

	private void checkStepActionForTile(WorldTile pTile)
	{
		if (pTile.Type.step_action != null && Randy.randomChance(pTile.Type.step_action_chance))
		{
			pTile.Type.step_action(pTile, a);
		}
		Building tBuilding = pTile.building;
		if (tBuilding == null || !tBuilding.asset.flora)
		{
			return;
		}
		BuildingAsset tBuildingAsset = tBuilding.asset;
		switch (tBuildingAsset.flora_type)
		{
		case FloraType.Fungi:
			if (WorldLawLibrary.world_law_exploding_mushrooms.isEnabled())
			{
				MapAction.damageWorld(pTile, 5, AssetManager.terraform.get("grenade"));
				EffectsLibrary.spawnAtTileRandomScale("fx_explosion_small", pTile, 0.1f, 0.15f);
			}
			break;
		case FloraType.Plant:
			if (tBuildingAsset.type == "type_flower" && WorldLawLibrary.world_law_nectar_nap.isEnabled() && Randy.randomChance(0.1f))
			{
				makeSleep(10f);
				break;
			}
			if (WorldLawLibrary.world_law_plants_tickles.isEnabled() && Randy.randomChance(0.3f))
			{
				tryToGetSurprised(pTile);
			}
			if (WorldLawLibrary.world_law_root_pranks.isEnabled() && Randy.randomChance(0.2f))
			{
				makeStunned();
			}
			break;
		}
	}

	public void setLover(Actor pActor)
	{
		lover = pActor;
	}

	public void setBestFriend(Actor pActor, bool pNew)
	{
		data.best_friend_id = pActor.data.id;
		if (pNew)
		{
			changeHappiness("just_made_friend");
		}
	}

	public void becomeLoversWith(Actor pTarget)
	{
		setLover(pTarget);
		pTarget.setLover(this);
		addStatusEffect("fell_in_love", 0f, pColorEffect: false);
		pTarget.addStatusEffect("fell_in_love", 0f, pColorEffect: false);
	}

	public void resetSocialize()
	{
		data.removeInt("socialize");
		timestamp_tween_session_social = 0.0;
	}

	public void addActionWaitAfterLand(float pTimer)
	{
		_action_wait_after_land = true;
		_action_wait_after_land_timer = pTimer;
	}

	private void actionMagnetAnimation(Actor pActor)
	{
		position_height = 0f;
	}

	private bool isSurprisedJump(WorldTile pTile)
	{
		bool num = canSeeTileBasedOnDirection(pTile);
		bool tMakeJump = false;
		if (!num && hasSubspecies() && subspecies.can_process_emotions && (subspecies.has_trait_timid || !hasStatus("on_guard")))
		{
			tMakeJump = true;
		}
		return tMakeJump;
	}

	private void checkLand(Actor pActor)
	{
		if (!should_check_land_cancel)
		{
			return;
		}
		should_check_land_cancel = false;
		if (has_attack_target && isEnemyTargetAlive() && _has_emotions && !hasStatusTantrum())
		{
			if (getHealthRatio() < 0.15f)
			{
				cancelAllBeh();
				setTask("run_away", pClean: true, pCleanJob: false, pForceAction: true);
				return;
			}
			if (Toolbox.DistVec2Float(current_position, attack_target.current_position) < 10f)
			{
				return;
			}
		}
		cancelAllBeh();
	}

	private void checkDeathOutsideMap(Actor pActor)
	{
		if (!inMapBorder())
		{
			getHitFullHealth(AttackType.Gravity);
		}
	}

	public void tryToGetSurprised(WorldTile pTile, bool pForceJump = false)
	{
		if (canBeSurprised(pTile))
		{
			getSurprised(pTile, pForceJump);
		}
	}

	public void getSurprised(WorldTile pTile, bool pForceJump = false)
	{
		if (!_has_emotions)
		{
			return;
		}
		float tTimer = 1f + Randy.randomFloat(0f, 2f);
		bool num = !hasStatus("surprised");
		bool tDoSurprisedJump = pForceJump || isSurprisedJump(pTile);
		if (num)
		{
			addStatusEffect("surprised", tTimer, pColorEffect: false);
			if (hasStatus("just_ate"))
			{
				poop(pApplyForce: false);
				tDoSurprisedJump = true;
			}
		}
		else
		{
			tTimer = 0.1f;
		}
		if (tDoSurprisedJump)
		{
			addActionWaitAfterLand(tTimer);
			applyRandomForce();
		}
		addStatusEffect("on_guard", 0f, pColorEffect: false);
		if (tDoSurprisedJump || !isTask("investigate_curiosity") || !is_moving)
		{
			lookTowardsPosition(pTile.posV3);
			stopMovement();
			cancelAllBeh();
			if (!tDoSurprisedJump)
			{
				makeWait(tTimer);
			}
			scheduleTask("investigate_curiosity", pTile);
		}
		float tRunAwayChance = 0.3f;
		if (hasSubspecies() && subspecies.has_trait_timid)
		{
			tRunAwayChance += 0.3f;
		}
		if (Randy.randomChance(tRunAwayChance))
		{
			cancelAllBeh();
			scheduleTask("run_away", null);
		}
	}

	public bool makeSleep(float pTime)
	{
		bool num = addStatusEffect("sleeping", pTime);
		if (num)
		{
			makeWait(pTime);
		}
		return num;
	}

	public void makeStunned(float pTime = 5f)
	{
		pTime += Randy.randomFloat(0f, pTime * 0.1f);
		cancelAllBeh();
		makeWait(pTime);
		if (addStatusEffect("stunned", pTime))
		{
			finishAngryStatus();
		}
	}

	public void makeStunnedFromUI()
	{
		makeStunned();
		updateStats();
	}

	public void justAte()
	{
		addStatusEffect("just_ate");
	}

	public void poop(bool pApplyForce)
	{
		donePooping();
		float tRandomChance = 1f;
		string tBuildingID;
		if (hasSubspecies())
		{
			tBuildingID = subspecies.getRandomBioProduct();
			tRandomChance = 0.2f;
		}
		else
		{
			tBuildingID = "poop";
		}
		if (tRandomChance >= 1f || Randy.randomChance(tRandomChance))
		{
			BuildingHelper.tryToBuildNear(current_tile, tBuildingID);
		}
		if (pApplyForce)
		{
			applyRandomForce();
		}
	}

	public void donePooping()
	{
		finishStatusEffect("just_ate");
		changeHappiness("just_pooped");
	}

	public void birthEvent(string pAddSpecialTrait = null, string pAddSpecialStatus = null)
	{
		changeHappiness("just_had_child");
		makeStunned(4f);
		spendNutritionOnBirth();
		if (!string.IsNullOrEmpty(pAddSpecialTrait))
		{
			addTrait(pAddSpecialTrait);
		}
		if (!string.IsNullOrEmpty(pAddSpecialStatus))
		{
			addStatusEffect(pAddSpecialStatus);
		}
	}

	public void consumeTopTile(WorldTile pTile)
	{
		if (Randy.randomChance(0.3f))
		{
			World.world.units.addRandomTraitFromBiomeToActor(this, pTile);
		}
		addNutritionFromEating(pTile.Type.nutrition, pSetMaxNutrition: false, pSetJustAte: true);
		countConsumed();
		pTile.topTileEaten();
		pTile.setBurned();
	}

	public void countConsumed()
	{
		data.food_consumed++;
	}

	public void consumeFoodResource(ResourceAsset pAsset)
	{
		ate_last_item_id = pAsset.id;
		timestamp_session_ate_food = World.world.getCurSessionTime();
		if (pAsset.give_experience != 0)
		{
			addExperience(pAsset.give_experience);
		}
		if (pAsset.restore_happiness != 0)
		{
			changeHappiness("just_ate", pAsset.restore_happiness);
		}
		int tRestoreHungerValue = pAsset.restore_nutrition;
		float tRestoreHealthPercent = pAsset.restore_health;
		if (hasFavoriteFood())
		{
			if (pAsset.id != data.favorite_food)
			{
				ResourceAsset tFavoriteFood = favorite_food_asset;
				if (pAsset.tastiness > tFavoriteFood.tastiness && Randy.randomChance(pAsset.favorite_food_chance))
				{
					data.favorite_food = pAsset.id;
				}
			}
		}
		else if (Randy.randomChance(pAsset.favorite_food_chance))
		{
			data.favorite_food = pAsset.id;
		}
		if (pAsset.id == data.favorite_food)
		{
			tRestoreHungerValue *= 2;
			tRestoreHealthPercent *= 2f;
		}
		addNutritionFromEating(tRestoreHungerValue, pSetMaxNutrition: false, pSetJustAte: true);
		restoreHealthPercent(tRestoreHealthPercent);
		countConsumed();
		if (!Randy.randomChance(pAsset.give_chance))
		{
			return;
		}
		ActorTrait[] give_trait = pAsset.give_trait;
		if (give_trait != null && give_trait.Length != 0 && Randy.randomBool())
		{
			ActorTrait tTrait = pAsset.give_trait.GetRandom();
			if (tTrait != null)
			{
				addTrait(tTrait);
			}
		}
		StatusAsset[] give_status = pAsset.give_status;
		if (give_status != null && give_status.Length != 0 && Randy.randomBool())
		{
			StatusAsset tStatus = pAsset.give_status.GetRandom();
			if (tStatus != null)
			{
				addStatusEffect(tStatus);
			}
		}
		if (pAsset.give_action != null && Randy.randomBool())
		{
			pAsset.give_action(pAsset);
		}
	}

	internal void justBorn()
	{
		setActorScale(0.02f);
	}

	public void stopBeingWarrior()
	{
		if (isProfession(UnitProfession.Warrior))
		{
			setProfession(UnitProfession.Unit);
			if (hasCity())
			{
				city.status.warriors_current--;
			}
		}
		removeFromArmy();
	}

	public void pokeFromAvatarUI()
	{
		if (getHealth() > 1)
		{
			getHit(1f, pFlash: true, AttackType.Divine);
		}
		if (Randy.randomChance(0.15f))
		{
			makeStunnedFromUI();
			changeHappiness("got_poked");
		}
		addStatusEffect("motivated");
		applyRandomForce();
		makeSoundAttack();
	}

	public void finishPossessionStatus()
	{
		finishStatusEffect("possessed");
		_has_status_possessed = false;
	}

	public void madePeace(War pWar)
	{
		changeHappiness("just_made_peace");
		if (isKing())
		{
			addRenown(pWar.getRenown(), 0.2f);
		}
		if (isCityLeader())
		{
			addRenown(pWar.getRenown(), 0.05f);
		}
		if (is_army_captain)
		{
			army.addRenown(pWar.getRenown(), 0.05f);
		}
		if (hasTag("love_peace"))
		{
			addStatusEffect("festive_spirit");
		}
	}

	public void warWon(War pWar)
	{
		if (!hasHappinessEntry("was_conquered", 300f))
		{
			if (isKing())
			{
				addRenown(pWar.getRenown());
			}
			if (isCityLeader())
			{
				addRenown(pWar.getRenown(), 0.2f);
			}
			if (isWarrior())
			{
				addRenown(pWar.getRenown(), 0.05f);
			}
			if (is_army_captain)
			{
				army.addRenown(pWar.getRenown(), 0.05f);
			}
			changeHappiness("just_won_war");
		}
		if (hasTag("love_peace"))
		{
			addStatusEffect("festive_spirit");
		}
	}

	public void warLost(War pWar)
	{
		changeHappiness("just_lost_war");
		if (isKing())
		{
			addRenown(pWar.getRenown(), 0.05f);
		}
		if (is_army_captain)
		{
			army.addRenown(pWar.getRenown(), 0.01f);
		}
	}

	public void setTransformed()
	{
		data.set("transformation_done", pData: true);
	}

	public bool isAlreadyTransformed()
	{
		data.get("transformation_done", out var tValue, false);
		return tValue;
	}

	public void makeConfused(float pConfusedTimer = -1f, bool pColorEffect = false)
	{
		cancelAllBeh();
		if (pColorEffect)
		{
			startColorEffect();
		}
		addStatusEffect("confused", pConfusedTimer, pColorEffect);
		makeStunned(3f);
	}

	public void checkShouldBeEgg()
	{
		if (hasSubspecies() && subspecies.has_egg_form && (float)age < subspecies.age_adult)
		{
			float tMaturationTime = getMaturationTimeSeconds();
			addStatusEffect("egg", tMaturationTime);
		}
	}

	public void leavePlot()
	{
		setPlot(null);
	}

	private void levelUp()
	{
		int tExpToLevelUp = getExpToLevelup();
		int tMaxLevel = getMaxPossibleLevel();
		data.experience = 0;
		data.level++;
		if (hasCulture() && culture.hasTrait("training_potential"))
		{
			data.level++;
		}
		if (data.level == tMaxLevel)
		{
			data.experience = tExpToLevelUp;
		}
		setStatsDirty();
		EffectsLibrary.showMetaEventEffect("fx_experience_gain", this);
	}

	private void checkGrowthEvent()
	{
		bool tWasBaby = isBaby();
		bool num = isEgg();
		calcAgeStates();
		if (animation_container != null && animation_container.child != isBaby())
		{
			clearSprites();
		}
		if (num && !isEgg())
		{
			batch.c_events_hatched.Add(this);
		}
		else if (tWasBaby && !isBaby())
		{
			batch.c_events_become_adult.Add(this);
		}
	}

	internal void eventHatchFromEgg()
	{
		growthStateEvent();
		triggerHatchFromEggAction();
		applyRandomForce();
		changeHappiness("just_got_out_of_egg");
		batch.c_events_hatched.Remove(this);
	}

	internal void eventBecomeAdult()
	{
		growthStateEvent();
		changeHappiness("just_became_adult");
		checkTraitMutationGrowUp();
		batch.c_events_become_adult.Remove(this);
		subspecies.counter_new_adults?.registerEvent();
	}

	private void growthStateEvent()
	{
		setStatsDirty();
		event_full_stats = true;
		if (hasCity())
		{
			city.setCitizensDirty();
			city.setStatusDirty();
		}
	}

	private void triggerHatchFromEggAction()
	{
		SubspeciesTrait tEgg = subspecies.egg_asset;
		if (tEgg != null && tEgg.has_after_hatch_from_egg_action)
		{
			tEgg.after_hatch_from_egg_action(this);
		}
	}

	public bool checkNaturalDeath()
	{
		if (!WorldLawLibrary.world_law_old_age.isEnabled())
		{
			return false;
		}
		if (hasTrait("immortal"))
		{
			return false;
		}
		float tAge = getAge();
		float tLifespan = stats["lifespan"];
		if (tLifespan == 0f)
		{
			return false;
		}
		if (tAge <= tLifespan)
		{
			return false;
		}
		float tOverAge = tAge - tLifespan;
		float tSeverity = 5f;
		if (Randy.randomChance(Mathf.Clamp(1f / (1f + Mathf.Exp((0f - tSeverity) * (tOverAge / tLifespan - 0.5f))), 0f, 0.9f)))
		{
			getHitFullHealth(AttackType.Age);
			return true;
		}
		return false;
	}

	public void spawnParticle(Color pColor)
	{
		if (!Randy.randomBool() && MapBox.isRenderGameplay())
		{
			Vector3 tVec = current_position;
			tVec.y += 0.5f * current_scale.y / 2f;
			tVec.x += Randy.randomFloat(-0.2f, 0.2f);
			tVec.y += Randy.randomFloat(-0.2f, 0.2f);
			BaseEffect tEffects = EffectsLibrary.spawn("fx_status_particle");
			if (tEffects != null)
			{
				((StatusParticle)tEffects).spawnParticle(tVec, pColor);
			}
		}
	}

	private void checkActionsFromAllMetas()
	{
		if (hasSubspecies())
		{
			addSpecialEffectsFromMetas(subspecies.all_actions_actor_special_effect);
			s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, subspecies.all_actions_actor_attack_target);
			s_get_hit_action = (GetHitAction)Delegate.Combine(s_get_hit_action, subspecies.all_actions_actor_get_hit);
		}
		if (hasClan())
		{
			addSpecialEffectsFromMetas(clan.all_actions_actor_special_effect);
			s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, clan.all_actions_actor_attack_target);
			s_get_hit_action = (GetHitAction)Delegate.Combine(s_get_hit_action, clan.all_actions_actor_get_hit);
		}
		if (hasLanguage())
		{
			addSpecialEffectsFromMetas(language.all_actions_actor_special_effect);
			s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, language.all_actions_actor_attack_target);
			s_get_hit_action = (GetHitAction)Delegate.Combine(s_get_hit_action, language.all_actions_actor_get_hit);
		}
		if (hasCulture())
		{
			addSpecialEffectsFromMetas(culture.all_actions_actor_special_effect);
			s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, culture.all_actions_actor_attack_target);
			s_get_hit_action = (GetHitAction)Delegate.Combine(s_get_hit_action, culture.all_actions_actor_get_hit);
		}
		if (hasReligion())
		{
			addSpecialEffectsFromMetas(religion.all_actions_actor_special_effect);
			s_action_attack_target = (AttackAction)Delegate.Combine(s_action_attack_target, religion.all_actions_actor_attack_target);
			s_get_hit_action = (GetHitAction)Delegate.Combine(s_get_hit_action, religion.all_actions_actor_get_hit);
		}
	}

	private void recalcCombatActions()
	{
		foreach (ActorTrait tTrait in traits)
		{
			if (tTrait.hasCombatActions())
			{
				_combat_actions.mergeWith(tTrait.combat_actions);
			}
		}
		checkCombatActions(subspecies?.combat_actions);
		checkCombatActions(clan?.combat_actions);
		checkCombatActions(religion?.combat_actions);
	}

	private void recalcSpells()
	{
		foreach (ActorTrait tTrait in traits)
		{
			if (tTrait.hasSpells())
			{
				_spells.mergeWith(tTrait.spells);
			}
		}
		if (!hasEquipment())
		{
			return;
		}
		foreach (ActorEquipmentSlot tSlot in equipment)
		{
			if (!tSlot.isEmpty())
			{
				Item tItem = tSlot.getItem();
				if (tItem.asset.hasSpells())
				{
					_spells.mergeWith(tItem.asset.spells);
				}
			}
		}
	}

	private void checkSpells(SpellHolder pSpellsHolder)
	{
		if (pSpellsHolder != null && pSpellsHolder.hasAny())
		{
			_spells.mergeWith(pSpellsHolder);
		}
	}

	private void checkCombatActions(CombatActionHolder pHolder)
	{
		if (pHolder != null && !pHolder.isEmpty())
		{
			_combat_actions.mergeWith(pHolder);
		}
	}

	public List<CombatActionAsset> getCombatActionPool(CombatActionPool pPool)
	{
		if (!_combat_actions.hasAny())
		{
			return null;
		}
		return _combat_actions.getPool(pPool);
	}

	private void clearCombatActions()
	{
		_combat_actions.reset();
	}

	private void clearSpells()
	{
		_spells.reset();
	}

	private bool checkCurrentEnemyTarget()
	{
		if (shouldSkipFightCheck())
		{
			return false;
		}
		if (!has_attack_target)
		{
			return false;
		}
		if (!isEnemyTargetAlive())
		{
			return false;
		}
		BaseSimObject tAttackTarget = attack_target;
		Actor tAttackTargetActor = attack_target.a;
		if (isKingdomCiv() && tAttackTarget.isKingdomCiv() && !shouldContinueToAttackTarget())
		{
			clearAttackTarget();
			return false;
		}
		if (tAttackTarget.isActor() && !hasStatusTantrum() && !tAttackTarget.areFoes(this) && tAttackTarget.a.is_unconscious)
		{
			clearAttackTarget();
			return false;
		}
		if (canAttackTarget(tAttackTarget, pCheckForFactions: true, asset.can_attack_buildings))
		{
			bool tAttackPossible = isAttackPossible();
			bool tInAttackInRange = isInAttackRange(tAttackTarget);
			if (!tInAttackInRange)
			{
				float tDist = distanceToObjectTarget(tAttackTarget);
				if (tDist > 20f && tAttackTargetActor != null && tAttackTargetActor.isTask("run_away"))
				{
					clearAttackTarget();
					return false;
				}
				if (tDist > 50f)
				{
					clearAttackTarget();
					return false;
				}
				if (tDist > 3f && tryToUseAdvancedCombatAction(getCombatActionPool(CombatActionPool.BEFORE_ATTACK_MELEE), tAttackTarget, out var tCombatAsset))
				{
					tCombatAsset.action_actor_target_position(this, tAttackTarget.current_position, tAttackTarget.current_tile);
					return false;
				}
			}
			if (attack_timer > 0f || (!tAttackPossible && tInAttackInRange))
			{
				stopMovement();
				if (hasRangeAttack() && tryToUseAdvancedCombatAction(getCombatActionPool(CombatActionPool.BEFORE_ATTACK_RANGE), tAttackTarget, out var tCombatAsset2))
				{
					tCombatAsset2.action_actor_target_position(this, tAttackTarget.current_position, tAttackTarget.current_tile);
				}
				return true;
			}
			if (tInAttackInRange && tryToAttack(tAttackTarget, pDoChecks: false))
			{
				stopMovement();
				return true;
			}
		}
		return false;
	}

	private bool checkEnemyTargets()
	{
		if (!isAllowedToLookForEnemies())
		{
			return false;
		}
		if (isInWaterAndCantAttack())
		{
			return false;
		}
		if (_has_status_strange_urge)
		{
			return false;
		}
		if (has_attack_target)
		{
			if (!hasTask() || !ai.task.in_combat)
			{
				setTask("fighting", pClean: true, pCleanJob: true);
			}
			return false;
		}
		if (_timeout_targets > 0f)
		{
			return false;
		}
		_timeout_targets = 0.1f + Randy.randomFloat(0f, 1f);
		BaseSimObject tNewTarget = findEnemyObjectTarget(asset.can_attack_buildings);
		if (tNewTarget == null && _aggression_targets.Count > 0)
		{
			using ListPool<Actor> tList = new ListPool<Actor>(_aggression_targets.Count);
			foreach (long tID in _aggression_targets)
			{
				Actor tActor = World.world.units.get(tID);
				if (!tActor.isRekt())
				{
					tList.Add(tActor);
				}
			}
			if (tList.Count > 0)
			{
				tNewTarget = checkObjectList(tList, asset.can_attack_buildings, pFindClosest: true, pIgnoreStunned: true, 30);
			}
			else
			{
				_aggression_targets.Clear();
			}
		}
		if (tNewTarget == null)
		{
			return false;
		}
		startFightingWith(tNewTarget);
		return true;
	}

	public void startFightingWith(BaseSimObject pSimObject)
	{
		setAttackTarget(pSimObject);
		setTask("fighting", pClean: false, pCleanJob: true);
		beh_actor_target = pSimObject;
	}

	internal void startAttackCooldown()
	{
		attack_timer = getAttackCooldown();
		last_attack_timestamp = World.world.getCurWorldTime();
	}

	internal bool isJustAttacked()
	{
		return World.world.getWorldTimeElapsedSince(last_attack_timestamp) < 0.25f;
	}

	internal bool tryToAttack(BaseSimObject pTarget, bool pDoChecks = true, Action pKillAction = null, Vector3 pAttackPosition = default(Vector3), Kingdom pForceKingdom = null, WorldTile pTileTarget = null, float pBonusAreOfEffect = 0f)
	{
		if (pDoChecks)
		{
			if (hasMeleeAttack() && pTarget != null && pTarget.position_height > 0f)
			{
				return false;
			}
			if (isInWaterAndCantAttack())
			{
				return false;
			}
			if (!isAttackPossible())
			{
				return false;
			}
			if (pTarget != null && !isInAttackRange(pTarget))
			{
				return false;
			}
		}
		float tZ = 0f;
		float tTargetSize = 0f;
		Vector3 tTargetPos;
		if (pTarget != null)
		{
			tTargetPos = pTarget.current_position;
			tZ = pTarget.getHeight();
			tTargetSize = pTarget.stats["size"];
		}
		else
		{
			tTargetPos = pAttackPosition;
		}
		bool tPossessed = _has_status_possessed;
		startAttackCooldown();
		punchTargetAnimation(tTargetPos, pFlip: true, hasRangeAttack());
		Vector3 tAttackPosition = new Vector3(tTargetPos.x, tTargetPos.y);
		if (pTarget != null && pTarget.isActor() && pTarget.a.is_moving && pTarget.isFlying())
		{
			tAttackPosition = Vector3.MoveTowards(tAttackPosition, pTarget.a.next_step_position, tTargetSize * 3f);
		}
		Vector3 tCurrentPos = current_position;
		float tDist = Vector2.Distance(tCurrentPos, tTargetPos) + tZ;
		Vector3 tAttackVector = Toolbox.getNewPoint(tCurrentPos.x, tCurrentPos.y, tAttackPosition.x, tAttackPosition.y, tDist - tTargetSize);
		string tProjectileID = getWeaponAsset().projectile;
		bool tRangeAttack = hasRangeAttack();
		Kingdom tKingdomForData = pForceKingdom ?? kingdom;
		WorldTile tHitTileTarget = pTileTarget ?? pTarget?.current_tile;
		Kingdom pKingdom = tKingdomForData;
		Vector3 pInitiatorPosition = tCurrentPos;
		AttackData tAttackData = new AttackData(this, tHitTileTarget, tAttackVector, pInitiatorPosition, pTarget, pKingdom, AttackType.Weapon, haveMetallicWeapon(), pSkipShake: true, tRangeAttack, tProjectileID, pKillAction, pBonusAreOfEffect);
		using ListPool<CombatActionAsset> tRandomPool = new ListPool<CombatActionAsset>();
		CombatActionAsset tCombatAsset = null;
		bool tCombatActionDone = false;
		if (hasSpells() && canUseSpells() && !tPossessed)
		{
			addToAttackPool(CombatActionLibrary.combat_cast_spell, tRandomPool);
		}
		if (tRandomPool.Count > 0)
		{
			if (hasMeleeAttack())
			{
				addToAttackPool(CombatActionLibrary.combat_attack_melee, tRandomPool);
			}
			else
			{
				addToAttackPool(CombatActionLibrary.combat_attack_range, tRandomPool);
			}
			tCombatAsset = tRandomPool.GetRandom();
			tCombatActionDone = tCombatAsset.action(tAttackData);
			if (!tCombatActionDone && !tCombatAsset.basic)
			{
				tCombatActionDone = ((!hasMeleeAttack()) ? CombatActionLibrary.combat_attack_range.action(tAttackData) : CombatActionLibrary.combat_attack_melee.action(tAttackData));
			}
		}
		else
		{
			tCombatAsset = ((!hasMeleeAttack()) ? CombatActionLibrary.combat_attack_range : CombatActionLibrary.combat_attack_melee);
			tCombatActionDone = tCombatAsset.action(tAttackData);
		}
		if (tCombatActionDone)
		{
			spendStamina(tCombatAsset.cost_stamina);
			spendMana(tCombatAsset.cost_mana);
		}
		if (tCombatAsset.play_unit_attack_sounds)
		{
			makeSoundAttack();
		}
		if (needsFood() && Randy.randomBool())
		{
			decreaseNutrition();
		}
		float tRecoil = stats.get("recoil");
		if (tRecoil > 0f)
		{
			calculateForce(current_position.x, current_position.y, tAttackPosition.x, tAttackPosition.y, tRecoil);
		}
		return true;
	}

	internal override void getHitFullHealth(AttackType pAttackType)
	{
		getHit(getHealth(), pFlash: false, pAttackType, null, pSkipIfShake: false, pMetallicWeapon: false, pCheckDamageReduction: false);
	}

	internal override void getHit(float pDamage, bool pFlash, AttackType pAttackType, BaseSimObject pAttacker = null, bool pSkipIfShake = true, bool pMetallicWeapon = false, bool pCheckDamageReduction = true)
	{
		_last_attack_type = pAttackType;
		if (_cache_check_has_status_removed_on_damage)
		{
			foreach (Status tStatusData in getStatuses())
			{
				if (!tStatusData.is_finished && tStatusData.asset.removed_on_damage)
				{
					finishStatusEffect(tStatusData.asset.id);
				}
			}
		}
		if (DebugConfig.isOn(DebugOption.IgnoreDamage) || (pSkipIfShake && _shake_active))
		{
			return;
		}
		attackedBy = null;
		if (pAttacker.isRekt())
		{
			pAttacker = null;
		}
		if (pAttacker != this)
		{
			attackedBy = pAttacker;
		}
		if (!hasHealth() || is_invincible)
		{
			return;
		}
		Actor tAttackerUnit = pAttacker?.a;
		if (pAttackType == AttackType.Weapon)
		{
			bool tClank = false;
			if (pMetallicWeapon && haveMetallicWeapon())
			{
				tClank = true;
			}
			if (tClank)
			{
				MusicBox.playSound("event:/SFX/HIT/HitSwordSword", current_tile, pGameViewOnly: false, pVisibleOnly: true);
			}
			else if (asset.has_sound_hit)
			{
				MusicBox.playSound(asset.sound_hit, current_tile, pGameViewOnly: false, pVisibleOnly: true);
			}
			if (tAttackerUnit != null && !hasStatus("shield"))
			{
				damageEquipmentOnGetHit(tAttackerUnit);
			}
		}
		if (pCheckDamageReduction)
		{
			if (pAttackType == AttackType.Other || pAttackType == AttackType.Weapon)
			{
				float tArmorPercent = 1f - stats["armor"] / 100f;
				pDamage *= tArmorPercent;
			}
			if (pDamage < 1f)
			{
				pDamage = 1f;
			}
			if (tAttackerUnit != null)
			{
				checkSpecialAttackLogic(tAttackerUnit, pAttackType, pDamage, out var tFinalDamage);
				pDamage = tFinalDamage;
				AchievementLibrary.clone_wars.checkBySignal((this, tAttackerUnit));
			}
		}
		changeHealth((int)(0f - pDamage));
		timer_action = 0.002f;
		s_get_hit_action?.Invoke(this, pAttacker, current_tile);
		if (pFlash)
		{
			startColorEffect(ActorColorEffect.Red);
		}
		if (!hasHealth())
		{
			batch.c_check_deaths.Add(this);
		}
		if (pAttackType == AttackType.Weapon && !asset.immune_to_injuries && !hasStatus("shield"))
		{
			if (Randy.randomChance(0.02f))
			{
				addInjuryTrait("crippled");
			}
			if (Randy.randomChance(0.02f))
			{
				addInjuryTrait("eyepatch");
			}
		}
		startShake();
		if (!has_attack_target)
		{
			if (attackedBy != null && !shouldIgnoreTarget(attackedBy) && canAttackTarget(attackedBy, pCheckForFactions: false))
			{
				setAttackTarget(attackedBy);
			}
		}
		else if (hasMeleeAttack() && attackedBy != null && canAttackTarget(attackedBy, pCheckForFactions: false))
		{
			float tDistToCurrentTarget = Toolbox.SquaredDistVec2Float(current_position, attack_target.current_position);
			float tDistToAttacker = Toolbox.SquaredDistVec2Float(current_position, pAttacker.current_position);
			if (tDistToCurrentTarget > getAttackRangeSquared() && tDistToAttacker < tDistToCurrentTarget)
			{
				setAttackTarget(pAttacker);
			}
		}
		if (hasAnyStatusEffect())
		{
			foreach (Status status in getStatuses())
			{
				status.asset.action_get_hit?.Invoke(this, pAttacker, current_tile);
			}
		}
		asset.action_get_hit?.Invoke(this, pAttacker, current_tile);
		if (!hasHealth())
		{
			checkCallbacksOnDeath();
		}
	}

	private void pickupResourcesFromKill(Actor pAttacker)
	{
		if (!pAttacker.hasCity())
		{
			return;
		}
		foreach (ResourceContainer tResource in getResourcesFromActor())
		{
			if (!isSameSpecies(pAttacker) || pAttacker.hasTrait("savage"))
			{
				pAttacker.addToInventory(tResource);
			}
		}
	}

	private void checkSpecialAttackLogic(Actor pAttacker, AttackType pAttackType, float pInitialDamage, out float pDamageFinal)
	{
		pDamageFinal = pInitialDamage;
		bool tIsSameKingdom = isSameKingdom(pAttacker);
		bool tLucky = false;
		bool tShouldStun = false;
		bool tAttackerFriendHasTantrum = pAttacker.hasStatus("tantrum") && !tIsSameKingdom;
		bool tAttackHasPossessed = pAttacker.hasStatus("possessed");
		bool num = kingdom.isEnemy(pAttacker.kingdom);
		float tStunChance = 0.1f;
		if (_has_status_possessed || tAttackHasPossessed)
		{
			tStunChance = 0.7f;
		}
		else if (tIsSameKingdom)
		{
			tStunChance = 0.5f;
		}
		if (num)
		{
			tStunChance = 0f;
		}
		tShouldStun = Randy.randomChance(tStunChance);
		if (tAttackerFriendHasTantrum)
		{
			tShouldStun = true;
		}
		if (getHealthRatio() < 0.5f && tShouldStun)
		{
			pDamageFinal = 1f;
			makeStunned();
			changeHappiness("lost_fight");
			finishAngryStatus();
			tLucky = true;
			if (tAttackerFriendHasTantrum)
			{
				pAttacker.finishStatusEffect("tantrum");
			}
			if (Randy.randomChance(0.4f))
			{
				pAttacker.finishAngryStatus();
			}
		}
		bool tIsAggroChecked = false;
		if (tIsSameKingdom && pAttackType != AttackType.Eaten)
		{
			if (Randy.randomChance(0.3f) || tAttackHasPossessed || pAttacker.hasStatus("angry"))
			{
				checkAggroAgainst(pAttacker, tAttackHasPossessed);
				tIsAggroChecked = true;
			}
			if (tLucky)
			{
				pDamageFinal = 0f;
				pAttacker.clearAttackTarget();
				pAttacker.makeWait(0.3f);
				if (pAttacker.hasStatus("angry"))
				{
					pAttacker.finishAngryStatus();
				}
			}
		}
		if (!tIsAggroChecked && tAttackHasPossessed)
		{
			checkAggroAgainst(pAttacker);
		}
	}

	private void damageEquipmentOnGetHit(Actor pAttacker)
	{
		if (!pAttacker.hasWeapon() || !hasEquipment())
		{
			return;
		}
		int tDamageMultiplier = 4;
		float tBowDamageChance = 0.35f;
		Item tAttackerWeapon = pAttacker.getWeapon();
		EquipmentAsset tAttackerWeaponAsset = tAttackerWeapon.getAsset();
		int tAttackerRigidityRating = tAttackerWeaponAsset.rigidity_rating;
		int tSumRigidityOfAttacked = 0;
		bool tSetStatsDirty = false;
		foreach (ActorEquipmentSlot tSlot in equipment)
		{
			if (Randy.randomBool())
			{
				continue;
			}
			Item tItem = tSlot.getItem();
			if (!tItem.isBroken())
			{
				EquipmentAsset equipmentAsset = tItem.getAsset();
				int tRigidityRating = equipmentAsset.rigidity_rating;
				if (!equipmentAsset.is_pool_weapon)
				{
					tSumRigidityOfAttacked += tRigidityRating;
				}
				int tDamage = tAttackerRigidityRating / tRigidityRating * tDamageMultiplier;
				tItem.getDamaged(tDamage);
				if (tItem.isBroken())
				{
					tSetStatsDirty = true;
				}
			}
		}
		if (tSetStatsDirty)
		{
			setStatsDirty();
		}
		if (!tAttackerWeapon.isBroken() && !Randy.randomBool())
		{
			if (tAttackerWeaponAsset.attack_type == WeaponType.Melee)
			{
				int tDamageToAttackerWeapon = tSumRigidityOfAttacked / 5 / tAttackerRigidityRating * tDamageMultiplier;
				tAttackerWeapon.getDamaged(tDamageToAttackerWeapon);
			}
			else if (tAttackerWeaponAsset.attack_type == WeaponType.Range && Randy.randomChance(tBowDamageChance))
			{
				tAttackerWeapon.getDamaged(1);
			}
			if (tAttackerWeapon.isBroken())
			{
				pAttacker.setStatsDirty();
			}
		}
	}

	public void addInjuryTrait(string pTraitID)
	{
		if (addTrait(pTraitID))
		{
			changeHappiness("just_injured");
		}
	}

	private void checkAggroAgainst(Actor pAttackedBy, bool pCheckAllLists = false)
	{
		addAggro(pAttackedBy);
		if (!pCheckAllLists)
		{
			return;
		}
		if (hasFamily())
		{
			family.allAngryAt(pAttackedBy, 10f);
		}
		if (hasClan())
		{
			clan.allAngryAt(pAttackedBy, 10f);
		}
		if (hasCity() && isBaby())
		{
			city.allAngryAt(pAttackedBy, 10f);
		}
		if (hasLover())
		{
			lover.addAggro(pAttackedBy);
		}
		if (hasBestFriend())
		{
			getBestFriend().addAggro(pAttackedBy);
		}
		if ((!isKing() && !isWarrior() && !isCityLeader() && !isBaby()) || pAttackedBy.isKing())
		{
			return;
		}
		foreach (City tCity in kingdom.getCities())
		{
			if (tCity.hasArmy())
			{
				tCity.army.allAngryAt(pAttackedBy, 10f);
			}
		}
	}

	internal void newKillAction(Actor pDeadUnit, Kingdom pPrevKingdom, AttackType pAttackType)
	{
		increaseKills();
		if (hasWeapon())
		{
			getWeapon().countKill();
		}
		if (isKingdomCiv() && pPrevKingdom.isCiv())
		{
			War tWar = World.world.wars.getWar(kingdom, pPrevKingdom, pOnlyMain: false);
			if (tWar != null)
			{
				if (tWar.isAttacker(kingdom))
				{
					tWar.increaseDeathsDefenders(pAttackType);
				}
				else
				{
					tWar.increaseDeathsAttackers(pAttackType);
				}
			}
		}
		if (isAlive())
		{
			if (timer_action <= 0f)
			{
				makeWait(Randy.randomFloat(0.1f, 1f));
			}
			if (hasTrait("bloodlust"))
			{
				changeHappiness("just_killed");
			}
			int tMoneys = pDeadUnit.giveAllLootAndMoney();
			addLoot(tMoneys);
			takeAllResources(pDeadUnit);
			if (data.kills > 10)
			{
				addTrait("veteran");
			}
			if (pDeadUnit.isKing())
			{
				addTrait("kingslayer");
			}
			addExperience(pDeadUnit.asset.experience_given);
			addRenown(pDeadUnit.asset.experience_given);
			if (hasTrait("madness"))
			{
				restoreHealth(getMaxHealthPercent(0.05f));
			}
			if (understandsHowToUseItems() && !pDeadUnit.hasTrait("infected") && canTakeItems())
			{
				takeItems(pDeadUnit);
			}
			checkRageDemon();
		}
	}

	internal void applyRandomForce(float pMinHeight = 1.5f, float pMaxHeight = 2f)
	{
		float tForceDirection = Randy.randomFloat(1.5f, 2f);
		float tForceHeight = Randy.randomFloat(pMinHeight, pMaxHeight);
		WorldTile tRandomTile = current_tile.neighboursAll.GetRandom();
		calculateForce(current_tile.posV3.x, current_tile.posV3.y, tRandomTile.posV3.x, tRandomTile.posV3.y, tForceDirection, tForceHeight, pCheckCancelJobOnLand: true);
	}

	internal void calculateForce(float pStartX, float pStartY, float pTargetX, float pTargetY, float pForceAmountDirection, float pForceHeight = 0f, bool pCheckCancelJobOnLand = false)
	{
		if (pForceHeight == 0f)
		{
			pForceHeight = pForceAmountDirection;
		}
		pForceAmountDirection *= SimGlobals.m.unit_force_multiplier;
		pForceHeight *= SimGlobals.m.unit_force_multiplier;
		if (!(pForceAmountDirection <= 0f))
		{
			float angle = Toolbox.getAngle(pStartX, pStartY, pTargetX, pTargetY);
			float tForceDirectionX = (0f - Mathf.Cos(angle)) * pForceAmountDirection;
			float tForceDirectionY = (0f - Mathf.Sin(angle)) * pForceAmountDirection;
			if (pStartX == pTargetX && pStartY == pTargetY)
			{
				tForceDirectionX = 0f;
				tForceDirectionY = 0f;
			}
			addForce(tForceDirectionX, tForceDirectionY, pForceHeight, pCheckCancelJobOnLand);
		}
	}

	public bool tryToUseAdvancedCombatAction(List<CombatActionAsset> pCombatActionAssetsCategory, BaseSimObject pAttackTarget, out CombatActionAsset pResultCombatAsset)
	{
		pResultCombatAsset = null;
		if (pCombatActionAssetsCategory == null)
		{
			return false;
		}
		if (pCombatActionAssetsCategory.Count == 0)
		{
			return false;
		}
		if (hasTrait("slow"))
		{
			return false;
		}
		if (combatActionOnTimeout())
		{
			return false;
		}
		using ListPool<CombatActionAsset> tPossibleList = new ListPool<CombatActionAsset>(pCombatActionAssetsCategory.Count);
		foreach (CombatActionAsset tCombatActionAsset in pCombatActionAssetsCategory)
		{
			if (!hasEnoughStamina(tCombatActionAsset.cost_stamina) || !hasEnoughMana(tCombatActionAsset.cost_mana))
			{
				continue;
			}
			if (pAttackTarget != null)
			{
				CombatActionCheckStart can_do_action = tCombatActionAsset.can_do_action;
				if (can_do_action != null && !can_do_action(this, pAttackTarget))
				{
					continue;
				}
			}
			tPossibleList.Add(tCombatActionAsset);
		}
		if (tPossibleList.Count == 0)
		{
			return false;
		}
		CombatActionAsset tAsset = tPossibleList.GetRandom();
		if (!Randy.randomChance(tAsset.chance + tAsset.chance * stats["skill_combat"]))
		{
			return false;
		}
		spendStamina(tAsset.cost_stamina);
		spendMana(tAsset.cost_mana);
		pResultCombatAsset = tAsset;
		addStatusEffect("recovery_combat_action", pResultCombatAsset.cooldown, pColorEffect: false);
		return true;
	}

	public void addAggro(long pActorID)
	{
		Actor tActor = World.world.units.get(pActorID);
		if (!tActor.isRekt())
		{
			addAggro(tActor);
		}
	}

	public void addAggro(Actor pActor)
	{
		if (!pActor.isRekt() && pActor != this)
		{
			addStatusEffect("angry", 0f, pColorEffect: false);
			_aggression_targets.Add(pActor.getID());
		}
	}

	public void finishAngryStatus()
	{
		_aggression_targets.Clear();
		finishStatusEffect("angry");
	}

	public void spawnSlashPunch(Vector2 pTowardsPosition)
	{
		spawnSlash(pTowardsPosition, "effects/slashes/slash_punch");
	}

	public void spawnSlashSteal(Vector2 pTowardsPosition)
	{
		spawnSlash(pTowardsPosition, "effects/slashes/slash_steal");
	}

	public void spawnSlashYell(Vector2 pTowardsPosition)
	{
		spawnSlash(pTowardsPosition, "effects/slashes/slash_swear");
	}

	public void spawnSlashTalk(Vector2 pTowardsPosition)
	{
		spawnSlash(pTowardsPosition, "effects/slashes/slash_talk");
	}

	public void spawnSlashKick(Vector2 pTowardsPosition)
	{
		spawnSlash(pTowardsPosition, "effects/slashes/slash_kick", 2f, 0f, (0f - actor_scale) * 8f);
	}

	public void spawnSlash(Vector2 pTowardsPosition, string pSlashType = null, float pDistMod = 2f, float pTargetZ = 0f, float pStartZ = 0f, float? pAngle = null)
	{
		if (is_visible && EffectsLibrary.canShowSlashEffect())
		{
			if (string.IsNullOrEmpty(pSlashType))
			{
				pSlashType = _attack_asset.path_slash_animation;
			}
			Vector2 tSlashPosition = getSlashPosition(this, pTowardsPosition, pDistMod, pTargetZ, pStartZ);
			float tAngle = (pAngle.HasValue ? pAngle.Value : getSlashAngle(tSlashPosition, pTowardsPosition));
			EffectsLibrary.spawnSlash(tSlashPosition, pSlashType, tAngle, actor_scale);
		}
	}

	public float getSlashAngle(Vector2 pSlashVector, Vector2 pAttackPosition)
	{
		return Toolbox.getAngleDegrees(pSlashVector.x, pSlashVector.y, pAttackPosition.x, pAttackPosition.y);
	}

	public Vector2 getSlashPosition(Actor pActor, Vector2 pAttackPosition, float pDistMod, float pTargetZ = 0f, float pStartZ = 0f)
	{
		float tScaleMod = pActor.getScaleMod();
		float num = pActor.stats["size"];
		Vector2 tSlashStartPosition = new Vector2(pActor.current_position.x, pActor.current_position.y);
		tSlashStartPosition.y += pActor.getHeight();
		tSlashStartPosition.y += 0.5f * tScaleMod;
		tSlashStartPosition.y += pStartZ;
		float tDist = num * tScaleMod * pDistMod;
		return Toolbox.getNewPointVec2(tSlashStartPosition.x, tSlashStartPosition.y, pAttackPosition.x, pAttackPosition.y + pTargetZ, tDist);
	}

	public void doCastAnimation()
	{
		if (is_visible)
		{
			Vector2 tHeadPos = getRenderedItemPosition();
			Vector3 tPos = cur_transform_position;
			EffectsLibrary.spawnAt(asset.effect_cast_ground, tPos, stats["scale"]);
			tPos.y += tHeadPos.y * 6f * current_scale.y;
			EffectsLibrary.spawnAt(asset.effect_cast_top, tPos, stats["scale"]);
		}
	}

	internal void punchTargetAnimation(Vector3 pDirection, bool pFlip = true, bool pReverse = false, float pAngle = 40f)
	{
		if (!asset.can_flip)
		{
			return;
		}
		if (pFlip && checkFlip())
		{
			if (current_position.x < pDirection.x)
			{
				setFlip(pFlip: true);
			}
			else
			{
				setFlip(pFlip: false);
			}
		}
		if (pReverse)
		{
			target_angle.z = 0f - pAngle;
		}
		else
		{
			target_angle.z = pAngle;
		}
	}

	private void addToAttackPool(CombatActionAsset pAsset, ListPool<CombatActionAsset> pPool)
	{
		for (int i = 0; i < pAsset.rate; i++)
		{
			pPool.Add(pAsset);
		}
	}

	private void checkHappinessChangeFromDeathEvent()
	{
		foreach (Actor parent in getParents())
		{
			parent.changeHappiness("death_child");
		}
		getBestFriend()?.changeHappiness("death_best_friend");
		if (hasLover())
		{
			lover.changeHappiness("death_lover");
			lover.finishStatusEffect("fell_in_love");
		}
		if (!hasFamily())
		{
			return;
		}
		foreach (Actor tFamilyMember in family.units)
		{
			if (tFamilyMember != this && !tFamilyMember.isParentOf(this))
			{
				tFamilyMember.changeHappiness("death_family_member");
			}
		}
	}

	private void checkCallbacksOnDeath()
	{
		current_tile.Type.unit_death_action?.Invoke(this, current_tile);
		asset.action_death?.Invoke(this, current_tile);
		using ListPool<ActorTrait> tListCurrentTraits = new ListPool<ActorTrait>(getTraits());
		for (int i = 0; i < tListCurrentTraits.Count; i++)
		{
			tListCurrentTraits[i].action_death?.Invoke(this, current_tile);
		}
		if (hasAnyStatusEffect())
		{
			foreach (Status tStatusEffectData in getStatuses())
			{
				if (tStatusEffectData.asset.action_death != null)
				{
					tStatusEffectData.asset.action_death(this, current_tile);
				}
			}
		}
		if (hasClan())
		{
			clan.all_actions_actor_death?.Invoke(this, current_tile);
		}
		if (hasSubspecies())
		{
			subspecies.all_actions_actor_death?.Invoke(this, current_tile);
		}
		if (hasReligion())
		{
			religion.all_actions_actor_death?.Invoke(this, current_tile);
		}
		callbacks_on_death?.Invoke(this);
	}

	public void checkDeath()
	{
		if (!hasHealth() && isAlive())
		{
			Kingdom tPrevKingdom = kingdom;
			Actor tAttackerUnit = null;
			if (!attackedBy.isRekt() && attackedBy.isActor() && attackedBy != this)
			{
				tAttackerUnit = attackedBy.a;
			}
			if (_last_attack_type == AttackType.Weapon && (isKingdomCiv() || (!tAttackerUnit.isRekt() && tAttackerUnit.isKingdomCiv())))
			{
				BattleKeeperManager.addUnitKilled(this);
			}
			if (tAttackerUnit != null)
			{
				tAttackerUnit.newKillAction(this, tPrevKingdom, _last_attack_type);
				pickupResourcesFromKill(tAttackerUnit);
			}
			die(pDestroy: false, _last_attack_type);
		}
	}

	public void dieSimpleNone()
	{
		die(pDestroy: false, AttackType.None, pCountDeath: false);
	}

	public void dieAndDestroy(AttackType pType)
	{
		die(pDestroy: true, pType, pCountDeath: false);
	}

	public void removeByMetamorphosis()
	{
		die(pDestroy: true, AttackType.Metamorphosis, pCountDeath: false, pLogFavorite: false);
	}

	private void die(bool pDestroy = false, AttackType pType = AttackType.Other, bool pCountDeath = true, bool pLogFavorite = true)
	{
		if (!isAlive() && !pDestroy)
		{
			return;
		}
		switch (pType)
		{
		case AttackType.Plague:
		case AttackType.Infection:
		case AttackType.Tumor:
		case AttackType.Divine:
		case AttackType.AshFever:
		case AttackType.Metamorphosis:
		case AttackType.Starvation:
		case AttackType.Age:
		case AttackType.None:
		case AttackType.Poison:
		case AttackType.Gravity:
		case AttackType.Drowning:
			attackedBy = null;
			break;
		}
		SelectedUnit.removeSelected(this);
		if (ControllableUnit.isControllingUnit(this))
		{
			ControllableUnit.remove(this);
			if (asset.id == "crabzilla")
			{
				pDestroy = true;
			}
		}
		if (isAlive())
		{
			setAlive(pValue: false);
			skipUpdates();
			if (is_inside_boat)
			{
				inside_boat.removePassenger(this);
				exitBoat();
			}
			if (pCountDeath)
			{
				countDeath(pType);
				checkHappinessChangeFromDeathEvent();
			}
			if (isFavorite() && pLogFavorite)
			{
				if (!attackedBy.isRekt() && attackedBy.isActor())
				{
					WorldLog.logFavMurder(this, attackedBy.a);
				}
				else
				{
					WorldLog.logFavDead(this);
				}
			}
			clearTasks();
		}
		exitBuilding();
		clearHomeBuilding();
		using ListPool<Item> tItems = new ListPool<Item>();
		if (hasEquipment())
		{
			tItems.AddRange(equipment.getItems());
			takeAwayItems();
		}
		if (current_tile.zone.hasCity())
		{
			current_tile.zone.city.tryToPutItems(tItems);
			tItems.Clear();
		}
		if (pDestroy)
		{
			World.world.units.scheduleDestroyOnPlay(this);
		}
		if (isKing())
		{
			kingdom.removeKing();
			kingdom.logKingDead(this);
		}
		if (hasCity())
		{
			stopBeingWarrior();
			if (pType == AttackType.Age)
			{
				city.tryToPutItems(tItems);
				tItems.Clear();
			}
			setCity(null);
		}
		if (isKing())
		{
			kingdom.removeKing();
		}
		clearManagers();
		if (hasEquipment())
		{
			equipment.destroyAllEquipment();
		}
		clearAttackTarget();
		clearTileTarget();
	}

	public void checkDieOnGroundBoat()
	{
		if (asset.is_boat && !current_tile.Type.liquid && isAlive() && !isInMagnet())
		{
			getHitFullHealth(AttackType.Gravity);
			skipBehaviour();
			if (hasStatus("magnetized"))
			{
				World.world.game_stats.data.boatsDestroyedByMagnet++;
				AchievementLibrary.boats_disposal.checkBySignal();
			}
		}
	}

	public void copyAggroFrom(Actor pTarget)
	{
		if (!pTarget.hasStatus("angry"))
		{
			return;
		}
		foreach (long tAggroTarget in pTarget._aggression_targets)
		{
			addAggro(tAggroTarget);
		}
		if (!pTarget.attackedBy.isRekt() && pTarget.attackedBy.isActor())
		{
			addAggro(pTarget.attackedBy.a);
		}
		if (!pTarget.attack_target.isRekt() && pTarget.attack_target.isActor())
		{
			addAggro(pTarget.attack_target.a);
		}
	}

	public bool isInAggroList(Actor pActor)
	{
		return _aggression_targets.Contains(pActor.getID());
	}

	public bool shouldContinueToAttackTarget()
	{
		BaseSimObject tAttackTarget = attack_target;
		if (areFoes(tAttackTarget))
		{
			return true;
		}
		if (!tAttackTarget.isActor())
		{
			return false;
		}
		if (tAttackTarget.a.hasStatusTantrum())
		{
			return true;
		}
		if (isInAggroList(tAttackTarget.a))
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void clearAttackTarget()
	{
		if (has_attack_target)
		{
			attack_target = null;
			has_attack_target = false;
			batch.c_check_attack_target.Remove(a);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool isEnemyTargetAlive()
	{
		if (has_attack_target)
		{
			if (attack_target.isRekt())
			{
				clearAttackTarget();
				return false;
			}
			if (attack_target.isBuilding() && !attack_target.b.isUsable())
			{
				clearAttackTarget();
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void setAttackTarget(BaseSimObject pAttackTarget)
	{
		attack_target = pAttackTarget;
		if (!has_attack_target)
		{
			has_attack_target = true;
			batch.c_check_attack_target.Add(a);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasRangeAttack()
	{
		return _attack_asset.attack_type == WeaponType.Range;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasMeleeAttack()
	{
		return _attack_asset.attack_type == WeaponType.Melee;
	}

	private void checkAttackTypes()
	{
		EquipmentAsset tWeaponAsset = getWeaponAsset();
		_attack_asset = tWeaponAsset;
	}

	private bool isEquipmentMeleeAttack()
	{
		EquipmentAsset tWeaponAsset = getWeaponAsset();
		if (asset.only_melee_attack)
		{
			return true;
		}
		return tWeaponAsset.attack_type == WeaponType.Melee;
	}

	public float getAttackCooldown()
	{
		return 1f / stats["attack_speed"];
	}

	private void takeAwayItems()
	{
		if (!hasEquipment())
		{
			return;
		}
		foreach (ActorEquipmentSlot tDeadActorSlot in equipment)
		{
			if (!tDeadActorSlot.isEmpty())
			{
				tDeadActorSlot.takeAwayItem();
			}
		}
	}

	public bool isInDangerZone()
	{
		if (hasCity())
		{
			return city.danger_zones.Contains(base.current_zone);
		}
		return false;
	}

	public void setPossessionAttackHappened()
	{
		_possession_attack_happened_frame = World.world.getCurWorldTime();
	}

	public bool isPossessionAttackJustHappened()
	{
		return World.world.getCurWorldTime() - _possession_attack_happened_frame <= 0.5;
	}

	public void addLoot(int pLootValue)
	{
		if (pLootValue != 0)
		{
			data.loot += pLootValue;
			data.loot = Mathf.Clamp(data.loot, 0, 99999);
			EffectsLibrary.showMoneyEffect("fx_money_got_loot", current_position, base.current_zone, actor_scale);
		}
	}

	public void addMoney(int pValue)
	{
		if (pValue != 0)
		{
			data.money += pValue;
			data.money = Mathf.Clamp(data.money, 0, 99999);
			EffectsLibrary.showMoneyEffect("fx_money_got_money", current_position, base.current_zone, actor_scale);
		}
	}

	public int giveAllLoot()
	{
		int result = data.loot;
		lootEmpty();
		return result;
	}

	public int giveAllMoney()
	{
		int result = data.money;
		data.money = 0;
		return result;
	}

	public void spendMoney(int pCost)
	{
		if (pCost != 0)
		{
			data.money -= pCost;
			EffectsLibrary.showMoneyEffect("fx_money_spend_money", current_position, base.current_zone, actor_scale);
		}
	}

	public int getMoneyForGift()
	{
		if (money < 10)
		{
			return 0;
		}
		float tRandomMoneyMod = Randy.randomFloat(0.05f, 0.1f);
		int tRandomMoney = Mathf.RoundToInt((float)money * tRandomMoneyMod);
		if (tRandomMoney == 0)
		{
			return 0;
		}
		EffectsLibrary.showMoneyEffect("fx_money_spend_money", current_position, base.current_zone, actor_scale);
		return tRandomMoney;
	}

	public void takeAllOwnLoot()
	{
		addMoney(giveAllLoot());
	}

	public int giveAllLootAndMoney()
	{
		return giveAllLoot() + giveAllMoney();
	}

	public void paidTax(float pTaxRate, string pEffect)
	{
		lootEmpty();
		EffectsLibrary.showMoneyEffect(pEffect, current_position, base.current_zone, actor_scale);
		int tBonusHappinessChange = -5;
		if ((double)pTaxRate > 0.7)
		{
			tBonusHappinessChange = -10;
		}
		changeHappiness("paid_tax", tBonusHappinessChange);
	}

	public void lootEmpty()
	{
		data.loot = 0;
	}

	public void giveInventoryResourcesToCity()
	{
		if (isCarryingResources() && hasCity() && city.isAlive())
		{
			foreach (ResourceContainer tContainer in inventory.getResources().Values)
			{
				city.addResourcesToRandomStockpile(tContainer.id, tContainer.amount);
			}
		}
		inventory.empty();
		setItemSpriteRenderDirty();
	}

	public void generateDefaultSpawnWeapons(bool pUseOwnerless)
	{
		if (pUseOwnerless && canUseItems())
		{
			foreach (Item tItem in World.world.items)
			{
				if (!tItem.isDestroyable() && !tItem.hasCity() && !tItem.hasActor())
				{
					equipment.setItem(tItem, this);
					return;
				}
			}
		}
		string[] default_weapons = asset.default_weapons;
		if (default_weapons != null && default_weapons.Length != 0)
		{
			string tItemId = asset.default_weapons.GetRandom();
			createNewWeapon(tItemId);
		}
	}

	public bool createNewWeapon(string pItemId)
	{
		EquipmentAsset tItemAsset = AssetManager.items.get(pItemId);
		Item tNewItem = World.world.items.generateItem(tItemAsset, null, null, 1, a, 10);
		equipment.weapon.setItem(tNewItem, a);
		return true;
	}

	internal void reloadInventory()
	{
		setStatsDirty();
	}

	public void stealActionFrom(Actor pTarget, float pTargetStunnedTimer = 5f, float pWaitTimerForThief = 1f, bool pAddAggro = true, bool pPossessedSteal = false)
	{
		bool tAnythingStolen = false;
		int tEverything = pTarget.giveAllLootAndMoney();
		if (tEverything > 0)
		{
			tAnythingStolen = true;
		}
		addLoot(tEverything);
		pTarget.cancelAllBeh();
		pTarget.makeStunned(pTargetStunnedTimer);
		makeWait(pWaitTimerForThief);
		addStatusEffect("being_suspicious");
		if (pAddAggro)
		{
			pTarget.addAggro(this);
		}
		punchTargetAnimation(current_position, pFlip: false, pReverse: false, -40f);
		if ((hasSubspeciesMetaTag("steal_items") || hasTag("steal_items") || pPossessedSteal) && tryToStealItems(pTarget, pPossessedSteal))
		{
			tAnythingStolen = true;
		}
		if (tAnythingStolen)
		{
			pTarget.changeHappiness("got_robbed");
		}
	}

	public bool tryToStealItems(Actor pActorTarget, bool pPossessedSteal = false)
	{
		if (!understandsHowToUseItems())
		{
			return false;
		}
		if (!hasMeleeAttack())
		{
			return false;
		}
		float tChance = 0.5f;
		if (pPossessedSteal)
		{
			tChance = 1f;
		}
		if (takeItems(pActorTarget, tChance, 1))
		{
			pActorTarget.makeStunned(1f);
			checkAttackTypes();
			pActorTarget.checkAttackTypes();
			return true;
		}
		return false;
	}

	public bool tryToAcceptGift(Actor pActorTarget)
	{
		if (!understandsHowToUseItems())
		{
			return false;
		}
		if (takeItems(pActorTarget, 0.5f, 1))
		{
			checkAttackTypes();
			pActorTarget.checkAttackTypes();
			return true;
		}
		return false;
	}

	public void takeAllResources(Actor pActorTarget)
	{
		if (!pActorTarget.isCarryingResources())
		{
			return;
		}
		foreach (KeyValuePair<string, ResourceContainer> tContainer in pActorTarget.inventory.getResources())
		{
			inventory.add(tContainer.Value);
		}
		pActorTarget.inventory.empty();
	}

	public bool takeItems(Actor pActorTarget, float pChance = 1f, int pMaxItems = 0)
	{
		if (!understandsHowToUseItems())
		{
			return false;
		}
		if (!pActorTarget.hasEquipment())
		{
			return false;
		}
		using ListPool<ActorEquipmentSlot> tList = new ListPool<ActorEquipmentSlot>(pActorTarget.equipment);
		bool tAnyItemTaken = false;
		if (pMaxItems == 0)
		{
			pMaxItems = tList.Count;
		}
		foreach (ActorEquipmentSlot tTargetSlot in tList.LoopRandom(pMaxItems))
		{
			if (!tTargetSlot.isEmpty())
			{
				ActorEquipmentSlot tOurSlot = equipment.getSlot(tTargetSlot.type);
				Item tOurItem = tOurSlot.getItem();
				Item tTargetItem = tTargetSlot.getItem();
				if (!tTargetItem.isCursed() && (tOurSlot.isEmpty() || (!tOurItem.isCursed() && tTargetItem.getValue() > tOurItem.getValue())))
				{
					tAnyItemTaken = true;
					tTargetSlot.takeAwayItem();
					tOurSlot.setItem(tTargetItem, this);
					setStatsDirty();
					pActorTarget.setStatsDirty();
				}
			}
		}
		return tAnyItemTaken;
	}

	public void addToInventory(string pResourceID, int pAmount)
	{
		inventory = inventory.add(pResourceID, pAmount);
		setItemSpriteRenderDirty();
	}

	public void addToInventory(ResourceContainer pResourceContainer)
	{
		inventory = inventory.add(pResourceContainer);
		setItemSpriteRenderDirty();
	}

	public void takeFromInventory(string pID, int pAmount)
	{
		inventory = inventory.remove(pID, pAmount);
		setItemSpriteRenderDirty();
	}

	public void setSubspecies(Subspecies pObject)
	{
		World.world.subspecies.setDirtyUnits(subspecies);
		subspecies = pObject;
		World.world.subspecies.unitAdded(pObject);
		setStatsDirty();
	}

	public void joinLanguage(Language pLanguage)
	{
		if (language != pLanguage)
		{
			bool tHadLanguage = false;
			if (hasLanguage())
			{
				language.increaseSpeakersLost();
				tHadLanguage = true;
			}
			if (pLanguage != null)
			{
				if (!tHadLanguage)
				{
					pLanguage.countNewSpeaker();
				}
				else
				{
					pLanguage.countConversion();
				}
			}
		}
		setLanguage(pLanguage);
	}

	public void setLanguage(Language pObject)
	{
		World.world.languages.setDirtyUnits(language);
		language = pObject;
		World.world.languages.unitAdded(pObject);
		setStatsDirty();
	}

	public void setPlot(Plot pObject)
	{
		World.world.plots.setDirtyUnits(plot);
		plot = pObject;
		World.world.plots.unitAdded(pObject);
		setStatsDirty();
	}

	public void setReligion(Religion pObject)
	{
		World.world.religions.setDirtyUnits(religion);
		religion = pObject;
		World.world.religions.unitAdded(pObject);
		setStatsDirty();
	}

	public void setFamily(Family pObject)
	{
		World.world.families.setDirtyUnits(family);
		family = pObject;
		World.world.families.unitAdded(pObject);
		setStatsDirty();
	}

	public void setClan(Clan pObject)
	{
		World.world.clans.setDirtyUnits(clan);
		clan = pObject;
		World.world.clans.unitAdded(pObject);
		setStatsDirty();
	}

	public void setCulture(Culture pCulture)
	{
		World.world.cultures.setDirtyUnits(culture);
		culture = pCulture;
		World.world.cultures.unitAdded(pCulture);
		setStatsDirty();
	}

	internal void removeFromArmy()
	{
		if (hasArmy())
		{
			Army obj = army;
			setArmy(null);
			obj.checkCaptainRemoval(this);
		}
	}

	public void setArmy(Army pObject)
	{
		World.world.armies.setDirtyUnits(army);
		army = pObject;
		World.world.armies.unitAdded(pObject);
		setStatsDirty();
	}

	internal void setCity(City pCity)
	{
		if (city != pCity)
		{
			if (city != null)
			{
				city.eventUnitRemoved(a);
			}
			World.world.cities.setDirtyUnits(city);
			city = pCity;
			if (city != null)
			{
				city.eventUnitAdded(a);
				setKingdom(city.kingdom);
			}
			World.world.cities.unitAdded(city);
			setStatsDirty();
		}
	}

	public void setMetasFromCity(City pCity)
	{
		if (pCity.hasCulture() && !hasCulture())
		{
			setCulture(pCity.culture);
		}
		if (pCity.hasLanguage() && !hasLanguage())
		{
			joinLanguage(pCity.language);
		}
		if (pCity.hasReligion() && !hasReligion())
		{
			setReligion(pCity.religion);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasClan()
	{
		return clan != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasSubspecies()
	{
		return subspecies != null;
	}

	public bool hasArmy()
	{
		return army != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasFamily()
	{
		return family != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasLanguage()
	{
		return language != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasPlot()
	{
		return plot != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasReligion()
	{
		return religion != null;
	}

	public bool tryToConvertToReligion(Religion pReligion)
	{
		if (!hasSubspecies() || !subspecies.has_advanced_memory)
		{
			return false;
		}
		if (hasReligion() && religion == pReligion)
		{
			return false;
		}
		if (hasCulture() && !culture.isPossibleToConvertToOtherMeta())
		{
			return false;
		}
		setReligion(pReligion);
		pReligion.countConversion();
		EffectsLibrary.showMetaEventEffectConversion("fx_conversion_religion", this);
		return true;
	}

	public bool tryToConvertToCulture(Culture pCulture)
	{
		if (!hasSubspecies() || !subspecies.has_advanced_memory)
		{
			return false;
		}
		if (hasCulture() && culture == pCulture)
		{
			return false;
		}
		if (hasCulture() && !culture.isPossibleToConvertToOtherMeta())
		{
			return false;
		}
		bool num = hasCulture();
		_ = culture;
		setCulture(pCulture);
		if (num)
		{
			pCulture.countConversion();
		}
		EffectsLibrary.showMetaEventEffectConversion("fx_conversion_culture", this);
		return true;
	}

	public bool tryToConvertToLanguage(Language pLanguage)
	{
		if (!hasSubspecies() || !subspecies.has_advanced_communication)
		{
			return false;
		}
		if (hasLanguage() && language == pLanguage)
		{
			return false;
		}
		if (hasCulture() && !culture.isPossibleToConvertToOtherMeta())
		{
			return false;
		}
		joinLanguage(pLanguage);
		EffectsLibrary.showMetaEventEffectConversion("fx_conversion_language", this);
		return true;
	}

	public void saveOriginFamily(long pID)
	{
		data.ancestor_family = pID;
	}

	private void clearManagers()
	{
		if (hasClan())
		{
			World.world.clans.unitDied(clan);
			clan = null;
		}
		if (hasArmy())
		{
			World.world.armies.unitDied(army);
			army = null;
		}
		if (hasCulture())
		{
			World.world.cultures.unitDied(culture);
			culture = null;
		}
		if (hasFamily())
		{
			World.world.families.unitDied(family);
			family = null;
		}
		if (hasLanguage())
		{
			World.world.languages.unitDied(language);
			language = null;
		}
		if (hasPlot())
		{
			World.world.plots.unitDied(plot);
			plot = null;
		}
		if (hasReligion())
		{
			World.world.religions.unitDied(religion);
			religion = null;
		}
	}

	internal bool isCitizenJob(string pJob)
	{
		if (citizen_job == null)
		{
			return false;
		}
		return citizen_job.id == pJob;
	}

	public void forgetCulture()
	{
		makeConfused();
		if (hasCulture())
		{
			setCulture(null);
		}
	}

	public void forgetReligion()
	{
		makeConfused();
		if (hasReligion())
		{
			setReligion(null);
		}
	}

	public void forgetLanguage()
	{
		makeConfused(10f);
		if (hasLanguage())
		{
			joinLanguage(null);
		}
	}

	public void forgetClan()
	{
		makeConfused();
		if (hasClan())
		{
			clan.tryForgetChief(this);
			setClan(null);
		}
	}

	public void forgetKingdomAndCity()
	{
		makeConfused();
		removeFromPreviousFaction();
		if (isKingdomCiv())
		{
			setDefaultKingdom();
		}
	}

	public void tryToConvertActorToMetaFromActor(Actor pActor, bool pStunOnSuccess = true)
	{
		int tCounterSuccess = 0;
		if (pActor.hasCulture() && Randy.randomBool() && tryToConvertToCulture(pActor.culture))
		{
			tCounterSuccess++;
		}
		if (pActor.hasLanguage() && Randy.randomBool() && tryToConvertToLanguage(pActor.language))
		{
			tCounterSuccess++;
		}
		if (pActor.hasReligion() && Randy.randomBool() && tryToConvertToReligion(pActor.religion))
		{
			tCounterSuccess++;
		}
		if (pStunOnSuccess)
		{
			if (tCounterSuccess > 0)
			{
				makeStunned();
				applyRandomForce();
				addStatusEffect("voices_in_my_head");
			}
			else if (Randy.randomChance(0.1f))
			{
				makeConfused(Randy.randomFloat(0.8f, 5f));
			}
		}
	}

	public void joinCity(City pCity)
	{
		bool tCount = !asset.is_boat;
		if (city != pCity)
		{
			bool tHadCity = hasCity();
			if (tHadCity && tCount)
			{
				city.increaseLeft();
			}
			if (pCity != null)
			{
				if (pCity.kingdom != kingdom)
				{
					joinKingdom(pCity.kingdom);
				}
				if (tCount)
				{
					if (tHadCity)
					{
						pCity.increaseMoved();
					}
					else
					{
						pCity.increaseJoined();
					}
				}
			}
		}
		setCity(pCity);
	}

	public void joinKingdom(Kingdom pKingdom)
	{
		if (!asset.is_boat && kingdom != pKingdom)
		{
			bool tHadKingdom = hasKingdom();
			if (tHadKingdom && kingdom.isCiv())
			{
				kingdom.increaseLeft();
			}
			if (pKingdom != null && pKingdom.isCiv())
			{
				if (tHadKingdom)
				{
					pKingdom.increaseMoved();
				}
				else
				{
					pKingdom.increaseJoined();
				}
			}
		}
		setKingdom(pKingdom);
	}

	internal void setKingdom(Kingdom pKingdomToSet)
	{
		if (kingdom != pKingdomToSet)
		{
			checkKingdom();
			kingdom = pKingdomToSet;
			checkKingdom();
			setStatsDirty();
		}
	}

	private void checkKingdom()
	{
		if (hasKingdom())
		{
			if (kingdom.wild)
			{
				World.world.kingdoms_wild.setDirtyUnits(kingdom);
			}
			else
			{
				World.world.kingdoms.setDirtyUnits(kingdom);
			}
		}
	}

	public void setForcedKingdom(Kingdom pForcedKingdom)
	{
		if (!(kingdom.asset.id == pForcedKingdom.asset.id))
		{
			joinKingdom(pForcedKingdom);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasCulture()
	{
		return culture != null;
	}

	public bool buildCityAndStartCivilization()
	{
		if (!World.world.cities.canStartNewCityCivilizationHere(this))
		{
			return false;
		}
		Kingdom obj = World.world.kingdoms.makeNewCivKingdom(this);
		City tNewCity = World.world.cities.buildFirstCivilizationCity(this);
		createDefaultCultureAndLanguageAndClan(tNewCity.name);
		obj.setUnitMetas(this);
		tNewCity.setUnitMetas(this);
		return true;
	}

	public void createDefaultCultureAndLanguageAndClan(string pCultureName = null)
	{
		if (!hasClan())
		{
			World.world.clans.newClan(this, pAddDefaultTraits: true);
		}
		if (!hasLanguage() && subspecies.has_advanced_communication)
		{
			Language tLanguage = World.world.languages.newLanguage(this, pAddDefaultTraits: true);
			joinLanguage(tLanguage);
			tLanguage.convertSameSpeciesAroundUnit(this);
		}
		if (!hasCulture() && subspecies.has_advanced_memory)
		{
			Culture tCulture = World.world.cultures.newCulture(this, pAddDefaultTraits: true);
			if (pCultureName != null)
			{
				tCulture.setName(pCultureName, pTrack: false);
			}
			setCulture(tCulture);
			tCulture.convertSameSpeciesAroundUnit(this);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void checkDefaultKingdom()
	{
		if (!hasKingdom())
		{
			setDefaultKingdom();
		}
	}

	public void setDefaultKingdom()
	{
		setKingdom(World.world.kingdoms_wild.get(asset.kingdom_id_wild));
	}

	public void removeFromPreviousFaction()
	{
		stopBeingWarrior();
		if (isKing())
		{
			kingdom.kingLeftEvent();
		}
		joinCity(null);
	}

	public bool wantsToSplitMeta()
	{
		if (hasKingdom() && isKingdomCiv() && hasSubspecies() && kingdom.getMainSubspecies() == subspecies)
		{
			return false;
		}
		if (hasTrait("ambitious"))
		{
			return true;
		}
		if (hasStatus("inspired"))
		{
			return true;
		}
		return false;
	}

	public NanoObject getMetaObjectOfType(MetaType pType)
	{
		return pType switch
		{
			MetaType.Alliance => kingdom.getAlliance(), 
			MetaType.Kingdom => kingdom, 
			MetaType.City => city, 
			MetaType.Clan => clan, 
			MetaType.Culture => culture, 
			MetaType.Family => family, 
			MetaType.Army => army, 
			MetaType.Language => language, 
			MetaType.Religion => religion, 
			MetaType.Subspecies => subspecies, 
			_ => null, 
		};
	}

	internal void setFlip(bool pFlip)
	{
		flip = pFlip;
	}

	public void precalcMovementSpeed(bool pForce = false)
	{
		if (!pForce)
		{
			if (!is_moving)
			{
				return;
			}
			if (_precalc_movement_speed_skips > 0)
			{
				_precalc_movement_speed_skips--;
				return;
			}
			_precalc_movement_speed_skips = 5;
		}
		bool tIsInAir = isInAir();
		bool tIsWaterCreature = isWaterCreature();
		float tWalkMultiplier = 1f;
		if (asset.ignore_tile_speed_multiplier || tIsInAir || tIsWaterCreature)
		{
			tWalkMultiplier = 1f;
		}
		else if (current_tile.is_liquid)
		{
			if (getStamina() <= 0 && !hasTag("fast_swimming"))
			{
				tWalkMultiplier *= 0.4f;
			}
		}
		else if (!string.IsNullOrEmpty(current_tile.Type.ignore_walk_multiplier_if_tag) && !stats.hasTag(current_tile.Type.ignore_walk_multiplier_if_tag))
		{
			tWalkMultiplier = current_tile.Type.walk_multiplier;
		}
		if (!asset.ignore_tile_speed_multiplier && _is_in_liquid && hasTag("fast_swimming"))
		{
			tWalkMultiplier *= 5f;
		}
		if (hasTask() && ai.task.speed_multiplier != 1f)
		{
			tWalkMultiplier *= ai.task.speed_multiplier;
		}
		float tSpeed = stats["speed"] * tWalkMultiplier;
		if (!tIsInAir && WorldLawLibrary.world_law_entanglewood.isEnabled())
		{
			Building building = current_tile.building;
			if (building != null && building.asset.flora_type == FloraType.Tree)
			{
				tSpeed *= 0.8f;
			}
		}
		if (tSpeed < 1f)
		{
			tSpeed = 1f;
		}
		if (DebugConfig.isOn(DebugOption.UnitsAlwaysFast))
		{
			tSpeed *= 20f;
		}
		tSpeed *= 0.4f;
		_current_combined_movement_speed = tSpeed * SimGlobals.m.unit_speed_multiplier;
		if (tile_target != null)
		{
			float tDistFinal = Toolbox.DistVec2Float(current_position, tile_target.posV3);
			if (tDistFinal < 1f && _current_combined_movement_speed > 3f)
			{
				float tSlodownFactor = Mathf.Lerp(1f, 0.3f, 1f - tDistFinal);
				_current_combined_movement_speed *= tSlodownFactor;
			}
		}
	}

	internal bool checkFlip()
	{
		return asset.check_flip(this);
	}

	protected void updateMovement(float pElapsed, float pWalkedDistance = 0f)
	{
		float tSqrRemainingDistance = Toolbox.DistVec2Float(current_position, next_step_position);
		if (asset.can_flip && checkFlip())
		{
			if (current_position.x < next_step_position.x)
			{
				setFlip(pFlip: true);
			}
			else
			{
				setFlip(pFlip: false);
			}
		}
		float tDelta = getMovementDelta(pElapsed, pWalkedDistance);
		if (tSqrRemainingDistance < tDelta)
		{
			tDelta = tSqrRemainingDistance;
			current_position = next_step_position;
			if (isUsingPath())
			{
				updatePathMovement();
			}
			else
			{
				stopMovement();
			}
			if (is_moving)
			{
				updateMovement(pElapsed, pWalkedDistance + tDelta);
			}
		}
		else
		{
			current_position = Vector2.MoveTowards(current_position, next_step_position, tDelta);
		}
	}

	private float getMovementDelta(float pElapsed, float pWalkedDistance = 0f)
	{
		float tDelta = _current_combined_movement_speed;
		tDelta *= pElapsed;
		tDelta -= pWalkedDistance;
		if (tDelta < 0f)
		{
			tDelta = 0f;
		}
		return tDelta;
	}

	internal void updateMovementPossessedFlip()
	{
		if (InputHelpers.mouseSupported)
		{
			checkFlipAgainstTargetPosition(World.world.getMousePos());
		}
		else if (ControllableUnit.isMovementActionActive() && !isPossessionAttackJustHappened())
		{
			Vector2 tPos = ControllableUnit.getMovementVector() + current_position;
			checkFlipAgainstTargetPosition(tPos);
		}
	}

	public void checkFlipAgainstTargetPosition(Vector2 pPosition)
	{
		if (asset.can_flip)
		{
			if (current_position.x < pPosition.x)
			{
				setFlip(pFlip: true);
			}
			else
			{
				setFlip(pFlip: false);
			}
		}
	}

	internal float updatePossessedMovementTowards(float pElapsed, Vector2 pMovementPoint)
	{
		precalcMovementSpeed(pForce: true);
		if (asset.can_flip && checkFlip())
		{
			float tMismatchFactor = getMismatchFactorForSideMovement(pMovementPoint);
			if (tMismatchFactor > 0.2f)
			{
				pElapsed *= Mathf.Lerp(1f, 0.8f, tMismatchFactor);
			}
		}
		float tDelta = getMovementDelta(pElapsed);
		Vector2 tNewPos = Vector2.MoveTowards(current_position, pMovementPoint, tDelta);
		tNewPos = checkVelocityAgainstBlock(tNewPos);
		if (!Toolbox.inMapBorder(ref tNewPos))
		{
			return 0f;
		}
		current_position = tNewPos;
		return tDelta;
	}

	public Vector2 getPossessionControlTargetPosition()
	{
		return ControllableUnit.getClickVector();
	}

	public Vector2 getPossessionControlTargetPositionMovementVector()
	{
		if (InputHelpers.mouseSupported)
		{
			return ControllableUnit.getClickVector();
		}
		return ControllableUnit.getMovementVector() + current_position;
	}

	private float getMismatchFactorForSideMovement(Vector2 pMovementPoint)
	{
		Vector2 tCursorPos = World.world.getMousePos();
		bool tSideLook = current_position.x < tCursorPos.x;
		bool tSideMove = current_position.x < pMovementPoint.x;
		bool num = current_position.y < tCursorPos.y;
		bool tUpMove = current_position.y < pMovementPoint.y;
		float tDeltaX = Mathf.Abs(pMovementPoint.x - current_position.x);
		float tDeltaY = Mathf.Abs(pMovementPoint.y - current_position.y);
		float tMismatchFactor = 0f;
		if (tSideLook != tSideMove)
		{
			tMismatchFactor += tDeltaX;
		}
		if (num != tUpMove)
		{
			tMismatchFactor += tDeltaY;
		}
		return Mathf.Clamp01(tMismatchFactor / (tDeltaX + tDeltaY + 0.001f));
	}

	internal void findCurrentTile(bool pCheckNeighbours = true)
	{
		Vector3 tCurrentPosition = current_position;
		if (!dirty_current_tile && tCurrentPosition.x == lastX && tCurrentPosition.y == lastY)
		{
			return;
		}
		dirty_current_tile = false;
		lastX = current_position.x;
		lastY = current_position.y;
		if (current_tile != null && Toolbox.SquaredDist(current_tile.posV3.x, current_tile.posV3.y, tCurrentPosition.x, tCurrentPosition.y) < 0.16000001f)
		{
			return;
		}
		WorldTile tNewTile = Toolbox.getTileAt(tCurrentPosition.x, tCurrentPosition.y);
		setCurrentTile(tNewTile);
		if (Toolbox.SquaredDist(tNewTile.posV3.x, tNewTile.posV3.y, tCurrentPosition.x, tCurrentPosition.y) < 0.09f || !pCheckNeighbours || isFlying())
		{
			return;
		}
		bool tMustAvoidGround = mustAvoidGround();
		if (tNewTile.Type.lava && asset.die_in_lava)
		{
			WorldTile[] tNeighboursAll = tNewTile.neighboursAll;
			foreach (WorldTile tTile in tNeighboursAll)
			{
				if (tTile.Type.ground)
				{
					setCurrentTile(tTile);
					break;
				}
			}
		}
		if (tNewTile.Type.ocean && isDamagedByOcean())
		{
			WorldTile[] tNeighboursAll2 = tNewTile.neighboursAll;
			foreach (WorldTile tTile2 in tNeighboursAll2)
			{
				if (!tTile2.is_liquid)
				{
					setCurrentTile(tTile2);
					break;
				}
			}
		}
		if (tNewTile.Type.block && !isFlying() && !tMustAvoidGround)
		{
			WorldTile[] tNeighboursAll3 = tNewTile.neighboursAll;
			foreach (WorldTile tTile3 in tNeighboursAll3)
			{
				if (tTile3.Type.ground)
				{
					setCurrentTile(tTile3);
					break;
				}
			}
		}
		if (!tNewTile.is_liquid && tMustAvoidGround)
		{
			WorldTile[] tNeighboursAll4 = tNewTile.neighboursAll;
			foreach (WorldTile tTile4 in tNeighboursAll4)
			{
				if (tTile4.is_liquid)
				{
					setCurrentTile(tTile4);
					break;
				}
			}
		}
		if (!tNewTile.isOnFire() || isImmuneToFire())
		{
			return;
		}
		WorldTile[] tNeighboursAll5 = tNewTile.neighboursAll;
		foreach (WorldTile tTile5 in tNeighboursAll5)
		{
			if (!tTile5.isOnFire())
			{
				setCurrentTile(tTile5);
				break;
			}
		}
	}

	internal void checkFindCurrentTile()
	{
		if (dirty_current_tile || (_next_step_tile != null && (float)Toolbox.SquaredDistTile(current_tile, _next_step_tile) > 4f))
		{
			findCurrentTile();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void setTileTarget(WorldTile pTile)
	{
		clearTileTarget();
		tile_target = pTile;
		tile_target.setTargetedBy(a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void clearTileTarget()
	{
		if (tile_target != null)
		{
			if (tile_target.isTargetedBy(this))
			{
				tile_target.cleanTargetedBy();
			}
			tile_target = null;
			scheduled_tile_target = null;
		}
	}

	internal void clearOldPath()
	{
		current_path.Clear();
		current_path_global = null;
		current_path_index = 0;
	}

	public virtual void updatePathMovement()
	{
		if (!isFollowingLocalPath())
		{
			setNotMoving();
			if (split_path != SplitPathStatus.Split)
			{
				split_path = SplitPathStatus.Split;
				timer_action = Randy.randomFloat(0f, asset.path_movement_timeout);
				return;
			}
			split_path = SplitPathStatus.Normal;
			if (tile_target != null)
			{
				goTo(tile_target);
			}
			return;
		}
		WorldTile tMoveTile = current_path[current_path_index];
		TileTypeBase tMoveTileType = tMoveTile.Type;
		current_path_index++;
		if (tMoveTileType.damaged_when_walked)
		{
			current_tile.tryToBreak();
		}
		bool tCheckFailure = true;
		if (_has_status_strange_urge)
		{
			tCheckFailure = false;
		}
		if (tCheckFailure)
		{
			if (asset.is_boat && !tMoveTile.isGoodForBoat())
			{
				callbacks_cancel_path_movement?.Invoke(this);
				cancelAllBeh();
				return;
			}
			if (tMoveTileType.block && !ignoresBlocks())
			{
				if (!hasTask() || !ai.task.move_from_block)
				{
					cancelAllBeh();
					return;
				}
			}
			else
			{
				if (asset.die_in_lava && tMoveTileType.lava)
				{
					cancelAllBeh();
					return;
				}
				if (isDamagedByOcean() && tMoveTileType.ocean && !_is_in_liquid)
				{
					cancelAllBeh();
					return;
				}
			}
		}
		if (tMoveTile.isOnFire() && !isImmuneToFire() && !hasStatus("burning") && !current_tile.isOnFire())
		{
			if (hasTask() && ai.task.is_fireman)
			{
				stopMovement();
				return;
			}
			cancelAllBeh();
			makeWait(0.3f);
		}
		else
		{
			moveTo(tMoveTile);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isFollowingLocalPath()
	{
		if (current_path.Count > 0 && current_path_index < current_path.Count)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool isUsingPath()
	{
		if (isFollowingLocalPath() || current_path_global != null)
		{
			return true;
		}
		return false;
	}

	public ExecuteEvent goTo(WorldTile pTile, bool pPathOnWater = false, bool pWalkOnBlocks = false, bool pWalkOnLava = false, int pLimitPathfindingRegions = 0)
	{
		setTileTarget(pTile);
		return ActorMove.goTo(this, pTile, pPathOnWater, pWalkOnBlocks, pWalkOnLava, pLimitPathfindingRegions);
	}

	public void clearPathForCalibration()
	{
		clearOldPath();
		next_step_position = current_position;
	}

	private void finishStrangeUrgeMovement()
	{
		_has_status_strange_urge = false;
		finishStatusEffect("strange_urge");
		setTask("strange_urge_finish");
	}

	public void stopMovement()
	{
		split_path = SplitPathStatus.Normal;
		_next_step_tile = null;
		clearOldPath();
		clearTileTarget();
		setNotMoving();
		next_step_position = Globals.emptyVector;
		dirty_current_tile = true;
		if (_has_status_strange_urge)
		{
			finishStrangeUrgeMovement();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void setIsMoving()
	{
		if (!is_moving)
		{
			_is_moving = true;
			batch.c_update_movement.Add(a);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void setNotMoving()
	{
		if (is_moving)
		{
			_is_moving = false;
			batch.c_update_movement.Remove(a);
		}
	}

	public void setPossessedMovement(bool pValue)
	{
		_possessed_movement = pValue;
	}

	public void moveTo(WorldTile pTileTarget)
	{
		setIsMoving();
		if (!has_attack_target && current_tile != null && pTileTarget.isOnFire() && !current_tile.isOnFire() && !isImmuneToFire())
		{
			cancelAllBeh();
			return;
		}
		_next_step_tile = pTileTarget;
		if ((float)Toolbox.SquaredDistTile(current_tile, pTileTarget) > 4f)
		{
			dirty_current_tile = true;
		}
		else
		{
			setCurrentTile(_next_step_tile);
		}
		checkStepActionForTile(current_tile);
		Vector3 tEnd = new Vector3(pTileTarget.posV3.x, pTileTarget.posV3.y);
		next_step_position = tEnd;
	}

	public Vector3 updatePos()
	{
		Vector3 tCurPosition = current_position;
		Vector2 tShakePos = shake_offset;
		Vector2 tMoveJumpOffset = move_jump_offset;
		float xx = tCurPosition.x + tMoveJumpOffset.x + tShakePos.x;
		float yy = tCurPosition.y + tMoveJumpOffset.y + tShakePos.y;
		yy += position_height;
		current_shadow_position.Set(tCurPosition.x + tShakePos.x, tCurPosition.y + tShakePos.y);
		float zz = position_height;
		cur_transform_position.Set(xx, yy, zz);
		return cur_transform_position;
	}

	public void stayInBuilding(Building pBuilding)
	{
		is_inside_building = true;
		inside_building = pBuilding;
	}

	internal void exitBuilding()
	{
		if (is_inside_building)
		{
			timer_action = 0f;
			is_inside_building = false;
			inside_building = null;
		}
	}

	internal void embarkInto(Boat pBoat)
	{
		stopMovement();
		data.transportID = pBoat.actor.data.id;
		is_inside_boat = true;
		inside_boat = pBoat;
		inside_boat.addPassenger(this);
		setTask("sit_inside_boat");
		ai.update();
	}

	internal void disembarkTo(Boat pBoat, WorldTile pTile)
	{
		spawnOn(pTile);
		data.transportID = -1L;
		exitBoat();
		setTask("short_move");
	}

	internal void exitBoat()
	{
		inside_boat = null;
		is_inside_boat = false;
		dirty_current_tile = true;
	}

	internal void changeMoveJumpOffset(float pValue)
	{
		move_jump_offset.y += pValue;
		if (move_jump_offset.y < 0f)
		{
			move_jump_offset.y = 0f;
		}
	}

	internal void setCurrentTile(WorldTile pTile)
	{
		current_tile = pTile;
	}

	internal void setCurrentTilePosition(WorldTile pTile)
	{
		setCurrentTile(pTile);
		current_position.Set(pTile.posV3.x, pTile.posV3.y);
	}

	protected void updateWalkJump(float pElapsed)
	{
		if ((!is_visible && move_jump_offset.y == 0f) || position_height > 0f || asset.disable_jump_animation)
		{
			return;
		}
		if (!is_moving)
		{
			if (move_jump_offset.y == 0f && (_jump_time == 0f || isAffectedByLiquid()))
			{
				return;
			}
		}
		else if ((!is_moving && _jump_time == 0f) || isAffectedByLiquid())
		{
			return;
		}
		_jump_time += World.world.elapsed * 6f;
		if (_jump_time >= 1f)
		{
			changeMoveJumpOffset(-3f * pElapsed);
		}
		else
		{
			changeMoveJumpOffset(3f * pElapsed);
		}
		if (_jump_time >= 2f)
		{
			_jump_time = 0f;
			changeMoveJumpOffset(0f);
		}
		if (asset.rotating_animation)
		{
			target_angle.z += (0f - move_jump_offset.y) * 200f * World.world.elapsed;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool inMapBorder()
	{
		return Toolbox.inMapBorder(ref current_position);
	}

	protected virtual void updateVelocity()
	{
		if (under_forces)
		{
			dirty_current_tile = true;
			float tElapsed = World.world.fixed_delta_time;
			float tDirectionSpeed = tElapsed * velocity_speed;
			float tDampMod = stats["mass"] * SimGlobals.m.gravity;
			tDampMod = Mathf.Min(tDampMod, 20f);
			float tVelocity = velocity.z * tElapsed * tDampMod;
			position_height += tVelocity;
			velocity.z -= tElapsed * tDampMod * 0.3f;
			Vector3 tCurrentPosition = current_position;
			Vector2 tNewPos = new Vector2(tCurrentPosition.x + velocity.x * tDirectionSpeed, tCurrentPosition.y + velocity.y * tDirectionSpeed);
			tNewPos = checkVelocityAgainstBlock(tNewPos);
			current_position.Set(tNewPos.x, tNewPos.y);
			if (position_height < 0f)
			{
				position_height = 0f;
				velocity.z = 0f;
			}
			if (position_height <= 0f)
			{
				stopForce();
			}
		}
	}

	private Vector2 checkVelocityAgainstBlock(Vector2 pNewPos)
	{
		WorldTile tTileAtCoords = Toolbox.getTileAt(pNewPos.x, pNewPos.y);
		if (current_tile.Type.block && (!current_tile.Type.mountains || tTileAtCoords.Type.mountains))
		{
			return pNewPos;
		}
		if (tTileAtCoords == current_tile)
		{
			return pNewPos;
		}
		if (asset.is_boat)
		{
			if (tTileAtCoords.Type.liquid)
			{
				return pNewPos;
			}
		}
		else
		{
			if (!tTileAtCoords.Type.block)
			{
				return pNewPos;
			}
			if (getHeight() > tTileAtCoords.Type.block_height)
			{
				return pNewPos;
			}
		}
		Vector2 tWallNormal = getWallNormal(pNewPos, tTileAtCoords.posV3);
		float tElasticity = 0.8f;
		float tDotProduct = velocity.x * tWallNormal.x + velocity.y * tWallNormal.y;
		float tReflectionX = velocity.x - 2f * tDotProduct * tWallNormal.x;
		float tReflectionY = velocity.y - 2f * tDotProduct * tWallNormal.y;
		velocity.x = tReflectionX * tElasticity;
		velocity.y = tReflectionY * tElasticity;
		pNewPos.x = current_position.x;
		pNewPos.y = current_position.y;
		return pNewPos;
	}

	private Vector2 getWallNormal(Vector2 pPosition, Vector2 pBlockPosition)
	{
		Vector2 dirToObject = (pPosition - pBlockPosition).normalized;
		if (Mathf.Abs(dirToObject.x) > Mathf.Abs(dirToObject.y))
		{
			return new Vector2(Mathf.Sign(dirToObject.x), 0f);
		}
		return new Vector2(0f, Mathf.Sign(dirToObject.y));
	}

	public void prepareForSave()
	{
		saveCoordinates();
		saveAssetID();
		saveProfession();
		saveHomeBuilding();
		saveEquipment();
		saveLover();
		saveCity();
		saveKingdomCiv();
		saveCulture();
		saveClan();
		saveSubspecies();
		saveFamily();
		saveArmy();
		saveLanguage();
		savePlot();
		saveReligion();
		saveTraits();
		finishSaving();
	}

	private void saveCoordinates()
	{
		data.x = current_tile.pos.x;
		data.y = current_tile.pos.y;
	}

	private void saveAssetID()
	{
		data.asset_id = asset.id;
	}

	private void saveProfession()
	{
		data.profession = _profession;
	}

	private void saveHomeBuilding()
	{
		if (_home_building != null && _home_building.isUsable() && !_home_building.isAbandoned())
		{
			data.homeBuildingID = _home_building.data.id;
		}
		else
		{
			data.homeBuildingID = -1L;
		}
	}

	private void saveEquipment()
	{
		if (hasEquipment())
		{
			List<long> tItems = equipment.getDataForSave();
			data.saved_items = tItems;
		}
	}

	private void saveLover()
	{
		if (hasLover())
		{
			data.lover = lover.data.id;
		}
		else
		{
			data.lover = -1L;
		}
	}

	private void saveCity()
	{
		if (hasCity() && city.isAlive())
		{
			data.cityID = city.id;
		}
		else
		{
			data.cityID = -1L;
		}
	}

	private void saveKingdomCiv()
	{
		if (isKingdomCiv())
		{
			data.civ_kingdom_id = kingdom.id;
		}
		else
		{
			data.civ_kingdom_id = -1L;
		}
	}

	private void saveCulture()
	{
		if (hasCulture())
		{
			data.culture = culture.id;
		}
		else
		{
			data.culture = -1L;
		}
	}

	private void saveClan()
	{
		if (hasClan())
		{
			data.clan = clan.id;
		}
		else
		{
			data.clan = -1L;
		}
	}

	private void saveSubspecies()
	{
		if (hasSubspecies())
		{
			data.subspecies = subspecies.id;
		}
		else
		{
			data.subspecies = -1L;
		}
	}

	private void saveFamily()
	{
		if (hasFamily())
		{
			data.family = family.id;
		}
		else
		{
			data.family = -1L;
		}
	}

	private void saveArmy()
	{
		if (hasArmy())
		{
			data.army = army.id;
		}
		else
		{
			data.army = -1L;
		}
	}

	private void saveLanguage()
	{
		if (hasLanguage())
		{
			data.language = language.id;
		}
		else
		{
			data.language = -1L;
		}
	}

	private void savePlot()
	{
		if (hasPlot())
		{
			data.plot = plot.id;
		}
		else
		{
			data.plot = -1L;
		}
	}

	private void saveReligion()
	{
		if (hasReligion())
		{
			data.religion = religion.id;
		}
		else
		{
			data.religion = -1L;
		}
	}

	private void saveTraits()
	{
		data.saved_traits = Toolbox.getListForSave(getTraits());
	}

	private void finishSaving()
	{
		data.save();
	}

	public void loadFromSave()
	{
		setStatsDirty();
		TraitTools.loadTraits(this, data.saved_traits);
		foreach (ActorTrait tTrait in traits)
		{
			tTrait.action_on_augmentation_load?.Invoke(this, tTrait);
		}
		if (isSapient() && is_profession_nothing)
		{
			data.profession = UnitProfession.Unit;
		}
		setProfession(data.profession, pCancelBeh: false);
		City tCity = World.world.cities.get(data.cityID);
		Kingdom tKingdom = World.world.kingdoms.get(data.civ_kingdom_id);
		if (tCity != null && !tCity.isNeutral())
		{
			setCity(tCity);
		}
		if (tKingdom != null)
		{
			setKingdom(tKingdom);
		}
		if (hasEquipment())
		{
			foreach (ActorEquipmentSlot tSlot in equipment)
			{
				if (tSlot.isEmpty())
				{
					continue;
				}
				Item tItem = tSlot.getItem();
				int k = 0;
				while (k < tItem.data.modifiers.Count)
				{
					if (AssetManager.items_modifiers.get(tItem.data.modifiers[k]) == null)
					{
						tItem.data.modifiers.RemoveAt(k);
					}
					else
					{
						k++;
					}
				}
			}
		}
		if (data.inventory.isEmpty())
		{
			data.inventory.empty();
		}
		foreach (Actor parent in getParents())
		{
			parent.increaseChildren();
		}
		asset.action_on_load?.Invoke(this);
	}

	private void countDeath(AttackType pType)
	{
		World.world.game_stats.data.creaturesDied++;
		World.world.map_stats.deaths++;
		switch (pType)
		{
		case AttackType.Plague:
			World.world.map_stats.deaths_plague++;
			break;
		case AttackType.Starvation:
			World.world.map_stats.deaths_hunger++;
			break;
		case AttackType.Eaten:
			World.world.map_stats.deaths_eaten++;
			break;
		case AttackType.Age:
			World.world.map_stats.deaths_age++;
			break;
		case AttackType.Poison:
			World.world.map_stats.deaths_poison++;
			break;
		case AttackType.Infection:
			World.world.map_stats.deaths_infection++;
			break;
		case AttackType.Tumor:
			World.world.map_stats.deaths_tumor++;
			break;
		case AttackType.Acid:
			World.world.map_stats.deaths_acid++;
			break;
		case AttackType.Fire:
			World.world.map_stats.deaths_fire++;
			break;
		case AttackType.Divine:
			World.world.map_stats.deaths_divine++;
			break;
		case AttackType.Metamorphosis:
			World.world.map_stats.metamorphosis++;
			break;
		case AttackType.Weapon:
			World.world.map_stats.deaths_weapon++;
			break;
		case AttackType.Gravity:
			World.world.map_stats.deaths_gravity++;
			break;
		case AttackType.Drowning:
			World.world.map_stats.deaths_drowning++;
			break;
		case AttackType.Water:
			World.world.map_stats.deaths_water++;
			break;
		case AttackType.Explosion:
			World.world.map_stats.deaths_explosion++;
			break;
		case AttackType.Smile:
			World.world.map_stats.deaths_smile++;
			break;
		default:
			throw new ArgumentOutOfRangeException($"Unknown attack type: {pType}");
		case AttackType.Other:
		case AttackType.AshFever:
		case AttackType.None:
			break;
		}
		if (hasArmy())
		{
			army.increaseDeaths(pType);
		}
		if (hasCity())
		{
			city.increaseDeaths(pType);
		}
		if (hasClan())
		{
			clan.increaseDeaths(pType);
		}
		if (hasCulture())
		{
			culture.increaseDeaths(pType);
		}
		if (hasFamily())
		{
			family.increaseDeaths(pType);
		}
		if (hasLanguage())
		{
			language.increaseDeaths(pType);
		}
		if (hasReligion())
		{
			religion.increaseDeaths(pType);
		}
		if (hasSubspecies())
		{
			subspecies.increaseDeaths(pType);
		}
		if (isKingdomCiv())
		{
			kingdom.increaseDeaths(pType);
		}
		foreach (Actor parent in getParents())
		{
			parent.decreaseChildren();
		}
	}

	public void increaseEvolutions()
	{
		World.world.map_stats.evolutions++;
		if (hasCity())
		{
			city.increaseEvolutions();
		}
		if (hasClan())
		{
			clan.increaseEvolutions();
		}
		if (hasReligion())
		{
			religion.increaseEvolutions();
		}
		if (hasSubspecies())
		{
			subspecies.increaseEvolutions();
		}
		if (isKingdomCiv())
		{
			kingdom.increaseEvolutions();
		}
	}

	private void increaseKills()
	{
		data.kills++;
		if (hasArmy())
		{
			army.increaseKills();
		}
		if (hasCity())
		{
			city.increaseKills();
		}
		if (hasClan())
		{
			clan.increaseKills();
		}
		if (hasCulture())
		{
			culture.increaseKills();
		}
		if (hasFamily())
		{
			family.increaseKills();
		}
		if (hasLanguage())
		{
			language.increaseKills();
		}
		if (hasReligion())
		{
			religion.increaseKills();
		}
		if (hasSubspecies())
		{
			subspecies.increaseKills();
		}
		if (isKingdomCiv())
		{
			kingdom.increaseKills();
		}
	}

	public void increaseChildren()
	{
		_current_children++;
	}

	public void decreaseChildren()
	{
		_current_children--;
	}

	public void increaseBirths()
	{
		data.births++;
	}

	public void applyForcedKingdomTrait()
	{
		removeFromPreviousFaction();
		removeTrait("peaceful");
		startShake(0.3f, 0.2f);
		startColorEffect();
		cancelAllBeh();
	}

	public string getTraitsAsLocalizedString()
	{
		string tResult = "";
		foreach (ActorTrait tBaseTrait in traits)
		{
			tResult = tResult + tBaseTrait.getTranslatedName() + ", ";
		}
		return tResult;
	}

	public void sortTraits(IReadOnlyCollection<ActorTrait> pTraits)
	{
		if (!traits.SetEquals(pTraits))
		{
			return;
		}
		traits.Clear();
		foreach (ActorTrait tTrait in pTraits)
		{
			traits.Add(tTrait);
		}
	}

	public void traitModifiedEvent()
	{
	}

	public void removeTrait(string pTraitID)
	{
		ActorTrait tTrait = AssetManager.traits.get(pTraitID);
		removeTrait(tTrait);
	}

	public bool removeTrait(ActorTrait pTrait)
	{
		bool num = traits.Remove(pTrait);
		if (num)
		{
			pTrait.action_on_augmentation_remove?.Invoke(this, pTrait);
			setStatsDirty();
			clearTraitCache();
		}
		return num;
	}

	public void removeTraits(ICollection<ActorTrait> pTraits)
	{
		bool tAnyRemoved = false;
		foreach (ActorTrait tTrait in pTraits)
		{
			if (traits.Remove(tTrait))
			{
				tTrait.action_on_augmentation_remove?.Invoke(this, tTrait);
				tAnyRemoved = true;
			}
		}
		if (tAnyRemoved)
		{
			setStatsDirty();
			clearTraitCache();
		}
	}

	public void clearTraitCache()
	{
		_traits_cache.Clear();
	}

	private void removeOppositeTraits(ActorTrait pTrait)
	{
		if (pTrait.hasOppositeTraits())
		{
			removeTraits(pTrait.opposite_traits);
		}
	}

	public bool addTrait(string pTraitID, bool pRemoveOpposites = false)
	{
		ActorTrait tTrait = AssetManager.traits.get(pTraitID);
		if (tTrait == null)
		{
			return false;
		}
		return addTrait(tTrait, pRemoveOpposites);
	}

	public bool addTrait(ActorTrait pTrait, bool pRemoveOpposites = false)
	{
		if (hasTrait(pTrait))
		{
			return false;
		}
		if (pTrait.affects_mind && hasTag("strong_mind"))
		{
			return false;
		}
		if (pTrait.traits_to_remove != null)
		{
			removeTraits(pTrait.traits_to_remove);
		}
		if (pRemoveOpposites)
		{
			removeOppositeTraits(pTrait);
		}
		else if (hasOppositeTrait(pTrait))
		{
			return false;
		}
		traits.Add(pTrait);
		pTrait.action_on_augmentation_add?.Invoke(this, pTrait);
		setStatsDirty();
		clearTraitCache();
		return true;
	}

	internal bool hasOppositeTrait(string pTraitID)
	{
		return TraitTools.hasOppositeTrait(pTraitID, traits);
	}

	internal bool hasOppositeTrait(ActorTrait pTrait)
	{
		return pTrait.hasOppositeTrait(traits);
	}

	public void generateRandomSpawnTraits(ActorAsset pAsset)
	{
		if (pAsset.traits != null)
		{
			for (int i = 0; i < pAsset.traits.Count; i++)
			{
				string tTrait = pAsset.traits[i];
				addTrait(tTrait);
			}
		}
	}

	public void checkTraitMutationOnBirth()
	{
		if (!hasSubspecies())
		{
			return;
		}
		int tCurrentRandomMutationsMax = subspecies.getAmountOfRandomMutationsActorTraits();
		if (tCurrentRandomMutationsMax == 0)
		{
			return;
		}
		for (int i = 0; i < tCurrentRandomMutationsMax; i++)
		{
			ActorTrait tTrait = AssetManager.traits.pot_traits_birth.GetRandom();
			if (asset.traits_ignore == null || !asset.traits_ignore.Contains(tTrait.id))
			{
				addTrait(tTrait);
			}
		}
	}

	public void checkTraitMutationGrowUp()
	{
		if (!hasSubspecies())
		{
			return;
		}
		int tRandomTraits = Randy.randomInt(0, 3);
		for (int i = 0; i < tRandomTraits; i++)
		{
			ActorTrait tTrait = AssetManager.traits.pot_traits_growup.GetRandom();
			if ((asset.traits_ignore == null || !asset.traits_ignore.Contains(tTrait.id)) && (!tTrait.acquire_grow_up_sapient_only || isSapient()))
			{
				addTrait(tTrait);
			}
		}
	}

	public int countTraits()
	{
		return traits.Count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasTrait(string pTraitID)
	{
		if (!_traits_cache.TryGetValue(pTraitID, out var tResult))
		{
			ActorTrait tTrait = AssetManager.traits.get(pTraitID);
			tResult = hasTrait(tTrait);
			_traits_cache[pTraitID] = tResult;
		}
		return tResult;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool hasTrait(ActorTrait pTrait)
	{
		return traits.Contains(pTrait);
	}

	public void updateParallelChecks(float pElapsed)
	{
		_update_done = false;
		_beh_skip = false;
		if (timer_jump_animation > 0f)
		{
			timer_jump_animation -= pElapsed;
		}
		checkFindCurrentTile();
		checkIsInLiquid();
		if (asset.update_z && position_height != 0f)
		{
			updateFall();
		}
		if (attackedBy != null && !attackedBy.isAlive())
		{
			attackedBy = null;
		}
		if (is_inside_boat)
		{
			return;
		}
		updateFlipRotation(pElapsed);
		if (under_forces)
		{
			for (int i = 0; (float)i < Config.time_scale_asset.multiplier; i++)
			{
				updateVelocity();
			}
		}
		if (!World.world.isPaused() && isAlive())
		{
			updateRotations(pElapsed);
			if (attack_timer >= 0f)
			{
				attack_timer -= pElapsed;
			}
			updateWalkJump(World.world.delta_time);
			if (_timeout_targets >= 0f)
			{
				_timeout_targets -= World.world.delta_time;
			}
			if (timer_action >= 0f)
			{
				timer_action -= pElapsed;
			}
			if (isAllowedToLookForEnemies())
			{
				targets_to_ignore_timer.update(pElapsed);
			}
			updateChangeScale(pElapsed);
			if (!is_immovable)
			{
				precalcMovementSpeed();
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void skipUpdates()
	{
		_update_done = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void skipBehaviour()
	{
		_beh_skip = true;
	}

	public void u1_checkInside(float pElapsed)
	{
		if (isInsideSomething() && is_inside_boat)
		{
			setCurrentTilePosition(inside_boat.actor.current_tile);
			skipUpdates();
		}
	}

	public void u2_updateChildren(float pElapsed)
	{
		if (!_update_done)
		{
			updateChildrenList(children_special, pElapsed);
			updateChildrenListSimple(children_pre_behaviour, pElapsed);
		}
	}

	public void u3_spriteAnimation(float pElapsed)
	{
		if (!_update_done && is_visible)
		{
			sprite_animation.update(pElapsed);
		}
	}

	public void u4_deadCheck(float pElapsed)
	{
		if (!_update_done)
		{
			if (!isAlive())
			{
				updateDeadAnimation(pElapsed);
				skipUpdates();
			}
			else if (isInMagnet() || under_forces)
			{
				skipUpdates();
			}
		}
	}

	public void u5_curTileAction()
	{
		if (_update_done || position_height > 0f)
		{
			return;
		}
		WorldTile tTile = current_tile;
		TileTypeBase tType = tTile.Type;
		if (isFlying())
		{
			return;
		}
		if (tType.block && !ignoresBlocks())
		{
			if (asset.move_from_block && !is_moving && (!hasTask() || !ai.task.move_from_block))
			{
				setTask("move_from_block", pClean: true, pCleanJob: true);
			}
			if (asset.die_on_blocks && !isUnderDamageCooldown() && !_shake_active && getHealth() > 1)
			{
				getHit(1f, pFlash: true, AttackType.Gravity);
			}
			if (!isInAir() || isHovering())
			{
				applyRandomForce(1.5f, 3f);
				if (Randy.randomChance(0.02f))
				{
					makeStunned();
				}
			}
			if (tType.mountains || tType.wall)
			{
				checkDieOnGroundBoat();
			}
			return;
		}
		if (tType.ground)
		{
			if (tTile.isOnFire() && !isImmuneToFire())
			{
				ActionLibrary.addBurningEffectOnTarget(null, this);
				if (!isAlive())
				{
					if (!_update_done)
					{
						Debug.LogError("If you ever see me, remove this line");
					}
					skipUpdates();
					return;
				}
			}
			if (isWaterCreature() && !asset.force_land_creature)
			{
				spendStaminaWithCooldown(Randy.randomInt(1, 6));
				if (!isUnderDamageCooldown() && !_shake_active)
				{
					getHit(1f, pFlash: true, AttackType.Other, null, pSkipIfShake: true, pMetallicWeapon: false, pCheckDamageReduction: true);
				}
			}
			checkDieOnGroundBoat();
		}
		else if (tType.liquid)
		{
			if (tType.damaged_when_walked)
			{
				tTile.tryToBreak();
			}
			if (!tType.lava)
			{
				finishStatusEffect("burning");
			}
			if (isDamagedByOcean() && tTile.Type.ocean && !isUnderDamageCooldown() && !_shake_active)
			{
				getHit(getWaterDamage(), pFlash: true, AttackType.Water);
			}
			if (!hasTag("fast_swimming") && !isWaterCreature() && !isInAir())
			{
				spendStaminaWithCooldown(Randy.randomInt(1, 6));
				if (getStamina() <= 0 && !isUnderDamageCooldown())
				{
					addStatusEffect("drowning", 0f, pColorEffect: false);
				}
			}
		}
		if (tType.damage_units && !isUnderDamageCooldown() && (!tType.lava || (asset.die_in_lava && !isImmuneToFire())))
		{
			getHit(tType.damage, pFlash: true, AttackType.Fire);
			if (!hasHealth())
			{
				if (tType.lava)
				{
					CursedSacrifice.checkGoodForSacrifice(this);
				}
				skipUpdates();
			}
		}
		if (tTile.hasBuilding() && tTile.building.asset.has_step_action)
		{
			tTile.building.asset.step_action(this, tTile.building);
			if (!hasHealth())
			{
				skipUpdates();
			}
		}
	}

	public void u6_checkFrozen(float pElapsed)
	{
		if (!_update_done && (is_ai_frozen || is_unconscious))
		{
			skipUpdates();
		}
	}

	public void u8_checkUpdateTimers(float pElapsed)
	{
		if (_update_done)
		{
			return;
		}
		if (timer_action >= 0f)
		{
			skipUpdates();
		}
		else if (!isAlive())
		{
			if (!_update_done)
			{
				Debug.LogError("If you ever see me, remove this line");
			}
			skipUpdates();
		}
	}

	public void u7_checkAugmentationEffects()
	{
		if (_update_done || World.world.getWorldTimeElapsedSince(_timestamp_augmentation_effects) < 1f)
		{
			return;
		}
		List<BaseAugmentationAsset> tTempAugmentationList = _tempAugmentationList;
		Dictionary<BaseAugmentationAsset, double> tDictTimers = _s_special_effect_augmentations_timers;
		double tWorldTime = (_timestamp_augmentation_effects = World.world.getCurWorldTime());
		int i = 0;
		for (int tLen = _s_special_effect_augmentations.Count; i < tLen; i++)
		{
			BaseAugmentationAsset tAugmentation = _s_special_effect_augmentations[i];
			if (tDictTimers.TryGetValue(tAugmentation, out var tLastUseTimestamp))
			{
				if (World.world.getWorldTimeElapsedSince(tLastUseTimestamp) < tAugmentation.special_effect_interval)
				{
					continue;
				}
				tTempAugmentationList.Add(tAugmentation);
			}
			tDictTimers[tAugmentation] = tWorldTime;
		}
		if (tTempAugmentationList.Count == 0)
		{
			return;
		}
		int j = 0;
		for (int tLen2 = tTempAugmentationList.Count; j < tLen2; j++)
		{
			BaseAugmentationAsset tAugmentation2 = tTempAugmentationList[j];
			WorldAction tSpecialEffect = tAugmentation2.action_special_effect;
			if (Bench.bench_enabled)
			{
				double tTimeStart = Time.realtimeSinceStartupAsDouble;
				tSpecialEffect(this, current_tile);
				double tTimeEnd = Time.realtimeSinceStartupAsDouble - tTimeStart;
				Bench.benchSaveSplit(tAugmentation2.id, tTimeEnd, 1, "effects_traits");
			}
			else
			{
				tSpecialEffect(this, current_tile);
			}
		}
		_tempAugmentationList.Clear();
	}

	public void b1_checkUnderForce(float pElapsed)
	{
		if (!_update_done)
		{
			if (under_forces)
			{
				skipBehaviour();
			}
			else if (asset.update_z && position_height != 0f)
			{
				skipBehaviour();
			}
		}
	}

	public void b2_checkCurrentEnemyTarget(float pElapsed)
	{
		if (!_update_done && !_beh_skip && checkCurrentEnemyTarget())
		{
			skipBehaviour();
		}
	}

	public void b3_findEnemyTarget(float pElapsed)
	{
		if (!_update_done && !_beh_skip && checkEnemyTargets())
		{
			stopMovement();
			skipBehaviour();
		}
	}

	public void b4_checkTaskVerifier(float pElapsed)
	{
		if (!_update_done && !_beh_skip)
		{
			if (hasTask() && ai.task.has_verifier && ai.task.task_verifier.execute(this) == BehResult.Stop)
			{
				cancelAllBeh();
				skipBehaviour();
			}
			else if (is_moving)
			{
				skipBehaviour();
			}
		}
	}

	public void b5_checkPathMovement(float pElapsed)
	{
		if (!_update_done && !_beh_skip && isUsingPath())
		{
			updatePathMovement();
			skipBehaviour();
		}
	}

	public void b6_0_updateDecision(float pElapsed)
	{
		if (!_update_done && !_beh_skip && !is_unconscious && !_has_status_possessed && asset.has_ai_system)
		{
			DecisionHelper.makeDecisionFor(this, out _last_decision_id);
		}
	}

	public string getLastDecisionForMindOverview()
	{
		return _last_decision_id;
	}

	public void b6_updateAI(float pElapsed)
	{
		if (!_update_done && !_beh_skip && !is_unconscious && !_has_status_possessed && asset.has_ai_system)
		{
			ai.update();
		}
	}

	public void b55_updateNaturalDeaths(float pElapsed)
	{
		if (!_update_done && !_beh_skip && !is_unconscious && !_has_status_possessed && asset.has_ai_system && ai.action_index == 0 && checkNaturalDeath())
		{
			skipBehaviour();
			skipUpdates();
		}
	}

	public void u10_checkSmoothMovement(float pElapsed)
	{
		if (!_update_done && !is_immovable)
		{
			if (!Config.time_scale_asset.sonic)
			{
				checkCalibrateTargetPosition();
			}
			updateMovement(pElapsed);
		}
	}
}
