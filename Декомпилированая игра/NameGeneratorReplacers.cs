using UnityEngine;

public static class NameGeneratorReplacers
{
	public static void replaceKingdom(ref string pName, Kingdom pKingdom)
	{
		if (pName.Contains("$kingdom$"))
		{
			if (pKingdom == null)
			{
				pName = "";
			}
			else
			{
				pName = pName.Replace("$kingdom$", pKingdom.name);
			}
		}
	}

	public static void replaceEnemyKing(ref string pName, Actor pActor)
	{
		using ListPool<Kingdom> tEnemyKingdoms = pActor.kingdom.getEnemiesKingdoms();
		foreach (Kingdom tKingdom in tEnemyKingdoms.LoopRandom())
		{
			if (tKingdom.hasKing() && Toolbox.isFirstLatin(tKingdom.king.getName()))
			{
				pName = pName.Replace("$king$", "King " + tKingdom.king.getName());
				return;
			}
		}
		pName = "";
	}

	public static void replaceOwnKingdom(ref string pName, Actor pActor)
	{
		if (pName.Contains("$kingdom$"))
		{
			if (!pActor.hasKingdom())
			{
				pName = "";
				return;
			}
			Kingdom tKingdom = pActor.kingdom;
			pName = pName.Replace("$kingdom$", tKingdom.name);
		}
	}

	public static void replaceEnemyKingdom(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$kingdom$"))
		{
			return;
		}
		using ListPool<Kingdom> tEnemyKingdoms = pActor.kingdom.getEnemiesKingdoms();
		foreach (Kingdom tKingdom in tEnemyKingdoms.LoopRandom())
		{
			if (Toolbox.isFirstLatin(tKingdom.name))
			{
				pName = pName.Replace("$kingdom$", tKingdom.name);
				return;
			}
		}
		pName = "";
	}

	public static void replaceFavoriteFood(ref string pName, Actor pActor)
	{
		if (pName.Contains("$food$"))
		{
			Kingdom kingdom = pActor.kingdom;
			string tFood;
			if (kingdom != null && kingdom.king?.hasFavoriteFood() == true)
			{
				tFood = pActor.kingdom.king.favorite_food_asset.getTranslatedName();
			}
			else
			{
				City city = pActor.city;
				tFood = ((city != null && city.leader?.hasFavoriteFood() == true) ? pActor.city.leader.favorite_food_asset.getTranslatedName() : ((!pActor.hasFavoriteFood()) ? AssetManager.resources.list.GetRandom().getTranslatedName() : pActor.favorite_food_asset.getTranslatedName()));
			}
			pName = pName.Replace("$food$", tFood);
		}
	}

	public static void replaceOwnName(ref string pName, Actor pActor)
	{
		if (pName.Contains("$unit$"))
		{
			pName = pName.Replace("$unit$", pActor.getName());
		}
	}

	public static void replaceOwnCity(ref string pName, Actor pActor)
	{
		if (pName.Contains("$city$"))
		{
			if (!pActor.hasCity())
			{
				pName = "";
				return;
			}
			City tCity = pActor.city;
			pName = pName.Replace("$city$", tCity.name);
		}
	}

	public static void replaceOwnSubspecies(ref string pName, Actor pActor)
	{
		if (pName.Contains("$subspecies$"))
		{
			if (!pActor.hasSubspecies())
			{
				pName = "";
				return;
			}
			Subspecies tSubspecies = pActor.subspecies;
			pName = pName.Replace("$subspecies$", tSubspecies.name);
		}
	}

	public static void replaceOwnAlliance(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$alliance$"))
		{
			return;
		}
		if (!pActor.hasKingdom())
		{
			pName = "";
			return;
		}
		Kingdom tKingdom = pActor.kingdom;
		if (!tKingdom.hasAlliance())
		{
			pName = "";
			return;
		}
		Alliance tAlliance = tKingdom.getAlliance();
		pName = pName.Replace("$alliance$", tAlliance.name);
	}

	public static void replaceOwnKingClan(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$clan$"))
		{
			return;
		}
		Kingdom tKingdom = pActor.kingdom;
		if (!tKingdom.hasKing())
		{
			pName = "";
			return;
		}
		Actor tKing = tKingdom.king;
		if (!tKing.hasClan())
		{
			pName = "";
		}
		else
		{
			pName = pName.Replace("$clan$", tKing.clan.name);
		}
	}

	public static void replaceOwnLeader(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$leader$"))
		{
			return;
		}
		if (!pActor.hasCity())
		{
			pName = "";
			return;
		}
		City tCity = pActor.city;
		if (!tCity.hasLeader())
		{
			pName = "";
			return;
		}
		Actor tLeader = tCity.leader;
		pName = pName.Replace("$leader$", tLeader.getName());
	}

	public static void replaceFigure(ref string pName, Actor pActor)
	{
		replaceOwnLeader(ref pName, pActor);
		replaceOwnKing(ref pName, pActor);
		replaceOwnKingClan(ref pName, pActor);
	}

