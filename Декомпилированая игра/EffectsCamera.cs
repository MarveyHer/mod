using UnityEngine;

public class EffectsCamera : MonoBehaviour
{
	private Camera _mainCamera;

	private Camera _effectsCamera;

	internal RenderTexture renderTexture;

	private void Awake()
	{
		_effectsCamera = GetComponent<Camera>();
	}

	private void Start()
	{
		_mainCamera = World.world.camera;
	}

	private void LateUpdate()
	{
		_effectsCamera.orthographicSize = _mainCamera.orthographicSize;
		int tWidth = Screen.width / 3;
		int tHeight = Screen.height / 3;
		if (renderTexture == null || renderTexture.width != tWidth || renderTexture.height != tHeight)
		{
			renderTexture = new RenderTexture(tWidth, tHeight, 0);
			renderTexture.filterMode = FilterMode.Point;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			_effectsCamera.targetTexture = renderTexture;
		}
	}
}
