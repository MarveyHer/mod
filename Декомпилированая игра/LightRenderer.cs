using UnityEngine;
using UnityEngine.UI;

public class LightRenderer : MonoBehaviour
{
	public static LightRenderer instance;

	public Camera camera;

	public EffectsCamera effectsCamera;

	private RawImage _rawImage;

	private void Awake()
	{
		instance = this;
		_rawImage = GetComponent<RawImage>();
	}

	public void update(float pElapsed)
	{
		_rawImage.texture = effectsCamera.renderTexture;
		Color tColor = World.world_era.light_color;
		tColor.a = World.world.era_manager.getNightMod() * 0.6f;
		_rawImage.color = tColor;
	}
}
