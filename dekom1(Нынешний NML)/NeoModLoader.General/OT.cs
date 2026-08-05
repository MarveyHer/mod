using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General;

public static class OT
{
	public static void InitializeCommonText(Text text)
	{
		text.font = LocalizedTextManager.current_font;
		text.supportRichText = true;
	}

	public static void InitializeNoActionVerticalLayoutGroup(VerticalLayoutGroup pVerticalLayoutGroup)
	{
		((LayoutGroup)pVerticalLayoutGroup).childAlignment = (TextAnchor)1;
		((HorizontalOrVerticalLayoutGroup)pVerticalLayoutGroup).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)pVerticalLayoutGroup).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)pVerticalLayoutGroup).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)pVerticalLayoutGroup).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)pVerticalLayoutGroup).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)pVerticalLayoutGroup).childScaleWidth = false;
	}
}
