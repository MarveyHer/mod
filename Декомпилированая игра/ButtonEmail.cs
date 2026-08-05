using System;
using UnityEngine;
using UnityEngine.Networking;

public class ButtonEmail : MonoBehaviour
{
	public void SendEmail()
	{
		string email = "supworldbox@gmail.com";
		string subject = convert("WorldBox Feedback ( " + Application.version + " )");
		string body = convert("Yo!\r\n");
		Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
		Analytics.LogEvent("clicked_send_email");
	}

	public void SendEmailLogs()
	{
		string email = "supworldbox+errors@gmail.com";
		string subject = convert("WorldBox Error Logs ( " + Application.version + " )");
		string body = convert("Please take a look at this error :\r\n" + LogHandler.log.Substring(Math.Max(0, LogHandler.log.Length - 4000)));
		Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
		Analytics.LogEvent("clicked_send_error_email");
	}

	private string convert(string url)
	{
		return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
	}
}
