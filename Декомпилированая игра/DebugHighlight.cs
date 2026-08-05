using System.Collections.Generic;
using UnityEngine;

public static class DebugHighlight
{
	public static HashSet<DebugHighlightContainer> hashset = new HashSet<DebugHighlightContainer>();

	private static List<DebugHighlightContainer> to_remove = new List<DebugHighlightContainer>();

	public static void updateDebugHighlights()
	{
		if (hashset.Count == 0)
		{
			return;
		}
		to_remove.Clear();
		foreach (DebugHighlightContainer tCont in hashset)
		{
			tCont.timer -= World.world.delta_time;
			if (tCont.timer < 0f)
			{
				to_remove.Add(tCont);
			}
		}
		foreach (DebugHighlightContainer tCont2 in to_remove)
		{
			hashset.Remove(tCont2);
		}
	}

	public static void newHighlightList(Color pColor, List<TileZone> pZones, float pTime = 3f)
	{
		foreach (TileZone iZone in pZones)
		{
			newHighlight(pColor, iZone, pTime);
		}
	}

	public static void newHighlightList(Color pColor, List<MapChunk> pChunks, float pTime = 3f)
	{
		foreach (MapChunk tChunk in pChunks)
		{
			newHighlight(pColor, tChunk, pTime);
		}
	}

	public static void clear()
	{
		hashset.Clear();
	}

	public static void newHighlight(Color pColor, MapChunk pChunk, float pTime = 3f)
	{
		DebugHighlightContainer tCont = new DebugHighlightContainer();
		tCont.chunk = pChunk;
		tCont.color = pColor;
		tCont.setTimer(pTime);
		hashset.Add(tCont);
	}

	public static void newHighlight(Color pColor, TileZone pZone, float pTime = 3f)
	{
		DebugHighlightContainer tCont = new DebugHighlightContainer();
		tCont.zone = pZone;
		tCont.color = pColor;
		tCont.setTimer(pTime);
		hashset.Add(tCont);
	}
}
