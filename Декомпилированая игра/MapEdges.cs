using UnityEngine;

public class MapEdges
{
	private static Texture2D textureLeft;

	private static Texture2D textureRight;

	private static Texture2D textureUp;

	private static Texture2D textureDown;

	private static Texture2D textureTempUp;

	private static Texture2D textureTempDown;

	private static int edgeSize;

	internal static void AddEdgeGradientCircle(WorldTile[,] pMap, string pWhat)
	{
		WorldTile tCenter = pMap[MapBox.width / 2, MapBox.height / 2];
		float tMaxMod = 0.99f;
		float tGradientMod = 0.85f;
		float tMaxCenter = (float)(MapBox.width / 2) * tMaxMod;
		float tGradient = (float)(MapBox.width / 2) * tGradientMod;
		float tDiff = tMaxCenter - tGradient;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			float tDist = Toolbox.DistTile(tTile, tCenter);
			if (tDist > tMaxCenter)
			{
				tTile.Height = 0;
			}
			else if (tDist < tMaxCenter && tDist > tGradient)
			{
				float tMod = (tMaxCenter - tDist) / tDiff;
				int tNewHeight = (int)((float)tTile.Height * tMod);
				tTile.Height = tNewHeight;
			}
		}
	}

	internal static void AddEdgeSquare(WorldTile[,] pMap, string pWhat)
	{
		edgeSize = 64;
		if (textureLeft == null)
		{
			textureLeft = (Texture2D)Resources.Load("edges/edge100xLeft");
			textureRight = (Texture2D)Resources.Load("edges/edge100xRight");
			textureUp = (Texture2D)Resources.Load("edges/edge100xUp");
			textureDown = (Texture2D)Resources.Load("edges/edge100xDown");
			textureTempUp = (Texture2D)Resources.Load("edges/edgeTempUp");
			textureTempDown = (Texture2D)Resources.Load("edges/edgeTempDown");
		}
		int tCountWidth = (int)((float)MapBox.width / (float)edgeSize) + 1;
		int tCountHeight = (int)((float)MapBox.height / (float)edgeSize) + 1;
		if (pWhat == "temperature")
		{
			for (int iX = 0; iX < tCountWidth; iX++)
			{
				fill(iX, 0, textureTempDown, pMap, pWhat);
			}
			for (int i = 0; i < tCountWidth; i++)
			{
				fill(i, tCountHeight - 2, textureTempUp, pMap, pWhat);
			}
			return;
		}
		for (int iY = 0; iY < tCountHeight; iY++)
		{
			fill(0, iY, textureLeft, pMap, pWhat);
		}
		for (int j = 0; j < tCountHeight; j++)
		{
			fill(tCountWidth - 2, j, textureRight, pMap, pWhat);
		}
		for (int k = 0; k < tCountWidth; k++)
		{
			fill(k, 0, textureDown, pMap, pWhat);
		}
		for (int l = 0; l < tCountWidth; l++)
		{
			fill(l, tCountHeight - 2, textureUp, pMap, pWhat);
		}
	}

	internal static void fill(int pX, int pY, Texture2D pTexture, WorldTile[,] tilesMap, string pWhat)
	{
		for (int y = 0; y < pTexture.height; y++)
		{
			for (int x = 0; x < pTexture.width; x++)
			{
				int tHeight = (int)(pTexture.GetPixel(x, y).a * 255f);
				int tX = x + pX * edgeSize;
				int tY = y + pY * edgeSize;
				if (tX < MapBox.width && tY < MapBox.height)
				{
					WorldTile tWorldTile = tilesMap[tX, tY];
					if (tWorldTile != null && pWhat == "height")
					{
						tWorldTile.Height -= tHeight;
					}
				}
			}
		}
	}
}
