using System;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine;

[Serializable]
[ObfuscateLiterals]
public class AchievementLibrary : AssetLibrary<Achievement>
{
	private const int ZOO_SPECIES_NEED = 33;

	private const int SPECIES_EXPLORER_ASSETS = 52;

	private const int TRAITS_EXPLORER_AMOUNT_FIRST = 40;

	private const int TRAITS_EXPLORER_AMOUNT_SECOND = 60;

	private const int TRAITS_EXPLORER_AMOUNT_THIRD = 90;

	private const int SUBSPECIES_TRAITS_EXPLORER_AMOUNT = 190;

	private const int CULTURE_TRAITS_EXPLORER_AMOUNT = 70;

	private const int LANGUAGE_TRAITS_EXPLORER_AMOUNT = 20;

	private const int CLAN_TRAITS_EXPLORER_AMOUNT = 25;

	private const int RELIGION_TRAITS_EXPLORER_AMOUNT = 33;

	private const int EQUIPMENT_EXPLORER_AMOUNT = 80;

	private const int GENES_EXPLORER_AMOUNT = 35;

	private const int PLOTS_EXPLORER_AMOUNT = 20;

	private const string ONOMASTICS_NAME_FOR_ACHIEVEMENT = "Mako Mako";

	private const int NOT_JUST_A_CULT_UNITS = 7777;

	private const int MULTIPLY_SPOKEN_UNITS = 5555;

	public static Achievement lava_strike;

	public static Achievement baby_tornado;

	public static Achievement rain_tornado;

	public static Achievement many_bombs;

	public static Achievement megapolis;

	public static Achievement wilhelm_scream;

	public static Achievement burger;

	public static Achievement mayday;

	public static Achievement destroy_worldbox;

	public static Achievement custom_world;

	public static Achievement four_race_cities;

	public static Achievement piranha_land;

	public static Achievement print_heart;

	public static Achievement sacrifice;

	public static Achievement final_resolution;

	public static Achievement tnt_and_heat;

	public static Achievement god_finger_lightning;

	public static Achievement ten_thousands_creatures;

	public static Achievement ant_world;

	public static Achievement traits_explorer_40;

	public static Achievement traits_explorer_60;

	public static Achievement traits_explorer_90;

	public static Achievement trait_explorer_subspecies;

	public static Achievement trait_explorer_culture;

	public static Achievement trait_explorer_language;

	public static Achievement trait_explorer_clan;

	public static Achievement trait_explorer_religion;

	public static Achievement equipment_explorer;

	public static Achievement genes_explorer;

	public static Achievement creatures_explorer;

	public static Achievement plots_explorer;

	public static Achievement the_builder;

	public static Achievement the_dwarf;

	public static Achievement the_creator;

	public static Achievement the_light;

	public static Achievement the_sky;

	public static Achievement the_land;

	public static Achievement the_sun;

	public static Achievement the_moon;

	public static Achievement the_living;

	public static Achievement the_rest_day;

	public static Achievement life_is_a_sim;

	public static Achievement gen_5_worlds;

	public static Achievement gen_50_worlds;

	public static Achievement gen_100_worlds;

	public static Achievement the_corrupted_trees;

	public static Achievement the_hell;

	public static Achievement lets_not;

	public static Achievement world_war;

	public static Achievement planet_of_apes;

	public static Achievement super_mushroom;

	public static Achievement the_princess;

	public static Achievement oh_my_crab;

	public static Achievement tornado;

	public static Achievement god_mode;

	public static Achievement greg;

	public static Achievement ninja_turtle;

	public static Achievement great_plague;

	public static Achievement no_hope_only_mush;

	public static Achievement touch_the_grass;

	public static Achievement the_broken;

	public static Achievement the_king;

	public static Achievement the_demon;

	public static Achievement the_accomplished;

	public static Achievement cursed_world;

	public static Achievement boats_disposal;

	public static Achievement engineered_evolution;

	public static Achievement simple_stupid_genetics;

	public static Achievement fast_living;

	public static Achievement long_living;

	public static Achievement ancient_war_of_geometry_and_evil;

	public static Achievement cant_be_too_much;

	public static Achievement zoo;

	public static Achievement mindless_husk;

	public static Achievement master_weaver;

	public static Achievement not_just_a_cult;

	public static Achievement succession;

	public static Achievement multiply_spoken;

	public static Achievement child_named_toto;

	public static Achievement flick_it;

	public static Achievement segregator;

	public static Achievement swarm;

	public static Achievement eternal_chaos;

	public static Achievement minefield;

	public static Achievement godly_smithing;

	public static Achievement master_of_combat;

	public static Achievement clannibals;

	public static Achievement social_network;

	public static Achievement not_on_my_watch;

	public static Achievement may_i_interrupt;

	public static Achievement watch_your_mouth;

	public static Achievement smelly_city;

	public static Achievement ball_to_ball;

	public static Achievement back_to_beta_testing;

	public static Achievement clone_wars;

	public static Achievement sword_with_shotgun;

	public static Achievement tldr;

	public const float LIFE_IS_SIM_HOURS = 24f;

