using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NeoModLoader.constants;

public static class Paths
{
	public static readonly string NMLModPath;

	public static readonly string PersistentDataPath;

	public static readonly string StreamingAssetsPath;

	public static readonly string NativeModsPath;

	public static readonly string ManagedPath;

	public static readonly string NMLPath;

	public static readonly string NMLCommitPath;

	public static readonly string NMLAutoUpdateModulePath;

	public static readonly string PublicizedAssemblyPath;

	public static readonly string ModsConfigPath;

	public static readonly string BepInExPluginsPath;

	public static readonly string ModsPath;

	public static readonly string NMLAssembliesPath;

	public static readonly string CompiledModsPath;

	public static readonly string TabOrderRecordPath;

	public static readonly string ModCompileRecordPath;

	public static readonly string ModsDisabledRecordPath;

	public static readonly string ModDeclarationFileName;

	public static readonly string ModDefaultConfigFileName;

	public static readonly string ModResourceFolderName;

	public static readonly string NCMSAdditionModResourceFolderName;

	public static readonly string ModAssetBundleFolderName;

	public static readonly string CommonModsWorkshopPath;

	public static readonly string NCMSModEmbededResourceFolderName;

	public static readonly HashSet<string> IgnoreSearchDirectories;

	internal static readonly string LinuxSteamLocalConfigPath;

	public static string GamePath
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			RuntimePlatform platform = Application.platform;
			if (1 == 0)
			{
			}
			string result = (((int)platform == 1) ? Combine(StreamingAssetsPath, "..", "..", "..", "..", "..") : (((int)platform == 2) ? Combine(StreamingAssetsPath, "..", "..") : (((int)platform != 13) ? Combine(StreamingAssetsPath, "..", "..") : Combine(StreamingAssetsPath, "..", ".."))));
			if (1 == 0)
			{
			}
			return result;
		}
	}

	static Paths()
	{
		PersistentDataPath = Combine(Application.persistentDataPath);
		StreamingAssetsPath = Combine(Application.streamingAssetsPath);
		NativeModsPath = Combine(StreamingAssetsPath, "mods");
		ManagedPath = (Others.is_editor ? Combine(StreamingAssetsPath, "..", ".Managed") : Combine(StreamingAssetsPath, "..", "Managed"));
		NMLPath = Combine(NativeModsPath, "NML");
		NMLCommitPath = Combine(NMLPath, "commit");
		NMLAutoUpdateModulePath = Combine(NativeModsPath, "NeoModLoader.AutoUpdate_memload.dll");
		PublicizedAssemblyPath = Combine(NMLPath, "Assembly-CSharp-Publicized.dll");
		ModsConfigPath = Combine(PersistentDataPath, "mods_config");
		BepInExPluginsPath = Combine(GamePath, "BepInEx", "plugins");
		ModsPath = (Others.is_editor ? Combine(GamePath, "Assets", "Mods") : Combine(GamePath, "Mods"));
		NMLAssembliesPath = Combine(NMLPath, "Assemblies");
		CompiledModsPath = Combine(NMLPath, "CompiledMods");
		TabOrderRecordPath = Combine(NMLPath, "tab_order_records.json");
		ModCompileRecordPath = Combine(NMLPath, "mod_compile_records.json");
		ModsDisabledRecordPath = Combine(NMLPath, "disabled_mods.txt");
		ModDeclarationFileName = "mod.json";
		ModDefaultConfigFileName = "default_config.json";
		ModResourceFolderName = (Others.is_editor ? "Resources" : "GameResources");
		NCMSAdditionModResourceFolderName = "GameResourcesReplace";
		ModAssetBundleFolderName = "AssetBundles";
		CommonModsWorkshopPath = Combine(GamePath, "..", "..", "workshop", "content", 1206560uL.ToString());
		NCMSModEmbededResourceFolderName = "EmbededResources";
		IgnoreSearchDirectories = new HashSet<string> { "bin", "obj", "Properties", "packages", "packages.config", "packages-lock.json", "packages-lock.xml" };
		LinuxSteamLocalConfigPath = "~/.local/share/Steam/userdata/{0}/config/localconfig.vdf";
		string text = Assembly.GetExecutingAssembly().Location;
		if (string.IsNullOrEmpty(text))
		{
			text = Combine(NativeModsPath, "NeoModLoader.dll");
			if (!File.Exists(text))
			{
				text = Combine(NativeModsPath, "NeoModLoader_memload.dll");
			}
		}
		NMLModPath = text;
	}

	private static string Combine(params string[] paths)
	{
		return new FileInfo(paths.Aggregate("", Path.Combine)).FullName;
	}
}
