using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using ReflectionUtility;
using Beebyte.Obfuscator;
using ai;
using ai.behaviours;
using HarmonyLib;
using Newtonsoft.Json;

namespace Magic
{
    public class bloodStats
    {
        public float health = 0f;
        public float damage = 0f;
        public float attack_speed = 0f;
        public float speed = 0f;
        public int max_age = 0;
        public float armor = 0f;
    } 
    class MagicTraits
    { 
        public static Dictionary<string,float> good_defiler = new Dictionary<string,float>()
            {
            {"unit_goblin",0.3f}, 
            {"unit_lizard",0.3f}, 
            {"unit_android",0.3f}, 
            {"unit_darkelve",0.3f}, 
            {"unit_beastmen",0.3f}, 
            {"unit_gnome",0.3f}, 
            {"unit_demonic",0.3f}, 
            {"unit_japaneses",0.3f}, 
            {"unit_ancientchina",0.3f},
            {"unit_vampire",0.3f},
            {"unit_human",0.3f},
            {"unit_orc",0.3f},
            {"unit_elf",0.3f},
            {"unit_dwarf",0.3f},
            {"defile_demon",0.3f},
            {"demonKing",0.45f}
            };
        public static Dictionary<Actor, int> deadBodies = new Dictionary<Actor, int>();
        public static Dictionary<Actor, bloodStats> bloodEnhancement = new Dictionary<Actor, bloodStats>();
       /* public static float magicBirth = 3f;
        public static float magicInherit = 30f;
        public static float BloodAge = 300f;
        public static float DefilerBirth = 0f;
        public static float DefilerInherit = 0f;Main.
        public static float DemonFighterBirth = 0f;
        public static float DemonFighterInherit = 10f;
        public static bool NewMagicOfDeath;*/
        public static void init()
        {
#region Черты  
        #region Скрытое Зло
            ActorTrait hiddenEvil = new ActorTrait();
            //hiddenEvil.action_attack_target = new AttackAction(hiddenEvil);
            //Pacification.action_get_hit = new GetHitAction(pacification);
            hiddenEvil.action_special_effect = new WorldAction(hiddenEvils);
            hiddenEvil.id = "hiddenEvil";
            hiddenEvil.path_icon = "ui/Extras/hiddenEvil";
            //hiddenEvil.birth = Main.DefilerBirth;
            hiddenEvil.inherit = Main.DefilerInherit;
            //hiddenEvil.base_stats[S.mod_health] += 0.5f;
            //hiddenEvil.base_stats[S.health] += 50f;
            //hiddenEvil.opposite = "Defiler";
            hiddenEvil.group_id = MagicTraitGroup.fel;
            //hiddenEvil.oppositeTraitMod -= 1000;
            hiddenEvil.can_be_given = true;
            AssetManager.traits.add(hiddenEvil);
            addTraitToLocalizedLibrary("en",hiddenEvil.id, "Hidden Evil","Find him before it's too late");
            addTraitToLocalizedLibrary("ru",hiddenEvil.id, "Скрытое Зло","Найди его прежде чем станет поздно");
            PlayerConfig.unlockTrait("hiddenEvil");
            #endregion
        #region Ангелы
            ActorTrait Pacification = new ActorTrait();
            Pacification.action_attack_target = new AttackAction(pacification);
            //Pacification.action_get_hit = new GetHitAction(pacification);
            Pacification.action_special_effect = new WorldAction(pacification1);
            Pacification.id = "Demon Fighter";
            Pacification.path_icon = "ui/Extras/Pacification";
            Pacification.birth = Main.DemonFighterBirth;
            Pacification.inherit = Main.DemonFighterInherit;
            Pacification.base_stats[S.mod_health] += 0.5f;
            Pacification.base_stats[S.health] += 50f;
            Pacification.opposite = "Defiler";
            Pacification.group_id = MagicTraitGroup.fel;
            Pacification.oppositeTraitMod -= 1000;
            Pacification.can_be_given = true;
            AssetManager.traits.add(Pacification);
            addTraitToLocalizedLibrary("en",Pacification.id, Pacification.id,"Demon fighters stop the spread of filth");
            addTraitToLocalizedLibrary("ru",Pacification.id, "Демоноборец","Демоноборцы останавливают распространение скверны");
            PlayerConfig.unlockTrait("Demon Fighter");
        #endregion 
        #region Герой    
            ActorTrait Hero = new ActorTrait();
            Hero.action_attack_target = new AttackAction(pacification);
            //Pacification.action_get_hit = new GetHitAction(pacification);
            Hero.action_special_effect = new WorldAction(pacification1);
            Hero.id = "Hero";
            Hero.path_icon = "ui/Extras/Hero";
            Hero.base_stats[S.mod_health] += 7.77f;
            Hero.base_stats[S.health] += 777f;
            Hero.base_stats[S.damage] += 777f;
            Hero.base_stats[S.armor] += 777f;
            Hero.base_stats[S.speed] += 77f;
            Hero.base_stats[S.attack_speed] += 77f;
            Hero.base_stats[S.warfare] += 77f;
            Hero.base_stats[S.intelligence] -= 777f;
            Hero.opposite = "Defiler";
            Hero.group_id = MagicTraitGroup.fel;
            Hero.oppositeTraitMod -= 1000;
            Hero.can_be_given = true;
            AssetManager.traits.add(Hero);
            addTraitToLocalizedLibrary("en",Hero.id, Hero.id, "Call upon him in the hour of need");
            addTraitToLocalizedLibrary("ru",Hero.id, "Герой", "Призовите его в час нужды");
            PlayerConfig.unlockTrait("Hero");
        #endregion
        #region демоны
            ActorTrait Desecration = new ActorTrait();
            Desecration.action_attack_target = new AttackAction(desecration);
            Desecration.action_special_effect = new WorldAction(desecration2);
            Desecration.action_get_hit = new GetHitAction(desecration);
            Desecration.action_death = new WorldAction(deathDesecration);
            Desecration.birth = Main.DefilerBirth;
            Desecration.inherit = Main.DefilerInherit;
            Desecration.id = "Defiler";
            Desecration.path_icon = "ui/Extras/desecration";
            Desecration.base_stats[S.damage] += 50f;
            Desecration.base_stats[S.attack_speed] += 50f;
            Desecration.group_id = MagicTraitGroup.fel;
            Desecration.can_be_given = true;
            Desecration.opposite = "Demon Fighter";
            //Desecration.oppositeTraitMod -= 100000;
            Desecration.can_be_removed = true;
            AssetManager.traits.add(Desecration);
            addTraitToLocalizedLibrary("en",Desecration.id, Desecration.id,"Demons destroy the universe and the minds of their victims");
            addTraitToLocalizedLibrary("ru",Desecration.id, "Осквернитель","Демоны разрушают мироздание и разумы своих жертв");
            PlayerConfig.unlockTrait("Defiler");
        #endregion 
        #region Некромант
        #endregion 
                /*ActorTrait licantrophy = new ActorTrait();
                licantrophy.action_attack_target = new AttackAction(LicanAtackEffect1);
                licantrophy.action_death = new WorldAction(LicanDeathEffect1);
                licantrophy.id = "Werewolf";
                licantrophy.path_icon = "ui/Extras/iconLicantrophy";
                licantrophy.base_stats[S.attack_speed] += 50f;
                licantrophy.base_stats[S.dodge] += 50f;
                licantrophy.base_stats[S.health] += 200f;
                licantrophy.action_special_effect = (WorldAction)Delegate.Combine(licantrophy.action_special_effect, new WorldAction(LicanRegen));
                licantrophy.group_id = MagicTraitGroup.magic;
                string[] oppositeArrLic = new string[] { "Vampirism", "Vampire", "Elder Vampire", "Lycanthropy", "AndroidPower2" };
                licantrophy.oppositeArr = oppositeArrLic;
                licantrophy.can_be_given = false;
                AssetManager.traits.add(licantrophy);
                addTraitToLocalizedLibrary(licantrophy.id, "Werewolves infect their victims through a bite");
                PlayerConfig.unlockTrait("Werewolf");

                ActorTrait licantrophy2 = new ActorTrait();
                licantrophy2.action_death = new WorldAction(LicanAtackEffect2);
                licantrophy2.id = "Lycanthropy";
                licantrophy2.path_icon = "ui/Extras/iconLicantrophy2";
                licantrophy2.group_id = MagicTraitGroup.magic;
                string[] oppositeArrLic2 = new string[] { "Vampirism", "Vampire", "Elder Vampire", "Werewolf", "Phoenix", "AndroidPower2", "AndroidPower1","Spirit"};
                licantrophy2.oppositeArr = oppositeArrLic2;
                licantrophy2.can_be_cured = true;
                licantrophy2.can_be_given = true;
                AssetManager.traits.add(licantrophy2);
                addTraitToLocalizedLibrary(licantrophy2.id, "He looks suspiciously at the moon");
                PlayerConfig.unlockTrait("Lycanthropy");*/
        #region Вампиры
            ActorTrait vampirism = new ActorTrait();
            vampirism.action_death = new WorldAction(VampireDeathEffect1);
            vampirism.id = "Vampirism";
            vampirism.path_icon = "ui/Extras/Vampirism";
            vampirism.group_id = TraitGroup.acquired;
            string[] oppositeArrVism = new string[] { "Lycanthropy", "Vampire", "Elder Vampire","Werewolf", "Phoenix","AndroidPower2", "AndroidPower1","Spirit" };
            vampirism.oppositeArr = oppositeArrVism;
            vampirism.can_be_given = true;
            AssetManager.traits.add(vampirism);
            addTraitToLocalizedLibrary("en",vampirism.id, vampirism.id,"He doesn't like walking in the sun");
            addTraitToLocalizedLibrary("ru",vampirism.id, "Вампиризм","Ему неприятно ходить под солнцем");
            PlayerConfig.unlockTrait("Vampirism");

            ActorTrait Vampire = new ActorTrait();
            Vampire.action_attack_target = new AttackAction(VampireAtackEffect1);
            Vampire.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
            Vampire.action_special_effect = new WorldAction(VampireDeathEffect2);
            Vampire.id = "Vampire";
            Vampire.path_icon = "ui/Extras/Vampire";
            /*Vampire.base_stats[S.attack_speed] += 75f;
            Vampire.base_stats[S.dodge] += 50f;
            Vampire.base_stats[S.health] += 200f;
            Vampire.base_stats[S.armor] += 20f;
            Vampire.base_stats[S.knockback_reduction] += 10f;
            Vampire.base_stats[S.fertility] -= 5000f;
            Vampire.base_stats[S.max_children] -= 200f;*/
            Vampire.group_id = TraitGroup.special;
            string[] oppositeArrvamp = new string[] { "Vampirism", "AndroidPower2" };
            Vampire.oppositeArr = oppositeArrvamp;
            Vampire.can_be_given = false;
            AssetManager.traits.add(Vampire);
            addTraitToLocalizedLibrary("en",Vampire.id, Vampire.id,"Vampires infect their victims through a bite and drink blood with pleasure");
            addTraitToLocalizedLibrary("ru",Vampire.id, "Вампир","Вампиры заражают своих жертв через укус и с удовольствием пьют кровь");
            PlayerConfig.unlockTrait("Vampire");

            ActorTrait ElderVampire = new ActorTrait();
            ElderVampire.action_attack_target = new AttackAction(VampireAtackEffect2);
            ElderVampire.action_special_effect = new WorldAction(LicanRegen);
            //ElderVampire.action_get_hit = new GetHitAction(VampireAtackEffect3);
            ElderVampire.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
            ElderVampire.id = "Elder Vampire";
            ElderVampire.path_icon = "ui/Extras/ElderVampire";
            ElderVampire.base_stats[S.attack_speed] += 300f;
            ElderVampire.base_stats[S.knockback_reduction] += 1000f;
            ElderVampire.base_stats[S.dodge] += 500f;
            ElderVampire.base_stats[S.health] += 10000f;
            ElderVampire.base_stats[S.armor] += 200f;
            ElderVampire.base_stats[S.fertility] -= 5000f;
            ElderVampire.base_stats[S.max_children] -= 200f;
            ElderVampire.group_id = TraitGroup.special;
            string[] oppositeArrElderVamp = new string[] { "Vampirism", "AndroidPower1" };
            ElderVampire.oppositeArr = oppositeArrElderVamp;
            ElderVampire.can_be_given = true;
            AssetManager.traits.add(ElderVampire);
            addTraitToLocalizedLibrary("en",ElderVampire.id, ElderVampire.id,"Ancient vampires are the millennial horror of the worlds");
            addTraitToLocalizedLibrary("ru",ElderVampire.id, "Древний Вампир","Древние вампиры это тысячелетний ужас миров");
            PlayerConfig.unlockTrait("Elder Vampire");

            ActorTrait bloodsucker = new ActorTrait();
            bloodsucker.action_attack_target = new AttackAction(bloodRestore);
            bloodsucker.id = "Bloodsucker";
            bloodsucker.path_icon = "ui/Extras/bloodsucker";
            bloodsucker.group_id = TraitGroup.spirit;
            bloodsucker.can_be_given = true;
            AssetManager.traits.add(bloodsucker);
            addTraitToLocalizedLibrary("en",bloodsucker.id, bloodsucker.id,"He lives off blood");
            addTraitToLocalizedLibrary("ru",bloodsucker.id, "Кровопийца","Он живет за счет крови");
            PlayerConfig.unlockTrait("Bloodsucker");
        #endregion 
        #region Феникс
            ActorTrait pheonix = new ActorTrait();
            pheonix.id = "Phoenix";
            pheonix.path_icon = "ui/Extras/phoenix";
            pheonix.group_id = TraitGroup.spirit;
            pheonix.birth = 0f;
            pheonix.inherit = 0f;
            pheonix.can_be_given = true;
            pheonix.base_stats[S.mod_health] += 1f;
            pheonix.base_stats[S.knockback_reduction] += 1000f;
            pheonix.base_stats[S.attack_speed] += 50f;
            pheonix.base_stats[S.speed] += 50f;
            //pheonix.base_stats[S.intelligence] += 50f;
            pheonix.action_special_effect = new WorldAction(removeBadTrait);
            pheonix.action_special_effect = (WorldAction)Delegate.Combine(pheonix.action_special_effect, new WorldAction(LizardsRegen1));
            pheonix.action_death =  new WorldAction(rebornANew);
            string[] oppositeArrPhoeenix = new string[] { "Vampirism", "Lycanthropy","Werewolf" };
            pheonix.oppositeArr = oppositeArrPhoeenix;
            AssetManager.traits.add(pheonix);
            addTraitToLocalizedLibrary("en",pheonix.id, pheonix.id,"From the ashes, I will be reborn");
            addTraitToLocalizedLibrary("ru",pheonix.id, "Феникс","Из пепла, я возрожусь");
            PlayerConfig.unlockTrait(pheonix.id);
        #endregion 
        #region Магический дар
            ActorTrait giftOfMagic = new ActorTrait();
            giftOfMagic.id = "Magical Gift";
            //giftOfMagic.action_attack_target = new AttackAction(SubjugationSpirit);
            //giftOfMagic.action_get_hit = new GetHitAction(summonSpirit);
            //giftOfMagic.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
            giftOfMagic.action_death = new WorldAction(ActionLibrary.mageSlayer);
            giftOfMagic.path_icon = "ui/Extras/giftOfMagic";
            giftOfMagic.group_id = MagicTraitGroup.magic;
            giftOfMagic.can_be_given = true;
            giftOfMagic.base_stats[S.mod_health] -= 0.1f;
            giftOfMagic.base_stats[S.speed] -= 10f;
            giftOfMagic.base_stats[S.intelligence] += 5f;
            giftOfMagic.action_special_effect = (WorldAction)Delegate.Combine(giftOfMagic.action_special_effect, new WorldAction(MagicUpgrade));
            giftOfMagic.special_effect_interval = 5f;
            giftOfMagic.birth = Main.magicBirth;
            giftOfMagic.inherit = Main.magicInherit;
            AssetManager.traits.add(giftOfMagic);
            addTraitToLocalizedLibrary("en",giftOfMagic.id, giftOfMagic.id,"A potential magician");
            addTraitToLocalizedLibrary("ru",giftOfMagic.id, "Магический Дар","Потенциальный маг");
            PlayerConfig.unlockTrait(giftOfMagic.id);
        #endregion 
        #region Огонь
                ActorTrait magicOfFire = new ActorTrait();
                magicOfFire.id = "Fire Magic";
                //magicOfFire.action_special_effect = (WorldAction)Delegate.Combine(magicOfFire.action_special_effect, new WorldAction(spellOfFire1));
                magicOfFire.action_attack_target = new AttackAction(spellOfFire1);
                //magicOfFire.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfFire.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                magicOfFire.action_death = new WorldAction(ActionLibrary.mageSlayer);
                magicOfFire.path_icon = "ui/Extras/magicOfFire";
                magicOfFire.group_id = MagicTraitGroup.magic;
                magicOfFire.can_be_given = true;
                magicOfFire.base_stats[S.mod_damage] += 0.5f;
                //magicOfFire.base_stats[S.speed] -= 10f;
                magicOfFire.base_stats[S.intelligence] += 20f;
                magicOfFire.base_stats[S.warfare] += 30f;
                magicOfFire.special_effect_interval = 3f;
                AssetManager.traits.add(magicOfFire);
                addTraitToLocalizedLibrary("en",magicOfFire.id, magicOfFire.id,"Fire Magic");
                addTraitToLocalizedLibrary("ru",magicOfFire.id, "Магия Огня","Магия Огня");
                PlayerConfig.unlockTrait(magicOfFire.id);
                #endregion 
        #region Вода
                ActorTrait magicOfWater = new ActorTrait();
                magicOfWater.id = "Water Magic";
                magicOfWater.action_special_effect = (WorldAction)Delegate.Combine(magicOfWater.action_special_effect, new WorldAction(spellOfWater1));
                magicOfWater.action_attack_target = new AttackAction(spellOfWater2);
                //magicOfFire.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfFire.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                magicOfWater.action_death = new WorldAction(ActionLibrary.mageSlayer);
                magicOfWater.path_icon = "ui/Extras/magicOfWater";
                magicOfWater.group_id = MagicTraitGroup.magic;
                magicOfWater.can_be_given = true;
                magicOfWater.base_stats[S.mod_health] += 0.5f;
                //magicOfFire.base_stats[S.speed] -= 10f;
                magicOfWater.base_stats[S.intelligence] += 20f;
                magicOfWater.base_stats[S.diplomacy] += 30f;
                magicOfWater.special_effect_interval = 3f;
                AssetManager.traits.add(magicOfWater);
                addTraitToLocalizedLibrary("en",magicOfWater.id, magicOfWater.id,"Water Magic");
                addTraitToLocalizedLibrary("ru",magicOfWater.id, "Магия Воды","Магия Воды");
                PlayerConfig.unlockTrait(magicOfWater.id);
                #endregion 
        #region Воздух
                ActorTrait magicOfAir = new ActorTrait();
                magicOfAir.id = "Air Magic";
                //magicOfAir.action_special_effect = (WorldAction)Delegate.Combine(magicOfAir.action_special_effect, new WorldAction(spellOfAir1));
                magicOfAir.action_attack_target = new AttackAction(spellOfAir2);
                //magicOfFire.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfFire.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                magicOfAir.action_death = new WorldAction(ActionLibrary.mageSlayer);
                magicOfAir.path_icon = "ui/Extras/magicOfAir";
                magicOfAir.group_id = MagicTraitGroup.magic;
                magicOfAir.can_be_given = true;
                magicOfAir.base_stats[S.mod_health] += 0.5f;
                //magicOfFire.base_stats[S.speed] -= 10f;
                magicOfAir.base_stats[S.intelligence] += 30f;
                magicOfAir.base_stats[S.diplomacy] += 30f;
                magicOfAir.base_stats[S.stewardship] -= 10f;
                magicOfAir.special_effect_interval = 3f;
                AssetManager.traits.add(magicOfAir);
                addTraitToLocalizedLibrary("en",magicOfAir.id, magicOfAir.id,"Air Magic");
                addTraitToLocalizedLibrary("ru",magicOfAir.id, "Магия Воздуха","Магия Воздуха");
                PlayerConfig.unlockTrait(magicOfAir.id);
                #endregion
        #region Земля
                ActorTrait magicOfEarth = new ActorTrait();
                magicOfEarth.id = "Earth Magic";
                //magicOfEarth.action_special_effect = (WorldAction)Delegate.Combine(magicOfEarth.action_special_effect, new WorldAction(spellOfEarth1));
                magicOfEarth.action_attack_target = new AttackAction(spellOfEarth2);
                //magicOfFire.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfFire.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                magicOfEarth.action_death = new WorldAction(ActionLibrary.mageSlayer);
                magicOfEarth.path_icon = "ui/Extras/magicOfEarth";
                magicOfEarth.group_id = MagicTraitGroup.magic;
                magicOfEarth.can_be_given = true;
                magicOfEarth.base_stats[S.mod_health] += 0.5f;
                //magicOfFire.base_stats[S.speed] -= 10f;
                magicOfEarth.base_stats[S.intelligence] += 20f;
                magicOfEarth.base_stats[S.stewardship] += 30f;
                magicOfEarth.special_effect_interval = 3f;
                AssetManager.traits.add(magicOfEarth);
                addTraitToLocalizedLibrary("en",magicOfEarth.id, magicOfEarth.id,"Earth Magic");
                addTraitToLocalizedLibrary("ru",magicOfEarth.id, "Магия Земли","Магия Земли");
                PlayerConfig.unlockTrait(magicOfEarth.id);
                #endregion
        #region Жизнь
                ActorTrait magicOfLife = new ActorTrait();
                magicOfLife.id = "The Magic of Life";
                magicOfLife.action_special_effect = (WorldAction)Delegate.Combine(magicOfLife.action_special_effect, new WorldAction(spellOfLife1));
                magicOfLife.action_attack_target = new AttackAction(spellOfLife2);
                //magicOfLife.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfLife.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                magicOfLife.action_death = new WorldAction(ActionLibrary.mageSlayer);
                magicOfLife.path_icon = "ui/Extras/magicOfLife";
                magicOfLife.group_id = MagicTraitGroup.magic;
                magicOfLife.can_be_given = true;
                magicOfLife.base_stats[S.mod_health] += 1.5f;
                //magicOfFire.base_stats[S.speed] -= 10f;
                magicOfLife.base_stats[S.intelligence] += 20f;
                magicOfLife.base_stats[S.max_age] += 100f;
                magicOfLife.base_stats[S.diplomacy] += 30f;
                magicOfLife.special_effect_interval = 3f;
                AssetManager.traits.add(magicOfLife);
                addTraitToLocalizedLibrary("en",magicOfLife.id, magicOfLife.id,"The Magic of Life");
                addTraitToLocalizedLibrary("ru",magicOfLife.id, "Магия Жизни","Магия Жизни");
                PlayerConfig.unlockTrait(magicOfLife.id);
        #endregion         
        #region Смерть
                ActorTrait magicOfDeath = new ActorTrait();
                magicOfDeath.id = "The Magic of Death";
                magicOfDeath.action_special_effect = (WorldAction)Delegate.Combine(magicOfWater.action_special_effect, new WorldAction(spellOfDeath1));
                magicOfDeath.action_attack_target = new AttackAction(spellOfDeath2);
                //magicOfDeath.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfDeath.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                magicOfDeath.action_death = new WorldAction(spellOfDeath3);
                magicOfDeath.path_icon = "ui/Extras/magicOfDeath";
                magicOfDeath.group_id = MagicTraitGroup.magic;
                magicOfDeath.can_be_given = true;
                magicOfDeath.base_stats[S.mod_health] += 1.5f;
                //magicOfFire.base_stats[S.speed] -= 10f;
                magicOfDeath.base_stats[S.intelligence] += 20f;
                magicOfDeath.base_stats[S.diplomacy] -= 30f;
                magicOfLife.base_stats[S.max_age] += 100f;
                magicOfDeath.base_stats[S.stewardship] += 30f;
                magicOfDeath.base_stats[S.warfare] += 30f;
                magicOfDeath.special_effect_interval = 3f;
                AssetManager.traits.add(magicOfDeath);
                addTraitToLocalizedLibrary("en",magicOfDeath.id, magicOfDeath.id,"The Magic of Death");
                addTraitToLocalizedLibrary("ru",magicOfDeath.id, "Магия Смерти","Магия Смерти");
                PlayerConfig.unlockTrait(magicOfDeath.id);
                #endregion  
        #region Кровь
                ActorTrait magicOfBlood = new ActorTrait();
                magicOfBlood.id = "Blood Magic";
                magicOfBlood.action_special_effect = (WorldAction)Delegate.Combine(magicOfWater.action_special_effect, new WorldAction(spellOfBlood1));
                magicOfBlood.action_attack_target = new AttackAction(spellOfBlood2);
                //magicOfDeath.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfDeath.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                magicOfBlood.action_death = new WorldAction(spellOfBlood3);
                magicOfBlood.path_icon = "ui/Extras/magicOfBlood";
                magicOfBlood.group_id = MagicTraitGroup.magic;
                magicOfBlood.can_be_given = true;
                magicOfBlood.base_stats[S.mod_health] += 2.5f;
                //magicOfFire.base_stats[S.speed] -= 10f;
                magicOfBlood.base_stats[S.intelligence] += 20f;
                magicOfBlood.base_stats[S.diplomacy] -= 30f;
                magicOfBlood.base_stats[S.stewardship] += 30f;
                magicOfBlood.base_stats[S.warfare] += 30f;
                magicOfBlood.special_effect_interval = 1f;
                AssetManager.traits.add(magicOfBlood);
                addTraitToLocalizedLibrary("en",magicOfBlood.id, magicOfBlood.id,"Blood Magic");
                addTraitToLocalizedLibrary("ru",magicOfBlood.id, "Магия Крови","Магия Крови");
                PlayerConfig.unlockTrait(magicOfBlood.id);
                #endregion
        #region Пространство
                ActorTrait MagicOfSpace = new ActorTrait();
                MagicOfSpace.id = "MagicOfSpace";
                MagicOfSpace.action_special_effect = (WorldAction)Delegate.Combine(magicOfWater.action_special_effect, new WorldAction(SpellOfSpace));
                MagicOfSpace.action_attack_target = new AttackAction(SpellOfSpace1);
                //magicOfDeath.action_get_hit = new GetHitAction(summonSpirit);
                //magicOfDeath.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                //magicOfBlood.action_death = new WorldAction(spellOfBlood3);
                MagicOfSpace.path_icon = "ui/Extras/MagicOfSpace";
                MagicOfSpace.group_id = MagicTraitGroup.magic;
                MagicOfSpace.can_be_given = true;
                MagicOfSpace.base_stats[S.knockback_reduction] += 1f;
                MagicOfSpace.base_stats[S.intelligence] += 30f;
                MagicOfSpace.base_stats[S.warfare] += 30f;
                MagicOfSpace.special_effect_interval = 1f;
                AssetManager.traits.add(MagicOfSpace);
                addTraitToLocalizedLibrary("en",MagicOfSpace.id, "Magic Of Space","Magic Of Space");
                addTraitToLocalizedLibrary("ru",MagicOfSpace.id, "Магия Пространства","Магия пространства");
                PlayerConfig.unlockTrait(MagicOfSpace.id);
                #endregion        
        #region Шаман
                ActorTrait shaman = new ActorTrait();
                shaman.id = "Shaman";
                //shaman.inherit = 30f;
                shaman.action_attack_target = new AttackAction(SubjugationSpirit);
                shaman.action_get_hit = new GetHitAction(summonSpirit);
                //shaman.action_attack_target = (AttackAction)Delegate.Combine(Vampire.action_attack_target, new AttackAction(bloodRestore));
                shaman.action_death = new WorldAction(ActionLibrary.mageSlayer);
                shaman.path_icon = "ui/Extras/Shaman";
                shaman.group_id = MagicTraitGroup.magic;
                shaman.can_be_given = true;
                shaman.base_stats[S.mod_health] += 0.1f;
                shaman.base_stats[S.speed] -= 10f;
                shaman.base_stats[S.intelligence] += 20f;
                shaman.base_stats[S.diplomacy] += 30f;
                //shaman.action_special_effect = (WorldAction)Delegate.Combine(shaman.action_special_effect, new WorldAction(SubjugationSpirit));
                //shaman.special_effect_interval = 10f;
                AssetManager.traits.add(shaman);
                addTraitToLocalizedLibrary("en",shaman.id, shaman.id,"Summoning and subordinating");
                addTraitToLocalizedLibrary("ru",shaman.id, "Шаман","Призывающий и подчиняющий");
                PlayerConfig.unlockTrait(shaman.id);
        #endregion
        #region Духи
                ActorTrait spirit = new ActorTrait();
                spirit.id = "Spirit";
                spirit.path_icon = "ui/Extras/Spirit";
                spirit.group_id = TraitGroup.special;
                spirit.can_be_given = false;
                string[] oppositeArrSpir = new string[] { "Lycanthropy", "AndroidPower2", "Vampirism" };
                spirit.oppositeArr = oppositeArrSpir;
                //spirit.action_special_effect = new WorldAction(following);
                //spirit.special_effect_interval = 1f;
                //spirit.action_attack_target = new AttackAction(SpiritInitiation);
                AssetManager.traits.add(spirit);
                addTraitToLocalizedLibrary("en",spirit.id, spirit.id,"Out of this world");
                addTraitToLocalizedLibrary("ru",spirit.id, "Дух","Не от мира сего");
                PlayerConfig.unlockTrait(spirit.id);

                ActorTrait chained = new ActorTrait();
                chained.id = "Subordinate";
                chained.path_icon = "ui/Extras/chained";
                chained.group_id = TraitGroup.special;
                chained.can_be_given = false;
                string[] oppositeArrChain = new string[] { "Lycanthropy", "AndroidPower2", "Vampirism" };
                chained.oppositeArr = oppositeArrChain;
                chained.action_special_effect = new WorldAction(following);
                chained.special_effect_interval = 2f;
                //spirit.action_attack_target = new AttackAction(SpiritInitiation);
                AssetManager.traits.add(chained);
                addTraitToLocalizedLibrary("en",chained.id, chained.id,"Called and obeying");
                addTraitToLocalizedLibrary("ru",chained.id, "Подчиненный","Призванный и подчиняющийся");
                PlayerConfig.unlockTrait(chained.id);
        #endregion
        #region регенерация Ящеров
         ActorTrait LizardRegen = new ActorTrait();
         //LizardRegen.action_attack_target = new AttackAction(LicanAtackEffect1);
         LizardRegen.id = "Regeneration of the Lizard";
         LizardRegen.path_icon = "ui/Extras/LizardRegen";
         LizardRegen.action_get_hit = new GetHitAction(LizardsRegen);
         LizardRegen.action_special_effect = (WorldAction)Delegate.Combine(LizardRegen.action_special_effect, new WorldAction(LizardsRegen1));
         LizardRegen.group_id = MagicTraitGroup.fel;
         LizardRegen.can_be_given = true;
         AssetManager.traits.add(LizardRegen);
         addTraitToLocalizedLibrary("en",LizardRegen.id, LizardRegen.id,"Lizards must be killed with one blow");
         addTraitToLocalizedLibrary("ru",LizardRegen.id, "Регенерация Ящера","Ящеров надо убивать одним ударом");
         PlayerConfig.unlockTrait("Regeneration of the Lizard");
#endregion         
#endregion       
        }

#region Эффекты
#region Магия
        public static bool MagicUpgrade(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            Actor a = pTarget.a;
            int hungVal = a.data.hunger + 3;
            hungVal = Mathf.Clamp(hungVal, 1, 100);
            a.data.hunger = hungVal;
            if (a.getAge()>30)
            {
                if(a.isRace(SK.orc))
                {
                    if (Toolbox.randomChance(0.9f) && !a.hasTrait("Air Magic"))
                        a.addTrait("Shaman");
                    else if (!a.hasTrait("Shaman"))
                        a.addTrait("Air Magic");
                }
                if (a.isRace(SK.human))
                {
                    if (Toolbox.randomChance(0.8f) && !a.hasTrait("Water Magic"))
                        {
                        a.addTrait("Fire Magic");
                        a.addTrait("Air Magic");
                        }
                    else if (!a.hasTrait("Fire Magic"))
                       {
                        a.addTrait("Water Magic");
                        a.addTrait("Earth Magic");
                       } 
                }
                if (a.isRace(SK.dwarf))
                {
                    if (Toolbox.randomChance(0.4f) && !a.hasTrait("Earth Magic"))
                        a.addTrait("Fire Magic");
                    else if (!a.hasTrait("Fire Magic"))
                        a.addTrait("Earth Magic");
                }
                if (a.isRace(SK.elf))
                {
                    if (Toolbox.randomChance(0.5f) && !a.hasTrait("Water Magic"))
                        a.addTrait("The Magic of Life");
                    else if (!a.hasTrait("The Magic of Life"))
                        a.addTrait("Water Magic");
                }
                if (a.isRace("goblin"))
                {
                    if (Toolbox.randomChance(0.6f) && !a.hasTrait("The Magic of Death"))
                        a.addTrait("Shaman");
                    else if (!a.hasTrait("Shaman"))
                        a.addTrait("The Magic of Death");
                }
                if (a.isRace("lizard"))
                {
                    if (Toolbox.randomChance(0.5f) && !a.hasTrait("Fire Magic"))
                        a.addTrait("Air Magic");
                    else if (!a.hasTrait("Air Magic"))
                        a.addTrait("Fire Magic");
                }
                if (a.isRace("beastmen"))
                {
                    if (Toolbox.randomChance(0.5f) && !a.hasTrait("The Magic of Life"))
                        a.addTrait("Shaman");
                    else if (!a.hasTrait("Shaman"))
                        a.addTrait("The Magic of Life");
                }
                if (a.isRace("vampire"))
                {
                    if (Toolbox.randomChance(0.5f) && !a.hasTrait("The Magic of Death"))
                        a.addTrait("MagicOfSpace");
                    else if (!a.hasTrait("MagicOfSpace"))
                        a.addTrait("The Magic of Death");
                }
                if (a.isRace("ancientchina"))
                {
                    a.addTrait("Shaman");
                }
                if (a.isRace("demonic"))
                {
                    if (Toolbox.randomChance(0.6f) && !a.hasTrait("Earth Magic"))
                        a.addTrait("Fire Magic");
                    else if (!a.hasTrait("Fire Magic"))
                        a.addTrait("Earth Magic");
                }
                if (a.isRace("angel"))
                {
                    if (Toolbox.randomChance(0.4f) && !a.hasTrait("Air Magic"))
                        a.addTrait("Water Magic");
                    else if (!a.hasTrait("Water Magic"))
                        a.addTrait("Air Magic");
                }
                if (a.isRace("gnome"))
                {
                    if (Toolbox.randomChance(0.6f) && !a.hasTrait("Earth Magic"))
                        a.addTrait("Fire Magic");
                    else if (!a.hasTrait("Fire Magic"))
                        a.addTrait("Earth Magic");
                }
                if (a.isRace("darkelve"))
                {
                    if (Toolbox.randomChance(0.5f) && !a.hasTrait("Earth Magic"))
                        a.addTrait("The Magic of Death");
                    else if (!a.hasTrait("The Magic of Death"))
                        a.addTrait("Earth Magic");
                }
            }
                
         }
      		return true;
        }
        #endregion
#region Огонь
//Огонь
        public static bool spellOfFire1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isBuilding())
            {
                if (Toolbox.randomChance(0.05f) && pSelf.a.data.hunger > 90)
                {
                    pSelf.a.addStatusEffect("invincible");
                    Protect(pSelf, 10);
                    EffectsLibrary.spawn("fx_meteorite", pTarget.currentTile, "meteorite_disaster", null, 0f, -1f, -1f);
                    spellCost(pSelf, 90);
                }
            }
            if (pTarget.isAlive())
            {
                
                if (pTarget.isActor() && Toolbox.randomChance(0.33f) && pSelf.a.data.hunger > 10)
                {
                    addStatusOnTarget(pSelf, pTarget, "burning", 10);
                }
                else if (pSelf.a.data.hunger > 20 && Toolbox.randomChance(0.33f))
                {
                    addStatusOnTarget(pSelf, pSelf, "fireEnhancement", 20);
                }
                else if (Toolbox.randomChance(0.5f) && pSelf.a.data.hunger > 50)
                {
                    summon(pSelf, "Fire_spirit", 50);
                }
                
            }
            
         }
      	 return true;
        }
        #endregion
