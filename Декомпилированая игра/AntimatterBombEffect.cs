public class AntimatterBombEffect : BaseEffect
{
	private bool used;

	private void Update()
	{
		World.world.startShake(0.03f, 0.01f, 0.3f);
		if (sprite_animation.currentFrameIndex >= 6 && !used)
		{
			used = true;
			World.world.applyForceOnTile(tile, 10, 0f, pForceOut: true, 1000);
			World.world.loopWithBrush(tile, Brush.get(11), tileAntimatter);
		}
	}

	public bool tileAntimatter(WorldTile pTile, string pPowerID)
	{
		TileType tToChange = TileLibrary.pit_deep_ocean;
		bool tSkipTerraform = false;
		if (!MapAction.checkTileDamageGaiaCovenant(pTile, pDamage: true))
		{
			tToChange = null;
			tSkipTerraform = true;
		}
		MapAction.terraformMain(pTile, tToChange, TerraformLibrary.destroy_no_flash, tSkipTerraform);
		return true;
	}

	internal override void spawnOnTile(WorldTile pTile)
	{
		tile = pTile;
		used = false;
		prepare(pTile);
		resetAnim();
	}
}
