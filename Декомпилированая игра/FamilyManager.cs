using System.Collections.Generic;

public class FamilyManager : MetaSystemManager<Family, FamilyData>
{
	public FamilyManager()
	{
		type_id = "family";
	}

	public Family newFamily(Actor pActor, WorldTile pTile, Actor pActor2)
	{
		World.world.game_stats.data.familiesCreated++;
		World.world.map_stats.familiesCreated++;
		Family tNewFamily = newObject();
		tNewFamily.newFamily(pActor, pActor2, pTile);
		if (pActor.hasFamily())
		{
			tNewFamily.saveOriginFamily1(pActor.family.id);
		}
		pActor.setFamily(tNewFamily);
		if (pActor2 != null)
		{
			if (pActor2.hasFamily())
			{
				tNewFamily.saveOriginFamily2(pActor2.family.id);
			}
			pActor2.setFamily(tNewFamily);
		}
		return tNewFamily;
	}

	public override void removeObject(Family pObject)
	{
		World.world.game_stats.data.familiesDestroyed++;
		World.world.map_stats.familiesDestroyed++;
		base.removeObject(pObject);
	}

	public Family getNearbyFamily(ActorAsset pUnitAsset, WorldTile pTile)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 4, pUnitAsset.family_spawn_radius, pRandom: true))
		{
			if (tActor.isAlive() && tActor.hasFamily() && !tActor.family.isFull() && tActor.family.isSameSpecies(pUnitAsset.id) && tActor.current_tile.isSameIsland(pTile))
			{
				return tActor.family;
			}
		}
		return null;
	}

	protected override void updateDirtyUnits()
	{
		List<Actor> tActorList = World.world.units.units_only_alive;
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tUnit = tActorList[i];
			Family tFamily = tUnit.family;
			if (tFamily != null && tFamily.isDirtyUnits())
			{
				tFamily.listUnit(tUnit);
			}
		}
	}
}
