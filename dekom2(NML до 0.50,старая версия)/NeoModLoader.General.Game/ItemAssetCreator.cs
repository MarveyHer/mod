using System;
using System.Collections.Generic;
using System.Text;
using NeoModLoader.services;

namespace NeoModLoader.General.Game;

public static class ItemAssetCreator
{
	public static ItemAsset CreateWeaponMaterial(string id, BaseStats base_stats = null, int cost_gold = 0, KeyValuePair<string, int>[] cost_resources = null, int equipment_value = 0, bool metallic = false, int minimum_city_storage_resource_1 = 0, int mod_rank = 0, Rarity quality = Rarity.R0_Normal, string tech_needed = null)
	{
		ItemAsset itemAsset = CreateAccessoryOrArmorMaterial(id, base_stats, cost_gold, cost_resources, equipment_value, minimum_city_storage_resource_1, mod_rank, quality, tech_needed);
		itemAsset.metallic = metallic;
		return itemAsset;
	}

	public static ItemAsset CreateAccessoryOrArmorMaterial(string id, BaseStats base_stats = null, int cost_gold = 0, KeyValuePair<string, int>[] cost_resources = null, int equipment_value = 0, int minimum_city_storage_resource_1 = 0, int mod_rank = 0, Rarity quality = Rarity.R0_Normal, string tech_needed = null)
	{
		ItemAsset itemAsset = new ItemAsset();
		itemAsset.id = id;
		itemAsset.base_stats = base_stats ?? itemAsset.base_stats;
		itemAsset.cost_gold = cost_gold;
		itemAsset.equipment_value = equipment_value;
		itemAsset.minimum_city_storage_resource_1 = minimum_city_storage_resource_1;
		itemAsset.mod_rank = mod_rank;
		itemAsset.quality = quality;
		itemAsset.cost_resource_id_1 = "none";
		itemAsset.cost_resource_id_2 = "none";
		if (cost_resources != null)
		{
			int num = cost_resources.Length;
			int num2 = num;
			if (num2 < 2)
			{
				if (num2 != 0 && num2 == 1)
				{
					itemAsset.cost_resource_1 = cost_resources[0].Value;
					itemAsset.cost_resource_id_1 = cost_resources[0].Key;
				}
			}
			else
			{
				itemAsset.cost_resource_1 = cost_resources[0].Value;
				itemAsset.cost_resource_id_1 = cost_resources[0].Key;
				itemAsset.cost_resource_2 = cost_resources[1].Value;
				itemAsset.cost_resource_id_2 = cost_resources[1].Key;
			}
		}
		return itemAsset;
	}

