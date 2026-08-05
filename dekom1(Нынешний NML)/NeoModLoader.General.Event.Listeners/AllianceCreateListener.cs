using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class AllianceCreateListener : AbstractListener<AllianceCreateListener, AllianceCreateHandler>
{
	protected static void HandleAll(Alliance pAlliance, Kingdom pKingdom, Kingdom pKingdom2)
	{
		StringBuilder stringBuilder = null;
		int i = 0;
		int count = AbstractListener<AllianceCreateListener, AllianceCreateHandler>.instance.handlers.Count;
		bool flag = false;
		while (!flag)
		{
			try
			{
				for (; i < count; i++)
				{
					AbstractListener<AllianceCreateListener, AllianceCreateHandler>.instance.handlers[i].Handle(pAlliance, pKingdom, pKingdom2);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				AbstractListener<AllianceCreateListener, AllianceCreateHandler>.instance.handlers[i].HitException();
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.AppendLine("Failed to handle event in " + AbstractListener<AllianceCreateListener, AllianceCreateHandler>.instance.handlers[i].GetType().FullName);
				stringBuilder.AppendLine(ex.Message);
				stringBuilder.AppendLine(ex.StackTrace);
				i++;
			}
		}
		if (stringBuilder != null)
		{
			LogService.LogError(stringBuilder.ToString());
		}
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(AllianceManager), "newAlliance")]
	private static IEnumerable<CodeInstruction> _newAllianceEvent_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = 9;
		list.Insert(pos++, new CodeInstruction(OpCodes.Dup, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_2, (object)null));
		AbstractListener<AllianceCreateListener, AllianceCreateHandler>.InsertCallHandleCode(list, pos);
		return list;
	}
}
