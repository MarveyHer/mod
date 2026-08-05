public static class CursedSacrifice
{
	private const int SACRIFICE_COUNT = 314;

	private const int MAX_MESSAGES = 9;

	private static int _last_message_index = -1;

	private static int _current_sacrifice_count = 0;

	private static double _cursed_world_timestamp = 0.0;

	private static bool _latest_sacrificed_was_egg;

	public static void checkGoodForSacrifice(Actor pActor)
	{
		bool tGoodForForbiddenKnowledge = false;
		if (pActor.hasSubspecies())
		{
			bool tHasControlledStatus = pActor.hasStatus("magnetized") || pActor.hasStatus("strange_urge") || pActor.hasStatus("possessed");
			if (pActor.hasSubspeciesTrait("pure") && tHasControlledStatus)
			{
				tGoodForForbiddenKnowledge = true;
			}
		}
		if (tGoodForForbiddenKnowledge)
		{
			if (pActor.asset.id == "elf")
			{
				World.world.game_stats.data.elvesSacrificed++;
				_latest_sacrificed_was_egg = pActor.isEgg();
				spawnVoidElves();
			}
			World.world.game_stats.data.creaturesSacrificed++;
			countSacrifice();
		}
	}

	public static void spawnVoidElves()
	{
		Subspecies tTargetSubspecies = getVoidElvesSubspecies();
		if (tTargetSubspecies != null)
		{
			TileZone tVisibleZone = World.world.zone_camera.getVisibleZones().GetRandom();
			if (tVisibleZone != null)
			{
				WorldTile tRandomTile = tVisibleZone.getRandomTile();
				World.world.units.spawnNewUnit("elf", tRandomTile, pSpawnSound: false, pMiracleSpawn: true, 6f, tTargetSubspecies, pGiveOwnerlessItems: true, pAdultAge: true);
			}
		}
	}

	private static Subspecies getVoidElvesSubspecies()
	{
		using ListPool<Subspecies> tVoidElvesPot = new ListPool<Subspecies>();
		ActorAsset tAssetElf = AssetManager.actor_library.get("elf");
		foreach (Subspecies tSubspecies in World.world.subspecies)
		{
			if (tSubspecies.getActorAsset() == tAssetElf && tSubspecies.hasTrait("mutation_skin_void"))
			{
				tVoidElvesPot.Add(tSubspecies);
			}
		}
		Subspecies tTargetSubspecies;
		if (tVoidElvesPot.Count == 0)
		{
			WorldTile tRandomGroundTile = World.world.islands_calculator.tryGetRandomGround();
			if (tRandomGroundTile == null)
			{
				return null;
			}
			Subspecies subspecies = World.world.subspecies.newSpecies(tAssetElf, tRandomGroundTile);
			subspecies.addTrait("mutation_skin_void");
			subspecies.addTrait("gift_of_void");
			subspecies.addTrait("reproduction_soulborne");
			subspecies.addTrait("big_stomach");
			subspecies.addTrait("voracious");
			subspecies.addTrait("genetic_mirror");
			subspecies.addTrait("genetic_psychosis");
			subspecies.addTrait("enhanced_strength");
			subspecies.addTrait("cold_resistance");
			subspecies.addTrait("heat_resistance");
			subspecies.addTrait("adaptation_corruption");
			subspecies.addTrait("adaptation_desert");
			subspecies.addTrait("hovering");
			subspecies.removeTrait("pure");
			subspecies.removeTrait("prefrontal_cortex");
			subspecies.removeTrait("advanced_hippocampus");
			subspecies.removeTrait("amygdala");
			subspecies.removeTrait("wernicke_area");
			subspecies.addBirthTrait("desire_harp");
			subspecies.addBirthTrait("evil");
			subspecies.data.name = "Elfus Voidus";
			tTargetSubspecies = subspecies;
		}
		else
		{
			tTargetSubspecies = tVoidElvesPot.GetRandom();
		}
		return tTargetSubspecies;
	}

	public static void countAllSacrificesDebug()
	{
		for (int i = 0; i < 314; i++)
		{
			countSacrifice();
		}
	}

	private static void countSacrifice()
	{
		if (_current_sacrifice_count == 314)
		{
			return;
		}
		_current_sacrifice_count++;
		int tNewMessageIndex = (int)(getCurseProgressRatio() * 9f);
		if (tNewMessageIndex > _last_message_index)
		{
			_last_message_index = tNewMessageIndex;
			string tColorHex = "#F3961F";
			if (_last_message_index > 6)
			{
				tColorHex = "#FF637D";
			}
			if (_last_message_index == 9)
			{
				tColorHex = "#E060CD";
			}
			WorldTip.showNow("world_curse_message_" + _last_message_index, pTranslate: true, "top", 3f, tColorHex);
			_ = _last_message_index;
			_ = 4;
			World.world.startShake(0.3f + (float)_last_message_index * 0.1f, 0.01f, 0.23f + (float)_last_message_index * 0.02f, pShakeX: true);
		}
	}

	public static float getCurseProgressRatio()
	{
		return (float)_current_sacrifice_count / 314f;
	}

	public static float getCurseProgressRatioForBlackhole()
	{
		if (AchievementLibrary.isUnlocked("achievementCursedWorld"))
		{
			return 1f;
		}
		return (float)_current_sacrifice_count / 314f;
	}

	public static void reset()
	{
		_current_sacrifice_count = 0;
		_last_message_index = 0;
		_latest_sacrificed_was_egg = false;
	}

	private static int getCurrentSacrificeCount()
	{
		return _current_sacrifice_count;
	}

	public static void loadAlreadyCursedState()
	{
		_current_sacrifice_count = 314;
	}

	public static bool isWorldReadyForCURSE()
	{
		if (AchievementLibrary.isUnlocked("achievementCursedWorld"))
		{
			return true;
		}
		return isAllSacrificesDone();
	}

	public static bool isAllSacrificesDone()
	{
		return getCurrentSacrificeCount() >= 314;
	}

	public static void justCursedWorld()
	{
		if (Config.hasPremium)
		{
			_cursed_world_timestamp = World.world.getCurSessionTime();
			AchievementLibrary.cursed_world.check();
		}
	}

	public static bool justGotCursedWorld()
	{
		return World.world.getRealTimeElapsedSince(_cursed_world_timestamp) < 1f;
	}

	public static bool isLatestWasEgg()
	{
		return _latest_sacrificed_was_egg;
	}
}
