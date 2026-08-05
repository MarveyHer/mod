using FMOD.Studio;
using UnityEngine;

public class GroupSpriteObject : MonoBehaviour
{
	internal EventInstance fmod_instance;

	internal Transform m_transform;

	internal SpriteRenderer sprite_renderer;

	private bool _has_sprite_renderer;

	private Vector2 _last_pos_v2 = new Vector2(-1f, -1f);

	private Vector2 _last_scale_v2 = new Vector2(-1f, -1f);

	private Vector3 _last_pos_v3 = new Vector3(-1f, -1f, -1f);

	private Vector3 _last_scale_v3 = new Vector3(-1f, -1f, -1f);

	private Vector3 _last_angles_v3 = new Vector3(-1f, -1f, -1f);

	private Color _last_color;

	private bool _last_flip_x;

	private int _last_sprite_hash_code = -1;

	private int _last_sprite_material = -1;

	public int last_id;

	public bool has_sprite_renderer => _has_sprite_renderer;

	private void Awake()
	{
		create();
	}

	protected void create()
	{
		m_transform = base.gameObject.transform;
		sprite_renderer = base.gameObject.GetComponent<SpriteRenderer>();
		if (sprite_renderer != null)
		{
			_has_sprite_renderer = true;
		}
	}

	public void checkRotation(Vector3 pPos, BaseSimObject pSimObject, float pZ)
	{
		pPos.z = pZ;
		Vector3 tPivot = default(Vector3);
		ref Vector3 tAngle = ref pSimObject.current_rotation;
		if (tAngle.y != 0f || tAngle.z != 0f)
		{
			tPivot.Set(pSimObject.cur_transform_position.x, pSimObject.cur_transform_position.y, 0f);
			pPos = Toolbox.RotatePointAroundPivot(ref pPos, ref tPivot, ref tAngle);
			pPos.z = pZ;
		}
		setPosOnly(ref pPos);
		setLocalEulerAngles(pSimObject.current_rotation);
	}

	public void setPosOnly(Vector2 pPosition)
	{
		if (_last_pos_v3.x != pPosition.x || _last_pos_v3.y != pPosition.y)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
	}

	public void setPosOnly(ref Vector2 pPosition)
	{
		if (_last_pos_v3.x != pPosition.x || _last_pos_v3.y != pPosition.y || _last_pos_v3.z != 0f)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
	}

	public void setPosOnly(ref Vector3 pPosition)
	{
		if (_last_pos_v3.x != pPosition.x || _last_pos_v3.y != pPosition.y || _last_pos_v3.z != pPosition.z)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
	}

	public void setRotation(ref Vector3 pVec)
	{
		if (_last_angles_v3.y != pVec.y || _last_angles_v3.z != pVec.z)
		{
			_last_angles_v3 = pVec;
			m_transform.eulerAngles = pVec;
		}
	}

	public void setLocalEulerAngles(Vector3 pVec)
	{
		if (_last_angles_v3.y != pVec.y || _last_angles_v3.z != pVec.z)
		{
			_last_angles_v3 = pVec;
			m_transform.localEulerAngles = pVec;
		}
	}

	public void setSprite(Sprite pSprite)
	{
		int tHashCode = pSprite.GetHashCode();
		if (_last_sprite_hash_code != tHashCode)
		{
			sprite_renderer.sprite = pSprite;
			_last_sprite_hash_code = tHashCode;
		}
	}

	public void setFlipX(bool pFlipX)
	{
		if (_last_flip_x != pFlipX)
		{
			_last_flip_x = pFlipX;
			sprite_renderer.flipX = pFlipX;
		}
	}

	public void setSharedMat(Material pMaterial)
	{
		int tHashCode = pMaterial.GetHashCode();
		if (_last_sprite_material != tHashCode)
		{
			sprite_renderer.sharedMaterial = pMaterial;
			_last_sprite_material = tHashCode;
		}
	}

	public void setColor(ref Color pColor)
	{
		if (_last_color.r != pColor.r || _last_color.g != pColor.g || _last_color.b != pColor.b || _last_color.a != pColor.a)
		{
			sprite_renderer.color = pColor;
			_last_color = pColor;
		}
	}

	public void setScale(float pScale)
	{
		if (_last_scale_v3.y != pScale)
		{
			_last_scale_v2 = new Vector2(pScale, pScale);
			_last_scale_v3 = _last_scale_v2;
			m_transform.localScale = _last_scale_v3;
		}
	}

	public void setScale(float pScaleX, float pScaleY)
	{
		if (_last_scale_v2.y != pScaleY || _last_scale_v2.x != pScaleX)
		{
			_last_scale_v2 = new Vector2(pScaleX, pScaleY);
			_last_scale_v3 = _last_scale_v2;
			m_transform.localScale = _last_scale_v3;
		}
	}

	public void setScale(ref Vector3 pScaleVec)
	{
		if (_last_scale_v3 != pScaleVec)
		{
			_last_scale_v3 = pScaleVec;
			m_transform.localScale = pScaleVec;
		}
	}

	public void set(ref Vector2 pPosition, ref Vector3 pScale)
	{
		if (_last_pos_v2.x != pPosition.x || _last_pos_v2.y != pPosition.y)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
		if (_last_scale_v2.y != pScale.y || _last_scale_v2.x != pScale.x)
		{
			_last_scale_v2 = pScale;
			_last_scale_v3 = _last_scale_v2;
			m_transform.localScale = pScale;
		}
	}

	public void set(ref Vector2 pPosition, float pScale)
	{
		if (_last_pos_v3.x != pPosition.x || _last_pos_v3.y != pPosition.y)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
		if (_last_scale_v2.x != pScale)
		{
			setScale(pScale);
		}
	}

	public void set(ref Vector3 pPosition, float pScale)
	{
		if (_last_pos_v3.x != pPosition.x || _last_pos_v3.y != pPosition.y || _last_pos_v3.z != pPosition.z)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
		if (_last_scale_v2.x != pScale)
		{
			setScale(pScale);
		}
	}

	public void set(ref Vector3 pPosition, ref Vector2 pScale)
	{
		if (_last_pos_v3.x != pPosition.x || _last_pos_v3.y != pPosition.y || _last_pos_v3.z != pPosition.z)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
		if (_last_scale_v2.y != pScale.y || _last_scale_v2.x != pScale.x)
		{
			_last_scale_v2 = pScale;
			_last_scale_v3 = _last_scale_v2;
			m_transform.localScale = pScale;
		}
	}

	public void set(ref Vector3 pPosition, ref Vector3 pScale)
	{
		if (_last_pos_v3.x != pPosition.x || _last_pos_v3.y != pPosition.y)
		{
			_last_pos_v2 = pPosition;
			_last_pos_v3 = pPosition;
			m_transform.localPosition = _last_pos_v3;
		}
		if (_last_scale_v2.y != pScale.y || _last_scale_v2.x != pScale.x)
		{
			_last_scale_v2 = pScale;
			_last_scale_v3 = _last_scale_v2;
			m_transform.localScale = pScale;
		}
	}
}
