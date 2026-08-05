using UnityEngine;

public class Cloud : BaseEffect
{
	public CloudAsset asset;

	private float speed = 1f;

	public SpriteShadow spriteShadow;

	private float _timer_action_1;

	private float _timer_action_2;

	internal float alive_time;

	private float _fade_multiplier = 0.2f;

	internal float effect_texture_width;

	internal float effect_texture_height;

	private float _lifespan;

	internal override void create()
	{
		base.create();
	}

	internal override void prepare()
	{
		sprite_renderer.sprite = Randy.getRandom(asset.cached_sprites);
		sprite_renderer.flipX = Randy.randomBool();
		speed = Randy.randomFloat(asset.speed_min, asset.speed_max);
		effect_texture_width = sprite_renderer.sprite.rect.width * 0.08f;
		effect_texture_height = sprite_renderer.sprite.rect.height * 0.04f;
		_timer_action_1 = asset.interval_action_1;
		_lifespan = 0f;
		alive_time = 0f;
		base.prepare();
		setAlpha(0f);
	}

	public void setLifespan(float pLifespan)
	{
		_lifespan = pLifespan;
	}

	internal void setType(CloudAsset pAsset)
	{
		asset = pAsset;
		sprite_renderer.color = asset.color;
	}

	internal void setType(string pType)
	{
		CloudAsset tAsset = AssetManager.clouds.get(pType);
		if (tAsset != null)
		{
			setType(tAsset);
		}
	}

	public void spawn(WorldTile pTile, string pType)
	{
		if (pTile == null)
		{
			setType(pType);
			prepare();
		}
		else
		{
			prepare(pTile.posV3, pType);
		}
	}

	internal void prepare(Vector3 pVec, string pType)
	{
		setType(pType);
		prepare();
		pVec.y -= spriteShadow.offset.y;
		base.transform.localPosition = pVec;
	}

	internal override void prepare(WorldTile pTile, float pScale = 0.5f)
	{
		prepare();
		base.transform.localPosition = new Vector3(pTile.posV3.x, pTile.posV3.y);
	}

	public override void update(float pElapsed)
	{
		alive_time += pElapsed;
		if (Config.time_scale_asset.sonic)
		{
			_fade_multiplier = 0.05f;
		}
		else
		{
			_fade_multiplier = 0.2f;
		}
		if (asset.draw_light_area)
		{
			Vector2 tPos = base.transform.localPosition;
			tPos.x += asset.draw_light_area_offset_x;
			tPos.y += asset.draw_light_area_offset_y;
			World.world.stack_effects.light_blobs.Add(new LightBlobData
			{
				position = tPos,
				radius = asset.draw_light_size
			});
		}
		if (!World.world.isPaused())
		{
			base.transform.Translate(speed * pElapsed, 0f, 0f);
			if (asset.cloud_action_1 != null)
			{
				if (_timer_action_1 > 0f)
				{
					_timer_action_1 -= pElapsed;
				}
				else
				{
					_timer_action_1 = asset.interval_action_1;
					asset.cloud_action_1(this);
				}
			}
			if (asset.cloud_action_2 != null)
			{
				if (_timer_action_2 > 0f)
				{
					_timer_action_2 -= pElapsed;
				}
				else
				{
					_timer_action_2 = asset.interval_action_2;
					asset.cloud_action_2(this);
				}
			}
		}
		if (base.transform.localPosition.x > (float)MapBox.width || (_lifespan > 0f && alive_time > _lifespan))
		{
			startToDie();
		}
		float mAlpha = asset.max_alpha;
		if (World.world.camera != null && World.world.camera.orthographicSize > 0f)
		{
			mAlpha *= World.world.camera.orthographicSize / 100f;
			if (mAlpha > asset.max_alpha)
			{
				mAlpha = asset.max_alpha;
			}
		}
		switch (state)
		{
		case 1:
			if (alpha < mAlpha)
			{
				alpha += pElapsed * _fade_multiplier;
				if (alpha >= mAlpha)
				{
					alpha = mAlpha;
				}
			}
			else if (alpha > mAlpha)
			{
				alpha -= pElapsed * _fade_multiplier;
				if (alpha <= mAlpha)
				{
					alpha = mAlpha;
				}
			}
			else
			{
				alpha = mAlpha;
			}
			setAlpha(alpha);
			break;
		case 2:
			if (alpha > 0f)
			{
				alpha -= pElapsed * _fade_multiplier;
				setAlpha(alpha);
			}
			else
			{
				alpha = 0f;
				controller.killObject(this);
			}
			break;
		}
	}
}
