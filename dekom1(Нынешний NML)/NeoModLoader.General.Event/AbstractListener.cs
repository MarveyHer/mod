using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NeoModLoader.services;

namespace NeoModLoader.General.Event;

public abstract class AbstractListener<TListener, THandler> : BaseListener where TListener : AbstractListener<TListener, THandler> where THandler : AbstractHandler<THandler>
{
	private bool _patched = false;

	protected static TListener instance { get; private set; }

	protected List<THandler> handlers { get; } = new List<THandler>();

	public AbstractListener()
	{
		instance = (TListener)this;
	}

	protected static void InsertCallHandleCode(List<CodeInstruction> codes, int pos)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		codes.Insert(pos, new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TListener), "HandleAll", (Type[])null, (Type[])null)));
	}

	public static void RegisterHandler(THandler handler)
	{
		if (!instance._patched)
		{
			instance._patched = true;
			Type type = instance.GetType();
			try
			{
				Harmony.CreateAndPatchAll(type, type.FullName);
			}
			catch (Exception ex)
			{
				LogService.LogError("Failed to patch listener: " + type.FullName + ", with handler: " + handler.GetType().FullName);
				LogService.LogError(ex.Message);
				LogService.LogError(ex.StackTrace);
				return;
			}
		}
		instance.handlers.Add(handler);
	}
}
