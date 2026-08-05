using System.Collections.Generic;
using UnityEngine.UI;

public static class BaseStatsHelper
{
	public delegate KeyValueField KeyValueFieldGetter(string pID);

	public static BaseStats _base_stats_tooltip_helper = new BaseStats();

	private static List<BaseStatsContainer> _stats_container_positive = new List<BaseStatsContainer>();

	private static List<BaseStatsContainer> _stats_container_negative = new List<BaseStatsContainer>();

	public static BaseStats getTotalStatsFrom(BaseStats pBaseStats, BaseStats pBaseStatsMeta)
	{
		_base_stats_tooltip_helper.clear();
		_base_stats_tooltip_helper.mergeStats(pBaseStats);
		_base_stats_tooltip_helper.mergeStats(pBaseStatsMeta);
		return _base_stats_tooltip_helper;
	}

	public static void showItemMods(Text pTextFieldDescription, Text pTextFieldValues, Item pItem)
	{
		using ListPool<TooltipModContainerInfo> tListMods = getItemModsBase(pItem);
		foreach (ref TooltipModContainerInfo item in tListMods)
		{
			TooltipModContainerInfo tContainer = item;
			addStatValues(pTextFieldDescription, pTextFieldValues, Toolbox.coloredText("+" + LocalizedTextManager.getText(tContainer.asset.getLocaleID()), "#45FFFE"), Toolbox.coloredText(tContainer.string_pluses, "#45FFFE"));
			addLineBreak(pTextFieldDescription, pTextFieldValues);
		}
	}

	public static void showItemModsRows(KeyValueFieldGetter pFieldsFabric, Item pItem)
	{
		using ListPool<TooltipModContainerInfo> tListMods = getItemModsBase(pItem);
		foreach (ref TooltipModContainerInfo item in tListMods)
		{
			TooltipModContainerInfo tContainer = item;
			string tStats = Toolbox.coloredText("+" + LocalizedTextManager.getText(tContainer.asset.getLocaleID()), "#45FFFE");
			string tValues = Toolbox.coloredText(tContainer.string_pluses, "#45FFFE");
			KeyValueField keyValueField = pFieldsFabric(tContainer.asset.getLocaleID());
			keyValueField.name_text.text = tStats;
			keyValueField.value.text = tValues;
		}
	}

	private static ListPool<TooltipModContainerInfo> getItemModsBase(Item pItem)
	{
		ListPool<TooltipModContainerInfo> tListMods = new ListPool<TooltipModContainerInfo>(pItem.data.modifiers.Count);
		foreach (ref string modifier in pItem.data.modifiers)
		{
			string tModID = modifier;
			ItemModAsset tModAsset = AssetManager.items_modifiers.get(tModID);
			string tPluses = "";
			for (int i = 0; i < tModAsset.mod_rank; i++)
			{
				tPluses += "+";
			}
			tListMods.Add(new TooltipModContainerInfo(tModAsset, tModAsset.mod_rank, tPluses));
		}
		tListMods.Sort(sortByPluses);
		return tListMods;
	}

	private static void addStatValues(Text pStatsField, Text pValuesField, string pStats, string pValues)
	{
		pStatsField.text += pStats;
		pValuesField.text += pValues;
	}

	private static void addLineBreak(Text pStatsField, Text pValuesField)
	{
		pStatsField.text += "\n";
		pValuesField.text += "\n";
	}

	private static int sortByPluses(TooltipModContainerInfo pContainer1, TooltipModContainerInfo pContainer2)
	{
		return pContainer2.pluses.CompareTo(pContainer1.pluses);
	}

	public static void showBaseStats(Text pStatsField, Text pValuesField, BaseStats pBaseStats, bool pAddPlus = true)
	{
		calcBaseStatsBase(pBaseStats);
		foreach (BaseStatsContainer tBaseStats in _stats_container_positive)
		{
			showBaseStatLine(pStatsField, pValuesField, tBaseStats, pAddColor: true, pAddPlus);
		}
		foreach (BaseStatsContainer tBaseStats2 in _stats_container_negative)
		{
			showBaseStatLine(pStatsField, pValuesField, tBaseStats2, pAddColor: true, pAddPlus);
		}
	}

