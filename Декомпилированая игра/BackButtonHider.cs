using UnityEngine;

internal class BackButtonHider : MonoBehaviour
{
	private void OnEnable()
	{
		if (WindowHistory.hasHistory())
		{
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
