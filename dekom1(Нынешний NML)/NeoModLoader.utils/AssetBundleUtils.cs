using System;
using System.Collections.Generic;
using System.IO;
using NeoModLoader.services;
using UnityEngine;

namespace NeoModLoader.utils;

public static class AssetBundleUtils
{
	private static readonly Dictionary<string, WrappedAssetBundle> LoadedAssetBundles = new Dictionary<string, WrappedAssetBundle>();

	private static readonly Dictionary<string, WrappedAssetBundle> LoadedAssetBundlesByPath = new Dictionary<string, WrappedAssetBundle>();

	public static WrappedAssetBundle GetAssetBundle(string name)
	{
		return LoadedAssetBundles[name];
	}

	public static WrappedAssetBundle LoadFromFile(string pPath, bool pForceReload = false)
	{
		FileInfo fileInfo = new FileInfo(pPath);
		if (LoadedAssetBundlesByPath.ContainsKey(fileInfo.FullName) && !pForceReload)
		{
			return LoadedAssetBundlesByPath[fileInfo.FullName];
		}
		using Stream stream = fileInfo.OpenRead();
		WrappedAssetBundle wrappedAssetBundle = new WrappedAssetBundle(AssetBundle.LoadFromStream(stream));
		LoadedAssetBundlesByPath[fileInfo.FullName] = wrappedAssetBundle;
		LoadedAssetBundles[wrappedAssetBundle.Name] = wrappedAssetBundle;
		return wrappedAssetBundle;
	}

	public static WrappedAssetBundle[] LoadFromFolder(string pFolder)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(pFolder);
		FileInfo[] files = directoryInfo.GetFiles();
		List<WrappedAssetBundle> list = new List<WrappedAssetBundle>();
		FileInfo[] array = files;
		foreach (FileInfo fileInfo in array)
		{
			if (fileInfo.Extension != ".manifest")
			{
				try
				{
					list.Add(LoadFromFile(fileInfo.FullName));
				}
				catch (Exception arg)
				{
					LogService.LogError($"Failed to load asset bundle {fileInfo.FullName}.\n{arg}");
				}
			}
		}
		return list.ToArray();
	}
}