#region Вода
//Вода
        public static bool spellOfWater1(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive())
            {
                if (pTarget.a.data.hunger > 10 && pTarget.hasStatus("burning"))
            {
                MagicSpells.CastDrop(3f,"rain",pTarget);
                spellCost(pTarget,10);
            }
            else if (pTarget.a.data.hunger > 90 && Toolbox.randomChance(0.1f))
            {
                if (pTile == null)
                    pTile = pTarget.currentTile;
                if (pTile == null)
                    return false;
                AssetManager.powers.spawnCloudRain(pTile,"cloudRain");
                spellCost(pTarget,90);
            }
            }
            
         }
      	 return true;
        }

        public static bool spellOfWater2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive())
            {
                if (Toolbox.randomChance(0.2f) && pSelf.a.data.hunger > 50)
            {
                summon(pSelf, "water_spirit", 50);
            }
            else if (Toolbox.randomChance(0.6f) && pSelf.a.data.hunger > 20)
            {
                addStatusOnTarget(pSelf, pSelf, "waterEnhancement", 20);
            }
            }
            
         }
         return true;
        }
        #endregion
#region Воздух
//Воздух
       /* public static bool spellOfAir1(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.a.data.hunger > 10 && pTarget.hasStatus("burning"))
            {
               
                spellCost(pTarget,10);
            }
            if (pTarget.a.data.hunger > 30 && Toolbox.randomChance(0.01f))
            {
                if (pTile == null)
                    pTile = pTarget.currentTile;
                if (pTile == null)
                    return false;
                AssetManager.powers.spawnCloudRain(pTile,"cloudRain");
                spellCost(pTarget,90);
            }
         }
      	 return true;
        }*/

        public static bool spellOfAir2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive())
            {
                if (Toolbox.randomChance(0.03f) && pSelf.a.data.hunger > 90)
                {
                    pSelf.a.addStatusEffect("invincible");
                    ActionLibrary.castTornado(pSelf,pTarget);
                    Protect(pSelf,10);
                    spellCost(pSelf,90);
                }
                else if (Toolbox.randomChance(0.25f) && pSelf.a.data.hunger > 10)
                {
                    MapBox.spawnLightningSmall(pTarget.currentTile,0.1f);
                    spellCost(pSelf,10);
                }
                else if (Toolbox.randomChance(0.33f) && pSelf.a.data.hunger > 20)
                {
                    addStatusOnTarget(pSelf, pSelf, "airEnhancement", 20);
                }
                else if (Toolbox.randomChance(0.33f) && pSelf.a.data.hunger > 30)
                {
                    MapBox.spawnLightningMedium(pTarget.currentTile);
                    spellCost(pSelf,30);
                }
                else if (Toolbox.randomChance(0.5f) && pSelf.a.data.hunger > 50)
                {
                    summon(pSelf, "air_spirit", 50);
                }
                
                
            }
            
         }
         return true;
        }
        #endregion
