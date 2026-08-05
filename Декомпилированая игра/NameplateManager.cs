using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameplateManager : MonoBehaviour
{
	private readonly Stack<NameplateText> _pool = new Stack<NameplateText>();

	private readonly List<NameplateText> _active = new List<NameplateText>();

	private int _next_index;

	public NameplateText prefab;

	private Canvas _canvas;

	internal CanvasScaler canvas_scaler;

	internal RectTransform canvas_rect;

	internal Vector2 canvas_size_delta;

	internal float canvas_size_delta_mod_x;

	internal float canvas_size_delta_mod_y;

	private MetaType _last_mode;

	public NameplateText cursor_over_text;

	private int _latest_touch_id;

	private bool _touch_released;

	private float _tween_timer;

	private float _tween_scale;

	internal bool cached_favorites_only;

	internal float cached_canvas_scale;

	private NameplateRenderingType _nameplate_mode;

	private bool _nano_object_set;

	private NanoObject _selected_nano_object;

	private void Awake()
	{
		_canvas = GetComponent<Canvas>();
		canvas_rect = _canvas.GetComponent<RectTransform>();
		canvas_scaler = GetComponent<CanvasScaler>();
	}

	private void prepare()
	{
		_next_index = 0;
		_nameplate_mode = ((PlayerConfig.getOptionInt("map_names") == 0) ? NameplateRenderingType.Full : NameplateRenderingType.BannerOnly);
		_nano_object_set = SelectedObjects.isNanoObjectSet();
		_selected_nano_object = SelectedObjects.getSelectedNanoObject();
		cached_favorites_only = PlayerConfig.optionBoolEnabled("only_favorited_meta");
		canvas_size_delta = canvas_rect.sizeDelta;
		cached_canvas_scale = canvas_scaler.scaleFactor;
		canvas_size_delta_mod_x = canvas_size_delta.x * 0.5f;
		canvas_size_delta_mod_y = canvas_size_delta.y * 0.5f;
	}

	internal void update()
	{
		Bench.bench("nameplates", "nameplates_total");
		Bench.bench("prepare", "nameplates");
		prepare();
		Bench.benchEnd("prepare", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("check_mode", "nameplates");
		MetaType tMetaType = getCurrentMode();
		setMode(tMetaType);
		NameplateAsset tNameplateAsset = null;
		MetaTypeAsset tMetaTypeAsset = null;
		if (!tMetaType.isNone())
		{
			tNameplateAsset = AssetManager.nameplates_library.map_modes_nameplates[tMetaType];
			tMetaTypeAsset = tMetaType.getAsset();
		}
		Bench.benchEnd("check_mode", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("set_nameplates", "nameplates");
		if (CanvasMain.isNameplatesAllowed())
		{
			if (tMetaType == MetaType.None)
			{
				if (base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: true);
				}
				tNameplateAsset.action_main(this, tNameplateAsset);
			}
		}
		Bench.benchEnd("set_nameplates", "nameplates", pSaveCounter: false, _active.Count);
		Bench.bench("updateOverlappingPositions", "nameplates");
		bool tUpdateOverlappingPositions = false;
		if (!tMetaType.isNone() && tMetaTypeAsset != null)
		{
			if (tMetaTypeAsset.isMetaZoneOptionSelectedFluid())
			{
				if (tNameplateAsset.overlap_for_fluid_mode)
				{
					tUpdateOverlappingPositions = true;
				}
			}
			else
			{
				tUpdateOverlappingPositions = true;
			}
		}
		if (tUpdateOverlappingPositions)
		{
			updateOverlappingPosition();
		}
		Bench.benchEnd("updateOverlappingPositions", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("updateTweenScale", "nameplates");
		updateTweenScale();
		Bench.benchEnd("updateTweenScale", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("checkActive", "nameplates");
		checkActive();
		Bench.benchEnd("checkActive", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("findObjectForTooltip", "nameplates");
		NanoObject tNanoObjectForTooltip = findObjectForTooltip();
		Bench.benchEnd("findObjectForTooltip", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("showTooltip", "nameplates");
		tNanoObjectForTooltip?.getMetaTypeAsset().cursor_tooltip_action(tNanoObjectForTooltip);
		Bench.benchEnd("showTooltip", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("check_siblings", "nameplates");
		checkSiblingsToFront();
		Bench.benchEnd("check_siblings", "nameplates", pSaveCounter: false, 0L);
		Bench.bench("finale", "nameplates");
		finale();
		Bench.benchEnd("finale", "nameplates", pSaveCounter: false, 0L);
		Bench.benchEnd("nameplates", "nameplates_total", pSaveCounter: false, 0L);
	}

	private void checkSiblingsToFront()
	{
		if (cursor_over_text != null)
		{
			cursor_over_text.transform.SetAsLastSibling();
		}
		if (!SelectedObjects.isNanoObjectSet())
		{
			return;
		}
		foreach (NameplateText tNameplate in _active)
		{
			if (tNameplate.nano_object == SelectedObjects.getSelectedNanoObject())
			{
				tNameplate.transform.SetAsLastSibling();
				break;
			}
		}
	}

	private void checkActive()
	{
		for (int i = _next_index - 1; i >= 0; i--)
		{
			_active[i].checkActive();
		}
	}

	private void updateTweenScale()
	{
		_tween_timer += Time.deltaTime * 2f;
		_tween_timer = Mathf.Clamp(_tween_timer, 0f, 1f);
		float tTargetY = iTween.easeOutBack(0f, 1f, _tween_timer);
		tTargetY *= 0.5f;
		_tween_scale = tTargetY;
	}

	private NanoObject findObjectForTooltip()
	{
		cursor_over_text = null;
		if (World.world.isBusyWithUI())
		{
			return null;
		}
		if (ControllableUnit.isControllingUnit())
		{
			return null;
		}
		if (!InputHelpers.mouseSupported && !checkTouch(out var tCursorPosition))
		{
			return null;
		}
		tCursorPosition = World.world.getMousePos();
		Vector2 tCursorScreenPos = World.world.camera.WorldToScreenPoint(tCursorPosition);
		bool tMouseSupported = InputHelpers.mouseSupported;
		NanoObject tResult = null;
		float tDistBest = float.MaxValue;
		NameplateText tBestNameplate = null;
		for (int i = 0; i < _active.Count; i++)
		{
			NameplateText tText1 = _active[i];
			if (tText1.isShowing())
			{
				Vector2 tTextScreenPosition = tText1.getLastScreenPosition();
				float tDist = Toolbox.SquaredDist(tTextScreenPosition.x, tTextScreenPosition.y, tCursorScreenPos.x, tCursorScreenPos.y);
				if (tText1.map_text_rect_click.Contains(tCursorScreenPos) && (!(tBestNameplate != null) || (!(tDist > tDistBest) && !(tDist > 625f))))
				{
					tBestNameplate = tText1;
					tDistBest = tDist;
				}
			}
		}
		if (tBestNameplate != null)
		{
			NanoObject tNanoObject = tBestNameplate.nano_object;
			if (Input.mousePresent)
			{
				tResult = tNanoObject;
			}
			cursor_over_text = tBestNameplate;
			Vector3 tCursorOverScale = tBestNameplate.transform.localScale;
			tCursorOverScale *= 1.1f;
			cursor_over_text.forceScale(tCursorOverScale);
			if (tNanoObject is IMetaObject && tMouseSupported)
			{
				((IMetaObject)tNanoObject).setCursorOver();
			}
		}
		return tResult;
	}

	public bool isOverNameplate()
	{
		return cursor_over_text != null;
	}

	private bool checkTouch(out Vector2 pPosition)
	{
		pPosition = Globals.POINT_IN_VOID;
		if (Input.touchCount == 0)
		{
			return false;
		}
		Touch tTouch = Input.touches[0];
		if (tTouch.phase == TouchPhase.Began && _touch_released)
		{
			_latest_touch_id = tTouch.fingerId;
			_touch_released = false;
			return false;
		}
		if (tTouch.fingerId != _latest_touch_id || tTouch.phase != TouchPhase.Ended || _touch_released)
		{
			return false;
		}
		_touch_released = true;
		pPosition = World.world.camera.ScreenToWorldPoint(tTouch.position);
		return true;
	}

	private MetaType getCurrentMode()
	{
		MetaType tMode = MetaType.None;
		if (Zones.showMapNames())
		{
			if (!Zones.hasPowerForceMapMode())
			{
				tMode = Zones.getCurrentMapBorderMode();
				if (tMode.isNone())
				{
					tMode = MetaType.City;
				}
			}
			else
			{
				tMode = Zones.getForcedMapMode();
			}
		}
		return tMode;
	}

	private void setMode(MetaType pMode)
	{
		if (_last_mode != pMode)
		{
			_last_mode = pMode;
			clearAll();
		}
	}

	private void updateOverlappingPosition()
	{
		if (_next_index <= 0)
		{
			return;
		}
		using ListPool<NameplateText> tActiveNameplates = new ListPool<NameplateText>(_next_index);
		for (int i = 0; i < _next_index; i++)
		{
			NameplateText tText = _active[i];
			tActiveNameplates.Add(tText);
		}
		if (tActiveNameplates.Count <= 1)
		{
			return;
		}
		tActiveNameplates.Sort(compareNameplates);
		using ListPool<NameplateText> tVisiblePlates = new ListPool<NameplateText>(_next_index);
		foreach (ref NameplateText item in tActiveNameplates)
		{
			NameplateText tCandidate = item;
			bool tOverlap = false;
			foreach (ref NameplateText item2 in tVisiblePlates)
			{
				NameplateText tVisiblePlate = item2;
				if (tCandidate.overlapsWithOtherPlate(tVisiblePlate))
				{
					tOverlap = true;
					break;
				}
			}
			if (tOverlap)
			{
				tCandidate.setShowing(pVal: false);
			}
			else
			{
				tVisiblePlates.Add(tCandidate);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Camera tCam = Camera.main;
		if (!(tCam == null))
		{
			for (int i = 0; i < _next_index; i++)
			{
				NameplateText tPlate = _active[i];
				Rect tRekt = tPlate.map_text_rect_overlap;
				Vector3 tScreenBottomLeft = new Vector3(tRekt.xMin, tRekt.yMin, tCam.nearClipPlane);
				Vector3 tScreenTopRight = new Vector3(tRekt.xMax, tRekt.yMax, tCam.nearClipPlane);
				Vector3 tWorldBottomLeft = tCam.ScreenToWorldPoint(tScreenBottomLeft);
				Vector3 tWorldTopRight = tCam.ScreenToWorldPoint(tScreenTopRight);
				Vector3 center = (tWorldBottomLeft + tWorldTopRight) * 0.5f;
				Vector3 tSize = new Vector3(tWorldTopRight.x - tWorldBottomLeft.x, tWorldTopRight.y - tWorldBottomLeft.y, 0.1f);
				Gizmos.color = (tPlate.isShowing() ? Color.green : Color.red);
				Gizmos.DrawWireCube(center, tSize);
			}
		}
	}

	private int compareNameplates(NameplateText pText1, NameplateText pText2)
	{
		NanoObject tSelectedObject = SelectedObjects.getSelectedNanoObject();
		bool tP1Selected = pText1.nano_object == tSelectedObject;
		bool tP2Selected = pText2.nano_object == tSelectedObject;
		if (tP1Selected != tP2Selected)
		{
			return (tP2Selected ? 1 : 0) - (tP1Selected ? 1 : 0);
		}
		if (pText1.favorited != pText2.favorited)
		{
			return (pText2.favorited ? 1 : 0) - (pText1.favorited ? 1 : 0);
		}
		if (pText1.priority_capital != pText2.priority_capital)
		{
			return (pText2.priority_capital ? 1 : 0) - (pText1.priority_capital ? 1 : 0);
		}
		int tPopCompare = pText2.priority_population.CompareTo(pText1.priority_population);
		if (tPopCompare != 0)
		{
			return tPopCompare;
		}
		return pText1.nano_object.id.CompareTo(pText2.nano_object.id);
	}

	public NameplateText prepareNext(NameplateAsset pAsset, NanoObject pMeta)
	{
		NameplateText nameplateToRender = getNameplateToRender();
		nameplateToRender.prepare(pAsset, pMeta, _tween_scale, _nameplate_mode, _nano_object_set, _selected_nano_object);
		return nameplateToRender;
	}

	private NameplateText getNameplateToRender()
	{
		NameplateText tObject;
		if (_active.Count > _next_index)
		{
			tObject = _active[_next_index];
		}
		else
		{
			tObject = ((_pool.Count != 0) ? _pool.Pop() : createNew());
			_active.Add(tObject);
		}
		_next_index++;
		return tObject;
	}

	protected virtual NameplateText createNew()
	{
		NameplateText nameplateText = Object.Instantiate(prefab, base.transform);
		nameplateText.newNameplate(this, $"map text {_pool.Count + _active.Count}");
		return nameplateText;
	}

	internal void clearAll()
	{
		_tween_timer = 0.5f;
		_tween_scale = 0f;
		if (_active.Count != 0)
		{
			for (int i = 0; i < _active.Count; i++)
			{
				NameplateText tObject = _active[i];
				tObject.clearFull();
				tObject.gameObject.SetActive(value: false);
				_pool.Push(tObject);
			}
			_active.Clear();
		}
	}

	private void finale()
	{
		clearLast();
	}

	public void clearCaches()
	{
		foreach (NameplateText item in _active)
		{
			item.clearCaches();
		}
	}

	public void clearLast()
	{
		int tDiff = _active.Count - _next_index;
		if (tDiff > 0)
		{
			while (tDiff > 0)
			{
				int tIndex = _active.Count - 1;
				NameplateText tObject = _active[tIndex];
				tObject.clearFull();
				tObject.gameObject.SetActive(value: false);
				_active.RemoveAt(tIndex);
				_pool.Push(tObject);
				tDiff--;
			}
		}
	}
}
