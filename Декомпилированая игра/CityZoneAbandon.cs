using System.Collections.Generic;

public class CityZoneAbandon : CityZoneWorkerBase
{
	private List<ListPool<TileZone>> _split_areas = new List<ListPool<TileZone>>();

	private HashSetTileZone _zones_to_check = new HashSetTileZone();

	public void checkCities()
	{
		foreach (City city in World.world.cities)
		{
			city.checkAbandon();
		}
	}

	public void check(City pCity, bool pDebug = false, HashSet<TileZone> pSetToFill = null)
	{
		if (pCity.buildings.Count == 0)
		{
			return;
		}
		clearAll();
		prepareCityZones(pCity);
		startCheckingFromBuildings(pCity);
		_split_areas.Sort(sorter);
		if (pDebug)
		{
			return;
		}
		abandonLeftoverZones(pCity);
		if (_split_areas.Count >= 2)
		{
			_split_areas[0].Dispose();
			_split_areas.RemoveAt(0);
			if (_split_areas.Count > 0)
			{
				abandonSmallAreas(pCity);
			}
		}
	}

	private void startCheckingFromBuildings(City pCity)
	{
		for (int i = 0; i < pCity.buildings.Count; i++)
		{
			Building building = pCity.buildings[i];
			WorldTile tTile = building.current_tile;
			if (!building.asset.docks)
			{
				startWaveFromTile(tTile, pCity);
			}
		}
	}

	private void startWaveFromTile(WorldTile pTile, City pCity)
	{
		if (!_zones_to_check.Contains(pTile.zone))
		{
			return;
		}
		prepareWave();
		Queue<ZoneConnection> tWaveQ = _wave;
		Queue<ZoneConnection> tNextWaveQ = _next_wave;
		ListPool<TileZone> tNewArea = new ListPool<TileZone>(_wave.Count + _next_wave.Count);
		_split_areas.Add(tNewArea);
		queueConnection(new ZoneConnection(pTile.zone, pTile.region), tWaveQ, true);
		using ListPool<MapRegion> tListRegions = new ListPool<MapRegion>();
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
			ZoneConnection zoneConnection = tWaveQ.Dequeue();
			TileZone tMainZone = zoneConnection.zone;
			MapRegion tMainRegion = zoneConnection.region;
			if (tMainZone.isSameCityHere(pCity))
			{
				tNewArea.Add(tMainZone);
			}
			TileZone[] tNeighbours = tMainZone.neighbours;
			foreach (TileZone tZone in tNeighbours)
			{
				if (!tZone.isSameCityHere(pCity) || tZone.tiles_with_ground == 0)
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
						queueConnection(tNewConnection, tNextWaveQ);
					}
				}
			}
		}
	}

	private void abandonLeftoverZones(City pCity)
	{
		if (_zones_to_check.Count == 0)
		{
			return;
		}
		foreach (TileZone tZone in _zones_to_check)
		{
			pCity.removeZone(tZone);
		}
	}

	private void abandonSmallAreas(City pCity)
	{
		for (int i = 0; i < _split_areas.Count; i++)
		{
			ListPool<TileZone> tList = _split_areas[i];
			for (int j = 0; j < tList.Count; j++)
			{
				TileZone tZone = tList[j];
				pCity.removeZone(tZone);
			}
		}
	}

	private void prepareCityZones(City pCity)
	{
		_zones_to_check.UnionWith(pCity.zones);
	}

	internal override void clearAll()
	{
		base.clearAll();
		foreach (ListPool<TileZone> split_area in _split_areas)
		{
			split_area.Dispose();
		}
		_split_areas.Clear();
		_zones_to_check.Clear();
	}

	private static int sorter(ListPool<TileZone> pList1, ListPool<TileZone> pList2)
	{
		return pList2.Count.CompareTo(pList1.Count);
	}

	protected override void queueConnection(ZoneConnection pConnection, Queue<ZoneConnection> pWave, bool pCheck = false)
	{
		base.queueConnection(pConnection, pWave, pCheck);
		_zones_to_check.Remove(pConnection.zone);
	}
}