#region Земля
//Земля
        public static bool spellOfEarth1(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
         }
      	 return true;
        }

        public static bool spellOfEarth2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isBuilding())
            {
               if (pSelf.a.data.hunger > 90 && Toolbox.randomChance(0.05f))
                {
                    pSelf.a.addStatusEffect("invincible");
                    Protect(pSelf,10);
                    MagicSpells.CastEarthquake(pSelf,pTarget);
                    spellCost(pSelf,90);
                }
            }
            if (pTarget.isAlive())
            {
                
                if (pSelf.a.data.hunger > 10 && Toolbox.randomChance(0.33f))
                {
                    addStatusOnTarget(pSelf, pTarget, "slowness", 10);
                } 
                else if (Toolbox.randomChance(0.33f) && pSelf.a.data.hunger > 20)
                {
                    addStatusOnTarget(pSelf, pSelf, "earthEnhancement", 20);
                }
                else if (Toolbox.randomChance(0.5f) && pSelf.a.data.hunger > 50)
                {
                    if(Toolbox.randomChance(0.5f))
                    {
                        summon(pSelf, "earth_spirit", 50);
                    }
                    else
                    {
                        summon(pSelf, "crystal_golem", 50);
                    }
                }
                
            }

         }
         return true;
        }
        #endregion
