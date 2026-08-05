using UnityEngine;

public class EffectHearts : BaseEffect
{
	internal override void spawnOnTile(WorldTile pTile)
	{
		float tScale = Randy.randomFloat(0.3f, 0.5f);
		prepare(pTile, tScale);
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		float tPosX = base.transform.position.x;
		float tPosY = base.transform.position.y + pElapsed * 3f / Config.time_scale_asset.multiplier;
		base.transform.position = new Vector3(tPosX, tPosY);
	}
}
