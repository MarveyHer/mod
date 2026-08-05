using System.Collections.Generic;
using UnityEngine;

public static class ActorAnimationLoader
{
	public static readonly Dictionary<Sprite, int> int_ids_heads = new Dictionary<Sprite, int>();

	private static readonly Dictionary<string, AnimationContainerUnit> _dict_units = new Dictionary<string, AnimationContainerUnit>();

	private static readonly Dictionary<string, AnimationDataBoat> _dict_boats = new Dictionary<string, AnimationDataBoat>();

	private static readonly Dictionary<string, Sprite> _dict_civ_heads = new Dictionary<string, Sprite>();

	public static int count_units => _dict_units.Count;

	public static int count_boats => _dict_boats.Count;

	public static int count_heads => _dict_civ_heads.Count;

	public static Sprite getHeadSpecial(string pPath)
	{
		if (!_dict_civ_heads.TryGetValue(pPath, out var tResult))
		{
			Sprite[] spriteList = SpriteTextureLoader.getSpriteList(pPath);
			foreach (Sprite tSprite in spriteList)
			{
				_dict_civ_heads.TryAdd(pPath, tSprite);
			}
			return _dict_civ_heads[pPath];
		}
		return tResult;
	}

	public static Sprite getHead(string pPath, int pHeadIndex)
	{
		string tHeadID = $"{pPath}_head_{pHeadIndex}";
		if (!_dict_civ_heads.TryGetValue(tHeadID, out var tResult))
		{
			Sprite[] spriteList = SpriteTextureLoader.getSpriteList(pPath);
			foreach (Sprite tSprite in spriteList)
			{
				string tNewID = pPath + "_" + tSprite.name;
				_dict_civ_heads.TryAdd(tNewID, tSprite);
			}
			return _dict_civ_heads[tHeadID];
		}
		return tResult;
	}

	public static AnimationDataBoat loadAnimationBoat(string pTexturePath)
	{
		if (!_dict_boats.TryGetValue(pTexturePath, out var tAnimationData))
		{
			Dictionary<string, Sprite> tDict = new Dictionary<string, Sprite>();
			Sprite[] sprites = SpriteTextureLoader.getSpriteList("actors/boats/" + pTexturePath);
			Sprite[] array = sprites;
			foreach (Sprite tSprite in array)
			{
				tDict.Add(tSprite.name, tSprite);
			}
			tAnimationData = new AnimationDataBoat();
			tAnimationData.broken = new ActorAnimation();
			tAnimationData.broken.frames = new Sprite[1] { tDict["broken"] };
			tAnimationData.normal = new ActorAnimation();
			tAnimationData.normal.frames = new Sprite[1] { tDict["normal"] };
			array = sprites;
			foreach (Sprite tSprite2 in array)
			{
				if (!tSprite2.name.Contains("@1") && tSprite2.name.Contains("@"))
				{
					createBoatAnimationArray(tAnimationData, tDict, tSprite2.name);
				}
			}
			_dict_boats[pTexturePath] = tAnimationData;
		}
		return tAnimationData;
	}

	private static void createBoatAnimationArray(AnimationDataBoat pAnimationData, Dictionary<string, Sprite> pDict, string pID, float pTimeBetween = 0.2f)
	{
		int tID_main = int.Parse(pID.Split('@')[0]);
		ActorAnimation tAnim = new ActorAnimation();
		tAnim.frames = new Sprite[2];
		tAnim.frames[0] = pDict[tID_main + "@" + 0];
		tAnim.frames[1] = pDict[tID_main + "@" + 1];
		pAnimationData.dict.Add(tID_main, tAnim);
	}

	public static AnimationContainerUnit getAnimationContainer(string pTexturePath, ActorAsset pAsset, SubspeciesTrait pEggAsset = null, SubspeciesTrait pMutationSkinAsset = null)
	{
		if (!_dict_units.TryGetValue(pTexturePath, out var tAnimationContainer))
		{
			return createAnimationContainer(pTexturePath, pAsset, pEggAsset, pMutationSkinAsset);
		}
		return tAnimationContainer;
	}