	public static void showBaseStatsRows(KeyValueFieldGetter pFieldsFabric, BaseStats pBaseStats, bool pAddPlus = true)
	{
		calcBaseStatsBase(pBaseStats);
		foreach (BaseStatsContainer tBaseStats in _stats_container_positive)
		{
			showBaseStatRow(pFieldsFabric, tBaseStats, pAddColor: true, pAddPlus);
		}
		foreach (BaseStatsContainer tBaseStats2 in _stats_container_negative)
		{
			showBaseStatRow(pFieldsFabric, tBaseStats2, pAddColor: true, pAddPlus);
		}
	}

	private static void calcBaseStatsBase(BaseStats pBaseStats)
	{
		_stats_container_positive.Clear();
		_stats_container_negative.Clear();
		foreach (BaseStatsContainer tContainer in pBaseStats.getList())
		{
			if (!tContainer.asset.hidden || DebugConfig.isOn(DebugOption.ShowHiddenStats))
			{
				queueStatContainer(tContainer);
			}
		}
		_stats_container_positive.Sort(sortByRank);
	}

	private static int sortByRank(BaseStatsContainer pContainerA, BaseStatsContainer pContainerB)
	{
		BaseStatAsset tAssetA = pContainerA.asset;
		return pContainerB.asset.sort_rank.CompareTo(tAssetA.sort_rank);
	}

	private static void queueStatContainer(BaseStatsContainer pContainer)
	{
		if (pContainer.value > 0f)
		{
			_stats_container_positive.Add(pContainer);
		}
		if (pContainer.value < 0f)
		{
			_stats_container_negative.Add(pContainer);
		}
	}

	private static void showBaseStatLine(Text pStatsField, Text pValuesField, BaseStatsContainer pContainer, bool pAddColor = true, bool pAddPlus = true, string pMainColor = "#43FF43", bool pForceZero = false)
	{
		calcBaseStatLineBase(pContainer, out var tId, out var tValue, out var tAsset);
		if (!tAsset.hidden)
		{
			addItemText(pStatsField, pValuesField, tId, tValue, tAsset.show_as_percents, pAddColor, pAddPlus, pMainColor, pForceZero);
			return;
		}
		if (pStatsField.text.Length > 0)
		{
			addLineBreak(pStatsField, pValuesField);
		}
		string tFinalTextLeft = tId;
		string tFinalTextRight = tValue.ToText();
		if (tAsset.show_as_percents)
		{
			tFinalTextRight += " %";
		}
		pValuesField.text += Toolbox.coloredText(tFinalTextRight, ColorStyleLibrary.m.color_text_grey);
		pStatsField.text += Toolbox.coloredText(tFinalTextLeft, ColorStyleLibrary.m.color_text_grey);
	}

	private static void showBaseStatRow(KeyValueFieldGetter pFieldsFabric, BaseStatsContainer pContainer, bool pAddColor = true, bool pAddPlus = true, string pMainColor = "#43FF43", bool pForceZero = false)
	{
		calcBaseStatLineBase(pContainer, out var tId, out var tValue, out var tAsset);
		addItemTextRow(pFieldsFabric(tId), tId, tValue, tAsset.show_as_percents, pAddColor, pAddPlus, pMainColor, pForceZero);
	}

	private static void calcBaseStatLineBase(BaseStatsContainer pContainer, out string tId, out float tValue, out BaseStatAsset tAsset)
	{
		tAsset = pContainer.asset;
		tId = tAsset.getLocaleID();
		tValue = pContainer.value;
		if (tAsset.tooltip_multiply_for_visual_number != 1f)
		{
			tValue *= tAsset.tooltip_multiply_for_visual_number;
		}
		if (tAsset.hidden && DebugConfig.isOn(DebugOption.ShowHiddenStats))
		{
			tId = "[HIDDEN] " + tId;
		}
	}

