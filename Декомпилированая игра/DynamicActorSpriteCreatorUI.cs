using UnityEngine;

public static class DynamicActorSpriteCreatorUI
{
	private static int[] _boat_angles = new int[8] { 0, -45, -90, -135, 180, 135, 90, 45 };

	public static AnimationContainerUnit getContainerForUI(ActorAsset pAsset, bool pAdult, ActorTextureSubAsset pTextureAsset, SubspeciesTrait pMutationAsset = null, bool pIsEgg = false, SubspeciesTrait pEggAsset = null, string pTexturePath = null)
	{
		string tPath = ((!string.IsNullOrEmpty(pTexturePath)) ? pTexturePath : (pIsEgg ? pEggAsset.sprite_path : ((pAdult || !pAsset.has_baby_form) ? pTextureAsset.getUnitTexturePathForUI(pAsset) : pTextureAsset.texture_path_baby)));
		return ActorAnimationLoader.getAnimationContainer(tPath, pAsset, pEggAsset, pMutationAsset);
	}

	public static AnimationContainerUnit getContainerForUI(Actor pActor)
	{
		Subspecies tSubspecies = pActor.subspecies;
		ActorAsset tAsset = pActor.asset;
		SubspeciesTrait tMutationAsset = null;
		ActorTextureSubAsset tTextureAsset;
		if (pActor.hasSubspecies() && tSubspecies.has_mutation_reskin)
		{
			tMutationAsset = tSubspecies.mutation_skin_asset;
			tTextureAsset = tMutationAsset.texture_asset;
		}
		else
		{
			tTextureAsset = tAsset.texture_asset;
		}
		return getContainerForUI(tAsset, pActor.isAdult(), tTextureAsset, tMutationAsset);
	}

	public static Sprite getUnitSpriteForUI(ActorAsset pAsset, Sprite pMainSprite, AnimationContainerUnit pContainer, bool pAdult, ActorSex pSex, int pPhenotypeIndex, int pPhenotypeShade, ColorAsset pKingdomColor, long pActorId, int pHeadId, bool pEgg = false, bool pKing = false, bool pWarrior = false, bool pWise = false)
	{
		long t_kingdomID = 0L;
		long t_phenotypeIndex = pPhenotypeIndex;
		long t_phenotypeShadeID = 0L;
		long t_headID = 0L;
		long t_bodyID = DynamicSpriteCreator.getBodySpriteSmallID(pMainSprite);
		if (t_phenotypeIndex != 0L)
		{
			t_phenotypeShadeID = pPhenotypeShade + 1;
		}
		Sprite tHeadSprite = getSpriteHeadForUI(pAsset, pSex, pContainer, pActorId, pHeadId, pAdult, pEgg, pKing, pWarrior, pWise);
		int tHeadId = 0;
		if (tHeadSprite != null)
		{
			ActorAnimationLoader.int_ids_heads.TryGetValue(tHeadSprite, out tHeadId);
			if (tHeadId == 0)
			{
				int tNewID = ActorAnimationLoader.int_ids_heads.Count + 1;
				ActorAnimationLoader.int_ids_heads.Add(tHeadSprite, tNewID);
				tHeadId = tNewID;
			}
			t_headID = tHeadId;
		}
		if (pKingdomColor != null)
		{
			t_kingdomID = pKingdomColor.index_id + 1;
		}
		long tId = t_kingdomID * 10000000 + t_headID * 1000000 + t_bodyID * 1000 + t_phenotypeIndex * 10 + t_phenotypeShadeID;
		AnimationFrameData tFrameData = null;
		pContainer?.dict_frame_data.TryGetValue(pMainSprite.name, out tFrameData);
		DynamicSpritesAsset tAsset = DynamicSpritesLibrary.units;
		Sprite tResult = tAsset.getSprite(tId);
		if ((object)tResult == null)
		{
			tResult = DynamicSpriteCreator.createNewSpriteUnit(tFrameData, pMainSprite, tHeadSprite, pKingdomColor, pAsset, pPhenotypeIndex, pPhenotypeShade, pAsset.texture_atlas);
			tAsset.addSprite(tId, tResult);
		}
		return tResult;
	}