	public static void replaceAnyCity(ref string pName, Actor pActor)
	{
		if (pName.Contains("$city_random$"))
		{
			if (!World.world.cities.hasAny())
			{
				pName = "";
				return;
			}
			City tCity = World.world.cities.getRandom();
			pName = pName.Replace("$city_random$", tCity.name);
		}
	}

	public static void replaceAnyKingdom(ref string pName, Actor _)
	{
		if (pName.Contains("$kingdom_random$"))
		{
			if (!World.world.kingdoms.hasAny())
			{
				pName = "";
				return;
			}
			Kingdom tKingdom = World.world.kingdoms.getRandom();
			pName = pName.Replace("$kingdom_random$", tKingdom.name);
		}
	}

	public static void replaceAnyCulture(ref string pName, Actor _)
	{
		if (pName.Contains("$culture_random$"))
		{
			if (!World.world.cultures.hasAny())
			{
				pName = "";
				return;
			}
			Culture tCulture = World.world.cultures.getRandom();
			pName = pName.Replace("$culture_random$", tCulture.name);
		}
	}

	public static void replaceAnyFamily(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$family_random$"))
		{
			return;
		}
		if (!World.world.families.hasAny())
		{
			pName = "";
			return;
		}
		int tTries = World.world.families.Count;
		do
		{
			Family tRandomFamily = World.world.families.getRandom();
			if (tRandomFamily.isSameSpecies(pActor.asset.id))
			{
				Family tFamily = tRandomFamily;
				pName = pName.Replace("$family_random$", tFamily.name);
				return;
			}
		}
		while (tTries-- > 0);
		pName = "";
	}

	public static void replaceAnySubspecies(ref string pName, Actor pActor)
	{
		if (pName.Contains("$random_subspecies$"))
		{
			if (!World.world.subspecies.hasAny())
			{
				pName = "";
				return;
			}
			Subspecies tSubspecies = World.world.subspecies.getRandom();
			pName = pName.Replace("$random_subspecies$", tSubspecies.name);
		}
	}

	public static void replaceAnyClan(ref string pName, Actor pActor)
	{
		if (pName.Contains("$clan_random$"))
		{
			if (!World.world.clans.hasAny())
			{
				pName = "";
				return;
			}
			Clan tClan = World.world.clans.getRandom();
			pName = pName.Replace("$clan_random$", tClan.name);
		}
	}

	public static void replaceAnyKing(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$king_random$"))
		{
			return;
		}
		if (!World.world.kingdoms.hasAny())
		{
			pName = "";
			return;
		}
		int i = 0;
		Kingdom tKingdom = null;
		while (tKingdom == null || !tKingdom.hasKing())
		{
			if (i++ > 10)
			{
				pName = "";
				return;
			}
			tKingdom = World.world.kingdoms.getRandom();
		}
		Actor tKing = tKingdom.king;
		pName = pName.Replace("$king_random$", tKing.getName());
	}

	public static void replaceAnyLeader(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$leader_random$"))
		{
			return;
		}
		if (!World.world.cities.hasAny())
		{
			pName = "";
			return;
		}
		int i = 0;
		City tCity = null;
		while (tCity == null || !tCity.hasLeader())
		{
			if (i++ > 10)
			{
				pName = "";
				return;
			}
			tCity = World.world.cities.getRandom();
		}
		Actor tLeader = tCity.leader;
		pName = pName.Replace("$leader_random$", tLeader.getName());
	}

	public static void replaceOwnKing(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$king$"))
		{
			return;
		}
		if (!pActor.hasKingdom())
		{
			pName = "";
			return;
		}
		Kingdom tKingdom = pActor.kingdom;
		if (!tKingdom.hasKing())
		{
			pName = "";
			return;
		}
		Actor tKing = tKingdom.king;
		pName = pName.Replace("$king$", tKing.getName());
	}

	public static void replaceOwnKingLover(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$king_lover$"))
		{
			return;
		}
		if (!pActor.hasKingdom())
		{
			pName = "";
			return;
		}
		Kingdom tKingdom = pActor.kingdom;
		if (!tKingdom.hasKing())
		{
			pName = "";
			return;
		}
		Actor tKing = tKingdom.king;
		if (!tKing.hasLover())
		{
			pName = "";
			return;
		}
		Actor tLover = tKing.lover;
		pName = pName.Replace("$king$", tKing.getName());
		pName = pName.Replace("$king_lover$", tLover.getName());
	}

	public static void replaceOwnCulture(ref string pName, Actor pActor)
	{
		if (pName.Contains("$culture$"))
		{
			if (!pActor.hasCulture())
			{
				pName = "";
				return;
			}
			Culture tCulture = pActor.culture;
			pName = pName.Replace("$culture$", tCulture.name);
		}
	}

	public static void replaceOwnLanguage(ref string pName, Actor pActor)
	{
		if (pName.Contains("$language$"))
		{
			if (!pActor.hasLanguage())
			{
				pName = "";
				return;
			}
			Language tLanguage = pActor.language;
			pName = pName.Replace("$language$", tLanguage.name);
		}
	}

