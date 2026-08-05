using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CitySortableElement : CityElement, ILayoutController
{
	private RectTransform _rect;

	private List<RectTransform> _rect_children = new List<RectTransform>();

	protected override void Awake()
	{
		_rect = GetComponent<RectTransform>();
		base.Awake();
	}

	protected virtual void onListChange()
	{
	}

	public void SetLayoutVertical()
	{
		if (_rect == null)
		{
			return;
		}
		using ListPool<RectTransform> tChildren = _rect.getLayoutChildren();
		if (!tChildren.SequenceEqual(_rect_children))
		{
			_rect_children.Clear();
			_rect_children.AddRange(tChildren);
			onListChange();
		}
	}

	public void SetLayoutHorizontal()
	{
	}
}
