using System.IO;
using UnityEngine;

namespace NCMS.Utils;

public class Sprites
{
	public static Sprite LoadSprite(string path, float offsetX = 0f, float offsetY = 0f)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(0, 0);
		((Texture)val).anisoLevel = 0;
		((Texture)val).filterMode = (FilterMode)0;
		ImageConversion.LoadImage(val, File.ReadAllBytes(path));
		return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(offsetX, offsetY), 1f);
	}
}
