using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using ReflectionUtility;
using HarmonyLib;
using System.Reflection;

namespace Magic
{
    class MagicUnitys
    {
      public static void init()
      {
        create_water_spirit();
        create_fire_spirit();
        create_earth_spirit();
        create_air_spirit();
        create_defile_demon();
        create_angel();
      }
      public static bool spawn_building(WorldTile pTile, string currentBuilding)
      {
        BuildingAsset buildingAsset = AssetManager.buildings.get(currentBuilding);
        Building newBuilding = null;
        if (!World.world.buildings.canBuildFrom(pTile, buildingAsset, (City) null))
        {
          //EffectsLibrary.spawnAtTile("fx_bad_place", pTile, 0.25f);
          return false;
        }
          
        if(pTile.building == null)
        {
          newBuilding = World.world.buildings.addBuilding(currentBuilding, pTile, true, false, BuildPlacingType.New);
          return true;
        }
        if (newBuilding == null )
        {
          foreach ( WorldTile neigh in pTile.neighboursAll)
          {
              if (neigh.building != null)
              {
                  return false;
              }
          }
          newBuilding = World.world.buildings.addBuilding(currentBuilding, pTile, true, false, BuildPlacingType.New);
          if (newBuilding == null )
          {
              return false;
          }
          return true;
        }
        // old: BuildingAsset.cityBuilding -> new: .city_building, verified BuildingAsset.cs
        if (newBuilding.asset.city_building && pTile.zone.city != null)
        {
          // old: City.addBuilding(Building) -> new: City.listBuilding(Building), verified City.cs (established fact)
          pTile.zone.city.listBuilding(newBuilding);
          // old: Building.retake() [internal, called setState(BuildingState.CivKingdom)] -> new: no retake() method exists,
          // call the same underlying state change directly, verified Building.cs (setState is private but accessible
          // via publicized assembly, see rule G) and old_game/Building.cs L637 for retake()'s original body
          newBuilding.setState(BuildingState.CivKingdom);
        }
        return true;
      }
      public static bool DemonSlayer(BaseSimObject pTarget, WorldTile pTile = null)
      {
        // old: Toolbox.randomChance -> new: Randy.randomChance (established fact)
        if(Randy.randomChance(0.4f) && pTarget.a.asset.id == "defile_demon")
        {
          if (Randy.randomChance(0.05f))
          {
            spawn_building( pTile,"Flame_Tower");
          }
          else if (Randy.randomChance(0.33f))
          {
            spawn_building( pTile,"barracks_demons");
          }
          else if (Randy.randomChance(0.5f))
          {
            spawn_building( pTile,"Flame_tower");
          }
          else
          {
            spawn_building(pTile,"HellKennel");
          }
        }
        if ( pTarget == null || !pTarget.isActor())
          return false;
        BaseSimObject attackedBy = pTarget.a.attackedBy;
        if (!(attackedBy != null) || !attackedBy.isActor() || !attackedBy.isAlive())
          return false;
        attackedBy.a.addTrait("Demon Fighter");
        attackedBy.a.addTrait("fire_proof");
        
        return true;
      }
        public static void create_defile_demon()
        {
          // old: SA.inner_demon -> the "inner_demon" content id no longer exists in the current game at all (checked
          // both ActorAssetLibrary.cs and full decompile, genuinely removed, not just renamed). The closest surviving
          // ancestor is the vanilla "demon" actor asset (old inner_demon was itself cloned from SA.demon in build 558,
          // see old_game/ActorAssetLibrary.cs L1808) - using it as the new clone base.
          var Defile_demon = AssetManager.actor_library.clone("defile_demon", "demon");
          // old: S.* stat ids -> bare strings, ids unchanged (established fact B)
          Defile_demon.base_stats["scale"] += 0.05f;
          Defile_demon.base_stats["speed"] = 45;
          Defile_demon.base_stats["armor"] = 97;
          // old: ActorAsset.race - field removed entirely in current game (no group/race equivalent exists on
          // ActorAsset, only BuildingAsset kept the renamed "group" field), TREBUET RUCHNOY PROVERKI: no replacement found
          // Defile_demon.race = "demon";
          Defile_demon.action_death += new  WorldAction(DemonSlayer);
          Defile_demon.base_stats["damage"] -= 50;
          // old: defaultWeapons (List<string>) -> new: default_weapons (string[]), verified ActorAsset.cs
          Defile_demon.default_weapons = List.Of<string>("evil_staff").ToArray();
          AssetManager.actor_library.add(Defile_demon);
          AssetManager.actor_library.CallMethod("addTrait", "Defiler");
          AssetManager.actor_library.CallMethod("addTrait", "Blood Magic"); 
          AssetManager.actor_library.CallMethod("addTrait", "S"); 
          AssetManager.actor_library.CallMethod("loadShadow", Defile_demon);

          var lowest_demon = AssetManager.actor_library.clone("lowest_defile_demon", "demon");
          // Defile_demon.race = "demon"; -- removed, see note above
          lowest_demon.base_stats["speed"] = 45;
          lowest_demon.default_weapons = List.Of<string>("spear", "sword", "bow").ToArray();
          // old: defaultWeaponsMaterial - field removed entirely, no equivalent found anywhere in current decompile,
          // TREBUET RUCHNOY PROVERKI: random weapon material tier for spawned equipment can't be set this way anymore
          // lowest_demon.defaultWeaponsMaterial = List.Of<string>("adamantine");
          AssetManager.actor_library.add(lowest_demon);
          AssetManager.actor_library.CallMethod("addTrait", "Defiler");
          AssetManager.actor_library.CallMethod("removeTrait", "burning_feet");
          AssetManager.actor_library.CallMethod("addTrait", "B"); 
          AssetManager.actor_library.CallMethod("loadShadow", lowest_demon);

          var hidden_demon = AssetManager.actor_library.clone("hidden_demon", "demon");
          hidden_demon.base_stats["health"] = 500f;
          // hidden_demon.race = "demonit"; -- removed, see note above
          AssetManager.actor_library.add(hidden_demon);
          AssetManager.actor_library.CallMethod("addTrait", "Defiler");
          AssetManager.actor_library.CallMethod("removeTrait", "burning_feet");
          AssetManager.actor_library.CallMethod("addTrait", "A");
          AssetManager.actor_library.CallMethod("addTrait", "hiddenEvil"); 
          AssetManager.actor_library.CallMethod("loadShadow", hidden_demon);

          // old: SA.dragon -> new: bare string "dragon", content id unchanged, verified ActorAssetLibrary.cs
          var fel_dragon = AssetManager.actor_library.clone("fel_dragon", "dragon");
          fel_dragon.base_stats["scale"] += 0.02f;
          fel_dragon.base_stats["armor"] = 30f;
          fel_dragon.base_stats["health"] = 6660f;
          //fel_dragon.action_death += new  WorldAction(DemonSlayer);
          // old: ActorAsset.kingdom -> new: .kingdom_id_wild, verified ActorAsset.cs/getDefaultKingdom()
          fel_dragon.kingdom_id_wild = "demons";
          fel_dragon.can_be_killed_by_divine_light = true;
          // fel_dragon.race = "dragons"; -- removed, see note above
          AssetManager.actor_library.add(fel_dragon);
          AssetManager.actor_library.CallMethod("addTrait", "Defiler");
          //AssetManager.actor_library.CallMethod("removeTrait", "burning_feet");
          AssetManager.actor_library.CallMethod("addTrait", "S"); 
          AssetManager.actor_library.CallMethod("loadShadow", fel_dragon);
          //AssetManager.actor_library.CallMethod("addTrait", "Blood Magic"); 

          // old: "_mob" template removed entirely, closest current equivalent for a simple non-breeding unit is
          // "$mob_no_genes$" (clones from $basic_unit_colored$, can_have_subspecies=false, no civilization kingdom,
          // disable_jump_animation=true), verified ActorAssetLibrary.cs initTemplates()
          var demonKing = AssetManager.actor_library.clone("demonKing", "$mob_no_genes$");
          // old: nameLocale -> new: name_locale, verified ActorAsset.cs (established fact)
          demonKing.name_locale = "demonKings";
          // old: nameTemplate -> new: name_template_unit, verified ActorAsset.cs (established fact)
          demonKing.name_template_unit = "demon_name";
          // demonKing.race = "demon"; -- removed, see note above
          demonKing.kingdom_id_wild = "demons";
          // old: zombieID -> new: zombie_id_internal, verified ActorAsset.cs (established fact)
          demonKing.zombie_id_internal = "zombie";
          // old: skeletonID -> new: skeleton_id, verified ActorAsset.cs
          demonKing.skeleton_id = "skeleton";
          // old: defaultAttack -> new: default_attack, verified ActorAsset.cs
          demonKing.default_attack = "evil_staff";
          //hellhound.defaultWeapons = List.Of<string>("white_staff");
          // old: animation_walk/animation_swim were single comma-joined strings -> new: string[], verified ActorAsset.cs,
          // splitting the same literal sequence to preserve identical frame order
          demonKing.animation_walk = "walk_0,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,".Split(',');
          demonKing.animation_swim = "walk_1,walk_2,walk_3".Split(',');
          // old: texture_path -> new: texture_id, verified ActorAsset.cs/ActorAssetLibrary.cs usage
          demonKing.texture_id = "demonKings";
          demonKing.icon = "demonKings";
          // old: job was a single string -> new: string[], verified ActorAsset.cs
          demonKing.job = new string[] { "evil_mage" };
          demonKing.effect_teleport = "fx_teleport_red";
          // old: attack_spells (List<string>) -> new: spell_ids (List<string>), verified ActorAsset.cs
          demonKing.spell_ids = List.Of<string>("teleportRandom");
          demonKing.color = Toolbox.makeColor("#8c160c", -1f);
          demonKing.base_stats["max_age"] = 10000;
          demonKing.base_stats["attack_speed"] = 40f;
          demonKing.base_stats["health"] = 666;
          demonKing.base_stats["speed"] = 66f;
          demonKing.base_stats["damage"] = -33f;
          demonKing.base_stats["scale"] += 0.09f;
          demonKing.base_stats["armor"] += 99f;
          demonKing.base_stats["knockback_reduction"] = 10f;
          demonKing.can_be_killed_by_divine_light = true;
          demonKing.can_be_killed_by_life_eraser = false;
          demonKing.ignored_by_infinity_coin = false;
          demonKing.disable_jump_animation = true;
          demonKing.can_be_moved_by_powers = false;
          demonKing.can_attack_buildings = true;
          demonKing.can_turn_into_zombie = false;
          demonKing.can_turn_into_mush = false;
          demonKing.can_turn_into_tumor = false;
          demonKing.hide_favorite_icon = false;
          demonKing.can_edit_traits = false;
          demonKing.very_high_flyer = true;
          demonKing.damaged_by_ocean = false;
          // old: damagedByRain - no non-subspecies equivalent exists anymore: Actor.isDamagedByRain() unconditionally
          // returns false when the actor has no subspecies (verified Actor.cs), and this unit intentionally has
          // can_have_subspecies=false ($mob_no_genes$ base). TREBUET RUCHNOY PROVERKI: original =true intent (rain
          // damage) can no longer be reproduced via ActorAsset for a non-breeding unit.
          // demonKing.damagedByRain = true;
          // old: action_liquid field removed from ActorAsset AND ActionLibrary.swimToIsland removed entirely,
          // TREBUET RUCHNOY PROVERKI: no current equivalent found for custom liquid-crossing behavior
          // demonKing.action_liquid = new WorldAction(ActionLibrary.swimToIsland);
          // old: landCreature -> new: force_land_creature, old: oceanCreature -> new: force_ocean_creature,
          // verified Actor.cs isWaterCreature()/mustAvoidGround() (closest surviving equivalents, semantics not 100% identical)
          demonKing.force_land_creature = true;
          demonKing.force_ocean_creature = false;
          // old: swampCreature - no equivalent found anywhere in current decompile, TREBUET RUCHNOY PROVERKI
          // demonKing.swampCreature = true;
          // old: dieOnGround - no equivalent found anywhere in current decompile, TREBUET RUCHNOY PROVERKI
          // demonKing.dieOnGround = false;
          demonKing.take_items = false;
          demonKing.use_items = false;
          // old: diet_meat - subspecies-only now (Actor.cs L3814: hasSubspecies() && subspecies.diet_meat), this unit
          // has no subspecies so diet checks never apply regardless - safe to drop, behavior preserved (was false anyway)
          // demonKing.diet_meat = true;
          demonKing.has_soul = false;
          // old: dieInLava -> new: die_in_lava, field still exists directly on ActorAsset, verified
          demonKing.die_in_lava = false;
          // old: needFood - Actor.needsFood() always returns false without subspecies (verified Actor.cs), matches
          // the mod's own intent here (=false), safe to drop
          // demonKing.needFood = false;
          demonKing.flying = false;
          //hellhound.action_death += new  WorldAction(DemonSlayer);
          AssetManager.actor_library.add(demonKing);
          AssetManager.actor_library.CallMethod("addTrait", "Defiler");
          AssetManager.actor_library.CallMethod("addTrait", "immortal");
          AssetManager.actor_library.CallMethod("addTrait", "SSS");
          AssetManager.actor_library.CallMethod("addTrait", "fire_proof");
          AssetManager.actor_library.CallMethod("addTrait", "burning_feet");
          AssetManager.actor_library.CallMethod("addTrait", "Regeneration of the Lizard");
          AssetManager.actor_library.CallMethod("addTrait", "fire_blood");
          AssetManager.actor_library.CallMethod("addTrait", "Blood Magic");
          AssetManager.actor_library.CallMethod("loadShadow", demonKing);
          Localization.addLocalization(demonKing.name_locale, demonKing.name_locale);
          
          var hellhound = AssetManager.actor_library.clone("hellhound", "$mob_no_genes$");
          hellhound.name_locale = "hellhound";
          hellhound.name_template_unit = "wolf_name";
          // hellhound.race = "demon"; -- removed, see note above
          hellhound.kingdom_id_wild = "demons";
          hellhound.zombie_id_internal = "zombie";
          hellhound.skeleton_id = "skeleton";
          //angel.defaultAttack = "white_staff";
          //hellhound.defaultWeapons = List.Of<string>("white_staff");
          hellhound.animation_walk = "walk_0,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2,walk_1,walk_2".Split(',');
          hellhound.animation_swim = "swim_0,swim_1,swim_2".Split(',');
          hellhound.texture_id = "hellhound";
          hellhound.icon = "hellhound";
          hellhound.job = new string[] { "move_mob" };
          hellhound.color = Toolbox.makeColor("#8c160c", -1f);
          hellhound.base_stats["max_age"] = 1000;
          hellhound.base_stats["attack_speed"] = 90f;
          hellhound.base_stats["health"] = 200;
          hellhound.base_stats["speed"] = 90f;
          hellhound.base_stats["damage"] = 60f;
          hellhound.base_stats["scale"] += 0.04f;
          hellhound.can_be_killed_by_divine_light = true;
          hellhound.can_be_killed_by_life_eraser = true;
          hellhound.ignored_by_infinity_coin = false;
          hellhound.disable_jump_animation = true;
          hellhound.can_be_moved_by_powers = true;
          hellhound.can_attack_buildings = true;
          hellhound.can_turn_into_zombie = false;
          hellhound.can_turn_into_mush = false;
          hellhound.can_turn_into_tumor = false;
          hellhound.hide_favorite_icon = false;
          hellhound.can_edit_traits = true;
          hellhound.very_high_flyer = false;
          hellhound.damaged_by_ocean = true;
          // hellhound.damagedByRain = true; -- removed, see TREBUET RUCHNOY PROVERKI note above
          // hellhound.action_liquid = new WorldAction(ActionLibrary.swimToIsland); -- removed, see note above
          hellhound.force_land_creature = true;
          hellhound.force_ocean_creature = false;
          // hellhound.swampCreature = true; -- removed, see note above
          // hellhound.dieOnGround = false; -- removed, see note above
          hellhound.take_items = false;
          hellhound.use_items = false;
          // hellhound.diet_meat = true; -- removed, subspecies-only now, see note above
          hellhound.has_soul = false;
          hellhound.die_in_lava = false;
          // hellhound.needFood = false; -- removed, always false without subspecies anyway
          hellhound.flying = false;
          //hellhound.action_death += new  WorldAction(DemonSlayer);
          AssetManager.actor_library.add(hellhound);
          AssetManager.actor_library.CallMethod("addTrait", "Defiler");
          AssetManager.actor_library.CallMethod("addTrait", "immortal");
          AssetManager.actor_library.CallMethod("addTrait", "C");
          AssetManager.actor_library.CallMethod("addTrait", "fire_proof");
          AssetManager.actor_library.CallMethod("addTrait", "regeneration");
          AssetManager.actor_library.CallMethod("addTrait", "fire_blood");
          AssetManager.actor_library.CallMethod("addTrait", "fast");
          AssetManager.actor_library.CallMethod("loadShadow", hellhound);
          Localization.addLocalization(hellhound.name_locale, hellhound.name_locale);
        }

