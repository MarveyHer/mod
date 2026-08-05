using FMOD.Studio;
using UnityEngine;

public class BaseEffect : BaseAnimatedObject
{
	private const int MAP_MARGIN_TOP = 25;

	private const int MAP_OFFSET_BOTTOM_MIN = -50;

	private const int MAP_OFFSET_BOTTOM_MAX = 30;

	internal bool active;

	internal int effectIndex;

	public const int STATE_START = 1;

	public const int STATE_ON_DEATH = 2;

	public const int STATE_KILLED = 3;

	protected float scale;

	protected float alpha;

	public WorldTile tile;

	internal BaseEffectController controller;

	internal int state;

	private double _timestamp_spawned;

	internal SpriteRenderer sprite_renderer;

	internal BaseCallback callback;

	internal int callbackOnFrame = -1;

	internal EventInstance fmod_instance;

	internal Actor attachedToActor;

	public double timestamp_spawned => _timestamp_spawned;

	public override void Awake()
	{
		sprite_renderer = GetComponent<SpriteRenderer>();
		base.Awake();
	}

	public void activate()
	{
		active = true;
		base.gameObject.SetActive(value: true);
		state = 1;
		_timestamp_spawned = Time.time;
		clear();
	}

	internal void attachTo(Actor pActor)
	{
		attachedToActor = pActor;
		updateAttached();
	}

	internal void makeParentController()
	{
		base.transform.SetParent(controller.transform, worldPositionStays: true);
	}

	internal virtual void prepare(WorldTile pTile, float pScale = 0.5f)
	{
		state = 1;
		base.transform.localEulerAngles = Vector3.zero;
		current_position = new Vector3((float)pTile.pos.x + 0.5f, pTile.pos.y);
		base.transform.localPosition = current_position;
		setScale(pScale);
		setAlpha(1f);
		resetAnim();
	}

	public void setScale(float pScale)
	{
		scale = pScale;
		if (scale < 0f)
		{
			scale = 0f;
		}
		base.transform.localScale = new Vector3(pScale, pScale);
	}

	internal virtual void prepare(Vector2 pVector, float pScale = 1f)
	{
		state = 1;
		base.transform.rotation = Quaternion.identity;
		base.transform.localPosition = pVector;
		setScale(pScale);
		setAlpha(1f);
		resetAnim();
	}

	protected void setAlpha(float pVal)
	{
		alpha = pVal;
		Color tColor = sprite_renderer.color;
		tColor.a = alpha;
		sprite_renderer.color = tColor;
	}

	internal virtual void prepare()
	{
		base.transform.position = new Vector3(Randy.randomInt(-50, 30), Randy.randomInt(0, MapBox.height + 25));
		state = 1;
		setAlpha(0f);
	}

	internal virtual void spawnOnTile(WorldTile pTile)
	{
		tile = pTile;
		base.transform.localPosition = new Vector3(pTile.posV3.x, pTile.posV3.y);
	}

	internal void startToDie()
	{
		state = 2;
	}

	public virtual void kill()
	{
		state = 3;
		controller.killObject(this);
	}

	public void deactivate()
	{
		if (fmod_instance.isValid())
		{
			fmod_instance.stop(STOP_MODE.ALLOWFADEOUT);
			fmod_instance.release();
		}
		active = false;
		base.transform.SetParent(base.transform);
		base.gameObject.SetActive(value: false);
		clear();
	}

	public void clear()
	{
		tile = null;
		attachedToActor = null;
		callback = null;
		callbackOnFrame = -1;
	}

	public override void update(float pElapsed)
	{
		if (controller.asset.draw_light_area)
		{
			Vector2 tPos = base.transform.position;
			tPos.x += controller.asset.draw_light_area_offset_x;
			tPos.y += controller.asset.draw_light_area_offset_y;
			World.world.stack_effects.light_blobs.Add(new LightBlobData
			{
				position = tPos,
				radius = controller.asset.draw_light_size
			});
		}
		if (!World.world.isPaused() || !DebugConfig.isOn(DebugOption.PauseEffects))
		{
			if (attachedToActor != null)
			{
				updateAttached();
			}
			base.update(pElapsed);
			if (callbackOnFrame != -1 && sprite_animation.currentFrameIndex == callbackOnFrame)
			{
				callback();
				clear();
			}
		}
	}

	private void updateAttached()
	{
		if (!attachedToActor.isAlive())
		{
			kill();
			return;
		}
		sprite_renderer.flipX = attachedToActor.a.flip;
		base.transform.localScale = attachedToActor.current_scale;
		base.transform.localPosition = attachedToActor.cur_transform_position;
		base.transform.eulerAngles = attachedToActor.current_rotation;
	}

	public void setCallback(int pFrame, BaseCallback pCallback)
	{
		callbackOnFrame = pFrame;
		callback = pCallback;
	}

	public bool isKilled()
	{
		return state == 3;
	}
}
