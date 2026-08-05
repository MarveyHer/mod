using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollableButton))]
public class GraphCompareMetaSelector : MonoBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDraggable
{
	[SerializeField]
	private bool _spawn_particles_on_drag = true;

	private Vector3 _start_local_position;

	private Transform _start_parent;

	private ScrollableButton _scrollable_button;

	private readonly List<Graphic> _raycastables = new List<Graphic>();

	private Vector2 _first_position = Vector2.zero;

	private bool _dragging;

	private readonly List<RectTransform> _dropzones = new List<RectTransform>();

	private GraphCompareWindow _window;

	public bool spawn_particles_on_drag => _spawn_particles_on_drag;

	private Transform _attach_parent => World.world.drag_parent;

	private void Awake()
	{
		_scrollable_button = GetComponent<ScrollableButton>();
		_start_parent = base.transform.parent;
		GetComponent<Button>().onClick.AddListener(showTooltip);
	}

	private void showTooltip()
	{
		IBanner tBanner = GetComponent<IBanner>();
		if (!InputHelpers.mouseSupported && !Tooltip.isShowingFor(tBanner))
		{
			tBanner.showTooltip();
		}
	}

	public void addWindow(GraphCompareWindow pWindow)
	{
		_window = pWindow;
	}

	public void addDropzones(params RectTransform[] pDropzones)
	{
		_dropzones.Clear();
		_dropzones.AddRange(pDropzones);
	}

	public bool isBeingDragged()
	{
		return _dragging;
	}

	public void OnInitializePotentialDrag(PointerEventData pEventData)
	{
		_dragging = false;
		_first_position = pEventData.position;
		_start_parent = base.transform.parent;
		_start_local_position = base.transform.localPosition;
	}

	public bool checkIfDragging(PointerEventData pEventData)
	{
		if (_window.countNoosItems() < 5)
		{
			return true;
		}
		Vector2 tMaxLowerLeft = new Vector2(float.MaxValue, 0f);
		Vector2 tMaxLowerRight = new Vector2(float.MinValue, 0f);
		foreach (RectTransform tDropzone in _dropzones)
		{
			Vector2 tLowerLeft = tDropzone.position;
			tLowerLeft.x -= tDropzone.rect.width * tDropzone.lossyScale.x / 2f;
			tLowerLeft.y -= tDropzone.rect.height * tDropzone.lossyScale.y / 2f;
			Vector2 tLowerRight = tDropzone.position;
			tLowerRight.x += tDropzone.rect.width * tDropzone.lossyScale.x / 2f;
			tLowerRight.y -= tDropzone.rect.height * tDropzone.lossyScale.y / 2f;
			if (tLowerLeft.x < tMaxLowerLeft.x)
			{
				tMaxLowerLeft = tLowerLeft;
			}
			if (tLowerRight.x > tMaxLowerRight.x)
			{
				tMaxLowerRight = tLowerRight;
			}
		}
		if (!Toolbox.isInTriangle(pEventData.position, _first_position, tMaxLowerLeft, tMaxLowerRight))
		{
			Vector2 tDirection = pEventData.position - _first_position;
			if (Mathf.Abs(tDirection.x) > Mathf.Abs(tDirection.y))
			{
				return false;
			}
		}
		return true;
	}

	public void OnBeginDrag(PointerEventData pEventData)
	{
		if (!Config.isDraggingItem() && !_dragging)
		{
			_dragging = checkIfDragging(pEventData);
			if (_dragging)
			{
				Config.setDraggingObject(this);
				pEventData.Use();
				_scrollable_button.enabled = false;
				GraphCompareMetaObject.disable_raycasts = true;
				base.transform.SetParent(_attach_parent);
				base.transform.position = pEventData.position;
				disableRaycast();
			}
		}
	}

	public void OnDrag(PointerEventData pEventData)
	{
		if (_dragging && Config.isDraggingObject(this))
		{
			pEventData.Use();
			base.transform.position = pEventData.position;
		}
	}

	public void OnEndDrag(PointerEventData pEventData)
	{
		_scrollable_button.OnEndDrag(pEventData);
		if (_dragging && Config.isDraggingObject(this))
		{
			pEventData.Use();
			base.transform.SetParent(_start_parent);
			base.transform.localPosition = _start_local_position;
			resetDrag();
		}
	}

	public void resetDrag()
	{
		if (!_dragging)
		{
			return;
		}
		Config.clearDraggingObject();
		_dragging = false;
		_scrollable_button.enabled = true;
		GraphCompareMetaObject.disable_raycasts = false;
		foreach (Graphic raycastable in _raycastables)
		{
			raycastable.raycastTarget = true;
		}
	}

	private void disableRaycast()
	{
		_raycastables.Clear();
		Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>();
		foreach (Graphic tGraphic in componentsInChildren)
		{
			if (tGraphic.raycastTarget)
			{
				_raycastables.Add(tGraphic);
			}
		}
		foreach (Graphic raycastable in _raycastables)
		{
			raycastable.raycastTarget = false;
		}
	}

	private void OnDisable()
	{
		resetDrag();
	}

	public void KillDrag()
	{
		OnDisable();
	}
}
