using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class ActorTextureSubAsset : ICloneable
{
	[DefaultValue("male_1")]
	public const string skin_civ_default_male = "male_1";

	[DefaultValue("female_1")]
	public const string skin_civ_default_female = "female_1";

	public static List<Sprite> all_preloaded_sprites_units = new List<Sprite>();

	[NonSerialized]
	public readonly Dictionary<string, Sprite[]> dict_mains = new Dictionary<string, Sprite[]>();

	private static Dictionary<string, Sprite> _shadow_sprites = new Dictionary<string, Sprite>();

	public string texture_path_base;

	public string texture_path_base_male;

	public string texture_path_base_female;

	public string texture_path_main;

	public string texture_path_baby;

	public string texture_path_king;

	public string texture_path_leader;

	public string texture_path_warrior;

	public string texture_path_zombie;

	public bool has_advanced_textures;

	public bool has_old_heads;

	[DefaultValue("")]
	public string texture_heads = string.Empty;

	[DefaultValue("")]
	public string texture_head_king = string.Empty;

	[DefaultValue("")]
	public string texture_head_warrior = string.Empty;

	[DefaultValue("")]
	public string texture_heads_old_male = string.Empty;

	[DefaultValue("")]
	public string texture_heads_old_female = string.Empty;

	[DefaultValue("")]
	public string texture_heads_male = string.Empty;

	[DefaultValue("")]
	public string texture_heads_female = string.Empty;

	public bool render_heads_for_children;

	public bool prevent_unconscious_rotation;

	[DefaultValue(true)]
	public bool shadow = true;

	[DefaultValue("unitShadow_5")]
	public string shadow_texture = "unitShadow_5";

	[NonSerialized]
	internal Sprite shadow_sprite;

	[NonSerialized]
	internal Vector2 shadow_size;

	[DefaultValue("unitShadow_2")]
	public string shadow_texture_egg = "unitShadow_2";

	[NonSerialized]
	internal Sprite shadow_sprite_egg;

	[NonSerialized]
	internal Vector2 shadow_size_egg;

	[DefaultValue("unitShadow_4")]
	public string shadow_texture_baby = "unitShadow_4";

	[NonSerialized]
	internal Sprite shadow_sprite_baby;

	[NonSerialized]
	internal Vector2 shadow_size_baby;

	private string _base_path;

	private static int _total;

	private static readonly Regex _regex_heads_sorter = new Regex("(\\D*)(\\d+)");

	public ActorTextureSubAsset(string pBasePath, bool pHasAdvancedTextures)
	{
		_total++;
		has_advanced_textures = pHasAdvancedTextures;
		_base_path = pBasePath;
		texture_path_base = pBasePath;
		texture_path_base_male = pBasePath + "male_1";
		texture_path_base_female = pBasePath + "female_1";
		if (string.IsNullOrEmpty(texture_head_warrior))
		{
			texture_head_warrior = pBasePath + "head_warrior";
		}
		if (string.IsNullOrEmpty(texture_head_king))
		{
			texture_head_king = pBasePath + "head_king";
		}
		if (string.IsNullOrEmpty(texture_heads_old_female))
		{
			texture_heads_old_female = pBasePath + "head_old_female";
		}
		if (string.IsNullOrEmpty(texture_heads_old_male))
		{
			texture_heads_old_male = pBasePath + "head_old_male";
		}
		if (string.IsNullOrEmpty(texture_heads_male))
		{
			texture_heads_male = pBasePath + "heads_male";
		}
		if (string.IsNullOrEmpty(texture_heads_female))
		{
			texture_heads_female = pBasePath + "heads_female";
		}
		texture_path_main = pBasePath + "main";
		if (!hasSpriteInResources(texture_path_main))
		{
			texture_path_main = texture_path_base_male;
		}
		if (string.IsNullOrEmpty(texture_path_king))
		{
			texture_path_king = pBasePath + "king";
		}
		if (string.IsNullOrEmpty(texture_path_leader))
		{
			texture_path_leader = pBasePath + "leader";
		}
		if (string.IsNullOrEmpty(texture_path_warrior))
		{
			texture_path_warrior = pBasePath + "warrior_1";
		}
		if (string.IsNullOrEmpty(texture_path_zombie))
		{
			texture_path_zombie = pBasePath + "zombie";
		}
		if (string.IsNullOrEmpty(texture_heads))
		{
			texture_heads = pBasePath + "heads";
			if (!hasSpriteInResources(texture_path_main))
			{
				texture_path_main = texture_heads_male;
			}
		}
		if (hasSpriteInResources(texture_heads_old_male))
		{
			has_old_heads = true;
		}
		texture_path_baby = pBasePath + "child";
	}

	private void logAssetError(string pMessage, string pPath)
	{
		BaseAssetLibrary.logAssetError(pMessage, pPath);
	}

	public string getUnitTexturePath(Actor pActor)
	{
		Subspecies tSubspecies = pActor.subspecies;
		if (pActor.isEgg())
		{
			return tSubspecies.egg_sprite_path;
		}
		if (pActor.isBaby())
		{
			return texture_path_baby;
		}
		if (pActor.hasSubspecies() && pActor.subspecies.has_mutation_reskin && pActor.asset.unit_zombie)
		{
			return texture_path_zombie;
		}
		string tResult = texture_path_main;
		ProfessionAsset tProfessionAsset = pActor.profession_asset;
		if (tProfessionAsset == null || tProfessionAsset.profession_id == UnitProfession.Nothing)
		{
			return tResult;
		}
		if (!has_advanced_textures)
		{
			return tResult;
		}
		switch (tProfessionAsset.profession_id)
		{
		case UnitProfession.Warrior:
		{
			string tWarriorId = texture_path_warrior;
			if (pActor.hasSubspecies())
			{
				tWarriorId = pActor.subspecies.getSkinWarrior();
			}
			if (tSubspecies.has_mutation_reskin)
			{
				List<string> tWarriorSkins = tSubspecies.mutation_skin_asset.skin_warrior;
				int tNextIndex = Toolbox.loopIndex(pActor.asset.skin_warrior.IndexOf(tWarriorId), tWarriorSkins.Count);
				tWarriorId = tWarriorSkins[tNextIndex];
			}
			return texture_path_base + tWarriorId;
		}
		case UnitProfession.King:
			return texture_path_king;
		case UnitProfession.Leader:
			return texture_path_leader;
		default:
			return getTextureSkinBasedOnSex(pActor);
		}
	}

	private string getTextureSkinBasedOnSex(Actor pActor)
	{
		if (pActor.isSexFemale())
		{
			if (pActor.hasSubspecies())
			{
				return texture_path_base + pActor.subspecies.getSkinFemale();
			}
			return texture_path_base_female;
		}
		if (pActor.hasSubspecies())
		{
			return texture_path_base + pActor.subspecies.getSkinMale();
		}
		return texture_path_base_male;
	}

	public string getUnitTexturePathForUI(ActorAsset pAsset)
	{
		string tResult = texture_path_main;
		if (!pAsset.civ)
		{
			return tResult;
		}
		if (AssetsDebugManager.actors_sex == ActorSex.Male)
		{
			return texture_path_base + pAsset.skin_citizen_male[0];
		}
		return texture_path_base + pAsset.skin_citizen_female[0];
	}

	private bool hasSpriteInResources(string pPath)
	{
		Sprite[] tSprites = SpriteTextureLoader.getSpriteList(pPath, pSkipIfEmpty: true);
		if (tSprites == null)
		{
			return false;
		}
		all_preloaded_sprites_units.AddRange(tSprites);
		return tSprites.Length != 0;
	}

	public object Clone()
	{
		return new ActorTextureSubAsset(_base_path, has_advanced_textures);
	}

	public void preloadSprites(bool pCivTextures, bool pHasBabyForm, IAnimationFrames pAnimationAsset)
	{
		if (!pCivTextures)
		{
			preloadSpritePath("texture_path_main", texture_path_main, pAnimationAsset);
		}
		if (pHasBabyForm)
		{
			preloadSpritePath("texture_path_baby", texture_path_baby, pAnimationAsset);
		}
		if (has_advanced_textures)
		{
			preloadSpritePath("texture_path_base_male", texture_path_base_male, pAnimationAsset);
			preloadSpritePath("texture_path_base_female", texture_path_base_female, pAnimationAsset);
			preloadSpritePath("texture_path_king", texture_path_king, pAnimationAsset);
			preloadSpritePath("texture_path_leader", texture_path_leader, pAnimationAsset);
			preloadSpritePath("texture_path_warrior", texture_path_warrior, pAnimationAsset);
			preloadSpritePath("texture_head_king", texture_head_king, pAnimationAsset, pLoadHeads: true, pThrowError: true, pSpecialHead: true);
			preloadSpritePath("texture_head_warrior", texture_head_warrior, pAnimationAsset, pLoadHeads: true, pThrowError: true, pSpecialHead: true);
			preloadSpritePath("texture_heads_male", texture_heads_male, pAnimationAsset, pLoadHeads: true);
			preloadSpritePath("texture_heads_female", texture_heads_female, pAnimationAsset, pLoadHeads: true);
		}
		else
		{
			preloadSpritePath("texture_heads", texture_heads, pAnimationAsset, pLoadHeads: true, pThrowError: false);
		}
	}

	private bool preloadSpritePath(string pType, string pPath, IAnimationFrames pAnimationAsset, bool pLoadHeads = false, bool pThrowError = true, bool pSpecialHead = false)
	{
		if (string.IsNullOrEmpty(pPath))
		{
			return false;
		}
		if (dict_mains.ContainsKey(pPath))
		{
			return false;
		}
		Sprite[] tSprites = SpriteTextureLoader.getSpriteList(pPath);
		if (!pLoadHeads)
		{
			dict_mains.Add(pPath, tSprites);
		}
		all_preloaded_sprites_units.AddRange(tSprites);
		if (tSprites.Length == 0)
		{
			if (pThrowError)
			{
				logAssetError("ActorAssetLibrary: <e>" + pType + "</e> doesn't exist for <e>" + ((Asset)pAnimationAsset).id + "</e> at ", pPath);
			}
			return false;
		}
		if (pLoadHeads)
		{
			if (has_advanced_textures)
			{
				for (int i = 0; i < tSprites.Length; i++)
				{
					if (pSpecialHead)
					{
						ActorAnimationLoader.getHeadSpecial(pPath);
					}
					else
					{
						ActorAnimationLoader.getHead(pPath, i);
					}
				}
			}
			checkHeads(tSprites, pPath, pAnimationAsset);
		}
		else
		{
			checkAnimations(tSprites, pPath, (Asset)pAnimationAsset, pAnimationAsset);
		}
		return true;
	}

	internal void loadShadow()
	{
		shadow_size = (shadow_sprite = getShadowSprite(shadow_texture)).rect.size;
		shadow_size_egg = (shadow_sprite_egg = getShadowSprite(shadow_texture_egg)).rect.size;
		shadow_size_baby = (shadow_sprite_baby = getShadowSprite(shadow_texture_baby)).rect.size;
	}

	private Sprite getShadowSprite(string pTexturePath)
	{
		if (!_shadow_sprites.ContainsKey(pTexturePath))
		{
			Sprite tSprite = SpriteTextureLoader.getSprite("shadows/" + pTexturePath);
			if (tSprite == null)
			{
				Debug.LogError("Shadow not found " + pTexturePath);
			}
			_shadow_sprites.Add(pTexturePath, tSprite);
		}
		Sprite sprite = _shadow_sprites[pTexturePath];
		return DynamicSprites.getShadowUnit(sprite, sprite.GetHashCode());
	}

	private void checkHeads(Sprite[] pSprites, string pPath, IAnimationFrames pAnimationAsset)
	{
		using ListPool<string> tSpriteNames = new ListPool<string>();
		for (int i = 0; i < pSprites.Length; i++)
		{
			string tName = pSprites[i].name;
			if (tName.Any(char.IsDigit))
			{
				if (tSpriteNames.Contains(tName))
				{
					Debug.LogError("Duplicate head " + tName);
				}
				tSpriteNames.Add(tName);
			}
		}
		tSpriteNames.Sort(headSorter);
		string tLastHead = "";
		int tLastId = -1;
		foreach (ref string item in tSpriteNames)
		{
			string[] array = item.Split("_");
			string tHead = array[0];
			if (!int.TryParse(array[1], out var tIndex))
			{
				continue;
			}
			if (tHead != tLastHead)
			{
				tLastHead = tHead;
				if (tIndex != 0)
				{
					logAssetError("ActorAssetLibrary: <e>" + ((Asset)pAnimationAsset).id + "</e> missing head: <e>" + tHead + "_0</e> at ", pPath);
				}
			}
			else if (tIndex != tLastId + 1)
			{
				logAssetError($"ActorAssetLibrary: <e>{((Asset)pAnimationAsset).id}</e> missing head: <e>{tHead}_{tLastId + 1}</e> at ", pPath);
			}
			tLastId = tIndex;
		}
	}

	private int headSorter(string x, string y)
	{
		Match xRegexResult = _regex_heads_sorter.Match(x);
		Match yRegexResult = _regex_heads_sorter.Match(y);
		if (xRegexResult.Success && yRegexResult.Success && xRegexResult.Groups[1].Value == yRegexResult.Groups[1].Value && int.TryParse(xRegexResult.Groups[2].Value, out var tIntX) && int.TryParse(yRegexResult.Groups[2].Value, out var tIntY))
		{
			return tIntX.CompareTo(tIntY);
		}
		return x.CompareTo(y);
	}

	private void checkAnimations(Sprite[] pSprites, string pPath, Asset pAsset, IAnimationFrames pAnimationFrames)
	{
		using ListPool<string> tSpriteNames = new ListPool<string>();
		foreach (Sprite tSprite in pSprites)
		{
			tSpriteNames.Add(tSprite.name);
		}
		using ListPool<string> tMissing = new ListPool<string>();
		string[] walk = pAnimationFrames.getWalk();
		string[] walk2;
		if (walk != null && walk.Length != 0)
		{
			tMissing.Clear();
			bool tFound = false;
			walk2 = pAnimationFrames.getWalk();
			foreach (string tAnimation in walk2)
			{
				if (!tSpriteNames.Contains(tAnimation))
				{
					tMissing.Add(tAnimation);
				}
				else
				{
					tFound = true;
				}
			}
			if (!tFound)
			{
				logAssetError("ActorAssetLibrary: <e>" + pAsset.id + "</e> missing all animation_walk sprites: <e>" + string.Join(", ", tMissing) + "</e> at ", pPath);
			}
		}
		string[] swim = pAnimationFrames.getSwim();
		if (swim != null && swim.Length != 0)
		{
			tMissing.Clear();
			bool tFound2 = false;
			walk2 = pAnimationFrames.getSwim();
			foreach (string tAnimation2 in walk2)
			{
				if (!tSpriteNames.Contains(tAnimation2))
				{
					tMissing.Add(tAnimation2);
				}
				else
				{
					tFound2 = true;
				}
			}
			if (!tFound2)
			{
				logAssetError("ActorAssetLibrary: <e>" + pAsset.id + "</e> missing all animation_swim sprites: <e>" + string.Join(", ", tMissing) + "</e> at ", pPath);
			}
		}
		string[] idle = pAnimationFrames.getIdle();
		if (idle == null || idle.Length == 0)
		{
			return;
		}
		tMissing.Clear();
		bool tFound3 = false;
		walk2 = pAnimationFrames.getIdle();
		foreach (string tAnimation3 in walk2)
		{
			if (!tSpriteNames.Contains(tAnimation3))
			{
				tMissing.Add(tAnimation3);
			}
			else
			{
				tFound3 = true;
			}
		}
		if (!tFound3)
		{
			logAssetError("ActorAssetLibrary: <e>" + pAsset.id + "</e> missing all animation_idle sprites: <e>" + string.Join(", ", tMissing) + "</e> at ", pPath);
		}
	}

	public static int getTotal()
	{
		return _total;
	}
}
