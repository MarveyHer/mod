using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ButtonExtensions
{
	public static void TriggerHover(this Button button)
	{
		if (Input.mousePresent)
		{
			EventTrigger tTriggerEnter = button.gameObject.GetComponent<EventTrigger>();
			if (tTriggerEnter == null)
			{
				tTriggerEnter = button.gameObject.AddComponent<EventTrigger>();
			}
			tTriggerEnter.OnPointerEnter(new PointerEventData(EventSystem.current));
		}
	}

	public static void OnHover(this Button button, UnityAction call)
	{
		if (Input.mousePresent)
		{
			EventTrigger tTriggerEnter = button.gameObject.GetComponent<EventTrigger>();
			if (tTriggerEnter == null)
			{
				tTriggerEnter = button.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry tPointerEnter = new EventTrigger.Entry();
			tPointerEnter.eventID = EventTriggerType.PointerEnter;
			tPointerEnter.callback.AddListener(delegate
			{
				call();
			});
			tTriggerEnter.triggers.Add(tPointerEnter);
		}
	}

	public static void OnHoverOut(this Button button, UnityAction call)
	{
		if (Input.mousePresent)
		{
			EventTrigger tTriggerEnter = button.gameObject.GetComponent<EventTrigger>();
			if (tTriggerEnter == null)
			{
				tTriggerEnter = button.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry tPointerExit = new EventTrigger.Entry();
			tPointerExit.eventID = EventTriggerType.PointerExit;
			tPointerExit.callback.AddListener(delegate
			{
				call();
			});
			tTriggerEnter.triggers.Add(tPointerExit);
		}
	}

	public static void OnHover(this Slider slider, UnityAction call)
	{
		if (Input.mousePresent)
		{
			EventTrigger tTriggerEnter = slider.gameObject.GetComponent<EventTrigger>();
			if (tTriggerEnter == null)
			{
				tTriggerEnter = slider.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry tPointerEnter = new EventTrigger.Entry();
			tPointerEnter.eventID = EventTriggerType.PointerEnter;
			tPointerEnter.callback.AddListener(delegate
			{
				call();
			});
			tTriggerEnter.triggers.Add(tPointerEnter);
		}
	}

	public static void OnHoverOut(this Slider slider, UnityAction call)
	{
		if (Input.mousePresent)
		{
			EventTrigger tTriggerEnter = slider.gameObject.GetComponent<EventTrigger>();
			if (tTriggerEnter == null)
			{
				tTriggerEnter = slider.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry tPointerExit = new EventTrigger.Entry();
			tPointerExit.eventID = EventTriggerType.PointerExit;
			tPointerExit.callback.AddListener(delegate
			{
				call();
			});
			tTriggerEnter.triggers.Add(tPointerExit);
		}
	}
}
