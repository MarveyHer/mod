using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMain : MonoBehaviour
{
	public static CanvasMain instance;

	public static float tooltip_show_timeout;

	public Canvas canvas_ui;

	public Canvas canvas_windows;

	public Canvas canvas_map_names;

	public Canvas canvas_tooltip;

	public Image blocker;

	private ScreenOrientation screenOrientation;

	private CanvasScaler scaler_main_ui;

	private CanvasScaler scaler_windows_ui;

	private CanvasScaler scaler_tooltip;

	private CanvasScaler scaler_map_names;

	public Transform transformWindows;

	private float last_width = -1f;

	private float last_height = -1f;

	private const int REFERENCE_SIZE_X = 285;

	private const int REFERENCE_SIZE_Y = 420;

	private ResizeAction _on_resize;

	private ResizeUIAction _on_resize_ui;

	private void Awake()
	{
		instance = this;
		scaler_main_ui = canvas_ui.GetComponent<CanvasScaler>();
		scaler_windows_ui = canvas_windows.GetComponent<CanvasScaler>();
		scaler_tooltip = canvas_tooltip.GetComponent<CanvasScaler>();
		scaler_map_names = canvas_map_names.GetComponent<CanvasScaler>();
	}

	public bool setMainUiEnabled(bool pEnabled)
	{
		if (canvas_ui.enabled == pEnabled)
		{
			return false;
		}
		canvas_ui.enabled = pEnabled;
		return true;
	}

	public float getLastWidth()
	{
		return last_width;
	}

	public float getLastHeight()
	{
		return last_height;
	}

	public void addCallbackResize(ResizeAction pAction)
	{
		_on_resize = (ResizeAction)Delegate.Combine(_on_resize, pAction);
	}

	public void removeCallbackResize(ResizeAction pAction)
	{
		_on_resize = (ResizeAction)Delegate.Remove(_on_resize, pAction);
	}

	public void addCallbackResizeUI(ResizeUIAction pAction)
	{
		_on_resize_ui = (ResizeUIAction)Delegate.Combine(_on_resize_ui, pAction);
	}

	public void removeCallbackResizeUI(ResizeUIAction pAction)
	{
		_on_resize_ui = (ResizeUIAction)Delegate.Remove(_on_resize_ui, pAction);
	}

	private void checkResize(float pWidth, float pHeight)
	{
		last_width = pWidth;
		last_height = pHeight;
		screenOrientation = Screen.orientation;
		resizeMainUI();
		resizeWindowsUI();
		resizeTooltipUI();
		_on_resize?.Invoke(pWidth, pHeight);
	}

	public void resizeWindowsUI()
	{
		changeCanvasSize(scaler_windows_ui, "ui_size_windows", 285f, 420f);
		float tPercent = (float)PlayerConfig.getIntValue("ui_size_windows") / 100f;
		_on_resize_ui?.Invoke(tPercent);
	}

	public void resizeTooltipUI()
	{
		changeCanvasSize(scaler_tooltip, "ui_size_tooltips", 285f, 420f);
	}

	public void resizeMapNames()
	{
		changeCanvasSize(scaler_map_names, "ui_size_map_names", 285f, 420f);
	}

	public void resizeMainUI()
	{
		changeCanvasSize(pReferenceHeight: (Screen.height <= Screen.width) ? 500f : 360f, pScaler: scaler_main_ui, pSizeOption: "ui_size_main", pReferenceWidth: 285f);
	}

	private void changeCanvasSize(CanvasScaler pScaler, string pSizeOption, float pReferenceWidth, float pReferenceHeight)
	{
		pScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		float tPercent = 1f;
		tPercent = (float)PlayerConfig.getIntValue(pSizeOption) / 100f;
		float tScale = 2f - tPercent;
		pScaler.referenceResolution = new Vector2(pReferenceWidth, pReferenceHeight * tScale);
	}

	private void Start()
	{
		screenOrientation = Screen.orientation;
	}

	private void Update()
	{
		if (tooltip_show_timeout > 0f)
		{
			tooltip_show_timeout -= Time.deltaTime;
		}
		if ((float)Screen.width != last_width || (float)Screen.height != last_height)
		{
			checkResize(Screen.width, Screen.height);
		}
		if (screenOrientation != Screen.orientation)
		{
			screenOrientation = Screen.orientation;
			if (ScrollWindow.isWindowActive())
			{
				ScrollWindow.hideAllEvent();
			}
		}
		if (Config.lockGameControls || (World.world?.stack_effects != null && World.world.stack_effects.isLocked()))
		{
			blocker.gameObject.SetActive(value: true);
		}
		else
		{
			blocker.gameObject.SetActive(value: false);
		}
	}

	public static void addTooltipShowTimeout(float pTime)
	{
		tooltip_show_timeout = pTime;
		Tooltip.hideTooltipNow();
	}

	public static bool isBottomBarShowing()
	{
		if (ScrollWindow.isWindowActive())
		{
			return false;
		}
		if (ControllableUnit.isControllingUnit())
		{
			return false;
		}
		if (MoveCamera.inSpectatorMode())
		{
			return false;
		}
		if (Config.ui_main_hidden)
		{
			return false;
		}
		if (SmoothLoader.isLoading())
		{
			return false;
		}
		if (SaveManager.isLoadingSaveAnimationActive())
		{
			return false;
		}
		return true;
	}

	public static bool isNameplatesAllowed()
	{
		if (SmoothLoader.isLoading())
		{
			return false;
		}
		if (SaveManager.isLoadingSaveAnimationActive())
		{
			return false;
		}
		return true;
	}
}
