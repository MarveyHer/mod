using System.IO;
using NeoModLoader.api;
using NeoModLoader.constants;
using NeoModLoader.General;
using NeoModLoader.utils;

namespace NeoModLoader.services;

internal static class ModReloadService
{
	public static bool HotfixMethods(IReloadable pMod, ModDeclare pModDeclare)
	{
		if (!ModReloadUtils.Prepare(pMod, pModDeclare))
		{
			return false;
		}
		if (!ModReloadUtils.CompileNew())
		{
			return false;
		}
		if (!ModReloadUtils.PatchHotfixMethodsNT())
		{
			return false;
		}
		return true;
	}

	public static bool ReloadResources(IMod pMod)
	{
		ResourcesPatch.LoadResourceFromFolder(Path.Combine(pMod.GetDeclaration().FolderPath, Paths.ModResourceFolderName));
		ResourcesPatch.LoadResourceFromFolder(Path.Combine(pMod.GetDeclaration().FolderPath, Paths.NCMSAdditionModResourceFolderName));
		return false;
	}

	public static void ReloadLocales(IMod pMod)
	{
		if (!(pMod is ILocalizable localizable))
		{
			return;
		}
		string localeFilesDirectory = localizable.GetLocaleFilesDirectory(pMod.GetDeclaration());
		if (Directory.Exists(localeFilesDirectory))
		{
			string[] files = Directory.GetFiles(localeFilesDirectory);
			string[] array = files;
			foreach (string text in array)
			{
				LogService.LogInfo("Reload " + text + " as " + Path.GetFileNameWithoutExtension(text));
				LM.LoadLocale(Path.GetFileNameWithoutExtension(text), text);
			}
			LM.ApplyLocale();
		}
	}
}
