using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.services;

namespace NeoModLoader.General.Event.Listeners;

public class ActorTryToAttackListener : AbstractListener<ActorTryToAttackListener, ActorTryToAttackHandler>
{
	protected static void HandleAll(Actor pAttacker, BaseSimObject pTarget, CombatActionAsset pCombatActionAsset, AttackData pAttackData)
	{
		StringBuilder stringBuilder = null;
		int i = 0;
		int count = AbstractListener<ActorTryToAttackListener, ActorTryToAttackHandler>.instance.handlers.Count;
		bool flag = false;
		while (!flag)
		{
			try
			{
				for (; i < count; i++)
				{
					AbstractListener<ActorTryToAttackListener, ActorTryToAttackHandler>.instance.handlers[i].Handle(pAttacker, pTarget, pCombatActionAsset, pAttackData);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				AbstractListener<ActorTryToAttackListener, ActorTryToAttackHandler>.instance.handlers[i].HitException();
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.AppendLine("Failed to handle event in " + AbstractListener<ActorTryToAttackListener, ActorTryToAttackHandler>.instance.handlers[i].GetType().FullName);
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
	[HarmonyPatch(typeof(Actor), "tryToAttack")]
	private static IEnumerable<CodeInstruction> _tryToAttack_Patch(IEnumerable<CodeInstruction> instr)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instr);
		int pos = list.FindIndex((CodeInstruction x) => x.opcode == OpCodes.Stloc_S && ((LocalBuilder)x.operand).LocalIndex == 7) - 1;
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldloc_S, (object)6));
		list.Insert(pos++, new CodeInstruction(OpCodes.Ldloc_S, (object)4));
		AbstractListener<ActorTryToAttackListener, ActorTryToAttackHandler>.InsertCallHandleCode(list, pos);
		return list;
	}
}
