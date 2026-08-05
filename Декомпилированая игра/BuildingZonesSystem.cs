using System.Collections.Generic;

public class BuildingZonesSystem
{
	private static bool _dirty;

	public static void setDirty()
	{
		_dirty = true;
	}

	public static void update()
	{
		if (!_dirty)
		{
			return;
		}
		_dirty = false;
		List<TileZone> tZones = World.world.zone_calculator.zones;
		using ListPool<TileZone> tDirtyZones = new ListPool<TileZone>();
		for (int i = 0; i < tZones.Count; i++)
		{
			TileZone tZone = tZones[i];
			if (tZone.isDirty())
			{
				tDirtyZones.Add(tZone);
			}
		}
		for (int j = 0; j < tDirtyZones.Count; j++)
		{
			TileZone tZone2 = tDirtyZones[j];
			tZone2.clearBuildingLists();
			tZone2.setDirty(pValue: false);
			foreach (Building tBuilding in tZone2.buildings_all)
			{
				if (tBuilding.isOnRemove() || tBuilding.isRemoved())
				{
					continue;
				}
				if (tBuilding.current_tile.zone == tZone2)
				{
					tZone2.buildings_render_list.Add(tBuilding);
				}
				tZone2.addBuildingToSet(tBuilding);
				if (tBuilding.asset.city_building && !tZone2.hasCity())
				{
					if (tBuilding.isCiv())
					{
						tBuilding.makeAbandoned();
					}
					else
					{
						tBuilding.makeAbandoned();
					}
				}
			}
		}
	}
}
