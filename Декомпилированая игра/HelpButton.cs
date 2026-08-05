using UnityEngine;

public class HelpButton : MonoBehaviour
{
	public void clickHelp()
	{
		string tLocale = PlayerConfig.dict["language"].stringVal;
		Analytics.LogEvent("open_help");
		string tLink = "";
		tLink = ((Application.platform != RuntimePlatform.Android) ? ("https://support.apple.com/" + tLocale + "-" + tLocale + "/HT203005") : ("https://support.google.com/googleplay/answer/1050566?hl=" + tLocale));
		Application.OpenURL(tLink);
	}
}
