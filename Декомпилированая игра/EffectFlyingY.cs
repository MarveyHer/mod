using UnityEngine;

public class EffectFlyingY : BaseEffect
{
	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		Vector3 position = base.transform.position;
		float tPosX = position.x;
		float tPosY = position.y + pElapsed * 1f / Config.time_scale_asset.multiplier;
		base.transform.position = new Vector3(tPosX, tPosY);
	}
}
