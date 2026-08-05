using System.Collections.Generic;
using UnityEngine;

public class KingdomBannerLibrary : GenericBannerLibrary
{
	public const string PATH_BANNER_KINGDOMS = "banners_kingdoms/";

	public const string PATH_BACKGROUND = "/background";

	public const string PATH_ICON = "/icon";

	public override void init()
	{
		base.init();
	}

	public override BannerAsset get(string pID)
	{
		if (dict.ContainsKey(pID))
		{
			return base.get(pID);
		}
		loadNewAssetRuntime(pID);
		return base.get(pID);
	}

	public static string getFullPathBackground(string pID)
	{
		return "banners_kingdoms/" + pID + "/background";
	}

	public static string getFullPathIcon(string pID)
	{
		return "banners_kingdoms/" + pID + "/icon";
	}

	private BannerAsset loadNewAssetRuntime(string pID)
	{
		string tPathBackgrounds = getFullPathBackground(pID);
		string tPathIcons = getFullPathIcon(pID);
		Sprite[] spriteList = SpriteTextureLoader.getSpriteList(tPathBackgrounds);
		Sprite[] tSpriteIcons = SpriteTextureLoader.getSpriteList(tPathIcons);
		List<string> tBackgrounds = new List<string>();
		List<string> tIcons = new List<string>();
		Sprite[] array = spriteList;
		foreach (Sprite tSprite in array)
		{
			string tPath = tPathBackgrounds + "/" + tSprite.name;
			tBackgrounds.Add(tPath);
		}
		array = tSpriteIcons;
		foreach (Sprite tSprite2 in array)
		{
			string tPath2 = tPathIcons + "/" + tSprite2.name;
			tIcons.Add(tPath2);
		}
		BannerAsset tAsset = new BannerAsset
		{
			id = pID,
			backgrounds = tBackgrounds,
			icons = tIcons
		};
		add(tAsset);
		return tAsset;
	}
}
