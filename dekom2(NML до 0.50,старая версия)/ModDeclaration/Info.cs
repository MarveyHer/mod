using System;
using NCMS;
using NeoModLoader.constants;
using UnityEngine;

namespace ModDeclaration;

[Obsolete("Compatible Layer will not be maintained and be removed in the future")]
public class Info
{
	public static readonly string DataPath = Application.dataPath;

	public static readonly string ModsPath = DataPath + "/StreamingAssets/Mods";

	public static readonly string NCMSPath = ModsPath + "/NCMS";

	public static readonly string NCMSModsPath = Paths.ModsPath;

	public readonly string Author;

	public readonly string Description;

	public readonly string IconPath;

	public readonly string Name;

	public readonly string Path;

	public readonly string Version;

	internal Info(NCMod mod)
	{
		Name = mod.name;
		Author = mod.author;
		Version = mod.version;
		Description = mod.description;
		IconPath = mod.iconPath;
		Path = mod.path;
	}
}
