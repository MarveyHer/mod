using System;
using UnityEngine;

namespace NeoModLoader.utils;

[Serializable]
internal class SingleSpriteMetaData
{
	public string name;

	public Rect rect;

	public SpriteAlignment alignment;

	public Vector2 pivot;

	public Vector4 border;
}
