public static class NameGeneratorChecks
{
	public static bool hasLatinKing(Actor pActor)
	{
		if (!hasCivKingdom(pActor))
		{
			return false;
		}
		if (!pActor.kingdom.hasKing())
		{
			return false;
		}
		if (!Toolbox.isFirstLatin(pActor.kingdom.king.getName()))
		{
			return false;
		}
		return true;
	}

	public static bool hasEnemyLatinKing(Actor pActor)
	{
		if (!hasCivKingdom(pActor))
		{
			return false;
		}
		if (!pActor.kingdom.hasEnemies())
		{
			return false;
		}
		using ListPool<Kingdom> tEnemyKingdoms = pActor.kingdom.getEnemiesKingdoms();
		foreach (ref Kingdom item in tEnemyKingdoms)
		{
			Kingdom tKingdom = item;
			if (tKingdom.hasKing() && Toolbox.isFirstLatin(tKingdom.king.getName()))
			{
				return true;
			}
		}
		return false;
	}

	public static bool hasCivKingdom(Actor pActor)
	{
		if (pActor == null)
		{
			return false;
		}
		if (pActor.kingdom == null)
		{
			return false;
		}
		if (!pActor.isKingdomCiv())
		{
			return false;
		}
		return true;
	}

	public static bool hasLatinKingdom(Actor pActor)
	{
		if (!hasCivKingdom(pActor))
		{
			return false;
		}
		if (!Toolbox.isFirstLatin(pActor.kingdom.name))
		{
			return false;
		}
		return true;
	}

	public static bool hasEnemyLatinKingdom(Actor pActor)
	{
		if (!hasCivKingdom(pActor))
		{
			return false;
		}
		if (!pActor.kingdom.hasEnemies())
		{
			return false;
		}
		using ListPool<Kingdom> tEnemyKingdoms = pActor.kingdom.getEnemiesKingdoms();
		foreach (ref Kingdom item in tEnemyKingdoms)
		{
			if (Toolbox.isFirstLatin(item.name))
			{
				return true;
			}
		}
		return false;
	}

	public static bool hasLatinCity(Actor pActor)
	{
		if (pActor == null)
		{
			return false;
		}
		if (!pActor.hasCity())
		{
			return false;
		}
		if (!Toolbox.isFirstLatin(pActor.city.name))
		{
			return false;
		}
		return true;
	}

	public static bool hasLatinCulture(Actor pActor)
	{
		if (pActor == null)
		{
			return false;
		}
		if (!pActor.hasCulture())
		{
			return false;
		}
		if (!Toolbox.isFirstLatin(pActor.culture.name))
		{
			return false;
		}
		return true;
	}
}
