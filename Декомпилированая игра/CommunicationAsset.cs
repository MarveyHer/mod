using System;
using UnityEngine;

[Serializable]
public class CommunicationAsset : Asset
{
	public string icon_path;

	public bool show_topic;

	public float rate;

	public TopicCheck check;

	public TopicPotFill pot_fill;

	[NonSerialized]
	private Sprite _sprite_cache;

	public Sprite getSpriteBubble()
	{
		if (_sprite_cache == null)
		{
			_sprite_cache = SpriteTextureLoader.getSprite(icon_path);
		}
		return _sprite_cache;
	}
}
