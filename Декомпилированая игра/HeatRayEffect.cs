using UnityEngine;

public class HeatRayEffect : BaseAnimatedObject
{
	public SpriteAnimation ray;

	public SpriteAnimation heat;

	private bool active;

	private int ticksActive;

	private bool touchedGround;

	private float rayScaleY;

	private float rayWidth = 1f;

	public override void Awake()
	{
		base.Awake();
		ray.transform.localScale = new Vector3(1f, 0f, 1f);
	}

	private void Update()
	{
		update(World.world.elapsed);
		ray.update(World.world.elapsed);
		heat.update(World.world.elapsed);
		if (ticksActive > 0)
		{
			ticksActive--;
		}
		else
		{
			active = false;
		}
	}

	internal bool isReady()
	{
		return touchedGround;
	}

	public Vector2 getPosForLight()
	{
		return heat.transform.position;
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		Vector3 tVec = ray.transform.position;
		tVec.z = base.transform.position.y;
		ray.transform.position = tVec;
		tVec = heat.transform.position;
		heat.transform.position = tVec;
		if (active)
		{
			if (rayScaleY < 2000f)
			{
				rayScaleY += pElapsed * 7000f;
				if (rayScaleY >= 2000f)
				{
					rayScaleY = 2000f;
					touchedGround = true;
				}
				ray.transform.localScale = new Vector3(rayWidth, rayScaleY, 1f);
			}
		}
		else
		{
			touchedGround = false;
			if (rayScaleY > 0f)
			{
				rayScaleY -= pElapsed * 4000f;
				if (rayScaleY < 0f)
				{
					rayScaleY = 0f;
					base.gameObject.SetActive(value: false);
				}
				ray.transform.localScale = new Vector3(rayWidth, rayScaleY, 1f);
			}
		}
		heat.gameObject.SetActive(touchedGround);
	}

	internal void play(Vector2 pPos, int pSize)
	{
		if (pSize >= 10)
		{
			rayWidth = 1f;
		}
		else
		{
			rayWidth = 0.4f;
		}
		base.transform.localPosition = new Vector3(pPos.x, pPos.y);
		active = true;
		ticksActive = 4;
		base.gameObject.SetActive(value: true);
	}
}
