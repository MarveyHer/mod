using System;
using System.Collections.Generic;
using System.Reflection;
using NeoModLoader.services;

namespace NeoModLoader.General.Event;

internal static class ListenerManager
{
	private static readonly string ListenerNamespace = "NeoModLoader.General.Event.Listeners";

	private static readonly HashSet<BaseListener> _listeners = new HashSet<BaseListener>();

	public static void _init()
	{
		Type[] types = Assembly.GetExecutingAssembly().GetTypes();
		Type[] array = types;
		foreach (Type type in array)
		{
			if (type.Namespace != ListenerNamespace)
			{
				continue;
			}
			try
			{
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
				if (constructor == null)
				{
					LogService.LogWarning("Cannot find constructor of " + type.FullName);
				}
				else if (!(constructor.Invoke(null) is BaseListener item))
				{
					LogService.LogWarning("Failed to construct listener instance of " + type.FullName);
				}
				else
				{
					_listeners.Add(item);
				}
			}
			catch (Exception ex)
			{
				LogService.LogError("Failed to patch listener: " + type.FullName);
				LogService.LogError(ex.Message);
				LogService.LogError(ex.StackTrace);
			}
		}
	}
}
