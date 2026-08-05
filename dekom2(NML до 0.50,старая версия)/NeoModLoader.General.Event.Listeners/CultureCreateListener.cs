using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class CultureCreateListener : AbstractListener<CultureCreateListener, CultureCreateHandler>
{
	protected static void HandleAll(Culture pCulture, Actor pActor, City pCity)
	{
		StringBuilder stringBuilder = null;
		foreach (CultureCreateHandler handler in AbstractListener<CultureCreateListener, CultureCreateHandler>.instance.handlers)
		{
			if (!handler.enabled)
			{
				continue;
			}
			try
			{
				handler.Handle(pCulture, pActor, pCity);
			}
			catch (Exception ex)
			{
				handler.HitException();
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.AppendLine("Failed to handle event in " + handler.GetType().FullName);
				stringBuilder.AppendLine(ex.Message);
				stringBuilder.AppendLine(ex.StackTrace);
			}
		}
		if (stringBuilder != null)
		{
			LogService.LogError(stringBuilder.ToString());
		}
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(Culture), "createCulture")]
	private static IEnumerable<CodeInstruction> _createCulture_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = 42;
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_2, (object)null));
		AbstractListener<CultureCreateListener, CultureCreateHandler>.InsertCallHandleCode(list, pos);
		return list;
	}
}
