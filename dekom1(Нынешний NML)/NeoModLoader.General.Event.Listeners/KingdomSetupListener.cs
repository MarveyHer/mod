using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class KingdomSetupListener : AbstractListener<KingdomSetupListener, KingdomSetupHandler>
{
	protected static void HandleAll(Kingdom pKingdom, bool pCiv)
	{
		StringBuilder stringBuilder = null;
		foreach (KingdomSetupHandler handler in AbstractListener<KingdomSetupListener, KingdomSetupHandler>.instance.handlers)
		{
			if (!handler.enabled)
			{
				continue;
			}
			try
			{
				handler.Handle(pKingdom, pCiv);
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
	[HarmonyPatch(typeof(KingdomManager), "makeNewCivKingdom")]
	private static IEnumerable<CodeInstruction> _setupKingdom_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = 28;
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_2, (object)null));
		AbstractListener<KingdomSetupListener, KingdomSetupHandler>.InsertCallHandleCode(list, pos);
		return list;
	}
}
