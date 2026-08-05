using System;
using System.Collections.Generic;
using NeoModLoader.General;
using UnityEngine;

namespace NCMS.Utils;

[Obsolete("Compatible Layer will not be maintained and be removed in the future")]
public class Windows
{
	public static Dictionary<string, ScrollWindow> AllWindows;

	internal static void init()
	{
		AllWindows = ScrollWindow._all_windows;
	}

	public static ScrollWindow GetWindow(string pWindowID)
	{
		ScrollWindow value;
		return ScrollWindow._all_windows.TryGetValue(pWindowID, out value) ? value : null;
	}

	public static ScrollWindow CreateNewWindow(string pWindowID, string pWindowTitle)
	{
		if (!LocalizedTextManager.stringExists(pWindowID))
		{
			LM.AddToCurrentLocale(pWindowID, pWindowTitle);
		}
		ScrollWindow scrollWindow = WindowCreator.CreateEmptyWindow(pWindowID, pWindowID);
		((Component)((Component)scrollWindow).gameObject.transform.Find("Background/Title")).GetComponent<LocalizedText>().setKeyAndUpdate(pWindowID);
		((Component)((Component)scrollWindow).gameObject.transform.Find("Background/Title")).GetComponent<LocalizedText>().autoField = false;
		return scrollWindow;
	}

	public static void ShowWindow(string pWindowID)
	{
		ScrollWindow.showWindow(pWindowID);
	}
}
