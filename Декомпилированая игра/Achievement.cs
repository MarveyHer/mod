using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Achievement : Asset, IDescriptionAsset, ILocalizedAsset
{
	public string play_store_id;

	public string steam_id;

	public bool hidden;

	public string group = "miscellaneous";

	public string icon;

	public string locale_key;

	public AchievementCheck action;

	public bool unlocks_something;

	public List<BaseUnlockableAsset> unlock_assets;

	[NonSerialized]
	protected Sprite cached_sprite;

	[NonSerialized]
	private SignalAsset _signal;

	public bool has_signal;

	public void checkBySignal(object pCheckData = null)
	{
		SignalManager.add(_signal, pCheckData);
	}

	public bool check(object pCheckData = null)
	{
		if (AchievementLibrary.isUnlocked(this))
		{
			return true;
		}
		bool tCheck = true;
		if (action != null)
		{
			tCheck = action(pCheckData);
		}
		if (tCheck)
		{
			AchievementLibrary.unlock(this);
			checkUnlockables();
			return true;
		}
		return false;
	}

	private void checkUnlockables()
	{
		if (!isUnlocked() || !unlocks_something)
		{
			return;
		}
		foreach (BaseUnlockableAsset unlock_asset in unlock_assets)
		{
			unlock_asset.unlock();
		}
	}

	public bool isUnlocked()
	{
		return AchievementLibrary.isUnlocked(this);
	}

	public string getLocaleID()
	{
		return locale_key;
	}

	public string getDescriptionID()
	{
		return getLocaleID() + "_description";
	}

	public Sprite getIcon()
	{
		if ((object)cached_sprite == null)
		{
			cached_sprite = SpriteTextureLoader.getSprite(icon);
		}
		if (cached_sprite == null)
		{
			Debug.LogError("Error: Sprite not found : " + icon);
		}
		return cached_sprite;
	}

	public void setSignal(SignalAsset pSignal)
	{
		_signal = pSignal;
	}

	public SignalAsset getSignal()
	{
		return _signal;
	}
}
