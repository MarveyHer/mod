using System;
using ai;
using UnityEngine;

public class CombatActionLibrary : AssetLibrary<CombatActionAsset>
{
	public static CombatActionAsset combat_attack_melee;

	public static CombatActionAsset combat_attack_range;

	public static CombatActionAsset combat_cast_spell;

	public static CombatActionAsset combat_action_deflect;

	public static CombatActionAsset combat_action_dash;

	public static CombatActionAsset combat_action_backstep;

	public override void init()
	{
		base.init();
		combat_attack_melee = add(new CombatActionAsset
		{
			id = "combat_attack_melee",
			play_unit_attack_sounds = true,
			rate = 6,
			action = attackMeleeAction,
			basic = true
		});
		combat_attack_range = add(new CombatActionAsset
		{
			id = "combat_attack_range",
			play_unit_attack_sounds = true,
			rate = 6,
			action = attackRangeAction,
			basic = true
		});
		combat_cast_spell = add(new CombatActionAsset
		{
			id = "combat_cast_spell",
			play_unit_attack_sounds = true,
			cost_stamina = 5,
			is_spell_use = true,
			rate = 3,
			action = tryToCastSpell
		});
		combat_action_deflect = add(new CombatActionAsset
		{
			id = "combat_deflect_projectile",
			cost_stamina = 5,
			chance = 0.2f,
			pools = new CombatActionPool[1],
			action_actor = doDeflect
		});
		add(new CombatActionAsset
		{
			id = "combat_dodge",
			chance = 0.2f,
			cost_stamina = 5,
			action_actor = doDodgeAction,
			pools = AssetLibrary<CombatActionAsset>.a<CombatActionPool>(CombatActionPool.BEFORE_HIT)
		});
		add(new CombatActionAsset
		{
			id = "combat_block",
			chance = 0.2f,
			cost_stamina = 5,
			cooldown = 0.5f,
			action_actor = doBlockAction,
			pools = AssetLibrary<CombatActionAsset>.a<CombatActionPool>(CombatActionPool.BEFORE_HIT_BLOCK)
		});
		add(new CombatActionAsset
		{
			id = "combat_random_jump",
			cost_stamina = 5,
			cooldown = 2f
		});
		combat_action_dash = add(new CombatActionAsset
		{
			id = "combat_dash",
			cost_stamina = 10,
			chance = 0.2f,
			cooldown = 2f,
			action_actor_target_position = doDashAction,
			pools = AssetLibrary<CombatActionAsset>.a<CombatActionPool>(CombatActionPool.BEFORE_ATTACK_MELEE)
		});
		combat_action_backstep = add(new CombatActionAsset
		{
			id = "combat_backstep",
			cost_stamina = 10,
			chance = 0.2f,
			cooldown = 1f,
			can_do_action = delegate(Actor pSelf, BaseSimObject pAttackTarget)
			{
				if (pSelf.current_tile.Type.block)
				{
					return false;
				}
				float num = Toolbox.SquaredDistVec2Float(pSelf.current_position, pAttackTarget.current_position);
				float num2 = pSelf.getAttackRangeSquared() * 0.5f;
				return (num < num2) ? true : false;
			},
			action_actor_target_position = doBackstepAction,
			pools = AssetLibrary<CombatActionAsset>.a<CombatActionPool>(CombatActionPool.BEFORE_ATTACK_RANGE)
		});
		add(new CombatActionAsset
		{
			id = "combat_throw_bomb",
			cost_stamina = 5,
			chance = 0.2f,
			cooldown = 8f,
			action_actor_target_position = doThrowBombAction,
			can_do_action = delegate(Actor pSelf, BaseSimObject pAttackTarget)
			{
				float num = Toolbox.SquaredDistVec2Float(pSelf.current_position, pAttackTarget.current_position);
				return num > 36f && num < 2500f;
			},
			pools = AssetLibrary<CombatActionAsset>.a<CombatActionPool>(CombatActionPool.BEFORE_ATTACK_MELEE, CombatActionPool.BEFORE_ATTACK_RANGE)
		});
		add(new CombatActionAsset
		{
			id = "combat_throw_torch",
			cost_stamina = 30,
			chance = 0.2f,
			cooldown = 8f,
			action_actor_target_position = doThrowTorchAction,
			can_do_action = delegate(Actor pSelf, BaseSimObject pAttackTarget)
			{
				float num = Toolbox.SquaredDistVec2Float(pSelf.current_position, pAttackTarget.current_position);
				return num > 36f && num < 2500f;
			},
			pools = AssetLibrary<CombatActionAsset>.a<CombatActionPool>(CombatActionPool.BEFORE_ATTACK_MELEE, CombatActionPool.BEFORE_ATTACK_RANGE)
		});
	}

