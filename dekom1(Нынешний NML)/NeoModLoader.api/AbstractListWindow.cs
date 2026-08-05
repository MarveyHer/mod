using System.Collections.Generic;
using NeoModLoader.General;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.api;

public abstract class AbstractListWindow<T, TItem> : AbstractWindow<T> where T : AbstractListWindow<T, TItem>
{
	protected static AbstractListWindowItem<TItem> ItemPrefab;

	private ObjectPoolGenericMono<AbstractListWindowItem<TItem>> _pool;

	protected Dictionary<TItem, AbstractListWindowItem<TItem>> ItemMap = new Dictionary<TItem, AbstractListWindowItem<TItem>>();

	protected virtual void AddItemToList(TItem item)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (_pool == null)
		{
			_pool = new ObjectPoolGenericMono<AbstractListWindowItem<TItem>>(ItemPrefab, base.ContentTransform);
		}
		if (!ItemMap.TryGetValue(item, out var value))
		{
			value = _pool.getNext();
			ItemMap[item] = value;
		}
		((Component)value).transform.localScale = Vector3.one;
		value.Setup(item);
	}

	protected virtual void RemoveItemFromList(TItem item)
	{
		if (ItemMap.TryGetValue(item, out var value))
		{
			if (((Component)value).gameObject.activeSelf)
			{
				((Component)value).gameObject.SetActive(false);
			}
			_pool._elements_inactive.Enqueue(value);
			ItemMap.Remove(item);
		}
	}

	protected virtual void ClearList()
	{
		_pool?.clear();
		ItemMap.Clear();
	}

	public new static T CreateAndInit(string pWindowId)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Expected O, but got Unknown
		ScrollWindow scrollWindow = WindowCreator.CreateEmptyWindow(pWindowId, pWindowId + " Title");
		GameObject gameObject = ((Component)scrollWindow).gameObject;
		AbstractWindow<T>.Instance = gameObject.AddComponent<T>();
		((Component)AbstractWindow<T>.Instance).gameObject.SetActive(false);
		AbstractWindow<T>.Instance.BackgroundTransform = ((Component)scrollWindow).transform.Find("Background");
		((Component)AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View")).gameObject.SetActive(true);
		((Component)AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View")).GetComponent<RectTransform>().sizeDelta = new Vector2(232f, 270f);
		AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View").localPosition = new Vector3(0f, -6f);
		((Component)AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View/Viewport")).GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 0f);
		AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View/Viewport").localPosition = new Vector3(-131f, 135f);
		AbstractWindow<T>.Instance.ContentTransform = AbstractWindow<T>.Instance.BackgroundTransform.Find("Scroll View/Viewport/Content");
		VerticalLayoutGroup val = ((Component)AbstractWindow<T>.Instance.ContentTransform).gameObject.AddComponent<VerticalLayoutGroup>();
		ContentSizeFitter val2 = ((Component)AbstractWindow<T>.Instance.ContentTransform).gameObject.AddComponent<ContentSizeFitter>();
		((HorizontalOrVerticalLayoutGroup)val).childControlWidth = true;
		((HorizontalOrVerticalLayoutGroup)val).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandWidth = true;
		((HorizontalOrVerticalLayoutGroup)val).childForceExpandHeight = false;
		((LayoutGroup)val).childAlignment = (TextAnchor)4;
		((HorizontalOrVerticalLayoutGroup)val).spacing = 10f;
		((LayoutGroup)val).padding = new RectOffset(30, 30, 10, 10);
		val2.verticalFit = (FitMode)2;
		ItemPrefab = AbstractWindow<T>.Instance.CreateItemPrefab();
		AbstractWindow<T>.Instance.Init();
		AbstractWindow<T>.Instance.Initialized = true;
		return AbstractWindow<T>.Instance;
	}

	protected abstract AbstractListWindowItem<TItem> CreateItemPrefab();
}
