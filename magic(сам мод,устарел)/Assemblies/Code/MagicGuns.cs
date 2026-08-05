using System;
using NCMS;
using UnityEngine;
using ReflectionUtility;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Beebyte.Obfuscator;
using NCMS.Utils;

namespace Magic
{
    class MagicGuns
    {
        public static void init()
        {

          //Angel Guns

          ItemAsset DivineStaff = AssetManager.items.clone("AirSpiritEffect", "_range");
          DivineStaff.id = "AirSpiritEffect";
          DivineStaff.projectile = "AngelProjetil";
          DivineStaff.path_slash_animation = "effects/slashes/slash_punch";
          DivineStaff.tech_needed = "weapon_bow";
          DivineStaff.base_stats[S.targets] = 1;
          DivineStaff.base_stats[S.range] = 22f;
          DivineStaff.base_stats[S.critical_chance] = 0.5f;
          DivineStaff.base_stats[S.projectiles] = 1;
          DivineStaff.materials = List.Of<string>(new string[]{"adamantine"});
          DivineStaff.name_templates = Toolbox.splitStringIntoList(new string[]
          {
            "bow_name#30",
		        "weapon_name_city",
		        "weapon_name_kingdom",
		        "weapon_name_culture",
		        "weapon_name_enemy_king",
		        "weapon_name_enemy_kingdom"
          });
          DivineStaff.equipmentType = EquipmentType.Weapon;
          DivineStaff.name_class = "item_class_weapon";
          AssetManager.items.list.AddItem(DivineStaff);
          Localization.addLocalization("item_AirSpiritEffect", "AirSpiritEffect");
          addGunsSprite(DivineStaff.id, DivineStaff.materials[0]);


          //DragonSlayer

         
  
          ItemAsset DragonSlayerSword = AssetManager.items.clone("FireSpiritEffect", "bow");
          DragonSlayerSword.id = "FireSpiritEffect";
          DragonSlayerSword.projectile = "FireSpiritEffect";
          DragonSlayerSword.name_templates = Toolbox.splitStringIntoList(new string[]
          {
          "sword_name#30",
          "sword_name_king#3",
          "weapon_name_city",
          "weapon_name_kingdom",
          "weapon_name_culture",
          "weapon_name_enemy_king",
          "weapon_name_enemy_kingdom"
          });
          DragonSlayerSword.materials = List.Of<string>(new string[]{"adamantine"});
          DragonSlayerSword.base_stats[S.fertility] = 0.0f;
          DragonSlayerSword.base_stats[S.max_children] = 0f;
          DragonSlayerSword.base_stats[S.max_age] = 0f;
          DragonSlayerSword.base_stats[S.attack_speed] = 20;
          DragonSlayerSword.base_stats[S.damage] = 30;
          DragonSlayerSword.base_stats[S.speed] = 30f;
          DragonSlayerSword.base_stats[S.health] = 0;
          DragonSlayerSword.base_stats[S.accuracy] = 0f;
          DragonSlayerSword.base_stats[S.range] = 10;
          DragonSlayerSword.base_stats[S.armor] = 10;
          DragonSlayerSword.base_stats[S.scale] = 0.0f;
          DragonSlayerSword.base_stats[S.dodge] = 0f;
          DragonSlayerSword.base_stats[S.targets] = 0f;
          DragonSlayerSword.base_stats[S.critical_chance] = 0.0f;
          DragonSlayerSword.base_stats[S.knockback] = 0f;
          DragonSlayerSword.base_stats[S.knockback_reduction] = 0f;
          DragonSlayerSword.base_stats[S.intelligence] = 0;
          DragonSlayerSword.base_stats[S.warfare] = 0;
          DragonSlayerSword.base_stats[S.diplomacy] = 0;
          DragonSlayerSword.base_stats[S.stewardship] = 0;
          DragonSlayerSword.base_stats[S.opinion] = 0f;
          DragonSlayerSword.base_stats[S.loyalty_traits] = 0f;
          DragonSlayerSword.base_stats[S.cities] = 0;
          DragonSlayerSword.base_stats[S.zone_range] = 0;
          DragonSlayerSword.equipment_value = 10000;
          DragonSlayerSword.path_slash_animation = "effects/slashes/slash_sword";
          DragonSlayerSword.tech_needed = "weapon_sword";
          DragonSlayerSword.equipmentType = EquipmentType.Weapon;
          DragonSlayerSword.name_class = "item_class_weapon";
          DragonSlayerSword.action_special_effect = new WorldAction(cureeffect);
          DragonSlayerSword.action_attack_target = new AttackAction(swordeffect);
          AssetManager.items.list.AddItem(DragonSlayerSword);
          Localization.addLocalization("item_FireSpiritEffect", "Оружие огненного духа");
          addGunsSprite(DragonSlayerSword.id, DragonSlayerSword.materials[0]);


          ProjectileAsset DragonSlayerEffect = new ProjectileAsset();
          DragonSlayerEffect.id = "FireSpiritEffect";
		      DragonSlayerEffect.texture = "FireSpiritEffect";
          DragonSlayerEffect.parabolic = false;
          DragonSlayerEffect.speed = 18f;
          AssetManager.projectiles.add(DragonSlayerEffect);

        }

        public static bool cureeffect(BaseSimObject pTarget, WorldTile pTile = null)
        {
        Actor a = Reflection.GetField(pTarget.GetType(), pTarget, "a") as Actor;
        if(Toolbox.randomChance(0.0025f)){
        
        World.world.dropManager.spawn(pTile, "cure", 15f, -1f);

        World.world.dropManager.spawn(pTile, "cure", 15f, -1f);

        World.world.dropManager.spawn(pTile, "cure", 15f, -1f);

        World.world.dropManager.spawn(pTile, "cure", 15f, -1f);

        World.world.dropManager.spawn(pTile, "cure", 15f, -1f);

        World.world.dropManager.spawn(pTile, "cure", 15f, -1f);

        }
        return false;
  
        }
        public static bool swordeffect(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
        if(pTarget != null){
        Actor a = Reflection.GetField(pTarget.GetType(), pTarget, "a") as Actor;
        if(Toolbox.randomChance(1f)){
        ActionLibrary.addBurningEffectOnTarget(null, pTarget, null);
        }}
        return false;
  
        }
            static void addGunsSprite(string id, string material)
            {
              var dictItems = Reflection.GetField(typeof(ActorAnimationLoader), null, "dictItems") as Dictionary<string, Sprite>;
              var sprite = Resources.Load<Sprite>("guns/w_" + id + "_" + material);
              dictItems.Add(sprite.name, sprite);
            }
      }
}