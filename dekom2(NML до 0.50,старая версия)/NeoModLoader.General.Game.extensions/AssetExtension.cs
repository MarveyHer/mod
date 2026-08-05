using System;

namespace NeoModLoader.General.Game.extensions;

public static class AssetExtension
{
	public static void ForEach<TAsset, TLibrary>(this TLibrary pLibrary, Action<TAsset> pAction) where TAsset : Asset where TLibrary : AssetLibrary<TAsset>
	{
		AssetExtensionInternal<TAsset, TLibrary>.ForEach(pLibrary, pAction);
	}
}
