using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDebugAssetElement : BaseDebugAssetElement<BuildingAsset>
{
	public BuildingDebugAnimationElement spawn;

	public BuildingDebugAnimationElement main;

	public BuildingDebugAnimationElement disabled;

	public BuildingDebugAnimationElement ruin;

	public BuildingDebugAnimationElement special;

	public Image construction;

	public Image mini;

	public override void setData(BuildingAsset pAsset)
	{
		asset = pAsset;
		title.text = asset.id;
		initAnimations();
		initStats();
	}

	protected override void initAnimations()
	{
		BuildingSprites tSprites = asset.building_sprites;
		spawn.setData(asset);
		main.setData(asset);
		disabled.setData(asset);
		ruin.setData(asset);
		special.setData(asset);
		List<DebugAnimatedVariation> tSpawnVariations = new List<DebugAnimatedVariation>();
		List<DebugAnimatedVariation> tMainVariations = new List<DebugAnimatedVariation>();
		List<DebugAnimatedVariation> tDisabledVariations = new List<DebugAnimatedVariation>();
		List<DebugAnimatedVariation> tRuinVariations = new List<DebugAnimatedVariation>();
		List<DebugAnimatedVariation> tSpecialVariations = new List<DebugAnimatedVariation>();
		foreach (BuildingAnimationData tAnimation in asset.building_sprites.animation_data)
		{
			tSpawnVariations.Add(new DebugAnimatedVariation(getBuildingColoredSprites(tAnimation.spawn), tAnimation.animated));
			tMainVariations.Add(new DebugAnimatedVariation(getBuildingColoredSprites(tAnimation.main), tAnimation.animated));
			tDisabledVariations.Add(new DebugAnimatedVariation(getBuildingColoredSprites(tAnimation.main_disabled), tAnimation.animated));
			tRuinVariations.Add(new DebugAnimatedVariation(getBuildingColoredSprites(tAnimation.ruins), tAnimation.animated));
			tSpecialVariations.Add(new DebugAnimatedVariation(getBuildingColoredSprites(tAnimation.special), tAnimation.animated));
		}
		spawn.setFrames(tSpawnVariations, asset.has_sprites_spawn);
		main.setFrames(tMainVariations, asset.has_sprites_main);
		disabled.setFrames(tDisabledVariations, asset.has_sprites_main_disabled);
		ruin.setFrames(tRuinVariations, asset.has_sprites_ruin);
		special.setFrames(tSpecialVariations, asset.has_sprites_special);
		if (tSprites.construction != null)
		{
			construction.sprite = tSprites.construction;
		}
		else if (asset.has_sprite_construction)
		{
			construction.sprite = no_animation;
		}
		else
		{
			construction.color = Color.clear;
		}
		mini.sprite = loadMini();
	}

	private Sprite loadMini()
	{
		string tPath = asset.sprite_path;
		if (string.IsNullOrEmpty(tPath))
		{
			tPath = asset.main_path + asset.id;
		}
		tPath += "/mini_0";
		Sprite tSprite = SpriteTextureLoader.getSprite(tPath);
		if (tSprite == null)
		{
			Debug.LogError("Not found mini sprite for building: " + asset.id);
			return tSprite;
		}
		KingdomAsset tKingdomAsset = AssetManager.kingdoms.get("mad");
		if (!asset.has_kingdom_color)
		{
			return tSprite;
		}
		ColorAsset tKingdomColor = tKingdomAsset.debug_color_asset;
		Texture2D tTexture = new Texture2D(tSprite.texture.width, tSprite.texture.height);
		tTexture.filterMode = tSprite.texture.filterMode;
		for (int x = 0; x < tTexture.width; x++)
		{
			for (int y = 0; y < tTexture.height; y++)
			{
				Color tOrigColor = tSprite.texture.GetPixel(x, y);
				Color tColor = getColor(tOrigColor, tKingdomColor);
				tTexture.SetPixel(x, y, tColor);
			}
		}
		tTexture.Apply();
		return Sprite.Create(tTexture, new Rect(Vector2.zero, new Vector2(tTexture.width, tTexture.height)), new Vector2(0.5f, 0.5f), 1f);
	}

	private Color32 getColor(Color pOrigColor, ColorAsset pKingdomColor)
	{
		if (Toolbox.areColorsEqual(pOrigColor, Toolbox.color_magenta_0))
		{
			pOrigColor = pKingdomColor.k_color_0;
		}
		else if (Toolbox.areColorsEqual(pOrigColor, Toolbox.color_magenta_1))
		{
			pOrigColor = pKingdomColor.k_color_1;
		}
		else if (Toolbox.areColorsEqual(pOrigColor, Toolbox.color_magenta_2))
		{
			pOrigColor = pKingdomColor.k_color_2;
		}
		else if (Toolbox.areColorsEqual(pOrigColor, Toolbox.color_magenta_3))
		{
			pOrigColor = pKingdomColor.k_color_3;
		}
		else if (Toolbox.areColorsEqual(pOrigColor, Toolbox.color_magenta_4))
		{
			pOrigColor = pKingdomColor.k_color_4;
		}
		return pOrigColor;
	}

	public override void update()
	{
		if (base.gameObject.activeSelf)
		{
			spawn.update();
			main.update();
			disabled.update();
			ruin.update();
			special.update();
		}
	}

	public override void stopAnimations()
	{
		spawn.stopAnimations();
		main.stopAnimations();
		disabled.stopAnimations();
		ruin.stopAnimations();
		special.stopAnimations();
	}

	public override void startAnimations()
	{
		spawn.startAnimations();
		main.startAnimations();
		disabled.startAnimations();
		ruin.startAnimations();
		special.startAnimations();
	}

	private Sprite[] getBuildingColoredSprites(Sprite[] pSprites)
	{
		if (pSprites == null)
		{
			return new Sprite[0];
		}
		Sprite[] tResult = new Sprite[pSprites.Length];
		for (int i = 0; i < pSprites.Length; i++)
		{
			tResult[i] = getBuildingColoredSprite(pSprites[i]);
		}
		return tResult;
	}

	private Sprite getBuildingColoredSprite(Sprite pMainSprite)
	{
		ColorAsset tKingdomColor = null;
		if (asset.has_kingdom_color)
		{
			tKingdomColor = AssetManager.kingdoms.get("mad").debug_color_asset;
		}
		return DynamicSprites.getRecoloredBuilding(pMainSprite, tKingdomColor, asset.atlas_asset);
	}

	protected override void initStats()
	{
		base.initStats();
		showStat("health", asset.base_stats["health"]);
		showStat("damage", asset.base_stats["damage"]);
		showStat("targets", asset.base_stats["targets"]);
		showStat("area_of_effect", asset.base_stats["area_of_effect"]);
	}

	protected override void showAssetWindow()
	{
		base.showAssetWindow();
		ScrollWindow.showWindow("building_asset");
	}
}
