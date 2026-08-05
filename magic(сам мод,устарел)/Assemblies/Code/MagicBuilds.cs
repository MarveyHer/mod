using System;
using System.Linq;
using System.Collections.Generic;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ReflectionUtility;
using DG.Tweening;
using Magic;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;

namespace Magic
{
    class MagicBuilds
    {
     private static List<BuildingAsset> humanBuildings = new List<BuildingAsset>();
     public static void init()
     {       
        BuildingAsset bonfire = AssetManager.buildings.get("bonfire");
            bonfire.burnable = false;        
        foreach (BuildingAsset humanBuilding in AssetManager.buildings.list)
        {
        if (humanBuilding.race == "human")
            {
            humanBuildings.Add(humanBuilding);
            }
        }
        demonBuilds_init();
        MagicBuilds_init();
        
        foreach(var race in MagicRaceLibrary.additionalRaces)
        {
            if(race == "vampire")
            {
                initvampire();
            }
        }
     }
        private static void demonBuilds_init()
		{
            BuildingAsset Flame_tower = AssetManager.buildings.clone("Flame_Tower", SB.flame_tower);
            Flame_tower.spawnUnits_asset = "fel_dragon";
            Flame_tower.transformTilesToTopTiles = ST.infernal_low;
            Flame_tower.base_stats[S.health] = 1000;
            Flame_tower.fundament = new BuildingFundament(1, 1, 1, 0);
            Flame_tower.spawnUnits = true;
            AssetManager.buildings.add(Flame_tower);
            AssetManager.buildings.loadSprites(Flame_tower);

        }

