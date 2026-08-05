using System.Collections.Generic;

public class CrabLimbGroup
{
	public CrabLimb crabLimb;

	private CrabLimbItem[] _list;

	private CrabLimbState _dmg_state;

	private Actor actor;

	private float _flicker_timer;

	private const float _flicker_interval = 0.15f;

	public CrabLimbGroup(CrabLimb pCrabLimb, Actor pActor)
	{
		actor = pActor;
		crabLimb = pCrabLimb;
		List<CrabLimbItem> tList = new List<CrabLimbItem>();
		CrabLimbItem[] componentsInChildren = actor.avatar.GetComponentsInChildren<CrabLimbItem>(includeInactive: false);
		foreach (CrabLimbItem tCrabLimb in componentsInChildren)
		{
			if (tCrabLimb.crabLimb == crabLimb)
			{
				tList.Add(tCrabLimb);
			}
		}
		_list = tList.ToArray();
		_dmg_state = CrabLimbState.HighHP;
	}

	internal void update(float pElapsed)
	{
		if (_flicker_timer != 0f)
		{
			_flicker_timer -= pElapsed;
			if (_flicker_timer < 0f)
			{
				_flicker_timer = 0f;
			}
			float tProgress = 1f - _flicker_timer / 0.15f;
			CrabLimbItem[] list = _list;
			for (int i = 0; i < list.Length; i++)
			{
				list[i].flicker(tProgress);
			}
		}
	}

	internal void showDamage()
	{
		if (IsFlickering())
		{
			return;
		}
		int tHealth = actor.getHealth();
		int tMaxHealth = actor.getMaxHealth();
		if ((float)tHealth > (float)tMaxHealth * 0.7f)
		{
			if (_dmg_state == CrabLimbState.HighHP)
			{
				return;
			}
			_dmg_state = CrabLimbState.HighHP;
		}
		else if ((float)tHealth > (float)tMaxHealth * 0.35f)
		{
			if (_dmg_state == CrabLimbState.MedHP)
			{
				return;
			}
			_dmg_state = CrabLimbState.MedHP;
		}
		else
		{
			if (_dmg_state == CrabLimbState.LowHP)
			{
				return;
			}
			_dmg_state = CrabLimbState.LowHP;
		}
		CrabLimbItem[] list = _list;
		for (int i = 0; i < list.Length; i++)
		{
			list[i].stateChange(_dmg_state);
		}
		_flicker_timer = 0.15f;
	}

	internal bool IsFlickering()
	{
		return _flicker_timer > 0f;
	}
}
