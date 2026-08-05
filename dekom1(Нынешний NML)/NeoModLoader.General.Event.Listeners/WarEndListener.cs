using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class WarEndListener : AbstractListener<WarEndListener, WarEndHandler>
{
	protected static void HandleAll(WarManager pWarManager, War pWar)
	{
		StringBuilder stringBuilder = null;
		foreach (WarEndHandler handler in AbstractListener<WarEndListener, WarEndHandler>.instance.handlers)
		{
			if (!handler.enabled)
			{
				continue;
			}
			try
			{
				handler.Handle(pWarManager, pWar);
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
	[HarmonyPatch(typeof(WarManager), "endWar")]
	private static IEnumerable<CodeInstruction> _endWar_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = 14;
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		AbstractListener<WarEndListener, WarEndHandler>.InsertCallHandleCode(list, pos);
		return list;
	}
}
