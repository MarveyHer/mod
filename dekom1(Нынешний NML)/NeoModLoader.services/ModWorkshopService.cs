using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.constants;
using NeoModLoader.General;
using NeoModLoader.utils;
using Newtonsoft.Json;
using RSG;
using UnityEngine;

namespace NeoModLoader.services;

[Experimental]
internal static class ModWorkshopService
{
	internal static Promise steamWorkshopPromise;

	private static IPlatformSpecificModWorkshopService workshopServiceBackend;

	public static void Init()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		steamWorkshopPromise = RF.GetStaticField<Promise, SteamSDK>("steamInitialized");
		if ((int)Application.platform == 2)
		{
			workshopServiceBackend = new ModWorkshopServiceWindows();
		}
		else
		{
			workshopServiceBackend = new ModWorkshopServiceUnix();
		}
	}

	private static void UploadModLoader(string changelog)
	{
		workshopServiceBackend.UploadModLoader(changelog);
	}

	public static Promise UploadMod(IMod mod, string changelog, bool verified = false)
	{
		ModDeclare declaration = mod.GetDeclaration();
		string name = declaration.Name;
		string description = name + " Uploaded by NeoModLoader\n" + name + " 由NeoModLoader上传\n\n" + declaration.Description + "\n\nModLoader: https://github.com/WorldBoxOpenMods/ModLoader\n\n模组加载器: https://github.com/WorldBoxOpenMods/ModLoader";
		string text = Path.Combine(SaveManager.generateMainPath("workshop_upload_mod") + declaration.UID);
		if (Directory.Exists(text))
		{
			Directory.Delete(text, recursive: true);
		}
		if (!Directory.Exists(SaveManager.generateMainPath("workshop_upload_mod")))
		{
			Directory.CreateDirectory(SaveManager.generateMainPath("workshop_upload_mod"));
		}
		Directory.CreateDirectory(text);
		List<string> list = SystemUtils.SearchFileRecursive(declaration.FolderPath, (string filename) => !filename.StartsWith("."), (string dirname) => !dirname.StartsWith(".") && !Paths.IgnoreSearchDirectories.Contains(dirname));
		foreach (string item in list)
		{
			string text2 = Path.Combine(text, item.Replace(declaration.FolderPath, "").Replace("\\", "/").Substring(1));
			if (!Directory.Exists(Path.GetDirectoryName(text2)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text2));
			}
			File.Copy(item, text2);
		}
		string previewImagePath;
		if (string.IsNullOrEmpty(declaration.IconPath))
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NeoModLoader.resources.logo.png");
			using FileStream destination = File.Create(Path.Combine(text, "preview.png"));
			stream.Seek(0L, SeekOrigin.Begin);
			stream.CopyTo(destination);
			previewImagePath = Path.Combine(text, "preview.png");
		}
		else
		{
			previewImagePath = Path.Combine(text, declaration.IconPath);
		}
		if (!File.Exists(Path.Combine(text, "mod.json")))
		{
			File.WriteAllText(Path.Combine(text, "mod.json"), JsonConvert.SerializeObject((object)declaration));
		}
		return workshopServiceBackend.UploadMod(name, description, previewImagePath, text, changelog, verified);
	}

	public static Promise TryEditMod(ulong fileID, IMod mod, string changelog)
	{
		ModDeclare declaration = mod.GetDeclaration();
		string text = Path.Combine(SaveManager.generateMainPath("workshop_upload_mod") + declaration.UID);
		if (Directory.Exists(text))
		{
			Directory.Delete(text, recursive: true);
		}
		if (!Directory.Exists(SaveManager.generateMainPath("workshop_upload_mod")))
		{
			Directory.CreateDirectory(SaveManager.generateMainPath("workshop_upload_mod"));
		}
		Directory.CreateDirectory(text);
		List<string> list = SystemUtils.SearchFileRecursive(declaration.FolderPath, (string filename) => !filename.StartsWith("."), (string dirname) => !dirname.StartsWith(".") && !Paths.IgnoreSearchDirectories.Contains(dirname));
		foreach (string item in list)
		{
			string text2 = Path.Combine(text, item.Replace(declaration.FolderPath, "").Replace("\\", "/").Substring(1));
			if (!Directory.Exists(Path.GetDirectoryName(text2)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text2));
			}
			File.Copy(item, text2);
		}
		string previewImagePath;
		if (string.IsNullOrEmpty(declaration.IconPath))
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NeoModLoader.resources.logo.png");
			using FileStream destination = File.Create(Path.Combine(text, "preview.png"));
			stream.Seek(0L, SeekOrigin.Begin);
			stream.CopyTo(destination);
			previewImagePath = Path.Combine(text, "preview.png");
		}
		else
		{
			previewImagePath = Path.Combine(text, declaration.IconPath);
		}
		if (!File.Exists(Path.Combine(text, "mod.json")))
		{
			File.WriteAllText(Path.Combine(text, "mod.json"), JsonConvert.SerializeObject((object)declaration));
		}
		return workshopServiceBackend.EditMod(fileID, previewImagePath, text, changelog);
	}

	public static void FindSubscribedMods()
	{
		workshopServiceBackend.FindSubscribedMods();
	}

	public static ModDeclare GetNextModFromWorkshopItem()
	{
		return workshopServiceBackend.GetNextModFromWorkshopItem();
	}
}
