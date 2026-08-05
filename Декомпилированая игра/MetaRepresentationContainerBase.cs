using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityPools;

public class MetaRepresentationContainerBase : StatsRowsContainer
{
	[SerializeField]
	protected MetaType _meta_type;

	[SerializeField]
	private LocalizedText _title;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _prefab_bar;

	[SerializeField]
	private LayoutElement _layout_element;

	protected MetaRepresentationAsset asset;

	protected override void init()
	{
		base.init();
		asset = AssetManager.meta_representation_library.getAsset(_meta_type);
		_prefab_bar.gameObject.SetActive(value: false);
		_title.setKeyAndUpdate(asset.getLocaleID());
	}

	protected override void showStats()
	{
		int tTotal = 0;
		bool tAny = false;
		Dictionary<IMetaObject, int> tDict = UnsafeCollectionPool<Dictionary<IMetaObject, int>, KeyValuePair<IMetaObject, int>>.Get();
		fillDict(ref tTotal, ref tAny, tDict);
		int tNone = tTotal;
		foreach (KeyValuePair<IMetaObject, int> tPair in tDict.OrderByDescending((KeyValuePair<IMetaObject, int> p) => p.Value))
		{
			IMetaObject tMeta = tPair.Key;
			int tAmount = tPair.Value;
			tNone -= tAmount;
			string tPopString = amountWithPercent(tAmount, tTotal);
			string tIconPath = asset.icon_getter(tMeta);
			string tSecondaryIconPath = (asset.show_species_icon ? tMeta.getActorAsset().icon : null);
			string tNameString = tMeta.name;
			tNameString += Toolbox.coloredGreyPart(tAmount, tMeta.getColor().color_text);
			KeyValueField tField = showStatRowTwoIcons(tNameString, tPopString, tMeta.getColor().color_text, asset.meta_type, tMeta.getID(), pColorText: true, tIconPath, tSecondaryIconPath, null, null, pLocalize: false);
			showBar(tField, tAmount, tTotal, tMeta.getColor().color_text);
		}
		checkShowNone(tAny, tNone, tTotal);
		UnsafeCollectionPool<Dictionary<IMetaObject, int>, KeyValuePair<IMetaObject, int>>.Release(tDict);
		_layout_element.ignoreLayout = !tAny;
		_background.enabled = tAny;
		_title.gameObject.SetActive(tAny);
	}

	protected virtual void fillDict(ref int pTotal, ref bool pAny, Dictionary<IMetaObject, int> pDict)
	{
		throw new NotImplementedException();
	}

	protected virtual void checkShowNone(bool pAny, int pNone, int pTotal)
	{
		throw new NotImplementedException();
	}

	protected void showBar(KeyValueField pField, int pAmount, int pTotal, string pColorHex)
	{
		float tFill = ((pTotal > 0) ? ((float)pAmount / (float)pTotal) : 0f);
		Image tBarImage = pField.transform.Find("gen_percent_bar")?.GetComponent<Image>();
		if (tBarImage == null)
		{
			tBarImage = UnityEngine.Object.Instantiate(_prefab_bar.gameObject, pField.transform).GetComponent<Image>();
			tBarImage.gameObject.SetActive(value: true);
			tBarImage.name = "gen_percent_bar";
		}
		float tWidth = 100f * tFill * 0.5f;
		Vector2 tSizeDelta = new Vector2(tWidth, 8.5f);
		tBarImage.GetComponent<RectTransform>().sizeDelta = tSizeDelta;
		tBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2f, 0f);
		tBarImage.transform.SetAsFirstSibling();
		Color tColor = Toolbox.makeColor(pColorHex);
		tColor.a = 0.4f;
		tBarImage.color = tColor;
	}

	protected string amountWithPercent(int pAmount, int pTotal)
	{
		float tPercent = ((pTotal > 0) ? ((float)pAmount / (float)pTotal * 100f) : 0f);
		if (pTotal == pAmount)
		{
			tPercent = 100f;
		}
		return tPercent.ToText() + "%";
	}

	internal KeyValueField showStatRowTwoIcons(string pId, object pValue, string pColor, MetaType pMetaType = MetaType.None, long pMetaId = -1L, bool pColorText = false, string pIconPath = null, string pIconSecondaryPath = null, string pTooltipId = null, TooltipDataGetter pTooltipData = null, bool pLocalize = true)
	{
		KeyValueField tNewRow = showStatRow(pId, pValue, pColor, pMetaType, pMetaId, pColorText, pIconPath, pTooltipId, pTooltipData, pLocalize);
		bool tShowIcon = !string.IsNullOrEmpty(pIconSecondaryPath);
		if (tShowIcon)
		{
			Sprite tIcon = SpriteTextureLoader.getSprite("ui/Icons/" + pIconSecondaryPath);
			tNewRow.icon_secondary.sprite = tIcon;
		}
		tNewRow.icon_secondary.gameObject.SetActive(tShowIcon);
		return tNewRow;
	}

	public void setMetaType(MetaType pType)
	{
		_meta_type = pType;
	}
}
