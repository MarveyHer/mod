using System;
using UnityEngine;
using UnityEngine.UI;

public class ListWindow : MonoBehaviour
{
	[SerializeField]
	private Transform _content_list;

	[SerializeField]
	private WindowMetaTab _tab_list;

	[SerializeField]
	private WindowMetaTab _tab_favorite;

	[SerializeField]
	private WindowMetaTab _tab_dead;

	[SerializeField]
	private WindowMetaTabButtonsContainer _tabs_container;

	[SerializeField]
	private Image _art;

	[SerializeField]
	private Image _tab_list_icon;

	[SerializeField]
	private Image _title_icon_left;

	[SerializeField]
	private Image _title_icon_right;

	[SerializeField]
	private Image _no_items_icon_left;

	[SerializeField]
	private Image _no_items_icon_right;

	[SerializeField]
	private TipButton _tip_button_favorite;

	[SerializeField]
	private LocalizedText _no_items_description;

	[SerializeField]
	private ListWindowStatistics _statistics;

	[SerializeField]
	private MetaRepresentationTotal _breakdown;

	[SerializeField]
	private GameObject _no_items;

	[SerializeField]
	private SortingTab _sorting_tab;

	[SerializeField]
	private Transform _list_transform;

	[SerializeField]
	private ScrollRect _scroll_rect;

	[SerializeField]
	private Text _title_counter;

	[SerializeField]
	private Text _favorites_counter;

	[SerializeField]
	private Text _dead_counter;

	[SerializeField]
	private GameObject _list_element_prefab;

	[SerializeField]
	private MetaType _meta_type;

	private ListWindowAsset _asset;

	private void Awake()
	{
		_asset = AssetManager.list_window_library.getByMetaType(_meta_type);
		_list_transform.gameObject.SetActive(value: false);
		IComponentList tComponent = _asset.set_list_component(_list_transform);
		initComponent(tComponent);
		initTabsCallbacks(tComponent);
		_tab_list.tab_action.AddListener(delegate
		{
			_no_items_description.setKeyAndUpdate(_asset.no_items_locale);
		});
		_no_items_description.setKeyAndUpdate(_asset.no_items_locale);
		if (_statistics != null)
		{
			_statistics.meta_type = _meta_type;
		}
		_list_transform.gameObject.SetActive(value: true);
		_art.sprite = SpriteTextureLoader.getSprite(_asset.art_path);
		Sprite tIcon = SpriteTextureLoader.getSprite(_asset.icon_path);
		_tab_list_icon.sprite = tIcon;
		_title_icon_left.sprite = tIcon;
		_title_icon_right.sprite = tIcon;
		_no_items_icon_left.sprite = tIcon;
		_no_items_icon_right.sprite = tIcon;
		if (_breakdown != null)
		{
			_breakdown.setMetaType(_meta_type);
		}
	}

	protected virtual void initComponent(IComponentList pComponent)
	{
		pComponent.init(_no_items, _sorting_tab, _list_element_prefab, _list_transform, _scroll_rect, _title_counter, _favorites_counter, _dead_counter);
	}

	protected virtual void initTabsCallbacks(IComponentList pComponent)
	{
		bool tHasFavorites = _tab_favorite != null && _tab_favorite.gameObject.activeSelf;
		bool tHasDead = _tab_dead != null && _tab_dead.gameObject.activeSelf;
		if (tHasFavorites || tHasDead)
		{
			setTabCallbacks(_tab_list, pComponent.setShowAll, pComponent.setDefault);
			if (tHasFavorites)
			{
				setTabCallbacks(_tab_favorite, pComponent.setShowFavoritesOnly, pComponent.setDefault);
				_tab_favorite.tab_action.AddListener(delegate
				{
					_no_items_description.setKeyAndUpdate("empty_favorites_list");
				});
			}
			if (tHasDead)
			{
				setTabCallbacks(_tab_list, pComponent.setShowAliveOnly, pComponent.setDefault);
				setTabCallbacks(_tab_dead, pComponent.setShowDeadOnly, pComponent.setDefault);
				_tab_dead.tab_action.AddListener(delegate
				{
					_no_items_description.setKeyAndUpdate(_asset.no_dead_items_locale);
				});
			}
		}
		else
		{
			_tab_list.tab_action.AddListener(delegate
			{
				_tabs_container.showTab(_tab_list);
			});
		}
	}

	protected void setTabCallbacks(WindowMetaTab pTab, Action pCallback, Action pDefaultCallback = null)
	{
		pTab.tab_action.RemoveAllListeners();
		if (pDefaultCallback != null)
		{
			pTab.tab_action.AddListener(delegate
			{
				pDefaultCallback();
			});
		}
		pTab.tab_action.AddListener(delegate
		{
			pCallback();
		});
		pTab.tab_action.AddListener(delegate(WindowMetaTab p)
		{
			_tabs_container.showTab(p);
		});
	}

	protected LocalizedText getNoItems()
	{
		return _no_items_description;
	}
}
