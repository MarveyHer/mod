using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BrushData : Asset, ILocalizedAsset
{
	[DefaultValue(1)]
	public int size = 1;

	[DefaultValue(1)]
	public int drops = 1;

	public BrushGroup group;

	public bool show_in_brush_window;

	public int width;

	public int height;

	public int sqr_size;

	public bool auto_size;

	public bool continuous;

	public bool fast_spawn;

	public string localized_key;

	public BrushPixelData[] pos;

	public BrushGenerateAction generate_action;

	public Vector2 ui_scale = new Vector2(1f, 1f);

	public Vector2 ui_size = new Vector2(28f, 28f);

	[NonSerialized]
	private Sprite _sprite;

	public void setupImage(Image pSprite)
	{
		pSprite.sprite = getSprite();
		Vector2 tUiScale = ui_scale;
		Vector2 tUiSize = ui_size;
		if (height < 28)
		{
			tUiSize = new Vector2(width, height);
		}
		pSprite.rectTransform.sizeDelta = new Vector2(tUiSize.x, tUiSize.y);
		pSprite.transform.localScale = new Vector3(tUiScale.x, tUiScale.y, 1f);
	}

	public Sprite getSprite()
	{
		if (_sprite != null)
		{
			return _sprite;
		}
		Texture2D tTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		Color[] tTransparentPixels = new Color[width * height];
		for (int i = 0; i < tTransparentPixels.Length; i++)
		{
			tTransparentPixels[i] = Color.clear;
		}
		tTexture.SetPixels(tTransparentPixels);
		Color tColor = Color.white;
		int tMinX = 0;
		int tMinY = 0;
		BrushPixelData[] array = pos;
		for (int j = 0; j < array.Length; j++)
		{
			BrushPixelData tPixel = array[j];
			if (tPixel.x < tMinX)
			{
				tMinX = tPixel.x;
			}
			if (tPixel.y < tMinY)
			{
				tMinY = tPixel.y;
			}
		}
		array = pos;
		for (int j = 0; j < array.Length; j++)
		{
			BrushPixelData tPixel2 = array[j];
			tTexture.SetPixel(tPixel2.x - tMinX, tPixel2.y - tMinY, tColor);
		}
		tTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		Rect tRect = new Rect(0f, 0f, tTexture.width, tTexture.height);
		Vector2 tPivot = new Vector2(0f, 0f);
		_sprite = Sprite.Create(tTexture, tRect, tPivot, 1f);
		_sprite.name = id;
		return _sprite;
	}

	public string getLocaleID()
	{
		return localized_key;
	}
}
