using System.Collections.Generic;
using UnityEngine;

public static class DynamicSpriteCreator
{
	public static Actor debug_actor;

	private static Dictionary<Sprite, int> _int_ids_body = new Dictionary<Sprite, int>();

	private static readonly Color32 _placeholder_color_skin = Toolbox.makeColor("#00FF00");

	private static readonly List<Vector2Int> _light_colors = new List<Vector2Int>();

	public static Sprite createNewItemSprite(DynamicSpritesAsset pAsset, Sprite pSource, ColorAsset pKingdomColor)
	{
		UnitSpriteConstructorAtlas tAtlas = pAsset.getAtlas();
		Rect tTextureRectBody = pSource.rect;
		int tWidth = (int)tTextureRectBody.width;
		int tHeight = (int)tTextureRectBody.height;
		tAtlas.checkBounds(tWidth, tHeight);
		int tTextureWidth = tAtlas.texture.width;
		_ = tAtlas.texture.height;
		Color32[] tPartPixels = pSource.texture.GetPixels32();
		int tBodyTextureWidth = pSource.texture.width;
		for (int xx = 0; (float)xx < tTextureRectBody.width; xx++)
		{
			for (int yy = 0; (float)yy < tTextureRectBody.height; yy++)
			{
				int num = xx + (int)tTextureRectBody.x;
				int tPixelY = yy + (int)tTextureRectBody.y;
				int tPixelID = num + tPixelY * tBodyTextureWidth;
				Color32 tColor = tPartPixels[tPixelID];
				if (tColor.a != 0)
				{
					tColor = DynamicColorPixelTool.checkSpecialColors(tColor, pKingdomColor, pCheckForLightColors: true);
					int pX = xx + tAtlas.last_x;
					int pY = yy + tAtlas.last_y;
					if (pX < 0)
					{
						pX = 0;
					}
					if (pY < 0)
					{
						pY = 0;
					}
					tPixelID = pX + pY * tTextureWidth;
					tAtlas.pixels[tPixelID] = tColor;
				}
			}
		}
		setAtlasDirty(tAtlas);
		return createFinalSprite(tAtlas, pSource, tWidth, tHeight);
	}

	private static Sprite createFinalSprite(UnitSpriteConstructorAtlas pAtlasTexture, Sprite pMain, int pWidth, int pHeight, int pResizeX = 0, int pResizeY = 0)
	{
		Sprite obj = Sprite.Create(rect: new Rect(pAtlasTexture.last_x, pAtlasTexture.last_y, pWidth, pHeight), pivot: new Vector2((pMain.pivot.x + (float)pResizeX) / (float)pWidth, pMain.pivot.y / (float)pHeight), texture: pAtlasTexture.texture, pixelsPerUnit: 1f);
		obj.name = "gen_" + pMain.name;
		pAtlasTexture.last_x += pWidth + 1;
		return obj;
	}

