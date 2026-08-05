using System.Collections.Generic;

public class ArmyManager : MetaSystemManager<Army, ArmyData>
{
	public ArmyManager()
	{
		type_id = "army";
	}

	public Army newArmy(Actor pActor, City pCity)
	{
		World.world.game_stats.data.armiesCreated++;
		World.world.map_stats.armiesCreated++;
		Army tNewArmy = newObject();
		tNewArmy.createArmy(pActor, pCity);
		pActor.setArmy(tNewArmy);
		pCity.setArmy(tNewArmy);
		return tNewArmy;
	}

	public override void removeObject(Army pObject)
	{
		World.world.game_stats.data.armiesDestroyed++;
		World.world.map_stats.armiesDestroyed++;
		base.removeObject(pObject);
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		using IEnumerator<Army> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.checkCaptainExistence();
		}
	}

	protected override void updateDirtyUnits()
	{
		List<Actor> tActorList = World.world.units.units_only_alive;
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tUnit = tActorList[i];
			Army tArmy = tUnit.army;
			if (tArmy != null && tArmy.isDirtyUnits())
			{
				tArmy.listUnit(tUnit);
			}
		}
	}
}
