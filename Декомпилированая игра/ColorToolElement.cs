using UnityEngine;
using UnityEngine.UI;

public class ColorToolElement : MonoBehaviour
{
	[Header("Edit Colors")]
	public Color colorMain;

	public Color colorMain2;

	public Color colorBanner;

	public Color colorText;

	[Header("Edit Asset Name / Id")]
	public string id;

	public bool favorite;

	[Header("Other Stuff")]
	[Space(30f)]
	public Image background;

	public Image icon;

	public Text text;

	public Image sprite_favorite;

	public Image borderInside;

	public Image borderOutside;

	[HideInInspector]
	public ColorAsset color_asset;

	public Image test_house;

	public Image test_face;

	public Sprite house_default_sprite;

	public Sprite face_default_sprite;

	public int debug_index;

	public void createKingdom(ColorAsset pColor)
	{
		color_asset = pColor;
	}

	public void createCulture(ColorAsset pColor)
	{
		color_asset = pColor;
		setColorsForObjects(pColor);
		saveColors(pColor);
	}

	public void createClans(ColorAsset pColor)
	{
		color_asset = pColor;
		string tPathBackground = AssetManager.clan_banners_library.main.backgrounds.GetRandom();
		string tPathIcon = AssetManager.clan_banners_library.main.icons.GetRandom();
		background.sprite = SpriteTextureLoader.getSprite(tPathBackground);
		icon.sprite = SpriteTextureLoader.getSprite(tPathIcon);
		setColorsForObjects(pColor);
		saveColors(pColor);
	}

	private void setColorsForObjects(ColorAsset pColorAsset)
	{
		borderInside.color = pColorAsset.getColorBorderInsideAlpha32();
		borderOutside.color = pColorAsset.getColorMainSecond();
		background.color = pColorAsset.getColorMainSecond();
		icon.color = pColorAsset.getColorBanner();
		text.color = pColorAsset.getColorText();
		favorite = pColorAsset.favorite;
		id = pColorAsset.id;
		text.text = pColorAsset.id + " |  " + pColorAsset.index_id;
		debug_index = pColorAsset.index_id;
		if (test_house != null && house_default_sprite != null)
		{
			test_house.sprite = DynamicSpriteCreator.createNewSpriteForDebug(house_default_sprite, pColorAsset);
		}
		if (test_face != null && face_default_sprite != null)
		{
			test_face.sprite = DynamicSpriteCreator.createNewSpriteForDebug(face_default_sprite, pColorAsset);
		}
		if (sprite_favorite != null)
		{
			sprite_favorite.gameObject.SetActive(favorite);
		}
	}

	private void OnValidate()
	{
		if (color_asset != null)
		{
			color_asset.color_main = Toolbox.colorToHex(colorMain, pAlpha: false);
			color_asset.color_main_2 = Toolbox.colorToHex(colorMain2, pAlpha: false);
			color_asset.color_banner = Toolbox.colorToHex(colorBanner, pAlpha: false);
			color_asset.color_text = Toolbox.colorToHex(colorText, pAlpha: false);
			color_asset.id = id;
			color_asset.favorite = favorite;
			color_asset.setEditorColors(colorMain, colorMain2, colorBanner, colorText);
			setColorsForObjects(color_asset);
		}
	}

	private void saveColors(ColorAsset pColor)
	{
		colorMain = pColor.getColorMain();
		colorMain2 = pColor.getColorMainSecond();
		colorBanner = pColor.getColorBanner();
		colorText = pColor.getColorText();
	}
}