#region Жизнь
//Жизнь
        public static bool spellOfLife1(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive())
            {
                if (pTarget.a.hasTrait("death_mark") && (pTarget.a.data.hunger > 1 || pTarget.a.isRace("good")))
            {
                removeBadTrait(pTarget);
                spellCost(pTarget, 1);
            }
            
            if (Toolbox.randomChance(0.1f) && (pTarget.a.data.hunger > 10 || pTarget.a.isRace("good")))
            {
                healing(pTarget,10, 0.5f);
                //spellCost(pTarget,10);
            }
            Regen(pTarget, 100);
            }

            
         }
      	 return true;
        }

        public static bool spellOfLife2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive())
            {
                if (Toolbox.randomChance(0.33f) && (pSelf.a.data.hunger > 30 || pSelf.a.isRace("good")))
                {
                    
                    if(Toolbox.randomChance(0.2f))
                    {
                        summon(pSelf, "druid", 30);
                    }
                    else if(Toolbox.randomChance(0.25f))
                    {
                        MagicSpells.CastDrop(5f,"fertilizerTrees",pSelf);
                        MagicSpells.CastDrop(5f,"livingPlants",pSelf);
                        spellCost(pSelf,30);
                        //summon(pSelf, "livingPlants", 30);
                    }
                    else if(Toolbox.randomChance(0.3f))
                    {
                        summon(pSelf, "bear", 30);
                    }
                    else if(Toolbox.randomChance(0.5f))
                    {
                        summon(pSelf, "snake", 30);
                    }
                    else 
                    {
                        summon(pSelf, "monkey", 30);
                    }
                }
                else if ((pSelf.a.data.hunger > 50 || pSelf.a.isRace("good")) && Toolbox.randomChance(0.33f))
                {
                    addStatusOnTarget(pSelf, pTarget, "ash_fever", 25);
                    addStatusOnTarget(pSelf, pTarget, "cough", 25);
                }
            }
            
         }
         return true;
        }
        #endregion
#region Смерть
//Смерть
        public static bool spellOfDeath1(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive())
            {
                Actor a = pTarget.a;
                if(!deadBodies.ContainsKey(a))
                {
                    deadBodies.Add(a,0);
                }
                if(Main.NewMagicOfDeath)
                {
                    deadBodies[a]+=pTarget.a.data.kills;
                    a.data.kills=0; 
                }
                
                if (pTarget.a.hasTrait("death_mark") && (pTarget.a.data.hunger > 10 || pTarget.a.isRace("necromancer")))
                {
                    pTarget.a.removeTrait("death_mark");
                    spellCost(pTarget, 10);
                }
                if (Toolbox.randomChance(0.3f) && 
                (pTarget.a.data.hunger > 30 || pTarget.a.isRace("necromancer")) &&
                Main.NewMagicOfDeath)
                {
                    if (deadBodies[a]>0)
                    {
                        if(a.hasTrait("mageslayer"))
                        {
                            necroSummon(pTarget, "necromancer");
                            a.removeTrait("mageslayer");
                        }
                        else if(Toolbox.randomChance(0.4f))
                        {
                            necroSummon(pTarget, "skeleton","tough");
                        }
                        else if(Toolbox.randomChance(0.5f))
                        {
                            necroSummon(pTarget, "skeleton","agile");
                        }
                        else if(Toolbox.randomChance(0.142857f))
                        {
                            necroSummon(pTarget, "zombie_orc","strong");
                        }
                        else if(Toolbox.randomChance(0.166f))
                        {
                            necroSummon(pTarget, "zombie_orc","fast");
                        }
                        else if(Toolbox.randomChance(0.2f))
                        {
                            necroSummon(pTarget, "zombie_orc","agile");
                        }
                        else if(Toolbox.randomChance(0.25f))
                        {
                            necroSummon(pTarget, "skeleton","eagle_eyed");
                        }
                        else if(Toolbox.randomChance(0.333f))
                        {
                            necroSummon(pTarget, "zombie_orc","venomous");
                        }
                        else if(Toolbox.randomChance(0.5f))
                        {
                            necroSummon(pTarget, "zombie_orc","giant");
                        }
                        else 
                        {
                            necroSummon(pTarget, "ghost","fast");
                        }
                        spellCost(pTarget,30);
                    }
                    
                
            }
            Regen(pTarget, 50);
            }

            
         }
      	 return true;
        }
        #region Некромантия

        
         public static bool necroSummon(BaseSimObject pTarget, string entity, string trait = "Subordinate", WorldTile pTile = null)
        {
            if (pTarget!=null)
            {
                if (pTarget.isAlive())
                {
                    Actor a = pTarget.a;
                    deadBodies[a]-=1;
                    Actor act = World.world.units.createNewUnit(entity, a.currentTile, 0f);
                    act.kingdom = pTarget.kingdom;
                    pTarget.kingdom.addUnit(act);
                    act.addTrait("Subordinate");
                    act.addTrait("evil");
                    act.addTrait(trait);
                    //act.data.setName(pTarget.a.getName());
                    Main.listOfTamedBeasts.Add(act, pTarget.a);
                    EffectsLibrary.spawn("fx_spawn", act.currentTile, null, null, 0f, -1f, -1f);
                }
            }
            return true;
        }
