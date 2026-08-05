using System.Collections.Generic;

public static class BuildingHelper
{
	private static List<WorldTile> _list_tiles = new List<WorldTile>();

	public static void tryToBuildNear(WorldTile pTile, string pAssetID)
	{
		BuildingAsset tBuildingAsset = AssetManager.buildings.get(pAssetID);
		if (tBuildingAsset != null)
		{
			if (World.world.buildings.canBuildFrom(pTile, tBuildingAsset, null))
			{
				World.world.buildings.addBuilding(tBuildingAsset, pTile);
			}
			else
			{
				tryToBuildNear(pTile, tBuildingAsset);
			}
		}
	}

	public static bool tryToBuildNear(WorldTile pTile, BuildingAsset pAsset)
	{
		List<WorldTile> tTempList = _list_tiles;
		fillEmptyTilesAroundMine(pTile, tTempList);
		bool result = tryToPlaceBuilding(pAsset, tTempList);
		tTempList.Clear();
		return result;
	}

	private static void fillEmptyTilesAroundMine(WorldTile pTile, List<WorldTile> pList)
	{
		pList.Clear();
		int tSize = 4;
		int startX = pTile.x - tSize;
		int startY = pTile.y - tSize;
		for (int iX = 0; iX < tSize * 2; iX++)
		{
			for (int iY = 0; iY < tSize * 2; iY++)
			{
				WorldTile tTile = World.world.GetTile(iX + startX, iY + startY);
				if (tTile != null && (!tTile.hasBuilding() || !tTile.building.isUsable() || !tTile.building.asset.city_building))
				{
					pList.Add(tTile);
				}
			}
		}
	}

	private static bool tryToPlaceBuilding(BuildingAsset pAsset, List<WorldTile> pList)
	{
		foreach (WorldTile tTile in pList.LoopRandom())
		{
			if (World.world.buildings.canBuildFrom(tTile, pAsset, null))
			{
				if (World.world.buildings.addBuilding(pAsset, tTile) != null)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}
}
