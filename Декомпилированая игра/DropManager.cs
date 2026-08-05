using System.Collections.Generic;
using UnityEngine;

public class DropManager
{
	private List<Drop> _drops = new List<Drop>();

	private float _timeout_timer;

	private int _activeIndex;

	private GameObject _original_drop;

	private Transform _dropContainer;

	public DropManager(Transform pDropContainer)
	{
		_dropContainer = pDropContainer;
		string tPath = "effects/p_drop";
		_original_drop = (GameObject)Resources.Load(tPath, typeof(GameObject));
	}

	public Drop spawn(WorldTile pTile, string pDropID, float zHeight = -1f, float pScale = -1f, long pCasterId = -1L)
	{
		DropAsset tAsset = AssetManager.drops.get(pDropID);
		return spawn(pTile, tAsset, zHeight, pScale, pForceSurprise: false, pCasterId);
	}

	public Drop spawn(WorldTile pTile, DropAsset pAsset, float zHeight = -1f, float pScale = -1f, bool pForceSurprise = false, long pCasterId = -1L)
	{
		Drop tDrop = getObject();
		if (pForceSurprise)
		{
			tDrop.setForceSurprise();
		}
		tDrop.launchStraight(pTile, pAsset, zHeight);
		if (pScale == -1f)
		{
			pScale = pAsset.default_scale;
		}
		tDrop.setScale(new Vector3(pScale, pScale, tDrop.transform.localScale.z));
		tDrop.setCasterId(pCasterId);
		return tDrop;
	}

	public void spawnParabolicDrop(WorldTile pTile, string pDropID, float pStartHeight = 0f, float pMinHeight = 0f, float pMaxHeight = 0f, float pMinRadius = 0f, float pMaxRadius = 0f, float pScale = -1f)
	{
		spawn(pTile, pDropID, pMinHeight, pScale, -1L).launchParabolic(pStartHeight, pMinHeight, pMaxHeight, pMinRadius, pMaxRadius);
	}

	public void clear()
	{
		List<Drop> tDrops = _drops;
		for (int i = 0; i < tDrops.Count; i++)
		{
			tDrops[i].makeInactive();
		}
		_activeIndex = 0;
	}

	private void killObject(Drop pObject)
	{
		pObject.makeInactive();
		int tDeadIndex = pObject.drop_index - 1;
		int tAliveIndex = _activeIndex - 1;
		List<Drop> tDrops = _drops;
		if (tDeadIndex != tAliveIndex)
		{
			Drop tSwitchDrop = tDrops[tAliveIndex];
			tDrops[tAliveIndex] = pObject;
			tDrops[tDeadIndex] = tSwitchDrop;
			pObject.drop_index = tAliveIndex + 1;
			tSwitchDrop.drop_index = tDeadIndex + 1;
		}
		if (_activeIndex > 0)
		{
			_activeIndex--;
		}
	}

	public void landDrop(Drop pObject)
	{
		WorldTile tTile = pObject.current_tile;
		killObject(pObject);
		if (tTile != null)
		{
			World.world.flash_effects.flashPixel(tTile, 14);
		}
	}

	public Drop getObject()
	{
		List<Drop> tDrops = _drops;
		Drop tDrop;
		if (tDrops.Count > _activeIndex)
		{
			tDrop = tDrops[_activeIndex];
		}
		else
		{
			tDrop = Object.Instantiate(_original_drop, _dropContainer).GetComponent<Drop>();
			tDrop.gameObject.layer = _dropContainer.gameObject.layer;
			tDrop.transform.parent = _dropContainer;
			tDrops.Add(tDrop);
			tDrop.drop_index = tDrops.Count;
		}
		_activeIndex++;
		tDrop.prepare();
		return tDrop;
	}

	public void update(float pElapsed)
	{
		Bench.bench("drops", "game_total");
		if (_timeout_timer > 0f)
		{
			_timeout_timer -= World.world.delta_time;
		}
		int i = 0;
		List<Drop> tDrops = _drops;
		for (i = _activeIndex - 1; i >= 0; i--)
		{
			Drop tObj = tDrops[i];
			if (tObj.created && tObj.active)
			{
				tObj.update(pElapsed);
			}
			else if (_activeIndex == tObj.drop_index)
			{
				_activeIndex--;
				Debug.LogError("do we ever hit this??? " + _activeIndex);
			}
		}
		Bench.benchEnd("drops", "game_total", pSaveCounter: false, 0L);
	}

	public void debug(DebugTool pTool)
	{
		pTool.setText("drops total", _drops.Count.ToString() ?? "", 0f, pShowBar: false, 0L);
		pTool.setText("drops active", _activeIndex.ToString() ?? "", 0f, pShowBar: false, 0L);
	}

	public int getActiveIndex()
	{
		return _activeIndex;
	}

	private void debugString()
	{
		string test = "";
		for (int i = 0; i < _drops.Count; i++)
		{
			test = ((!_drops[i].active) ? (test + "x") : (test + "O"));
		}
		Debug.Log(test + " ::: " + _activeIndex);
	}
}
