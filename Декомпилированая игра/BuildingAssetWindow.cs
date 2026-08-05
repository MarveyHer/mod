using UnityEngine;

public class BuildingAssetWindow : BaseDebugAssetWindow<BuildingAsset, BuildingDebugAssetElement>
{
	public void clickRandomKingdomColor()
	{
		AssetsDebugManager.setRandomKingdomColor(asset.civ_kingdom);
		asset_debug_element.setData(asset);
	}

	protected override void initSprites()
	{
		base.initSprites();
		string tPath = asset.sprite_path;
		if (string.IsNullOrEmpty(tPath))
		{
			tPath = asset.main_path + asset.id;
		}
		Sprite[] spriteList = SpriteTextureLoader.getSpriteList(tPath);
		foreach (Sprite tSprite in spriteList)
		{
			SpriteElement spriteElement = Object.Instantiate(sprite_element_prefab, sprite_elements_parent);
			spriteElement.image.sprite = tSprite;
			spriteElement.text_name.text = tSprite.name;
		}
	}

	public static void reloadSprites()
	{
		BaseDebugAssetWindow<BuildingAsset, BuildingDebugAssetElement>.current_element.setData(BaseDebugAssetWindow<BuildingAsset, BuildingDebugAssetElement>.current_element.asset);
	}
}
