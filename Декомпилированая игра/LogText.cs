using System.IO;
using UnityEngine;

public class LogText
{
	private static string dataName = "/wb_runtime.log";

	private static bool created = false;

	internal static int offset = 0;

	public static void log(string pEvent, string pInfo = "", string pState = "")
	{
		if (!Globals.DIAGNOSTIC)
		{
			return;
		}
		if (!created)
		{
			created = true;
			File.WriteAllText(getPath(), "");
		}
		if (pState == "st")
		{
			offset++;
		}
		else if (pState == "en")
		{
			offset--;
		}
		string tOffsetStr = "";
		for (int i = 0; i < offset; i++)
		{
			tOffsetStr += " ";
		}
		if (pState == "en")
		{
			tOffsetStr += " ";
		}
		else if (pState == "")
		{
			tOffsetStr += " ";
		}
		if (pState == "en")
		{
			pState = "x";
		}
		else if (pState == "st")
		{
			pState = "!";
		}
		if (pInfo != "")
		{
			pEvent = tOffsetStr + pEvent + " :: " + pInfo;
			if (pState != "")
			{
				pEvent = pEvent + " - " + pState;
			}
		}
		File.AppendAllText(getPath(), pEvent + "\n");
	}

	public static string getPath()
	{
		return Application.persistentDataPath + dataName;
	}
}
