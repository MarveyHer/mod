using System.Collections.Generic;
using UnityEngine;

public class MultiBannerPool
{
	private Dictionary<string, ObjectPoolGenericMono<MonoBehaviour>> _pool_banners;

	private Transform _pool_container;

	private Transform _prefab_area;

	public MultiBannerPool(Transform pPoolContainer)
	{
		_pool_banners = new Dictionary<string, ObjectPoolGenericMono<MonoBehaviour>>();
		_pool_container = pPoolContainer;
		GameObject tNewArea = new GameObject("PrefabArea", typeof(RectTransform));
		tNewArea.transform.SetParent(_pool_container);
		_prefab_area = tNewArea.transform;
		_prefab_area.gameObject.SetActive(value: false);
	}

	public IBanner getNext(NanoObject pObject)
	{
		string tBannerType = pObject.getType();
		MetaCustomizationAsset tMetaAsset = AssetManager.meta_customization_library.get(tBannerType);
		if (!_pool_banners.TryGetValue(tBannerType, out var tPoolElements))
		{
			GameObject tBannerArea = new GameObject("BannerArea " + tBannerType, typeof(RectTransform));
			tBannerArea.transform.SetParent(_pool_container, worldPositionStays: false);
			MonoBehaviour tPrefabItem = (MonoBehaviour)tMetaAsset.get_banner(tMetaAsset, pObject, _prefab_area);
			tPrefabItem.gameObject.name = tBannerType;
			_pool_banners.Add(tBannerType, new ObjectPoolGenericMono<MonoBehaviour>(tPrefabItem, tBannerArea.transform));
			tPoolElements = _pool_banners[tBannerType];
		}
		return tPoolElements.getNext() as IBanner;
	}

	public void release(IBanner pItem)
	{
		getItemPool(pItem).release(pItem as MonoBehaviour);
	}

	public void resetParent(IBanner pItem)
	{
		getItemPool(pItem).resetParent(pItem as MonoBehaviour);
	}

	private ObjectPoolGenericMono<MonoBehaviour> getItemPool(IBanner pItem)
	{
		MetaCustomizationAsset tCurrentMetaAsset = pItem.meta_asset;
		if (_pool_banners.TryGetValue(tCurrentMetaAsset.id, out var tBannerPool))
		{
			return tBannerPool;
		}
		return null;
	}

	public void clear()
	{
		foreach (ObjectPoolGenericMono<MonoBehaviour> value in _pool_banners.Values)
		{
			value.clear();
			value.resetParent();
		}
	}
}
