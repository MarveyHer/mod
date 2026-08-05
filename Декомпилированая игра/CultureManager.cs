using System.Collections.Generic;

public class CultureManager : MetaSystemManager<Culture, CultureData>
{
	private bool _dirty_kingdoms = true;

	private bool _dirty_cities = true;

	public CultureManager()
	{
		type_id = "culture";
	}

	public Culture newCulture(Actor pFounder, bool pAddDefaultTraits)
	{
		World.world.game_stats.data.culturesCreated++;
		World.world.map_stats.culturesCreated++;
		Culture tNewObject = newObject();
		tNewObject.createCulture(pFounder, pAddDefaultTraits);
		addRandomTraitFromBiomeToCulture(tNewObject, pFounder.current_tile);
		MetaHelper.addRandomTrait(tNewObject, AssetManager.culture_traits);
		return tNewObject;
	}

	public void addRandomTraitFromBiomeToCulture(Culture pCulture, WorldTile pTile)
	{
		pCulture.addRandomTraitFromBiome(pTile, pTile.Type.biome_asset?.spawn_trait_culture, AssetManager.culture_traits);
	}

	public override void removeObject(Culture pObject)
	{
		World.world.game_stats.data.culturesForgotten++;
		World.world.map_stats.culturesForgotten++;
		base.removeObject(pObject);
	}

	protected override void updateDirtyUnits()
	{
		List<Actor> tActorList = World.world.units.units_only_alive;
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tUnit = tActorList[i];
			Culture tCulture = tUnit.culture;
			if (tCulture != null && tCulture.isDirtyUnits())
			{
				tCulture.listUnit(tUnit);
			}
		}
	}

	public void beginChecksKingdoms()
	{
		if (_dirty_kingdoms)
		{
			updateDirtyKingdoms();
		}
		_dirty_kingdoms = false;
	}

	private void updateDirtyKingdoms()
	{
		clearAllKingdomLists();
		foreach (Kingdom tKingdom in World.world.kingdoms)
		{
			if (tKingdom.hasCulture())
			{
				tKingdom.culture.listKingdom(tKingdom);
			}
		}
	}

	private void clearAllKingdomLists()
	{
		using IEnumerator<Culture> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.clearListKingdoms();
		}
	}

	public void beginChecksCities()
	{
		if (_dirty_cities)
		{
			updateDirtyCities();
		}
		_dirty_cities = false;
	}

	private void updateDirtyCities()
	{
		clearAllCitiesListst();
		foreach (City tCity in World.world.cities)
		{
			if (tCity.hasCulture())
			{
				tCity.culture.listCity(tCity);
			}
		}
	}

	private void clearAllCitiesListst()
	{
		using IEnumerator<Culture> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.clearListCities();
		}
	}

	public void setDirtyCities()
	{
		_dirty_cities = true;
	}

	public void setDirtyKingdoms()
	{
		_dirty_kingdoms = true;
	}

	public override bool isLocked()
	{
		if (isUnitsDirty())
		{
			return true;
		}
		if (_dirty_cities)
		{
			return true;
		}
		if (_dirty_kingdoms)
		{
			return true;
		}
		return false;
	}

	public Culture getMainCulture(List<Actor> pUnitList)
	{
		for (int i = 0; i < pUnitList.Count; i++)
		{
			Actor tActor = pUnitList[i];
			if (tActor.hasCulture())
			{
				Culture tMetaObject = tActor.culture;
				countMetaObject(tMetaObject);
			}
		}
		return getMostUsedMetaObject();
	}

	public override void clear()
	{
		base.clear();
	}
}
