using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseDebugAssetsComponent<TAsset, TAssetElement, TAssetElementPlace> : MonoBehaviour where TAsset : Asset where TAssetElement : BaseDebugAssetElement<TAsset> where TAssetElementPlace : BaseAssetElementPlace<TAsset, TAssetElement>
{
	public TAssetElementPlace place_prefab;

	public TAssetElement element_prefab;

	public ScrollRect scroll_rect;

	private RectTransform _scroll_rect_transform;

	private Rect _scroll_world_rect;

	public InputField search_input_field;

	public SortingTab sorting_tab;

	protected List<TAsset> list_assets_sorted;

	protected List<TAsset> list_assets_sorting;

	protected List<TAsset> list_assets_sorting_default;

	protected bool default_sort_reversed;

	protected List<TAssetElementPlace> list_places;

	private bool _initialized;

	protected virtual List<TAsset> getAssetsList()
	{
		throw new NotImplementedException();
	}

	protected virtual List<TAsset> getListCivsSort()
	{
		throw new NotImplementedException();
	}

	private void OnEnable()
	{
		refresh();
	}

	private void Start()
	{
		_scroll_rect_transform = scroll_rect.GetComponent<RectTransform>();
		search_input_field.onValueChanged.AddListener(setDataSearched);
		init();
	}

	protected virtual void init()
	{
		list_assets_sorted = new List<TAsset>(getAssetsList());
		list_assets_sorting = new List<TAsset>(getAssetsList());
		list_assets_sorting_default = new List<TAsset>(getAssetsList());
		foreach (Transform item in base.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		list_places = new List<TAssetElementPlace>();
		foreach (TAsset tAsset in getAssetsList())
		{
			TAssetElementPlace tPlace = UnityEngine.Object.Instantiate(place_prefab, base.transform);
			list_places.Add(tPlace);
			tPlace.setData(tAsset, element_prefab);
		}
		sorting_tab.addButton("ui/Icons/iconHumans", "sort_by_civs", setDataResorted, delegate
		{
			list_assets_sorted = getListCivsSort();
		});
		sorting_tab.addButton("ui/Icons/actor_traits/iconClumsy", "default_sort", setDataResorted, delegate
		{
			list_assets_sorted = list_assets_sorting_default;
			if (sorting_tab.getCurrentButton().getState() == SortButtonState.Down || default_sort_reversed)
			{
				default_sort_reversed = !default_sort_reversed;
				list_assets_sorted.Reverse();
			}
		}).click();
		_initialized = true;
	}

	private void Update()
	{
		if (!_initialized)
		{
			return;
		}
		_scroll_world_rect = _scroll_rect_transform.GetWorldRect();
		foreach (TAssetElementPlace tPlace in list_places)
		{
			if (tPlace.game_object_cache.activeSelf)
			{
				if (tPlace.element != null)
				{
					tPlace.element.update();
				}
				checkVisible(tPlace);
			}
		}
	}

	private void checkVisible(TAssetElementPlace pPlace)
	{
		if (pPlace.gameObject.activeSelf)
		{
			bool tIsVisible = isElementVisible(pPlace);
			if (!tIsVisible && pPlace.has_element)
			{
				pPlace.clear();
			}
			else if (tIsVisible && !pPlace.has_element)
			{
				TAsset tAsset = list_assets_sorted[pPlace.rect_transform.GetSiblingIndex()];
				pPlace.setData(tAsset, element_prefab);
			}
		}
	}

	public void refresh()
	{
		if (_initialized)
		{
			setDataResorted();
		}
	}

	public bool isElementVisible(TAssetElementPlace pPlace)
	{
		return _scroll_world_rect.Overlaps(pPlace.rect_transform.GetWorldRect());
	}

	protected void setDataResorted()
	{
		int tLastIndex = list_assets_sorted.Count - 1;
		for (int i = 0; i < list_places.Count; i++)
		{
			TAssetElementPlace tPlace = list_places[i];
			if (i > tLastIndex)
			{
				tPlace.game_object_cache.SetActive(value: false);
				tPlace.allowed_for_search = false;
				continue;
			}
			tPlace.game_object_cache.SetActive(value: true);
			tPlace.allowed_for_search = true;
			if (isElementVisible(tPlace) && tPlace.has_element)
			{
				TAsset tAsset = list_assets_sorted[i];
				tPlace.element.setData(tAsset);
			}
		}
		setDataSearched(search_input_field.text);
	}

	protected void checkReverseSort()
	{
		if (sorting_tab.getCurrentButton().getState() == SortButtonState.Down)
		{
			list_assets_sorted.Reverse();
		}
	}

	private void setDataSearched(string pValue)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		pValue = pValue.ToLower();
		if (string.IsNullOrEmpty(pValue))
		{
			foreach (TAssetElementPlace tPlace in list_places)
			{
				if (tPlace.allowed_for_search)
				{
					tPlace.game_object_cache.SetActive(value: true);
				}
			}
			return;
		}
		for (int i = 0; i < list_assets_sorted.Count; i++)
		{
			TAssetElementPlace tPlace2 = list_places[i];
			if (tPlace2.allowed_for_search)
			{
				bool tContains = list_assets_sorted[i].id.ToLower().Contains(pValue);
				tPlace2.game_object_cache.SetActive(tContains);
			}
		}
	}
}
