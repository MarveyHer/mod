using System;
using UnityEngine;

[Serializable]
public class HistoryGroupAsset : Asset, ILocalizedAsset
{
	public string icon_path;

	private Sprite _icon_cache;

	public string getLocaleID()
	{
		return "history_group_" + id;
	}

	public Sprite getSprite()
	{
		if (_icon_cache == null)
		{
			_icon_cache = SpriteTextureLoader.getSprite(icon_path);
		}
		return _icon_cache;
	}
}
