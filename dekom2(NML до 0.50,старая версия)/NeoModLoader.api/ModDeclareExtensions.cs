using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NeoModLoader.api;

public static class ModDeclareExtensions
{
	public static Version ParseVersion(this ModDeclare pModDeclare)
	{
		try
		{
			Version version = Version.Parse(pModDeclare.Version);
			int major = Math.Max(0, version.Major);
			int minor = Math.Max(0, version.Minor);
			int build = Math.Max(0, version.Build);
			int revision = Math.Max(0, version.Revision);
			return new Version(major, minor, build, revision);
		}
		catch (Exception)
		{
			return new Version(0, 0, 0, 0);
		}
	}

	public static bool TryGetDeclaration(this Assembly pModAssembly, out ModDeclare pModDeclare)
	{
		foreach (ModDeclare mod in WorldBoxMod.AllRecognizedMods.Keys)
		{
			switch (mod.ModType)
			{
			case ModTypeEnum.NEOMOD:
				if (mod.UID == pModAssembly.GetName().Name)
				{
					pModDeclare = mod;
					return true;
				}
				break;
			case ModTypeEnum.COMPILED_NEOMOD:
			{
				IMod modObj = WorldBoxMod.LoadedMods.FirstOrDefault((IMod m) => m.GetDeclaration() == mod);
				if (modObj != null)
				{
					if (pModAssembly == modObj.GetType().Assembly)
					{
						pModDeclare = mod;
						return true;
					}
					if ((from t in pModAssembly.Modules.SelectMany((Module m) => m.GetTypes())
						where t.GetInterfaces().Contains(typeof(IMod))
						select t).Any((Type modClass) => modClass.IsInstanceOfType(modObj)))
					{
						pModDeclare = mod;
						return true;
					}
				}
				else
				{
					if (Directory.GetFiles(mod.FolderPath).Any((string possible_file) => Path.GetFullPath(possible_file) == Path.GetFullPath(pModAssembly.Location)))
					{
						pModDeclare = mod;
						return true;
					}
					if (string.Concat(mod.Name.Where((char c) => new Regex("\\S").IsMatch(c.ToString()))) == pModAssembly.GetName().Name)
					{
						pModDeclare = mod;
						return true;
					}
				}
				break;
			}
			case ModTypeEnum.BEPINEX:
				if (mod.Name == pModAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title)
				{
					pModDeclare = mod;
					return true;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case ModTypeEnum.RESOURCE_PACK:
				break;
			}
		}
		pModDeclare = null;
		return false;
	}
}
