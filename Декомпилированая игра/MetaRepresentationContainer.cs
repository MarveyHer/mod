using System.Collections.Generic;

public class MetaRepresentationContainer : MetaRepresentationContainerBase
{
	private IMetaWindow _meta_window;

	protected override void init()
	{
		base.init();
		_meta_window = GetComponentInParent<IMetaWindow>();
	}

	protected override void fillDict(ref int pTotal, ref bool pAny, Dictionary<IMetaObject, int> pDict)
	{
		foreach (Actor tActor in getMetaObject().getUnits())
		{
			pTotal++;
			if (asset.check_has_meta(tActor))
			{
				pAny = true;
				IMetaObject tMeta = asset.meta_getter(tActor);
				if (!pDict.ContainsKey(tMeta))
				{
					pDict.Add(tMeta, 0);
				}
				pDict[tMeta]++;
			}
		}
	}

	protected override void checkShowNone(bool pAny, int pNone, int pTotal)
	{
		if (pAny && asset.show_none_percent && pNone > 0)
		{
			string tNoneRow = amountWithPercent(pNone, pTotal);
			KeyValueField tField = showStatRow("statistics_breakdown_none", tNoneRow, ColorStyleLibrary.m.color_text_grey, MetaType.None, -1L, pColorText: true, asset.general_icon_path);
			showBar(tField, pNone, pTotal, ColorStyleLibrary.m.color_text_grey);
		}
	}

	private IMetaObject getMetaObject()
	{
		return _meta_window.getCoreObject() as IMetaObject;
	}
}
