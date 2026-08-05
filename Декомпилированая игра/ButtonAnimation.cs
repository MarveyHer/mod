using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
	public static float scaleTime = 0.1f;

	private IEnumerator newAnim()
	{
		base.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		yield return CoroutineHelper.wait_for_0_01_s;
		base.gameObject.transform.DOScale(1f, scaleTime).SetEase(Ease.InOutBack);
	}

	public void clickAnimation()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(newAnim());
		}
	}
}
