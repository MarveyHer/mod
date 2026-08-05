using System.Collections.Generic;
using UnityEngine;

public class ConwayLife : MapLayer
{
	public static Color32 colorEater = new Color(1f, 0.2f, 1f);

	public static Color32 colorCreator;

	public bool makeFlash = true;

	private HashSetWorldTile newList;

	private float nextTickTimer;

	private float nextTickInterval = 0.05f;

	private int decreaseTick;

	private List<WorldTile> toRemove = new List<WorldTile>();

	internal override void create()
	{
		base.create();
		colorCreator = Toolbox.makeColor("#3BCC55");
		hashsetTiles = new HashSetWorldTile();
		newList = new HashSetWorldTile();
	}

	protected override void UpdateDirty(float pElapsed)
	{
		UpdateVisual();
		if (World.world.isPaused())
		{
			return;
		}
		if (nextTickTimer > 0f)
		{
			nextTickTimer -= pElapsed;
			return;
		}
		nextTickTimer = nextTickInterval;
		for (int i = 0; i < Config.time_scale_asset.conway_ticks; i++)
		{
			updateTick();
		}
	}

	private void UpdateVisual()
	{
		if (pixels_to_update.Count == 0)
		{
			return;
		}
		foreach (WorldTile tTile in pixels_to_update)
		{
			if (hashsetTiles.Contains(tTile))
			{
				if (tTile.data.conwayType == ConwayType.Eater)
				{
					pixels[tTile.data.tile_id] = colorEater;
				}
				else if (tTile.data.conwayType == ConwayType.Creator)
				{
					pixels[tTile.data.tile_id] = colorCreator;
				}
				else
				{
					pixels[tTile.data.tile_id] = Toolbox.clear;
				}
			}
			else
			{
				tTile.data.conwayType = ConwayType.None;
				pixels[tTile.data.tile_id] = Toolbox.clear;
			}
		}
		pixels_to_update.Clear();
		updatePixels();
	}

	public void remove(WorldTile pTile)
	{
		if (hashsetTiles.Count != 0)
		{
			hashsetTiles.Remove(pTile);
			pixels_to_update.Add(pTile);
			pTile.data.conwayType = ConwayType.None;
		}
	}

	public void add(WorldTile pTile, string pType)
	{
		if (pType == "conway")
		{
			pTile.data.conwayType = ConwayType.Eater;
		}
		else
		{
			pTile.data.conwayType = ConwayType.Creator;
		}
		hashsetTiles.Add(pTile);
		pixels_to_update.Add(pTile);
	}

	private void updateTick()
	{
		if (decreaseTick-- <= 0)
		{
			decreaseTick = 5;
		}
		if (hashsetTiles.Count <= 0 && newList.Count <= 0)
		{
			return;
		}
		newList.Clear();
		foreach (WorldTile tMainTile in hashsetTiles)
		{
			checkCell(tMainTile);
			WorldTile[] neighboursAll = tMainTile.neighboursAll;
			foreach (WorldTile tTile in neighboursAll)
			{
				checkCell(tTile);
			}
		}
		HashSetWorldTile tTemp = hashsetTiles;
		hashsetTiles = newList;
		newList = tTemp;
		UpdateVisual();
	}

	private void makeAlive(WorldTile pCell)
	{
		if (decreaseTick == 5)
		{
			MusicBox.playSound("event:/SFX/UNIQUE/ConwayMove", pCell);
			if (pCell.data.conwayType == ConwayType.Eater)
			{
				MapAction.decreaseTile(pCell, pDamage: true, "destroy_no_flash");
			}
			else
			{
				MapAction.increaseTile(pCell, pDamage: true, "destroy_no_flash");
			}
		}
		newList.Add(pCell);
		if (makeFlash)
		{
			makeFlashh(pCell, 25);
		}
	}

	internal void makeFlashh(WorldTile pCell, int pAmount)
	{
		if (pCell.data.conwayType != ConwayType.None)
		{
			_ = pCell.data.conwayType;
		}
	}

	internal override void clear()
	{
		base.clear();
		newList.Clear();
		hashsetTiles.Clear();
	}

	private void checkCell(WorldTile pCell)
	{
		if (pixels_to_update.Contains(pCell))
		{
			return;
		}
		int count = 0;
		int eaters = 0;
		int creators = 0;
		pixels_to_update.Add(pCell);
		if (pCell.data.conwayType == ConwayType.Eater)
		{
			eaters++;
		}
		if (pCell.data.conwayType == ConwayType.Creator)
		{
			creators++;
		}
		WorldTile[] neighboursAll;
		if (hashsetTiles.Contains(pCell))
		{
			neighboursAll = pCell.neighboursAll;
			foreach (WorldTile tTile in neighboursAll)
			{
				if (hashsetTiles.Contains(tTile))
				{
					count++;
					if (tTile.data.conwayType == ConwayType.Creator)
					{
						creators++;
					}
					else if (tTile.data.conwayType == ConwayType.Eater)
					{
						eaters++;
					}
				}
				if (count >= 4)
				{
					if (makeFlash)
					{
						makeFlashh(pCell, 15);
					}
					pCell.data.conwayType = ConwayType.None;
					return;
				}
			}
			if (count == 2 || count == 3)
			{
				if (pCell.data.conwayType == ConwayType.None && (eaters != 0 || creators != 0))
				{
					if (eaters >= creators)
					{
						pCell.data.conwayType = ConwayType.Eater;
					}
					else
					{
						pCell.data.conwayType = ConwayType.Creator;
					}
				}
				makeAlive(pCell);
			}
			else
			{
				pCell.data.conwayType = ConwayType.None;
			}
			return;
		}
		neighboursAll = pCell.neighboursAll;
		foreach (WorldTile tTile2 in neighboursAll)
		{
			if (hashsetTiles.Contains(tTile2))
			{
				count++;
			}
			if (tTile2.data.conwayType == ConwayType.Eater)
			{
				eaters++;
			}
			if (tTile2.data.conwayType == ConwayType.Creator)
			{
				creators++;
			}
		}
		if (count != 3)
		{
			return;
		}
		if (pCell.data.conwayType == ConwayType.None && (eaters != 0 || creators != 0))
		{
			if (eaters >= creators)
			{
				pCell.data.conwayType = ConwayType.Eater;
			}
			else
			{
				pCell.data.conwayType = ConwayType.Creator;
			}
		}
		makeAlive(pCell);
	}

	internal void checkKillRange(Vector2Int pPos, int pRad)
	{
		if (hashsetTiles.Count == 0)
		{
			return;
		}
		toRemove.Clear();
		foreach (WorldTile pCell in hashsetTiles)
		{
			if (Toolbox.DistVec2(pCell.pos, pPos) <= (float)pRad)
			{
				pCell.data.conwayType = ConwayType.None;
				toRemove.Add(pCell);
			}
		}
		foreach (WorldTile tTile in toRemove)
		{
			remove(tTile);
		}
	}
}
