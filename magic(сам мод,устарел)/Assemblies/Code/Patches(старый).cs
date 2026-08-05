using ReflectionUtility;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NCMS;
using NCMS.Utils;
using Newtonsoft.Json;
using ai;
using ai.behaviours;
using HarmonyLib;

namespace Magic
{

    class Patches : MonoBehaviour
    {
        public static Harmony harmony = new Harmony("androlg.mod.worldbox.magic");
        public static void init()
        {
                harmony.Patch(
                AccessTools.Method(typeof(ActorBase), "updateStats"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "updateStats_Prefix"))
            );
        }
        public static bool updateStats_Prefix(ActorBase __instance)
        {
            if (!__instance.statsDirty)
            {
                return false;
            }
            // __instance.updateStats();
            __instance.statsDirty = false;
            __instance.batch.c_stats_dirty.Remove(__instance.a);
            if (!__instance.isAlive())
            {
                return false;
            }
            __instance.checkColorSets();
            if (string.IsNullOrEmpty(__instance.data.mood))
            {
                __instance.data.mood = "normal";
            }
            MoodAsset moodAsset = AssetManager.moods.get(__instance.data.mood);
            __instance.stats.clear();
            __instance.stats.mergeStats(__instance.asset.base_stats);
            __instance.stats.mergeStats(moodAsset.base_stats);
            BaseStats baseStats = __instance.stats;
            string text = S.diplomacy;
            baseStats[text] += (float)__instance.data.diplomacy;
            //baseStats = __instance.stats;
            text = S.stewardship;
            baseStats[text] += (float)__instance.data.stewardship;
            //baseStats = __instance.stats;
            text = S.intelligence;
            baseStats[text] += (float)__instance.data.intelligence;
            //baseStats = __instance.stats;
            text = S.warfare;
            baseStats[text] += (float)__instance.data.warfare;
            bool blood_magic = __instance.hasTrait("Blood Magic");//Patch on patch
            if (blood_magic)
            {
                if (!MagicTraits.bloodEnhancement.ContainsKey(__instance.a))
                {
                    MagicTraits.bloodEnhancement.Add(__instance.a,new bloodStats());
                }
                baseStats = __instance.stats;
                text = S.health;
                baseStats[text] += MagicTraits.bloodEnhancement[__instance.a].health;
                //baseStats = __instance.stats;
                text = S.armor;
                baseStats[text] += MagicTraits.bloodEnhancement[__instance.a].armor;
                //baseStats = __instance.stats;
                text = S.max_age;
                baseStats[text] += (float)MagicTraits.bloodEnhancement[__instance.a].max_age;
                //baseStats = __instance.stats;
                text = S.attack_speed;
                baseStats[text] += MagicTraits.bloodEnhancement[__instance.a].attack_speed;
                //baseStats = __instance.stats;
                text = S.speed;
                baseStats[text] += MagicTraits.bloodEnhancement[__instance.a].speed;
                //baseStats = __instance.stats;
                text = S.damage;
                baseStats[text] += MagicTraits.bloodEnhancement[__instance.a].damage;
            }
            if (__instance.hasAnyStatusEffect())
            {
                foreach (StatusEffectData statusEffectData in __instance.activeStatus_dict.Values)
                {
                    __instance.stats.mergeStats(statusEffectData.asset.base_stats);
                }
            }
            if (!__instance.hasWeapon())
            {
                ItemAsset itemAsset = AssetManager.items.get(__instance.asset.defaultAttack);
                if (itemAsset != null)
                {
                    __instance.stats.mergeStats(itemAsset.base_stats);
                }
            }
            __instance.s_attackType = __instance.getWeaponAsset().attackType;
            __instance.s_slashType = __instance.getWeaponAsset().path_slash_animation;
            __instance.dirty_sprite_item = true;
            for (int i = 0; i < __instance.data.traits.Count; i++)
            {
                string text2 = __instance.data.traits[i];
                ActorTrait actorTrait = AssetManager.traits.get(text2);
                if (actorTrait != null && (!actorTrait.only_active_on_era_flag || ((!actorTrait.era_active_moon || World.world_era.flag_moon) && (!actorTrait.era_active_night || World.world_era.overlay_darkness))))
                {
                    __instance.stats.mergeStats(actorTrait.base_stats);
                }
            }
            if (__instance.asset.unit)
            {
                __instance.s_personality = null;
                if ((__instance.kingdom != null && __instance.kingdom.isCiv() && __instance.isKing()) || (__instance.city != null && __instance.city.leader == __instance))
                {
                    string text3 = "balanced";
                    float num = __instance.stats[S.diplomacy];
                    if (__instance.stats[S.diplomacy] > __instance.stats[S.stewardship])
                    {
                        text3 = "diplomat";
                        num = __instance.stats[S.diplomacy];
                    }
                    else if (__instance.stats[S.diplomacy] < __instance.stats[S.stewardship])
                    {
                        text3 = "administrator";
                        num = __instance.stats[S.stewardship];
                    }
                    if (__instance.stats[S.warfare] > num)
                    {
                        text3 = "militarist";
                    }
                    __instance.s_personality = AssetManager.personalities.get(text3);
                    __instance.stats.mergeStats(__instance.s_personality.base_stats);
                }
            }
            Clan clan = __instance.getClan();
            if (clan != null && clan.bonus_stats != null)
            {
                __instance.stats.mergeStats(clan.bonus_stats.base_stats);
            }
            // baseStats = __instance.stats;
            text = S.health;
            baseStats[text] += (float)((__instance.data.level - 1) * 20);
            //baseStats = __instance.stats;
            text = S.damage;
            baseStats[text] += (float)((__instance.data.level - 1) / 2);
            //baseStats = __instance.stats;
            text = S.armor;
            baseStats[text] += (float)((__instance.data.level - 1) / 3);
            // baseStats = __instance.stats;
            text = S.attack_speed;
            baseStats[text] += (float)(__instance.data.level - 1);

            bool flag = __instance.hasTrait("madness");
            __instance.data.s_traits_ids.Clear();
            __instance.s_action_attack_target = null;
            List<ItemAsset> list = __instance.s_special_effect_items;
            if (list != null)
            {
                list.Clear();
            }
            Dictionary<ItemAsset, double> dictionary = __instance.s_special_effect_items_timers;
            if (dictionary != null)
            {
                dictionary.Clear();
            }
            List<ActorTrait> list2 = __instance.s_special_effect_traits;
            if (list2 != null)
            {
                list2.Clear();
            }
            Dictionary<ActorTrait, double> dictionary2 = __instance.s_special_effect_traits_timers;
            if (dictionary2 != null)
            {
                dictionary2.Clear();
            }
            foreach (string text4 in __instance.data.traits)
            {
                ActorTrait actorTrait2 = AssetManager.traits.get(text4);
                if (actorTrait2 != null)
                {
                    __instance.data.s_traits_ids.Add(text4);
                    if (actorTrait2.action_special_effect != null)
                    {
                        if (__instance.s_special_effect_traits == null)
                        {
                            __instance.s_special_effect_traits = new List<ActorTrait>();
                            __instance.s_special_effect_traits_timers = new Dictionary<ActorTrait, double>();
                        }
                        __instance.s_special_effect_traits.Add(actorTrait2);
                    }
                    if (actorTrait2.action_attack_target != null)
                    {
                        __instance.s_action_attack_target = (AttackAction)Delegate.Combine(__instance.s_action_attack_target, actorTrait2.action_attack_target);
                    }
                }
            }
            __instance.has_trait_light = __instance.hasTrait("light_lamp");
            __instance.has_trait_weightless = __instance.hasTrait("weightless");
            __instance.has_status_frozen = __instance.hasStatus("frozen");
            if (!__instance.hasWeapon())
            {
                ItemAsset weaponAsset = __instance.getWeaponAsset();
                __instance.addItemActions(weaponAsset);
                if (weaponAsset.item_modifiers != null)
                {
                    foreach (string text5 in weaponAsset.item_modifiers)
                    {
                        ItemAsset itemAsset2 = AssetManager.items_modifiers.get(text5);
                        if (itemAsset2 != null)
                        {
                            __instance.addItemActions(itemAsset2);
                        }
                    }
                }
            }
            if (__instance.asset.use_items)
            {
                List<ActorEquipmentSlot> list3 = ActorEquipment.getList(__instance.equipment);
                for (int j = 0; j < list3.Count; j++)
                {
                    ActorEquipmentSlot actorEquipmentSlot = list3[j];
                    if (actorEquipmentSlot.data != null)
                    {
                        ItemData itemData = actorEquipmentSlot.data;
                        ItemAsset itemAsset3 = AssetManager.items.get(itemData.id);
                        __instance.addItemActions(itemAsset3);
                        foreach (string text6 in itemData.modifiers)
                        {
                            ItemAsset itemAsset4 = AssetManager.items_modifiers.get(text6);
                            __instance.addItemActions(itemAsset4);
                        }
                    }
                }
            }
            bool flag2 = __instance.hasTrait("madness");
            
            if (__instance.s_special_effect_traits == null || __instance.s_special_effect_traits.Count == 0)
            {
                __instance.s_special_effect_traits = null;
                __instance.s_special_effect_traits_timers = null;
                __instance.batch.c_trait_effects.Remove(__instance.a);
            }
            else
            {
                __instance.batch.c_trait_effects.Add(__instance.a);
            }
            if (__instance.s_special_effect_items == null || __instance.s_special_effect_items.Count == 0)
            {
                __instance.s_special_effect_items = null;
                __instance.s_special_effect_items_timers = null;
                __instance.batch.c_item_effects.Remove(__instance.a);
            }
            else
            {
                __instance.batch.c_item_effects.Add(__instance.a);
            }
            if (flag2 != flag)
            {
                __instance.checkMadness(flag2);
            }
            __instance.has_trait_peaceful = __instance.hasTrait("peaceful");
            __instance.has_trait_fire_resistant = __instance.hasTrait("fire_proof");
            __instance.has_status_burning = __instance.hasStatus("burning");
            __instance.has_trait_madness = __instance.hasTrait("madness");
            if (__instance.asset.use_items)
            {
                List<ActorEquipmentSlot> list4 = ActorEquipment.getList(__instance.equipment);
                for (int k = 0; k < list4.Count; k++)
                {
                    ActorEquipmentSlot actorEquipmentSlot2 = list4[k];
                    if (actorEquipmentSlot2.data != null)
                    {
                        ItemTools.mergeStatsWithItem(__instance.stats, actorEquipmentSlot2.data, false);
                    }
                }
            }
            __instance.stats.normalize();
            __instance.stats.checkMods();
            if (__instance.event_full_heal)
            {
                __instance.event_full_heal = false;
                __instance.stats.normalize();
                __instance.data.health = __instance.getMaxHealth();
            }
            Culture culture = __instance.getCulture();
            if (culture != null)
            {
                baseStats = __instance.stats;
                text = S.damage;
                baseStats[text] += __instance.stats[S.damage] * culture.stats.bonus_damage.value;
                //baseStats = __instance.stats;
                text = S.armor;
                baseStats[text] += __instance.stats[S.armor] * culture.stats.bonus_armor.value;
                //baseStats = __instance.stats;
                text = S.max_age;
                baseStats[text] += (float)culture.getMaxAgeBonus();
            }
            if (__instance.kingdom != null)
            {
                //baseStats = __instance.stats;
                text = S.damage;
                baseStats[text] += __instance.stats[S.damage] * __instance.kingdom.stats.bonus_damage.value;
                //baseStats = __instance.stats;
                text = S.armor;
                baseStats[text] += __instance.stats[S.armor] * __instance.kingdom.stats.bonus_armor.value;
            }
            if (__instance.asset.unit)
            {
                __instance.calculateFertility();
            }
            //baseStats = __instance.stats;
            text = S.zone_range;
            baseStats[text] += (float)((int)(__instance.stats[S.stewardship] / 10f));
            //baseStats = __instance.stats;
            text = S.cities;
            baseStats[text] += (float)((int)__instance.stats[S.stewardship] / 6 + 1);
            //baseStats = __instance.stats;
            text = S.bonus_towers;
            baseStats[text] += (float)((int)(__instance.stats[S.warfare] / 10f));
            if (__instance.s_attackType == WeaponType.Range)
            {
                //baseStats = __instance.stats;
                text = S.range;
                baseStats[text] += __instance.stats[S.range] * World.world_era.range_weapons_mod;
            }
            __instance.attackTimer = 0f;
            __instance.stats.normalize();
            if (__instance.data.health > __instance.getMaxHealth())
            {
                __instance.data.health = __instance.getMaxHealth();
            }
            __instance.target_scale = __instance.stats[S.scale];
            __instance.s_attackSpeed_seconds = (300f - __instance.stats[S.attack_speed]) / (100f + __instance.stats[S.attack_speed]);
            WorldAction action_recalc_stats = __instance.asset.action_recalc_stats;
            if (action_recalc_stats == null)
            {
                return false;
            }
            action_recalc_stats(__instance, null);
            return false;
        }
    }
}