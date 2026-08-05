using System;
using UnityEngine;

[Serializable]
public class ForcedFontStyle
{
	public FontStyle style;

	public bool shadow;

	public ForcedFontStyle(FontStyle pStyle, bool pShadow = false)
	{
		style = pStyle;
		shadow = pShadow;
	}
}
