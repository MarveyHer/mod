using System.Collections.Generic;
using UnityEngine;

public class HistoryHud : MonoBehaviour
{
	public static HistoryHud instance;

	[SerializeField]
	private GameObject _template_obj;

	private List<HistoryHudItem> _history_items = new List<HistoryHudItem>(10);

	private ObjectPoolGenericMono<HistoryHudItem> _parked_items;

	private Transform _content_group;

	private Transform _parked_group;

	private const int HISTORY_ITEM_SIZE = 15;

	private const int MAX_HISTORY_ITEMS = 10;

	private const float START_POSITION = 0f;

	private bool _recalc;

	private static bool _raycast_on = true;

	public bool raycastOn
	{
		get
		{
			if (MoveCamera.camera_drag_run)
			{
				return false;
			}
			if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
			{
				return false;
			}
			if (MapBox.controlsLocked())
			{
				return false;
			}
			if (MapBox.isControllingUnit())
			{
				return false;
			}
			if (World.world.isAnyPowerSelected())
			{
				return false;
			}
			return _raycast_on;
		}
		set
		{
			_raycast_on = value;
		}
	}

	private void Awake()
	{
		instance = this;
		_content_group = base.transform.Find("Scroll View/Viewport/Content");
		_parked_group = base.transform.Find("Scroll View/Viewport/Parked");
		_parked_items = new ObjectPoolGenericMono<HistoryHudItem>(_template_obj.GetComponent<HistoryHudItem>(), _parked_group);
	}

	private void OnEnable()
	{
		if (Config.game_loaded)
		{
			_content_group.gameObject.GetComponent<RectTransform>().SetTop(0f);
			_content_group.gameObject.GetComponent<RectTransform>().SetLeft(0f);
		}
	}

	private void OnDisable()
	{
		Clear();
	}

	private void Update()
	{
		checkEnabled();
		if (_recalc)
		{
			_recalc = false;
			recalcPositions();
		}
	}

	public static void disableRaycasts()
	{
		instance.raycastOn = false;
	}

	public static void enableRaycasts()
	{
		instance.raycastOn = true;
	}

	private float recalcPositions()
	{
		if (_history_items.Count == 0)
		{
			return 0f;
		}
		float tNewBottom = 0f;
		float tMaxBottom = 0f;
		int tItemsToRemove = 0;
		if (_history_items.Count > 10)
		{
			tItemsToRemove = _history_items.Count - 10;
		}
		for (int i = 0; i < _history_items.Count; i++)
		{
			if (tItemsToRemove > 0)
			{
				if (_history_items[i].target_bottom != (float)(tItemsToRemove * -15))
				{
					_history_items[i].moveToAndDestroy(tItemsToRemove * -15);
				}
				tItemsToRemove--;
			}
			else
			{
				if (_history_items[i].isRemoving())
				{
					continue;
				}
				if (_history_items[i].target_bottom != tNewBottom)
				{
					_history_items[i].moveTo(tNewBottom);
				}
				tNewBottom += 15f;
			}
			tMaxBottom = 0f - _history_items[i].GetComponent<RectTransform>().offsetMax.y;
		}
		if (tMaxBottom >= tNewBottom)
		{
			return tMaxBottom + 15f;
		}
		return tNewBottom;
	}

	private bool checkEnabled()
	{
		if (!PlayerConfig.optionBoolEnabled("history_log"))
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
			return false;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		return true;
	}

	public void newHistory(WorldLogMessage pMessage)
	{
		if (checkEnabled())
		{
			newText(pMessage);
			base.gameObject.SetActive(value: true);
		}
	}

	public void makeInactive(HistoryHudItem historyItem)
	{
		_parked_items.resetParent(historyItem);
		_parked_items.release(historyItem);
		_history_items.Remove(historyItem);
		_recalc = true;
	}

	public void Clear()
	{
		for (int i = _history_items.Count - 1; i >= 0; i--)
		{
			makeInactive(_history_items[i]);
		}
	}

	private void newText(WorldLogMessage pMessage)
	{
		HistoryHudItem tObj = _parked_items.getNext();
		tObj.transform.SetParent(_content_group);
		tObj.gameObject.name = "HistoryItem " + (_history_items.Count + 1);
		tObj.gameObject.SetActive(value: true);
		RectTransform component = tObj.GetComponent<RectTransform>();
		component.localScale = Vector3.one;
		component.localPosition = Vector3.zero;
		component.SetLeft(0f);
		float newBottom = recalcPositions();
		component.SetTop(newBottom);
		component.sizeDelta = new Vector2(component.sizeDelta.x, 15f);
		tObj.target_bottom = newBottom;
		tObj.setMessage(pMessage);
		_history_items.Add(tObj);
		_recalc = true;
	}
}