	private bool doThrowBombAction(Actor pSelf, Vector2 pTarget, WorldTile pTile = null)
	{
		ActionLibrary.throwBombAtTile(pSelf, pTile);
		pSelf.punchTargetAnimation(pTarget, pFlip: true, pReverse: false, 45f);
		return true;
	}

	private bool doThrowTorchAction(Actor pSelf, Vector2 pTarget, WorldTile pTile = null)
	{
		ActionLibrary.throwTorchAtTile(pSelf, pTile);
		pSelf.punchTargetAnimation(pTarget, pFlip: true, pReverse: false, 45f);
		return true;
	}

	private bool doBackstepAction(Actor pActor, Vector2 pTarget, WorldTile pTile = null)
	{
		float tDodgePowerSide = 5f;
		float tDodgePowerHeight = 1.2f;
		Vector2 tStart = pActor.current_position;
		pActor.punchTargetAnimation(pTarget, pFlip: false, pReverse: false, -20f);
		pActor.calculateForce(tStart.x, tStart.y, pTarget.x, pTarget.y, tDodgePowerSide, tDodgePowerHeight);
		Vector2 tEffectPos = pActor.current_position;
		tEffectPos.y += pActor.getHeight();
		BaseEffect tEffect = EffectsLibrary.spawnAt("fx_dodge", tEffectPos, pActor.actor_scale);
		if (tEffect != null)
		{
			tEffect.transform.rotation = Toolbox.getEulerAngle(tStart, pTarget);
		}
		return true;
	}

	private bool doDashAction(Actor pActor, Vector2 pTarget, WorldTile pTile = null)
	{
		float tDodgePowerSide = 5f;
		float tDodgePowerHeight = 1.2f;
		Vector2 tStart = pActor.current_position;
		pActor.punchTargetAnimation(pTarget, pFlip: true, pReverse: false, 50f);
		pActor.addStatusEffect("dash", 0f, pColorEffect: false);
		pActor.calculateForce(pTarget.x, pTarget.y, tStart.x, tStart.y, tDodgePowerSide, tDodgePowerHeight);
		Vector2 tEffectPos = pActor.current_position;
		tEffectPos.y += pActor.getHeight();
		BaseEffect tEffect = EffectsLibrary.spawnAt("fx_dodge", tEffectPos, pActor.actor_scale);
		if (tEffect != null)
		{
			tEffect.transform.rotation = Toolbox.getEulerAngle(tStart, pTarget);
		}
		return true;
	}

	private bool doBlockAction(Actor pActor, AttackData pData, float pTargetX = 0f, float pTargetY = 0f)
	{
		ActorTool.applyForceToUnit(pData, pActor, 0.1f);
		if (!pActor.is_visible)
		{
			return true;
		}
		Vector2 tCurPosition = pActor.current_position;
		Vector2 tHitPosition = pData.hit_position;
		pActor.punchTargetAnimation(tHitPosition, pFlip: false, pReverse: false, -40f);
		BaseEffect tEffect = EffectsLibrary.spawnAt("fx_block", tHitPosition, pActor.a.actor_scale);
		if (tEffect == null)
		{
			return true;
		}
		tEffect.transform.rotation = Toolbox.getEulerAngle(tCurPosition.x, tCurPosition.y, tHitPosition.x, tHitPosition.y);
		return true;
	}

