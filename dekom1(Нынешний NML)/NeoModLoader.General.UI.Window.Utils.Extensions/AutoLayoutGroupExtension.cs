using NeoModLoader.General.UI.Window.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Window.Utils.Extensions;

public static class AutoLayoutGroupExtension
{
	public static AutoHoriLayoutGroup BeginHoriGroup<T, TElement>(this AutoLayoutGroup<T, TElement> pThis, Vector2 pSize = default(Vector2), TextAnchor pAlignment = (TextAnchor)3, float pSpacing = 3f, RectOffset pPadding = null) where T : LayoutGroup where TElement : AutoLayoutGroup<T, TElement>
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		AutoHoriLayoutGroup autoHoriLayoutGroup = pThis.BeginSubGroup<AutoHoriLayoutGroup, HorizontalLayoutGroup>(pSize);
		if (pSize == default(Vector2))
		{
			ContentSizeFitter val = ((Component)autoHoriLayoutGroup).gameObject.GetComponent<ContentSizeFitter>();
			if ((Object)(object)val == (Object)null)
			{
				val = ((Component)autoHoriLayoutGroup).gameObject.AddComponent<ContentSizeFitter>();
			}
			val.horizontalFit = (FitMode)2;
			val.verticalFit = (FitMode)2;
		}
		HorizontalLayoutGroup layoutGroup = autoHoriLayoutGroup.GetLayoutGroup();
		((LayoutGroup)layoutGroup).childAlignment = pAlignment;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).spacing = pSpacing;
		((LayoutGroup)layoutGroup).padding = (RectOffset)(((object)pPadding) ?? ((object)new RectOffset(3, 3, 3, 3)));
		return autoHoriLayoutGroup;
	}

	public static AutoVertLayoutGroup BeginVertGroup<T, TElement>(this AutoLayoutGroup<T, TElement> pThis, Vector2 pSize = default(Vector2), TextAnchor pAlignment = (TextAnchor)1, float pSpacing = 3f, RectOffset pPadding = null) where T : LayoutGroup where TElement : AutoLayoutGroup<T, TElement>
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		AutoVertLayoutGroup autoVertLayoutGroup = pThis.BeginSubGroup<AutoVertLayoutGroup, VerticalLayoutGroup>(pSize);
		if (pSize == default(Vector2))
		{
			ContentSizeFitter val = ((Component)autoVertLayoutGroup).gameObject.GetComponent<ContentSizeFitter>();
			if ((Object)(object)val == (Object)null)
			{
				val = ((Component)autoVertLayoutGroup).gameObject.AddComponent<ContentSizeFitter>();
			}
			val.horizontalFit = (FitMode)2;
			val.verticalFit = (FitMode)2;
		}
		VerticalLayoutGroup layoutGroup = autoVertLayoutGroup.GetLayoutGroup();
		((LayoutGroup)layoutGroup).childAlignment = pAlignment;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childControlWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childForceExpandWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleHeight = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).childScaleWidth = false;
		((HorizontalOrVerticalLayoutGroup)layoutGroup).spacing = pSpacing;
		((LayoutGroup)layoutGroup).padding = (RectOffset)(((object)pPadding) ?? ((object)new RectOffset(3, 3, 3, 3)));
		return autoVertLayoutGroup;
	}

	public static AutoGridLayoutGroup BeginGridGroup<T, TElement>(this AutoLayoutGroup<T, TElement> pThis, int pConstraintCount, Constraint pConstraint = (Constraint)1, Vector2 pSize = default(Vector2), Vector2 pCellSize = default(Vector2), Vector2 pSpacing = default(Vector2), Axis pStartAxis = (Axis)0, Corner pStartCorner = (Corner)0) where T : LayoutGroup where TElement : AutoLayoutGroup<T, TElement>
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		AutoGridLayoutGroup autoGridLayoutGroup = pThis.BeginSubGroup<AutoGridLayoutGroup, GridLayoutGroup>(pSize);
		if (pSize == default(Vector2))
		{
			ContentSizeFitter val = ((Component)autoGridLayoutGroup).gameObject.GetComponent<ContentSizeFitter>();
			if ((Object)(object)val == (Object)null)
			{
				val = ((Component)autoGridLayoutGroup).gameObject.AddComponent<ContentSizeFitter>();
			}
			val.horizontalFit = (FitMode)2;
			val.verticalFit = (FitMode)2;
		}
		GridLayoutGroup layoutGroup = autoGridLayoutGroup.GetLayoutGroup();
		layoutGroup.constraint = pConstraint;
		layoutGroup.constraintCount = pConstraintCount;
		layoutGroup.cellSize = (Vector2)((pCellSize == default(Vector2)) ? new Vector2(16f, 16f) : pCellSize);
		layoutGroup.spacing = (Vector2)((pSpacing == default(Vector2)) ? new Vector2(3f, 3f) : pSpacing);
		layoutGroup.startAxis = pStartAxis;
		layoutGroup.startCorner = pStartCorner;
		return autoGridLayoutGroup;
	}
}