        public static void create_angel()
        {
          var angel = AssetManager.actor_library.clone("angel", "$mob_no_genes$");
          angel.name_locale = "angel";
          angel.name_template_unit = "elf_name";
          // angel.race = "angel"; -- removed, see note above
          angel.kingdom_id_wild = "good";
          angel.zombie_id_internal = "zombie";
          angel.skeleton_id = "skeleton";
          angel.spell_ids = List.Of<string>(new string[]
          {
            "divine",
            "cure",
            "bloodRain",
            "shield"
          });
          angel.effect_cast_top = "fx_cast_top_blue";
          angel.effect_cast_ground = "fx_cast_ground_blue";
          //angel.defaultAttack = "white_staff";
          angel.default_weapons = List.Of<string>("white_staff").ToArray();
          angel.animation_walk = "walk_0,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3".Split(',');
          angel.animation_swim = "walk_1,walk_2,walk_3".Split(',');
          angel.texture_id = "angel";
          angel.icon = "angel";
          angel.job = new string[] { "move_mob" };
          angel.color = Toolbox.makeColor("#f0f227", -1f);
          angel.base_stats["max_age"] = 1000;
          angel.base_stats["attack_speed"] = 90f;
          angel.base_stats["health"] = 355;
          angel.base_stats["speed"] = 60f;
          angel.base_stats["damage"] = 1f;
          angel.can_be_killed_by_divine_light = false;
          angel.can_be_killed_by_life_eraser = true;
          angel.ignored_by_infinity_coin = false;
          angel.disable_jump_animation = true;
          angel.can_be_moved_by_powers = true;
          angel.can_attack_buildings = true;
          angel.can_turn_into_zombie = false;
          angel.can_turn_into_mush = false;
          angel.can_turn_into_tumor = false;
          angel.hide_favorite_icon = false;
          angel.can_edit_traits = true;
          angel.very_high_flyer = true;
          angel.damaged_by_ocean = false;
          // angel.damagedByRain = false; -- removed, matches default (false) without subspecies anyway
          // angel.action_liquid = new WorldAction(ActionLibrary.swimToIsland); -- removed, see note above
          angel.force_land_creature = true;
          angel.force_ocean_creature = false;
          // angel.swampCreature = true; -- removed, see note above
          // angel.dieOnGround = false; -- removed, see note above
          angel.take_items = false;
          angel.use_items = true;
          // angel.diet_meat = false; -- removed, subspecies-only now, matches default
          angel.has_soul = false;
          angel.die_in_lava = true;
          // angel.needFood = false; -- removed, matches default without subspecies
          angel.flying = false;
          AssetManager.actor_library.add(angel);
          AssetManager.actor_library.CallMethod("addTrait", "immortal"); 
          AssetManager.actor_library.CallMethod("addTrait", "freeze_proof");
          AssetManager.actor_library.CallMethod("addTrait", "A");
          AssetManager.actor_library.CallMethod("addTrait", "Demon Fighter");   
          AssetManager.actor_library.CallMethod("addTrait", "blessed"); 
          //AssetManager.actor_library.CallMethod("addTrait", "fire_proof"); 
          //AssetManager.actor_library.CallMethod("addTrait", "Spirit"); 
          //AssetManager.actor_library.CallMethod("addTrait", "poison_immune");
          //AssetManager.actor_library.CallMethod("addTrait", "C"); 
          AssetManager.actor_library.CallMethod("loadShadow", angel);
          Localization.addLocalization(angel.name_locale, angel.name_locale);
        }
        public static void create_water_spirit()
        {
        var water_spirit = AssetManager.actor_library.clone("water_spirit", "$mob_no_genes$");
        water_spirit.name_locale = "water_spirit";
        water_spirit.name_template_unit = "phoenix_name";
        // water_spirit.race = "spirit"; -- removed, see note above
        water_spirit.kingdom_id_wild = "spirit";
        water_spirit.zombie_id_internal = "zombie";
        water_spirit.skeleton_id = "skeleton";
        water_spirit.spell_ids = List.Of<string>(new string[]
		    {
        "rain",
        "spiritInitiation",
        "cure",
        "bloodRain",
        "shield"
		    });
        water_spirit.effect_cast_top = "fx_cast_top_green";
		    water_spirit.effect_cast_ground = "fx_cast_ground_green";
        water_spirit.default_attack = "white_staff";
        water_spirit.animation_walk = "walk_0,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3".Split(',');
        water_spirit.animation_swim = "walk_1,walk_2,walk_3".Split(',');
        water_spirit.texture_id = "water_spirit";
        water_spirit.icon = "water_spirit";
        water_spirit.job = new string[] { "move_mob" };
        water_spirit.color = Toolbox.makeColor("#8b3aee", -1f);
        water_spirit.base_stats["max_age"] = 1000;
        water_spirit.base_stats["attack_speed"] = 40f;
        water_spirit.base_stats["health"] = 300;
        water_spirit.base_stats["speed"] = 100f;
        water_spirit.base_stats["damage"] = 1f;
        water_spirit.can_be_killed_by_divine_light = true;
        water_spirit.can_be_killed_by_life_eraser = true;
        water_spirit.ignored_by_infinity_coin = false;
        water_spirit.disable_jump_animation = true;
        water_spirit.can_be_moved_by_powers = true;
        water_spirit.can_attack_buildings = true;
        water_spirit.can_turn_into_zombie = false;
        water_spirit.can_turn_into_mush = false;
        water_spirit.can_turn_into_tumor = false;
        water_spirit.hide_favorite_icon = false;
        water_spirit.can_edit_traits = true;
        water_spirit.very_high_flyer = true;
        water_spirit.damaged_by_ocean = false;
        // water_spirit.damagedByRain = false; -- removed, matches default without subspecies
        // water_spirit.action_liquid = new WorldAction(ActionLibrary.swimToIsland); -- removed, see note above
        water_spirit.force_land_creature = true;
        water_spirit.force_ocean_creature = true;
        // water_spirit.swampCreature = true; -- removed, see note above
        // water_spirit.dieOnGround = false; -- removed, see note above
        water_spirit.take_items = false;
        water_spirit.use_items = false;
        // water_spirit.diet_meat = false; -- removed, subspecies-only now, matches default
        water_spirit.has_soul = false;
		    water_spirit.die_in_lava = true;
        // water_spirit.needFood = false; -- removed, matches default without subspecies
        water_spirit.flying = false;
        AssetManager.actor_library.add(water_spirit);
        AssetManager.actor_library.CallMethod("addTrait", "immortal"); 
        AssetManager.actor_library.CallMethod("addTrait", "freeze_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "fire_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "Spirit"); 
        AssetManager.actor_library.CallMethod("addTrait", "poison_immune");
        AssetManager.actor_library.CallMethod("addTrait", "C"); 
        AssetManager.actor_library.CallMethod("loadShadow", water_spirit);
        Localization.addLocalization(water_spirit.name_locale, water_spirit.name_locale);
        }

