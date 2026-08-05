using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace NeoModLoader.General.Game.extensions;

internal static class AssetExtensionInternal<TAsset, TLibrary> where TAsset : Asset where TLibrary : AssetLibrary<TAsset>
{
	private class LibraryState
	{
		public readonly HashSet<string> done = new HashSet<string>();

		public Action<TAsset> action;
	}

	private static readonly Dictionary<TLibrary, List<LibraryState>> _states = new Dictionary<TLibrary, List<LibraryState>>();

	private static bool _assetlibrary_patched;

	public static void ForEach(TLibrary pLibrary, Action<TAsset> pAction)
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected O, but got Unknown
		if (pLibrary == null)
		{
			return;
		}
		LibraryState libraryState = new LibraryState();
		foreach (TAsset item in pLibrary.list)
		{
			pAction(item);
		}
		libraryState.action = delegate(TAsset asset)
		{
			pAction(asset);
		};
		libraryState.done.UnionWith(pLibrary.list.Select((TAsset x) => x.id));
		if (!_states.ContainsKey(pLibrary))
		{
			_states.Add(pLibrary, new List<LibraryState>());
		}
		_states[pLibrary].Add(libraryState);
		if (!_assetlibrary_patched)
		{
			_assetlibrary_patched = true;
			new Harmony("NeoModLoader.ForEach").Patch((MethodBase)AccessTools.Method(typeof(AssetLibrary<TAsset>), "add", (Type[])null, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(AccessTools.FirstMethod(typeof(AssetExtensionInternal<TAsset, TLibrary>), (Func<MethodInfo, bool>)((MethodInfo x) => x.Name.Contains("AppendAssetToAction")))), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
		}
	}

	private static void AppendAssetToAction(TLibrary __instance, TAsset pAsset)
	{
		if (!_states.TryGetValue(__instance, out var value))
		{
			return;
		}
		foreach (LibraryState item in value)
		{
			if (!item.done.Add(pAsset.id))
			{
				break;
			}
			item.action(pAsset);
		}
	}
}
