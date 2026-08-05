using System.Collections.Generic;
using UnityEngine;

public class DebugLayerCursor : MapLayer
{
	private Color color_highlight_white;

	private Color color_main;

	private Color color_neighbour;

	private Color color_neighbour_2;

	private Color color_region;

	private Color color_edges;

	private Color color_chunk_bounds;

	private Color color_edges_blink;

	private List<WorldTile> _tiles = new List<WorldTile>();

	private bool blink = true;

	private float timerBlink = 0.2f;

	private float timerRecalc = 0.1f;

	private MapChunk lastChunk;

	internal override void create()
	{
		base.create();
		color_highlight_white = Toolbox.makeColor("#FFFFFF77");
		color_main = new Color(0f, 1f, 0f, 0.1f);
		color_neighbour = new Color(1f, 0f, 1f, 0.8f);
		color_neighbour_2 = new Color(1f, 0f, 1f, 0.3f);
		color_edges = new Color(1f, 0f, 0f, 0.5f);
		color_chunk_bounds = new Color(0f, 1f, 1f, 0.5f);
		color_edges_blink = new Color(0.1f, 0.1f, 1f, 1f);
		color_region = new Color(0f, 0f, 1f, 0.8f);
	}

	protected override void UpdateDirty(float pElapsed)
	{
		if (ScrollWindow.isWindowActive())
		{
			return;
		}
		if (!Config.isEditor && !DebugConfig.instance.debugButton.gameObject.activeSelf)
		{
			clear();
			return;
		}
		if (timerBlink > 0f)
		{
			timerBlink -= Time.deltaTime;
		}
		else
		{
			timerBlink = 0.2f;
			blink = !blink;
		}
		if (timerRecalc > 0f)
		{
			timerRecalc -= pElapsed;
			clear();
			WorldTile tCursorTile = World.world.getMouseTilePos();
			if (tCursorTile == null)
			{
				return;
			}
			lastChunk = tCursorTile.chunk;
			_ = tCursorTile.chunk;
			_ = lastChunk;
			if (DebugConfig.isOn(DebugOption.RenderIslands) && tCursorTile?.region?.island != null)
			{
				drawIsland(tCursorTile.region.island);
			}
			if (DebugConfig.isOn(DebugOption.CursorChunk))
			{
				fill(lastChunk.tiles, color_highlight_white);
			}
			if (DebugConfig.isOn(DebugOption.RenderConnectedIslands) && tCursorTile?.region?.island != null)
			{
				foreach (TileIsland connectedIsland in tCursorTile.region.island.getConnectedIslands())
				{
					foreach (MapRegion tReg in connectedIsland.regions)
					{
						fill(tReg.tiles, Color.blue);
					}
				}
			}
			if (DebugConfig.isOn(DebugOption.PossibleCityReach))
			{
				renderPossibleCityReach();
			}
			if (DebugConfig.isOn(DebugOption.RenderIslandsInsideRegionCorners) && tCursorTile?.region?.island != null)
			{
				foreach (MapRegion tRegion in tCursorTile.region.island.insideRegionEdges)
				{
					fill(tRegion.tiles, Color.magenta);
				}
			}
			if (DebugConfig.isOn(DebugOption.RenderIslandsTileCorners) && tCursorTile?.region?.island != null)
			{
				foreach (MapRegion tRegion2 in tCursorTile.region.island.insideRegionEdges)
				{
					fill(tRegion2.getEdgeTiles(), Color.red);
				}
			}
			if (DebugConfig.isOn(DebugOption.RenderIslandCenterRegions) && tCursorTile?.region?.island != null)
			{
				foreach (MapRegion tRegion3 in tCursorTile.region.island.regions)
				{
					if (!tRegion3.center_region)
					{
						fill(tRegion3.tiles, Color.red);
					}
				}
			}
			if (DebugConfig.isOn(DebugOption.RenderRegionOutsideRegionCorners) && tCursorTile?.region != null)
			{
				foreach (MapRegion tRegion4 in tCursorTile.region.getEdgeRegions())
				{
					fill(tRegion4.tiles, Color.yellow);
				}
			}
			if (DebugConfig.isOn(DebugOption.RenderMapRegionEdges) && tCursorTile.region != null)
			{
				fill(tCursorTile.region.getEdgeTiles(), Color.red);
			}
			if (DebugConfig.isOn(DebugOption.RegionNeighbours) && tCursorTile.region != null)
			{
				HashSet<MapRegion> tWave1 = new HashSet<MapRegion>();
				HashSet<MapRegion> tWave2 = new HashSet<MapRegion>();
				tWave1.Add(tCursorTile.region);
				foreach (MapRegion tRegion5 in tCursorTile.region.neighbours)
				{
					tWave1.Add(tRegion5);
				}
				foreach (MapRegion item in tWave1)
				{
					foreach (MapRegion tRegionNeighbour in item.neighbours)
					{
						if (!tWave1.Contains(tRegionNeighbour))
						{
							tWave2.Add(tRegionNeighbour);
						}
					}
				}
				foreach (MapRegion tReg2 in tWave1)
				{
					fill(tReg2.tiles, color_neighbour);
				}
				foreach (MapRegion tReg3 in tWave2)
				{
					fill(tReg3.tiles, color_neighbour_2);
				}
			}
			if (DebugConfig.isOn(DebugOption.Region) && tCursorTile.region != null)
			{
				fill(tCursorTile.region.tiles, color_region);
			}
			if (DebugConfig.isOn(DebugOption.ConnectedZones) && tCursorTile.zone != null)
			{
				TileZone tMainZone = tCursorTile.zone;
				MapRegion tMainRegion = tCursorTile.region;
				fill(tMainZone.tiles, color_region);
				using ListPool<MapRegion> tPool = new ListPool<MapRegion>();
				TileZone[] tNeighbours = tMainZone.neighbours;
				foreach (TileZone tNZone in tNeighbours)
				{
					tPool.Clear();
					if (TileZone.hasZonesConnectedViaRegions(tMainZone, tNZone, tMainRegion, tPool))
					{
						fill(tNZone.tiles, color_neighbour);
					}
				}
			}
			if (DebugConfig.isOn(DebugOption.ChunkEdges) && tCursorTile.chunk != null)
			{
				fill(tCursorTile.chunk.edges_all, color_edges);
			}
			if (DebugConfig.isOn(DebugOption.ChunkBounds) && tCursorTile.chunk != null)
			{
				fill(tCursorTile.chunk.chunk_bounds, color_chunk_bounds);
			}
			if (DebugConfig.isOn(DebugOption.Connections) && tCursorTile.region != null)
			{
				drawConnections(tCursorTile);
			}
			updatePixels();
		}
		else
		{
			timerRecalc = 0.1f;
		}
	}