	public static Sprite getUnitSpriteForUI(Actor pActor, Sprite pSprite)
	{
		ActorAsset tAsset = pActor.asset;
		AnimationContainerUnit tContainer = pActor.animation_container;
		if (tAsset.has_override_avatar_frames)
		{
			return tAsset.get_override_avatar_frames(pActor)[0];
		}
		int tPhenotypeShade = pActor.data.phenotype_shade;
		int tPhenotypeIndex = pActor.data.phenotype_index;
		return getUnitSpriteForUI(pActor.asset, pSprite, tContainer, pActor.isAdult(), pActor.data.sex, tPhenotypeIndex, tPhenotypeShade, pActor.kingdom.getColor(), pActor.data.id, pActor.data.head, pActor.isEgg());
	}

	public static Sprite getSpriteHeadForUI(ActorAsset pAsset, ActorSex pSex, AnimationContainerUnit pContainer, long pActorId, int pHeadId, bool pAdult = true, bool pEgg = false, bool pKing = false, bool pWarrior = false, bool pWise = false, bool pRandom = false)
	{
		if (pEgg)
		{
			return null;
		}
		if (pAsset.is_boat)
		{
			return null;
		}
		if (!pAdult && !pContainer.render_heads_for_children)
		{
			return null;
		}
		string tHeadPath = "";
		int tHeadIndex = 0;
		bool tSpecial = false;
		ActorTextureSubAsset tTextureAsset = pAsset.texture_asset;
		if (!tTextureAsset.has_advanced_textures)
		{
			Sprite[] heads = pContainer.heads;
			if (heads != null && heads.Length != 0)
			{
				if (pRandom)
				{
					return pContainer.heads.GetRandom();
				}
				tHeadIndex = AnimationHelper.getSpriteIndex(pActorId, pContainer.heads.Length);
				return getSprite(tHeadIndex, pContainer.heads);
			}
			return null;
		}
		if (pKing)
		{
			tHeadPath = tTextureAsset.texture_head_king;
			tSpecial = true;
		}
		else if (pWarrior)
		{
			tHeadPath = tTextureAsset.texture_head_warrior;
			tSpecial = true;
		}
		else if (pWise && tTextureAsset.has_old_heads)
		{
			tSpecial = true;
			tHeadPath = ((pSex != ActorSex.Male) ? tTextureAsset.texture_heads_old_female : tTextureAsset.texture_heads_old_male);
		}
		if (tSpecial)
		{
			return ActorAnimationLoader.getHeadSpecial(tHeadPath);
		}
		if (pSex == ActorSex.Male)
		{
			if (pRandom)
			{
				return pContainer.heads_male.GetRandom();
			}
			tHeadIndex = ((pHeadId != -1) ? pHeadId : AnimationHelper.getSpriteIndex(pActorId, pContainer.heads_male.Length));
			return getSprite(tHeadIndex, pContainer.heads_male);
		}
		if (pRandom)
		{
			return pContainer.heads_female.GetRandom();
		}
		tHeadIndex = ((pHeadId != -1) ? pHeadId : AnimationHelper.getSpriteIndex(pActorId, pContainer.heads_female.Length));
		return getSprite(tHeadIndex, pContainer.heads_female);
	}

	private static Sprite getSprite(int pIndex, Sprite[] pSprites)
	{
		return pSprites[pIndex];
	}

	public static ActorAnimation getBoatAnimation(AnimationDataBoat pBoatAnimation)
	{
		ActorAnimation tResult = new ActorAnimation();
		Sprite[] tSprites = new Sprite[_boat_angles.Length * 2];
		for (int i = 0; i < _boat_angles.Length; i++)
		{
			int tAngle = _boat_angles[i];
			ActorAnimation tAnim = pBoatAnimation.dict[tAngle];
			int tArrayIndex = i * 2;
			tSprites[tArrayIndex] = tAnim.frames[0];
			tSprites[tArrayIndex + 1] = tAnim.frames[1];
		}
		tResult.frames = tSprites;
		return tResult;
	}
}