        private static void MagicBuilds_init()
		{
            BuildingAsset DefGate = AssetManager.buildings.clone("DefilerGate", "!building");
            DefGate.id = "DefilerGate";
            DefGate.type = "DefilerGate";
            DefGate.race = "demon";
            DefGate.kingdom = "demons";
            DefGate.spawnUnits_asset = "defile_demon";
            DefGate.material = "building";
            DefGate.sound_idle = "event:/SFX/BUILDINGS_IDLE/IdleIceTower";
		    DefGate.sound_built = "event:/SFX/BUILDINGS/SpawnBuildingStone";
		    DefGate.sound_destroyed = "event:/SFX/BUILDINGS/DestroyBuildingStone";
            DefGate.base_stats[S.health] = 300000;
            DefGate.transformTilesToTopTiles = "infernal_low";
            DefGate.fundament = new BuildingFundament(3, 3, 2, 1);
            DefGate.damagedByRain = false;
            DefGate.burnable = false;
	        DefGate.buildRoadTo = false;
            DefGate.destroyOnLiquid = false;
	        DefGate.affectedByAcid = false;
    	    DefGate.affectedByLava = false;
            DefGate.canBePlacedOnLiquid = false;
            DefGate.canBePlacedOnBlocks = true;
	        DefGate.canBeDamagedByTornado = true;
            DefGate.ignoredByCities = false;
            DefGate.canBeLivingHouse = false;
            DefGate.checkForCloseBuilding = true;
            DefGate.ignoreBuildings = true;
            DefGate.spawnUnits = true;
            //AssetManager.buildings.setGrowBiomeAround("biome_fel", 5, 2, 0.1f, CreepWorkerMovementType.Direction);
            AssetManager.buildings.add(DefGate);
            AssetManager.buildings.loadSprites(DefGate);

            BuildingAsset Lighthouse = AssetManager.buildings.clone("lighthouse", "!building");
            Lighthouse.id = "lighthouse";
            Lighthouse.type = "lighthouse";
            Lighthouse.race = "lighthouses";
            Lighthouse.kingdom = "lighthouses";
            Lighthouse.material = "building";
            Lighthouse.sound_idle = "event:/SFX/BUILDINGS_IDLE/IdleIceTower";
		    Lighthouse.sound_built = "event:/SFX/BUILDINGS/SpawnBuildingStone";
		    Lighthouse.sound_destroyed = "event:/SFX/BUILDINGS/DestroyBuildingStone";
            Lighthouse.base_stats[S.health] = 2000;
            Lighthouse.fundament = new BuildingFundament(1, 1, 1, 0);
            Lighthouse.damagedByRain = false;
            Lighthouse.burnable = false;
	        Lighthouse.buildRoadTo = false;
            Lighthouse.destroyOnLiquid = true;
	        Lighthouse.affectedByAcid = false;
    	    Lighthouse.affectedByLava = false;
            Lighthouse.canBePlacedOnLiquid = false;
            Lighthouse.canBePlacedOnBlocks = true;
	        Lighthouse.canBeDamagedByTornado = true;
            Lighthouse.ignoredByCities = false;
            Lighthouse.canBeLivingHouse = false;
            Lighthouse.checkForCloseBuilding = true;
            Lighthouse.ignoreBuildings = true;
            Lighthouse.spawnUnits = false;
            //AssetManager.buildings.setGrowBiomeAround("biome_fel", 5, 2, 0.1f, CreepWorkerMovementType.Direction);
            AssetManager.buildings.add(Lighthouse);
            AssetManager.buildings.loadSprites(Lighthouse);

            BuildingAsset HellKennel = AssetManager.buildings.clone("HellKennel", "!building");
            HellKennel.id = "HellKennel";
            HellKennel.type = "HellKennel";
            HellKennel.race = "demon";
            HellKennel.kingdom = "demons";
            HellKennel.spawnUnits_asset = "hellhound";
            HellKennel.material = "building";
            HellKennel.sound_idle = "event:/SFX/BUILDINGS_IDLE/IdleIceTower";
		    HellKennel.sound_built = "event:/SFX/BUILDINGS/SpawnBuildingStone";
		    HellKennel.sound_destroyed = "event:/SFX/BUILDINGS/DestroyBuildingStone";
            HellKennel.base_stats[S.health] = 500;
            HellKennel.transformTilesToTopTiles = "infernal_low";
            HellKennel.fundament = new BuildingFundament(1, 1, 1, 0);
            HellKennel.damagedByRain = false;
            HellKennel.burnable = false;
	        HellKennel.buildRoadTo = false;
            HellKennel.destroyOnLiquid = true;
	        HellKennel.affectedByAcid = false;
    	    HellKennel.affectedByLava = false;
            HellKennel.canBePlacedOnLiquid = false;
            HellKennel.canBePlacedOnBlocks = true;
	        HellKennel.canBeDamagedByTornado = true;
            HellKennel.ignoredByCities = false;
            HellKennel.canBeLivingHouse = false;
            HellKennel.checkForCloseBuilding = true;
            HellKennel.ignoreBuildings = true;
            HellKennel.spawnUnits = true;
            //AssetManager.buildings.setGrowBiomeAround("biome_fel", 5, 2, 0.1f, CreepWorkerMovementType.Direction);
            AssetManager.buildings.add(HellKennel);
            AssetManager.buildings.loadSprites(HellKennel);

            BuildingAsset barracks_demons = AssetManager.buildings.clone("barracks_demons", "!building");
            barracks_demons.id = "barracks_demons";
            barracks_demons.type = "barracks_demons";
            barracks_demons.race = "demon";
            barracks_demons.kingdom = "demons";
            barracks_demons.spawnUnits_asset = "hidden_demon";
            barracks_demons.material = "building";
            barracks_demons.sound_idle = "event:/SFX/BUILDINGS_IDLE/IdleIceTower";
		    barracks_demons.sound_built = "event:/SFX/BUILDINGS/SpawnBuildingStone";
		    barracks_demons.sound_destroyed = "event:/SFX/BUILDINGS/DestroyBuildingStone";
            barracks_demons.base_stats[S.health] = 1000;
            barracks_demons.transformTilesToTopTiles = "infernal_low";
            barracks_demons.fundament = new BuildingFundament(1, 1, 1, 0);
            barracks_demons.damagedByRain = false;
            barracks_demons.burnable = false;
	        barracks_demons.buildRoadTo = false;
            barracks_demons.destroyOnLiquid = true;
	        barracks_demons.affectedByAcid = false;
    	    barracks_demons.affectedByLava = false;
            barracks_demons.canBePlacedOnLiquid = false;
            barracks_demons.canBePlacedOnBlocks = true;
	        barracks_demons.canBeDamagedByTornado = true;
            barracks_demons.ignoredByCities = false;
            barracks_demons.canBeLivingHouse = false;
            barracks_demons.checkForCloseBuilding = true;
            barracks_demons.ignoreBuildings = true;
            barracks_demons.spawnUnits = true;
            //AssetManager.buildings.setGrowBiomeAround("biome_fel", 5, 2, 0.1f, CreepWorkerMovementType.Direction);
            AssetManager.buildings.add(barracks_demons);
            AssetManager.buildings.loadSprites(barracks_demons);
            
            BuildingAsset Flame_tower = AssetManager.buildings.clone("Flame_tower", SB.flame_tower);
            Flame_tower.spawnUnits_asset = "lowest_defile_demon";
            Flame_tower.transformTilesToTopTiles = ST.infernal_low;
            Flame_tower.base_stats[S.health] = 1000;
            Flame_tower.fundament = new BuildingFundament(1, 1, 1, 0);
            Flame_tower.spawnUnits = true;
            AssetManager.buildings.add(Flame_tower);
            AssetManager.buildings.loadSprites(Flame_tower);
            
            

        }  
        private static void initvampire()
        {
            RaceBuildOrderAsset pAsset = new RaceBuildOrderAsset();
            pAsset.id = "vampire";
            AssetManager.race_build_orders.add(pAsset);
            pAsset.addBuilding("bonfire", 1);
            pAsset.addBuilding("tent_vampire", pHouseLimit: true);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            pAsset.addUpgrade("tent_vampire");
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("tent_vampire");
            addVariantsUpgrade(pAsset, "house_vampire", List.Of<string>("hall_vampire"));
            addVariantsUpgrade(pAsset, "1house_vampire", List.Of<string>("1hall_vampire"));
            addVariantsUpgrade(pAsset, "2house_vampire", List.Of<string>("1hall_vampire"));
            addVariantsUpgrade(pAsset, "3house_vampire", List.Of<string>("2hall_vampire"));
            addVariantsUpgrade(pAsset, "4house_vampire", List.Of<string>("2hall_vampire"));
            pAsset.addUpgrade("hall_vampire", pPop: 30, pBuildings: 8);
            pAsset.addUpgrade("1hall_vampire", pPop: 100, pBuildings: 20);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("statue", "mine", "barracks_vampire");
            pAsset.addUpgrade("fishing_docks_vampire");
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("fishing_docks_vampire");
            pAsset.addBuilding("windmill_vampire", 1, pPop: 6, pBuildings: 5);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            pAsset.addUpgrade("windmill_vampire", pPop: 40, pBuildings: 10);
            pAsset.addBuilding("fishing_docks_vampire", 5, pBuildings: 2);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            pAsset.addBuilding("well", 1, pPop: 20, pBuildings: 10);
            BuildOrderLibrary.b.requirements_types = List.Of<string>("hall");
            pAsset.addBuilding("hall_vampire", 1, pPop: 10, pBuildings: 6);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire");
            BuildOrderLibrary.b.requirements_types = List.Of<string>("house");
            pAsset.addBuilding("mine", 1, pPop: 20, pBuildings: 10);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire", "hall_vampire");
            pAsset.addBuilding("barracks_vampire", 1, pPop: 50, pBuildings: 16, pMinZones: 20);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("1hall_vampire");
            pAsset.addBuilding("watch_tower_vampire", 1, pPop: 30, pBuildings: 10);
            pAsset.addUpgrade("watch_tower_vampire" , 0, 0, 3, 3, false, false, 0);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire", "hall_vampire");
            pAsset.addBuilding("temple_vampire", 1, pPop: 90, pBuildings: 20, pMinZones: 20);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("bonfire", "1hall_vampire", "statue");
            pAsset.addBuilding("statue", 1, pPop: 70, pBuildings: 15);
            BuildOrderLibrary.b.requirements_orders = List.Of<string>("1hall_vampire");
        }
        private void loadSprites(BuildingAsset pTemplate)
        {
            string folder = pTemplate.race;
            if (folder == string.Empty)
            {
                folder = "Others";
            }
            folder = folder + "/" + pTemplate.id;
            Sprite[] array = Utils.ResourcesHelper.loadAllSprite("buildings/" + folder, 0.5f, 0.0f);

            pTemplate.sprites = new BuildingSprites();
            foreach (Sprite sprite in array)
            {
                string[] array2 = sprite.name.Split(new char[] { '_' });
                string text = array2[0];
                int num = int.Parse(array2[1]);

                if (array2.Length == 3)
                {
                    int.Parse(array2[2]);
                }
                while (pTemplate.sprites.animationData.Count < num + 1)
                {
                    pTemplate.sprites.animationData.Add(null);
                }
                if (pTemplate.sprites.animationData[num] == null)
                {
                    pTemplate.sprites.animationData[num] = new BuildingAnimationDataNew();
                }
                BuildingAnimationDataNew buildingAnimationDataNew = pTemplate.sprites.animationData[num];
                if (text.Equals("main"))
                {
                    buildingAnimationDataNew.list_main.Add(sprite);
                    if (buildingAnimationDataNew.list_main.Count > 1)
                    {
                        buildingAnimationDataNew.animated = true;
                    }
                }
                else if (text.Equals("ruin"))
                {
                    buildingAnimationDataNew.list_ruins.Add(sprite);
                }
                else if (text.Equals("special"))
                {
                    buildingAnimationDataNew.list_special.Add(sprite);
                }
                else if (text.Equals("mini"))
                {
                    pTemplate.sprites.mapIcon = new BuildingMapIcon(sprite);
                }
            }
            foreach (BuildingAnimationDataNew buildingAnimationDataNew2 in pTemplate.sprites.animationData)
            {
                buildingAnimationDataNew2.main = buildingAnimationDataNew2.list_main.ToArray();
                buildingAnimationDataNew2.ruins = buildingAnimationDataNew2.list_ruins.ToArray();
                buildingAnimationDataNew2.special = buildingAnimationDataNew2.list_special.ToArray();
            }
           
        }
        private static void addVariantsUpgrade(RaceBuildOrderAsset pAsset, string name, List<string> requirementsBuildings)
        {
            foreach(var race in MagicRaceLibrary.defaultRaces)
            {
                pAsset.addUpgrade($"{name}_{race}");
                BuildOrderLibrary.b.requirements_orders = requirementsBuildings;
            }
        }
    }
}