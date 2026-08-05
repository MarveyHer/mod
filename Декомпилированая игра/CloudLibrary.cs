using UnityEngine;

public class CloudLibrary : AssetLibrary<CloudAsset>
{
	private static string[] _sprites_small = new string[2] { "effects/clouds/cloud_small_1", "effects/clouds/cloud_small_2" };

	private static string[] _sprites_big = new string[3] { "effects/clouds/cloud_big_1", "effects/clouds/cloud_big_2", "effects/clouds/cloud_big_3" };

	private static string[] _sprites_all = new string[5] { "effects/clouds/cloud_small_1", "effects/clouds/cloud_small_2", "effects/clouds/cloud_big_1", "effects/clouds/cloud_big_2", "effects/clouds/cloud_big_3" };

	public override void init()
	{
		base.init();
		add(new CloudAsset
		{
			id = "cloud_rain",
			color_hex = "#5D728C",
			drop_id = "rain",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_big,
			speed_max = 4f
		});
		add(new CloudAsset
		{
			id = "cloud_rage",
			color_hex = "#FF3030",
			drop_id = "rage",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_big,
			speed_max = 4f,
			considered_disaster = true,
			draw_light_area = true
		});
		add(new CloudAsset
		{
			id = "cloud_lightning",
			color_hex = "#445366",
			drop_id = "rain",
			cloud_action_1 = dropAction,
			cloud_action_2 = spawnLightning,
			path_sprites = _sprites_big,
			speed_max = 4f,
			considered_disaster = true
		});
		add(new CloudAsset
		{
			id = "cloud_acid",
			color_hex = "#17D41C",
			drop_id = "acid",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_big,
			speed_max = 4f,
			considered_disaster = true
		});
		add(new CloudAsset
		{
			id = "cloud_lava",
			color_hex = "#D17119",
			drop_id = "lava",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_big,
			speed_max = 3f,
			considered_disaster = true,
			draw_light_area = true
		});
		add(new CloudAsset
		{
			id = "cloud_fire",
			color_hex = "#D14219",
			drop_id = "fire",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_big,
			speed_max = 3f,
			considered_disaster = true,
			draw_light_area = true
		});
		add(new CloudAsset
		{
			id = "cloud_snow",
			color_hex = "#B8FFFA",
			drop_id = "snow",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_big,
			considered_disaster = true,
			speed_max = 4f
		});
		add(new CloudAsset
		{
			id = "cloud_ash",
			color_hex = "#C6B077",
			drop_id = "ash",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_big,
			considered_disaster = true,
			speed_max = 4f
		});
		add(new CloudAsset
		{
			id = "cloud_magic",
			color_hex = "#C976CC",
			drop_id = "magic_rain",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_small,
			considered_disaster = true,
			speed_max = 7f,
			draw_light_area = true
		});
		add(new CloudAsset
		{
			id = "cloud_normal",
			max_alpha = 0.5f,
			interval_action_1 = 2f,
			drop_id = "life_seed",
			cloud_action_1 = dropAction,
			path_sprites = _sprites_all,
			speed_min = 2f,
			normal_cloud = true
		});
	}

	public override void linkAssets()
	{
		base.linkAssets();
		using ListPool<Sprite> tSprites = new ListPool<Sprite>();
		foreach (CloudAsset tAsset in list)
		{
			if (tAsset.color_hex != null)
			{
				tAsset.color = Toolbox.makeColor(tAsset.color_hex);
			}
			tSprites.Clear();
			string[] path_sprites = tAsset.path_sprites;
			foreach (string tPath in path_sprites)
			{
				Sprite tSprite = SpriteTextureLoader.getSprite(tPath);
				if (tSprite == null)
				{
					BaseAssetLibrary.logAssetError("cloud sprite not found", tPath);
				}
				else
				{
					tSprites.Add(tSprite);
				}
			}
			tAsset.cached_sprites = tSprites.ToArray();
		}
	}

	public static void dropAction(Cloud pCloud)
	{
		if (!(pCloud.alive_time < 3f))
		{
			float tWidth = pCloud.effect_texture_width;
			float tHeight = pCloud.effect_texture_height;
			int posX = (int)pCloud.transform.localPosition.x;
			int posY = (int)pCloud.transform.localPosition.y;
			posX += (int)Randy.randomFloat(0f - tWidth, tWidth);
			posY += (int)Randy.randomFloat(0f - tHeight + pCloud.spriteShadow.offset.y, tHeight + pCloud.spriteShadow.offset.y);
			WorldTile tTile = World.world.GetTile(posX, posY);
			if (tTile != null)
			{
				World.world.drop_manager.spawn(tTile, pCloud.asset.drop_id, 10f, -1f, -1L);
			}
		}
	}

	public static void spawnLightning(Cloud pCloud)
	{
		if (!Randy.randomChance(0.01f))
		{
			return;
		}
		int posX = (int)pCloud.transform.localPosition.x;
		int posY = (int)pCloud.transform.localPosition.y;
		float tWidth = pCloud.effect_texture_width;
		float tHeight = pCloud.effect_texture_height;
		posX += (int)Randy.randomFloat(tWidth * 0.5f, tWidth);
		posY += (int)Randy.randomFloat(0f - tHeight + pCloud.spriteShadow.offset.y, tHeight + pCloud.spriteShadow.offset.y);
		WorldTile tTile = World.world.GetTile(posX, posY);
		if (tTile != null)
		{
			if (Randy.randomBool())
			{
				MapBox.spawnLightningMedium(tTile, 0.15f);
			}
			else
			{
				MapBox.spawnLightningSmall(tTile, 0.15f);
			}
		}
	}
}
