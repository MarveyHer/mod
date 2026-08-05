using System;
using NCMS;
using UnityEngine;
using ReflectionUtility;
namespace Magic
{
    class NaturalBirth
    {
        
        public static void init()
        {   
            /*var tnt = AssetManager.biome_library.get("tnt");
            tnt.addUnit("bandit");*/
            var forest = AssetManager.biome_library.get(ST.biome_grass);
            var forest_soil_corrupted = AssetManager.biome_library.get(ST.biome_corrupted); 
            var forest_soil_wasteland = AssetManager.biome_library.get(ST.biome_wasteland); 
            var forest_soil_infernal = AssetManager.biome_library.get(ST.biome_infernal);
            var forest_soil_savanna = AssetManager.biome_library.get(ST.biome_savanna);
            var forest_soil_enchanted = AssetManager.biome_library.get(ST.biome_enchanted);
            var forest_soil_crystal = AssetManager.biome_library.get(ST.biome_crystal);
            var forest_soil_mushroom = AssetManager.biome_library.get(ST.biome_mushroom);
            var forest_soil_jungle = AssetManager.biome_library.get(ST.biome_jungle);
            var forest_soil_swamp = AssetManager.biome_library.get(ST.biome_swamp);
            var forest_soil_desert = AssetManager.biome_library.get(ST.biome_desert);
            var forest_soil_frozen = AssetManager.biome_library.get(ST.biome_permafrost);
            var forest_soil_candy = AssetManager.biome_library.get(ST.biome_candy);
            var forest_soil_lemon = AssetManager.biome_library.get(ST.biome_lemon);
            if (Main.spawn_race)
            {
                
                forest.addUnit("unit_human", 10);
                forest.addUnit("unit_dwarf", 1);
                forest.addUnit("unit_elf", 2);
                forest.addUnit("unit_orc", 2);
                forest.addUnit("plagueDoctor", 3);
                forest.addUnit("unit_angel", 2);
                forest.addUnit("griffins", 3);
                forest.addUnit("bandit", 4);
                
                forest_soil_corrupted.addUnit("zombie", 3);
                forest_soil_corrupted.addUnit("ghost", 5);
                forest_soil_corrupted.addUnit("skeleton", 5);
                forest_soil_corrupted.addUnit("necromancer", 1);
                forest_soil_corrupted.addUnit("unit_vampire", 3);
                forest_soil_corrupted.addUnit("unit_goblin", 3);
                forest_soil_corrupted.addUnit("witch", 1);
                 
                forest_soil_wasteland.addUnit("alien", 5);
                forest_soil_wasteland.addUnit("zombie_dragon", 1);
                forest_soil_wasteland.addUnit("unit_android", 5);
                
                forest_soil_infernal.addUnit("dragon", 1);
                forest_soil_infernal.addUnit("evilMage", 2);
                forest_soil_infernal.addUnit("demons", 5);
                forest_soil_infernal.addUnit("unit_demonic", 8);
                forest_soil_infernal.addUnit("DemonKing", 1);
                
                forest_soil_savanna.addUnit("unit_orc", 6);
                forest_soil_savanna.addUnit("unit_lizard", 2);
                
                forest_soil_enchanted.addUnit("unit_elf", 3);
                forest_soil_enchanted.addUnit("plagueDoctor", 1);
                forest_soil_enchanted.addUnit("druid", 3);
                forest_soil_enchanted.addUnit("unit_angel", 3);
                forest_soil_enchanted.addUnit("griffins", 3);
                
                forest_soil_crystal.addUnit("unit_dwarf", 6);
                forest_soil_crystal.addUnit("unit_gnome", 6);
                forest_soil_crystal.addUnit("crystal_golem", 7);
                
                forest_soil_mushroom.addUnit("unit_dwarf", 6);
                forest_soil_mushroom.addUnit("unit_gnome", 6);
                
                forest_soil_jungle.addUnit("unit_human", 3);
                forest_soil_jungle.addUnit("plagueDoctor", 1);
                forest_soil_jungle.addUnit("unit_darkelve", 6);
                forest_soil_jungle.addUnit("unit_beastmen", 3);
                forest_soil_jungle.addUnit("bandit", 4);
                
                forest_soil_swamp.addUnit("unit_lizard", 6);
                forest_soil_swamp.addUnit("unit_goblin", 4);
                forest_soil_swamp.addUnit("unit_darkelve", 2);
                forest_soil_swamp.addUnit("unit_orc", 1);
                
                forest_soil_desert.addUnit("unit_human", 2);
                forest_soil_desert.addUnit("unit_demonic", 3);
                forest_soil_desert.addUnit("unit_orc", 2);
                
                forest_soil_lemon.addUnit("unit_human", 2);
                forest_soil_lemon.addUnit("unit_dwarf", 2);
                forest_soil_lemon.addUnit("unit_elf", 2);
                forest_soil_lemon.addUnit("unit_orc", 2);
                forest_soil_lemon.addUnit("unit_angel", 2);
                forest_soil_lemon.addUnit("unit_demonic", 2);
                forest_soil_lemon.addUnit("unit_goblin", 2);
                forest_soil_lemon.addUnit("unit_lizard", 2);
                forest_soil_lemon.addUnit("unit_gnome", 2);
                forest_soil_lemon.addUnit("unit_human", 2);
                
                forest_soil_candy.addUnit("unit_human", 2);
                forest_soil_candy.addUnit("unit_dwarf", 2);
                forest_soil_candy.addUnit("unit_elf", 2);
                forest_soil_candy.addUnit("unit_orc", 2);
                forest_soil_candy.addUnit("unit_angel", 2);
                forest_soil_candy.addUnit("unit_demonic", 2);
                forest_soil_candy.addUnit("unit_goblin", 2);
                forest_soil_candy.addUnit("unit_lizard", 2);
                forest_soil_candy.addUnit("unit_gnome", 2);
                forest_soil_candy.addUnit("unit_human", 2);
                
                forest_soil_frozen.addUnit("whiteMage", 2);
                forest_soil_frozen.addUnit("snowman", 2);
                forest_soil_frozen.addUnit("unit_beastmen", 3);
                forest_soil_frozen.addUnit("unit_human", 6);
                forest_soil_frozen.addUnit("walker", 3);
            }
            
            if (Main.spirit_spawn)
            {
                
                forest.addUnit("earth_spirit", 3);
                forest_soil_frozen.addUnit("water_spirit", 2);
                forest_soil_desert.addUnit("eatrh_spirit", 1);
                forest_soil_desert.addUnit("air_spirit", 1);
                forest_soil_swamp.addUnit("water_spirit", 2);
                forest_soil_infernal.addUnit("Fire_spirit", 1);
                forest_soil_savanna.addUnit("air_spirit", 2);
                BuildingAsset volcano = AssetManager.buildings.get("volcano");
                BuildingAsset geyser = AssetManager.buildings.get("geyser");
                BuildingAsset geyserAcid = AssetManager.buildings.get("geyserAcid");

                volcano.spawnUnits = true;
                volcano.spawnUnits_asset = "Fire_spirit";
                geyser.spawnUnits = true;
                geyser.spawnUnits_asset = "water_spirit";
                geyserAcid.spawnUnits = true;
                geyserAcid.spawnUnits_asset = SA.alien;
            }
            if (Main.spawn_demon)
            {
                //forest_soil_infernal.addMineral("DefilerGate",1);
                forest_soil_infernal.addUnit("hellhound",5);
                forest_soil_infernal.addUnit("hidden_demon",3);
                forest_soil_infernal.addUnit("lowest_defile_demon",7);
                forest_soil_infernal.addUnit("defile_demon",1);
                forest_soil_infernal.addUnit("fel_dragon",1);
                //forest_soil_infernal.addMineral("Flame_Tower",1);
            }
            
        }
    }
}