#endregion

        public static bool spellOfDeath2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive() && pTarget.isActor())
            {
                if (Toolbox.randomChance(0.33f) && 
                (pSelf.a.data.hunger > 30 || pSelf.a.isRace("necromancer")) &&
                !Main.NewMagicOfDeath)
                {
                
                    if(Toolbox.randomChance(0.01f))
                    {
                        summon(pSelf, "necromancer", 30);
                    }
                    else if(Toolbox.randomChance(0.25f))
                    {
                        summon(pSelf, "cursed_skeleton", 30);
                    }
                    else if(Toolbox.randomChance(0.333f))
                    {
                        summon(pSelf, "skeleton", 30);
                    }
                    else if(Toolbox.randomChance(0.5f))
                    {
                        summon(pSelf, "zombie_orc", 30);
                    }
                    else 
                    {
                        summon(pSelf, "ghost", 30);
                    }
                
                }
                else if ((pSelf.a.data.hunger > 50 || pSelf.a.isRace("necromancer")) && Toolbox.randomChance(0.33f))
                {
                    addStatusOnTarget(pSelf, pTarget, "poisoned", 25);
                    addStatusOnTarget(pSelf, pTarget, "curse", 25);
                    //addStatusOnTarget(pSelf, pTarget, "cough", 25);
                }
                else if ((pSelf.a.data.hunger > 90 || pSelf.a.isRace("necromancer")) && Toolbox.randomChance(0.01f))
                {
                    pTarget.a.addTrait("death_mark");
                    spellCost(pSelf,90);
                    //addStatusOnTarget(pSelf, pTarget, "cough", 25);
                }
            }
            
         }
         return true;
        }
        public static bool spellOfDeath3(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null && 
         !pTarget.a.isRace("necromancer") && 
         !(pTarget.a.asset.id=="ghost") && 
         !pTarget.a.hasTrait("Werewolf"))
         {
            removeInfectTrait(pTarget);
            reborn(pTarget,"necromancer",pTile,true);
            pTarget.a.removeTrait("Blood Magic");
            ActionLibrary.mageSlayer(pTarget);
            return true;
         }
      	 return true;
        }
        #endregion
#region Кровь
//Кровь
        public static bool spellOfBlood1(BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null)
         {
            if (pTarget.isAlive())
            {
                if (!bloodEnhancement.ContainsKey(pTarget.a))
                {
                    MagicTraits.bloodEnhancement.Add(pTarget.a,new bloodStats());
                }
            if (pTarget.a.hasTrait("madness"))
            {
                bloodMadness(pTarget,0.5f);
                pTarget.a.addTrait("Rhjdfdsq lj;lm");
                spellCost(pTarget,1,1);
                //spellCost(pTarget,0,World.world.temp_map_objects.Count*10-10);
            }
            if (pTarget.a.data.health<=10)
            {
                pTarget.a.removeTrait("strong_minded");
                pTarget.a.addTrait("madness");
            }
            }
            
            //Regen(pTarget, 10);
            //pTarget.a.removeTrait("Rhjdfdsq lj;lm");
         }
      	 return true;
        }

        public static bool spellOfBlood2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
      	{
         if (pTarget != null && pTarget.isActor() && pTarget.isAlive())
         {
            if (!bloodEnhancement.ContainsKey(pSelf.a))
                {
                    MagicTraits.bloodEnhancement.Add(pSelf.a,new bloodStats());
                }
            if ((pSelf.a.data.health > 70) && Toolbox.randomChance(0.33f) && pTarget != null)
            {
                bloodEnhancement[pSelf.a].health+=10f;
                bloodEnhancement[pSelf.a].max_age+=1;
                bloodEnhancement[pSelf.a].speed+=0.5f;
                bloodEnhancement[pSelf.a].attack_speed+=0.1f;
                
                pSelf.a.addTrait("Rhjdfdsq lj;lm");
                //pSelf.a.stats[S.health]+=10f;
                //pSelf.a.stats[S.max_age]+=1f;
                //pSelf.a.stats[S.damage]+=0.1f;
                //pSelf.a.restoreHealth(10);
                spellCost(pTarget,0,10);
                spellCost(pSelf,0,60);
                //addStatusOnTarget(pSelf, pTarget, "cough", 25);
            }
            else if ((pSelf.a.data.health > 95) && Toolbox.randomChance(0.25f) && pTarget != null)
            {
                spellCost(pTarget,0,100);
                bloodEnhancement[pSelf.a].armor+=1f;
                pSelf.a.addTrait("Rhjdfdsq lj;lm");
                
                //pSelf.a.stats[S.armor]+=1f;
                spellCost(pSelf,0,90);
                //addStatusOnTarget(pSelf, pTarget, "cough", 25);
            }
            else if (Toolbox.randomChance(0.2f) && (pSelf.a.data.health > 140) && pTarget != null)
            {
                spellCost(pTarget,0,10);
                bloodEnhancement[pSelf.a].damage+=1f;
                //pSelf.a.stats[S.speed]+=0.1f;
                //pSelf.a.stats[S.attack_speed]+=0.01f;
                pSelf.a.addTrait("Rhjdfdsq lj;lm");
                spellCost(pSelf,0,130);
            }
            
            //pSelf.a.removeTrait("Rhjdfdsq lj;lm");
            bloodRestore(pSelf,pTarget);
         }
         return true;
        }
        public static bool spellOfBlood3(BaseSimObject pTarget, WorldTile pTile = null)
      	{
            foreach (string race in Main.Races)
            {
                if (pTarget != null && 
                !pTarget.a.isRace("vampire") && 
                !(pTarget.a.asset.id=="ghost") && 
                !pTarget.a.isRace("necromancer")&& 
                pTarget.a.isRace(race)&& 
                !pTarget.a.hasTrait("Werewolf"))
                {
                    pTarget.a.removeTrait("Werewolf");
                    pTarget.a.removeTrait("Lycanthropy");
                    pTarget.a.removeTrait("Phoenix");
                    removeInfectTrait(pTarget);
                    reborn(pTarget,"unit_vampire",pTile);
                    ActionLibrary.mageSlayer(pTarget);
                }
            }
      	 return true;
        }
        public static bool bloodMadness(BaseSimObject pTarget,float pChance = 0.2f, int pRad = 10, int pHealth = 10, WorldTile pTile = null)
        {
            if (Toolbox.randomChance(pChance))
            {
                World.world.getObjectsInChunks(pTarget.currentTile, pRad, MapObjectType.Actor);
                for (int index = 0; index < World.world.temp_map_objects.Count; ++index)
                {
                    Actor tempMapObject = (Actor) World.world.temp_map_objects[index];
                    if (tempMapObject != pTarget.a && 
                    !tempMapObject.isRace("vampire"))
                    {
                        if(tempMapObject.data.health<pHealth)
                        {
                            tempMapObject.killHimself();
                            tempMapObject.getHit(877,false, AttackType.Eaten, pTarget, false, false);
                            //pTarget.a.data.kills++;
                            //pTarget.a.data.experience += 10;
                            //tempMapObject.data.health -= tempMapObject.data.health;
                            continue;
                        }
                        tempMapObject.data.health -= pHealth;
                        tempMapObject.getHit(1,false, AttackType.Eaten, pTarget, false, false);
                        tempMapObject.spawnParticle(Toolbox.color_red);
                        //pTarget.a.stats[S.health]+=pHealth/3;
                        bloodEnhancement[pTarget.a].health+=pHealth/3;
                        pTarget.a.restoreHealth(pHealth);
                        pTarget.a.data.diplomacy -= 1;
                        pTarget.a.data.intelligence -= 1;
                        //pTarget.a.restoreHealth(pHealth);
                    }
                    
                }
            }
            return true;
        }
        #endregion
#region Шаманизм
        public static bool summonSpirit(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (Toolbox.randomChance(0.7f) && pTarget != null && pSelf.a.data.hunger>30)
            {
                    if(Toolbox.randomChance(0.25f))
                    {
                        summon(pSelf, "water_spirit", 20);
                    }
                    else if (Toolbox.randomChance(0.33f))
                    {
                        summon(pSelf, "Fire_spirit", 20);
                    }
                    else if (Toolbox.randomChance(0.5f))
                    {
                        summon(pSelf, "earth_spirit", 20);
                    }
                    else
                    {
                        summon(pSelf, "air_spirit", 20);
                    }
            }
            

            return true;
        }
        
        public static bool SubjugationSpirit(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget!=null && pTarget.isActor() ){
                Actor b = pTarget.a;
                if (b.hasTrait("Spirit") && Toolbox.randomChance(0.7f) && pSelf.a.data.hunger>10)
                {
                    pSelf.a.data.hunger -= 10;
                    obey(pSelf,pTarget);
                }
            }
            return true;
        }
        #endregion
#region Пространство
        public static bool SpellOfSpace (BaseSimObject pTarget, WorldTile pTile = null)
        {
            
            if (pTarget != null)
            {
                if (pTarget.isAlive())
                {
                    Actor pActor = pTarget.a;
                    if (pTarget.a.has_attack_target && 
                    pTarget.a.attackTarget.isActor() && 
                    pTarget.a.attackTarget.isAlive() &&
                    pTarget.a.attackTarget!=null)
                    {
                        Actor enemy = pTarget.a.attackTarget.a;
                        if (pTarget.a.s_attackType == WeaponType.Melee && 
                        Toolbox.DistTile(enemy.currentTile, pActor.currentTile) > pActor.stats[S.range] &&
                        pTarget.a.data.hunger>10)
                        {
                            BaseSimObject pAttackTarget = pTarget.a.attackTarget;
                            teleportToTarget(pTarget, enemy.currentTile);
                            //setAttackTarget(pTarget, pAttackTarget);
                            spellCost(pTarget,10);
                        }
                        if (pTarget.a.s_attackType == WeaponType.Range && 
                        (Toolbox.DistTile(enemy.currentTile, pActor.currentTile)<=5 || 
                        Toolbox.DistTile(enemy.currentTile, pActor.currentTile) > pActor.stats[S.range]) &&
                        pTarget.a.data.hunger>10)
                        {
                            
                            BaseSimObject pAttackTarget = pTarget.a.attackTarget;
                            teleportToTarget(pTarget, enemy.currentTile.region.tiles.GetRandom<WorldTile>());
                            //setAttackTarget(pTarget, pAttackTarget);
                            spellCost(pTarget,10);
                        }
                    }
                    if (Toolbox.randomChance(0.3f) && pTarget.a.data.health<60 && pTarget.a.data.hunger>10)
                    {
                        WorldTile SafeTile = pTarget.currentTile.region.tiles.GetRandom<WorldTile>();
                        if (SafeTile.isSameIsland(pActor.currentTile))
                        {
                            if (teleportToTarget(pTarget, SafeTile))
                            {
                                pTarget.a.restoreHealth(30);
                                spellCost(pTarget,10);
                            }
                        }

                    }
                }
            }
            return true;
        }
        public static bool SpellOfSpace1 (BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
        
            if (pTarget != null)
            {
                if (pTarget.isAlive())
                {
                    if(Toolbox.randomChance(0.05f) && pSelf.a.data.hunger>90)
                    {
                        Protect(pSelf,5,60);
                        spellCost(pSelf, 90);
                    }
                    else if (pSelf.a.data.hunger > pSelf.kingdom.units.Count*10)
                    {
                        Evacuation(pSelf);
                    }
                }
                
            }
            
            return true;
        }
        #endregion
