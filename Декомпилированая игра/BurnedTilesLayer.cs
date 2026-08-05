using UnityEngine;

public class BurnedTilesLayer : MapLayer
{
	public Color color;

	private WorldBehaviour worldBehaviour;

	internal override void create()
	{
		colorValues = new Color(color.r, color.g, color.b, 0.5f);
		colors_amount = 15;
		autoDisable = false;
		base.create();
		base.enabled = true;
	}

	public void setTileDirty(WorldTile pTile)
	{
		if (!pixels_to_update.Contains(pTile))
		{
			pixels_to_update.Add(pTile);
		}
	}

	protected override void UpdateDirty(float pElapsed)
	{
		if (pixels_to_update.Count <= 0)
		{
			return;
		}
		foreach (WorldTile tTile in pixels_to_update)
		{
			if (tTile.burned_stages > 0)
			{
				pixels[tTile.data.tile_id] = colors[tTile.burned_stages - 1];
			}
			else
			{
				pixels[tTile.data.tile_id] = Toolbox.clear;
			}
		}
		pixels_to_update.Clear();
		updatePixels();
	}
}
