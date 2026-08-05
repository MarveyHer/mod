using UnityEngine;

public readonly struct BuildingColorPixel(Color32 pColor, Color32 pColorAbandoned, Color32 pColorRuin)
{
	public readonly Color32 color = pColor;

	public readonly Color32 color_abandoned = pColorAbandoned;

	public readonly Color32 color_ruin = pColorRuin;
}
