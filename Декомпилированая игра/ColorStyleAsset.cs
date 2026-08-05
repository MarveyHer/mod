using System;
using UnityEngine;

[Serializable]
public class ColorStyleAsset : Asset
{
	public string taxonomy_kingdom = "#76FFF8";

	public string taxonomy_phylum = "#74FFA3";

	public string taxonomy_subphylum = "#54FF8D";

	public string taxonomy_class = "#76FF4A";

	public string taxonomy_order = "#B9FF48";

	public string taxonomy_family = "#FEFD46";

	public string taxonomy_genus = "#F8AB4F";

	public string taxonomy_common_name = "#DC8D4E";

	public string color_text_grey = "#ADADAD";

	public string color_text_grey_dark = "#7D7D7D";

	public string color_text_selector = "#7FFF75AA";

	public string color_text_selector_remove = "#FF182AAA";

	public string color_text_pumpkin = "#FFA94C";

	public string color_text_pumpkin_light = "#FFBC66";

	public Color favorite_selected = Color.white;

	public Color favorite_not_selected = new Color(0.7f, 0.7f, 0.7f, 0.3f);

	public Color health_bar_main_green = Toolbox.makeColor("#00C21F");

	public Color health_bar_main_red = Toolbox.makeColor("#FF4300");

	public Color health_bar_background = Toolbox.makeColor("#303030");

	public string color_dead_text => color_text_grey_dark;

	public Color getSelectorColor()
	{
		return Toolbox.makeColor(color_text_selector);
	}

	public Color getSelectorRemoveColor()
	{
		return Toolbox.makeColor(color_text_selector_remove);
	}

	public string getColorForTaxonomy(string pID)
	{
		return pID switch
		{
			"taxonomy_kingdom" => taxonomy_kingdom, 
			"taxonomy_phylum" => taxonomy_phylum, 
			"taxonomy_subphylum" => taxonomy_subphylum, 
			"taxonomy_class" => taxonomy_class, 
			"taxonomy_order" => taxonomy_order, 
			"taxonomy_family" => taxonomy_family, 
			"taxonomy_genus" => taxonomy_genus, 
			"taxonomy_common_name" => taxonomy_common_name, 
			_ => "0xFFFFFF", 
		};
	}
}
