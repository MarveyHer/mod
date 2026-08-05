using System.Collections.Generic;
using UnityEngine;

public class Boulder : BaseEffect
{
	private const float SPEED = 2.5f;

	private const int BOUNCES_AMOUNT = 3;

	private const float SINGLE_BOUNCE_TIMER = 2f;

	private const float BASE_HEIGHT_MULTIPLIER = 10f;

	private const float BASE_LENGTH_MULTIPLIER = 40f;

	private const float INITIAL_ANGLE_RANGE = 200f;

	private const float CHARGE_VECTOR_MULTIPLIER = 0.777f;

	private const float Z_SORTING_FIX = 5f;

	private const int NO_TOUCH_ID = -2;

	private float angle;

	private float angleRotation;

	private float impactEffect;

	public GameObject mainSprite;

	public GameObject shadowSprite;

	private SpriteRenderer shadowRenderer;

	private Transform mainTransform;

	private Transform shadowTransform;

	private Vector2 _previous_bounce_position;

	private List<Vector2> _bounce_positions = new List<Vector2>();

	private int _bounces_left;

	private float _force_timer;

	private static bool _charge_started;

	private static Vector2 _initial_charge_position;

	private static Touch _latest_touch;

	private static int _latest_touch_id = -2;

	public override void Awake()
	{
		base.Awake();
		sprite_renderer = mainSprite.GetComponent<SpriteRenderer>();
		shadowRenderer = shadowSprite.GetComponent<SpriteRenderer>();
		mainTransform = mainSprite.transform;
		shadowTransform = shadowSprite.transform;
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		updateForce(pElapsed);
		if (impactEffect > 0f)
		{
			impactEffect -= pElapsed;
		}
		if (position_height != 0f)
		{
			angle += angleRotation * pElapsed;
			mainTransform.localEulerAngles = new Vector3(0f, 0f, angle);
		}
	}

	private void updateForce(float pElapsed)
	{
		_force_timer -= pElapsed * 2.5f;
		if (_force_timer <= 0f)
		{
			_force_timer = 2f;
			actionLanded();
			return;
		}
		float tHeight = getHeightPosition();
		Vector2 tUpdatedBouncePos = calcCurrentPos();
		setCurrentPosition(tUpdatedBouncePos.x, tUpdatedBouncePos.y, tHeight);
		updateCurrentPosition();
	}

	private void updateShadow()
	{
		float value = (position_height / -5f + 10f) / 10f;
		float tShadowAlpha = Mathf.Clamp(value, 0.15f, 1f) * 0.3f;
		setShadowAlpha(tShadowAlpha);
		float tShadowScale = Mathf.Clamp(value, 0.25f, 0.9f) * 0.3f;
		Vector3 tScale = shadowTransform.localScale;
		tScale.Set(tShadowScale, tShadowScale, 1f);
		shadowTransform.localScale = tScale;
	}

	private void setShadowAlpha(float pVal)
	{
		float alpha = pVal;
		if (alpha < 0f)
		{
			alpha = 0f;
		}
		Color tColor = shadowRenderer.color;
		tColor.a = alpha;
		shadowRenderer.color = tColor;
	}

	private void spawnEffect(string pEffectID)
	{
		if (!(impactEffect > 0f))
		{
			impactEffect = 0.8f;
			Vector3 tVec = current_position;
			tVec.y -= 2f;
			EffectsLibrary.spawnAt(pEffectID, tVec, mainTransform.localScale.x);
		}
	}

	internal void actionLanded()
	{
		_previous_bounce_position = current_position;
		_bounces_left--;
		current_tile = World.world.GetTile((int)base.transform.localPosition.x, (int)base.transform.localPosition.y);
		bool tBounceAgain = true;
		if (current_tile != null && current_tile.Type.lava)
		{
			tBounceAgain = false;
		}
		if (_bounces_left < 1)
		{
			tBounceAgain = false;
		}
		if (tBounceAgain)
		{
			sequencedBounce();
		}
		else
		{
			explosion();
		}
	}

