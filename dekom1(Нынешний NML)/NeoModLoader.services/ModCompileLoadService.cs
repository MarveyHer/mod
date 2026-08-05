using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using ModDeclaration;
using NCMS;
using NeoModLoader.api;
using NeoModLoader.constants;
using NeoModLoader.General;
using NeoModLoader.ncms_compatible_layer;
using NeoModLoader.utils;
using NeoModLoader.utils.Builders;
using UnityEngine;

namespace NeoModLoader.services;

public static class ModCompileLoadService
{
	private static string[] _default_ref_path = null;

	private static readonly Dictionary<string, string> mod_inc_path = new Dictionary<string, string>();

	private static readonly HashSet<string> _loaded_ref = new HashSet<string>();

	private static MetadataReference[] _default_ref = null;

	private static MetadataReference _publicized_assembly_ref = null;

	private static readonly Dictionary<string, MetadataReference> mod_ref = new Dictionary<string, MetadataReference>();

	private static bool compileMod(ModDeclare pModDecl, IEnumerable<MetadataReference> pDefaultInc, string[] pAddInc, Dictionary<string, MetadataReference> pModInc, bool pForce = false, bool pDisableOptionalDepen = false)
	{
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Expected O, but got Unknown
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Expected O, but got Unknown
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Expected O, but got Unknown
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Expected O, but got Unknown
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Invalid comparison between Unknown and I4
		List<string> list = (pDisableOptionalDepen ? new List<string>() : pModDecl.OptionalDependencies.Where(pModInc.ContainsKey).ToList());
		List<string> list2 = pModDecl.Dependencies.Where(pModInc.ContainsKey).ToList();
		if (!pForce && !ModInfoUtils.doesModNeedRecompile(pModDecl, list2, list))
		{
			LoadAddInc();
			return true;
		}
		List<string> list3 = new List<string>();
		List<MetadataReference> list4 = pDefaultInc.ToList();
		list4.AddRange((IEnumerable<MetadataReference>)pAddInc.Select(delegate(string inc)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			return MetadataReference.CreateFromFile(inc, default(MetadataReferenceProperties), (DocumentationProvider)null);
		}));
		LoadAddInc();
		if (pModDecl.UsePublicizedAssembly)
		{
			list4.Add(_publicized_assembly_ref);
		}
		foreach (string item3 in list2)
		{
			list4.Add(pModInc[item3]);
			if (pModInc[item3] != null)
			{
				continue;
			}
			LogService.LogError(pModDecl.UID + "'s optional ref of " + item3 + " instance is null");
			return false;
		}
		foreach (string item4 in list)
		{
			list4.Add(pModInc[item4]);
			list3.Add(ModDependencyUtils.ParseDepenNameToPreprocessSymbol(item4));
			if (pModInc[item4] != null)
			{
				continue;
			}
			LogService.LogError(pModDecl.UID + "'s optional ref of " + item4 + " instance is null");
			return false;
		}
		List<SyntaxTree> list5 = new List<SyntaxTree>();
		List<string> list6 = SystemUtils.SearchFileRecursive(pModDecl.FolderPath, (string file_name) => file_name.EndsWith(".cs") && !file_name.StartsWith("."), (string dir_name) => !dir_name.StartsWith(".") && !Paths.IgnoreSearchDirectories.Contains(dir_name));
		List<ResourceDescription> list7 = new List<ResourceDescription>();
		bool flag = false;
		CSharpParseOptions val = new CSharpParseOptions((LanguageVersion)int.MaxValue, (DocumentationMode)1, (SourceCodeKind)0, (IEnumerable<string>)list3);
		foreach (string item5 in list6)
		{
			SourceText val2 = SourceText.From(File.ReadAllText(item5), Encoding.UTF8, (SourceHashAlgorithm)1);
			SyntaxTree val3 = CSharpSyntaxTree.ParseText(val2, val, item5.Substring(pModDecl.FolderPath.Length + 1), default(CancellationToken));
			list5.Add(val3);
			if (!flag)
			{
				flag = NCMSCompatibleLayer.IsNCMSMod(val3);
			}
		}
		if (flag)
		{
			string text = Path.Combine(pModDecl.FolderPath, Paths.NCMSModEmbededResourceFolderName);
			if (Directory.Exists(text))
			{
				string[] files = Directory.GetFiles(text, "*", SearchOption.AllDirectories);
				string[] array = files;
				foreach (string file in array)
				{
					string text2 = file.Substring(text.Length + 1);
					string text3 = pModDecl.Name + ".Resources." + text2.Replace('\\', '.').Replace('/', '.');
					ResourceDescription item = new ResourceDescription(text3, (Func<Stream>)(() => File.OpenRead(file)), true);
					list7.Add(item);
				}
			}
			SourceText val4 = SourceText.From("\r\n    using System;\r\n    using System.IO;\r\n    using System.Reflection;\r\n    using UnityEngine;\r\n    using UnityEngine.Events;\r\n    using UnityEngine.UI;\r\n    using NeoModLoader.services;\r\n    using System.Collections.Generic;\r\n\r\n\r\n    internal class Mod\r\n    {\r\n        public static ModDeclaration.Info Info;\r\n        public static GameObject GameObject;\r\n        public static Action OnDebug;\r\n\r\n        private static int debugClicked = 0;\r\n\r\n        public static void Initialize(Button button)\r\n        {\r\n            OnDebug += new Action(() => { LogService.LogInfo($\"Debug toggled for mod {Info.Name}\"); });\r\n\r\n            button.onClick.AddListener(new UnityAction(() =>\r\n            {\r\n                if (debugClicked < 10)\r\n                {\r\n                    debugClicked++;\r\n                    return;\r\n                }\r\n\r\n                OnDebug();\r\n            }));\r\n        }\r\n\r\n        public class EmbededResources\r\n        {\r\n            private static Assembly this_assembly = Assembly.GetExecutingAssembly();\r\n\r\n            public static Sprite LoadSprite(string name, float pivotX = 0, float pivotY = 0, float pixelsPerUnit = 1f)\r\n            {\r\n                string hash = $\"{name}-{pivotX}-{pivotY}-{pixelsPerUnit}\";\r\n                if (sprite_cache.TryGetValue(hash, out var sprite))\r\n                    return sprite;\r\n                Texture2D texture2D = new Texture2D(0, 0);\r\n                texture2D.LoadImage(GetBytes(name));\r\n                texture2D.anisoLevel = 0;\r\n                texture2D.filterMode = FilterMode.Point;\r\n                sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, (float)texture2D.width, (float)texture2D.height),\r\n                    new Vector2(pivotX, pivotY), pixelsPerUnit);\r\n                sprite_cache.Add(hash, sprite);\r\n                return sprite;\r\n            }\r\n\r\n            private static Dictionary<string, Sprite> sprite_cache = new();\r\n\r\n            public static byte[] GetBytes(string name)\r\n            {\r\n                return ReadFully(this_assembly.GetManifestResourceStream(name));\r\n            }\r\n\r\n            internal static byte[] ReadFully(Stream input)\r\n            {\r\n                using var ms = new MemoryStream();\r\n                input.CopyTo(ms);\r\n                return ms.ToArray();\r\n            }\r\n        }\r\n    }", Encoding.UTF8, (SourceHashAlgorithm)1);
			SyntaxTree item2 = CSharpSyntaxTree.ParseText(val4, val, pModDecl.Name + ".GlobalObject.cs", default(CancellationToken));
			list5.Add(item2);
		}
		pModDecl.IsNCMSMod = flag;
		AssemblyIdentity val5 = new AssemblyIdentity(pModDecl.UID, pModDecl.ParseVersion(), (string)null, default(ImmutableArray<byte>), false, false, AssemblyContentType.Default);
		string obj = pModDecl.UID ?? "";
		AssemblyIdentityComparer val6 = AssemblyIdentityComparer.Default;
		CSharpCompilation val7 = CSharpCompilation.Create(obj, (IEnumerable<SyntaxTree>)list5, (IEnumerable<MetadataReference>)list4, new CSharpCompilationOptions((OutputKind)2, false, (string)null, (string)null, (string)null, (IEnumerable<string>)null, (OptimizationLevel)0, false, true, (string)null, (string)null, default(ImmutableArray<byte>), (bool?)null, (Platform)0, (ReportDiagnostic)0, 4, (IEnumerable<KeyValuePair<string, ReportDiagnostic>>)null, true, true, (XmlReferenceResolver)null, (SourceReferenceResolver)null, (MetadataReferenceResolver)null, val6, (StrongNameProvider)null, false, (MetadataImportOptions)0, (NullableContextOptions)0));
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using MemoryStream memoryStream2 = new MemoryStream();
			string path = Path.Combine(Paths.CompiledModsPath, pModDecl.UID + ".dll");
			string text4 = Path.Combine(Paths.CompiledModsPath, pModDecl.UID + ".pdb");
			EmitResult val8 = ((Compilation)val7).Emit((Stream)memoryStream, (Stream)memoryStream2, (Stream)null, (Stream)null, (IEnumerable<ResourceDescription>)list7, new EmitOptions(false, (DebugInformationFormat)2, text4, (string)null, 0, 0uL, false, default(SubsystemVersion), (string)null, false, true, default(ImmutableArray<InstrumentationKind>), (HashAlgorithmName?)null, (Encoding)null, (Encoding)null), (IMethodSymbol)null, (Stream)null, (IEnumerable<EmbeddedText>)null, (Stream)null, default(CancellationToken));
			if (!val8.Success)
			{
				StringBuilder stringBuilder = new StringBuilder();
				ImmutableArray<Diagnostic>.Enumerator enumerator4 = val8.Diagnostics.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					Diagnostic current4 = enumerator4.Current;
					if ((int)current4.Severity == 3)
					{
						stringBuilder.AppendLine(((object)current4).ToString());
					}
				}
				LogService.LogError(stringBuilder.ToString());
				return false;
			}
			using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			memoryStream.WriteTo(stream);
			using FileStream stream2 = new FileStream(text4, FileMode.Create, FileAccess.Write);
			memoryStream2.Seek(0L, SeekOrigin.Begin);
			memoryStream2.WriteTo(stream2);
			ModInfoUtils.RecordMod(pModDecl, list2, list, pDisabled: false, pSave: false);
			return true;
		}
		void LoadAddInc()
		{
			string[] array2 = pAddInc;
			foreach (string text5 in array2)
			{
				string fileName = Path.GetFileName(text5);
				if (!(fileName == "Assembly-CSharp.dll") && !_loaded_ref.Contains(fileName))
				{
					_loaded_ref.Add(fileName);
					try
					{
						Assembly assembly = Assembly.LoadFrom(text5);
						LogService.LogInfo("Load " + assembly.FullName);
					}
					catch (Exception ex)
					{
						LogService.LogWarning("Failed to load Assembly " + fileName + " for mod " + pModDecl.UID);
						LogService.LogWarning(ex.Message);
						LogService.LogWarning(ex.StackTrace);
					}
				}
			}
		}
	}

	public static void prepareCompile(List<ModDependencyNode> pModNodes)
	{
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		foreach (ModDependencyNode pModNode in pModNodes)
		{
			mod_inc_path.Add(pModNode.mod_decl.UID, Path.Combine(Paths.CompiledModsPath, pModNode.mod_decl.UID + ".dll"));
		}
		List<string> list = new List<string>();
		list.AddRange(Directory.GetFiles(Paths.ManagedPath, "*.dll"));
		list.AddRange(Directory.GetFiles(Paths.NMLAssembliesPath, "*.dll"));
		list.Add(Paths.NMLModPath);
		_default_ref_path = list.ToArray();
		_default_ref = (MetadataReference[])(object)new MetadataReference[_default_ref_path.Length];
		for (int i = 0; i < _default_ref_path.Length; i++)
		{
			try
			{
				_default_ref[i] = (MetadataReference)(object)MetadataReference.CreateFromFile(_default_ref_path[i], default(MetadataReferenceProperties), (DocumentationProvider)null);
				if (_default_ref[i] == null)
				{
					throw new Exception("Ref created is null");
				}
			}
			catch (Exception ex)
			{
				LogService.LogError("Error when load default reference " + _default_ref_path[i] + ": " + ex.Message);
			}
		}
		_publicized_assembly_ref = (MetadataReference)(object)MetadataReference.CreateFromFile(Paths.PublicizedAssemblyPath, default(MetadataReferenceProperties), (DocumentationProvider)null);
	}

	public static void prepareCompileRuntime(ModDependencyNode pModNode)
	{
		mod_inc_path.Add(pModNode.mod_decl.UID, Path.Combine(Paths.CompiledModsPath, pModNode.mod_decl.UID + ".dll"));
	}

	public static bool compileMod(ModDependencyNode pModNode, bool pForce = false)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (Directory.GetFiles(pModNode.mod_decl.FolderPath).Any((string file) => file.EndsWith(".dll")))
		{
			LogService.LogInfo(pModNode.mod_decl.UID + " detected as precompiled, compilation phase will be skipped on it!");
			pModNode.mod_decl.SetModType(ModTypeEnum.COMPILED_NEOMOD);
			return true;
		}
		bool flag = false;
		bool flag2 = false;
		while (true)
		{
			flag = compileMod(pModNode.mod_decl, _default_ref, pModNode.GetAdditionReferences(!flag2).ToArray(), mod_ref, pForce, flag2);
			if (flag)
			{
				mod_ref[pModNode.mod_decl.UID] = (MetadataReference)(object)MetadataReference.CreateFromFile(Path.Combine(Paths.CompiledModsPath, pModNode.mod_decl.UID + ".dll"), default(MetadataReferenceProperties), (DocumentationProvider)null);
				break;
			}
			if (flag2 || pModNode.mod_decl.OptionalDependencies.Length == 0)
			{
				break;
			}
			LogService.LogWarning("Cannot compile mod " + pModNode.mod_decl.UID + " with Optional Dependencies, try to disable them");
			flag2 = true;
		}
		if (!flag)
		{
			mod_inc_path.Remove(pModNode.mod_decl.UID);
			pModNode.mod_decl.FailReason.AppendLine("Compile Failed\n Check Log for details\n All mods compiled before it will be recompiled next time");
			File.WriteAllText(Paths.ModCompileRecordPath, "");
		}
		return flag;
	}

	public static void loadMods(List<ModDeclare> mods_to_load)
	{
		foreach (ModDeclare item in mods_to_load)
		{
			try
			{
				LoadMod(item);
			}
			catch (ReflectionTypeLoadException exception)
			{
				LogService.LogError("Compiled mod " + item.UID + " out of date, if it happens again after restarting game, please update, delete or unsubscribe it");
				LogService.LogException(exception);
				string path = Path.Combine(Paths.CompiledModsPath, item.UID + ".dll");
				string path2 = Path.Combine(Paths.CompiledModsPath, item.UID + ".pdb");
				try
				{
					if (File.Exists(path))
					{
						File.Delete(path);
					}
					if (File.Exists(path2))
					{
						File.Delete(path2);
					}
				}
				catch (Exception)
				{
				}
				ModInfoUtils.clearModCompileTimestamp(item.UID);
			}
		}
	}

	public static void LoadMod(ModDeclare pMod)
	{
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Expected O, but got Unknown
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Expected O, but got Unknown
		Assembly[] array;
		switch (pMod.ModType)
		{
		case ModTypeEnum.NEOMOD:
			array = new Assembly[1] { Assembly.Load(File.ReadAllBytes(Path.Combine(Paths.CompiledModsPath, pMod.UID + ".dll")), File.ReadAllBytes(Path.Combine(Paths.CompiledModsPath, pMod.UID + ".pdb"))) };
			break;
		case ModTypeEnum.COMPILED_NEOMOD:
		{
			string[] files = Directory.GetFiles(pMod.FolderPath, "*.dll");
			List<string> list = Directory.GetFiles(pMod.FolderPath, "*.pdb").ToList();
			array = new Assembly[files.Length];
			for (int i = 0; i < files.Length; i++)
			{
				string text = Path.GetFileName(files[i]).Replace(".dll", "");
				int num = list.IndexOf(Path.Combine(pMod.FolderPath, text + ".pdb"));
				if (num != -1)
				{
					array[i] = Assembly.Load(File.ReadAllBytes(files[i]), File.ReadAllBytes(list[num]));
					list.RemoveAt(num);
				}
				else
				{
					array[i] = Assembly.Load(File.ReadAllBytes(files[i]));
				}
			}
			break;
		}
		default:
			throw new ArgumentException("Cannot load mod of type " + pMod.ModType.ToString() + " with NML!");
		}
		Assembly[] array2 = array;
		foreach (Assembly assembly in array2)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				Attribute customAttribute = Attribute.GetCustomAttribute(type, typeof(ModEntry));
				if (!type.IsSubclassOf(typeof(MonoBehaviour)) || (type.GetInterface("IMod") == null && customAttribute == null) || type.IsAbstract)
				{
					continue;
				}
				GameObject val = new GameObject(pMod.Name);
				val.transform.parent = GameObject.Find("Services/ModLoader").transform;
				GameObject val2 = val;
				val2.SetActive(false);
				if (customAttribute != null)
				{
					pMod.IsNCMSMod = true;
					Type type2 = assembly.GetType("Mod");
					type2.GetField("Info")?.SetValue(null, new Info(NCMSCompatibleLayer.GenerateNCMSMod(pMod)));
					type2.GetField("GameObject")?.SetValue(null, val2);
				}
				IMod mod = null;
				try
				{
					MonoBehaviour val3 = null;
					if (type.GetInterface("IMod") == null)
					{
						mod = val2.AddComponent<AttachedModComponent>();
						val3 = (MonoBehaviour)val2.AddComponent(type);
					}
					else
					{
						mod = (IMod)val2.AddComponent(type);
						val3 = (MonoBehaviour)mod;
					}
					auto_localize(val3);
					mod.OnLoad(pMod, val2);
					val2.SetActive(true);
				}
				catch (Exception ex)
				{
					LogService.LogError(ex.Message);
					if (ex.StackTrace != null)
					{
						LogService.LogError(ex.StackTrace);
					}
					val2.SetActive(false);
					LogService.LogError(pMod.Name + " has been disabled due to an error. Please check the log for details.");
					continue;
				}
				WorldBoxMod.LoadedMods.Add(val2.GetComponent<IMod>());
				WorldBoxMod.AllRecognizedMods[pMod] = ModState.LOADED;
				break;
			}
			if (WorldBoxMod.AllRecognizedMods[pMod] != ModState.LOADED)
			{
				pMod.FailReason.AppendLine("No Valid Mod Component Found");
				ModInfoUtils.clearModCompileTimestamp(pMod.UID);
			}
		}
		void auto_localize(object mod_component)
		{
			if (mod_component is ILocalizable localizable)
			{
				string localeFilesDirectory = localizable.GetLocaleFilesDirectory(pMod);
				if (Directory.Exists(localeFilesDirectory))
				{
					string[] files2 = Directory.GetFiles(localeFilesDirectory, "*", SearchOption.AllDirectories);
					char pSep = ',';
					if (mod_component is ICsvSepCustomized csvSepCustomized)
					{
						pSep = csvSepCustomized.GetCsvSeparator();
					}
					string[] array3 = files2;
					foreach (string text2 in array3)
					{
						try
						{
							if (text2.EndsWith(".json"))
							{
								LM.LoadLocale(Path.GetFileNameWithoutExtension(text2), text2);
							}
							else if (text2.EndsWith(".csv"))
							{
								LM.LoadLocales(text2, pSep);
							}
						}
						catch (FormatException ex2)
						{
							LogService.LogWarning(ex2.Message);
						}
					}
					LM.ApplyLocale(pUpdateTexts: false);
				}
			}
		}
	}

	public static bool TryInitMod(IMod mod)
	{
		if (mod is IStagedLoad stagedLoad)
		{
			try
			{
				stagedLoad.Init();
			}
			catch (Exception ex)
			{
				LogService.LogError(ex.Message);
				if (ex.StackTrace != null)
				{
					LogService.LogError(ex.StackTrace);
				}
				mod.GetGameObject().SetActive(false);
				LogService.LogError(mod.GetDeclaration().Name + " has been disabled due to an init error. Please check the log for details.");
				return false;
			}
			return true;
		}
		return false;
	}

	public static void PostInitMod(IMod mod)
	{
		if (!(mod is IStagedLoad stagedLoad))
		{
			return;
		}
		try
		{
			stagedLoad.PostInit();
		}
		catch (Exception ex)
		{
			LogService.LogError(ex.Message);
			if (ex.StackTrace != null)
			{
				LogService.LogError(ex.StackTrace);
			}
			mod.GetGameObject().SetActive(false);
			LogService.LogError(mod.GetDeclaration().Name + " has been disabled due to a post init error. Please check the log for details.");
		}
	}

	public static bool IsModLoaded(string uid)
	{
		foreach (IMod loadedMod in WorldBoxMod.LoadedMods)
		{
			if (loadedMod.GetDeclaration().UID == uid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool TryCompileModAtRuntime(ModDeclare pModDeclare, bool pForce = false)
	{
		if (pModDeclare.ModType == ModTypeEnum.BEPINEX)
		{
			ModInfoUtils.LinkBepInExModToLocalRequest(pModDeclare);
			ModInfoUtils.DealWithBepInExModLinkRequests();
			return false;
		}
		ModDependencyNode modDependencyNode = ModDepenSolveService.SolveModDependencyRuntime(pModDeclare);
		if (modDependencyNode == null)
		{
			ErrorWindow.errorMessage = "Failed to load mod " + pModDeclare.Name + ":\nFailed to solve mod dependency.Check Incompatible mods and dependencies, then try again.";
			ScrollWindow.get("error_with_reason").clickShow();
			return false;
		}
		if (!compileMod(modDependencyNode, pForce))
		{
			ErrorWindow.errorMessage = "Failed to load mod " + pModDeclare.Name + ":\nFailed to compile mod.Check Incompatible mods and dependencies, then try again.";
			ScrollWindow.get("error_with_reason").clickShow();
			return false;
		}
		ModInfoUtils.SaveModRecords();
		return true;
	}

	public static bool TryCompileAndLoadModAtRuntime(ModDeclare mod_declare)
	{
		if (IsModLoaded(mod_declare.UID))
		{
			return false;
		}
		if (!TryCompileModAtRuntime(mod_declare))
		{
			return false;
		}
		MasterBuilder masterBuilder = new MasterBuilder();
		ResourcesPatch.LoadResourceFromFolder(Path.Combine(mod_declare.FolderPath, Paths.ModResourceFolderName), out var Builders);
		ResourcesPatch.LoadResourceFromFolder(Path.Combine(mod_declare.FolderPath, Paths.NCMSAdditionModResourceFolderName), out var Builders2);
		LoadMod(mod_declare);
		masterBuilder.AddBuilders(Builders);
		masterBuilder.AddBuilders(Builders2);
		masterBuilder.BuildAll();
		return true;
	}

	public static void loadInfoOfBepInExPlugins()
	{
		List<ModDeclare> list = ModInfoUtils.recogBepInExMods();
		GameObject val = GameObject.Find("BepInEx_Manager");
		foreach (ModDeclare mod in list)
		{
			if (IsModLoaded(mod.UID))
			{
				LogService.LogWarning("Repeat Mod with " + mod.UID + ", Only load one of them");
				continue;
			}
			BepinexMod bepinexMod = new BepinexMod();
			MonoBehaviour pModComponent = null;
			if ((Object)(object)val != (Object)null)
			{
				MonoBehaviour[] components = val.GetComponents<MonoBehaviour>();
				using IEnumerator<MonoBehaviour> enumerator2 = components.Where((MonoBehaviour component) => (((object)component).GetType().FullName ?? "").Contains(mod.Name)).GetEnumerator();
				if (enumerator2.MoveNext())
				{
					MonoBehaviour current = enumerator2.Current;
					pModComponent = current;
				}
			}
			bepinexMod.OnLoad(mod, pModComponent);
			WorldBoxMod.LoadedMods.Add(bepinexMod);
			WorldBoxMod.AllRecognizedMods[mod] = ModState.LOADED;
		}
	}
}
