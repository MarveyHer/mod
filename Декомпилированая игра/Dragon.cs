using System.Collections.Generic;

public class Dragon : BaseActorComponent
{
	private DragonAsset dragonAsset;

	private DragonState state;

	internal float idle_time = -1f;

	internal float sleep_time = -1f;

	internal SpriteAnimation spriteAnimation;

	internal HashSet<Actor> aggroTargets = new HashSet<Actor>();

	internal WorldTile lastLanded;

	private HashSet<WorldTile> _landAttackTiles = new HashSet<WorldTile>();

	private WorldTile _landAttackPosCheck;

	internal int _landAttackCache;

	internal HashSet<WorldTile> _slideAttackTilesFlip = new HashSet<WorldTile>();

	internal HashSet<WorldTile> _slideAttackTilesNoFlip = new HashSet<WorldTile>();

	private WorldTile _slideAttackPosCheckFlip;

	private WorldTile _slideAttackPosCheckNoFlip;

	internal int _slideAttackTilesFlipCache;

	internal int _slideAttackTilesNoFlipCache;

	internal override void create(Actor pActor)
	{
		base.create(pActor);
		spriteAnimation = GetComponent<SpriteAnimation>();
		if (actor.asset.id == "zombie_dragon")
		{
			dragonAsset = PrefabLibrary.instance.zombieDragonAsset;
		}
		else
		{
			dragonAsset = PrefabLibrary.instance.dragonAsset;
		}
		actor.setFlying(pVal: true);
		setFrames(DragonState.Fly, pForce: true);
	}

	private void playSound(DragonState pState)
	{
		switch (state)
		{
		case DragonState.LandAttack:
			MusicBox.playSound("event:/SFX/UNITS/dragon/fire_breath", base.transform.localPosition.x, base.transform.localPosition.y);
			break;
		case DragonState.Slide:
			MusicBox.playSound("event:/SFX/UNITS/dragon/swoop", base.transform.localPosition.x, base.transform.localPosition.y);
			break;
		}
	}

	internal static bool shouldFly(Actor pActor, WorldTile pTile = null)
	{
		if (pTile == null)
		{
			pTile = pActor.current_tile;
		}
		return !canLand(pActor, pTile);
	}

	internal static bool canLand(Actor pActor, WorldTile pTile = null)
	{
		if (pTile == null)
		{
			pTile = pActor.current_tile;
		}
		if (!pTile.Type.ground)
		{
			if (pTile.Type.lava)
			{
				return !pActor.asset.die_in_lava;
			}
			return false;
		}
		return true;
	}

	internal void attackTile(WorldTile pTile)
	{
		if (pTile == null)
		{
			return;
		}
		bool tIsZombie = actor.hasTrait("zombie");
		if (tIsZombie)
		{
			DropsLibrary.action_acid(pTile);
			if (pTile.hasUnits() || Randy.randomBool())
			{
				World.world.drop_manager.spawnParabolicDrop(pTile, "acid", 0f, 0.1f, 3.5f, 0.5f, 4f, Randy.randomFloat(0.025f, 0.2f));
			}
		}
		else
		{
			pTile.startFire(pForce: true);
			if (pTile.hasBuilding())
			{
				pTile.building.getHit(10f);
			}
			if (pTile.hasUnits() || Randy.randomBool())
			{
				World.world.drop_manager.spawnParabolicDrop(pTile, "fire", 0f, 0.1f, 3.5f, 0.5f, 4f, Randy.randomFloat(0.025f, 0.2f));
			}
		}
		if (pTile.hasUnits())
		{
			MapAction.damageWorld(pTile, 2, AssetManager.terraform.get(tIsZombie ? "zombie_dragon_attack" : "dragon_attack"), actor);
		}
	}

	internal bool hasTargetsForSlide()
	{
		if (WorldLawLibrary.world_law_peaceful_monsters.isEnabled())
		{
			return false;
		}
		attackRange(actor.flip);
		foreach (WorldTile item in actor.flip ? _slideAttackTilesFlip : _slideAttackTilesNoFlip)
		{
			if (hasTarget(item, actor))
			{
				return true;
			}
		}
		return false;
	}

	internal bool targetWithinSlide(WorldTile pTargetTile)
	{
		if (WorldLawLibrary.world_law_peaceful_monsters.isEnabled())
		{
			return false;
		}
		attackRange(flip: true);
		if (_slideAttackTilesFlip.Contains(pTargetTile))
		{
			actor.setFlip(pFlip: true);
			return true;
		}
		attackRange(flip: false);
		if (_slideAttackTilesNoFlip.Contains(pTargetTile))
		{
			actor.setFlip(pFlip: false);
			return true;
		}
		return false;
	}

	internal static Kingdom getIgnoredKingdom(Actor pActor)
	{
		if (pActor.hasTrait("zombie"))
		{
			return World.world.kingdoms_wild.get("undead");
		}
		return World.world.kingdoms_wild.get("dragons");
	}