	private void sequencedBounce()
	{
		Vector3 tVec = current_position;
		tVec.y -= 2f;
		EffectsLibrary.spawnExplosionWave(tVec, (float)_bounces_left * 0.14f, 6f);
		World.world.startShake(0.3f, 0.01f, 1f);
		if (!Toolbox.inMapBorder(ref current_position))
		{
			spawnEffect("fx_boulder_impact_water");
		}
		else if (current_tile != null)
		{
			if (current_tile.Type.ocean)
			{
				spawnEffect("fx_boulder_impact_water");
			}
			else
			{
				spawnEffect("fx_boulder_impact");
			}
			World.world.loopWithBrush(current_tile, Brush.get(5), tileDrawBoulder);
			World.world.applyForceOnTile(current_tile, 5, 0.5f, pForceOut: false);
			World.world.conway_layer.checkKillRange(current_tile.pos, 5);
		}
	}

	private void explosion()
	{
		if (current_tile == null || current_tile.Type.ocean)
		{
			spawnEffect("fx_boulder_impact_water");
		}
		else
		{
			spawnEffect("fx_boulder_impact");
		}
		impactEffect = 0f;
		if (Toolbox.inMapBorder(ref current_position))
		{
			MapAction.damageWorld(current_tile, 10, AssetManager.terraform.get("bomb"));
		}
		spawnEffect("fx_explosion_small");
		controller.killObject(this);
	}

	public static bool tileDrawBoulder(WorldTile pTile, string pPowerID)
	{
		pTile.doUnits(delegate(Actor pActor)
		{
			AchievementLibrary.ball_to_ball.checkBySignal(pActor);
			pActor.getHitFullHealth(AttackType.Gravity);
		});
		if (pTile.Type.ocean && Randy.randomChance(0.3f))
		{
			World.world.drop_manager.spawnParabolicDrop(pTile, "rain", 0f, 1f, 30f, 0.7f, 22f);
		}
		if (pTile.Type.lava && Randy.randomChance(0.3f))
		{
			World.world.drop_manager.spawnParabolicDrop(pTile, "lava", 0f, 1f, 30f, 0.7f, 22f);
		}
		MapAction.decreaseTile(pTile, pDamage: true, "destroy");
		return true;
	}

	public void spawnOn(Vector2 pPosition)
	{
		_bounce_positions.Clear();
		if (isRandomLaunch(pPosition))
		{
			_force_timer = 1f;
		}
		else
		{
			_force_timer = 2f;
		}
		_bounces_left = 3;
		angle = 0f;
		angleRotation = Randy.randomFloat(-200f, 200f);
		impactEffect = 0f;
		Vector2 tForce = default(Vector2);
		if (isRandomLaunch(pPosition))
		{
			tForce.x = Randy.randomFloat(-40f, 40f);
			tForce.y = Randy.randomFloat(-40f, 40f);
			tForce = Vector2.ClampMagnitude(tForce, 40f);
		}
		else
		{
			tForce = chargeVector(pPosition) * 0.777f;
		}
		_previous_bounce_position = pPosition;
		_previous_bounce_position.y -= getHeightPosition();
		_previous_bounce_position -= tForce * getBounceProgress();
		for (int i = 0; i < 3; i++)
		{
			int tMultiplier = i + 1;
			Vector2 tVector = new Vector2
			{
				x = _previous_bounce_position.x + tForce.x * (float)tMultiplier,
				y = _previous_bounce_position.y + tForce.y * (float)tMultiplier
			};
			_bounce_positions.Add(tVector);
		}
		updateCurrentPosition();
		endCharging();
	}

	private void setCurrentPosition(float pX, float pY, float pHeight)
	{
		current_position.x = pX;
		current_position.y = pY;
		position_height = pHeight;
	}

	private void updateCurrentPosition()
	{
		Vector3 tPosition = base.transform.localPosition;
		tPosition.x = current_position.x;
		tPosition.y = current_position.y;
		tPosition.z = position_height + 5f;
		base.transform.localPosition = tPosition;
		Vector3 tHeight = mainTransform.localPosition;
		tHeight.y = position_height;
		mainTransform.localPosition = tHeight;
		updateShadow();
	}

	private float getBounceProgress()
	{
		return 1f - _force_timer / 2f;
	}

	private float getBounceProgressMirrored()
	{
		return 1f - Mathf.Abs(getBounceProgress() * 2f - 1f);
	}

