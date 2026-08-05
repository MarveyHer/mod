using UnityEngine;

public class BuildingSmokeEffect : BaseBuildingComponent
{
	private float smokeTimer;

	private Vector3 centerTopVec;

	internal override void create(Building pBuilding)
	{
		base.create(pBuilding);
		Sprite tDefaultSprite = building.asset.building_sprites.animation_data[0].main[0];
		centerTopVec = default(Vector3);
		centerTopVec.x = building.current_tile.pos.x;
		centerTopVec.y = (float)building.current_tile.pos.y + tDefaultSprite.rect.height * building.asset.scale_base.y;
	}

	public override void update(float pElapsed)
	{
		if (building.asset.smoke && !building.isUnderConstruction())
		{
			if (smokeTimer > 0f)
			{
				smokeTimer -= Time.deltaTime;
				return;
			}
			smokeTimer = building.asset.smoke_interval;
			World.world.particles_smoke.spawn(centerTopVec.x, centerTopVec.y, pRemoveCooldown: true);
		}
	}
}
