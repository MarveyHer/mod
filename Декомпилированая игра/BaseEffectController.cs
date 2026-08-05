using System.Collections.Generic;
using UnityEngine;

public class BaseEffectController : BaseMapObject
{
	public Transform prefab;

	private int _active_index;

	private readonly List<BaseEffect> _list = new List<BaseEffect>();

	private float _timer;

	private float _timer_interval = 1f;

	private bool _object_limit_used;

	private int _object_limit;

	private bool _limit_unload;

	public bool useInterval = true;

	public EffectAsset asset;

	internal override void create()
	{
		base.create();
		_timer_interval = 0.9f;
	}

	public void setLimits(int pLimitObjects, bool pLimitUnload)
	{
		if (pLimitObjects > 0)
		{
			_object_limit_used = true;
		}
		_object_limit = pLimitObjects;
		_limit_unload = pLimitUnload;
	}

	public BaseEffect GetObject()
	{
		BaseEffect tNewEffect = null;
		List<BaseEffect> tList = _list;
		if (tList.Count > _active_index)
		{
			tNewEffect = tList[_active_index];
		}
		else
		{
			tNewEffect = Object.Instantiate(prefab).gameObject.GetComponent<BaseEffect>();
			addNewObject(tNewEffect);
			if (!tNewEffect.created)
			{
				tNewEffect.create();
			}
			tList.Add(tNewEffect);
			tNewEffect.effectIndex = tList.Count;
		}
		_active_index++;
		tNewEffect.activate();
		return tNewEffect;
	}

	public int getActiveIndex()
	{
		return _active_index;
	}

	internal void addNewObject(BaseEffect pEffect)
	{
		pEffect.controller = this;
		pEffect.transform.parent = base.transform;
	}

	public void killObject(BaseEffect pObject)
	{
		if (pObject.active)
		{
			makeInactive(pObject);
			List<BaseEffect> tList = _list;
			int deadIndex = pObject.effectIndex - 1;
			int aliveIndex = _active_index - 1;
			if (deadIndex != aliveIndex)
			{
				BaseEffect switchObject = tList[aliveIndex];
				tList[aliveIndex] = pObject;
				tList[deadIndex] = switchObject;
				pObject.effectIndex = aliveIndex + 1;
				switchObject.effectIndex = deadIndex + 1;
			}
			if (_active_index > 0)
			{
				_active_index--;
			}
		}
	}

	private void makeInactive(BaseEffect pObject)
	{
		pObject.deactivate();
	}

	private void debugString()
	{
		string test = "";
		List<BaseEffect> tList = _list;
		for (int i = 0; i < tList.Count; i++)
		{
			test = ((!tList[i].active) ? (test + "x") : (test + "O"));
		}
		Debug.Log(test + " ::: " + _active_index);
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		updateChildren(pElapsed);
		updateSpawn(pElapsed);
	}

	private void updateSpawn(float pElapsed)
	{
		if (!World.world.isPaused() && useInterval)
		{
			if (_timer > 0f)
			{
				_timer -= pElapsed;
				return;
			}
			_timer = _timer_interval;
			spawn();
		}
	}

	private void updateChildren(float pElapsed)
	{
		List<BaseEffect> tList = _list;
		for (int i = _active_index - 1; i >= 0; i--)
		{
			BaseEffect tObj = tList[i];
			if (tObj.created && tObj.active)
			{
				tObj.update(pElapsed);
			}
		}
	}

	public virtual void spawn()
	{
	}

	public BaseEffect spawnNew()
	{
		if (isLimitReached())
		{
			if (!_limit_unload)
			{
				return null;
			}
			killOldest();
		}
		BaseEffect tObject = GetObject();
		if (tObject.sprite_animation != null)
		{
			tObject.sprite_animation.resetAnim();
		}
		return tObject;
	}

	private void killOldest()
	{
		if (_list.Count == 0)
		{
			return;
		}
		BaseEffect tEffectOldest = _list[0];
		double tEffectOldestTimestamp = double.MaxValue;
		foreach (BaseEffect tEffect in _list)
		{
			if (tEffect.timestamp_spawned < tEffectOldestTimestamp)
			{
				tEffectOldest = tEffect;
				tEffectOldestTimestamp = tEffect.timestamp_spawned;
			}
		}
		killObject(tEffectOldest);
	}

	internal bool isLimitReached()
	{
		if (_object_limit_used && _active_index >= _object_limit)
		{
			return true;
		}
		return false;
	}

	internal void clear()
	{
		List<BaseEffect> tList = _list;
		for (int i = 0; i < tList.Count; i++)
		{
			BaseEffect tEffect = tList[i];
			makeInactive(tEffect);
		}
		_active_index = 0;
	}

	public bool isAnyActive()
	{
		return _active_index > 0;
	}

	internal void debug(DebugTool pTool)
	{
		pTool.setText(base.name, _active_index + "/" + _list.Count, 0f, pShowBar: false, 0L);
	}

	internal List<BaseEffect> getList()
	{
		return _list;
	}
}
