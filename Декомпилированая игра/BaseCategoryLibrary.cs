using System;

[Serializable]
public class BaseCategoryLibrary<T> : AssetLibrary<T> where T : BaseCategoryAsset
{
	public override void editorDiagnosticLocales()
	{
		base.editorDiagnosticLocales();
		foreach (T tAsset in list)
		{
			checkLocale(tAsset, tAsset.getLocaleID());
		}
	}
}
