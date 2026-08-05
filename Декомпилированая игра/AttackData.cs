using System;
using UnityEngine;

public readonly struct AttackData
{
	public readonly BaseSimObject initiator;

	public readonly Action kill_action;

	public readonly Kingdom kingdom;

	public readonly WorldTile hit_tile;

	public readonly Vector3 hit_position;

	public readonly Vector3 initiator_position;

	public readonly BaseSimObject target;

	public readonly AttackType attack_type;

	public readonly bool skip_shake;

	public readonly bool metallic_weapon;

	public readonly bool critical;

	public readonly int targets;

	public readonly int critical_damage_multiplier;

	public readonly float area_of_effect;

	public readonly int damage;

	public readonly float damage_range;

	public readonly bool is_projectile;

	public readonly string projectile_id;

	public readonly float knockback;

	public AttackData(BaseSimObject pInitiator, WorldTile pHitTile, Vector3 pHitPosition, Vector3 pInitiatorPosition, BaseSimObject pTarget, Kingdom pKingdom, AttackType pAttackType = AttackType.Other, bool pMetallicWeapon = false, bool pSkipShake = true, bool pProjectile = false, string pProjectileID = "", Action pKillAction = null, float pBonusAreOfEffect = 0f)
	{
		bool tCriticalDamage = false;
		float tKnockback = 0f;
		int tTargets = 1;
		float tAreaOfEffect = 0.1f;
		int tDamage = 1;
		float tDamageRange = 1f;
		float tCriticalDamageMultiplier = 1f;
		if (pInitiator != null)
		{
			tCriticalDamage = Randy.randomChance(pInitiator.stats["critical_chance"]);
			tKnockback = pInitiator.stats["knockback"];
			tTargets = (int)pInitiator.stats["targets"];
			tAreaOfEffect = pInitiator.stats["area_of_effect"];
			tDamage = (int)pInitiator.stats["damage"];
			tDamageRange = pInitiator.stats["damage_range"];
			tCriticalDamageMultiplier = pInitiator.stats["critical_damage_multiplier"];
		}
		tAreaOfEffect += pBonusAreOfEffect;
		kill_action = pKillAction;
		initiator = pInitiator;
		kingdom = pKingdom;
		hit_tile = pHitTile;
		initiator_position = pInitiatorPosition;
		hit_position = pHitPosition;
		target = pTarget;
		attack_type = pAttackType;
		metallic_weapon = pMetallicWeapon;
		skip_shake = pSkipShake;
		is_projectile = pProjectile;
		projectile_id = pProjectileID;
		targets = tTargets;
		critical = tCriticalDamage;
		knockback = tKnockback;
		area_of_effect = tAreaOfEffect;
		damage = tDamage;
		damage_range = tDamageRange;
		critical_damage_multiplier = (int)tCriticalDamageMultiplier;
	}
}
