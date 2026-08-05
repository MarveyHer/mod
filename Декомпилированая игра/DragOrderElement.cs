using LayoutGroupExt;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragOrderElement : MonoBehaviour, IDraggable, IEndDragHandler, IEventSystemHandler
{
	[SerializeField]
	private bool _spawn_particles_on_drag = true;

	public RectTransform main_transform;

	public bool can_be_dragged = true;

	private DragOrderContainer _container;

	private int _parent_canvas_sorting_order;

	private Canvas _canvas;

	private GraphicRaycaster _raycaster;

	private Button _button;

	private Transform _current_parent;

	internal Vector2 current_destination;

	internal bool is_target_reached;

	internal int order_index;

	private bool _drag_initialized;

	private float _drag_started_at;

	private int _mouse_button = -1;

	private Vector3 _prev_mouse_position;

	public bool spawn_particles_on_drag => _spawn_particles_on_drag;

	private void Start()
	{
		if (main_transform == null)
		{
			main_transform = GetComponent<RectTransform>();
		}
		_parent_canvas_sorting_order = main_transform.gameObject.GetComponentInParent<Canvas>().sortingOrder;
		_canvas = main_transform.gameObject.AddComponent<Canvas>();
		_canvas.sortingOrder = _parent_canvas_sorting_order;
		_canvas.overrideSorting = false;
		_raycaster = main_transform.gameObject.AddComponent<GraphicRaycaster>();
		_raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.All;
		_raycaster.blockingMask = -1;
		_raycaster.ignoreReversedGraphics = true;
		_button = GetComponent<Button>();
		_button.onClick.AddListener(delegate
		{
			_container?.updateChildrenData();
		});
		is_target_reached = true;
		checkContainer();
	}

	private void checkContainer()
	{
		if (!(_container != null))
		{
			_container = main_transform.GetComponentInParent<DragOrderContainer>();
		}
	}

	private void checkParent()
	{
		Transform tNewParent = main_transform.parent;
		if (!(_current_parent == tNewParent))
		{
			_current_parent = tNewParent;
			checkContainer();
		}
	}

	private void Update()
	{
		if (!base.enabled)
		{
			return;
		}
		checkParent();
		if (_container == null)
		{
			return;
		}
		checkDrag();
		if (_container.grid_layout.enabled)
		{
			return;
		}
		if (_container.dragging_element == this)
		{
			moveDraggingTab();
		}
		else if (!is_target_reached)
		{
			if (Vector3.Distance(main_transform.localPosition, current_destination) < 0.1f)
			{
				is_target_reached = true;
				unsetOnTop();
			}
			else
			{
				main_transform.localPosition = Vector3.Lerp(main_transform.localPosition, current_destination, Time.deltaTime * 10f);
			}
		}
	}

	private void setOnTop()
	{
		_canvas.overrideSorting = true;
		_canvas.sortingOrder = 24;
	}

	internal void unsetOnTop()
	{
		if (_canvas.overrideSorting)
		{
			_canvas.sortingOrder = _parent_canvas_sorting_order;
			_canvas.overrideSorting = false;
		}
	}

	public void updatePosition()
	{
		Vector2 tDestination = getChildPositionInContainer();
		if (!((Vector2)main_transform.localPosition == tDestination) && !(current_destination == tDestination))
		{
			current_destination = tDestination;
			is_target_reached = false;
		}
	}

	private void moveDraggingTab()
	{
		if (!_container.is_anything_dragging)
		{
			endDrag();
			return;
		}
		if (!InputHelpers.GetMouseButton(_mouse_button))
		{
			endDrag();
			return;
		}
		Vector3 tNewPosition = Input.mousePosition;
		switch (_container.snapping_axis)
		{
		case DragOrderContainer.SnapAxis.Horizontal:
			tNewPosition.y = main_transform.position.y;
			break;
		case DragOrderContainer.SnapAxis.Vertical:
			tNewPosition.x = main_transform.position.x;
			break;
		}
		if (!_container.limit_moving)
		{
			main_transform.position = tNewPosition;
			return;
		}
		Rect tContainerRect = _container.rect_transform.GetWorldRect();
		getGridValues(_container.grid_layout, out var tCellSize, out var _);
		tCellSize *= 0.5f;
		tNewPosition.x = Mathf.Min(tNewPosition.x, tContainerRect.xMax - tCellSize.x);
		tNewPosition.x = Mathf.Max(tNewPosition.x, tContainerRect.xMin + tCellSize.x);
		tNewPosition.y = Mathf.Min(tNewPosition.y, tContainerRect.yMax - tCellSize.y);
		tNewPosition.y = Mathf.Max(tNewPosition.y, tContainerRect.yMin + tCellSize.y);
		main_transform.position = tNewPosition;
	}

	private void checkDrag()
	{
		checkDragBegin();
		checkDragEnd();
	}

	private void checkDragBegin()
	{
		if (Config.isDraggingItem() || _container.is_anything_dragging || !can_be_dragged)
		{
			return;
		}
		if (_container.delay_before_drag && !isMouseOver())
		{
			_drag_initialized = false;
			return;
		}
		if (InputHelpers.GetAnyMouseButtonDown() && isMouseOver())
		{
			_mouse_button = InputHelpers.GetAnyMouseButtonDownIndex();
			_drag_started_at = Time.time;
			if (!_container.delay_before_drag)
			{
				_drag_started_at -= DragOrderContainer.drag_delay;
				_prev_mouse_position = Input.mousePosition;
			}
			_drag_initialized = true;
		}
		if (_drag_initialized)
		{
			if (!InputHelpers.GetMouseButton(_mouse_button))
			{
				_drag_initialized = false;
			}
			else if ((_container.delay_before_drag || shouldStartDrag(Input.mousePosition, _prev_mouse_position)) && !(Time.time - _drag_started_at < DragOrderContainer.drag_delay))
			{
				startDrag();
			}
		}
	}

	private void checkDragEnd()
	{
		if (InputHelpers.GetMouseButtonUp(_mouse_button) && _container.is_anything_dragging)
		{
			_drag_initialized = false;
			endDrag();
		}
	}

	public void OnEndDrag(PointerEventData pData)
	{
		if (_container.is_anything_dragging)
		{
			_drag_initialized = false;
			endDrag();
		}
	}

	private void startDrag()
	{
		_drag_started_at = Time.realtimeSinceStartup;
		_container.dragging_element = this;
		Config.setDraggingObject(this);
		_container.is_anything_dragging = true;
		_container.grid_layout.enabled = false;
		_container.layout_element.enabled = true;
		_button.interactable = false;
		if (_container.scroll_rect != null)
		{
			_container.scroll_rect.enabled = false;
		}
		_container.updateChildrenData();
		setOnTop();
	}

	public void stopDrag()
	{
		endDrag();
	}

	private void endDrag()
	{
		if (Config.isDraggingObject(this))
		{
			_button.interactable = true;
			_mouse_button = -1;
			if (_container.scroll_rect != null)
			{
				_container.scroll_rect.enabled = true;
			}
			Vector3 tTo = getChildPositionInContainer();
			current_destination = tTo;
			is_target_reached = false;
			if (!(_container.dragging_element != this))
			{
				_container.layout_element.enabled = false;
				_drag_initialized = false;
				_container.dragging_element = null;
				Config.clearDraggingObject();
				_container.is_anything_dragging = false;
			}
		}
	}

	private Vector3 getChildPositionInContainer()
	{
		return _container.getChildPosition(order_index);
	}

	private bool isMouseOver()
	{
		Vector2 tPoint = _container.rect_transform.InverseTransformPoint(Input.mousePosition);
		return getRect().Contains(tPoint);
	}

	public Rect getRect()
	{
		getGridValues(_container.grid_layout, out var tCellSize, out var tSpacing);
		Vector2 position = (Vector2)main_transform.localPosition - tCellSize * main_transform.pivot - tSpacing / 2f;
		Vector2 tRectSize = tCellSize + tSpacing;
		return new Rect(position, tRectSize);
	}

	private void getGridValues(LayoutGroup pLayoutGroup, out Vector2 pCellSize, out Vector2 pSpacing)
	{
		if (!(pLayoutGroup is GridLayoutGroup tGrid))
		{
			if (pLayoutGroup is GridLayoutGroupExtended tGrid2)
			{
				pCellSize = tGrid2.cellSize;
				pSpacing = tGrid2.spacing;
			}
			else
			{
				pCellSize = Vector2.zero;
				pSpacing = Vector2.zero;
			}
		}
		else
		{
			pCellSize = tGrid.cellSize;
			pSpacing = tGrid.spacing;
		}
	}

	private static bool shouldStartDrag(Vector2 pPressPos, Vector2 pCurrentPos)
	{
		float tThreshold = EventSystem.current.pixelDragThreshold;
		return (pPressPos - pCurrentPos).sqrMagnitude >= tThreshold * tThreshold;
	}

	private void OnDisable()
	{
		DragOrderContainer container = _container;
		if ((object)container != null && container.is_anything_dragging)
		{
			OnEndDrag(new PointerEventData(EventSystem.current));
		}
	}

	public void KillDrag()
	{
		OnDisable();
	}
}
