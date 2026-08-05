using UnityEngine;

public class FireLayer : MapLayer
{
	internal override void create()
	{
		base.create();
	}

	public void setTileDirty(WorldTile pTile)
	{
		if (!pixels_to_update.Contains(pTile))
		{
			pixels_to_update.Add(pTile);
		}
	}

	protected override void checkAutoDisable()
	{
		bool tEnabled = WorldBehaviourActionFire.hasFires();
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

	protected override void UpdateDirty(float pElapsed)
	{
		if (pixels_to_update.Count <= 0)
		{
			return;
		}
		Color tColor = Toolbox.color_fire;
		foreach (WorldTile tTile in pixels_to_update)
		{
			if (tTile.isOnFire())
			{
				float tFireTime = World.world.getWorldTimeElapsedSince(tTile.data.fire_timestamp);
				tColor.a = 0.5f + (1f - tFireTime / SimGlobals.m.fire_stop_time);
				pixels[tTile.data.tile_id] = tColor;
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
