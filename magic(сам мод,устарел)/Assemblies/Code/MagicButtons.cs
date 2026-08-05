using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using static Config;

namespace Magic
{
    class MagicButtons
    {
        public static string currentBuilding = "DefilerGate";
        public static void init()
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            if(language == "ru")
            {
                MagicTab.createTab("Button Tab_Magic", "Tab_Magic", "Магия", "Творите магию прямо в вашем мире!", -150);
            }
            else
            {
                MagicTab.createTab("Button Tab_Magic", "Tab_Magic", "Magic", "Create magic right in your world!", -150);
            }
            loadButtons();
        }

        private static void loadButtons()
        {
            int index_x = 0;
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            PowersTab magicTab = getPowersTab("Magic");
            //Races
            
            #region существа

            var vampire = new GodPower();
            vampire.id = "spawnvampire";
            vampire.showSpawnEffect = true;
            vampire.multiple_spawn_tip = true;
            vampire.actorSpawnHeight = 3f;
            vampire.name = "spawnvampire";
            vampire.spawnSound = "spawnelf";
            vampire.actor_asset_id = "unit_vampire";
            vampire.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(vampire);

            if(language == "ru")
            {
                var buttonvampire = NCMS.Utils.PowerButtons.CreateButton(
                "spawnvampire",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Vampire.png"),
                "Вампиры",
                "Темная раса вампиров",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonvampire = NCMS.Utils.PowerButtons.CreateButton(
                "spawnvampire",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Vampire.png"),
                "The Vampire",
                "The Dark Race of vampires",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }

            index_x++;
            index_x++;

            var waterspirit = new GodPower();
            waterspirit.id = "spawnwaterspirit";
            waterspirit.showSpawnEffect = true;
            waterspirit.multiple_spawn_tip = true;
            waterspirit.actorSpawnHeight = 3f;
            waterspirit.name = "spawnwaterspirit";
            waterspirit.spawnSound = "spawnelf";
            waterspirit.actor_asset_id = "water_spirit";
            waterspirit.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(waterspirit);

            if(language == "ru")
            {
                var buttonWaterSpirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnwaterspirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.water_spirit.png"),
                "Водный Дух",
                "Дух воды живущий в океане",
                new Vector2(72+index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else
            {
                var buttonWaterSpirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnwaterspirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.water_spirit.png"),
                "The Water Spirit",
                "The spirit of water living in the ocean",
                new Vector2(72+index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            //index_x++;

            var fire_spirit = new GodPower();
            fire_spirit.id = "spawnfire_spirit";
            fire_spirit.showSpawnEffect = true;
            fire_spirit.multiple_spawn_tip = true;
            fire_spirit.actorSpawnHeight = 3f;
            fire_spirit.name = "spawnfire_spirit";
            fire_spirit.spawnSound = "spawnelf";
            fire_spirit.actor_asset_id = "Fire_spirit";
            fire_spirit.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(fire_spirit);

            if(language == "ru")
            {
                var buttonFireSpirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnfire_spirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Fire_spirit.png"),
                "Огненный Дух",
                "Дух огня живущий в вулканах",
                new Vector2(72+index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else
            {
                var buttonFireSpirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnfire_spirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Fire_spirit.png"),
                "The Fiery Spirit",
                "The spirit of fire living in volcanoes",
                new Vector2(72+index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            index_x++;

            var earth_spirit = new GodPower();
            earth_spirit.id = "spawnearth_spirit";
            earth_spirit.showSpawnEffect = true;
            earth_spirit.multiple_spawn_tip = true;
            earth_spirit.actorSpawnHeight = 3f;
            earth_spirit.name = "spawnearth_spirit";
            earth_spirit.spawnSound = "spawnelf";
            earth_spirit.actor_asset_id = "earth_spirit";
            earth_spirit.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(earth_spirit);

            if(language == "ru")
            {
                var buttonearth_spirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnearth_spirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.earth_spirit.png"),
                "Дух Земли",
                "Дух Земли самый полезный в хозяйстве и самый разрушительный в бою",
                new Vector2(72+index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );  
            }
            else
            {
                var buttonearth_spirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnearth_spirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.earth_spirit.png"),
                "The Earth Spirit",
                "The Earth Spirit is the most useful in the household and the most destructive in battle",
                new Vector2(72+index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );    
            }
            
            
            var air_spirit = new GodPower();
            air_spirit.id = "spawnair_spirit";
            air_spirit.showSpawnEffect = true;
            air_spirit.multiple_spawn_tip = true;
            air_spirit.actorSpawnHeight = 3f;
            air_spirit.name = "spawnair_spirit";
            air_spirit.spawnSound = "spawnelf";
            air_spirit.actor_asset_id = "air_spirit";
            air_spirit.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(air_spirit);

            if(language == "ru")
            {
                var buttonair_spirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnair_spirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.air_spirit.png"),
                "Дух Ветра",
                "Дух Ветра непостоянен в своем положении и очень опасен в своей ветренности",
                new Vector2(72+index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else
            {
                var buttonair_spirit = NCMS.Utils.PowerButtons.CreateButton(
                "spawnair_spirit",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.air_spirit.png"),
                "The Wind Spirit",
                "The Wind Spirit is fickle in its position and very dangerous in its windiness",
                new Vector2(72+index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            index_x++;
            index_x++;

            GodPower lighthouse = new GodPower();
            lighthouse.id = "lighthouse";
            lighthouse.name = "lighthouse";
            lighthouse.rank = PowerRank.Rank3_good;
            lighthouse.select_button_action = lighthouse.select_button_action;
            lighthouse.click_power_action = new PowerAction(action_spawn_building);
            AssetManager.powers.add(lighthouse);
            if(language == "ru")
            {
                var buttonlighthouse = NCMS.Utils.PowerButtons.CreateButton(
                "lighthouse",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.lighthouse.png"),
                "Маяк",
                "Укажите на великую опасность",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonlighthouse = NCMS.Utils.PowerButtons.CreateButton(
                "lighthouse",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.lighthouse.png"),
                "Lighthouse",
                "Point out the great danger",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }

            var demonKing = new GodPower();
            demonKing.id = "spawnDemonKings";
            demonKing.showSpawnEffect = true;
            demonKing.multiple_spawn_tip = true;
            demonKing.actorSpawnHeight = 3f;
            demonKing.name = "spawnDemonKings";
            demonKing.spawnSound = "spawnorc";
            demonKing.actor_asset_id = "demonKing";
            demonKing.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(demonKing);

            if(language == "ru")
            {
                var buttondemonKing = NCMS.Utils.PowerButtons.CreateButton(
                "spawnDemonKings",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.demonKings.png"),
                "Архидемон",
                "Разрушитель миров, ты не можешь контроллировать его",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttondemonKing = NCMS.Utils.PowerButtons.CreateButton(
                "spawnDemonKings",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.demonKings.png"),
                "The Archdemon",
                "Destroyer of worlds, you can't control him",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            index_x++;

            GodPower DefilerGate = new GodPower();
            DefilerGate.id = "DefilerGate";
            DefilerGate.name = "DefilerGate";
            DefilerGate.rank = PowerRank.Rank3_good;
            DefilerGate.select_button_action = DefilerGate.select_button_action;
            DefilerGate.click_power_action = new PowerAction(action_spawn_building);
            AssetManager.powers.add(DefilerGate);
            if(language == "ru")
            {
                var buttonDefilerGate = NCMS.Utils.PowerButtons.CreateButton(
                "DefilerGate",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.DefGate.png"),
                "Врата в Бездну",
                "Не делай этого. Серьёзно",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonDefilerGate = NCMS.Utils.PowerButtons.CreateButton(
                "DefilerGate",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.DefGate.png"),
                "The Gate to the Abyss",
                "Don't do this. Seriously",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }

            var Defiler = new GodPower();
            Defiler.id = "spawnDefiler";
            Defiler.showSpawnEffect = true;
            Defiler.multiple_spawn_tip = true;
            Defiler.actorSpawnHeight = 3f;
            Defiler.name = "spawnDefiler";
            Defiler.spawnSound = "spawnorc";
            Defiler.actor_asset_id = "defile_demon";
            Defiler.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(Defiler);

            if(language == "ru")
            {
                var buttonDefiler = NCMS.Utils.PowerButtons.CreateButton(
                "spawnDefiler",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Defiler.png"),
                "Высший Демон Скверны",
                "Вы не хотите это выпускать",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonDefiler = NCMS.Utils.PowerButtons.CreateButton(
                "spawnDefiler",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Defiler.png"),
                "The Supreme Demon of Fel",
                "You don't want to release this",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            index_x++;
            
            /*GodPower TowerOfFel = new GodPower();
            TowerOfFel.id = "Flame_tower";
            TowerOfFel.name = "Flame_tower";
            TowerOfFel.rank = PowerRank.Rank3_good;
            TowerOfFel.select_button_action = TowerOfFel.select_button_action;
            TowerOfFel.click_power_action = new PowerAction(action_spawn_building);
            AssetManager.powers.add(TowerOfFel);
            if(language == "ru")
            {
                var buttonDefiler = NCMS.Utils.PowerButtons.CreateButton(
                "Flame_tower",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.DefGate.png"),
                "Башня Скверны",
                "Здесь живут низшие",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonDefiler = NCMS.Utils.PowerButtons.CreateButton(
                "Flame_tower",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.DefGate.png"),
                "The Tower of Fel",
                "Lower demons live here",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            GodPower DemonsHouse = new GodPower();
            DemonsHouse.id = "DemonsHouse";
            DemonsHouse.name = "DemonsHouse";
            DemonsHouse.rank = PowerRank.Rank3_good;
            DemonsHouse.select_button_action = DemonsHouse.select_button_action;
            DemonsHouse.click_power_action = new PowerAction(action_spawn_building);
            AssetManager.powers.add(DemonsHouse);
            if(language == "ru")
            {
                var buttonDefiler = NCMS.Utils.PowerButtons.CreateButton(
                "DemonsHouse",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.DemonsHouse.png"),
                "Дом демонидов",
                "Дом демонидов",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonvampire = NCMS.Utils.PowerButtons.CreateButton(
                "DemonsHouse",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.DemonsHouse.png"),
                "Demons House",
                "Demons House",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }*/

            var LowDefiler = new GodPower();
            LowDefiler.id = "spawnLowDefiler";
            LowDefiler.showSpawnEffect = true;
            LowDefiler.multiple_spawn_tip = true;
            LowDefiler.actorSpawnHeight = 3f;
            LowDefiler.name = "spawnLowDefiler";
            LowDefiler.spawnSound = "spawnorc";
            LowDefiler.actor_asset_id = "lowest_defile_demon";
            LowDefiler.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(LowDefiler);

            if(language == "ru")
            {
                var buttonLowDefiler = NCMS.Utils.PowerButtons.CreateButton(
                "spawnLowDefiler",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Defiler.png"),
                "Низший Демон Скверны",
                "Опасны в больших колличествах",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonLowDefiler = NCMS.Utils.PowerButtons.CreateButton(
                "spawnLowDefiler",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Defiler.png"),
                "The Lowest Demon of Fel",
                "They are dangerous in large quantities",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            index_x++;

            GodPower barracks_demons = new GodPower();
            barracks_demons.id = "barracks_demons";
            barracks_demons.name = "barracks_demons";
            barracks_demons.rank = PowerRank.Rank3_good;
            barracks_demons.select_button_action = barracks_demons.select_button_action;
            barracks_demons.click_power_action = new PowerAction(action_spawn_building);
            AssetManager.powers.add(barracks_demons);
            if(language == "ru")
            {
                var buttonbarracks_demons = NCMS.Utils.PowerButtons.CreateButton(
                "barracks_demons",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.barracks_demons.png"),
                "Демонические бараки",
                "Место подготовки демонов",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonbarracks_demons = NCMS.Utils.PowerButtons.CreateButton(
                "barracks_demons",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.barracks_demons.png"),
                "Demonic Barracks",
                "A place of demon training",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }

            var HiddenEvil = new GodPower();
            HiddenEvil.id = "spawnHiddenEvil";
            HiddenEvil.showSpawnEffect = true;
            HiddenEvil.multiple_spawn_tip = true;
            HiddenEvil.actorSpawnHeight = 3f;
            HiddenEvil.name = "spawnHiddenEvil";
            HiddenEvil.spawnSound = "spawnorc";
            HiddenEvil.actor_asset_id = "hidden_demon";
            HiddenEvil.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(HiddenEvil);

            if(language == "ru")
            {
                var buttonHiddenEvil = NCMS.Utils.PowerButtons.CreateButton(
                "spawnHiddenEvil",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Defiler.png"),
                "Демон пересмешник",
                "Демоны предательства и обмана",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonHiddenEvil = NCMS.Utils.PowerButtons.CreateButton(
                "spawnHiddenEvil",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Defiler.png"),
                "The Mockingbird Demon",
                "Demons of betrayal and deception",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            index_x++;

            GodPower HellKennel = new GodPower();
            HellKennel.id = "HellKennel";
            HellKennel.name = "HellKennel";
            HellKennel.rank = PowerRank.Rank3_good;
            HellKennel.select_button_action = HellKennel.select_button_action;
            HellKennel.click_power_action = new PowerAction(action_spawn_building);
            AssetManager.powers.add(HellKennel);
            if(language == "ru")
            {
                var buttonHellKennel = NCMS.Utils.PowerButtons.CreateButton(
                "HellKennel",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.HellKennel.png"),
                "Адская псарня",
                "Агрессивные гав гав",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonHellKennel = NCMS.Utils.PowerButtons.CreateButton(
                "HellKennel",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.HellKennel.png"),
                "Hell's Kennel",
                "Aggressive woof woof",
                
                new Vector2(72 + index_x*36, 18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }

            var hellhound = new GodPower();
            hellhound.id = "spawnHellhound";
            hellhound.showSpawnEffect = true;
            hellhound.multiple_spawn_tip = true;
            hellhound.actorSpawnHeight = 3f;
            hellhound.name = "spawnHellhound";
            hellhound.spawnSound = "spawnorc";
            hellhound.actor_asset_id = "hellhound";
            hellhound.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(hellhound);

            if(language == "ru")
            {
                var buttonhellhound = NCMS.Utils.PowerButtons.CreateButton(
                "spawnHellhound",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.hellhound.png"),
                "Адская гончая",
                "Сильны, быстры, опасны",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonhellhound = NCMS.Utils.PowerButtons.CreateButton(
                "spawnHellhound",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.hellhound.png"),
                "Hellhound",
                "Strong, fast, dangerous",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            index_x++;
            index_x++;

            var angel = new GodPower();
            angel.id = "spawnAngels";
            angel.showSpawnEffect = true;
            angel.multiple_spawn_tip = true;
            angel.actorSpawnHeight = 3f;
            angel.name = "spawnAngels";
            angel.spawnSound = "spawnorc";
            angel.actor_asset_id = "angel";
            angel.click_action = new PowerActionWithID(callSpawnUnit);
            AssetManager.powers.add(angel);

            if(language == "ru")
            {
                var buttonangels = NCMS.Utils.PowerButtons.CreateButton(
                "spawnAngels",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.angel.png"),
                "Ангел",
                "Очень добры и особенно любят демонов",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            else{
                var buttonangels = NCMS.Utils.PowerButtons.CreateButton(
                "spawnAngels",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.angel.png"),
                "Angel",
                "They are very kind and especially love demons",
                
                new Vector2(72 + index_x*36, -18),
                ButtonType.GodPower, 
                magicTab.transform, 
                null
                );
            }
            
            
            
            #endregion
            if(language == "ru")
            {
                var MagicSettings = NCMS.Utils.PowerButtons.CreateButton(
                    "MagicStatsWindow", 
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.icon.png"),
                    "Насыщение мира магией", 
                    "Управляйте количеством магов и магии в вашем мире", 
                    new Vector2(72, 18), 
                    ButtonType.Click, 
                    magicTab.transform, 
                    MagicStatsWindow.openWindow
                );
            }
            else{
                var MagicSettings = NCMS.Utils.PowerButtons.CreateButton(
                    "MagicStatsWindow", 
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.icon.png"),
                    "Saturation of magic", 
                    "Manage the fullness of the magic of your world", 
                    new Vector2(72, 18), 
                    ButtonType.Click, 
                    magicTab.transform, 
                    MagicStatsWindow.openWindow
                );
            }
            
        }
        public static bool callSpawnUnit(WorldTile pTile, string pPowerID)
        {
            AssetManager.powers.CallMethod("spawnUnit", pTile, pPowerID);
            return true;
        }

        public static bool action_spawn_building(WorldTile pTile, GodPower pPower)
        {
            Building newBuilding = World.world.buildings.addBuilding(pPower.id, pTile, false, false, BuildPlacingType.New);
            if (newBuilding == null)
            {
                EffectsLibrary.spawnAtTile("fx_bad_place", pTile, 0.25f);
                return false;
            }
            if (newBuilding.asset.cityBuilding && pTile.zone.city != null)
			{
				pTile.zone.city.addBuilding(newBuilding);
				newBuilding.retake();
			}
            return true;
        }
        private static PowersTab getPowersTab(string id)
		{
		GameObject gameObject = GameObjects.FindEvenInactive("Tab_" + id);
		return gameObject.GetComponent<PowersTab>();
        }
    }
}