#region Вампиры
//Вампирские эффекты
        private static bool bloodRestore(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget != null && pTarget.isActor())
            {
                if ((double) Toolbox.DistTile(pSelf.a.currentTile, pTarget.a.currentTile) < 2.0)
                {
                    int hungVal = pSelf.a.data.hunger + 5;
                    hungVal = Mathf.Clamp(hungVal, 1, 100);
                    pSelf.a.data.hunger = hungVal;
                    pSelf.a.restoreHealth(30);
                }
                
            }
            return true;
        }

        public static bool VampireAtackEffect1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) 
      	{
         if (pTarget != null && pTarget.isActor() && 
         !pTarget.a.hasTrait("Vampirism") &&
         !pTarget.a.hasTrait("The Magic of Life") &&
         !pTarget.a.hasTrait("The Magic of Death"))
         {
            Actor b = pTarget.a;
            foreach (string race in Main.Races)
            {
                if (b.isRace(race))
                {
                    if (Toolbox.randomChance(0.3f) && 
                    !Main.listOfKingdoms.ContainsKey(b) &&
                    (double) Toolbox.DistTile(pSelf.a.currentTile, pTarget.a.currentTile) < 2.0)
                    { 
                        Main.listOfKingdoms.Add(b, pSelf.a.kingdom);
                        b.addTrait("Vampirism");
                        bloodRestore(pSelf,pTarget);
                        return true;
                    }
                    continue;
                }
                
            }
            bloodRestore(pSelf,pTarget);
         }
         return false;

        }

        public static bool VampireAtackEffect2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) 
      	{
         if (pTarget != null && pTarget.isActor() && 
         !pTarget.a.hasTrait("Vampirism") &&
         !pTarget.a.hasTrait("The Magic of Life") &&
         !pTarget.a.hasTrait("The Magic of Death"))
         {
            Actor b = pTarget.a;//Reflection.GetField(pTarget.GetType(), pTarget, "a") as Actor;
            foreach (string race in Main.Races)
            {
                if (b.isRace(race))
                {
                    if (Toolbox.randomChance(0.9f) && 
                    !Main.listOfKingdoms.ContainsKey(b)&&
                    (double) Toolbox.DistTile(pSelf.a.currentTile, pTarget.a.currentTile) < 2.0)
                    { 
                        Main.listOfKingdoms.Add(b, pSelf.a.kingdom);
                        
                        b.addTrait("Vampirism");
                        bloodRestore(pSelf,pTarget);
                        return true;
                    }
                    
                }

            }
            bloodRestore(pSelf,pTarget);
         }
         return false;

        }

        public static bool VampireDeathEffect1(BaseSimObject pTarget, WorldTile pTile = null) 
      	{
         if (pTarget != null)
         {
            removeInfectTrait(pTarget);
            reborn(pTarget,"unit_vampire", pTile);
            
            
         }
         return true;
        }
        
        
        public static bool VampireDeathEffect2(BaseSimObject pTarget, WorldTile pTile = null) 
      	{
         if (pTarget != null)
         {
            Actor a = pTarget.a;
            if (a.getAge()>Main.BloodAge)
            {
                a.addTrait("Blood Magic");
            }
            if (a.getAge()>1000){
                removeInfectTrait(pTarget);
                a.addTrait("Elder Vampire");
                //a.addTrait("SSS");
                a.removeTrait("cursed");
                a.data.health += 10000;
            }
            

         }
         return true;
        }
        #endregion
#region Оборотни
//Оборотни
        public static bool LicanAtackEffect1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) 
      	{
         if (pTarget != null && 
         pTarget.isActor() && 
         !pTarget.a.hasTrait("Lycanthropy") &&
         !pTarget.a.hasTrait("The Magic of Life") &&
         !pTarget.a.hasTrait("The Magic of Death"))
         {
            Actor b = Reflection.GetField(pTarget.GetType(), pTarget, "a") as Actor;
            foreach (string race in Main.Races)
            {
                if (b.isRace(race))
                {
                    if (Toolbox.randomChance(0.05f) && !Main.listOfKingdoms.ContainsKey(b))
                    { 
                        //b.kingdom = pSelf.kingdom;
                        Main.listOfKingdoms.Add(b, pSelf.a.kingdom);
                        b.addTrait("Lycanthropy");
                        return true;
                    }
                    continue;
                }
            }
         }
         return false;

        }
        public static bool LicanDeathEffect1(BaseSimObject pTarget, WorldTile pTile = null)
        {
        return true;
        }
        public static bool LicanAtackEffect2(BaseSimObject pTarget, WorldTile pTile = null)
        
        {
            Actor a = pTarget.a;
            //a.addTrait("Werewolf");
            removeInfectTrait(pTarget);
            reborn(pTarget,"unit_beastmen",pTile);
            return true;

        }
        public static bool LicanRegen(BaseSimObject pTarget, WorldTile pTile = null)
        {
            Regen(pTarget,100);
            return true;
        }
        #endregion
#region Демоноборцы
//Ангелы
        public static bool pacification(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget != null)
            {
                if (pTarget.isActor())
                {
                    Actor a = pSelf.a;
                    //a.addTrait("strong_minded");
                    Actor b = pTarget.a;
                    if (b.isRace("demon") || b.isRace("demonit"))
                    {
                        spellCost(pTarget,0,650);
                    }
                    if (b.isRace("demonic") || b.hasTrait("Defiler"))
                    {
                        spellCost(pTarget,0,77);
                    }
                    if (a.hasTrait("Hero") && (b.asset.id == "demonKing") && b.data.health == 1)
                    {
                        //spellCost(pTarget,0,1250);
                        b.killHimself();
                        a.data.kills++;
                        a.addTrait("kingslayer");
                    }
                        return true;
                    
                }
            }
            return false;
            //}
        }
        public static bool pacification1 (BaseSimObject pTarget, WorldTile pTile = null)
        {
            Regen(pTarget,100);
            Actor pActor = pTarget.a;
            if (pActor.hasTrait("Hero"))
            {
                if (BehaviourActionBase<Actor>.world.worldLaws.world_law_peaceful_monsters.boolVal)
                    return false;
                Actor King = (Actor) null;
                float num1 = 0.0f;
                foreach (Actor Kings in Main.DemonKing)
                {
                if (Kings.currentTile.isSameIsland(pActor.currentTile))
                    {
                        float num2 = Toolbox.DistTile(Kings.currentTile, pActor.currentTile);
                        if ( King == null || (double) num2 < (double) num1)
                        {
                            King = Kings;
                            num1 = num2;
                        }
                    }
                }
                if ( King !=  null)
                {
                    pActor.goTo(King.currentTile);
                    return true;
                }
            }  
            if (Toolbox.randomChance(0.3f))
            {
                goToTarget(pTarget,"lighthouses",pTile);
            }
            
            return true;
        }
        #endregion
