using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class ModLoader : MonoBehaviour
{
	private bool initialized;

	private static List<string> modsLoaded = new List<string>();

	private const string MODS_FOLDER = "mods";

	public void Update()
	{
		if (Config.game_loaded && Config.experimental_mode && !initialized)
		{
			initialized = true;
			Initialize();
			base.enabled = false;
		}
	}

	internal static List<string> getModsLoaded()
	{
		return modsLoaded;
	}

	public void Initialize()
	{
		string tPath = Path.Combine(Application.streamingAssetsPath, "mods");
		if (!Directory.Exists(tPath))
		{
			Debug.LogError("Can not find mod dlls - there is no 'Mods' folder");
			return;
		}
		using ListPool<FileInfo> tRandomFiles = new ListPool<FileInfo>(new DirectoryInfo(tPath).GetFiles());
		tRandomFiles.RemoveAll((FileInfo file) => !file.Name.ToLower().EndsWith(".dll"));
		HashSet<string> tCheckFilenames = new HashSet<string>();
		foreach (ref FileInfo item2 in tRandomFiles)
		{
			string tFilename = item2.Name.ToLower();
			tCheckFilenames.Add(tFilename);
		}
		tRandomFiles.RemoveAll(delegate(FileInfo fileInfo)
		{
			string text = fileInfo.Name.ToLower();
			if (!text.EndsWith("_memload.dll"))
			{
				return false;
			}
			string item = text.Replace("_memload.dll", ".dll");
			return tCheckFilenames.Contains(item) ? true : false;
		});
		tRandomFiles.Shuffle();
		foreach (ref FileInfo item3 in tRandomFiles)
		{
			FileInfo tFileInfo = item3;
			bool tMemload = false;
			if (!tFileInfo.Name.ToLower().EndsWith(".dll"))
			{
				continue;
			}
			string tModPath = tFileInfo.FullName;
			string directoryName = Path.GetDirectoryName(tModPath);
			string tModName = Path.GetFileNameWithoutExtension(tFileInfo.Name).Replace("_memload", "");
			string tDLLname = tModName;
			string tMemLoadFilename = tModName + "_memload.dll";
			string tMemLoadPath = Path.Combine(directoryName, tMemLoadFilename);
			if (File.Exists(tMemLoadPath))
			{
				tMemload = true;
				tDLLname = Path.GetFileNameWithoutExtension(tMemLoadFilename);
				tModPath = tMemLoadPath;
				Debug.Log("[" + tModName + "] Loading " + tFileInfo.Name + " into memory");
			}
			else
			{
				tMemload = false;
				Debug.Log("[" + tModName + "] Loading " + tFileInfo.Name);
			}
			try
			{
				Assembly tAssembly;
				if (!tMemload)
				{
					tAssembly = Assembly.LoadFile(tModPath);
				}
				else
				{
					byte[] tAssemblyBytes = File.ReadAllBytes(tModPath);
					string tPdbPath = Path.Combine(Path.GetDirectoryName(tModPath), tDLLname + ".pdb");
					if (File.Exists(tPdbPath))
					{
						Debug.Log("[" + tModName + "] .pdb symbol file found");
						try
						{
							byte[] tPdbBytes = File.ReadAllBytes(tPdbPath);
							tAssembly = Assembly.Load(tAssemblyBytes, tPdbBytes);
						}
						catch (Exception ex)
						{
							Debug.LogError("[" + tModName + "] Failed to load with .pdb symbol file");
							Debug.LogError(ex.Message);
							tAssembly = Assembly.Load(tAssemblyBytes);
						}
					}
					else
					{
						tAssembly = Assembly.Load(tAssemblyBytes);
					}
				}
				Debug.Log("[" + tModName + "] Assembly: " + tAssembly);
				Debug.Log("[" + tModName + "] classes inside the mod:");
				Type[] types = tAssembly.GetTypes();
				for (int num = 0; num < types.Length; num++)
				{
					Debug.Log("[" + tModName + "] " + types[num]);
				}
				Debug.Log("[" + tModName + "] Attempting to load " + tModName + ".WorldBoxMod");
				Type tModType = tAssembly.GetType(tModName + ".WorldBoxMod");
				if (tModType != null)
				{
					GameObject obj = new GameObject(tModName);
					obj.transform.parent = base.transform;
					obj.AddComponent(tModType);
					modsLoaded.Add(tModName);
					Config.MODDED = true;
					Debug.Log("[" + tModName + "] Was added");
				}
				else
				{
					Debug.LogError("[" + tModName + "] Missing className: " + tModName + ".WorldBoxMod");
				}
			}
			catch (Exception ex2)
			{
				Debug.Log("[" + tModName + "] Failed to load mod from path : ");
				Debug.Log("[" + tModName + "] " + tModPath);
				Debug.LogError(ex2.Message);
			}
		}
	}
}
