using System.Collections.Generic;
using UnityEngine;

public class ExplosionChecker
{
	private const float TIMER = 1f;

	private Dictionary<int, ExplosionMemoryData> data = new Dictionary<int, ExplosionMemoryData>();

	private List<int> _to_remove = new List<int>(16);

	public bool checkNearby(WorldTile pTile, int pRange)
	{
		int tID = pRange * 10000000 + pTile.x * 1000 + pTile.y;
		if (data.ContainsKey(tID) || isNearbyOthers(pTile, pRange / 3))
		{
			return true;
		}
		add(tID, pTile, pRange);
		updateNearbyTimers(pTile, pRange);
		return false;
	}

	private void updateNearbyTimers(WorldTile pTile, float pRange)
	{
		float tTimer = 1f;
		float tRange = pRange;
		tTimer += (float)(data.Count / 10);
		tRange += (float)(data.Count / 5);
		tTimer = Mathf.Clamp(tTimer, 1f, 5f);
		tRange = Mathf.Clamp(tRange, pRange, pRange * 5f);
		foreach (int tKey in data.Keys)
		{
			ExplosionMemoryData tData = data[tKey];
			if (Toolbox.Dist(pTile.x, pTile.y, tData.x, tData.y) < tRange)
			{
				tData.timer = tTimer;
			}
		}
	}

	private bool isNearbyOthers(WorldTile pTile, float pRange)
	{
		foreach (ExplosionMemoryData tData in data.Values)
		{
			if (Toolbox.Dist(pTile.x, pTile.y, tData.x, tData.y) < pRange)
			{
				return true;
			}
		}
		return false;
	}

	private void add(int pID, WorldTile pTile, int pRange)
	{
		ExplosionMemoryData tData = new ExplosionMemoryData();
		tData.range = pRange;
		tData.x = pTile.x;
		tData.y = pTile.y;
		float tTimer = 1f;
		tTimer += (float)(data.Count / 10);
		tTimer = Mathf.Clamp(tTimer, 1f, 5f);
		tData.timer = tTimer;
		data.Add(pID, tData);
	}

	public void update(float pElapsed)
	{
		Bench.bench("explosion_checker", "game_total");
		foreach (int tId in data.Keys)
		{
			ExplosionMemoryData explosionMemoryData = data[tId];
			explosionMemoryData.timer -= pElapsed;
			if (explosionMemoryData.timer <= 0f)
			{
				_to_remove.Add(tId);
			}
		}
		if (_to_remove.Count > 0)
		{
			for (int i = 0; i < _to_remove.Count; i++)
			{
				data.Remove(_to_remove[i]);
			}
			_to_remove.Clear();
		}
		Bench.benchEnd("explosion_checker", "game_total", pSaveCounter: false, 0L);
	}

	public void clear()
	{
		data.Clear();
	}

	public static void debug(DebugTool pTool)
	{
		pTool.setText("explosion_checker", MapBox.instance.explosion_checker.data.Count, 0f, pShowBar: false, 0L);
	}
}
