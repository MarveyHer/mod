using System;

public abstract class ItemAssetLibrary<T> : BaseLibraryWithUnlockables<T> where T : ItemAsset
{
	public override T add(T pAsset)
	{
		T tNewAsset = base.add(pAsset);
		if (tNewAsset.base_stats == null)
		{
			tNewAsset.base_stats = new BaseStats();
		}
		return tNewAsset;
	}

	public override void editorDiagnosticLocales()
	{
		foreach (T tAsset in list)
		{
			if (!tAsset.has_locales)
			{
				continue;
			}
			string tName = tAsset.getLocaleID();
			checkLocale(tAsset, tName);
			if (tAsset.isMod())
			{
				continue;
			}
			string tDescription1 = tAsset.getDescriptionID();
			checkLocale(tAsset, tDescription1);
			if (tAsset.material != "basic")
			{
				checkLocale(tAsset, tAsset.getMaterialID());
			}
			foreach (Rarity tRarity in Enum.GetValues(typeof(Rarity)))
			{
				string tResult = tAsset.getLocaleRarity(tRarity);
				checkLocale(tAsset, tResult);
			}
		}
	}
}