	internal bool targetsWithinLandAttackRange()
	{
		foreach (Actor tAttackTarget in aggroTargets)
		{
			if (!tAttackTarget.isRekt() && landAttackRange(tAttackTarget.current_tile))
			{
				return true;
			}
		}
		return false;
	}

	internal bool landAttackRange(WorldTile pTargetTile)
	{
		if (Toolbox.Dist(actor.current_tile.pos.x, actor.current_tile.pos.y, pTargetTile.pos.x, pTargetTile.pos.y) > 9f)
		{
			return false;
		}
		landAttackTiles(actor.current_tile);
		return _landAttackTiles.Contains(pTargetTile);
	}

	internal HashSet<WorldTile> landAttackTiles(WorldTile pTile)
	{
		if (_landAttackPosCheck == pTile)
		{
			_landAttackCache++;
			return _landAttackTiles;
		}
		_landAttackCache = 0;
		_landAttackTiles.Clear();
		_landAttackPosCheck = pTile;
		for (int yy = 0; yy < 12; yy++)
		{
			for (int xx = 0; xx < 20; xx++)
			{
				WorldTile tTile = World.world.GetTile(pTile.pos.x + xx - 10, pTile.pos.y - yy + 1);
				if (tTile != null && !(Toolbox.Dist(pTile.pos.x, pTile.pos.y, tTile.pos.x, tTile.pos.y) > 9f))
				{
					_landAttackTiles.Add(tTile);
				}
			}
		}
		return _landAttackTiles;
	}

	internal WorldTile randomTileWithinLandAttackRange(WorldTile pTile)
	{
		Toolbox.temp_list_tiles.Clear();
		for (int yy = 9; yy > 1; yy--)
		{
			WorldTile tTile = World.world.GetTile(pTile.pos.x, pTile.pos.y + yy);
			if (tTile != null)
			{
				pTile = tTile;
				break;
			}
		}
		for (int i = 0; i < 12; i++)
		{
			for (int xx = 0; xx < 20; xx++)
			{
				WorldTile tTile2 = World.world.GetTile(pTile.pos.x + xx - 10, pTile.pos.y - i + 1);
				if (tTile2 != null && !(Toolbox.Dist(pTile.pos.x, pTile.pos.y, tTile2.pos.x, tTile2.pos.y) > 9f) && canLand(actor, tTile2))
				{
					Toolbox.temp_list_tiles.Add(tTile2);
				}
			}
		}
		if (Toolbox.temp_list_tiles.Count == 0)
		{
			return pTile;
		}
		return Toolbox.temp_list_tiles.GetRandom();
	}

	internal HashSet<WorldTile> attackRange(bool flip)
	{
		if (flip)
		{
			if (_slideAttackPosCheckFlip == actor.current_tile)
			{
				_slideAttackTilesFlipCache++;
				return _slideAttackTilesFlip;
			}
			_slideAttackTilesFlipCache = 0;
			_slideAttackTilesFlip.Clear();
			_slideAttackPosCheckFlip = actor.current_tile;
		}
		else
		{
			if (_slideAttackPosCheckNoFlip == actor.current_tile)
			{
				_slideAttackTilesNoFlipCache++;
				return _slideAttackTilesNoFlip;
			}
			_slideAttackTilesNoFlipCache = 0;
			_slideAttackTilesNoFlip.Clear();
			_slideAttackPosCheckNoFlip = actor.current_tile;
		}
		int tXOffset = 0;
		tXOffset = ((!flip) ? 20 : (-25));
		for (int yy = 0; yy < 4; yy++)
		{
			for (int xx = 0; xx < 35; xx++)
			{
				WorldTile tTile = World.world.GetTile(actor.current_tile.x + xx - 15 + tXOffset, actor.current_tile.y - yy + 2);
				if (tTile != null)
				{
					if (flip)
					{
						_slideAttackTilesFlip.Add(tTile);
					}
					if (!flip)
					{
						_slideAttackTilesNoFlip.Add(tTile);
					}
				}
			}
		}
		if (flip)
		{
			return _slideAttackTilesFlip;
		}
		return _slideAttackTilesNoFlip;
	}

	private static bool hasTarget(WorldTile tTile, Actor pActor)
	{
		if (tTile.hasBuilding() && tTile.building.isUsable())
		{
			return true;
		}
		if (!tTile.hasUnits())
		{
			return false;
		}
		Kingdom tIgnoredKingdom = getIgnoredKingdom(pActor);
		bool tTargetFound = false;
		tTile.doUnits(delegate(Actor actor)
		{
			if (actor.position_height > 0f)
			{
				return true;
			}
			if (actor.kingdom == tIgnoredKingdom)
			{
				return true;
			}
			tTargetFound = true;
			return false;
		});
		return tTargetFound;
	}

	public void setFrames(DragonState pDragonState, bool pForce = false)
	{
		if (state != pDragonState || pForce)
		{
			actor.setShowShadow(pDragonState == DragonState.Fly);
			state = pDragonState;
			playSound(state);
			DragonAssetContainer tContainer = dragonAsset.getAsset(pDragonState);
			spriteAnimation.setFrames(tContainer.frames);
			spriteAnimation.timeBetweenFrames = tContainer.speed;
			spriteAnimation.resetAnim();
			spriteAnimation.looped = true;
		}
	}

