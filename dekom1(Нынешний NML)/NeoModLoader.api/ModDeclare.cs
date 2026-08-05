using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NeoModLoader.utils;
using Newtonsoft.Json;

namespace NeoModLoader.api;

[Serializable]
public class ModDeclare
{
	[JsonProperty("name")]
	public string Name { get; private set; }

	[JsonProperty("GUID")]
	public string UID { get; private set; }

	[JsonProperty("author")]
	public string Author { get; private set; }

	[JsonProperty("version")]
	public string Version { get; private set; }

	[JsonProperty("description")]
	public string Description { get; private set; }

	[JsonProperty("RepoUrl")]
	public string RepoUrl { get; private set; }

	[JsonProperty("Dependencies")]
	public string[] Dependencies { get; private set; }

	[JsonProperty("OptionalDependencies")]
	public string[] OptionalDependencies { get; private set; }

	[JsonProperty("IncompatibleWith")]
	public string[] IncompatibleWith { get; private set; }

	public string FolderPath { get; private set; } = null;

	[JsonProperty("targetGameBuild")]
	public int TargetGameBuild { get; private set; }

	[JsonProperty("iconPath")]
	public string IconPath { get; private set; }

	[JsonProperty("ModType")]
	public ModTypeEnum ModType { get; private set; } = ModTypeEnum.NEOMOD;

	[JsonProperty("UsePublicizedAssembly")]
	public bool UsePublicizedAssembly { get; private set; } = true;

	public bool IsNCMSMod { get; internal set; } = false;

	public StringBuilder FailReason { get; } = new StringBuilder();

	public bool IsWorkshopLoaded { get; internal set; } = false;

	private ModDeclare()
	{
	}

	public ModDeclare(string pName, string pAuthor, string pIconPath, string pVersion, string pDescription, string pFolderPath, string[] pDependencies, string[] pOptionalDependencies, string[] pIncompatibleWith, bool pIsWorkshopLoaded = false)
	{
		Name = pName;
		Author = pAuthor;
		IconPath = pIconPath;
		Version = pVersion;
		Description = pDescription;
		Dependencies = pDependencies ?? Array.Empty<string>();
		OptionalDependencies = pOptionalDependencies ?? Array.Empty<string>();
		IncompatibleWith = pIncompatibleWith ?? Array.Empty<string>();
		IsWorkshopLoaded = pIsWorkshopLoaded;
		UID = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(Author + "." + Name);
		for (int i = 0; i < Dependencies.Length; i++)
		{
			Dependencies[i] = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(Dependencies[i]);
		}
		for (int j = 0; j < OptionalDependencies.Length; j++)
		{
			OptionalDependencies[j] = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(OptionalDependencies[j]);
		}
		for (int k = 0; k < IncompatibleWith.Length; k++)
		{
			IncompatibleWith[k] = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(IncompatibleWith[k]);
		}
		FolderPath = pFolderPath;
	}

	public ModDeclare(string pFilePath)
	{
		ModDeclare modDeclare = JsonConvert.DeserializeObject<ModDeclare>(File.ReadAllText(pFilePath)) ?? throw new InvalidOperationException("Input Mod Config file path cannot be null");
		if (modDeclare == null)
		{
			throw new Exception("Mod Config file at \"" + pFilePath + "\" is invalid");
		}
		Name = modDeclare.Name;
		Author = modDeclare.Author;
		Version = modDeclare.Version;
		IconPath = modDeclare.IconPath;
		Description = modDeclare.Description;
		Dependencies = modDeclare.Dependencies;
		OptionalDependencies = modDeclare.OptionalDependencies;
		IncompatibleWith = modDeclare.IncompatibleWith;
		ModType = modDeclare.ModType;
		UsePublicizedAssembly = modDeclare.UsePublicizedAssembly;
		if (Dependencies == null)
		{
			string[] array = (Dependencies = Array.Empty<string>());
		}
		if (OptionalDependencies == null)
		{
			string[] array = (OptionalDependencies = Array.Empty<string>());
		}
		if (IncompatibleWith == null)
		{
			string[] array = (IncompatibleWith = Array.Empty<string>());
		}
		UID = modDeclare.UID;
		if (string.IsNullOrEmpty(UID))
		{
			UID = Author + "." + Name;
		}
		UID = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(UID);
		for (int i = 0; i < Dependencies.Length; i++)
		{
			Dependencies[i] = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(Dependencies[i]);
		}
		for (int j = 0; j < OptionalDependencies.Length; j++)
		{
			OptionalDependencies[j] = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(OptionalDependencies[j]);
		}
		for (int k = 0; k < IncompatibleWith.Length; k++)
		{
			IncompatibleWith[k] = ModDependencyUtils.ParseDepenNameToPreprocessSymbol(IncompatibleWith[k]);
		}
		FolderPath = Path.GetDirectoryName(pFilePath) ?? throw new Exception("Cannot get folder path from input file path");
		string[] array5 = FolderPath.Split(Path.DirectorySeparatorChar);
		int num = array5.IndexOf("workshop");
		if (num != -1 && num + 3 < array5.Length && !(array5[++num] != "content") && !(array5[++num] != "1206560"))
		{
			Regex regex = new Regex("^\\d+$");
			if (regex.IsMatch(array5[++num]))
			{
				IsWorkshopLoaded = true;
			}
		}
	}

	internal void SetRepoUrlToWorkshopPage(string id)
	{
		RepoUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id=" + id;
	}

	internal void SetModType(ModTypeEnum modType)
	{
		if (modType < ModTypeEnum.NEOMOD || modType > ModTypeEnum.RESOURCE_PACK)
		{
			throw new ArgumentOutOfRangeException("modType", modType, null);
		}
		ModType = modType;
	}

	internal void SetIconPath(string iconPath)
	{
		IconPath = iconPath;
	}
}
