using UnityEngine;

public class BuildingTower : BaseBuildingComponent
{
	protected float _check_targets_timeout = 1f;

	private bool _test_shooting;

	protected int _shooting_amount;

	private bool _shooting_active;

	protected BaseSimObject _shooting_target;

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		if (!building.isUnderConstruction())
		{
			updateTestShooting();
			updateTower(pElapsed);
		}
	}

	protected void updateTestShooting()
	{
		if (_test_shooting && Input.GetMouseButtonDown(2))
		{
			Vector3 tStart = new Vector3(building.current_tile.posV3.x, building.current_tile.posV3.y);
			tStart.y += building.asset.tower_projectile_offset;
			World.world.projectiles.spawn(building, null, building.asset.tower_projectile, tStart, World.world.getMouseTilePos().posV3);
		}
	}

	protected virtual void updateTower(float pElapsed)
	{
		if (_shooting_active)
		{
			shootAtTarget();
		}
		else
		{
			updateCheckTargets(pElapsed);
		}
	}

	protected virtual void updateCheckTargets(float pElapsed)
	{
		if (_check_targets_timeout > 0f)
		{
			_check_targets_timeout -= pElapsed;
		}
		else
		{
			checkTargets();
		}
	}

	protected virtual void resetTimeout()
	{
		_check_targets_timeout = building.asset.tower_projectile_reload + Randy.randomFloat(0f, building.asset.tower_projectile_reload);
	}

	protected virtual void shootAtTarget()
	{
		if (_shooting_target == null || !_shooting_target.isAlive())
		{
			_shooting_active = false;
			return;
		}
		_shooting_amount--;
		if (_shooting_amount <= 0)
		{
			_shooting_active = false;
		}
		Vector3 tStart = new Vector3(building.current_tile.posV3.x, building.current_tile.posV3.y);
		tStart.y += building.asset.tower_projectile_offset;
		Vector3 tTargetPos = _shooting_target.current_tile.posV3;
		tTargetPos.x += Randy.randomFloat(0f - (_shooting_target.stats["size"] + 1f), _shooting_target.stats["size"] + 1f);
		tTargetPos.y += Randy.randomFloat(0f - _shooting_target.stats["size"], _shooting_target.stats["size"]);
		float tZ = 0f;
		if (_shooting_target.isInAir())
		{
			tZ = _shooting_target.getHeight();
		}
		projectileStarted();
		World.world.projectiles.spawn(building, _shooting_target, building.asset.tower_projectile, tStart, tTargetPos, tZ);
	}

	protected virtual void projectileStarted()
	{
	}

	protected virtual void checkTargets()
	{
		resetTimeout();
		_shooting_target = null;
		_shooting_active = false;
		_shooting_amount = 0;
		BaseSimObject tNewTarget = findTarget();
		if (tNewTarget != null)
		{
			_shooting_active = true;
			_shooting_target = tNewTarget;
			_shooting_amount = building.asset.tower_projectile_amount;
		}
	}

	protected virtual BaseSimObject findTarget()
	{
		return building.findEnemyObjectTarget(building.asset.tower_attack_buildings);
	}

	public override void Dispose()
	{
		_shooting_target = null;
		base.Dispose();
	}
}
