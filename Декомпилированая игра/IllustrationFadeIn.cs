using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IllustrationFadeIn : MonoBehaviour
{
	public float scale_start = 1.5f;

	public float scale_end = 1f;

	public float duration = 1f;

	public Ease ease_type = Ease.OutQuart;

	private void Awake()
	{
		if (!TryGetComponent<Button>(out var tButton))
		{
			tButton = base.gameObject.AddComponent<Button>();
		}
		tButton.onClick.AddListener(onCLick);
		GetComponent<Image>().raycastTarget = true;
	}

	private void OnEnable()
	{
		startTween();
	}

	public void startTween()
	{
		Vector3 tStartScale = new Vector3(scale_start, scale_start, scale_start);
		Vector3 tEndScale = new Vector3(scale_end, scale_end, scale_end);
		base.transform.DOKill();
		base.transform.DOScale(tEndScale, duration).From(tStartScale).SetEase(ease_type);
	}

	public void onCLick()
	{
		startTween();
	}
}
