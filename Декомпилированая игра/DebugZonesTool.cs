using UnityEngine;

public static class DebugZonesTool
{
	public static void actionGrowBorder()
	{
		WorldTile tCursorTile = World.world.getMouseTilePos();
		if (tCursorTile != null)
		{
			TileZone tMainZone = tCursorTile.zone;
			if (tMainZone.hasCity())
			{
				World.world.city_zone_helper.city_growth.getZoneToClaim(null, tMainZone.city);
			}
		}
	}

	public static void actionAbandonZones()
	{
		WorldTile tCursorTile = World.world.getMouseTilePos();
		if (tCursorTile != null)
		{
			TileZone tMainZone = tCursorTile.zone;
			if (tMainZone.hasCity())
			{
				Bench.bench("abandon_stuff", "meh");
				World.world.city_zone_helper.city_abandon.check(tMainZone.city, pDebug: true);
				Debug.Log("bench abandon: " + Bench.benchEnd("abandon_stuff", "meh", pSaveCounter: false, 0L));
			}
		}
	}
}
