using UnityEngine;

public class Drop : BaseMapObject
{
	private readonly bool DEBUG_COLOR;

	public int drop_index;

	internal bool active;

	private SpriteRenderer _sprite_renderer;

	private SpriteAnimation _sprite_animation;

	private float _currentHeightZ;

	private bool _landed;

	private DropAsset _asset;

	internal bool soundOn;

	private bool _parabolic;

	private float _falling_speed;

	private float _scale = 1f;

	private bool _force_surprise;

	private long _caster_id = -1L;

	private Vector2 _targetPosition;

	private Vector2 _startPosition;

	private float _targetHeight;

	private float _timeToTarget;

	private float _timeInAir;

	private Color _gizmoColor = Vector4.zero;

	private Color _gizmoColor2 = Vector4.zero;

	private float _rotation_speed;

	private void Awake()
	{
		_sprite_renderer = base.gameObject.GetComponent<SpriteRenderer>();
		_sprite_animation = base.gameObject.GetComponent<SpriteAnimation>();
	}

	public void setForceSurprise()
	{
		_force_surprise = true;
	}

	internal void prepare()
	{
		if (!created)
		{
			create();
		}
		base.gameObject.SetActive(value: true);
		m_transform.localScale = Vector3.one;
		active = true;
		_force_surprise = false;
		_timeInAir = 0f;
		_timeToTarget = 0f;
		_landed = false;
		_parabolic = false;
		soundOn = false;
		_currentHeightZ = 0f;
		_caster_id = -1L;
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		if (DEBUG_COLOR)
		{
			_sprite_renderer.color = Randy.getRandomColor();
		}
	}

	internal void launchStraight(WorldTile pTile, DropAsset pAsset, float zDropHeight = -1f)
	{
		_asset = pAsset;
		if (_asset.animation_rotation)
		{
			_rotation_speed = Randy.randomFloat(_asset.animation_rotation_speed_min, _asset.animation_rotation_speed_max);
			if (Randy.randomBool())
			{
				_rotation_speed *= -1f;
			}
		}
		if (!string.IsNullOrEmpty(_asset.sound_launch))
		{
			MusicBox.playSound(_asset.sound_launch, pTile);
		}
		if (_asset.action_launch != null)
		{
			_asset.action_launch();
		}
		_falling_speed = _asset.falling_speed + Randy.randomFloat(0f, _asset.falling_speed_random);
		if (_asset.cached_sprites == null || _asset.cached_sprites.Length == 0)
		{
			_asset.cached_sprites = SpriteTextureLoader.getSpriteList(_asset.path_texture);
		}
		_sprite_renderer.sharedMaterial = LibraryMaterials.instance.dict[_asset.material];
		_sprite_animation.setFrames(_asset.cached_sprites);
		if (_asset.random_flip)
		{
			_sprite_renderer.flipX = (Randy.randomBool() ? true : false);
		}
		if (_asset.animated)
		{
			_sprite_animation.isOn = true;
			_sprite_animation.timeBetweenFrames = _asset.animation_speed + Randy.randomFloat(0f, _asset.animation_speed_random);
		}
		else
		{
			_sprite_animation.isOn = false;
		}
		if (_asset.random_frame)
		{
			_sprite_animation.setRandomFrame();
		}
		_sprite_animation.forceUpdateFrame();
		current_tile = pTile;
		if (zDropHeight != -1f)
		{
			_currentHeightZ = zDropHeight;
		}
		else
		{
			_currentHeightZ = (int)Randy.randomFloat(pAsset.falling_height.x, pAsset.falling_height.y);
		}
		current_position = new Vector2(pTile.posV3.x, pTile.posV3.y);
		_startPosition = current_position;
		updatePosition();
	}

	public void launchParabolic(float pStartHeight, float pMinHeight, float pMaxHeight, float pMinRadius, float pMaxRadius)
	{
		Vector2 tRandomVec = Randy.randomPointOnCircle(pMinRadius, pMaxRadius);
		_targetPosition = _startPosition + tRandomVec;
		_targetHeight = Randy.randomFloat(pMinHeight, pMaxHeight);
		_startPosition.y += pStartHeight;
		_currentHeightZ = _startPosition.y;
		_timeInAir = 0f;
		if (_scale < 1f)
		{
			_falling_speed /= _scale * 2f;
		}
		float tDist = Toolbox.DistVec2Float(_startPosition, _targetPosition);
		_timeToTarget = (tDist + _targetHeight * 3f) * 0.25f / _falling_speed;
		if (_timeToTarget < 1f)
		{
			_timeToTarget += 0.5f;
		}
		_parabolic = true;
		updatePosition();
	}

	private void updateStraightFall(float pElapsed)
	{
		float tChange = 15f * pElapsed;
		tChange = ((!(_scale < 1f)) ? (tChange * _falling_speed) : (tChange * (_falling_speed / (_scale * 2f))));
		if (_currentHeightZ < 0f)
		{
			tChange = 0f;
		}
		_currentHeightZ -= tChange * _scale;
		applyRandomXMove(tChange);
		if (_currentHeightZ <= 0f)
		{
			_currentHeightZ = 0f;
			updatePosition();
			current_tile = World.world.GetTile((int)current_position.x, (int)current_position.y);
			land();
		}
		else
		{
			updatePosition();
		}
	}

