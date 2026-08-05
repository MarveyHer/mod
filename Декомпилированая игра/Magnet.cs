using System;
using System.Collections.Generic;
using UnityEngine;

public class Magnet
{
	private const float ANIMATED_SHRINK_SPEED = 0.3f;

	private const float PICKED_UP_SPEED_MULTIPLIER = 0.1f;

	private int _magnet_state;

	private WorldTile _magnet_last_pos;

	private bool _has_units;

	internal List<Actor> magnet_units = new List<Actor>();

	private HashSet<Actor> _magnet_units = new HashSet<Actor>();

	private float _picked_up_multiplier = 1f;

	private float _angle;

	public float moving_angle;

	private MagnetThrow _magnet_throw = new MagnetThrow();

	private float _target_angle;

	private float _current_angle;

	private float _rotation_velocity;

	internal void magnetAction(bool pFromUpdate, WorldTile pTile = null)
	{
		if (ScrollWindow.isWindowActive())
		{
			dropPickedUnits();
		}
		else
		{
			if (pFromUpdate && _magnet_state != 1 && _magnet_state != 3)
			{
				return;
			}
			if (pTile != null)
			{
				_magnet_last_pos = pTile;
			}
			_magnet_throw.trackMouseMovement(_magnet_state);
			updatePickedUnits();
			if (pTile != null)
			{
				World.world.flash_effects.flashPixel(pTile, 10);
			}
			switch (_magnet_state)
			{
			case 0:
				if (Input.GetMouseButton(0))
				{
					_magnet_state = 1;
					_magnet_throw.initializeMouseTracking();
				}
				break;
			case 1:
				if (!pFromUpdate)
				{
					pickupUnits(pTile);
				}
				if (Input.GetMouseButtonUp(0))
				{
					_magnet_state = 2;
					dropPickedUnits();
				}
				break;
			case 2:
				if (!pFromUpdate && Input.GetMouseButton(0))
				{
					dropPickedUnits();
					_magnet_state = 0;
				}
				break;
			}
		}
	}

	public void dropPickedUnits()
	{
		if (magnet_units.Count == 0)
		{
			return;
		}
		Vector2 tForce = _magnet_throw.calculateThrowForce();
		for (int i = 0; i < magnet_units.Count; i++)
		{
			Actor tActor = magnet_units[i];
			if (tActor != null && tActor.isAlive())
			{
				tActor.current_position.y -= tActor.position_height;
				tActor.is_in_magnet = false;
				tActor.dirty_current_tile = true;
				tActor.findCurrentTile();
				tActor.spawnOn(tActor.current_tile, tActor.getActorAsset().default_height);
				tActor.makeStunned(1f);
				tActor.addStatusEffect("magnetized");
				tActor.target_angle.z = 0f;
				if (tForce.magnitude > 0.1f)
				{
					Vector2 tRandomUnitForce = tForce;
					tRandomUnitForce.x += UnityEngine.Random.Range(-0.3f, 0.3f);
					tRandomUnitForce.y += UnityEngine.Random.Range(-0.3f, 0.3f);
					tActor.addForce(tRandomUnitForce.x, tRandomUnitForce.y, tRandomUnitForce.magnitude * 0.3f, pCheckLandCancelAllActions: true, pIgnorePosHeight: true);
				}
				else
				{
					tActor.addForce(0f, 0f, 0.1f, pCheckLandCancelAllActions: true);
				}
				tActor.addActionWaitAfterLand(0.5f);
			}
		}
		magnet_units.Clear();
		_magnet_units.Clear();
		_has_units = false;
		_magnet_throw.clear();
	}

	private void updatePickedUnits()
	{
		if (_magnet_last_pos == null || magnet_units.Count == 0)
		{
			return;
		}
		updateMovingForce();
		if (_picked_up_multiplier > 0.1f)
		{
			_picked_up_multiplier -= World.world.delta_time * 0.3f;
			if (_picked_up_multiplier < 0.1f)
			{
				_picked_up_multiplier = 0.1f;
			}
		}
		float tCount = magnet_units.Count;
		float tSeconds = 6f;
		if (tCount > 100f)
		{
			tSeconds = 4f;
		}
		else if (tCount > 50f)
		{
			tSeconds = 4.5f;
		}
		else if (tCount > 5f)
		{
			tSeconds = 5f;
		}
		float tSpeed = MathF.PI * 2f / tSeconds;
		int tBrushSize = Config.current_brush_data.width + 1;
		float tRadius = ((0 == 0) ? ((float)tBrushSize / 2f) : (Mathf.Lerp(0f, tBrushSize, _picked_up_multiplier) / 2f));
		float tMultiplier = 1f / tCount * tRadius;
		_angle += tSpeed * Time.deltaTime;
		Vector2 tCursorPos = World.world.getMousePos();
		for (int i = 0; (float)i < tCount; i++)
		{
			Actor tActor = magnet_units[i];
			if (tActor != null && tActor.isAlive())
			{
				tActor.findCurrentTile();
				Vector3 tPos = tCursorPos;
				tPos.x += Mathf.Cos(_angle + (float)i) * (tMultiplier * (float)i);
				tPos.y += Mathf.Sin(_angle + (float)i) * (tMultiplier * (float)i);
				tActor.current_position = new Vector2(tPos.x, tPos.y - tActor.position_height);
				tActor.callbacks_magnet_update?.Invoke(tActor);
			}
		}
	}

	private void updateMovingForce()
	{
		Vector2 tForce = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 3f;
		if (tForce.magnitude > 0.1f)
		{
			_target_angle = Mathf.Atan2(tForce.y, tForce.x) * 57.29578f;
			_target_angle -= 90f;
		}
		else
		{
			_target_angle = 0f;
		}
		_current_angle = Mathf.SmoothDampAngle(_current_angle, _target_angle, ref _rotation_velocity, 0.2f);
		moving_angle = _current_angle;
	}

	private void pickupUnits(WorldTile pTile)
	{
		BrushPixelData[] tBrushPixels = Config.current_brush_data.pos;
		for (int i = 0; i < tBrushPixels.Length; i++)
		{
			WorldTile tTile = World.world.GetTile(tBrushPixels[i].x + pTile.x, tBrushPixels[i].y + pTile.y);
			if (tTile == null || !tTile.hasUnits())
			{
				continue;
			}
			tTile.doUnits(delegate(Actor tActor)
			{
				if (tActor.asset.can_be_moved_by_powers && !tActor.isInsideSomething() && _magnet_units.Add(tActor))
				{
					tActor.cancelAllBeh();
					magnet_units.Add(tActor);
					tActor.is_in_magnet = true;
					_picked_up_multiplier = 2f;
				}
			});
		}
		_has_units = _magnet_units.Count > 0;
	}

	public int countUnits()
	{
		return _magnet_units.Count;
	}

	public bool hasUnits()
	{
		return _has_units;
	}
}
