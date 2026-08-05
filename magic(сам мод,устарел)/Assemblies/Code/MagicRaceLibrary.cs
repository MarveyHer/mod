using HarmonyLib;
using System;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using ReflectionUtility;
using System.Threading;
using System.Text;
using System.IO;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Reflection.Emit;
using ai;

namespace Magic{
    class MagicRaceLibrary
    {
        internal static List<string> defaultRaces = new List<string>(){
            "human", "elf", "orc", "dwarf"
        };

        internal static List<string> human = new List<string>(){
            "human"
        };

        internal static List<string> additionalRaces = new List<string>(){
            "vampire"
        };
        
        internal void init(){

            

            var vampire = AssetManager.raceLibrary.clone("vampire", "human");
            vampire.civ_baseCities =  -2;
            vampire.civ_base_army_mod  = 0.9f;
            vampire.civ_base_zone_range = 1;
            vampire.culture_rate_tech_limit = 6;
            vampire.culture_knowledge_gain_per_intelligence = 1.9f;

            vampire.build_order_id = "kingdom_base";
            vampire.path_icon = "ui/Icons/Vampire";
            vampire.nameLocale = "Vampire";
            vampire.banner_id= "human";
            vampire.main_texture_path = "races/vampire/";
            vampire.name_template_city = "human_city";
            vampire.name_template_kingdom = "human_kingdom";
            vampire.name_template_culture = "human_culture";
            vampire.name_template_clan = "human_clan";
            vampire.production = new string[] { "jam" };
            vampire.skin_citizen_male = List.Of<string>(new string[] {	
			"unit_male_1","unit_male_2"});
            vampire.skin_citizen_female = List.Of<string>(new string[] {
 			"unit_female_1","unit_female_2"});
            vampire.skin_warrior = List.Of<string>(new string[] {
  			"unit_warrior_1","unit_warrior_2"});
            vampire.nomad_kingdom_id = $"nomads_{vampire.id}";

            vampire.preferred_weapons.Clear();
            AssetManager.raceLibrary.CallMethod("setPreferredStatPool", "diplomacy#1,stewardship#2,intelligence#10,warfare#10");
            AssetManager.raceLibrary.CallMethod("setPreferredFoodPool", "meat#5");
            AssetManager.raceLibrary.CallMethod("addPreferredWeapon", "bow", 8);
            AssetManager.raceLibrary.CallMethod("addPreferredWeapon", "sword", 10);
            AssetManager.raceLibrary.CallMethod("addPreferredWeapon", "necromancer_staff", 10);

            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_bonfire, SB.bonfire);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_tent, "tent_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_0, "house_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_1, "1house_vampire");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_statue, SB.statue);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_mine, SB.mine);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_barracks, "barracks_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_2, "2house_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_3, "3house_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_4, "4house_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_house_5, "5house_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_hall_0, "hall_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_hall_1, "1hall_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_hall_2, "2hall_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_windmill_0, "windmill_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_windmill_1, "windmill_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_docks_0, "fishing_docks_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_docks_1, "docks_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_watch_tower, "watch_tower_vampire");
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_temple, "temple_vampire");
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_statue, SB.statue);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_well, SB.well);
            AssetManager.raceLibrary.addBuildingOrderKey(SB.order_bonfire, SB.bonfire);
		    AssetManager.raceLibrary.addBuildingOrderKey(SB.order_mine, SB.mine);



            vampire.hateRaces = List.Of<string>(new string[]
		    {
			SK.elf, SK.dwarf, SK.human
		    });

            var human = AssetManager.raceLibrary.get("human");
            human.hateRaces = List.Of<string>(new string[]
		    {
			SK.orc,vampire.id
		    });

            var elf = AssetManager.raceLibrary.get("elf");
            elf.hateRaces = List.Of<string>(new string[]
		    {
			SK.orc,SK.dwarf,vampire.id
		    });

            var orc = AssetManager.raceLibrary.get("orc");
            orc.hateRaces = List.Of<string>(new string[]
		    {
			SK.human,SK.elf,SK.dwarf
		    });

            var dwarf = AssetManager.raceLibrary.get("dwarf");
            dwarf.hateRaces = List.Of<string>(new string[]
		    {
			SK.orc,SK.elf,vampire.id
		    });
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorAnimationLoader), "loadAnimationBoat")]
        public static bool loadAnimationBoat_Prefix(ref string pTexturePath, ActorAnimationLoader __instance)
        {
        if (pTexturePath.EndsWith("_"))
        {
         pTexturePath = pTexturePath + "human";
         return true;
        }
        foreach (string race in Main.instance.addRaces)
        {
        if (race == "android")
         {
          pTexturePath = pTexturePath.Replace(race, "android");
         }
        else if (race == "goblin")
         {
          pTexturePath = pTexturePath.Replace(race, "goblin");
         }
        else if (race == "vampire")
         {
          pTexturePath = pTexturePath.Replace(race, "vampire");
         }
        else if (race == "lizard")
         {
          pTexturePath = pTexturePath.Replace(race, "lizard");
         }
        else if (race == "darkelve")
         {
          pTexturePath = pTexturePath.Replace(race, "darkelve");
         }
        else if (race == "beastmen")
         {
          pTexturePath = pTexturePath.Replace(race, "beastmen");
         }
        else if (race == "gnome")
         {
          pTexturePath = pTexturePath.Replace(race, "gnome");
         }
        else if (race == "angel")
         {
          pTexturePath = pTexturePath.Replace(race, "angel");
         }
        else if (race == "demonic")
         {
          pTexturePath = pTexturePath.Replace(race, "demonic");
         }
         else if (race == "japaneses")
         {
          pTexturePath = pTexturePath.Replace(race, "japaneses");
         }
         else if (race == "ancientchina")
         {
          pTexturePath = pTexturePath.Replace(race, "ancientchina");
         }
        else
         {
          pTexturePath = pTexturePath.Replace(race, "human");
         }
        }
        return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorAnimationLoader), "generateFrameData")]
        
        public static void generateFrameData_post(ActorAnimationLoader __instance,string pFrameString, AnimationContainerUnit pAnimContainer, Dictionary<string, Sprite> pFrames, string pFramesIDs)
        {

        }

        public static Sprite[] loadAllSprite(string path, bool withFolders = false)
        {
            string p = $"{Main.mainPath}/EmbededResources/{path}";
            DirectoryInfo folder = new DirectoryInfo(p);
            List<Sprite> res = new List<Sprite>();
            foreach (FileInfo file in folder.GetFiles("*.png"))
            {
                Sprite sprite = Sprites.LoadSprite($"{file.FullName}");
                sprite.name = file.Name.Replace(".png", "");
                res.Add(sprite);
            }
            foreach (DirectoryInfo cFolder in folder.GetDirectories())
            {
                foreach (FileInfo file in cFolder.GetFiles("*.png"))
                {
                    Sprite sprite = Sprites.LoadSprite($"{file.FullName}");
                    sprite.name = file.Name.Replace(".png", "");
                    res.Add(sprite);
                }
            }
            return res.ToArray();
        }
    }
}