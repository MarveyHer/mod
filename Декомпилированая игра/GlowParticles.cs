using UnityEngine;

public class GlowParticles : MonoBehaviour
{
	private float cooldown;

	public ParticleSystem particles;

	private void Awake()
	{
		particles = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
		}
	}

	public void spawn(float pX, float pY, bool pRemoveCooldown = false)
	{
		if (base.enabled && particles.particleCount <= 50 && MapBox.isRenderGameplay())
		{
			if (pRemoveCooldown)
			{
				cooldown = 0f;
			}
			if (!(cooldown > 0f))
			{
				cooldown = 0.2f + Randy.randomFloat(0f, 0.3f);
				ParticleSystem.EmitParams tParam = new ParticleSystem.EmitParams
				{
					position = new Vector3(pX, pY)
				};
				particles.Emit(tParam, 1);
			}
		}
	}

	public void spawn(Vector3 pPos)
	{
		spawn(pPos.x, pPos.y);
	}

	public void clear()
	{
		particles.Clear();
	}
}
