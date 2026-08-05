public static class GenerateLLMPrompt
{
	public static string getText(Actor pActor)
	{
		using StringBuilderPool tPool = new StringBuilderPool();
		tPool.AppendLine("World Name: " + World.world.map_stats.name);
		tPool.AppendLine($"World Year: {Date.getCurrentYear()}");
		tPool.AppendLine("World Age: " + World.world.era_manager.getCurrentAge().id);
		tPool.AppendLine($"Deaths: {World.world.map_stats.deaths}, Population: {World.world.map_stats.population}, Mobs: {World.world.map_stats.current_mobs}");
		tPool.AppendLine("God Architector Name: " + World.world.map_stats.player_name);
		tPool.AppendLine();
		tPool.AppendLine($"World has subspecies: {World.world.subspecies.Count}, families: {World.world.families.Count}, languages: {World.world.languages.Count}, religions: {World.world.religions.Count}, items: {World.world.items.Count}, buildings: {World.world.buildings.Count}, cultures: {World.world.cultures.Count}, kingdoms: {World.world.kingdoms.Count}, cities: {World.world.cities.Count}, clans: {World.world.clans.Count}, units: {World.world.units.Count}");
		tPool.AppendLine($"World has islands: {World.world.islands_calculator.countLandIslands()}");
		tPool.AppendLine("Unit Name: " + pActor.name);
		tPool.AppendLine($"Age: {pActor.getAge()}, Species: {pActor.asset.id}, Sex: {pActor.data.sex}, Level: {pActor.data.level}");
		tPool.AppendLine($"Births: {pActor.data.births}, Kills: {pActor.data.kills}, Generation: {pActor.data.generation}");
		tPool.AppendLine("Actor Traits: " + pActor.getTraitsAsLocalizedString());
		if (pActor.hasSubspecies())
		{
			tPool.AppendLine($"Subspecies: {pActor.subspecies.name}, Age: {pActor.subspecies.getAge()}");
			tPool.AppendLine("Subspecies traits: " + pActor.subspecies.getTraitsAsLocalizedString());
		}
		if (pActor.hasKingdom() && pActor.isKingdomCiv())
		{
			tPool.AppendLine("Kingdom: " + pActor.kingdom.name);
			tPool.AppendLine($"Kingdom Age: {pActor.kingdom.getAge()}, Population: {pActor.kingdom.getPopulationPeople()}, Children: {pActor.kingdom.countChildren()}, Warriors: {pActor.kingdom.countTotalWarriors()}");
			if (pActor.kingdom.hasKing())
			{
				tPool.AppendLine($"King: {pActor.kingdom.king.name}, Age: {pActor.kingdom.king.getAge()}");
				tPool.AppendLine($"Births: {pActor.kingdom.king.data.births}, Kills: {pActor.kingdom.king.data.kills}, Level: {pActor.kingdom.king.data.level}");
			}
		}
		int tParentCount = 0;
		foreach (Actor tParent in pActor.getParents())
		{
			tPool.AppendLine($"Parent {++tParentCount}: {tParent.name}, Age: {tParent.getAge()}");
		}
		if (pActor.hasCity())
		{
			tPool.AppendLine("City: " + pActor.city.name);
			tPool.AppendLine($"City Age: {pActor.city.getAge()}, Population: {pActor.city.getPopulationPeople()}, Children: {pActor.city.countPopulationChildren()}, Warriors: {pActor.city.countWarriors()}");
		}
		if (pActor.hasClan())
		{
			tPool.AppendLine($"Bloodline Clan is: {pActor.clan.name}, Members: {pActor.clan.countUnits()}, Age: {pActor.clan.getAge()} years");
			tPool.AppendLine("Clan traits: " + pActor.clan.getTraitsAsLocalizedString());
		}
		if (pActor.hasFamily())
		{
			tPool.AppendLine($"Family: {pActor.family.name}, Members: {pActor.family.countUnits()}, Age: {pActor.family.getAge()} years");
		}
		if (pActor.hasCulture())
		{
			tPool.AppendLine($"Culture: {pActor.culture.name}, Followers: {pActor.culture.countUnits()}, Age: {pActor.culture.getAge()} years");
			tPool.AppendLine("Culture traits: " + pActor.culture.getTraitsAsLocalizedString());
		}
		if (pActor.hasLanguage())
		{
			tPool.AppendLine($"Language: {pActor.language.name}, Users: {pActor.language.countUnits()}, Age: {pActor.language.getAge()} years");
			tPool.AppendLine("Language traits: " + pActor.language.getTraitsAsLocalizedString());
		}
		if (pActor.hasReligion())
		{
			tPool.AppendLine($"Religion: {pActor.religion.name}, Followers: {pActor.religion.countUnits()}, age {pActor.religion.getAge()} years");
			tPool.AppendLine("Religion traits: " + pActor.religion.getTraitsAsLocalizedString());
		}
		if (pActor.hasLover())
		{
			tPool.AppendLine($"Lover: {pActor.lover.name}, {pActor.data.sex}, level: {pActor.data.level}, Age: {pActor.lover.getAge()}, money: {pActor.lover.data.money}, kills: {pActor.lover.data.kills}");
		}
		if (pActor.hasBestFriend())
		{
			tPool.AppendLine($"Best Friend: {pActor.getBestFriend().name}. Age of friendship: {pActor.getBestFriend().getAge()}");
			if (pActor.getBestFriend().hasLover())
			{
				tPool.AppendLine($"Best Friend's Lover: {pActor.getBestFriend().lover.name}. Of age {pActor.getBestFriend().lover.getAge()}");
			}
		}
		if (pActor.hasWeapon())
		{
			tPool.AppendLine($"Weapon: {pActor.getWeapon().getName()}. Rarity: {pActor.getWeapon().getQuality()}. Age is {pActor.getWeapon().getAge()} years");
		}
		tPool.AppendLine($"Happiness: {pActor.getHappiness()}/{pActor.getMaxHappiness()}");
		tPool.AppendLine($"Health: {pActor.data.health}/{pActor.getMaxHealth()}");
		tPool.AppendLine($"Stamina: {pActor.data.stamina}/{pActor.getMaxStamina()}");
		tPool.AppendLine($"Nutrition: {pActor.data.nutrition}/{pActor.getMaxNutrition()}");
		tPool.AppendLine($"Mana: {pActor.data.mana}/{pActor.getMaxMana()}");
		tPool.AppendLine($"Money: {pActor.data.money}");
		tPool.AppendLine();
		tPool.AppendLine("He lives in a fantasy simulated world. Write a story about his life, his thoughts, and his adventures.");
		tPool.AppendLine("Make it as though it were from a classic fantasy tale like Lord of the Rings or a D&D campaign.");
		tPool.AppendLine("Make it epic, dramatic, and full of lore. Infuse it with light and darkness, fun and sadness.");
		tPool.AppendLine("Give it character and heart, and make it unforgettable.");
		tPool.AppendLine("Reply should be in this language: " + LocalizedTextManager.instance.language);
		return tPool.ToString();
	}
}
