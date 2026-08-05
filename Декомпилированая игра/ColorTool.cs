using System.IO;
using UnityEngine;

public class ColorTool : MonoBehaviour
{
	public string colorString;

	public GameObject prefabKingdom;

	public GameObject prefabClan;

	public GameObject prefabCulture;

	public GameObject prefabAlliance;

	public Transform container;

	public string last_editor = "";

	private void resetCoords()
	{
	}

	public void InitKingdoms()
	{
		cleanup();
		last_editor = "kingdoms";
		KingdomColorsLibrary kingdomColorsLibrary = new KingdomColorsLibrary();
		kingdomColorsLibrary.init();
		kingdomColorsLibrary.post_init();
		foreach (ColorAsset tColor in kingdomColorsLibrary.list)
		{
			createColorToolElement(tColor, prefabKingdom, last_editor);
		}
	}

	public void InitCultures()
	{
		cleanup();
		last_editor = "cultures";
		CultureColorsLibrary cultureColorsLibrary = new CultureColorsLibrary();
		cultureColorsLibrary.init();
		cultureColorsLibrary.post_init();
		foreach (ColorAsset tColor in cultureColorsLibrary.list)
		{
			createColorToolElement(tColor, prefabCulture, last_editor);
		}
	}

	public void InitClans()
	{
		cleanup();
		last_editor = "clans";
		ClanColorsLibrary clanColorsLibrary = new ClanColorsLibrary();
		clanColorsLibrary.init();
		clanColorsLibrary.post_init();
		foreach (ColorAsset tColor in clanColorsLibrary.list)
		{
			createColorToolElement(tColor, prefabClan, last_editor);
		}
	}

	public void cleanup()
	{
		resetCoords();
		while (container.childCount > 0)
		{
			Object.DestroyImmediate(container.GetChild(0).gameObject);
		}
	}

	private void createColorToolElement(ColorAsset pColor, GameObject pPrefab, string pWhat)
	{
		ColorToolElement tColorTool = Object.Instantiate(pPrefab, container).GetComponent<ColorToolElement>();
		if (last_editor == "kingdoms")
		{
			tColorTool.createKingdom(pColor);
		}
		else if (last_editor == "clans")
		{
			tColorTool.createClans(pColor);
		}
		else if (last_editor == "cultures")
		{
			tColorTool.createCulture(pColor);
		}
		tColorTool.transform.name = pColor.index_id + "-" + pColor.id;
		tColorTool.transform.SetSiblingIndex(pColor.index_id);
	}

	public void saveEditor()
	{
		if (last_editor == "kingdoms")
		{
			saveKingdoms();
		}
		else if (last_editor == "clans")
		{
			saveClans();
		}
		else if (last_editor == "cultures")
		{
			saveCultures();
		}
	}

	private void convertToolIntoAsset(ColorToolElement pTool, ColorAsset pAsset)
	{
		pAsset.color_main = Toolbox.colorToHex(pTool.colorMain, pAlpha: false);
		pAsset.color_main_2 = Toolbox.colorToHex(pTool.colorMain2, pAlpha: false);
		pAsset.color_banner = Toolbox.colorToHex(pTool.colorBanner, pAlpha: false);
		pAsset.color_text = Toolbox.colorToHex(pTool.colorText, pAlpha: false);
		pAsset.id = pTool.id;
		pAsset.favorite = pTool.favorite;
	}

	private void saveKingdoms()
	{
		KingdomColorsLibrary tData = new KingdomColorsLibrary();
		saveLib(tData);
	}

	private void saveCultures()
	{
		CultureColorsLibrary tData = new CultureColorsLibrary();
		saveLib(tData);
	}

	private void saveClans()
	{
		ClanColorsLibrary tData = new ClanColorsLibrary();
		saveLib(tData);
	}

	private void saveLib(ColorLibrary pLibrary)
	{
		for (int i = 0; i < container.childCount; i++)
		{
			ColorToolElement tColorTool = container.GetChild(i).GetComponent<ColorToolElement>();
			ColorAsset tColorAsset = new ColorAsset();
			convertToolIntoAsset(tColorTool, tColorAsset);
			tColorAsset.index_id = i;
			pLibrary.list.Add(tColorAsset);
		}
		string tJson = JsonUtility.ToJson(pLibrary, prettyPrint: true);
		File.WriteAllText(pLibrary.getEditorPathForSave(), tJson);
	}
}
