using System.Collections.Generic;

public class CityPlaceFinder : CityZoneWorkerBase
{
	private bool _dirty;

	internal List<TileZone> zones = new List<TileZone>();

	internal bool isDirty()
	{
		if (!DebugConfig.isOn(DebugOption.SystemCityPlaceFinder))
		{
			return false;
		}
		return _dirty;
	}

	internal void recalc()
	{
		if (isDirty())
		{
			_dirty = false;
			clearAll();
			prepareBasicZones();
			prepareQueueFromCities();
			startWave();
			createFinalList();
		}
	}

	internal override void clearAll()
	{
		base.clearAll();
		zones.Clear();
		List<TileZone> tZones = World.world.zone_calculator.zones;
		for (int i = 0; i < tZones.Count; i++)
		{
			tZones[i].setGoodForNewCity(pValue: true);
		}
	}

	private void prepareBasicZones()
	{
		List<TileZone> tZones = World.world.zone_calculator.zones;
		for (int i = 0; i < tZones.Count; i++)
		{
			TileZone tZone = tZones[i];
			if (!tZone.canStartCityHere())
			{
				tZone.setGoodForNewCity(pValue: false);
			}
			else if (tZone.centerTile.region.island.getTileCount() < 300)
			{
				tZone.setGoodForNewCity(pValue: false);
			}
		}
	}

	private void prepareQueueFromCities()
	{
		prepareWave();
		foreach (City tCity in World.world.cities)
		{
			checkCity(tCity, _wave);
		}
	}

	private void checkCity(City pCity, Queue<ZoneConnection> pWaveQ)
	{
		WorldTile tCityTile = pCity.getTile();
		if (tCityTile == null)
		{
			return;
		}
		TileIsland tCityIsland = tCityTile.region.island;
		foreach (TileZone tZone in pCity.border_zones)
		{
			for (int i = 0; i < tZone.centerTile.chunk.regions.Count; i++)
			{
				MapRegion tRegion = tZone.chunk.regions[i];
				if (tRegion.isTypeGround() && tRegion.zones.Contains(tZone) && tRegion.island == tCityIsland)
				{
					queueConnection(new ZoneConnection(tZone, tRegion), pWaveQ, pSetChecked: true);
				}
			}
		}
	}

	private void startWave()
	{
		Queue<ZoneConnection> tWaveQ = _wave;
		Queue<ZoneConnection> tNextWaveQ = _next_wave;
		using ListPool<MapRegion> tListRegions = new ListPool<MapRegion>();
		int tMaxWave = 3;
		int tWaveID = 0;
		while (tNextWaveQ.Count > 0 || tWaveQ.Count > 0)
		{
			if (tWaveQ.Count == 0)
			{
				Queue<ZoneConnection> queue = tWaveQ;
				tWaveQ = tNextWaveQ;
				tNextWaveQ = queue;
				tWaveID++;
			}
			if (tWaveID > tMaxWave)
			{
				break;
			}
			ZoneConnection zoneConnection = tWaveQ.Dequeue();
			TileZone tMainZone = zoneConnection.zone;
			MapRegion tMainRegion = zoneConnection.region;
			TileZone[] tNeighbours = tMainZone.neighbours;
			foreach (TileZone tZone in tNeighbours)
			{
				if (tZone.hasCity())
				{
					continue;
				}
				tListRegions.Clear();
				if (!TileZone.hasZonesConnectedViaRegions(tMainZone, tZone, tMainRegion, tListRegions))
				{
					continue;
				}
				for (int iReg = 0; iReg < tListRegions.Count; iReg++)
				{
					MapRegion tRegionToCheck = tListRegions[iReg];
					ZoneConnection tNewConnection = new ZoneConnection(tZone, tRegionToCheck);
					if (_zones_checked.Add(tNewConnection))
					{
						tZone.setGoodForNewCity(pValue: false);
						queueConnection(tNewConnection, tNextWaveQ);
					}
				}
			}
		}
	}

	private void createFinalList()
	{
		for (int i = 0; i < World.world.zone_calculator.zones.Count; i++)
		{
			TileZone tZone = World.world.zone_calculator.zones[i];
			if (tZone.isGoodForNewCity())
			{
				zones.Add(tZone);
			}
		}
	}

	public bool hasPossibleZones()
	{
		if (_dirty)
		{
			return false;
		}
		return zones.Count > 0;
	}

	internal void setDirty()
	{
		_dirty = true;
		clearCurrentZones();
	}

	private void clearCurrentZones()
	{
		if (zones.Count != 0)
		{
			for (int i = 0; i < zones.Count; i++)
			{
				zones[i].setGoodForNewCity(pValue: false);
			}
			zones.Clear();
		}
	}
}
