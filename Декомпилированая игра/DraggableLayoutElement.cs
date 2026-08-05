using System;
using System.Collections.Generic;
using LayoutGroupExt;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableLayoutElement : MonoBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, ILayoutIgnorer, IDraggable
{
	public const float TOUCH_DELAY = 0.2f;

	[SerializeField]
	private bool _spawn_particles_on_drag = true;

	private RectTransform _rect;

	private CanvasGroup _canvas_group;

	private LayoutGroupExtended _parent_layout;

	private RectTransform _parent;

	private Rect _cached_parent_rect;

	private Vector3 _cached_parent_position;

	[SerializeField]
	private Transform _attach_parent;

	[SerializeField]
	private bool _touch_drag_delay;

	private DraggableLayoutElement _drag_object;

	private int _target_index = -1;

	private List<MonoBehaviour> _toggle_elements = new List<MonoBehaviour>(3);

	private static bool _any_dragging;

	private bool? _dragging_cache;

	[SerializeField]
	private bool _drag_only_over_parent = true;

	internal Action<DraggableLayoutElement> start_being_dragged;

	private float _drag_timer_started_at;

	public bool spawn_particles_on_drag => _spawn_particles_on_drag;

	public bool ignoreLayout { get; set; }

	private List<RectTransform> _siblings => _parent_layout.m_Children;

	private Vector2[] _sibling_positions => _parent_layout.m_Positions;

	private void Start()
	{
		_rect = GetComponent<RectTransform>();
		_canvas_group = GetComponent<CanvasGroup>();
		_parent = base.transform.parent.GetComponent<RectTransform>();
		_parent_layout = _parent.GetComponent<LayoutGroupExtended>();
		if (!(_parent_layout == null))
		{
			addToggleComponent<ScrollableButton>();
			addToggleComponent<Button>();
			addToggleComponent<TipButton>();
			if (_attach_parent == null)
			{
				_attach_parent = World.world.drag_parent;
			}
		}
	}

	private void OnEnable()
	{
		_cached_parent_position = new Vector3(-1000f, -1000f, -1000f);
		_target_index = -1;
	}

	public void KillDrag()
	{
		OnDisable();
	}

	private void OnDisable()
	{
		if (_any_dragging && !(_drag_object == null))
		{
			OnEndDrag(new PointerEventData(EventSystem.current));
		}
	}

	public void OnInitializePotentialDrag(PointerEventData pEventData)
	{
		if (_touch_drag_delay)
		{
			_drag_timer_started_at = Time.time;
		}
	}

	public void OnBeginDrag(PointerEventData pEventData)
	{
		if (!Config.isDraggingItem() && !isTouchDragDelayed() && !_any_dragging)
		{
			_any_dragging = true;
			_drag_object = UnityEngine.Object.Instantiate(this, _attach_parent, worldPositionStays: true);
			_drag_object.transform.position = pEventData.position;
			_drag_object.ignoreLayout = true;
			_drag_object.start_being_dragged?.Invoke(this);
			_canvas_group.alpha = 0.2f;
			Config.setDraggingObject(_drag_object);
		}
	}

	public void OnDrag(PointerEventData pEventData)
	{
		if (!isTouchDragDelayed() && _any_dragging && Config.isDraggingObject(_drag_object))
		{
			_drag_object.transform.position = pEventData.position;
			if (isOverParent(pEventData.position))
			{
				findTarget();
			}
		}
	}

	public void OnEndDrag(PointerEventData pEventData)
	{
		ScrollRectExtended.SendMessageToAll("OnEndDrag", pEventData);
		if (_any_dragging && Config.isDraggingObject(_drag_object))
		{
			Config.clearDraggingObject();
			_any_dragging = false;
			_drag_timer_started_at = 0f;
			UnityEngine.Object.Destroy(_drag_object.gameObject);
			_canvas_group.alpha = 1f;
		}
	}

	public void Update()
	{
		if (_dragging_cache != _any_dragging)
		{
			_dragging_cache = _any_dragging;
			_canvas_group.interactable = !_any_dragging;
			_canvas_group.blocksRaycasts = !_any_dragging;
			foreach (MonoBehaviour tToggleElement in _toggle_elements)
			{
				if (tToggleElement is Selectable)
				{
					(tToggleElement as Selectable).interactable = !_any_dragging;
				}
				else
				{
					tToggleElement.enabled = !_any_dragging;
				}
			}
		}
		moveToTarget();
	}

	internal void lockToParent(bool pLock = true)
	{
		_drag_only_over_parent = pLock;
	}

	internal void setDragParent(Transform pParent)
	{
		_attach_parent = pParent;
	}

	private void moveToTarget()
	{
		if (_target_index < 0)
		{
			return;
		}
		int tMyIndex = _siblings.IndexOf(_rect);
		int tTargetIndex = _target_index;
		using ListPool<int> tNeighbours = getNeighbours(tMyIndex);
		if (!tNeighbours.Contains(tTargetIndex))
		{
			tTargetIndex = findClosestNeighbour(tTargetIndex, tNeighbours);
		}
		swapSiblings(tMyIndex, tTargetIndex);
		if (tTargetIndex == _target_index)
		{
			_target_index = -1;
		}
	}

	private void recalcParent()
	{
		if (!(_cached_parent_position == _parent.position))
		{
			_cached_parent_position = _parent.position;
			_cached_parent_rect = _parent.GetWorldRect();
			float tMarginWidth = _rect.rect.width * 10f;
			float tMarginHeight = _rect.rect.height * 10f;
			_cached_parent_rect.x -= tMarginWidth;
			_cached_parent_rect.y -= tMarginHeight;
			_cached_parent_rect.width += tMarginWidth * 2f;
			_cached_parent_rect.height += tMarginHeight * 2f;
		}
	}

	private void findTarget()
	{
		if (_drag_object == null)
		{
			return;
		}
		Vector3 tPosition = _drag_object.transform.position;
		float tClosest = float.MaxValue;
		float tMyDist = float.MaxValue;
		int tTargetIndex = -1;
		int tMyIndex = -1;
		for (int i = 0; i < _sibling_positions.Length; i++)
		{
			float tDistance = Vector2.Distance(_sibling_positions[i], tPosition);
			if (_siblings[i] == _rect)
			{
				tMyIndex = i;
				tMyDist = tDistance;
			}
			if (tDistance < tClosest)
			{
				tClosest = tDistance;
				tTargetIndex = i;
			}
		}
		if (tTargetIndex != tMyIndex && !Mathf.Approximately(tMyDist, tClosest))
		{
			_target_index = tTargetIndex;
		}
	}

	private bool isOverParent(Vector2 pPosition)
	{
		recalcParent();
		if (_cached_parent_rect.Contains(pPosition))
		{
			return true;
		}
		return false;
	}

	private void swapSiblings(int pStartIndex, int pTargetIndex)
	{
		if (pStartIndex < _siblings.Count && pTargetIndex < _siblings.Count)
		{
			int pStartSiblingIndex = _siblings[pStartIndex].transform.GetSiblingIndex();
			int pTargetSiblingIndex = _siblings[pTargetIndex].transform.GetSiblingIndex();
			if (pStartSiblingIndex > pTargetSiblingIndex)
			{
				_siblings[pTargetIndex].transform.SetSiblingIndex(pStartSiblingIndex);
				base.transform.SetSiblingIndex(pTargetSiblingIndex);
			}
			else
			{
				base.transform.SetSiblingIndex(pTargetSiblingIndex);
				_siblings[pTargetIndex].transform.SetSiblingIndex(pStartSiblingIndex);
			}
			_siblings.Swap(pStartIndex, pTargetIndex);
		}
	}

	private int findClosestNeighbour(int pIndex, ListPool<int> pNeighbours)
	{
		int tClosestIndex = pIndex;
		Vector2 tTargetPosition = _sibling_positions[pIndex];
		float tClosestNeighbour = float.MaxValue;
		foreach (ref int pNeighbour in pNeighbours)
		{
			int tNeighbourIndex = pNeighbour;
			float tDistance = Vector2.Distance(_sibling_positions[tNeighbourIndex], tTargetPosition);
			if (tDistance < tClosestNeighbour)
			{
				tClosestNeighbour = tDistance;
				tClosestIndex = tNeighbourIndex;
			}
		}
		return tClosestIndex;
	}

	private ListPool<int> getNeighbours(int pIndex)
	{
		ListPool<int> tNeighbours = new ListPool<int>(8);
		if (_sibling_positions.Length < 2)
		{
			return tNeighbours;
		}
		Vector2 tMyPosition = _sibling_positions[pIndex];
		float tDistance = Vector2.Distance(_sibling_positions[0], _sibling_positions[1]) * 1.5f;
		for (int i = 0; i < _sibling_positions.Length; i++)
		{
			if (i != pIndex && Vector2.Distance(tMyPosition, _sibling_positions[i]) <= tDistance)
			{
				tNeighbours.Add(i);
			}
		}
		return tNeighbours;
	}

	private void addToggleComponent<T>() where T : MonoBehaviour
	{
		if (this.HasComponent<T>())
		{
			_toggle_elements.Add(GetComponent<T>());
		}
	}

	private bool isTouchDragDelayed()
	{
		if (_touch_drag_delay && !InputHelpers.mouseSupported)
		{
			return Time.time - _drag_timer_started_at < 0.2f;
		}
		return false;
	}
}