#region Осквернители
//Демоны
        public static bool desecration(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) 
      	{
            
         if (pTarget != null)
         {
            if (pTarget.isActor())
            {
                Actor a = pTarget.a;
                    if (
                    (good_defiler.ContainsKey(pSelf.a.asset.id)) )
                    { 
                        if (Toolbox.randomChance(good_defiler[pSelf.a.asset.id]))
                        {
                            desecration1(pSelf,pTarget);
                        }
                    }
                    else if (Toolbox.randomChance(0.07f) && 
                    !(a.hasTrait("Vampire")))
                    {
                        desecration1(pSelf,pTarget);
                    }
                
                if (Toolbox.randomChance(0.07f))
                {
                    foreach (string race in Main.Races)
                    {
                        if ((a.isRace(race)))
                        {
                            a.addTrait("Demon Fighter");
                        }
                    }
                }
                
                return true;
            }
         }
         return false;

        }
        
        public static bool desecration1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
            if(pTarget!=null && pTarget.isActor())
            {
                if (!(pTarget.a.hasTrait("Demon Fighter")) && 
                !(pTarget.a.hasTrait("Defiler")) &&
                !(pTarget.a.hasTrait("Hero")) && 
                !(pTarget.a.hasTrait("Lycanthropy")) && 
                !(pTarget.a.hasTrait("Spirit")) && 
                !pTarget.a.isRace(SK.crabzilla) && 
                !pTarget.a.hasTrait("boat"))
                {
                    Actor a = pTarget.a;
                    if (((Component) a).gameObject == null ||  a == null || !a.inMapBorder())
                        return false;
                    a.removeTrait("cursed");
                    a.removeTrait("death_mark");
                    a.removeTrait("peaceful");
                    removeInfectTrait(pTarget);
                    Actor newUnit = (Actor) null;
                    if (a.asset.id == "dragon" || a.asset.id == "zombie_dragon")
                    {
                        newUnit = World.world.units.createNewUnit("fel_dragon", a.currentTile);
                    }
                    else if (a.asset.id == "wolf" || a.asset.id == "hyena" || a.asset.id == "dog")
                    {
                        newUnit = World.world.units.createNewUnit("hellhound", a.currentTile);
                    }
                    else
                    {
                        newUnit = World.world.units.createNewUnit("lowest_defile_demon", a.currentTile);
                    }
                    ActorTool.copyUnitToOtherUnit(a, newUnit);
                    newUnit.kingdom = pSelf.kingdom;
                    Main.listOfTamedBeasts.Add(newUnit, pSelf.a);
                    pSelf.kingdom.addUnit(newUnit);
                    newUnit.addTrait("Subordinate");
                    EffectsLibrary.spawn("fx_spawn", newUnit.currentTile);
                    if (Main.listOfTamedBeasts.ContainsKey(pTarget.a))
                    {
                        Main.listOfTamedBeasts.Remove(pTarget.a);
                    }
                    ActionLibrary.removeUnit(pTarget.a);
                    spellCost(pSelf,-100);
                    pSelf.a.restoreHealth(1000);
                }
            }
                
                
            return true;
        }
        public static bool desecration2 (BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget.a.asset.id == "demonKing")
            {
                if(!Main.DemonKing.Contains(pTarget.a))
                {
                    Main.DemonKing.Add(pTarget.a);
                }
                if (!Main.listOfBuilding.ContainsKey(pTarget.a))
                {
                    spawn_building(pTarget.a, pTile, "DefilerGate");                  
                }
                else if (Main.listOfBuilding[pTarget.a]==null )
                {
                    Main.listOfBuilding.Remove(pTarget.a);
                    spawn_building(pTarget.a, pTile, "DefilerGate");   
                }
                else if (Main.listOfBuilding[pTarget.a].isRuin())
                {
                    Main.listOfBuilding.Remove(pTarget.a);
                    spawn_building(pTarget.a, pTile, "DefilerGate");   
                }
            }
            string building = "";
            if (Toolbox.randomChance(0.05f))
            {
                building = "Flame_Tower";
            }
            else if (Toolbox.randomChance(0.33f))
            {
                building = "HellKennel";
            }
            else if (Toolbox.randomChance(0.5f))
            {
                building = "Flame_tower";
            }
            else
            {
                building = "barracks_demons";
            }

            
            if (Toolbox.randomChance(0.1f) && pTarget.a.asset.id == "defile_demon")
            {
                if (!Main.listOfBuilding.ContainsKey(pTarget.a))
                {
                    spawn_building(pTarget.a, pTile, building);                  
                }
                else if (Main.listOfBuilding[pTarget.a]==null )
                {
                    Main.listOfBuilding.Remove(pTarget.a);
                    spawn_building(pTarget.a, pTile, building);   
                }
                else if (Main.listOfBuilding[pTarget.a].isRuin())
                {
                    Main.listOfBuilding.Remove(pTarget.a);
                    spawn_building(pTarget.a, pTile, building);   
                }
            }
            World.world.getObjectsInChunks(pTarget.currentTile, 5, MapObjectType.Actor);
            for (int index = 0; index < World.world.temp_map_objects.Count; ++index)
            {
                Actor tempMapObject = (Actor) World.world.temp_map_objects[index];
                if (tempMapObject != pTarget.a &&
                tempMapObject.isRace("demon") &&
                !tempMapObject.hasTrait("Subordinate") )
                {
                    Actor b = tempMapObject;
                    if (pTarget.a.asset.id == "demonKing" && b.asset.id != "demonKing")
                    {
                        if (b!= null && b.isActor())
                        {
                            if (!Main.listOfTamedBeasts.ContainsKey(b))
                            {
                                b.kingdom.removeUnit(b);
                                b.kingdom = pTarget.kingdom;
                                pTarget.kingdom.addUnit(b);
                                Main.listOfTamedBeasts.Add(b, pTarget.a);
                                b.addTrait("Subordinate");
                            }
                        }
                    }
                    else if(b.asset.id != "defile_demon" && b.asset.id != "demonKing" && pTarget.a.asset.id == "defile_demon")
                    {
                        if (b!= null && b.isActor())
                        {
                            if (!Main.listOfTamedBeasts.ContainsKey(b))
                            {
                                b.kingdom.removeUnit(b);
                                b.kingdom = pTarget.kingdom;
                                pTarget.kingdom.addUnit(b);
                                Main.listOfTamedBeasts.Add(b, pTarget.a);
                                b.addTrait("Subordinate");
                            }
                        }
                    }
                    //obey(pTarget, tempMapObject);
                        
                    
                }
            }
            if (
            (pTarget.a.asset.id == "defile_demon" && Main.InvasionDemons) || 
            (Toolbox.randomChance(0.1f) && pTarget.a.hasTrait("hiddenEvil"))
            )
            {
                City Target = (City) null;
                float num1 = 0f;
                foreach (City listCity in World.world.cities.list)
                {
                    float num2 = Toolbox.DistVec3(listCity.cityCenter, pTarget.currentTile.posV);
                    if ( (Target == null || (double) num2 < (double) num1) && listCity.race.nameLocale != "Demonic")
                    {
                        Target = listCity;
                        num1 = num2;
                    }
                }
                if (Target == null)
                {
                    return false;
                }
                goToTarget(pTarget,Target.kingdom.id,pTile);
            }
            else if ((Toolbox.randomChance(0.1f) && pTarget.a.asset.id == "defile_demon" && !Main.InvasionDemons) || 
            (Toolbox.randomChance(0.1f) && pTarget.a.isRace("demon") && !pTarget.a.hasTrait("Subordinate")))
            {
                goToTarget(pTarget,"demons");
            }
            return true;
        }
        public static bool deathDesecration(BaseSimObject pTarget, WorldTile pTile = null) 
      	{
         if (pTarget != null)
         {
            Actor a = pTarget.a;
            a.removeTrait("Defiler");
            a.removeTrait("madness");
            if (Toolbox.randomChance(0.001f) && 
            good_defiler.ContainsKey(pTarget.a.asset.id) &&
            pTarget.a.asset.id != "defile_demon" &&
            pTarget.a.asset.id != "demonKing")
            {
                World.world.buildings.addBuilding("DefilerGate", pTile, false, false, BuildPlacingType.New);
            }
            if (pTarget.a.asset.id == "demonKing")
            {
                Main.DemonKing.Remove(pTarget.a);
                ActionLibrary.kingSlayer(pTarget, pTile);
            }
            return true;
            
         }
         return false;

        }
        public static bool hiddenEvils(BaseSimObject pTarget, WorldTile pTile = null) 
      	{
            World.world.getObjectsInChunks(pTarget.currentTile, 20, MapObjectType.Actor);
            if (pTarget.a.asset.id == "hidden_demon")
            {
                for (int index = 0; index < World.world.temp_map_objects.Count; ++index)
                {
                    Actor tempMapObject = (Actor) World.world.temp_map_objects[index];
                    if (tempMapObject.hasTrait("Demon Fighter") && 
                    tempMapObject != pTarget.a && !tempMapObject.hasTrait("hiddenEvil"))
                    {
                        reborn(pTarget, tempMapObject.asset.id);
                        return false;
                    }
                }
            }
            else
            {
                int index1 = 0;
                for (int index = 0; index < World.world.temp_map_objects.Count; ++index)
                {
                    Actor tempMapObject = (Actor) World.world.temp_map_objects[index];
                    if (!tempMapObject.hasTrait("Demon Fighter") || tempMapObject.hasTrait("hiddenEvil"))
                    {
                        index1++;
                    }  
                }
                if (index1 == World.world.temp_map_objects.Count)
                {
                    reborn(pTarget, "hidden_demon");
                }
            }
            return true;
        }
        #endregion
#region Ящеры
        
//Ящеры
        public static bool LizardsRegen(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
        {
            Regen(pSelf,777);
            return true;
        }
        public static bool LizardsRegen1(BaseSimObject pTarget, WorldTile pTile = null)
        {
            Regen(pTarget,100);
            return true;
        }
        #endregion
#endregion
#region функции        
#region Восстанови
        public static bool Regen(BaseSimObject pTarget, int pHealth = 1, WorldTile pTile = null)
        {
            if (pTarget.a.hasTrait("infected"))
            {
                return true;
            }
            bool flag = true;
            if (pTarget.a.asset.needFood)
            {
                flag = (pTarget.a.data.hunger > 0);
            }
            if (pTarget.a.data.health != pTarget.getMaxHealth() && flag)
            {
                pTarget.a.restoreHealth(pHealth);
                pTarget.a.spawnParticle(Toolbox.color_heal);
            }
            if (pTarget.a.data.health == pTarget.getMaxHealth() && flag)
            {
                pTarget.a.removeTrait("eyepatch");
                pTarget.a.removeTrait("skin_burns");
                pTarget.a.removeTrait("crippled"); 
            }
            return true;
        }
        #endregion
#region Очисти
        private static bool removeBadTrait(BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget.a != null)
            {
                
                //insert bad traits here
                pTarget.a.removeTrait("madness");
                pTarget.a.removeTrait("crippled");
                pTarget.a.removeTrait("cursed");
                pTarget.a.removeTrait("death_mark");
                pTarget.a.removeTrait("voices_in_my_head");
                pTarget.a.removeTrait("eyepatch");
                pTarget.a.removeTrait("infected");
                pTarget.a.removeTrait("plague");
                pTarget.a.removeTrait("mushSpores");
                pTarget.a.removeTrait("tumorInfection");
                pTarget.a.removeTrait("skin_burns");
                //pTarget.a.removeTrait("scar_of_divinity");
                pTarget.a.removeTrait("Lycanthropy");
                pTarget.a.removeTrait("Vampirism");
                pTarget.a.removeTrait("AndroidPower2");  //kinda hate this trait
            }
            
            return true;
        }
        private static bool removeInfectTrait(BaseSimObject pTarget, WorldTile pTile = null)
        {
            if (pTarget.a != null)
            {
                
                //insert bad traits here
                pTarget.a.removeTrait("infected");
                pTarget.a.removeTrait("plague");
                pTarget.a.removeTrait("mushSpores");
                pTarget.a.removeTrait("tumorInfection");
                pTarget.a.removeTrait("zombie");
                pTarget.a.removeTrait("AndroidPower1");
                //pTarget.a.removeTrait("scar_of_divinity");
                pTarget.a.removeTrait("Lycanthropy");
                pTarget.a.removeTrait("Werewolf");
                pTarget.a.removeTrait("Vampire");
                pTarget.a.removeTrait("Elder Vampire");
                pTarget.a.removeTrait("Vampirism");
                pTarget.a.removeTrait("AndroidPower2");  //kinda hate this trait
            }
            
            return true;
        }
        #endregion
#region Восстань
        private static bool rebornANew(BaseSimObject pTarget, WorldTile pTile)
        {
            if (!pTarget.a.hasTrait("blessed")){
                Actor a = pTarget.a;
                pTarget.a.removeTrait("cursed");
                pTarget.a.removeTrait("infected");
                pTarget.a.removeTrait("mushSpores");
                pTarget.a.removeTrait("tumorInfection");
                pTarget.a.removeTrait("madness");
                pTarget.a.removeTrait("eyepatch");
                pTarget.a.removeTrait("plague");
                pTarget.a.removeTrait("voices_in_my_head");
                pTarget.a.removeTrait("death_mark");
                pTarget.a.removeTrait("Phoenix");
                pTarget.a.removeTrait("crippled");
                pTarget.a.removeTrait("skin_burns");
                pTarget.a.removeTrait("Subordinate");
                a.addTrait("fire_proof"); //what kind of phoenix that got burned lol
                //a.removeTrait("Pheonix");
                removeInfectTrait(pTarget);
                var act = World.world.units.createNewUnit(a.asset.id, pTile, 0f);
                ActorTool.copyUnitToOtherUnit(a, act);
                if (pTarget.kingdom.isAlive())
                    act.kingdom = pTarget.kingdom;
                act.data.setName(pTarget.a.getName());
                act.data.health += 1000;
                //EffectsLibrary.spawn("fx_nuke_flash", pTarget.a.currentTile, null, null, 0f, -1f, -1f);
                //act.addStatusEffect("Phoenix", 7f);
                act.a.makeWait(3);
                act.addStatusEffect("invincible", 5);
                //spawn effect for cooler looks
                ActionLibrary.castLightning(null, act, null);
                //castLightningWithoutLava(pTarget, pTarget, null);
                PowerLibrary pb = new PowerLibrary();
                pb.divineLightFX(pTarget.a.currentTile, null);
                EffectsLibrary.spawnExplosionWave(pTile.posV3, 1f, 1f);
                World.world.applyForce(pTile, 10, 1.5f, true, true, 0, null, null, null);
            }
            return true;
        }
        #endregion