	public override void init()
	{
		base.init();
		Debug.Log("Init Achievements");
		the_accomplished = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_20",
			id = "achievementTheAccomplished",
			action = checkTheAccomplished,
			icon = "ui/Icons/achievements/achievements_theAccomplished",
			group = "creatures"
		});
		the_king = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_17",
			id = "achievementTheKing",
			action = checkTheKing,
			icon = "ui/Icons/achievements/achievements_theKing",
			group = "creatures"
		});
		the_demon = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_21",
			id = "achievementTheDemon",
			action = checkTheDemon,
			icon = "ui/Icons/achievements/achievements_theDemon",
			group = "creatures"
		});
		the_broken = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_22",
			id = "achievementTheBroken",
			action = checkTheBroken,
			icon = "ui/Icons/achievements/achievements_theBroken",
			group = "creatures"
		});
		touch_the_grass = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_10",
			id = "achievementTouchTheGrass",
			icon = "ui/Icons/achievements/achievements_touchTheGrass",
			group = "nature",
			hidden = true
		});
		gen_5_worlds = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_0",
			id = "achievementGen5Worlds",
			action = checkMapCreations5,
			icon = "ui/Icons/achievements/achievements_generate5",
			group = "worlds"
		});
		gen_50_worlds = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_1",
			id = "achievementGen50Worlds",
			action = checkMapCreations50,
			icon = "ui/Icons/achievements/achievements_generate50",
			group = "worlds"
		});
		gen_100_worlds = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_2",
			id = "achievementGen100Worlds",
			action = checkMapCreations100,
			icon = "ui/Icons/achievements/achievements_generate100",
			group = "worlds"
		});
		life_is_a_sim = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_3",
			id = "achievementLifeIsASim",
			hidden = true,
			action = checkLifeIsASim,
			icon = "ui/Icons/achievements/achievements_lifeissimulation",
			group = "miscellaneous"
		});
		the_corrupted_trees = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_31",
			id = "achievementTheCorruptedTrees",
			icon = "ui/Icons/achievements/achievements_corruptedbiome",
			group = "exploration"
		});
		the_hell = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_0",
			id = "achievementTheHell",
			hidden = true,
			action = checkTheHell,
			icon = "ui/Icons/achievements/achievements_infernalbiome",
			group = "destruction"
		});
		lets_not = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_1",
			id = "achievementLetsNot",
			hidden = true,
			action = checkLetsNot,
			icon = "ui/Icons/achievements/achievements_wastelandbiome",
			group = "destruction"
		});
		world_war = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_2",
			id = "achievementWorldWar",
			hidden = true,
			action = checkWorldWar,
			icon = "ui/Icons/achievements/achievements_worldwar",
			group = "civilizations"
		});
		planet_of_apes = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_23",
			id = "achievementPlanetOfApes",
			hidden = true,
			action = checkPlanetOfTheApes,
			icon = "ui/Icons/achievements/achievements_planetofapes",
			group = "creatures"
		});
		super_mushroom = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_24",
			id = "achievementSuperMushroom",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_supermush",
			group = "creatures"
		});
		the_princess = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_25",
			id = "achievementThePrincess",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_princess",
			group = "creatures"
		});
		tornado = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_11",
			id = "achievementTORNADO",
			locale_key = "achievement_tornado",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_cursedtornado",
			group = "nature"
		});
		god_mode = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_13",
			id = "achievementGodMode",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_godmode",
			group = "miscellaneous"
		});
		greg = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_26",
			id = "achievementGreg",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_greg",
			group = "forbidden"
		});
		ninja_turtle = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_27",
			id = "achievementNinjaTurtle",
			icon = "ui/Icons/achievements/achievements_ninjaturtle",
			action = delegate(object pActor)
			{
				Actor actor = pActor as Actor;
				if (!actor.asset.flag_turtle)
				{
					return false;
				}
				return actor.level >= 10;
			},
			group = "creatures"
		});
		great_plague = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_3",
			id = "achievementGreatPlague",
			hidden = true,
			action = checkGreatPlague,
			icon = "ui/Icons/achievements/achievements_plagueworld",
			group = "experiments"
		});
		lava_strike = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "2_4",
			id = "achievementLavaStrike",
			hidden = true,
			icon = "ui/Icons/actor_traits/iconLightning",
			group = "destruction"
		});
		baby_tornado = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAw",
			steam_id = "2_12",
			id = "achievementBabyTornado",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_babytornado",
			group = "nature"
		});
		rain_tornado = add(new Achievement
		{
			id = "achievementRainTornado",
			play_store_id = "CgkIia6M98wfEAIQAw",
			steam_id = "2_21",
			icon = "ui/Icons/achievements/achievements_raintornado",
			group = "nature",
			hidden = true,
			action = checkRainTornado
		});
		ten_thousands_creatures = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQBA",
			steam_id = "1_4",
			id = "achievement10000Creatures",
			action = check10000Creatures,
			icon = "ui/Icons/achievements/achievements_1000Creatures",
			group = "creation"
		});
		many_bombs = add(new Achievement
		{
			play_store_id = "",
			steam_id = "2_5",
			id = "achievementManyBombs",
			action = checkManyBombs,
			icon = "ui/Icons/iconBomb",
			group = "destruction"
		});
		megapolis = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQBg",
			steam_id = "1_18",
			hidden = true,
			id = "achievementMegapolis",
			action = checkMegapolis,
			icon = "ui/Icons/achievements/achievements_megapolis",
			group = "civilizations"
		});
		wilhelm_scream = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQBw",
			steam_id = "2_14",
			id = "achievementMakeWilhelmScream",
			hidden = true,
			icon = "ui/Icons/iconHumans",
			group = "exploration"
		});
		burger = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_15",
			id = "achievementBurger",
			hidden = true,
			icon = "ui/Icons/iconBurger",
			group = "exploration"
		});
		burger = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_16",
			id = "achievementPie",
			hidden = true,
			icon = "ui/Icons/iconResPie",
			group = "exploration"
		});
		mayday = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_6",
			id = "achievementMayday",
			hidden = true,
			icon = "ui/Icons/iconSanta",
			group = "destruction"
		});
		destroy_worldbox = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_17",
			id = "achievementDestroyWorldBox",
			hidden = true,
			icon = "ui/Icons/iconBrowse2",
			group = "exploration"
		});
		custom_world = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "1_5",
			id = "achievementCustomWorld",
			hidden = true,
			icon = "ui/Icons/iconTileSoil",
			group = "creation"
		});
		four_race_cities = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "1_19",
			id = "achievement4RaceCities",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_4Races",
			action = check4RaceCities,
			group = "civilizations"
		});
		piranha_land = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "1_28",
			id = "achievementPiranhaLand",
			hidden = true,
			icon = "ui/Icons/iconPiranha",
			action = checkPiranhaLand,
			group = "experiments"
		});
		print_heart = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "1_6",
			id = "achievementPrintHeart",
			hidden = true,
			action = checkPrintHeart,
			icon = "ui/Icons/achievements/achievements_printHeart",
			group = "creation"
		});
		sacrifice = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "1_29",
			id = "achievementSacrifice",
			hidden = true,
			icon = "ui/Icons/iconSheep",
			group = "experiments"
		});
		ant_world = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "1_30",
			id = "achievementAntWorld",
			hidden = true,
			icon = "ui/Icons/iconAntBlack",
			action = checkAntWorld,
			group = "creatures"
		});
		final_resolution = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_7",
			id = "achievementFinalResolution",
			hidden = true,
			icon = "ui/Icons/iconGreygoo",
			group = "destruction"
		});
		tnt_and_heat = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_8",
			id = "achievementTntAndHeat",
			hidden = true,
			icon = "ui/Icons/iconTnt",
			group = "destruction"
		});
		god_finger_lightning = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_9",
			id = "achievementGodFingerLightning",
			hidden = true,
			icon = "ui/Icons/iconGodFinger",
			group = "destruction"
		});
		traits_explorer_40 = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_18",
			id = "achievementTraitsExplorer40",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traitexplorer1",
			group = "collection",
			action = (object _) => checkTraitsExplorer(40)
		});
		traits_explorer_60 = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_19",
			id = "achievementTraitsExplorer60",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traitexplorer2",
			group = "collection",
			action = (object _) => checkTraitsExplorer(60)
		});
		traits_explorer_90 = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "2_20",
			id = "achievementTraitsExplorer90",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traitexplorer3",
			group = "collection",
			action = (object _) => checkTraitsExplorer(90)
		});
		trait_explorer_subspecies = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementTraitExplorerSubspecies",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traits_explorer_subspecies",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(190, AssetManager.subspecies_traits)
		});
		trait_explorer_culture = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementTraitExplorerCulture",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traits_explorer_culture",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(70, AssetManager.culture_traits)
		});
		trait_explorer_language = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementTraitExplorerLanguage",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traits_explorer_language",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(20, AssetManager.language_traits)
		});
		trait_explorer_clan = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementTraitExplorerClan",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traits_explorer_clan",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(25, AssetManager.clan_traits)
		});
		trait_explorer_religion = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementTraitExplorerReligion",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_traits_explorer_religion",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(33, AssetManager.religion_traits)
		});
		equipment_explorer = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementEquipmentExplorer",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_equipment_explorer",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(80, AssetManager.items)
		});
		genes_explorer = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementGenesExplorer",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_genes_explorer",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(35, AssetManager.gene_library)
		});
		creatures_explorer = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementCreaturesExplorer",
			hidden = true,
			icon = "ui/Icons/achievements/achievements_creatures_explorer",
			group = "collection",
			action = checkCreaturesExplorer
		});
		plots_explorer = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementPlotsExplorer",
			hidden = false,
			icon = "ui/Icons/achievements/achievements_plots_explorer",
			group = "collection",
			action = (object _) => checkUnlockAugmentations(20, AssetManager.plots_library)
		});
		cursed_world = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementCursedWorld",
			hidden = true,
			icon = "ui/Icons/achievements/achievement_cursed_world",
			action = checkCursedWorld,
			group = "forbidden"
		});
		boats_disposal = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementBoatsDisposal",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_boats_disposal",
			group = "destruction",
			action = checkBoatDisposal
		});
		engineered_evolution = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementEngineeredEvolution",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_engineered_evolution",
			group = "experiments"
		});
		simple_stupid_genetics = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementSimpleStupidGenetics",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_simple_stupid_genetics",
			group = "experiments",
			action = checkSimpleStupidGenetics
		});
		fast_living = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementFastLiving",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_fast_living",
			group = "experiments",
			action = checkFastLiving
		});
		long_living = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementLongLiving",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_long_living",
			group = "experiments",
			action = checkLongLiving
		});
		ancient_war_of_geometry_and_evil = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementAncientWarOfGeometryAndEvil",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_ancient_war_of_geometry_and_evil",
			group = "civilizations",
			action = checkAncientWarOfGeometryAndEvil
		});
		cant_be_too_much = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementCantBeTooMuch",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_cant_be_too_much",
			group = "creation",
			action = checkCantBeTooMuch
		});
		zoo = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementZoo",
			hidden = true,
			icon = "ui/Icons/achievements/achievement_zoo",
			group = "civilizations",
			action = checkZoo
		});
		mindless_husk = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementMindlessHusk",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_mindless_husk",
			group = "experiments",
			action = checkMindlessHusk
		});
		master_weaver = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementMasterWeaver",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_master_weaver",
			group = "experiments",
			action = checkMasterWeaver
		});
		not_just_a_cult = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementNotJustACult",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_not_just_a_cult",
			group = "civilizations",
			action = checkNotJustACult
		});
		succession = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementSuccession",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_succession",
			group = "experiments"
		});
		multiply_spoken = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementMultiplySpoken",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_multiply_spoken",
			group = "civilizations",
			action = checkMultiplySpoken
		});
		child_named_toto = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementChildNamedToto",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_child_named_toto",
			group = "experiments",
			action = checkChildNamedMakoMako
		});
		flick_it = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementFlickIt",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_flick_it",
			group = "experiments"
		});
		segregator = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementSegregator",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_segregator",
			group = "creation",
			action = checkSegregator
		});
		eternal_chaos = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementEternalChaos",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_eternal_chaos",
			group = "nature",
			action = checkEternalChaos
		});
		minefield = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementMinefield",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_minefield",
			group = "nature",
			action = checkMinefield
		});
		godly_smithing = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementGodlySmithing",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_godly_smithing",
			group = "experiments"
		});
		master_of_combat = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementMasterOfCombat",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_master_of_combat",
			group = "creatures",
			action = checkMasterOfCombat
		});
		clannibals = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementClannibals",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_clannibals",
			group = "creatures",
			action = checkClannibals
		});
		social_network = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementSocialNetwork",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_social_network",
			group = "miscellaneous"
		});
		watch_your_mouth = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementWatchYourMouth",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_watch_your_mouth",
			group = "creatures",
			action = checkWatchYourMouth
		});
		clone_wars = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementCloneWars",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_clone_wars",
			group = "experiments",
			action = checkCloneWars
		});
		smelly_city = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementSmellyCity",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_smelly_city",
			group = "civilizations",
			action = checkSmellyCity
		});
		tldr = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementTLDR",
			locale_key = "achievement_tldr",
			hidden = true,
			icon = "ui/Icons/achievements/achievement_tldr",
			group = "exploration"
		});
		not_on_my_watch = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementNotOnMyWatch",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_not_on_my_watch",
			group = "civilizations",
			action = checkNotOnMyWatch
		});
		may_i_interrupt = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementMayIInterrupt",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_may_i_interrupt",
			group = "civilizations",
			action = checkMayIInterrupt
		});
		ball_to_ball = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementBallToBall",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_ball_to_ball",
			group = "destruction",
			action = checkBallToBall
		});
		sword_with_shotgun = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementSwordWithShotgun",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_sword_with_shotgun",
			group = "creatures",
			action = checkSwordWithShotgun
		});
		back_to_beta_testing = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementBackToBetaTesting",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_back_to_beta_testing",
			group = "creatures",
			action = checkBackToBetaTesting
		});
		swarm = add(new Achievement
		{
			play_store_id = "?",
			steam_id = "?",
			id = "achievementSwarm",
			hidden = false,
			icon = "ui/Icons/achievements/achievement_swarm",
			group = "creatures",
			action = checkSwarm
		});
		standaloneAchievements();
	}

	private void standaloneAchievements()
	{
		the_builder = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_7",
			id = "achievementTheBuilder",
			icon = "ui/Icons/achievements/achievements_thebuilder",
			group = "worlds"
		});
		the_dwarf = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_8",
			id = "achievementTheDwarf",
			icon = "ui/Icons/achievements/achievements_thedwarf",
			group = "worlds"
		});
		the_creator = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_9",
			id = "achievementTheCreator",
			icon = "ui/Icons/achievements/achievements_thecreator",
			group = "worlds"
		});
		the_light = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_10",
			id = "achievementTheLight",
			icon = "ui/Icons/achievements/achievements_thelight",
			group = "worlds"
		});
		the_sky = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_11",
			id = "achievementTheSky",
			icon = "ui/Icons/achievements/achievements_thesky",
			group = "worlds"
		});
		the_land = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_12",
			id = "achievementTheLand",
			icon = "ui/Icons/achievements/achievements_theland",
			group = "worlds"
		});
		the_sun = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_13",
			id = "achievementTheSun",
			icon = "ui/Icons/achievements/achievements_thesun",
			group = "worlds"
		});
		the_moon = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_14",
			id = "achievementTheMoon",
			icon = "ui/Icons/achievements/achievements_themoon",
			group = "worlds"
		});
		the_living = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_15",
			id = "achievementTheLiving",
			icon = "ui/Icons/achievements/achievements_theliving",
			group = "worlds"
		});
		the_rest_day = add(new Achievement
		{
			play_store_id = "CgkIia6M98wfEAIQAg",
			steam_id = "1_16",
			id = "achievementTheRestDay",
			icon = "ui/Icons/achievements/achievements_restday",
			group = "worlds"
		});
		WorkshopAchievements.checkAchievements();
	}

	public override void post_init()
	{
		base.post_init();
		foreach (Achievement tAchievement in list)
		{
			Achievement achievement = tAchievement;
			if (achievement.locale_key == null)
			{
				achievement.locale_key = tAchievement.id.Underscore();
			}
			if (tAchievement.getSignal() != null)
			{
				tAchievement.has_signal = true;
			}
		}
	}

	public override void linkAssets()
	{
		base.linkAssets();
		foreach (Achievement tAchievement in list)
		{
			addAsUnlockAssets(tAchievement, AssetManager.traits);
			addAsUnlockAssets(tAchievement, AssetManager.subspecies_traits);
			addAsUnlockAssets(tAchievement, AssetManager.culture_traits);
			addAsUnlockAssets(tAchievement, AssetManager.language_traits);
			addAsUnlockAssets(tAchievement, AssetManager.clan_traits);
			addAsUnlockAssets(tAchievement, AssetManager.religion_traits);
			addAsUnlockAssets(tAchievement, AssetManager.kingdoms_traits);
			addAsUnlockAssets(tAchievement, AssetManager.gene_library);
			addAsUnlockAssets(tAchievement, AssetManager.items);
			addAsUnlockAssets(tAchievement, AssetManager.actor_library);
			addAsUnlockAssets(tAchievement, AssetManager.plots_library);
		}
	}

	private void addAsUnlockAssets(Achievement pAchievement, ILibraryWithUnlockables pLibrary)
	{
		foreach (BaseUnlockableAsset tAsset in pLibrary.elements_list)
		{
			if (tAsset.unlocked_with_achievement && !(tAsset.achievement_id != pAchievement.id))
			{
				if (pAchievement.unlock_assets == null)
				{
					pAchievement.unlock_assets = new List<BaseUnlockableAsset>();
					pAchievement.unlocks_something = true;
				}
				pAchievement.unlock_assets.Add(tAsset);
			}
		}
	}

	public static void unlock(string pID)
	{
		Achievement tAchievement = AssetManager.achievements.get(pID);
		if (tAchievement != null)
		{
			unlock(tAchievement);
		}
	}

	public static void unlock(Achievement pAchievement)
	{
		if (WorldLawLibrary.world_law_cursed_world.isEnabled())
		{
			return;
		}
		SteamAchievements.TriggerAchievement(pAchievement.id);
		if (!isUnlocked(pAchievement))
		{
			if (GameProgress.unlockAchievement(pAchievement.id))
			{
				AchievementPopup.show(pAchievement);
			}
			Analytics.LogEvent("Achievement", "id", pAchievement.id);
			MapBox.aye();
		}
	}

	public static bool isUnlocked(Achievement pAchievement)
	{
		return GameProgress.isAchievementUnlocked(pAchievement.id);
	}

	public static bool isUnlocked(string pID)
	{
		return GameProgress.isAchievementUnlocked(pID);
	}

	private static bool checkTraitsExplorer(int pAmount)
	{
		int tUnlocked = 0;
		foreach (ActorTrait item in AssetManager.traits.list)
		{
			if (item.isAvailable())
			{
				tUnlocked++;
			}
		}
		if (tUnlocked < pAmount)
		{
			return false;
		}
		return true;
	}

	private static bool checkUnlockAugmentations(int pAmount, ILibraryWithUnlockables pLibrary)
	{
		int tCounterUnlocked = 0;
		foreach (BaseUnlockableAsset item in pLibrary.elements_list)
		{
			if (item.isAvailable())
			{
				tCounterUnlocked++;
			}
		}
		if (tCounterUnlocked < pAmount)
		{
			return false;
		}
		return true;
	}

	private static bool checkAntWorld(object pCheckData = null)
	{
		List<Actor> tUnits = World.world.kingdoms_wild.get("ants").units;
		if (tUnits.Count < 40)
		{
			return false;
		}
		int tBlack = 0;
		int tGreen = 0;
		int tRed = 0;
		int tBlue = 0;
		for (int i = 0; i < tUnits.Count; i++)
		{
			switch (tUnits[i].asset.id)
			{
			case "ant_black":
				tBlack++;
				break;
			case "ant_blue":
				tBlue++;
				break;
			case "ant_red":
				tRed++;
				break;
			case "ant_green":
				tGreen++;
				break;
			}
		}
		if (tBlue >= 10 && tGreen >= 10 && tRed >= 10 && tBlack >= 10)
		{
			return true;
		}
		return false;
	}

	private static bool checkCursedWorld(object pCheckData = null)
	{
		return WorldLawLibrary.world_law_cursed_world.isEnabledRaw();
	}

	private static bool checkBoatDisposal(object pCheckData = null)
	{
		if (StatsHelper.getStat("statistics_boats_destroyed_by_magnet") < 10)
		{
			return false;
		}
		return true;
	}

	private static bool check10000Creatures(object pCheckData = null)
	{
		if (World.world.game_stats.data.creaturesCreated >= 10000)
		{
			return true;
		}
		return false;
	}

	private static bool checkManyBombs(object pCheckData = null)
	{
		if (World.world.game_stats.data.bombsDropped >= 1000)
		{
			return true;
		}
		return false;
	}

	private static bool checkMegapolis(object pCheckData = null)
	{
		foreach (City tCity in World.world.cities)
		{
			if (!(tCity.getSpecies() != "human") && tCity.getPopulationPeople() >= 200)
			{
				return true;
			}
		}
		return false;
	}

	private static bool check4RaceCities(object pCheckData = null)
	{
		if (World.world.cities.Count < 4)
		{
			return false;
		}
		bool tHumans = false;
		bool tOrcs = false;
		bool tElves = false;
		bool tDwarves = false;
		foreach (City city in World.world.cities)
		{
			switch (city.getSpecies())
			{
			case "human":
				tHumans = true;
				break;
			case "orc":
				tOrcs = true;
				break;
			case "elf":
				tElves = true;
				break;
			case "dwarf":
				tDwarves = true;
				break;
			}
		}
		if (tHumans && tOrcs && tElves && tDwarves)
		{
			return true;
		}
		return false;
	}

	private static bool checkPiranhaLand(object pCheckData = null)
	{
		Actor tActor = (Actor)pCheckData;
		if (tActor.asset.id != "piranha")
		{
			return false;
		}
		if (!tActor.mustAvoidGround())
		{
			return false;
		}
		if (tActor.current_tile.Type.liquid)
		{
			return false;
		}
		return true;
	}

	private static bool checkPrintHeart(object pCheckData = null)
	{
		if (((GodPower)pCheckData).printers_print == "heart")
		{
			return true;
		}
		return false;
	}

	internal static void checkSteamMapUploads()
	{
		long workshopUploads = World.world.game_stats.data.workshopUploads;
		if (workshopUploads >= 1)
		{
			the_builder.check();
		}
		if (workshopUploads >= 3)
		{
			the_dwarf.check();
		}
		if (workshopUploads >= 5)
		{
			the_creator.check();
		}
	}

	internal static void checkSteamMapDownloads(int pDownloads)
	{
		if (pDownloads >= 1)
		{
			the_light.check();
		}
		if (pDownloads >= 2)
		{
			the_sky.check();
		}
		if (pDownloads >= 3)
		{
			the_land.check();
		}
		if (pDownloads >= 4)
		{
			the_sun.check();
		}
		if (pDownloads >= 5)
		{
			the_moon.check();
		}
		if (pDownloads >= 6)
		{
			the_living.check();
		}
		if (pDownloads >= 7)
		{
			the_rest_day.check();
		}
	}

	private static bool checkLifeIsASim(object pCheckData = null)
	{
		if (Mathf.Ceil(Time.realtimeSinceStartup) / 3600f > 24f)
		{
			return true;
		}
		return false;
	}

	private static bool checkTheDemon(object pCheckData = null)
	{
		if (!SelectedUnit.isSet())
		{
			return false;
		}
		if (SelectedUnit.unit.hasDivineScar())
		{
			return false;
		}
		if (SelectedUnit.unit.asset.id != "demon")
		{
			return false;
		}
		if (SelectedUnit.unit.countTraits() < 10)
		{
			return false;
		}
		return true;
	}

	private static bool checkTheKing(object pCheckData = null)
	{
		if (!SelectedUnit.isSet())
		{
			return false;
		}
		if (SelectedUnit.unit.hasDivineScar())
		{
			return false;
		}
		if (!SelectedUnit.unit.isKing())
		{
			return false;
		}
		if (SelectedUnit.unit.countTraits() < 20)
		{
			return false;
		}
		return true;
	}

	private static bool checkTheAccomplished(object pCheckData = null)
	{
		if (!SelectedUnit.isSet())
		{
			return false;
		}
		if (SelectedUnit.unit.hasDivineScar())
		{
			return false;
		}
		if (!SelectedUnit.unit.hasTrait("veteran"))
		{
			return false;
		}
		if (!SelectedUnit.unit.hasTrait("mageslayer"))
		{
			return false;
		}
		if (!SelectedUnit.unit.hasTrait("dragonslayer"))
		{
			return false;
		}
		if (!SelectedUnit.unit.hasTrait("kingslayer"))
		{
			return false;
		}
		return true;
	}

	private static bool checkTheBroken(object pCheckData = null)
	{
		if (!SelectedUnit.isSet())
		{
			return false;
		}
		if (SelectedUnit.unit.hasDivineScar())
		{
			return false;
		}
		if (!SelectedUnit.unit.hasTrait("crippled"))
		{
			return false;
		}
		if (!SelectedUnit.unit.hasTrait("eyepatch"))
		{
			return false;
		}
		if (!SelectedUnit.unit.hasTrait("skin_burns"))
		{
			return false;
		}
		return true;
	}

	private static bool checkMapCreations100(object pCheckData = null)
	{
		if (World.world.game_stats.data.mapsCreated >= 100)
		{
			return true;
		}
		return false;
	}

	private static bool checkMapCreations50(object pCheckData = null)
	{
		if (World.world.game_stats.data.mapsCreated >= 50)
		{
			return true;
		}
		return false;
	}

	private static bool checkMapCreations5(object pCheckData = null)
	{
		if (World.world.game_stats.data.mapsCreated >= 5)
		{
			return true;
		}
		return false;
	}

	private static bool checkTheHell(object pCheckData = null)
	{
		float tCount = TopTileLibrary.infernal_high.hashset.Count + TopTileLibrary.infernal_low.hashset.Count;
		if (tCount == 0f)
		{
			return false;
		}
		float tTotalLand = World.world.tiles_list.Length;
		if (tCount / tTotalLand < 0.666f)
		{
			return false;
		}
		if (World.world.kingdoms_wild.get("demon").units.Count >= 66)
		{
			return true;
		}
		return false;
	}

	private static bool checkLetsNot(object pCheckData = null)
	{
		float num = TopTileLibrary.wasteland_high.hashset.Count + TopTileLibrary.wasteland_low.hashset.Count;
		float tTotalLand = World.world.tiles_list.Length;
		if (num / tTotalLand >= 0.9f)
		{
			return true;
		}
		return false;
	}

	private static bool checkWorldWar(object pCheckData = null)
	{
		int tKingdomsAtWarCount = 0;
		foreach (Kingdom kingdom in World.world.kingdoms)
		{
			if (kingdom.hasEnemies())
			{
				tKingdomsAtWarCount++;
				if (tKingdomsAtWarCount >= 10)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool checkPlanetOfTheApes(object pCheckData = null)
	{
		float num = TopTileLibrary.wasteland_high.hashset.Count + TopTileLibrary.wasteland_low.hashset.Count;
		float tTotalJungles = TopTileLibrary.jungle_high.hashset.Count + TopTileLibrary.jungle_low.hashset.Count;
		float tTotalLand = World.world.tiles_list.Length;
		float num2 = num / tTotalLand;
		float tPercentJungles = tTotalJungles / tTotalLand;
		float tTotalMonkeyWorld = num2 + tPercentJungles;
		if (World.world.kingdoms_wild.get("monkey").units.Count >= 100 && tTotalMonkeyWorld > 0.8f)
		{
			return true;
		}
		return false;
	}

	private static bool checkGreatPlague(object pCheckData = null)
	{
		if (World.world.map_stats.current_infected_plague < 1000)
		{
			return false;
		}
		return true;
	}

	private static bool checkRainTornado(object pCheckData = null)
	{
		if (World.world.map_stats.world_age_id != "age_tears")
		{
			return false;
		}
		return World.world.stack_effects.get("fx_tornado").getList().Count >= 100;
	}

	private static bool checkSimpleStupidGenetics(object pCheckData = null)
	{
		Subspecies tSubspecies = SelectedMetas.selected_subspecies;
		if (tSubspecies == null)
		{
			return false;
		}
		List<Chromosome> chromosomes = tSubspecies.nucleus.chromosomes;
		GeneAsset tGeneToCompare = null;
		foreach (Chromosome item in chromosomes)
		{
			foreach (GeneAsset tGene in item.genes)
			{
				if (!tGene.is_empty)
				{
					if (tGeneToCompare == null)
					{
						tGeneToCompare = tGene;
					}
					if (tGeneToCompare != tGene)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private static bool checkFastLiving(object pCheckData = null)
	{
		Subspecies tSubspecies = SelectedMetas.selected_subspecies;
		if (tSubspecies == null)
		{
			return false;
		}
		int tFastLivingLifespan = 2;
		float tLifespanBase = tSubspecies.base_stats["lifespan"];
		float tLifespanMale = tSubspecies.base_stats["lifespan"] + tLifespanBase;
		float tLifespanFemale = tSubspecies.base_stats_female["lifespan"] + tLifespanBase;
		if (tLifespanMale <= (float)tFastLivingLifespan && tLifespanFemale <= (float)tFastLivingLifespan)
		{
			return true;
		}
		return false;
	}

	private static bool checkLongLiving(object pCheckData = null)
	{
		Subspecies tSubspecies = SelectedMetas.selected_subspecies;
		if (tSubspecies == null)
		{
			return false;
		}
		int tLongLivingLifespan = 3000;
		float tLifespanBase = tSubspecies.base_stats["lifespan"];
		float tLifespanMale = tSubspecies.base_stats_male["lifespan"] + tLifespanBase;
		float tLifespanFemale = tSubspecies.base_stats_female["lifespan"] + tLifespanBase;
		if (tLifespanMale >= (float)tLongLivingLifespan && tLifespanFemale >= (float)tLongLivingLifespan)
		{
			return true;
		}
		return false;
	}

	private static bool checkAncientWarOfGeometryAndEvil(object pCheckData = null)
	{
		foreach (War tWar in World.world.wars)
		{
			if (tWar.hasEnded())
			{
				continue;
			}
			bool tHasAngleKingdom = false;
			bool tHasDemonKingdom = false;
			foreach (Kingdom attacker in tWar.getAttackers())
			{
				if (attacker.getSpecies() == "angle")
				{
					tHasAngleKingdom = true;
				}
				if (attacker.getSpecies() == "demon")
				{
					tHasDemonKingdom = true;
				}
			}
			foreach (Kingdom defender in tWar.getDefenders())
			{
				if (defender.getSpecies() == "angle")
				{
					tHasAngleKingdom = true;
				}
				if (defender.getSpecies() == "demon")
				{
					tHasDemonKingdom = true;
				}
			}
			if (tHasAngleKingdom && tHasDemonKingdom)
			{
				return true;
			}
		}
		return false;
	}

	private static bool checkCantBeTooMuch(object pCheckData = null)
	{
		int tNeedAmount = 10;
		int tAmount = 0;
		foreach (Building building in World.world.buildings)
		{
			if (!(building.asset.id != "monolith"))
			{
				tAmount++;
			}
		}
		if (tAmount >= tNeedAmount)
		{
			return true;
		}
		return false;
	}

	private static bool checkZoo(object pCheckData = null)
	{
		using ListPool<string> tList = new ListPool<string>();
		using (IEnumerator<City> enumerator = World.world.cities.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				City current = enumerator.Current;
				tList.Clear();
				foreach (Actor unit in current.units)
				{
					string tId = unit.asset.id;
					if (!tList.Contains(tId))
					{
						tList.Add(tId);
					}
				}
				if (tList.Count < 33)
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private static bool checkMindlessHusk(object pCheckData = null)
	{
		Actor tActor = (Actor)pCheckData;
		UtilityBasedDecisionSystem decision_system = DecisionHelper.decision_system;
		int tAmount = decision_system.getCounter();
		DecisionAsset[] tAssets = decision_system.getActions();
		for (int i = 0; i < tAmount; i++)
		{
			DecisionAsset tAsset = tAssets[i];
			if (tActor.isDecisionEnabled(tAsset.decision_index))
			{
				return false;
			}
		}
		return true;
	}

	private static bool checkMasterWeaver(object pCheckData = null)
	{
		Subspecies tSubspecies = SelectedMetas.selected_subspecies;
		if (tSubspecies == null)
		{
			return false;
		}
		if (tSubspecies.getActorAsset().id != "butterfly")
		{
			return false;
		}
		foreach (Chromosome chromosome in tSubspecies.nucleus.chromosomes)
		{
			if (!chromosome.isAllLociSynergy())
			{
				return false;
			}
		}
		return true;
	}

	private static bool checkNotJustACult(object pCheckData = null)
	{
		if (((Religion)pCheckData).countUnits() < 7777)
		{
			return false;
		}
		return true;
	}

	private static bool checkMultiplySpoken(object pCheckData = null)
	{
		if (((Language)pCheckData).countUnits() < 5555)
		{
			return false;
		}
		return true;
	}

	private static bool checkChildNamedMakoMako(object pCheckData = null)
	{
		if ((string)pCheckData != "Mako Mako")
		{
			return false;
		}
		return true;
	}

	private static bool checkSegregator(object pCheckData = null)
	{
		if (World.world.game_stats.data.wallsPlaced < 10000)
		{
			return false;
		}
		return true;
	}

	private static bool checkEternalChaos(object pCheckData = null)
	{
		if (World.world_era.id != "age_chaos")
		{
			return false;
		}
		if ((float)Date.getYearsSince(World.world.map_stats.same_world_age_started_at) < 1000f)
		{
			return false;
		}
		return true;
	}

	private static bool checkMinefield(object pCheckData = null)
	{
		if (!WorldLawLibrary.world_law_exploding_mushrooms.isEnabled())
		{
			return false;
		}
		if ((float)Date.getYearsSince(World.world.map_stats.exploding_mushrooms_enabled_at) < 1000f)
		{
			return false;
		}
		return true;
	}

	private static bool checkWatchYourMouth(object pCheckData = null)
	{
		Actor tActor = (Actor)pCheckData;
		if (!tActor.isBaby())
		{
			return false;
		}
		if (!tActor.hasStatus("swearing"))
		{
			return false;
		}
		return true;
	}

	private static bool checkCloneWars(object pCheckData = null)
	{
		var (tMain, tTarget) = ((Actor, Actor))pCheckData;
		if (!tMain.isAlive() || !tTarget.isAlive())
		{
			return false;
		}
		if (!tMain.isSameClones(tTarget) && !tMain.isClonedFrom(tTarget) && !tTarget.isClonedFrom(tMain))
		{
			return false;
		}
		return true;
	}

	private static bool checkCreaturesExplorer(object pCheckData = null)
	{
		int tCountTotal = 0;
		int tCountExplored = 0;
		foreach (ActorAsset tAsset in AssetManager.actor_library.list)
		{
			if (tAsset.needs_to_be_explored && !tAsset.isTemplateAsset())
			{
				tCountTotal++;
				if (tAsset.isAvailable())
				{
					tCountExplored++;
				}
			}
		}
		if (tCountExplored < 52)
		{
			return false;
		}
		return true;
	}

	private static bool checkMasterOfCombat(object pCheckData = null)
	{
		Actor tActor = (Actor)pCheckData;
		if (tActor == null)
		{
			return false;
		}
		if (!tActor.isAlive())
		{
			return false;
		}
		if (tActor.hasDivineScar())
		{
			return false;
		}
		int tCombatTraitsAmount = 0;
		foreach (ActorTrait trait in tActor.getTraits())
		{
			if (trait.in_training_dummy_combat_pot)
			{
				tCombatTraitsAmount++;
			}
		}
		if (tCombatTraitsAmount < 5)
		{
			return false;
		}
		return true;
	}

	private static bool checkClannibals(object pCheckData = null)
	{
		var (tMain, tTargetClan) = ((Actor, Clan))pCheckData;
		if (!tMain.hasClan() || tTargetClan == null)
		{
			return false;
		}
		if (tMain.clan.id != tTargetClan.id)
		{
			return false;
		}
		return true;
	}

	private static bool checkSmellyCity(object pCheckData = null)
	{
		if (((City)pCheckData).getResourcesAmount("fertilizer") < 999)
		{
			return false;
		}
		return true;
	}

	private static bool checkNotOnMyWatch(object pCheckData = null)
	{
		if (!((Actor)pCheckData).hasStatus("being_suspicious"))
		{
			return false;
		}
		return true;
	}

	private static bool checkMayIInterrupt(object pCheckData = null)
	{
		if ((string)pCheckData != "socialize_do_talk")
		{
			return false;
		}
		return true;
	}

	private static bool checkBallToBall(object pCheckData = null)
	{
		if (((Actor)pCheckData).asset.id != "armadillo")
		{
			return false;
		}
		return true;
	}

	private static bool checkSwordWithShotgun(object pCheckData = null)
	{
		Actor tActor = (Actor)pCheckData;
		if (tActor.asset.id != "crystal_sword")
		{
			return false;
		}
		if (!tActor.hasWeapon())
		{
			return false;
		}
		Item tWeapon = tActor.getWeapon();
		if (tWeapon.getAsset().id != "shotgun")
		{
			return false;
		}
		if (tWeapon.hasMod("divine_rune"))
		{
			return false;
		}
		return true;
	}

	private static bool checkBackToBetaTesting(object pCheckData = null)
	{
		Actor tActor = (Actor)pCheckData;
		if (tActor.asset.id != "beetle")
		{
			return false;
		}
		if (tActor.current_tile.Type.biome_id != "biome_singularity")
		{
			return false;
		}
		return true;
	}

	private static bool checkSwarm(object pCheckData = null)
	{
		Subspecies tSubspecies = (Subspecies)pCheckData;
		if (tSubspecies.hasPopulationLimit())
		{
			return false;
		}
		if (!tSubspecies.hasTrait("high_fecundity"))
		{
			return false;
		}
		if (!tSubspecies.hasTrait("reproduction_spores"))
		{
			return false;
		}
		if (!tSubspecies.hasTrait("gestation_short"))
		{
			return false;
		}
		if (!tSubspecies.hasTrait("rapid_aging"))
		{
			return false;
		}
		return true;
	}

	public static void checkCityAchievements(City pCity)
	{
		zoo.checkBySignal();
		four_race_cities.check(pCity);
		smelly_city.check(pCity);
		megapolis.checkBySignal();
	}

	public static void checkUnitAchievements(Actor pActor)
	{
		creatures_explorer.check();
		the_broken.check();
		the_demon.check();
		the_king.check();
		the_accomplished.check();
		watch_your_mouth.check(pActor);
		master_of_combat.check(pActor);
		sword_with_shotgun.check(pActor);
		ninja_turtle.check(pActor);
	}

	public static void checkSubspeciesAchievements(Subspecies pSubspecies)
	{
		creatures_explorer.check();
		swarm.checkBySignal(pSubspecies);
	}

	public static void login()
	{
	}

	public override void editorDiagnosticLocales()
	{
		base.editorDiagnosticLocales();
		foreach (Achievement tAsset in list)
		{
			checkLocale(tAsset, tAsset.getLocaleID());
			checkLocale(tAsset, tAsset.getDescriptionID());
		}
	}

	public static int countUnlocked()
	{
		int tUnlocked = 0;
		foreach (Achievement item in AssetManager.achievements.list)
		{
			if (isUnlocked(item.id))
			{
				tUnlocked++;
			}
		}
		return tUnlocked;
	}

	public static bool isAllUnlocked()
	{
		int num = countUnlocked();
		int tMax = AssetManager.achievements.list.Count;
		return num >= tMax;
	}
}
