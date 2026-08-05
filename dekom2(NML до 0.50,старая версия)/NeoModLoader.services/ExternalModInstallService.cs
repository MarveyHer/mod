using System;
using System.Collections.Generic;
using System.Linq;
using NeoModLoader.utils.installers;

namespace NeoModLoader.services;

internal static class ExternalModInstallService
{
	public static async void CheckExternalModInstall()
	{
		List<string> args = new List<string>(Environment.GetCommandLineArgs());
		args.RemoveAt(0);
		foreach (string arg in args)
		{
			LogService.LogInfo(arg);
		}
		Type[] types = WorldBoxMod.NeoModLoaderAssembly.GetTypes();
		List<ACmdModInstaller> cmd_installers = (from type in types
			where type.IsSubclassOf(typeof(ACmdModInstaller)) && !type.IsAbstract
			select (ACmdModInstaller)Activator.CreateInstance(type)).ToList();
		foreach (ACmdModInstaller installer in cmd_installers)
		{
			for (int i = 0; i < args.Count; i++)
			{
				if (await installer.CheckInstall(args[i]))
				{
					args.RemoveAt(i--);
				}
			}
		}
	}
}
