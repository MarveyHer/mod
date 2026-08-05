using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NeoModLoader.api;
using NeoModLoader.constants;
using NeoModLoader.General;
using NeoModLoader.General.Event;
using NeoModLoader.General.UI.Tab;
using NeoModLoader.ncms_compatible_layer;
using NeoModLoader.services;
using NeoModLoader.ui;
using NeoModLoader.utils;
using UnityEngine;

namespace NeoModLoader;

public class WorldBoxMod : MonoBehaviour
{
	public static List<IMod> LoadedMods = new List<IMod>();

	internal static Dictionary<ModDeclare, ModState> AllRecognizedMods = new Dictionary<ModDeclare, ModState>();

	internal static Transform Transform;

	internal static Transform InactiveTransform;

	internal static Assembly NeoModLoaderAssembly = Assembly.GetExecutingAssembly();

	private bool initialized = false;

	private bool initialized_successfully = false;

	private void Start()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Others.unity_player_enabled = true;
		Transform = ((Component)this).transform;
		InactiveTransform = new GameObject("Inactive").transform;
		InactiveTransform.SetParent(Transform);
		((Component)InactiveTransform).gameObject.SetActive(false);
		LogService.Init();
		fileSystemInitialize();
		LogService.LogInfo("NeoModLoader Version: " + InternalResourcesGetter.GetCommit());
	}

	private void Update()
	{
		if (!Config.game_loaded)
		{
			return;
		}
		if (initialized_successfully)
		{
			TabManager._checkNewTabs();
		}
		if (initialized)
		{
			return;
		}
		initialized = true;
		ModUploadAuthenticationService.AutoAuth();
		HarmonyUtils._init();
		Harmony.CreateAndPatchAll(typeof(LM), "wbom.nml");
		Harmony.CreateAndPatchAll(typeof(ResourcesPatch), "wbom.nml");
		Harmony.CreateAndPatchAll(typeof(CustomAudioManager), "wbom.nml");
		if (!SmoothLoader.isLoading())
		{
			SmoothLoader.prepare();
		}
		SmoothLoader.add((MapLoaderAction)delegate
		{
			ResourcesPatch.Initialize();
			LoadLocales();
			LM.ApplyLocale();
			TabManager._init();
			WindowCreator.init();
			ListenerManager._init();
			WrappedPowersTab._init();
			NCMSCompatibleLayer.PreInit();
			ModInfoUtils.InitializeModCompileCache();
		}, "Initialize NeoModLoader", false, 0.001f);
		List<ModDependencyNode> mod_nodes = new List<ModDependencyNode>();
		SmoothLoader.add((MapLoaderAction)delegate
		{
			ModCompileLoadService.loadInfoOfBepInExPlugins();
			List<ModDeclare> mods = ModInfoUtils.findAndPrepareMods();
			mod_nodes.AddRange(ModDepenSolveService.SolveModDependencies(mods));
			ModCompileLoadService.prepareCompile(mod_nodes);
		}, "Load Mods Info And Prepare Mods", false, 0.001f);
		SmoothLoader.add((MapLoaderAction)delegate
		{
			List<ModDeclare> mods_to_load = new List<ModDeclare>();
			foreach (ModDependencyNode mod in mod_nodes)
			{
				SmoothLoader.add((MapLoaderAction)delegate
				{
					if (ModCompileLoadService.compileMod(mod))
					{
						mods_to_load.Add(mod.mod_decl);
					}
					else
					{
						LogService.LogError("Failed to compile mod " + mod.mod_decl.Name);
					}
				}, "Compile Mod " + mod.mod_decl.Name, false, 0.001f);
			}
			foreach (ModDependencyNode mod2 in mod_nodes)
			{
				SmoothLoader.add((MapLoaderAction)delegate
				{
					if (mods_to_load.Contains(mod2.mod_decl))
					{
						ResourcesPatch.LoadResourceFromFolder(Path.Combine(mod2.mod_decl.FolderPath, Paths.ModResourceFolderName));
						ResourcesPatch.LoadResourceFromFolder(Path.Combine(mod2.mod_decl.FolderPath, Paths.NCMSAdditionModResourceFolderName));
						ResourcesPatch.LoadAssetBundlesFromFolder(Path.Combine(mod2.mod_decl.FolderPath, Paths.ModAssetBundleFolderName));
					}
				}, "Load Resources From Mod " + mod2.mod_decl.Name, false, 0.001f);
			}
			SmoothLoader.add((MapLoaderAction)delegate
			{
				ModCompileLoadService.loadMods(mods_to_load);
				ModInfoUtils.SaveModRecords();
				NCMSCompatibleLayer.Init();
				Dictionary<IMod, bool> successfulInit = new Dictionary<IMod, bool>();
				foreach (IMod mod3 in LoadedMods.Where((IMod mod5) => mod5 is IStagedLoad))
				{
					SmoothLoader.add((MapLoaderAction)delegate
					{
						successfulInit.Add(mod3, ModCompileLoadService.TryInitMod(mod3));
					}, "Init Mod " + mod3.GetDeclaration().Name, false, 0.001f);
				}
				foreach (IMod mod4 in LoadedMods.Where((IMod mod5) => mod5 is IStagedLoad))
				{
					SmoothLoader.add((MapLoaderAction)delegate
					{
						if (successfulInit.ContainsKey(mod4) && successfulInit[mod4])
						{
							ModCompileLoadService.PostInitMod(mod4);
						}
					}, "Post-Init Mod " + mod4.GetDeclaration().Name, false, 0.001f);
				}
			}, "Load Mods", false, 0.001f);
			SmoothLoader.add((MapLoaderAction)ResourcesPatch.PatchSomeResources, "Patch part of Resources into game", false, 0.001f);
			SmoothLoader.add((MapLoaderAction)delegate
			{
				ModWorkshopService.Init();
				UIManager.init();
				ModInfoUtils.DealWithBepInExModLinkRequests();
				LM.ApplyLocale();
				initialized_successfully = true;
			}, "NeoModLoader Post Initialize", false, 0.001f);
			SmoothLoader.add((MapLoaderAction)ExternalModInstallService.CheckExternalModInstall, "Check External Mods to Install", false, 0.001f);
		}, "Compile Mods And Load resources", false, 0.001f);
	}

	private void LoadLocales()
	{
		string[] manifestResourceNames = NeoModLoaderAssembly.GetManifestResourceNames();
		string text = "NeoModLoader.resources.locales.";
		string[] array = manifestResourceNames;
		foreach (string text2 in array)
		{
			if (text2.StartsWith(text))
			{
				LM.LoadLocale(text2.Replace(text, "").Replace(".json", ""), NeoModLoaderAssembly.GetManifestResourceStream(text2));
			}
		}
	}

	private void fileSystemInitialize()
	{
		if (!Directory.Exists(Paths.ModsPath))
		{
			Directory.CreateDirectory(Paths.ModsPath);
			LogService.LogInfo("Create Mods folder at " + Paths.ModsPath);
		}
		if (!Directory.Exists(Paths.CompiledModsPath))
		{
			Directory.CreateDirectory(Paths.CompiledModsPath);
			LogService.LogInfo("Create CompiledMods folder at " + Paths.CompiledModsPath);
		}
		if (!Directory.Exists(Paths.ModsConfigPath))
		{
			Directory.CreateDirectory(Paths.ModsConfigPath);
			LogService.LogInfo("Create mods_config folder at " + Paths.ModsConfigPath);
		}
		if (!File.Exists(Paths.ModCompileRecordPath))
		{
			File.Create(Paths.ModCompileRecordPath).Close();
			LogService.LogInfo("Create mod_compile_records.json at " + Paths.ModCompileRecordPath);
		}
		if (!Directory.Exists(Paths.NMLAssembliesPath))
		{
			Directory.CreateDirectory(Paths.NMLAssembliesPath);
			LogService.LogInfo("Create NMLAssemblies folder at " + Paths.NMLAssembliesPath);
			extractAssemblies();
		}
		else
		{
			DateTime lastWriteTime = new FileInfo(Paths.NMLModPath).LastWriteTime;
			DateTime creationTime = new DirectoryInfo(Paths.NMLAssembliesPath).CreationTime;
			if (lastWriteTime > creationTime)
			{
				LogService.LogInfo("NeoModLoader.dll is newer than assemblies in NMLAssemblies folder, re-extract assemblies from NeoModLoader.dll");
				Debug.Log((object)Paths.NMLAssembliesPath);
				Directory.Delete(Paths.NMLAssembliesPath, recursive: true);
				Directory.CreateDirectory(Paths.NMLAssembliesPath);
				LogService.LogInfo("Create new NMLAssemblies folder at " + Paths.NMLAssembliesPath);
				extractAssemblies();
			}
		}
		try
		{
			using Stream stream = NeoModLoaderAssembly.GetManifestResourceStream("NeoModLoader.resources.assemblies.Assembly-CSharp-Publicized.dll");
			if (File.Exists(Paths.PublicizedAssemblyPath))
			{
				DateTime lastWriteTime2 = new FileInfo(Paths.NMLModPath).LastWriteTime;
				DateTime creationTime2 = new FileInfo(Paths.PublicizedAssemblyPath).CreationTime;
				if (lastWriteTime2 > creationTime2)
				{
					LogService.LogInfo("NeoModLoader.dll is newer than Assembly-CSharp-Publicized.dll, re-extract Assembly-CSharp-Publicized.dll from NeoModLoader.dll");
					File.Delete(Paths.PublicizedAssemblyPath);
					using FileStream destination = new FileStream(Paths.PublicizedAssemblyPath, FileMode.Create, FileAccess.Write);
					stream.CopyTo(destination);
				}
			}
			else
			{
				using FileStream destination2 = new FileStream(Paths.PublicizedAssemblyPath, FileMode.CreateNew, FileAccess.Write);
				stream.CopyTo(destination2);
			}
		}
		catch (UnauthorizedAccessException)
		{
			File.Delete(Paths.PublicizedAssemblyPath);
			using Stream stream2 = NeoModLoaderAssembly.GetManifestResourceStream("NeoModLoader.resources.assemblies.Assembly-CSharp-Publicized.dll");
			using FileStream destination3 = new FileStream(Paths.PublicizedAssemblyPath, FileMode.CreateNew, FileAccess.Write);
			stream2.CopyTo(destination3);
		}
		string[] files = Directory.GetFiles(Paths.NMLAssembliesPath, "*.dll");
		foreach (string text in files)
		{
			try
			{
				Assembly.LoadFrom(text);
			}
			catch (BadImageFormatException)
			{
				LogService.LogError("BadImageFormatException: The file " + text + " is not a valid assembly.");
			}
			catch (Exception ex3)
			{
				LogService.LogError("Exception: Failed to load assembly " + text + ".");
				LogService.LogError(ex3.Message);
				LogService.LogError(ex3.StackTrace);
			}
		}
		File.WriteAllText(Paths.NMLCommitPath, InternalResourcesGetter.GetCommit());
		if (File.Exists(Paths.NMLAutoUpdateModulePath))
		{
			FileInfo fileInfo = new FileInfo(Paths.NMLAutoUpdateModulePath);
			if (fileInfo.LastWriteTimeUtc.Ticks < InternalResourcesGetter.GetLastWriteTime())
			{
				try
				{
					fileInfo.Delete();
					LogService.LogInfo("NeoModLoader.dll is newer than AutoUpdate.dll, re-extract AutoUpdate.dll from NeoModLoader.dll");
				}
				catch (Exception)
				{
				}
			}
		}
		if (File.Exists(Paths.NMLAutoUpdateModulePath))
		{
			return;
		}
		using (Stream stream3 = NeoModLoaderAssembly.GetManifestResourceStream("NeoModLoader.resources.assemblies.NeoModLoader.AutoUpdate.dll"))
		{
			using FileStream destination4 = new FileStream(Paths.NMLAutoUpdateModulePath, FileMode.CreateNew, FileAccess.Write);
			stream3.CopyTo(destination4);
		}
		static void extractAssemblies()
		{
			string[] manifestResourceNames = NeoModLoaderAssembly.GetManifestResourceNames();
			string[] array = manifestResourceNames;
			foreach (string text2 in array)
			{
				if (text2.EndsWith(".dll") && !text2.Contains("Assembly-CSharp-Publicized") && !text2.Contains("AutoUpdate"))
				{
					string path = text2.Replace("NeoModLoader.resources.assemblies.", "");
					string path2 = Path.Combine(Paths.NMLAssembliesPath, path).Replace("-renamed", "");
					using Stream stream4 = NeoModLoaderAssembly.GetManifestResourceStream(text2);
					using FileStream destination5 = new FileStream(path2, FileMode.Create, FileAccess.Write);
					stream4.CopyTo(destination5);
				}
			}
		}
	}
}
