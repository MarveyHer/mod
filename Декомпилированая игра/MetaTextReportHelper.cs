public class MetaTextReportHelper
{
	public static string color_text_main => ColorStyleLibrary.m.color_text_grey;

	public static string color_text_quote => ColorStyleLibrary.m.color_text_grey_dark;

	public static string addSingleUnitText(Actor pActor, bool pAddGap = true, bool pAddNameQuote = true)
	{
		if (!pActor.hasHappinessHistory())
		{
			return string.Empty;
		}
		using ListPool<HappinessHistory> tTempListPool = new ListPool<HappinessHistory>(pActor.happiness_change_history);
		HappinessHistory tHappinessElement = tTempListPool.GetRandom();
		string tTranslatedResult = tHappinessElement.asset.getRandomTextSingleReportLocalized();
		string tQuote = "<i>\"" + tTranslatedResult + "\"</i>";
		string tName = "\n— " + pActor.name;
		string tInfo = pActor.getAge().ToString();
		tInfo = ((!pActor.isSexFemale()) ? (tInfo + " M") : (tInfo + " F"));
		string tAgo = tHappinessElement.getAgoString().ColorHex(ColorStyleLibrary.m.color_text_grey_dark);
		string tFinalResult = "";
		if (pAddGap)
		{
			tFinalResult = "\n\n";
		}
		tFinalResult = tFinalResult + tQuote.ColorHex(color_text_quote) + "  " + tAgo;
		if (pAddNameQuote)
		{
			tFinalResult = tFinalResult + tName.ColorHex(pActor.kingdom.getColor().color_text) + "  " + tInfo.ColorHex(ColorStyleLibrary.m.color_text_grey_dark);
		}
		return tFinalResult;
	}

	public static string addSingleUnitTextRandomUnit(IMetaObject pMetaObject, out Actor pActorResult)
	{
		pActorResult = null;
		int tTries = 10;
		while (tTries-- > 0)
		{
			Actor tActor = pMetaObject.getRandomUnit();
			if (tActor != null && tActor.isAlive() && tActor.hasHappinessHistory())
			{
				string tFinalResult = addSingleUnitText(tActor);
				if (!string.IsNullOrEmpty(tFinalResult))
				{
					pActorResult = tActor;
					return tFinalResult;
				}
			}
		}
		return string.Empty;
	}

	public static string getText(IMetaObject pMetaObject, MetaTypeAsset pAsset, out Actor pActorResult)
	{
		pActorResult = null;
		string tFinalText = string.Empty;
		bool tAnyTextAdded = false;
		string[] reports = pAsset.reports;
		foreach (string tReportID in reports)
		{
			MetaTextReportAsset tReportAsset = AssetManager.meta_text_report_library.get(tReportID);
			if (tReportAsset.report_action(pMetaObject))
			{
				if (tAnyTextAdded)
				{
					tFinalText += " ";
				}
				tAnyTextAdded = true;
				string tTextToAdd = tReportAsset.get_random_text;
				if (tReportAsset.color != null)
				{
					tTextToAdd = tTextToAdd.ColorHex(tReportAsset.color);
				}
				tFinalText += tTextToAdd;
			}
		}
		if (tAnyTextAdded)
		{
			tFinalText += addSingleUnitTextRandomUnit(pMetaObject, out var tActorResult);
			pActorResult = tActorResult;
			tFinalText = tFinalText.ColorHex(color_text_main);
		}
		return tFinalText;
	}
}