        public static void create_fire_spirit()
        {
        var Fire_spirit = AssetManager.actor_library.clone("Fire_spirit", "$mob_no_genes$");
        Fire_spirit.name_locale = "Fire_spirit";
        Fire_spirit.name_template_unit = "phoenix_name";
        // Fire_spirit.race = "Fire_spirit"; -- removed, see note above
        Fire_spirit.kingdom_id_wild = "spirit";
        Fire_spirit.zombie_id_internal = "zombie";
        Fire_spirit.skeleton_id = "skeleton";
        Fire_spirit.spell_ids = List.Of<string>(new string[]
		    {
			  "fire",
        "spiritInitiation",
        "shield",
        "lava",
        "bloodRain",
        //"Rain"
		    });
        Fire_spirit.effect_cast_top = "fx_cast_top_red";
		    Fire_spirit.effect_cast_ground = "fx_cast_ground_red";
        Fire_spirit.default_attack = "FireSpiritEffect";
        Fire_spirit.animation_walk = "walk_0,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3".Split(',');
        Fire_spirit.animation_swim = "walk_1,walk_2,walk_3".Split(',');
        Fire_spirit.texture_id = "Fire_spirit";
        Fire_spirit.icon = "Fire_spirit";
        Fire_spirit.job = new string[] { "move_mob" };
        Fire_spirit.color = Toolbox.makeColor("#8b3aee", -1f);
        Fire_spirit.base_stats["max_age"] = 1000;
        Fire_spirit.base_stats["attack_speed"] = 40f;
        Fire_spirit.base_stats["health"] = 200;
        Fire_spirit.base_stats["speed"] = 100f;
        Fire_spirit.base_stats["damage"] = 20f;
        Fire_spirit.can_be_killed_by_divine_light = true;
        Fire_spirit.can_be_killed_by_life_eraser = true;
        Fire_spirit.ignored_by_infinity_coin = false;
        Fire_spirit.disable_jump_animation = true;
        Fire_spirit.can_be_moved_by_powers = true;
        Fire_spirit.can_attack_buildings = true;
        Fire_spirit.can_turn_into_zombie = false;
        Fire_spirit.can_turn_into_mush = false;
        Fire_spirit.can_turn_into_tumor = false;
        Fire_spirit.hide_favorite_icon = false;
        Fire_spirit.can_edit_traits = true;
        Fire_spirit.very_high_flyer = true;
        Fire_spirit.damaged_by_ocean = true;
        // Fire_spirit.swampCreature = true; -- removed, see note above
        // Fire_spirit.damagedByRain = true; -- removed, TREBUET RUCHNOY PROVERKI (original intent was =true), see note above
        Fire_spirit.force_ocean_creature = false;
        // Fire_spirit.action_liquid = new WorldAction(ActionLibrary.swimToIsland); -- removed, see note above
        Fire_spirit.force_land_creature = true;
        // Fire_spirit.dieOnGround = false; -- removed, see note above
        Fire_spirit.take_items = false;
        Fire_spirit.use_items = false;
        // Fire_spirit.diet_meat = false; -- removed, subspecies-only now, matches default
        Fire_spirit.has_soul = false;
		    Fire_spirit.die_in_lava = false;
        // Fire_spirit.needFood = false; -- removed, matches default without subspecies
        Fire_spirit.flying = false;
        Fire_spirit.disable_jump_animation = true;
        AssetManager.actor_library.add(Fire_spirit);
        AssetManager.actor_library.CallMethod("addTrait", "immortal"); 
        AssetManager.actor_library.CallMethod("addTrait", "freeze_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "fire_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "Spirit"); 
        AssetManager.actor_library.CallMethod("addTrait", "poison_immune");
        AssetManager.actor_library.CallMethod("addTrait", "C"); 
        AssetManager.actor_library.CallMethod("loadShadow", Fire_spirit);
        Localization.addLocalization(Fire_spirit.name_locale, Fire_spirit.name_locale);
        }

