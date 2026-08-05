using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MetaSwitchManager : MonoBehaviour
{
	public enum Direction
	{
		Left,
		Right
	}

	private const float POSITION_SHOW = 0f;

	private const float POSITION_HIDE = -44f;

	private const float WINDOW_MAX_SIZE_PERCENT = 100f;

	private const float WINDOW_SIZE_PORTRAIT_START = 100f;

	private const float WINDOW_SIZE_PORTRAIT_END = 115f;

	private const float WINDOW_SIZE_PORTRAIT_RATIO_MIN = 1.275f;

	private const float WINDOW_SIZE_PORTRAIT_RATIO_MAX = 1.45f;

	private const float ANIMATION_DURATION = 0.35f;

	[SerializeField]
	private MetaSwitchButton _button_left;

	[SerializeField]
	private MetaSwitchButton _button_right;

	[SerializeField]
	private Text _window_number_current;

	[SerializeField]
	private Text _window_number_total;

	[SerializeField]
	private GameObject _container;

	private StatsWindow _stats_window;

	private MetaTypeAsset _meta_type_asset;

	private ListPool<NanoObject> _list;

	private static MetaSwitchManager _instance;

	private bool _is_switching_enabled;

	private bool _was_just_opened;

	private bool _is_enabled;

	private Tweener _tweener;

	private void Awake()
	{
		_instance = this;
		ScrollWindow.addCallbackOpen(delegate
		{
			_was_just_opened = true;
			enable(pOpen: true);
		});
		ScrollWindow.addCallbackShow(delegate
		{
			if (_was_just_opened)
			{
				_was_just_opened = false;
			}
			else
			{
				enable(pOpen: false);
			}
		});
		ScrollWindow.addCallbackClose(delegate
		{
			disable();
		});
		_button_left.init(Direction.Left, switchWindowsWithCheck);
		_button_right.init(Direction.Right, switchWindowsWithCheck);
	}

	private void Start()
	{
		CanvasMain.instance.addCallbackResize(delegate
		{
			if (!_is_enabled)
			{
				enable(pOpen: false, pCompleteOnDisable: false);
			}
			else
			{
				refresh(pCompleteTween: false, pCompleteOnDisable: false);
			}
		});
	}

	private void enable(bool pOpen, bool pCompleteOnDisable = true)
	{
		_is_enabled = true;
		StatsWindow tStatsWindow = ScrollWindow.getCurrentWindow()?.GetComponent<StatsWindow>();
		if (tStatsWindow == _stats_window && _stats_window != null)
		{
			updateShowingData();
			return;
		}
		if (tStatsWindow == null)
		{
			disable(!pOpen);
			return;
		}
		_stats_window = tStatsWindow;
		_meta_type_asset = AssetManager.meta_type_library.getAsset(_stats_window.meta_type);
		refresh(pCompleteTween: true, pCompleteOnDisable);
	}

	private void disable(bool pAnimated = true, bool pCompleteTween = true)
	{
		_is_enabled = false;
		if (pAnimated)
		{
			toggleControlsPosition(pState: false, pCompleteTween);
		}
		else
		{
			toggleControls(pState: false);
		}
		_stats_window = null;
		_list?.Dispose();
		_list = null;
	}

	public static void checkAndRefresh()
	{
		_instance.checkRefresh();
	}

	public static void refresh()
	{
		_instance.refresh(true, true);
	}

	public static void refreshWithoutComplete()
	{
		_instance.refresh(pCompleteTween: false);
	}

	private void checkRefresh()
	{
		if (_is_enabled)
		{
			refresh(pCompleteTween: false);
		}
	}

	internal void refresh(bool pCompleteTween = true, bool pCompleteOnDisable = true)
	{
		int tSize = PlayerConfig.getOptionInt("ui_size_windows");
		if ((float)tSize > 100f)
		{
			float tRatio = Mathf.Lerp(1.275f, 1.45f, 1f - Mathf.InverseLerp(100f, 115f, tSize));
			float tScreenRatio = (float)Screen.width / (float)Screen.height * tRatio;
			if ((float)tSize * tScreenRatio > 100f)
			{
				disable(pAnimated: true, pCompleteOnDisable);
				return;
			}
		}
		_list?.Dispose();
		_list = _meta_type_asset.getSortedList();
		bool tEnabled = (_is_switching_enabled = _list.Count >= 2);
		toggleControlsPosition(tEnabled, pCompleteTween);
		if (tEnabled)
		{
			updateShowingData();
		}
	}

	private static void switchWindowsWithCheck(Direction pDirection)
	{
		if (ScrollWindow.isWindowActive() && !ScrollWindow.isAnimationActive())
		{
			switchWindows(pDirection);
		}
	}

	public static void switchWindows(Direction pDirection)
	{
		_instance.switchWindow(pDirection);
	}

	private int getCurrentMetaIndex()
	{
		NanoObject tSelected = _meta_type_asset.get_selected();
		int tIndex = _list.IndexOf(tSelected);
		if (tIndex == -1)
		{
			_list.Add(tSelected);
			tIndex = _list.IndexOf(tSelected);
		}
		return tIndex;
	}

	private void switchWindow(Direction pDirection)
	{
		if (_is_switching_enabled && !(_stats_window == null) && _list.Count >= 2)
		{
			NanoObject tNextElement = getElement(pDirection);
			_meta_type_asset.set_selected(tNextElement);
			WindowHistory.popHistory();
			ScrollWindow.showWindow(_meta_type_asset.window_name);
			updateShowingData();
		}
	}

	private void updateShowingData()
	{
		updateWindowNumber();
		showBannersAndNames();
	}

	private void updateWindowNumber()
	{
		if (_list == null)
		{
			_window_number_current.text = "";
			_window_number_total.text = "";
			return;
		}
		int tNumber = getCurrentMetaIndex() + 1;
		int tCount = _list.Count;
		_window_number_current.text = $"{tNumber}";
		_window_number_total.text = $"{tCount}";
	}

	private void showBannersAndNames()
	{
		clear();
		showBanner(getIndex(Direction.Left), _button_left);
		showBanner(getIndex(Direction.Right), _button_right);
	}

	private IBanner showBanner(int pIndex, MetaSwitchButton pButton)
	{
		NanoObject tObject = _list[pIndex];
		IBanner tBanner = pButton.getPool().getNext(tObject);
		tBanner.load(tObject);
		if (tBanner.gameObject.TryGetComponent<Button>(out var tButton))
		{
			tButton.enabled = false;
		}
		pButton.setBanner(tBanner);
		Transform obj = tBanner.transform;
		Transform parent = obj.parent;
		parent.localPosition = Vector3.zero;
		parent.localScale = Vector3.one;
		obj.localPosition = Vector3.zero;
		ColorAsset tColorAsset = tObject.getColor();
		if (tColorAsset != null)
		{
			string tColor = tColorAsset.color_text;
			pButton.meta_name.text = tObject.name.ColorHex(tColor);
		}
		return tBanner;
	}

	private void toggleControlsPosition(bool pState, bool pCompleteTween = true)
	{
		_tweener.Kill(pCompleteTween);
		float tPositionY = (pState ? 0f : (-44f));
		if (pState)
		{
			toggleControls(pState: true);
		}
		if (Mathf.Approximately(base.transform.localPosition.y, tPositionY))
		{
			return;
		}
		_tweener = base.transform.DOLocalMoveY(tPositionY, 0.35f).SetEase(Ease.InOutCubic).OnComplete(delegate
		{
			if (!pState)
			{
				toggleControls(pState: false);
			}
			checkRefresh();
		});
	}

	private void toggleControls(bool pState)
	{
		_container.SetActive(pState);
	}

	private void clear()
	{
		_button_left.clear();
		_button_right.clear();
	}

	private NanoObject getElement(Direction pDirection)
	{
		int tIndex = getIndex(pDirection);
		return _list[tIndex];
	}

	private int getIndex(Direction pDirection)
	{
		int tIndex = getCurrentMetaIndex();
		return Toolbox.loopIndex((pDirection == Direction.Left) ? (tIndex - 1) : (tIndex + 1), _list.Count);
	}

	public static bool isAnimationActive()
	{
		return _instance._tweener.IsActive();
	}

	public static bool isSwitcherEnabled()
	{
		return _instance._is_enabled;
	}

	public static MetaSwitchButton getLeftbutton()
	{
		return _instance._button_left;
	}

	public static MetaSwitchButton getRightButton()
	{
		return _instance._button_right;
	}
}
