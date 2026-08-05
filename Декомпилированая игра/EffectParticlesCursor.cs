using UnityEngine;

public class EffectParticlesCursor : MonoBehaviour
{
	private SpriteAnimationSimple _sprite_animation;

	private float _speed = 50f;

	private void Awake()
	{
		_sprite_animation = GetComponent<SpriteAnimationSimple>();
	}

	public void launch()
	{
		_sprite_animation.resetAnim();
		_speed = 50f + Randy.randomFloat(-10f, 10f);
	}

	public void update()
	{
		_sprite_animation.update(Time.deltaTime);
		base.transform.position += new Vector3(0f, _speed * Time.deltaTime, 0f);
	}

	public SpriteAnimationSimple getAnimation()
	{
		return _sprite_animation;
	}

	public void setFrames(Sprite[] pFrames)
	{
		_sprite_animation.setFrames(pFrames);
	}
}