	private void renderPossibleCityReach()
	{
		WorldTile tCursorTile = World.world.getMouseTilePos();
		if (tCursorTile.zone.city == null)
		{
			return;
		}
		TileIsland tCityIsland = tCursorTile.region.island;
		foreach (ref TileIsland island in World.world.islands_calculator.islands)
		{
			TileIsland tIsland = island;
			if (tCityIsland == tIsland || !tCityIsland.reachableByCityFrom(tIsland))
			{
				continue;
			}
			foreach (MapRegion tReg in tIsland.regions)
			{
				fill(tReg.tiles, Color.blue);
			}
		}
	}

	private void drawIsland(TileIsland pIsland)
	{
		Color32 tRed = Color.red;
		foreach (MapRegion tRegion in pIsland.regions)
		{
			_tiles.AddRange(tRegion.tiles);
			foreach (WorldTile tTile in tRegion.tiles)
			{
				pixels[tTile.data.tile_id] = tRed;
			}
		}
	}

	private void drawConnections(WorldTile pTile)
	{
		if (blink && pTile.region.debug_blink_edges_up != null)
		{
			fill(pTile.region.debug_blink_edges_up, color_edges_blink, pEdge: true);
			fill(pTile.region.debug_blink_edges_down, color_edges_blink, pEdge: true);
			fill(pTile.region.debug_blink_edges_left, color_edges_blink, pEdge: true);
			fill(pTile.region.debug_blink_edges_right, color_edges_blink, pEdge: true);
		}
	}

	private void fill(List<WorldTile> pTiles, Color pColor, bool pEdge = false)
	{
		createTextureNew();
		for (int i = 0; i < pTiles.Count; i++)
		{
			WorldTile tTile = pTiles[i];
			if (!pEdge || tTile.region != null)
			{
				_tiles.Add(tTile);
				pixels[tTile.data.tile_id] = pColor;
			}
		}
	}

	private void fill(WorldTile[] pTiles, Color pColor, bool pEdge = false)
	{
		createTextureNew();
		foreach (WorldTile tTile in pTiles)
		{
			if (!pEdge || tTile.region != null)
			{
				_tiles.Add(tTile);
				pixels[tTile.data.tile_id] = pColor;
			}
		}
	}

	internal override void clear()
	{
		if (_tiles.Count != 0)
		{
			_tiles.Clear();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = Color.clear;
			}
			createTextureNew();
		}
	}
}
