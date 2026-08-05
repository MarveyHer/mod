using System;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Window.Layout;

public class AutoHoriLayoutGroup : AutoLayoutGroup<HorizontalLayoutGroup, AutoHoriLayoutGroup>
{
	public void Setup(Vector2 pSize = default(Vector2), TextAnchor pAlignment = (TextAnchor)3, float pSpacing = 3f, RectOffset pPadding = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Init();
		if (pSize == default(Vector2))
		{
			((Behaviour)base.fitter).enabled = true;
		}
		else
		{
			((Behaviour)base.fitter).enabled = false;
			((Component)this).GetComponent<RectTransform>().sizeDelta = pSize;
		}
		((LayoutGroup)base.layout).childAlignment = pAlignment;
		((HorizontalOrVerticalLayoutGroup)base.layout).spacing = pSpacing;
		((LayoutGroup)base.layout).padding = (RectOffset)(((object)pPadding) ?? ((object)new RectOffset(3, 3, 3, 3)));
	}

	internal static void _init()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		GameObject val = new GameObject("AutoHoriLayoutGroup", new Type[3]
		{
			typeof(HorizontalLayoutGroup),
			typeof(AutoHoriLayoutGroup),
			typeof(ContentSizeFitter)
		});
		ContentSizeFitter component = val.GetComponent<ContentSizeFitter>();
		component.verticalFit = (FitMode)2;
		component.horizontalFit = (FitMode)2;
		HorizontalLayoutGroup component2 = val.GetComponent<HorizontalLayoutGroup>();
		((LayoutGroup)component2).childAlignment = (TextAnchor)3;
		((HorizontalOrVerticalLayoutGroup)component2).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)component2).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)component2).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)component2).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)component2).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)component2).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)component2).spacing = 3f;
		((LayoutGroup)component2).padding = new RectOffset(3, 3, 3, 3);
		APrefab<AutoHoriLayoutGroup>.Prefab = val.GetComponent<AutoHoriLayoutGroup>();
	}
}
