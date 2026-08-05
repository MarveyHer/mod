using UnityEngine;

public class LoadingScreenOut : MonoBehaviour
{
	public CanvasGroup canvasGroup;

	private void Update()
	{
		canvasGroup.alpha -= Time.deltaTime * 2f;
		if (canvasGroup.alpha <= 0f)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
