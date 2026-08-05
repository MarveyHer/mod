using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class KingdomAsset : Asset
{
	public bool civ;

	[DefaultValue(-1)]
	public int default_civ_color_index = -1;

	public bool nomads;

	public bool nature;

	public bool abandoned;

	public bool concept;

	public bool always_attack_each_other;

	public bool units_always_looking_for_enemies;

	[NonSerialized]
	public HashSet<string> assets_discrepancies;

	[NonSerialized]
	public HashSet<string> assets_discrepancies_bad;

	public bool force_look_all_chunks;

	public bool mobs;

	public bool neutral;

	public bool brain;

	public bool group_miniciv;

	public bool group_minicivs_cool;

	public bool group_creeps;

	public bool group_main;

	public bool is_forced_by_trait;

	[DefaultValue("")]
	public string forced_by_trait_kingdom_id = string.Empty;

	[DefaultValue("")]
	public string building_attractor_id = string.Empty;

	public bool count_as_danger = true;

	public HashSet<string> friendly_tags = new HashSet<string>();

	public HashSet<string> enemy_tags = new HashSet<string>();

	public HashSet<string> list_tags = new HashSet<string>();

	private Dictionary<int, int> _cached_enemies = new Dictionary<int, int>();

	public ColorAsset default_kingdom_color;

	public Color color_building = Color.white;

	private Sprite _cached_sprite;

	public string path_icon = "ui/Icons/iconWarning";

	public bool show_icon;

	public bool friendship_for_everyone;

	private ColorAsset _debug_color_asset;

	[JsonIgnore]
	public ColorAsset debug_color_asset
	{
		get
		{
			if (_debug_color_asset == null)
			{
				_debug_color_asset = AssetManager.kingdom_colors_library?.list?.GetRandom();
			}
			return _debug_color_asset;
		}
		set
		{
			_debug_color_asset = value;
		}
	}

	public Sprite getSprite()
	{
		if (_cached_sprite == null)
		{
			_cached_sprite = SpriteTextureLoader.getSprite(path_icon);
		}
		return _cached_sprite;
	}

	public void setIcon(string pPath)
	{
		path_icon = pPath;
		show_icon = true;
	}

	public void addTag(string pTag)
	{
		list_tags.Add(pTag);
	}

	public void addFriendlyTag(string pTag)
	{
		friendly_tags.Add(pTag);
	}

	public void addEnemyTag(string pTag)
	{
		enemy_tags.Add(pTag);
	}

	public bool isFoe(KingdomAsset pTarget)
	{
		int tCachedResult = 0;
		int tHashCode = pTarget.GetHashCode();
		_cached_enemies.TryGetValue(tHashCode, out tCachedResult);
		if (tCachedResult != 0)
		{
			return tCachedResult == 1;
		}
		if (nature || pTarget.nature)
		{
			_cached_enemies.Add(tHashCode, -1);
			return false;
		}
		if (this == pTarget)
		{
			_cached_enemies.Add(tHashCode, always_attack_each_other ? 1 : (-1));
			return always_attack_each_other;
		}
		if (enemy_tags.Count > 0 && enemy_tags.Overlaps(pTarget.list_tags))
		{
			_cached_enemies.Add(tHashCode, 1);
			return true;
		}
		pTarget.list_tags.Add(pTarget.id);
		list_tags.Add(id);
		if (friendly_tags.Count > 0 && friendly_tags.Overlaps(pTarget.list_tags))
		{
			_cached_enemies.Add(tHashCode, -1);
			return false;
		}
		_cached_enemies.Add(tHashCode, 1);
		return true;
	}

	public void clearKingdomColor()
	{
		default_kingdom_color = null;
	}
}
