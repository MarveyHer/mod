using System.Collections;
using System.Collections.Generic;
using db;
using UnityEngine;
using UnityEngine.UI;

public class GraphCompareWindow : MonoBehaviour
{
	public GraphCompareMetaObject meta_object_1;

	public GraphCompareMetaObject meta_object_2;

	public GraphCompareMetaObject meta_object_3;

	public GraphController graph_controller;

	[SerializeField]
	private GameObject _empty_list_message;

	[SerializeField]
	private RectTransform _meta_drag_object;

	private ObjectPoolGenericMono<RectTransform> _pool_drag_objects;

	private MultiBannerPool _pool_banners;

	private MultiBannerPool _pool_drop_banners;

	[SerializeField]
	private Button _noos_button;

	[SerializeField]
	private Image _noos_icon;

	[SerializeField]
	private Transform _noos_list_container;

	[SerializeField]
	private Transform _pool_banner_container;

	[SerializeField]
	private Transform _pool_drop_banner_container;

	private MetaTypeAsset _current_asset;

	private List<MetaTypeAsset> _noos_list = new List<MetaTypeAsset>();

	private List<NanoObject> _noos_items = new List<NanoObject>();

	private Coroutine _load_noos_items;

	private const int VISIBLE_ITEMS = 6;

	[SerializeField]
	private CanvasGroup[] _block_during_random;

	private bool _is_randomizing;

	private bool _stop_randomizer;

	private void Awake()
	{
		foreach (Transform tChild in _noos_list_container)
		{
			if (tChild.gameObject.name.StartsWith("MetaContainer"))
			{
				Object.Destroy(tChild.gameObject);
			}
		}
		_pool_drag_objects = new ObjectPoolGenericMono<RectTransform>(_meta_drag_object, _noos_list_container);
		_pool_banners = new MultiBannerPool(_pool_banner_container);
		_pool_drop_banners = new MultiBannerPool(_pool_drop_banner_container);
		_noos_button.onClick.AddListener(delegate
		{
			nextNoos();
		});
	}

	internal MultiBannerPool getDropBannerPool()
	{
		return _pool_drop_banners;
	}

	private void OnEnable()
	{
		ScrollWindow.addCallbackHide(resetPoolsAndParents);
		loadNoos();
		if (hasAny())
		{
			if (Config.selected_objects_graph.Count == 0)
			{
				StartCoroutine(displayRandom());
			}
			else
			{
				StartCoroutine(displaySelected());
			}
		}
	}

