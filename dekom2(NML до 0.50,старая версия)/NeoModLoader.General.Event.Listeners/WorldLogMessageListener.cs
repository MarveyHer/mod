using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;
using UnityEngine;
using UnityEngine.UI;

namespace NeoModLoader.General.Event.Listeners;

public class WorldLogMessageListener : AbstractListener<WorldLogMessageListener, WorldLogMessageHandler>
{
	protected static string HandleAll(ref WorldLogMessage pMessage, string pCurrentText, Color pCurrentColor, Text pTextfield, bool pColorField, bool pColorTags)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = null;
		int i = 0;
		int count = AbstractListener<WorldLogMessageListener, WorldLogMessageHandler>.instance.handlers.Count;
		bool flag = false;
		while (!flag)
		{
			try
			{
				for (; i < count; i++)
				{
					AbstractListener<WorldLogMessageListener, WorldLogMessageHandler>.instance.handlers[i].Handle(ref pMessage, ref pCurrentText, ref pCurrentColor, ref pColorField, pColorTags);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				AbstractListener<WorldLogMessageListener, WorldLogMessageHandler>.instance.handlers[i].HitException();
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.AppendLine("Failed to handle event in " + AbstractListener<WorldLogMessageListener, WorldLogMessageHandler>.instance.handlers[i].GetType().FullName);
				stringBuilder.AppendLine(ex.Message);
				stringBuilder.AppendLine(ex.StackTrace);
				i++;
			}
		}
		if (stringBuilder != null)
		{
			LogService.LogError(stringBuilder.ToString());
		}
		if (pColorField)
		{
			((Graphic)pTextfield).color = pCurrentColor;
		}
		else
		{
			((Graphic)pTextfield).color = Toolbox.color_log_neutral;
		}
		return pCurrentText;
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(WorldLogMessageExtensions), "getFormatedText")]
	private static IEnumerable<CodeInstruction> _WorldLogMessage_getFormatedText_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int index = list.Count - 2;
		list.Insert(index++, new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list.Insert(index++, new CodeInstruction(OpCodes.Ldloc_0, (object)null));
		list.Insert(index++, new CodeInstruction(OpCodes.Ldloc_1, (object)null));
		list.Insert(index++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Insert(index++, new CodeInstruction(OpCodes.Ldarg_2, (object)null));
		list.Insert(index++, new CodeInstruction(OpCodes.Ldarg_3, (object)null));
		list.Insert(index++, new CodeInstruction(OpCodes.Callvirt, (object)AccessTools.Method(typeof(WorldLogMessageListener), "HandleAll", (Type[])null, (Type[])null)));
		list.Insert(index, new CodeInstruction(OpCodes.Stloc_0, (object)null));
		return list;
	}
}
