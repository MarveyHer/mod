using UnityEngine;

public static class DynamicSprites
{
	public const int NO_COLOR_ID = -900000;

	public static Sprite getIconWithColors(Sprite pSprite, PhenotypeAsset pPhenotype, ColorAsset pKingdomColor)
	{
		DynamicSpritesAsset tAsset = DynamicSpritesLibrary.icons;
		long tId = pSprite.GetHashCode() * 10000 + (pPhenotype?.GetHashCode() ?? 0) * 100 + (pKingdomColor?.GetHashCode() ?? 0);
		Sprite tResult = tAsset.getSprite(tId);
		if ((object)tResult == null)
		{
			tResult = DynamicSpriteCreator.createNewIcon(tAsset, pSprite, pKingdomColor, pPhenotype);
			tAsset.addSprite(tId, tResult);
		}
		return tResult;
	}

	public static Sprite getRecoloredBuilding(Sprite pBuildingSprite, ColorAsset pColor, DynamicSpritesAsset pAtlasAsset)
	{
		long tId = getBuildingSpriteID(pBuildingSprite.GetHashCode(), pColor);
		Sprite tResult = pAtlasAsset.getSprite(tId);
		if ((object)tResult == null)
		{
			tResult = DynamicSpriteCreator.createNewSpriteBuilding(pAtlasAsset, tId, pBuildingSprite, pColor);
			pAtlasAsset.addSprite(tId, tResult);
		}
		return tResult;
	}

	private static long getBuildingSpriteID(int pBaseSpriteID, ColorAsset pColor)
	{
		long t_kingdomID = ((pColor != null) ? (pColor.index_id + 1) : (-1000000));
		return (t_kingdomID + 1) * 10000000 + pBaseSpriteID;
	}

	public static Sprite getBuildingLight(Building pBuilding)
	{
		DynamicSpritesAsset building_lights = DynamicSpritesLibrary.building_lights;
		int tID = pBuilding.last_main_sprite.GetHashCode();
		return building_lights.getSprite(tID);
	}

	public static Sprite getIcon(Sprite pSprite, ColorAsset pColorAsset)
	{
		DynamicSpritesAsset tAsset = DynamicSpritesLibrary.icons;
		long tId = pSprite.GetHashCode() * 10000 + pColorAsset.GetHashCode();
		Sprite tResult = tAsset.getSprite(tId);
		if ((object)tResult == null)
		{
			tResult = DynamicSpriteCreator.createNewIcon(tAsset, pSprite, pColorAsset);
			tAsset.addSprite(tId, tResult);
		}
		return tResult;
	}

	public static Sprite getShadowBuilding(BuildingAsset pAsset, Sprite pSprite)
	{
		if (!pAsset.shadow)
		{
			return null;
		}
		int tId = pSprite.GetHashCode();
		return DynamicSpritesLibrary.building_shadows.getSprite(tId);
	}

	public static Sprite getShadowUnit(Sprite pSprite, int pHashCode)
	{
		DynamicSpritesAsset tAsset = DynamicSpritesLibrary.units_shadows;
		Sprite tResult = tAsset.getSprite(pHashCode);
		if ((object)tResult == null)
		{
			tResult = DynamicSpriteCreator.createNewUnitShadow(tAsset, pSprite);
			tAsset.addSprite(pHashCode, tResult);
		}
		return tResult;
	}

	public static void preloadItemSprite(Sprite pSprite, ColorAsset pColorAsset = null)
	{
		long tId = getItemSpriteID(pSprite, pColorAsset);
		DynamicSpritesAsset items = DynamicSpritesLibrary.items;
		Sprite tNewSprite = DynamicSpriteCreator.createNewItemSprite(items, pSprite, pColorAsset);
		items.addSprite(tId, tNewSprite);
	}

	public static long getItemSpriteID(Sprite pSprite, ColorAsset pColor)
	{
		int tHashCodeColor = pColor?.GetHashCode() ?? (-900000);
		return getItemSpriteID(pSprite, tHashCodeColor);
	}

	public static long getItemSpriteID(Sprite pSprite, int pColorID = -900000)
	{
		return pSprite.GetHashCode() * 10000 + pColorID;
	}

	public static Sprite getCachedAtlasItemSprite(long pID, Sprite pSpriteSource)
	{
		Sprite tResult = DynamicSpritesLibrary.items.getSprite(pID);
		if ((object)tResult == null)
		{
			Debug.LogError("[getCachedAtlasItemSprite]Dynamic sprite not found: " + pID + " " + pSpriteSource);
			return pSpriteSource;
		}
		return tResult;
	}

	public static Sprite getCachedAtlasItemSprite(long pID, Sprite pSpriteSource, ColorAsset pColorAsset)
	{
		Sprite tResult = DynamicSpritesLibrary.items.getSprite(pID);
		if ((object)tResult == null)
		{
			Debug.LogError("[getCachedAtlasItemSprite]Dynamic sprite not found: " + pID + " " + pSpriteSource?.ToString() + " " + ((pColorAsset != null) ? (pColorAsset.index_id + " " + pColorAsset.color_main) : "null"));
			return pSpriteSource;
		}
		return tResult;
	}
}
