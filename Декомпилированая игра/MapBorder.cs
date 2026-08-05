using System.Collections.Generic;
using UnityEngine;

public class MapBorder : BaseEffect
{
	private int currentState;

	private WorldTimer updateTimer;

	private WorldTimer alphaTimer;

	private int curWidth;

	private int curHeight;

	internal override void create()
	{
		base.create();
		updateTimer = new WorldTimer(0.12f, updateEffect);
		alphaTimer = new WorldTimer(0.02f, updateAlpha);
	}

	internal void generateTexture()
	{
		if (curWidth == MapBox.width && curHeight == MapBox.height)
		{
			return;
		}
		curWidth = MapBox.width;
		curHeight = MapBox.height;
		SpriteRenderer sprRnd = base.gameObject.GetComponent<SpriteRenderer>();
		Texture2D texture = new Texture2D(curWidth, curHeight, TextureFormat.RGBA32, mipChain: false);
		texture.filterMode = FilterMode.Point;
		texture.name = "MapBorder_" + curWidth + "x" + curHeight;
		int tSize = texture.height * texture.width;
		Color32[] pixels = new Color32[tSize];
		List<int> borderPixels = new List<int>();
		List<int> newPixels = new List<int>();
		int tX = 0;
		int tY = 0;
		newPixels.Clear();
		tX = 0;
		tY = 0;
		for (int i = 0; i < tSize; i++)
		{
			if (tY == 0 && !borderPixels.Contains(i))
			{
				newPixels.Add(i);
			}
			tX++;
			if (tX >= curWidth)
			{
				tX = 0;
				tY++;
			}
		}
		borderPixels.AddRange(newPixels);
		newPixels.Clear();
		tX = 0;
		tY = 0;
		for (int j = 0; j < tSize; j++)
		{
			if (tX == curWidth - 1 && !borderPixels.Contains(j))
			{
				newPixels.Add(j);
			}
			tX++;
			if (tX >= curWidth)
			{
				tX = 0;
				tY++;
			}
		}
		borderPixels.AddRange(newPixels);
		newPixels.Clear();
		tX = 0;
		tY = 0;
		for (int k = 0; k < tSize; k++)
		{
			if (tY == curHeight - 1 && !borderPixels.Contains(k))
			{
				newPixels.Add(k);
			}
			tX++;
			if (tX >= curWidth)
			{
				tX = 0;
				tY++;
			}
		}
		borderPixels.AddRange(newPixels);
		newPixels.Clear();
		tX = 0;
		tY = 0;
		for (int l = 0; l < tSize; l++)
		{
			if (tX == 0 && !borderPixels.Contains(l))
			{
				newPixels.Add(l);
			}
			tX++;
			if (tX >= curWidth)
			{
				tX = 0;
				tY++;
			}
		}
		newPixels.Reverse();
		borderPixels.AddRange(newPixels);
		int tStroke = 0;
		for (int m = 0; m < borderPixels.Count; m++)
		{
			int ii = borderPixels[m];
			if (tStroke == 0 || tStroke == 1 || tStroke == 2)
			{
				pixels[ii] = Color.white;
				tStroke++;
			}
			else
			{
				tStroke = 0;
			}
		}
		sprRnd.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
		texture.SetPixels32(pixels);
		texture.Apply();
		base.gameObject.transform.localPosition = new Vector3(curWidth / 2, curHeight / 2);
	}

	private void Update()
	{
		updateTimer.update();
		alphaTimer.update();
	}

	private void updateAlpha()
	{
		if (World.world.selected_buttons.selectedButton == null)
		{
			alpha -= 0.02f;
			if (alpha < 0f)
			{
				alpha = 0f;
			}
		}
		else
		{
			alpha += 0.02f;
			if (alpha > 0.42f)
			{
				alpha = 0.42f;
			}
		}
		if (sprite_renderer.color.a != alpha)
		{
			setAlpha(alpha);
		}
	}

	private void updateEffect()
	{
		if (alpha != 0f)
		{
			currentState++;
			if (currentState > 3)
			{
				currentState = 0;
			}
			switch (currentState)
			{
			case 0:
				sprite_renderer.flipX = false;
				sprite_renderer.flipY = false;
				break;
			case 1:
				sprite_renderer.flipX = true;
				sprite_renderer.flipY = false;
				break;
			case 2:
				sprite_renderer.flipX = true;
				sprite_renderer.flipY = true;
				break;
			case 3:
				sprite_renderer.flipX = false;
				sprite_renderer.flipY = true;
				break;
			}
		}
	}
}
