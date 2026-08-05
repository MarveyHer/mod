using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CrabLimbItem : MonoBehaviour
{
	public CrabLimb crabLimb;

	public Sprite high_hp;

	public Sprite med_hp;

	public Sprite low_hp;

	internal SpriteRenderer _sprite_renderer;

	private Color _shade;

	private Color _dmg = new Color(1f, 0f, 0f, 1f);

	private void Awake()
	{
		_sprite_renderer = GetComponent<SpriteRenderer>();
		_sprite_renderer.sprite = high_hp;
		_shade = _sprite_renderer.color;
	}

	internal void stateChange(CrabLimbState pState)
	{
		switch (pState)
		{
		case CrabLimbState.HighHP:
			_sprite_renderer.sprite = high_hp;
			break;
		case CrabLimbState.MedHP:
			_sprite_renderer.sprite = med_hp;
			break;
		case CrabLimbState.LowHP:
			_sprite_renderer.sprite = low_hp;
			break;
		}
		_sprite_renderer.color = _dmg;
	}

	internal void flicker(float pProgress)
	{
		_sprite_renderer.color = Color.Lerp(_dmg, _shade, pProgress);
	}
}
