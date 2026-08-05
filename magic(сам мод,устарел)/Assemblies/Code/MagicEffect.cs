using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using ReflectionUtility;
using HarmonyLib;
using ai;
using System.Reflection;
using Newtonsoft.Json;
namespace Magic
{
    class MagicEffect
    {
        public static void init()
        {
            StatusEffect burning = AssetManager.status.get("burning");
            burning.opposite_traits.Add("Fire Magic");

            StatusEffect cough = AssetManager.status.get("cough");
            cough.opposite_traits.Add("The Magic of Life");
            cough.opposite_traits.Add("The Magic of Death");

            StatusEffect ash_fever = AssetManager.status.get("ash_fever");
            ash_fever.opposite_traits.Add("The Magic of Life");
            cough.opposite_traits.Add("The Magic of Death");

            StatusEffect fireEnhancement = new StatusEffect();
            fireEnhancement.id = "fireEnhancement";
            fireEnhancement.duration = 20.0f;
            fireEnhancement.base_stats[S.armor] += 10f;
            //fireEnhancement.base_stats[S.attack_speed] += 80f;
            fireEnhancement.base_stats[S.mod_damage] += 0.5f;
            fireEnhancement.base_stats[S.damage] += 100f;
            //fireEnhancement.base_stats[S.knockback_reduction] += 100.0f;
            fireEnhancement.animated = false;
            fireEnhancement.path_icon = "ui/effect_icons/fireEnhancement";
            fireEnhancement.action = new WorldAction(sparksOfFire);
            fireEnhancement.action_get_hit = new GetHitAction(burningAction);
            fireEnhancement.description = "status_description_fireEnhancement";
            fireEnhancement.name = "status_title_fireEnhancement";
            addTraitToLocalizedLibrary("ru", fireEnhancement.name, fireEnhancement.description, "Усиление Огня", "Огонь пожирает его врагов");
            addTraitToLocalizedLibrary("en", fireEnhancement.name, fireEnhancement.description, "Fire Enhancement", "The fire devours his enemies");
            AssetManager.status.add(fireEnhancement);

            StatusEffect waterEnhancement = new StatusEffect();
            waterEnhancement.id = "waterEnhancement";
            waterEnhancement.duration = 60.0f;
            waterEnhancement.base_stats[S.health] += 100f;
            //waterEnhancement.base_stats[S.attack_speed] += 80f;
            waterEnhancement.base_stats[S.mod_health] += 0.5f;
            waterEnhancement.base_stats[S.armor] += 5f;
            //fireEnhancement.base_stats[S.knockback_reduction] += 100.0f;
            waterEnhancement.animated = false;
            waterEnhancement.path_icon = "ui/effect_icons/waterEnhancement";
            waterEnhancement.action = new WorldAction(waterEnhancementAction);
            //waterEnhancement.action_get_hit = new GetHitAction(burningAction);
            waterEnhancement.description = "status_description_waterEnhancement";
            waterEnhancement.name = "status_title_waterEnhancement";
            addTraitToLocalizedLibrary("ru", waterEnhancement.name, waterEnhancement.description, "Усиление Воды", "Вода питает его жизнь");
            addTraitToLocalizedLibrary("en", waterEnhancement.name, waterEnhancement.description, "Water Enhancement", "Water feeds his life");
            AssetManager.status.add(waterEnhancement);

            StatusEffect airEnhancement = new StatusEffect();
            airEnhancement.id = "airEnhancement";
            airEnhancement.duration = 30.0f;
            airEnhancement.base_stats[S.speed] += 100f;
            airEnhancement.base_stats[S.attack_speed] += 80f;
            airEnhancement.base_stats[S.mod_speed] += 0.5f;
            airEnhancement.base_stats[S.armor] += 3f;
            airEnhancement.base_stats[S.dodge] += 5f;
            airEnhancement.base_stats[S.knockback_reduction] -= 30f;
            //fireEnhancement.base_stats[S.knockback_reduction] += 100.0f;
            airEnhancement.animated = false;
            airEnhancement.path_icon = "ui/effect_icons/airEnhancement";
            //airEnhancement.action = new WorldAction(airEnhancementAction);
            //waterEnhancement.action_get_hit = new GetHitAction(burningAction);
            airEnhancement.description = "status_description_airEnhancement";
            airEnhancement.name = "status_title_airEnhancement";
            addTraitToLocalizedLibrary("ru", airEnhancement.name, airEnhancement.description, "Усиление Воздуха", "Воздух делает жизнь легче");
            addTraitToLocalizedLibrary("en", airEnhancement.name, airEnhancement.description, "Air Enhancement", "Air makes life easier");
            AssetManager.status.add(airEnhancement);

            StatusEffect earthEnhancement = new StatusEffect();
            earthEnhancement.id = "earthEnhancement";
            earthEnhancement.duration = 60.0f;
            earthEnhancement.base_stats[S.speed] -= 20f;
            earthEnhancement.base_stats[S.attack_speed] -= 10f;
            //earthEnhancement.base_stats[S.mod_speed] += 0.5f;
            earthEnhancement.base_stats[S.armor] += 70f;
            earthEnhancement.base_stats[S.knockback_reduction] += 100f;
            //fireEnhancement.base_stats[S.knockback_reduction] += 100.0f;
            earthEnhancement.animated = false;
            earthEnhancement.path_icon = "ui/effect_icons/earthEnhancement";
            //earthEnhancement.action = new WorldAction(earthEnhancementAction);
            //waterEnhancement.action_get_hit = new GetHitAction(burningAction);
            earthEnhancement.description = "status_description_earthEnhancement";
            earthEnhancement.name = "status_title_earthEnhancement";
            addTraitToLocalizedLibrary("ru", earthEnhancement.name, earthEnhancement.description, "Усиление Земли", "Земля защищает свое дитя");
            addTraitToLocalizedLibrary("en", earthEnhancement.name, earthEnhancement.description, "Earth Enhancement", "The earth protects its child");
            AssetManager.status.add(earthEnhancement);

            StatusEffect curse = new StatusEffect();
            curse.id = "curse";
            curse.duration = 60.0f;
            curse.base_stats[S.mod_speed] -= 10f;
            curse.base_stats[S.attack_speed] -= 60f;
            //earthEnhancement.base_stats[S.mod_speed] += 0.5f;
            curse.base_stats[S.armor] -= 50f;
            curse.base_stats[S.mod_health] -= 0.6f;
            curse.base_stats[S.mod_damage] -= 1f;
            curse.base_stats[S.knockback_reduction] += 100f;
            curse.base_stats[S.max_age] -= 30f;
            //fireEnhancement.base_stats[S.knockback_reduction] += 100.0f;
            curse.animated = false;
            curse.path_icon = "ui/effect_icons/curse";
            //earthEnhancement.action = new WorldAction(earthEnhancementAction);
            //waterEnhancement.action_get_hit = new GetHitAction(burningAction);
            curse.description = "status_description_curse";
            curse.name = "status_title_curse";
            addTraitToLocalizedLibrary("ru", curse.name, curse.description, "Проклятье Смерти", "Она тянет к нему свои руки");
            addTraitToLocalizedLibrary("en", curse.name, curse.description, "The Curse of Death", "She holds out her hands to him");

            AssetManager.status.add(curse);

            loadCustomEffect();
        }
        private static bool waterEnhancementAction(BaseSimObject pTarget, WorldTile pTile)
        {
            pTarget.a.restoreHealth(30);
            pTarget.a.finishStatusEffect("burning");
            return true;
        }
        private static bool sparksOfFire(BaseSimObject pTarget, WorldTile pTile)
        {
            pTarget.a.spawnParticle(Toolbox.makeColor("#D95032"));
            //pTarget.a.startShake(0.4f, 0.2f, true, false);
            pTarget.a.spawnParticle(Toolbox.makeColor("#F27F3D"));
            pTarget.a.spawnParticle(Toolbox.makeColor("#F2A444"));
            pTarget.a.spawnParticle(Toolbox.makeColor("#F2C36B"));
            pTarget.a.spawnParticle(Toolbox.makeColor("#F2CA50"));
            pTarget.a.spawnParticle(Toolbox.makeColor("#E35632"));
            pTarget.a.spawnParticle(Toolbox.makeColor("#EEB543"));
            return true;
        }