        public static void create_earth_spirit()
        {
        var earth_spirit = AssetManager.actor_library.clone("earth_spirit", "$mob_no_genes$");
        earth_spirit.name_locale = "earth_spirit";
        earth_spirit.name_template_unit = "phoenix_name";
        // earth_spirit.race = "spirit"; -- removed, see note above
        earth_spirit.kingdom_id_wild = "spirit";
        earth_spirit.zombie_id_internal = "zombie";
        earth_spirit.skeleton_id = "skeleton";
        earth_spirit.spell_ids = List.Of<string>(new string[]
		    {
        "spiritInitiation",
        "Earthquake",
        "spawnFertilizer",
        "shield",
        //"bloodRain",
        "lava",
		    });
        earth_spirit.effect_cast_top = "fx_cast_top_green";
		    earth_spirit.effect_cast_ground = "fx_cast_ground_green";
        earth_spirit.default_attack = "druid_staff";
        earth_spirit.animation_walk = "walk_0,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3".Split(',');
        earth_spirit.animation_swim = "walk_1,walk_2,walk_3".Split(',');
        earth_spirit.texture_id = "earth_spirit";
        earth_spirit.icon = "earth_spirit";
        earth_spirit.job = new string[] { "move_mob" };
        earth_spirit.color = Toolbox.makeColor("#8b3aee", -1f);
        earth_spirit.base_stats["max_age"] = 1000;
        earth_spirit.base_stats["attack_speed"] = 40f;
        earth_spirit.base_stats["health"] = 500;
        earth_spirit.base_stats["speed"] = 30f;
        earth_spirit.base_stats["armor"] = 50f;
        earth_spirit.base_stats["damage"] = 1f;
        earth_spirit.can_be_killed_by_divine_light = true;
        earth_spirit.can_be_killed_by_life_eraser = true;
        earth_spirit.ignored_by_infinity_coin = false;
        earth_spirit.disable_jump_animation = true;
        earth_spirit.can_be_moved_by_powers = true;
        earth_spirit.can_attack_buildings = true;
        earth_spirit.can_turn_into_zombie = false;
        earth_spirit.can_turn_into_mush = false;
        earth_spirit.can_turn_into_tumor = false;
        earth_spirit.hide_favorite_icon = false;
        earth_spirit.can_edit_traits = true;
        earth_spirit.very_high_flyer = true;
        earth_spirit.damaged_by_ocean = true;
        // earth_spirit.swampCreature = true; -- removed, see note above
        // earth_spirit.damagedByRain = false; -- removed, matches default without subspecies
        earth_spirit.force_ocean_creature = false;
        // earth_spirit.action_liquid = new WorldAction(ActionLibrary.swimToIsland); -- removed, see note above
        earth_spirit.force_land_creature = true;
        // earth_spirit.dieOnGround = false; -- removed, see note above
        earth_spirit.take_items = false;
        earth_spirit.use_items = false;
        // earth_spirit.diet_meat = false; -- removed, subspecies-only now, matches default
        earth_spirit.has_soul = false;
		    earth_spirit.die_in_lava = false;
        // earth_spirit.needFood = false; -- removed, matches default without subspecies
        earth_spirit.flying = false;
        earth_spirit.disable_jump_animation = true;
        AssetManager.actor_library.add(earth_spirit);
        AssetManager.actor_library.CallMethod("addTrait", "immortal"); 
        AssetManager.actor_library.CallMethod("addTrait", "flower_prints"); 
        AssetManager.actor_library.CallMethod("addTrait", "freeze_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "fire_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "Spirit"); 
        AssetManager.actor_library.CallMethod("addTrait", "poison_immune");
        AssetManager.actor_library.CallMethod("addTrait", "C"); 
        AssetManager.actor_library.CallMethod("loadShadow", earth_spirit);
        Localization.addLocalization(earth_spirit.name_locale, earth_spirit.name_locale);
        }

