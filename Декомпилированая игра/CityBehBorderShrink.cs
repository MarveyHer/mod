using System.Collections.Generic;
using ai.behaviours;

public class CityBehBorderShrink : BehaviourActionCity
{
	public override bool errorsFound(City pCity)
	{
		return false;
	}

	public override bool shouldRetry(City pCity)
	{
		return false;
	}

	public override BehResult execute(City pCity)
	{
		if (BehaviourActionBase<City>.world.getWorldTimeElapsedSince(pCity.timestamp_shrink) < SimGlobals.m.empty_city_borders_shrink_time)
		{
			return BehResult.Stop;
		}
		if (pCity.hasUnits())
		{
			return BehResult.Stop;
		}
		TileZone tZoneToRemove = getZoneToRemove(pCity);
		if (tZoneToRemove == null)
		{
			return BehResult.Stop;
		}
		pCity.removeZone(tZoneToRemove);
		pCity.timestamp_shrink = BehaviourActionBase<City>.world.getCurWorldTime();
		return BehResult.Continue;
	}

	private TileZone getZoneToRemove(City pCity)
	{
		TileZone tZoneToRemove = null;
		if (pCity.border_zones.Count > 0)
		{
			tZoneToRemove = getRandomZoneFromList(pCity.border_zones);
		}
		else if (pCity.zones.Count > 0)
		{
			tZoneToRemove = getRandomZoneFromList(pCity.zones);
		}
		return tZoneToRemove;
	}

	private TileZone getRandomZoneFromList(IReadOnlyCollection<TileZone> pList)
	{
		if (pList.Count == 0)
		{
			return null;
		}
		using ListPool<TileZone> tZones = new ListPool<TileZone>(pList);
		return tZones.GetRandom();
	}
}
