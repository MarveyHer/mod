using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GeneratorTool : ScriptableObject
{
	private static WorldTile[,] _tiles_map;

	private static Texture2D[] _textures;

	private static List<WorldTile> _neighbours = new List<WorldTile>(4);

	private static List<WorldTile> _neighbours_all = new List<WorldTile>(8);

	internal static void Setup(WorldTile[,] pTilesMap)
	{
		_tiles_map = pTilesMap;
	}

	public static void Init()
	{
		LoadGenShapeTextures();
	}

	internal static void applyTemplate(string pID, float pMod = 1f)
	{
		Texture2D tTexture = Resources.LoadAll<Texture2D>("map_gen/" + pID).GetRandom();
		tTexture = TextureRotator.Rotate(tTexture, Randy.randomInt(0, 360), new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
		TextureScale.Bilinear(tTexture, MapBox.width, MapBox.height);
		float tHeightMod = 255f * pMod;
		for (int x = 0; x < tTexture.width; x++)
		{
			for (int y = 0; y < tTexture.height; y++)
			{
				WorldTile tTile = World.world.GetTile(x, y);
				if (tTile != null)
				{
					tTexture.GetPixel(x, y);
					int tVal = (int)((1f - tTexture.GetPixel(x, y).g) * tHeightMod);
					tTile.Height += tVal;
				}
			}
		}
	}

	internal static void ApplyRandomShape(string pWhat = "height", float tDistMax = 2f, float pMod = 0.7f, bool pSubtract = false)
	{
		Texture2D texture = null;
		int newW = 0;
		int newH = 0;
		texture = Object.Instantiate(_textures.GetRandom());
		texture.name = "random_shape";
		newW = (int)((float)texture.width * Randy.randomFloat(0.3f, 2f));
		newH = (int)((float)texture.height * Randy.randomFloat(0.3f, 2f));
		texture = TextureRotator.Rotate(texture, Randy.randomInt(0, 360), new Color32(0, 0, 0, 0));
		TextureScale.Bilinear(texture, newW, newH);
		newW = texture.width;
		newH = texture.height;
		int tPosX = MapBox.width / 2 - newW / 2 - (int)Randy.randomFloat((float)(-newW) * tDistMax, (float)newW * tDistMax);
		int tPosY = MapBox.height / 2 - newH / 2 - (int)Randy.randomFloat((float)(-newH) * tDistMax, (float)newH * tDistMax);
		if (tPosX < 0)
		{
			tPosX = 0;
		}
		if (tPosY < 0)
		{
			tPosY = 0;
		}
		if (tPosX + newW > MapBox.width)
		{
			tPosX = MapBox.width - newW;
		}
		if (tPosY + newH > MapBox.height)
		{
			tPosY = MapBox.height - newH;
		}
		float tHeightMod = 255f * pMod;
		for (int x = 0; x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++)
			{
				WorldTile tTile = World.world.GetTile(tPosX + x, tPosY + y);
				if (tTile != null)
				{
					int tVal = (int)(texture.GetPixel(x, y).a * tHeightMod);
					if (pSubtract)
					{
						tVal = -tVal;
					}
					tTile.Height += tVal;
				}
			}
		}
		Object.Destroy(texture);
	}

	private static void LoadGenShapeTextures()
	{
		if (_textures == null)
		{
			_textures = Resources.LoadAll<Texture2D>("gen_shapes");
		}
	}

	public static void ApplyWaterLevel(WorldTile[,] tilesMap, int width, int height, int pVal)
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				tilesMap[x, y].Height -= pVal;
			}
		}
	}

	public static void ApplyPerlinNoise(WorldTile[,] tilesMap, int width, int height, float pPosX, float pPosY, float pAlphaMod, float pScaleMod, bool pSubtract = false, GeneratorTarget pTarget = GeneratorTarget.Height)
	{
		float tAlphaMod = 255f * pAlphaMod;
		float tScaleX = 1f;
		float tScaleY = 1f;
		if (width > height)
		{
			tScaleX = width / height;
		}
		else
		{
			tScaleY = height / width;
		}
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				float num = (pPosX + (float)x) / (float)width;
				float tY = (pPosY + (float)y) / (float)height;
				int tValue = (int)(Mathf.PerlinNoise(num * pScaleMod * tScaleX, tY * pScaleMod * tScaleY) * tAlphaMod);
				if (pSubtract)
				{
					tValue = -tValue;
				}
				if (pTarget == GeneratorTarget.Height)
				{
					tilesMap[x, y].Height += tValue;
				}
			}
		}
	}

	public static void ApplyPerlinReplace(PerlinReplaceContainer pContainer)
	{
		float pPosX = Randy.randomInt(0, 15000);
		float pPosY = Randy.randomInt(0, 15000);
		int width = MapBox.width;
		int height = MapBox.height;
		float pScaleMod = pContainer.scale;
		float tMaxHeight = 255f;
		float tScaleX = 1f;
		float tScaleY = 1f;
		if (width > height)
		{
			tScaleX = width / height;
		}
		else
		{
			tScaleY = height / width;
		}
		WorldTile[,] tTilesMap = _tiles_map;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				WorldTile tTile = tTilesMap[x, y];
				float num = (pPosX + (float)x) / (float)width;
				float tY = (pPosY + (float)y) / (float)height;
				int tValue = (int)(Mathf.PerlinNoise(num * pScaleMod * tScaleX, tY * pScaleMod * tScaleY) * tMaxHeight);
				for (int i = 0; i < pContainer.options.Count; i++)
				{
					PerlinReplaceOption tOption = pContainer.options[i];
					if (tValue > tOption.replace_height_value && tTile.main_type.IsType(tOption.from))
					{
						tTile.setTileType(tOption.to);
					}
				}
			}
		}
	}

	public static void UpdateTileTypes(bool pGeneratorStage = false, int pStartIndex = 0, int pAmount = 0)
	{
		int tMax = pStartIndex + pAmount;
		for (int i = pStartIndex; i < tMax; i++)
		{
			WorldTile tTile = World.world.tiles_list[i];
			TileType tType = AssetManager.tiles.getTypeByDepth(tTile);
			tTile.setTileType(tType);
		}
	}

	public static void GenerateTileNeighbours(WorldTile[] pTilesList)
	{
		int tCount = pTilesList.Length;
		for (int i = 0; i < tCount; i++)
		{
			generateTileNeighbours(pTilesList[i]);
		}
	}

	public static void generateTileNeighbours(WorldTile pTile)
	{
		WorldTile tNeighbour = getTile(pTile.x - 1, pTile.y);
		pTile.addNeighbour(tNeighbour, TileDirection.Left, _neighbours, _neighbours_all);
		tNeighbour = getTile(pTile.x + 1, pTile.y);
		pTile.addNeighbour(tNeighbour, TileDirection.Right, _neighbours, _neighbours_all);
		tNeighbour = getTile(pTile.x, pTile.y - 1);
		pTile.addNeighbour(tNeighbour, TileDirection.Down, _neighbours, _neighbours_all);
		tNeighbour = getTile(pTile.x, pTile.y + 1);
		pTile.addNeighbour(tNeighbour, TileDirection.Up, _neighbours, _neighbours_all);
		tNeighbour = getTile(pTile.x - 1, pTile.y - 1);
		pTile.addNeighbour(tNeighbour, TileDirection.Null, _neighbours, _neighbours_all, pDiagonal: true);
		tNeighbour = getTile(pTile.x - 1, pTile.y + 1);
		pTile.addNeighbour(tNeighbour, TileDirection.Null, _neighbours, _neighbours_all, pDiagonal: true);
		tNeighbour = getTile(pTile.x + 1, pTile.y - 1);
		pTile.addNeighbour(tNeighbour, TileDirection.Null, _neighbours, _neighbours_all, pDiagonal: true);
		tNeighbour = getTile(pTile.x + 1, pTile.y + 1);
		pTile.addNeighbour(tNeighbour, TileDirection.Null, _neighbours, _neighbours_all, pDiagonal: true);
		pTile.neighbours = _neighbours.ToArray();
		pTile.neighboursAll = _neighbours_all.ToArray();
		_neighbours.Clear();
		_neighbours_all.Clear();
	}

	public static void ApplyRingEffect()
	{
		WorldTile[,] tTilesMap = _tiles_map;
		for (int x = 0; x < MapBox.width; x++)
		{
			for (int y = 0; y < MapBox.height; y++)
			{
				for (int i1 = 0; i1 < AssetManager.tiles.list.Count; i1++)
				{
					TileType tType = AssetManager.tiles.list[i1];
					if (tType.additional_height == null)
					{
						continue;
					}
					bool found = false;
					for (int j = 0; j < tType.additional_height.Length; j++)
					{
						WorldTile tWorldTile = tTilesMap[x, y];
						if (tWorldTile.Height == tType.height_min - tType.additional_height[j])
						{
							tWorldTile.Height = tType.height_min;
							found = true;
							break;
						}
					}
					if (found)
					{
						break;
					}
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static WorldTile getTile(int pX, int pY)
	{
		if (pX < 0 || pX >= MapBox.width)
		{
			return null;
		}
		if (pY < 0 || pY >= MapBox.height)
		{
			return null;
		}
		return _tiles_map[pX, pY];
	}
}