	private static Sprite createNewSpriteBuildingShadow(DynamicSpritesAsset pDynamicSpritesAsset, BuildingAsset tAsset, Sprite pSource, bool pIsContructionSprite)
	{
		UnitSpriteConstructorAtlas tAtlas = pDynamicSpritesAsset.getAtlas();
		Rect tTextureRectBody = pSource.rect;
		int tBonusWidth = 3;
		int tWidth = (int)tTextureRectBody.width;
		int tHeight = (int)tTextureRectBody.height;
		int tSpriteX = (int)tTextureRectBody.x;
		int tSpriteY = (int)tTextureRectBody.y;
		tAtlas.checkBounds(tWidth + tBonusWidth, tHeight);
		int tDrawTextureWidth = tAtlas.texture.width;
		_ = tAtlas.texture.height;
		Color32[] tPartPixels = pSource.texture.GetPixels32();
		Vector2 tShadowBound;
		float tDistortion;
		if (pIsContructionSprite)
		{
			tShadowBound = BuildingLibrary.shadow_under_construction_bound;
			tDistortion = BuildingLibrary.shadow_under_construction_distortion;
		}
		else
		{
			tShadowBound = tAsset.shadow_bound;
			tDistortion = tAsset.shadow_distortion;
		}
		int tBoundX = (int)(tShadowBound.x * (float)tWidth);
		int tBoundY = (int)((float)tHeight * tShadowBound.y);
		List<Vector2Int> tListAdds = new List<Vector2Int>();
		Color32 tColorBlack = Color.black;
		int tBodyTextureWidth = pSource.texture.width;
		for (int xx = 0; xx < tWidth; xx++)
		{
			for (int yy = 0; yy < tHeight; yy++)
			{
				int num = xx + tSpriteX;
				int tPixelY = yy + tSpriteY;
				int tPixelID = num + tPixelY * tBodyTextureWidth;
				Color32 tColor = tPartPixels[tPixelID];
				if (tColor.a == 0)
				{
					continue;
				}
				tColor = tColorBlack;
				if (xx >= tBoundX)
				{
					int pX = xx + tAtlas.last_x;
					int pY = yy + tAtlas.last_y;
					if (yy > tBoundY)
					{
						pY = (int)((float)yy * tDistortion) + tAtlas.last_y;
					}
					if (pX < 0)
					{
						pX = 0;
					}
					if (pY < 0)
					{
						pY = 0;
					}
					tListAdds.Add(new Vector2Int(pX, pY));
					tPixelID = pX + pY * tDrawTextureWidth;
					tAtlas.pixels[tPixelID] = tColor;
				}
			}
		}
		setAtlasDirty(tAtlas);
		tWidth += tBonusWidth;
		foreach (Vector2Int tPix in tListAdds)
		{
			int num2 = tPix.x + 1;
			int tPy = tPix.y;
			int pixelId = num2 + tPy * tDrawTextureWidth;
			tAtlas.pixels[pixelId] = tColorBlack;
			int num3 = tPix.x + 2;
			tPy = tPix.y;
			pixelId = num3 + tPy * tDrawTextureWidth;
			tAtlas.pixels[pixelId] = tColorBlack;
			int num4 = tPix.x + 1;
			tPy = tPix.y + 1;
			pixelId = num4 + tPy * tDrawTextureWidth;
			tAtlas.pixels[pixelId] = tColorBlack;
		}
		return createFinalSprite(tAtlas, pSource, tWidth, tHeight);
	}

	public static Sprite createNewUnitShadow(DynamicSpritesAsset pAsset, Sprite pSource)
	{
		UnitSpriteConstructorAtlas tAtlas = pAsset.getAtlas();
		Rect tTextureRectBody = pSource.rect;
		int tBonusWidth = 1;
		int tWidth = (int)tTextureRectBody.width;
		int tHeight = (int)tTextureRectBody.height;
		int tSpriteX = (int)tTextureRectBody.x;
		int tSpriteY = (int)tTextureRectBody.y;
		tAtlas.checkBounds(tWidth + tBonusWidth, tHeight);
		int tDrawTextureWidth = tAtlas.texture.width;
		_ = tAtlas.texture.height;
		Color32[] tPartPixels = pSource.texture.GetPixels32();
		int tBodyTextureWidth = pSource.texture.width;
		for (int xx = 0; xx < tWidth; xx++)
		{
			for (int yy = 0; yy < tHeight; yy++)
			{
				int num = xx + tSpriteX;
				int tPixelY = yy + tSpriteY;
				int tPixelId = num + tPixelY * tBodyTextureWidth;
				Color32 tColor = tPartPixels[tPixelId];
				if (tColor.a != 0)
				{
					int pX = xx + tAtlas.last_x;
					int pY = yy + tAtlas.last_y;
					if (pX < 0)
					{
						pX = 0;
					}
					if (pY < 0)
					{
						pY = 0;
					}
					tPixelId = pX + pY * tDrawTextureWidth;
					tAtlas.pixels[tPixelId] = tColor;
				}
			}
		}
		tWidth += tBonusWidth;
		setAtlasDirty(tAtlas);
		return createFinalSprite(tAtlas, pSource, tWidth, tHeight);
	}

	public static void createBuildingShadow(BuildingAsset pAsset, Sprite pSprite, bool pIsContructionSprite)
	{
		DynamicSpritesAsset building_shadows = DynamicSpritesLibrary.building_shadows;
		int tId = pSprite.GetHashCode();
		Sprite tResult = createNewSpriteBuildingShadow(building_shadows, pAsset, pSprite, pIsContructionSprite);
		building_shadows.addSprite(tId, tResult);
	}

