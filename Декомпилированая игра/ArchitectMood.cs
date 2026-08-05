using System;
using UnityEngine;

[Serializable]
public class ArchitectMood : Asset, ILocalizedAsset
{
	public string color_main;

	public string color_text;

	public string path_icon;

	private Color _cached_color = Color.clear;

	private Color _cached_color_text = Color.clear;

	private Sprite _cached_sprite;

	public Sprite getSprite()
	{
		if (_cached_sprite == null)
		{
			_cached_sprite = SpriteTextureLoader.getSprite(path_icon);
		}
		return _cached_sprite;
	}

	public string getLocaleID()
	{
		return "architect_mood_" + id;
	}

	public Color getColor()
	{
		if (_cached_color == Color.clear)
		{
			_cached_color = Toolbox.makeColor(color_main);
		}
		return _cached_color;
	}

	public Color getColorText()
	{
		if (_cached_color_text == Color.clear)
		{
			_cached_color_text = Toolbox.makeColor(color_text);
		}
		return _cached_color_text;
	}
}