	private static void addItemText(Text pStatsField, Text pValuesField, string pID, float pValue, bool pPercent = false, bool pAddColor = true, bool pAddPlus = true, string pMainColor = "#43FF43", bool pForceZero = false)
	{
		addItemTextBase(pValue, out var tValString, pPercent, pForceZero);
		if (!pAddColor)
		{
			addLineText(pStatsField, pValuesField, pID, tValString, null, pPercent);
		}
		else if (pValue > 0f)
		{
			if (pAddPlus)
			{
				tValString = "+" + tValString;
			}
			addLineText(pStatsField, pValuesField, pID, tValString, pMainColor, pPercent);
		}
		else
		{
			addLineText(pStatsField, pValuesField, pID, tValString, "#FB2C21", pPercent);
		}
	}

	private static void addItemTextRow(KeyValueField pField, string pID, float pValue, bool pPercent = false, bool pAddColor = true, bool pAddPlus = true, string pMainColor = "#43FF43", bool pForceZero = false)
	{
		addItemTextBase(pValue, out var tValString, pPercent, pForceZero);
		if (!pAddColor)
		{
			addRowText(pField, pID, tValString, null, pPercent);
		}
		else if (pValue > 0f)
		{
			if (pAddPlus)
			{
				tValString = "+" + tValString;
			}
			addRowText(pField, pID, tValString, pMainColor, pPercent);
		}
		else
		{
			addRowText(pField, pID, tValString, "#FB2C21", pPercent);
		}
	}

	private static void addItemTextBase(float pValue, out string pValString, bool pPercent = false, bool pForceZero = false)
	{
		pValString = pValue.ToText();
		if ((pValue != 0f || pForceZero) && pPercent)
		{
			pValString += "%";
		}
	}

	private static void addLineIntText(Text pStatsField, Text pValuesField, string pID, int pValue, string pColor = null)
	{
		addLineText(pStatsField, pValuesField, pID, pValue.ToText(), pColor);
	}

	private static void addLineText(Text pStatsField, Text pValuesField, string pID, string pValue, string pColor = null, bool pPercent = false)
	{
		if (pStatsField.text.Length > 0)
		{
			addLineBreak(pStatsField, pValuesField);
		}
		if (pValue.Length > 21)
		{
			pValue = pValue.Substring(0, 20) + "...";
		}
		string tFinalText = LocalizedTextManager.getText(pID);
		if (pPercent)
		{
			tFinalText += " %";
		}
		if (!string.IsNullOrEmpty(pColor))
		{
			pStatsField.text += tFinalText;
			pValuesField.text += Toolbox.coloredText(pValue, pColor);
		}
		else
		{
			pStatsField.text += tFinalText;
			pValuesField.text += pValue;
		}
	}

	private static void addRowText(KeyValueField pField, string pID, string pValue, string pColor = null, bool pPercent = false)
	{
		if (pValue.Length > 21)
		{
			pValue = pValue.Substring(0, 20) + "...";
		}
		string tLocalizedText;
		if (pID.Contains("[HIDDEN]"))
		{
			tLocalizedText = pID;
			pColor = ColorStyleLibrary.m.color_text_grey;
		}
		else
		{
			tLocalizedText = LocalizedTextManager.getText(pID);
		}
		if (pPercent)
		{
			tLocalizedText += " %";
		}
		if (!string.IsNullOrEmpty(pColor))
		{
			pField.name_text.text = Toolbox.coloredText(tLocalizedText, pColor);
			pField.value.text = Toolbox.coloredText(pValue, pColor);
		}
		else
		{
			pField.name_text.text = tLocalizedText;
			pField.value.text = pValue;
		}
	}
}