	public static Sprite createNewIcon(DynamicSpritesAsset pAsset, Sprite pSource, ColorAsset pKingdomColor, PhenotypeAsset pPhenotype = null)
	{
		UnitSpriteConstructorAtlas tAtlas = pAsset.getAtlas();
		if (pPhenotype != null)
		{
			DynamicColorPixelTool.loadSkinColorsPreview(pPhenotype, 0);
		}
		Rect tTextureRectBody = pSource.rect;
		int tWidth = (int)tTextureRectBody.width;
		int tHeight = (int)tTextureRectBody.height;
		tAtlas.checkBounds(tWidth, tHeight);
		int tTextureWidth = tAtlas.texture.width;
		_ = tAtlas.texture.height;
		Color32[] tPartPixels = pSource.texture.GetPixels32();
		int tBodyTextureWidth = pSource.texture.width;
		for (int xx = 0; (float)xx < tTextureRectBody.width; xx++)
		{
			for (int yy = 0; (float)yy < tTextureRectBody.height; yy++)
			{
				int num = xx + (int)tTextureRectBody.x;
				int tPixelY = yy + (int)tTextureRectBody.y;
				int tPixelID = num + tPixelY * tBodyTextureWidth;
				Color32 tColor = tPartPixels[tPixelID];
				if (tColor.a != 0)
				{
					tColor = DynamicColorPixelTool.checkSpecialColors(tColor, pKingdomColor, pCheckForLightColors: true);
					int pX = xx + tAtlas.last_x;
					int pY = yy + tAtlas.last_y;
					if (pX < 0)
					{
						pX = 0;
					}
					if (pY < 0)
					{
						pY = 0;
					}
					tPixelID = pX + pY * tTextureWidth;
					tAtlas.pixels[tPixelID] = tColor;
				}
			}
		}
		setAtlasDirty(tAtlas);
		return createFinalSprite(tAtlas, pSource, tWidth, tHeight);
	}

	public static Sprite createNewSpriteBuilding(DynamicSpritesAsset pAssetAtlas, long pID, Sprite pSource, ColorAsset pKingdomColor)
	{
		UnitSpriteConstructorAtlas tAtlas = pAssetAtlas.getAtlas();
		Rect tTextureRectBody = pSource.rect;
		int tWidth = (int)tTextureRectBody.width;
		int tHeight = (int)tTextureRectBody.height;
		tAtlas.checkBounds(tWidth, tHeight);
		int tTextureWidth = tAtlas.texture.width;
		_ = tAtlas.texture.height;
		Color32[] tPartPixels = pSource.texture.GetPixels32();
		_light_colors.Clear();
		int tBodyTextureWidth = pSource.texture.width;
		for (int xx = 0; (float)xx < tTextureRectBody.width; xx++)
		{
			for (int yy = 0; (float)yy < tTextureRectBody.height; yy++)
			{
				int num = xx + (int)tTextureRectBody.x;
				int tPixelY = yy + (int)tTextureRectBody.y;
				int pixelId = num + tPixelY * tBodyTextureWidth;
				Color32 tColor = tPartPixels[pixelId];
				if (tColor.a != 0)
				{
					if (Toolbox.areColorsEqual(tColor, Toolbox.color_light))
					{
						_light_colors.Add(new Vector2Int(xx, yy));
					}
					tColor = DynamicColorPixelTool.checkSpecialColors(tColor, pKingdomColor, pCheckForLightColors: true);
					int pX = xx + tAtlas.last_x;
					int pY = yy + tAtlas.last_y;
					if (pX < 0)
					{
						pX = 0;
					}
					if (pY < 0)
					{
						pY = 0;
					}
					pixelId = pX + pY * tTextureWidth;
					tAtlas.pixels[pixelId] = tColor;
				}
			}
		}
		setAtlasDirty(tAtlas);
		Sprite result = createFinalSprite(tAtlas, pSource, tWidth, tHeight);
		if (_light_colors.Count > 0)
		{
			checkBuildingLightSprite(DynamicSpritesLibrary.building_lights, pSource.GetHashCode(), pSource);
		}
		return result;
	}

	private static void checkBuildingLightSprite(DynamicSpritesAsset pQuantumAsset, long pHashcodeMainSprite, Sprite pSprite)
	{
		Sprite tResult = pQuantumAsset.getSprite(pHashcodeMainSprite);
		if ((object)tResult == null)
		{
			tResult = createNewSpriteBuildingLight(pQuantumAsset, pSprite);
			pQuantumAsset.addSprite(pHashcodeMainSprite, tResult);
		}
	}

