using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeaderContainer : MonoBehaviour, ILayoutController
{
	public RectTransform header_transform;

	public RectTransform content_transform;

	public RectTransform runes_container;

	public VerticalLayoutGroup content;

	private VerticalLayoutGroup _vertical_layout_group;

	private LayoutElement _layout_element;

	private int _default_top_padding;

	private RectOffset _default_padding;

	private void Awake()
	{
		if (content == null)
		{
			content = content_transform.GetComponent<VerticalLayoutGroup>();
		}
		_vertical_layout_group = GetComponent<VerticalLayoutGroup>();
		_default_padding = _vertical_layout_group.padding;
		_layout_element = GetComponent<LayoutElement>();
		_default_top_padding = content.padding.top;
	}

	public void SetLayoutVertical()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		int tFinalHeight = _default_top_padding;
		if (!hasAnyElementActive())
		{
			if (_layout_element.preferredHeight != 0f)
			{
				_vertical_layout_group.padding = new RectOffset(0, 0, 0, 0);
				_layout_element.preferredHeight = 0f;
				LayoutRebuilder.ForceRebuildLayoutImmediate(header_transform);
			}
		}
		else
		{
			if (_layout_element.preferredHeight >= 0f)
			{
				_vertical_layout_group.padding = _default_padding;
				_layout_element.preferredHeight = -1f;
				LayoutRebuilder.ForceRebuildLayoutImmediate(header_transform);
			}
			tFinalHeight += (int)header_transform.rect.height;
		}
		if (content.padding.top != tFinalHeight)
		{
			content.padding.top = tFinalHeight;
			LayoutRebuilder.ForceRebuildLayoutImmediate(content_transform);
			StartCoroutine(toggleRunes());
		}
	}

	public void SetLayoutHorizontal()
	{
	}

	private IEnumerator toggleRunes()
	{
		yield return null;
		bool tHasAnyElementActive = hasAnyElementActive();
		runes_container.gameObject.SetActive(tHasAnyElementActive);
		if (tHasAnyElementActive)
		{
			runes_container.localPosition = new Vector2(runes_container.localPosition.x, 0f - header_transform.rect.height);
		}
	}

	private bool hasAnyElementActive()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				return true;
			}
		}
		return false;
	}
}
