using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeoModLoader.utils;

public class WrappedAssetBundle
{
	private class AssetNode
	{
		public readonly Dictionary<string, AssetNode> children = new Dictionary<string, AssetNode>();

		public readonly List<string> resources_full_names = new List<string>();
	}

	private readonly AssetBundle assetBundle;

	private readonly Dictionary<string, AssetNode> direct_visit = new Dictionary<string, AssetNode>();

	private readonly AssetNode root = new AssetNode();

	public string Name => ((Object)assetBundle).name;

	internal WrappedAssetBundle(AssetBundle ab)
	{
		assetBundle = ab;
		string[] allAssetNames = ab.GetAllAssetNames();
		string[] array = allAssetNames;
		foreach (string text in array)
		{
			string[] array2 = text.Split('/');
			AssetNode assetNode = root;
			for (int j = 0; j < array2.Length - 1; j++)
			{
				string key = array2[j];
				if (!assetNode.children.TryGetValue(key, out var value))
				{
					value = new AssetNode();
					assetNode.children[key] = value;
				}
				assetNode = value;
			}
			assetNode.resources_full_names.Add(text);
		}
	}

	public string[] GetAllAssetNames()
	{
		return assetBundle.GetAllAssetNames();
	}

	public string[] GetAllScenePaths()
	{
		return assetBundle.GetAllScenePaths();
	}

	public Object GetObject(string pName)
	{
		return assetBundle.LoadAsset(pName);
	}

	public Object GetObject(string pName, Type type)
	{
		return assetBundle.LoadAsset(pName, type);
	}

	public T GetObject<T>(string pName) where T : Object
	{
		return assetBundle.LoadAsset<T>(pName);
	}

	public Object[] GetAllObjects(Type pType)
	{
		return assetBundle.LoadAllAssets(pType);
	}

	public T[] GetAllObjects<T>() where T : Object
	{
		return assetBundle.LoadAllAssets<T>();
	}

	public Object[] GetAllObjects(string pPath, Type pType)
	{
		pPath = pPath.ToLower();
		if (!direct_visit.TryGetValue(pPath, out var value))
		{
			value = root;
			string[] array = pPath.ToLower().Split('/');
			foreach (string key in array)
			{
				if (!value.children.ContainsKey(key))
				{
					return null;
				}
				value = value.children[key];
			}
			direct_visit[pPath] = value;
		}
		if (value.resources_full_names.Count == 0)
		{
			return null;
		}
		List<Object> list = new List<Object>();
		foreach (string resources_full_name in value.resources_full_names)
		{
			Object val = assetBundle.LoadAsset(resources_full_name, pType);
			if (val != (Object)null)
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}

	public T[] GetAllObjects<T>(string pPath) where T : Object
	{
		pPath = pPath.ToLower();
		if (!direct_visit.TryGetValue(pPath, out var value))
		{
			value = root;
			string[] array = pPath.ToLower().Split('/');
			foreach (string key in array)
			{
				if (!value.children.ContainsKey(key))
				{
					return null;
				}
				value = value.children[key];
			}
			direct_visit[pPath] = value;
		}
		if (value.resources_full_names.Count == 0)
		{
			return null;
		}
		List<T> list = new List<T>();
		foreach (string resources_full_name in value.resources_full_names)
		{
			T val = assetBundle.LoadAsset<T>(resources_full_name);
			if ((Object)(object)val != (Object)null)
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}
}