	public static Sprite createNewSpriteBuildingLight(DynamicSpritesAsset pAsset, Sprite pSource)
	{
		UnitSpriteConstructorAtlas tAtlas = pAsset.getAtlas();
		Rect tTextureRectBody = pSource.rect;
		int tWidth = (int)tTextureRectBody.width;
		int tHeight = (int)tTextureRectBody.height;
		tAtlas.checkBounds(tWidth, tHeight);
		int tBodyTextureWidth = pSource.texture.width;
		for (int i = 0; i < _light_colors.Count; i++)
		{
			Vector2Int tColorCoords = _light_colors[i];
			drawLightPixel(tAtlas, tColorCoords.x, tColorCoords.y, tWidth, tHeight, tBodyTextureWidth, Toolbox.color_light_100);
			drawLightPixel(tAtlas, tColorCoords.x, tColorCoords.y - 1, tWidth, tHeight, tBodyTextureWidth, Toolbox.color_light_10);
			drawLightPixel(tAtlas, tColorCoords.x - 1, tColorCoords.y, tWidth, tHeight, tBodyTextureWidth, Toolbox.color_light_10);
			drawLightPixel(tAtlas, tColorCoords.x + 1, tColorCoords.y, tWidth, tHeight, tBodyTextureWidth, Toolbox.color_light_10);
			drawLightPixel(tAtlas, tColorCoords.x, tColorCoords.y + 1, tWidth, tHeight, tBodyTextureWidth, Toolbox.color_light_10);
		}
		setAtlasDirty(tAtlas);
		return createFinalSprite(tAtlas, pSource, tWidth, tHeight);
	}

	private static void drawLightPixel(UnitSpriteConstructorAtlas pAtlas, int pColorCoordsX, int pColorCoordsY, int pWidth, int pHeight, int pBodyTextureWidth, Color32 pColor)
	{
		int pX = pColorCoordsX + pAtlas.last_x;
		int pY = pColorCoordsY + pAtlas.last_y;
		if (pX < 0)
		{
			pX = 0;
		}
		if (pY < 0)
		{
			pY = 0;
		}
		int tPixelID = pX + pY * pAtlas.texture.width;
		if (pAtlas.pixels[tPixelID].a < pColor.a)
		{
			pAtlas.pixels[tPixelID] = pColor;
		}
	}

