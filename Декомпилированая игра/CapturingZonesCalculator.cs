using System.Collections.Generic;

public class CapturingZonesCalculator
{
	private static int _zoneTicks = 0;

	private static Queue<TileZone> _currentWave = new Queue<TileZone>();

	private static Queue<TileZone> _nextWave = new Queue<TileZone>();

	private static HashSet<TileZone> _waveChecked = new HashSet<TileZone>();

	public static void getListToDraw(City pCity, int pTicks, ListPool<TileZone> pResults)
	{
		pResults.Clear();
		TileZone tCityZone = pCity.getTile()?.zone;
		if (tCityZone == null)
		{
			tCityZone = pCity.zones[0];
		}
		Queue<TileZone> tCurrentWave = _currentWave;
		tCurrentWave.Enqueue(tCityZone);
		_zoneTicks = pTicks;
		while (tCurrentWave.Count > 0 && _zoneTicks != 0)
		{
			TileZone tZone = tCurrentWave.Dequeue();
			check(tZone, pCity);
			pResults.Add(tZone);
			if (tCurrentWave.Count == 0)
			{
				Queue<TileZone> nextWave = tCurrentWave;
				tCurrentWave = _nextWave;
				_nextWave = nextWave;
			}
		}
		_nextWave.Clear();
		_waveChecked.Clear();
		tCurrentWave.Clear();
	}

	private static void check(TileZone pTargetZone, City pCity)
	{
		_zoneTicks--;
		_waveChecked.Add(pTargetZone);
		TileZone[] tNeighbours = pTargetZone.neighbours;
		foreach (TileZone tZone in tNeighbours)
		{
			if (tZone.city == pCity && !_waveChecked.Contains(tZone))
			{
				_waveChecked.Add(tZone);
				_nextWave.Enqueue(tZone);
			}
		}
	}
}