	public static void replaceOwnReligion(ref string pName, Actor pActor)
	{
		if (pName.Contains("$religion$"))
		{
			if (!pActor.hasReligion())
			{
				pName = "";
				return;
			}
			Religion tReligion = pActor.religion;
			pName = pName.Replace("$religion$", tReligion.name);
		}
	}

	public static void replaceOwnFamily(ref string pName, Actor pActor)
	{
		if (pName.Contains("$family$"))
		{
			if (!pActor.hasFamily())
			{
				pName = "";
				return;
			}
			Family tFamily = pActor.family;
			pName = pName.Replace("$family$", tFamily.name);
		}
	}

	public static void replaceAnyFamilyFounders(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$family_founder_1$") && !pName.Contains("$family_founder_2$"))
		{
			return;
		}
		if (!World.world.families.hasAny())
		{
			pName = "";
			return;
		}
		int tTries = World.world.families.list.Count;
		do
		{
			Family tRandomFamily = World.world.families.getRandom();
			if (tRandomFamily.isSameSpecies(pActor.asset.id) && tRandomFamily.hasFounders())
			{
				Family tFamily = tRandomFamily;
				replaceFamilyFounder1(ref pName, tFamily.units[0]);
				replaceFamilyFounder2(ref pName, tFamily.units[0]);
				return;
			}
		}
		while (tTries-- > 0);
		pName = "";
	}

	public static void replaceOwnFamilyFounders(ref string pName, Actor pActor)
	{
		if (pName.Contains("$family_founder_1$") || pName.Contains("$family_founder_2$"))
		{
			if (!pActor.hasFamily())
			{
				pName = "";
				return;
			}
			replaceFamilyFounder1(ref pName, pActor);
			replaceFamilyFounder2(ref pName, pActor);
		}
	}

	public static void replaceFamilyFounder1(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$family_founder_1$"))
		{
			return;
		}
		if (!pActor.hasFamily())
		{
			pName = "";
			return;
		}
		string tFamilyFounder1 = pActor.family.data.founder_actor_name_1;
		if (string.IsNullOrEmpty(tFamilyFounder1))
		{
			pName = "";
		}
		else
		{
			pName = pName.Replace("$family_founder_1$", tFamilyFounder1);
		}
	}

	public static void replaceFamilyFounder2(ref string pName, Actor pActor)
	{
		if (!pName.Contains("$family_founder_2$"))
		{
			return;
		}
		if (!pActor.hasFamily())
		{
			pName = "";
			return;
		}
		string tFamilyFounder2 = pActor.family.data.founder_actor_name_2;
		if (string.IsNullOrEmpty(tFamilyFounder2))
		{
			pName = "";
		}
		else
		{
			pName = pName.Replace("$family_founder_2$", tFamilyFounder2);
		}
	}

	public static void replaceWorldName(ref string pName, Actor pActor)
	{
		if (pName.Contains("$world_name$"))
		{
			pName = pName.Replace("$world_name$", World.world.map_stats.name);
		}
	}

	public static void replaceArchitectName(ref string pName, Actor pActor)
	{
		if (pName.Contains("$architect_name$"))
		{
			pName = pName.Replace("$architect_name$", World.world.map_stats.player_name);
		}
	}

	public static void replacer_debug(ref string pName)
	{
		pName = pName.Replace("$alliance$", "Pact of Gregs");
		pName = pName.Replace("$food$", "Tea");
		pName = pName.Replace("$family$", "Gregovich");
		pName = pName.Replace("$family_random$", "Urg Zurg");
		pName = pName.Replace("$family_founder_1$", "Greg");
		pName = pName.Replace("$family_founder_2$", "Gregia");
		pName = pName.Replace("$king$", "Gregor");
		pName = pName.Replace("$king_lover$", "Gregoria");
		pName = pName.Replace("$king_random$", "Zurg Gurg");
		pName = pName.Replace("$kingdom$", "Kingdom of Greg");
		pName = pName.Replace("$kingdom_random$", "Brothers of Wargh");
		pName = pName.Replace("$clan$", "Greg Clan");
		pName = pName.Replace("$clan_random$", "Deze Zaz");
		pName = pName.Replace("$leader$", "Gregoryl");
		pName = pName.Replace("$leader_random$", "Orcaryl");
		pName = pName.Replace("$culture$", "Gragian Culture");
		pName = pName.Replace("$culture_random$", "Orkian Kult");
		pName = pName.Replace("$city$", "Gregopolis");
		pName = pName.Replace("$city_random$", "Orcville");
		pName = pName.Replace("$unit$", "Greg the Great");
		pName = pName.Replace("$warrior$", "Greg the Warrior");
		pName = pName.Replace("$language$", "Gregian Language");
		pName = pName.Replace("$religion$", "Gregianity");
		pName = pName.Replace("$subspecies$", "Gregian Sapient");
		pName = pName.Replace("$random_subspecies$", "Weird Dudes");
		pName = pName.Replace("$world_name$", "The Bad Place");
		pName = pName.Replace("$architect_name$", "Your Mom");
		pName = pName.Replace("$item$", "Legendary Greg Axe");
		if (pName.Contains('$'))
		{
			Debug.LogWarning("replacer_debug missing variable " + pName);
		}
	}
}
