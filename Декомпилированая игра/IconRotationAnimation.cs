using DG.Tweening;
using UnityEngine;

public class IconRotationAnimation : MonoBehaviour
{
	public float delay = 5f;

	public bool randomDelay;

	private Vector3 initScale;

	private Vector3 scaleTo;

	internal Tweener curTween;

	private void Awake()
	{
		initScale = base.transform.localScale;
		scaleTo = initScale * 1.1f;
		if (randomDelay)
		{
			delay = Randy.randomFloat(1f, 10f);
		}
	}

	private void checkDestroyTween()
	{
		if (curTween != null && curTween.active)
		{
			curTween.Complete(withCallbacks: false);
			curTween.Kill();
			curTween = null;
		}
	}

	private void rotate1()
	{
		if (!(base.transform == null))
		{
			curTween = base.transform.DOScale(scaleTo, 0.3f).SetDelay(delay).SetEase(Ease.InOutBack)
				.OnComplete(rotate2);
		}
	}

	private void rotate2()
	{
		if (!(base.transform == null))
		{
			if (randomDelay)
			{
				delay = Randy.randomFloat(1f, 10f);
			}
			curTween = base.transform.DOScale(initScale, 0.3f).SetDelay(0f).SetEase(Ease.InOutBack)
				.OnComplete(rotate1);
		}
	}

	private void OnEnable()
	{
		checkDestroyTween();
		rotate1();
	}

	private void OnDisable()
	{
		checkDestroyTween();
	}

	private void OnDestroy()
	{
		checkDestroyTween();
	}
}
