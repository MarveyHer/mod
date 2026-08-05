using System.Collections.Generic;

public class LanguageManager : MetaSystemManager<Language, LanguageData>
{
	private bool _dirty_kingdoms = true;

	private bool _dirty_cities = true;

	public LanguageManager()
	{
		type_id = "language";
	}

	public Language newLanguage(Actor pActor, bool pAddDefaultTraits)
	{
		World.world.game_stats.data.languagesCreated++;
		World.world.map_stats.languagesCreated++;
		Language tNewObject = newObject();
		tNewObject.newLanguage(pActor, pAddDefaultTraits);
		MetaHelper.addRandomTrait(tNewObject, AssetManager.language_traits);
		addRandomTraitFromBiomeToLanguage(tNewObject, pActor.current_tile);
		return tNewObject;
	}

	public void addRandomTraitFromBiomeToLanguage(Language pLanguage, WorldTile pTile)
	{
		pLanguage.addRandomTraitFromBiome(pTile, pTile.Type.biome_asset?.spawn_trait_language, AssetManager.language_traits);
	}

	public Language getMainLanguage(List<Actor> pUnitList)
	{
		for (int i = 0; i < pUnitList.Count; i++)
		{
			Actor tActor = pUnitList[i];
			if (tActor.hasLanguage())
			{
				Language tMetaObject = tActor.language;
				countMetaObject(tMetaObject);
			}
		}
		return getMostUsedMetaObject();
	}

	public override void removeObject(Language pObject)
	{
		World.world.game_stats.data.languagesForgotten++;
		World.world.map_stats.languagesForgotten++;
		base.removeObject(pObject);
	}

	protected override void updateDirtyUnits()
	{
		List<Actor> tActorList = World.world.units.units_only_alive;
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tUnit = tActorList[i];
			Language tLanguage = tUnit.language;
			if (tLanguage != null && tLanguage.isDirtyUnits())
			{
				tLanguage.listUnit(tUnit);
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
		clearAllKingdomListst();
		foreach (Kingdom tKingdom in World.world.kingdoms)
		{
			tKingdom.getLanguage()?.listKingdom(tKingdom);
		}
	}

	private void clearAllKingdomListst()
	{
		using IEnumerator<Language> enumerator = GetEnumerator();
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
			if (tCity.hasLanguage())
			{
				tCity.language.listCity(tCity);
			}
		}
	}

	private void clearAllCitiesListst()
	{
		using IEnumerator<Language> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.clearListCities();
		}
	}

	public void setDirtyKingdoms()
	{
		_dirty_kingdoms = true;
	}

	public void setDirtyCities()
	{
		_dirty_cities = true;
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
}
