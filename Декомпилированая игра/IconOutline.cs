using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconOutline : MonoBehaviour
{
	private static Dictionary<string, Sprite> _cached_textures = new Dictionary<string, Sprite>();

	private Image _image;

	public Image parent_image;

	private void Awake()
	{
		checkInit();
	}

	private void checkInit()
	{
		if (!(_image != null))
		{
			_image = GetComponent<Image>();
			base.gameObject.AddComponent<FadeInOutAnimation>();
		}
	}

	public void show(ContainerItemColor pContainer)
	{
		checkInit();
		base.gameObject.SetActive(value: true);
		Color tColor = pContainer.color;
		tColor.a = 1f;
		_image.color = tColor;
		string tID = parent_image.sprite.texture.GetHashCode() + "_" + tColor.GetHashCode();
		Sprite tSprite = null;
		if (_cached_textures.ContainsKey(tID))
		{
			tSprite = _cached_textures[tID];
		}
		else
		{
			tSprite = generateSprite();
			_cached_textures.Add(tID, tSprite);
		}
		_image.sprite = tSprite;
	}

	private Sprite generateSprite()
	{
		int width = parent_image.sprite.texture.width;
		int tHeight = parent_image.sprite.texture.height;
		Texture2D tTexture = new Texture2D(width, tHeight);
		Color tColor = new Color(1f, 1f, 1f, 0f);
		for (int xx = 0; xx < tTexture.width; xx++)
		{
			for (int yy = 0; yy < tTexture.height; yy++)
			{
				tTexture.SetPixel(xx, yy, tColor);
			}
		}
		makePixels(-1, -1, tTexture);
		makePixels(1, 1, tTexture);
		makePixels(1, -1, tTexture);
		makePixels(-1, 1, tTexture);
		makePixels(1, 0, tTexture);
		makePixels(-1, 0, tTexture);
		makePixels(0, 1, tTexture);
		makePixels(0, -1, tTexture);
		tTexture.Apply();
		tTexture.filterMode = FilterMode.Point;
		tTexture.name = "IconOutline";
		Rect tRect = new Rect(0f, 0f, tTexture.width, tTexture.height);
		Vector2 tPivot = new Vector2(0.5f, 0.5f);
		return Sprite.Create(tTexture, tRect, tPivot, 1f);
	}

	private void makePixels(int pOffsetX, int pOffsetY, Texture2D pTexture)
	{
		for (int xx = 0; xx < pTexture.width; xx++)
		{
			for (int yy = 0; yy < pTexture.height; yy++)
			{
				if (parent_image.sprite.texture.GetPixel(xx, yy).a != 0f)
				{
					int tNewX = xx + pOffsetX;
					int tNewY = yy + pOffsetY;
					if (tNewX >= 0 && tNewX <= pTexture.width && tNewY >= 0 && tNewY <= pTexture.height)
					{
						Color tColorNew = pTexture.GetPixel(tNewX, tNewY);
						tColorNew.a += 0.3f;
						pTexture.SetPixel(tNewX, tNewY, tColorNew);
					}
				}
			}
		}
	}
}
