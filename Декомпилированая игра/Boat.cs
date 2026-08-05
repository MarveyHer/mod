using System;
using System.Collections.Generic;
using life.taxi;
using UnityEngine;

public class Boat : ActorSimpleComponent
{
	private readonly HashSet<Actor> _passengers = new HashSet<Actor>();

	internal TaxiRequest taxi_request;

	internal int passengerWaitCounter;

	public WorldTile taxi_target;

	public bool pickup_near_dock;

	internal int last_movement_angle;

	private Vector2 _last_step = Vector2.zero;

	private string _boat_texture_id => actor.asset.boat_texture_id;

	internal override void create(Actor pActor)
	{
		base.create(pActor);
		Actor obj = actor;
		obj.callbacks_on_death = (BaseActionActor)Delegate.Combine(obj.callbacks_on_death, new BaseActionActor(deathAction));
		Actor obj2 = actor;
		obj2.callbacks_on_death = (BaseActionActor)Delegate.Combine(obj2.callbacks_on_death, new BaseActionActor(spawnBoatExplosion));
		Actor obj3 = actor;
		obj3.callbacks_landed = (BaseActionActor)Delegate.Combine(obj3.callbacks_landed, new BaseActionActor(cancelWork));
		Actor obj4 = actor;
		obj4.callbacks_cancel_path_movement = (BaseActionActor)Delegate.Combine(obj4.callbacks_cancel_path_movement, new BaseActionActor(cancelPathfinderMovement));
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		if (actor.is_moving)
		{
			calculateMovementAngle();
		}
	}

	public void calculateMovementAngle()
	{
		Vector2 tCurPos = actor.current_position;
		Vector2 tNextPos = actor.next_step_position;
		if (!(_last_step == tNextPos))
		{
			_last_step = tNextPos;
			float tAngle = Toolbox.getAngleDegrees(tCurPos.x, tCurPos.y, tNextPos.x, tNextPos.y);
			last_movement_angle = (int)tAngle;
		}
	}

	public bool isNearDock()
	{
		Building tBuilding = actor.getHomeBuilding();
		if (tBuilding == null)
		{
			return false;
		}
		if (tBuilding.component_docks.tiles_ocean.Contains(actor.current_tile))
		{
			return true;
		}
		return false;
	}

	private void cancelPathfinderMovement(Actor pActor)
	{
		cancelWork(pActor);
	}

	internal void cancelWork(Actor pActor)
	{
		actor.cancelAllBeh();
		if (taxi_request != null)
		{
			TaxiManager.cancelRequest(taxi_request);
			taxi_request = null;
			taxi_target = null;
		}
	}

	public AnimationDataBoat getAnimationDataBoat()
	{
		return ActorAnimationLoader.loadAnimationBoat(_boat_texture_id);
	}

	public void spawnBoatExplosion(Actor pActor)
	{
		EffectsLibrary.spawnAt("fx_boat_explosion", actor.current_position, actor.asset.base_stats["scale"]);
	}

	private void deathAction(Actor pActor)
	{
		if (taxi_request != null)
		{
			TaxiManager.finish(taxi_request);
			taxi_request = null;
		}
		unloadPassengers(actor.current_tile, pRandomForce: true);
	}

	internal void unloadPassengers(WorldTile pTile, bool pRandomForce = false)
	{
		foreach (Actor tActor in _passengers)
		{
			if (tActor.isAlive())
			{
				tActor.disembarkTo(this, pTile);
				if (pRandomForce)
				{
					tActor.applyRandomForce();
				}
			}
		}
		_passengers.Clear();
		taxi_target = null;
	}

	internal bool hasPassengers()
	{
		return _passengers.Count > 0;
	}

	internal int countPassengers()
	{
		return _passengers.Count;
	}

	internal bool hasPassenger(Actor pActor)
	{
		return _passengers.Contains(pActor);
	}

	public IReadOnlyCollection<Actor> getPassengers()
	{
		return _passengers;
	}

	internal void removePassenger(Actor pActor)
	{
		_passengers.Remove(pActor);
	}

	internal void addPassenger(Actor pActor)
	{
		if (_passengers.Add(pActor))
		{
			passengerWaitCounter = 0;
			if (taxi_request != null)
			{
				taxi_request.embarkToBoat(pActor);
			}
		}
	}

	public bool isHomeDockFull()
	{
		Building tHomeBuilding = actor.getHomeBuilding();
		if (tHomeBuilding == null)
		{
			return true;
		}
		if (tHomeBuilding.component_docks.isFull(actor.asset.boat_type))
		{
			return true;
		}
		return false;
	}

	public bool isHomeDockOverfilled()
	{
		Building tHomeBuilding = actor.getHomeBuilding();
		if (tHomeBuilding == null)
		{
			return true;
		}
		if (tHomeBuilding.component_docks.isOverfilled(actor.asset.boat_type))
		{
			return true;
		}
		return false;
	}

	public void destroyBecauseOverfilled()
	{
		if (isHomeDockOverfilled())
		{
			actor.getHitFullHealth(AttackType.Explosion);
		}
	}

	public override void Dispose()
	{
		_passengers.Clear();
		taxi_target = null;
		taxi_request = null;
		base.Dispose();
	}
}
