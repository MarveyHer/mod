using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragOrderContainer : MonoBehaviour
{
	public enum SnapAxis
	{
		Horizontal,
		Vertical,
		No
	}

	internal static float drag_delay = 0.25f;

	public MonoBehaviour scroll_rect;

	public SnapAxis snapping_axis = SnapAxis.No;

	public bool limit_moving;

	public bool delay_before_drag = true;

	public bool debug;

	public Action on_order_changed;

	internal DragOrderElement dragging_element;

	internal bool is_anything_dragging;

	internal RectTransform rect_transform;

	internal LayoutGroup grid_layout;

	internal LayoutElement layout_element;

	private List<DragOrderElement> _elements = new List<DragOrderElement>();

	private Dictionary<int, DragOrderElement> _elements_dict = new Dictionary<int, DragOrderElement>();

	private Dictionary<int, Vector2> _children_positions = new Dictionary<int, Vector2>();

	private Dictionary<int, Rect> _children_rects = new Dictionary<int, Rect>();

	private Transform _to_ignore_in_intersection;

	private int _previous_elements_count;

	private bool _marked_for_update;

	private int _marked_for_update_on_frame;

	private bool _initialized;

	private void Awake()
	{
		if (scroll_rect == null)
		{
			scroll_rect = GetComponentInParent<ScrollRectExtended>();
		}
		if (scroll_rect == null)
		{
			scroll_rect = GetComponentInParent<ScrollRect>();
		}
		rect_transform = GetComponent<RectTransform>();
		grid_layout = GetComponent<LayoutGroup>();
		layout_element = base.gameObject.AddOrGetComponent<LayoutElement>();
		layout_element.enabled = false;
	}

	private void markForUpdate()
	{
		_marked_for_update = true;
		_marked_for_update_on_frame = Time.frameCount;
	}

	private void OnApplicationFocus(bool pHasFocus)
	{
		if (!pHasFocus)
		{
			disable();
		}
	}

	private void OnEnable()
	{
		markForUpdate();
		ScrollWindow.addCallbackShow(onWindowClose);
		ScrollWindow.addCallbackHide(onWindowClose);
	}

	private void OnDisable()
	{
		disable();
		ScrollWindow.removeCallbackShow(onWindowClose);
		ScrollWindow.removeCallbackHide(onWindowClose);
	}

	private void onWindowClose(string pId)
	{
		disable();
	}

	private void disable()
	{
		grid_layout.enabled = true;
		LayoutRebuilder.MarkLayoutForRebuild(rect_transform);
		if (dragging_element != null)
		{
			dragging_element.stopDrag();
		}
		foreach (DragOrderElement tElement in _elements)
		{
			if (!tElement.is_target_reached)
			{
				tElement.is_target_reached = true;
				tElement.unsetOnTop();
			}
		}
	}

	private void Update()
	{
		if (_marked_for_update && _marked_for_update_on_frame != Time.frameCount)
		{
			_marked_for_update = false;
			updateChildrenData();
		}
		checkIntersections();
		updatePositions();
	}

	private void OnDrawGizmos()
	{
		if (!debug)
		{
			return;
		}
		foreach (Rect value in _children_rects.Values)
		{
			Rect tLocalRect = value;
			tLocalRect.min = rect_transform.TransformPoint(tLocalRect.min);
			tLocalRect.max = rect_transform.TransformPoint(tLocalRect.max);
			drawRect(tLocalRect, Color.green);
		}
	}

	private void checkIntersections()
	{
		if (is_anything_dragging)
		{
			DragOrderElement tElement = getIntersectedWith();
			if (tElement == null)
			{
				_to_ignore_in_intersection = null;
			}
			else if (!(tElement.main_transform == _to_ignore_in_intersection))
			{
				_to_ignore_in_intersection = tElement.main_transform;
				switchElements(dragging_element, tElement);
				on_order_changed?.Invoke();
			}
		}
	}

	private DragOrderElement getIntersectedWith()
	{
		int tDraggingIndex = dragging_element.order_index;
		Vector2 tDraggingPosition = dragging_element.main_transform.localPosition;
		Debug.DrawLine(rect_transform.TransformPoint(_children_rects[tDraggingIndex].center), rect_transform.TransformPoint(tDraggingPosition));
		if (snapping_axis != SnapAxis.No)
		{
			int tFirstIndex = 0;
			int tLastIndex = _elements.Count - 1;
			Rect tFirstChildRect = _children_rects[tFirstIndex];
			Rect tLastChildRect = _children_rects[tLastIndex];
			if (snapping_axis == SnapAxis.Horizontal)
			{
				if (tDraggingPosition.x <= tFirstChildRect.xMax)
				{
					return _elements_dict[tFirstIndex];
				}
				if (tDraggingPosition.x >= tLastChildRect.xMin)
				{
					return _elements_dict[tLastIndex];
				}
			}
			if (snapping_axis == SnapAxis.Vertical)
			{
				if (tDraggingPosition.y >= tFirstChildRect.yMax)
				{
					return _elements_dict[tFirstIndex];
				}
				if (tDraggingPosition.y <= tLastChildRect.yMin)
				{
					return _elements_dict[tLastIndex];
				}
			}
		}
		for (int tOrderIndex = 0; tOrderIndex < _elements.Count; tOrderIndex++)
		{
			if (tOrderIndex != tDraggingIndex && _children_rects[tOrderIndex].Contains(tDraggingPosition))
			{
				return _elements_dict[tOrderIndex];
			}
		}
		return null;
	}

	private void updatePositions()
	{
		if (grid_layout.enabled)
		{
			return;
		}
		bool tAnyAnimPlaying = false;
		foreach (DragOrderElement tElem in _elements)
		{
			if (!(tElem == dragging_element))
			{
				tElem.updatePosition();
				if (!tElem.is_target_reached)
				{
					tAnyAnimPlaying = true;
				}
			}
		}
		if (!tAnyAnimPlaying && !is_anything_dragging)
		{
			grid_layout.enabled = true;
		}
	}

	public void updateChildrenData()
	{
		layout_element.minHeight = rect_transform.rect.height;
		layout_element.minWidth = rect_transform.rect.width;
		_elements.Clear();
		_elements_dict.Clear();
		_children_positions.Clear();
		_children_rects.Clear();
		DragOrderElement[] tElems = rect_transform.GetComponentsInChildren<DragOrderElement>();
		int tOrderIndex = 0;
		DragOrderElement[] array = tElems;
		foreach (DragOrderElement tElem in array)
		{
			Vector2 tPosition = ((!tElem.is_target_reached && _previous_elements_count == tElems.Length) ? tElem.current_destination : ((Vector2)tElem.main_transform.localPosition));
			tElem.order_index = tOrderIndex;
			_elements.Add(tElem);
			_elements_dict.Add(tOrderIndex, tElem);
			_children_positions.Add(tOrderIndex, tPosition);
			Rect tRect = tElem.getRect();
			_children_rects.Add(tOrderIndex, tRect);
			tElem.current_destination = tPosition;
			tElem.unsetOnTop();
			tOrderIndex++;
		}
		_previous_elements_count = tElems.Length;
	}

	private void switchElements(DragOrderElement pFirst, DragOrderElement pSecond)
	{
		pFirst.main_transform.SetSiblingIndex(pSecond.main_transform.GetSiblingIndex());
		int tFirstIndex = pFirst.order_index;
		int tSecondIndex = pSecond.order_index;
		bool tIsAscending = tFirstIndex > tSecondIndex;
		pFirst.order_index = tSecondIndex;
		_elements.Sort((DragOrderElement e1, DragOrderElement e2) => sort(e1, e2, tIsAscending));
		int tIndexToCompare = pFirst.order_index;
		foreach (DragOrderElement tElement in _elements)
		{
			if (!(tElement == pFirst) && (!tIsAscending || tElement.order_index >= tIndexToCompare) && (tIsAscending || tElement.order_index <= tIndexToCompare) && tElement.order_index == tIndexToCompare)
			{
				tElement.order_index += (tIsAscending ? 1 : (-1));
				tIndexToCompare = tElement.order_index;
			}
		}
		foreach (DragOrderElement tElement2 in _elements)
		{
			_elements_dict[tElement2.order_index] = tElement2;
		}
	}

	public Vector3 getChildPosition(int pIndex)
	{
		return _children_positions[pIndex];
	}

	private int sort(DragOrderElement pFirst, DragOrderElement pSecond, bool pIsAscending)
	{
		return pFirst.order_index.CompareTo(pSecond.order_index) * (pIsAscending ? 1 : (-1));
	}

	private static void drawRect(Rect pRect, Color pColor)
	{
		Vector3 tMin = pRect.min;
		Vector3 tMax = pRect.max;
		Debug.DrawLine(tMin, new Vector3(tMin.x, tMax.y), pColor);
		Debug.DrawLine(new Vector3(tMin.x, tMax.y), tMax, pColor);
		Debug.DrawLine(tMax, new Vector3(tMax.x, tMin.y), pColor);
		Debug.DrawLine(tMin, new Vector3(tMax.x, tMin.y), pColor);
	}
}
