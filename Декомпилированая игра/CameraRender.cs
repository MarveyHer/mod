using UnityEngine;

public class CameraRender : MonoBehaviour
{
	public Material PostProcessMaterial;

	public Camera BackgroundCamera;

	public Camera MainCamera;

	private RenderTexture mainRenderTexture;

	private void Start()
	{
		mainRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
		mainRenderTexture.Create();
		BackgroundCamera.targetTexture = mainRenderTexture;
		MainCamera.targetTexture = mainRenderTexture;
	}

	private void Update()
	{
	}

	private void OnPostRender()
	{
		Graphics.Blit(mainRenderTexture, PostProcessMaterial);
	}
}
