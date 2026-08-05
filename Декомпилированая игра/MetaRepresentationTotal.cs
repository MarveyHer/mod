using System.Collections.Generic;

public class MetaRepresentationTotal : MetaRepresentationContainerBase
{
	protected override void fillDict(ref int pTotal, ref bool pAny, Dictionary<IMetaObject, int> pDict)
	{
		List<Actor> tUnits = asset.world_units_getter();
		pTotal = tUnits.Count;
		foreach (Actor tActor in tUnits)
		{
			if (!asset.check_has_meta(tActor))
			{
				if (!asset.show_none_percent_for_total)
				{
					pTotal--;
				}
				continue;
			}
			pAny = true;
			using ListPool<IMetaObject> tMetas = asset.meta_getter_total(tActor);
			foreach (ref IMetaObject item in tMetas)
			{
				IMetaObject tMeta = item;
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
		if (asset.show_none_percent_for_total && pAny && pNone > 0)
		{
			string tLocalizedTitle = "statistics_breakdown_none_list".Localize();
			tLocalizedTitle += Toolbox.coloredGreyPart(pNone, ColorStyleLibrary.m.color_text_grey);
			string tNoneRow = amountWithPercent(pNone, pTotal);
			KeyValueField tField = showStatRow(tLocalizedTitle, tNoneRow, ColorStyleLibrary.m.color_text_grey, MetaType.None, -1L, pColorText: true, asset.general_icon_path, null, null, pLocalize: false);
			showBar(tField, pNone, pTotal, ColorStyleLibrary.m.color_text_grey);
		}
	}
}
