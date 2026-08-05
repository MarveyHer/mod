using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonSwitch : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public Action click_increase;

	public Action click_decrease;

	private Animator anim;

	private Vector3 defaultScale;

	private Vector3 clickedScale;

	private void Start()
	{
		anim = base.gameObject.GetComponent<Animator>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			click_decrease?.Invoke();
			SoundBox.click();
			newClickAnimation();
		}
		else
		{
			click_increase?.Invoke();
			SoundBox.click();
			newClickAnimation();
		}
	}

	private void Awake()
	{
		defaultScale = base.transform.localScale;
		clickedScale = defaultScale * 1.1f;
	}

	public void newClickAnimation()
	{
		base.transform.DOKill();
		base.transform.localScale = clickedScale;
		base.transform.DOScale(defaultScale, 0.3f).SetEase(Ease.InOutBack);
	}

	private void OnDestroy()
	{
		base.transform.DOKill();
	}
}
