using UnityEngine;

public class CircleIconShaderMod : MonoBehaviour
{
	public Material prefab_radial_fill;

	private Material _instance_material;

	public SpriteRenderer sprite_renderer_with_mat;

	private void Awake()
	{
		_instance_material = new Material(prefab_radial_fill);
		sprite_renderer_with_mat.material = _instance_material;
	}

	public void setShaderVal(float pVal)
	{
		if (!(sprite_renderer_with_mat == null))
		{
			float fillAmount = Mathf.PingPong(pVal, 1f);
			_instance_material.SetFloat("_FillAmount", fillAmount);
		}
	}
}
