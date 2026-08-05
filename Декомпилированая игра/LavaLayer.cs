using System.Collections.Generic;

public class LavaLayer : MapLayer
{
	private List<WorldTile> _to_clear = new List<WorldTile>();

	internal override void create()
	{
		base.create();
	}

	protected override void checkAutoDisable()
	{
		bool tEnabled = WorldBehaviourActionLava.hasLava();
		if (!MapBox.isRenderMiniMap())
		{
			tEnabled = false;
		}
		if (tEnabled)
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

	public override void update(float pElapsed)
	{
		checkAutoDisable();
		updateLava();
	}

	public override void draw(float pElapsed)
	{
	}

	private void updateLava()
	{
		bool tUpdate = false;
		if (_to_clear.Count > 0)
		{
			foreach (WorldTile tTile in _to_clear)
			{
				pixels[tTile.data.tile_id] = Toolbox.clear;
			}
			_to_clear.Clear();
			tUpdate = true;
		}
		if (WorldBehaviourActionLava.hasLava())
		{
			tUpdate = true;
			drawLavaPixel(TileLibrary.lava0);
			drawLavaPixel(TileLibrary.lava1);
			drawLavaPixel(TileLibrary.lava2);
			drawLavaPixel(TileLibrary.lava3);
		}
		if (tUpdate)
		{
			updatePixels();
		}
	}

	internal override void clear()
	{
		base.clear();
		_to_clear.Clear();
	}

	private void drawLavaPixel(TileType pType)
	{
		if (pType.hashset.Count == 0)
		{
			return;
		}
		foreach (WorldTile tTile in pType.hashset)
		{
			pixels[tTile.data.tile_id] = pType.color;
			_to_clear.Add(tTile);
		}
	}
}
