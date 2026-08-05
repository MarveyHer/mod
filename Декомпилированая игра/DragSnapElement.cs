using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class DragSnapElement : MonoBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler, IDraggable, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private bool _spawn_particles_on_drag = true;

	[SerializeField]
	private bool _touch_drag_delay;

	private float _drag_timer_started_at;

	private Tweener _tweener;

	private LayoutElement _layout_element;

	private Button _button;

	private Vector3 _start_local_position;

	private Transform _start_parent;

	public float limit_max_drag_distance = 77f;

	public float snapback_max_distance = 77f;

	public float snapback_speed_max_distance = 0.35f;

	public float snapback_min_distance = 22f;

	public float snapback_speed_min_distance = 0.9f;

	public Transform attach_parent;

	public Transform fly_back_parent;

	public Ease ease = Ease.OutElastic;

	public float speed = 0.4f;

	private ScrollRect _scroll_rect;

	private ScrollRectExtended _scroll_rect_extended;

	private bool _initial_ignore_layout;

	private bool _hovered;

	private bool _is_dragging;

	public bool spawn_particles_on_drag => _spawn_particles_on_drag;

	private void Start()
	{
		_layout_element = GetComponent<LayoutElement>();
		_button = GetComponent<Button>();
		_start_parent = base.transform.parent;
		_start_local_position = base.transform.localPosition;
		_initial_ignore_layout = _layout_element.ignoreLayout;
		if (attach_parent == null)
		{
			attach_parent = World.world.drag_parent;
		}
		if (fly_back_parent == null)
		{
			fly_back_parent = base.transform.FindParentWithName("Content", "Viewport") ?? attach_parent;
		}
		if (base.gameObject.TryGetComponent<ScrollableButton>(out var tScrollableButton))
		{
			tScrollableButton.enabled = false;
			_scroll_rect_extended = base.gameObject.GetComponentInParent<ScrollRectExtended>();
			if (_scroll_rect_extended == null)
			{
				_scroll_rect = base.gameObject.GetComponentInParent<ScrollRect>();
			}
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
		if (!_is_dragging && !Config.isDraggingItem() && !isTouchDragDelayed())
		{
			_tweener.Kill();
			Config.setDraggingObject(this);
			_is_dragging = true;
			base.transform.SetParent(attach_parent);
			_button.enabled = false;
			_layout_element.enabled = true;
			updatePosition(pEventData.position);
		}
	}

	public void OnDrag(PointerEventData pEventData)
	{
		if (Config.isDraggingObject(this) && !isTouchDragDelayed())
		{
			updatePosition(pEventData.position);
		}
	}

	public float getDragMod()
	{
		if (!_is_dragging)
		{
			return 0f;
		}
		Vector3 vector = _start_parent.TransformPoint(_start_local_position);
		float tMod = Mathf.Clamp01((vector - base.transform.position).magnitude / limit_max_drag_distance);
		if (vector.y > base.transform.position.y)
		{
			tMod = 0f - tMod;
		}
		return tMod;
	}

	private void updatePosition(Vector3 pTargetPosition)
	{
		Vector3 tStartPosition = _start_parent.TransformPoint(_start_local_position);
		Vector3 tDirection = pTargetPosition - tStartPosition;
		if (tDirection.magnitude > limit_max_drag_distance)
		{
			base.transform.position = tStartPosition + tDirection.normalized * limit_max_drag_distance;
		}
		else
		{
			base.transform.position = pTargetPosition;
		}
	}

	public void OnEndDrag(PointerEventData pEventData)
	{
		if (Config.isDraggingItem() && Config.isDraggingObject(this))
		{
			Config.clearDraggingObject();
			_is_dragging = false;
			_drag_timer_started_at = 0f;
			_layout_element.ignoreLayout = true;
			base.transform.SetParent(fly_back_parent);
			_tweener?.Kill();
			Vector3 tStartPosition = _start_parent.TransformPoint(_start_local_position);
			float tDistance = (tStartPosition - base.transform.position).magnitude;
			float tSpeed = dragSpeed(tDistance);
			_tweener = DOTween.To(() => base.transform.position, delegate(Vector3 pVector)
			{
				base.transform.position = pVector;
			}, tStartPosition, tSpeed).SetEase(ease).OnComplete(resetElement);
			Tooltip.blockTooltips(tSpeed * 0.7f);
		}
	}

	public void resetElement()
	{
		if (_hovered)
		{
			_button.TriggerHover();
		}
		if (!(_start_parent == null) && (!(fly_back_parent != _start_parent) || !(_start_parent == base.transform.parent)))
		{
			base.transform.SetParent(_start_parent);
			base.transform.localPosition = _start_local_position;
			_button.enabled = true;
			_layout_element.ignoreLayout = _initial_ignore_layout;
			_layout_element.enabled = false;
		}
	}

	public float dragSpeed(float pDistance)
	{
		float tLerped = (Mathf.Clamp(pDistance, snapback_min_distance, snapback_max_distance) - snapback_min_distance) / (snapback_max_distance - snapback_min_distance);
		return Mathf.Lerp(snapback_speed_min_distance, snapback_speed_max_distance, tLerped);
	}

	public void onWindowClose(string pId)
	{
		_tweener.Kill(complete: true);
	}

	public void OnScroll(PointerEventData pEventData)
	{
		sendMessage("OnScroll", pEventData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_hovered = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_hovered = false;
	}

	private void sendMessage(string pMethodName, PointerEventData pEventData)
	{
		_scroll_rect?.SendMessage(pMethodName, pEventData);
		_scroll_rect_extended?.SendMessage(pMethodName, pEventData);
	}

	public void OnEnable()
	{
		ScrollWindow.addCallbackHide(onWindowClose);
	}

	public void OnDisable()
	{
		ScrollWindow.removeCallbackHide(onWindowClose);
		KillDrag();
		if (_tweener.IsActive())
		{
			Debug.LogError("OnDisable kill called, shouldn't happen", this);
			_tweener.Kill();
		}
	}

	public void KillDrag()
	{
		if (_is_dragging)
		{
			OnEndDrag(new PointerEventData(EventSystem.current));
			_tweener.Kill(complete: true);
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
