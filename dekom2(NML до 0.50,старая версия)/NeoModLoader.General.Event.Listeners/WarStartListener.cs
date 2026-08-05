using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class WarStartListener : AbstractListener<WarStartListener, WarStartHandler>
{
	protected static void HandleAll(War pWar, Kingdom pAttacker, Kingdom pDefender, WarTypeAsset pWarType)
	{
		StringBuilder stringBuilder = null;
		foreach (WarStartHandler handler in AbstractListener<WarStartListener, WarStartHandler>.instance.handlers)
		{
			if (!handler.enabled)
			{
				continue;
			}
			try
			{
				handler.Handle(pWar, pAttacker, pDefender, pWarType);
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
	[HarmonyPatch(typeof(WarManager), "newWar")]
	private static IEnumerable<CodeInstruction> _newWar_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = list.FindIndex((CodeInstruction c) => c.opcode == OpCodes.Ret);
		list.Insert(pos++, new CodeInstruction(OpCodes.Dup, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_2, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_3, (object)null));
		AbstractListener<WarStartListener, WarStartHandler>.InsertCallHandleCode(list, pos);
		return list;
	}
}
