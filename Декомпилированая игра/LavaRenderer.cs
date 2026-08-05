using UnityEngine;
using UnityEngine.Rendering;

public class LavaRenderer : MonoBehaviour
{
	public Camera curCamera;

	public Camera targetCamera;

	private RenderTexture renderTexture;

	private void Start()
	{
		renderTexture = new RenderTexture(Screen.width, Screen.height, 8, RenderTextureFormat.ARGB32);
		renderTexture.dimension = TextureDimension.Tex2D;
		renderTexture.antiAliasing = 1;
		renderTexture.anisoLevel = 0;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		curCamera.targetTexture = renderTexture;
	}

	private void OnPreRender()
	{
		targetCamera.targetTexture = renderTexture;
	}

	private void OnPostRender()
	{
		targetCamera.targetTexture = null;
		Graphics.DrawTexture(new Rect(0f, 0f, Screen.width / 2, Screen.height / 2), renderTexture, null);
	}
}
