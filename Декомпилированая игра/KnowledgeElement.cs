using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeElement : MonoBehaviour
{
	[SerializeField]
	private LocalizedText _localized_text;

	[SerializeField]
	private Image _icon_left;

	[SerializeField]
	private Image _icon_right;

	[SerializeField]
	private EasterEggBanner _icon_easter_left;

	[SerializeField]
	private EasterEggBanner _icon_easter_right;

	[SerializeField]
	private StatBar _progress_bar;

	[SerializeField]
	private RunningIcons _running_icons;

	private CubeOverview _cube_overview_big;

	private WindowMetaTab _cube_tab;

	private KnowledgeAsset _asset;

	private int _running_icon_latest_index;

	private ILibraryWithUnlockables _library;

	private List<BaseUnlockableAsset> _assets_list = new List<BaseUnlockableAsset>();

	private int _items;

	private bool _initialized;

	private void OnEnable()
	{
		if (_initialized)
		{
			resetBar();
		}
	}

	private void Start()
	{
		init(_asset);
		resetBar();
	}

	public void setAsset(KnowledgeAsset pAsset)
	{
		_asset = pAsset;
	}

	public void setCube(CubeOverview pBigCube, WindowMetaTab pCubeTab)
	{
		_cube_overview_big = pBigCube;
		_cube_tab = pCubeTab;
	}

	private void init(KnowledgeAsset pAsset)
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		_asset = pAsset;
		_localized_text.setKeyAndUpdate(_asset.getLocaleID());
		Sprite tIcon = _asset.getIcon();
		_icon_left.sprite = tIcon;
		_icon_left.GetComponentInParent<Button>().onClick.AddListener(delegate
		{
			_asset.click_icon_action(_asset);
		});
		_icon_right.GetComponentInParent<Button>().onClick.AddListener(delegate
		{
			_cube_overview_big.setFilterAsset(_asset);
			_cube_tab.tab_action.Invoke(_cube_tab);
		});
		_library = _asset.get_library();
		foreach (BaseUnlockableAsset tAsset in _library.elements_list)
		{
			if (tAsset.show_in_knowledge_window)
			{
				_assets_list.Add(tAsset);
			}
		}
		_assets_list.Shuffle();
		_running_icons.init(prevItem, nextItem);
		using ListPool<Vector3> tPositions = new ListPool<Vector3>(_running_icons.transform.childCount);
		foreach (Transform tChild in _running_icons.transform)
		{
			tPositions.Add(tChild.localPosition);
			Object.Destroy(tChild.gameObject);
		}
		Transform tPrefab = Resources.Load<Transform>(pAsset.button_prefab_path);
		foreach (ref Vector3 item in tPositions)
		{
			Vector3 tPosition = item;
			Transform tElement = Object.Instantiate(tPrefab, _running_icons.transform);
			tElement.transform.localPosition = tPosition;
			tElement.SetSiblingIndex(_items++);
			if (!tElement.HasComponent<RunningIcon>())
			{
				tElement.AddComponent<RunningIcon>();
			}
			_running_icons.addIcon(tElement.GetComponent<RunningIcon>());
			Button componentInChildren = tElement.GetComponentInChildren<Button>();
			componentInChildren.enabled = false;
			componentInChildren.OnHover(delegate
			{
				_running_icons.toggle(pState: false);
			});
			componentInChildren.OnHoverOut(delegate
			{
				_running_icons.toggle(pState: true);
			});
			if (tElement.TryGetComponent<DraggableLayoutElement>(out var tDraggable))
			{
				tDraggable.enabled = false;
			}
			BaseUnlockableAsset tAsset2 = getNextAsset();
			_asset.load_button(tElement, tAsset2);
			_asset.tip_button_loader?.Invoke(tElement, tAsset2);
		}
		checkEasterEggsSprite();
	}

	private void checkEasterEggsSprite()
	{
		if (string.IsNullOrEmpty(_asset.path_icon_easter_egg))
		{
			_icon_easter_left.gameObject.SetActive(value: false);
			_icon_easter_right.gameObject.SetActive(value: false);
		}
		else
		{
			Sprite tIcon = SpriteTextureLoader.getSprite(_asset.path_icon_easter_egg);
			_icon_easter_left.main_image.sprite = tIcon;
			_icon_easter_right.main_image.sprite = tIcon;
		}
	}

	private void resetBar()
	{
		int tCurrent = _asset.countUnlockedByPlayer();
		int tMax = _asset.countTotal();
		_progress_bar.setBar(tCurrent, tMax, "/" + tMax.ToText());
	}

	private void nextItem(Transform pButton)
	{
		BaseUnlockableAsset tAsset = getNextAsset();
		_asset.load_button(pButton, tAsset);
		_asset.tip_button_loader?.Invoke(pButton, tAsset);
	}

	private BaseUnlockableAsset getNextAsset()
	{
		_running_icon_latest_index++;
		int tIndex = (_running_icon_latest_index = Toolbox.loopIndex(_running_icon_latest_index, _assets_list.Count));
		return _assets_list[tIndex];
	}

	private void prevItem(Transform pButton)
	{
		BaseUnlockableAsset tAsset = getPrevAsset();
		_asset.load_button(pButton, tAsset);
		_asset.tip_button_loader?.Invoke(pButton, tAsset);
	}

	private BaseUnlockableAsset getPrevAsset()
	{
		_running_icon_latest_index--;
		int tPrevIndex = Toolbox.loopIndex((_running_icon_latest_index = Toolbox.loopIndex(_running_icon_latest_index, _assets_list.Count)) - _items + 1, _assets_list.Count);
		return _assets_list[tPrevIndex];
	}
}
