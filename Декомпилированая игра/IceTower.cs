public class IceTower : BaseBuildingComponent
{
	private float _action_interval = 0.3f;

	private float _action_timer;

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		if (_action_timer > 0f)
		{
			_action_timer -= pElapsed;
			return;
		}
		_action_timer = _action_interval;
		freezeRandomTile();
	}

	private void freezeRandomTile()
	{
		WorldTile tCenter = building.current_tile;
		TileIsland tIsland = tCenter.region?.island;
		if (tIsland != null)
		{
			WorldTile tTile = tIsland.regions.GetRandom().tiles.GetRandom();
			freeze(tCenter, tTile);
		}
	}

	private void freeze(WorldTile pCenter, WorldTile pTile)
	{
		if (!(Toolbox.DistTile(pCenter, pTile) > 50f))
		{
			pTile.freeze();
		}
	}
}
