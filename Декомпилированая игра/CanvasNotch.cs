using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CanvasNotch : MonoBehaviour
{
	private bool screenChangeVarsInitialized;

	private bool ranFirstTime;

	private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

	private Vector2 lastResolution = Vector2.zero;

	private Rect lastSafeArea = Rect.zero;

	private Rect lastCanvasRect = Rect.zero;

	private RectTransform safeAreaTransform;

	private Canvas _canvas;

	private void Awake()
	{
		_canvas = base.gameObject.transform.GetComponentInParent<Canvas>();
		safeAreaTransform = GetComponent<RectTransform>();
		if (!screenChangeVarsInitialized)
		{
			lastOrientation = Screen.orientation;
			lastResolution.x = Screen.width;
			lastResolution.y = Screen.height;
			lastSafeArea = Screen.safeArea;
			screenChangeVarsInitialized = true;
		}
	}

	private void Start()
	{
		ApplySafeArea();
	}

	private void Update()
	{
		if (Application.isMobilePlatform && Screen.orientation != lastOrientation)
		{
			OrientationChanged();
		}
		if (Screen.safeArea != lastSafeArea)
		{
			SafeAreaChanged();
		}
		if (_canvas != null && _canvas.pixelRect != lastCanvasRect)
		{
			CanvasChanged();
		}
		if ((float)Screen.width != lastResolution.x || (float)Screen.height != lastResolution.y)
		{
			ResolutionChanged();
		}
		if (!ranFirstTime)
		{
			ApplySafeArea();
		}
	}

	private void ApplySafeArea()
	{
		if (!(_canvas == null) && !(safeAreaTransform == null))
		{
			ranFirstTime = true;
			Rect safeArea = Screen.safeArea;
			Rect tScreen = new Rect(0f, 0f, Screen.width, Screen.height);
			Vector2 tMinDiff = safeArea.min - tScreen.min;
			Vector2 tMaxDiff = safeArea.max - tScreen.max;
			safeArea.min -= tMaxDiff;
			safeArea.max -= tMinDiff;
			Vector2 anchorMin = safeArea.position;
			Vector2 anchorMax = safeArea.position + safeArea.size;
			anchorMin.x /= _canvas.pixelRect.width;
			anchorMin.y /= _canvas.pixelRect.height;
			anchorMax.x /= _canvas.pixelRect.width;
			anchorMax.y /= _canvas.pixelRect.height;
			safeAreaTransform.anchorMin = anchorMin;
			safeAreaTransform.anchorMax = anchorMax;
		}
	}

	private void OrientationChanged()
	{
		lastOrientation = Screen.orientation;
		lastResolution.x = Screen.width;
		lastResolution.y = Screen.height;
		ApplySafeArea();
	}

	private void ResolutionChanged()
	{
		lastResolution.x = Screen.width;
		lastResolution.y = Screen.height;
		ApplySafeArea();
	}

	private void SafeAreaChanged()
	{
		lastSafeArea = Screen.safeArea;
		ApplySafeArea();
	}

	private void CanvasChanged()
	{
		lastCanvasRect = _canvas.pixelRect;
		ApplySafeArea();
	}

	private void debugConsole()
	{
		Dictionary<string, Rect> sizes = new Dictionary<string, Rect>();
		Debug.Log("amount of cutouts: " + Screen.cutouts.Length);
		sizes["screen"] = new Rect(0f, 0f, Screen.width, Screen.height);
		sizes["safearea"] = Screen.safeArea;
		foreach (string screenId in sizes.Keys)
		{
			Debug.Log("[o] " + screenId + ": x:" + sizes[screenId].x + ", y:" + sizes[screenId].y + ", w:" + sizes[screenId].width + ", h:" + sizes[screenId].height);
		}
		if (_canvas == null)
		{
			Debug.Log("canvas not ready");
			return;
		}
		foreach (string screenId2 in sizes.Keys)
		{
			Debug.Log("[c] " + screenId2 + ": x:" + sizes[screenId2].x / _canvas.scaleFactor + ", y:" + sizes[screenId2].y / _canvas.scaleFactor + ", w:" + sizes[screenId2].width / _canvas.scaleFactor + ", h:" + sizes[screenId2].height / _canvas.scaleFactor);
		}
	}
}
