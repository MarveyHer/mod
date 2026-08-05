using UnityEngine;

public class ExplosionFlash : BaseEffect
{
	private float speed;

	public void start(Vector3 pVector, float pRadius, float pSpeed = 1f)
	{
		speed = pSpeed;
		base.transform.position = new Vector3(pVector.x, pVector.y);
		setScale(0.005f * pRadius);
		setAlpha(1f);
	}

	public override void update(float pElapsed)
	{
		setAlpha(alpha - pElapsed * speed * 0.5f);
		setScale(scale += pElapsed * speed * 0.1f);
		if (alpha <= 0f)
		{
			kill();
		}
	}
}