	public static ItemAsset CreateAndAddModifier(string id, string mod_type, int mod_rank, string translation_key, string[] pools, int rarity = 1, int equipment_value = 0, Rarity quality = Rarity.R0_Normal, BaseStats base_stats = null, AttackAction action_attack_target = null, WorldAction action_special_effect = null, float special_effect_interval = 0.1f)
	{
		ItemAsset itemAsset = new ItemAsset();
		itemAsset.id = id;
		itemAsset.mod_type = mod_type;
		itemAsset.mod_rank = mod_rank;
		itemAsset.translation_key = translation_key;
		itemAsset.rarity = Math.Min(100, rarity);
		itemAsset.equipment_value = equipment_value;
		itemAsset.quality = quality;
		itemAsset.base_stats = base_stats;
		itemAsset.action_attack_target = action_attack_target;
		itemAsset.action_special_effect = action_special_effect;
		itemAsset.special_effect_interval = special_effect_interval;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string value in pools)
		{
			stringBuilder.Append(value);
			stringBuilder.Append(',');
		}
		stringBuilder.Remove(stringBuilder.Length - 1, 1);
		itemAsset.pool = stringBuilder.ToString();
		foreach (string text in pools)
		{
			if (!AssetManager.items_modifiers.pools.ContainsKey(text))
			{
				LogService.LogWarning("Invalid pool id " + text + " for modifier " + id);
				continue;
			}
			for (int k = 0; k < rarity; k++)
			{
				AssetManager.items_modifiers.pools[text].Add(itemAsset);
			}
		}
		((AssetLibrary<ItemAsset>)(object)AssetManager.items_modifiers).add(itemAsset);
		return itemAsset;
	}

	public static ItemAsset CreateMeleeWeapon(string id, BaseStats base_stats = null, string material = null, List<string> item_modifiers = null, string name_class = null, List<string> name_templates = null, string tech_needed = null, AttackAction action_attack_target = null, WorldAction action_special_effect = null, float special_effect_interval = 1f, int equipment_value = 0, string path_slash_animation = "effects/slashes/slash_base")
	{
		ItemAsset itemAsset = ((AssetLibrary<ItemAsset>)(object)AssetManager.items).clone(id, "_melee");
		itemAsset.base_stats = base_stats ?? itemAsset.base_stats;
		itemAsset.material = material ?? itemAsset.material;
		itemAsset.item_modifier_ids = ((item_modifiers != null) ? item_modifiers.ToArray() : itemAsset.item_modifier_ids);
		itemAsset.name_class = (string.IsNullOrEmpty(name_class) ? itemAsset.name_class : name_class);
		itemAsset.name_templates = name_templates ?? itemAsset.name_templates;
		itemAsset.action_attack_target = action_attack_target;
		itemAsset.action_special_effect = action_special_effect;
		itemAsset.special_effect_interval = special_effect_interval;
		itemAsset.equipment_value = equipment_value;
		itemAsset.path_slash_animation = path_slash_animation;
		itemAsset.attack_type = WeaponType.Melee;
		itemAsset.equipment_type = EquipmentType.Weapon;
		return itemAsset;
	}

	public static ItemAsset CreateRangeWeapon(string id, string projectile, BaseStats base_stats = null, string material = null, List<string> item_modifiers = null, string name_class = null, List<string> name_templates = null, string tech_needed = null, AttackAction action_attack_target = null, WorldAction action_special_effect = null, float special_effect_interval = 1f, int equipment_value = 0, string path_slash_animation = "effects/slashes/slash_punch")
	{
		ItemAsset itemAsset = ((AssetLibrary<ItemAsset>)(object)AssetManager.items).clone(id, "_range");
		itemAsset.base_stats = base_stats ?? itemAsset.base_stats;
		itemAsset.material = material ?? itemAsset.material;
		itemAsset.item_modifier_ids = ((item_modifiers != null) ? item_modifiers.ToArray() : itemAsset.item_modifier_ids);
		itemAsset.name_class = (string.IsNullOrEmpty(name_class) ? itemAsset.name_class : name_class);
		itemAsset.name_templates = name_templates ?? itemAsset.name_templates;
		itemAsset.action_attack_target = action_attack_target;
		itemAsset.action_special_effect = action_special_effect;
		itemAsset.special_effect_interval = special_effect_interval;
		itemAsset.equipment_value = equipment_value;
		itemAsset.path_slash_animation = path_slash_animation;
		itemAsset.projectile = (string.IsNullOrEmpty(projectile) ? "snowball" : projectile);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Some unexpected for " + id + " as a range weapon:");
		if (string.IsNullOrEmpty(projectile))
		{
			stringBuilder.AppendLine("\t projectile is null or empty. ");
		}
		itemAsset.attack_type = WeaponType.Range;
		itemAsset.equipment_type = EquipmentType.Weapon;
		return itemAsset;
	}

	public static ItemAsset CreateArmorOrAccessory(string id, EquipmentType equipmentType, BaseStats base_stats = null, string material = null, List<string> item_modifiers = null, string name_class = null, List<string> name_templates = null, string tech_needed = null, AttackAction action_attack_target = null, WorldAction action_special_effect = null, float special_effect_interval = 1f, int equipment_value = 0)
	{
		if (1 == 0)
		{
		}
		string text = equipmentType switch
		{
			EquipmentType.Armor => "armor", 
			EquipmentType.Boots => "boots", 
			EquipmentType.Helmet => "helmet", 
			EquipmentType.Ring => "ring", 
			EquipmentType.Amulet => "amulet", 
			_ => throw new ArgumentOutOfRangeException("equipmentType", equipmentType, null), 
		};
		if (1 == 0)
		{
		}
		string pFrom = text;
		ItemAsset itemAsset = ((AssetLibrary<ItemAsset>)(object)AssetManager.items).clone(id, pFrom);
		itemAsset.base_stats = base_stats ?? itemAsset.base_stats;
		itemAsset.material = material ?? itemAsset.material;
		itemAsset.item_modifier_ids = ((item_modifiers != null) ? item_modifiers.ToArray() : itemAsset.item_modifier_ids);
		itemAsset.name_class = (string.IsNullOrEmpty(name_class) ? itemAsset.name_class : name_class);
		itemAsset.name_templates = name_templates ?? itemAsset.name_templates;
		itemAsset.action_attack_target = action_attack_target;
		itemAsset.action_special_effect = action_special_effect;
		itemAsset.special_effect_interval = special_effect_interval;
		itemAsset.equipment_value = equipment_value;
		itemAsset.equipment_type = equipmentType;
		return itemAsset;
	}
}