	public static Sprite createNewSpriteForDebug(Sprite pSpriteSource, ColorAsset pKingdomColor)
	{
		Rect tTextureRectBody = pSpriteSource.rect;
		int width = (int)tTextureRectBody.width;
		int tHeight = (int)tTextureRectBody.height;
		Color32[] tMainPixels = pSpriteSource.texture.GetPixels32();
		Texture2D tTexture = new Texture2D(width, tHeight);
		tTexture.filterMode = FilterMode.Point;
		tTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] tNewPixels = tTexture.GetPixels32();
		int tBodyTextureWidth = pSpriteSource.texture.width;
		for (int xx = 0; (float)xx < tTextureRectBody.width; xx++)
		{
			for (int yy = 0; (float)yy < tTextureRectBody.height; yy++)
			{
				int num = xx + (int)tTextureRectBody.x;
				int tPixelY = yy + (int)tTextureRectBody.y;
				int tPixelID = num + tPixelY * tBodyTextureWidth;
				Color32 tColor = tMainPixels[tPixelID];
				if (tColor.a == 0)
				{
					tNewPixels[tPixelID] = tColor;
					continue;
				}
				tColor = DynamicColorPixelTool.checkSpecialColors(tColor, pKingdomColor, pCheckForLightColors: true);
				tNewPixels[tPixelID] = tColor;
			}
		}
		tTexture.SetPixels32(tNewPixels);
		tTexture.Apply();
		Sprite sprite = Sprite.Create(tTexture, tTextureRectBody, pSpriteSource.pivot, 1f);
		sprite.name = "gen_" + pSpriteSource.name;
		return sprite;
	}

	public static Sprite createNewSpriteUnit(AnimationFrameData pFrameData, Sprite pSourceBody, Sprite pSourceHead, ColorAsset pKingdomColor, ActorAsset pAsset, int pPhenotypeIndex, int pPhenotypeShade, UnitTextureAtlasID pAtlasID)
	{
		UnitSpriteConstructorAtlas tAtlas = null;
		switch (pAtlasID)
		{
		case UnitTextureAtlasID.Units:
			tAtlas = DynamicSpritesLibrary.units.getAtlas();
			break;
		case UnitTextureAtlasID.Boats:
			tAtlas = DynamicSpritesLibrary.boats.getAtlas();
			break;
		}
		PixelBag pixelBag = PixelBagManager.getPixelBag(pSourceBody, pCheckPhenotypes: true);
		int tWidth = pixelBag.texture_rect_width;
		int tHeight = pixelBag.texture_rect_height;
		int tAdditionalXHead = 0;
		int tAdditionalYHead = 0;
		DynamicColorPixelTool.setPlaceholderSkinColor(_placeholder_color_skin);
		DynamicColorPixelTool.resetSkinColors();
		if (pPhenotypeIndex != 0)
		{
			DynamicColorPixelTool.loadPhenotype(pPhenotypeIndex, pPhenotypeShade);
		}
		if ((object)pSourceHead != null && pFrameData != null)
		{
			Rect tTextureRectHead = pSourceHead.rect;
			Vector2 tHeadPos = pFrameData.pos_head_new;
			int tDiffY = (int)tHeadPos.y + (int)tTextureRectHead.height - tHeight;
			if (tDiffY > 0)
			{
				tAdditionalYHead = tDiffY;
			}
			int tDiffX = (int)tHeadPos.x + (int)tTextureRectHead.width - tWidth;
			if (tDiffX > 0)
			{
				tAdditionalXHead = tDiffX;
			}
			else if (tHeadPos.x < 0f)
			{
				tAdditionalXHead = -(int)tHeadPos.x;
			}
		}
		int tResizeX = tAdditionalXHead;
		int tResizeY = tAdditionalYHead;
		tWidth += tResizeX;
		tHeight += tResizeY;
		tAtlas.checkBounds(tWidth, tHeight);
		fillDebugColor(tWidth, tHeight, tAtlas);
		bool tDynamicZombie = pAsset.dynamic_sprite_zombie;
		int tPartX = tResizeX + tAtlas.last_x;
		int tPartY = tAtlas.last_y;
		drawPixelsAll(pixelBag, tAtlas, pKingdomColor, tPartX, tPartY, tDynamicZombie, pAsset);
		if ((object)pSourceHead != null && pFrameData != null)
		{
			PixelBag pixelBag2 = PixelBagManager.getPixelBag(pSourceHead, pCheckPhenotypes: true);
			Vector2 pos_head_new = pFrameData.pos_head_new;
			Vector2 tPivotHead = pSourceHead.pivot;
			int tPartHeadX = (int)pos_head_new.x - (int)tPivotHead.x;
			int tPartHeadY = (int)pos_head_new.y - (int)tPivotHead.y;
			tPartX += tPartHeadX;
			tPartY += tPartHeadY;
			drawPixelsAll(pixelBag2, tAtlas, pKingdomColor, tPartX, tPartY, tDynamicZombie, pAsset, pHead: true);
		}
		setAtlasDirty(tAtlas);
		return createFinalSprite(tAtlas, pSourceBody, tWidth, tHeight, tResizeX);
	}

	private static void fillDebugColor(int pWidth, int pHeight, UnitSpriteConstructorAtlas pAtlas)
	{
	}

	private static void drawPixelsAll(PixelBag pBag, UnitSpriteConstructorAtlas pAtlas, ColorAsset pKingdomColor, int pPartX, int pPartY, bool pDynamicZombie, ActorAsset pActorAsset, bool pHead = false)
	{
		Color32[] pixels = pAtlas.pixels;
		int tAtlasWidth = pAtlas.texture.width;
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k1_0, pKingdomColor.k_color_0, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k1_1, pKingdomColor.k_color_1, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k1_2, pKingdomColor.k_color_2, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k1_3, pKingdomColor.k_color_3, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k1_4, pKingdomColor.k_color_4, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k2_0, pKingdomColor.k2_color_0, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k2_1, pKingdomColor.k2_color_1, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k2_2, pKingdomColor.k2_color_2, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k2_3, pKingdomColor.k2_color_3, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_k2_4, pKingdomColor.k2_color_4, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_light, Toolbox.color_light_replace, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_normal, Toolbox.color_magenta_1, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: true, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_phenotype_shade_0, DynamicColorPixelTool.phenotype_shade_0, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_phenotype_shade_1, DynamicColorPixelTool.phenotype_shade_1, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_phenotype_shade_2, DynamicColorPixelTool.phenotype_shade_2, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
		drawPixels(pixels, tAtlasWidth, pBag.arr_pixels_phenotype_shade_3, DynamicColorPixelTool.phenotype_shade_3, pPartX, pPartY, pDynamicZombie, pActorAsset, pUseNormal: false, pHead);
	}

	private static void drawPixels(Color32[] pPixels, int pAtlasWidth, Pixel[] pListSourcePixels, Color32 pNewColor, int pPartX, int pPartY, bool pDrawDynamicZombie, ActorAsset pActorAsset, bool pUseNormal = false, bool pHead = false)
	{
		if (pListSourcePixels == null)
		{
			return;
		}
		for (int tIndex = 0; tIndex < pListSourcePixels.Length; tIndex++)
		{
			Pixel tSourcePixel = pListSourcePixels[tIndex];
			Color32 tColorToSet = pNewColor;
			int tX = tSourcePixel.x + pPartX;
			int tY = tSourcePixel.y + pPartY;
			if (tX < 0)
			{
				tX = 0;
			}
			if (tY < 0)
			{
				tY = 0;
			}
			int tPixelID = tX + tY * pAtlasWidth;
			if (pUseNormal)
			{
				tColorToSet = tSourcePixel.color;
			}
			if (pDrawDynamicZombie)
			{
				tColorToSet = DynamicColorPixelTool.checkZombieColors(pActorAsset, tColorToSet, tPixelID / 3 + tX, pHead);
			}
			pPixels[tPixelID] = tColorToSet;
		}
	}

	public static Sprite getSpriteUnit(AnimationFrameData pFrameData, Sprite pMainSprite, Actor pActor, ColorAsset pKingdomColor, int pPhenotypeIndex, int pPhenotypeShade, UnitTextureAtlasID pTextureAtlasID)
	{
		long t_kingdomID = 0L;
		long t_phenotypeShadeID = 0L;
		long t_phenotypeIndex = pPhenotypeIndex;
		long t_headID = 0L;
		long t_bodyID = getBodySpriteSmallID(pMainSprite);
		if (pActor.has_rendered_sprite_head)
		{
			ActorAnimationLoader.int_ids_heads.TryGetValue(pActor.cached_sprite_head, out var tHeadId);
			if (tHeadId == 0)
			{
				int tNewID = ActorAnimationLoader.int_ids_heads.Count + 1;
				ActorAnimationLoader.int_ids_heads.Add(pActor.cached_sprite_head, tNewID);
				tHeadId = tNewID;
			}
			t_headID = tHeadId;
		}
		if (t_phenotypeIndex != 0L)
		{
			t_phenotypeShadeID = pPhenotypeShade + 1;
		}
		if (pKingdomColor != null)
		{
			t_kingdomID = pKingdomColor.index_id + 1;
		}
		long tId = t_kingdomID * 1000000000000L + t_headID * 1000000000 + t_bodyID * 1000000 + t_phenotypeIndex * 1000 + t_phenotypeShadeID;
		if (debug_actor == pActor)
		{
			AssetManager.dynamic_sprites_library.setDebugActor(tId, t_kingdomID, t_headID, t_bodyID, t_phenotypeIndex, t_phenotypeShadeID);
		}
		DynamicSpritesAsset tAsset = DynamicSpritesLibrary.units;
		Sprite tResult = tAsset.getSprite(tId);
		if ((object)tResult == null)
		{
			tResult = createNewSpriteUnit(pFrameData, pMainSprite, pActor.cached_sprite_head, pKingdomColor, pActor.asset, pPhenotypeIndex, pPhenotypeShade, pTextureAtlasID);
			tAsset.addSprite(tId, tResult);
		}
		return tResult;
	}

	public static void setAtlasDirty(UnitSpriteConstructorAtlas pAtlas)
	{
		AssetManager.dynamic_sprites_library.setDirty();
		pAtlas.dirty = true;
		if (!pAtlas.isBigSpriteSheetAtlas())
		{
			pAtlas.checkDirty();
		}
	}

	public static int getBodySpriteSmallID(Sprite pSprite)
	{
		if (!_int_ids_body.TryGetValue(pSprite, out var tResult))
		{
			tResult = _int_ids_body.Count + 1;
			_int_ids_body.Add(pSprite, tResult);
		}
		return tResult;
	}
}
