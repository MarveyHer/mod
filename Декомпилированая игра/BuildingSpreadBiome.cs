public class BuildingSpreadBiome : BaseBuildingComponent
{
	private const float SPREAD_INTERVAL_MIN = 8f;

	private const float SPREAD_INTERVAL_MAX = 16f;

	private const int SPREAD_RANGE = 2;

	private BiomeAsset _biome_asset;

	private float _spread_timer = 1f;

	internal override void create(Building pBuilding)
	{
		base.create(pBuilding);
		_biome_asset = AssetManager.biome_library.get(pBuilding.asset.spread_biome_id);
	}

	public override void update(float pElapsed)
	{
		if (WorldLawLibrary.world_law_terramorphing.isEnabled())
		{
			base.update(pElapsed);
			if (_spread_timer > 0f)
			{
				_spread_timer -= pElapsed;
				return;
			}
			_spread_timer = Randy.randomFloat(8f, 16f);
			spreadBiome();
		}
	}

	private void spreadBiome()
	{
		TileTypeBase tSpreadType = _biome_asset.getTileHigh();
		WorldTile tTile = ((building.current_tile.Type.biome_asset == tSpreadType.biome_asset) ? Toolbox.getRandomTileWithinDistance(building.current_tile, 2) : building.current_tile);
		WorldBehaviourActionBiomes.trySpreadBiomeAround(tTile, tSpreadType, pCheckRoad: false, pCheckBonuses: false, pForce: false, pSkipEraCheck: true);
	}

	public override void Dispose()
	{
		_biome_asset = null;
		base.Dispose();
	}
}
