using System;
using ChartAndGraph;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HistoryDataAsset : Asset, IDescriptionAsset, ILocalizedAsset
{
	public string localized_key;

	public string localized_key_description;

	public string statistics_asset;

	public string color_hex;

	public string tooltip_color_hex;

	public string path_icon;

	public bool enabled_default;

	private Material _material_point;

	private Material _material_line;

	private Material _material_gradient;

	private ChartItemEffect _hover_prefab;

	public bool average;

	public bool max;

	public bool sum;

	public GraphCategoryGroup category_group = GraphCategoryGroup.General;

	public Material getChartPointMaterial()
	{
		if (_material_point == null)
		{
			_material_point = cloneMaterial("materials/graph/graph_base_point");
			_material_point.SetTexture("_MainTex", Resources.Load<Texture2D>(path_icon));
		}
		return _material_point;
	}

	public ChartItemEffect getHoverPointMaterial()
	{
		if (_hover_prefab == null)
		{
			_hover_prefab = clonePrefab("Prefabs/graph/PointHover", GameObject.Find("Charts").transform);
			_hover_prefab.gameObject.name = "Hover " + id;
			_hover_prefab.GetComponent<Image>().sprite = SpriteTextureLoader.getSprite(path_icon);
			_hover_prefab.gameObject.SetActive(value: false);
		}
		return _hover_prefab;
	}

	public Material getChartLineMaterial()
	{
		if (_material_line == null)
		{
			_material_line = getChartLineMaterial(getColorMain());
		}
		return _material_line;
	}

	public static Material getChartLineMaterial(Color pColor)
	{
		Material material = cloneMaterial("materials/graph/graph_base_line");
		material.SetColor("_Color", pColor);
		return material;
	}

	public Material getChartInnerFillMaterial()
	{
		if (_material_gradient == null)
		{
			_material_gradient = getChartInnerFillMaterial(getColorMain());
		}
		return _material_gradient;
	}

	public static Material getChartInnerFillMaterial(Color pColor)
	{
		Material material = cloneMaterial("materials/graph/graph_base_gradient");
		Color tColor1 = pColor;
		tColor1.a = 0.4f;
		Color tColor2 = pColor;
		tColor2.a = 0.1f;
		material.SetColor("_ColorFrom", tColor1);
		material.SetColor("_ColorTo", tColor2);
		return material;
	}

	public string getLocaleID()
	{
		return localized_key ?? id;
	}

	public string getDescriptionID()
	{
		string tDescriptionID = getLocaleID() + "_description";
		if (!string.IsNullOrEmpty(localized_key_description))
		{
			tDescriptionID = localized_key_description;
		}
		if (LocalizedTextManager.stringExists(tDescriptionID))
		{
			return tDescriptionID;
		}
		return getLocaleID() + "_description";
	}

	public Color getColorMain()
	{
		return Toolbox.makeColor(color_hex);
	}

	private static Material cloneMaterial(string pPath)
	{
		return UnityEngine.Object.Instantiate(Resources.Load<Material>(pPath));
	}

	private static ChartItemEffect clonePrefab(string pPath, Transform pParentTransform)
	{
		return UnityEngine.Object.Instantiate(Resources.Load<ChartItemEffect>(pPath), pParentTransform);
	}
}
