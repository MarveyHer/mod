using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HoveringIcon : MonoBehaviour
{
	private Vector3 _original_pos;

	private float _random_timer;

	public Image image;

	public float min = -2f;

	public float max = 2f;

	public float timer_mod = 1f;

	private Tweener _tweener;

	internal RectTransform rect;

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		image = GetComponent<Image>();
	}

	internal void clear()
	{
		_tweener.Kill();
	}

	internal void init()
	{
		_original_pos = base.transform.localPosition;
		_random_timer = Randy.randomFloat(1f * timer_mod, 1.5f * timer_mod);
		startAnimation();
	}

	private void OnDisable()
	{
		clear();
	}

	private void startAnimation()
	{
		_tweener.Kill();
		base.transform.localPosition = new Vector3(_original_pos.x, _original_pos.y += Randy.randomFloat(min, max));
		if (Randy.randomBool())
		{
			moveStageOne();
		}
		else
		{
			moveStageTwo();
		}
	}

	private void moveStageTwo()
	{
		_tweener = base.transform.DOLocalMove(_original_pos, _random_timer).SetEase(Ease.InOutQuad).OnComplete(moveStageOne);
	}

	private void moveStageOne()
	{
		Vector3 tVec = new Vector3(_original_pos.x, _original_pos.y, 1f);
		tVec.y += 3f;
		_tweener = base.transform.DOLocalMove(tVec, _random_timer).SetEase(Ease.InOutQuad).OnComplete(moveStageTwo);
	}
}
