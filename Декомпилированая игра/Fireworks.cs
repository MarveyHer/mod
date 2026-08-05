using UnityEngine;

public class Fireworks : BaseEffect
{
	internal override void spawnOnTile(WorldTile pTile)
	{
		float tScale = Randy.randomFloat(0.3f, 1f);
		prepare(pTile, tScale);
		if (Randy.randomBool())
		{
			loadSprites("effects/fireworks1");
		}
		else
		{
			loadSprites("effects/fireworks2");
		}
		sprite_renderer.flipX = Randy.randomBool();
		Color tColor = new Color
		{
			a = 1f,
			r = Randy.randomFloat(0f, 1f),
			b = Randy.randomFloat(0f, 1f),
			g = Randy.randomFloat(0f, 1f)
		};
		sprite_renderer.color = tColor;
		float tRotation = Randy.randomFloat(-15f, 15f);
		base.transform.localEulerAngles = new Vector3(0f, 0f, tRotation);
	}

	private void loadSprites(string pPath)
	{
		Sprite[] tSprites = SpriteTextureLoader.getSpriteList(pPath);
		sprite_animation.frames = tSprites;
	}
}