	private float getHeightProgress()
	{
		return iTween.easeOutQuad(0f, 1f, getBounceProgressMirrored());
	}

	private float getHeightPosition()
	{
		return (float)_bounces_left * getHeightProgress() * 10f;
	}

	private int getCurrentBounceIndex()
	{
		return 3 - _bounces_left;
	}

	private Vector2 getNextBouncePos()
	{
		return _bounce_positions[getCurrentBounceIndex()];
	}

	private Vector2 calcCurrentPos()
	{
		return Vector2.Lerp(_previous_bounce_position, getNextBouncePos(), getBounceProgress());
	}

	public static void chargeBoulder(Vector2 pPosition, Touch pTouch = default(Touch))
	{
		_latest_touch = pTouch;
		if (ScrollWindow.isWindowActive())
		{
			endCharging();
		}
		else if (HotkeyLibrary.many_mod.isHolding() || (!InputHelpers.mouseSupported && DebugConfig.isOn(DebugOption.FastSpawn)))
		{
			if (_charge_started)
			{
				endCharging();
			}
			releaseManyBoulders(pPosition);
		}
		else if (isInteractionJustStarted())
		{
			startCharging(pPosition);
		}
		else if (isInteractionJustEnded())
		{
			releaseBoulder();
		}
	}

	private static void startCharging(Vector2 pPosition)
	{
		_charge_started = true;
		_initial_charge_position = pPosition;
		_latest_touch_id = _latest_touch.fingerId;
	}

	private static void endCharging()
	{
		_charge_started = false;
		_latest_touch_id = -2;
	}

	public static void checkRelease()
	{
		if (!_charge_started)
		{
			return;
		}
		if (!isBoulderPowerSelected())
		{
			endCharging();
			return;
		}
		spawnParticles();
		if (isInteractionJustEnded())
		{
			releaseBoulder();
		}
	}

	private static void releaseManyBoulders(Vector2 pPosition)
	{
		_initial_charge_position = pPosition;
		releaseBoulder();
	}

	private static void releaseBoulder()
	{
		Vector2 tPosition = getPointerPosition();
		EffectsLibrary.spawnAt("fx_boulder", tPosition, 1f);
	}

	private static void spawnParticles()
	{
		Vector2 tPosition = Vector2.zero;
		if (getPointerPositionPure(ref tPosition) && !isRandomLaunch(tPosition))
		{
			EffectsLibrary.spawnAt("fx_boulder_charge", tPosition, 1f);
		}
	}

	private static bool isRandomLaunch(Vector2 pPosition)
	{
		return chargeVector(pPosition).magnitude < 1.5f;
	}

	private static bool isBoulderPowerSelected()
	{
		return PowerButtonSelector.instance.selectedButton?.godPower?.id == "bowling_ball";
	}

	private static Vector2 chargeVector(Vector2 pPosition)
	{
		return _initial_charge_position - pPosition;
	}

	public static Vector2 chargeVector()
	{
		return chargeVector(getPointerPosition());
	}

	private static bool isInteractionJustStarted()
	{
		if (_charge_started)
		{
			return false;
		}
		if (InputHelpers.mouseSupported)
		{
			if (Input.GetMouseButtonDown(0))
			{
				return true;
			}
		}
		else if (_latest_touch.fingerId != _latest_touch_id)
		{
			return true;
		}
		return false;
	}

	private static bool isInteractionJustEnded()
	{
		if (InputHelpers.mouseSupported)
		{
			if (Input.GetMouseButtonUp(0))
			{
				return true;
			}
		}
		else if (Input.touchCount == 0 || _latest_touch.phase == TouchPhase.Ended)
		{
			return true;
		}
		return false;
	}

	private static Vector2 getPointerPosition()
	{
		if (InputHelpers.mouseSupported)
		{
			return World.world.getMousePos();
		}
		return World.world.camera.ScreenToWorldPoint(_latest_touch.position);
	}

	private static bool getPointerPositionPure(ref Vector2 pPosition)
	{
		if (InputHelpers.mouseSupported)
		{
			pPosition = World.world.getMousePos();
			return true;
		}
		if (World.world.player_control.getTouchPos(out var tTouch))
		{
			pPosition = World.world.camera.ScreenToWorldPoint(tTouch.position);
			return true;
		}
		return false;
	}
}
