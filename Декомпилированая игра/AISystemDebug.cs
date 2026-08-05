using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AISystemDebug
{
	private static string dataName = "/ai_system.log";

	private static List<string> debug_list_actions = new List<string>();

	public static void clear()
	{
		debug_list_actions.Clear();
	}

	public static void debugLog(string pString)
	{
		debug_list_actions.Add(pString);
		if (debug_list_actions.Count > 1000)
		{
			debug_list_actions.RemoveAt(0);
		}
	}

	public static void log()
	{
		string tResult = "";
		foreach (string tAction in debug_list_actions)
		{
			tResult = tResult + tAction + "\n";
		}
		File.WriteAllText(getPath(), tResult);
	}

	public static string getPath()
	{
		return Application.persistentDataPath + dataName;
	}
}
