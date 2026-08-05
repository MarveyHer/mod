using System;
using System.Linq;
using System.Collections.Generic;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ReflectionUtility;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Beebyte.Obfuscator;
using HarmonyLib;

namespace Magic{
    class MagicRaces
    {
        public static void init(){

            var unit_demonic = AssetManager.actor_library.get("unit_demonic");
            if (unit_demonic != null)
            {
                unit_demonic.traits.Add("Defiler");
            }

            var unit_angel = AssetManager.actor_library.get("unit_angel");
            if (unit_angel != null)
            {
                unit_angel.traits.Add("Demon Fighter");
            }

            var unit_orc = AssetManager.actor_library.get("unit_orc");
            unit_orc.base_stats[S.opinion] -= 20;
            unit_orc.base_stats[S.fertility] += 1;
            unit_orc.base_stats[S.max_children] +=4;
            unit_orc.base_stats[S.scale] += 0.05f;
            unit_orc.base_stats[S.loyalty_traits] -= 100;

            var unit_elf = AssetManager.actor_library.get("unit_elf");
            unit_elf.base_stats[S.max_age] = 1000;
            unit_elf.base_stats[S.opinion] -= 5f;
            unit_elf.base_stats[S.fertility] -= 0.2f;
            unit_elf.base_stats[S.max_children] -= 2;
            unit_elf.base_stats[S.scale] += 0.02f;


            var unit_dwarf = AssetManager.actor_library.get("unit_dwarf");
            unit_dwarf.base_stats[S.max_age] = 200;
            unit_dwarf.base_stats[S.opinion] -= 3f;
            unit_dwarf.base_stats[S.fertility] -= 0.2f;
            unit_dwarf.base_stats[S.max_children] -= 2f;
            unit_dwarf.base_stats[S.scale] -= 0.03f;

            var unit_human = AssetManager.actor_library.get("unit_human");
            unit_human.base_stats[S.fertility] += 0.2f;
            unit_human.base_stats[S.diplomacy] += 5f;



            var vampire = AssetManager.actor_library.clone("unit_vampire", "unit_human");
            vampire.base_stats[S.max_age] = 1000;
            vampire.base_stats[S.max_children] = -200f;
            vampire.base_stats[S.fertility] = -10000f;
            vampire.base_stats[S.attack_speed] = 90f;
            vampire.base_stats[S.knockback_reduction] += 10f;
            vampire.setBaseStats(333, 30, 80, 5, 30, 92, 0);
            vampire.nameLocale = "Vampire";
            vampire.nameTemplate = "human_name";
            vampire.race = "vampire";
            vampire.icon = "Vampire";
            vampire.effect_teleport = "fx_teleport_red";
		    vampire.fmod_spawn = "event:/SFX/UNITS/Human/HumanSpawn";
		    vampire.fmod_attack = "event:/SFX/UNITS/Human/HumanAttack";
		    vampire.fmod_idle = "event:/SFX/UNITS/Human/HumanIdle";
		    vampire.fmod_death = "event:/SFX/UNITS/Human/HumanDeath";
            vampire.zombieID = "zombie";
            vampire.canTurnIntoZombie = false;
            vampire.canTurnIntoMush = false;
            vampire.has_soul = false;
            vampire.canBeKilledByDivineLight = true;
            vampire.canTurnIntoTumorMonster = false;
            vampire.can_turn_into_demon_in_age_of_chaos = false;
            vampire.canTurnIntoIceOne = false;
            vampire.disableJumpAnimation = true;
            vampire.body_separate_part_head = false;
            vampire.color = Toolbox.makeColor("#005E72");
            AssetManager.actor_library.CallMethod("addTrait", "evil");
            AssetManager.actor_library.CallMethod("addTrait", "bloodlust");
            AssetManager.actor_library.CallMethod("addTrait", "Vampire");
            AssetManager.actor_library.CallMethod("addTrait", "immortal");
            AssetManager.actor_library.CallMethod("addTrait", "venomous");
            AssetManager.actor_library.CallMethod("addTrait", "poison_immune");
            AssetManager.actor_library.CallMethod("addTrait", "nightchild");
            AssetManager.actor_library.CallMethod("addTrait", "strong_minded");
            AssetManager.actor_library.CallMethod("loadShadow", vampire);
            Localization.addLocalization(vampire.nameLocale, vampire.nameLocale);

            var babyvampire = AssetManager.actor_library.clone("baby_vampire", "unit_vampire");
            babyvampire.base_stats[S.speed] = 10f;
            babyvampire.animation_idle = "walk_3";
            babyvampire.growIntoID = "unit_vampire";
            babyvampire.canTurnIntoZombie = false;
            babyvampire.canTurnIntoMush = false;
            babyvampire.has_soul = false;
            babyvampire.canTurnIntoTumorMonster = false;
            babyvampire.can_turn_into_demon_in_age_of_chaos = false;
            babyvampire.canTurnIntoIceOne = false;
            babyvampire.body_separate_part_head = false;
            babyvampire.body_separate_part_hands = false;
            babyvampire.take_items = false;
            babyvampire.baby = true;
            babyvampire.disableJumpAnimation = true;
            babyvampire.color_sets = vampire.color_sets;
            AssetManager.actor_library.CallMethod("addTrait", "peaceful");
            AssetManager.actor_library.CallMethod("addTrait", "evil");
            AssetManager.actor_library.CallMethod("addTrait", "bloodlust");
            AssetManager.actor_library.CallMethod("addTrait", "Vampire");
            AssetManager.actor_library.CallMethod("addTrait", "immortal");
            AssetManager.actor_library.CallMethod("addTrait", "poison_immune");
            AssetManager.actor_library.CallMethod("addTrait", "nightchild");
            //AssetManager.actor_library.CallMethod("addTrait", "Кровопийца");
            AssetManager.actor_library.CallMethod("loadShadow", babyvampire);
        }
    }
}