        private static bool burningAction(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
        {
            if (pTarget != null && pTarget.isActor())
                pTarget.a.addStatusEffect("burning");
            return true;
        }
        public static void loadCustomEffect()
        {
            //please, this took me a whole day and the entire modder team to help me with this
            var effect = AssetManager.effects_library.add(new EffectAsset
            {
                id = "fx_YOYO_effect",
                use_basic_prefab = true,
                show_on_mini_map = true,
                sprite_path = "effects/antimatterEffect",
                sound_launch = "event:/SFX/EXPLOSIONS/ExplosionAntimatterBomb",
                sorting_layer_id = "Objects"
            });
            World.world.stackEffects.CallMethod("add", effect);

            var effectCustomEffect = AssetManager.effects_library.add(new EffectAsset
            {
                id = "fx_CustomTeleport_effect",
                use_basic_prefab = true,
                show_on_mini_map = true,
                sprite_path = "effects/fx_tele_minato",
                sorting_layer_id = "Objects"
            });
            World.world.stackEffects.CallMethod("add", effectCustomEffect);

            Debug.Log("AHHHHHHHHHHHHHHHHHHHHHHHHHHHH WORKS PLEASE");
        }
        public static void addTraitToLocalizedLibrary(string planguage, string name, string desc, string id, string description)
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            string templanguage;
            templanguage = language;
            if (templanguage != "ru" && templanguage != "en")
            {
                templanguage = "en";
            }
            if (planguage == templanguage)
            {
                Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;
                localizedText.Add(name, id);
                localizedText.Add(desc, description);
            }
           
        }
    }
}