	private void applyRandomXMove(float pChangeX)
	{
		if (_asset.falling_random_x_move && !(pChangeX <= 0f) && Randy.randomBool())
		{
			if (Randy.randomBool())
			{
				current_position.x -= 1f * _scale;
			}
			else
			{
				current_position.x += 1f * _scale;
			}
		}
	}

	private void land()
	{
		if (current_tile != null)
		{
			if (_asset.action_landed != null)
			{
				_asset.action_landed(current_tile, _asset.id);
			}
			if (_asset.action_landed_drop != null)
			{
				_asset.action_landed_drop(this, current_tile, _asset.id);
			}
			if (current_tile.zone.visible && _asset.sound_drop != string.Empty)
			{
				MusicBox.playSound(_asset.sound_drop, current_tile);
			}
			if (_force_surprise || _asset.surprises_units)
			{
				ActionLibrary.suprisedByArchitector(null, current_tile);
			}
		}
		World.world.drop_manager.landDrop(this);
		_landed = true;
	}

	public override void update(float pElapsed)
	{
		if (!_landed)
		{
			_sprite_animation.update(pElapsed);
			if (_parabolic)
			{
				updateParabolicFall(pElapsed);
			}
			else
			{
				updateStraightFall(pElapsed);
			}
			if (!_landed && _asset.animation_rotation)
			{
				updateRotation(pElapsed);
			}
		}
	}

	private void updateRotation(float pElapsed)
	{
		float tAngleZ = base.transform.rotation.eulerAngles.z + _rotation_speed * pElapsed;
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, tAngleZ));
	}

	private void updateParabolicFall(float pElapsed)
	{
		if (!(_timeInAir > _timeToTarget))
		{
			_timeInAir += pElapsed;
			if (_timeInAir > _timeToTarget)
			{
				_timeInAir = _timeToTarget;
			}
			float tTime = _timeInAir / _timeToTarget;
			Vector2 tParabolicPos = Toolbox.ParabolaDrag(_startPosition, _targetPosition, _targetHeight, tTime);
			Vector2 tStraightLine = Vector2.Lerp(_startPosition, _targetPosition, tTime);
			_currentHeightZ = tParabolicPos.y - tStraightLine.y;
			float tX = tParabolicPos.x;
			float tY = tParabolicPos.y - _currentHeightZ;
			current_position.Set(tX, tY);
			if (current_position == _targetPosition)
			{
				current_tile = World.world.GetTile((int)_targetPosition.x, (int)_targetPosition.y);
				land();
			}
			else if (_timeInAir >= _timeToTarget)
			{
				current_tile = World.world.GetTile((int)_targetPosition.x, (int)_targetPosition.y);
				land();
			}
			else
			{
				updatePosition();
			}
		}
	}

	private void updatePosition()
	{
		Vector3 tVec = new Vector3(current_position.x, current_position.y + _currentHeightZ, _currentHeightZ);
		m_transform.position = tVec;
	}

	public void setScale(Vector3 pVec)
	{
		m_transform.localScale = pVec;
		_scale = pVec.x;
	}

	public void setCasterId(long pCasterId)
	{
		_caster_id = pCasterId;
	}

	public long getCasterId()
	{
		return _caster_id;
	}

	public void makeInactive()
	{
		reset();
		active = false;
		base.gameObject.SetActive(value: false);
	}

	public void reset()
	{
		_asset = null;
		current_tile = null;
	}

	private void OnDrawGizmos()
	{
		if (_parabolic && !_landed && _timeToTarget != 0f && !(_timeInAir > _timeToTarget))
		{
			if (_gizmoColor.Equals(Vector4.zero))
			{
				_gizmoColor = Randy.ColorHSV();
			}
			if (_gizmoColor2.Equals(Vector4.zero))
			{
				_gizmoColor2 = Randy.ColorHSV();
				_gizmoColor2.a = 0.5f;
			}
			Gizmos.color = _gizmoColor;
			Vector2 previousDrawPoint = _startPosition;
			Vector2 previousDrawPoint2 = _startPosition;
			int resolution = 60;
			for (int i = 1; i <= resolution; i++)
			{
				float simulationTime = (float)i / (float)resolution * _timeToTarget;
				Vector2 currentPosition = Toolbox.ParabolaDrag(_startPosition, _targetPosition, _targetHeight, simulationTime);
				Vector2 currentPosition2 = Toolbox.Parabola(_startPosition, _targetPosition, _targetHeight, simulationTime);
				Gizmos.color = _gizmoColor;
				Gizmos.DrawLine(previousDrawPoint, currentPosition);
				Gizmos.color = _gizmoColor2;
				Gizmos.DrawLine(previousDrawPoint, currentPosition2);
				Gizmos.DrawLine(currentPosition, currentPosition2);
				Gizmos.DrawLine(previousDrawPoint2, currentPosition2);
				previousDrawPoint = currentPosition;
				previousDrawPoint2 = currentPosition2;
			}
		}
	}
}
