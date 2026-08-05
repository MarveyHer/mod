using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPools;

public class DropsLibrary : AssetLibrary<DropAsset>
{
	private const string TEMPLATE_BIOME_SEEDS = "$biome_seeds$";

	private const string TEMPLATE_SPAWN_BUILDING = "$spawn_building$";

	private const string TEMPLATE_SPAWN_MINERAL = "$spawn_mineral$";

	private const string TEMPLATE_SPAWN_CREEP = "$spawn_creep$";

	private static HashSet<TileZone> _paint_zones_hashset = new HashSet<TileZone>();

	public override void init()
	{
		base.init();
		add(new DropAsset
		{
			id = "paint",
			path_texture = "drops/drop_paint",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			action_landed = action_paint,
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "dust_black",
			path_texture = "drops/drop_dust_black",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		DropAsset dropAsset = t;
		dropAsset.action_landed = (DropsAction)Delegate.Combine(dropAsset.action_landed, new DropsAction(action_dust_white));
		DropAsset dropAsset2 = t;
		dropAsset2.action_landed = (DropsAction)Delegate.Combine(dropAsset2.action_landed, new DropsAction(action_dust_red));
		DropAsset dropAsset3 = t;
		dropAsset3.action_landed = (DropsAction)Delegate.Combine(dropAsset3.action_landed, new DropsAction(action_dust_blue));
		DropAsset dropAsset4 = t;
		dropAsset4.action_landed = (DropsAction)Delegate.Combine(dropAsset4.action_landed, new DropsAction(action_dust_gold));
		DropAsset dropAsset5 = t;
		dropAsset5.action_landed = (DropsAction)Delegate.Combine(dropAsset5.action_landed, new DropsAction(action_dust_purple));
		DropAsset dropAsset6 = t;
		dropAsset6.action_landed = (DropsAction)Delegate.Combine(dropAsset6.action_landed, new DropsAction(action_dust_black));
		add(new DropAsset
		{
			id = "dust_white",
			path_texture = "drops/drop_dust_white",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			action_landed = action_dust_white,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "dust_red",
			path_texture = "drops/drop_dust_red",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			action_landed = action_dust_red,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "dust_blue",
			path_texture = "drops/drop_dust_blue",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			action_landed = action_dust_blue,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "dust_gold",
			path_texture = "drops/drop_dust_gold",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			action_landed = action_dust_gold,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "dust_purple",
			path_texture = "drops/drop_dust_purple",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			action_landed = action_dust_purple,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "gamma_rain",
			path_texture = "drops/drop_gamma_rain",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_gamma_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropRainGamma",
			type = DropType.DropTraitRain
		});
		add(new DropAsset
		{
			id = "delta_rain",
			path_texture = "drops/drop_delta_rain",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_delta_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropRainDelta",
			type = DropType.DropTraitRain
		});
		add(new DropAsset
		{
			id = "omega_rain",
			path_texture = "drops/drop_omega_rain",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_omega_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropRainOmeaga",
			type = DropType.DropTraitRain
		});
		add(new DropAsset
		{
			id = "loot_rain",
			path_texture = "drops/drop_loot_rain",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_equipment_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropRainGamma",
			type = DropType.DropEquipmentRain
		});
		add(new DropAsset
		{
			id = "tnt",
			animated = true,
			path_texture = "drops/drop_tnt",
			animation_speed = 0.03f,
			default_scale = 0.2f,
			action_landed = action_tnt,
			sound_drop = "event:/SFX/DROPS/DropTnt",
			type = DropType.DropTile
		});
		add(new DropAsset
		{
			id = "tnt_timed",
			path_texture = "drops/drop_tnttimed",
			default_scale = 0.2f,
			action_landed = action_tnt_timed,
			sound_drop = "event:/SFX/DROPS/DropTnt",
			type = DropType.DropTile
		});
		add(new DropAsset
		{
			id = "water_bomb",
			path_texture = "drops/drop_waterbomb",
			default_scale = 0.2f,
			action_landed = action_water_bomb,
			sound_drop = "event:/SFX/DROPS/DropWaterBomb",
			type = DropType.DropTile
		});
		add(new DropAsset
		{
			id = "landmine",
			path_texture = "drops/drop_landmine",
			default_scale = 0.2f,
			action_landed = action_landmine,
			sound_drop = "event:/SFX/DROPS/DropLandmine",
			type = DropType.DropTile
		});
		add(new DropAsset
		{
			id = "fireworks",
			path_texture = "drops/drop_fireworks",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_fireworks,
			sound_drop = "event:/SFX/DROPS/DropFireworks",
			type = DropType.DropTile
		});
		add(new DropAsset
		{
			id = "inspiration",
			path_texture = "drops/drop_inspiration",
			default_scale = 0.2f,
			action_landed = action_inspiration,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropInspiration",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "discord",
			path_texture = "drops/drop_discord",
			default_scale = 0.2f,
			action_landed = action_discord,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropInspiration",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "friendship",
			path_texture = "drops/drop_friendship",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_friendship,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropFriendship",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "spite",
			path_texture = "drops/drop_spite",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_spite,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropSpite",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "madness",
			path_texture = "drops/drop_madness",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_madness,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropMadness",
			type = DropType.DropTrait
		});
		add(new DropAsset
		{
			id = "blessing",
			path_texture = "drops/drop_blessing",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.1f,
			action_landed = action_blessing,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		DropAsset dropAsset7 = t;
		dropAsset7.action_landed = (DropsAction)Delegate.Combine(dropAsset7.action_landed, new DropsAction(ActionLibrary.action_shrinkTornadoes));
		add(new DropAsset
		{
			id = "shield",
			path_texture = "drops/drop_shield",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_shield,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropShield",
			type = DropType.DropStatus
		});
		add(new DropAsset
		{
			id = "coffee",
			path_texture = "drops/drop_coffee",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_coffee,
			sound_drop = "event:/SFX/DROPS/DropCoffee",
			type = DropType.DropStatus
		});
		add(new DropAsset
		{
			id = "powerup",
			path_texture = "drops/drop_mushroom_powerup",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_powerup,
			sound_drop = "event:/SFX/DROPS/DropPowerup",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "curse",
			path_texture = "drops/drop_curse",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_curse,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropCurse",
			type = DropType.DropTrait
		});
		DropAsset dropAsset8 = t;
		dropAsset8.action_landed = (DropsAction)Delegate.Combine(dropAsset8.action_landed, new DropsAction(ActionLibrary.action_growTornadoes));
		add(new DropAsset
		{
			id = "spell_silence",
			path_texture = "drops/drop_spell_silence",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_spell_silence,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropCurse",
			type = DropType.DropTrait
		});
		add(new DropAsset
		{
			id = "zombie_infection",
			path_texture = "drops/drop_zombieinfection",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_zombie_infection,
			sound_drop = "event:/SFX/DROPS/DropZombieInfection",
			type = DropType.DropTrait
		});
		add(new DropAsset
		{
			id = "mush_spores",
			path_texture = "drops/drop_mushSpores",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_mush_spore,
			sound_drop = "event:/SFX/DROPS/DropMushSpores",
			type = DropType.DropTrait
		});
		add(new DropAsset
		{
			id = "plague",
			path_texture = "drops/drop_plague",
			random_frame = true,
			default_scale = 0.1f,
			action_landed = action_plague,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropPlague",
			type = DropType.DropTrait
		});
		add(new DropAsset
		{
			id = "living_plants",
			path_texture = "drops/drop_blessing",
			animated = true,
			default_scale = 0.1f,
			action_landed = action_living_plants,
			sound_drop = "event:/SFX/DROPS/DropLivingPlants",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "living_house",
			path_texture = "drops/drop_blessing",
			animated = true,
			default_scale = 0.1f,
			action_landed = action_living_house,
			sound_drop = "event:/SFX/DROPS/DropLivingHouse",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "bomb",
			path_texture = "drops/drop_bomb",
			default_scale = 0.2f,
			falling_height = new Vector2(60f, 70f),
			sound_launch = "event:/SFX/DROPS/DropLaunchBombSmall",
			action_landed = action_bomb,
			type = DropType.DropBomb
		});
		DropAsset dropAsset9 = t;
		dropAsset9.action_launch = (DropsAction)Delegate.Combine(dropAsset9.action_launch, new DropsAction(ActionLibrary.increaseDroppedBombsCounter));
		add(new DropAsset
		{
			id = "grenade",
			path_texture = "drops/drop_grenade",
			animated = true,
			default_scale = 0.2f,
			animation_speed = 0.03f,
			falling_height = new Vector2(60f, 70f),
			action_landed = action_grenade,
			random_flip = true,
			sound_launch = "event:/SFX/DROPS/DropLaunchGrenade",
			type = DropType.DropBomb
		});
		add(new DropAsset
		{
			id = "crab_bomb",
			path_texture = "drops/drop_crab_bomb_parachute",
			animated = true,
			default_scale = 0.1f,
			animation_speed = 0.05f,
			falling_height = new Vector2(60f, 70f),
			action_landed = action_crab_bomb_impact,
			random_flip = true,
			sound_launch = "event:/SFX/DROPS/DropLaunchCrabBomb",
			type = DropType.DropBomb
		});
		add(new DropAsset
		{
			id = "crab_bomb_shrapnel",
			path_texture = "drops/drop_crab_bomb_shrapnel",
			animated = true,
			animation_rotation = true,
			animation_rotation_speed_min = 50f,
			animation_rotation_speed_max = 200f,
			default_scale = 0.175f,
			animation_speed = 0.05f,
			falling_height = new Vector2(60f, 70f),
			action_landed = action_crab_bomb_shrapnel,
			random_flip = true,
			sound_launch = "event:/SFX/DROPS/DropLaunchCrabBomb",
			type = DropType.DropBomb,
			surprises_units = true
		});
		add(new DropAsset
		{
			id = "napalm_bomb",
			path_texture = "drops/drop_napalmbomb",
			default_scale = 0.2f,
			falling_height = new Vector2(60f, 70f),
			action_landed = action_napalm_bomb,
			random_flip = true,
			type = DropType.DropBomb
		});
		DropAsset dropAsset10 = t;
		dropAsset10.action_launch = (DropsAction)Delegate.Combine(dropAsset10.action_launch, new DropsAction(ActionLibrary.increaseDroppedBombsCounter));
		add(new DropAsset
		{
			id = "atomic_bomb",
			path_texture = "drops/drop_atomicbomb",
			default_scale = 0.2f,
			falling_height = new Vector2(60f, 70f),
			action_landed = action_atomic_bomb,
			random_flip = true,
			sound_launch = "event:/SFX/DROPS/DropLaunchGrenadeHuge",
			type = DropType.DropBomb
		});
		DropAsset dropAsset11 = t;
		dropAsset11.action_launch = (DropsAction)Delegate.Combine(dropAsset11.action_launch, new DropsAction(ActionLibrary.increaseDroppedBombsCounter));
		add(new DropAsset
		{
			id = "antimatter_bomb",
			path_texture = "drops/drop_antimatterbomb",
			default_scale = 0.2f,
			falling_height = new Vector2(60f, 70f),
			action_landed = action_antimatter_bomb,
			sound_launch = "event:/SFX/DROPS/DropLaunchGrenadeHuge",
			type = DropType.DropBomb
		});
		DropAsset dropAsset12 = t;
		dropAsset12.action_launch = (DropsAction)Delegate.Combine(dropAsset12.action_launch, new DropsAction(ActionLibrary.increaseDroppedBombsCounter));
		add(new DropAsset
		{
			id = "czar_bomba",
			path_texture = "drops/drop_czarbomba",
			default_scale = 0.2f,
			falling_height = new Vector2(60f, 70f),
			action_landed = action_czar_bomba,
			sound_launch = "event:/SFX/DROPS/DropLaunchGrenadeHuge",
			type = DropType.DropBomb
		});
		DropAsset dropAsset13 = t;
		dropAsset13.action_launch = (DropsAction)Delegate.Combine(dropAsset13.action_launch, new DropsAction(ActionLibrary.increaseDroppedBombsCounter));
		add(new DropAsset
		{
			id = "rain",
			path_texture = "drops/drop_rain",
			random_frame = true,
			default_scale = 0.2f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_rain,
			sound_drop = "event:/SFX/DROPS/DropRain",
			type = DropType.DropGeneric,
			surprises_units = false
		});
		add(new DropAsset
		{
			id = "blood_rain",
			path_texture = "drops/drop_blood",
			random_frame = true,
			default_scale = 0.1f,
			falling_height = new Vector2(30f, 45f),
			action_landed_drop = action_blood_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBloodRain",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "clone_rain",
			path_texture = "drops/drop_clone",
			random_frame = true,
			default_scale = 0.1f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_clone_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBloodRain",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "jazz",
			path_texture = "drops/drop_jazz",
			random_frame = true,
			default_scale = 0.1f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_jazz_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBloodRain",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "dispel",
			path_texture = "drops/drop_dispel",
			random_frame = true,
			default_scale = 0.1f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_dispel_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBloodRain",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "sleep",
			path_texture = "drops/drop_sleep",
			random_frame = true,
			default_scale = 0.1f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_sleep_rain,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBloodRain",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "cure",
			path_texture = "drops/drop_cure",
			random_frame = true,
			default_scale = 0.1f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_cure,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropCure",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "fire",
			path_texture = "drops/drop_fire",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.2f,
			falling_height = new Vector2(30f, 45f),
			falling_random_x_move = true,
			particle_interval = 0.3f,
			action_landed = action_fire,
			animation_speed_random = 0.08f,
			random_frame = true,
			random_flip = true,
			sound_drop = "event:/SFX/DROPS/DropFire",
			material = "mat_world_object_lit",
			type = DropType.DropGeneric
		});
		add(new DropAsset
		{
			id = "snow",
			path_texture = "drops/drop_snow",
			random_frame = true,
			default_scale = 0.2f,
			falling_speed = 0.3f,
			falling_height = new Vector2(30f, 45f),
			falling_random_x_move = true,
			particle_interval = 0.15f,
			sound_drop = "event:/SFX/DROPS/DropSnow",
			action_landed = action_snow,
			type = DropType.DropGeneric
		});
		add(new DropAsset
		{
			id = "life_seed",
			path_texture = "drops/drop_life_seed",
			random_frame = true,
			default_scale = 0.2f,
			falling_speed = 0.3f,
			falling_height = new Vector2(30f, 45f),
			falling_random_x_move = true,
			particle_interval = 0.15f,
			sound_drop = "event:/SFX/DROPS/DropSeedGrass",
			action_landed = action_life_seed,
			type = DropType.DropGeneric
		});
		add(new DropAsset
		{
			id = "ash",
			path_texture = "drops/drop_ash",
			random_frame = true,
			default_scale = 0.2f,
			falling_speed = 0.3f,
			falling_height = new Vector2(30f, 45f),
			falling_random_x_move = true,
			particle_interval = 0.15f,
			sound_drop = "event:/SFX/DROPS/DropAsh",
			action_landed = action_ash,
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "magic_rain",
			path_texture = "drops/drop_magic_rain",
			random_frame = true,
			default_scale = 0.2f,
			falling_speed = 0.3f,
			falling_height = new Vector2(30f, 45f),
			falling_random_x_move = true,
			particle_interval = 0.15f,
			sound_drop = "event:/SFX/DROPS/DropMagicRain",
			action_landed = action_magic_rain,
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "rage",
			path_texture = "drops/drop_rage",
			random_frame = true,
			default_scale = 0.2f,
			falling_speed = 0.3f,
			falling_height = new Vector2(30f, 45f),
			falling_random_x_move = true,
			particle_interval = 0.15f,
			sound_drop = "event:/SFX/DROPS/DropRage",
			action_landed = action_rage,
			type = DropType.DropStatus
		});
		add(new DropAsset
		{
			id = "acid",
			path_texture = "drops/drop_acid",
			random_frame = true,
			default_scale = 0.2f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_acid,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropAcid",
			type = DropType.DropMagic
		});
		add(new DropAsset
		{
			id = "lava",
			path_texture = "drops/drop_lava",
			animated = true,
			animation_speed = 0.03f,
			default_scale = 0.2f,
			falling_height = new Vector2(30f, 45f),
			action_landed = action_lava,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropLava",
			type = DropType.DropGeneric
		});
		add(new DropAsset
		{
			id = "santa_bomb",
			path_texture = "drops/drop_santabomb",
			random_frame = true,
			default_scale = 0.2f,
			sound_launch = "event:/SFX/DROPS/DropLaunchSantaBomb",
			action_landed = action_santa_bomb,
			type = DropType.DropBomb,
			surprises_units = true
		});
		add(new DropAsset
		{
			id = "$spawn_building$",
			path_texture = "drops/drop_stone",
			random_frame = true,
			default_scale = 0.2f,
			falling_height = new Vector2(10f, 15f),
			falling_speed = 5f,
			type = DropType.DropBuilding
		});
		t.action_landed = action_spawn_building;
		clone("$biome_seeds$", "$spawn_building$");
		t.type = DropType.DropSeed;
		t.action_landed = null;
		clone("seeds_grass", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_grass";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "grass_low";
		t.drop_type_high = "grass_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_enchanted", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_enchanted";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "enchanted_low";
		t.drop_type_high = "enchanted_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedEnchanted";
		clone("seeds_savanna", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_savanna";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "savanna_low";
		t.drop_type_high = "savanna_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedSavanna";
		clone("seeds_corrupted", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_corrupted";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "corrupted_low";
		t.drop_type_high = "corrupted_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedCorrupted";
		clone("seeds_mushroom", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_mushroom";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "mushroom_low";
		t.drop_type_high = "mushroom_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedMushroom";
		clone("seeds_jungle", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_jungle";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "jungle_low";
		t.drop_type_high = "jungle_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedJungle";
		clone("seeds_desert", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_desert";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "desert_low";
		t.drop_type_high = "desert_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedDesert";
		clone("seeds_lemon", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_lemon";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "lemon_low";
		t.drop_type_high = "lemon_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedLemon";
		clone("seeds_permafrost", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_permafrost";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "permafrost_low";
		t.drop_type_high = "permafrost_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedPermafrost";
		clone("seeds_candy", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_candy";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "candy_low";
		t.drop_type_high = "candy_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedCandy";
		clone("seeds_crystal", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_crystal";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "crystal_low";
		t.drop_type_high = "crystal_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedCrystal";
		clone("seeds_swamp", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_swamp";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "swamp_low";
		t.drop_type_high = "swamp_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedSwamp";
		clone("seeds_infernal", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_infernal";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "infernal_low";
		t.drop_type_high = "infernal_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedInfernal";
		clone("seeds_birch", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_birch";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "birch_low";
		t.drop_type_high = "birch_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_maple", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_maple";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "maple_low";
		t.drop_type_high = "maple_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_rocklands", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_rocklands";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "rocklands_low";
		t.drop_type_high = "rocklands_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_garlic", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_garlic";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "garlic_low";
		t.drop_type_high = "garlic_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_flower", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_flower";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "flower_low";
		t.drop_type_high = "flower_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_celestial", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_celestial";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "celestial_low";
		t.drop_type_high = "celestial_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_singularity", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_singularity";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "singularity_low";
		t.drop_type_high = "singularity_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_clover", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_clover";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "clover_low";
		t.drop_type_high = "clover_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("seeds_paradox", "$biome_seeds$");
		t.path_texture = "drops/drop_seed_paradox";
		t.falling_speed = 3f;
		t.action_landed = action_drop_seeds;
		t.drop_type_low = "paradox_low";
		t.drop_type_high = "paradox_high";
		t.sound_drop = "event:/SFX/DROPS/DropSeedGrass";
		clone("fruit_bush", "$spawn_building$");
		t.path_texture = "drops/drop_seed";
		t.falling_speed = 3f;
		t.action_landed = action_fruit_bush;
		t.sound_drop = "event:/SFX/DROPS/DropBush";
		clone("fertilizer_plants", "$biome_seeds$");
		t.surprises_units = false;
		t.path_texture = "drops/drop_fertilizer";
		t.falling_speed = 5f;
		DropAsset dropAsset14 = t;
		dropAsset14.action_landed = (DropsAction)Delegate.Combine(dropAsset14.action_landed, new DropsAction(action_fertilizer_plants));
		DropAsset dropAsset15 = t;
		dropAsset15.action_landed = (DropsAction)Delegate.Combine(dropAsset15.action_landed, new DropsAction(tryToGrowWheat));
		DropAsset dropAsset16 = t;
		dropAsset16.action_landed = (DropsAction)Delegate.Combine(dropAsset16.action_landed, new DropsAction(flash));
		t.sound_drop = "event:/SFX/DROPS/DropFertilizerPlants";
		clone("fertilizer_trees", "$biome_seeds$");
		t.path_texture = "drops/drop_fertilizer";
		t.falling_speed = 5f;
		t.action_landed = action_fertilizer_trees;
		DropAsset dropAsset17 = t;
		dropAsset17.action_landed = (DropsAction)Delegate.Combine(dropAsset17.action_landed, new DropsAction(flash));
		t.sound_drop = "event:/SFX/DROPS/DropFertilizerPlants";
		clone("$spawn_mineral$", "$spawn_building$");
		t.falling_speed = 6f;
		t.type = DropType.DropMineral;
		clone("stone", "$spawn_mineral$");
		t.path_texture = "drops/drop_stone";
		t.default_scale = 0.2f;
		t.building_asset = "mineral_stone";
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropStone";
		clone("metals", "$spawn_mineral$");
		t.path_texture = "drops/drop_metal";
		t.default_scale = 0.2f;
		t.building_asset = "mineral_metals";
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropMineral";
		clone("gold", "$spawn_mineral$");
		t.path_texture = "drops/drop_gold";
		t.default_scale = 0.2f;
		t.building_asset = "mineral_gold";
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropGold";
		clone("silver", "$spawn_mineral$");
		t.path_texture = "drops/drop_stone";
		t.default_scale = 0.2f;
		t.building_asset = "mineral_silver";
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropMineral";
		clone("mythril", "$spawn_mineral$");
		t.path_texture = "drops/drop_stone";
		t.default_scale = 0.2f;
		t.building_asset = "mineral_mythril";
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropMineral";
		clone("adamantine", "$spawn_mineral$");
		t.path_texture = "drops/drop_stone";
		t.default_scale = 0.2f;
		t.building_asset = "mineral_adamantine";
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropMineral";
		clone("$spawn_creep$", "$spawn_building$");
		t.type = DropType.DropCreep;
		clone("tumor", "$spawn_creep$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropTumor";
		clone("biomass", "$spawn_creep$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropBiomass";
		clone("cybercore", "$spawn_creep$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropCybercore";
		clone("super_pumpkin", "$spawn_creep$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropSuperPumpkin";
		clone("geyser", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropGeyser";
		clone("geyser_acid", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropGeyser";
		clone("volcano", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropVolcano";
		clone("golden_brain", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropGoldenBrain";
		clone("monolith", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		DropAsset dropAsset18 = t;
		dropAsset18.action_landed = (DropsAction)Delegate.Combine(dropAsset18.action_landed, (DropsAction)delegate
		{
			AchievementLibrary.cant_be_too_much.checkBySignal();
		});
		t.sound_drop = "event:/SFX/DROPS/DropMonolith";
		clone("corrupted_brain", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropCorruptedBrain";
		clone("ice_tower", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropIceTower";
		clone("angle_tower", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropIceTower";
		clone("beehive", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropBeehive";
		clone("flame_tower", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropFlameTower";
		addWaypointDrops();
	}

	public override void linkAssets()
	{
		foreach (DropAsset tAsset in list)
		{
			if (!string.IsNullOrEmpty(tAsset.drop_type_high))
			{
				tAsset.cached_drop_type_high = AssetManager.top_tiles.get(tAsset.drop_type_high);
			}
			if (!string.IsNullOrEmpty(tAsset.drop_type_low))
			{
				tAsset.cached_drop_type_low = AssetManager.top_tiles.get(tAsset.drop_type_low);
			}
		}
		base.linkAssets();
	}

	private void addWaypointDrops()
	{
		add(new DropAsset
		{
			id = "desire_alien_mold",
			path_texture = "drops/drop_alien_mold",
			animated = false,
			default_scale = 0.1f,
			material = "mat_world_object_lit",
			sound_drop = "event:/SFX/DROPS/DropBlessing",
			type = DropType.DropMagic
		});
		t.action_landed = action_alien_mold;
		t.surprises_units = true;
		clone("desire_computer", "desire_alien_mold");
		t.path_texture = "drops/drop_computer";
		t.action_landed = action_drop_computer;
		clone("desire_golden_egg", "desire_alien_mold");
		t.path_texture = "drops/drop_golden_egg";
		t.action_landed = action_drop_golden_egg;
		clone("desire_harp", "desire_alien_mold");
		t.path_texture = "drops/drop_harp";
		t.action_landed = action_drop_harp;
		clone("waypoint_alien_mold", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropCorruptedBrain";
		clone("waypoint_computer", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropCorruptedBrain";
		clone("waypoint_golden_egg", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropCorruptedBrain";
		clone("waypoint_harp", "$spawn_building$");
		t.building_asset = t.id;
		t.action_landed = action_spawn_building;
		t.sound_drop = "event:/SFX/DROPS/DropCorruptedBrain";
	}

	public static void action_drop_seeds(WorldTile pTile = null, string pDropID = null)
	{
		DropAsset tDropAsset = AssetManager.drops.get(pDropID);
		useDropSeedOn(pTile, tDropAsset.cached_drop_type_low, tDropAsset.cached_drop_type_high);
	}

	public static void useDropSeedOn(WorldTile pTile, TopTileType pTypeLow, TopTileType pHigh)
	{
		useSeedOn(pTile, pTypeLow, pHigh);
		for (int i = 0; i < pTile.neighbours.Length; i++)
		{
			useSeedOn(pTile.neighbours[i], pTypeLow, pHigh);
		}
	}

	public static void tryToGrowWheat(WorldTile pTile = null, string pDropID = null)
	{
		if (pTile.Type.farm_field && !pTile.hasBuilding())
		{
			World.world.buildings.addBuilding("wheat", pTile);
		}
	}

	public static void useSeedOn(WorldTile pTile, TopTileType pTypeLow, TopTileType pHigh)
	{
		pTile.unfreeze();
		if (!pTile.Type.can_be_biome)
		{
			return;
		}
		if (pTile.isTileRank(TileRank.Low))
		{
			MapAction.growGreens(pTile, pTypeLow);
		}
		else if (pTile.isTileRank(TileRank.High))
		{
			MapAction.growGreens(pTile, pHigh);
		}
		BiomeAsset tBiome = pTile.getBiome();
		if (tBiome == null)
		{
			return;
		}
		pTile.doUnits(delegate(Actor tActor)
		{
			if (!tActor.hasSubspecies() || tBiome.spawn_trait_subspecies_always == null)
			{
				return;
			}
			foreach (string current in tBiome.spawn_trait_subspecies_always)
			{
				tActor.subspecies.addTrait(current);
			}
		});
	}

	public static void action_rain(WorldTile pTile = null, string pDropID = null)
	{
		useRainOn(pTile);
		for (int i = 0; i < pTile.neighbours.Length; i++)
		{
			useRainOn(pTile.neighbours[i]);
		}
		for (int j = 0; j < pTile.neighbours.Length; j++)
		{
			WorldTile tNeighbour = pTile.neighbours[j];
			if (tNeighbour.isOnFire())
			{
				tNeighbour.stopFire();
			}
		}
	}

	private static void useRainOn(WorldTile pTile)
	{
		pTile.stopFire();
		pTile.doUnits(delegate(Actor tActor)
		{
			tActor.finishStatusEffect("burning");
			tActor.finishAngryStatus();
			if (tActor.isDamagedByRain())
			{
				tActor.getHit(tActor.getWaterDamage(), pFlash: true, AttackType.Water);
			}
			else
			{
				tActor.addStamina((int)((float)tActor.getMaxStamina() * 0.1f));
			}
		});
		if (pTile.hasBuilding())
		{
			pTile.building.stopFire();
			if (pTile.building.asset.wheat)
			{
				pTile.building.component_wheat.grow();
			}
		}
		if (pTile.hasBuilding() && pTile.building.asset.damaged_by_rain)
		{
			pTile.building.getHit(20f);
		}
		pTile.removeBurn();
		if (pTile.Type.can_be_filled_with_ocean)
		{
			MapAction.setOcean(pTile);
		}
		if (pTile.Type.lava)
		{
			LavaHelper.putOut(pTile);
		}
		if (pTile.Type.explodable_by_ocean)
		{
			World.world.explosion_layer.explodeBomb(pTile);
		}
		BiomeAsset biome = pTile.getBiome();
		if (biome != null && biome.spread_by_drops_water)
		{
			WorldBehaviourActionBiomes.trySpreadBiomeAround(pTile, pTile);
		}
	}

	public static void action_gamma_rain(WorldTile pTile = null, string pDropID = null)
	{
		List<string> tList = PlayerConfig.instance.data.trait_editor_gamma;
		useTraitRain(pTile, tList, PlayerConfig.instance.data.trait_editor_gamma_state);
	}

	public static void action_delta_rain(WorldTile pTile = null, string pDropID = null)
	{
		List<string> tList = PlayerConfig.instance.data.trait_editor_delta;
		useTraitRain(pTile, tList, PlayerConfig.instance.data.trait_editor_delta_state);
	}

	public static void action_omega_rain(WorldTile pTile = null, string pDropID = null)
	{
		List<string> tList = PlayerConfig.instance.data.trait_editor_omega;
		useTraitRain(pTile, tList, PlayerConfig.instance.data.trait_editor_omega_state);
	}

	private static void useTraitRain(WorldTile pTile, List<string> pList, RainState pRainState)
	{
		if (pList.Count == 0)
		{
			return;
		}
		using ListPool<ActorTrait> tList = new ListPool<ActorTrait>(pList.Count);
		foreach (string tTraitID in pList)
		{
			ActorTrait tTrait = AssetManager.traits.get(tTraitID);
			if (tTrait != null && tTrait.isAvailable() && (pRainState != RainState.Add || tTrait.can_be_given) && (pRainState != RainState.Remove || tTrait.can_be_removed))
			{
				tList.Add(tTrait);
			}
		}
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (!tActor.asset.can_edit_traits)
			{
				continue;
			}
			if (pRainState == RainState.Remove)
			{
				tActor.removeTraits(tList);
			}
			else
			{
				foreach (ref ActorTrait item in tList)
				{
					ActorTrait tTrait2 = item;
					tActor.addTrait(tTrait2, pRemoveOpposites: true);
				}
			}
			tActor.addTrait("scar_of_divinity");
			tActor.startShake();
			tActor.makeConfused(-1f, pColorEffect: true);
		}
	}

	public static void action_equipment_rain(WorldTile pTile = null, string pDropID = null)
	{
		List<string> tItems = PlayerConfig.instance.data.equipment_editor;
		useEquipmentRain(pTile, tItems, PlayerConfig.instance.data.equipment_editor_state);
	}

	private static void useEquipmentRain(WorldTile pTile, List<string> pItems, RainState pRainState)
	{
		if (pItems.Count == 0)
		{
			return;
		}
		pItems.Shuffle();
		using ListPool<EquipmentAsset> tListItems = new ListPool<EquipmentAsset>(pItems.Count);
		HashSet<EquipmentType> tTempSetTypes = UnsafeCollectionPool<HashSet<EquipmentType>, EquipmentType>.Get();
		for (int i = 0; i < pItems.Count; i++)
		{
			string tItemId = pItems[i];
			EquipmentAsset tItemAsset = AssetManager.items.get(tItemId);
			if (tItemAsset != null && (pRainState != RainState.Add || !tTempSetTypes.Contains(tItemAsset.equipment_type)) && tItemAsset.isAvailable() && (pRainState != RainState.Add || tItemAsset.can_be_given) && (pRainState != RainState.Remove || tItemAsset.can_be_removed))
			{
				tTempSetTypes.Add(tItemAsset.equipment_type);
				tListItems.Add(tItemAsset);
			}
		}
		UnsafeCollectionPool<HashSet<EquipmentType>, EquipmentType>.Release(tTempSetTypes);
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (!tActor.canEditEquipment())
			{
				continue;
			}
			for (int j = 0; j < tListItems.Count; j++)
			{
				EquipmentAsset tItemAsset2 = tListItems[j];
				if (!tActor.asset.canEditItem(tItemAsset2))
				{
					continue;
				}
				ActorEquipmentSlot tSlot = tActor.equipment.getSlot(tItemAsset2.equipment_type);
				Item tSlotItem = tSlot.getItem();
				if (pRainState == RainState.Remove)
				{
					if (tSlot.isEmpty() || tSlotItem.asset.id != tItemAsset2.id)
					{
						continue;
					}
				}
				else if (!tSlot.isEmpty() && (tSlotItem.asset.id == tItemAsset2.id || tSlotItem.isFavorite() || tSlotItem.isCursed()))
				{
					continue;
				}
				if (pRainState == RainState.Remove)
				{
					tSlotItem.data.favorite = false;
					tSlotItem.removeMod("eternal");
					tSlot.takeAwayItem();
				}
				else
				{
					Item tItem = World.world.items.generateItem(tItemAsset2, tActor.kingdom, World.world.map_stats.player_name, 1, tActor, 0, pByPlayer: true);
					tItem.addMod("divine_rune");
					tActor.equipment.setItem(tItem, tActor);
				}
			}
			tActor.startShake();
			tActor.makeConfused(-1f, pColorEffect: true);
		}
	}

	public static void action_acid(WorldTile pTile = null, string pDropID = null)
	{
		MapAction.checkAcidTerraform(pTile);
		if (Randy.randomChance(0.2f))
		{
			World.world.particles_smoke.spawn(pTile.posV3);
		}
		if (pTile.hasBuilding() && pTile.building.asset.affected_by_acid && pTile.building.isAlive())
		{
			pTile.building.getHit(20f);
		}
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (!Randy.randomChance(0.6f) && !tActor.hasTrait("acid_proof") && !tActor.hasTrait("acid_blood"))
			{
				tActor.getHit(20f, pFlash: true, AttackType.Acid);
			}
		}
		World.world.conway_layer.checkKillRange(pTile.pos, 2);
		BiomeAsset biome = pTile.getBiome();
		if (biome != null && biome.spread_by_drops_acid)
		{
			WorldBehaviourActionBiomes.trySpreadBiomeAround(pTile, pTile, pCheckRoad: false, pCheckBonuses: false, pForce: true);
		}
	}

	public static void action_fire(WorldTile pTile = null, string pDropID = null)
	{
		ActionLibrary.burnTile(null, null, pTile);
		ActionLibrary.startBurningObjects(null, null, pTile);
		BiomeAsset biome = pTile.getBiome();
		if (biome != null && biome.spread_by_drops_fire)
		{
			WorldBehaviourActionBiomes.trySpreadBiomeAround(pTile, pTile, pCheckRoad: false, pCheckBonuses: false, pForce: true);
		}
	}

	public static void action_fireworks(WorldTile pTile = null, string pDropID = null)
	{
		MapAction.terraformTop(pTile, TopTileLibrary.fireworks, TerraformLibrary.remove);
	}

	public static void action_tnt(WorldTile pTile = null, string pDropID = null)
	{
		if (pTile.Type.lava || pTile.isOnFire())
		{
			MapAction.terraformTop(pTile, TopTileLibrary.tnt, TerraformLibrary.remove);
			World.world.explosion_layer.explodeBomb(pTile);
		}
		else
		{
			MapAction.terraformTop(pTile, TopTileLibrary.tnt, TerraformLibrary.remove);
		}
	}

	public static void action_tnt_timed(WorldTile pTile = null, string pDropID = null)
	{
		if (pTile.Type.lava || pTile.isOnFire())
		{
			MapAction.terraformTop(pTile, TopTileLibrary.tnt_timed, TerraformLibrary.remove);
			World.world.explosion_layer.explodeBomb(pTile);
		}
		else
		{
			MapAction.terraformTop(pTile, TopTileLibrary.tnt_timed, TerraformLibrary.remove);
		}
	}

	public static void action_czar_bomba(WorldTile pTile = null, string pDropID = null)
	{
		EffectsLibrary.spawn("fx_nuke_flash", pTile, "czar_bomba");
		World.world.startShake(0.3f, 0.01f, 2.5f, pShakeX: true);
	}

	public static void action_atomic_bomb(WorldTile pTile = null, string pDropID = null)
	{
		World.world.startShake(0.3f, 0.01f, 2f, pShakeX: true);
		EffectsLibrary.spawn("fx_nuke_flash", pTile, "atomic_bomb");
	}

	public static void action_antimatter_bomb(WorldTile pTile = null, string pDropID = null)
	{
		World.world.startShake(0.3f, 0.01f, 0.03f);
		EffectsLibrary.spawn("fx_antimatter_effect", pTile);
	}

	public static void action_napalm_bomb(WorldTile pTile = null, string pDropID = null)
	{
		World.world.startShake(0.3f, 0.01f, 0.5f, pShakeX: true);
		EffectsLibrary.spawn("fx_napalm_flash", pTile);
		EffectsLibrary.spawnAtTileRandomScale("fx_explosion_tiny", pTile, 0.15f, 0.3f);
	}

	public static void action_crab_bomb_impact(WorldTile pTile = null, string pDropID = null)
	{
		MusicBox.playSound("event:/SFX/DESTRUCTION/CrabBombImpact", pTile);
		int tAmount = Randy.randomInt(1, 4);
		for (int i = 0; i < tAmount; i++)
		{
			World.world.drop_manager.spawnParabolicDrop(pTile, "crab_bomb_shrapnel", 1f, 15f, 40f, 4f, 16f);
		}
	}

	public static void action_crab_bomb_shrapnel(WorldTile pTile = null, string pDropID = null)
	{
		EffectsLibrary.spawnAt("fx_explosion_crab_bomb", pTile.posV, 0.25f);
		World.world.startShake(0.3f, 0.01f, 0.5f, pShakeX: true);
		MapAction.damageWorld(pTile, 2, AssetManager.terraform.get("crab_bomb"));
		if (Randy.randomChance(0.05f))
		{
			action_crab_bomb_impact(pTile, "crab_bomb_shrapnel");
		}
	}

	public static void action_grenade(WorldTile pTile = null, string pDropID = null)
	{
		MapAction.damageWorld(pTile, 5, AssetManager.terraform.get("grenade"));
		EffectsLibrary.spawnAtTileRandomScale("fx_explosion_small", pTile, 0.1f, 0.15f);
	}

	public static void action_bomb(WorldTile pTile = null, string pDropID = null)
	{
		EffectsLibrary.spawnAtTileRandomScale("fx_explosion_middle", pTile, 0.45f, 0.6f);
		if (!World.world.explosion_checker.checkNearby(pTile, 10))
		{
			MapAction.damageWorld(pTile, 10, AssetManager.terraform.get("bomb"));
		}
	}

	public static void action_santa_bomb(WorldTile pTile = null, string pDropID = null)
	{
		MapAction.damageWorld(pTile, 10, AssetManager.terraform.get("santa_bomb"));
		EffectsLibrary.spawnAtTileRandomScale("fx_explosion_small", pTile, 0.45f, 0.6f);
	}

	public static void action_water_bomb(WorldTile pTile = null, string pDropID = null)
	{
		if (pTile.Type.liquid || pTile.Type.lava || pTile.isOnFire())
		{
			MapAction.terraformTop(pTile, TopTileLibrary.water_bomb, TerraformLibrary.remove);
			World.world.explosion_layer.explodeBomb(pTile);
		}
		else
		{
			MapAction.terraformTop(pTile, TopTileLibrary.water_bomb, TerraformLibrary.remove);
		}
	}

	public static void action_lava(WorldTile pTile = null, string pDropID = null)
	{
		LavaHelper.addLava(pTile);
	}

	public static void action_rage(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (Randy.randomChance(0.2f))
			{
				tActor.addStatusEffect("rage");
			}
		}
	}

	public static void action_magic_rain(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (Randy.randomChance(0.2f))
			{
				tActor.addStatusEffect("powerup");
			}
			if (Randy.randomChance(0.2f))
			{
				tActor.addStatusEffect("spell_boost");
			}
			if (Randy.randomChance(0.2f))
			{
				tActor.addStatusEffect("shield");
			}
			if (Randy.randomChance(0.2f))
			{
				tActor.addStatusEffect("caffeinated");
			}
			tActor.addMana((int)((float)tActor.getMaxMana() * 0.1f));
		}
	}

	public static void action_ash(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (Randy.randomChance(0.3f))
			{
				tActor.addStatusEffect("cough");
			}
			if (Randy.randomChance(0.1f))
			{
				tActor.addStatusEffect("ash_fever");
			}
		}
	}

	public static void action_life_seed(WorldTile pTile = null, string pDropID = null)
	{
		if (WorldLawLibrary.world_law_animals_spawn.isEnabled())
		{
			trySpawnUnit(pTile);
		}
		if (WorldLawLibrary.world_law_vegetation_random_seeds.isEnabled())
		{
			trySpawnVegetation(pTile);
		}
	}

	private void action_jazz_rain(WorldTile pTile, string pDropID)
	{
		Actor tRandomParent = null;
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f, pRandom: true))
		{
			if (tActor.hasSubspecies() && tActor.isBreedingAge())
			{
				tRandomParent = tActor;
				break;
			}
		}
		if (tRandomParent != null)
		{
			BabyMaker.makeBabyFromMiracle(tRandomParent, ActorSex.None, pAddToFamily: true);
		}
	}

	private static void trySpawnUnit(WorldTile pTile)
	{
		BiomeAsset tBiomeAsset = pTile.Type.biome_asset;
		if (tBiomeAsset == null || !tBiomeAsset.pot_spawn_units_auto)
		{
			return;
		}
		string tUnitID = tBiomeAsset.pot_units_spawn.GetRandom();
		bool tSapient = false;
		if (WorldLawLibrary.world_law_drop_of_thoughts.isEnabled() && Randy.randomBool() && tBiomeAsset.pot_sapient_units_spawn != null)
		{
			foreach (string tID in tBiomeAsset.pot_sapient_units_spawn.LoopRandom())
			{
				ActorAsset tAsset = AssetManager.actor_library.get(tID);
				if (tAsset.isAvailable())
				{
					GodPower tPower = tAsset.getGodPower();
					if (tPower == null || tPower.isAvailable())
					{
						tUnitID = tID;
						tSapient = true;
						break;
					}
				}
			}
		}
		ActorAsset tActorAsset = AssetManager.actor_library.get(tUnitID);
		if (tActorAsset == null || tActorAsset.units.Count > tActorAsset.max_random_amount)
		{
			return;
		}
		int tCountActors = 0;
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1))
		{
			_ = item;
			if (tCountActors++ > 3)
			{
				return;
			}
		}
		Actor tActor = World.world.units.spawnNewUnit(tActorAsset.id, pTile);
		if (tSapient && tActor != null && tActor.subspecies.isJustCreated())
		{
			tActor.subspecies.makeSapient();
		}
	}

	private static void trySpawnVegetation(WorldTile pTile)
	{
		BiomeAsset tBiomeAsset = pTile.Type.biome_asset;
		if (tBiomeAsset != null && tBiomeAsset.grow_vegetation_auto)
		{
			ActionLibrary.growRandomVegetation(pTile, tBiomeAsset);
		}
	}

	public static void action_snow(WorldTile pTile = null, string pDropID = null)
	{
		if (pTile.canBeFrozen())
		{
			pTile.freeze();
		}
		for (int i = 0; i < 10; i++)
		{
			WorldTile tRandTile = pTile.chunk.tiles.GetRandom();
			if (tRandTile.canBeFrozen())
			{
				if (Toolbox.DistTile(pTile, tRandTile) < 11f)
				{
					break;
				}
				tRandTile.freeze();
			}
		}
		if (pTile.Type.lava)
		{
			return;
		}
		if (Randy.randomBool())
		{
			foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
			{
				ActionLibrary.addFrozenEffectOnTarget(tActor, tActor);
			}
		}
		checkColdOneBabies(pTile);
	}

	public static void checkColdOneBabies(WorldTile pTile)
	{
		if (!WorldLawLibrary.world_law_disasters_other.isEnabled() || !World.world_era.era_disaster_snow_turns_babies_into_ice_ones)
		{
			return;
		}
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.canTurnIntoColdOne())
			{
				ActionLibrary.turnIntoIceOne(tActor);
			}
		}
	}

	private static void action_cure(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 4f))
		{
			item.removeTrait("plague");
			item.removeTrait("tumor_infection");
			item.removeTrait("mush_spores");
			item.removeTrait("infected");
			item.finishStatusEffect("ash_fever");
			item.finishStatusEffect("cursed");
			item.startShake();
			item.startColorEffect();
		}
	}

	private static void action_clone_rain(WorldTile pTile, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 1f, pRandom: true))
		{
			WorldTile tTileTarget = null;
			foreach (WorldTile tTile in tActor.current_tile.neighboursAll.LoopRandom())
			{
				if (!tTile.hasUnits())
				{
					tTileTarget = tTile;
					break;
				}
			}
			if (tTileTarget != null && World.world.units.cloneUnit(tActor, tTileTarget))
			{
				break;
			}
		}
	}

	private void action_sleep_rain(WorldTile pTile, string pDropID)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.makeSleep(60f) && !tActor.isLying())
			{
				tActor.applyRandomForce();
			}
		}
	}

	private void action_dispel_rain(WorldTile pTile, string pDropID)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			tActor.finishStatusEffect("powerup");
			tActor.finishStatusEffect("enchanted");
			tActor.finishStatusEffect("slowness");
			tActor.finishStatusEffect("shield");
			tActor.finishStatusEffect("invincible");
			tActor.finishStatusEffect("spell_boost");
			if (tActor.asset.die_from_dispel)
			{
				tActor.getHit(tActor.getMaxHealthPercent(0.5f), pFlash: true, AttackType.Other, null, pSkipIfShake: true, pMetallicWeapon: false, pCheckDamageReduction: true);
			}
		}
	}

	public static void action_blood_rain(Drop pDrop, WorldTile pTile = null, string pDropID = null)
	{
		long tCasterId = pDrop.getCasterId();
		Actor tCaster = World.world.units.get(tCasterId);
		bool tIsCasterOk = !tCaster.isRekt();
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (!tIsCasterOk || tActor.id == tCasterId || !tCaster.kingdom.isEnemy(tActor.kingdom))
			{
				tActor.finishStatusEffect("burning");
				tActor.restoreHealth(tActor.getMaxHealthPercent(0.2f));
				tActor.startShake();
				tActor.startColorEffect();
			}
		}
	}

	public static void action_plague(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 4f))
		{
			if (tActor.hasTrait("plague"))
			{
				tActor.startShake();
				tActor.startColorEffect();
			}
			else
			{
				tActor.addTrait("plague");
			}
		}
	}

	public static void action_zombie_infection(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.can_turn_into_zombie && !tActor.hasTrait("zombie"))
			{
				tActor.addTrait("infected");
				tActor.startShake();
				tActor.startColorEffect();
			}
		}
	}

	public static void action_mush_spore(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.can_turn_into_mush && !tActor.hasTrait("mush_spores"))
			{
				tActor.addTrait("mush_spores");
				tActor.startShake();
				tActor.startColorEffect();
			}
		}
	}

	private static void action_curse(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.addStatusEffect("cursed"))
			{
				tActor.setStatsDirty();
				tActor.removeTrait("blessed");
				tActor.startShake();
				tActor.startColorEffect();
			}
		}
		BiomeAsset biome = pTile.getBiome();
		if (biome != null && biome.spread_by_drops_curse)
		{
			WorldBehaviourActionBiomes.trySpreadBiomeAround(pTile, pTile, pCheckRoad: false, pCheckBonuses: false, pForce: true);
		}
	}

	private static void action_spell_silence(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addStatusEffect("spell_silence");
		}
	}

	private static void action_shield(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addStatusEffect("shield");
		}
	}

	private static void action_powerup(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			tActor.addStatusEffect("powerup");
			if (tActor.isSameSpecies("mush_unit") || tActor.isSameSpecies("mush_animal"))
			{
				AchievementLibrary.super_mushroom.check();
			}
		}
		BiomeAsset biome = pTile.getBiome();
		if (biome != null && biome.spread_by_drops_powerup)
		{
			WorldBehaviourActionBiomes.trySpreadBiomeAround(pTile, pTile, pCheckRoad: false, pCheckBonuses: false, pForce: true);
		}
	}

	private static void action_paint(WorldTile pTile = null, string pDropID = null)
	{
		TileZone tMainZone = pTile.zone;
		if (!tMainZone.hasCity())
		{
			return;
		}
		City tMainCity = tMainZone.city;
		World.world.city_zone_helper.city_growth.getZoneToClaim(null, tMainCity, pDebug: true, _paint_zones_hashset, 1);
		using ListPool<TileZone> tZones = new ListPool<TileZone>();
		foreach (TileZone tZone in _paint_zones_hashset)
		{
			if (tZone.hasCity())
			{
				continue;
			}
			TileZone[] tNeighbours = tZone.neighbours;
			for (int i = 0; i < tNeighbours.Length; i++)
			{
				if (tNeighbours[i].city == tMainCity)
				{
					tZones.Add(tZone);
				}
			}
		}
		if (tZones.Count > 0)
		{
			TileZone tRandomZone = tZones.GetRandom();
			tMainCity.addZone(tRandomZone);
			tMainCity.setAbandonedZonesDirty();
		}
		_paint_zones_hashset.Clear();
	}

	public static void action_dust_black(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.affected_by_dust)
			{
				tActor.makeConfused(-1f, pColorEffect: true);
			}
		}
	}

	public static void action_dust_white(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.affected_by_dust)
			{
				tActor.forgetLanguage();
			}
		}
	}

	public static void action_dust_red(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.affected_by_dust)
			{
				tActor.makeConfused(-1f, pColorEffect: true);
				if (tActor.hasFamily())
				{
					tActor.setFamily(null);
				}
				if (tActor.hasClan())
				{
					tActor.forgetClan();
				}
				if (tActor.hasLover())
				{
					Actor lover = tActor.lover;
					tActor.setLover(null);
					lover.setLover(null);
				}
			}
		}
	}

	public static void action_dust_blue(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.affected_by_dust)
			{
				tActor.forgetCulture();
			}
		}
	}

	public static void action_dust_gold(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.affected_by_dust)
			{
				tActor.forgetKingdomAndCity();
			}
		}
	}

	public static void action_dust_purple(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.asset.affected_by_dust)
			{
				tActor.forgetReligion();
			}
		}
	}

	public static void action_coffee(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addStatusEffect("caffeinated");
		}
		BiomeAsset biome = pTile.getBiome();
		if (biome != null && biome.spread_by_drops_coffee)
		{
			WorldBehaviourActionBiomes.trySpreadBiomeAround(pTile, pTile, pCheckRoad: false, pCheckBonuses: false, pForce: true);
		}
	}

	public static void action_blessing(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			if (tActor.addTrait("blessed"))
			{
				tActor.setStatsDirty();
				tActor.event_full_stats = true;
			}
			tActor.finishStatusEffect("cursed");
			tActor.startShake();
			if (tActor.isSameSpecies("frog"))
			{
				AchievementLibrary.the_princess.check();
			}
			tActor.startColorEffect();
		}
		BiomeAsset biome = pTile.getBiome();
		if (biome != null && biome.spread_by_drops_blessing)
		{
			WorldBehaviourActionBiomes.trySpreadBiomeAround(pTile, pTile, pCheckRoad: false, pCheckBonuses: false, pForce: true);
		}
	}

	public static void action_alien_mold(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addTrait("desire_alien_mold");
		}
	}

	public static void action_drop_computer(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addTrait("desire_computer");
		}
	}

	public static void action_drop_golden_egg(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addTrait("desire_golden_egg");
		}
	}

	public static void action_drop_harp(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addTrait("desire_harp");
		}
	}

	public static void action_madness(WorldTile pTile = null, string pDropID = null)
	{
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 3f))
		{
			item.addTrait("madness");
		}
	}

	public static void action_inspiration(WorldTile pTile, string pDropID = null)
	{
		if (pTile.zone.hasCity() && !World.world.cities.isLocked())
		{
			City tCity = pTile.zone_city;
			if (!tCity.isNeutral() && tCity.kingdom.countCities() != 1 && !tCity.isCapitalCity() && tCity.hasLeader())
			{
				tCity.leader.addStatusEffect("voices_in_my_head");
				tCity.useInspire(tCity.leader);
			}
		}
	}

	public static void action_discord(WorldTile pTile, string pDropID = null)
	{
		if (!pTile.zone.hasCity())
		{
			return;
		}
		City tCity = pTile.zone_city;
		if (tCity != null && !tCity.isNeutral())
		{
			Alliance tAlliance = tCity.kingdom.getAlliance();
			if (tAlliance != null)
			{
				World.world.alliances.useDiscordPower(tAlliance, tCity);
			}
		}
	}

	public static void action_spite(WorldTile pTile, string pDropID = null)
	{
		if (pTile.zone.hasCity())
		{
			Kingdom tKingdom = pTile.zone.city.kingdom;
			if (!tKingdom.isNeutral())
			{
				World.world.diplomacy.eventSpite(tKingdom);
			}
		}
	}

	public static void action_friendship(WorldTile pTile, string pDropID = null)
	{
		if (pTile.zone.hasCity())
		{
			Kingdom tKingdom = pTile.zone.city.kingdom;
			if (!tKingdom.isNeutral())
			{
				World.world.diplomacy.eventFriendship(tKingdom);
			}
		}
	}

	public static void action_spawn_building(WorldTile pTile = null, string pDropID = null)
	{
		string tBuildingAssetID = AssetManager.drops.get(pDropID).getRandomBuildingAsset();
		BuildingAsset tBuildingAsset = AssetManager.buildings.get(tBuildingAssetID);
		Building tNewBuilding = World.world.buildings.addBuilding(tBuildingAssetID, pTile, pCheckForBuild: true);
		if (tNewBuilding == null)
		{
			EffectsLibrary.spawnAtTile("fx_bad_place", pTile, 0.25f);
		}
		else
		{
			tBuildingAsset.checkLimits(tNewBuilding);
		}
	}

	public static void flash(WorldTile pTile, string pDropID)
	{
		World.world.flash_effects.flashPixel(pTile, 20);
	}

	public static void action_fertilizer_plants(WorldTile pTile = null, string pDropID = null)
	{
		BuildingActions.tryGrowVegetationRandom(pTile, VegetationType.Plants, pOnStart: false, pCheckLimit: false, pCheckRandom: false);
		if (pTile.Type.biome_asset != null && pTile.Type.biome_asset.grow_type_selector_plants == null)
		{
			EffectsLibrary.spawnAtTile("fx_bad_place", pTile, 0.25f);
		}
	}

	public static void action_fertilizer_trees(WorldTile pTile = null, string pDropID = null)
	{
		BiomeAsset tBiomeAsset = pTile.Type.biome_asset;
		BuildingActions.tryGrowVegetationRandom(pTile, VegetationType.Trees, pOnStart: false, pCheckLimit: false, pCheckRandom: false);
		if (tBiomeAsset != null && tBiomeAsset.grow_type_selector_trees == null)
		{
			EffectsLibrary.spawnAtTile("fx_bad_place", pTile, 0.25f);
		}
	}

	public static void action_fruit_bush(WorldTile pTile = null, string pDropID = null)
	{
		BuildingAsset tBuildingAsset = AssetManager.buildings.get("fruit_bush");
		BuildingActions.tryGrowVegetation(pTile, tBuildingAsset.id, pSfx: true, pCheckLimit: false);
		if (!tBuildingAsset.isOverlaysBiomeTags(pTile.Type))
		{
			EffectsLibrary.spawnAtTile("fx_bad_place", pTile, 0.25f);
		}
	}

	public static void action_landmine(WorldTile pTile = null, string pDropID = null)
	{
		if (pTile.Type.lava)
		{
			World.world.explosion_layer.explodeBomb(pTile);
		}
		else
		{
			MapAction.terraformTop(pTile, TopTileLibrary.landmine, TerraformLibrary.remove);
		}
	}

	public static void action_living_house(WorldTile pTile = null, string pDropID = null)
	{
		TileZone tZone = pTile.zone;
		if (!tZone.hasAnyBuildings())
		{
			return;
		}
		using ListPool<Building> tTempPool = new ListPool<Building>();
		if (tZone.hasAnyBuildingsInSet(BuildingList.Civs))
		{
			tTempPool.AddRange(tZone.getHashset(BuildingList.Civs));
		}
		if (tZone.hasAnyBuildingsInSet(BuildingList.Ruins))
		{
			tTempPool.AddRange(tZone.getHashset(BuildingList.Ruins));
		}
		if (tZone.hasAnyBuildingsInSet(BuildingList.Abandoned))
		{
			tTempPool.AddRange(tZone.getHashset(BuildingList.Abandoned));
		}
		for (int i = 0; i < tTempPool.Count; i++)
		{
			ActionLibrary.tryToMakeBuildingAlive(tTempPool[i]);
		}
	}

	public static void action_living_plants(WorldTile pTile = null, string pDropID = null)
	{
		TileZone tZone = pTile.zone;
		if (!tZone.hasAnyBuildings())
		{
			return;
		}
		using ListPool<Building> tTempPool = new ListPool<Building>();
		if (tZone.hasAnyBuildingsInSet(BuildingList.Food))
		{
			tTempPool.AddRange(tZone.getHashset(BuildingList.Food));
		}
		if (tZone.hasAnyBuildingsInSet(BuildingList.Trees))
		{
			tTempPool.AddRange(tZone.getHashset(BuildingList.Trees));
		}
		if (tZone.hasAnyBuildingsInSet(BuildingList.Wheat))
		{
			tTempPool.AddRange(tZone.getHashset(BuildingList.Wheat));
		}
		for (int i = 0; i < tTempPool.Count; i++)
		{
			ActionLibrary.tryToMakeFloraAlive(tTempPool[i]);
		}
	}
}
