using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class BaseLibraryWithUnlockables<T> : AssetLibrary<T>, ILibraryWithUnlockables where T : BaseUnlockableAsset
{
	[JsonIgnore]
	public IEnumerable<BaseUnlockableAsset> elements_list => list;

	public override void editorDiagnosticLocales()
	{
		base.editorDiagnosticLocales();
		foreach (T tAsset in list)
		{
			checkLocale(tAsset, tAsset.getLocaleID());
		}
	}
}
