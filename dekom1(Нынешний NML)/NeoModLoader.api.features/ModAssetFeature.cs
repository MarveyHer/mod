using System.Linq;

namespace NeoModLoader.api.features;

public abstract class ModAssetFeature<TAsset> : ModObjectFeature<TAsset> where TAsset : Asset
{
	protected virtual bool AddToLibrary => true;

	public override bool Init()
	{
		if (!base.Init())
		{
			return false;
		}
		if (AddToLibrary)
		{
			AssetLibrary<TAsset> assetLibrary = AssetManager._instance._list.OfType<AssetLibrary<TAsset>>().FirstOrDefault();
			if (assetLibrary == null)
			{
				throw new FeatureLoadException("No library found for " + typeof(TAsset).Name);
			}
			assetLibrary.add(base.Object);
		}
		return true;
	}
}
