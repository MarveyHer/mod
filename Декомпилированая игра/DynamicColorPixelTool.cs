using System;
using UnityEngine;

public static class DynamicColorPixelTool
{
	private static bool _draw_phenotype;

	private static Color32 _phenotype_color;

	public static Color32 phenotype_shade_0;

	public static Color32 phenotype_shade_1;

	public static Color32 phenotype_shade_2;

	public static Color32 phenotype_shade_3;

	private static readonly Color32 _zombie_blood_color = Toolbox.makeColor("#CE566E");

	public static Color32 checkSpecialColors(Color32 pColor, ColorAsset pKingdomColor, bool pCheckForLightColors = false)
	{
		if (Config.EVERYTHING_MAGIC_COLOR)
		{
			return Toolbox.EVERYTHING_MAGIC_COLOR32;
		}
		if (pCheckForLightColors && Toolbox.areColorsEqual(pColor, Toolbox.color_light))
		{
			pColor = Toolbox.color_light_replace;
			return pColor;
		}
		if (pKingdomColor != null)
		{
			if (Toolbox.areColorsEqual(pColor, Toolbox.color_magenta_0))
			{
				pColor = pKingdomColor.k_color_0;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_magenta_1))
			{
				pColor = pKingdomColor.k_color_1;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_magenta_2))
			{
				pColor = pKingdomColor.k_color_2;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_magenta_3))
			{
				pColor = pKingdomColor.k_color_3;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_magenta_4))
			{
				pColor = pKingdomColor.k_color_4;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_teal_0))
			{
				pColor = pKingdomColor.k2_color_0;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_teal_1))
			{
				pColor = pKingdomColor.k2_color_1;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_teal_2))
			{
				pColor = pKingdomColor.k2_color_2;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_teal_3))
			{
				pColor = pKingdomColor.k2_color_3;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_teal_4))
			{
				pColor = pKingdomColor.k2_color_4;
			}
		}
		if (_draw_phenotype)
		{
			if (Toolbox.areColorsEqual(pColor, Toolbox.color_phenotype_green_0))
			{
				pColor = phenotype_shade_0;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_phenotype_green_1))
			{
				pColor = phenotype_shade_1;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_phenotype_green_2))
			{
				pColor = phenotype_shade_2;
			}
			else if (Toolbox.areColorsEqual(pColor, Toolbox.color_phenotype_green_3))
			{
				pColor = phenotype_shade_3;
			}
		}
		return pColor;
	}

	public static Color32 checkZombieColors(ActorAsset pAsset, Color32 pColor, int pID, bool pHead = false)
	{
		Color32 tZombieColor = Toolbox.makeColor(pAsset.zombie_color_hex);
		return addNoiseAndBlood(multiplyBlend(pColor, tZombieColor), pID);
	}

	private static Color32 addNoiseAndBlood(Color32 pTargetColor, int pID)
	{
		System.Random tPixelRandomizer = new System.Random(pID);
		if (tPixelRandomizer.NextDouble() < 0.5)
		{
			return multiplyBlend(pTargetColor, _zombie_blood_color, 0.2f);
		}
		int tNoiseAmount = tPixelRandomizer.Next(0, 20);
		int num = Mathf.Clamp(pTargetColor.r + tNoiseAmount, 0, 255);
		int tNewG = Mathf.Clamp(pTargetColor.g + tNoiseAmount, 0, 255);
		int tNewB = Mathf.Clamp(pTargetColor.b + tNoiseAmount, 0, 255);
		return new Color32((byte)num, (byte)tNewG, (byte)tNewB, pTargetColor.a);
	}

	private static Color32 multiplyBlend(Color32 pBaseColor, Color32 pTargetBlendColor, float pIntensity = 1f)
	{
		float num = (float)(int)pBaseColor.r / 255f;
		float tG = (float)(int)pBaseColor.g / 255f;
		float tB = (float)(int)pBaseColor.b / 255f;
		float tBlendR = Mathf.Lerp(1f, (float)(int)pTargetBlendColor.r / 255f, pIntensity);
		float tBlendG = Mathf.Lerp(1f, (float)(int)pTargetBlendColor.g / 255f, pIntensity);
		float tBlendB = Mathf.Lerp(1f, (float)(int)pTargetBlendColor.b / 255f, pIntensity);
		float num2 = Mathf.Clamp01(num * tBlendR);
		float tNewG = Mathf.Clamp01(tG * tBlendG);
		float tNewB = Mathf.Clamp01(tB * tBlendB);
		return new Color32((byte)(num2 * 255f), (byte)(tNewG * 255f), (byte)(tNewB * 255f), pBaseColor.a);
	}

	private static Color32 overlayBlend(Color32 pBaseColor, Color32 pTargetBlendColor)
	{
		float tR = (float)(int)pBaseColor.r / 255f;
		float tG = (float)(int)pBaseColor.g / 255f;
		float tB = (float)(int)pBaseColor.b / 255f;
		float tBlendR = (float)(int)pTargetBlendColor.r / 255f;
		float tBlendG = (float)(int)pTargetBlendColor.g / 255f;
		float tBlendB = (float)(int)pTargetBlendColor.b / 255f;
		float num = ((tR < 0.5f) ? (2f * tR * tBlendR) : (1f - 2f * (1f - tR) * (1f - tBlendR)));
		float tNewG = ((tG < 0.5f) ? (2f * tG * tBlendG) : (1f - 2f * (1f - tG) * (1f - tBlendG)));
		float tNewB = ((tB < 0.5f) ? (2f * tB * tBlendB) : (1f - 2f * (1f - tB) * (1f - tBlendB)));
		return new Color32((byte)(num * 255f), (byte)(tNewG * 255f), (byte)(tNewB * 255f), pBaseColor.a);
	}

	public static void loadPhenotype(int pPhenotypeIndex, int pPhenotypeShadeIndex)
	{
		loadPhenotype(AssetManager.phenotype_library.getAssetByPhenotypeIndex(pPhenotypeIndex), pPhenotypeShadeIndex);
	}

	public static void loadPhenotype(PhenotypeAsset pPhenotypeAsset, int pPhenotypeShadeIndex)
	{
		_phenotype_color = pPhenotypeAsset.colors[pPhenotypeShadeIndex];
		_draw_phenotype = true;
		phenotype_shade_0 = Toolbox.makeDarkerColor(_phenotype_color, 1f);
		phenotype_shade_1 = Toolbox.makeDarkerColor(_phenotype_color, 0.9f);
		phenotype_shade_2 = Toolbox.makeDarkerColor(_phenotype_color, 0.8f);
		phenotype_shade_3 = Toolbox.makeDarkerColor(_phenotype_color, 0.7f);
	}

	public static void loadSkinColorsPreview(PhenotypeAsset pPhenotype, int pSkinColor)
	{
		_draw_phenotype = true;
		phenotype_shade_0 = pPhenotype.colors[0];
		phenotype_shade_1 = pPhenotype.colors[1];
		phenotype_shade_2 = pPhenotype.colors[2];
		phenotype_shade_3 = pPhenotype.colors[3];
	}

	public static void resetSkinColors()
	{
		_draw_phenotype = false;
	}

	public static void setPlaceholderSkinColor(Color32 pColor)
	{
		_phenotype_color = pColor;
	}
}
