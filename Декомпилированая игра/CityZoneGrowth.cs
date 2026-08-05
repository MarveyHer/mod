using System.Collections.Generic;

public class CityZoneGrowth : CityZoneWorkerBase
{
	private const float MOD_RADIUS = 0.75f;

	public TileZone getZoneToClaim(Actor pActor, City pCity, bool pDebug = false, HashSet<TileZone> pSetToFill = null, int pBonusRange = 0)
	{
		clearAll();
		WorldTile tTile = pCity.getTile();
		if (tTile == null)
		{
			return null;
		}
		bool tStopWaveWhenEmptyZoneFound = !pDebug;
		startWaveFromTile(pActor, tTile, pCity, tStopWaveWhenEmptyZoneFound, pBonusRange);
		if (pDebug && pSetToFill != null)
		{
			foreach (ZoneConnection item in _zones_checked)
			{
				pSetToFill.Add(item.zone);
			}
			return null;
		}
		return checkGrowBorder(pCity);
	}

	private TileZone checkGrowBorder(City pCity)
	{
		bool num = Randy.randomChance(0.7f);
		TileZone tResultZone = null;
		if (num)
		{
			TileZone tRandomZone = getRandomZone(pCity);
			if (tRandomZone != null)
			{
				tResultZone = tRandomZone;
			}
		}
		else
		{
			tResultZone = getRandomCheckedZone(pCity);
		}
		return tResultZone;
	}

	private TileZone getRandomZone(City pCity)
	{
		using ListPool<TileZone> tListZonesToCheck = new ListPool<TileZone>(pCity.border_zones);
		WorldTile tMainCityTile = pCity.getTile();
		if (tMainCityTile == null)
		{
			return null;
		}
		TileZone tMainCityZone = tMainCityTile.zone;
		float tMaxRadius = (float)pCity.getZoneRange() * 0.75f;
		tMaxRadius *= tMaxRadius;
		foreach (TileZone item in tListZonesToCheck.LoopRandom())
		{
			foreach (TileZone tZone in item.neighbours.LoopRandom())
			{
				if (tZone.canBeClaimedByCity(pCity) && tZone.centerTile.isSameIsland(tMainCityTile) && !((float)Toolbox.SquaredDist(tMainCityZone.x, tMainCityZone.y, tZone.x, tZone.y) > tMaxRadius))
				{
					return tZone;
				}
			}
		}
		return null;
	}

	private TileZone getBestZoneFromList(City pCity, List<TileZone> pList)
	{
		TileZone tResultZone = null;
		TileZone tMainCityZone = pCity.getTile().zone;
		int tBestDist = int.MaxValue;
		for (int i = 0; i < pList.Count; i++)
		{
			TileZone tZone = pList[i];
			int tCurrentDist = Toolbox.SquaredDist(tZone.x, tZone.y, tMainCityZone.x, tMainCityZone.y);
			if (tCurrentDist < tBestDist)
			{
				tResultZone = tZone;
				tBestDist = tCurrentDist;
			}
		}
		return tResultZone;
	}

	private TileZone getRandomCheckedZone(City pCity)
	{
		using ListPool<TileZone> tListZones = new ListPool<TileZone>(_zones_checked.Count);
		foreach (ZoneConnection item in _zones_checked)
		{
			TileZone tZone = item.zone;
			if (tZone.canBeClaimedByCity(pCity))
			{
				tListZones.Add(tZone);
			}
		}
		if (tListZones.Count > 0)
		{
			return tListZones.GetRandom();
		}
		return null;
	}

	private void startWaveFromTile(Actor pActor, WorldTile pTile, City pCity, bool pStopWaveWhenEmptyZoneFound = true, int pBonusRange = 0)
	{
		prepareWave();
		if (pActor == null)
		{
			pActor = pCity.leader;
		}
		Queue<ZoneConnection> tWaveQ = _wave;
		Queue<ZoneConnection> tNextWaveQ = _next_wave;
		queueConnection(new ZoneConnection(pTile.zone, pTile.region), tWaveQ, pSetChecked: true);
		using ListPool<MapRegion> tListRegions = new ListPool<MapRegion>();
		int tMaxWaves = pCity.getZoneRange() + pBonusRange;
		float tMaxRadius = (float)tMaxWaves * 0.75f;
		tMaxRadius *= tMaxRadius;
		int tWaveID = 0;
		bool tEmptyZoneFound = false;
		while ((tNextWaveQ.Count > 0 || tWaveQ.Count > 0) && !(pStopWaveWhenEmptyZoneFound && tEmptyZoneFound))
		{
			if (tWaveQ.Count == 0)
			{
				Queue<ZoneConnection> queue = tWaveQ;
				tWaveQ = tNextWaveQ;
				tNextWaveQ = queue;
				tWaveID++;
				if (tWaveID > tMaxWaves)
				{
					break;
				}
			}
			ZoneConnection zoneConnection = tWaveQ.Dequeue();
			TileZone tMainZone = zoneConnection.zone;
			MapRegion tMainRegion = zoneConnection.region;
			for (int i = 0; i < tMainZone.neighbours.Length; i++)
			{
				TileZone tZone = tMainZone.neighbours[i];
				if ((pStopWaveWhenEmptyZoneFound && tZone.hasCity() && !tZone.isSameCityHere(pCity)) || tZone.tiles_with_ground == 0 || (pActor != null && pActor.hasSubspecies() && !tZone.checkCanSettleInThisBiomes(pActor.subspecies)))
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
					if (!_zones_checked.Add(tNewConnection))
					{
						continue;
					}
					if (tZone.canBeClaimedByCity(pCity))
					{
						tEmptyZoneFound = true;
					}
					if (!((float)Toolbox.SquaredDist(pTile.zone.x, pTile.zone.y, tZone.x, tZone.y) > tMaxRadius))
					{
						if (pStopWaveWhenEmptyZoneFound && tEmptyZoneFound)
						{
							break;
						}
						queueConnection(tNewConnection, tNextWaveQ);
					}
				}
			}
		}
	}
}