	private bool doDeflect(Actor pActor, AttackData pData, float pTargetX = 0f, float pTargetY = 0f)
	{
		Vector2 tOldStartPos = pData.initiator_position;
		pActor.spawnSlashPunch(tOldStartPos);
		pActor.stopMovement();
		pActor.punchTargetAnimation(tOldStartPos, pFlip: true, pActor.hasRangeAttack());
		pActor.startAttackCooldown();
		return true;
	}

	private bool doDodgeAction(Actor pActor, AttackData pData, float pTargetX = 0f, float pTargetY = 0f)
	{
		float tDodgePowerSide = 3f;
		float tDodgePowerHeight = 1.5f;
		Vector2 tStart = pActor.cur_transform_position;
		Vector2 tFrom = pData.initiator_position;
		Vector2 tAB = tStart - tFrom;
		Vector2 tTarget = ((!Randy.randomBool()) ? (tStart + Toolbox.rotateVector(tAB, -90f) * tDodgePowerSide) : (tStart + Toolbox.rotateVector(tAB, 90f) * tDodgePowerSide));
		pActor.calculateForce(tStart.x, tStart.y, tTarget.x, tTarget.y, tDodgePowerSide, tDodgePowerHeight);
		pActor.addStatusEffect("dodge", 0f, pColorEffect: false);
		pActor.punchTargetAnimation(tStart, pFlip: false, pReverse: false, -60f);
		Vector2 tEffectPos = pActor.current_position;
		tEffectPos.y += pActor.getHeight();
		BaseEffect tEffect = EffectsLibrary.spawnAt("fx_dodge", tEffectPos, pActor.actor_scale);
		if (tEffect != null)
		{
			tEffect.transform.rotation = Toolbox.getEulerAngle(tStart, tTarget);
		}
		return true;
	}

	public bool attackRangeAction(AttackData pData)
	{
		Actor tSelf = pData.initiator.a;
		BaseSimObject tAttackTarget = pData.target;
		string tProjectileID = pData.projectile_id;
		_ = tSelf.actor_scale;
		float tScaleMod = tSelf.getScaleMod();
		float tSizeThis = tSelf.stats["size"];
		int tProjectiles = (int)tSelf.stats["projectiles"];
		Vector2 tAttackPosition;
		if (tAttackTarget == null)
		{
			tAttackPosition = pData.hit_position;
		}
		else
		{
			tAttackPosition = getAttackTargetPosition(pData);
			tAttackPosition.y += 0.2f * tScaleMod;
		}
		float tAccuracy = tSelf.stats["accuracy"];
		float tDistanceAccuracyMod = Toolbox.DistVec2Float(tSelf.current_position, tAttackPosition) / tAccuracy * 0.25f;
		tDistanceAccuracyMod = Randy.randomFloat(0f, tDistanceAccuracyMod);
		tDistanceAccuracyMod = Mathf.Clamp(tDistanceAccuracyMod, 0f, 2f);
		float tStartHeight = 0.6f * tScaleMod;
		float tTargetHeight = 0f;
		float tAngle = 0f;
		for (int i = 0; i < tProjectiles; i++)
		{
			Vector2 tProjectileAttackVector = new Vector2(tAttackPosition.x, tAttackPosition.y);
			if (tAccuracy < 10f)
			{
				Vector2 tInnacuracyVector = getInnacuracyVector(tAccuracy);
				tInnacuracyVector *= tDistanceAccuracyMod;
				tProjectileAttackVector += tInnacuracyVector;
			}
			Vector3 tStartProjectile = Toolbox.getNewPoint(tSelf.current_position.x, tSelf.current_position.y, tProjectileAttackVector.x, tProjectileAttackVector.y, tSizeThis * tScaleMod);
			tStartProjectile.y += tSelf.getHeight();
			if (tAttackTarget != null && tAttackTarget.isInAir())
			{
				tTargetHeight = tAttackTarget.getHeight();
			}
			tAngle = World.world.projectiles.spawn(tSelf, tAttackTarget, tProjectileID, tStartProjectile, tProjectileAttackVector, tTargetHeight, tStartHeight, pData.kill_action, pData.kingdom).getLaunchAngle();
		}
		tSelf.spawnSlash(tAttackPosition, null, 2f, tTargetHeight, 0f, tAngle);
		return true;
	}

