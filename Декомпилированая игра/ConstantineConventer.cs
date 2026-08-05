using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConstantineConventer
{
	private static bool enabled;

	public static void init()
	{
		if (enabled)
		{
			string[] array = Resources.Load<TextAsset>("texts/fmod_sheet").text.Split('\n');
			Debug.Log(array[0]);
			List<string> tNewList = new List<string>();
			string tResult = "";
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string tEventString = array2[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
				string tId = tEventString.Split('/')[^1];
				string tNewString = "\tpublic const string ";
				tNewString += tId;
				tNewString += " = ";
				tNewString += "\"";
				tNewString += tEventString;
				tNewString += "\"";
				tNewString += ";";
				tNewList.Add(tNewString);
				tResult = tResult + tNewString + "\n";
			}
			File.WriteAllText(Application.dataPath + "/Resources/texts/fmod_sheet_converted.txt", tResult);
		}
	}

	public static void init2()
	{
		string[] array = Resources.Load<TextAsset>("texts/fmod_sheet").text.Split('\n');
		Debug.Log(array[0]);
		List<string> tNewList = new List<string>();
		string tResult = "";
		string tCurrentTag = "";
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string tCleanString = array2[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
			if (tCleanString.Contains("$"))
			{
				tCleanString = tCleanString.Replace("$ ", "");
				tCleanString = tCleanString.Replace("$", "");
				tCurrentTag = tCleanString;
				continue;
			}
			if (!tCleanString.Contains("WB_SFX_"))
			{
				tResult += "\n";
				continue;
			}
			string tNewString = "\tpublic const string ";
			tNewString += tCleanString;
			tNewString += " = ";
			tNewString = tNewString + tCurrentTag + " + ";
			tNewString += "\"";
			tNewString += tCleanString;
			tNewString += "\"";
			tNewString += ";";
			tNewList.Add(tNewString);
			tResult = tResult + tNewString + "\n";
		}
		File.WriteAllText(Application.dataPath + "/Resources/texts/fmod_sheet_converted.txt", tResult);
	}
}