#region Излечи
        public static bool healing(BaseSimObject pTarget, int pHealth = 10, float pChance = 0.2f, int pDistance = 4, WorldTile pTile = null )
      	{
            if (Toolbox.randomChance(pChance))
            {
                World.world.getObjectsInChunks(pTarget.currentTile, pDistance, MapObjectType.Actor);
                if(World.world.temp_map_objects.Count>1)
                    spellCost(pTarget,10);
                for (int index = 0; index < World.world.temp_map_objects.Count; ++index)
                {
                    Actor tempMapObject = (Actor) World.world.temp_map_objects[index];
                    if (tempMapObject != pTarget.a && 
                    tempMapObject.data.health < tempMapObject.getMaxHealth() &&
                    tempMapObject.kingdom == pTarget.a.kingdom)
                    {
                        tempMapObject.restoreHealth(pHealth);
                        tempMapObject.spawnParticle(Toolbox.color_heal);
                        tempMapObject.removeTrait("plague");
                    }
                }
            }
            return true;
        }
        #endregion
#region Защити
        public static bool Protect(BaseSimObject pTarget, int pDistance = 5, float pOverrideTimer = 5f)
      	{
            World.world.getObjectsInChunks(pTarget.currentTile, pDistance, MapObjectType.Actor);
            for (int index = 0; index < World.world.temp_map_objects.Count; ++index)
            {
                Actor tempMapObject = (Actor) World.world.temp_map_objects[index];
                if (tempMapObject.kingdom == pTarget.a.kingdom)
                {
                    addStatusOnTarget(pTarget, tempMapObject, "shield", 0,0,pOverrideTimer);
                }
            }
            return true;
        }
        #endregion
#region Подчинись
        public static bool obey(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) 
      	{
            Actor b = pTarget.a;
            if (b!= null && b.isActor())
            {
                if (!Main.listOfTamedBeasts.ContainsKey(b))
                {
                    if (b.kingdom != pSelf.a.kingdom)
                    {
                        b.kingdom.removeUnit(b);
                        b.kingdom = pSelf.kingdom;
                        pSelf.kingdom.addUnit(b);
                        Main.listOfTamedBeasts.Add(b, pSelf.a);
                    }
                    b.addTrait("Subordinate");
                }
            }
            return true;
        }
        #endregion
#region Получи
        public static bool addStatusOnTarget(BaseSimObject pSelf, BaseSimObject pTarget, string pStatus, int Hunger, int Health = 0, float pOverrideTimer = -1f)
      	{
            if(pTarget!=null)
            {
                if(pTarget.isAlive() && pTarget.isActor()){
                        pTarget.a.addStatusEffect(pStatus,pOverrideTimer);
                    spellCost(pSelf, Hunger, Health);
                }
            }
            return true;
        }
        #endregion
#region Приди
        public static bool summon(BaseSimObject pTarget, string entity, int Hunger, int Health = 0, WorldTile pTile = null)
        {
            Actor a = pTarget.a;
            if (pTarget!=null)
            {
                if (pTarget.isAlive())
                {
                    spellCost(pTarget, Hunger, Health);
                    Actor act = World.world.units.createNewUnit(entity, a.currentTile, 0f);
                    act.kingdom = pTarget.kingdom;
                    pTarget.kingdom.addUnit(act);
                    act.addTrait("Subordinate");
                    //act.data.setName(pTarget.a.getName());
                    Main.listOfTamedBeasts.Add(act, pTarget.a);
                    EffectsLibrary.spawn("fx_spawn", act.currentTile, null, null, 0f, -1f, -1f);
                }
            }
            
            return true;
        }
        #endregion
#region Заплати
        public static bool spellCost(BaseSimObject pTarget, int Hunger, int Health = 0)
      	{
            if (pTarget != null){
                if (pTarget.isAlive()){
                    int hungVal = pTarget.a.data.hunger - Hunger;
                    hungVal = Mathf.Clamp(hungVal, 1, 100);
                    pTarget.a.data.hunger = hungVal;
                    int healVal = pTarget.a.data.health - Health;
                    healVal = Mathf.Clamp(healVal, 1, pTarget.a.getMaxHealth());
                    pTarget.a.data.health = healVal;
                    
                }
            }
            return true;
        }
        #endregion
#region Следуй
        public static bool following (BaseSimObject pTarget, WorldTile pTile = null)
        {
            Actor b = pTarget.a;
            if (Toolbox.randomChance(0.8f))
            {
            if(Main.listOfTamedBeasts.ContainsKey(b))
            {
                if(Main.listOfTamedBeasts[b].isAlive())
                {
                    if(Main.listOfTamedBeasts[b].hasTrait("The Magic of Death"))
                    {
                        if(deadBodies.ContainsKey(Main.listOfTamedBeasts[b]) && Main.NewMagicOfDeath)
                        {
                            deadBodies[Main.listOfTamedBeasts[b]]+=pTarget.a.data.kills;
                            pTarget.a.data.kills=0;
                        }
                        else
                        {
                            deadBodies.Add(Main.listOfTamedBeasts[b],0);
                            if (Main.NewMagicOfDeath)
                            {
                                deadBodies[Main.listOfTamedBeasts[b]]+=pTarget.a.data.kills;
                                pTarget.a.data.kills=0;
                            }
                            
                        }
                    }
                    if (Main.listOfTamedBeasts[b].kingdom != b.kingdom)
                    {
                        b.kingdom.removeUnit(b);
                        b.kingdom = Main.listOfTamedBeasts[b].kingdom;
                        Main.listOfTamedBeasts[b].kingdom.addUnit(b);
                    }
                    if (b.is_moving)
                    {
                        pTile = Main.listOfTamedBeasts[b].currentTile.region.tiles.GetRandom<WorldTile>();
                        b.goTo(pTile, true, true);

                    }
                }
                else 
                {
                    Main.listOfTamedBeasts.Remove(b);
                    b.removeTrait("Subordinate");
                }
            }
            else
                b.removeTrait("Subordinate");
                return false;
            }
            return true;
        }
        #endregion
#region Переродись
        public static bool reborn(BaseSimObject pTarget, string pStatsID, WorldTile pTile = null, bool saveKingdom=false) 
      	{
         if (pTarget != null)
         {
            Actor a = pTarget.a;
            pTarget.a.removeTrait("Subordinate");
            removeBadTrait(pTarget);
            removeInfectTrait(pTarget);
            var act = World.world.units.createNewUnit(pStatsID, pTarget.currentTile, 0f);
            ActorTool.copyUnitToOtherUnit(a, act);
            
            act.data.health += 1000;
            EffectsLibrary.spawn("fx_spawn", act.currentTile, null, null, 0f, -1f, -1f);
            if (deadBodies.ContainsKey(a))
            {
                deadBodies.Add(act,deadBodies[a]);
                deadBodies.Remove(a);
            }
            if (Main.listOfKingdoms.ContainsKey(a))
            {
                if (Main.listOfKingdoms[a].isAlive())
                {
                    act.kingdom = Main.listOfKingdoms[a];
                    Main.listOfKingdoms[a].addUnit(act);
                }
            }
            else
                if (a.kingdom.isAlive() && !a.hasTrait("madness") && saveKingdom)
                {
                    act.kingdom = a.kingdom;
                    a.kingdom.addUnit(act);
                }
            ActionLibrary.removeUnit(pTarget.a);
            return true;
         }
         return true;
        }
        #endregion
#region Строй
        public static bool spawn_building(Actor act, WorldTile pTile, string currentBuilding)
        {
            BuildingAsset buildingAsset = AssetManager.buildings.get(currentBuilding);
            Building newBuilding = null;
            if (!World.world.buildings.canBuildFrom(pTile, buildingAsset, (City) null))
            {
                return false;
            }
            int index = 0;
            if(pTile.building == null)
            {
                foreach ( WorldTile neigh in pTile.neighboursAll)
                {
                    if (neigh.building == null)
                    {
                        index+=1;
                        
                    }
                }
                if (index == pTile.neighboursAll.Length)
                {
                    newBuilding = World.world.buildings.addBuilding(currentBuilding, pTile, true, false, BuildPlacingType.New);
                    if (newBuilding == null )
                    {
                        return false;
                    }
                    Main.listOfBuilding.Add(act,newBuilding);
                    return true;
                }
                
            }
            
            return true;
        }
        #endregion
#region Иди к
        public static bool goToTarget(BaseSimObject pTarget, string kingdom, WorldTile pTile = null)
        {
            Actor pActor = pTarget.a;
            if (BehaviourActionBase<Actor>.world.worldLaws.world_law_peaceful_monsters.boolVal)
                return false;
            Building building1 = (Building) null;
            float num1 = 0.0f;
            foreach (Building building2 in (ObjectContainer<Building>) World.world.kingdoms.getKingdomByID(kingdom).buildings)
            {
                if (building2.currentTile.isSameIsland(pActor.currentTile))
                {
                    float num2 = Toolbox.DistTile(building2.currentTile, pActor.currentTile);
                    if ( building1 == null || (double) num2 < (double) num1)
                    {
                        building1 = building2;
                        num1 = num2;
                    }
                }
            }
            if (building1 == null)
                return false;
            if ( building1 !=  null)
            {
                pActor.goTo(building1.currentTile);
            }
            return true;
        }
#endregion
#region Телепортируйся
        public static bool teleportToTarget(BaseSimObject pTarget, WorldTile pTile = null)
        {
            WorldTile worldTile = pTile;
            WorldTile pTile1 = worldTile;
            if (pTile1 == null || pTile1.Type.block || !pTile1.Type.ground)
            return false;
            string pID = pTarget.a.asset.effect_teleport;
            if (string.IsNullOrEmpty(pID))
                pID = "fx_teleport_blue";
            EffectsLibrary.spawnAt(pID, pTarget.currentTile.posV3, pTarget.a.stats[S.scale]);
            BaseEffect baseEffect = EffectsLibrary.spawnAt(pID, pTile1.posV3, pTarget.a.stats[S.scale]);
            if (baseEffect != null)
            baseEffect.spriteAnimation.setFrameIndex(9);
            //pTarget.a.cancelAllBeh();
            pTarget.a.spawnOn(pTile1, 0.0f);
            return true;
        }
  #endregion
#region Эвакуируй

        public static void Evacuation(BaseSimObject pTarget)
        {
            if (pTarget.kingdom.hasEnemies() && pTarget.kingdom.units.Count<=9)
            {
                ActionLibrary.teleportRandom(pTarget.a, pTarget.a);
                foreach (Actor unit in (ObjectContainer<Actor>) pTarget.kingdom.units)
                {
                    if (unit != pTarget.a)
                    {
                        teleportToTarget(unit, pTarget.currentTile);
                        spellCost(pTarget, 10);
                    }
                    
                }
                spellCost(pTarget, 10);
                //spellCost(pTarget,90);
            }
        }
#endregion
#endregion        
        public static void addTraitToLocalizedLibrary(string planguage, string id, string name, string description)
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
                localizedText.Add("trait_" + id, name);
                localizedText.Add("trait_" + id + "_info", description);
            }
        }
    }
}