	internal static bool clickToWakeup(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget.a.isTask("dragon_sleep"))
		{
			pTarget.a.cancelAllBeh();
			pTarget.a.setTask("dragon_wakeup");
			return true;
		}
		return false;
	}

	internal static bool canFlip(BaseSimObject pTarget = null, WorldTile pTile = null)
	{
		switch (pTarget.a.getActorComponent<Dragon>().state)
		{
		case DragonState.Fly:
		case DragonState.Idle:
			return true;
		case DragonState.LandAttack:
		case DragonState.Death:
		case DragonState.SleepStart:
		case DragonState.SleepLoop:
		case DragonState.SleepUp:
		case DragonState.Landing:
		case DragonState.Slide:
		case DragonState.Up:
			return false;
		default:
			return true;
		}
	}

	internal static bool getHit(BaseSimObject pSelf, BaseSimObject pAttackedBy = null, WorldTile pTile = null)
	{
		Actor tActor = pSelf.a;
		Dragon tDragon = tActor.getActorComponent<Dragon>();
		if (WorldLawLibrary.world_law_peaceful_monsters.isEnabled())
		{
			return true;
		}
		bool tNewAttacker = false;
		tDragon.aggroTargets.RemoveWhere((Actor tAttacker) => tAttacker.isRekt());
		if (pAttackedBy != null)
		{
			if (pAttackedBy.isActor() && tDragon.aggroTargets.Add(pAttackedBy.a))
			{
				tNewAttacker = tDragon.aggroTargets.Count == 1;
			}
			if (pAttackedBy.hasCity())
			{
				tActor.data.set("cityToAttack", pAttackedBy.getCity().data.id);
				tActor.data.set("attacksForCity", Randy.randomInt(4, 12));
			}
		}
		switch (tActor.ai.task?.id)
		{
		case "dragon_sleep":
			tActor.data.set("justGotHit", pData: true);
			tActor.cancelAllBeh();
			tActor.setTask("dragon_wakeup");
			break;
		case "dragon_idle":
		{
			tActor.data.get("landAttacks", out var landAttacks, 0);
			if (landAttacks > 2 || shouldFly(tActor) || pAttackedBy == null)
			{
				tActor.data.set("justGotHit", pData: true);
				tActor.cancelAllBeh();
				tActor.setTask("dragon_up");
			}
			else if (!pAttackedBy.isFlying() && tDragon.landAttackRange(pAttackedBy.current_tile) && canLand(tActor))
			{
				tActor.cancelAllBeh();
				tActor.setTask("dragon_land_attack");
			}
			break;
		}
		case "dragon_fly":
			if (tNewAttacker)
			{
				tActor.cancelAllBeh();
				if (!pAttackedBy.isFlying() && tDragon.landAttackRange(pAttackedBy.current_tile) && canLand(tActor) && tDragon.lastLanded != tActor.current_tile)
				{
					tActor.setTask("dragon_land");
				}
				else if (tDragon.targetWithinSlide(pAttackedBy.current_tile))
				{
					tActor.setTask("dragon_slide");
				}
				else
				{
					tActor.setTask("dragon_fly");
				}
			}
			break;
		case "dragon_wakeup":
		case "dragon_up":
			tActor.data.set("justGotHit", pData: true);
			break;
		}
		return true;
	}

	internal static bool dragonFall(BaseSimObject pTarget, WorldTile pTile, float pElapsed)
	{
		Dragon tDragon = pTarget.a.getActorComponent<Dragon>();
		SpriteAnimation tSpriteAnimation = tDragon.spriteAnimation;
		tSpriteAnimation.looped = false;
		tSpriteAnimation.ignorePause = true;
		if (pTarget.isFlying())
		{
			tDragon.setFrames(DragonState.Landing);
			if (tSpriteAnimation.currentFrameIndex < tSpriteAnimation.frames.Length - 1)
			{
				return true;
			}
			pTarget.a.setFlying(pVal: false);
			return true;
		}
		tDragon.setFrames(DragonState.Death);
		if (tSpriteAnimation.currentFrameIndex == tSpriteAnimation.frames.Length - 1)
		{
			pTarget.a.updateDeadBlackAnimation(World.world.elapsed);
		}
		return true;
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		if (!actor.isRekt() && !World.world.isPaused())
		{
			checkLiquid();
		}
	}

	internal void checkLiquid()
	{
		if (actor.isFlying() || actor.is_moving || actor.isEgg() || !actor.current_tile.Type.liquid)
		{
			return;
		}
		if (actor.hasTask())
		{
			if (actor.isTask("dragon_up") || actor.isTask("dragon_wakeup"))
			{
				return;
			}
			if (actor.isTask("dragon_sleep"))
			{
				actor.cancelAllBeh();
				actor.setTask("dragon_wakeup");
				return;
			}
		}
		actor.cancelAllBeh();
		actor.setTask("dragon_up");
	}

	public HashSet<WorldTile> getLandAttackTiles()
	{
		return _landAttackTiles;
	}
}