        public static void create_air_spirit()
        {
        var air_spirit = AssetManager.actor_library.clone("air_spirit", "$mob_no_genes$");
        air_spirit.name_locale = "air_spirit";
        air_spirit.name_template_unit = "phoenix_name";
        // air_spirit.race = "spirit"; -- removed, see note above
        air_spirit.kingdom_id_wild = "spirit";
        air_spirit.zombie_id_internal = "zombie";
        air_spirit.skeleton_id = "skeleton";
        air_spirit.spell_ids = List.Of<string>(new string[]
		    {
        "spiritInitiation",
        "tornado",
        "lightning",
        "bloodRain",
        "tornado",
		    });
        air_spirit.effect_cast_top = "fx_cast_top_blue";
		    air_spirit.effect_cast_ground = "fx_cast_ground_blue";
        air_spirit.default_attack = "AirSpiritEffect";
        air_spirit.animation_walk = "walk_0,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3,walk_1,walk_2,walk_3".Split(',');
        air_spirit.animation_swim = "walk_1,walk_2,walk_3".Split(',');
        air_spirit.texture_id = "air_spirit";
        air_spirit.icon = "air_spirit";
        air_spirit.job = new string[] { "move_mob" };
        air_spirit.color = Toolbox.makeColor("#8b3aee", -1f);
        air_spirit.base_stats["max_age"] = 1000;
        air_spirit.base_stats["attack_speed"] = 80f;
        air_spirit.base_stats["health"] = 150;
        air_spirit.base_stats["speed"] = 90f;
        //air_spirit.base_stats["armor"] = 50f;
        air_spirit.base_stats["damage"] = 30f;
        air_spirit.can_be_killed_by_divine_light = true;
        air_spirit.can_be_killed_by_life_eraser = true;
        air_spirit.ignored_by_infinity_coin = false;
        air_spirit.disable_jump_animation = true;
        air_spirit.can_be_moved_by_powers = true;
        air_spirit.can_attack_buildings = true;
        air_spirit.can_turn_into_zombie = false;
        air_spirit.can_turn_into_mush = false;
        air_spirit.can_turn_into_tumor = false;
        air_spirit.hide_favorite_icon = false;
        air_spirit.can_edit_traits = true;
        air_spirit.very_high_flyer = true;
        air_spirit.damaged_by_ocean = false;
        // air_spirit.swampCreature = true; -- removed, see note above
        // air_spirit.damagedByRain = false; -- removed, matches default without subspecies
        air_spirit.force_ocean_creature = true;
        // air_spirit.action_liquid = new WorldAction(ActionLibrary.swimToIsland); -- removed, see note above
        air_spirit.force_land_creature = true;
        // air_spirit.dieOnGround = false; -- removed, see note above
        air_spirit.take_items = false;
        air_spirit.use_items = false;
        // air_spirit.diet_meat = false; -- removed, subspecies-only now, matches default
        air_spirit.has_soul = false;
		    air_spirit.die_in_lava = false;
        // air_spirit.needFood = false; -- removed, matches default without subspecies
        air_spirit.flying = false;
        AssetManager.actor_library.add(air_spirit);
        AssetManager.actor_library.CallMethod("addTrait", "immortal"); 
        //AssetManager.actor_library.CallMethod("addTrait", "whirlwind");
        AssetManager.actor_library.CallMethod("addTrait", "freeze_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "fire_proof"); 
        AssetManager.actor_library.CallMethod("addTrait", "Spirit"); 
        AssetManager.actor_library.CallMethod("addTrait", "poison_immune");
        AssetManager.actor_library.CallMethod("addTrait", "C"); 
        AssetManager.actor_library.CallMethod("loadShadow", air_spirit);
        Localization.addLocalization(air_spirit.name_locale, air_spirit.name_locale);
        }

        
       
    }
}
