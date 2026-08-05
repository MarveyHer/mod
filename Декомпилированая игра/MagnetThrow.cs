using System.Collections.Generic;
using UnityEngine;

public class MagnetThrow
{
	private Vector2 _mouse_velocity = Vector2.zero;

	private Vector2 _last_mouse_position;

	private readonly List<Vector2> _velocity_samples = new List<Vector2>();

	private const int MAX_VELOCITY_SAMPLES = 5;

	private const float THROW_FORCE_MULTIPLIER = 5f;

	public const float MIN_THROW_FORCE = 0.1f;

	private const float MAX_THROW_FORCE = 10f;

	private Vector2 _throw_momentum = Vector2.zero;

	private const float MOMENTUM_DECAY = 0.85f;

	private const float MOMENTUM_BUILD_RATE = 0.7f;

	public void initializeMouseTracking()
	{
		_last_mouse_position = World.world.getMousePos();
		_velocity_samples.Clear();
		_throw_momentum = Vector2.zero;
	}

	public void trackMouseMovement(int pMagnetState)
	{
		if (pMagnetState == 1)
		{
			Vector2 tCurrentMousePos = World.world.getMousePos();
			Vector2 tMouseDelta = tCurrentMousePos - _last_mouse_position;
			_mouse_velocity = tMouseDelta * 60f;
			_velocity_samples.Add(_mouse_velocity);
			if (_velocity_samples.Count > 5)
			{
				_velocity_samples.RemoveAt(0);
			}
			Vector2 tTargetMomentum = tMouseDelta * 0.7f;
			_throw_momentum = Vector2.Lerp(_throw_momentum, tTargetMomentum, Time.deltaTime * 10f);
			_last_mouse_position = tCurrentMousePos;
		}
	}

	public Vector2 calculateThrowForce()
	{
		Vector2 tAverageVelocity = Vector2.zero;
		if (_velocity_samples.Count > 0)
		{
			foreach (Vector2 sample in _velocity_samples)
			{
				tAverageVelocity += sample;
			}
			tAverageVelocity /= (float)_velocity_samples.Count;
		}
		Vector2 tThrowForce = tAverageVelocity * 5f * Time.deltaTime;
		float tForceMagnitude = tThrowForce.magnitude;
		if (tForceMagnitude > 10f)
		{
			tThrowForce = tThrowForce.normalized * 10f;
		}
		else if (tForceMagnitude < 0.1f && tForceMagnitude > 0.1f)
		{
			tThrowForce = tThrowForce.normalized * 0.1f;
		}
		return tThrowForce;
	}

	public void clear()
	{
		_velocity_samples.Clear();
		_throw_momentum = Vector2.zero;
	}
}
