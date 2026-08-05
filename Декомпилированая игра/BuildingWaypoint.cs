public abstract class BuildingWaypoint : BaseBuildingComponent
{
	private const int UNITS_AFFECTED_PER_ACTION = 5;

	private const float ACTION_INTERVAL = 20f;

	private float _action_timer = 20f;

	protected abstract string effect_id { get; }

	protected abstract string trait_id { get; }

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		if (_action_timer > 0f)
		{
			_action_timer -= pElapsed;
			return;
		}
		_action_timer = 20f;
		doAction(building.current_tile);
	}

	internal void doAction(WorldTile pFromTile)
	{
		spawnMainEffect();
		World.world.applyForceOnTile(building.current_tile, 10, 3f);
		int tCount = 0;
		foreach (Actor tActor in Finder.getUnitsFromChunk(pFromTile, 1, 0f, pRandom: true))
		{
			if (!tActor.hasTrait(trait_id))
			{
				if (tActor.addTrait(trait_id))
				{
					tCount++;
				}
				if (tCount >= 5)
				{
					break;
				}
			}
		}
	}

	public void spawnMainEffect()
	{
		EffectsLibrary.spawnAt(effect_id, building.current_tile.posV3, building.current_scale.y);
	}
}
