using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorAsset : Asset
{
	private static int _create_last_index_id = 1000;

	public int index_id;

	public string color_main;

	public string color_main_2;

	public string color_banner;

	public string color_text;

	public bool favorite;

	[NonSerialized]
	public Color32 k_color_0;

	[NonSerialized]
	public Color32 k_color_1;

	[NonSerialized]
	public Color32 k_color_2;

	[NonSerialized]
	public Color32 k_color_3;

	[NonSerialized]
	public Color32 k_color_4;

	[NonSerialized]
	public Color32 k2_color_0;

	[NonSerialized]
	public Color32 k2_color_1;

	[NonSerialized]
	public Color32 k2_color_2;

	[NonSerialized]
	public Color32 k2_color_3;

	[NonSerialized]
	public Color32 k2_color_4;

	private Color32 _color_main_32;

	private Color32 _color_main_second_32;

	private Color32 _color_unit_32;

	private Color32 _color_border_inside_alpha_32;

	private Color _color_main;

	private Color _color_main_second;

	private Color _color_text;

	private Color _color_minimap_element;

	private Color _color_border_out_capture;

	private Color _color_banner;

	private static readonly List<ColorAsset> _all_colors_list = new List<ColorAsset>();

	private static readonly Dictionary<string, ColorAsset> _all_colors_dict = new Dictionary<string, ColorAsset>();

	public const byte ALPHA_BORDER_INSIDE_BYTE = 170;

	private bool _initialized;

	private Material _material_line;

	private Material _material_gradient;

	public static List<ColorAsset> getAllColorsList()
	{
		return _all_colors_list;
	}

	public ColorAsset()
	{
	}

	public static bool isColorAssetExists(string pColorMain)
	{
		return _all_colors_dict.ContainsKey(pColorMain);
	}

	public static ColorAsset getExistingColorAsset(string pColorMain)
	{
		_all_colors_dict.TryGetValue(pColorMain, out var tAsset);
		return tAsset;
	}

	public static ColorAsset tryMakeNewColorAsset(string pColorMain)
	{
		_all_colors_dict.TryGetValue(pColorMain, out var tAsset);
		if (tAsset == null)
		{
			return new ColorAsset(pColorMain);
		}
		return tAsset;
	}

	private ColorAsset(string pColorMain)
	{
		setMainHexColors(pColorMain, pColorMain, pColorMain);
		index_id = _create_last_index_id++;
		saveToGlobalList(this);
	}

	public static void saveToGlobalList(ColorAsset pAsset, bool pMustBeGlobal = false)
	{
		if (isColorAssetExists(pAsset.color_main))
		{
			if (pMustBeGlobal)
			{
				Debug.LogError("ColorAsset with same <b>color_main</b> already exists in global list: " + pAsset.id + " " + pAsset.index_id + " " + pAsset.color_main);
			}
		}
		else
		{
			_all_colors_list.Add(pAsset);
			_all_colors_dict.Add(pAsset.color_main, pAsset);
		}
	}

	private void setMainHexColors(string pColorMain, string pColorMain2, string pColorBanner)
	{
		color_main = pColorMain;
		color_main_2 = pColorMain2;
		color_banner = pColorBanner;
		color_text = pColorMain;
	}

	public void setEditorColors(Color pMain, Color pMain2, Color pBanner, Color pText)
	{
		initColor();
	}

	public void initColor()
	{
		if (!_initialized)
		{
			_initialized = true;
			_color_main = Toolbox.makeColor(color_main);
			_color_main_32 = Toolbox.makeColor(color_main);
			_color_main_second = Toolbox.makeColor(color_main_2);
			_color_main_second_32 = Toolbox.makeColor(color_main_2);
			_color_text = Toolbox.makeColor(color_text);
			_color_banner = Toolbox.makeColor(color_banner);
			Color tColor = _color_main_32;
			_color_border_inside_alpha_32 = new Color(tColor.r, tColor.g, tColor.b);
			_color_border_inside_alpha_32.a = 170;
			Color32 tColorDarkVersion = new Color32(30, 30, 30, byte.MaxValue);
			_color_border_out_capture = new Color(_color_main_second.r, _color_main_second.g, _color_main_second.b, 0.8f);
			_color_unit_32 = Color.Lerp(_color_main_second, Color.white, 0.3f);
			_color_unit_32.a = byte.MaxValue;
			k_color_0 = _color_text;
			k_color_0 = checkIfColorTooDark(k_color_0);
			_color_minimap_element = Color32.Lerp(k_color_0, Color.white, 0.2f);
			k_color_1 = Color32.Lerp(k_color_0, tColorDarkVersion, 0.13f);
			k_color_2 = Color32.Lerp(k_color_0, tColorDarkVersion, 0.35000002f);
			k_color_3 = Color32.Lerp(k_color_0, tColorDarkVersion, 0.51f);
			k_color_4 = Color32.Lerp(k_color_0, tColorDarkVersion, 0.65999997f);
			k2_color_0 = _color_main_32;
			k2_color_0 = checkIfColorTooDark(k2_color_0);
			k2_color_1 = Color32.Lerp(k2_color_0, tColorDarkVersion, 0.13f);
			k2_color_2 = Color32.Lerp(k2_color_0, tColorDarkVersion, 0.35000002f);
			k2_color_3 = Color32.Lerp(k2_color_0, tColorDarkVersion, 0.51f);
			k2_color_4 = Color32.Lerp(k2_color_0, tColorDarkVersion, 0.65999997f);
		}
	}

	public Material getChartLineMaterial()
	{
		if (_material_line == null)
		{
			_material_line = cloneMaterial("materials/graph/graph_base_line");
			Color tColor = getColorText();
			_material_line.SetColor("_Color", tColor);
		}
		return _material_line;
	}

	public Material getChartInnerFillMaterial()
	{
		if (_material_gradient == null)
		{
			_material_gradient = cloneMaterial("materials/graph/graph_base_gradient");
			Color tColor1 = getColorText();
			tColor1.a = 0.4f;
			Color tColor2 = getColorText();
			tColor2.a = 0.1f;
			_material_gradient.SetColor("_ColorFrom", tColor1);
			_material_gradient.SetColor("_ColorTo", tColor2);
		}
		return _material_gradient;
	}

	private Material cloneMaterial(string pPath)
	{
		return UnityEngine.Object.Instantiate(Resources.Load<Material>(pPath));
	}

	private Color32 checkIfColorTooDark(Color32 pColor)
	{
		if (pColor.r < 128 && pColor.g < 128 && pColor.b < 128)
		{
			pColor.r += 50;
			pColor.g += 50;
			pColor.b += 50;
		}
		return pColor;
	}

	private Color32 getDarkerColor(Color32 pColor, byte pValue)
	{
		pColor.r += pValue;
		pColor.g += pValue;
		pColor.b += pValue;
		return pColor;
	}

	public Color32 getColorUnit32()
	{
		return _color_unit_32;
	}

	public Color32 getColorMain32()
	{
		return _color_main_32;
	}

	public Color32 getColorBorderInsideAlpha32()
	{
		return _color_border_inside_alpha_32;
	}

	public Color32 getColorMainSecond32()
	{
		return _color_main_second_32;
	}

	public Color getColorMainSecond()
	{
		return _color_main_second;
	}

	public Color getColorMain()
	{
		return _color_main;
	}

	public Color getColorText()
	{
		return _color_text;
	}

	public ref Color getColorTextRef()
	{
		return ref _color_text;
	}

	public Color getColorMinimapElements()
	{
		return _color_minimap_element;
	}

	public Color getColorBorderOut_capture()
	{
		return _color_border_out_capture;
	}

	public Color getColorBanner()
	{
		return _color_banner;
	}
}