	private IEnumerator selectNoosCoroutine()
	{
		if (Config.selected_objects_graph.Count != 0)
		{
			selectNoos(Config.selected_objects_graph.First());
			SoundBox.click();
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator updateGraph()
	{
		if (Config.selected_objects_graph.Count != 0)
		{
			string tActiveCategory = graph_controller.getActiveCategory();
			graph_controller.resetAndUpdateGraph();
			graph_controller.tryEnableCategory(tActiveCategory);
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator displaySelected(bool pUpdate = true)
	{
		if (Config.selected_objects_graph.Count == 0)
		{
			yield break;
		}
		using ListPool<NanoObject> tSelectedObjects = new ListPool<NanoObject>(3);
		tSelectedObjects.Add(Config.selected_objects_graph[0]);
		tSelectedObjects.Add(Config.selected_objects_graph[1]);
		tSelectedObjects.Add(Config.selected_objects_graph[2]);
		meta_object_1.empty();
		meta_object_2.empty();
		meta_object_3.empty();
		Config.selected_objects_graph.Clear();
		meta_object_1.setObject(tSelectedObjects[0]);
		yield return new WaitForEndOfFrame();
		meta_object_2.setObject(tSelectedObjects[1]);
		yield return new WaitForEndOfFrame();
		meta_object_3.setObject(tSelectedObjects[2]);
		yield return new WaitForEndOfFrame();
		if (pUpdate)
		{
			yield return selectNoosCoroutine();
			yield return updateGraph();
		}
	}

	private void OnDisable()
	{
		clearNoosItems();
		clearAsset();
	}

	private void clearAsset()
	{
		_current_asset = null;
	}

	private void loadNoos()
	{
		_noos_list.Clear();
		foreach (HistoryMetaDataAsset iAsset in AssetManager.history_meta_data_library.list)
		{
			MetaTypeAsset tAsset = AssetManager.meta_type_library.get(iAsset.id);
			if (tAsset.has_any())
			{
				_noos_list.Add(tAsset);
			}
		}
		showItems(hasAny());
	}

	private bool hasAny()
	{
		return _noos_list.Count > 0;
	}

	private void showItems(bool pShow)
	{
		Transform tContent = base.transform.FindRecursive("Content");
		for (int i = 0; i < tContent.childCount; i++)
		{
			tContent.GetChild(i).gameObject.SetActive(pShow);
		}
		_empty_list_message.SetActive(!pShow);
	}

	private void updateNoosIcon(MetaTypeAsset pAsset)
	{
		Sprite tImage = SpriteTextureLoader.getSprite("ui/Icons/" + pAsset.icon_list);
		_noos_icon.sprite = tImage;
	}

	public void clearNoosItems()
	{
		_noos_items.Clear();
		_pool_banners.clear();
		_pool_drag_objects.clear();
	}

	private void resetNoosList()
	{
		_noos_list_container.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
		_noos_list_container.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
		clearNoosItems();
	}

	private void resetPoolsAndParents(string pID)
	{
		if (!(pID != "chart_comparer"))
		{
			StopAllCoroutines();
			clearNoosItems();
			meta_object_1.empty();
			meta_object_2.empty();
			meta_object_3.empty();
			ScrollWindow.removeCallbackHide(resetPoolsAndParents);
		}
	}

	public IEnumerator loadNoosItemsCoroutine(bool pSilent = false)
	{
		resetNoosList();
		_noos_items.AddRange(_current_asset.get_list());
		_noos_items.Sort(sortByUnits);
		using ListPool<NanoObject> tItems = new ListPool<NanoObject>(_noos_items);
		int tCount = 0;
		foreach (ref NanoObject item in tItems)
		{
			NanoObject tItem = item;
			if (tItem != meta_object_1.current_item && tItem != meta_object_2.current_item && tItem != meta_object_3.current_item)
			{
				RectTransform tObject = _pool_drag_objects.getNext();
				tObject.gameObject.name = "MetaContainer " + tItem.getID();
				IBanner tBanner = setupDragBanner(tItem, tObject.transform, _pool_banners);
				if (tCount++ < 6)
				{
					tBanner.jump(0.1f, pSilent);
					yield return new WaitForEndOfFrame();
				}
			}
		}
	}

	public int countNoosItems()
	{
		return _noos_items.Count;
	}

	public static int sortByUnits(NanoObject pNanoObject1, NanoObject pNanoObject2)
	{
		return ((IMetaObject)pNanoObject2).countUnits().CompareTo(((IMetaObject)pNanoObject1).countUnits());
	}

	private void nextNoos()
	{
		int tIndex = _noos_list.IndexOf(_current_asset);
		tIndex = Toolbox.loopIndex(++tIndex, _noos_list.Count);
		selectNoos(_noos_list[tIndex]);
	}

	private void selectNoos(NanoObject pObject)
	{
		MetaTypeAsset tAsset = AssetManager.meta_type_library.get(pObject.getType());
		selectNoos(tAsset);
	}

	private void selectNoos(MetaTypeAsset pAsset)
	{
		if (_current_asset != pAsset)
		{
			clearNoosItems();
			_current_asset = pAsset;
			updateNoosIcon(_current_asset);
			loadNoosItems();
		}
	}

	public IBanner setupBanner(NanoObject pObject, Transform pBannerArea, MultiBannerPool pBannerPool)
	{
		IBanner tBanner = pBannerPool.getNext(pObject);
		tBanner.load(pObject);
		tBanner.transform.localScale = new Vector3(1f, 1f, 1f);
		tBanner.transform.SetParent(pBannerArea);
		UiButtonHoverAnimation component = tBanner.GetComponent<UiButtonHoverAnimation>();
		component.enabled = false;
		component.scale_size = 1f;
		component.default_scale = new Vector3(1f, 1f, 1f);
		tBanner.GetComponent<TipButton>().setDefaultScale(pBannerArea.localScale);
		if (!tBanner.HasComponent<LayoutElement>())
		{
			tBanner.AddComponent<LayoutElement>().ignoreLayout = true;
		}
		RectTransform component2 = tBanner.GetComponent<RectTransform>();
		component2.SetAnchor(AnchorPresets.MiddleCenter);
		component2.localScale = new Vector3(1f, 1f, 1f);
		component2.anchoredPosition = new Vector2(0f, 0f);
		return tBanner;
	}

	private IBanner setupDragBanner(NanoObject pObject, Transform pBannerArea, MultiBannerPool pBannerPool)
	{
		IBanner tBanner = setupBanner(pObject, pBannerArea, pBannerPool);
		if (!tBanner.HasComponent<GraphCompareMetaSelector>())
		{
			GraphCompareMetaSelector graphCompareMetaSelector = tBanner.AddComponent<GraphCompareMetaSelector>();
			graphCompareMetaSelector.addWindow(this);
			graphCompareMetaSelector.addDropzones(meta_object_1.GetComponent<RectTransform>(), meta_object_2.GetComponent<RectTransform>(), meta_object_3.GetComponent<RectTransform>());
		}
		return tBanner;
	}

	private ListPool<NanoObject> getPossibleItems()
	{
		ListPool<NanoObject> tPossibleItems = new ListPool<NanoObject>();
		foreach (MetaTypeAsset item in _noos_list)
		{
			foreach (NanoObject tItem in item.get_list())
			{
				tPossibleItems.Add(tItem);
			}
		}
		return tPossibleItems;
	}

	internal void loadNoosItems(bool pSilent = false)
	{
		if (_load_noos_items != null)
		{
			StopCoroutine(_load_noos_items);
		}
		_load_noos_items = StartCoroutine(loadNoosItemsCoroutine(pSilent));
	}

	private void selectRandom()
	{
		using ListPool<NanoObject> tPossibleItems = getPossibleItems();
		Config.selected_objects_graph.Clear();
		int tMax = Mathf.Min(tPossibleItems.Count, 3);
		foreach (NanoObject tItem in tPossibleItems.LoopRandom(tMax))
		{
			Config.selected_objects_graph.Add(tItem);
		}
		if (tPossibleItems.Count <= 7)
		{
			_stop_randomizer = true;
		}
	}

	public void randomizeSelection()
	{
		if (_is_randomizing)
		{
			_stop_randomizer = true;
			return;
		}
		StopAllCoroutines();
		StartCoroutine(displayRandom());
	}

	private IEnumerator displayRandom()
	{
		_is_randomizing = true;
		CanvasGroup[] block_during_random = _block_during_random;
		foreach (CanvasGroup obj in block_during_random)
		{
			obj.interactable = false;
			obj.blocksRaycasts = false;
		}
		for (int j = 0; j < 10; j++)
		{
			if (_stop_randomizer)
			{
				break;
			}
			selectRandom();
			yield return displaySelected(pUpdate: false);
			yield return randomizeCategories();
			yield return updateGraph();
			yield return randomNoosItems();
			updateNoosIcon(_noos_list.GetRandom());
		}
		yield return randomizeCategories();
		yield return randomizeTimescale();
		clearAsset();
		yield return selectNoosCoroutine();
		block_during_random = _block_during_random;
		foreach (CanvasGroup obj2 in block_during_random)
		{
			obj2.interactable = true;
			obj2.blocksRaycasts = true;
		}
		_stop_randomizer = false;
		_is_randomizing = false;
	}

	private IEnumerator randomizeCategories()
	{
		graph_controller.pickRandomCategory();
		SoundBox.click();
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator randomizeTimescale()
	{
		if (graph_controller.randomTimeScale())
		{
			SoundBox.click();
			yield return new WaitForEndOfFrame();
		}
	}

	public IEnumerator randomNoosItems()
	{
		resetNoosList();
		using ListPool<NanoObject> tPossibleItems = getPossibleItems();
		int tMax = Mathf.Min(6, tPossibleItems.Count);
		foreach (NanoObject tItem in tPossibleItems.LoopRandom(tMax))
		{
			RectTransform tObject = _pool_drag_objects.getNext();
			tObject.gameObject.name = "MetaContainer " + tItem.getID();
			IBanner tBanner = setupDragBanner(tItem, tObject.transform, _pool_banners);
			if (Randy.randomBool())
			{
				tBanner.jump(0.025f, pSilent: true);
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
