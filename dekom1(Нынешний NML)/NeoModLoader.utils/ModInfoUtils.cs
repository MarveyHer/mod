using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using NeoModLoader.api;
using NeoModLoader.constants;
using NeoModLoader.General;
using NeoModLoader.services;
using NeoModLoader.ui;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Steamworks;
using UnityEngine;

namespace NeoModLoader.utils;

internal static class ModInfoUtils
{
	private static Queue<ModDeclare> link_request_mods = new Queue<ModDeclare>();

	private static bool to_install_bepinex;

	private static Dictionary<string, ModCompilationCache> mod_compilation_caches;

	private static readonly Dictionary<string, long> mod_last_update_timestamps = new Dictionary<string, long>();

	public static void InitializeModCompileCache()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (!File.Exists(Paths.ModCompileRecordPath))
		{
			File.WriteAllText(Paths.ModCompileRecordPath, "{}");
		}
		string text = File.ReadAllText(Paths.ModCompileRecordPath);
		JsonSerializerSettings val = new JsonSerializerSettings
		{
			ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
			Formatting = (Formatting)1
		};
		try
		{
			mod_compilation_caches = JsonConvert.DeserializeObject<Dictionary<string, ModCompilationCache>>(text, val) ?? new Dictionary<string, ModCompilationCache>();
		}
		catch (Exception)
		{
			mod_compilation_caches = new Dictionary<string, ModCompilationCache>();
		}
		finally
		{
			if (mod_compilation_caches == null)
			{
				mod_compilation_caches = new Dictionary<string, ModCompilationCache>();
			}
		}
		if (!File.Exists(Paths.ModsDisabledRecordPath))
		{
			return;
		}
		List<string> list = new List<string>(File.ReadAllLines(Paths.ModsDisabledRecordPath));
		foreach (string item in list)
		{
			if (!mod_compilation_caches.ContainsKey(item))
			{
				mod_compilation_caches[item] = new ModCompilationCache(item);
				mod_compilation_caches[item].disabled = true;
			}
			else
			{
				mod_compilation_caches[item].disabled = true;
			}
		}
		File.Delete(Paths.ModsDisabledRecordPath);
	}

	public static string TryToUnzipModZip(string pZipFile)
	{
		string text = Path.Combine(Application.temporaryCachePath, Path.GetFileNameWithoutExtension(pZipFile));
		if (Directory.Exists(text))
		{
			Directory.Delete(text, recursive: true);
		}
		try
		{
			ZipFile.ExtractToDirectory(pZipFile, text);
		}
		catch (Exception ex)
		{
			if (Directory.Exists(text))
			{
				Directory.Delete(text, recursive: true);
			}
			LogService.LogError("Error occurs when extracting " + pZipFile);
			LogService.LogError(ex.Message);
			LogService.LogError(ex.StackTrace);
			return "";
		}
		List<string> list = SystemUtils.SearchFileRecursive(text, (string filename) => filename == Paths.ModDeclarationFileName, (string dirname) => true);
		if (list.Count == 0)
		{
			Directory.Delete(text, recursive: true);
			return "";
		}
		if (list.Count > 1)
		{
			LogService.LogWarning("More than one mod.json file in " + pZipFile + ", only load the first one");
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pZipFile);
		try
		{
			ModDeclare modDeclare = new ModDeclare(list[0]);
			fileNameWithoutExtension = modDeclare.UID;
		}
		catch (Exception)
		{
			return "";
		}
		try
		{
			SystemUtils.CopyDirectory(Path.GetDirectoryName(list[0]), Path.Combine(Paths.ModsPath, fileNameWithoutExtension));
			return Path.Combine(Paths.ModsPath, fileNameWithoutExtension);
		}
		catch (UnauthorizedAccessException)
		{
			ZipFile.ExtractToDirectory(pZipFile, Path.Combine(Paths.ModsPath, Path.GetFileNameWithoutExtension(pZipFile)));
		}
		finally
		{
			try
			{
				File.Delete(pZipFile);
				if (Directory.Exists(text))
				{
					Directory.Delete(text, recursive: true);
				}
			}
			catch (Exception)
			{
			}
		}
		return "";
	}

	public static void CheckModsFolder(string pFolderPath, HashSet<string> pFindModsIDs, List<ModDeclare> pModsToFill, bool pLogModJsonNotFound = true)
	{
		if (!Directory.Exists(pFolderPath))
		{
			return;
		}
		IEnumerable<string> enumerable = new HashSet<string>(Directory.GetFiles(pFolderPath, "*.zip")).Union(Directory.GetFiles(pFolderPath, "*.7z")).Union(Directory.GetFiles(pFolderPath, "*.rar")).Union(Directory.GetFiles(pFolderPath, "*.tar"))
			.Union(Directory.GetFiles(pFolderPath, "*.tar.gz"))
			.Union(Directory.GetFiles(pFolderPath, "*.mod"));
		foreach (string item in enumerable)
		{
			TryToUnzipModZip(item);
		}
		string[] directories = Directory.GetDirectories(pFolderPath);
		string[] array = directories;
		foreach (string pModFolderPath in array)
		{
			ModDeclare modDeclare = recogMod(pModFolderPath, pLogModJsonNotFound);
			if (modDeclare != null)
			{
				if (pFindModsIDs.Contains(modDeclare.UID))
				{
					LogService.LogWarning("Repeat Mod with " + modDeclare.UID + ", Only load one of them");
					continue;
				}
				pModsToFill.Add(modDeclare);
				pFindModsIDs.Add(modDeclare.UID);
			}
		}
	}

	public static List<ModDeclare> findAndPrepareMods()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Invalid comparison between Unknown and I4
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		HashSet<string> hashSet = new HashSet<string>();
		List<ModDeclare> list = new List<ModDeclare>();
		if (!NCMSHere())
		{
			CheckModsFolder(Paths.ModsPath, hashSet, list);
		}
		CheckModsFolder(Paths.NativeModsPath, hashSet, list, pLogModJsonNotFound: false);
		if (!Others.is_editor)
		{
			string[] directories;
			try
			{
				RuntimePlatform platform = Application.platform;
				RuntimePlatform val = platform;
				if (val - 1 <= 1 || (int)val == 13)
				{
					directories = Directory.GetDirectories(Paths.CommonModsWorkshopPath);
				}
				else
				{
					LogService.LogWarning("Your platform " + ((object)Application.platform/*cast due to constrained. prefix*/).ToString() + " doesn't have defined behaviour, trying to handle it like Windows...");
					directories = Directory.GetDirectories(Paths.CommonModsWorkshopPath);
				}
			}
			catch (DirectoryNotFoundException)
			{
				LogService.LogWarning("Workshop folder not found, skip loading workshop mods");
				goto IL_0199;
			}
			string[] array = directories;
			foreach (string text in array)
			{
				ModDeclare modDeclare = recogMod(text, pLogModJsonNotFound: false);
				if (modDeclare == null)
				{
					continue;
				}
				if (modDeclare.ModType == ModTypeEnum.NEOMOD)
				{
					if (hashSet.Contains(modDeclare.UID))
					{
						LogService.LogWarning("Repeat Mod with " + modDeclare.UID + ", Only load one of them");
						continue;
					}
					if (string.IsNullOrEmpty(modDeclare.RepoUrl))
					{
						modDeclare.SetRepoUrlToWorkshopPage(Path.GetFileName(text));
					}
					list.Add(modDeclare);
					hashSet.Add(modDeclare.UID);
				}
				else if (modDeclare.ModType == ModTypeEnum.BEPINEX)
				{
					LinkBepInExModToLocalRequest(modDeclare);
				}
			}
		}
		goto IL_0199;
		IL_0199:
		foreach (ModDeclare item in list)
		{
			WorldBoxMod.AllRecognizedMods[item] = ModState.FAILED;
		}
		return removeDisabledMods(list);
		static bool NCMSHere()
		{
			return false;
		}
	}

	private static List<ModDeclare> removeDisabledMods(List<ModDeclare> mods_to_process)
	{
		List<ModDeclare> list = new List<ModDeclare>();
		foreach (ModDeclare item in mods_to_process)
		{
			if (isModDisabled(item.UID))
			{
				WorldBoxMod.AllRecognizedMods[item] = ModState.DISABLED;
			}
			else
			{
				list.Add(item);
			}
		}
		return list;
	}

	internal static void DealWithBepInExModLinkRequests()
	{
		if (link_request_mods.Count != 0)
		{
			InformationWindow.ShowWindow(LM.Get("ModLinkRequest"), InstallBepInExMod);
		}
	}

	private static void InstallBepInExMod()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		if (to_install_bepinex)
		{
			try
			{
				InstallBepInEx();
			}
			catch (Exception ex)
			{
				LogService.LogError(ex.Message);
				LogService.LogError(ex.StackTrace);
				return;
			}
			to_install_bepinex = false;
		}
		if (!Directory.Exists(Paths.BepInExPluginsPath))
		{
			Directory.CreateDirectory(Paths.BepInExPluginsPath);
		}
		List<string> list = new List<string>();
		RuntimePlatform platform = Application.platform;
		RuntimePlatform val = platform;
		if ((int)val != 1)
		{
			if ((int)val == 2)
			{
				list.Add("/c");
				while (link_request_mods.Count > 0)
				{
					ModDeclare modDeclare = link_request_mods.Dequeue();
					if (list.Count != 1)
					{
						list.Add("&&");
					}
					list.Add("mklink");
					list.Add("/D");
					list.Add("\"" + Path.Combine(Paths.BepInExPluginsPath, modDeclare.Name) + "\"");
					list.Add("\"" + modDeclare.FolderPath + "\"");
				}
				SystemUtils.CmdRunAs(list.ToArray());
				return;
			}
			if ((int)val != 13)
			{
				return;
			}
		}
		list.Add("-c");
		while (link_request_mods.Count > 0)
		{
			ModDeclare modDeclare2 = link_request_mods.Dequeue();
			if (list.Count != 1)
			{
				list.Add("&&");
			}
			list.Add("ln");
			list.Add("-s");
			list.Add("\"" + modDeclare2.FolderPath + "\"");
			list.Add("\"" + Path.Combine(Paths.BepInExPluginsPath, modDeclare2.Name) + "\"");
		}
		SystemUtils.BashRun(list.ToArray());
	}

	private static void InstallBepInEx()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Invalid comparison between Unknown and I4
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Invalid comparison between Unknown and I4
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		WebClient webClient = new WebClient();
		string text = Path.Combine(Path.GetTempPath(), "bepinex.zip");
		RuntimePlatform platform = Application.platform;
		if (1 == 0)
		{
		}
		string text2 = (((int)platform == 1) ? "https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_unix_5.4.22.0.zip" : (((int)platform == 2) ? "https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x64_5.4.22.0.zip" : (((int)platform != 13) ? "https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x64_5.4.22.0.zip" : "https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_unix_5.4.22.0.zip")));
		if (1 == 0)
		{
		}
		string address = text2;
		webClient.DownloadFile(address, text);
		try
		{
			ZipFile.ExtractToDirectory(text, Paths.GamePath);
		}
		catch (Exception)
		{
		}
		File.Delete(text);
		RuntimePlatform platform2 = Application.platform;
		RuntimePlatform val = platform2;
		if ((int)val == 1 || (int)val == 13)
		{
			string text3 = Path.Combine(Paths.GamePath, "run_bepinex.sh");
			string text4 = "";
			string[] files = Directory.GetFiles(Paths.GamePath);
			foreach (string fileName in files)
			{
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Name.Contains("worldbox"))
				{
					text4 = fileInfo.Name;
					break;
				}
			}
			if (string.IsNullOrEmpty(text4))
			{
				LogService.LogErrorConcurrent("Failed to find WorldBox executable file!");
				LogService.LogWarningConcurrent("Set it as \"worldbox\" automatically");
				text4 = "worldbox";
			}
			string text5 = File.ReadAllText(text3);
			text5 = text5.Replace("executable_name=\"\"", "executable_name=\"" + text4 + "\"");
			File.WriteAllText(text3, text5);
			if ((int)Application.platform == 13)
			{
				string linuxSteamLocalConfigPath = Paths.LinuxSteamLocalConfigPath;
				SteamId steamId = SteamClient.SteamId;
				string path = string.Format(linuxSteamLocalConfigPath, ((SteamId)(ref steamId)).AccountId.ToString());
				VProperty val2 = VdfConvert.Deserialize(File.ReadAllText(path));
				val2.Value[(object)"Software"][(object)"Valve"][(object)"Steam"][(object)"apps"][(object)1206560uL.ToString()][(object)"LaunchOptions"] = (VToken)new VValue((object)(text3 + " %command%"));
				File.WriteAllText(path, VdfConvert.Serialize((VToken)(object)val2));
			}
			else
			{
				LogService.LogWarningConcurrent("You are using macOS, please add launch script manually");
			}
			SystemUtils.BashRun(new string[4] { "-c", "chmod", "u+x", text3 });
		}
		LogService.LogInfo("Install BepInEx to " + Paths.GamePath);
	}

	internal static void LinkBepInExModToLocalRequest(ModDeclare mod)
	{
		if (!Directory.Exists(Paths.BepInExPluginsPath))
		{
			LogService.LogInfo("Find a BepInEx mod " + mod.Name + " but BepInEx not found, Add Install BepInEx Task into queue");
			to_install_bepinex = true;
		}
		bool flag = false;
		foreach (IMod loadedMod in WorldBoxMod.LoadedMods)
		{
			if (loadedMod.GetDeclaration().UID == mod.UID)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			link_request_mods.Enqueue(mod);
		}
	}

	public static ModDeclare recogMod(string pModFolderPath, bool pLogModJsonNotFound = true)
	{
		string text = Path.Combine(pModFolderPath, Paths.ModDeclarationFileName);
		if (!File.Exists(text))
		{
			List<string> list = SystemUtils.SearchFileRecursive(pModFolderPath, (string file_name) => file_name == Paths.ModDeclarationFileName, (string _) => true);
			if (list.Count == 0)
			{
				if (pLogModJsonNotFound)
				{
					LogService.LogWarning("No mod.json file for folder " + pModFolderPath + " in Mods");
				}
				return null;
			}
			if (list.Count > 1)
			{
				LogService.LogWarning("More than one mod.json file in mod folder, only load the first one at '" + list[0] + "'");
			}
			text = list[0];
		}
		try
		{
			return new ModDeclare(text);
		}
		catch (Exception ex)
		{
			LogService.LogError("Error occurs when loading mod config file " + text);
			LogService.LogError(ex.Message);
			LogService.LogError(ex.StackTrace);
			return null;
		}
	}

	public static List<ModDeclare> recogBepInExMods()
	{
		List<ModDeclare> list = new List<ModDeclare>();
		if (!Directory.Exists(Paths.BepInExPluginsPath))
		{
			return list;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(Paths.BepInExPluginsPath);
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		HashSet<string> hashSet = new HashSet<string>();
		DirectoryInfo[] array = directories;
		foreach (DirectoryInfo directoryInfo2 in array)
		{
			FileInfo[] files;
			try
			{
				files = directoryInfo2.GetFiles("*.dll");
			}
			catch (DirectoryNotFoundException)
			{
				continue;
			}
			if (files.Length != 0)
			{
				hashSet.Add(files[0].FullName);
			}
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		Assembly[] array2 = assemblies;
		foreach (Assembly assembly in array2)
		{
			string location;
			try
			{
				location = assembly.Location;
			}
			catch (NotSupportedException)
			{
				continue;
			}
			if (!hashSet.Contains(location))
			{
				continue;
			}
			string directoryName = Path.GetDirectoryName(location);
			ModDeclare modDeclare = recogBepInExMod(directoryName, assembly);
			if (modDeclare != null)
			{
				if (File.Exists(Path.Combine(directoryName, "icon.png")))
				{
					modDeclare.SetIconPath(Path.Combine(directoryName, "icon.png"));
				}
				list.Add(modDeclare);
			}
		}
		return list;
	}

	public static ModDeclare recogBepInExMod(string folder, Assembly pAssembly)
	{
		AssemblyName[] referencedAssemblies = pAssembly.GetReferencedAssemblies();
		bool flag = false;
		LogService.LogWarning("Checking " + pAssembly.FullName);
		AssemblyName[] array = referencedAssemblies;
		foreach (AssemblyName assemblyName in array)
		{
			if (!(assemblyName.FullName != "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return null;
		}
		string pName = pAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
		string pAuthor = pAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
		string text = pAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version;
		if (string.IsNullOrEmpty(text))
		{
			text = pAssembly.GetName().Version.ToString();
		}
		string pDescription = pAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
		ModDeclare modDeclare = new ModDeclare(pName, pAuthor, null, text, pDescription, folder, null, null, null);
		modDeclare.SetModType(ModTypeEnum.BEPINEX);
		return modDeclare;
	}

	public static bool isModDisabled(string pModUID)
	{
		ModCompilationCache value;
		return mod_compilation_caches.TryGetValue(pModUID, out value) && value.disabled;
	}

	public static bool toggleMod(string pModUID, bool pSave = true)
	{
		if (!mod_compilation_caches.TryGetValue(pModUID, out var value))
		{
			value = new ModCompilationCache(pModUID);
			value.disabled = true;
			mod_compilation_caches[pModUID] = value;
			return false;
		}
		bool disabled = value.disabled;
		value.disabled = !value.disabled;
		if (pSave)
		{
			SaveModRecords();
		}
		return disabled;
	}

	public static void SaveModRecords()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		JsonSerializerSettings val = new JsonSerializerSettings
		{
			ContractResolver = (IContractResolver)new DefaultContractResolver(),
			Formatting = (Formatting)1
		};
		string contents = JsonConvert.SerializeObject((object)mod_compilation_caches, val);
		File.WriteAllText(Paths.ModCompileRecordPath, contents);
	}

	public static void RecordMod(ModDeclare pModDeclare, List<string> pDependencies, List<string> pOptionalDependencies, bool pDisabled = false, bool pSave = true)
	{
		if (!mod_compilation_caches.TryGetValue(pModDeclare.UID, out var value))
		{
			value = new ModCompilationCache(pModDeclare, pDependencies, pOptionalDependencies);
		}
		else
		{
			value.dependencies = new List<string>(pDependencies);
			value.optional_dependencies = new List<string>(pOptionalDependencies);
		}
		value.disabled = pDisabled;
		value.timestamp = getModNewestUpdateTimestamp(pModDeclare.FolderPath);
		mod_compilation_caches[pModDeclare.UID] = value;
		if (pSave)
		{
			SaveModRecords();
		}
	}

	public static bool doesModNeedRecompile(ModDeclare pModDeclare, List<string> pDependencies, List<string> pOptionalDependencies)
	{
		if (!mod_compilation_caches.TryGetValue(pModDeclare.UID, out var value))
		{
			return true;
		}
		if (!File.Exists(Path.Combine(Paths.CompiledModsPath, pModDeclare.UID)))
		{
			return true;
		}
		HashSet<string> hashSet = new HashSet<string>(pDependencies);
		HashSet<string> hashSet2 = new HashSet<string>(value.dependencies);
		if (!hashSet.SetEquals(hashSet2))
		{
			return true;
		}
		hashSet = new HashSet<string>(pOptionalDependencies);
		hashSet2 = new HashSet<string>(value.optional_dependencies);
		if (!hashSet.SetEquals(hashSet2))
		{
			return true;
		}
		long timestamp = value.timestamp;
		bool flag = timestamp < 100000000 + getModNewestUpdateTimestamp(pModDeclare.FolderPath);
		if (flag)
		{
			return true;
		}
		foreach (string pDependency in pDependencies)
		{
			flag |= timestamp < 100000000 + getModLastCompileTimestamp(pDependency);
			if (flag)
			{
				return true;
			}
		}
		foreach (string pOptionalDependency in pOptionalDependencies)
		{
			flag |= timestamp < 100000000 + getModLastCompileTimestamp(pOptionalDependency);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	public static void clearModCompileTimestamp(string pModUUID, bool pSave = true)
	{
		if (!mod_compilation_caches.TryGetValue(pModUUID, out var value))
		{
			value = new ModCompilationCache(pModUUID);
			value.disabled = false;
			value.timestamp = 0L;
			mod_compilation_caches[pModUUID] = value;
		}
		else
		{
			value.timestamp = 0L;
			if (pSave)
			{
				SaveModRecords();
			}
		}
	}

	private static long getModLastCompileTimestamp(string pModUID)
	{
		ModCompilationCache value;
		return mod_compilation_caches.TryGetValue(pModUID, out value) ? value.timestamp : 0;
	}

	private static long getModNewestUpdateTimestamp(string pModFolderPath)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(pModFolderPath);
		if (mod_last_update_timestamps.ContainsKey(directoryInfo.FullName))
		{
			return mod_last_update_timestamps[directoryInfo.FullName];
		}
		List<string> source = SystemUtils.SearchFileRecursive(directoryInfo.FullName, (string filename) => !filename.StartsWith("."), (string dirname) => !dirname.StartsWith(".") && !Paths.IgnoreSearchDirectories.Contains(dirname));
		long num = (from filepath in source
			select new FileInfo(filepath) into file_info
			select Math.Max(file_info.CreationTimeUtc.Ticks, file_info.LastWriteTimeUtc.Ticks)).Prepend(Math.Max(directoryInfo.CreationTimeUtc.Ticks, directoryInfo.LastWriteTimeUtc.Ticks)).Prepend(InternalResourcesGetter.GetLastWriteTime()).Max();
		mod_last_update_timestamps[directoryInfo.FullName] = num;
		return num;
	}
}