	public Vector2 getInnacuracyVector(float pAccuracyStat)
	{
		float tInaccuracy = 1f * (10f - pAccuracyStat) / 10f;
		float tInaccuracyAngle = (float)((double)(Randy.random() * 2f) * Math.PI);
		return new Vector2(tInaccuracy * (float)Math.Cos(tInaccuracyAngle), tInaccuracy * (float)Math.Sin(tInaccuracyAngle));
	}

	public static bool tryToCastSpell(AttackData pData)
	{
		Actor tHimself = pData.initiator.a;
		BaseSimObject tTarget = pData.target;
		SpellAsset tSpellAsset = tHimself.getRandomSpell();
		if (!tHimself.hasEnoughMana(tSpellAsset.cost_mana))
		{
			return false;
		}
		if (!Randy.randomChance(tSpellAsset.chance + tSpellAsset.chance * tHimself.stats["skill_spell"]))
		{
			return false;
		}
		if (tSpellAsset.cast_target == CastTarget.Himself)
		{
			tTarget = tHimself;
		}
		if (tSpellAsset.cast_entity == CastEntity.BuildingsOnly)
		{
			if (tTarget.isActor())
			{
				return false;
			}
		}
		else if (tSpellAsset.cast_entity == CastEntity.UnitsOnly && tTarget.isBuilding())
		{
			return false;
		}
		if (tSpellAsset.health_ratio > 0f)
		{
			float tCurrentHealth = tHimself.getHealthRatio();
			if (tSpellAsset.health_ratio <= tCurrentHealth)
			{
				return false;
			}
		}
		if (tSpellAsset.min_distance > 0f && (float)Toolbox.SquaredDistTile(tHimself.current_tile, tTarget.current_tile) < tSpellAsset.min_distance * tSpellAsset.min_distance)
		{
			return false;
		}
		bool tWasCast = false;
		if (tSpellAsset.action != null)
		{
			tWasCast = tSpellAsset.action.RunAnyTrue(tHimself, tTarget, tTarget.current_tile);
		}
		if (tWasCast)
		{
			tHimself.doCastAnimation();
			tHimself.addStatusEffect("recovery_spell");
		}
		return tWasCast;
	}

	public bool attackMeleeAction(AttackData pData)
	{
		AttackDataResult attackDataResult = MapBox.newAttack(pData);
		if (pData.initiator.a.is_visible && EffectsLibrary.canShowSlashEffect())
		{
			showMeleeSlashAttack(pData);
		}
		pData.kill_action?.Invoke();
		return attackDataResult.state == ApplyAttackState.Hit;
	}

	public void showMeleeSlashAttack(AttackData pData)
	{
		pData.initiator.a.spawnSlash(pData.hit_position);
	}

	public Vector2 getAttackTargetPosition(AttackData pData)
	{
		BaseSimObject tTarget = pData.target;
		Vector2 tResultVector = new Vector2(pData.hit_position.x, pData.hit_position.y);
		if (tTarget == null)
		{
			return tResultVector;
		}
		float tTargetSize = tTarget.stats["size"];
		if (tTarget.isActor() && tTarget.a.is_moving && tTarget.isFlying())
		{
			return Vector2.MoveTowards(tResultVector, tTarget.a.next_step_position, tTargetSize * 3f);
		}
		return tResultVector;
	}
}
