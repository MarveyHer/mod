using System.IO;
using NeoModLoader.api;
using NeoModLoader.constants;
using NeoModLoader.General;
using NeoModLoader.utils;
using NeoModLoader.utils.Builders;

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
		MasterBuilder masterBuilder = new MasterBuilder();
		ResourcesPatch.LoadResourceFromFolder(Path.Combine(pMod.GetDeclaration().FolderPath, Paths.ModResourceFolderName), out var Builders);
		ResourcesPatch.LoadResourceFromFolder(Path.Combine(pMod.GetDeclaration().FolderPath, Paths.NCMSAdditionModResourceFolderName), out var Builders2);
		masterBuilder.AddBuilders(Builders);
		masterBuilder.AddBuilders(Builders2);
		masterBuilder.BuildAll();
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
