using UnityEngine;

namespace NCMS;

public class Core
{
	public static string WBGamePath;

	public static string ModsPath;

	public static string ManagedPath;

	public static string NCMSPath;

	public static string NCMSModsPath;

	public static string CorePath;

	public static string AssembliesPath;

	public static string TempPath;

	static Core()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		WBGamePath = (((int)Application.platform == 2) ? (Application.dataPath + "/..") : (Application.dataPath + "/../.."));
		ModsPath = Application.streamingAssetsPath + "/Mods";
		ManagedPath = Application.streamingAssetsPath + "/../Managed";
		NCMSPath = ModsPath + "/NCMS";
		NCMSModsPath = WBGamePath + "/Mods";
		CorePath = NCMSPath + "/Core";
		AssembliesPath = CorePath + "/Assemblies";
		TempPath = CorePath + "/Temp";
	}
}
