using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AssetModLoader
{
	private static string path_log;

	public static void load()
	{
		path_log = Application.streamingAssetsPath + "/mod_loading_logs.log";
		File.WriteAllText(path_log, "");
		string mainPath = Application.streamingAssetsPath + "/mods/";
		List<string> mainDirs = getDirectories(mainPath);
		log("# HELLO");
		log("# GOTTA LOAD MODS FAST");
		log("# LOADING MODS NOW");
		log("########");
		log("");
		log("# MAIN PATH: " + mainPath);
		log("# TOTAL MODS: " + mainDirs.Count);
		log("");
		for (int i = 0; i < mainDirs.Count; i++)
		{
			string text = mainDirs[i];
			log("---------START------------------------------------------------------------------------------------");
			log("## LOADING MOD N " + (i + 1));
			log(text);
			loadMod(text);
			log("---------FINISH-----------------------------------------------------------------------------------");
			log("");
			log("");
		}
	}

	private static void loadMod(string pPath)
	{
		string tFolder = pPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[^1];
		log("# CHECKING MOD... " + tFolder);
		foreach (string directory in getDirectories(pPath))
		{
			checkModAssets(directory);
		}
	}

	private static void checkModAssets(string pPath)
	{
		List<string> tDirs = getDirectories(pPath);
		string[] array = pPath.Split(Path.DirectorySeparatorChar);
		log("");
		string tFolder = array[^1];
		log("## CHECKING MOD FOLDER... " + tFolder);
		log("## SUB FOLDERS FOUND: " + tDirs.Count);
		log("");
		foreach (string item in tDirs)
		{
			checkModFolder(item, tFolder);
		}
	}

	private static void checkModFolder(string pPath, string pType)
	{
		List<string> tFiles = getFiles(pPath);
		string[] tSplit = pPath.Split(Path.DirectorySeparatorChar);
		log("");
		log("# CHECKING PATH... " + tSplit[^1]);
		log("FILES: " + tFiles.Count);
		log("");
		foreach (string tPath in tFiles)
		{
			log(tPath);
			if (tPath.Contains("json"))
			{
				loadFileJson(tPath, pType);
			}
			if (tPath.Contains("png"))
			{
				loadTexture(tPath);
			}
		}
	}

	private static void loadTexture(string pPath)
	{
		string tFile = pPath.Split(Path.DirectorySeparatorChar)[^1];
		log("# LOAD TEXTURE: " + tFile);
		byte[] tPNGBytes = File.ReadAllBytes(pPath);
		string tTextureID = "@wb_" + tFile;
		log("ADDING TEXTURE... " + tTextureID);
		SpriteTextureLoader.addSprite(tTextureID, tPNGBytes);
	}

	private static void loadFileJson(string pPath, string pType)
	{
		string tFile = pPath.Split(Path.DirectorySeparatorChar)[^1];
		log("# LOAD ASSET: " + tFile);
		string tStringData = File.ReadAllText(pPath);
		switch (pType)
		{
		default:
			_ = pType == "traits";
			break;
		case "buildings":
			loadAssetBuilding(tStringData);
			break;
		case "powers":
			loadAssetPowers(tStringData);
			break;
		case "kingdoms":
			break;
		}
	}

	private static void loadAssetActor(string pData)
	{
		ActorAsset tAsset = JsonUtility.FromJson<ActorAsset>(pData);
		AssetManager.actor_library.add(tAsset);
	}

	private static void loadAssetBuilding(string pData)
	{
		BuildingAsset tAsset = JsonUtility.FromJson<BuildingAsset>(pData);
		AssetManager.buildings.add(tAsset);
	}

	private static void loadAssetKingdom(string pData)
	{
		KingdomAsset tAsset = JsonUtility.FromJson<KingdomAsset>(pData);
		AssetManager.kingdoms.add(tAsset);
	}

	private static void loadAssetPowers(string pData)
	{
		GodPower tAsset = JsonUtility.FromJson<GodPower>(pData);
		AssetManager.powers.add(tAsset);
	}

	private static void loadAssetTraits(string pData)
	{
		ActorTrait tAsset = JsonUtility.FromJson<ActorTrait>(pData);
		AssetManager.traits.add(tAsset);
	}

	private static void log(string pLog)
	{
		File.AppendAllText(path_log, pLog + "\n");
	}

	private static List<string> getDirectories(string pPath)
	{
		List<string> tList = new List<string>();
		string[] tArr = Directory.GetDirectories(pPath);
		foreach (string tStr in tArr)
		{
			if (!tStr.Contains(".meta"))
			{
				tList.Add(tStr);
			}
		}
		return tList;
	}

	private static List<string> getFiles(string pPath)
	{
		List<string> tList = new List<string>();
		string[] tArr = Directory.GetFiles(pPath);
		foreach (string tStr in tArr)
		{
			if (!tStr.Contains(".meta"))
			{
				tList.Add(tStr);
			}
		}
		return tList;
	}
}
