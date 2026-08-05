using UnityEngine;

public class BuildingMapIcon
{
	private BuildingColorPixel[][] _tex;

	private BuildingColorPixel _clear_color_pixel = new BuildingColorPixel(Toolbox.clear, Toolbox.clear, Toolbox.clear);

	private int _width;

	private int _height;

	public BuildingMapIcon(Sprite sprite)
	{
		_width = sprite.texture.width;
		_height = sprite.texture.height;
		_tex = new BuildingColorPixel[_height][];
		for (int yy = 0; yy < _height; yy++)
		{
			BuildingColorPixel[] row = new BuildingColorPixel[_width];
			for (int xx = 0; xx < _width; xx++)
			{
				Color32 tColor = sprite.texture.GetPixel(xx, yy);
				if (tColor.a == 0)
				{
					row[xx] = _clear_color_pixel;
					continue;
				}
				Color tColorAbandoned = Toolbox.makeDarkerColor(tColor, 0.9f);
				Color tColorRuin = Toolbox.makeDarkerColor(tColor, 0.6f);
				row[xx] = new BuildingColorPixel(tColor, tColorAbandoned, tColorRuin);
			}
			_tex[yy] = row;
		}
	}

	internal Color32 getColor(int pX, int pY, Building pBuilding)
	{
		if (pX >= _width || pY >= _height)
		{
			return Toolbox.clear;
		}
		BuildingColorPixel tItem = _tex[pY][pX];
		Color32 tColor = tItem.color;
		bool tPixelChanged = false;
		ColorAsset tAsset = pBuilding.kingdom.getColor();
		if (tAsset != null)
		{
			if (Toolbox.areColorsEqual(tColor, Toolbox.color_magenta_0))
			{
				tColor = tAsset.k_color_0;
				tPixelChanged = true;
			}
			else if (Toolbox.areColorsEqual(tColor, Toolbox.color_magenta_1))
			{
				tColor = tAsset.k_color_1;
				tPixelChanged = true;
			}
			else if (Toolbox.areColorsEqual(tColor, Toolbox.color_magenta_2))
			{
				tColor = tAsset.k_color_2;
				tPixelChanged = true;
			}
			else if (Toolbox.areColorsEqual(tColor, Toolbox.color_magenta_3))
			{
				tColor = tAsset.k_color_3;
				tPixelChanged = true;
			}
			else if (Toolbox.areColorsEqual(tColor, Toolbox.color_magenta_4))
			{
				tColor = tAsset.k_color_4;
				tPixelChanged = true;
			}
		}
		if (pBuilding.asset.has_get_map_icon_color && Toolbox.areColorsEqual(tColor, Toolbox.color_map_icon_green))
		{
			tColor = pBuilding.asset.get_map_icon_color(pBuilding);
			tPixelChanged = true;
		}
		if (!tPixelChanged)
		{
			if (pBuilding.isAbandoned())
			{
				tColor = tItem.color_abandoned;
			}
			else if (pBuilding.isRuin())
			{
				tColor = tItem.color_ruin;
			}
		}
		return tColor;
	}
}
