using System.Collections.Generic;
using UnityEngine;

public class MapLayer : BaseMapObject
{
	public bool autoDisable;

	public bool autoDisableCheckPixels;

	public int textureID;

	protected float timer;

	protected Color colorValues;

	protected int colors_amount = 1;

	internal SpriteRenderer sprRnd;

	internal Texture2D texture;

	internal Color32[] pixels;

	internal HashSetWorldTile pixels_to_update;

	protected List<Color32> colors;

	internal HashSetWorldTile hashsetTiles;

	private int textureWidth;

	private int textureHeight;

	public bool rewriteSortingLayer = true;

	internal override void create()
	{
		base.create();
		pixels_to_update = new HashSetWorldTile();
		sprRnd = base.gameObject.GetComponent<SpriteRenderer>();
		if (rewriteSortingLayer)
		{
			sprRnd.sortingLayerName = World.world.GetComponent<SpriteRenderer>().sortingLayerName;
		}
		colors = new List<Color32>();
		createColors();
	}

	protected virtual void checkAutoDisable()
	{
		if (!autoDisable)
		{
			return;
		}
		if (autoDisableCheckPixels)
		{
			if (pixels_to_update.Count > 0)
			{
				if (!sprRnd.enabled)
				{
					sprRnd.enabled = true;
				}
			}
			else if (sprRnd.enabled)
			{
				sprRnd.enabled = false;
			}
		}
		else if (hashsetTiles.Count > 0)
		{
			if (!sprRnd.enabled)
			{
				sprRnd.enabled = true;
			}
		}
		else if (sprRnd.enabled)
		{
			sprRnd.enabled = false;
		}
	}

	internal void createTextureNew()
	{
		if (texture == null || MapBox.width != textureWidth || MapBox.height != texture.height)
		{
			if (sprRnd.sprite != null && textureWidth != 0)
			{
				Texture2DStorage.addToStorage(sprRnd.sprite, textureWidth, textureHeight);
			}
			textureWidth = MapBox.width;
			textureHeight = MapBox.height;
			sprRnd.sprite = Texture2DStorage.getSprite(textureWidth, textureHeight);
			texture = sprRnd.sprite.texture;
			textureID = texture.GetHashCode();
			int tSize = texture.height * texture.width;
			Color32 tClear = Color.clear;
			pixels = new Color32[tSize];
			for (int i = 0; i < tSize; i++)
			{
				pixels[i] = tClear;
			}
			updatePixels();
		}
	}

	public bool contains(WorldTile pTile)
	{
		return pixels_to_update.Contains(pTile);
	}

	internal virtual void clear()
	{
		if (pixels != null)
		{
			pixels_to_update.Clear();
			Color32 tClear = Color.clear;
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = tClear;
			}
			updatePixels();
		}
	}

	public void setRendererEnabled(bool pBool)
	{
		sprRnd.enabled = pBool;
	}

	protected void createColors()
	{
		for (int i = 0; i < colors_amount; i++)
		{
			float tVal = ((i <= 0) ? 0f : (1f / (float)colors_amount * (float)i));
			colors.Add(new Color(colorValues.r, colorValues.g, colorValues.b, tVal * colorValues.a));
		}
	}

	public override void update(float pElapsed)
	{
		checkAutoDisable();
	}

	public virtual void draw(float pElapsed)
	{
		if (sprRnd.enabled)
		{
			UpdateDirty(pElapsed);
		}
	}

	internal void updatePixels()
	{
		texture.SetPixels32(pixels);
		texture.Apply();
	}

	protected virtual void UpdateDirty(float pElapsed)
	{
	}
}
