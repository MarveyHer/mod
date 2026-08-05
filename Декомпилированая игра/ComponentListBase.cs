using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ComponentListBase<TListElement, TMetaObject, TData, TComponent> : MonoBehaviour, IComponentList, IShouldRefreshWindow where TListElement : WindowListElementBase<TMetaObject, TData> where TMetaObject : CoreSystemObject<TData> where TData : BaseSystemData where TComponent : ComponentListBase<TListElement, TMetaObject, TData, TComponent>
{
	public GameObject no_items;

	public SortingTab sorting_tab;

	public TListElement element_prefab;

	public Transform list_transform;

	public ScrollRect scroll_rect;

	[SerializeField]
	private Text _title_counter;

	[SerializeField]
	private Text _favorites_counter;

	[SerializeField]
	private Text _dead_counter;

	private ListItemsFilter _show_items;

	public GetListOfObjectsFunc<TListElement, TMetaObject, TData, TComponent> get_objects_delegate = getObjects;

	private ObjectPoolGenericMono<TListElement> _pool_elements;

	private ObjectPoolGenericMono<BaseEmptyListMono> _pool_empty_elements;

	protected Comparison<TMetaObject> current_sort;

	public readonly List<NanoObject> meta_list = new List<NanoObject>();

	private bool autolayout_done;

	private const int PADDING_ELEMENTS = 3;

	private static readonly bool _debug;

	private bool _created;

	protected int latest_counted;

	private float _element_height;

	protected virtual MetaType meta_type
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	private MetaTypeAsset _meta_type_asset => AssetManager.meta_type_library.getAsset(meta_type);

	protected virtual bool change_asset_sort_order => true;

	protected virtual IEnumerable<TMetaObject> getObjectsList()
	{
		return get_objects_delegate((TComponent)this);
	}

	protected ObjectPoolGenericMono<BaseEmptyListMono> getPoolEmpty()
	{
		return _pool_empty_elements;
	}

	private void checkCreate()
	{
		if (!_created)
		{
			_created = true;
			create();
		}
	}

	protected virtual void create()
	{
		_pool_elements = new ObjectPoolGenericMono<TListElement>(element_prefab, list_transform);
		_element_height = element_prefab.transform.GetComponent<RectTransform>().sizeDelta.y;
		addEmptyPoolSystem();
		showSortingTabs();
	}

	protected virtual void setupSortingTabs()
	{
	}

	protected virtual void showSortingTabs()
	{
		sorting_tab.clearButtons();
		setupSortingTabs();
		sorting_tab.enableFirstIfNone();
	}

	private void OnRenderObject()
	{
		autolayout_done = true;
	}

	private void LateUpdate()
	{
		if (!autolayout_done)
		{
			return;
		}
		IReadOnlyList<BaseEmptyListMono> tList = _pool_empty_elements.getListTotal();
		int tFirstVisible = int.MaxValue;
		int tLastVisible = int.MinValue;
		float tScrollRectBottom = scroll_rect.content.localPosition.y;
		float tScrollRectTop = tScrollRectBottom + scroll_rect.viewport.rect.height;
		for (int i = 0; i < tList.Count; i++)
		{
			BaseEmptyListMono tEmptyMono = tList[i];
			if (!tEmptyMono.gameObject.activeSelf)
			{
				continue;
			}
			if (IsVisibleInScrollRect(tEmptyMono.rect_transform, scroll_rect, tScrollRectTop, tScrollRectBottom))
			{
				if (tFirstVisible == int.MaxValue)
				{
					tFirstVisible = i;
				}
				tLastVisible = i;
			}
			else if (tLastVisible > int.MinValue)
			{
				break;
			}
		}
		if (tLastVisible == int.MaxValue || tFirstVisible == int.MinValue)
		{
			return;
		}
		int tFirstPadding = Math.Max(0, tFirstVisible - 3);
		int tLastPadding = Math.Min(tList.Count - 1, tLastVisible + 3);
		for (int j = 0; j < tList.Count; j++)
		{
			if (j < tFirstPadding || j > tLastPadding)
			{
				BaseEmptyListMono tEmptyMono2 = tList[j];
				releaseElement(tEmptyMono2);
			}
		}
		for (int k = tFirstPadding; k <= tLastPadding; k++)
		{
			BaseEmptyListMono tEmptyMono3 = tList[k];
			if (tEmptyMono3.gameObject.activeSelf && !tEmptyMono3.hasElement())
			{
				makeElementVisible(tEmptyMono3);
			}
		}
		if (_debug)
		{
			debugUpdateElementNames(tList, tScrollRectTop, tScrollRectBottom);
		}
	}

	private void makeElementVisible(BaseEmptyListMono pEmptyMono)
	{
		TListElement tElement = _pool_elements.getNext();
		tElement.show((TMetaObject)pEmptyMono.meta_object);
		tElement.transform.SetParent(pEmptyMono.transform);
		tElement.transform.localPosition = Vector3.zero;
		pEmptyMono.assignElement(tElement);
	}

	private bool IsVisibleInScrollRect(RectTransform pRectTransform, ScrollRect pScrollRect, float pScrollRectTop, float pScrollRectBottom)
	{
		Vector2 tLocal = pRectTransform.localPosition;
		tLocal *= -1f;
		float tHeight = pRectTransform.sizeDelta.y * 0.6f;
		if (tLocal.y <= pScrollRectTop + tHeight + base.transform.localPosition.y)
		{
			return tLocal.y >= pScrollRectBottom - tHeight + base.transform.localPosition.y;
		}
		return false;
	}

	private void addEmptyPoolSystem()
	{
		BaseEmptyListMono tEmptyObject = Resources.Load<BaseEmptyListMono>("ui/list_element_empty");
		tEmptyObject = UnityEngine.Object.Instantiate(tEmptyObject, list_transform);
		tEmptyObject.gameObject.SetActive(value: false);
		if (_element_height > 0f && tEmptyObject.TryGetComponent<LayoutElement>(out var tLayoutElement))
		{
			tLayoutElement.minHeight = _element_height;
		}
		_pool_empty_elements = new ObjectPoolGenericMono<BaseEmptyListMono>(tEmptyObject, list_transform);
	}

	private void showElement(TMetaObject pObject)
	{
		_pool_empty_elements.getNext().assignObject(pObject);
	}

	protected static IEnumerable<TMetaObject> getObjects(ComponentListBase<TListElement, TMetaObject, TData, TComponent> pComponentList)
	{
		IEnumerable<TMetaObject> tList = pComponentList._meta_type_asset.get_list().Cast<TMetaObject>();
		foreach (TMetaObject item in pComponentList.getFiltered(tList))
		{
			yield return item;
		}
	}

	protected virtual IEnumerable<TMetaObject> getFiltered(IEnumerable<TMetaObject> pList)
	{
		switch (getCurrentFilter())
		{
		case ListItemsFilter.Favorites:
			foreach (TMetaObject tMeta3 in pList)
			{
				if (tMeta3.isFavorite())
				{
					yield return tMeta3;
				}
			}
			yield break;
		case ListItemsFilter.Dead:
			foreach (TMetaObject tMeta2 in pList)
			{
				if (tMeta2.hasDied())
				{
					yield return tMeta2;
				}
			}
			yield break;
		case ListItemsFilter.OnlyAlive:
			foreach (TMetaObject tMeta in pList)
			{
				if (!tMeta.hasDied())
				{
					yield return tMeta;
				}
			}
			yield break;
		}
		foreach (TMetaObject p in pList)
		{
			yield return p;
		}
	}

	private void OnEnable()
	{
		checkCreate();
		showSortingTabs();
		show();
	}

	protected virtual void show()
	{
		if (!Config.game_loaded)
		{
			return;
		}
		clear();
		latest_counted = 0;
		if (isEmpty())
		{
			if (no_items != null)
			{
				no_items.SetActive(value: true);
			}
		}
		else
		{
			if (no_items != null)
			{
				no_items.SetActive(value: false);
			}
			showElements();
			latest_counted = _pool_empty_elements.countActive();
		}
		if (_title_counter != null)
		{
			_title_counter.text = latest_counted.ToString();
		}
		if (_favorites_counter != null)
		{
			_favorites_counter.text = latest_counted.ToString();
		}
		if (_dead_counter != null)
		{
			_dead_counter.text = latest_counted.ToString();
		}
		_pool_empty_elements.disableInactive();
		ScrollWindow.checkElements();
	}

	public ListPool<NanoObject> getElements()
	{
		meta_list.Clear();
		meta_list.AddRange(getObjectsList());
		meta_list.Sort((NanoObject a, NanoObject b) => current_sort(a as TMetaObject, b as TMetaObject));
		SortButton currentButton = sorting_tab.getCurrentButton();
		if ((object)currentButton != null && currentButton.getState() == SortButtonState.Down)
		{
			meta_list.Reverse();
		}
		return new ListPool<NanoObject>(meta_list);
	}

	protected void showElements()
	{
		using ListPool<NanoObject> tTempList = getElements();
		for (int i = 0; i < tTempList.Count; i++)
		{
			NanoObject tObject = tTempList[i];
			showElement(tObject as TMetaObject);
		}
		if (change_asset_sort_order)
		{
			_meta_type_asset.setListGetter(getElements);
		}
	}

	public virtual bool isEmpty()
	{
		IEnumerable<TMetaObject> tList = getObjectsList();
		if (tList == null)
		{
			return true;
		}
		return !tList.Any();
	}

	public virtual void clear()
	{
		IReadOnlyList<BaseEmptyListMono> tList = _pool_empty_elements.getListTotal();
		for (int ii = 0; ii < tList.Count; ii++)
		{
			BaseEmptyListMono tEmptyMono = tList[ii];
			releaseElement(tEmptyMono);
			tEmptyMono.clearObject();
		}
		_pool_empty_elements.clear();
		_pool_elements.resetParent();
		meta_list.Clear();
		_meta_type_asset.setListGetter(null);
	}

	private void releaseElement(BaseEmptyListMono pEmptyMono)
	{
		if (pEmptyMono.hasElement())
		{
			TListElement tElement = (TListElement)pEmptyMono.element;
			pEmptyMono.clearElement();
			_pool_elements.release(tElement);
		}
	}

	private void debugUpdateElementNames(IReadOnlyList<BaseEmptyListMono> pList, float pScrollRectTop, float pScrollRectBottom)
	{
		for (int i = 0; i < pList.Count; i++)
		{
			BaseEmptyListMono tEmptyMono = pList[i];
			bool tIsVisible = IsVisibleInScrollRect(tEmptyMono.rect_transform, scroll_rect, pScrollRectTop, pScrollRectBottom);
			tEmptyMono.debugUpdateName(tIsVisible);
		}
	}

	private void OnDisable()
	{
		clear();
	}

	public void setShowFavoritesOnly()
	{
		_show_items = ListItemsFilter.Favorites;
	}

	public void setShowAll()
	{
		_show_items = ListItemsFilter.All;
	}

	public void setShowDeadOnly()
	{
		_show_items = ListItemsFilter.Dead;
	}

	public void setShowAliveOnly()
	{
		_show_items = ListItemsFilter.OnlyAlive;
	}

	public virtual void setDefault()
	{
	}

	public ListItemsFilter getCurrentFilter()
	{
		return _show_items;
	}

	public void init(GameObject pNoItems, SortingTab pSortingTab, GameObject pListElementPrefab, Transform pListTransform, ScrollRect pScrollRect, Text pTitleCounter, Text pFavoritesCounter, Text pDeadCounter)
	{
		no_items = pNoItems;
		sorting_tab = pSortingTab;
		element_prefab = pListElementPrefab.GetComponent<TListElement>();
		list_transform = pListTransform;
		scroll_rect = pScrollRect;
		_title_counter = pTitleCounter;
		_favorites_counter = pFavoritesCounter;
		_dead_counter = pDeadCounter;
	}

	public virtual bool checkRefreshWindow()
	{
		foreach (NanoObject item in meta_list)
		{
			if (item.isRekt())
			{
				return true;
			}
		}
		return false;
	}

	protected void genericMetaSortByAge(Comparison<TMetaObject> pAction)
	{
		sorting_tab.tryAddButton("ui/Icons/iconAge", "sort_by_age", show, delegate
		{
			current_sort = pAction;
		});
	}

	protected void genericMetaSortByRenown(Comparison<TMetaObject> pAction)
	{
		sorting_tab.tryAddButton("ui/Icons/iconRenown", "sort_by_renown", show, delegate
		{
			current_sort = pAction;
		});
	}

	protected void genericMetaSortByPopulation(Comparison<TMetaObject> pAction)
	{
		sorting_tab.tryAddButton("ui/Icons/iconPopulation", "sort_by_members", show, delegate
		{
			current_sort = pAction;
		});
	}

	protected void genericMetaSortByKills(Comparison<TMetaObject> pAction)
	{
		sorting_tab.tryAddButton("ui/Icons/iconKills", "sort_by_kills", show, delegate
		{
			current_sort = pAction;
		});
	}

	protected void genericMetaSortByDeath(Comparison<TMetaObject> pAction)
	{
		sorting_tab.tryAddButton("ui/Icons/iconDead", "sort_by_dead", show, delegate
		{
			current_sort = pAction;
		});
	}

	protected int sortByRenown(IMetaObject p1, IMetaObject p2)
	{
		return p2.getRenown().CompareTo(p1.getRenown());
	}

	protected int sortByAge(IMetaObject p1, IMetaObject p2)
	{
		return -p2.getMetaData().created_time.CompareTo(p1.getMetaData().created_time);
	}

	public static int sortByPopulation(IMetaObject p1, IMetaObject p2)
	{
		return p2.getPopulationPeople().CompareTo(p1.getPopulationPeople());
	}

	public static int sortByKills(IMetaObject p1, IMetaObject p2)
	{
		return p2.getTotalKills().CompareTo(p1.getTotalKills());
	}

	public static int sortByDeaths(IMetaObject p1, IMetaObject p2)
	{
		return p2.getTotalDeaths().CompareTo(p1.getTotalDeaths());
	}
}
