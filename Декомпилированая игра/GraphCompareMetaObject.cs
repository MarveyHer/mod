using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphCompareMetaObject : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	private GraphCompareWindow _graph_window;

	private GraphController _graph_controller;

	private MultiBannerPool _pool_drop_banners;

	public NanoObject current_item;

	public GameObject empty_drop_icon;

	public LocalizedText meta_title;

	public Text meta_name;

	private IBanner _current_banner;

	public static bool disable_raycasts;

	private bool _disable_raycasts;

	private List<Graphic> _raycast_children = new List<Graphic>();

	private bool _initialized;

	public void Awake()
	{
		init();
	}

	private void init()
	{
		if (!_initialized)
		{
			_initialized = true;
			_graph_window = GetComponentInParent<GraphCompareWindow>();
			_graph_controller = _graph_window.graph_controller;
			_pool_drop_banners = _graph_window.getDropBannerPool();
		}
	}

	public void OnEnable()
	{
		if (current_item == null)
		{
			empty_drop_icon.SetActive(value: true);
			meta_title.setKeyAndUpdate("graph_drop_to_compare");
			meta_name.gameObject.SetActive(value: false);
		}
	}

	public void Update()
	{
		if (_disable_raycasts != disable_raycasts)
		{
			_disable_raycasts = disable_raycasts;
			if (disable_raycasts)
			{
				disableRaycastChildren();
			}
			else
			{
				enableRaycastChildren();
			}
		}
	}

	public void disableRaycastChildren()
	{
		_raycast_children.Clear();
		Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>();
		foreach (Graphic tGraphic in componentsInChildren)
		{
			if (!(tGraphic.gameObject == base.gameObject) && tGraphic.raycastTarget)
			{
				_raycast_children.Add(tGraphic);
				tGraphic.raycastTarget = false;
			}
		}
	}

	public void enableRaycastChildren()
	{
		foreach (Graphic raycast_child in _raycast_children)
		{
			raycast_child.raycastTarget = true;
		}
		_raycast_children.Clear();
	}

	public void OnDrop(PointerEventData pEventData)
	{
		if (pEventData.pointerDrag == null)
		{
			return;
		}
		BannerBase tBannerBase = pEventData.pointerDrag.GetComponent<BannerBase>();
		if (!(tBannerBase == null))
		{
			GraphCompareMetaSelector tSelector = pEventData.pointerDrag.GetComponent<GraphCompareMetaSelector>();
			if (!(tSelector == null) && tSelector.isBeingDragged())
			{
				tSelector.OnEndDrag(pEventData);
				SoundBox.click();
				setObjectAndUpdate(tBannerBase.GetNanoObject());
				pEventData.Use();
			}
		}
	}

	public void empty()
	{
		init();
		clearObject();
		empty_drop_icon.SetActive(value: true);
	}

	public void clear()
	{
		init();
		Config.selected_objects_graph.Remove(current_item);
		clearObject();
		empty_drop_icon.SetActive(value: true);
	}

	public void clearAndSetObject(NanoObject pObject)
	{
		clear();
		setObject(pObject);
	}

	public void setObject(NanoObject pObject)
	{
		if (!pObject.isRekt())
		{
			empty_drop_icon.SetActive(value: false);
			current_item = pObject;
			_current_banner = _graph_window.setupBanner(current_item, base.transform, _pool_drop_banners);
			_current_banner.jump();
			_current_banner.GetComponent<Button>().onClick.AddListener(removeOnClick);
			if (!Config.selected_objects_graph.Contains(current_item))
			{
				Config.selected_objects_graph.Add(current_item);
			}
			Color tColor = current_item.getColor().getColorText();
			meta_title.text.color = tColor;
			MetaCustomizationAsset tCurrentMetaAsset = AssetManager.meta_customization_library.getAsset(current_item.getMetaType());
			meta_title.setKeyAndUpdate(tCurrentMetaAsset.localization_title);
			meta_name.gameObject.SetActive(value: true);
			meta_name.text = current_item.name;
			meta_name.color = tColor;
		}
	}

	private void setObjectAndUpdate(NanoObject pObject)
	{
		string tActiveCategory = _graph_controller.getActiveCategory();
		clearAndSetObject(pObject);
		_graph_window.loadNoosItems(pSilent: true);
		_graph_controller.resetAndUpdateGraph();
		_graph_controller.tryEnableCategory(tActiveCategory);
	}

	private void removeOnClick()
	{
		SoundBox.click();
		if (!InputHelpers.mouseSupported && !Tooltip.isShowingFor(_current_banner))
		{
			_current_banner.showTooltip();
		}
		else
		{
			setObjectAndUpdate(null);
		}
	}

	private void clearObject()
	{
		if (current_item != null)
		{
			releaseChild();
			current_item = null;
			meta_title.text.color = Toolbox.color_text_default;
			meta_name.color = Toolbox.color_text_default;
			meta_name.gameObject.SetActive(value: false);
		}
	}

	private void releaseChild()
	{
		if (_current_banner != null)
		{
			_current_banner.GetComponent<Button>().onClick.RemoveListener(removeOnClick);
			_pool_drop_banners.resetParent(_current_banner);
			_pool_drop_banners.release(_current_banner);
			_current_banner = null;
		}
	}
}
