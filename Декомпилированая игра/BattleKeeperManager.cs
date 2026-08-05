using System.Collections.Generic;
using UnityEngine;

public static class BattleKeeperManager
{
	private const int MAX_FRAMES = 8;

	private static HashSet<BattleContainer> _hashset;

	private static readonly List<BattleContainer> _to_remove = new List<BattleContainer>();

	public static void clear()
	{
		if (_hashset == null)
		{
			_hashset = new HashSet<BattleContainer>();
		}
		_hashset.Clear();
		_to_remove.Clear();
	}

	public static HashSet<BattleContainer> get()
	{
		return _hashset;
	}

	public static void update(float pElapsed)
	{
		if (_hashset.Count == 0)
		{
			return;
		}
		foreach (BattleContainer tCont in _hashset)
		{
			if (tCont.timer > 1f)
			{
				tCont.timer -= pElapsed;
				tCont.timer = Mathf.Clamp(tCont.timer, 1f, tCont.timer);
			}
			if (tCont.isRendered())
			{
				if (tCont.timer_animation > 0f)
				{
					tCont.timer_animation -= pElapsed;
				}
				else
				{
					tCont.timer_animation = 0.04f;
					tCont.frame++;
					if (tCont.frame >= 8)
					{
						tCont.frame = 7;
					}
				}
			}
			if (tCont.timeout > 0f)
			{
				tCont.timeout -= pElapsed;
				continue;
			}
			tCont.timer -= pElapsed;
			if (tCont.timer <= 0f)
			{
				_to_remove.Add(tCont);
			}
		}
		if (_to_remove.Count <= 0)
		{
			return;
		}
		foreach (BattleContainer tCont2 in _to_remove)
		{
			_hashset.Remove(tCont2);
		}
		_to_remove.Clear();
	}

	public static void addUnitKilled(Actor pActor)
	{
		BattleContainer tCont = null;
		foreach (BattleContainer iCont in _hashset)
		{
			if ((float)Toolbox.SquaredDistTile(iCont.tile, pActor.current_tile) < 1600f)
			{
				tCont = iCont;
				break;
			}
		}
		if (tCont != null || pActor.isSapient())
		{
			if (tCont == null)
			{
				tCont = new BattleContainer();
				tCont.tile = pActor.current_tile;
				_hashset.Add(tCont);
			}
			tCont.increaseDeaths(pActor);
			if (tCont.tile != pActor.current_tile && ((float)Toolbox.SquaredDistTile(tCont.tile, pActor.current_tile) < 25f || tCont.getDeathsTotal() < 3))
			{
				tCont.tile = pActor.current_tile;
			}
			tCont.timer = 1.2f;
			tCont.timeout = 1f;
			if (tCont.frame >= 7)
			{
				tCont.frame = 0;
			}
		}
	}
}
