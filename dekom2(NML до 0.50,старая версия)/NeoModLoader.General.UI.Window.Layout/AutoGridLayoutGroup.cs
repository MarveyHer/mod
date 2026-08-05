using System;
using NeoModLoader.General.UI.Prefabs;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.UI.Window.Layout;

public class AutoGridLayoutGroup : AutoLayoutGroup<GridLayoutGroup, AutoGridLayoutGroup>
{
	public void Setup(int pConstraintCount, Constraint pConstraint = (Constraint)1, Vector2 pSize = default(Vector2), Vector2 pCellSize = default(Vector2), Vector2 pSpacing = default(Vector2), Axis pStartAxis = (Axis)0, Corner pStartCorner = (Corner)0)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
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
		base.layout.constraint = pConstraint;
		base.layout.constraintCount = pConstraintCount;
		base.layout.cellSize = (Vector2)((pCellSize == default(Vector2)) ? new Vector2(16f, 16f) : pCellSize);
		base.layout.spacing = (Vector2)((pSpacing == default(Vector2)) ? new Vector2(3f, 3f) : pSpacing);
		base.layout.startAxis = pStartAxis;
		base.layout.startCorner = pStartCorner;
	}

	internal static void _init()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("AutoGridLayoutGroup", new Type[3]
		{
			typeof(GridLayoutGroup),
			typeof(AutoGridLayoutGroup),
			typeof(ContentSizeFitter)
		});
		ContentSizeFitter component = val.GetComponent<ContentSizeFitter>();
		component.verticalFit = (FitMode)2;
		component.horizontalFit = (FitMode)2;
		GridLayoutGroup component2 = val.GetComponent<GridLayoutGroup>();
		component2.constraint = (Constraint)1;
		component2.constraintCount = 3;
		component2.cellSize = new Vector2(16f, 16f);
		component2.spacing = new Vector2(3f, 3f);
		component2.startAxis = (Axis)0;
		component2.startCorner = (Corner)0;
		APrefab<AutoGridLayoutGroup>.Prefab = val.GetComponent<AutoGridLayoutGroup>();
	}
}