	private static AnimationContainerUnit createAnimationContainer(string pTexturePath, ActorAsset pAsset, SubspeciesTrait pEggAsset, SubspeciesTrait pMutationSkinAsset = null)
	{
		AnimationContainerUnit tAnim = new AnimationContainerUnit(pTexturePath);
		_dict_units.Add(pTexturePath, tAnim);
		string[] tAnimationWalk;
		string[] tAnimationSwim;
		string[] tAnimationIdle;
		if (pTexturePath.Contains("eggs/"))
		{
			tAnimationWalk = pEggAsset.animation_walk;
			tAnimationSwim = pEggAsset.animation_swim;
			tAnimationIdle = pEggAsset.animation_idle;
		}
		else if (pTexturePath.Contains("species/mutations"))
		{
			tAnimationWalk = pMutationSkinAsset.animation_walk;
			tAnimationSwim = pMutationSkinAsset.animation_swim;
			tAnimationIdle = pMutationSkinAsset.animation_idle;
		}
		else
		{
			tAnimationWalk = pAsset.animation_walk;
			tAnimationSwim = pAsset.animation_swim;
			tAnimationIdle = pAsset.animation_idle;
		}
		generateFrameData(pTexturePath, tAnim, tAnim.sprites, tAnimationSwim);
		generateFrameData(pTexturePath, tAnim, tAnim.sprites, tAnimationWalk);
		generateFrameData(pTexturePath, tAnim, tAnim.sprites, tAnimationIdle);
		if (tAnimationSwim != null && tAnimationSwim.Length != 0)
		{
			tAnim.swimming = createAnim(0, tAnim.sprites, tAnimationSwim);
			if (tAnim.swimming != null)
			{
				tAnim.has_swimming = true;
			}
		}
		if (tAnimationWalk != null && tAnimationWalk.Length != 0)
		{
			tAnim.walking = createAnim(1, tAnim.sprites, tAnimationWalk);
			if (tAnim.walking != null)
			{
				tAnim.has_walking = true;
			}
		}
		if (tAnimationIdle != null && tAnimationIdle.Length != 0)
		{
			tAnim.idle = createAnim(2, tAnim.sprites, tAnimationIdle);
			if (tAnim.idle != null)
			{
				tAnim.has_idle = true;
			}
		}
		if (pTexturePath.Contains("/child"))
		{
			tAnim.child = true;
		}
		ActorTextureSubAsset tTextureAsset = ((pMutationSkinAsset == null || !pMutationSkinAsset.is_mutation_skin) ? pAsset.texture_asset : pMutationSkinAsset.texture_asset);
		if (tTextureAsset.texture_heads != string.Empty)
		{
			tAnim.heads = SpriteTextureLoader.getSpriteList(tTextureAsset.texture_heads);
		}
		if (tTextureAsset.texture_heads_male != string.Empty)
		{
			tAnim.heads_male = SpriteTextureLoader.getSpriteList(tTextureAsset.texture_heads_male);
		}
		if (tTextureAsset.texture_heads_female != string.Empty)
		{
			tAnim.heads_female = SpriteTextureLoader.getSpriteList(tTextureAsset.texture_heads_female);
		}
		if (tAnim.heads == null || tAnim.heads.Length == 0)
		{
			tAnim.heads = tAnim.heads_male;
		}
		if (tTextureAsset.render_heads_for_children)
		{
			tAnim.render_heads_for_children = true;
		}
		return tAnim;
	}

	private static void generateFrameData(string pFrameString, AnimationContainerUnit pAnimContainer, Dictionary<string, Sprite> pFrames, string[] pStringIDs)
	{
		if (string.IsNullOrEmpty(pFrameString) || pStringIDs == null)
		{
			return;
		}
		foreach (string tID in pStringIDs)
		{
			if (!pAnimContainer.dict_frame_data.ContainsKey(tID) && pFrames.ContainsKey(tID))
			{
				AnimationFrameData tFrameData = new AnimationFrameData();
				tFrameData.id = tID;
				tFrameData.sheet_path = pFrameString;
				Sprite tBodySprite = pFrames[tID];
				tFrameData.size_unit = tBodySprite.rect.size;
				string tFrameHeadID = tID + "_head";
				if (pFrames.TryGetValue(tFrameHeadID, out var tCoSprite1))
				{
					float tX = tCoSprite1.rect.x - tBodySprite.rect.x;
					tX = tX - tBodySprite.pivot.x + tCoSprite1.pivot.x;
					float tY = tCoSprite1.rect.y - tBodySprite.rect.y;
					tY = tY - tBodySprite.pivot.y + tCoSprite1.pivot.y;
					tFrameData.pos_head = new Vector2(tX, tY);
					float tNewX = tCoSprite1.rect.x - tBodySprite.rect.x;
					float tNewY = tCoSprite1.rect.y - tBodySprite.rect.y;
					tFrameData.pos_head_new = new Vector2(tNewX, tNewY);
					tFrameData.show_head = true;
				}
				string tFrameItemID = tID + "_item";
				if (pFrames.TryGetValue(tFrameItemID, out var tCoSprite2))
				{
					float tX2 = tCoSprite2.rect.x - tBodySprite.rect.x;
					tX2 = tX2 - tBodySprite.pivot.x + tCoSprite2.pivot.x;
					float tY2 = tCoSprite2.rect.y - tBodySprite.rect.y;
					tY2 = tY2 - tBodySprite.pivot.y + tCoSprite2.pivot.y;
					tFrameData.pos_item = new Vector2(tX2, tY2);
					tFrameData.show_item = true;
				}
				pAnimContainer.dict_frame_data.Add(tID, tFrameData);
			}
		}
	}

	private static ActorAnimation createAnim(int pID, Dictionary<string, Sprite> pDict, string[] pStringIDs)
	{
		Sprite[] tFrames = createArray(pDict, pStringIDs);
		if (tFrames.Length == 0)
		{
			return null;
		}
		return new ActorAnimation
		{
			id = pID,
			frames = tFrames
		};
	}

	private static Sprite[] createArray(Dictionary<string, Sprite> pDict, string[] pStringIDs)
	{
		using ListPool<Sprite> tSprites = new ListPool<Sprite>(pStringIDs.Length);
		foreach (string tString in pStringIDs)
		{
			if (!pDict.TryGetValue(tString, out var tSprite))
			{
				break;
			}
			tSprites.Add(tSprite);
		}
		return tSprites.ToArray();
	}
}
